using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float time;
    [SerializeField] float timeScale;
    private bool isSlowing;
    private float unslowTimeStamp = 0f;
    private UIManager uim;

    private void Start()
    {
        uim = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSlowing)
        {
            isSlowing = true;
            unslowTimeStamp = Time.time + time * timeScale;
        }
        if (isSlowing)
        {
            uim.UpdateSlider((unslowTimeStamp - Time.time) / (time * timeScale));
        }
        if(isSlowing && Time.time > unslowTimeStamp)
        {
            isSlowing = false;
        }
        UpdateSlow();
    }
    void UpdateSlow()
    {
        if (isSlowing)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02F;
        }
    }
}
