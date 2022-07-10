using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bruh");
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            Destroy(this);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Shot");
        }
    }
}
