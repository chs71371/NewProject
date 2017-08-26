using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class BuildUIScriptFromPrefab
{
 
 
    [MenuItem("Build/BuildUIScriptFromPrefab")]
    public static void BuildUIScript()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("请选择需要生产脚本的UI对象!");
            return;
        }

        var dicUIType = new Dictionary<string, string>();

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
            var mainItem = from trans in transforms where trans.name.Contains('_') && dicUIType.Keys.Contains(trans.name.Split('_')[0]) select trans;
            var  pathList = new Dictionary<string, string>();
            foreach (Transform itemtran in mainItem)
            {
                string path = string.Empty;
                Transform tr = itemtran;
                List<string> tempList = new List<string>();
                tempList.Add(tr.name);
                while (tr != itemtran.root)
                {
                    tr = tr.parent;
                    tempList.Add(tr.name);
                }
                for (int j = tempList.Count - 2; j >= 0; j--)
                {
                    path = path + "/" + tempList[j];
                }
                path = path.Substring(1);
                Debug.Log(itemtran.name);
                pathList.Add(itemtran.name, path);
            }

 
            //初始化代码
            string loadedcontant = "";
            //定义代码
            string memberstring = "";
            //释放代码
            string releasestring = "";

            foreach (Transform itemtran in mainItem)
            {
                memberstring += "public " + dicUIType[itemtran.name.Split('_')[0]] + " " + itemtran.name + " = null;\r\n\t";
                releasestring += itemtran.name + " = null;\r\n\t\t";
                loadedcontant += itemtran.name + " = " + "mUIShowObj.transform.Find(\"" + pathList[itemtran.name] + "\").GetComponent<" + dicUIType[itemtran.name.Split('_')[0]] + ">();\r\n\t\t";
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

  
}
