#pragma once
#include "FPSFilter.h"
#include <string>

struct ShareMemoryVideo;

class VideoFromShareMemory : public FPSFilter<40>
{
public:
	VideoFromShareMemory(const std::wstring& sharename);
	virtual ~VideoFromShareMemory();

	virtual unsigned char* GetFrame(int* width, int* height) override final;

private:
	ShareMemoryVideo* m_share;
	ShareMemoryVideo* m_copy;
};

