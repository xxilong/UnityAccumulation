#pragma once
#include "AsyncVideo.h"
#include "SyncVideo.h"
#include <string>

class GraspDesktop : public SyncVideo
{
public:
	GraspDesktop(const std::wstring& args);
	virtual ~GraspDesktop();

	virtual void FetchFrame(BGRA* image, int width, int height) override;

private:
	HWND m_hWnd;
	HDC  m_hDC;
	HDC  m_hCompDC;
	HBITMAP  m_hBitmap;
	BITMAPINFO  m_BitInfo;
};

