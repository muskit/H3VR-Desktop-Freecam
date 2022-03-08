using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DesktopFreecam
{
	enum ControlType { KBMouse, Controller }

    // TODO:
    // * move FreecamUI into its own prefab and class?
    //   * FreecamUI manages Freecam objects
	class Freecam : MonoBehaviour
	{
        public CharacterController characterController;
        public new Camera camera;
		public ControlType lookMode;
		public float joystickSpeed;

        private bool controlEnabled = false;
        private bool physicsEnabled = false;

        private float height;
        private float moveSpeed;
        private float verticalVelocity = 0;

        private float gravitationalAccel = .2f;
        private float jumpVel = 6.5f;

        private Vector3 inputDeltaMove = Vector3.zero;
		private Vector2 currentRotation = Vector2.zero;

		private GameObject vrCamera;
        private GameObject povCamera;

        #region INITIALIZATION
        private void Awake()
		{
            characterController.gameObject.layer = LayerMask.NameToLayer("ExternalCamOnly");
            height = characterController.height;
			lookMode = ControlType.KBMouse;
			joystickSpeed = 15;

            Settings.freecamVisibleToPlayer.ValueChanged += OnVRVisibilityChange;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        
		private void Start()
		{
            // Initial state
            SetVisibleToPlayer(Settings.freecamVisibleToPlayer.Value);
			DelayedGoToPlayer();
            SetVRCamera();
        }
        #endregion

        private void OnSceneChanged(Scene from, Scene to)
        {
            SetVRCamera();
            DelayedGoToPlayer();
        }

        private void SetVRCamera()
        {
            // TODO: use FistVR/game manager function?
            //vrCamera = GameObject.Find("Camera (head) (eye)");
            vrCamera = GameObject.FindWithTag("MainCamera");
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

        private void OnVRVisibilityChange(object sender, EventArgs e)
        {
            SetVisibleToPlayer(Settings.freecamVisibleToPlayer.Value);
        }

        #region UPDATE
        private void UpdateInputMovement()
        {
            // Speed modifier
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.SlowDown].Value))
            {
                moveSpeed = Settings.cfgCameraFlySlowMult.Value * Settings.cfgCameraFlySpeed.Value;
            }
            else if (Input.GetKey(Settings.cfgKeyboard[KBControls.SpeedUp].Value))
            {
                moveSpeed = Settings.cfgCameraFlyFastMult.Value * Settings.cfgCameraFlySpeed.Value;
            }
            else
                moveSpeed = Settings.cfgCameraFlySpeed.Value;

            // Crouch state
            if (physicsEnabled)
            {
                if (Input.GetKeyDown(Settings.cfgKeyboard[KBControls.Descend].Value))
                {
                    characterController.height = height / 2;

                    var pos = characterController.transform.position;
                    pos.y -= height / 2;
                    characterController.transform.position = pos;
                }
                if (Input.GetKey(Settings.cfgKeyboard[KBControls.Descend].Value))
                {
                    moveSpeed *= Settings.cfgCameraFlySlowMult.Value;
                }

                // Un-crouch
                if (Input.GetKeyUp(Settings.cfgKeyboard[KBControls.Descend].Value))
                {
                    characterController.height = height;

                    var pos = characterController.transform.position;
                    pos.y += height / 2;
                    characterController.transform.position = pos;
                }

                camera.transform.localPosition = new Vector3(0, characterController.height / 2, 0);
            }

            // Movement
            inputDeltaMove = Vector3.zero;
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.MoveForward].Value))
            {
                inputDeltaMove += camera.transform.forward;
            }
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.MoveBackward].Value))
            {
                inputDeltaMove -= camera.transform.forward;
            }
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.MoveLeft].Value))
            {
                inputDeltaMove -= camera.transform.right;
            }
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.MoveRight].Value))
            {
                inputDeltaMove += camera.transform.right;
            }
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.Ascend].Value))
            {
                inputDeltaMove += Vector3.up;
            }
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.Descend].Value))
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
            // Gravity
            if (characterController.isGrounded)
            {
                verticalVelocity = -0.2f; // properly updates the isGrounded variable
            }
            else
            {
                verticalVelocity -= gravitationalAccel;
            }

            // Jump
            if (Input.GetKey(Settings.cfgKeyboard[KBControls.Ascend].Value) && characterController.isGrounded)
                verticalVelocity += jumpVel;

            // Movement
            Vector3 moveDelta = inputDeltaMove;
            moveDelta.y = 0;
            moveDelta.Normalize();

            characterController.Move((moveSpeed * moveDelta + new Vector3(0, verticalVelocity)) * Time.unscaledDeltaTime);
        }

        private void Look()
        {
            // Look
            float mouseX = Input.GetAxis("Horizontal") * (Settings.cfgMouseYawFlip.Value ? -1 : 1);
            float mouseY = Input.GetAxis("Vertical") * (Settings.cfgMousePitchFlip.Value ? 1 : -1);

            if (lookMode == ControlType.KBMouse) // Mouse axes don't require deltaTime
            {
                currentRotation.x = Mathf.Clamp(currentRotation.x + Settings.cfgMouseSensitivity.Value / 10 * mouseY, -89.99f, 89.99f);
                currentRotation.y += Settings.cfgMouseSensitivity.Value / 10 * mouseX;
            }
            else if (lookMode == ControlType.Controller) // Controller (needs configuration by SHIFT-starting game)
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
                switch (Settings.cfgScrollMode.Value)
                {
                    case ScrollWheelMode.MoveSpeed:
                        float newVal = Settings.cfgCameraFlySpeed.Value + 0.6f * Input.mouseScrollDelta.y;
                        if (newVal > 0)
                            Settings.cfgCameraFlySpeed.Value = newVal;
                        break;
                    case ScrollWheelMode.FieldOfView:
                        newVal = Settings.cfgCameraFov.Value + 2 * Input.mouseScrollDelta.y;
                        if (5 < newVal && newVal < 179.99)
                            Settings.cfgCameraFov.Value = newVal;
                        break;
                }
            }
        }

        private void Update()
		{
            camera.fieldOfView = Settings.cfgCameraFov.Value;

            if (Input.GetKeyDown(KeyCode.F))
                physicsEnabled = !physicsEnabled;

            // controls toggle
            if (Input.GetKeyDown(Settings.cfgKeyboard[KBControls.ToggleControls].Value))
				controlEnabled = !controlEnabled;

            // Input & movement
			if (controlEnabled)
			{
				Cursor.lockState = CursorLockMode.Locked;
                UpdateInputMovement();
                if (Input.GetKey(Settings.cfgKeyboard[KBControls.TeleportToPlayer].Value))
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
        #endregion

        private void OnDestroy()
        {
            Destroy(povCamera);

            Settings.freecamVisibleToPlayer.ValueChanged -= OnVRVisibilityChange;
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }
    }
}