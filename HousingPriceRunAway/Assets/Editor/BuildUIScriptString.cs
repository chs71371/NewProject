using UnityEngine;
using System.Collections;

public class BuildUIScriptString
{
    public static string NewUIClass =
@"using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UISystem;
using System;

public class #UIName# : UIBase
{
    public override void OnLoadedUIObj()
    {
    }
    public override void OnRelease()
    {
    }
    #refresh#

//auto generatescript,do not make script under this line==
    #region UI对象初始化
    public override void OnAutoLoadedUIObj()
	{
        #AddListener#
		#OnAutoLoadedUIObj#
		OnLoadedUIObj();

	}
	public override void OnAutoRelease()
	{
        #RemoveListener#
        OnRelease();
		#OnAutoRelease#
	}
	#Member#
    #endregion
}
";
    public static string NewUIIconClass =
@"using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class #UIName# : MonoBehaviour
{
//auto generatescript,do not make script under this line==
    #region UI对象初始化
    public static #UIName# Initialization(GameObject ob)
    {
        GameObject GameObj = GameObject.Instantiate(ob) as GameObject;
        #UIName# #object# = GameObj.AddComponent<#UIName#>();
        #OnAutoRelease#
        return #object#;
    }
    #Member#
    #endregion
}
";
}
