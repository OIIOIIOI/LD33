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

		for (int i = 1; i < Input.GetJoystickNames().Length; i++) {
			player = Instantiate (entityPrefab, new Vector2 (0f, 0f), Quaternion.identity) as GameObject;
			player.name = "Player" + (i + 1);
			ent = player.GetComponent<Entity> ();
			ent.SetIsAI (false);
			ent.SetPlayerNum (i + 1);
			players.Add (player);
		}

		GameObject prey = Instantiate (entityPrefab, new Vector2(3f, 0f), Quaternion.identity) as GameObject;
		prey.name = "Jimmy";
		prey = Instantiate (entityPrefab, new Vector2(-3f, 0f), Quaternion.identity) as GameObject;
		prey.name = "Theo";
		prey = Instantiate (entityPrefab, new Vector2(0f, 3f), Quaternion.identity) as GameObject;
		prey.name = "Juliet";
		prey = Instantiate (entityPrefab, new Vector2(0f, -3f), Quaternion.identity) as GameObject;
		prey.name = "Dorothy";
		//Entity ent = prey.GetComponent<Entity> ();
		//ent.SetIsWolf (true);

		Camera.main.GetComponent<FollowCam> ().targets = players;
	}
	
	void Update ()
	{
		Time.timeScale = 1 - Input.GetAxis ("Fire");
	}

	public void SetWolf (GameObject go)
	{
		wolf = go;
	}

	public GameObject GetWolf () { return wolf; }
}
