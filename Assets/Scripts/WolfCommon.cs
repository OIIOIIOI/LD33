using UnityEngine;
using System.Collections;

public class WolfCommon : MonoBehaviour {

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (coll.collider.tag == "Prey")
		{
			CommonAI cAI = coll.collider.gameObject.GetComponent<CommonAI>();
			if (cAI.isInvincible)	return;
			
			// Prey becomes Wolf
			cAI.BecomeWolf();
			// Wolf becomes Prey
			cAI = this.gameObject.GetComponent<CommonAI>();
			cAI.BecomePrey();

			foreach (GameObject p in GameObject.FindGameObjectsWithTag("Prey"))
			{
				// Update Wolf reference for all Prey AIs
				PreyAI pAI = p.GetComponent<PreyAI>();
				if (pAI != null)	pAI.setWolf(coll.collider.gameObject);
			}
		}
	}

}
