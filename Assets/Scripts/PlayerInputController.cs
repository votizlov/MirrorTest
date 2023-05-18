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
    private float x = 0.0f;
    private float y = 0.0f;

    public override void OnStartAuthority()
    {
        Debug.Log("StartAuthority " + gameObject.name);
        _characterController.enabled = true;
        this.enabled = true;
        _camera.gameObject.SetActive(true);
        Vector3 angles = _cameraPivot.eulerAngles;
        x = angles.y;
        y = angles.x;
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
        x += horizontalSpeed * Input.GetAxis("Mouse X");
        y += verticalSpeed * Input.GetAxis("Mouse Y");
        y = ClampAngle(y, -_maxVerticalCamAngle, _maxVerticalCamAngle);
        
        Quaternion rot = Quaternion.Euler(y,x,0);

        _cameraPivot.rotation = rot;

        if (Input.GetMouseButtonDown(0))
        {
            _characterController.Dash();
        }

        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");
        var t = _camera.forward * vMove + _camera.right * hMove;
        _characterController.movementVector = new Vector3(t.x, 0, t.z);
    }
    
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}