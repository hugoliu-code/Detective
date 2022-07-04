using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneFailsafe : MonoBehaviour
{
    [SerializeField] private GameObject dest;
    [SerializeField] private GameObject play;

    // Start is called before the first frame update
    void Start()
    {
        dest = GameObject.Find("PlayerOffset");
        play = GameObject.Find("PlayerBody");
    }


    // Update is called once per frame
    void Update()
    {
        if (play.transform.localScale[0] == 1f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (play.transform.localScale[0] == -1f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        Failsafe();
    }

    void Failsafe()
    {
        //checks the distance betwee the drone and destination
        Transform destTransform = dest.transform;
        Vector2 position = destTransform.position;
        //Debug.Log(Vector2.Distance(position, transform.position));
        if (Vector2.Distance(position, transform.position) > 15.0)
        {
            transform.position = position;
        }
    }
}
