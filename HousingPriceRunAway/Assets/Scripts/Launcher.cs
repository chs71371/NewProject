using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Launcher : MonoBehaviour
{
    //在Editor模式下是否读取bundle资源
    public bool UseBundleInEditor = true;
    //是否开启Debug输出窗口
    public bool ShowDebugLogWin = true;
    //是否开启DLoger.Log输出
    public bool ShowLog = true;
    //是否显示FPS
    public bool ShowFPS = false;
    //在Editor模式下是否读取本地代码(如果不用bundle,就不能使用分离代码,代码也是bundle)
    public bool UseLocalCodeInEditor = true;
    /// <summary>
    /// 是否开启GM命令
    /// </summary>
    public bool IsOpenGM = false;

    /// <summary>
    /// 测试发消息Crush
    /// </summary>
    public bool crushTest = false;
    /// <summary>
    /// 是否选择网络
    /// </summary>
    public bool canSelectnet = false;

    /// <summary>
    /// 是否开启新手引导
    /// </summary>
    public bool isOpenNewPlayer = false;
    /// <summary>
    /// 角色测试列表
    /// </summary>
    public string TestCharaList;
    /// <summary>
    /// 怪物测试列表
    /// </summary>
    public string TestMonList;

    public string FirstStartFunc = "Login";

    public bool IsOpenTestUI = false;

    public string testSceneName = "";

    public int testSceneId = -1;

    public int NetChoose = 0;

    /// <summary>
    /// 是否使用资源服
    /// </summary>
    public bool isResSer = false;


    public string debugTag = "";

    /// <summary>
    /// 本地是否为高清资源
    /// </summary>
    public int localIsHdRes = 0;
    /// <summary>
    /// 传递SDK信息
    /// </summary>
    public int sdkInfo = 0;


    public void Awake()
    {
        var obj = new GameObject("MainUpdate");

        obj.AddComponent<AppMain>();
        DontDestroyOnLoad(obj);
    }

}