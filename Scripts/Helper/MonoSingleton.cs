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
                //先在场景中查找一下是否有当前类的对象
                var array = FindObjectsOfType(type);
                if (array != null && array.Length > 1) //如果多于一个，销毁到只剩一个
                {
                    m_Instance = array[0] as T;
                    for (int i = 1; i < array.Length; i++)
                        Destroy(array[i]);
                }
                //如果找不到
                if (m_Instance == null)
                {
                    //创建一个GameObject， 以添加组件的方式创建对象 //从GameObject上取得该组件 
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