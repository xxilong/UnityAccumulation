#pragma once

void* alloc_share_memory(const wchar_t* name, int size);
void* open_share_memory(const wchar_t* name, int size);
void* alloc_process_memory(const wchar_t* name, int size);  // ������ǰ����ϵ�ǰ���̵� ID �ò�ͬ����֮�䲻�Ṳ��
bool  free_share_memory(void* pMemory);