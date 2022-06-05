using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ScreenShakeScript : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource big;
    [SerializeField] CinemachineImpulseSource small;
    // Start is called before the first frame update


    public void BigShake()
    {
        big.GenerateImpulse();
    }
    public void SmallShake()
    {
        small.GenerateImpulse();
    }
}
