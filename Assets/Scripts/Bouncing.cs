using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bouncing : MonoBehaviour
{

    public float frequency;
    public float magnitude;
    Vector3 initialScale;
    
    void Start()
    {
        initialScale = transform.localScale;      
    }

    void Update()
    {
        transform.localScale = initialScale * (1 + (Mathf.Sin(Time.time * frequency) * magnitude));
    }
}
