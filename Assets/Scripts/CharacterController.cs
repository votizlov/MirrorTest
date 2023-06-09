using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CharacterController : NetworkBehaviour
{
    [SyncVar] public float dashDistance = 10;
    [SyncVar] public float dashTime = 0.5f;
    [SyncVar] public float tagDuration = 3;
    public GameObject dashEffectPrefab;
    [HideInInspector] public Vector3 movementVector;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] private PlayerScore score;
    //states
    [SyncVar] private bool isDashing = false;
    [SyncVar] public bool isTagged = false;

    public void Dash()
    {
        if (isDashing) return;
        SpawnDashEffect();
        StartCoroutine(DashCoroutine());
    }

    [Command]
    private void SpawnDashEffect()
    {
        GameObject temp = Instantiate(dashEffectPrefab, transform.position, Quaternion.LookRotation(-movementVector),transform);
        NetworkServer.Spawn(temp);
    }

    [ClientRpc]
    public void Tag()
    {
        if (isTagged)
            return;
        isTagged = true;
        _renderer.material.color = Color.red;
        StartCoroutine(UntagCoroutine());
    }

    private IEnumerator UntagCoroutine()
    {
        yield return new WaitForSeconds(tagDuration);
        _renderer.material.color = Color.white;
        isTagged = false;
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        Vector3 startPosition = _rigidbody.transform.position;
        Vector3 targetPosition = startPosition + movementVector.normalized * dashDistance;
        float remaningTime = 0;
        while (remaningTime < dashTime)
        {
            _rigidbody.MovePosition(Vector3.Lerp(startPosition, targetPosition, remaningTime / dashTime));
            remaningTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move(Vector3 position)
    {
        _rigidbody.MovePosition(position);
    }

    private void Move()
    {
        if (isDashing) return;

        movementVector = movementVector.normalized * 10 * Time.fixedDeltaTime;

        _rigidbody.MovePosition(_rigidbody.transform.position + movementVector);
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Player"))
        {
            var t = collision.gameObject.GetComponent<CharacterController>();
            if (t.isTagged)
                return;
            t.Tag();
            score.score++;
        }
    }
}