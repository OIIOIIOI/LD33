using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	public static Main instance;

	public GameObject entityPrefab;

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

		GameObject player = Instantiate (entityPrefab, new Vector2(0f, 0f), Quaternion.identity) as GameObject;
		player.name = "Player1";
		Entity ent = player.GetComponent<Entity> ();
		ent.SetIsAI (false);
		ent.SetIsWolf (true);
		players.Add (player);

		/*for (int i = 1; i < Input.GetJoystickNames().Length; i++) {
			player = Instantiate (entityPrefab, new Vector2 (0f, 0f), Quaternion.identity) as GameObject;
			player.name = "Player" + (i + 1);
			ent = player.GetComponent<Entity> ();
			ent.SetIsAI (false);
			ent.SetPlayerNum (i + 1);
			players.Add (player);
		}*/

		for (int i = 0; i < 30; i++)
		{
			Vector2 pos = Vector2.zero;
			pos.x += Random.Range(-5f, 5f);
			pos.y += Random.Range(-5f, 5f);
			GameObject prey = Instantiate (entityPrefab, pos, Quaternion.identity) as GameObject;
		}

		Camera.main.GetComponent<FollowCam> ().targets = players;
	}
	
	void Update ()
	{
		Time.timeScale = 1 - Input.GetAxis ("Fire");
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
