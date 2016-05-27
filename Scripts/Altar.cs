using UnityEngine;
using System.Collections;

public class Altar : MonoBehaviour {

	[SerializeField] private GameObject[] skullObjects;
	public AudioClip placeSkullClip;
	private AudioSource audioSource;

	private void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	public void PlaceSkullObject (Material skullMat) {
		for (int i = 0; i < skullObjects.Length; i++) {
			if (!skullObjects[i].activeInHierarchy) {
				skullObjects[i].SetActive(true);
				skullObjects[i].GetComponent<MeshRenderer>().material = skullMat;
				break;
			}
		}
		audioSource.Play();
	}

	public void Reset () {
		foreach (GameObject g in skullObjects) {
			g.SetActive(false);
		}
	}
}
