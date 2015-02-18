﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	// available wavelengths
	private enum Wavelength { RED, GREEN, BLUE };
	
	// tracks the current wavelength
	private Wavelength currentWavelength;
	
	private Map map;
	
	// create a 3x6 map
	private int[,,] map_source = {
		{ {0,0,1}, {0,0,0}, {0,0,0} },
		{ {0,1,0}, {1,0,0}, {0,1,0} },
		{ {1,0,0}, {0,0,1}, {0,1,0} },
		{ {0,1,0}, {1,0,0}, {0,1,0} },
		{ {0,0,1}, {0,0,1}, {0,1,0} },
		{ {1,0,0}, {0,1,0}, {0,0,0} }
	};
	
	// create a 4x4 map with a red wall at each extent
	private int[,,] extents = {
		{ {1,0,0}, {0,0,0}, {0,0,0}, {1,0,0} },
		{ {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0} },
		{ {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0} },
		{ {1,0,0}, {0,0,0}, {0,0,0}, {1,0,0} }
	};
	
	// basic 3x3 map
	private int[,,] simple = {
		{ {1,0,0}, {0,0,0}, {0,1,0} },
		{ {1,0,0}, {0,0,0}, {0,0,1} },
		{ {1,0,0}, {0,0,0}, {0,1,0} }
	};
	
	// create a 1x3 map - using a traditional, c-style array
	private int[][][] jagged_extents = new int[][][] {
		new int[][] {new int[] {1, 0, 0 }, new int[] {0, 0, 0 }, new int[] {0, 1, 0 } },
		new int[][] {new int[] {1, 0, 0 }, new int[] {0, 0, 0 }, new int[] {0, 0, 1 } },
		new int[][] {new int[] {1, 0, 0 }, new int[] {0, 0, 0 }, new int[] {0, 1, 0 } },
	};

	// Use this for initialization
	void Start () {
		// set the start wavelength
		currentWavelength = Wavelength.BLUE;
		
		// create the map
		//map = new Map(map_source);
		//map = new Map(extents);
		map = new Map(simple);
		//map = new Map(jagged_extents);
		Debug.Log ("Map created.");
		
		// spawn the player
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T)) {
			nextWavelength();
			Debug.Log("Current Wavelength: " + getCurrentWavelengthAsString());
		}
		if(Input.GetKeyDown(KeyCode.R)) {
			prevWavelength();
			Debug.Log("Current Wavelength: " + getCurrentWavelengthAsString());
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
	public void nextWavelength()
	{
		currentWavelength = currentWavelength.Next();
	}
	
	/**
	 * Moves the current wavelength one step back
	 */
	public void prevWavelength()
	{
		currentWavelength = currentWavelength.Prev();
	}
}
