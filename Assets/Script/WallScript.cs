using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    public static float Haba = 0.5f;
    void Start()
    {
        float ScreenHalfHeight = Camera.main.orthographicSize;
        float ScreenHalfWidth = ScreenHalfHeight*Camera.main.aspect;
        transform.localScale = new Vector3(transform.localScale.x*ScreenHalfWidth*2 + PersonSpawner.spawnRange*2,transform.localScale.y*ScreenHalfHeight*2*Haba,transform.localScale.z);
    }

    void Update()
    {
        
    }
}
