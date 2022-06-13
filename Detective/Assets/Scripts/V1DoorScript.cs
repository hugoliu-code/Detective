using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1DoorScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform spawnLocation;
    [SerializeField] GameObject spawnEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(spawnEnemy, spawnLocation);
        }
    }
}
