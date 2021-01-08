#include "../../stdafx.h"
#include "TestVideo.h"

TestVideo::TestVideo()
{
	m_buff = new unsigned char[m_width * m_height * 4];
	memset(m_buff, 255, m_width * m_height * 4);

	m_aniline = 0;
}

TestVideo::~TestVideo()
{
	delete[] m_buff;
}

unsigned char* TestVideo::GetFrame(int* width, int* height)
{
	*width = m_width;
	*height = m_height;
	
	if (m_aniline >= m_height)
	{
		m_aniline = 0;
		memset(m_buff, 255, m_width * m_height * 4);
	}
	DrawLine();

	++m_aniline;


	return m_buff;
}

void TestVideo::DrawLine()
{
	struct BGRA {
		unsigned char b;
		unsigned char g;
		unsigned char r;
		unsigned char a;
	};

	BGRA* pixel = (BGRA*)m_buff;
	BGRA* begin = pixel + m_width * m_aniline;

	for (int i = 0; i < m_width; ++i)
	{
		begin[i].g = 0;
		begin[i].b = 0;
	}
}
