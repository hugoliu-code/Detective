using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1ArmController : MonoBehaviour
{
    [SerializeField] Transform shoulderIndicator;
    [SerializeField] float angle;

    private Animator anim;
    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        MoveShoulder();
        RotateArm();
    }

    void RotateArm()
    {
        Vector3 worldPosMouse = cam.ScreenToWorldPoint(Input.mousePosition);
        angle = 180*(1/Mathf.PI) * Mathf.Atan2(worldPosMouse.y - shoulderIndicator.position.y, worldPosMouse.x - shoulderIndicator.position.x);
        anim.SetFloat("Angle", angle);
    }
    void MoveShoulder()
    {
        transform.position = shoulderIndicator.transform.position;
    }

}
