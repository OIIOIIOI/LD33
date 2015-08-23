using UnityEngine;
using System.Collections;

public class ChargePoint : MonoBehaviour {

	Animator anim;

	void Awake () {
		anim = gameObject.GetComponent<Animator>();
	}

	void OnTriggerEnter2D (Collider2D coll)
	{
		Entity ent = coll.gameObject.GetComponent<Entity> ();
		if (ent == null || ent.IsAI() || ent.IsWolf())
			return;

		bool charged = anim.GetBool("isCharged");
		if (charged) {
			bool used = ent.Charge();
			if (used) {
				anim.SetBool("isCharged", false);
				Invoke("ResetCharge", 10f);
			}
		}
	}
	
	void ResetCharge ()
	{
		anim.SetBool("isCharged", true);
	}

}
