using UnityEngine;
using UnityEngine.InputSystem;

namespace TreasureDivers.Player
{
    [DisallowMultipleComponent]
    public sealed class DiverInputReader : MonoBehaviour
    {
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction swimDownAction;
        private InputAction interactAction;

        public Vector2 Move => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public Vector2 LookDelta => lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool AscendHeld => jumpAction != null && jumpAction.IsPressed();
        public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPressedThisFrame();
        public bool SwimDownHeld => swimDownAction != null && swimDownAction.IsPressed();
        public bool InteractPressedThisFrame => interactAction != null && interactAction.WasPressedThisFrame();

        private void Awake()
        {
            CreateActions();
        }

        private void OnEnable()
        {
            moveAction?.Enable();
            lookAction?.Enable();
            jumpAction?.Enable();
            swimDownAction?.Enable();
            interactAction?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
            lookAction?.Disable();
            jumpAction?.Disable();
            swimDownAction?.Disable();
            interactAction?.Disable();
        }

        private void OnDestroy()
        {
            moveAction?.Dispose();
            lookAction?.Dispose();
            jumpAction?.Dispose();
            swimDownAction?.Dispose();
            interactAction?.Dispose();
        }

        private void CreateActions()
        {
            moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            moveAction.AddBinding("<Gamepad>/leftStick");

            lookAction = new InputAction("Look", InputActionType.Value, expectedControlType: "Vector2");
            lookAction.AddBinding("<Mouse>/delta");
            lookAction.AddBinding("<Gamepad>/rightStick");

            jumpAction = new InputAction("JumpOrSwimUp", InputActionType.Button, "<Keyboard>/space");
            jumpAction.AddBinding("<Gamepad>/buttonSouth");

            swimDownAction = new InputAction("SwimDown", InputActionType.Button, "<Keyboard>/leftCtrl");
            swimDownAction.AddBinding("<Keyboard>/rightCtrl");
            swimDownAction.AddBinding("<Gamepad>/buttonEast");

            interactAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
            interactAction.AddBinding("<Gamepad>/buttonNorth");
        }
    }
}
