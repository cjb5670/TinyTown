using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeadAndFollowMan : MonoBehaviour
{
	public float distBehindLeader;
	public float saveZoneRadius;
	public float avoidenceStrength;
	public float safeFollowerRadius;
	public float slowDistance;
	public float safeLeaderDistance;
	public GameObject Leader;
	public List<GameObject> Followers;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
