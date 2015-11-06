using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour 
{
	public float ForwardSpeed = 3f;

	public float SideSpeed = 3f;

	Vector3 cameraOffset;

	void Start()
	{
		cameraOffset = transform.position - Camera.main.transform.position;
	}

	Vector2 GetControlDirection()
	{
		var x = (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
		return new Vector2(x, 0f);
	}
	
	void FixedUpdate () 
	{


		Camera.main.transform.position = transform.position - cameraOffset;

		var dir = GetControlDirection ();
		var body = GetComponent<Rigidbody> ();
		body.AddForce (transform.parent.forward * ForwardSpeed, ForceMode.Acceleration);
		body.AddForce (transform.parent.right * dir.x * SideSpeed, ForceMode.Acceleration);
	}
}
