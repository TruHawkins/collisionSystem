using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour {

	[HideInInspector]
	public Form self;
	public int range;
	public int speedCounter;
	public int speed;
	public int hyperSpeed = 1;

	public virtual void Awake(){
		self = gameObject.GetComponent<Form> ();
		if (!self) {
			self = gameObject.GetComponentInParent<Form> ();
			if (!self) {
				Debug.LogError ("no form attached to this object");
			}
		}
		if (hyperSpeed < 1) {
			hyperSpeed = 1;
		}
	}

	public virtual IEnumerator callAction(int delay) {yield return new WaitForFixedUpdate();}

	public virtual void callAction(Form other) {}

	public virtual void updateObj() {}

}
