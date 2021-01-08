#pragma once
#include "../IVideoProvider.h"

template <int fpsMSTime>
class FPSFilter : public IVideoProvider
{
public:
	FPSFilter() : lastGet(0) {};
	virtual ~FPSFilter() {};

	virtual unsigned char* GetFrameTexture(int* width, int* height) override final
	{
		unsigned long long now = GetTickCount64();
		if (now - lastGet < fpsMSTime)
		{
			return nullptr;
		}

		unsigned char* res = GetFrame(width, height);
		if (res != nullptr)
		{
			lastGet = now;
		}

		return res;
	}

	virtual unsigned char* GetFrame(int* width, int* height) = 0;

private:
	unsigned long long lastGet;
};
