using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 全局常量
/// </summary>
public static class AppConst
{
    /// <summary>
    /// 限帧（-1： 不限制）
    /// </summary>
    public static int FrameRate = -1;

    /// <summary>
    /// 垂直同步
    /// </summary>
    public static int vSyncCount = 0;

    /// <summary>
    /// 约定分辨率
    /// </summary>
    public static Vector2 ReferenceResolution = new Vector2(720, 1280);

    /// <summary>
    /// 是否激活测试UI
    /// </summary>
    public static bool IsOpenTestUI = false;

    /// <summary>
    /// 是否连接服务器
    /// </summary>
    public static bool isUseServer = true;
    /// <summary>
    /// 是否开启debug窗口功能
    /// </summary>
    public static bool isOpenDebugWin = false;


    /// <summary>
    /// 是否开启GM命令
    /// </summary>
    public static bool isGmOperation = false;
    /// <summary>
    /// 是否开启网络选择
    /// </summary>
    public static bool isCanSelectNet = false;
    /// <summary>
    /// 0是外网测试 1外网体验  2内网  3正式服  4备用 5审核
    /// </summary>
    public static int netServer;
    /// <summary>
    /// 初次加载成功后进入的功能块
    /// </summary>
    public static string FirstStartFunc = FunctionModuleName.Login;
    /// <summary>
    /// 是否开启新手引导
    /// </summary>
    public static bool isOpenNewPlayerGuide = false;

    /// <summary>
    /// 测试场景名字
    /// </summary>
    public static string testSceneName = "";

    /// <summary>
    /// 测试角色
    /// </summary>
    public static string testCharaList = "";

    /// <summary>
    /// 测试怪物角色
    /// </summary>
    public static string testMonList = "";

    /// <summary>
    /// 测试关卡ID
    /// </summary>
    public static int testSceneId = -1;
}



/// <summary>
/// 运行功能模块名
/// </summary>
public class FunctionModuleName
{
    public const string MainCity = "MainCity";
    public const string Battle = "Battle";
    public const string Login = "Login";
    public const string BattleTest = "BattleTest";
}


public class PathTool
{ 
    public const string uiPath = "assetsbundles/ui/";
 
    public const string effectPath = "assetsbundles/effect/";

    public const string audioPath = "assetsbundles/audio/";
    public const string scenePath = "assetsbundles/scenes/";
    public const string prefabsPath = "assetsbundles/prefabs/";
    public const string modelPath = "assetsbundles/model/";
    public const string txtPath = "assetsbundles/config/";
    public const string jsonPath = "data/";
    public const string localSysPath = "local/systemres/";
    public const string matPath = "";
}
