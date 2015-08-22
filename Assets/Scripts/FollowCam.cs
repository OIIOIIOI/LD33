using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

	public GameObject target;
	public float smooth = 5f;

	float mapWidth = 28f;
	float mapHeight = 16f;
	
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
		Vector3 targetPos = target.transform.position;
		targetPos.z = this.transform.position.z;
		Vector3 destPos = Vector3.Lerp (this.transform.position, targetPos, Time.deltaTime * smooth);
		destPos.x = Mathf.Clamp(destPos.x, minX, maxX);
		destPos.y = Mathf.Clamp(destPos.y, minY, maxY);
		this.transform.position = destPos;

	}
}
