using UnityEngine;
using System.Collections;

public class Mariachi : MonoBehaviour {

	[SerializeField] private float walkSpeed = 9;
	[SerializeField] private float curveSpeed = 9;

	private Transform _target;
	private CharacterController _charController;
	private bool _walkToTarget = false;
	private NavMeshAgent agent;
	private Animator anim;
	private AudioSource audioSource;

	void Awake () {
		GameManager.Instance.GetAudioSource.clip = GameManager.Instance.gameMusic.mariachiRoundMusic;
		GameManager.Instance.GetAudioSource.Play();

		_charController = GetComponent<CharacterController>();
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
	}
	
	void Update () {
		if (_walkToTarget) {

			agent.destination = _target.position;

			StartCoroutine(Timer());
		}
	}

	private IEnumerator Timer () {
		yield return new WaitForSeconds(15);
		GameManager.Instance.NextGameRound();
		gameObject.SetActive(false);
		Destroy (gameObject);
	}

	public void SetTarget (Transform target) {
		_target = target;
		StartCoroutine(SetTarget());
	}

	private IEnumerator SetTarget () {
		yield return new WaitForSeconds(3);
		_walkToTarget = true;
	}

	void OnCollisionEnter (Collision c) {
		if (c.transform == _target) {
			StartCoroutine (Eat(c.gameObject));
		}
	}

	private IEnumerator Eat (GameObject target) {
		anim.SetTrigger("eat");
		yield return new WaitForSeconds(2);
		audioSource.Play();
		gameObject.SetActive(false);
		GameManager.Instance.EatSenor(target);
		Destroy(target);
		GameManager.Instance.NextGameRound();
		Destroy (gameObject);
	}
}
