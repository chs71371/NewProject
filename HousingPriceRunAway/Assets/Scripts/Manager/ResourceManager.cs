using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEngine.Events;


public enum eResType
{
    UI,
    ACTOR,
    EFFECT,
    AUDIO,
    TXT,
    MAT,
    SCENES,
    PREFABS,
    MODEL
}

public  class ResourceManager :MonoSingleton<ResourceManager>
{


 

    private Dictionary<string, UnityEngine.Object> pool = new Dictionary<string, UnityEngine.Object>();

    public void Load(string name, UnityAction<UnityEngine.Object, object> callBack, eResType type, object paraObj = null)
    {
        StringBuilder filepath = new StringBuilder();

        switch (type)
        {
            case eResType.UI: filepath = filepath.Append(PathTool.uiPath); break;
            case eResType.EFFECT: filepath = filepath.Append(PathTool.effectPath); break;
            case eResType.AUDIO: filepath = filepath.Append(PathTool.audioPath); break;
            case eResType.TXT: filepath = filepath.Append(PathTool.txtPath); break;
            case eResType.MAT: filepath = filepath.Append(PathTool.matPath); break;
            case eResType.SCENES: filepath = filepath.Append(PathTool.scenePath); break;
            case eResType.PREFABS: filepath = filepath.Append(PathTool.prefabsPath); break;
            case eResType.MODEL: filepath = filepath.Append(PathTool.modelPath); break;
            default: filepath = filepath.Append(""); Debug.LogError("Unkown eResType"); break;
        }


        filepath.Append(name);
        UnityEngine.Object obj;
        if (pool.ContainsKey(name))
            obj = pool[name];
        else
        {
            obj = Resources.Load(filepath.ToString());
            pool[name] = obj;
        }

        if (callBack != null)
        {
            callBack(obj, paraObj);
        }
    }

 
    /// <summary>
    /// 获取加载过的资源
    /// </summary>
    public UnityEngine.Object GetRes(string resName)
    {
        UnityEngine.Object resObj;
        pool.TryGetValue(resName, out resObj);
        if (resObj != null)
        {
            return resObj;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 获取加载过的资源 
    /// </summary>
    public T GetRes<T>(string resName) where T : UnityEngine.Object
    {
        return GetRes(resName) as T;
    }


    public void Release(string name)
    {
        if (pool.ContainsKey(name))
        {
            pool.Remove(name);
        }
    }

    public void ReleaseAll()
    {
        if (pool.Count > 0)
        {
            List<string> keys = new List<string>(pool.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                Release(keys[i]);
            }
        }
    }

    /// <summary>
    /// 获取加载过的资源
    /// </summary>
    public T GetRes<T>(string path, bool rInstantiate = true) where T : UnityEngine.Object
    {
        var res = Resources.Load<T>(path);

        if (res == null)
        {
            Debug.LogError(path + "不存在资源");
            return null;
        }

        if (rInstantiate)
        {
            Debug.Log(path + "加载");
            var obj = GameObject.Instantiate(res);
            if (typeof(T) == typeof(GameObject))
            {
                obj.name = res.name;
            }
            return obj;
        }
        else
        {
            return res;
        }
    }

}

 



