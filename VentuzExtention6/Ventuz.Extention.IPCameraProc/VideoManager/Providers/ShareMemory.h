#pragma once

void* alloc_share_memory(const wchar_t* name, int size);
void* open_share_memory(const wchar_t* name, int size);
void* alloc_process_memory(const wchar_t* name, int size);  // 在名字前面加上当前进程的 ID 让不同进程之间不会共享
bool  free_share_memory(void* pMemory);