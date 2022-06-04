using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1ShootingScript: MonoBehaviour
{
    private LayerMask shootLayers;
    public LayerMask enemy;
    public LayerMask floor;

    [Header("Tracers")]
    [SerializeField] GameObject mainShot;
    [SerializeField] float firstTracerTime;
    [SerializeField] float thickness;
    [SerializeField] GameObject secondShot;
    [SerializeField] float secondTracerTime;
    void Start()
    {
        shootLayers = enemy | floor;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosMouse.z = 0;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, worldPosMouse - transform.position, 100, shootLayers);
            if (hit && hit.collider.gameObject.CompareTag("Enemy"))
                Destroy(hit.collider.gameObject);



            StartCoroutine(Tracer(transform.position,
                180 * (1 / Mathf.PI) * Mathf.Atan2(worldPosMouse.y - transform.position.y, worldPosMouse.x - transform.position.x),
            10f));
            //Debug.DrawRay(transform.position, worldPosition - transform.position);
        }
    }

    IEnumerator Tracer(Vector3 startPos, float angle, float distance)
    {
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
