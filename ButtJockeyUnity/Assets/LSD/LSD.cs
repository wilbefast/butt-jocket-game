using UnityEngine;
using System.Collections;

public class LSD : MonoBehaviour 
{
	private static LSD _instance;

	public static LSD instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<LSD>();
				if(_instance == null)
				{
					_instance = new GameObject("LSD").AddComponent<LSD>();
				}
			}
			return _instance;
		}
	}

	Vector2 pos;

	Vector2 desiredPos;

	IEnumerator changeDesiredPosPeriodically()
	{
		while(enabled)
		{
			desiredPos = Random.insideUnitCircle;

			yield return new WaitForSeconds(Random.Range(1f, 2f));
		}
	}

	Material material, materialAvatar;

	public void init()
	{
	}

	void Start()
	{
		pos = new Vector2 (0f, 0f);
		desiredPos = new Vector2 (0f, 0f);

		StartCoroutine (changeDesiredPosPeriodically ());

		material = (GameObject.Find ("TrackPiece") as GameObject).GetComponent<Renderer> ().sharedMaterial;
		materialAvatar = (GameObject.Find ("AvatarBall") as GameObject).GetComponent<Renderer> ().sharedMaterial;
	}

	void Update()
	{
		pos = Vector2.Lerp (pos, desiredPos, Time.deltaTime / 3f);

		material.SetFloat ("_X", pos.x);
		material.SetFloat ("_Y", pos.y);
		materialAvatar.SetFloat ("_X", pos.x);
		materialAvatar.SetFloat ("_Y", pos.y);
	}
}
