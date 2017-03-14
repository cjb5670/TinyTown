using UnityEngine;
using System.Collections;

public class DrawDebugLines : MonoBehaviour
{
	public GameObject manager;
	public Material material1;
	public Material material2;
	public Vector3 thisDirection;
	


	// Use this for initialization
	void Start()
	{
		manager = GameObject.Find("GameManager");
	}

	// Update is called once per frame
	void Update()
	{
		thisDirection = manager.GetComponent<FlockManager>().averageFlockDirection;
	}

	void OnRenderObject()
	{
		// Set the material to be used for the first line
		material1.SetPass(0);

		// Draws one line
		// Average direction vector
		// Orange
		GL.Begin(GL.LINES); // Begin to draw lines
		GL.Vertex(manager.GetComponent<FlockManager>().averageFlockPosition); // First endpoint of this line
		GL.Vertex(DirectionEndpoint());
		GL.End(); // Finish drawing the line

		// Second line

		// Set another material to draw this second line in a different color

		material2.SetPass(0);
		// Average center
		// blue
		GL.Begin(GL.LINES);
		GL.Vertex(manager.GetComponent<FlockManager>().averageFlockPosition);
		GL.Vertex(manager.GetComponent<FlockManager>().averageFlockPosition + Vector3.up);
		GL.End();
	}

	Vector3 DirectionEndpoint()
	{
		return new Vector3(thisDirection.x, // x value
			(Terrain.activeTerrain.SampleHeight(new Vector3(thisDirection.x, 0, thisDirection.z))), // Terrain y value
			thisDirection.z).normalized // z value
			+
			manager.GetComponent<FlockManager>().averageFlockPosition;
	}
}
