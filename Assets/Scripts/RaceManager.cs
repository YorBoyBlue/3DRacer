using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour {

	public float m_player1DistanceToNextCheckpoint;
	public float m_player2DistanceToNextCheckpoint;
	public float m_AI_1DistanceToNextCheckpoint;
	public float m_AI_2DistanceToNextCheckpoint;
	public float m_AI_3DistanceToNextCheckpoint;
	public int m_player1CurrentCheckpoint;
	public int m_player2CurrentCheckpoint;
	public int m_AI_1CurrentCheckpoint;
	public int m_AI_2CurrentCheckpoint;
	public int m_AI_3CurrentCheckpoint;
	public int m_player1CurrentLap;
	public int m_player2CurrentLap;
	public int m_AI_1CurrentLap;
	public int m_AI_2CurrentLap;
	public int m_AI_3CurrentLap;
	private int m_AI_1Ahead;
	private int m_AI_2Ahead;
	private int m_AI_3Ahead;
	private int m_player1Rank;
	public Text m_startTimerText;
	private GameObject[] m_AICars;
	private GameObject[] m_players;
	public UnityStandardAssets.Vehicles.Car.CarAIControl[] m_AIScripts;
	public PlayerManager[] m_playerScripts;
	private float m_startTime = 7f;
	//private int m_amountOfAICarsInRace;
	//private int m_amountOfPlayerCarsInRace;

	// Ranking stuff
	public Text m_playerRankText;
	private string m_trailingRankText = "";
	private string m_rankText = "";


	void Start() {
		m_AICars = GameObject.FindGameObjectsWithTag("AICar");
		m_players = GameObject.FindGameObjectsWithTag("Player");
		//m_amountOfAICarsInRace = m_AICars.Length;
		//m_amountOfPlayerCarsInRace = m_players.Length;
		m_AIScripts = new UnityStandardAssets.Vehicles.Car.CarAIControl[m_AICars.Length];
		m_playerScripts = new PlayerManager[m_players.Length];
		// Freeze all vehicles positions until race start
		foreach(GameObject g in m_players) {
			g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
		foreach(GameObject g in m_AICars) {
			g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
		// Get references to all vehicles scripts and assign the laps for the AI cars		
		for(int i = 0; i < m_players.Length; i++) {
			m_playerScripts[i] = m_players[i].gameObject.GetComponent<PlayerManager>();
		}
		for(int i = 0; i < m_AICars.Length; i++) {
			m_AIScripts[i] = m_AICars[i].gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>();
			m_AIScripts[i].SetMaxLaps(m_playerScripts[0].GetMaxLaps());
		}
	}

	void Update() {
		HandleRaceStart();	
		HandleRanking();
		SetTrailingRankText();	
		m_rankText = m_player1Rank.ToString() + m_trailingRankText;
		m_playerRankText.text = m_rankText;
	}

	void HandleRanking() {
		// Update all vehicles current laps, checkpoints and distances to next checkpoints
		for(int i = 0; i < m_AIScripts.Length; i++) {
			switch(i) {
				case 0: // First AI Car
					m_AI_1CurrentCheckpoint = m_AIScripts[i].GetCheckpoint();
					m_AI_1DistanceToNextCheckpoint = m_AIScripts[i].GetDistanceToNextCheckpoint();
					m_AI_1CurrentLap = m_AIScripts[i].GetLap();
				break;

				case 1: // Second AI Car
					m_AI_2CurrentCheckpoint = m_AIScripts[i].GetCheckpoint();
					m_AI_2DistanceToNextCheckpoint = m_AIScripts[i].GetDistanceToNextCheckpoint();
					m_AI_2CurrentLap = m_AIScripts[i].GetLap();
				break;

				case 2: // Third AI Car
					m_AI_3CurrentCheckpoint = m_AIScripts[i].GetCheckpoint();
					m_AI_3DistanceToNextCheckpoint = m_AIScripts[i].GetDistanceToNextCheckpoint();
					m_AI_3CurrentLap = m_AIScripts[i].GetLap();
				break;

				default:
				break;
			}
		}
		for(int i = 0; i < m_playerScripts.Length; i++) {
			switch(i) {
				case 0: // Player 1
					m_player1DistanceToNextCheckpoint = m_playerScripts[i].GetDistanceToNextCheckpoint();
					m_player1CurrentCheckpoint = m_playerScripts[i].GetCheckpoint();
					m_player1CurrentLap = m_playerScripts[i].GetLap();
				break;

				case 1: // Player 2
					m_player2DistanceToNextCheckpoint = m_playerScripts[i].GetDistanceToNextCheckpoint();
					m_player2CurrentCheckpoint = m_playerScripts[i].GetCheckpoint();
					m_player2CurrentLap = m_playerScripts[i].GetLap();
				break;

				default:
				break;
			}
		}

		// Check which rank player 1 is in
		// For the integers m_AI_1Ahead, m_AI_2Ahead, and m_AI_3Ahead I have used 1 to mean 
		// the AI car is ahead (true), and 0 to mean the AI car is not ahead (false).
		// I did it this way so that I can add the result to the players rank to calculate 
		// what rank the player is in.
		// First AI
		if(m_player1CurrentCheckpoint == m_AI_1CurrentCheckpoint) {
			if(m_player1DistanceToNextCheckpoint > m_AI_1DistanceToNextCheckpoint) {
				m_AI_1Ahead = 1;
			} else {
				m_AI_1Ahead = 0;
			}			
		} else if((m_player1CurrentCheckpoint > m_AI_1CurrentCheckpoint && m_player1CurrentLap == m_AI_1CurrentLap) || m_player1CurrentLap > m_AI_1CurrentLap) {
			m_AI_1Ahead = 0;
		} else if(m_player1CurrentCheckpoint < m_AI_1CurrentCheckpoint || m_player1CurrentLap < m_AI_1CurrentLap) {
			m_AI_1Ahead = 1;
		}
		// Second AI
		if(m_player1CurrentCheckpoint == m_AI_2CurrentCheckpoint) {
			if(m_player1DistanceToNextCheckpoint > m_AI_2DistanceToNextCheckpoint) {
				m_AI_2Ahead = 1;
			} else {
				m_AI_2Ahead = 0;
			}			
		} else if((m_player1CurrentCheckpoint > m_AI_2CurrentCheckpoint && m_player1CurrentLap == m_AI_2CurrentLap) || m_player1CurrentLap > m_AI_2CurrentLap) {
			m_AI_2Ahead = 0;
		} else if(m_player1CurrentCheckpoint < m_AI_2CurrentCheckpoint || m_player1CurrentLap < m_AI_2CurrentLap) {
			m_AI_2Ahead = 1;
		}
		// Third AI
		if(m_player1CurrentCheckpoint == m_AI_3CurrentCheckpoint) {
			if(m_player1DistanceToNextCheckpoint > m_AI_3DistanceToNextCheckpoint) {
				m_AI_3Ahead = 1;
			} else {
				m_AI_3Ahead = 0;
			}			
		} else if((m_player1CurrentCheckpoint > m_AI_3CurrentCheckpoint && m_player1CurrentLap == m_AI_3CurrentLap) || m_player1CurrentLap > m_AI_3CurrentLap) {
			m_AI_3Ahead = 0;
		} else if(m_player1CurrentCheckpoint < m_AI_3CurrentCheckpoint || m_player1CurrentLap < m_AI_3CurrentLap) {
			m_AI_3Ahead = 1;
		}

		// Set Rank for player 1
		for(int i = 0; i < m_AIScripts.Length; i++) {
			if(m_AIScripts[i].HasFinished() && !m_playerScripts[0].HasFinished()) {
				switch(i) {
					case 0:
						m_AI_1Ahead = 1;
					break;

					case 1:
						m_AI_2Ahead = 1;
					break;

					case 2:
						m_AI_3Ahead = 1;
					break;

					default:
					break;
				}
			}
		}
		int tmpRank = 1 + m_AI_1Ahead + m_AI_2Ahead + m_AI_3Ahead;
		if(!m_playerScripts[0].HasFinished()) {
			m_player1Rank = tmpRank;
			m_playerScripts[0].SetRank(m_player1Rank);
		}
	}

	void SetTrailingRankText() {
		// Set the trailing text for displaying the ranking during a race
		switch(m_player1Rank){
			case 1:
				m_trailingRankText = "st";
			 break;
			 
			case 2:
				m_trailingRankText = "nd";
			 break;

			case 3:
				m_trailingRankText = "rd";
			 break;

			case 4:
				m_trailingRankText = "th";
			 break;
			 
			default:
			break;
		}
	}

	void HandleRaceStart() {
		if(m_startTime >= 1f && m_startTime <= 4f) {
			m_startTimerText.text = Timer().ToString();
		} else if (m_startTime < 1f) {
			m_startTimerText.color = Color.green;
			m_startTimerText.text = "G O !!!!";
		}
		if(m_startTime > -5f) {
			m_startTime -= Time.deltaTime;
		}
		if(m_startTime <= 1) {
			StartRace();
		}
		if (m_startTime <= 0) {
			m_startTimerText.gameObject.SetActive(false);
		}
	}

	void StartRace() {
		foreach(GameObject g in m_players) {
			g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		}
		foreach(GameObject g in m_AICars) {
			g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		}
	}

	int Timer() {
		int timer;
		timer = (int)m_startTime;
		return timer;
	}
}
