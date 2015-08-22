using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreyAI : MonoBehaviour {

	public GameObject wolf;

	public GameObject walls;
	PolygonCollider2D wallsColl;
	List<Vector2> wallsPoints;

	public GameObject[] feelers;

	Rigidbody2D wolfRB;

	Rigidbody2D RB;

	float maxSpeed = 3.5f;

	Vector2 intPoint;

	// Use this for initialization
	void Start ()
	{
		wolfRB = wolf.GetComponent<Rigidbody2D>();

		wallsColl = walls.GetComponent<PolygonCollider2D> ();
		wallsPoints = new List<Vector2> ();
		for (int i = 0; i < wallsColl.GetPath (0).Length; i++) {
			wallsPoints.Add(wallsColl.GetPath (0)[i]);
		}
		wallsPoints.Add(wallsColl.GetPath (0)[0]);

		this.RB = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector2 vel = this.RB.velocity + Evade ();
		//vel = vel + WallAvoid();
		vel *= 0.9f;
		this.RB.velocity = Vector2.ClampMagnitude (vel, this.maxSpeed);

		float angle = Mathf.Atan2(this.RB.velocity.y, this.RB.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	Vector2 Evade ()
	{
		Vector2 toWolf = wolf.transform.position - this.transform.position;
		float lat = toWolf.magnitude / (this.maxSpeed + wolfRB.velocity.magnitude);
		//float lat = 1f;
		return Flee((Vector2)wolf.transform.position + wolfRB.velocity * lat);
	}

	Vector2 Flee (Vector2 targetPos)
	{
		Vector2 vel = (Vector2)this.transform.position - targetPos;
		vel.Normalize ();
		vel *= this.maxSpeed;
		return vel - this.RB.velocity;
	}

	Vector2 WallAvoid ()
	{
		Vector2 force = Vector2.zero;
		float distToIP = 0f;
		float distToClosestIP = float.MaxValue;
		int closestWallPoint = -1;
		Vector2 closestIP = Vector2.zero;

		foreach (GameObject f in feelers)
		{
			for (int i = 0; i < wallsPoints.Count - 1; i++)
			{
				bool intersect = LineIntersection((Vector2)this.transform.position, (Vector2)f.transform.position,
				                                  wallsPoints[i], wallsPoints[i + 1]);
				//Debug.Log(intersect);
				if (intersect) {
					Debug.Log (intPoint);
					distToIP = Vector2.Distance((Vector2)this.transform.position, intPoint);
					if (distToIP < distToClosestIP) {
						distToClosestIP = distToIP;
						closestWallPoint = i;
						closestIP = intPoint;
					}
				}
			}
			//Debug.Log("----");

			if (closestWallPoint >= 0)
			{
				Vector2 overShoot = (Vector2)f.transform.position - closestIP;
				overShoot *= 4f;

				Vector2 p1 = wallsPoints[closestWallPoint];
				Vector2 p2 = wallsPoints[closestWallPoint + 1];
				Vector2 dir = Vector2.zero;
				if (Mathf.Abs(p1.x - p2.x) > Mathf.Abs(p1.y - p2.y)) {
					dir = Vector2.up;
					if (p1.y > this.transform.position.y)	dir = Vector2.down;
				} else {
					dir = Vector2.right;
					if (p1.x > this.transform.position.x)	dir = Vector2.left;
				}
				
				force = dir * overShoot.magnitude;
			}
		}

		return force;
	}

	bool LineIntersection (Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
	{
		//Debug.Log ("CHECK " + a1 + " / " + a2 + " / " + b1 + " / " + b2);

		float firstLineSlopeX, firstLineSlopeY, secondLineSlopeX, secondLineSlopeY;
		
		firstLineSlopeX = a2.x - a1.x;
		firstLineSlopeY = a2.y - a1.y;
		
		secondLineSlopeX = b2.x - b1.x;
		secondLineSlopeY = b2.y - b1.y;
		
		float s, t;
		s = (-firstLineSlopeY * (a1.x - b1.x) + firstLineSlopeX * (a1.y - b1.y)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
		t = (secondLineSlopeX * (a1.y - b1.y) - secondLineSlopeY * (a1.x - b1.x)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
		
		if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
		{
			float intersectionPointX = a1.x + (t * firstLineSlopeX);
			float intersectionPointY = a1.y + (t * firstLineSlopeY);
			
			// Collision detected
			intPoint = new Vector2(intersectionPointX, intersectionPointY);

			return true;
		}

		intPoint = Vector2.zero;
		return false; // No collision
	}
	
}














