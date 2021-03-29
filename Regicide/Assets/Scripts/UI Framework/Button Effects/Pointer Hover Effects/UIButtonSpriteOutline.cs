
using UnityEngine;

namespace UI
{
    public class UIButtonSpriteOutline : MonoBehaviour, IUIButtonHoverResponse
    {
        private Material spriteDefaultMaterial = null;
        [SerializeField] private Material spriteOutlineMaterial = null;

        [SerializeField] private SpriteRenderer spriteRenderer = null;

        private void OnValidate()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            spriteDefaultMaterial = spriteRenderer.material;
        }

        public void OnDeselect(UIButtonFramework button)
        {
            spriteRenderer.material = spriteDefaultMaterial;
        }

        public void OnSelect(UIButtonFramework button)
        {
            spriteRenderer.material = spriteOutlineMaterial;
        }
    }
}