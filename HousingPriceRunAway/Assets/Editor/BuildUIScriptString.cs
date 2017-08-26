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
     

//auto generatescript,do not make script under this line==
    #region UI对象初始化
    public override void OnAutoLoadedUIObj()
	{
       
		#OnAutoLoadedUIObj#
		OnLoadedUIObj();

	}
	public override void OnAutoRelease()
	{
       
        OnRelease();
		#OnAutoRelease#
	}
	#Member#
    #endregion
}
";
}
