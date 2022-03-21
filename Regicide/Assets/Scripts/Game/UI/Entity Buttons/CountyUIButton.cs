
using Regicide.Game.Entity;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Regicide.UI
{
    public class CountyUIButton : UIStateButtonFramework
    {
        [SerializeField] private County _county = null;
        [SerializeField] private IUIButtonHoverResponse _buttonHoverEffect = null;

        public static List<CountyUIButton> CountyButtons { get; private set; } = new List<CountyUIButton>();

        private void OnValidate()
        {
            _buttonHoverEffect = GetComponent<IUIButtonHoverResponse>();
        }

        private void OnEnable()
        {
            CountyButtons.Add(this);
            if (_buttonHoverEffect != null)
            {
                AddPointerEnterEvent(() => _buttonHoverEffect.OnSelect(this));
                AddPointerExitEvent(() => _buttonHoverEffect.OnDeselect(this));
            }
        }

        private void OnDisable()
        {
            CountyButtons.Remove(this);
            if (_buttonHoverEffect != null)
            {
                RemovePointerEnterEvent(() => _buttonHoverEffect.OnSelect(this));
                RemovePointerExitEvent(() => _buttonHoverEffect.OnDeselect(this));
            }
        }

        public County County { get => _county; }
    }
}