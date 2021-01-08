#include "../../stdafx.h"
#include "VideoFromShareMemory.h"
#include "ShareMemory.h"

static constexpr int MAXBUFFSIZE = 1920 * 1080 * 4;

struct ShareMemoryVideo
{
	volatile /* atomic */ long status;	// 0 无视频, 1 视频已准备好  2 数据操作中
	volatile int width;
	volatile int height;
	unsigned char buff[MAXBUFFSIZE];
};

VideoFromShareMemory::VideoFromShareMemory(const std::wstring& sharename)
{
	m_share = (ShareMemoryVideo*)alloc_share_memory(sharename.c_str(), sizeof(ShareMemoryVideo));
	m_copy = new ShareMemoryVideo;
}

VideoFromShareMemory::~VideoFromShareMemory()
{
	free_share_memory(m_share);
	delete m_copy;
}

unsigned char* VideoFromShareMemory::GetFrame(int* width, int* height)
{
	long st = InterlockedCompareExchange(&(m_share->status), 2, 1);
	if (st != 1)
	{
		return nullptr;
	}

	memcpy_s(m_copy, sizeof(ShareMemoryVideo), m_share, sizeof(ShareMemoryVideo));
	InterlockedExchange(&(m_share->status), 0);

	*width = m_copy->width;
	*height = m_copy->height;

	return m_copy->buff;
}
