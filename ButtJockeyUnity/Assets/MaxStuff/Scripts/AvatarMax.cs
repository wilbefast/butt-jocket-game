	using UnityEngine;
using System.Collections;

public class AvatarMax : MonoBehaviour {

	public bool boostActive = false;

	public AnimationCurve accelerationCurve;
	public AnimationCurve impulseCurve;
	public AnimationCurve forceCurve;
	public AnimationCurve velocityCurve;

	public ForceMode mode;

	public UnityEngine.UI.Text  feedbackValue;
	public int playerID;
	public float value;

	public float accelerationGain = 15f;
	public float maxAcceleration = 30f;
	public float velocityGain = 5f;
	public float forceGain = 10f;
	public float impulseGain = 20f;
	public float impulseBoost = 5f;

	private bool startRegister = false;
	private float nextkey = 0;

	private bool didBoost = false;

	// Use this for initialization
	void Start () {
		accelerationCurve = new AnimationCurve ();
	}
	
	// Update is called once per frame
	void Update()
	{

		if (startRegister) {
			if (mode == ForceMode.Impulse)
			{
				this.impulseCurve.AddKey(nextkey,this.GetComponent<Rigidbody>().velocity.z);
			}
			else if (mode == ForceMode.Acceleration)
			{
				this.accelerationCurve.AddKey(nextkey,this.GetComponent<Rigidbody>().velocity.z);
			}
			else if (mode == ForceMode.VelocityChange)
			{
				this.velocityCurve.AddKey(nextkey,this.GetComponent<Rigidbody>().velocity.z);
			}
			else if (mode == ForceMode.Force)
			{
				this.forceCurve.AddKey(nextkey,this.GetComponent<Rigidbody>().velocity.z);
			}

			nextkey += 0.25f;
		}

		//catch input
		if (Input.GetButtonDown ("Spank" + playerID)) {
			if (mode == ForceMode.Impulse)
			{
				this.value += this.impulseGain; 	// do it log;
			}
			else if (mode == ForceMode.Acceleration)
			{
				if (this.value < 0f)
					this.value = 0f;

				this.value += (this.maxAcceleration - this.value) * 0.5f; // do it log;
			}
			else if (mode == ForceMode.VelocityChange)
			{
				this.value += this.velocityGain; // do it log;
			}
			else if (mode == ForceMode.Force)
			{
				this.value += this.forceGain; // do it log;
			}

			if (this.boostActive)
				this.didBoost = true;
		}

		if (Input.GetKeyUp (KeyCode.R)) {
			startRegister = !startRegister;

			nextkey = 0f;
		}

	}

	void OnGUI()
	{
		this.feedbackValue.text = "Value: " +Mathf.Round (this.value).ToString()+ "\n"+"Velocity: " + this.GetComponent<Rigidbody>().velocity.z;
	}

	void FixedUpdate () {
		this.GetComponent<Rigidbody> ().AddForce (transform.parent.forward * this.value, this.mode);
		if (this.didBoost)
			this.GetComponent<Rigidbody> ().AddForce (transform.parent.forward * this.impulseBoost, ForceMode.Impulse);

		if (mode == ForceMode.Impulse)
		{
			this.value = 0f; // do it log;
		}

		this.didBoost = false;
		//if (this.GetComponent<Rigidbody> ().velocity.z < 0.5f)
		//	this.GetComponent<Rigidbody> ().Sleep ();
	}

	void LateUpdate()
	{
		if (this.value > 0f)
		{

			if (mode == ForceMode.Acceleration)
			{
				if (this.GetComponent<Rigidbody>().velocity.z > 0) this.value -= 1f; // do it log;
			}
			else if (mode == ForceMode.VelocityChange)
			{
				this.value = Mathf.Max (0,this.value - 1f); // do it log;
			}
			else if (mode == ForceMode.Force)
			{
				this.value -= 1f; // do it log;
			}

		}
	}
}
