// USBCameraList.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include "pch.h"
#include <iostream>
#include <mfapi.h>
#include <mfidl.h>
#include <stdio.h>
#include <conio.h>

template <typename T>
void SafeRelease(T*&p)
{
	if (p != nullptr)
	{
		p->Release();
	}

	p = nullptr;
}

void ShowDeviceList()
{
	IMFMediaSource *pSource = NULL;
	IMFAttributes *pAttributes = NULL;
	IMFActivate **ppDevices = NULL;
	HRESULT hr = MFCreateAttributes(&pAttributes, 1);

	wchar_t bufName[1024];
	wchar_t bufSerial[1024];
	UINT32 count = 0;
	UINT32 len = 0;

	for (;;)
	{
		if (FAILED(hr)){ break;}
		hr = pAttributes->SetGUID(MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
		if (FAILED(hr)) { break; }
		
		hr = MFEnumDeviceSources(pAttributes, &ppDevices, &count);
		if (FAILED(hr)) { break; }

		for (unsigned int i = 0; i < count; ++i)
		{
			if (i != 0)
			{
				printf("\n");
			}

			hr = ppDevices[i]->GetString(MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, bufName, 1024, &len);
			bufName[len] = 0;
			hr = ppDevices[i]->GetString(MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK, bufSerial, 1024, &len);
			bufSerial[len] = 0;
			wprintf(L"Name: %s\nID: %s\n", bufName, bufSerial);
		}

		break;
	}
	
	SafeRelease(pAttributes);
	for (UINT32 i = 0; i < count; ++i)
	{
		SafeRelease(ppDevices[i]);
	}

	CoTaskMemFree(ppDevices);
	SafeRelease(pSource);
}

int main()
{
	CoInitialize(NULL);
	ShowDeviceList();

	while (1)
	{
		printf("----------------------------------------------------------------------------------------------------------------------\n");
		printf("按任意键刷新, ESC退出");
		int ch = _getch();
		if (ch == VK_ESCAPE)
		{
			break;
		}
		printf("\r                                                                                                                   \r");
		ShowDeviceList();
	}

	CoUninitialize();
	return 0;
}