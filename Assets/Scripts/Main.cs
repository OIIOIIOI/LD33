using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	public static Main instance;

	public GameObject entityPrefab;
	public GameObject obstaclePrefab;

	List<GameObject> players;

	GameObject wolf;

	void Awake()
	{
		if (instance != null)
			throw new UnityException ("Main is already instanciated");
		instance = this;
	}

	void Start ()
	{
		players = new List<GameObject> ();

		/*for (int i = 1; i < Input.GetJoystickNames().Length; i++) {
			player = Instantiate (entityPrefab, new Vector2 (0f, 0f), Quaternion.identity) as GameObject;
			player.name = "Player" + (i + 1);
			ent = player.GetComponent<Entity> ();
			ent.SetIsAI (false);
			ent.SetPlayerNum (i + 1);
			players.Add (player);
		}*/

		for (int i = 0; i < 10; i++)
		{
			Vector2 pos = Vector2.zero;
			pos.x += Random.Range(-10f, 10f);
			pos.y += Random.Range(-7f, 7f);
			Instantiate (entityPrefab, pos, Quaternion.identity);
		}

		for (int i = 0; i < 20; i++)
		{
			Vector2 pos = Vector2.zero;
			pos.x += Random.Range(-10f, 10f);
			pos.y += Random.Range(-7f, 7f);
			Instantiate (obstaclePrefab, pos, Quaternion.identity);
		}
		
		GameObject player = Instantiate (entityPrefab, new Vector2(-7f, 0f), Quaternion.identity) as GameObject;
		player.name = "Player1";
		players.Add (player);
		Entity ent = player.GetComponent<Entity> ();
		ent.SetIsAI (false);
		ent.SetIsWolf (true);

		Camera.main.GetComponent<FollowCam> ().targets = players;
	}
	
	void Update ()
	{
		Time.timeScale = 1 - Input.GetAxis ("Fire");
	}

	void LateUpdate ()
	{
		SpriteRenderer[] srs = GameObject.FindObjectsOfType<SpriteRenderer> ();
		foreach (SpriteRenderer sr in srs)
		{
			float y = sr.gameObject.transform.position.y;
			sr.sortingOrder = -Mathf.FloorToInt(y * 1000);
		}
	}

	public void SetWolf (GameObject go)
	{
		wolf = go;

		Entity ent = go.GetComponent<Entity> ();

		// Tell every Prey entity whether or not the new Wolf is a player
		GameObject[] preys = GameObject.FindGameObjectsWithTag ("Prey");
		foreach (GameObject p in preys)
		{
			Animator anim = p.GetComponent<Animator>();
			anim.SetBool("playerIsWolf", !ent.IsAI());
		}
	}

	public GameObject GetWolf () { return wolf; }
}
