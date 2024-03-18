using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInput;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour, IPlayerActions
{
    private PlayerInput _playerInput;
    private Vector2 _moveInput = new();
    private Vector2 _cursorLocation;

    private Rigidbody2D _rb;

    private Transform turretPivotTransform;

    public UnityAction<bool> onFireEvent;
    public UnityAction onMissileEvent;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float shipRotationSpeed = 100f;
    [SerializeField] private float turretRotationSpeed = 4f;

    public NetworkVariable<int> Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        if (_playerInput == null)
        {
            _playerInput = new();
            _playerInput.Player.SetCallbacks(this);
        }
        _playerInput.Player.Enable();

        _rb = GetComponent<Rigidbody2D>();
        turretPivotTransform = transform.Find("PivotTurret");
        if (turretPivotTransform == null) Debug.LogError("PivotTurret is not found", gameObject);
    }

    public void OnFire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onFireEvent.Invoke(true);
        }
        else if (context.canceled)
        {
            onFireEvent.Invoke(false);
        }
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        _rb.velocity = transform.up * _moveInput.y * movementSpeed;
        _rb.MoveRotation(_rb.rotation + _moveInput.x * -shipRotationSpeed * Time.fixedDeltaTime);
    }
    private void LateUpdate()
    {
        if(!IsOwner) return;
        Vector2 screenToWorldPosition = Camera.main.ScreenToWorldPoint(_cursorLocation);
        Vector2 targetDirection = new Vector2(screenToWorldPosition.x - turretPivotTransform.position.x, screenToWorldPosition.y - turretPivotTransform.position.y).normalized;
        Vector2 currentDirection = Vector2.Lerp(turretPivotTransform.up, targetDirection, Time.deltaTime * turretRotationSpeed);
        turretPivotTransform.up = currentDirection;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _cursorLocation = context.ReadValue<Vector2>();
    }

    public void IncreaseScore()
    {
        Score.Value += 1;
    }

    public void OnMissile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onMissileEvent.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StunServerRpc(float stunTime)
    {
        StunClientRpc(stunTime);
    }

    [ClientRpc]
    private void StunClientRpc(float stunTime)
    {
        ActuallyStun(stunTime);
    }

    private async void ActuallyStun(float stunTime) 
    {
        if (IsOwner)
        {
            Score.Value -= 1;
            _playerInput.Player.Disable();
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        var ogColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        await Task.Delay((int)(stunTime * 1000));

        if (IsOwner)
        {
            _playerInput.Player.Enable();
        }
        spriteRenderer.color = ogColor;
    }
}
