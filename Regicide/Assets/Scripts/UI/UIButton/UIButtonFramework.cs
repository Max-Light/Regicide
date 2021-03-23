using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public abstract class UIButtonFramework : UIInteractable
    {
        [SerializeField] protected bool isInteractable = true;

        private Action leftClickCallback = null;
        private Action rightClickCallback = null;
        private Action middleClickCallback = null;
        private Action pointerEnterCallback = null;
        private Action pointerExitCallback = null;
        private Action pointerDownCallback = null;
        private Action pointerUpCallback = null;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (!isInteractable)
            {
                return;
            }
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    leftClickCallback?.Invoke();
                    break;
                case PointerEventData.InputButton.Right:
                    rightClickCallback?.Invoke();
                    break;
                case PointerEventData.InputButton.Middle:
                    middleClickCallback?.Invoke();
                    break;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (!isInteractable)
            {
                return;
            }
            pointerEnterCallback?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (!isInteractable)
            {
                return;
            }
            pointerExitCallback?.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!isInteractable)
            {
                return;
            }
            pointerDownCallback?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!isInteractable)
            {
                return;
            }
            pointerUpCallback?.Invoke();
        }

        public void AddLeftClickEvent(Action leftClickAction) { leftClickCallback += leftClickAction; }
        public void AddRightClickEvent(Action rightClickAction) { rightClickCallback += rightClickAction; }
        public void AddMiddleClickEvent(Action middleClickAction) { middleClickCallback += middleClickAction; }
        public void AddPointerEnterEvent(Action enterAction) { pointerEnterCallback += enterAction; }
        public void AddPointerExitEvent(Action exitAction) { pointerExitCallback += exitAction; }
        public void AddPointerDownEvent(Action downEvent) { pointerDownCallback += downEvent; }
        public void AddPointerUpEvent(Action upEvent) { pointerUpCallback += upEvent; }

        public void RemoveLeftClickEvent(Action leftClickAction) { leftClickCallback -= leftClickAction; }
        public void RemoveRightClickEvent(Action rightClickAction) { rightClickCallback -= rightClickAction; }
        public void RemoveMiddleClickEvent(Action middleClickAction) { middleClickCallback -= middleClickAction; }
        public void RemovePointerEnterEvent(Action enterEvent) { pointerEnterCallback -= enterEvent; }
        public void RemovePointerExitEvent(Action exitEvent) { pointerExitCallback -= exitEvent; }
        public void RemovePointerDownEvent(Action downEvent) { pointerDownCallback -= downEvent; }
        public void RemovePointerUpEvent(Action upEvent) { pointerUpCallback -= upEvent; }
    }
}