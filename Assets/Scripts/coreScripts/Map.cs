using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public static Map S;
	public int seed;
	bool flowing = true;

	public Cell[,] world;

	public int worldSizeX;
	public int worldSizeY;

	public List<Form> forms;
	public List<SpriteRenderer> allSprites;

	public GameObject DebugCell;

	[HideInInspector]
	public Form terrain;// terrain is inert Forms used to block out space
	public GameObject voidForm; // inert Form

	void Awake(){
		S = this;
		GameObject tmp = Instantiate (voidForm);
		terrain = tmp.GetComponent<Form> (); 

		world = new Cell[worldSizeX, worldSizeY];
		forms = new List<Form> ();
		allSprites = new List<SpriteRenderer> ();

		if (seed != 0) {
			Random.InitState (seed);
		}

		spawnWorld ();

		debugDraw ();
	}

	/* 
	 * Instantiate the Cells that make up the visuals of the world
	 */

	void spawnWorld(){
		for (int x = 0; x < worldSizeX; x++) {
			for (int y = 0; y < worldSizeY; y++) {
				GameObject tmp = Instantiate (DebugCell, new Vector3 (x, y, 0), transform.rotation, transform) as GameObject;
				world [x, y] = tmp.GetComponent<Cell> ();
				world [x, y].heightMod += worldSizeY - y; // heightMod used to create illsuion of depth, added to sprite sorting order in Form.cs
			}
		}
	}

	/*
	 * checks if Cell is empty
	 */

	public bool checkCell(Vector2Int pos){
		Cell c = world [pos.x, pos.y];
		if (c.height == 0) {
			return true;
		} else {
			return false;
		}
	}

	/*
	 * Takes a Gameobjects and its Form script and spawns the Form into the World as given position
	 */

	public void spawnForm(GameObject newForm, int x, int y){
		Form f = newForm.GetComponent<Form>();
		if (f.spawn(x, y)) {
			// add to list of active Forms, if it is active
			if (f.active) {
				forms.Add (f);
			}
			// saves its sprite
			SpriteRenderer sr = newForm.GetComponentInChildren<SpriteRenderer> ();
			if (sr) {
				allSprites.Add (sr);
			}
			// draws world to include new Form
			debugDraw ();
			f.spawned =  true;
		} else {
			//newForm.die ();
			f.spawned = false;
		}
	}

	/*
	 * Calls all of the actions of active Forms
	 */

	public IEnumerator callActions(){
		while (flowing) {
			for (int i = 0; i < forms.Count; i++) {
				if (forms [i]) {
					if (forms [i].active  && forms[i].activeAction) {
						StartCoroutine(forms[i].activeAction.callAction(0));
					}
				}
			}
			debugDraw ();
			yield return new WaitForFixedUpdate ();

		}
	}

	void Update(){
		if (Input.GetKeyDown ("p")) {
			debugStep ();
		}
	}

	/* 
	 * steps through actions of Active Forms one by one
	 */ 

	void debugStep(){
		for (int i = 0; i < forms.Count; i++) {
			forms [i].activeAction.callAction (0);
		}
	}

	/*
	 * draws out the hit boxes of all the Forms in the Map
	 */

	public void debugDraw(){
		Cell[,] cur;
		int xEdge;
		int yEdge;
		cur = world;
		xEdge = worldSizeX;
		yEdge = worldSizeY;

			for (int x = 0; x < xEdge; x++) {
				for (int y = 0; y < yEdge; y++) {
					if (cur [x, y].height > 0) {
						cur [x, y].sr.color = cur [x, y].within [0].color;
					} else {
						cur [x, y].sr.color = new Color (0,0,0);
					}
				}
			}
	}

	/*
	 * pauses the World flow
	 */

	public IEnumerator pauseWorld(){
		flowing = false;
		yield return new WaitForSeconds (1);
		List<Form> deadList = new List<Form> ();
		//gathers unimportant Forms to have them destroyed
		for (int i = 0; i < forms.Count; i++) {
			if (forms [i].unImportant) {
				deadList.Add (forms [i]);
			}
		}
		int count = deadList.Count;
		for (int i = 0; i < count; i++) {
			deadList[i].die ();
		}
	}

	public void startWorld(){
		flowing = true;
		StartCoroutine ("callActions");
	}

}
