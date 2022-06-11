using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02F;
        }
    }
}
