using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour {
	public float speed;

	protected Rigidbody _rbody;

	protected virtual void Start () {
		_rbody = GetComponent<Rigidbody>();
	}

	protected void Move (Vector3 velocity) {
		_rbody.MovePosition(_rbody.position + velocity * speed * Time.fixedDeltaTime);
	}

	protected void Bash (float force) {
		Vector3 attackVector = transform.forward * force;
		attackVector = new Vector3(attackVector.x, 3, attackVector.z);
		_rbody.AddForce(attackVector, ForceMode.Impulse);
	}
}
