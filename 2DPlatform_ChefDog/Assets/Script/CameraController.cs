using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Vector3 offset;
	GameObject player;
	public Transform cbl;
	public Transform cbr;
	public bool haveBoundary=false;

	void Awake () {
		player = GameObject.FindWithTag ("Player");
		offset = transform.position - player.transform.position;
	}

	void Start () {
		transform.position = player.transform.position + offset;
	}

	void FixedUpdate () {
		Vector3 dest = player.transform.position + offset;
		if (haveBoundary)
			dest.x = Mathf.Clamp (dest.x, cbl.position.x, cbr.position.x);
		transform.position = Vector3.Lerp (transform.position, dest, Time.deltaTime * 5);
	}
}
