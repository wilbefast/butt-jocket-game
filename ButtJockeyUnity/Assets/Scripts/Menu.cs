using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {


	public UnityEngine.UI.Image nextRaceButton;
	public UnityEngine.UI.Image prevRaceButton;
	public UnityEngine.UI.Image player1ReadyFeedback;
	public UnityEngine.UI.Image player2ReadyFeedback;

	public Sprite playerIsReady;

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

	IEnumerator  DeactivateBoolean(string boolean)
	{
		yield return new WaitForSeconds (0.3f);
		if (boolean == "r1")
			this.rSpank1 = false;
		else if (boolean == "l1")
			this.lSpank1 = false;
		else if (boolean == "r2")
			this.rSpank2 = false;
		else if (boolean == "l2")
			this.lSpank2 = false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("RightSpank1")) {
			this.rSpank1 = true;
			StartCoroutine("DeactivateBoolean","r1");

		}

		if(Input.GetButtonDown ("LeftSpank1"))
	   	{
			this.lSpank1 = true;
			StartCoroutine("DeactivateBoolean","l1");

		}

		if (this.rSpank1 && this.lSpank1)
		{
			this.player1ReadyFeedback.sprite = this.playerIsReady;
			this.player1ReadyFeedback.GetComponent<ShakeButtInterface>().ShakeInstant();
			this.player1ready = true;
		}

		if (Input.GetButtonDown ("RightSpank2")) {
			this.rSpank2 = true;
			StartCoroutine("DeactivateBoolean","r2");

		}
		
		if(Input.GetButtonDown ("LeftSpank2"))
		{
			this.lSpank2 = true;
			StartCoroutine("DeactivateBoolean","l2");

		}
		
		if (this.rSpank2 && this.lSpank2)
		{
			this.player2ReadyFeedback.sprite = this.playerIsReady;
			this.player2ReadyFeedback.GetComponent<ShakeButtInterface>().ShakeInstant();
			this.player2ready = true;
		}


		if (this.player1ready && this.player2ready)
			Application.LoadLevel ("main");


		if ((Input.GetButtonDown ("RightSpank1") && !this.lSpank1) || (Input.GetButtonDown ("RightSpank2") && !this.lSpank2))
		{
			this.nextRaceButton.GetComponent<ShakeButtInterface>().ShakeInstant();
			ChangeRace(1);
		}
		else  if ((!this.rSpank1 && Input.GetButtonDown ("LeftSpank1")) || (!this.rSpank2 && Input.GetButtonDown ("LeftSpank2")))
		{
			this.prevRaceButton.GetComponent<ShakeButtInterface>().ShakeInstant();
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
