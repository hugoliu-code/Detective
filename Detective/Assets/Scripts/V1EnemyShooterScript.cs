using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1EnemyShooterScript : MonoBehaviour
{
    private V1GameManager gm;
    [SerializeField] float detectDelay;
    [SerializeField] float shootDelay;
    [SerializeField] LayerMask shootLayers;
    [SerializeField] float detectDistance;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed;

    private float nextShootTime = 0f;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<V1GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
    }
    void FindPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, gm.player.transform.position-transform.position, detectDistance, shootLayers);
        if (hit && hit.collider.gameObject.CompareTag("Player"))
        {
            if(Time.time > nextShootTime)
            {
                nextShootTime = Time.time + detectDelay;
                StartCoroutine("Shoot");
            }
        }
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(shootDelay);
        GameObject tempBullet = Instantiate(bullet);
        tempBullet.transform.position = transform.position;
        tempBullet.GetComponent<Rigidbody2D>().velocity = (gm.player.transform.position - transform.position).normalized * bulletSpeed;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position, detectDistance);
    }
}
