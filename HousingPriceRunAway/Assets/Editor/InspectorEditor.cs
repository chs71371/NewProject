using UnityEditor;
using UnityEngine;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class InspectorEditor : UnityEditor.Editor
{

    public static int nPosX;
    public static int nPosY;
    public static GUIStyle titleText = new GUIStyle();
    public static GUIStyle notesText = new GUIStyle();
    public static bool isInit = false;

    private Dictionary<string, UnityEngine.Object> objectDict = new Dictionary<string, UnityEngine.Object>();

 


    public UnityEngine.Object CheckObjectDict(string key)
    {
        UnityEngine.Object InfoObj = null;

        if (key != null && objectDict.ContainsKey(key))
        {
            InfoObj = objectDict[key];
        }

        return InfoObj;
    }

    public void AddObjectDict(string key, UnityEngine.Object InfoObj) 
    {
        if (objectDict.ContainsKey(key)) 
        {
            objectDict.Remove(key);
        }
        objectDict.Add(key, InfoObj);
    }

    public static void InitPara()
    {
        if (!isInit) 
        {
            nPosX = 65;
            nPosY = 28;
            titleText.fontSize = 15;
            titleText.fontStyle = FontStyle.Normal;
            titleText.normal.textColor = Color.white;
            notesText.fontSize = 13;
            notesText.fontStyle = FontStyle.Normal;
            notesText.normal.textColor = Color.white;
            isInit = true;
        }
        
    }


    public static void DoUndo(UnityEngine.Object obj)
    {
        if (obj != null) 
        {
            EditorUtility.SetDirty(obj);
            Undo.RecordObjects(new UnityEngine.Object[] { obj }, obj.name);
        }
    }


   public static GUILayoutOption[] GUILayerOp(float hight, float width = 0, bool isExpand = true)
    {
        List<GUILayoutOption> op = new List<GUILayoutOption>();
        if (hight != 0)
        {
            op.Add(GUILayout.Height(hight));
        }

        if (width != 0)
        {
            op.Add(GUILayout.Width(width));
        }

        op.Add(GUILayout.ExpandWidth(isExpand));

        return op.ToArray();
    }

   public static void TitleDraw(string rTitleName)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField(rTitleName, titleText);
        GUILayout.Space(5);
    }

   public static void NodeDraw(string rNodeName)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField(rNodeName, notesText);
        GUILayout.Space(5);
    }


    public static void BeginBox(UnityEngine.Events.UnityAction func)
    {
        BeginHorizontalBox(() => 
        {
            BeginVerticalBox(() => 
            {
                func();
            });
        });
          
    }

   public static void BeginVerticalBox(UnityEngine.Events.UnityAction func)
    {
        GUILayout.BeginVertical("box");
        func();
        GUILayout.EndVertical();
    }

   public static void BeginHorizontalBox(UnityEngine.Events.UnityAction func)
    {
        GUILayout.BeginHorizontal();
        func();
        GUILayout.EndHorizontal();
    }
}
