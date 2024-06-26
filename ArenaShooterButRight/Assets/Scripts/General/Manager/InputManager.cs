using General.Player;
using General.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace General.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput PlayerInput;

        public Vector2 Move {get; private set;}
        public Vector2 Look {get; private set;}
        public bool Run {get; private set;}
        public bool Jump {get; private set;}
        public bool Crouch {get; private set;}
        public bool Shoot { get; private set; }
        public bool Reload { get; private set; }
        public bool Interact { get; private set; }

        private InputActionMap _currentMap;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _runAction;
        private InputAction _jumpAction;
        private InputAction _crouchAction;
        private InputAction _interactAction;
        private InputAction _reloadAction;
        private InputAction _shootAction;

        private void Awake() {
            HideCursor();
            _currentMap = PlayerInput.currentActionMap;
            _moveAction = _currentMap.FindAction("Move");
            _lookAction = _currentMap.FindAction("Look");
            _runAction  = _currentMap.FindAction("Run");
            _jumpAction = _currentMap.FindAction("Jump");
            _crouchAction = _currentMap.FindAction("Crouch");
            _interactAction = _currentMap.FindAction("Interact");
            _reloadAction = _currentMap.FindAction("Reload");
            _shootAction = _currentMap.FindAction("Shoot");
            

            _moveAction.performed += onMove;
            _lookAction.performed += onLook;
            _runAction.performed += onRun;
            _jumpAction.performed += onJump;
            _crouchAction.started += onCrouch;
            _interactAction.started += onInteraction;
            _reloadAction.performed += onReload;
            _shootAction.performed += onShoot;

            _moveAction.canceled += onMove;
            _lookAction.canceled += onLook;
            _runAction.canceled += onRun;
            _jumpAction.canceled += onJump;
            _crouchAction.canceled += onCrouch;
            _reloadAction.canceled += onReload;
            _shootAction.canceled += onShoot;
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void onMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }
        private void onLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }
        private void onRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }
        private void onJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
        }
        private void onCrouch(InputAction.CallbackContext context)
        {
            Crouch = context.ReadValueAsButton();
        }

        private void onInteraction(InputAction.CallbackContext context)
        {
            PlayerInteraction.Interact();
        }

        private void onReload(InputAction.CallbackContext context)
        {
            Reload = context.ReadValueAsButton();
        }

        private void onShoot(InputAction.CallbackContext context)
        {
            Shoot = context.ReadValueAsButton();
        }

        private void OnEnable() {
            _currentMap.Enable();
        }

        private void OnDisable() {
            _currentMap.Disable();
        }
        
    }
}
