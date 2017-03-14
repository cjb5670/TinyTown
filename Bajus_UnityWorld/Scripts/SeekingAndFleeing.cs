using UnityEngine;
using System.Collections;

public abstract class SeekingAndFleeing : MonoBehaviour
{

	// Vectors for force-based movement
	public Vector3 position;
	public Vector3 direction;
	public Vector3 acceleration;
	public Vector3 velocity;

	// floats necessary for forces
	public float mass;
	public float maxSpeed;
	public bool isSeeking;
	public bool isFleeing;
	public bool isWandering;

	public float jitterValue;
	public float turnRadius;
	public float wanderAngle;
	public float safeZone;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		CalculateSteeringForces();
		UpdatePosition();
		SetTransform();
	}

	/// <summary>
	/// Set the transform component to reflect the local position vector
	/// </summary>
	public void SetTransform()
	{
		GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
		// Rotate thes vehicle based on its forward vector
		gameObject.transform.forward = direction;
	}

	/// <summary>
	/// UpdatePosition
	/// Calculate a new position for this vehicle based on incoming forces
	/// </summary>
	public void UpdatePosition()
	{
		// Grab the world position from the transform component
		position = gameObject.transform.position;

		// Step 1: Add accel to vel * time
		velocity += acceleration * Time.deltaTime;

		// Step 2: Add velocity to position
		position += velocity * Time.deltaTime;

		// Step 3: Derive a direction 
		direction = velocity.normalized;

		// Step 4: Zero out acceleration
		// (start fresh with new forces every frame)
		acceleration = Vector3.zero;
	}

	protected abstract void CalculateSteeringForces();

	/// <summary>
	/// Applies the force to the vehicle. 
	/// </summary>
	/// <param name="force">Force.</param>
	public void ApplyForce(Vector3 force)
	{
		acceleration += (force / mass);
	}


	/// <summary>
	/// Apply friction to a vehicle 
	/// </summary>
	void ApplyFriction(float coeff)
	{
		// Step 1: Get the negative velocity
		Vector3 friction = velocity * -1;

		// Step 2: Normalize it (friction is not dependent on vel mag)
		friction.Normalize();

		// Step 3: Multiply by coefficient of friction
		friction = friction * coeff;

		// Step 4: Add to acceleration
		acceleration += friction;
	}

	/// <summary>
	/// Applies any fore as a gravity force, where the result is independent of the mass
	/// </summary>
	/// <param name="force">Force.</param>
	void ApplyGravity(Vector3 force)
	{
		acceleration += force;
	}



	public Vector3 Seek(Vector3 targetPos)
	{
		// Step 1: Calculate desired velocity
		// This is the vector pointing for myself to my target
		Vector3 desiredVelocity = targetPos - position;

		// Step 2: Scale desired to the maximum speed
		//   so that I can move as quickly as possible
		desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

		desiredVelocity.Normalize();
		desiredVelocity *= maxSpeed;

		// Step 3: Calculate the steering force for seeking
		// Steering is desired - current
		Vector3 steeringForce = desiredVelocity - velocity;

		// Step 4: return the steering force so the cheicle can apply it to it's acceleration.
		return steeringForce;
	}

	Vector3 Persue(Vector3 targetPos, Vector3 targetVelocity)
	{
		// Step 1: Calculate desired velocity
		// This is the vector pointing for myself to my target
		Vector3 desiredVelocity = targetPos + targetVelocity - position;

		// Step 2: Scale desired to the maximum speed
		//   so that I can move as quickly as possible
		desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

		desiredVelocity.Normalize();
		desiredVelocity *= maxSpeed;

		// Step 3: Calculate the steering force for seeking
		// Steering is desired - current
		Vector3 steeringForce = desiredVelocity - velocity;

		// Step 4: return the steering force so the cheicle can apply it to it's acceleration.
		return steeringForce;
	}

	public Vector3 Flee(Vector3 targetPos)
	{
		// Step 1: Calculate desired velocity
		// This is the vector pointing for myself away from my target
		Vector3 desiredVelocity = -(targetPos - position);

		// Step 2: Scale desired to the maximum speed
		//   so that I can move as quickly as possible
		desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

		desiredVelocity.Normalize();
		desiredVelocity *= maxSpeed;

		// Step 3: Calculate the steering force for seeking
		// Steering is desired - current
		Vector3 steeringForce = desiredVelocity - velocity;

		// Step 4: return the steering force so the cheicle can apply it to it's acceleration.
		return steeringForce;
	}

	Vector3 Evade(Vector3 targetPos, Vector3 targetVelocity)
	{
		// Step 1: Calculate desired velocity
		// This is the vector pointing for myself away from my target
		Vector3 desiredVelocity = -(targetPos + targetVelocity - position);

		// Step 2: Scale desired to the maximum speed
		//   so that I can move as quickly as possible
		desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

		desiredVelocity.Normalize();
		desiredVelocity *= maxSpeed;

		// Step 3: Calculate the steering force for seeking
		// Steering is desired - current
		Vector3 steeringForce = desiredVelocity - velocity;

		// Step 4: return the steering force so the cheicle can apply it to it's acceleration.
		return steeringForce;
	}

	public Vector3 Wandering()
	{

		// Find value a distance multiplied by the forward
		Vector3 circleCenter = new Vector3(transform.position.x, 0, transform.position.z) + new Vector3(transform.forward.x, 0, transform.forward.z) * jitterValue;

		var x = Mathf.Cos(wanderAngle);
		var z = Mathf.Sin(wanderAngle);
		var displacement = new Vector3(x, transform.position.y, z);
		var seekPoint = circleCenter + displacement;

		wanderAngle += Random.Range(0f, 1f) * 360 - (360 * .5f);


		// Seek that point
		return Seek(seekPoint);

	}

	Vector3 Avoid(GameObject avoid, float DistanceAvoid)
	{
		Vector3 steeringForce = new Vector3(0, 0, 0);
		Vector3 CenterVector = transform.position + avoid.transform.position;

		float distance = CenterVector.magnitude;
		float ForwardDotProduct = (CenterVector.x * transform.forward.x) + (CenterVector.y * transform.forward.y);
		float RightDotProduct = (CenterVector.x * transform.right.x) + (CenterVector.y * transform.right.y);

		if (DistanceAvoid < distance)
			return steeringForce;
		else if (ForwardDotProduct < 0)
			return steeringForce;
		else if (RightDotProduct > distance)
			return steeringForce;
		else
		{
			if (RightDotProduct > 0)
			{
				steeringForce = -transform.right * maxSpeed;
			}
			else
			{
				steeringForce = transform.right * maxSpeed;
			}
			return steeringForce;
		}

	}




}