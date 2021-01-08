#pragma once
#include "FPSFilter.h"

class TestVideo : public FPSFilter<40>
{
public:
	TestVideo();
	virtual ~TestVideo();

	virtual unsigned char* GetFrame(int* width, int* height) override;

private:
	void DrawLine();

private:
	const int m_width = 1920;
	const int m_height = 1080;
	unsigned char* m_buff;

	int m_aniline;
};

