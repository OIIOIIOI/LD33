using UnityEngine;
using System.Collections;

public class WolfPlayer : MonoBehaviour {
	
	Rigidbody2D RB;

	float maxSpeed = 5f;

	// Use this for initialization
	void Start () {
		RB = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 mov = new Vector2 ();
		mov.x = Input.GetAxis ("Horizontal");
		mov.y = Input.GetAxis ("Vertical");

		Vector2 vel = this.RB.velocity + mov;
		vel *= 0.9f;
		this.RB.velocity = Vector2.ClampMagnitude (vel, this.maxSpeed);

		float angle = Mathf.Atan2(this.RB.velocity.y, this.RB.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
