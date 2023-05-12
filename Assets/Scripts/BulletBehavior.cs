using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BulletBehavior : NetworkBehaviour
{
    // The settings
    [SerializeField] private float _maxLifetime = 3.0f;
    [SerializeField] private float _speed = 200.0f;
    [SerializeField] private LayerMask _asteroidLayer;

    // The countdown for a bullet lifetime.
    [Networked] private TickTimer _currentLifetime { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;

        // The network parameters get initializes by the host. These will be propagated to the clients since the
        // variables are [Networked]
        _currentLifetime = TickTimer.CreateFromSeconds(Runner, _maxLifetime);
    }

    public override void FixedUpdateNetwork()
    {
        // If the bullet has not hit an asteroid, moves forward.
        if (HasHitPlayer() == false)
        {
            transform.Translate(transform.forward * _speed * Runner.DeltaTime, Space.World);
        }
        else
        {
            Runner.Despawn(Object);
            return;
        }

        CheckLifetime();
    }

    // If the bullet has exceeded its lifetime, it gets destroyed
    private void CheckLifetime()
    {
        if (_currentLifetime.Expired(Runner) == false) return;

        Runner.Despawn(Object);
    }
    
    private bool HasHitPlayer()
    {
        var hitAsteroid = Runner.LagCompensation.Raycast(transform.position, transform.forward, _speed * Runner.DeltaTime,
            Object.InputAuthority, out var hit);

        if (hitAsteroid == false) return false;

        var charBehaviour = hit.GameObject.GetComponent<PlayerDataNetworked>();
        Debug.Log("hit");
        if (charBehaviour == null) return false;

        charBehaviour.SubtractLife();//DealDamage(Object.InputAuthority);

        return true;
    }
}
