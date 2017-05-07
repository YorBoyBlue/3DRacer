using UnityEngine;

public class Respawn : MonoBehaviour {

	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag == "Water") {
			transform.position = GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>().GetLastWaypoint().position;
			transform.rotation = GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>().GetLastWaypoint().rotation;
		}
	}
}
