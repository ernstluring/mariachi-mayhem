using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

	[SerializeField] private GameObject _titleScreen;

	public void StartGame () {
		if (InputManager.Instance.ActiveControllers.Length != 0)
			SceneManager.LoadScene("Scene_Main");
	}

	public void ExitGame () {
		Application.Quit();
	}
}
