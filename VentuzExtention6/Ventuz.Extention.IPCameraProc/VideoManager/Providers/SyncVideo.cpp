#include "../../stdafx.h"
#include "SyncVideo.h"

SyncVideo::SyncVideo()
	: m_imgWidth(0), m_imgHeight(0),
	m_imgData1(nullptr)
{
}

SyncVideo::~SyncVideo()
{
	delete[] m_imgData1;
}

unsigned char* SyncVideo::GetFrame(int* width, int* height)
{
	FetchFrame((BGRA*)m_imgData1, m_imgWidth, m_imgHeight);
	*width = m_imgWidth;
	*height = m_imgHeight;
	return m_imgData1;
}

void SyncVideo::SetSize(int width, int height)
{
	if (m_imgWidth == width && m_imgHeight == height)
	{
		return;
	}

	m_imgWidth = width;
	m_imgHeight = height;

	delete[] m_imgData1;

	m_imgData1 = new unsigned char[m_imgWidth * m_imgHeight * 4];
}
