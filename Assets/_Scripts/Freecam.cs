using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DesktopFreecam
{
	enum LookMode { Mouse, Controller }

    // TODO:
    // * move FreecamUI into its own prefab and class?
    //   * FreecamUI manages Freecam objects
	class Freecam : MonoBehaviour
	{
		public float joystickSpeed;
		public LookMode lookMode;
		public float baseMoveSpeed;

		private bool controlEnabled;
		private Vector2 currentRotation;

		private GameObject vrCamera;
        private GameObject pipCamera;

		// UI
		private Toggle uiHideFromPlayerToggle;
		private Toggle uiPIPToggle;

        private void OnSceneChanged(Scene from, Scene to)
        {
            SetVRCamera();
            DelayedGoToPlayer();
        }

		private void Awake()
		{
			lookMode = LookMode.Mouse;
			joystickSpeed = 15;
			baseMoveSpeed = 2;
			controlEnabled = false;
            currentRotation = Vector2.zero;
            SceneManager.activeSceneChanged += OnSceneChanged;
		}
        
		private void Start()
		{
			// UI
			uiHideFromPlayerToggle = transform.FindDeepChild("VisibilityToggle").GetComponent<Toggle>();
			uiPIPToggle = transform.FindDeepChild("PIPToggle").GetComponent<Toggle>();
			uiHideFromPlayerToggle.onValueChanged.AddListener(SetVisibleToPlayer);
			uiPIPToggle.onValueChanged.AddListener(SetPIP);
            transform.FindDeepChild("Panel").GetComponent<RectTransform>()
                .CopyFrom(MeatKitPlugin.mainUI.transform.FindDeepChild("Panel").GetComponent<RectTransform>());

            // Initial state
            SetVisibleToPlayer(false);
			SetPIP(false);
			DelayedGoToPlayer();
            SetVRCamera();
        }

        private void SetVRCamera()
        {
            // TODO: use FistVR/game manager function?
            //vrCamera = GameObject.Find("Camera (head) (eye)");
            vrCamera = GameObject.FindWithTag("MainCamera");
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
                GoToPlayer();
            }
            transform.position += deltaMove.normalized * moveSpeed * Time.unscaledDeltaTime;
        }

        private void Look()
        {
            // Look
            float mouseX = Input.GetAxis("Horizontal") * (MeatKitPlugin.cfgMouseYawFlip.Value ? -1 : 1);
            float mouseY = Input.GetAxis("Vertical") * (MeatKitPlugin.cfgMousePitchFlip.Value ? 1 : -1);

            if (lookMode == LookMode.Mouse) // Mouse axes don't require deltaTime
            {
                currentRotation.x = Mathf.Clamp(currentRotation.x + MeatKitPlugin.cfgMouseSensitivity.Value / 10 * mouseY, -90, 90);
                currentRotation.y += MeatKitPlugin.cfgMouseSensitivity.Value / 10 * mouseX;
            }
            else if (lookMode == LookMode.Controller) // Controller (needs configuration by SHIFT-starting game)
            {
                currentRotation.x = Mathf.Clamp(currentRotation.x + 10 * joystickSpeed * Input.GetAxis("RightStickYAxis") * Time.deltaTime, -90, 90);
                currentRotation.y += 10 * joystickSpeed * Input.GetAxis("RightStickXAxis") * Time.deltaTime;
            }
            transform.eulerAngles = currentRotation;
        }

        // Set position to player's head
        public void GoToPlayer()
        {
            //var vrCamera = Camera.main;
            transform.position = vrCamera.transform.position - vrCamera.transform.forward;
            currentRotation = vrCamera.transform.forward;
        }
        public void DelayedGoToPlayer()
        {
            StartCoroutine(_DelayedGoToPlayer());
        }
        private IEnumerator _DelayedGoToPlayer()
        {
            yield return new WaitForSeconds(0.2f);
            yield return new WaitForEndOfFrame();
            GoToPlayer();
        }

        public void SetVisibleToPlayer(bool vis)
        {
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.enabled = vis;
            }
        }

        public void SetPIP(bool doPIP)
        {
            if (doPIP)
            {
                pipCamera = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("PIPCamera"));
                DontDestroyOnLoad(pipCamera);
            }
            else
            {
                Destroy(pipCamera);
            }
        }

        private void Update()
		{
			// Mouse lock, controls toggle
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
    }
}