using System;
using Fusion;
using UnityEngine;

    // The SpaceshipSpawner, just like the AsteroidSpawner, only executes on the Host.
    // Therefore none of its parameters need to be [Networked].
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
    {
        // References to the NetworkObject prefab to be used for the players' spaceships.
        [SerializeField] private NetworkPrefabRef _playerNetworkPrefab = NetworkPrefabRef.Empty;

        private bool _gameIsReady = false;
        private ShooterStateManager _gameStateController = null;

        private SpawnPoint[] _spawnPoints = null;

        private void Awake()
        {
            _spawnPoints = FindObjectsOfType<SpawnPoint>();
        }

        // The spawner is started when the GameStateController switches to GameState.Running.
        public void StartPlayerSpawner(ShooterStateManager gameStateController)
        {
            _gameIsReady = true;
            _gameStateController = gameStateController;
            foreach (var player in Runner.ActivePlayers)
            {
                SpawnPlayer(player);
            }
        }

        // Spawns a new spaceship if a client joined after the game already started
        public void PlayerJoined(PlayerRef player)
        {
            Debug.Log("player joined " + player);
            if (_gameIsReady == false) return;
            SpawnPlayer(player);
        }

        private void SpawnPlayer(PlayerRef player)
        {
            // Modulo is used in case there are more players than spawn points.
            int index = player % _spawnPoints.Length;
            var spawnPosition = _spawnPoints[index].transform.position;
            var playerObject = Runner.Spawn(_playerNetworkPrefab, spawnPosition, Quaternion.identity, player);
            // Set Player Object to facilitate access across systems.
            Runner.SetPlayerObject(player, playerObject);

            // Add the new spaceship to the players to be tracked for the game end check.
            _gameStateController.TrackNewPlayer(playerObject.GetComponent<PlayerDataNetworked>().Id);
        }

        // Despawns the spaceship associated with a player when their client leaves the game session.
        public void PlayerLeft(PlayerRef player)
        {
            DespawnPlayer(player);
        }

        private void DespawnPlayer(PlayerRef player)
        {
            if (Runner.TryGetPlayerObject(player, out var spaceshipNetworkObject))
            {
                Runner.Despawn(spaceshipNetworkObject);
            }

            // Reset Player Object
            Runner.SetPlayerObject(player, null);
        }
    }
