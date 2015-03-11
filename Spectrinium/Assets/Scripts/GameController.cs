﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

public class GameController : MonoBehaviour
{
    //public GUIText currentColor;

    public Text currentColor;
    public static GameController Instance;

	// available wavelengths
	public enum Wavelength { RED, GREEN, BLUE };
	
	// tracks the current wavelength
	private Wavelength currentWavelength;
	
	private Map map;
	public GameObject wall;

    public int mapDimensionX = 16;
    public int mapDimensionY = 16;

    private EnemyManager enemyManager;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
		// set the start wavelength
		currentWavelength = Wavelength.BLUE;
        map = new Map(Map.GenerateMapArray(mapDimensionX, mapDimensionY), wall);

        NavMeshBuilder.BuildNavMesh();

        enemyManager = GetComponent<EnemyManager>();
        enemyManager.AssignFloors(Map.floor_group);
        enemyManager.SpawnEnemies();

		/*
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            GameObject enemyObject = enemyObjects[i];
            EnemyAI enemy = enemyObject.GetComponent<EnemyAI>();
            enemy.SetupNavMeshAgent();
        }
		*/

        UpdateLayers();
	}


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)) 
        {
            if (nextWavelength())
            {
                UpdateLayers();
            }

            Debug.Log("Current Wavelength: " + getCurrentWavelengthAsString());
            currentColor.text = getCurrentWavelengthAsString();
        }
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            if(prevWavelength())
            {
                UpdateLayers();
            }

            Debug.Log("Current Wavelength: " + getCurrentWavelengthAsString());
            currentColor.text = getCurrentWavelengthAsString();

        }
	}
	
	/**
	 * Returns the current wavelength's int value
	 */
	public int getCurrentWavelength()
	{
		return (int)currentWavelength;
	}
	
	/**
	 * Returns the current wavelength as a string e.g. "RED"
	 */
	public string getCurrentWavelengthAsString()
	{
		return currentWavelength.ToString();
	}
	
	/**
	 * Moves the current wavelength one step forward.
	 */
	public bool nextWavelength()
	{
        if (Wall.CanSwitch(currentWavelength.Next().ToString()))
        {
            currentWavelength = currentWavelength.Next();
            return true;
        }
        else if(Wall.CanSwitch(currentWavelength.Next().Next().ToString()))
        {
            currentWavelength = currentWavelength.Next().Next();
            return true;
        }
        else
            return false;
	}
	
	/**
	 * Moves the current wavelength one step back
	 */
	public bool prevWavelength()
	{
        if (Wall.CanSwitch(currentWavelength.Prev().ToString()))
        {
            currentWavelength = currentWavelength.Prev();
            return true;
        }
        else if (Wall.CanSwitch(currentWavelength.Prev().Prev().ToString()))
        {
            currentWavelength = currentWavelength.Prev().Prev();
            return true;
        }
        else
            return false;
	}



    private void SetPlayerLayer()
    {
        string wavString = getCurrentWavelengthAsString();
        string layerName = wavString.Substring(0, 1).ToUpper() + wavString.Substring(1, wavString.Length - 1).ToLower();
        int layerID = LayerMask.NameToLayer(layerName);

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObjects.Length; i++)
            playerObjects[i].layer = layerID;
    }

    //toggles visibility/collidability of enemies
    //should be moved to enemy manager/similar when enemy spawner completed
    private void EnemyVisibileCollidable()
    {
        string wavString = getCurrentWavelengthAsString();
        string layerName = wavString.Substring(0, 1).ToUpper() + wavString.Substring(1, wavString.Length - 1).ToLower();
        int layerID = LayerMask.NameToLayer(layerName);

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyObjects.Length; i++)
            if (enemyObjects[i].layer == layerID)
            {
                enemyObjects[i].GetComponent<Renderer>().enabled = true;
                enemyObjects[i].GetComponent<Collider>().isTrigger = false;
            }
            else
            {
                enemyObjects[i].GetComponent<Renderer>().enabled = false;
                enemyObjects[i].GetComponent<Collider>().isTrigger = true;
            }
    }

    private void UpdateLayers()
    {

        map.UpdateVisibleCollidable();
        SetPlayerLayer();
        enemyManager.UpdateVisibleCollidable();

    }

}
