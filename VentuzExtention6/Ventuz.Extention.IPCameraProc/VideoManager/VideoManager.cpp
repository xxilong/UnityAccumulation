#include "../stdafx.h"
#include "VideoManager.h"
#include "Providers/TestVideo.h"
#include "Providers/GraspDesktop.h"
#include "Providers/VideoFromShareMemory.h"
#include <algorithm>
#include <cwctype>

VideoManager VideoManager::Instance;

VideoManager::VideoManager()
{
}

VideoManager::~VideoManager()
{
	for (auto& item : m_providers)
	{
		delete item.second;
	}
}

IVideoProvider* VideoManager::GetProvider(const std::wstring& id)
{
	auto iter = m_providers.find(id);

	if (iter == m_providers.end())
	{
		IVideoProvider* p = CreateProvider(id);
		m_providers[id] = p;
		return p;
	}

	return iter->second;
}

IVideoProvider* VideoManager::CreateProvider(const std::wstring& id)
{
	std::wstring vtype = id;
	std::wstring args = L"";
	std::wstring::size_type pos = id.find(L':');
	
	if (pos != std::wstring::npos)
	{
		vtype = id.substr(0, pos);
		args = id.substr(pos + 1);
	}

	std::transform(vtype.begin(), vtype.end(), vtype.begin(), std::towlower);
	return CreateProviderByType(vtype, args);
}

IVideoProvider* VideoManager::CreateProviderByType(const std::wstring& type, const std::wstring& args)
{
	if (type == L"test")
	{
		return new TestVideo;
	}
	else if (type == L"desktop")
	{
		return new GraspDesktop(args);
	}
	else if (type == L"shmm")
	{
		return new VideoFromShareMemory(args);
	}
	else
	{
		return new TestVideo;
	}
}
