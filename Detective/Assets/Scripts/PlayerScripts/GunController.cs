using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    #region Variables
    [Header("Gun Stats")]
    [SerializeField] float bulletSpeed;
    [SerializeField] float fireRate; //delay between shots
    [SerializeField] float bulletSpread;
    private float nextAvailableFireTime = 0;
    [Space(2)]
    [Header("Object References")]
    [SerializeField] GameObject normalBullet;
    [SerializeField] Transform gunTipIndicator;

    #endregion
    private void Update()
    {
        ShootingController();
    }
    void ShootingController()
    {
        /* When the Mouse is clicked (TO BE CHANGED)
         * Get the Mouse Position and rotate it with a random spread
         * Then Draw a raycast to simulate shooting
         * Then Call coroutine to draw the tracer
         */
        if (Input.GetMouseButton(0))
        {
            //IF not enough time has passed, return
            if (Time.time < nextAvailableFireTime)
            {
                return;
            }

            Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosMouse.z = 0;

            //Gunshot Sound
            FMODUnity.RuntimeManager.PlayOneShot("event:/Characters/Player/Pistol", GetComponent<Transform>().position);

            //Creating New endpoint with spread
            float spread = Random.Range(-bulletSpread / 2, bulletSpread / 2);
            Vector3 worldPosMouseWithSpread = worldPosMouse - gunTipIndicator.position; //the relative vector from P2 to P1.
            worldPosMouseWithSpread = Quaternion.Euler(0, 0, spread) * worldPosMouseWithSpread; //rotatate
            worldPosMouseWithSpread = gunTipIndicator.position + worldPosMouseWithSpread; //bring back to world space


            //Generating the Bullet
            GameObject bullet = Instantiate(normalBullet);
            bullet.transform.position = gunTipIndicator.position;
            bullet.GetComponent<Rigidbody2D>().velocity = (worldPosMouseWithSpread - gunTipIndicator.position).normalized * bulletSpeed;
           

            //Screenshake
            //gm.screenShake.SmallShake();

            //New Last Shot Time
            nextAvailableFireTime = Time.time+fireRate;
        }
    }

}
