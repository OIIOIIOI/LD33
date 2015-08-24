using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HelpScreen : MonoBehaviour {
	
	Image blackOverlay;
	
	bool canPress;
	
	// Use this for initialization
	void Start () {
		blackOverlay = GameObject.Find("BlackOverlay").GetComponent<Image>();
		//blackOverlay.color = Color.clear;
		
		canPress = false;

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
		canPress = true;
	}
	
	void Update ()
	{
		if (Input.GetButtonDown("Start") && canPress) {
			canPress = false;
			
			StartCoroutine(FadeToBlack());
		}
	}
	
	IEnumerator FadeToBlack()
	{
		while (blackOverlay.color.a < 1) {
			blackOverlay.color = new Color(0f, 0f, 0f, blackOverlay.color.a + 0.025f);
			yield return new WaitForFixedUpdate();
		}
		Invoke("LoadGame", 0.5f);
	}
	
	void LoadGame()
	{
		Application.LoadLevel("Main");
	}
}
