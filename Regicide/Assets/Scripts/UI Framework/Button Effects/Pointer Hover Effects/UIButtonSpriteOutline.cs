
using UnityEngine;

namespace UI
{
    public class UIButtonSpriteOutline : MonoBehaviour, IUIButtonHoverResponse
    {
        private Material _spriteDefaultMaterial = null;
        [SerializeField] private Material _spriteOutlineMaterial = null;

        [SerializeField] private SpriteRenderer _spriteRenderer = null;

        private void OnValidate()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            _spriteDefaultMaterial = _spriteRenderer.material;
        }

        public void OnDeselect(UIButtonFramework button)
        {
            _spriteRenderer.material = _spriteDefaultMaterial;
        }

        public void OnSelect(UIButtonFramework button)
        {
            _spriteRenderer.material = _spriteOutlineMaterial;
        }
    }
}