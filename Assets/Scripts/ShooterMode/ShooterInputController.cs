using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShooterInputController : NetworkBehaviour
{
    public float horizontalSpeed = 2f;
    public float verticalSpeed = 2f;
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Transform _camera;
    [SerializeField] private ShooterCharacterController _characterController;
    private float _maxVerticalCamAngle = 30;
    private float _currentVecticalAngle = 0;
    private FloatingJoystick _floatingJoystick;
    [Networked] private NetworkButtons _buttonsPrevious { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            OnStartAuthority();
        }
        else
        {
            OnStopAuthority();
        }
    }
    public void OnStartAuthority()
    {
        Debug.Log("StartAuthority " + gameObject.name);
        _characterController.enabled = true;
        this.enabled = true;
        _floatingJoystick = FloatingJoystick.instance;
        _floatingJoystick.shootBtn.onClick.AddListener(Shoot);
        _camera = Camera.main.transform;
    }

    public void OnStopAuthority()
    {
        Debug.Log("StopAuthority " + gameObject.name);
        this.enabled = false;
        _characterController.enabled = false;
        //_camera.gameObject.SetActive(false);
    }

    void Shoot()
    {
        _characterController.Shoot();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<PlayerInput>(out var input) == false) return;
        if (input.Buttons.WasPressed(_buttonsPrevious, SpaceshipButtons.Fire))
        {
            Shoot();
            _buttonsPrevious = input.Buttons;
        }
        float hMove = horizontalSpeed * _floatingJoystick.Horizontal;
        float vMove = verticalSpeed * _floatingJoystick.Vertical;
        var t = _camera.forward * vMove + _camera.right * hMove;
        _characterController.movementVector = new Vector3(t.x, 0, t.z);
    }

    void Update()
    {
        _camera.LookAt(_cameraPivot);

    }
}
