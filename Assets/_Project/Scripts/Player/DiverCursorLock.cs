using UnityEngine;
using UnityEngine.InputSystem;

namespace TreasureDivers.Player
{
    [DisallowMultipleComponent]
    public sealed class DiverCursorLock : MonoBehaviour
    {
        [SerializeField] private bool lockOnStart = true;
        [SerializeField] private bool relockOnMouseClick = true;

        private InputAction releaseCursorAction;
        private InputAction lockCursorAction;

        public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;

        private void Awake()
        {
            releaseCursorAction = new InputAction("ReleaseCursor", InputActionType.Button, "<Keyboard>/escape");
            lockCursorAction = new InputAction("LockCursor", InputActionType.Button, "<Mouse>/leftButton");
        }

        private void OnEnable()
        {
            releaseCursorAction.Enable();
            lockCursorAction.Enable();

            if (lockOnStart)
            {
                LockCursor();
            }
        }

        private void OnDisable()
        {
            releaseCursorAction.Disable();
            lockCursorAction.Disable();
            ReleaseCursor();
        }

        private void OnDestroy()
        {
            releaseCursorAction.Dispose();
            lockCursorAction.Dispose();
        }

        private void Update()
        {
            if (releaseCursorAction.WasPressedThisFrame())
            {
                ReleaseCursor();
            }
            else if (relockOnMouseClick && lockCursorAction.WasPressedThisFrame() && !IsLocked)
            {
                LockCursor();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && lockOnStart)
            {
                LockCursor();
            }
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ReleaseCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
