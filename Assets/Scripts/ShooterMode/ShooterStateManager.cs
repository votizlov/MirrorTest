using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class ShooterStateManager : NetworkBehaviour
{
    enum GameState
    {
        Starting,
        Lobby,
        Running,
        Ending
    }
    public static ShooterStateManager Instance;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private TMP_Text _winnerText;
    [SerializeField] private GameObject _lobbyText;
    private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();
    [Networked] private GameState _gameState { get; set; }

    [Networked] private NetworkBehaviourId _winner { get; set; }

    bool showStartButton;

    public void Awake()
    {
        Instance = this;
    }
    
    public override void Spawned()
    {
        // --- This section is for all information which has to be locally initialized based on the networked game state
        // --- when a CLIENT joins a game

        // If the game has already started, find all currently active players' PlayerDataNetworked component Ids
        if (_gameState != GameState.Starting)
        {
            foreach (var player in Runner.ActivePlayers)
            {
                if (Runner.TryGetPlayerObject(player, out var playerObject) == false) continue;
                TrackNewPlayer(playerObject.GetComponent<PlayerDataNetworked>().Id);
            }
        }

        _winnerText.transform.parent.gameObject.SetActive(false);
        // --- This section is for all networked information that has to be initialized by the HOST

        if (Object.HasStateAuthority == false) return;

        // Initialize the game state on the host
        _gameState = GameState.Starting;
    }
    
    public override void FixedUpdateNetwork()
    {
        // Update the game display with the information relevant to the current game state
        switch (_gameState)
        {
            case GameState.Starting:
                _playerSpawner.StartPlayerSpawner(this);
                _gameState = GameState.Lobby;
                //UpdateStartingDisplay();
                break;
            case GameState.Lobby:
                _lobbyText.SetActive(true);
                break;
            case GameState.Running:
                _lobbyText.SetActive(false);
                CheckIfGameHasEnded();
                //UpdateRunningDisplay();

                break;
            case GameState.Ending:
                if (Runner.TryFindBehaviour(_winner, out PlayerDataNetworked playerData) == false) return;
                Debug.Log("game ended");
                _winnerText.gameObject.SetActive(true);
                _winnerText.text =
                    $"{playerData.NickName} won with {playerData.Score} points. Disconnecting";

                // --- Host
                // Shutdowns the current game session.
                // The disconnection behaviour is found in the OnServerDisconnect.cs script

                if (Object.HasStateAuthority)
                {
                    Runner.Shutdown();
                }
                break;
            default:
                break;
        }
    }
/*
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer,
        GameObject gamePlayer)
    {
        //var charController = gamePlayer.GetComponent<CharacterController>();
        //charController.dashDistance = _gameConfig.DashLength;
        //charController.tagDuration = _gameConfig.TaggedDuration;
        var scoreView = gamePlayer.GetComponent<PlayerData>();
        scoreView.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
        scoreView.OnScoreUpdated.AddListener(OnScoreUpdated);
        _playerScores.Add(scoreView);
        return true;
    }*/

    //matchLogic

    private void OnHpUpdated(PlayerData data)
    {
        Debug.Log("checkHp");
        CheckIfGameHasEnded();
        /*if (data.score > _gameConfig.DashCounttoWin && !winScreen.showWinner)
        {
            winScreen.winnerName = data.index.ToString();
            StartCoroutine(WinScreenCoroutine());
        }*/
    }
    
    public void CheckIfGameHasEnded()
    {
        if (Object.HasStateAuthority == false) return;

        int playersAlive = 0;

        for (int i = 0; i < _playerDataNetworkedIds.Count; i++)
        {
            if (Runner.TryFindBehaviour(_playerDataNetworkedIds[i],
                    out PlayerDataNetworked playerDataNetworkedComponent) == false)
            {
                _playerDataNetworkedIds.RemoveAt(i);
                i--;
                continue;
            }

            if (playerDataNetworkedComponent.Lives > 0) playersAlive++;
        }

        // If more than 1 player is left alive, the game continues.
        // If only 1 player is left, the game ends immediately.
        if (playersAlive > 1) return;

        foreach (var playerDataNetworkedId in _playerDataNetworkedIds)
        {
            if (Runner.TryFindBehaviour(playerDataNetworkedId,
                    out PlayerDataNetworked playerDataNetworkedComponent) ==
                false) continue;

            if (playerDataNetworkedComponent.Lives > 0 == false) continue;

            _winner = playerDataNetworkedId;
        }

        GameHasEnded();
    }

    private void GameHasEnded()
    {
        _gameState = GameState.Ending;
    }
    
    public void TrackNewPlayer(NetworkBehaviourId playerDataNetworkedId)
    {
        Debug.Log("tracking player " + playerDataNetworkedId);
        _playerDataNetworkedIds.Add(playerDataNetworkedId);
        if (_playerDataNetworkedIds.Count > 1)
            _gameState = GameState.Running;
    }
}
