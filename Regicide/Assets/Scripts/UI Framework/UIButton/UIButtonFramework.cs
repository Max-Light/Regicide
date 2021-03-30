using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public abstract class UIButtonFramework : UIInteractable
    {
        [SerializeField] protected bool isInteractable = true;

        private Action _leftClickCallback = null;
        private Action _rightClickCallback = null;
        private Action _middleClickCallback = null;
        private Action _pointerEnterCallback = null;
        private Action _pointerExitCallback = null;
        private Action _pointerDownCallback = null;
        private Action _pointerUpCallback = null;

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
                    _leftClickCallback?.Invoke();
                    break;
                case PointerEventData.InputButton.Right:
                    _rightClickCallback?.Invoke();
                    break;
                case PointerEventData.InputButton.Middle:
                    _middleClickCallback?.Invoke();
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
            _pointerEnterCallback?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (!isInteractable)
            {
                return;
            }
            _pointerExitCallback?.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!isInteractable)
            {
                return;
            }
            _pointerDownCallback?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!isInteractable)
            {
                return;
            }
            _pointerUpCallback?.Invoke();
        }

        public void AddLeftClickEvent(Action leftClickAction) { _leftClickCallback += leftClickAction; }
        public void AddRightClickEvent(Action rightClickAction) { _rightClickCallback += rightClickAction; }
        public void AddMiddleClickEvent(Action middleClickAction) { _middleClickCallback += middleClickAction; }
        public void AddPointerEnterEvent(Action enterAction) { _pointerEnterCallback += enterAction; }
        public void AddPointerExitEvent(Action exitAction) { _pointerExitCallback += exitAction; }
        public void AddPointerDownEvent(Action downEvent) { _pointerDownCallback += downEvent; }
        public void AddPointerUpEvent(Action upEvent) { _pointerUpCallback += upEvent; }

        public void RemoveLeftClickEvent(Action leftClickAction) { _leftClickCallback -= leftClickAction; }
        public void RemoveRightClickEvent(Action rightClickAction) { _rightClickCallback -= rightClickAction; }
        public void RemoveMiddleClickEvent(Action middleClickAction) { _middleClickCallback -= middleClickAction; }
        public void RemovePointerEnterEvent(Action enterEvent) { _pointerEnterCallback -= enterEvent; }
        public void RemovePointerExitEvent(Action exitEvent) { _pointerExitCallback -= exitEvent; }
        public void RemovePointerDownEvent(Action downEvent) { _pointerDownCallback -= downEvent; }
        public void RemovePointerUpEvent(Action upEvent) { _pointerUpCallback -= upEvent; }
    }
}