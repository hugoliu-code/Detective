using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DroneController : MonoBehaviour
{
    [SerializeField] private GameObject dest;
    [SerializeField] private GameObject play;


    [SerializeField] Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 1;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        dest = GameObject.Find("PlayerOffset");
        play = GameObject.Find("PlayerBody");

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PathFinding();

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

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 1;
        }
    }

    void PathFinding()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (force.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
