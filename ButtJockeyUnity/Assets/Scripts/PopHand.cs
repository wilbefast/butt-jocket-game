using UnityEngine;
using System.Collections;

public class PopHand : MonoBehaviour 
{
	public float duration = 0.3f;

	IEnumerator PopAndFade()
	{
		var sprite = GetComponent<SpriteRenderer> ();
		sprite.color = Color.white;

		Color transparent = new Color (1, 1, 1, 0);

		float start_t = Time.time;
		float t = start_t;
		while (t < start_t + duration) 
		{
			float progress = Mathf.Clamp01((t - start_t) / duration);

			sprite.color = Color.Lerp(Color.white, transparent, progress);
			sprite.transform.localScale = Vector3.one*(1f + 3.5f*progress);

			t = Time.time;

			yield return null;
		}
	}

	public void Pop()
	{
		StopAllCoroutines ();
		StartCoroutine (PopAndFade ());
	}

	void Start()
	{
		GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
	}
}
