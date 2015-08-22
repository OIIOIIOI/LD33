using UnityEngine;
using System.Collections;

public class CommonAI : MonoBehaviour {

	public Sprite wolfSprite;
	public Sprite preySprite;

	// Use this for initialization
	void Start () {

	}

	public void BecomeWolf ()
	{
		PreyAI pAI = this.gameObject.GetComponent<PreyAI>();
		if (pAI != null)
		{
			Destroy(pAI);
			this.gameObject.AddComponent<WolfAI>();
			this.gameObject.GetComponent<SpriteRenderer>().sprite = wolfSprite;
			this.gameObject.tag = "Wolf";
		}
	}

	public void BecomePrey ()
	{
		WolfAI wAI = this.gameObject.GetComponent<WolfAI>();
		if (wAI != null)
		{
			Destroy(wAI);
			PreyAI pAI = this.gameObject.AddComponent<PreyAI>();
			pAI.StartInvincibility();
			this.gameObject.GetComponent<SpriteRenderer>().sprite = preySprite;
			this.gameObject.tag = "Prey";
		}
	}
}
