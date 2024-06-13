using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowlyRotateSelf : MonoBehaviour
{
    [SerializeField] float degreesPerSecond = 2f;
    void Update()
    {
        transform.Rotate(Vector3.up * degreesPerSecond * Time.deltaTime);
    }
}
