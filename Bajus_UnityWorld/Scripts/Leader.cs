using UnityEngine;
using System.Collections;

public class Leader : SeekingAndFleeing
{
	// Varibles for this object
	public Vector3 followerSeekPoint;
	// Varibles for collection
	GameObject mainManager;
	LeadAndFollowMan manager;
	// Collected Varibles
	float distanceWeight;

	// Use this for initialization
	void Start()
	{
		mainManager = GameObject.Find("Managers");
		manager = mainManager.GetComponentInChildren<LeadAndFollowMan>();
		distanceWeight = manager.distBehindLeader;
	}

	// Update is called once per frame
	protected override void CalculateSteeringForces()
	{
		// Staying in bounds
		if (transform.position.x >= Terrain.activeTerrain.terrainData.size.x - safeZone || transform.position.z >= Terrain.activeTerrain.terrainData.size.x - safeZone || transform.position.x < 0 + safeZone || transform.position.z + safeZone < 0)
		{
			var vecX = Terrain.activeTerrain.terrainData.size.x / 2;
			
			var vecZ = Terrain.activeTerrain.terrainData.size.z / 2;

			ApplyForce(Seek(new Vector3(vecX, transform.position.y, vecZ)));
		}

		ApplyForce(Wandering());

		//Finds seek point for follower
		followerSeekPoint = position + -(velocity.normalized * distanceWeight);
	}
}
