
using Regicide.Game.Entities;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Regicide.UI
{
    public class CountyUIButton : UIStateButtonFramework
    {
        [SerializeField] private County county = null;
        [SerializeField] private IUIButtonHoverResponse buttonHoverEffect = null;

        public static List<CountyUIButton> CountyButtons { get; private set; } = new List<CountyUIButton>();

        private void OnValidate()
        {
            buttonHoverEffect = GetComponent<IUIButtonHoverResponse>();
        }

        private void OnEnable()
        {
            CountyButtons.Add(this);
            if (buttonHoverEffect != null)
            {
                AddPointerEnterEvent(() => buttonHoverEffect.OnSelect(this));
                AddPointerExitEvent(() => buttonHoverEffect.OnDeselect(this));
            }
        }

        private void OnDisable()
        {
            CountyButtons.Remove(this);
            if (buttonHoverEffect != null)
            {
                RemovePointerEnterEvent(() => buttonHoverEffect.OnSelect(this));
                RemovePointerExitEvent(() => buttonHoverEffect.OnDeselect(this));
            }
        }

        public County County { get => county; }
    }
}