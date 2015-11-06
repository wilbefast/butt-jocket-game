using UnityEngine;
using System.Collections;

public class ShakeButtInterface : MonoBehaviour {

	protected Vector3 originPosition;  
	protected Quaternion originRotation; 

	protected float shake_decay = 0.002f;  
	protected float shake_intensity;  
	protected float coef_shake_intensity = 0.1f;  

	// Use this for initialization
	void Start () {
		StartCoroutine ("StartShaking");
	}
	
	// Update is called once per frame
	void Update () {
		if(shake_intensity > 0){  
			this.GetComponent<RectTransform>().transform.position = originPosition + Random.insideUnitSphere * shake_intensity;  
			this.GetComponent<RectTransform>().transform.rotation =  new Quaternion(  
			                                            originRotation.x + Random.Range(-shake_intensity,shake_intensity)*.2f,  
			                                            originRotation.y + Random.Range(-shake_intensity,shake_intensity)*.2f,  
			                                            originRotation.z + Random.Range(-shake_intensity,shake_intensity)*.2f,  
			                                            originRotation.w + Random.Range(-shake_intensity,shake_intensity)*.2f);  
			shake_intensity -= shake_decay;  
			//Handheld.Vibrate ();
			if (shake_intensity <= 0)
			{
				this.GetComponent<RectTransform>().transform.position = originPosition;
				this.GetComponent<RectTransform>().transform.rotation = originRotation;

				StartCoroutine ("StartShaking");
			}
			
		}
	}

	IEnumerator StartShaking()
	{
		yield return new WaitForSeconds(Random.Range(3f,6f));


		originPosition = transform.position;  
		originRotation = transform.rotation;  
		shake_intensity = coef_shake_intensity; 


	}

	public void ShakeInstant()
	{
		StopCoroutine ("StartShaking");

		originPosition = transform.position;  
		originRotation = transform.rotation;  
		shake_intensity = coef_shake_intensity; 
	}
}
