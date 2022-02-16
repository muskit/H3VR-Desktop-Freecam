using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
	enum LookMode { Mouse, Controller }
	class Freecam : MonoBehaviour
	{
		public float joystickSpeed;
		public LookMode lookMode;
		public float baseMoveSpeed;

		private bool controlEnabled;
		private Vector2 currentRotation;

		private GameObject vrCamera;
		private RenderTexture rt;
		private GameObject pipCamera;

		// UI
		private Toggle uiHideFromPlayerToggle;
		private Toggle uiPIPToggle;
		private RawImage uiPIPImage;

		private void Awake()
		{
			lookMode = LookMode.Mouse;
			joystickSpeed = 15;
			baseMoveSpeed = 2;
			controlEnabled = false;
            currentRotation = Vector2.zero;
            SetVRCamera();
		}

		private void Start()
		{
			// UI
			uiHideFromPlayerToggle = transform.FindDeepChild("VisibilityToggle").GetComponent<Toggle>();
			uiHideFromPlayerToggle.onValueChanged.AddListener(SetVisible);
			uiPIPToggle = transform.FindDeepChild("PIPToggle").GetComponent<Toggle>();
			uiPIPToggle.onValueChanged.AddListener(SetPIP);

			// PIP Camera
			rt = new RenderTexture(640, 480, 16, RenderTextureFormat.Default);
			pipCamera = transform.Find("PIPCamera").gameObject;
			if (vrCamera != null)
            {
				pipCamera.transform.SetParent(vrCamera.transform);
				pipCamera.GetComponent<Camera>().targetTexture = rt;
			}
			uiPIPImage = transform.FindDeepChild("PIPRawImage").GetComponent<RawImage>();
			uiPIPImage.texture = rt;

			// Initial state
			SetVisible(false);
			SetPIP(false);
			ResetTransform();
		}

        private void SetVRCamera()
        {
            vrCamera = GameObject.Find("Camera (head) (eye)");
        }

        private void Update()
		{
			if (vrCamera != null)
			{
				pipCamera.transform.position = vrCamera.transform.position;
				pipCamera.transform.rotation = vrCamera.transform.rotation;
			}

			// Mouse locking
			if (Input.GetKeyDown("left alt") || Input.GetKeyDown("right alt"))
				controlEnabled = !controlEnabled;
			if (controlEnabled)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
			}

			if (controlEnabled)
				Look();
		}

		private void FixedUpdate()
		{
			if (controlEnabled)
				Move();
		}

		private void Move()
		{
			float moveSpeed;
			Vector3 deltaMove = Vector3.zero;

			// Speed modifier
			if (Input.GetKey("left ctrl"))
			{
				moveSpeed = .45f * baseMoveSpeed;
			}
			else if (Input.GetKey("left shift"))
			{
				moveSpeed = 6 * baseMoveSpeed;
			}
			else
				moveSpeed = baseMoveSpeed;

			// Move
			if (Input.GetKey("w"))
			{
				deltaMove += transform.forward;
			}
			if (Input.GetKey("s"))
			{
				deltaMove -= transform.forward;
			}
			if (Input.GetKey("a"))
			{
				deltaMove -= transform.right;
			}
			if (Input.GetKey("d"))
			{
				deltaMove += transform.right;
			}
			if (Input.GetKey("e"))
			{
				deltaMove += transform.up;
			}
			if (Input.GetKey("q"))
			{
				deltaMove -= transform.up;
			}
			if (Input.GetKey("r"))
			{
				ResetTransform();
			}
			transform.position += deltaMove.normalized * moveSpeed * Time.unscaledDeltaTime;
		}

		private void Look()
		{
			// Look
			if (lookMode == LookMode.Mouse) // Mouse axes don't require deltaTime
			{
				currentRotation.x = Mathf.Clamp(currentRotation.x + MeatKitPlugin.cfgMouseSpeed.Value/10 * -Input.GetAxis("Vertical"), -90, 90);
				currentRotation.y += MeatKitPlugin.cfgMouseSpeed.Value/10 * Input.GetAxis("Horizontal");
			}
			else if (lookMode == LookMode.Controller) // Controller (needs configuration by SHIFT-starting game)
			{
				currentRotation.x = Mathf.Clamp(currentRotation.x + 10 * joystickSpeed * Input.GetAxis("RightStickYAxis") * Time.deltaTime, -90, 90);
				currentRotation.y += 10 * joystickSpeed * Input.GetAxis("RightStickXAxis") * Time.deltaTime;
			}
			transform.eulerAngles = currentRotation;
		}

		// Set position to player's head
		public void ResetTransform()
		{
			StartCoroutine(_ResetTransform());
		}
		private IEnumerator _ResetTransform()
		{
			yield return new WaitForSeconds(0.2f);
			yield return new WaitForEndOfFrame();

			var vrCamera = Camera.main;
			transform.position = vrCamera.transform.position - vrCamera.transform.forward;
			currentRotation = vrCamera.transform.forward;
		}

		public void SetVisible(bool vis)
		{
			foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
			{
				r.enabled = vis;
			}
		}

		public void SetPIP(bool doPIP)
		{
			pipCamera.SetActive(doPIP);
			uiPIPImage.enabled = doPIP;
		}
	}
}