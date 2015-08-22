using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreyAI : MonoBehaviour {

	public GameObject wolf;
	Rigidbody2D wolfRB;

	Rigidbody2D RB;

	float maxSpeed = 3.5f;

	// Use this for initialization
	void Start ()
	{
		wolfRB = wolf.GetComponent<Rigidbody2D>();

		this.RB = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector2 vel = this.RB.velocity + Evade ();
		vel *= 0.9f;
		this.RB.velocity = Vector2.ClampMagnitude (vel, this.maxSpeed);

		float angle = Mathf.Atan2(this.RB.velocity.y, this.RB.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	Vector2 Evade ()
	{
		Vector2 toWolf = wolf.transform.position - this.transform.position;
		float lat = toWolf.magnitude / (this.maxSpeed + wolfRB.velocity.magnitude);
		return Flee((Vector2)wolf.transform.position + wolfRB.velocity * lat);
	}

	Vector2 Flee (Vector2 targetPos)
	{
		Vector2 vel = (Vector2)this.transform.position - targetPos;
		vel.Normalize ();
		vel *= this.maxSpeed;
		return vel - this.RB.velocity;
	}
	
}














