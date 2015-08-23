using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public GameObject haloPrefab;
	public GameObject hidePrefab;

	Rigidbody2D RB;
	
	float runSpeed;
	
	int moveTickDelay;
	int moveTick;

	int hideTotal;
	int hideLeft;

	int humanityTotal;
	int humanityLeft;

	bool isWolf;
	bool isAI;
	bool isInvincible;
	bool isHiding;

	int playerNum;

	int AIModel;
	int playerModel;

	GameObject halo;
	GameObject hideCircle;

	void Awake ()
	{
		RB = gameObject.GetComponent<Rigidbody2D>();

		playerNum = 1;
		playerModel = playerNum + 10;

		hideTotal = 60;
		humanityLeft = humanityTotal = 2000;

		AIModel = Random.Range (0, 2);

		SetIsAI (true, true);
		SetIsWolf (false, true);
		SetIsInvincible (false, true);
		SetIsHiding (false, true);
	}

	void FixedUpdate ()
	{
		// Check if activating base
		if (!isWolf) {
			if (isAI)
				isHiding = PreyHide ();
			else
				isHiding = PlayerHide ();

			if (isHiding) {
				hideLeft--;
				if (hideLeft <= 0) {
					hideLeft = 0;
					HideCircle ();
				} else {
					ShowCircle ();
				}
			} else {
				HideCircle ();
			}
		} else if (!isAI) {
			humanityLeft--;

			Main.instance.SetWolfBar(1f - ((float)humanityLeft / (float)humanityTotal));

			if (humanityLeft == 0)
				Debug.Log("GAME OVER");
		}

		Main.instance.SetHideBar ((float)hideLeft / (float)hideTotal);

		// Check if time to move and get Vector2
		Vector2 mov = Vector2.zero;
		// No movement if base active
		if (!isHiding)
		{
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
		}

		// Change velocity
		Vector2 vel = RB.velocity + mov;
		vel *= 0.9f;
		RB.velocity = vel;

		// Flip
		if (RB.velocity.x > 0)		transform.localScale = new Vector3 (-1f, 1f, 1f);
		else if (RB.velocity.x < 0)	transform.localScale = new Vector3 (1f, 1f, 1f);

		// Adjust rotation
		//float angle = Mathf.Atan2(RB.velocity.y, RB.velocity.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	// PLAYER

	Vector2 GetPlayerMove ()
	{
		Vector2 mov = Vector2.zero;

		mov.x = Input.GetAxis ("HorizontalP" + playerNum);
		mov.y = Input.GetAxis ("VerticalP" + playerNum);

		//mov.Normalize ();
		return mov;
	}
	
	bool PlayerHide ()
	{
		return (Input.GetButton ("HideP" + playerNum) && hideLeft > 0);
		//return false;
	}

	// PREY
	
	Vector2 GetPreyMove ()
	{
		Vector2 mov = Vector2.zero;

		GameObject wolf = Main.instance.GetWolf ();
		if (wolf != null)
		{
			if (Vector2.Distance(wolf.transform.position, transform.position) < 5f)
				mov = Evade (wolf);
			else {
				mov = GetClosestWayPoint ();
			}
		}

		mov.Normalize ();
		return mov;
	}
	
	GameObject currentWaypoint;
	GameObject previousWaypoint;

	Vector2 GetClosestWayPoint ()
	{
		Vector2 mov = Vector2.zero;

		GameObject[] preys = GameObject.FindGameObjectsWithTag ("Waypoint");
		if (preys.Length == 0)
			return mov;
		
		float shortestDist = float.MaxValue;
		GameObject closestPrey = preys [0];
		
		bool currentIsHidden = false;

		foreach (GameObject p in preys)
		{
			if (p == previousWaypoint)
				continue;

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
				if (p == currentWaypoint)	currentIsHidden = true;
				continue;
			}
			// Didn't hit a wall, check if prey is closer
			float dist = Vector2.Distance(transform.position, p.transform.position);
			if (dist < shortestDist) {
				shortestDist = dist;
				closestPrey = p;
			}
		}
		// First time assign or currentPrey no more available
		if (currentWaypoint == null || currentIsHidden) {
			currentWaypoint = closestPrey;
		}

		if (currentWaypoint != previousWaypoint)
			mov = closestPrey.transform.position - transform.position;

		mov.Normalize ();
		return mov;
	}

	void OnTriggerEnter2D (Collider2D coll)
	{
		if (!isAI || coll.tag != "Waypoint")	return;

		if (currentWaypoint != null && coll.gameObject == currentWaypoint) {
			previousWaypoint = currentWaypoint;
			currentWaypoint = null;
		}

	}

	bool PreyHide ()
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
			if (ent.IsInvincible() || ent.IsHiding()) {
				currentIsHidden = true;
				continue;
			}
			
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
		// First time assign or currentPrey no more available
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
		if (isWolf && coll.collider.tag == "Prey")
		{
			Entity ent = coll.collider.gameObject.GetComponent<Entity>();
			//Debug.Log ("wolf (" + gameObject.name + ", " + isInvincible + ") collided with prey (" + coll.collider.gameObject.name + ", " + ent.IsInvincible() + ")");
			if (ent.IsInvincible() || ent.IsHiding())	return;
			
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
				runSpeed = 0.6f;
				moveTick = moveTickDelay = 1;
			} else {
				runSpeed = 0.6f;
				moveTick = moveTickDelay = 1;
			}
		} else {
			if (isAI) {
				runSpeed = 0.45f;
				moveTick = moveTickDelay = 1;
			} else {
				runSpeed = 0.55f;
				moveTick = moveTickDelay = 1;
			}
		}
	}

	public void SetIsWolf (bool b, bool force = false)
	{
		if (!force && b == isWolf)	return;
		isWolf = b;

		Animator anim = gameObject.GetComponent<Animator>();
		anim.SetBool ("entityIsWolf", isWolf);

		if (isWolf)
		{
			// Stop hiding (shouldn't happen)
			SetIsHiding (false);
			// Change tag
			gameObject.tag = "Wolf";
			// Set base power
			hideLeft = 0;
			// Set Wolf in Main
			Main.instance.SetWolf(gameObject);
		}
		else
		{
			// Change tag
			gameObject.tag = "Prey";
			// Set base power
			hideLeft = hideTotal / 3;
		}

		AdjustParams ();
	}

	public bool IsWolf () { return isWolf; }

	public void SetIsAI (bool b, bool force = false)
	{
		if (!force && b == isAI)	return;
		isAI = b;

		// Change model
		Animator anim = gameObject.GetComponent<Animator>();
		if (isAI)	anim.SetInteger ("model", AIModel);
		else		anim.SetInteger ("model", playerModel);

		AdjustParams ();
	}

	public bool IsAI () { return isAI; }
	
	public void SetIsInvincible (bool b, bool force = false)
	{
		if (!force && b == isInvincible)	return;
		isInvincible = b;

		if (isInvincible) {
			// Remove invincibility for anyone who might have it until now
			/*foreach (Entity ent in GameObject.FindObjectsOfType<Entity> ()) {
				if (ent != this)	ent.SetIsInvincible (false);
			}*/
			// Add halo
			halo = Instantiate(haloPrefab) as GameObject;
			halo.transform.SetParent (transform, false);
			// Start timer
			Invoke ("LooseInvincibility", 3f);
		} else {
			// Remove halo
			if (halo != null)	Destroy (halo);
		}
	}

	public bool IsInvincible () { return isInvincible; }

	void LooseInvincibility ()
	{
		SetIsInvincible (false);
	}
	
	public void SetIsHiding (bool b, bool force = false)
	{
		if (!force && b == isHiding)	return;
		isHiding = b;
	}
	
	public bool IsHiding () { return isHiding; }

	public void SetPlayerNum (int n)
	{
		playerNum = n;
		playerModel = playerNum + 10;
	}
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
	
	void ShowCircle ()
	{
		if (hideCircle != null)	return;
		hideCircle = Instantiate(hidePrefab) as GameObject;
		hideCircle.transform.SetParent(transform, false);
		SetIsHiding (true);
	}

	void HideCircle ()
	{
		if (hideCircle != null)	Destroy (hideCircle);
		SetIsHiding (false);
	}

	public bool Charge ()
	{
		if (hideLeft < hideTotal) {
			hideLeft = Mathf.Clamp(hideLeft + hideTotal / 3, 0, hideTotal);
			return true;
		}
		return false;
	}

}











