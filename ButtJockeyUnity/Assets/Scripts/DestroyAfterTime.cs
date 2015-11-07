using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour 
{
	float time = 0.5f;

	IEnumerator WaitThenDestroy()
	{
		yield return new WaitForSeconds (time);

		Destroy (gameObject);
	}
	
	void Start () 
	{
		StartCoroutine (WaitThenDestroy ());
	}

}
