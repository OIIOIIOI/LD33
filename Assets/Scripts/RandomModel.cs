using UnityEngine;
using System.Collections;

public class RandomModel : MonoBehaviour {

	public int modelsTotal;
	int model;

	void Awake ()
	{
		model = Random.Range (0, modelsTotal);

		Animator anim = gameObject.GetComponent<Animator> ();
		if (anim != null)	anim.SetInteger("model", model);
	}

}
