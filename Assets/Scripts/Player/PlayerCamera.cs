using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField] private AudioListener audioSource;
    // Start is called before the first frame update
    void Start()
    {
        var NetworkObject = GetComponentInParent<NetworkObject>();
        if (!NetworkObject.HasInputAuthority)
        {
            _camera.enabled = false;
            audioSource.enabled = false;
            _camera.tag = "MainCamera";
        }
    }
}
