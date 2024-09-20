using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private GameObject scorePrefab;

    public static PlayerSpawner Instance;
    private FusionManager.NetworkCallbacks callbacks = null;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        callbacks = new FusionManager.NetworkCallbacks
        {
            OnSceneLoadDone = OnSceneLoadDone
        };
            
        FusionManager.Instance.BindCallbacks(callbacks);
    }

    private void OnSceneLoadDone(NetworkRunner runner)
    {
        SpawnNewPlayer(runner, runner.LocalPlayer);
    }

    public void SpawnNewPlayer(NetworkRunner runner,PlayerRef localPlayer, bool isDead = false)
    {
        var posindex = Random.Range(0, spawnPositions.Length);
        NetworkObject obj = runner.Spawn(PlayerPrefab, spawnPositions[posindex].position, spawnPositions[posindex].rotation, localPlayer);
        Runner.SetPlayerObject(localPlayer, obj);
        //Scoreboard.Instance.AddLocalPlayerToBoard(Runner, localPlayer);

        //Runner.Spawn(scorePrefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer, OnBeforeSspawnScoreElement);
    }
}
