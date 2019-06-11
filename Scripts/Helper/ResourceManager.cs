using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class ResourceManager
{
    public static class ResConfiguration
    {
        //内存中对配置文件的键值映射表
        private static Dictionary<string, string> prefabMap = new Dictionary<string, string>();
        private static Dictionary<string, string> imageMap = new Dictionary<string, string>();
        static ResConfiguration()
        {
            LoadPrefab("prefabMap");
            LoadSprite("imageMap");

            //Debug.Log(imageMap.Count);
        }

        //取得配置项的内容
        public static string GetFrefabValue(string key)
        {
            if (prefabMap.ContainsKey(key))
                return prefabMap[key];
            return null;
        }
        //取得配置项的内容
        public static string GetImageValue(string key)
        {
            if (imageMap.ContainsKey(key))
                return imageMap[key];
            return null;
        }
        //加载配置文件
        public static void LoadPrefab(string path)
        {
            prefabMap.Clear();
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            StringReader reader = new StringReader(textAsset.text);
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                var keyValue = line.Split('=');
                string key = keyValue[0].Trim();
                if (prefabMap.ContainsKey(key))
                {
                    Debug.Log("预制体<" + key + ">重复");
                }
                //Debug.Log(keyValue[0].Trim());
                prefabMap.Add(key, keyValue[1].Trim());
                //Debug.Log(count++);

            }
        }
        public static void LoadSprite(string path)
        {
            imageMap.Clear();
            //int count = 0;
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            StringReader reader = new StringReader(textAsset.text);
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                var keyValue = line.Split('=');
                string key = keyValue[0].Trim();
                if (imageMap.ContainsKey(key))
                {
                    Debug.Log("图片<" + key + ">重复");
                }
                //Debug.Log(key);
                imageMap.Add(key, keyValue[1].Trim());
                //Debug.Log(count++);
            }       
        }
    }
    //加载资源  
    public static T Load<T>(string resName) where T : Object
    {
        return Resources.Load<T>(ResConfiguration.GetFrefabValue(resName));
    }
    public static object Load(string resName)
    {
        return Resources.Load(ResConfiguration.GetFrefabValue(resName));
    }
    public static Sprite LoadSprite(string resName)
    {
        return Resources.Load<Sprite>(ResConfiguration.GetImageValue(resName));
    }
   
     
}

