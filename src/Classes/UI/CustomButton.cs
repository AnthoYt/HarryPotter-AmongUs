using System;
using System.Linq;
using TMPro;
using hunterlib.Classes;
using UnityEngine;

namespace HarryPotter.Classes.Helpers.UI
{
    public delegate void ClickEvent();
    
    [RegisterInIl2Cpp]
    public class CustomButton : MonoBehaviour
    {
        public event ClickEvent OnClick;
        public event ClickEvent OnRightClick;
        public Color HoverColor { get; set; }
        public bool Enabled { get; set; }
        public SpriteRenderer Renderer { get; set; }

        public CustomButton(IntPtr ptr) : base(ptr) { }

        public void SetColor(Color color)
        {
            HoverColor = color;
            if (Renderer != null)
            {
                Renderer.material.SetColor("_OutlineColor", color);
            }
        }
        
        private void Start()
        {
            Enabled = true;
            Renderer = gameObject.GetComponent<SpriteRenderer>();
            if (Renderer != null)
            {
                Renderer.material.shader = Shader.Find("Sprites/Outline");
                Renderer.material.SetColor("_OutlineColor", HoverColor);
            }
            gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        }

        private void Update()
        {
            if (!Enabled && Renderer != null)
            {
                Renderer.material.SetFloat("_Outline", 0f);
            }
        }

        private void OnMouseDown()
        {
            if (!Enabled) return;
            OnClick?.Invoke();
        }

        public void OnMouseOver()
        {
            if (!Enabled || !Input.GetMouseButtonDown(1)) return;
            OnRightClick?.Invoke();
        }

        private void OnMouseEnter()
        {
            if (!Enabled || Renderer == null) return;
            Renderer.material.SetFloat("_Outline", 1f);
        }

        private void OnMouseExit()
        {
            if (!Enabled || Renderer == null) return;
            Renderer.material.SetFloat("_Outline", 0f);
        }
    }
}
