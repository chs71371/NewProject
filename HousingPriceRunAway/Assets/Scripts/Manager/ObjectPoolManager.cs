using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UISystem;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    ObjectPoolManager() { }

    private Hashtable cachePool = new Hashtable();
    private Hashtable resPool = new Hashtable();
    private Hashtable residentPool = new Hashtable();
    private GameObject CachePanel;

    /// <summary>
    /// 缓存池
    /// </summary>
    private Dictionary<string, Queue<GameObject>> m_Pool = new Dictionary<string, Queue<GameObject>>();
    /// <summary>
    /// 缓存池标记数组
    /// </summary>
    private Dictionary<GameObject, string> m_GoTag = new Dictionary<GameObject, string>();

    public bool ResPoolContainsKey(string resName)
    {
        if (resPool.ContainsKey(resName)) { return true; }
        return false;
    }

    public bool CachePoolContainsKey(string resName)
    {
        if (cachePool.ContainsKey(resName)) {  return true; }
        return false;
    }

    /// <summary>
    /// 调用添加缓存资源
    /// </summary>
    public void AddRes(UnityEngine.Object obj, UnityAction func = null)
    {
        if (obj==null)
        {
            return;
        }

        string resName = obj.name;

        if (resPool.ContainsKey(resName))
        {
            resPool.Remove(resName);
        }

        resPool.Add(resName, obj);

        if (func != null)
        {
            func();
        }
    }

    /// <summary>
    /// 添加常驻资源
    /// </summary>
    public void AddResidentRes(UnityEngine.Object obj, UnityAction func = null)
    {
        string resName = obj.name;

        if (residentPool.ContainsKey(resName))
        {
            residentPool.Remove(resName);
        }

        residentPool.Add(resName, obj);

        if (func != null)
        {
            func();
        }
    }




    public T GetRes<T>(string resName) where T : UnityEngine.Object
    {
        if (!resPool.ContainsKey(resName))
        {
            if (!residentPool.ContainsKey(resName))
            {
                Debug.LogError(resName + "资源池内不存在");
                return default(T);
            }

            return residentPool[resName] as T;
        }
        return resPool[resName] as T;
    }

    /// <summary>
    /// 调用添加缓存资源
    /// </summary>
    public void AddObjectPoolRes(GameObject obj, UnityAction func = null)
    {
        string resName = obj.name;

        if (cachePool.ContainsKey(resName))
        {
            return;
        }


        if (CachePanel == null)
        {
            CachePanel = new GameObject();
            CachePanel.name = "CachePanel";
            GameObject.DontDestroyOnLoad(CachePanel);
        }


        GameObject gameObj = GameObject.Instantiate(obj) as GameObject;
        gameObj.name = resName;

        gameObj.transform.SetParent(CachePanel.transform);
        gameObj.transform.position = Vector3.zero;

        cachePool.Add(resName, gameObj);
        gameObj.SetActive(false);

        if (func != null)
        {
            func();
        }
    }

    /// <summary>
    /// 得到对象池资源   暂时直接复制存入的资源，避免被外部调整大小的脚本污染对象池
    /// </summary>
    public GameObject GetObjectPoolRes(string resName, bool isGetSelf = false)
    {
        if (!cachePool.ContainsKey(resName))
        {
    
            return null;
        }

        GameObject resObj = cachePool[resName] as GameObject;
        GameObject getObj = null;

        if (!isGetSelf)
        {
            if (resObj == null)
            {
                Debug.LogError(resName + "资源为空");
                return null;
            }
            GameObject newObj = GameObject.Instantiate(resObj) as GameObject;
            newObj.SetActive(true);
            newObj.name = resName;
            newObj.transform.SetParent(CachePanel.transform);
            return newObj;

        }
        else
        {
            getObj = resObj;
        }
        return getObj;
    }

    /// <summary>
    /// 回收对象池资源
    /// </summary>
    public void BackObjectPoolRes(GameObject resObj)
    {
        if (resObj == null)
        {
            return;
        }
        GameObject.Destroy(resObj);
    }

    public void ClearAllRes()
    {
        for (int i = 0; i < cachePool.Count; i++)
        {
            List<GameObject> objList = cachePool[i] as List<GameObject>;
            if (objList != null)
            {
                for (int j = 0; j < objList.Count; j++)
                {
                    if (objList[j] != null)
                    {
                        GameObject.DestroyImmediate(objList[j]);
                    }
                }
            }

        }

        if (CachePanel != null)
        {
            GameObject.DestroyImmediate(CachePanel);
        }

        resPool.Clear();
        cachePool.Clear();
    }

    public void OnRelease()
    {
        ClearAllRes();
        ClearCachePool();
    }


    #region  缓存池
    /// <summary>
    /// 清空缓存池，释放所有引用
    /// </summary>
    public void ClearCachePool()
    {
        m_Pool.Clear();
        m_GoTag.Clear();
    }

    /// <summary>
    /// 回收GameObject
    /// </summary>
    public void ReturnCacheGameObejct(GameObject go)
    {
        if (CachePanel == null)
        {
            CachePanel = new GameObject();
            CachePanel.name = "CachePanel";
            GameObject.DontDestroyOnLoad(CachePanel);
        }

        if (go==null)
        {
            return;
        }
 
        go.transform.parent = CachePanel.transform;
        go.SetActive(false);

        string tag = go.GetInstanceID().ToString(); ;

        if (m_GoTag.ContainsKey(go))
        {
            tag = m_GoTag[go];
            RemoveOutMark(go);
        }


        if (!m_Pool.ContainsKey(tag))
        {
            m_Pool[tag] = new Queue<GameObject>();
        }

        m_Pool[tag].Enqueue(go);
    }

    /// <summary>
    /// 请求GameObject
    /// </summary>
    public GameObject RequestCacheGameObejct(GameObject prefab)
    {
        string tag = prefab.GetInstanceID().ToString();
        GameObject go = GetFromPool(tag);
        if (go == null)
        {
            go = GameObject.Instantiate<GameObject>(prefab);
            go.name = prefab .name+ Time.time;
        }

       
        MarkAsOut(go, tag);
        return go;
    }

    
    private GameObject GetFromPool(string tag)
    {
        if (m_Pool.ContainsKey(tag) && m_Pool[tag].Count > 0)
        {
            GameObject obj = m_Pool[tag].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return null;
        }
    }

 
    private void MarkAsOut(GameObject go, string tag)
    {
        m_GoTag.Add(go, tag);
    }
     
    private void RemoveOutMark(GameObject go)
    {
        if (m_GoTag.ContainsKey(go))
        {
            m_GoTag.Remove(go);
        }
        else
        {
            Debug.LogError("remove out mark error, gameObject has not been marked");
        }
    }
    #endregion
}
