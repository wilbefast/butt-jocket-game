using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {



	// Use this for initialization
	void Start () {
		if (GameObject.FindGameObjectsWithTag ("musique").Length > 1)
			Destroy (this.gameObject);
		else 
			DontDestroyOnLoad (this.gameObject);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
