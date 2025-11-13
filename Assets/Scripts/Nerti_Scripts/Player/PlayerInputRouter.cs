using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Game.Player
{
    public class PlayerInputRouter : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool attack;
        public bool crouch;
        public bool interact;

        [Header("Inventory / Slots")]
        [Tooltip("Currently selected hotbar slot (0 = slot 1, 1 = slot 2, etc.)")]
        public int selectedSlot = 0;

        [Tooltip("How many slots are available in the hotbar.")]
        public int maxSlots = 4;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        void Start()
        {
            SetCursorState(true);
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnAttack(InputValue value)
        {
            AttackInput(value.isPressed);
        }

        public void OnCrouch(InputValue value)
        {
            CrouchInput(value.isPressed);
        }

        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
        }

        public void OnEquipSlot(InputValue value)
        {
            if (!value.isPressed)
                return;

            bool changed = false;

            // Keyboard
            if (Keyboard.current != null)
            {
                if (Keyboard.current.digit1Key.wasPressedThisFrame) { selectedSlot = 0; changed = true; }
                else if (Keyboard.current.digit2Key.wasPressedThisFrame) { selectedSlot = 1; changed = true; }
                else if (Keyboard.current.digit3Key.wasPressedThisFrame) { selectedSlot = 2; changed = true; }
                else if (Keyboard.current.digit4Key.wasPressedThisFrame) { selectedSlot = 3; changed = true; }
            }

            // Gamepad RB - next, LB - previous
            if (Gamepad.current != null)
            {
                if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                {
                    selectedSlot = (selectedSlot + 1) % maxSlots;
                    changed = true;
                }
                else if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                {
                    selectedSlot = (selectedSlot - 1 + maxSlots) % maxSlots;
                    changed = true;
                }
            }

            if (changed)
            {
                selectedSlot = Mathf.Clamp(selectedSlot, 0, maxSlots - 1);

                Debug.Log($"[Input] Equip slot: {selectedSlot + 1}");
                // add for example InventoryManager.Equip(selectedSlot);
            }
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        public void CrouchInput(bool newCrouchState)
        {
            crouch = newCrouchState;
        }

        public void InteractInput(bool newInteractState)
        {
            interact = newInteractState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        public void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}