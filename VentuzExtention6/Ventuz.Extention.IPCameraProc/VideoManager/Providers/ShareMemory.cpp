#include "../../stdafx.h"
#include "ShareMemory.h"
#include <windows.h>
#include <map>

struct ShareMemoryInnterHandle
{
	HANDLE m_hFileMapHandle;
};

static std::map<void*, ShareMemoryInnterHandle>* g_pAllocedMemoryList = nullptr;

void* alloc_share_memory(const wchar_t* name, int size)
{
	static std::map<void*, ShareMemoryInnterHandle> alloc_memory_list;
	g_pAllocedMemoryList = &alloc_memory_list;

	ShareMemoryInnterHandle handles;

	bool firstopen = false;

	handles.m_hFileMapHandle = OpenFileMappingW(FILE_MAP_ALL_ACCESS, FALSE, name);
	if (handles.m_hFileMapHandle == nullptr)
	{
		handles.m_hFileMapHandle = CreateFileMappingW(INVALID_HANDLE_VALUE, 
			NULL, PAGE_READWRITE, 0, size, name);

		if (handles.m_hFileMapHandle == nullptr)
			return nullptr;

		firstopen = true;
	}

	void* pMemory = MapViewOfFile(handles.m_hFileMapHandle, FILE_MAP_ALL_ACCESS, 0, 0, size);

	if (pMemory == nullptr)
	{
		CloseHandle(handles.m_hFileMapHandle);
		return nullptr;
	}

	if (firstopen)
	{
		memset(pMemory, 0, size);
	}


	(*g_pAllocedMemoryList)[pMemory] = handles;
	return pMemory;
}

void* open_share_memory(const wchar_t* name, int size)
{
	static std::map<void*, ShareMemoryInnterHandle> alloc_memory_list;
	g_pAllocedMemoryList = &alloc_memory_list;

	ShareMemoryInnterHandle handles;

	handles.m_hFileMapHandle = OpenFileMappingW(FILE_MAP_ALL_ACCESS, FALSE, name);
	if (handles.m_hFileMapHandle == nullptr)
	{
		return nullptr;
	}

	void* pMemory = MapViewOfFile(handles.m_hFileMapHandle, FILE_MAP_ALL_ACCESS, 0, 0, size);

	if (pMemory == nullptr)
	{
		CloseHandle(handles.m_hFileMapHandle);
		return nullptr;
	}

	(*g_pAllocedMemoryList)[pMemory] = handles;
	return pMemory;
}

bool free_share_memory(void* pMemory)
{
	auto iter = g_pAllocedMemoryList->find(pMemory);
	if (iter == g_pAllocedMemoryList->end())
		return false;

	UnmapViewOfFile(pMemory);
	CloseHandle(iter->second.m_hFileMapHandle);
	g_pAllocedMemoryList->erase(pMemory);
	return true;
}

void* alloc_process_memory(const wchar_t* name, int size)
{
	wchar_t realname[1024];
	swprintf_s(realname, L"[%d]%s", GetCurrentProcessId(), name);
	return alloc_share_memory(realname, size);
}

