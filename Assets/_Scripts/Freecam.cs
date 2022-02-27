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
        //public Rigidbody rigidBody;
        public CharacterController characterController;
        public Camera camera;
		public LookMode lookMode;
		public float joystickSpeed;

        private bool controlEnabled = false;
        private bool physicsEnabled = false;

        private float height;
        private float moveSpeed;
        private float verticalVelocity = 0;
        private float gravitationalAccel = .2f;
        private Vector3 inputDeltaMove = Vector3.zero;
		private Vector2 currentRotation = Vector2.zero;

		private GameObject vrCamera;
        private GameObject povCamera;

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
            characterController.gameObject.layer = LayerMask.NameToLayer("ExternalCamOnly");
            height = characterController.height;
			lookMode = LookMode.Mouse;
			joystickSpeed = 15;
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

        private void UpdateInputMovement()
        {
            // Speed modifier
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.SlowDown].Value))
            {
                moveSpeed = MeatKitPlugin.cfgCameraFlySlowMult.Value * MeatKitPlugin.cfgCameraFlySpeed.Value;
            }
            else if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.SpeedUp].Value))
            {
                moveSpeed = MeatKitPlugin.cfgCameraFlyFastMult.Value * MeatKitPlugin.cfgCameraFlySpeed.Value;
            }
            else
                moveSpeed = MeatKitPlugin.cfgCameraFlySpeed.Value;

            // Movement
            inputDeltaMove = Vector3.zero;
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.MoveForward].Value))
            {
                inputDeltaMove += camera.transform.forward;
            }
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.MoveBackward].Value))
            {
                inputDeltaMove -= camera.transform.forward;
            }
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.MoveLeft].Value))
            {
                inputDeltaMove -= camera.transform.right;
            }
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.MoveRight].Value))
            {
                inputDeltaMove += camera.transform.right;
            }
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.Ascend].Value))
            {
                inputDeltaMove += Vector3.up;
            }
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.Descend].Value))
            {
                inputDeltaMove -= Vector3.up;
            }
            inputDeltaMove.Normalize();
        }

        private void MoveNoclip()
        {
            characterController.transform.position += inputDeltaMove * moveSpeed * Time.unscaledDeltaTime;
        }

        private void UpdatePhysics()
        {
            // Crouch
            if (Input.GetKeyDown(MeatKitPlugin.cfgKeyboard[KBControls.Descend].Value))
            {
                characterController.height = height / 2;

                var pos = characterController.transform.position;
                pos.y -= height / 2;
                characterController.transform.position = pos;
            }
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.Descend].Value))
            {
                moveSpeed *= MeatKitPlugin.cfgCameraFlySlowMult.Value;
            }

            // Un-crouch
            if (Input.GetKeyUp(MeatKitPlugin.cfgKeyboard[KBControls.Descend].Value))
            {
                characterController.height = height;

                var pos = characterController.transform.position;
                pos.y += height / 2;
                characterController.transform.position = pos;
            }

            camera.transform.localPosition = new Vector3(0, characterController.height/2, 0);

            // Jump
            if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.Ascend].Value) && characterController.isGrounded)
                verticalVelocity += 3;

            // Gravity
            if (characterController.isGrounded)
            {
                verticalVelocity = 0;
            }
            else
            {
                verticalVelocity -= gravitationalAccel;
            }

            // Movement
            Vector3 moveDelta = inputDeltaMove;
            moveDelta.y = 0;
            moveDelta.Normalize();

            characterController.Move( (moveSpeed*moveDelta + new Vector3(0, verticalVelocity)) * Time.fixedDeltaTime );
        }

        private void Look()
        {
            // Look
            float mouseX = Input.GetAxis("Horizontal") * (MeatKitPlugin.cfgMouseYawFlip.Value ? -1 : 1);
            float mouseY = Input.GetAxis("Vertical") * (MeatKitPlugin.cfgMousePitchFlip.Value ? 1 : -1);

            if (lookMode == LookMode.Mouse) // Mouse axes don't require deltaTime
            {
                currentRotation.x = Mathf.Clamp(currentRotation.x + MeatKitPlugin.cfgMouseSensitivity.Value / 10 * mouseY, -89.99f, 89.99f);
                currentRotation.y += MeatKitPlugin.cfgMouseSensitivity.Value / 10 * mouseX;
            }
            else if (lookMode == LookMode.Controller) // Controller (needs configuration by SHIFT-starting game)
            {
                currentRotation.x = Mathf.Clamp(currentRotation.x + 10 * joystickSpeed * Input.GetAxis("RightStickYAxis") * Time.deltaTime, -90, 90);
                currentRotation.y += 10 * joystickSpeed * Input.GetAxis("RightStickXAxis") * Time.deltaTime;
            }
            camera.transform.eulerAngles = currentRotation;
        }

        private void ScrollWheel()
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.05f)
            {
                switch (MeatKitPlugin.cfgScrollMode.Value)
                {
                    case ScrollWheelMode.MoveSpeed:
                        float newVal = MeatKitPlugin.cfgCameraFlySpeed.Value + 0.6f * Input.mouseScrollDelta.y;
                        if (newVal > 0)
                            MeatKitPlugin.cfgCameraFlySpeed.Value = newVal;
                        break;
                    case ScrollWheelMode.FieldOfView:
                        newVal = MeatKitPlugin.cfgCameraFov.Value + 2 * Input.mouseScrollDelta.y;
                        if (5 < newVal && newVal < 179.99)
                            MeatKitPlugin.cfgCameraFov.Value = newVal;
                        break;
                }
            }   
        }

        // Set position to player's head
        public void GoToPlayer()
        {
            //var vrCamera = Camera.main;
            characterController.transform.position = vrCamera.transform.position - vrCamera.transform.forward;
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

        public void SetPIP(bool pipEnabled)
        {
            if (pipEnabled)
            {
                povCamera = Instantiate(MeatKitPlugin.bundle.LoadAsset<GameObject>("POVCamera"));
                DontDestroyOnLoad(povCamera);
            }
            else
            {
                Destroy(povCamera);
            }
        }

        private void Update()
		{
            camera.fieldOfView = MeatKitPlugin.cfgCameraFov.Value;

            if (Input.GetKeyDown(KeyCode.F))
                physicsEnabled = !physicsEnabled;

            // controls toggle
            if (Input.GetKeyDown(MeatKitPlugin.cfgKeyboard[KBControls.ToggleControls].Value))
				controlEnabled = !controlEnabled;

            // Input & movement
			if (controlEnabled)
			{
				Cursor.lockState = CursorLockMode.Locked;
                UpdateInputMovement();
                if (Input.GetKey(MeatKitPlugin.cfgKeyboard[KBControls.TeleportToPlayer].Value))
                {
                    GoToPlayer();
                }
                if (!physicsEnabled)
                    MoveNoclip();

                Look();
                ScrollWheel();
            }
			else
			{
				Cursor.lockState = CursorLockMode.None;
			}
		}
        private void FixedUpdate()
        {
            if (physicsEnabled)
            {
                UpdatePhysics();
            }
            else
            {
                verticalVelocity = 0;
            }
        }

        private void OnDestroy()
        {
            Destroy(povCamera);
        }
    }
}