﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

	// lifetime of the bullet
	public float lifetime = 3.0f;

	// damage the bullet does if it hits an active object
	public int damage = 10;

	// bullet speed
	public int speed = 15;

    public Transform sparkPrefab;

    private int killed;

	// update the bullet
	void FixedUpdate ()
    {
		lifetime -= Time.deltaTime;
		
		if(lifetime <= 0.0f) {
			Destroy(gameObject);
		}
		
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
        saveAll();
	}
	
	void OnCollisionEnter(Collision other)
    {
		// check if the other object is a player/enemy - hurt it if it is
		if(other.gameObject.tag == "Enemy")
        {
			// deduct it's health if it has the EnemyHealth component
			if(other.gameObject.GetComponent<EnemyHealth>() != null)
            {
                killed++;
				other.gameObject.GetComponent<EnemyHealth>().shot(damage);
			}
           
		}
		
		// kill the bullet as soon as we hit the other object
		if(other.gameObject.tag == "Environment" || other.gameObject.tag == "Enemy")
        {
            Debug.Log("hit something");
			Destroy(gameObject);

            // fire spark particle at the point of contact
            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            Transform spark = (Transform)Instantiate(sparkPrefab, pos, rot);
		}
	}


    public void saveAll()
    {
        PlayerPrefs.SetInt("Killed", killed);
    }
}
