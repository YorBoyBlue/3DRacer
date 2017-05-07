using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {
	
	public void Retry() {
		SceneManager.LoadScene("Main");
	}
	public void Exit() {
		Application.Quit();
	}
}
