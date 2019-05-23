using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : Action {

	public override IEnumerator callAction(int delay) {
		if (speedCounter == speed) {
			speedCounter = 0;
			for (int hs = 0; hs < hyperSpeed; hs++) {
				self.move (4);
			}
		} else {
			speedCounter++;
		}
		yield return new WaitForFixedUpdate();
	}
}
