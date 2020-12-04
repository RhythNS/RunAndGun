using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform character;

    void Update()
    {
        Vector3 pos = character.position;
        pos.z = -5;
        transform.position = pos;
    }
}
