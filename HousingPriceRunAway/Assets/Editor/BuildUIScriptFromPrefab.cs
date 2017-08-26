using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class BuildUIScriptFromPrefab
{
    //UI类型缩写和类型的对应
    static Dictionary<string, string> dicUIType = null;
    static Dictionary<string, string> pathList = null;


    [MenuItem("Build/BuildComUIForNameInScript")]
    public static void BuildComUIForNameInScript()
    {
        var scriptpaths = from findcspaths in AssetDatabase.GetAllAssetPaths() where findcspaths.Contains("Assets/Scripts") &&
                          Path.GetFileNameWithoutExtension(findcspaths) == "AppMain" select findcspaths;

        string cspath = scriptpaths.ToArray<string>()[0];

        FileStream file = new FileStream(Path.GetFullPath(cspath), FileMode.Open);
        StreamReader filer = new StreamReader(file);
        string ExitUIClass = filer.ReadToEnd();
        filer.Close();
        file.Close();

        string splitstr = "//----------auto ComFilesName----------";
        string endsplitstr = "//----------auto END ----------";



        string unchangestr = ExitUIClass.Substring(0, ExitUIClass.IndexOf(splitstr));
        string changestr = ExitUIClass.Substring(ExitUIClass.IndexOf(endsplitstr), ExitUIClass.Length - ExitUIClass.IndexOf(endsplitstr));
        changestr = changestr.Replace(endsplitstr, "");

        scriptpaths = from findcspaths in AssetDatabase.GetAllAssetPaths() where findcspaths.Contains("Assets/Resources/assetbundles/ui") && Path.GetFileNameWithoutExtension(findcspaths).Contains("Com_")
                      select findcspaths;
        string comUInames = string.Empty;
        string kv = string.Empty;
        int index = 0;
        comUInames += "\r\n            ";
        foreach (string v in scriptpaths)
        {
            kv = v.Replace("Assets/Resources/assetbundles/ui/", "");
            kv = kv.Replace(".prefab", "");
            comUInames += "\"" + kv + "\"";
            if (index < scriptpaths.Count()-1)
                comUInames += ",";
            if (index!= 0 && index % 4 == 0)
                comUInames += "\r\n            ";
            index++;
        }
        comUInames += "\r\n            ";
        string finalstr = unchangestr + splitstr  + comUInames + endsplitstr + changestr;

        file = new FileStream(Path.GetFullPath(cspath), FileMode.Create);
        StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
        fileW.Write(finalstr);
        fileW.Flush();
        fileW.Close();
        file.Close();

        Debug.Log("更新脚本 AppMain.cs 成功!");
    }

    [MenuItem("Build/BuildUIScriptFromPrefab")]
    public static void BuildUIScript()
    {
        dicUIType = new Dictionary<string, string>();
        pathList = new Dictionary<string, string>();
        dicUIType.Add("Img", "Image");
        dicUIType.Add("Btn", "Button");
        dicUIType.Add("Txt", "Text");
        dicUIType.Add("Tran", "Transform");
        dicUIType.Add("RecTran", "RectTransform");
        dicUIType.Add("Scrb", "Scrollbar");
        dicUIType.Add("Sld", "Slider");
        dicUIType.Add("Ipt", "InputField");
        dicUIType.Add("Scrr", "ScrollRect");
        dicUIType.Add("RImg", "RawImage");
        dicUIType.Add("Tog", "Toggle");
        dicUIType.Add("Drop", "Dropdown");
        dicUIType.Add("Sol", "Transform");

        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("请选择需要生产脚本的UI对象!");
            return;
        }
        GameObject[] selectobjs = Selection.gameObjects;
        foreach (GameObject go in selectobjs)
        {
            //被选中的
            GameObject selectobj = go.transform.root.gameObject;
            if (selectobj.GetComponent<RectTransform>() == null)
            {
                Debug.LogWarning("请选择需要生产脚本的UI对象!");
                return;
            }

            Transform[] _transforms = selectobj.GetComponentsInChildren<Transform>(true);
            List<Transform> transforms = new List<Transform>(_transforms);
            //总节点
            var Mainuiitem = from trans in transforms where trans.name.Contains('_') && dicUIType.Keys.Contains(trans.name.Split('_')[0]) select trans;
            foreach (Transform itemtran in Mainuiitem)
            {
                string path = string.Empty;
                Transform tr = itemtran;
                List<string> pathlist = new List<string>();
                pathlist.Add(tr.name);
                while (tr != itemtran.root)
                {
                    tr = tr.parent;
                    pathlist.Add(tr.name);
                }
                for (int j = pathlist.Count - 2; j >= 0; j--)
                {
                    path = path + "/" + pathlist[j];
                }
                path = path.Substring(1);
                Debug.Log(itemtran.name);
                pathList.Add(itemtran.name, path);
            }

           
            var uiitem = from trans in Mainuiitem where !pathList[trans.name].Contains("Sol_") || trans.name.Contains("Sol_") select trans;

            var uiiconitem = from trans in Mainuiitem where trans.name.Split('_')[0] == "Sol" select trans;
            foreach (Transform itemtran in uiiconitem)
            {
                #region 刷新Icon cs文件
                var _uiIconitems = from trans in Mainuiitem where pathList[trans.name].Contains(itemtran.name) && trans.name != itemtran.name select trans;
                BuildUIIconScript(_uiIconitems, itemtran);
                #endregion
            }

            //初始化代码
            string loadedcontant = "";
            //定义代码
            string memberstring = "";
            //释放代码
            string releasestring = "";

            foreach (Transform itemtran in uiitem)
            {
                memberstring += "public " + dicUIType[itemtran.name.Split('_')[0]] + " " + itemtran.name + " = null;\r\n\t";
                releasestring += itemtran.name + " = null;\r\n\t\t";
                loadedcontant += itemtran.name + " = " + "mUIShowObj.transform.Find(\"" + pathList[itemtran.name] + "\").GetComponent<" + dicUIType[itemtran.name.Split('_')[0]] + ">();\r\n\t\t";

                if (itemtran.name.Contains("Txt_"))
                {
                    string Textdesc = string.Format("if (string.IsNullOrEmpty(LanguageManager.GetTextByKey({0}.text)) == false)\r\n\t\t\t{1}.text = LanguageManager.GetTextByKey({2}.text);\r\n\t\t", itemtran.name.ToString(), itemtran.name.ToString(), itemtran.name.ToString());
                    loadedcontant += Textdesc;
                }
            }

            var scriptpaths =
                from findcspaths in AssetDatabase.GetAllAssetPaths()
                where findcspaths.Contains("Assets/Scripts") && Path.GetFileNameWithoutExtension(findcspaths) == selectobj.name
                select findcspaths;
            if (scriptpaths.Count() == 0)
            {//第一次创建

                string NewUIClass = BuildUIScriptString.NewUIClass;
                NewUIClass = NewUIClass.Replace("#UIName#", selectobj.name);
                NewUIClass = NewUIClass.Replace("#OnAutoLoadedUIObj#", loadedcontant);
                NewUIClass = NewUIClass.Replace("#OnAutoRelease#", releasestring);
                NewUIClass = NewUIClass.Replace("#Member#", memberstring);

                NewUIClass = NewUIClass.Replace("#refresh#", selectobj.name.Contains("Com_") ? "" : "public void refresh(IMessage msg){}");
                NewUIClass = NewUIClass.Replace("#AddListener#", selectobj.name.Contains("Com_") ? "" : @"MessageDispatcher.AddListener(gameObject,DispatcherName.UI_Refresh_Event, refresh);");
                NewUIClass = NewUIClass.Replace("#RemoveListener#", selectobj.name.Contains("Com_") ? "" : @"MessageDispatcher.RemoveListener(gameObject,DispatcherName.UI_Refresh_Event, refresh);");

                FileStream file = new FileStream(Application.dataPath + "/Scripts/" + selectobj.name + ".cs", FileMode.CreateNew);
                StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
                fileW.Write(NewUIClass);
                fileW.Flush();
                fileW.Close();
                file.Close();


                Debug.Log("创建脚本 " + Application.dataPath + "/Scripts/" + selectobj.name + ".cs 成功!");

            }
            else if (scriptpaths.Count() > 1)
            {
                foreach (string cspath in scriptpaths)
                {
                    Debug.LogError("有重复脚本 " + cspath + "===" + selectobj.name + ".cs 更新失败!");
                }

            }
            else
            {

                string cspath = scriptpaths.ToArray<string>()[0];

                FileStream file = new FileStream(Path.GetFullPath(cspath), FileMode.Open);
                StreamReader filer = new StreamReader(file);
                string ExitUIClass = filer.ReadToEnd();
                filer.Close();
                file.Close();

                string splitstr = "//auto generatescript,do not make script under this line==";



                string unchangestr = ExitUIClass.Substring(0, ExitUIClass.IndexOf(splitstr));

                string NewUIClass = BuildUIScriptString.NewUIClass;
                string changestr = NewUIClass.Substring(NewUIClass.IndexOf(splitstr) + splitstr.Length, NewUIClass.Length - (NewUIClass.IndexOf(splitstr) + splitstr.Length));

                changestr = changestr.Replace("#OnAutoLoadedUIObj#", loadedcontant);
                changestr = changestr.Replace("#OnAutoRelease#", releasestring);
                changestr = changestr.Replace("#Member#", memberstring);
                changestr = changestr.Replace("#AddListener#", selectobj.name.Contains("Com_") ? "" : @"MessageDispatcher.AddListener(gameObject,DispatcherName.UI_Refresh_Event, refresh);");
                changestr = changestr.Replace("#RemoveListener#", selectobj.name.Contains("Com_") ? "" : @"MessageDispatcher.RemoveListener(gameObject,DispatcherName.UI_Refresh_Event, refresh);");

                string finalstr = unchangestr + splitstr + changestr;

                file = new FileStream(Path.GetFullPath(cspath), FileMode.Create);
                StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
                fileW.Write(finalstr);
                fileW.Flush();
                fileW.Close();
                file.Close();

                Debug.Log("更新脚本 " + selectobj.name + ".cs 成功!");
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

        }
    }

    public static void BuildUIIconScript(IEnumerable<Transform> rv, Transform oc)
    {
        string objectname = string.Empty;
        string objectInitname = string.Empty;
        string onAutoReleasename = string.Empty;
        string Membername = string.Empty;

        objectname = oc.name.Replace("_", "");
        objectInitname = oc.name.ToLower();

        Dictionary<string, string> _pathList = new Dictionary<string, string>();
        foreach (Transform itemtran in rv)
        {
            string path = string.Empty;
            Transform tr = itemtran;
            List<string> pathlist = new List<string>();
            pathlist.Add(tr.name);
            while (tr != oc)
            {
                tr = tr.parent;
                pathlist.Add(tr.name);
            }
            for (int j = pathlist.Count - 2; j >= 0; j--)
            {
                path = path + "/" + pathlist[j];
            }
            path = path.Substring(1);
            _pathList.Add(itemtran.name, path);
        }

        foreach (Transform itemtran in rv)
        {
            Membername += "public " + dicUIType[itemtran.name.Split('_')[0]] + " " + itemtran.name + " = null;\r\n\t";
            onAutoReleasename += objectInitname + "."+itemtran.name + " = " + "GameObj.transform.Find(\"" + _pathList[itemtran.name] + "\").GetComponent<" + dicUIType[itemtran.name.Split('_')[0]] + ">();\r\n\t\t";
            if (itemtran.name.Contains("Txt_"))
            {
                string Textdesc = string.Format("if (string.IsNullOrEmpty(LanguageManager.GetTextByKey({0}.{1}.text)) == false)\r\n\t\t\t{2}.{3}.text = LanguageManager.GetTextByKey({4}.{5}.text);\r\n\t\t", objectInitname.ToString(), itemtran.name.ToString(), objectInitname.ToString(), itemtran.name.ToString(), objectInitname.ToString(), itemtran.name.ToString());
                onAutoReleasename += Textdesc;
            }
        }

        var scriptpaths =
            from findcspaths in AssetDatabase.GetAllAssetPaths()
            where findcspaths.Contains("Assets/Scripts") && Path.GetFileNameWithoutExtension(findcspaths) == objectname
            select findcspaths;
        if (scriptpaths.Count() == 0)
        {//第一次创建

            string NewUIClass = BuildUIScriptString.NewUIIconClass;
            NewUIClass = NewUIClass.Replace("#UIName#", objectname);
            NewUIClass = NewUIClass.Replace("#OnAutoRelease#", onAutoReleasename);
            NewUIClass = NewUIClass.Replace("#Member#", Membername);
            NewUIClass = NewUIClass.Replace("#object#", objectInitname);

            FileStream file = new FileStream(Application.dataPath + "/Scripts/" + objectname + ".cs", FileMode.CreateNew);
            StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
            fileW.Write(NewUIClass);
            fileW.Flush();
            fileW.Close();
            file.Close();

                
            Debug.Log("创建脚本 " + Application.dataPath + "/Scripts/" + objectname + ".cs 成功!");

        }
        else if (scriptpaths.Count() > 1)
        {
            foreach (string cspath in scriptpaths)
            {
                Debug.LogError("有重复脚本 " + cspath + "===" + oc.name + ".cs 更新失败!");
            }

        }
        else
        {

            string cspath = scriptpaths.ToArray<string>()[0];

            FileStream file = new FileStream(Path.GetFullPath(cspath), FileMode.Open);
            StreamReader filer = new StreamReader(file);
            string ExitUIClass = filer.ReadToEnd();
            filer.Close();
            file.Close();

            string splitstr = "//auto generatescript,do not make script under this line==";



            string unchangestr = ExitUIClass.Substring(0, ExitUIClass.IndexOf(splitstr));

            string NewUIClass = BuildUIScriptString.NewUIIconClass;
            string changestr = NewUIClass.Substring(NewUIClass.IndexOf(splitstr) + splitstr.Length, NewUIClass.Length - (NewUIClass.IndexOf(splitstr) + splitstr.Length));

            changestr = changestr.Replace("#UIName#", objectname);
            changestr = changestr.Replace("#OnAutoRelease#", onAutoReleasename);
            changestr = changestr.Replace("#Member#", Membername);
            changestr = changestr.Replace("#object#", objectInitname);

            string finalstr = unchangestr + splitstr + changestr;

            file = new FileStream(Path.GetFullPath(cspath), FileMode.Create);
            StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
            fileW.Write(finalstr);
            fileW.Flush();
            fileW.Close();
            file.Close();

            Debug.Log("更新脚本 " + oc.name + ".cs 成功!");
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
}
