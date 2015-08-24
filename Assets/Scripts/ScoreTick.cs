using UnityEngine;
using System.Collections;

public class ScoreTick : MonoBehaviour {

	public Sprite one;
	public Sprite two;
	public Sprite three;

	SpriteRenderer sr;
	GameObject parentGO;

	void Awake () {
		sr = gameObject.GetComponent<SpriteRenderer> ();
	}

	public void Setup (int v, GameObject go)
	{
		v = Mathf.Clamp (v, 1, 3);
		if (v == 1)			sr.sprite = one;
		else if (v == 2)	sr.sprite = two;
		else if (v == 3)	sr.sprite = three;

		parentGO = go;

		StartCoroutine (Fade ());
	}

	IEnumerator Fade ()
	{
		while (sr.color.a > 0.05f)
		{
			Color c = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a * 0.95f);
			sr.color = c;
			transform.Translate(0f, 0.03f, 0f);
			transform.localScale = parentGO.transform.localScale;
			//yield return new WaitForSeconds(0.1f);
			yield return new WaitForFixedUpdate();
		}
		Destroy (gameObject);
	}

}
