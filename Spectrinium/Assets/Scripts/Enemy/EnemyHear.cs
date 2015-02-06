﻿using UnityEngine;
using System.Collections;

public class EnemyHear : MonoBehaviour
{
    public GameObject ears;
    public bool playerHeard;
    public GameObject enemy;
    private NavMeshAgent nav;
    private SphereCollider col;

    void Start()
    {
        playerHeard = false;
        col = GetComponent<SphereCollider>();
        nav = enemy.GetComponent<NavMeshAgent>();
    }

    //rigidbody in hear sphere
    void OnTriggerStay(Collider other)
    {
        GameObject other_object = other.gameObject;

        //if the object is the player
        if (other_object.tag == "Player")
        {
            float soundPathLength = CalculateSoundPathLength(other_object.transform.position);

            if (soundPathLength < col.radius)
            {

                playerHeard = true;
                Debug.Log("i can hear the player");
                Vector3 player_position = other_object.transform.position;
                Vector3 ear_position = ears.transform.position;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject other_object = other.gameObject;

        //if the object is the player
        if (other_object.tag == "Player")
        {
            playerHeard = false;
        }
    }

    float CalculateSoundPathLength(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();

        if (nav.enabled)
            nav.CalculatePath(targetPos, path);

        Vector3[] wayPoints = new Vector3[path.corners.Length + 2];

        wayPoints[0] = transform.position;
        wayPoints[wayPoints.Length - 1] = targetPos;

        for (int i = 0; i < path.corners.Length; i++)
            wayPoints[i + 1] = path.corners[i];

        float pathLength = 0.0f;

        for(int i=0; i<wayPoints.Length-1; i++)
        {
            float dist = Vector3.Distance(wayPoints[i], wayPoints[i+1]);
            pathLength += dist;
        }

        return pathLength;
    }
}