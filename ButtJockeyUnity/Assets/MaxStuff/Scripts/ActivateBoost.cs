using UnityEngine;
using System.Collections;

public class ActivateBoost : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") {
			other.GetComponent<AvatarMax>().boostActive = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") {
			other.GetComponent<AvatarMax>().boostActive = false;
		}
	}
}
