﻿using UnityEngine;
using System.Collections;

public class GameSystem : MonoBehaviour {

	public UnityEngine.UI.Image[] pointCounters;
	public Sprite p1points;
	public Sprite p2points;

	public int totalPoint = 6;
	private int player1Score = 0;
	private int player2Score = 0;

	public UnityEngine.UI.Text countdownText;
	public UnityEngine.UI.Text victoryText;
	private int countdownValue = 3;

	public GameObject player1;
	public GameObject player2;

	private float pongValue = 0f;
	///private bool gameEnded = false;
	

	// Use this for initialization
	void Start () {
		//instantiate race
		Menu menu = GameObject.Find ("race container").GetComponent<Menu> ();
		Instantiate(menu.raceList[menu.currentRace],Vector3.zero,Quaternion.identity);
		Destroy (menu.gameObject);

		this.player1Score = Mathf.RoundToInt(this.totalPoint / 2);
		this.player2Score = Mathf.RoundToInt(this.totalPoint / 2);

		for (int i = 0; i < this.totalPoint; i++) {
			if (i < this.player1Score)
				this.pointCounters[i].sprite = this.p1points;
			else
				this.pointCounters[i].sprite = this.p2points;
		}

		StartCoroutine ("Countdown");
		StartCoroutine ("CountdownBlinck");
	
	}

	public void UpdateScore(string player)
	{
		if (player == "Player1") {
			this.player1Score--;
			this.player2Score++;

			if (this.player2Score <= this.pointCounters.Length)
				this.pointCounters[this.totalPoint - this.player2Score].sprite = this.p2points;

			if (this.player2Score == this.totalPoint)
			{
				//active players
				player1.GetComponent<Avatar>().active = false;
				player2.GetComponent<Avatar>().active = false;
				Debug.Log("Victory P2");
				StartCoroutine(Victory(player));
			}
		}
		else if (player == "Player2") {
			this.player2Score--;
			this.player1Score++;

			if (this.player1Score <= this.pointCounters.Length)
				this.pointCounters[this.player1Score -1].sprite = this.p1points;

			if (this.player1Score == this.totalPoint)
			{
				//active players
				player1.GetComponent<Avatar>().active = false;
				player2.GetComponent<Avatar>().active = false;
				Debug.Log("Victory P1");
				StartCoroutine(Victory(player));
			}
		}
	}

	IEnumerator Victory(string player)
	{
		//gameEnded = true;
		victoryText.text = player + " wins!";
		victoryText.gameObject.SetActive(true);

		yield return new WaitForSeconds (3f);

		Application.LoadLevel ("menu");
	}

	IEnumerator CountdownBlinck()
	{
		while(this.countdownText != null) {
			this.countdownText.fontSize = Mathf.RoundToInt(80 - Mathf.PingPong(pongValue,30));
			pongValue += 2.5f;

			yield return new WaitForSeconds(0.01f);
		}
	}

	IEnumerator Countdown()
	{
		int iteration = this.countdownValue;
		for (int i = 0; i < iteration; i++)
		{
			yield return new WaitForSeconds (1);
			this.countdownValue--;
			//let the countdown blinck
		}

		//active players
		player1.GetComponent<Avatar>().active = true;
		player2.GetComponent<Avatar>().active = true;

		Destroy (this.countdownText.gameObject);
	}

	void OnGUI()
	{
		if (this.countdownText != null)
			this.countdownText.text = this.countdownValue.ToString ();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
