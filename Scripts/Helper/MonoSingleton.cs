/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
     where T : MonoSingleton<T>
{

    private static T m_Instance;

    public static T instance
    {
        get
        {
            if (m_Instance == null)
            {
                var type = typeof(T);
                //���ڳ����в���һ���Ƿ��е�ǰ��Ķ���
                var array = FindObjectsOfType(type);
                if (array != null && array.Length > 1) //�������һ�������ٵ�ֻʣһ��
                {
                    m_Instance = array[0] as T;
                    for (int i = 1; i < array.Length; i++)
                        Destroy(array[i]);
                }
                //����Ҳ���
                if (m_Instance == null)
                {
                    //����һ��GameObject�� ���������ķ�ʽ�������� //��GameObject��ȡ�ø���� 
                    m_Instance = new GameObject("Singleton of " + type.Name,
                        type).GetComponent<T>();
                    m_Instance.Init();
                }
            }
            return m_Instance;
        }
    }

    private void Awake()
    {
        m_Instance = this as T;
        Init();
        //if (m_Instance == null)
        //{
        //    m_Instance = this as T;
        //}
    }

    public virtual void Init() { }

    private void OnApplicationQuit()
    {
        m_Instance = null;
    }
}