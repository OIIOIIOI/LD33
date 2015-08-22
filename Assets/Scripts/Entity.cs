﻿using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	
	public Sprite wolfSprite;
	public Sprite preySprite;
	public GameObject haloPrefab;

	Rigidbody2D RB;
	
	float runSpeed;
	
	int moveTickDelay;
	int moveTick;

	int basePower;

	bool isWolf;
	bool isAI;
	bool isInvincible;

	int playerNum;

	GameObject halo;

	void Awake ()
	{
		RB = gameObject.GetComponent<Rigidbody2D>();
		
		SetIsAI (true, true);
		SetIsWolf (false, true);
		SetIsInvincible (false, true);

		playerNum = 1;
	}

	void FixedUpdate ()
	{
		// Check if activating base
		if (!isWolf)
		{
			bool activeBase = false;
			if (isAI)	activeBase = PreyBase();
			else		activeBase = PlayerBase();

			if (activeBase)
			{
				//TODO invincible is the minimum base, show base and consume no power
				basePower--;
				if (basePower <= 0)	HideBase();
				else				ShowBase();
			}
		}

		// Check if time to move and get Vector2
		Vector2 mov = Vector2.zero;
		moveTick--;
		if (moveTick == 0)
		{
			// Each of these return a normalized Vector2
			if (isAI && isWolf)			mov = GetWolfMove();
			else if (isAI && !isWolf)	mov = GetPreyMove();
			else 						mov = GetPlayerMove();
			// Apply speed
			mov *= runSpeed;
			// While not moving, keep ticking each frame
			if (mov != Vector2.zero)	moveTick = moveTickDelay;
			else						moveTick = 1;
		}

		// Change velocity
		Vector2 vel = RB.velocity + mov;
		vel *= 0.9f;
		RB.velocity = vel;
		// Adjust orientation
		float angle = Mathf.Atan2(RB.velocity.y, RB.velocity.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	// PLAYER

	Vector2 GetPlayerMove ()
	{
		Vector2 mov = Vector2.zero;

		mov.x = Input.GetAxis ("HorizontalP" + playerNum);
		mov.y = Input.GetAxis ("VerticalP" + playerNum);

		mov.Normalize ();
		return mov;
	}
	
	bool PlayerBase ()
	{
		return false;
	}

	// PREY
	
	Vector2 GetPreyMove ()
	{
		Vector2 mov = Vector2.zero;

		if (Main.instance.GetWolf () != null)
			mov = Evade (Main.instance.GetWolf ());

		mov.Normalize ();
		return mov;
	}

	bool PreyBase ()
	{
		return false;
	}

	// WOLF
	
	Vector2 GetWolfMove ()
	{
		Vector2 mov = Vector2.zero;

		GetClosestPrey ();
		mov = Pursuit (currentPrey);
		
		mov.Normalize ();
		return mov;
	}

	GameObject currentPrey;

	void GetClosestPrey ()
	{
		GameObject[] preys = GameObject.FindGameObjectsWithTag ("Prey");
		if (preys.Length == 0)
			return;

		float shortestDist = float.MaxValue;
		GameObject closestPrey = preys [0];
		
		bool currentIsHidden = false;
		
		foreach (GameObject p in preys)
		{
			Entity ent = p.GetComponent<Entity>();
			if (ent.isInvincible)	continue;
			
			// Get dir for raycast
			Vector2 dir = p.transform.position - transform.position;
			// Get all colliders on the way
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir);
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
			if (hitWall) {
				if (p == currentPrey)	currentIsHidden = true;
				continue;
			}
			// Didn't hit a wall, check if prey is closer
			float dist = Vector2.Distance(transform.position, p.transform.position);
			if (dist < shortestDist) {
				shortestDist = dist;
				closestPrey = p;
			}
		}
		// First time assign
		if (currentPrey == null || currentIsHidden) {
			currentPrey = closestPrey;
		}
		// Keep current if distance difference is not so big
		else if (closestPrey != currentPrey) {
			float d1 = Vector2.Distance(transform.position, closestPrey.transform.position);
			float d2 = Vector2.Distance(transform.position, currentPrey.transform.position);
			//Debug.Log (closestPrey.name + ": " + d1 + " / " + currentPrey.name + ": " + d2);
			if (d1 < d2 - 2f)	currentPrey = closestPrey;
		}
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (!isWolf)	return;

		if (coll.collider.tag == "Prey")
		{
			Entity ent = coll.collider.gameObject.GetComponent<Entity>();
			//Debug.Log ("wolf (" + gameObject.name + ", " + isInvincible + ") collided with prey (" + coll.collider.gameObject.name + ", " + ent.IsInvincible() + ")");
			if (ent.IsInvincible())	return;
			
			// Wolf (this) becomes Prey
			SetIsWolf(false);
			SetIsInvincible(true);

			// Prey becomes Wolf
			ent.SetIsWolf(true);
		}
	}

	// GETTERS / SETTERS

	void AdjustParams ()
	{
		if (isWolf) {
			if (isAI) {
				runSpeed = 8f;
				moveTick = moveTickDelay = 15;
			} else {
				runSpeed = 8f;
				moveTick = moveTickDelay = 10;
			}
		} else {
			if (isAI) {
				runSpeed = 0.6f;
				moveTick = moveTickDelay = 1;
			} else {
				runSpeed = 0.6f;
				moveTick = moveTickDelay = 1;
			}
		}
	}

	public void SetIsWolf (bool b, bool force = false)
	{
		if (!force && b == isWolf)	return;
		isWolf = b;

		if (isWolf)
		{
			// Change sprite and tag
			gameObject.GetComponent<SpriteRenderer>().sprite = wolfSprite;
			gameObject.tag = "Wolf";
			// Set base power
			basePower = 0;
			// Set Wolf in Main
			Main.instance.SetWolf(gameObject);
		}
		else
		{
			// Change sprite and tag
			gameObject.GetComponent<SpriteRenderer>().sprite = preySprite;
			gameObject.tag = "Prey";
			// Set base power
			basePower = 35;
		}

		AdjustParams ();
	}

	public bool IsWolf () { return isWolf; }

	public void SetIsAI (bool b, bool force = false)
	{
		if (!force && b == isAI)	return;
		isAI = b;

		AdjustParams ();
	}

	public bool IsAI () { return isAI; }
	
	public void SetIsInvincible (bool b, bool force = false)
	{
		if (!force && b == isInvincible)	return;
		isInvincible = b;

		// Remove invincibility for anyone who might have it until now
		if (isInvincible) {
			foreach (Entity ent in GameObject.FindObjectsOfType<Entity> ()) {
				if (ent != this)	ent.SetIsInvincible (false);
			}
			// Add halo
			halo = Instantiate(haloPrefab) as GameObject;
			halo.transform.SetParent (transform, false);
		} else {
			// Remove halo
			if (halo != null)	Destroy (halo);
		}
	}

	public bool IsInvincible () { return isInvincible; }

	public void SetPlayerNum (int n) { playerNum = n; }
	public int GetPlayerNum () { return playerNum; }

	// STEERING BEHAVIOURS

	Vector2 Evade (GameObject predator)
	{
		Rigidbody2D predatorRB = predator.GetComponent<Rigidbody2D> ();
		Vector2 toWolf = predator.transform.position - transform.position;
		float lat = toWolf.magnitude / (runSpeed + predatorRB.velocity.magnitude);
		Vector2 mov = (Vector2)predator.transform.position + predatorRB.velocity * lat;
		mov = (Vector2)transform.position - mov;
		return mov;
	}

	Vector2 Pursuit (GameObject prey)
	{
		Rigidbody2D preyRB = prey.GetComponent<Rigidbody2D> ();

		Vector2 mov = Vector2.zero;
		Vector2 toPrey = prey.transform.position - transform.position;
		float relativeHeading = Vector2.Dot (transform.right, prey.transform.right);
		if (Vector2.Dot(toPrey, transform.right) > 0 && relativeHeading < -0.95) {
			mov = (Vector2)prey.transform.position - (Vector2)transform.position;
			return mov;
		}
		float lat = toPrey.magnitude / (runSpeed + preyRB.velocity.magnitude);
		mov = (Vector2)prey.transform.position + preyRB.velocity * lat;
		mov = mov - (Vector2)transform.position;
		return mov;
	}

	// BASE
	
	void ShowBase ()
	{
		
	}

	void HideBase ()
	{

	}

}










