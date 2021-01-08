#pragma once
#include "FPSFilter.h"
#include <atomic>
#include <thread>
#include <mutex>
#include <condition_variable>

class AsyncVideo : public FPSFilter<40>
{
public:
	AsyncVideo();

	struct BGRA {
		unsigned char b;
		unsigned char g;
		unsigned char r;
		unsigned char a;
	};

	virtual ~AsyncVideo();
	virtual unsigned char* GetFrame(int* width, int* height) override final;
	virtual void FetchFrame(BGRA* image, int width, int height) = 0;

protected:
	void SetSize(int width, int height);

private:
	void VideoFetchThread();

protected:
	int m_imgWidth;
	int m_imgHeight;
	unsigned char* m_imgData1;
	unsigned char* m_imgData2;

private:
	std::atomic<unsigned char*> m_readyBuffer;
	std::atomic<unsigned char*> m_freeBuffer;

	std::mutex m_lock;
	std::condition_variable m_taskcv;
};
