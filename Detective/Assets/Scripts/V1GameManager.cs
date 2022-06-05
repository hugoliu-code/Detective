using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1GameManager : MonoBehaviour
{
    public ScreenShakeScript screenShake;
    public V1PlayerControllerScript player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<V1PlayerControllerScript>();
        screenShake = GameObject.FindObjectOfType<ScreenShakeScript>();
    }
}
