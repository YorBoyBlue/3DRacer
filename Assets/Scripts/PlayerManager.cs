using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

	// Best Score stuff
	private float m_startTimer = 6.8f;
	private float m_bestTime;
	private float m_bestTimeTimer = 0;
	public Text m_bestTimeText;
	public Text m_currentTimeText;

	public GameObject m_win;
	public GameObject m_lose;
	public GameObject m_menuPanel;
	public Text m_lapText;
	private Rigidbody m_rb;
	private GameObject m_checkpointsContainer;

	// The first element of the m_checkpoints array includes the parent empty GameObject,
	// When accessing the array make sure to compensate for that. 
	// Starting line of race is element 1 and first checkpoint is element 2, and so on.
	public Transform[] m_checkpoints;
	private Transform m_lastCheckpoint;
	private Transform m_currentCheckpointPosition;
	[SerializeField] private int m_currentCheckpoint;
	private float m_distanceToNextCheckpoint;

	// Rank during race
	public int m_rank;
	private bool m_hasFinished = false;

	// Laps in race
	public int m_lap;
	public int m_laps;
	public int m_maxLaps;
	public Texture2D speedometer;   
	public Texture2D needle;

	private float m_currentSpeed;
	public float CurrentSpeed{ get { return m_rb.velocity.magnitude*2.23693629f; }}
	void OnGUI()   {     
		GUI.DrawTexture(new Rect(Screen.width-300,Screen.height-150,300,150),speedometer);     
		float speedFactor=CurrentSpeed/120f;     // TODO: Refactor: Passing in 300 as top speed but need to get it properly
		float rotationAngle = Mathf.Lerp(0,180,Mathf.Abs(speedFactor));      
		GUIUtility.RotateAroundPivot(rotationAngle,new Vector2(Screen.width-150,Screen.height));
		GUI.DrawTexture(new Rect(Screen.width - 280, Screen.height - 150, 230, 300),needle);   
	}

	void Awake() {
		m_maxLaps = m_laps;
	}

	void Start() {
		m_bestTimeTimer = 0;
		m_bestTime = PlayerPrefs.GetFloat("BestTime");
		m_bestTimeText.text = "BEST TIME - " + m_bestTime.ToString();
		m_win.SetActive(false);
		m_lose.SetActive(false);
		m_menuPanel.SetActive(false);
		m_lap = 1;
		m_rb = GetComponent<Rigidbody>();
		m_checkpointsContainer = GameObject.FindGameObjectWithTag("Checkpoints");
		m_checkpoints = m_checkpointsContainer.GetComponentsInChildren<Transform>();
		// Set the checkpointCounter to 2 because the starting line of race is element 1 and first checkpoint is element 2
		m_currentCheckpoint = 2;
		// Set the last checkpoint to the starting line and m_currentCheckpoint to the 1st checkpoint
		m_lastCheckpoint = m_checkpoints[1];
		m_currentCheckpointPosition = m_checkpoints[2];
		// Make all checkpoints look at the next checkpoint excluding the finish line
		// This way I can set the players rotation to the checkpoints when respawned.
		for(int i = 1; i < m_checkpoints.Length - 1; i++) {	
			m_checkpoints[i].LookAt(m_checkpoints[i + 1]);
    	}
		// Set all checkpoints to inactive except the first and second one because the 
		// first is the parent empty gameobject and the second is the starting line.
		m_checkpoints[1].gameObject.SetActive(false);
		for(int i = 3; i < m_checkpoints.Length; i++) {
			m_checkpoints[i].gameObject.SetActive(false);
		}
	}

	void Update() {
		if(m_startTimer > 0) {
			m_startTimer -= Time.deltaTime;
		}
		if(m_startTimer <= 0) {
			m_bestTimeTimer += Time.deltaTime;
		}
		float tmpBestTime = Mathf.Round(m_bestTimeTimer * 100f) / 100f;
		m_currentTimeText.text = tmpBestTime.ToString();
		PlayerInput();
		m_checkpoints[1].LookAt(m_checkpoints[2]);
		m_distanceToNextCheckpoint = Vector3.Distance(m_currentCheckpointPosition.position, transform.position);
		m_lapText.text = "Lap " + m_lap.ToString() + "/" + m_maxLaps.ToString();
	}

	void PlayerInput() {
		if(Input.GetKeyDown(KeyCode.R)) {
			Respawn();
		}
	}
	
	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag == "Water") {
			Respawn();
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.tag == "Checkpoint") {
			// Set this to the lastCheckpoint for respawning
			m_lastCheckpoint = m_checkpoints[m_currentCheckpoint];
			// Deactivate this GameObject
			other.gameObject.SetActive(false);
			// Increment the currentCheckpoint
			m_currentCheckpoint++;
			// Set currentCheckpoint to the next checkPoint
			m_currentCheckpointPosition = m_checkpoints[m_currentCheckpoint];
			m_distanceToNextCheckpoint = Vector3.Distance(m_currentCheckpointPosition.position, transform.position);
			// Handle decrementing the laps
			if(m_currentCheckpoint >= (m_checkpoints.Length - 1)) {
				m_lap++;
				if(m_lap <= m_maxLaps) {
					m_currentCheckpoint = 2;
				}
				if(m_lap > m_maxLaps) {
					m_lap = m_maxLaps;
				}
			}
			// Activate the next checkpoint
			m_checkpoints[m_currentCheckpoint].gameObject.SetActive(true);
		}
		if(other.tag == "Finish") {
			m_hasFinished = true;
			if(m_rank == 1 && m_lose.activeSelf == false) {
				m_win.SetActive(true); // TODO: Refactor
			} else if(m_rank != 1 && m_win.activeSelf == false) {
				m_lose.SetActive(true); // TODO: Refactor
			}
			// Set Best Score
			float tmpBestTime = Mathf.Round(m_bestTimeTimer * 100f) / 100f;
			if(tmpBestTime < m_bestTime) {
				PlayerPrefs.SetFloat("BestTime", tmpBestTime);
				m_bestTimeText.text = "NEW BEST TIME!! - " + tmpBestTime.ToString();
			}
			m_menuPanel.SetActive(true);
		}
	}

	void Respawn() {
		// Lift off the ground a little to prevent spawning in the terrain
		Vector3 offsetY = Vector3.up * 2f;
		// Zero out the velocity and angularVelocity of the vehicle
		m_rb.velocity = Vector3.zero;
		m_rb.angularVelocity = Vector3.zero;
		// Set the vehicle the the lastCheckpoints rotation which is looking at the nextCheckpoint
		transform.rotation = m_lastCheckpoint.rotation;
		// Set the position to the lastCheckpoint plus the offSet
		transform.position = m_lastCheckpoint.position + offsetY;
	}

	// Getters and Setters
	public int GetCheckpoint() {
		return m_currentCheckpoint;
	}

	public int GetRank() {
		return m_rank;
	}

	public void SetRank(int value) {
		m_rank = value;
	}

	public float GetDistanceToNextCheckpoint() {
		return m_distanceToNextCheckpoint;
	}

	public int GetMaxLaps() {
		return m_maxLaps;
	}

	public int GetLap() {
		return m_lap;
	}

	public bool HasFinished() {
		return m_hasFinished;
	}
}
