using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1GameManager : MonoBehaviour
{
    public ScreenShakeScript screenShake;
    public V2PlayerControllerScript player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<V2PlayerControllerScript>();
        screenShake = GameObject.FindObjectOfType<ScreenShakeScript>();
    }
}
