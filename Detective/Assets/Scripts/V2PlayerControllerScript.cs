using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2PlayerControllerScript : MonoBehaviour
{
    //Private
    private State currentState = State.idle;
    private Rigidbody2D rb;
    private Animator anim;
    [Header("Movement")]
    [SerializeField] float maxMoveSpeed = 9;
    [SerializeField] float initialMoveSpeed = 5;
    [SerializeField] float currentMoveSpeed = 3;
    [SerializeField] float accelerationSpeed = 2;
    [SerializeField] float reverseMoveSpeed = 4;
    [SerializeField] float minMouseDistance = 1f;
    public int moveDirection = -1; //Movement of Playerbody
    public int mouseDirection = -1; //Position of mouse relative to player
    [SerializeField] float walkAnimationLimit = 4;
    [SerializeField] float aimWalkSpeed;
    [Header("Jumping")]
    [SerializeField] Vector2 bottomOffset;
    [SerializeField] float collisionRadius;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpSpeed;
    [SerializeField] float lowJumpMultiplier;
    private bool jumpBool = false;
    [Header("Conditions")]
    public bool isAiming;
    public bool onGround;
    //[SerializeField] bool isFacingRight;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        ScaleLogic(); //Changing the LocalScale accordingly based on mouse position
        HorizontalMovement();
        VerticalMovement();
        CheckGround();
    }
    void Update()
    {
        AnimationLogic();
    }
    private enum State
    {
        idle,
        walking,
        run,
        reverseWalk,
        jump
    }
    void CheckGround()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
    }
    void ScaleLogic()
    {
        //Make the player always face the mouse
        Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (worldPosMouse.x < transform.position.x)
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
        anim.SetFloat("Yvelocity", rb.velocity.y);
        if(Mathf.Abs(rb.velocity.y) > 0.1f && !onGround)
        {
            currentState = State.jump;
        }
        else if(mouseDirection != moveDirection && currentMoveSpeed > 0.1f)
        {
            currentState = State.reverseWalk;
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

            if (rb.velocity.x > -1*initialMoveSpeed + 0.1f) {
                currentMoveSpeed = initialMoveSpeed;
   
            }
            if (moveDirection != mouseDirection)
            {
                currentMoveSpeed = reverseMoveSpeed;
            }
            else
            { 
                Accelerate();
            }


        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDirection = 1;

            if (rb.velocity.x < initialMoveSpeed - 0.1f)
            {
                currentMoveSpeed = initialMoveSpeed;
            }
            if (moveDirection != mouseDirection)
                currentMoveSpeed = reverseMoveSpeed;
            else
                Accelerate();
        }
        else
        {
            moveDirection = 0;
            currentMoveSpeed = 0;
        }
    }
    void Accelerate()
    {
        //Accelerate the CurrentMoveSpeed variable
        if (currentMoveSpeed < maxMoveSpeed)
        {
            currentMoveSpeed += accelerationSpeed * Time.deltaTime;
        }
        else
        {
            currentMoveSpeed = maxMoveSpeed;
        }
    }
    private void VerticalMovement()
    {
        if (Input.GetKeyDown(KeyCode.W)&&onGround)
        {
            jumpBool = true;
        }
        if (onGround && jumpBool)
        {
            jumpBool = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.UpArrow))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
    }

}
