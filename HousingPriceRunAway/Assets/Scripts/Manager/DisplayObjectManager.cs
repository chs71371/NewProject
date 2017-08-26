using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisplayObjectManager : MonoSingleton<DisplayObjectManager>
{
    DisplayObjectManager() { }
 

    public GameObject CreatRole(string rResName)
    {
        var roleObj= ResourceManager.Instance.GetRes<GameObject>(PathTool.modelPath + rResName);
        if (roleObj == null)
        {
            Debug.LogError("不存在资源:"+ PathTool.modelPath + rResName);
            return null;
        }

        return roleObj;
    }

}
