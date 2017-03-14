using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComplexPath : SeekingAndFleeing
{
	// Varibles for this object
	public GameObject Begin;
	public GameObject Next;
	Vector3 NormalBase;
	float pathWidth;
	// Varibles for collecting
	GameObject mainManager;
	ComplexManager manager;
	// collected varibles
	public List<GameObject> points;
	public List<GameObject> pathFollowers;
	public float fleeStrength;
	public int start;
	public float seekNextDistance;


	// Use this for initialization
	void Start()
	{
		mainManager = GameObject.Find("Managers");
		manager = mainManager.GetComponentInChildren<ComplexManager>();
		points = manager.points;
		pathFollowers = manager.pathFollowers;
		pathWidth = manager.pathWidth;
		fleeStrength = manager.fleeStrength;
		seekNextDistance = manager.nextPointSeekRadius;
		Begin = points[start];
		if(points.IndexOf(Begin) + 1 == points.Count)
		{
			Next = points[0];
		}
		else
		{
			Next = points[points.IndexOf(Begin) + 1];
		}
		ApplyForce(Seek(Begin.transform.position));
	}

	// Update is called once per frame
	protected override void CalculateSteeringForces()
	{
		

		

		// save position vectors
		Vector3 startPoint = Begin.transform.position;
		Vector3 endPoint = Next.transform.position;

		// Save best dot
		Vector3 bestDot = CalcDot(startPoint, endPoint);


		if (Vector3.Distance(startPoint, endPoint) > Vector3.Distance(startPoint, bestDot)) // if the dot is not past the endpoint
		{
			// Seek dot only if too far from dot
			if (Vector3.Distance(position + velocity, bestDot) > pathWidth)
			{
				ApplyForce(Seek(bestDot));
			}

		}
		else
		{
			ApplyForce(Seek(Vector3.forward));
		}

		if (Vector3.Distance(gameObject.transform.position, findNearest(gameObject)) < safeZone)
		{
			ApplyForce(Seperation() * fleeStrength);
		}

		DebugStuff();

		// updates start and next point if necessary
		if (Vector3.Distance(bestDot, endPoint) < seekNextDistance)
		{
			// Updates begin
			if (points.IndexOf(Begin) != points.Count - 1)
			{
				Begin = points[points.IndexOf(Begin) + 1];
			}
			else
			{
				Begin = points[0];
			}

			// updates Next
			if (points.IndexOf(Begin) != points.Count - 1)
			{
				Next = points[points.IndexOf(Begin) + 1];
			}
			else
			{
				Next = points[0];
			}

			//Makes sure vehicle is pointed in the correct direction
			ApplyForce(Seek(Next.transform.position));
		}


		
	}

	public Vector3 findNearest(GameObject searchFrom)
	{
		var minDistance = 10000000f;
		var index = 0;
		// Keep track of the lowest distance and the index of the lowest distance
		for (int i = 0; i < pathFollowers.Count; i++)
		{
			// Calculate distance between each zombie and human
			var distance = Vector3.Distance(pathFollowers[i].GetComponent<MeshRenderer>().bounds.center, searchFrom.GetComponent<MeshRenderer>().bounds.center);
			// Compare distance to lowest distance
			if (distance < minDistance && distance > 0.01)
			{
				// if lower, set to lowestDistance and set lowest index to index.
				minDistance = distance;
				index = i;
			}
		}
		return pathFollowers[index].transform.position;
	}

	public Vector3 CalcDot(Vector3 beginPos, Vector3 endPos)
	{
		// Vector from start of line to future position
		Vector3 strToVel = (position + velocity) - beginPos;

		// Vector from start of path to end
		Vector3 strToEnd = endPos - beginPos;

		// normalize start to end
		strToEnd.Normalize();

		// multiply by dot product between star to velocity and start to end.
		Vector3 dotValue = strToEnd * (Vector3.Dot(strToVel, strToEnd));

		// normalpoint is equal to the start plus doted
		NormalBase = beginPos + dotValue;

		return NormalBase;
	}

	Vector3 Seperation()
	{
		return Flee(findNearest(gameObject));
	}

	void DebugStuff()
	{
		//for (int i = 0; i < points.Count; i++)
		//{
		//	if (i != 0)
		//	{
		//		Debug.DrawLine(points[i].transform.position, points[i - 1].transform.position, Color.green);
		//	}
		//	else
		//	{
		//		Debug.DrawLine(points[points.Count - 1].transform.position, points[0].transform.position, Color.green);
		//	}
		//}

		//Debug.DrawLine(position + velocity, CalcDot(Begin.transform.position, Next.transform.position), Color.red);


	}

}
