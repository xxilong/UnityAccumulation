#pragma once

class IVideoProvider
{
public:
	IVideoProvider() {};
	virtual ~IVideoProvider() {};

	virtual unsigned char* GetFrameTexture(int* width, int* height) = 0;
};
