using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1ShootingScript: MonoBehaviour
{

    public LayerMask shootLayers;

    private V1GameManager gm;


    [Header("Tracers")]
    [SerializeField] GameObject mainShot;
    [SerializeField] float firstTracerTime;
    [SerializeField] float thickness;
    [SerializeField] GameObject secondShot;
    [SerializeField] float secondTracerTime;
    [Header("AimTracers")]
    [SerializeField] GameObject aimTracer;
    private GameObject topAimTracer;
    private GameObject bottomAimTracer;
    [Header("Indicators")]
    [SerializeField] Transform gunTipIndicator;
    [Header("Variables")]
    [SerializeField] float normalSpread;
    [SerializeField] float aimSpeed;
    [SerializeField] float aimSpread;
    [SerializeField] float shootDelay;
    [SerializeField] float currentShootDelay;
    [SerializeField] float currentSpread;
    [SerializeField] float slowMoAimMultiplier; //In slow-mo, how much faster/slower does the player aim;
    [SerializeField] float minimumAimingDistance = 0; //How far must the mouse be for a shot to be fired?
    private bool canShoot = false;
    private float lastShotTime = 0;

    void Start()
    {
        //Layers the bullet raycast will hit
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<V1GameManager>();
        //TEMPORARY, probably wont be final system
        topAimTracer = Instantiate(mainShot, gunTipIndicator);
        bottomAimTracer = Instantiate(mainShot, gunTipIndicator);
    }


    void Update()
    {
        SpreadUpdate();
        ShootDelayUpdate();
        CheckDistance();
        
    }

    private void FixedUpdate()
    {
        Shoot();
        AimTracersUpdate();
    }
    private void CheckDistance() 
    {
        Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosMouse.z = 0;
        if (Vector3.Distance(worldPosMouse, gm.player.transform.position)> minimumAimingDistance)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }
    void AimTracersUpdate()
    {
        if (canShoot)
        {
            topAimTracer.SetActive(true);
            bottomAimTracer.SetActive(true);
        }
        else
        {
            topAimTracer.SetActive(false);
            bottomAimTracer.SetActive(false);
        }
        Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosMouse.z = 0;

        topAimTracer.transform.rotation = Quaternion.Euler(0, 0, 180 * (1 / Mathf.PI) * Mathf.Atan2(worldPosMouse.y - gunTipIndicator.position.y, worldPosMouse.x - gunTipIndicator.position.x) + currentSpread/2);

        bottomAimTracer.transform.rotation = Quaternion.Euler(0, 0, 180 * (1 / Mathf.PI) * Mathf.Atan2(worldPosMouse.y - gunTipIndicator.position.y, worldPosMouse.x - gunTipIndicator.position.x) - currentSpread/2);
    }
    void ShootDelayUpdate()
    {
        if(Time.timeScale < 1f)
        {
            currentShootDelay = shootDelay *Time.timeScale;
        }
        else
        {
            currentShootDelay = shootDelay;
        }
    }
    void SpreadUpdate()
    {
        //Update Spread according to various variables
        if (Input.GetMouseButton(1))
        {
            if (currentSpread > aimSpread)
            {
                if (Time.timeScale < 1f)
                {
                    currentSpread -= aimSpeed * Time.deltaTime * (1 / Time.timeScale) * slowMoAimMultiplier;
                }
                else 
                {
                    currentSpread -= aimSpeed * Time.deltaTime;
                }
            }
            else
            {
                currentSpread = aimSpread;
            }
        }
        else
        {
            currentSpread = normalSpread;
        }
    }
    void Shoot()
    {
        /* When the Mouse is clicked (TO BE CHANGED)
         * Get the Mouse Position and rotate it with a random spread
         * Then Draw a raycast to simulate shooting
         * Then Call coroutine to draw the tracer
         */
        if (Input.GetMouseButton(0) && canShoot)
        {
            Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosMouse.z = 0;

            //IF not enough time has passed, return
            if(Time.time < lastShotTime + currentShootDelay)
            {
                return;
            }

            //Gunshot Sound
            FMODUnity.RuntimeManager.PlayOneShot("event:/Characters/Player/Pistol", GetComponent<Transform>().position);

            //Creating New endpoint with spread
            float spread = Random.Range(-currentSpread / 2, currentSpread / 2);
            Vector3 worldPosMouseWithSpread = worldPosMouse - gunTipIndicator.position; //the relative vector from P2 to P1.
            worldPosMouseWithSpread = Quaternion.Euler(0,0,spread) * worldPosMouseWithSpread; //rotatate
            worldPosMouseWithSpread = gunTipIndicator.position + worldPosMouseWithSpread; //bring back to world space

            //The Raycast
            RaycastHit2D hit = Physics2D.Raycast(gunTipIndicator.position, worldPosMouseWithSpread - gunTipIndicator.position, 100, shootLayers);
            if (hit && hit.collider.gameObject.CompareTag("Enemy")) //When raycast hits an enemy
                Destroy(hit.collider.gameObject);

            //Reset Aim
            currentSpread = normalSpread;

            //Tracers
            float hitDistance = 15f;
            if (hit)
            {
                hitDistance = Vector2.Distance(hit.point, gunTipIndicator.position); 
            }
            StartCoroutine(Tracer(gunTipIndicator.position,
                180 * (1 / Mathf.PI) * Mathf.Atan2(worldPosMouseWithSpread.y - gunTipIndicator.position.y, worldPosMouseWithSpread.x - gunTipIndicator.position.x),
            hitDistance));

            //Screenshake
            //gm.screenShake.SmallShake();

            //New Last Shot Time
            lastShotTime = Time.time;
        }
    }

    IEnumerator Tracer(Vector3 startPos, float angle, float distance)
    {
        /* Using proper input, draw 2 prefabs one after the other to represent tracer lines
         */
        GameObject tempMainShot = Instantiate(mainShot);
        tempMainShot.transform.position = startPos;
        tempMainShot.transform.rotation = Quaternion.Euler(0, 0, angle);
        tempMainShot.transform.localScale = new Vector3(distance, thickness, 0);
        yield return new WaitForSeconds(firstTracerTime);
        Destroy(tempMainShot);
        yield return new WaitForSeconds(Time.deltaTime);
        GameObject tempSecondShot = Instantiate(secondShot);
        tempSecondShot.transform.position = startPos;
        tempSecondShot.transform.rotation = Quaternion.Euler(0, 0, angle);
        yield return new WaitForSeconds(secondTracerTime);
        Destroy(tempSecondShot);
        yield return new WaitForSeconds(Time.deltaTime);
    }
    
}
