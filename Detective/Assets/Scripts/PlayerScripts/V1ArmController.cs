using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1ArmController : MonoBehaviour
{
    //Shoulder Indicator is the empty gameObject that references where the shoulder on the player sprite is
    [SerializeField] Transform shoulderIndicator;
    [SerializeField] Transform backShoulderIndicator;
    //Used to visualize the angle in the editor
    [SerializeField] float angle;
    [SerializeField] float distance;

    private V1GameManager gm;
    private Animator anim;
    private Camera cam;
    private SpriteRenderer sr;
    private void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<V1GameManager>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        RotateArm();
    }
    void Update()
    {
        MoveShoulder();
    }

    void RotateArm()
    {
        //Finds the angle to the mouse and changes the angle accordingly
        //Also sets the animation variable
        Vector3 worldPosMouse = cam.ScreenToWorldPoint(Input.mousePosition);
        angle = 180*(1/Mathf.PI) * Mathf.Atan2(worldPosMouse.y - shoulderIndicator.position.y, worldPosMouse.x - shoulderIndicator.position.x);
        anim.SetFloat("Angle", angle);

        
        worldPosMouse.z = 0; //for the purpose of calculating distance properly
        distance = Vector3.Distance(worldPosMouse, gm.player.transform.position);
        anim.SetFloat("Distance", distance);
       
    }
    void MoveShoulder()
    {
        //Moves this object to the correct shoulder location
        if (gm.player.transform.localScale.x == 1)
        {
            transform.position = shoulderIndicator.transform.position;
            sr.sortingLayerName = "Shoulder";
        }
        else if(gm.player.transform.localScale.x == -1)
        {
            transform.position = backShoulderIndicator.transform.position;
            sr.sortingLayerName = "BackShoulder";
        }
    }

}
