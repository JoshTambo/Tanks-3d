using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(IsShootingChanged))] public bool isShooting { get; set; }
    [Networked] public Quaternion bulletRotation { get; set; }

    [Header("Main Shoot")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletCooldown;
    [SerializeField] private Transform turretTransform; 

    private float bulletTimer;
    private bool canShoot = false; 

    // Start is called before the first frame update
    void Start()
    {
        bulletTimer = bulletCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if(HasStateAuthority == false) return;

        if(gameObject != null)
        {
            // Rotate bullet spawn based on mouse position
            RotateTowardsMouse();
        }

        if (canShoot == false )
        {
            bulletTimer -=Time.deltaTime;
            
            if(bulletTimer < 0)
            {
                canShoot = true;
                isShooting = false; 
            }
        }

        if(Input.GetKey(KeyCode.Space) && canShoot)
        {
            isShooting = true;
            bulletRotation = bulletSpawn.rotation;
            canShoot = false;
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        float cameraDistance = bulletSpawn.position.z - Camera.main.transform.position.z;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cameraDistance));
        bulletSpawn.LookAt(mousePosition);

        //Rotate Turret
        Quaternion pos = bulletSpawn.rotation;
        turretTransform.rotation = pos; 

    }

    public void IsShootingChanged()
    {
        if (isShooting)
        {
            //Spawn bullet
            EnemyBullet enemyBullet = Instantiate(bullet, bulletSpawn.position, bulletRotation).GetComponent<EnemyBullet>();
            enemyBullet.SetOwner(Object.StateAuthority);
            
            //Start Timer 
            bulletTimer = bulletCooldown;

            Debug.Log("Shooting!");
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 mousePosition = Input.mousePosition;
        Gizmos.DrawSphere(new Vector2(mousePosition.x, mousePosition.y), 20f);
    }
}
