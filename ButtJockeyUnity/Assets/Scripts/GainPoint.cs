using UnityEngine;
using System.Collections;

public class GainPoint : MonoBehaviour {

	float pongValue;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator UpdateState()
	{
		float startingTime = Time.time;

		while(Time.time <= startingTime + 0.75f) {
			this.GetComponent<RectTransform>().localScale = new Vector3(0.5f + Mathf.PingPong(pongValue,1f),0.5f + Mathf.PingPong(pongValue,1f),1);
			pongValue += 0.05f;
			this.GetComponent<RectTransform>().Rotate(Vector3.forward * 15f);

			
			yield return new WaitForSeconds(0.01f);
		}

		this.GetComponent<RectTransform> ().localScale = new Vector3(1f,1f,1f);
		this.GetComponent<RectTransform> ().rotation = Quaternion.identity;
	}
}
