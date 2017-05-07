using UnityEngine;

public class WaypointGizmo : MonoBehaviour {

	public float m_size = 1f;
	private Transform[] m_waypoints;
	
	void OnDrawGizmos() {
		m_waypoints = gameObject.GetComponentsInChildren<Transform>();
		Vector3 last = m_waypoints[m_waypoints.Length - 1].position;
		// Make all waypoints point to the next waypoint
		// This way I can set the AICars rotation to the waypoints when respawned.
		for(int i = 1; i < m_waypoints.Length - 1; i++) {	
			m_waypoints[i].LookAt(m_waypoints[i + 1]);
    	}
		for (int i = 1; i < m_waypoints.Length; i++) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(m_waypoints[i].position, m_size);
			Gizmos.DrawLine(last, m_waypoints[i].position);
			last = m_waypoints[i].position;
		}
	}
}
