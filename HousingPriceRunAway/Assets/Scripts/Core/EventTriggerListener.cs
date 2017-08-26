using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem
{
    public class EventTriggerListener : EventTrigger
    {
        public delegate void PointerEventDelegate(PointerEventData eventData);
        public delegate void PointerEventDelegateObj(PointerEventData eventData, GameObject ob);
        public PointerEventDelegate onClick;
        public PointerEventDelegate onDown;
        public PointerEventDelegate onEnter;
        public PointerEventDelegate onExit;
        public PointerEventDelegate onUp;
        public PointerEventDelegate onDrag;
        public PointerEventDelegate onDrop;
        public PointerEventDelegate onBeginDrag;
        public PointerEventDelegate onEndDrag;

        public PointerEventDelegateObj onObjClick;
        public PointerEventDelegateObj onObjDown;
        public PointerEventDelegateObj onObjDrag;
        public PointerEventDelegateObj onObjUp;
        public PointerEventDelegateObj onObjBeginDrag;
        public PointerEventDelegateObj onObjEndDrag;

        public delegate void BaseEventDelegate(BaseEventData eventData);
        public BaseEventDelegate onSelect;
        public BaseEventDelegate onUpdateSelect;

        private bool IsDraging = false;

        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsDraging)
            {
                if (onClick != null) onClick(eventData);
                if (onObjClick != null) onObjClick(eventData, gameObject);
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null) onDown(eventData);
            if (onObjDown != null) onObjDown(eventData,gameObject);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null) onUp(eventData);
            if (onObjUp != null) onObjUp(eventData,gameObject);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(eventData);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null) onUpdateSelect(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) onDrag(eventData);
            if (onObjDrag != null) onObjDrag(eventData,gameObject);
        }

        public override void OnDrop(PointerEventData eventData)
        {
            if (onDrop != null) onDrop(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDrag != null) onBeginDrag(eventData);
            if (onObjBeginDrag != null) onObjBeginDrag(eventData,gameObject);
            IsDraging = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDrag != null) onEndDrag(eventData);
            if (onObjEndDrag != null) onObjEndDrag(eventData,gameObject);
            IsDraging = false;
        }
    }
}

