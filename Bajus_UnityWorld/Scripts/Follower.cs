using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Follower : SeekingAndFleeing
{
	// Varibles for this object

	// Varibles for collecting
	GameObject mainManager;
	LeadAndFollowMan manager;
	// collected varibles
	GameObject leader;
	Leader leaderData;
	List<GameObject> followers;
	float slowDistance;


	// Use this for initialization
	void Start()
	{
		mainManager = GameObject.Find("Managers");
		manager = mainManager.GetComponentInChildren<LeadAndFollowMan>();
		leader = manager.Leader;
		followers = manager.Followers;
		leaderData = leader.GetComponent<Leader>();
		slowDistance = manager.slowDistance;
	}

	// Update is called once per frame
	protected override void CalculateSteeringForces()
	{
		// avoids collisions
		if (Vector3.Distance(gameObject.transform.position, findNearest(gameObject)) < manager.safeFollowerRadius)
		{
			ApplyForce(Seperation() * manager.avoidenceStrength);
		}

		ApplyForce(Seek(leaderData.followerSeekPoint));
		ApplyForce(Arrival());
		if (Vector3.Distance(gameObject.transform.position, leader.transform.position) < manager.safeLeaderDistance)
		{
			ApplyForce(Flee(leader.transform.position));
		}

	}

	Vector3 Seperation()
	{
		return Flee(findNearest(gameObject));
	}

	public Vector3 findNearest(GameObject searchFrom)
	{
		var minDistance = 10000000f;
		var index = 0;
		// Keep track of the lowest distance and the index of the lowest distance
		for (int i = 0; i < followers.Count; i++)
		{
			// Calculate distance between each zombie and human
			var distance = Vector3.Distance(followers[i].GetComponent<MeshRenderer>().bounds.center, searchFrom.GetComponent<MeshRenderer>().bounds.center);
			// Compare distance to lowest distance
			if (distance < minDistance && distance > 0.01)
			{
				// if lower, set to lowestDistance and set lowest index to index.
				minDistance = distance;
				index = i;
			}
		}
		return followers[index].transform.position;
	}

	public Vector3 Arrival()
	{
		float distance = Vector3.Distance(gameObject.transform.position, leaderData.followerSeekPoint);
		if (distance < slowDistance)
		{
			return - velocity * (slowDistance / distance);
		}
		else return velocity;
	}


}
