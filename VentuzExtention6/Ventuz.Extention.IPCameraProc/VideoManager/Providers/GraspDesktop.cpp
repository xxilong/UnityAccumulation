#include "../../stdafx.h"
#include "GraspDesktop.h"
#include <windows.h>

GraspDesktop::GraspDesktop(const std::wstring& args)
{
	RECT rect;
	m_hWnd = GetDesktopWindow();
	GetWindowRect(m_hWnd, &rect);
	int width = rect.right - rect.left;
	int height = rect.bottom - rect.top;

	m_hDC = GetDC(m_hWnd);
	m_hCompDC = CreateCompatibleDC(m_hDC);

	m_hBitmap = CreateCompatibleBitmap(m_hDC, width, height);
	SelectObject(m_hCompDC, m_hBitmap);

	memset(&m_BitInfo.bmiHeader, 0, sizeof(BITMAPINFOHEADER));
	m_BitInfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	m_BitInfo.bmiHeader.biWidth = width;
	m_BitInfo.bmiHeader.biHeight = -height;
	m_BitInfo.bmiHeader.biPlanes = 1;
	m_BitInfo.bmiHeader.biBitCount = 32; // 这里建议用32，经过测试在我的机器上比24速度快比较多 
	m_BitInfo.bmiHeader.biCompression = BI_RGB;

	SetSize(width, height);
}

GraspDesktop::~GraspDesktop()
{
	DeleteObject(m_hBitmap);
	DeleteDC(m_hCompDC);
	ReleaseDC(m_hWnd, m_hDC);
}

void GraspDesktop::FetchFrame(BGRA* image, int width, int height)
{
	BitBlt(m_hCompDC, 0, 0, width, height, m_hDC, 0, 0, SRCCOPY);										  
	GetDIBits(m_hCompDC, m_hBitmap, 0, height, image, &m_BitInfo, DIB_RGB_COLORS);
}
