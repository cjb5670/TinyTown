using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flocker : SeekingAndFleeing
{
	public GameObject mainManager;
	public FlockManager flockData;
	public List<GameObject> obst;

	// Use this for initialization
	public void Start()
	{
		mainManager = GameObject.Find("Managers");
		flockData = mainManager.GetComponentInChildren<FlockManager>();
		obst = flockData.buildList;
	}

	protected override void CalculateSteeringForces()
	{
		// Staying in bounds
		if (transform.position.x >= Terrain.activeTerrain.terrainData.size.x - safeZone || transform.position.z >= Terrain.activeTerrain.terrainData.size.x - safeZone || transform.position.x < 0 + safeZone || transform.position.z + safeZone < 0)
		{
			var vecX = Terrain.activeTerrain.terrainData.size.x / 2;
			// Gets a random y
			var vecZ = Terrain.activeTerrain.terrainData.size.z / 2;
			// Sticks it at the terrain height
			float vecY = Terrain.activeTerrain.SampleHeight(new Vector3(vecX, 0, vecZ));

			ApplyForce(Seek(new Vector3(vecX, vecY, vecZ)));
		}
		// other logic
		else
		{
			// Applies WanderingForce
			ApplyForce(Wandering() * flockData.wanderStrength);
			// Applies Alignment force
			ApplyForce(Seek(Align()) * flockData.alignmentStrength);
			// Applies Cohesion force
			ApplyForce(Seek(Cohesion()) * flockData.coherenceStrength);
			// Gives this unit more room if it needs it
			if (Vector3.Distance(gameObject.transform.position, flockData.findNearest(gameObject)) < flockData.separationStrength)
			{
				ApplyForce(Seperation());
			}
			foreach (GameObject obs in obst)
			{
				ApplyForce(AvoidObstacle(obs) * flockData.avoidenceStrength);
			}

		}
	}

	/// <summary>
	/// Matches forward with other forward vectors
	/// </summary>
	/// <returns>The desired Velocity minus the current velocity</returns>
	Vector3 Align()
	{
		return flockData.averageFlockDirection - velocity;
	}

	/// <summary>
	/// Seeks the center of the crowd
	/// </summary>
	/// <returns>The seeking force of the center of the crowd</returns>
	Vector3 Cohesion()
	{
		return flockData.averageFlockPosition;
	}

	/// <summary>
	/// makes sure no unit is too close to another
	/// </summary>
	/// <returns>A fleeing force from it's nearest neighbor</returns>
	Vector3 Seperation()
	{
		return Flee(flockData.findNearest(gameObject));
	}

	Vector3 AvoidObstacle(GameObject obsToCheck)
	{
		Vector3 vecToC = obsToCheck.GetComponent<CharacterController>().center - GetComponent<CharacterController>().center;
		float radiusObs = (obsToCheck.GetComponent<Renderer>().bounds.size.x + obsToCheck.GetComponent<Renderer>().bounds.size.z) / 2;
		float radiusFlock = (GetComponent<Renderer>().bounds.size.x + GetComponent<Renderer>().bounds.size.z) / 2;
		float radiiSum = radiusObs + radiusFlock;
		float forwardDot = transform.forward.x * vecToC.x + transform.forward.z * vecToC.z;
		float rightDot = transform.right.x * vecToC.x + transform.right.z * vecToC.z;

		Vector3 desiredVelocity = Vector3.zero;

		if (Vector3.Distance(GetComponent<CharacterController>().center, obsToCheck.GetComponent<CharacterController>().center) < flockData.obsticleAvoidanceDistance)
			return Vector3.zero;

		else if (forwardDot < 0)
			return Vector3.zero;

		else if (radiiSum < rightDot)
			return Vector3.zero;

		else if (rightDot > 0)
			desiredVelocity = -transform.right * maxSpeed;

		else if (rightDot < 0)
			desiredVelocity = transform.right * maxSpeed;

		return desiredVelocity - GetComponent<SeekingAndFleeing>().velocity;
		
	}


}
