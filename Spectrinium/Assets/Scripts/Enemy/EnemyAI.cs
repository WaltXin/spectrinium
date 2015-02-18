﻿using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public EnemySight sight;
    public EnemyHear hearing;
    private NavMeshAgent nav;
    public EnemyShooting gun;

    public float runSpeed = 2f;
    public float walkSpeed = 1f;

    public float waitTime = 1f;
    private float chaseTimer;


	private Vector3 lastSeen;
	private float sq_distance;



    void Start()
    {
  
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            gun.Shoot();

        if (sight.playerInSight)
            if(checkInRange())
				Attack();
			else
				Chase ();
        else if (hearing.playerHeard)
            Look();
        else
            Idle();
    }

    void Attack()
    {
        Debug.Log("pew pew");
		nav.Stop ();
        Vector3 dist_vec = lastSeen - rigidbody.position;
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        Quaternion lookDir = Quaternion.LookRotation(dist_vec, up);

        transform.rotation = lookDir;
        gun.Shoot();

    }

	bool checkInRange()
	{
		lastSeen = sight.lastSeenPosition;
		Vector3 dist_vec = lastSeen - transform.position;
		sq_distance = dist_vec.sqrMagnitude;

        float attackRange = gun.fireRange;

		if (sq_distance <= attackRange * attackRange)
			return true;

		return false;
	}

    void Chase()
    {
        Debug.Log("chasing");


        if (sq_distance >= runSpeed)
            nav.destination = lastSeen;

        nav.speed = runSpeed;
    }

    void Look()
    {
        Debug.Log("looking");

        Vector3 lastHeard = hearing.lastHeardPosition;
		Vector3 diffHearing = lastHeard - transform.position;

		if (diffHearing.sqrMagnitude >= walkSpeed)
			nav.destination = lastHeard;

        nav.speed = walkSpeed;
    }

    void Idle()
    {
        Debug.Log("idle");
        nav.Stop();
    }
}
