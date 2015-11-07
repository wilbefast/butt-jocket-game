using UnityEngine;
using System.Collections;

public class Gigote : MonoBehaviour 
{
	public float amount = 1f;

	public float duration = 0.5f;

	public float timeOffset = 0f;

	public float multiplier = 1f;

	public bool position = true;
	Vector3 initialPosition;

	public bool scale = true;
	Vector3 initialScale;

	IEnumerator doGigote(float offset = 0f)
	{
		while (offset > 1f)
			offset -= 1f;

		bool gonfle = true;

		while (enabled) 
		{
			float dur = duration / multiplier;

			float start_t = Time.time + offset*duration;
			float t = start_t;
			offset = 0f;
			while(t <= start_t + dur)
			{
				float progress = Mathf.Clamp01((t - start_t) / dur);

				if(scale)
					transform.localScale = initialScale *
						(gonfle 
						 ? (1f + amount*progress) 
						 : (1f + amount*(1f - progress)));

				if(position)
					transform.localPosition = initialPosition + Vector3.up *
						(gonfle 
						 ? (1f + multiplier*amount*progress) 
						 : (1f + multiplier*amount*(1f - progress)));

				t = Time.time;
				yield return null;
			}
			offset = 0f;
			gonfle = !gonfle;
		}
	}

	void Start () 
	{
		initialPosition = transform.localPosition;
		initialScale = transform.localScale;
		StartCoroutine (doGigote (timeOffset + Random.value));
	}

}
