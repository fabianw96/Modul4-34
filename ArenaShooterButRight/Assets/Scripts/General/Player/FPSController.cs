using System;
using System.Collections;
using System.Collections.Generic;
using General.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{

    [SerializeField] private float raycastDistance = 1.5f;
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 6;
    [SerializeField] private float smoothMoveTime = 0.1f;
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float gravity = 18;

    [SerializeField] private bool lockCursor;
    [SerializeField] private float mouseSensitivity = 10;
    [SerializeField] private Vector2 pitchMinMax = new Vector2 (-40, 85);
    [SerializeField] private float rotationSmoothTime = 0.1f;

    private CharacterController _controller;
    private Shooter _shooter;
    private Camera _cam;
    private SpellType _chosenSpell;
    [SerializeField] private float yaw;
    [SerializeField] private float pitch;
    private float _smoothYaw;
    private float _smoothPitch;

    private float _yawSmoothV;
    private float _pitchSmoothV;
    private float _verticalVelocity;
    private Vector3 _velocity;
    private Vector3 _smoothV;
    private Vector3 _rotationSmoothVelocity;
    private Vector3 _currentRotation;

    private bool _jumping;
    private float _lastGroundedTime;
    private bool _disabled;
    
    private Vector2 _input;
    private bool _isSprintPressed;
    private bool _isJumpPressed;
    private bool _isShootPressed;
    private bool _isInteractPressed;
    private float _mX;
    private float _mY;

    void Start () {
        _cam = Camera.main;
        _shooter = GetComponent<Shooter>();
        
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        _controller = GetComponent<CharacterController> ();

        yaw = transform.eulerAngles.y;
        pitch = _cam.transform.localEulerAngles.x;
        _smoothYaw = yaw;
        _smoothPitch = pitch;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.P)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break();
        }
        if (Input.GetKeyDown (KeyCode.O)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _disabled = !_disabled;
        }
        if (_disabled) {
            return;
        }

        _mX = Input.GetAxisRaw("Mouse X");
        _mY = Input.GetAxisRaw("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _chosenSpell = SpellType.Fireball;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _chosenSpell = SpellType.Iceball;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _chosenSpell = SpellType.Electroball;
        }
        
        
        HandleLook();
        HandleShoot();
        // HandleInteraction();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        Vector3 inputDir = new Vector3 (_input.x, 0, _input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection (inputDir);

        float currentSpeed = _isSprintPressed ? runSpeed : walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        _velocity = Vector3.SmoothDamp (_velocity, targetVelocity, ref _smoothV, smoothMoveTime);

        _verticalVelocity -= gravity * Time.deltaTime;
        _velocity = new Vector3 (_velocity.x, _verticalVelocity, _velocity.z);

        var flags = _controller.Move (_velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below) {
            _jumping = false;
            _lastGroundedTime = Time.time;
            _verticalVelocity = 0;
        }
    }
    private void HandleJump()
    {
        if (_isJumpPressed) {
            float timeSinceLastTouchedGround = Time.time - _lastGroundedTime;
            if (_controller.isGrounded || (!_jumping && timeSinceLastTouchedGround < 0.15f)) {
                _jumping = true;
                _verticalVelocity = jumpForce;
            }
        }
    }
    private void HandleShoot()
    {
        if (_isShootPressed)
        {
            // _shooter.ChooseSpell(_chosenSpell);
        }
    }
    private void HandleLook()
    {
        float mMag = Mathf.Sqrt (_mX * _mX + _mY * _mY);
        if (mMag > 5) {
            _mX = 0;
            _mY = 0;
        }

        yaw += _mX * mouseSensitivity;
        pitch -= _mY * mouseSensitivity;
        pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);
        _smoothPitch = Mathf.SmoothDampAngle (_smoothPitch, pitch, ref _pitchSmoothV, rotationSmoothTime);
        _smoothYaw = Mathf.SmoothDampAngle (_smoothYaw, yaw, ref _yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * _smoothYaw;
        _cam.transform.localEulerAngles = Vector3.right * _smoothPitch;
    }
    private void HandleInteraction()
    {
        RaycastHit hitInfo = new RaycastHit();
        int layer = 1 << LayerMask.NameToLayer("Portal");
        // bool hit = Camera.main != null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, raycastDistance, layer, QueryTriggerInteraction.Ignore);
        bool hit = Camera.main != null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, raycastDistance);

        if (!hit) return;
        GameObject hitObject = hitInfo.transform.gameObject;
        if (hitObject != null && hitObject.GetComponent<IInteractable>() != null)
        {
            Debug.Log("Interaction!");
            hitObject.GetComponent<IInteractable>().Interaction();
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _input = ctx.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext ctx)
    {
        _isSprintPressed = ctx.ReadValueAsButton();
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        _isJumpPressed = ctx.ReadValueAsButton();
    }
    public void OnShoot(InputAction.CallbackContext ctx)
    {
        _isShootPressed = ctx.ReadValueAsButton();
    }
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Interacted");
            HandleInteraction();
        }
    }
}