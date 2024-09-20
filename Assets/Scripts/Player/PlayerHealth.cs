using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))] private int health { get; set; } = 100;

    [SerializeField] private GameObject deathParticles; 
    [SerializeField] private GameObject body; 

    [Header("Variables")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int hps = 2;

    [Header("Components")]
    [SerializeField] private TankHUD tankHud;

    private Rigidbody rb; 

    private void Start()
    {
        tankHud.Init(maxHealth);

        health = maxHealth;
        timer = hps; 
    
        rb = GetComponent<Rigidbody>();
    }

    private void OnHealthChanged()
    {
        tankHud.OnHealthChanged(health); 

        if(health <= 0)
        {
            Die();
        }
    }

    private float timer; 

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            health -= 10; 
        }

        timer += Time.deltaTime; 
        if(timer > 1 ) 
        {
            timer = 0;
            
            if(health < maxHealth) 
                health += hps; 
        }
    }
    private void Die()
    {
        Debug.Log("Die");

        Instantiate(deathParticles, transform.position, Quaternion.identity, transform.root.parent);
        StartCoroutine(Respawn());
        body.SetActive(false); 
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(5);
        if (Object.HasStateAuthority)
        {
            Debug.Log("Live");

            PlayerSpawner.Instance.SpawnNewPlayer(Runner, Object.InputAuthority, true);
            Runner.Despawn(Object); 
        }
        else
        {
            Debug.Log("No state Auth");
        }
    }

    public void TakeDamage(int damage, PlayerRef owner)
    {
        Scoreboard.Instance.UpdateScore(owner, damage); 
        
        if(Object.HasStateAuthority)
        {
            health -= damage;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(Object.HasStateAuthority)
        {
            if (collision.rigidbody == null) return; 

            if(collision.rigidbody.velocity.magnitude > rb.velocity.magnitude)
            {
                TakeDamage(25, collision.gameObject.GetComponent<NetworkObject>().StateAuthority);
            }
        }
    }
}
