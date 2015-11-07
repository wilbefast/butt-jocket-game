using UnityEngine;
using System.Collections;

public class GameSystem : MonoBehaviour {
	
	public UnityEngine.UI.Image[] pointCounters;
	public Sprite p1points;
	public Sprite p2points;
	
	public GameObject defaultTrack;
	
	public int totalPoint = 6;
	private int player1Score = 0;
	private int player2Score = 0;
	
	public UnityEngine.UI.Text countdownText;
	public UnityEngine.UI.Text victoryText;
	private int countdownValue = 3;
	
	public GameObject player1;
	public GameObject player2;
	
	private float pongValue = 0f;
	private bool destructingCountdown = false;
	///private bool gameEnded = false;
	
	
	// Use this for initialization
	void Start () {
		//instantiate race
		GameObject raceContainer = GameObject.Find ("race container");	
		if (raceContainer == null) {
			defaultTrack.SetActive(true);
		}
		else
		{
			Menu menu = raceContainer.GetComponent<Menu> ();
			Instantiate(menu.raceList[menu.currentRace],Vector3.zero,Quaternion.identity);
			Destroy (menu.gameObject);
			
		}
		
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
			{
				this.pointCounters[this.totalPoint - this.player2Score].sprite = this.p2points;
				this.pointCounters[this.totalPoint - this.player2Score].GetComponent<GainPoint>().StartCoroutine("UpdateState");
			}
			
			if (this.player2Score == this.totalPoint)
			{
				//active players
				player1.GetComponent<Avatar>().active = false;
				player2.GetComponent<Avatar>().active = false;
				this.victoryText.color = new Color(1,1,1);
				StartCoroutine(Victory(player));
			}
		}
		else if (player == "Player2") {
			this.player2Score--;
			this.player1Score++;
			
			if (this.player1Score <= this.pointCounters.Length)
			{
				this.pointCounters[this.player1Score -1].sprite = this.p1points;
				this.pointCounters[this.player1Score -1].GetComponent<GainPoint>().StartCoroutine("UpdateState");

			}
			
			if (this.player1Score == this.totalPoint)
			{
				//active players
				player1.GetComponent<Avatar>().active = false;
				player2.GetComponent<Avatar>().active = false;
				this.victoryText.color = new Color(1,1,1);
				StartCoroutine(Victory(player));
			}
		}
	}
	
	IEnumerator Victory(string player)
	{
		//gameEnded = true;
		victoryText.text = (player == "Player1" ? "Wooster" : "Jeeves") + " wins!";
		victoryText.gameObject.SetActive(true);
		
		yield return new WaitForSeconds (3f);
		
		Application.LoadLevel ("menu");
	}
	
	IEnumerator CountdownBlinck()
	{
		while(this.countdownText != null) {
			this.countdownText.fontSize = Mathf.RoundToInt(150 - Mathf.PingPong(pongValue,100));
			pongValue += 5f;
			
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
		
		
	}
	
	IEnumerator EndRace()
	{
		//deactive players
		player1.GetComponent<Avatar>().active = false;
		player2.GetComponent<Avatar>().active = false;
		
		if (this.player1Score > this.player2Score)
		{
			this.victoryText.color = new Color(1, 1, 1);
			victoryText.text = "Wooster wins!";
		}
		else if (this.player1Score < this.player2Score)
		{
			this.victoryText.color = new Color(1, 1, 1);
			victoryText.text = "Jeeves wins!";
		}
		else 
			victoryText.text = "Draw!";
		
		victoryText.gameObject.SetActive(true);
		
		yield return new WaitForSeconds (3f);
		
		Application.LoadLevel ("menu");
		
		
	}
	
	IEnumerator DestroyCountdown()
	{
		yield return new WaitForSeconds (0.25f);
		Destroy (this.countdownText.gameObject);
	}
	
	void OnGUI()
	{
		if (this.countdownText != null)
		{
			this.countdownText.text = this.countdownValue > 0 ? this.countdownValue.ToString () : "SPANK!";
			
			if (this.countdownValue <= 0 && !destructingCountdown)
			{
				destructingCountdown = true;
				StartCoroutine("DestroyCountdown");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
