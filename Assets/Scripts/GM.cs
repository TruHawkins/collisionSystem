using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {
	public GameObject player;

	public static GM S;

	void Awake() {
		S = this;
	}

	void Start() {
		createWalls ();
		spawnObj (Instantiate (player), 50, 50);
		Map.S.startWorld ();
	}

	void Update() {
		if (Input.GetKeyDown ("r")) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);

		}
		if (Input.GetKeyDown ("q")) {
				Application.Quit ();
		}
	}

	void createWalls() {
		int xEdge = Map.S.worldSizeX;
		int yEdge = Map.S.worldSizeY;
		for (int x = 0; x < xEdge; x++) {
			for (int y = 0; y < yEdge; y++) {
				if (x == 0 || x == xEdge - 1 || y == 0 || y == yEdge - 1) {
					Map.S.world [x, y].fill ();
				}
			}
		}
	}


	public void spawnObj(GameObject o, int xPos, int yPos) {
		Form f = o.GetComponent<Form> ();
		f.squareBody ();
		Map.S.spawnForm(o, xPos, yPos);
	}

}
