using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComplexManager : MonoBehaviour
{
	public List<GameObject> points;
	public List<GameObject> pathFollowers;
	public float pathWidth;
	public float fleeStrength;
	public float nextPointSeekRadius;

	public Material material1;
	public bool drawLines;

	// Use this for initialization
	void Start()
	{
		drawLines = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (drawLines && Input.GetKeyDown(KeyCode.D))
		{
			drawLines = false;
		}

		else if (!drawLines && Input.GetKeyDown(KeyCode.D))
		{
			drawLines = true;
		}
	}

	void OnRenderObject()
	{
		// Set the material to be used for the first line
		material1.SetPass(0);
		if (drawLines)
		{
			for (int i = 0; i < points.Count; i++)
			{
				GL.Begin(GL.LINES); // Begin to draw lines
				if (i == points.Count - 1)
				{
					GL.Vertex(points[0].transform.position); // First endpoint of this line
					GL.Vertex(points[points.Count - 1].transform.position);
				}
				else
				{
					GL.Vertex(points[i].transform.position);
					GL.Vertex(points[i + 1].transform.position);
				}

				GL.End(); // Finish drawing the line
			}
		}

	}
}
