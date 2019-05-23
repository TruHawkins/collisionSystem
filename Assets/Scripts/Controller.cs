using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	public int jumpPower;
	Form self;

	void Awake() {
		self = gameObject.GetComponent<Form> ();
	}

	void Update () {
		if(Input.GetKeyUp("w")) {
			StartCoroutine (jump ());
		}
		if(Input.GetKey("a")) {
			self.move (2);
		}
		if(Input.GetKey("d")) {
			self.move (6);
		}
	}

	IEnumerator jump () {
		for (int i = 0; i < jumpPower; i++) {
			self.move (0);
			yield return new WaitForFixedUpdate ();
		}
	}
}
