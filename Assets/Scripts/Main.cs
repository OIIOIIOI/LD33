using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	public static Main instance;

	public GameObject entityPrefab;
	public GameObject obstaclePrefab;
	public GameObject wallPrefab;
	public GameObject scoreTickPrefab;

	List<GameObject> players;

	GameObject wolf;
	Image wolfBar;
	Image hideBar;

	int scoreTickDelay;
	int scoreTick;
	float bestDist;

	int playerScore;
	Text scoreTF;

	GameObject endPanel;
	Image blackOverlay;

	[HideInInspector]
	public bool canPlay;

	bool canPress;

	void Awake()
	{
		if (instance != null)
			throw new UnityException ("Main is already instanciated");
		instance = this;

		endPanel = GameObject.Find ("EndPanel");
		endPanel.SetActive (false);

		canPlay = false;
		canPress = false;
	}

	void Start ()
	{
		//CreateWalls ();

		players = new List<GameObject> ();

		for (int i = 0; i < 10; i++)
		{
			Vector2 pos = Vector2.zero;
			pos.x += Random.Range(-4f, 4f);
			pos.y += Random.Range(-4f, 4f);
			Instantiate (entityPrefab, pos, Quaternion.identity);
		}

		GameObject player = Instantiate (entityPrefab) as GameObject;
		player.name = "Player1";
		players.Add (player);
		Entity ent = player.GetComponent<Entity> ();
		ent.SetIsAI (false);
		ent.SetIsWolf (true);

		Camera.main.GetComponent<FollowCam> ().targets = players;

		wolfBar = GameObject.Find ("BarFill").GetComponent<Image>();
		hideBar = GameObject.Find ("HideBarFill").GetComponent<Image>();

		scoreTick = scoreTickDelay = 45;
		bestDist = float.MaxValue;

		playerScore = 0;
		scoreTF = GameObject.Find ("ScoreTF").GetComponent<Text> ();

		blackOverlay = GameObject.Find("BlackOverlay").GetComponent<Image>();
		blackOverlay.color = Color.black;
		
		StartCoroutine(FadeFromBlack());
	}
	
	IEnumerator FadeFromBlack()
	{
		while (blackOverlay.color.a > 0)
		{
			blackOverlay.color = new Color(0f, 0f, 0f, blackOverlay.color.a - 0.025f);
			yield return new WaitForFixedUpdate();
		}
		canPlay = true;
	}

	void Update ()
	{
		//Time.timeScale = 1 - Input.GetAxis ("Fire");
		
		if (Input.GetButtonDown("Start") && canPress) {
			canPress = false;

			blackOverlay.color = Color.clear;
			StartCoroutine(FadeToBlack());
		}
	}

	void FixedUpdate ()
	{
		if (!canPlay)	return;

		// Checks
		if (players.Count == 0 || wolf == null)
			return;
		
		Entity ent = players [0].GetComponent<Entity> ();
		if (ent.IsWolf () || ent.IsInvincible())
			return;

		// Store dist

		float dist = Vector2.Distance (wolf.transform.position, ent.transform.position);
		//Debug.Log (dist + " / " + bestDist);
		if (dist < bestDist) {
			bestDist = dist;
			//Debug.Log("better");
		}

		// Score tick
		scoreTick--;

		if (scoreTick <= 0)
		{
			int score = 1;
			if (bestDist < 1.5f)		score = 3;
			else if (bestDist < 5f)	score = 2;

			GameObject go = Instantiate (scoreTickPrefab) as GameObject;
			go.transform.SetParent(players[0].transform, false);
			go.transform.localScale = players[0].transform.localScale;
			ScoreTick st = go.GetComponent<ScoreTick>();
			st.Setup (score, players[0]);

			if (score == 1)			playerScore += 1;
			else if (score == 2)	playerScore += 5;
			else if (score == 3)	playerScore += 25;

			if (scoreTF != null)	scoreTF.text = "Score: " + playerScore;

			scoreTick = scoreTickDelay;
			bestDist = float.MaxValue;
		}
	}

	void LateUpdate ()
	{
		if (!canPlay)	return;

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

		if (players.Count > 0 && go == players[0])	bestDist = float.MaxValue;

		//Entity ent = go.GetComponent<Entity> ();

		// Tell every Prey entity whether or not the new Wolf is a player
		/*GameObject[] preys = GameObject.FindGameObjectsWithTag ("Prey");
		foreach (GameObject p in preys)
		{
			Animator anim = p.GetComponent<Animator>();
			anim.SetBool("playerIsWolf", !ent.IsAI());
		}*/
	}

	public GameObject GetWolf () { return wolf; }
	
	public void SetWolfBar (float v)
	{
		v = Mathf.Clamp(v, 0f, 1f);
		wolfBar.fillAmount = v;
	}

	public void SetHideBar (float v)
	{
		v = Mathf.Clamp(v, 0f, 1f);
		hideBar.fillAmount = v;
	}

	public void GameOver()
	{
		canPlay = false;

		foreach (Rigidbody2D rb in GameObject.FindObjectsOfType<Rigidbody2D> ()) {
			rb.velocity = Vector2.zero;
		}

		endPanel.SetActive (true);
		Text finalScoreTF = GameObject.Find ("FinalScoreTF").GetComponent<Text> ();
		if (finalScoreTF != null)	finalScoreTF.text = "Your final score: " + playerScore;

		canPress = true;
	}
	
	IEnumerator FadeToBlack()
	{
		while (blackOverlay.color.a < 1)
		{
			blackOverlay.color = new Color(0f, 0f, 0f, blackOverlay.color.a + 0.025f);
			yield return new WaitForFixedUpdate();
		}
		Invoke("BackToMenu", 0.5f);
	}
	
	void BackToMenu()
	{
		Application.LoadLevel("Splash");
	}

}
