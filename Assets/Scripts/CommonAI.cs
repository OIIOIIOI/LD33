using UnityEngine;
using System.Collections;

public class CommonAI : MonoBehaviour {

	[HideInInspector]
	public bool isInvincible = false;

	GameObject halo;

	public void BecomeWolf ()
	{
		PreyAI pAI = this.gameObject.GetComponent<PreyAI>();
		if (pAI != null)
		{
			Destroy(pAI);
			this.gameObject.AddComponent<WolfAI>();
		}
		this.gameObject.AddComponent<WolfCommon>();
		this.gameObject.GetComponent<SpriteRenderer>().sprite = Main.instance.wolfSprite;
		this.gameObject.tag = "Wolf";
	}

	public void BecomePrey ()
	{
		WolfCommon wc = this.gameObject.GetComponent<WolfCommon>();
		Destroy (wc);

		WolfAI wAI = this.gameObject.GetComponent<WolfAI>();
		if (wAI != null)
		{
			Destroy(wAI);
			this.gameObject.AddComponent<PreyAI>();
		}
		foreach (CommonAI cAI in GameObject.FindObjectsOfType<CommonAI> ()) {
			cAI.EndInvincibility();
		}
		StartInvincibility ();
		this.gameObject.GetComponent<SpriteRenderer>().sprite = Main.instance.preySprite;
		this.gameObject.tag = "Prey";
	}
	
	public void StartInvincibility ()
	{
		halo = Instantiate (Main.instance.haloPrefab) as GameObject;
		halo.transform.SetParent (this.transform, false);
		isInvincible = true;
	}

	public void EndInvincibility ()
	{
		if (halo != null)	Destroy (halo);
		isInvincible = false;
	}

}










