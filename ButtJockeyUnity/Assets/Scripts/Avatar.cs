using UnityEngine;
using System.Collections;
using System.Linq;

public class Avatar : MonoBehaviour 
{
	public bool active = false;
	public bool boostActive = false;

	public int playerID;

	
	public float accelerationGain = 15f;
	public float maxAcceleration = 30f;
	public float impulseBoost = 5f;
	public float impulseRespawn = 10f;

	private bool didBoost = false;
	private float value;
	private Vector2 dir;
	
	public float SideSpeed = 3f;

	Vector3 cameraOffset;

	static Avatar master, leader;

	float life = 1.0f;

	bool IsDead()
	{
		return false;//return (transform.position.y < 0f);
	}

	SpriteRenderer sprite;

	void Start()
	{
		sprite = transform.parent.FindChild("Teleport").FindChild("SpriteOffset").GetComponentInChildren<SpriteRenderer> ();

		cameraOffset = transform.position - Camera.main.transform.position;

		if (master == null) 
		{
			master = this;
			//LSD.instance.init();
		}
	}

	
	// Update is called once per frame
	void Update()
	{
		if (!this.active)
			return;

		//catch input
		if (Input.GetButtonDown ("RightSpank" + playerID) || Input.GetButtonDown ("LeftSpank" + playerID)) 
		{
			if (this.value < 0f)
				this.value = 0f;
			
			this.value += (this.maxAcceleration - this.value) * 0.5f; // do it log;

			dir = new Vector2((Input.GetButtonDown("RightSpank"+this.playerID) ? -1f : 0f) + (Input.GetButtonDown("LeftSpank"+this.playerID) ? 1f : 0f),0f);

			if (this.boostActive)
				this.didBoost = true;
		}
		 
	}
	
	void LateUpdate()
	{
		if (this.value > 0f)
		{
			if (this.GetComponent<Rigidbody>().velocity.z > 0) this.value -= 1f; // do it log;
		}
	}

	void FixedUpdate () 
	{
		// One avatar calcules who is the winner
		if (master == this) 
		{
			var avatars = FindObjectsOfType<Avatar>().Where(a => !a.IsDead());
			if(avatars.Any())
				leader = avatars.OrderBy(a => a.transform.position.z ).Last();
			else leader = null;
		}

		// The winner controls the camera
		if (leader == this) 
		{
			var c = Camera.main.transform;
			c.position = Vector3.Lerp(c.position, transform.position - cameraOffset, Time.fixedDeltaTime*3f);
			life = 1f;

			c.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>().focalTransform = transform;

			//GetComponent<ParticleSystem>().enableEmission = true;
		}
		else
		{
			//GetComponent<ParticleSystem>().enableEmission = false;

			if(sprite.isVisible)
			{
				// regenerate life
				life = Mathf.Clamp01(life + Time.fixedDeltaTime);
			}
			else
			{
				// lose life
				life = Mathf.Clamp01(life - Time.fixedDeltaTime);
				if(life == 0f)
				{
					//score progress
					Camera.main.GetComponent<GameSystem>().UpdateScore("Player"+this.playerID);

					// death and respawn
					life = 1f;

					// maybe leader is also dead
					if(leader != null)
					{
						transform.position = leader.transform.position - Vector3.forward;
						//GetComponent<Rigidbody>().velocity = leader.GetComponent<Rigidbody>().velocity;
						GetComponent<Rigidbody>().AddForce(Vector3.forward *impulseRespawn,ForceMode.Impulse);
					}
					else
					{
						Debug.LogWarning("Everyone has fallen off!");
					}
				}
			}
		}

		var body = GetComponent<Rigidbody> ();

		// sideways
		body.AddForce (transform.parent.right * dir.x * SideSpeed, ForceMode.Acceleration);

		// forwards
		body.AddForce (transform.parent.forward * this.value, ForceMode.Acceleration);

		// boosts
		if (this.didBoost)
			body.AddForce (transform.parent.forward * this.impulseBoost, ForceMode.Impulse);
		this.didBoost = false;
	}
}
