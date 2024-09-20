using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scoreboard : SimulationBehaviour
{
    [Header("ScorePrefab")]
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private RectTransform scoreBoardParent;

    public static Scoreboard Instance;
    public Dictionary<PlayerRef, ScoreElement> scores = new Dictionary<PlayerRef, ScoreElement>();

    public RectTransform ScoreBoardParent => scoreBoardParent;

    FusionManager.NetworkCallbacks callbacks = null;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        callbacks = new FusionManager.NetworkCallbacks()
        {
            OnPlayerJoined = OnPlayerJoined,
            OnPlayerLeft = OnPlayerLeft,
        };

        FusionManager.Instance.BindCallbacks(callbacks);
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        RemovePersonFromScoreboard(player);
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Ensure all active players are added to the scoreboard
        //foreach (var _player in runner.ActivePlayers)
        //{
        //    if (!scores.ContainsKey(_player))
        //    {
        //        AddPersonToScoreboard(_player, runner);
        //    }
        //}

        if(player == FusionManager.Instance.LocalRunner.LocalPlayer)
        {
            AddPersonToScoreboard(player, runner);
        }
    }

    private void AddPersonToScoreboard(PlayerRef player, NetworkRunner runner, string name = "")
    {
        if (!player.IsRealPlayer || player.IsNone)
        {
            return;
        }

        if (!scores.ContainsKey(player))
        {
            scores[player] = null; 
            runner.Spawn(scorePrefab, Vector3.zero, Quaternion.identity, player, OnBeforeSpawnScoreElement);
        }
    }

    public void OnBeforeSpawnScoreElement(NetworkRunner runner, NetworkObject obj)
    {
        scores[obj.InputAuthority] = obj.GetComponent<ScoreElement>();

        UpdateScore(obj.InputAuthority, 0);

        obj.transform.SetParent(scoreBoardParent, false);

        if (obj.HasInputAuthority)
        {
            scores[obj.InputAuthority].SetName(PlayerPrefs.GetString("Player Name", "New Player"));
        }
    }

    private void RemovePersonFromScoreboard(PlayerRef player)
    {
        if (scores.ContainsKey(player))
        {
            GameObject score = scores[player].gameObject;
            scores.Remove(player);
            Destroy(score);
        }
    }

    public void UpdateScore(PlayerRef player, int addScore)
    {
        if (!scores.ContainsKey(player))
        {
            AddPersonToScoreboard(player, FusionManager.Instance.LocalRunner);
        }
        else if (scores[player] != null)
        {
            scores[player].AddScore(addScore);
        }
        else
            Debug.LogError($"Did not add score to player!");

        SortScores();
    }

    private void SortScores()
    {
        var sortedScores = scores.OrderByDescending(kvp =>
        {
            if (kvp.Value != null)
                return kvp.Value.GetScore;
            else
                return 0;
        }).ToList();

        sortedScores.RemoveAll((x) =>
        {
            if (x.Value == null)
                return true;

            return false; 
        }); 

        for (int i = 0; i < sortedScores.Count(); i++)
        {
            sortedScores[i].Value.gameObject.transform.SetSiblingIndex(i);
        }
    }
}
