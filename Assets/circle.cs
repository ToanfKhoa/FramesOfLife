using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circle : MonoBehaviour
{
    // Vận tốc góc (độ mỗi giây)
    public float rotationSpeed = 90f;


    void Update()
    {
        // Quay quanh trục Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
