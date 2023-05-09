using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using UnityEngine;

public class PlayerInputController : NetworkBehaviour
{
    public float horizontalSpeed = 2f;
    public float verticalSpeed = 2f;
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Transform _camera;
    [SerializeField] private CharacterController _characterController;
    private float _maxVerticalCamAngle = 30;
    private float _currentVecticalAngle = 0;

    public override void OnStartAuthority()
    {
        Debug.Log("StartAuthority " + gameObject.name);
        _characterController.enabled = true;
        this.enabled = true;
        _camera.gameObject.SetActive(true);
    }

    public override void OnStopAuthority()
    {
        Debug.Log("StopAuthority " + gameObject.name);
        this.enabled = false;
        _characterController.enabled = false;
        _camera.gameObject.SetActive(false);
    }

    void Update()
    {
        float h = -horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");
        if (Math.Abs(_currentVecticalAngle + v) >= _maxVerticalCamAngle)
            v = 0;
        _currentVecticalAngle += v;
        _cameraPivot.Rotate(v, h, 0);

        _camera.LookAt(_cameraPivot);

        if (Input.GetMouseButtonDown(0))
        {
            _characterController.Dash();
        }

        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");
        var t = _camera.forward * vMove + _camera.right * hMove;
        _characterController.movementVector = new Vector3(t.x, 0, t.z);
    }
}