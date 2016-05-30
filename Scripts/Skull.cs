using UnityEngine;
using System;
using System.Collections;

public enum SkullState { Idle, Thrown }

[RequireComponent(typeof(Rigidbody))]
public class Skull : MonoBehaviour {

	public SkullState skullState;

	public Player owner;
	public BoxCollider collisionBox;
	public BoxCollider triggerBox;

	private Rigidbody rb;
	private Collider col;
	private MeshRenderer meshRenderer;


	void Start () {
		meshRenderer = GetComponent<MeshRenderer>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}

	public void EnableGravity () {
		rb.isKinematic = false;
		rb.useGravity = true;
		EnableCollider();
	}

	public void DisableGravity () {
		rb.isKinematic = true;
		rb.useGravity = false;
		DisableCollider ();
	}

	public IEnumerator Throw (Transform dir, float throwForce) {
		transform.parent = null;
		rb.isKinematic = false;
		rb.useGravity = true;

		skullState = SkullState.Thrown;
		Vector3 throwVector = dir.forward * throwForce;
		throwVector = new Vector3(throwVector.x, 3, throwVector.z);
		rb.AddForce(throwVector, ForceMode.Impulse);
		EnableCollider ();

		yield return new WaitForSeconds (2);
		rb.useGravity = false;
		rb.isKinematic = true;
		skullState = SkullState.Idle;
		owner = null;
	}

	public void EnableCollider () {
		collisionBox.enabled = true;
	}

	public void DisableCollider () {
		collisionBox.enabled = false;
	}
}
