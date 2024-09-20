using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField TextMeshPro;

    private void Start()
    {
        TextMeshPro.onEndEdit.AddListener(OnEndEdit);

        TextMeshPro.text = PlayerPrefs.GetString("Player Name", "");
    }
    public void OnEndEdit(string name)
    {
        PlayerPrefs.SetString("Player Name", name);
    }
}
