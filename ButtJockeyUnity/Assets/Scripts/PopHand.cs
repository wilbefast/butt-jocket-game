using UnityEngine;
using System.Collections;

public class PopHand : MonoBehaviour 
{
	public AudioClip slap;
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
		GameObject slapSource = new GameObject ();
		slapSource.AddComponent<AudioSource>();
		slapSource.GetComponent<AudioSource>().PlayOneShot(slap);
		StopCoroutine ("PopAndFade");
		StartCoroutine ("PopAndFade");
		StartCoroutine ("RemoveSource", slapSource);
	}

	IEnumerator RemoveSource(GameObject slapSource)
	{
		yield return new WaitForSeconds (0.5f);
		Destroy (slapSource);
	}

	void Start()
	{
		GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
	}
}
