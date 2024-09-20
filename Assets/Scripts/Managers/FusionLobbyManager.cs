using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

public class FusionLobbyManager : SimulationBehaviour
{
    private FusionManager.NetworkCallbacks callbacks = null;

    private NetworkRunner RunnerPrefab;

    [SerializeField] private int loadSceneIndex; 

    // Start is called before the first frame update
    void Start()
    {
        callbacks = new FusionManager.NetworkCallbacks
        {
            OnPlayerJoined = OnPlayerJoined,
            OnPlayerLeft = OnPlayerLeft
        };

        FusionManager.Instance.BindCallbacks(callbacks);

    }

    private void OnDisable()
    {
        FusionManager.Instance.UnbindCallbacks(callbacks);
    }

    public void StartGameButton()
    {
        RunnerPrefab = FusionManager.Instance.CreateRunner(); 

        Task task = StartGame(); 
    }

    public async Task StartGame()
    {
        Debug.Log($"Starting Game Lobby");

        NetworkSceneInfo networkSceneInfo = new NetworkSceneInfo();
        networkSceneInfo.AddSceneRef(SceneRef.FromIndex(loadSceneIndex), UnityEngine.SceneManagement.LoadSceneMode.Single, UnityEngine.SceneManagement.LocalPhysicsMode.Physics3D);

        var result = await RunnerPrefab.StartGame(new StartGameArgs
        {
            // ...
            // Arguments related to the Matchmaking between peers
            GameMode = GameMode.Shared,
            SessionName = "New Game",
            SessionProperties = new Dictionary<string, SessionProperty>(),
            CustomLobbyName = "",
            EnableClientSessionCreation = true,
            PlayerCount = 5,
            IsOpen = true,
            OnGameStarted = OnGameStarted,
            
            Scene = networkSceneInfo,
            IsVisible = true,
            MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.FillRoom,
        });;

        if (result.Ok)
        {
            Debug.Log($"Started Game Lobby");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    private void OnGameStarted(NetworkRunner runner)
    {
        OnPlayerJoined(runner, runner.LocalPlayer); 
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("On Player Joined");
    }
}
