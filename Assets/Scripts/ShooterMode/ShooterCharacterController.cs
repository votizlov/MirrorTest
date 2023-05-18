using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShooterCharacterController : NetworkBehaviour
{
    public GameObject bulletEffectPrefab;
    [HideInInspector] public Vector3 movementVector;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] CapsuleCollider _capsuleCollider;
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] LayerMask _coinCollisionLayer;
    private PlayerDataNetworked _playerDataNetworked = null;

    [SerializeField] private Transform _bulletSource;
    private List<LagCompensatedHit> _lagCompensatedHits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        _playerDataNetworked = GetComponent<PlayerDataNetworked>();
    }

    public void Shoot()
    {
        SpawnShootEffect();
    }

    private void SpawnShootEffect()
    {
        Runner.Spawn(bulletEffectPrefab, _bulletSource.position,
            _bulletSource.rotation,Object.InputAuthority);
    }

    public override void FixedUpdateNetwork()
    {
        Move();
        if (CollectedCoin())
        {
        }
    }

    // Check asteroid collision using a lag compensated OverlapSphere
    private bool CollectedCoin()
    {
        _lagCompensatedHits.Clear();

        var count = Runner.LagCompensation.OverlapSphere(_rigidbody.position, _capsuleCollider.radius,
            Object.InputAuthority, _lagCompensatedHits, _coinCollisionLayer);

        if (count <= 0) return false;
        
        _lagCompensatedHits.SortDistance();

        var coinBehaviour = _lagCompensatedHits[0].GameObject.GetComponent<Coin>();
        if (coinBehaviour == null) return false;

        coinBehaviour.Collect(Object.InputAuthority);

        return true;
    }

    public void Move(Vector3 position)
    {
        _rigidbody.MovePosition(position);
    }

    private void Move()
    {
        if (movementVector == Vector3.zero) return;

        movementVector = movementVector.normalized * 10 * Runner.DeltaTime;

        _rigidbody.MovePosition(_rigidbody.transform.position + movementVector);
        _rigidbody.MoveRotation(Quaternion.LookRotation(movementVector.normalized));
    }
}