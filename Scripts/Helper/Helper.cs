using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public static class Helper 
{
    #region TransformHelper
    /// <summary>
    /// 在层级未知情况下，查找子物体变换组件
    /// </summary>
    /// <param name="parentTF">父物体变换组件</param>
    /// <param name="childName">子物体名称</param>
    /// <returns>子物体变换组件</returns>
    public static Transform GetChild(this Transform parentTF, string childName)
    {//递归算法：在方法内部，调用自身的过程。
     //               将大问题缩小化，再转移给子问题。
     // 5！  5*4*3*2*1     
        Transform childTF = parentTF.Find(childName);
        //如果找到，则返回
        if (childTF != null) return childTF;
        //将问题转移给子物体
        int count = parentTF.childCount;
        for (int i = 0; i < count; i++)
        {
            //调用自身方法
            childTF = GetChild(parentTF.GetChild(i), childName);
            if (childTF != null) return childTF;
        }
        return null;
    }

    /// <summary>
    /// 获取所有层级的子物体
    /// </summary>
    /// <param name="parentTF"></param>
    /// <returns></returns>
    public static List<Transform> GetAllChildren(this Transform parentTF,bool includeSelf=false)
    {
        List<Transform> ChildrenList = new List<Transform>(parentTF.GetComponentsInChildren<Transform>());
        if (!includeSelf)
        {
        ChildrenList.Remove(parentTF);
        }
        return ChildrenList;
    }
    /// <summary>
    /// 获取所有第一层子物体的Transform
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static List<Transform> GetAllFirstChild(this Transform parent)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            list.Add(parent.GetChild(i));
        }
        return list;
    }

    /// <summary>
    /// 销毁所有子物体
    /// </summary>
    /// <param name="parentTF"></param>
    public static void DestroyAllChileren(this Transform parentTF)
    {
        for (int i = 0; i < parentTF.childCount; i++)
        {
            GameObject.Destroy(parentTF.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 随机排列子物体
    /// </summary>
    /// <param name="parentTF">父物体</param>
    public static void SortChildByRandom(this Transform parentTF)
    {
        List<Transform> childList = parentTF.GetAllFirstChild();
        List<int> list = new List<int>();
        for (int i = 0; i < childList.Count; i++)
        {
            list.Add(i);
        }
        list = list.OrderBy(p => UnityEngine.Random.Range(0, childList.Count)).ToList();

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].SetSiblingIndex(list[i]);
        }

    }

    /// <summary>
    /// 开关目标物体的碰撞器
    /// </summary>
    /// <param name="tf">目标物体Transform</param>
    /// <param name="enabled">开/关</param>
    public static void SetCollidersEnable(this Transform tf, bool enabled)
    {
        Collider[] colliders = tf.GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = enabled;
        }
    }

    /// <summary>
    /// 转向
    /// </summary>
    public static void LookAtTarget(this Transform tf,Vector3 targetDir, float rotationSpeed)
    {
        if (targetDir != Vector3.zero)
        {
            Quaternion dir = Quaternion.LookRotation(targetDir);
            tf.rotation = Quaternion.Lerp(tf.rotation, dir, rotationSpeed);
        }
    }

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="tf"></param>
    /// <param name="layer">层级id</param>
    /// <param name="includeChild">是否包含子物体</param>
    public static void SetLayer(this Transform tf,int layer,bool includeChild = false)
    {
        if (!includeChild)
        {
            tf.gameObject.layer = layer;
        }
        else
        {
            foreach (var item in tf.GetAllChildren(true))
            {
                item.gameObject.layer = layer;
            }
        }
        
    }
    #endregion
    #region CollectionHelper
    /// <summary>
    /// 根据条件升序排列源数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="array">源数组</param>
    /// <param name="handler">排序条件</param>
    public static void OrderBy<T, TKey>(this T[] array, SelectHandler<T, TKey> handler)
       where TKey : IComparable<TKey>
    {
        for (int i = 0; i < array.Length - 1; i++)
            for (int j = i + 1; j < array.Length; j++)
                if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
    }

    /// <summary>
    /// 根据条件降序排列源数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="array">源数组</param>
    /// <param name="handler">排序条件</param>
    public static void OrderByDescending<T, TKey>(this T[] array, SelectHandler<T, TKey> handler)
        where TKey : IComparable
    {
        for (int i = 0; i < array.Length - 1; i++)
            for (int j = i + 1; j < array.Length; j++)
                if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
    }

    public delegate bool FindHandler<T>(T item);
    public delegate TKey SelectHandler<TSource, TKey>(TSource source);
    /// <summary>
    /// 从源数组中获取第一个符合条件的目标
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">源数组</param>
    /// <param name="handler">条件</param>
    /// <returns>获取到的目标</returns>
    public static T Find<T>(this T[] array, FindHandler<T> handler)
    {
        foreach (var item in array)
        {
            //调用委托
            if (handler(item))
                return item;
        }
        return default(T);
    }
    /// <summary>
    /// 从源数组中获取符合条件的所有目标的列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">源数组</param>
    /// <param name="handler">挑选条件</param>
    /// <returns>符合条件的目标,有责为列表,无则为空</returns>
    public static T[] FindAll<T>(this T[] array, FindHandler<T> handler)
    {
        List<T> tempList = new List<T>();
        foreach (var item in array)
        {
            //调用委托
            if (handler(item))
                tempList.Add(item);
        }
        return tempList.Count > 0 ? tempList.ToArray() : null;
    }
    /// <summary>
    /// 从源数组中获取符合条件的所有目标的数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="array">源数组</param>
    /// <param name="handler">挑选条件</param>
    /// <returns>符合条件的目标数组</returns>
    public static TKey[] Select<T, TKey>(this T[] array,
        SelectHandler<T, TKey> handler)
    {
        TKey[] tempArr = new TKey[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            tempArr[i] = handler(array[i]);
        }
        return tempArr;
    }
    /// <summary>
    /// 查询源数组中是否包含目标
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">源数组</param>
    /// <param name="target">目标</param>
    /// <returns></returns>
    public static bool Contain<T>(this T[] array, T target)
        where T : IEquatable<T>, IComparable
    {
        bool result = false;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(target)) result = true;
        }
        return result;
    }
    /// <summary>
    /// 按照比较条件从数组中找出最大的元素
    /// </summary>
    /// <typeparam name="T">对象数据类型</typeparam>
    /// <typeparam name="TKey">比较大小的关键数据的类型</typeparam>
    /// <param name="array">源数组</param>
    /// <param name="handler">提取比较关键数据的委托</param>
    /// <returns>按比较关键字找出的最大对象</returns>
    public static T Max<T, TKey>(this T[] array, SelectHandler<T, TKey> handler)
        where TKey : IComparable, IComparable<TKey>
    {
        T max = array[0];
        for (int i = 0; i < array.Length; i++)
        {
            if (handler(array[i]).CompareTo(handler(max)) > 0)
                max = array[i];
        }
        return max;
    }

    /// <summary>
    /// 按照比较条件从数组中找出最小的元素
    /// </summary>
    /// <typeparam name="T">对象数据类型</typeparam>
    /// <typeparam name="TKey">比较大小的关键数据的类型</typeparam>
    /// <param name="array">源数组</param>
    /// <param name="handler">提取比较关键数据的委托</param>
    /// <returns>按比较关键字找出的最小对象</returns>
    public static T Min<T, TKey>(this T[] array, SelectHandler<T, TKey> handler)
       where TKey : IComparable, IComparable<TKey>
    {
        T min = array[0];
        for (int i = 0; i < array.Length; i++)
        {
            if (handler(array[i]).CompareTo(handler(min)) < 0)
                min = array[i];
        }
        return min;
    }
    #endregion
}
