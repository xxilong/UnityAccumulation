#pragma once
#include <string>
#include <map>
#include "IVideoProvider.h"

class VideoManager
{
public:
	VideoManager();
	virtual ~VideoManager();

	IVideoProvider* GetProvider(const std::wstring& id);

private:
	IVideoProvider* CreateProvider(const std::wstring& id);
	IVideoProvider* CreateProviderByType(const std::wstring& type, const std::wstring& args);

private:
	std::map<std::wstring, IVideoProvider*> m_providers;

public:
	static VideoManager Instance;
};

