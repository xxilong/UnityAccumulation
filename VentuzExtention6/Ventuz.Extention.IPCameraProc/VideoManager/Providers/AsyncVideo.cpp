#include "../../stdafx.h"
#include "AsyncVideo.h"

AsyncVideo::AsyncVideo()
	: m_imgWidth(0), m_imgHeight(0),
	m_imgData1(nullptr), m_imgData2(nullptr),
	m_readyBuffer(nullptr), m_freeBuffer(nullptr)
{
	std::thread([this]() { VideoFetchThread(); }).detach();
}

AsyncVideo::~AsyncVideo()
{
	delete[] m_imgData1;
	delete[] m_imgData2;
}

unsigned char* AsyncVideo::GetFrame(int* width, int* height)
{
	unsigned char* ready = m_readyBuffer.exchange(nullptr);
	unsigned char* expect = nullptr;

	if (ready == nullptr)
	{
		m_freeBuffer.compare_exchange_strong(expect, m_imgData1);	// 为空则设置为 data1, 否则不变
	}
	else
	{
		unsigned char* tofetch = m_imgData1;

		if (ready == m_imgData1)
		{
			tofetch = m_imgData2;
		}

		m_freeBuffer.compare_exchange_strong(expect, tofetch);
	}

	m_taskcv.notify_one();

	if (ready != nullptr)
	{
		*width = m_imgWidth;
		*height = m_imgHeight;
	}

	return ready;
}

void AsyncVideo::SetSize(int width, int height)
{
	if (m_imgWidth == width && m_imgHeight == height)
	{
		return;
	}

	m_imgWidth = width;
	m_imgHeight = height;

	delete[] m_imgData1;
	delete[] m_imgData2;

	m_imgData1 = new unsigned char[m_imgWidth * m_imgHeight * 4];
	m_imgData2 = new unsigned char[m_imgWidth * m_imgHeight * 4];

	m_readyBuffer.store(nullptr);
	m_freeBuffer.store(m_imgData1);

	m_taskcv.notify_one();
}

void AsyncVideo::VideoFetchThread()
{
	while (1)
	{
		if (1)
		{
			std::unique_lock<std::mutex> locker(m_lock);
			m_taskcv.wait(locker, [this] { 
				return m_freeBuffer != nullptr && m_readyBuffer == nullptr; 
			});

			if (m_freeBuffer.load() == nullptr)
			{
				continue;
			}
		}

		unsigned char* freeBuffer = m_freeBuffer.load();
		if (freeBuffer != nullptr)
		{
			FetchFrame((BGRA*)freeBuffer, m_imgWidth, m_imgHeight);
			m_freeBuffer.store(nullptr);
			m_readyBuffer.store(freeBuffer);
		}
	}
}
