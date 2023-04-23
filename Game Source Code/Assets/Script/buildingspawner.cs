using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class buildingspawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] buildingPrefabs;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log(typeof(buildingPrefabs));
            int randBuilding = UnityEngine.Random.Range(0, buildingPrefabs.Length);
            int randSpawnPoint = UnityEngine.Random.Range(0, spawnPoints.Length);

            GameObject newObject = Instantiate(buildingPrefabs[randBuilding], spawnPoints[0].position, transform.rotation);
        }
    }
}
