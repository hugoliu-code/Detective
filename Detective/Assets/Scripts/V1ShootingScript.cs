using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1ShootingScript: MonoBehaviour
{
    private LayerMask shootLayers;
    public LayerMask enemy;
    public LayerMask floor;
    private V1GameManager gm;


    [Header("Tracers")]
    [SerializeField] GameObject mainShot;
    [SerializeField] float firstTracerTime;
    [SerializeField] float thickness;
    [SerializeField] GameObject secondShot;
    [SerializeField] float secondTracerTime;
    [Header("Indicators")]
    [SerializeField] Transform gunTipIndicator;
    [Header("Variables")]
    [SerializeField] float normalSpread;
    [SerializeField] float aimSpread;
    [SerializeField] float shootDelay;
    private float currentSpread;
    private float lastShotTime = 0;

    void Start()
    {
        //Layers the bullet raycast will hit
        shootLayers = enemy | floor;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<V1GameManager>();
    }


    void Update()
    {
        SpreadUpdate();
        Shoot();
    }
    void SpreadUpdate()
    {
        //Update Spread according to various variables
        if (gm.player.isAiming)
            currentSpread = aimSpread;
        else
            currentSpread = normalSpread;
    }
    void Shoot()
    {
        /* When the Mouse is clicked (TO BE CHANGED)
         * Get the Mouse Position and rotate it with a random spread
         * Then Draw a raycast to simulate shooting
         * Then Call coroutine to draw the tracer
         */
        if (Input.GetMouseButton(0))
        {
            Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosMouse.z = 0;

            //IF not enough time has passed, return
            if(Time.time < lastShotTime + shootDelay)
            {
                return;
            }

            //Creating New endpoint with spread
            float spread = Random.Range(-currentSpread / 2, currentSpread / 2);
            Vector3 worldPosMouseWithSpread = worldPosMouse - gunTipIndicator.position; //the relative vector from P2 to P1.
            worldPosMouseWithSpread = Quaternion.Euler(0,0,spread) * worldPosMouseWithSpread; //rotatate
            worldPosMouseWithSpread = gunTipIndicator.position + worldPosMouseWithSpread; //bring back to world space

            //The Raycast
            RaycastHit2D hit = Physics2D.Raycast(gunTipIndicator.position, worldPosMouseWithSpread - gunTipIndicator.position, 100, shootLayers);
            if (hit && hit.collider.gameObject.CompareTag("Enemy")) //When raycast hits an enemy
                Destroy(hit.collider.gameObject);


            //Tracers
            StartCoroutine(Tracer(gunTipIndicator.position,
                180 * (1 / Mathf.PI) * Mathf.Atan2(worldPosMouseWithSpread.y - gunTipIndicator.position.y, worldPosMouseWithSpread.x - gunTipIndicator.position.x),
            15f));

            //Screenshake
            gm.screenShake.SmallShake();

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
