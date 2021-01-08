// Ventuz.Extention.IPCameraProc.cpp: 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "VideoManager/VideoManager.h"
#include "IPCameraProc.h"

void Output(const char *log);
int count = 1;

extern "C" __declspec(dllexport) unsigned char* 
__stdcall GetNativeTexture(const wchar_t* videoId, int* width, int* height)
{
	IVideoProvider* vp = VideoManager::Instance.GetProvider(videoId);

	if (vp != nullptr)
	{
		Output("GetNativeTexture");

		return vp->GetFrameTexture(width, height);
	}
	Output("No Data");
	return nullptr;
}

void Output(const char *log)
{
	FILE *fp;
	fopen_s(&fp,"d:\\out.txt", "wt");
	fprintf(fp, "%d\t", count++);

	fprintf(fp, " ");
	fprintf(fp, "%s", log);
	fprintf(fp, "\n");
	fclose(fp);
}
