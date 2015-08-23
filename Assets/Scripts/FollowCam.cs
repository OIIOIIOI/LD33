using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowCam : MonoBehaviour {

	[HideInInspector]
	public List<GameObject> targets;

	public float smooth = 5f;

	float mapWidth = 16f;
	float mapHeight = 12f;
	
	float minX;
	float maxX;
	float minY;
	float maxY;

	void Start ()
	{
		var vertExtent = Camera.main.orthographicSize;    
		var horzExtent = vertExtent * Screen.width / Screen.height;

		minX = horzExtent - mapWidth / 2f;
		maxX = mapWidth / 2f - horzExtent;
		minY = vertExtent - mapHeight / 2f;
		maxY = mapHeight / 2f - vertExtent;
	}

	void LateUpdate ()
	{
		if (targets == null || targets.Count == 0)	return;

		Vector3 targetPos = Vector3.zero;
		foreach (GameObject t in targets) {
			targetPos += t.transform.position;
		}
		targetPos = targetPos / targets.Count;

		targetPos.z = transform.position.z;
		Vector3 destPos = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * smooth);
		destPos.x = Mathf.Clamp(destPos.x, minX, maxX);
		destPos.y = Mathf.Clamp(destPos.y, minY, maxY);
		transform.position = destPos;

	}
}
