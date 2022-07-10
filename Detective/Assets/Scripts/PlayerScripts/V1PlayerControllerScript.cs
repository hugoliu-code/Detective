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
    public int moveDirection = -1; //Movement of Playerbody
    public int mouseDirection = -1; //Position of mouse relative to player
    [SerializeField] float walkAnimationLimit = 4;
    [SerializeField] float aimWalkSpeed;
    [Header("Conditions")]
    public bool isAiming;
    //[SerializeField] bool isFacingRight;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ScaleLogic(); //Changing the LocalScale accordingly based on mouse position
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
    void ScaleLogic()
    {
        Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(worldPosMouse.x < transform.position.x)
        {
            mouseDirection = -1;
        }
        else
        {
            mouseDirection = 1;
        }
        transform.localScale = new Vector3(mouseDirection, 1, 1);
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
