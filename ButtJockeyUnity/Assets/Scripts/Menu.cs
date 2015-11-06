using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {


	public UnityEngine.UI.Text selectedRace;
	public GameObject[] raceList;

	private bool rSpank1 = false;
	private bool lSpank1 = false;
	private bool rSpank2 = false;
	private bool lSpank2 = false;

	private bool player1ready = false;
	private bool player2ready = false;
	public int currentRace = 0;

	void Start()
	{

		DontDestroyOnLoad (this.gameObject);
	}

	IEnumerator DeactivateBoolean(bool b)
	{
		yield return new WaitForSeconds (0.25f);
		b = false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("RightSpank1")) {
			DeactivateBoolean(rSpank1);
			rSpank1 = true;
		}

		if(Input.GetButtonDown ("LeftSpank1"))
	   	{
			DeactivateBoolean(lSpank1);
			lSpank1 = true;
		}

		if (rSpank1 && lSpank1)
			player1ready = true;

		if (Input.GetButtonDown ("RightSpank2")) {
			DeactivateBoolean(rSpank2);
			rSpank2 = true;
		}
		
		if(Input.GetButtonDown ("LeftSpank2"))
		{
			DeactivateBoolean(lSpank2);
			lSpank2 = true;
		}
		
		if (rSpank2 && lSpank2)
			player2ready = true;


		if (this.player1ready && this.player2ready)
			Application.LoadLevel ("main");


		if ((Input.GetButtonDown ("RightSpank1") && !Input.GetButtonDown ("LeftSpank1")) || (Input.GetButtonDown ("RightSpank2") && !Input.GetButtonDown ("LeftSpank2")))
		{
			ChangeRace(1);
		}
		else  if ((!Input.GetButtonDown ("RightSpank1") && Input.GetButtonDown ("LeftSpank1")) || (!Input.GetButtonDown ("RightSpank2") && Input.GetButtonDown ("LeftSpank2")))
		{
			ChangeRace(-1);	
		}
	}	

	void OnGUI()
	{
		this.selectedRace.text = this.raceList [this.currentRace].name;
	}

	void ChangeRace(int nextElement)
	{
		this.currentRace += nextElement;

		if (this.currentRace >= raceList.Length)
			currentRace = 0;
		else if (this.currentRace < 0)
			currentRace = raceList.Length - 1;
	}
}
