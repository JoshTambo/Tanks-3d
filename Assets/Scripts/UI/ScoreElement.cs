using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreElement : NetworkBehaviour, ISpawned
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI nameText;

    PlayerRef owner;
    public int GetScore => score;
    [Networked, OnChangedRender(nameof(UpdateScore))] private int score { get; set; }
    private void UpdateScore()
    {
        scoreText.text = (score).ToString();
    }

    [Networked, OnChangedRender(nameof(UpdateName)), Capacity(32)] private String playerName{ get; set; }
    public void UpdateName()
    {
        nameText.text = playerName.ToString();
    }

    public override void Spawned()
    {
        base.Spawned();
        owner = Object.InputAuthority;
        Debug.Log($"Spawned score element {this.name}");
        if(owner != Runner.LocalPlayer)
        {
            transform.SetParent(Scoreboard.Instance.ScoreBoardParent, false);
            Scoreboard.Instance.scores.Add(owner, this);
            UpdateName();
            UpdateScore();
        }
    }
    
    public void SetName(String name)
    {
        playerName = name;
        UpdateName();
    }
    public void AddScore(int add)
    {
        score += add;
    }

    public void AddName(string name)
    {
        //playerName = name; 
    }
}
