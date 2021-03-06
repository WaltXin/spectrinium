﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpectriniumSpawner : MonoBehaviour
{
    public int numSpec;

    private GameObject spec_group;

    public GameObject specPrefab;

    private List<GameObject> floor_objects;
    private int num_floorObjects;

    private List<int> takenSpawnSpots;


    // Use this for initialization
    void Awake()
    {
        spec_group = new GameObject();
        spec_group.name = "Spectriniums";

        takenSpawnSpots = new List<int>();
    }



    public void AssignFloors(GameObject floors)
    {
        AssignFloorObjects(floors);
    }

    private void AssignFloorObjects(GameObject floor_group)
    {
        floor_objects = new List<GameObject>();
        foreach (Transform child in floor_group.transform)
        {
            floor_objects.Add(child.gameObject);
            num_floorObjects++;
        }
    }


    //NOTE may spawn on same spot
    public void SpawnSpectrinium()
    {
        for (int i = 0; i < numSpec; i++)
        {
            int randomNum;
            while (true)
            {
                randomNum = Random.Range(0, num_floorObjects);

                if(!CheckIntInList(randomNum, takenSpawnSpots))
                {
                    takenSpawnSpots.Add(randomNum);
                    break;
                }
            }
            GameObject floorObject = floor_objects[randomNum];
            Vector3 floorPos = floorObject.transform.position;
            Vector3 specPos = specPrefab.transform.position;
            specPos.x += floorPos.x;
            specPos.z = floorPos.z;
			specPos.y = 1.0f;
            GameObject specObject = (GameObject)Instantiate(specPrefab, specPos, specPrefab.transform.rotation);
            specObject.transform.parent = spec_group.transform;
            specObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    private bool CheckIntInList(int num, List<int> list)
    {
        int lengthList = list.Count;

        for (int i = 0; i < lengthList; i++)
            if (list[i] == num)
                return true;

        return false;
    }

}
