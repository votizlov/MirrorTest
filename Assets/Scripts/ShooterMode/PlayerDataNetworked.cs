using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// Holds the player's information and ensures it is replicated to all clients.
public class PlayerDataNetworked : NetworkBehaviour
{
    // Global static setting
    private const int STARTING_LIVES = 10;

    // Local Runtime references
    private PlayerOverviewPanel _overviewPanel = null;
    [SerializeField] public PlayerDataView playerDataView;

    // Game Session SPECIFIC Settings are used in the UI.
    // The method passed to the OnChanged attribute is called everytime the [Networked] parameter is changed.
    [HideInInspector]
    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> NickName { get; private set; }

    [HideInInspector]
    [Networked(OnChanged = nameof(OnLivesChanged))]
    public int Lives { get; private set; }

    [HideInInspector]
    [Networked(OnChanged = nameof(OnScoreChanged))]
    public int Score { get; private set; }

    public override void Spawned()
    {
        // --- Client
        // Find the local non-networked PlayerData to read the data and communicate it to the Host via a single RPC 
        if (Object.HasInputAuthority)
        {
            var nickName = FindObjectOfType<PlayerData>().GetNickName();
            RpcSetNickName(nickName);
        }

        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {
            Lives = STARTING_LIVES;
            Score = 0;
        }

        // --- Host & Client
        // Set the local runtime references.
        _overviewPanel = FindObjectOfType<PlayerOverviewPanel>();
        // Add an entry to the local Overview panel with the information of this spaceship
        //_overviewPanel.AddEntry(Object.InputAuthority, this);
    }

    // Increase the score by X amount of points
    public void AddToScore(int points)
    {
        Score += points;
    }

    // Decrease the current Lives by 1
    public void SubtractLife()
    {
        Lives--;
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }

    public static void OnNickNameChanged(Changed<PlayerDataNetworked> playerInfo)
    {
        playerInfo.Behaviour.playerDataView.UpdateName(playerInfo.Behaviour.NickName.ToString());
    }

    public static void OnScoreChanged(Changed<PlayerDataNetworked> playerInfo)
    {
        playerInfo.Behaviour.playerDataView.UpdateScore(playerInfo.Behaviour.Score);
    }

    public static void OnLivesChanged(Changed<PlayerDataNetworked> playerInfo)
    {
        playerInfo.Behaviour.playerDataView.UpdateLives(playerInfo.Behaviour.Lives);
    }
}