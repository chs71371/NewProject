using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using UnityEngine.Events;

namespace UISystem
{
    public abstract partial class UIBase : MonoBehaviour
    {
        //UI显示的GameObject
        public GameObject mUIShowObj = null;
        public eUIType mUIType = eUIType.None;
        public Canvas _mCanvas = null;
        public int iaddoder = 0;
        protected CanvasScaler _mCanvasScaler = null;
        public List<UIBase> CommonUIList = new List<UIBase>(); // 通用Com小UI
        public object refreshFuncData = null;
        public bool refreshFuncCallBack = false;
        /// <summary>
        /// 当前UI脚本类型名
        /// </summary>
        public string uiTypeName;
        /// <summary>
        /// 是否开启UI
        /// </summary>
        public bool isActive = false;
        /// <summary>
        /// 是否切换场景删除
        /// </summary>
        public bool isSwichSceneDestroy = false;
        /// <summary>
        /// 物体层级
        /// </summary>
        public int _miUIOder = 0;
        /// <summary>
        /// 打开时的层级
        /// </summary>
        protected int _perUIOder = -1;

        protected bool _mbShowUILoaded = false;


        //存储gameobject零时数据
        protected Hashtable _mTemporaryData = new Hashtable();
 

        /// <summary>
        /// 屏蔽板
        /// </summary>
        private Image ShieldPlateImage = null;
        
        internal void _OnLoadedShowUI()
        {
            _mbShowUILoaded = true;
            _mCanvas = mUIShowObj.GetComponent<Canvas>();
            _mCanvasScaler = mUIShowObj.GetComponent<CanvasScaler>();
            UIManager.Instance.SetUICompent(_mCanvas, _mCanvasScaler);
            _InitAllDicCom();
        }

        /// <summary>
        /// 引导功能逻辑接口
        /// </summary>
        public virtual GameObject getGuideObject(string str)
        {
            return null;
        }

        //显示UIprefab加载完毕后调用,所有控件对象的初始化赋值在这里进行
        public abstract void OnAutoLoadedUIObj();
        public abstract void OnLoadedUIObj();
        public abstract void OnAutoRelease();
        public abstract void OnRelease();

        public bool bShowUILoaded
        {
            get { return _mbShowUILoaded; }
        }
        /// <summary>
        /// 清理UI物体层级
        /// </summary>
        public void recoverOder()
        {
            _miUIOder = 0;
            _perUIOder = 0;
            _mCanvas.sortingOrder = 0;
        }

        /// <summary>
        /// 设置UI物体层级
        /// </summary>
        public void setCanvasOder()
        {
            _miUIOder = UIManager.Instance.GetCanvasOder(mUIType);
        }

        /// <summary>
        /// 刷新层级让UI预设提层级生效
        /// </summary>
        public void freshCanvasOder()
        {
            _miUIOder = UIManager.Instance.GetCanvasOder(mUIType);

            if (_mCanvas != null)
            {
                _mCanvas.sortingOrder = _miUIOder;
            }
            else
            {
                Debug.LogError(gameObject.name + "不存在_mCanvas");
            }

            if (_perUIOder != _miUIOder)
            {
                FreshCanvas();
                _perUIOder = _miUIOder;
            }
        }


        /// <summary>
        /// 得到当前列表层级
        /// </summary>
        public int GetCurrentCanvasOder()
        {
            return _miUIOder;
        }

        public virtual void FreshCanvas()
        {

        }

    //一些功能函数
    public string getTextColorString(string text, string color = "ffffff")
        {
            return "<color=#" + color + ">" + text + "</color>";
        }

        public string getTextImageString(string name, string size)
        {
            return "<quad emoji=" + name + " size=" + size + "/>";
        }

        /// <summary>
        /// 获取字符串宽度
        /// </summary>
        /// <param name="TargetText"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public float getTextWidth(Text TargetText, int size = 0)
        {
            CharacterInfo characterInfo;
            float width = 0f;
            if (size != 0)
                TargetText.font.RequestCharactersInTexture(TargetText.text, size, FontStyle.Normal);
            for (int i = 0; i < TargetText.text.Length; i++)
            {
                if (size != 0)
                    TargetText.font.GetCharacterInfo(TargetText.text[i], out characterInfo, size);
                else
                    TargetText.font.GetCharacterInfo(TargetText.text[i], out characterInfo);

                width += characterInfo.advance;
            }
            return width;
        }

        public void setRectTransformByPosSize(RectTransform setrect, Vector2 pos, Vector2 size)
        {
            int screenw = (int)UIManager.Instance.mReferenceResolution.x;
            int screenh = (int)UIManager.Instance.mReferenceResolution.y;

            int posx = (int)pos.x;
            int posy = (int)pos.y;

            int sizex = (int)size.x;
            int sizey = (int)size.y;

            float pivx = setrect.pivot.x;
            float pivy = setrect.pivot.y;

            Vector2 offsetmin = new Vector2(screenw / 2.0f + (posx - sizex * pivx), screenh / 2.0f + (posy - sizey * pivy));
            Vector2 offsetmax = new Vector2(-screenw / 2.0f + (posx + sizex * (1 - pivx)), -screenh / 2.0f + (posy + sizey * (1 - pivy)));


            setRectTransformByOffset(setrect, offsetmin, offsetmax);
        }

        public void setTemporaryData(GameObject ob, int index, object data)
        {
            if (_mTemporaryData.ContainsKey(ob.GetInstanceID()))
            {
                List<object> datas = (List<object>)_mTemporaryData[ob.GetInstanceID()];
                InspectCurrency(ref datas, index);
                datas[index] = data;
            }
            else
            {
                List<object> datas = new List<object>();
                InspectCurrency(ref datas, index);
                datas[index] = data;
                _mTemporaryData.Add(ob.GetInstanceID(), datas);
            }
        }

        private void InspectCurrency(ref List<object> datas, int id)
        {
            if (datas.Count <= id)
            {
                int size = id - datas.Count + 1;
                for (int i = 0; i < size; ++i)
                {
                    datas.Add(null);
                }
            }
        }

        /// <summary>
        /// 计算字符串的字符数  英文1个    汉子2个
        /// </summary>在 ASCII码表中，英文的范围是0-127，而汉字则是大于127
        /// <param name="str"></param>
        /// <returns></returns>
        public int CalculateStringChar(string str)
        {
            int charNum = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if ((int)str[i] > 127)
                    charNum += 2;
                else if ((int)str[i] != 0)
                    charNum += 1;
                else
                    charNum += 0;
            }
            return charNum;
        }

        public object getTemporaryData(GameObject ob, int index)
        {
            if (_mTemporaryData.ContainsKey(ob.GetInstanceID()))
            {
                List<object> datas = (List<object>)_mTemporaryData[ob.GetInstanceID()];
                if (index < 0 || index >= datas.Count)
                    return null;

                return datas[index];
            }
            else
            {
                return null;
            }
        }
        public void setRectTransformByOffset(RectTransform setrect, Vector2 offsetmin, Vector2 offsetmax)
        {

            int screenw = (int)UIManager.Instance.mReferenceResolution.x;
            int screenh = (int)UIManager.Instance.mReferenceResolution.y;

            int minx = (int)offsetmin.x;
            int miny = (int)offsetmin.y;

            int maxx = (int)offsetmax.x;
            int maxy = (int)offsetmax.y;

            Vector2 offsetMin = new Vector2((minx - setrect.anchorMin.x * screenw), (miny - setrect.anchorMin.y * screenh));
            Vector2 offsetMax = new Vector2((maxx + (1 - setrect.anchorMax.x) * screenw), (maxy + (1 - setrect.anchorMax.y) * screenh));

            setrect.offsetMin = offsetMin;
            setrect.offsetMax = offsetMax;
        }

        public void getRectTransformByOffset(RectTransform setrect, out Vector2 offsetmin, out Vector2 offsetmax)
        {

            int screenw = (int)UIManager.Instance.mReferenceResolution.x;
            int screenh = (int)UIManager.Instance.mReferenceResolution.y;

            offsetmin.x = setrect.offsetMin.x + setrect.anchorMin.x * screenw;
            offsetmin.y = setrect.offsetMin.y + setrect.anchorMin.y * screenh;

            offsetmax.x = setrect.offsetMax.x - (1 - setrect.anchorMax.x) * screenw;
            offsetmax.y = setrect.offsetMax.y - (1 - setrect.anchorMax.y) * screenh;
        }

        //public void OpenShieldPlate()
        //{
        //    if (ShieldPlateImage == null)
        //    {
        //        GameObject imageObj = new GameObject("ShieldPlate");
        //        RectTransform rectImage = imageObj.AddComponent<RectTransform>();
        //        ShieldPlateImage = imageObj.AddComponent<Image>();
        //        imageObj.transform.SetParent(gate.PanelWindow);
        //        rectImage.anchorMax = Vector2.one;
        //        rectImage.anchorMin = Vector2.zero;
        //        rectImage.pivot = Vector2.one * 0.5f;
        //        rectImage.anchoredPosition3D = Vector3.zero;
        //        rectImage.sizeDelta = Vector2.zero;
        //        Color color = ShieldPlateImage.color;
        //        color.a = 0;
        //        ShieldPlateImage.color = color;
        //    }
        //    ShieldPlateImage.gameObject.SetActive(true);
        //}

        public void CloseShieldPlate()
        {
            if (ShieldPlateImage != null)
            {
                ShieldPlateImage.gameObject.SetActive(false);
            }
        }




   
    }

    public abstract partial class UIBase : MonoBehaviour
    {
        public Dictionary<string, GameObject> mDicBtns = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> mDicTexts = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> mDicImages = new Dictionary<string, GameObject>();



        private void _InitAllDicCom()
        {
            mDicBtns = Util.GetAllCom<Button>(gameObject);
            mDicTexts = Util.GetAllCom<Text>(gameObject);
            mDicImages = Util.GetAllCom<Image>(gameObject);
        }


        
        protected bool CheckCurUIActive()
        {
            return UIManager.Instance.checkTargetUIActive(this.uiTypeName);
        }
        protected void SetComListState<T>(List<T> clearlist, bool state = false) where T : UIBase
        {
            if (clearlist == null)
                return;
            for (int i = 0; i < clearlist.Count; ++i)
            {
                if (clearlist[i] == null)
                    continue;
                clearlist[i].gameObject.SetActive(state);
            }
        }

        public void setOnClick(EventTriggerListener.PointerEventDelegateObj onclick, GameObject obj, string DefaultSound = "")
        {
            int key = obj.GetInstanceID();
            EventTriggerListener.Get(obj).onObjClick += onclick;
        }



        public void ReleaseClick(GameObject obj)
        {
            EventTriggerListener.Get(obj).onObjClick = null;
        }
 

    }
}



