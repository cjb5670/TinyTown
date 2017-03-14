using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockManager : MonoBehaviour
{
	// Data to give to class
	FlockManager manager;
	GameObject mainManager;
	public float wanderStrength;
	public float separationStrength;
	public float alignmentStrength;
	public float coherenceStrength;
	public float avoidenceStrength;
	public float obsticleAvoidanceDistance;
	public List<GameObject> flockerList;
	public List<GameObject> buildList;

	public GameObject FlockCenter;

	// Data to get from class
	public Vector3 averageFlockDirection;
	public Vector3 averageFlockPosition;

	void Start()
	{
		mainManager = GameObject.Find("Managers");
		manager = mainManager.GetComponent<FlockManager>();
	}

	// Update is called once per frame
	void Update()
	{

		CalcAverages();
		// Pass in that object's transform

	}

	/// <summary>
	/// Gets a random location on the selected plane
	/// </summary>
	/// <returns></returns>
	private Vector3 OnScreenLoc()
	{
		// Gets a random x
		int vecX = Random.Range(0, (int)Terrain.activeTerrain.terrainData.size.x);
		// Gets a random y
		int vecZ = Random.Range(0, (int)Terrain.activeTerrain.terrainData.size.z);
		// Sticks it at the terrain height
		float vecY = Terrain.activeTerrain.SampleHeight(new Vector3(vecX, 0, vecZ));

		return new Vector3(vecX, vecY, vecZ);
	}

	/// <summary>
	/// Finds the average direction and position of all the flockers
	/// </summary>
	private void CalcAverages()
	{
		Vector3 directionSum = Vector3.zero;
		Vector3 locationSum = Vector3.zero;
		foreach (GameObject flocker in flockerList)
		{
			directionSum += flocker.GetComponent<SeekingAndFleeing>().direction;
			locationSum += flocker.transform.position;
		}

		averageFlockDirection = directionSum.normalized * flockerList[0].GetComponent<SeekingAndFleeing>().maxSpeed;
		averageFlockPosition = locationSum / flockerList.Count;
		FlockCenter.transform.position = averageFlockPosition;

	}

	/// <summary>
	/// Finds the nearest flocker to the parameter
	/// </summary>
	/// <param name="searchFrom">The game object you want to find the nearest neighbor to</param>
	/// <returns>The transform position of the nearest flocker</returns>
	public Vector3 findNearest(GameObject searchFrom)
	{
		var minDistance = 10000000f;
		var index = 0;
		// Keep track of the lowest distance and the index of the lowest distance
		for (int i = 0; i < flockerList.Count; i++)
		{
			// Calculate distance between each zombie and human
			var distance = Vector3.Distance(flockerList[i].GetComponentInChildren<SkinnedMeshRenderer>().bounds.center, searchFrom.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
			// Compare distance to lowest distance
			if (distance < minDistance && distance > 0.01)
			{
				// if lower, set to lowestDistance and set lowest index to index.
				minDistance = distance;
				index = i;
			}
		}
		return flockerList[index].transform.position;
	}
}
