using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreyAI : MonoBehaviour {

	public GameObject wolf;
	Rigidbody2D wolfRB;

	Rigidbody2D RB;
	List<Vector2> feelers;

	float maxSpeed = 3.5f;

	// Use this for initialization
	void Start () {
		wolfRB = wolf.GetComponent<Rigidbody2D>();

		this.RB = this.GetComponent<Rigidbody2D>();

		this.feelers = new List<Vector2>(3);
		feelers.Add (Vector2.zero);
		feelers.Add (Vector2.zero);
		feelers.Add (Vector2.zero);
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 vel = this.RB.velocity + Evade () + AvoidWalls ();
		vel *= 0.9f;
		this.RB.velocity = Vector2.ClampMagnitude (vel, this.maxSpeed);

		float angle = Mathf.Atan2(this.RB.velocity.y, this.RB.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	Vector2 Evade () {
		Vector2 toWolf = wolf.transform.position - this.transform.position;
		float lat = toWolf.magnitude / (this.maxSpeed + wolfRB.velocity.magnitude);
		//float lat = 1f;
		return Flee((Vector2)wolf.transform.position + wolfRB.velocity * lat);
	}

	Vector2 Flee (Vector2 targetPos) {
		Vector2 vel = (Vector2)this.transform.position - targetPos;
		vel.Normalize ();
		vel *= this.maxSpeed;
		return vel - this.RB.velocity;
	}

	Vector2 AvoidWalls () {
		CreateFeelers ();
		return new Vector2 ();
	}

	void CreateFeelers () {
		feelers[0] = this.transform.position + 100 * this.transform.forward;

		Vector2 tmp = this.transform.forward;
		Quaternion q = Quaternion.AngleAxis(45, Vector3.forward);
		tmp = q * tmp;
		feelers[1] = (Vector2)this.transform.position + 50 * tmp;

		tmp = this.transform.forward;
		q = Quaternion.AngleAxis(315, Vector3.forward);
		tmp = q * tmp;
		feelers[2] = (Vector2)this.transform.position + 50 * tmp;
	}

}














