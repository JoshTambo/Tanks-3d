using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    public delegate void OnPlayerCallback(NetworkRunner runner, PlayerRef player);
    public delegate void NetworkRunnerCallback(NetworkRunner runner);

    public class NetworkCallbacks
    {
        public OnPlayerCallback OnPlayerJoined = null; 
        public OnPlayerCallback OnPlayerLeft = null;

        public NetworkRunnerCallback OnSceneLoadDone = null; 
        public NetworkRunnerCallback OnSceneLoadStart = null; 
    }
    
    private OnPlayerCallback _OnPlayerJoined; 
    private OnPlayerCallback _OnPlayerLeft;
    private NetworkRunnerCallback _OnSceneLoadDone;
    private NetworkRunnerCallback _OnSceneLoadStart;

    [SerializeField] private NetworkRunner RunnerPrefab;

    public NetworkRunner LocalRunner = null; 
    public static FusionManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("Too Many Fusion Managers. Deleting this!"); 
            Destroy(gameObject);
        }

    }

    public NetworkRunner CreateRunner()
    {
        if(LocalRunner)
            Destroy(LocalRunner.gameObject);

        LocalRunner = Instantiate(RunnerPrefab).GetComponent<NetworkRunner>();
        DontDestroyOnLoad(RunnerPrefab);
        LocalRunner.AddCallbacks(this);

        return LocalRunner; 
    }

    public void BindCallbacks(NetworkCallbacks networkCallbacks)
    {
        if(networkCallbacks.OnPlayerJoined != null)
        {
            _OnPlayerJoined += networkCallbacks.OnPlayerJoined;
        }

        if (networkCallbacks.OnPlayerLeft != null)
        {
            _OnPlayerLeft += networkCallbacks.OnPlayerLeft;
        }

        if (networkCallbacks.OnSceneLoadDone != null)
        {
            _OnSceneLoadDone += networkCallbacks.OnSceneLoadDone;
        }

        if (networkCallbacks.OnSceneLoadStart != null)
        {
            _OnSceneLoadStart += networkCallbacks.OnSceneLoadStart;
        }
    }

    public void UnbindCallbacks(NetworkCallbacks networkCallbacks)
    {
        if (networkCallbacks.OnPlayerJoined != null)
        {
            _OnPlayerJoined -= networkCallbacks.OnPlayerJoined;
        }

        if (networkCallbacks.OnPlayerLeft != null)
        {
            _OnPlayerLeft -= networkCallbacks.OnPlayerLeft;
        }

        if (networkCallbacks.OnSceneLoadDone != null)
        {
            _OnSceneLoadDone -= networkCallbacks.OnSceneLoadDone;
        }

        if (networkCallbacks.OnSceneLoadStart != null)
        {
            _OnSceneLoadStart -= networkCallbacks.OnSceneLoadStart;
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        _OnPlayerJoined?.Invoke(runner, player);
        Debug.Log("ON PLAYER JOINED");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        _OnPlayerLeft?.Invoke(runner, player);
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        _OnSceneLoadDone?.Invoke(runner);   
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        _OnSceneLoadStart?.Invoke(runner);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
