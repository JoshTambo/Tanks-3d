using UnityEngine;
using System.Collections;
using Fusion;
using System.Collections.Generic;

public class EnemyBullet : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 moveForce;
    [SerializeField] private float timeToLive = 5;

    [Header("Damage")]
    [SerializeField] private int damage = 50;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    [Header("Particles")]
    [SerializeField] private GameObject impactParticle; // bullet impact    

    bool canExplode = false;
    float bulletTriggerDelay = 0.2f;
    float bulletTTK = 0.05f; // delay time of bullet destruction
    private PlayerRef owner;

    private Vector3 explodePos; 
    private void Start()
    {
        rb ??= GetComponent<Rigidbody>();

        rb.AddRelativeForce(moveForce, ForceMode.Impulse);
    }

    public void SetOwner(PlayerRef owner)
    {
        this.owner = owner;
    }

    private void Update()
    {
        if (canExplode == false)
        {
            bulletTriggerDelay -= Time.deltaTime;

            if (bulletTriggerDelay <= 0)
            {
                canExplode = true;
            }
        }

        timeToLive -= Time.deltaTime;

        if (timeToLive < 0)
        {
            Explode();
            timeToLive = 5;
        }

        RotateDirection();
    }

    private void RotateDirection()
    {
        Vector3 newPos = transform.position + rb.velocity;
        transform.LookAt(newPos);
    }

    // Bullet hit
    void OnTriggerEnter(Collider other)
    {
        if (canExplode)
        {
            ModifyTerrainHeight(other.GetComponent<Terrain>(),transform.position, -0.002f, 10);
            Explode();

            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, owner);
            }
        }
    }

    private void Explode()
    {
        Destroy(gameObject, bulletTTK); // destroy bullet
        impactParticle = Instantiate(impactParticle, transform.position, Quaternion.Euler(rb.velocity.x, rb.velocity.y, rb.velocity.z)) as GameObject;
        Destroy(impactParticle, 3);
    }

    void ModifyTerrainHeight(Terrain terrain, Vector3 position, float deltaHeight, float radius)
    {
        if (terrain != null)
        {
            TerrainData terrainData = terrain.terrainData;

            // Convert world position to normalized terrain coordinates
            Vector3 terrainPosition = position - terrain.transform.position;
            float normalizedX = terrainPosition.x / terrainData.size.x;
            float normalizedZ = terrainPosition.z / terrainData.size.z;

            // Convert normalized coordinates to heightmap coordinates
            int heightmapX = Mathf.RoundToInt(normalizedX * terrainData.heightmapResolution);
            int heightmapZ = Mathf.RoundToInt(normalizedZ * terrainData.heightmapResolution);

            // Define the crater radius in heightmap coordinates
            int craterRadius = Mathf.RoundToInt(5 * terrainData.heightmapResolution / terrainData.size.x);

            // Define the area to modify
            int startX = Mathf.Max(0, heightmapX - craterRadius);
            int startZ = Mathf.Max(0, heightmapZ - craterRadius);
            int endX = Mathf.Min(terrainData.heightmapResolution, heightmapX + craterRadius);
            int endZ = Mathf.Min(terrainData.heightmapResolution, heightmapZ + craterRadius);

            // Get current heights
            float[,] heights = terrainData.GetHeights(startX, startZ, endX - startX, endZ - startZ);

            // Adjust the height of each point in the area
            for (int z = 0; z < endZ - startZ; z++)
            {
                for (int x = 0; x < endX - startX; x++)
                {
                    // Calculate the distance from the center of the impact
                    float distX = (x + startX) - heightmapX;
                    float distZ = (z + startZ) - heightmapZ;
                    float distanceFromCenter = Mathf.Sqrt(distX * distX + distZ * distZ);

                    // Normalize distance and apply a Gaussian falloff for smooth transition
                    float normalizedDistance = distanceFromCenter / craterRadius;
                    if (normalizedDistance < 1.0f) // Only affect points within the crater radius
                    {
                        float falloff = Mathf.Exp(-normalizedDistance * normalizedDistance * 3.0f); // Adjust the falloff intensity

                        // Modify the height using the falloff
                        heights[z, x] = Mathf.Clamp(heights[z, x] + deltaHeight * falloff, 0, 1);
                    }
                }
            }

            // Apply the modified heights back to the terrain
            terrainData.SetHeights(startX, startZ, heights);
        }
        else
        {
            Debug.Log($"Ray hit nothing at pos {position}");
        }
    }

    private void OnDrawGizmos()
    {
        if (explodePos != Vector3.zero)
        {
            Gizmos.DrawRay(explodePos, Vector3.down * 20f); 
        }
    }
}




