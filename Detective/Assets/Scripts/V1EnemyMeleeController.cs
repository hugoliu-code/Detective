using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Written by Hugo Liu and Nikolas Simpfendorfer 2022
 * A general script for a "patrol" grounded enemy that will move side to side,
 * and attack the player when the player gets too close
 * The attack type is somewhat generalizable
 * 
 * Sets velocity for movement
 */
public class V1EnemyMeleeController : MonoBehaviour
{
    #region variables
    [Header("General Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float chaseSpeed;

    [Space(2)]
    [Header("Detection")]
    //Used to determine the location of the circlecasts used to detect obstacles and player
    [SerializeField] Vector2 wallLeftDetectionOffset;
    [SerializeField] Vector2 wallRightDetectionOffset;
    [SerializeField] Vector2 floorLeftDetectionOffset;
    [SerializeField] Vector2 floorRightDetectionOffset;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float obstacleDetectionRadius;
    private bool floorLeftDetected;
    private bool floorRightDetected;
    private bool wallLeftDetected;
    private bool wallRightDetected;
    private bool isAttacking;
    private bool isTakingDamage;
    private bool isWalking;
    private bool playerDetected;
    private bool playerWithinAttack;
    [SerializeField] float playerDetectDistance;
    [SerializeField] float playerDetectRadius;
    [SerializeField] float playerAttackDistance;
    [SerializeField] float playerAttackRadius;

    [Space(2)]
    [Header("Attack")]
    [SerializeField] float attackTime;
    [SerializeField] float attackAnticipation;
    [SerializeField] float attackForce;
    [SerializeField] float anticipationForce;
    [SerializeField] float attackDelay;
    [SerializeField] GameObject attackObject;

  

    private Animator anim;
    private Rigidbody2D rb;
    private bool isAnticipating;
    private State currentState = State.idle;

    private float lastAttackTime = 0;
    #endregion

    #region timeKeeping
    private float nextAttackTime = 0;
    private float nextChaseTime = 0;
    #endregion
    public enum State
    {
        //For animations
        idle,
        walking,
        anticipating,
        attacking,
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        StartCoroutine(PassiveMovement());
    }
    private void Update()
    {
        Looper();

        AttackLogic();


    }
    private void FixedUpdate()
    {
        MovementLogic();
    }


    private void MovementLogic()
    {
        //General Movement, the Passive Movement may stop at any time, so this makes sure it will be restarted
        if (!playerWithinAttack && !playerDetected && !isWalking && !isTakingDamage)
        {
            StartCoroutine(PassiveMovement());
        }
    }
    private void AttackLogic()
    {
        /* Attack Logic
         * If the player isnt attacking already, the method will send two circlecasts to see if the player
         * is in attacking range or chasing range. If it is in attacking range, the enemy will attempt to attack
         * If it is not in attacking range but is in chasing range, the method will stop movement to chase the player
         */
        if (isAttacking || Time.time < nextAttackTime)
        {
            return;
        }
        RaycastHit2D playerDetectCircleCast = Physics2D.CircleCast(transform.position + new Vector3(playerDetectDistance, 0, 0), playerDetectRadius, Vector2.left, playerDetectDistance * 2, playerLayer);
        RaycastHit2D playerAttackRangeCircleCast = Physics2D.CircleCast(transform.position + new Vector3(playerAttackDistance, 0, 0), playerAttackRadius, Vector2.left, playerAttackDistance * 2, playerLayer);
        playerDetected = playerDetectCircleCast;
        playerWithinAttack = playerAttackRangeCircleCast;

        //Detected Player, but not close enough to attack
        if (playerWithinAttack || playerDetected)
        {
            StopCoroutine(PassiveMovement());
            isWalking = false;
        }
        //So there is no violent twitching from stepping in and out of the detect zone
        if (playerDetected && Time.time > nextChaseTime)
        {
            nextChaseTime += 1f;
        }
        //Get information for the attack coroutine and start the attack coroutine
        if (playerWithinAttack)
        {
            Vector2 playerDirection = Vector2.right * (playerDetectCircleCast.collider.gameObject.transform.position.x - transform.position.x);
            playerDirection.Normalize();
            transform.localScale = new Vector3(-playerDirection.x, 1, 1);
            rb.velocity = new Vector2(0, rb.velocity.y);
            nextAttackTime = Time.time + attackDelay;
            //StartCoroutine(Attack(attackAnticipation, attackTime, attackForce, anticipationForce, (int)playerDirection.x));
            return;
        }

        //Chasing the player
        if (playerDetected && Time.time < nextChaseTime && !isTakingDamage)
        {
            Vector2 playerDirection = Vector2.right * (playerDetectCircleCast.collider.gameObject.transform.position.x - transform.position.x);
            playerDirection.Normalize();
            transform.localScale = new Vector3(-playerDirection.x, 1, 1);
            rb.velocity = new Vector2(playerDirection.x * chaseSpeed, rb.velocity.y);
        }


    }

    private void Looper()
    {
        //Checking for obstacles
        wallLeftDetected = Physics2D.OverlapCircle((Vector2)transform.position + wallLeftDetectionOffset, obstacleDetectionRadius, groundLayer);
        wallRightDetected = Physics2D.OverlapCircle((Vector2)transform.position + wallRightDetectionOffset, obstacleDetectionRadius, groundLayer);
        floorLeftDetected = Physics2D.OverlapCircle((Vector2)transform.position + floorLeftDetectionOffset, obstacleDetectionRadius, groundLayer);
        floorRightDetected = Physics2D.OverlapCircle((Vector2)transform.position + floorRightDetectionOffset, obstacleDetectionRadius, groundLayer);
    }
    IEnumerator PassiveMovement()
    {
        /* Will randomly move left or right or stay still for a random amount of time.
         * If an obstacle is seen, the enemy will just go in the opposite direcion for the remaining time.
         */
        isWalking = true;
        yield return new WaitForSeconds(1f);
        float nextTime = Time.time;
        float nextDetectionTime = Time.time;

        //This is used when one of the circleoverlaps for detecting ground and wall is triggered
        //0 is unmoving, 1 is detected to the left, 2 is detected to the right

        //Used for direction. 0 is unmoving, 1 is to the right, 2 is to the left
        int currentDirection = 0;
        while (!isAttacking && !playerDetected)
        {
            if (Time.time > nextTime)
            {
                nextTime += Random.Range(2f, 5f);
                currentDirection = Random.Range(0, 3);


                switch (currentDirection)
                {
                    case 0:
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        break;
                    case 1:
                        transform.localScale = new Vector3(-1, 1, 1);
                        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                        break;
                    case 2:
                        transform.localScale = new Vector3(1, 1, 1);
                        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                        break;
                }
            }
            if (Time.time > nextDetectionTime)
            {
                nextDetectionTime += 0.5f;
                if (wallLeftDetected || !floorLeftDetected)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                }
                if (wallRightDetected || !floorRightDetected)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                }
            }

            yield return new WaitForSeconds(0.01f);
        }


    }
   
    void OnDrawGizmos()
    {
        //For visualization in editor

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + wallLeftDetectionOffset, obstacleDetectionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + wallRightDetectionOffset, obstacleDetectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + floorLeftDetectionOffset, obstacleDetectionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + floorRightDetectionOffset, obstacleDetectionRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(playerDetectDistance, 0), playerDetectRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(-playerDetectDistance, 0), playerDetectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(playerAttackDistance, 0), playerAttackRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(-playerAttackDistance, 0), playerAttackRadius);
        //Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0.15f, -0.5f), 1);
    }
}
