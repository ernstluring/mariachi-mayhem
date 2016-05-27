using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {

	private static InputManager _instance;
	public static InputManager Instance {
		get { return _instance; }
	}

	private string[] _activeControllers;
	public string[] ActiveControllers { get { return _activeControllers; } }

	[SerializeField] private GameObject[] skulls;

	void Awake () {
		if (_instance == null) {
			_instance = this;
		} else if (_instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (_instance);
	}

	void Start () {
		_activeControllers = Input.GetJoystickNames();
		Debug.Log ("amount of controllers: "+_activeControllers.Length);
		for (int i = 0; i < _activeControllers.Length; i++) {
			skulls[i].GetComponent<MeshRenderer>().material.color = Color.white;
		}
	}
}
