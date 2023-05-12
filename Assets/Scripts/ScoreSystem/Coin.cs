
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    [SerializeField] private int _points = 1;
    
    public void Collect(PlayerRef player)
    {
        // The asteroid hit only triggers behaviour on the host and if the asteroid had not yet been hit.
        if (Object == null) return;
        if (Object.HasStateAuthority == false) return;
        
        // If this hit was triggered by a projectile, the player who shot it gets points
        // The player object is retrieved via the Runner.
        
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject))
        {
            playerNetworkObject.GetComponent<PlayerDataNetworked>().AddToScore(_points);
            
            Runner.Despawn(Object);
        }
    }
}
