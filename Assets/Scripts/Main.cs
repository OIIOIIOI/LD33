using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	public static Main instance;

	public GameObject player;
	public Sprite wolfSprite;
	public Sprite preySprite;
	public GameObject haloPrefab;

	void Awake()
	{
		if (instance != null)
			throw new UnityException ("Main is already instanciated");
		
		instance = this;
	}

	void Update ()
	{
		Time.timeScale = 1 - Input.GetAxis ("Fire");
	}
}
