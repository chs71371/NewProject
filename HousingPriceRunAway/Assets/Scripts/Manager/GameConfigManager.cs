using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigManager : Singleton<GameConfigManager>
{
    GameConfigManager(){ }

    //public RoleInfo GetRoleConfig(string rModelName)
    //{
    //    var config = ResourceLoader.Instance.GetRes<RoleInfo>(PathTool.txtPath + "dynConfig_model_" + rModelName);

    //    if (config == null)
    //    {
    //        Debug.LogError("不存在角色配置资源："+ rModelName);
    //    }
    //    return config;
    //}

}
