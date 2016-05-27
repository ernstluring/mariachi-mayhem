using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerNumber {
	ONE,
	TWO,
	THREE,
	FOUR
}
[RequireComponent(typeof(AudioSource))]
public class Player : Entity {

	[System.Serializable]
	public class Sounds {
		public AudioClip attackClip;
		public AudioClip gettingHitClip;
		public AudioClip obtainSkullClip;
		public AudioClip placeSkullClip;
		public AudioClip throwSkullClip;
	}
	public Sounds sounds;
	public enum PlayerState { GoingToWalk, Walking, DealingDamage, HasSkull, Stun, Mariachi }
	public PlayerState playerState;
	public int Score { get { return _score; } }

	[SerializeField] private PlayerNumber playerNumber;
	[Range(0, 0.9f)]
	[SerializeField] private float decrementSpeedPercentage = 0.5f;
	[SerializeField] private float bashForce = 10;
	[SerializeField] private float throwForce = 15;
	[SerializeField] private float stunTime = 1;
	[SerializeField] private float skullImpactForce = 10;
	[SerializeField] private float bashImpactForce = 10;
	[SerializeField] private Material skullColorMaterial;
	[SerializeField] private Transform skullHandlePoint;

	private AudioSource audioSource;

	private Vector3 _startPos;
	private float _slowWalkSpeed;
	private float _runSpeed;
	//Key, Value
	private Dictionary<string, string> _controls = new Dictionary<string, string>();
	private Vector3 _velocity;
	private int _score = 0;
	private GameObject _holdingSkull;
	private Animator _anim;

	protected override void Start () {
		base.Start();
		_startPos = transform.position;
		_runSpeed = base.speed;
		_slowWalkSpeed = base.speed - (decrementSpeedPercentage*base.speed);
		switch (playerNumber) {
		case PlayerNumber.ONE:
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
				_controls.Add("Horizontal", "HorizontalP1Windows");
				_controls.Add("Vertical", "VerticalP1Windows");
				_controls.Add("PickUp", "PickUpP1Windows");
				_controls.Add("Attack", "AttackP1Windows");
			}
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
				_controls.Add("Horizontal", "HorizontalP1OSX");
				_controls.Add("Vertical", "VerticalP1OSX");
				_controls.Add("PickUp", "PickUpP1OSX");
				_controls.Add("Attack", "AttackP1OSX");
			}
			break;
		case PlayerNumber.TWO:
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
				_controls.Add("Horizontal", "HorizontalP2Windows");
				_controls.Add("Vertical", "VerticalP2Windows");
				_controls.Add("PickUp", "PickUpP2Windows");
				_controls.Add("Attack", "AttackP2Windows");
			}
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
				_controls.Add("Horizontal", "HorizontalP2OSX");
				_controls.Add("Vertical", "VerticalP2OSX");
				_controls.Add("PickUp", "PickUpP2OSX");
				_controls.Add("Attack", "AttackP2OSX");
			}
			break;
		case PlayerNumber.THREE:
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
				_controls.Add("Horizontal", "HorizontalP3Windows");
				_controls.Add("Vertical", "VerticalP3Windows");
				_controls.Add("PickUp", "PickUpP3Windows");
				_controls.Add("Attack", "AttackP3Windows");
			}
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
				_controls.Add("Horizontal", "HorizontalP3OSX");
				_controls.Add("Vertical", "VerticalP3OSX");
				_controls.Add("PickUp", "PickUpP3OSX");
				_controls.Add("Attack", "AttackP3OSX");
			}
			break;
		case PlayerNumber.FOUR:
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
				_controls.Add("Horizontal", "HorizontalP4Windows");
				_controls.Add("Vertical", "VerticalP4Windows");
				_controls.Add("PickUp", "PickUpP4Windows");
				_controls.Add("Attack", "AttackP4Windows");
			}
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
				_controls.Add("Horizontal", "HorizontalP4OSX");
				_controls.Add("Vertical", "VerticalP4OSX");
				_controls.Add("PickUp", "PickUpP4OSX");
				_controls.Add("Attack", "AttackP4OSX");
			}
			break;
		}
		playerState = PlayerState.Walking;
		audioSource = GetComponent<AudioSource>();
		_anim = GetComponent<Animator>();
	}

	void FixedUpdate () {
		switch (playerState) {
		case PlayerState.GoingToWalk:
			playerState = PlayerState.Walking;
			break;
		case PlayerState.Walking:
			_anim.SetLayerWeight(1,0);
			_anim.SetBool("carry", false);
			_anim.SetBool("mariachi", false);
			Move (_runSpeed);
			if (Input.GetButtonDown(_controls["Attack"])) {
				StartCoroutine(Bash ());
			}
			break;
		case PlayerState.DealingDamage:
			_anim.SetLayerWeight(1,0);
			break;
		case PlayerState.HasSkull:
			if (!_holdingSkull)
				playerState = PlayerState.Walking;
			_anim.SetLayerWeight(1,1);
			_anim.SetBool("carry", true);
			Move (_slowWalkSpeed);
			if (Input.GetButtonDown(_controls["Attack"])) {
				PlayAudioClip(sounds.throwSkullClip);
				StartCoroutine(_holdingSkull.GetComponent<Skull>().Throw(this.transform, throwForce));
				_holdingSkull = null;
				playerState = PlayerState.GoingToWalk;
			}
			break;
		case PlayerState.Stun:
			StartCoroutine (Stunned());
			break;
		case PlayerState.Mariachi:
			_anim.SetLayerWeight(1,0);
			_anim.SetBool("carry", false);
			_anim.SetBool("mariachi", true);
			MoveMariachiMode (base.speed);
			if (Input.GetButtonDown(_controls["Attack"])) {
				StartCoroutine(BashMariatchiMode());
			}
			break;
		}
	}

	private void Move (float speed) {
		_velocity = new Vector3 (Input.GetAxis(_controls["Horizontal"]), 0, Input.GetAxis(_controls["Vertical"])).normalized;
		base.speed = speed;
		if (_velocity != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(_velocity);
			_anim.SetBool("running", true);
			base.Move(_velocity);
		} else {
			_anim.SetBool("running", false);
			base.Move(transform.forward*0.2f);
		}
	}

	private void MoveMariachiMode (float speed) {
		_velocity = new Vector3 (Input.GetAxis(_controls["Horizontal"]), 0, Input.GetAxis(_controls["Vertical"])).normalized;

		if (_velocity != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(_velocity);
			base.Move(_velocity);
		} else {
			base.Move(transform.forward*0.2f);
		}
	}

	private IEnumerator Stunned () {
		yield return new WaitForSeconds (stunTime);
		playerState = PlayerState.Walking;
	}

	private IEnumerator Bash () {
		_anim.SetTrigger("bash");
		playerState = PlayerState.DealingDamage;
		base.Bash(bashForce);
		this.PlayAudioClip (sounds.attackClip);
		yield return new WaitForSeconds(1f);
		if (!_holdingSkull)
			playerState = PlayerState.Walking;
		else
			playerState = PlayerState.HasSkull;
	}

	private IEnumerator BashMariatchiMode () {
		_anim.SetTrigger("bash");
		playerState = PlayerState.DealingDamage;
		base.Bash(bashForce);
		this.PlayAudioClip (sounds.attackClip);
		yield return new WaitForSeconds(1f);
		playerState = PlayerState.Mariachi;
	}

	public void Reset () {
		Debug.Log("reset");
		if (_holdingSkull) {
			_holdingSkull.transform.parent = null;
			_holdingSkull = null;
		}
		transform.position = _startPos;
		playerState = PlayerState.Walking;
	}

	public void GoMariachi () {
		playerState = PlayerState.Mariachi;
	}

	void OnTriggerStay (Collider col) {
		if (col.CompareTag("Skull") && _holdingSkull == null && playerState == PlayerState.Walking) {
			if (Input.GetButtonDown(_controls["PickUp"])) {
				col.transform.SetParent (this.transform);
				col.GetComponent<Skull>().owner = this;
				col.GetComponent<Skull>().DisableGravity();
				_holdingSkull = col.gameObject;
				_holdingSkull.transform.position = skullHandlePoint.position;
				PlayAudioClip(sounds.obtainSkullClip);
				playerState = PlayerState.HasSkull;
			}
		}
		if (col.CompareTag("Altar")) {
			if (Input.GetButtonDown(_controls["PickUp"]) && playerState == PlayerState.HasSkull && _holdingSkull != null){
				Altar altar = col.GetComponent<Altar>();
				if (_holdingSkull) {
//					PlayAudioClip(sounds.placeSkullClip);
					altar.PlaceSkullObject(skullColorMaterial);
					_score++;
					Destroy(_holdingSkull);
					_holdingSkull = null;
					playerState = PlayerState.Walking;
					GameManager.Instance.ItemHasSpawned = false;
				}
			}
		}
	}

	void OnCollisionEnter (Collision c) {
		if (c.transform.CompareTag("Player")) {
			Player other = c.transform.GetComponent<Player>();
			if (other.playerState == PlayerState.DealingDamage) {
				this.PlayAudioClip(sounds.gettingHitClip);
				Vector3 dirVector = transform.position - other.transform.position;
				_rbody.AddForce(dirVector * bashImpactForce, ForceMode.Impulse);
				if (_holdingSkull) {
					Skull skull = _holdingSkull.GetComponent<Skull>();
					_holdingSkull.transform.parent = null;
					skull.EnableGravity();

					_holdingSkull = null;
				}
				_anim.SetTrigger("bash");
				playerState = PlayerState.Stun;
			}
		}
		if (c.transform.CompareTag("Skull")) {
			if (c.transform.GetComponent<Skull>().owner != this) {
				Skull skull = c.transform.GetComponent<Skull>();
				if (skull.skullState == SkullState.Thrown) {
					this.PlayAudioClip (sounds.gettingHitClip);
					playerState = PlayerState.Stun;
					Vector3 dirVector = transform.position - c.transform.position;
					Vector3 dirVector2 = c.transform.position - transform.position;
					_rbody.AddForce(dirVector2 * skullImpactForce, ForceMode.Impulse);
				}
			}
		}
	}

	private void PlayAudioClip (AudioClip audioClip) {
		audioSource.clip = audioClip;
		audioSource.Play ();
	}

	private IEnumerator OnBecameInvisible () {
		Debug.Log("Repositioning over 5 seconds");
		yield return new WaitForSeconds(5);
		transform.position = _startPos;
	}
}
