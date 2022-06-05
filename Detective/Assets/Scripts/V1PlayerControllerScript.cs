using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1PlayerControllerScript : MonoBehaviour
{
    //Private
    private State currentState = State.idle;
    private Rigidbody2D rb;
    private Animator anim;
    [Header("Movement")]
    [SerializeField] float maxMoveSpeed = 5;
    [SerializeField] float initialMoveSpeed = 3;
    [SerializeField] float currentMoveSpeed = 3;
    [SerializeField] float accelerationSpeed = 1;
    [SerializeField] int moveDirection = -1;
    [SerializeField] float walkAnimationLimit = 4;
    [SerializeField] float aimWalkSpeed;
    [Header("Conditions")]
    [SerializeField] bool isAiming;
    



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        ShootingLogic();
        AnimationLogic();
    }
    private enum State
    {
        idle,
        walking,
        run,
        aimWalk
    }
    void AnimationLogic()
    {
        if (isAiming && currentMoveSpeed > 0.1f)
        {
            currentState = State.aimWalk;
        }
        else if (currentMoveSpeed < walkAnimationLimit && currentMoveSpeed > 0.1f)
        {
            currentState = State.walking;
        }
        else if (currentMoveSpeed > 0.1f)
        {
            currentState = State.run;
        }
        else
        {
            currentState = State.idle;
        }
        anim.SetInteger("State", (int)currentState);
    }
    void ShootingLogic()
    {
        //Change states based on input
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }
    }
    private void HorizontalMovement()
    {
        Running();
        rb.velocity = new Vector2(moveDirection * currentMoveSpeed, rb.velocity.y);
    }
    void Running()
    {
       //With Acceleration
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection = -1;
            transform.localScale = new Vector3(-1, 1, 1);
            if (rb.velocity.x > -initialMoveSpeed+0.1f)
            {
                currentMoveSpeed = initialMoveSpeed;
            }
            if (currentMoveSpeed < maxMoveSpeed)
            {
                currentMoveSpeed += accelerationSpeed * Time.deltaTime;
            }
            else
            {
                currentMoveSpeed = maxMoveSpeed;
            }

            if (isAiming)
            {
                currentMoveSpeed = aimWalkSpeed;
            }

        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDirection = 1;
            transform.localScale = new Vector3(1, 1, 1);
            if (rb.velocity.x < initialMoveSpeed-0.1f)
            {
                currentMoveSpeed = initialMoveSpeed;
            }
            if (currentMoveSpeed < maxMoveSpeed)
            {
                currentMoveSpeed += accelerationSpeed * Time.deltaTime;
            }
            else
            {
                currentMoveSpeed = maxMoveSpeed;
            }
            if (isAiming)
            {
                currentMoveSpeed = aimWalkSpeed;
            }
        }
        else
        {
            moveDirection = 0;
            currentMoveSpeed = 0;
        }
    }


}
