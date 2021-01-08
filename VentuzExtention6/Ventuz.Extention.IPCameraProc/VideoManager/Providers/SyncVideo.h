#pragma once
#include "FPSFilter.h"
#include <atomic>
#include <thread>
#include <mutex>
#include <condition_variable>

class SyncVideo : public FPSFilter<40>
{
public:
	SyncVideo();

	struct BGRA {
		unsigned char b;
		unsigned char g;
		unsigned char r;
		unsigned char a;
	};

	virtual ~SyncVideo();
	virtual unsigned char* GetFrame(int* width, int* height) override final;
	virtual void FetchFrame(BGRA* image, int width, int height) = 0;

protected:
	void SetSize(int width, int height);

protected:
	int m_imgWidth;
	int m_imgHeight;
	unsigned char* m_imgData1;
};
