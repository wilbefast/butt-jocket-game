using UnityEngine;
using System.Collections;

public class TriggerVictory : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		Debug.Log ("trigger victory");
		Camera.main.GetComponent<GameSystem> ().StartCoroutine ("EndRace");
	}
}
