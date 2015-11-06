using UnityEngine;
using System.Collections;
using System.Linq;

public class Avatar : MonoBehaviour 
{
	public float ForwardSpeed = 3f;

	public float SideSpeed = 3f;

	Vector3 cameraOffset;

	static Avatar master, leader;

	float life = 1.0f;

	void Start()
	{
		cameraOffset = transform.position - Camera.main.transform.position;

		if (master == null)
			master = this;
	}

	Vector2 GetControlDirection()
	{
		var x = (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
		return new Vector2(x, 0f);
	}
	
	void FixedUpdate () 
	{
		// One avatar calcules who is the winner
		if (master == this) 
		{
			leader = FindObjectsOfType<Avatar>().OrderBy(
				a => a.transform.position.y < 0f ? -float.MaxValue : a.transform.position.z ).Last();
		}

		// The winner controls the camera
		if (leader == this) 
		{
			var c = Camera.main.transform;
			c.position = Vector3.Lerp(c.position, transform.position - cameraOffset, Time.fixedDeltaTime*3f);
			life = 1f;

			GetComponent<ParticleSystem>().enableEmission = true;
		}
		else
		{
			GetComponent<ParticleSystem>().enableEmission = false;

			if(GetComponent<Renderer>().isVisible)
			{
				// regenerate life
				life = Mathf.Clamp01(life + Time.fixedDeltaTime);
			}
			else
			{
				Debug.Log(name + " is not visible");

				// lose life
				life = Mathf.Clamp01(life + Time.fixedDeltaTime);
				if(life == 0f)
				{
					// death
					Debug.Log(name + " has fallen behind");
					life = 1f;
				}
			}
		}

		var dir = GetControlDirection ();
		var body = GetComponent<Rigidbody> ();
		body.AddForce (transform.parent.forward * ForwardSpeed, ForceMode.Acceleration);
		body.AddForce (transform.parent.right * dir.x * SideSpeed, ForceMode.Acceleration);
	}
}
