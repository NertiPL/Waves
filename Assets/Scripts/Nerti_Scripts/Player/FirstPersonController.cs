using Game.Player;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]

        /*[Header("Interaction")]
        [SerializeField] private float interactionRange = 4f;
        private Interactable focusedInteractable;*/

        [Header("Look Settings")]
        [Tooltip("Mouse sensitivity multiplier for raw Pointer/Delta input")]
        public float MouseSensitivity = 1.0f;
        [Tooltip("Gamepad sensitivity in degrees per second")]
        public float GamepadSensitivity = 120.0f;
        [Tooltip("Invert horizontal mouse axis")]
        public bool InvertMouseX = false;
        [Tooltip("Invert vertical mouse axis")]
        public bool InvertMouseY = false;
        [Tooltip("Invert horizontal gamepad axis")]
        public bool InvertPadX = false;
        [Tooltip("Invert vertical gamepad axis")]
        public bool InvertPadY = false;

        [Header("Crouch Settings")]
        [Tooltip("Standing capsule height of the CharacterController")]
        [SerializeField] private float _standingHeight = 1.8f;
        [Tooltip("Crouched capsule height of the CharacterController")]
        [SerializeField] private float _crouchHeight = 1.2f;
        [Tooltip("Camera target local Y when standing (eye height)")]
        [SerializeField] private float _cameraStandY = 1.6f;
        [Tooltip("Camera target local Y when crouched")]
        [SerializeField] private float _cameraCrouchY = 1.0f;
        [Tooltip("How fast camera and capsule blend between stand and crouch")]
        [SerializeField] private float _crouchLerpSpeed = 10f;
        [Tooltip("Move speed of the character in m/s")]
        [SerializeField] private float CrouchSpeed = 2f;
        [Tooltip("Layers checked above head to prevent standing up into geometry")]
        [SerializeField] private LayerMask _headBlockLayers = ~0;

        [Header("Movement Settings")]
        [Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

        private bool _isCrouched;
        private bool _prevCrouchEdge;

        // timeout deltatime
        private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private PlayerInputRouter _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

        EventBinding<InteractEvent> interactEventBinding;

        private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<PlayerInputRouter>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            // Trust serialized heights unless you want to read current controller
            _controller.height = _standingHeight;
            var c = _controller.center;
            c.y = _controller.height * 0.5f;
            _controller.center = c;

            // snap camera to stand height on start
            if (CinemachineCameraTarget != null)
            {
                var lp = CinemachineCameraTarget.transform.localPosition;
                lp.y = _cameraStandY;
                CinemachineCameraTarget.transform.localPosition = lp;
            }

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

        private void OnEnable()
        {

            interactEventBinding = new EventBinding<InteractEvent>(HandleInteract);
            EventBus<InteractEvent>.Register(interactEventBinding);
        }

        private void OnDisable()
        {
            EventBus<InteractEvent>.Deregister(interactEventBinding);
        }

        private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();

            HandleCrouchToggle();
            ApplyCrouchBlend();
            //HandleInteract();
        }

		private void LateUpdate()
		{
			CameraRotation();
		}

		public void ChangeRotationSpeed(float amount)
		{
			RotationSpeed = amount;
		}

        void HandleInteract(InteractEvent e)
        {
            if (e.showInteract)
            {
                if (_input.interact)
                {
                    _input.interact = false;
                    if (e.type == TypesOfInteractables.FlashBomb)
                    {
                        Destroy(e.interactableObject);
                        //event to change eq, add bomb to eq
                    }
                    else if (e.type == TypesOfInteractables.Door)
                    {
                        //animate opening of door or some shi
                    }
                    else
                    {
                        Destroy(e.interactableObject);
                        //event to change eq, add jar to eq
                    }
                }
            }
        }

       /* private void HandleInteract()
        {
            if (_mainCamera == null) return;

            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, ~0, QueryTriggerInteraction.Collide))
            {
                var interactable = hit.collider.GetComponentInParent<Interactable>();
                if (interactable != null && interactable.isActive)
                {
                    focusedInteractable = interactable;

                    if (_input.interact)
                    {
                        _input.interact = false;

                        float d = Vector3.Distance(transform.position, interactable.transform.position);
                        if (d <= interactable.interactionRadius)
                        {
                            interactable.OnInteract(this);
                        }
                    }
                    return;
                }
            }

            focusedInteractable = null;
        }*/

        private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude < _threshold) return;

            bool isMouse = IsCurrentDeviceMouse;

            // invert
            float invX = 1f, invY = 1f;
            if (isMouse)
            {
                if (InvertMouseX) invX = -1f;
                if (InvertMouseY) invY = -1f;
            }
            else
            {
                if (InvertPadX) invX = -1f;
                if (InvertPadY) invY = -1f;
            }

            // mice are raw delta, do not multiply by dt
            float dt = isMouse ? 1f : Time.deltaTime;
            float sens = isMouse ? MouseSensitivity : GamepadSensitivity;

            _cinemachineTargetPitch += (_input.look.y * invY) * sens * dt;
            _rotationVelocity = (_input.look.x * invX) * sens * dt;

            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, 0f);
            transform.Rotate(Vector3.up * _rotationVelocity);
        }

        private void Move()
		{
            // block sprint when crouched
            if (_isCrouched && _input.sprint)
                _input.sprint = false;

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // crouch always reduces to crouch multiplier and disables sprint speed
            if (_isCrouched)
                targetSpeed = CrouchSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

        private void HandleCrouchToggle()
        {
            bool edgeDown = _input.crouch && !_prevCrouchEdge;

            if (edgeDown)
            {
                if (_isCrouched)
                {
                    // try to stand up; only if head space is clear
                    if (CanStandUp())
                        _isCrouched = false;
                }
                else
                {
                    // crouch always allowed
                    _isCrouched = true;
                    // cancel sprint while crouched
                    if (_input.sprint) _input.sprint = false;
                }
            }

            _prevCrouchEdge = _input.crouch;

            // reset crouch after handling
            _input.crouch = false;
        }

        private void ApplyCrouchBlend()
        {
            float targetHeight = _isCrouched ? _crouchHeight : _standingHeight;
            float targetCamY = _isCrouched ? _cameraCrouchY : _cameraStandY;

            // capsule height and center
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, Time.deltaTime * _crouchLerpSpeed);
            var c = _controller.center;
            c.y = Mathf.Lerp(c.y, _controller.height * 0.5f, Time.deltaTime * _crouchLerpSpeed);
            _controller.center = c;

            // camera target Y (local)
            if (CinemachineCameraTarget != null)
            {
                var lp = CinemachineCameraTarget.transform.localPosition;
                lp.y = Mathf.Lerp(lp.y, targetCamY, Time.deltaTime * _crouchLerpSpeed);
                CinemachineCameraTarget.transform.localPosition = lp;
            }
        }

        private bool CanStandUp()
        {
            // define a test capsule slightly taller than standing to be safe
            float radius = _controller.radius * 0.95f;
            Vector3 basePos = transform.position + _controller.center - Vector3.up * (_controller.height * 0.5f);
            Vector3 standTop = basePos + Vector3.up * _standingHeight;

            // small inset to avoid grazing the floor
            Vector3 p1 = basePos + Vector3.up * (radius + 0.02f);
            Vector3 p2 = standTop - Vector3.up * (radius + 0.02f);

            // if anything blocks, cannot stand
            bool blocked = Physics.CheckCapsule(p1, p2, radius, _headBlockLayers, QueryTriggerInteraction.Ignore);
            return !blocked;
        }

        private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}