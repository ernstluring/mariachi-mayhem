using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum GameRounds { Starting, SacrificeRound, MariachiRound, None }

public class GameManager : MonoBehaviour {

	[System.Serializable]
	public class Music {
		public AudioClip sacrificeRoundMusic;
		public AudioClip mariachiRoundMusic;
	}
	public Music gameMusic;

	private static GameManager _instance;
	public static GameManager Instance {
		get { return _instance; }
	}

	[SerializeField] private GameObject _menuScreen;
	[SerializeField] private Transform[] _skullSpawnPoints;
	[SerializeField] private Altar _altar;
	[SerializeField] private GameObject _skullObject;
	[SerializeField] private GameObject _mariachi;
	[SerializeField] private List<Player> _players;

	private int _amountOfPlayers;
	private List<GameObject> _skullsInField = new List<GameObject>();
	private GameRounds _gameRound;
	private bool _itemHasSpawned = false;
	public bool ItemHasSpawned { set {_itemHasSpawned = value;} }
	private bool _mariachiHasSpawned = false;
	public bool MariachiHasSpawned { set {_mariachiHasSpawned = value;}}

	private int _timesSkullHasSpawned;
	private AudioSource _audioSource;
	public AudioSource GetAudioSource {get {return _audioSource;}}

	void Awake () {
		if (_instance == null) {
			_instance = this;
		} else if (_instance != this) {
			Destroy (gameObject);
		}
		_audioSource = GetComponent<AudioSource>();
		DontDestroyOnLoad (_instance);
	}

	void Start () {
		_amountOfPlayers = InputManager.Instance.ActiveControllers.Length;

		for (int i = 0; i < _amountOfPlayers; i++) {
			_players[i].gameObject.SetActive(true);
		}

		for (int i = 0; i < _amountOfPlayers; i++) {
			if (!_players[i].gameObject.activeInHierarchy) {
				_players.Remove(_players[i]);
				Destroy(_players[i].gameObject);
			}
		}
		_gameRound = GameRounds.SacrificeRound;
	}

	void Update () {
		
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.B)) {
			_gameRound = GameRounds.MariachiRound;
		}
		#endif

		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadScene(0);
		}

		switch (_gameRound) {
		case GameRounds.SacrificeRound:
			StartCoroutine (SacrificeRound ());
			break;
		case GameRounds.MariachiRound:
			StartCoroutine (MariachiRound ());
			break;
		case GameRounds.None:
			break;
		}
	}

	private IEnumerator SacrificeRound () {
		if (!_itemHasSpawned && _timesSkullHasSpawned <= 5) {
			int r = Random.Range (0, 4);
			SpawnSkull (r);
			_itemHasSpawned = true;
			_timesSkullHasSpawned++;
		} else if (_timesSkullHasSpawned > 5) {
			foreach (Player p in _players) {
				p.Reset();
			}
			_gameRound = GameRounds.MariachiRound;

		}
		yield return null;
	}

	private void SpawnSkull (float number) {
		if (number == 0) {
			GameObject o = Instantiate (_skullObject, _skullSpawnPoints[0].position, _skullObject.transform.rotation) as GameObject;
			_skullsInField.Add(o);
		} else if (number == 1) {
			GameObject o = Instantiate (_skullObject, _skullSpawnPoints[1].position, _skullObject.transform.rotation) as GameObject;
			_skullsInField.Add(o);
		} else if (number == 2) {
			GameObject o =Instantiate (_skullObject, _skullSpawnPoints[2].position, _skullObject.transform.rotation) as GameObject;
			_skullsInField.Add(o);
		} else {
			GameObject o = Instantiate (_skullObject, _skullSpawnPoints[3].position, _skullObject.transform.rotation) as GameObject;
			_skullsInField.Add(o);
		}
	}

	private IEnumerator MariachiRound () {
		if (!_mariachiHasSpawned) {

			if (_skullsInField.Count > 0) {
				foreach (GameObject g in _skullsInField) {
					Destroy(g);
				}
				_skullsInField.Clear();
			}

			Player playerToTarget = _players[0];
			for (int i = 0; i < _players.Count; i++) {
				if (playerToTarget.Score > _players[i].Score) {
					playerToTarget = _players[i];
				}
			}

			foreach (Player p in _players) {
				p.GoMariachi();
			}

			GameObject obj = Instantiate(_mariachi);
			Mariachi m = obj.GetComponent<Mariachi>();
			Debug.Log (playerToTarget.name);
			m.SetTarget(playerToTarget.transform);
			_mariachiHasSpawned = true;
		}
		yield return null;
	}

	public void EatSenor (GameObject p) {
		_players.Remove(p.GetComponent<Player>());
	}

	public void AdvanceGameRound (GameRounds roundToGo) {
		_gameRound = roundToGo;
	}

	public void NextGameRound () {
		foreach (Player p in _players) {
			p.Reset();
		}
		_audioSource.Stop();
		_audioSource.clip = gameMusic.sacrificeRoundMusic;
		_audioSource.Play();
		_itemHasSpawned = false;
		_timesSkullHasSpawned = 0;
		_altar.Reset();
		if (_skullsInField.Count > 0) {
			foreach (GameObject g in _skullsInField) {
				Destroy(g);
			}
			_skullsInField.Clear();
		}
		_mariachiHasSpawned = false;
		_gameRound = GameRounds.SacrificeRound;
	}
}
