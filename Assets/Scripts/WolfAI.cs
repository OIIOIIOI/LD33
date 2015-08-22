using UnityEngine;
using System.Collections;

public class WolfAI : MonoBehaviour {
	
	Rigidbody2D RB;

	float maxSpeed = 5f;

	GameObject currentPrey;

	// Use this for initialization
	void Start () {
		RB = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

		GameObject[] preys = GameObject.FindGameObjectsWithTag ("Prey");
		if (preys.Length == 0)
			return;

		float shortestDist = float.MaxValue;
		GameObject closestPrey = preys [0];

		foreach (GameObject p in preys)
		{
			// Get dir for raycast
			Vector2 dir = p.transform.position - this.transform.position;
			// Get all colliders on the way
			RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, dir);
			bool hitWall = false;
			foreach (RaycastHit2D hit in hits) {
				// If hit a wall, stop
				if (hit.collider.tag == "Wall") {
					hitWall = true;
					break;
				}
				// If hit target, stop
				else if (hit.collider.gameObject == p) {
					break;
				}
			}
			// Hit a wall, ignore prey
			if (hitWall)	continue;
			// Didn't hit a wall, check if prey is closer
			float dist = Vector2.Distance(this.transform.position, p.transform.position);
			if (dist < shortestDist) {
				shortestDist = dist;
				closestPrey = p;
			}
		}
		// First time assign
		if (currentPrey == null) {
			currentPrey = closestPrey;
		}
		// Keep current if distance difference is not so big
		else if (closestPrey != currentPrey) {
			float d1 = Vector2.Distance(this.transform.position, closestPrey.transform.position);
			float d2 = Vector2.Distance(this.transform.position, currentPrey.transform.position);
			//Debug.Log (closestPrey.name + ": " + d1 + " / " + currentPrey.name + ": " + d2);
			if (d1 < d2 - 2f)	currentPrey = closestPrey;
		}

		Vector2 vel = this.RB.velocity + Pursuit ();
		vel *= 0.9f;
		this.RB.velocity = Vector2.ClampMagnitude (vel, this.maxSpeed);

		float angle = Mathf.Atan2(this.RB.velocity.y, this.RB.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	Vector2 Pursuit ()
	{
		Rigidbody2D preyRB = currentPrey.GetComponent<Rigidbody2D> ();

		Vector2 toPrey = currentPrey.transform.position - this.transform.position;
		float relativeHeading = Vector2.Dot (this.transform.right, currentPrey.transform.right);
		if (Vector2.Dot(toPrey, this.transform.right) > 0 && relativeHeading < -0.95) {
			return Seek (currentPrey.transform.position);
		}
		float lat = toPrey.magnitude / (this.maxSpeed + preyRB.velocity.magnitude);
		return Seek((Vector2)currentPrey.transform.position + preyRB.velocity * lat);
	}

	Vector2 Seek (Vector2 targetPos)
	{
		Vector2 vel = targetPos - (Vector2)this.transform.position;
		vel.Normalize ();
		vel *= this.maxSpeed;
		return vel - this.RB.velocity;
	}

}










