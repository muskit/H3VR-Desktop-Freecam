using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    [RequireComponent(typeof(UIDraggable))]
    public class PIPWindow : MonoBehaviour
    {
        private new Camera camera;
        private RenderTexture rt;

        private RawImage uiRawImage;
        private UIDraggable uiDraggable;

        [SerializeField]
        private Texture defaultPipTexture;

        private void Awake()
        {
            uiRawImage = GetComponent<RawImage>();
            uiDraggable = GetComponent<UIDraggable>();

            Settings.specWindowDraggable.ValueChanged += OnWindowEditChange;
        }

        void Start()
        {
            SetupUI();
        }

        // trig by Settings.specEditWindow.ValueChanged
        private void OnWindowEditChange(object s, EventArgs e)
        {
            SetWindowDraggable(((State<bool>)s).Value);
        }

        public void SetWindowDraggable(bool val)
        {
            uiDraggable.enabled = val;
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
            SetupUI();
        }

        private void SetupUI(int width = -1, int height = -1)
        {
            ReleaseTexture();

            // Destroy() is delayed. Destroy(rt) would destroy our newly
            // created RT. We set the old one to another variable and
            // destroy that instead.
            var a = rt;
            Destroy(a);

            if (camera != null)
            {
                rt = new RenderTexture((width > 0 ? width : Settings.cfgPipResX.Value),
                (height > 0 ? height : Settings.cfgPipResY.Value), 16, RenderTextureFormat.ARGB32);
                camera.targetTexture = rt;
                uiRawImage.texture = rt;
            }
            else
            {
                uiRawImage.texture = defaultPipTexture;
            }
            uiRawImage.rectTransform.sizeDelta = new Vector2(uiRawImage.texture.width, uiRawImage.texture.height);
        }

        private void ReleaseTexture()
        {
            if (rt != null)
                rt.Release();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            ReleaseTexture();
            Destroy(rt);

            Settings.specWindowDraggable.ValueChanged -= OnWindowEditChange;
        }
    }
}