using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject deadUI; 
    [SerializeField] private TextMeshProUGUI deadUITxt; 

    public static MainMenu Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void ShowDeadUI(bool isDead)
    {
        if(isDead)
        {
            StartCoroutine(DeadUITimer());
        }
        else
        {
            deadUI.SetActive(false);
        }
    }

    private IEnumerator DeadUITimer(int time =0)
    {
        if(time < 5)
        {
            deadUI.SetActive(true);
            deadUITxt.text = $"{time}"; 
            time++; 
            DeadUITimer(time);
            yield return new WaitForSeconds(1);
        }
        else
        {
            deadUI.SetActive(false);    
            StopAllCoroutines();
            yield return null;
        }
    }
}
