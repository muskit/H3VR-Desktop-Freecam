using UnityEngine;
using UnityEngine.UI;

namespace Nobi.UiRoundedCorners {
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ImageWithRoundedCorners : MonoBehaviour {
        private static int Props;

		public float radius;
		private Material material;

		[HideInInspector, SerializeField] private Image image;

        private void Awake()
        {
            Props = Shader.PropertyToID("_WidthHeightRadius");
        }

        private void OnValidate() {
			Validate();
			Refresh();
		}

		private void OnDestroy() {
			DestroyHelper.Destroy(material);
			image = null;
			material = null;
		}

		private void OnEnable() {
			Validate();
			Refresh();
		}

		private void OnRectTransformDimensionsChange() {
			if (enabled && material != null) {
				Refresh();
			}
		}

		public void Validate() {
			if (material == null) {
                var shader = Shader.Find("UI/RoundedCorners/RoundedCorners");
                if (shader == null)
                    shader = MeatKitPlugin.bundle.LoadAsset<Shader>("RoundedCorners");

                material = new Material(shader);
			}

			if (image == null) {
                image = GetComponent<Image>();
            }

			if (image != null) {
				image.material = material;
			}
		}

		public void Refresh() {
			var rect = ((RectTransform)transform).rect;
			material.SetVector(Props, new Vector4(rect.width, rect.height, radius, 0));
		}
	}
}