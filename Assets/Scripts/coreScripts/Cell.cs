using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

	public int heightMod;
	public int height;
	public List<Form> within;

	public SpriteRenderer sr;

	void Awake () {
		
		within = new List<Form> ();
		sr = gameObject.GetComponent<SpriteRenderer> ();
	}

	public bool formLeave(Form f){
		if (within.Remove (f)) {
			if (within.Count > 0) {
				bool onlyTheGhosts = true;
				height = -10;
				for (int i = 0; i < within.Count; i++) {
					if (!within [i].etheral) {
						if (height < within [i].height) {
							height = within [i].height;
						}
						onlyTheGhosts = false;
					}
				}
				if (onlyTheGhosts) {
					height = 0;
				}
			} else {
				height = 0;
			}
			return true;
		} else {
			return false;
		}
	}

	public void fill(){
		if (height == 0) {
			within.Add (Map.S.terrain);
			height = 1000;
		}
	}

	public void empty(){
		height = 0;
		within = new List<Form> ();
	}

	public void hide(){
		Color tmp = sr.color;
		tmp.a = 0;
		sr.color = tmp;
	}

	public void reveal(){
		Color tmp = sr.color;
		tmp.a = 1;
		sr.color = tmp;
	}
}
