using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
 

[CustomEditor(typeof(Launcher))]
public class LauncherInspectorEditor : InspectorEditor
{
    Launcher mScript;


    void OnEnable()
    {
        mScript = target as Launcher;
        InitPara();
    }


    private enum FirstStartFunc
    {
        开场引导,
        登录,
        主城,
        战斗,
    }

    private FirstStartFunc eFirstStartFunc = FirstStartFunc.登录;

    public override void OnInspectorGUI()
    {
        TitleDraw("运行设定");

        BeginVerticalBox(() =>
        {
            mScript.UseBundleInEditor = GUILayout.Toggle(mScript.UseBundleInEditor, "Editor模式下读取bundle资源");
            mScript.UseLocalCodeInEditor = GUILayout.Toggle(mScript.UseLocalCodeInEditor, "使用本地代码");

        });

        TitleDraw("显示信息");

        BeginVerticalBox(() =>
        {
            mScript.ShowDebugLogWin = GUILayout.Toggle(mScript.ShowDebugLogWin, "开启Debug输出窗口");
            mScript.ShowLog = GUILayout.Toggle(mScript.ShowLog, "开启DLoger.Log输出");
            mScript.ShowFPS = GUILayout.Toggle(mScript.ShowFPS, "开启FPS显示");
            mScript.IsOpenGM = GUILayout.Toggle(mScript.IsOpenGM, "开启GM命令");
            mScript.crushTest = GUILayout.Toggle(mScript.crushTest,"测试收发消息");
            mScript.canSelectnet = GUILayout.Toggle(mScript.canSelectnet,"开启网络选择");
            mScript.NetChoose = EditorGUILayout.IntField("当前网络", mScript.NetChoose);
            if (GUI.changed)
            {
                GUI.changed = false;
                DoUndo(mScript);
            }
        });
       
        TitleDraw("输出信息设置");
        BeginVerticalBox(() =>
        {
            mScript.debugTag = EditorGUILayout.TextField("要显示的DebugTag", mScript.debugTag);

            if (GUI.changed)
            {
                GUI.changed = false;
                DoUndo(mScript);
            }
        });


        TitleDraw("游戏功能");


        BeginVerticalBox(() =>
        {
            if (mScript.FirstStartFunc != null) 
            {
                switch (mScript.FirstStartFunc)
                {
                    case "Login":
                        eFirstStartFunc = FirstStartFunc.登录;
                        break;
                    case "MainCity":
                        eFirstStartFunc = FirstStartFunc.主城;
                        break;
                    case "Battle":
                        eFirstStartFunc = FirstStartFunc.战斗;
                        break;
                    case "BattleTest":
                        eFirstStartFunc = FirstStartFunc.开场引导;
                        break;
                }    
            }

       
            eFirstStartFunc = (FirstStartFunc)EditorGUILayout.EnumPopup("运行模块", eFirstStartFunc);

            switch (eFirstStartFunc)
            {
                case FirstStartFunc.登录:
                    mScript.FirstStartFunc = FunctionModuleName.Login;
                    break;
                case FirstStartFunc.主城:
                    mScript.FirstStartFunc = FunctionModuleName.MainCity;
                    break;
                case FirstStartFunc.战斗:
                    mScript.FirstStartFunc = FunctionModuleName.Battle;
                    break;
                case FirstStartFunc.开场引导:
                    mScript.FirstStartFunc = FunctionModuleName.BattleTest;
                    break;
            }

            if (GUI.changed)
            {
                GUI.changed = false;
                DoUndo(mScript);
            }

            mScript.isOpenNewPlayer = EditorGUILayout.Toggle("是否开启新手引导", mScript.isOpenNewPlayer);

            mScript.IsOpenTestUI = EditorGUILayout.Toggle("是否打开测试UI", mScript.IsOpenTestUI);

            mScript.testSceneName = EditorGUILayout.TextField("测试场景名", mScript.testSceneName);

            mScript.testSceneId = EditorGUILayout.IntField("测试场景Id", mScript.testSceneId);

            mScript.TestCharaList = EditorGUILayout.TextField("测试角色的Id", mScript.TestCharaList);

            mScript.TestMonList = EditorGUILayout.TextField("测试怪物的Id", mScript.TestMonList);


            if (GUI.changed)
            {
                GUI.changed = false;
                DoUndo(mScript);
            }
        });
    }
}
