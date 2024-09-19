using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float PlayerSpeed = 2f;
    [SerializeField] private float rotationSpeed = 2f; 

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasInputAuthority == false)
        {
            return;
        }

        float moveInput = Input.GetAxis("Horizontal"); // W/S or Up/Down arrow keys
        //float turnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys

        Vector3 movement = transform.forward * moveInput * PlayerSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

       // Quaternion rotation = Quaternion.Euler(0f, turnInput * moveInput* rotationSpeed * Time.fixedDeltaTime, 0f);
        //rb.MoveRotation(rb.rotation * rotation);
    }
}
