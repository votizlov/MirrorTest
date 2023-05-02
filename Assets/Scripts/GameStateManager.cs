using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameStateManager : NetworkRoomManager
{
    public WinScreen winScreen;
    public static GameStateManager Instance;
    [SerializeField] private GameConfig _gameConfig;
    private List<PlayerScore> _playerScores;

    bool showStartButton;

    public override void Awake()
    {
        base.Awake();
        ShuffleStartPositions(); //randomize roundRobin spawn strategy
        Instance = this;
        _playerScores = new List<PlayerScore>();
        Cursor.lockState = CursorLockMode.Confined;
        Application.targetFrameRate = 60;
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer,
        GameObject gamePlayer)
    {
        var charController = gamePlayer.GetComponent<CharacterController>();
        charController.dashDistance = _gameConfig.DashLength;
        charController.tagDuration = _gameConfig.TaggedDuration;
        var scoreView = gamePlayer.GetComponent<PlayerScore>();
        scoreView.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
        scoreView.OnScoreUpdated.AddListener(OnScoreUpdated);
        _playerScores.Add(scoreView);
        return true;
    }

    public override void OnRoomServerPlayersReady()
    {
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
        showStartButton = true;
#endif
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
        {
            showStartButton = false;

            ServerChangeScene(GameplayScene);
        }
    }

    //matchLogic

    private void OnScoreUpdated(PlayerScore score)
    {
        Debug.Log("checkScore");
        if (score.score > _gameConfig.DashCounttoWin && !winScreen.showWinner)
        {
            winScreen.winnerName = score.index.ToString();
            StartCoroutine(WinScreenCoroutine());
        }
    }

    private IEnumerator WinScreenCoroutine()
    {
        winScreen.showWinner = true;
        yield return new WaitForSeconds(5);
        winScreen.showWinner = false;
        RestartMatch();
    }

    private void RestartMatch()
    {
        ShuffleStartPositions();
        foreach (var playerScore in _playerScores)
        {
            playerScore.score = 0;
            playerScore.GetComponent<CharacterController>().Move(GetStartPosition().position);
        }
    }
}