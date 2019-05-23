using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Form : MonoBehaviour {

	public int id;
	public Color color;
	public bool spawned;
	public Form parent;

	public bool unImportant;
	public bool active;
	public bool etheral;

	public Vector2Int originPoint;
	public Vector2Int centerPoint;
	public Vector2Int[] body;

	public int direction;
	public int height;
	public int width;
	public int length;

	public Sprite[] sprites;
	public int speed;
	public int hyperSpeed = 1;
	[HideInInspector]
	public int speedCounter = 0;
	public Action activeAction;

	public SpriteRenderer skin;
	Animator myGraphics;


	public int xEdge;
	public int yEdge;
	public Cell[,] curMap;
	int lastX;
	int lastY;

	Vector2Int[][] checks;
	Vector2Int[] upCheck;
	Vector2Int[] nwCheck;
	Vector2Int[] leftCheck;
	Vector2Int[] wsCheck;
	Vector2Int[] downCheck;
	Vector2Int[] seCheck;
	Vector2Int[] rightCheck;
	Vector2Int[] enCheck;

	//Switches
	public Action[] colActions;
	public Action[] entActions;

	public List<Form> curCollided;

	bool moving = false;
	public bool dead;

	void Awake(){
		myGraphics = gameObject.GetComponentInChildren<Animator> ();
		skin = gameObject.GetComponentInChildren<SpriteRenderer> ();
		xEdge = Map.S.worldSizeX;
		yEdge = Map.S.worldSizeY;
		curMap = Map.S.world;

		if (hyperSpeed < 1) {
			hyperSpeed = 1;
		}
		curCollided = new List<Form> ();
	}

	/* Detroys Form, removing it from the map and any other objects that may have references to it */
	public void die(){
		if (active) {
			Map.S.forms.Remove (this);
			if (Map.S.forms.Contains (this)) {
				Debug.LogError ("failed, " + gameObject.name + " was not destroyed properly or not activated properly");
			}
		}

		SpriteRenderer sr = gameObject.GetComponentInChildren<SpriteRenderer> ();
		if (sr) {
			Map.S.allSprites.Remove (sr);
		}

		for (int i = 0; i < body.Length; i++) {
			curMap[originPoint.x + body [i].x, originPoint.y + body [i].y].formLeave (this);
		}

		if (gameObject != null) {
			Destroy (this.gameObject);
		}
	}
		

	public void setAnim(string type, bool value){
		myGraphics.SetBool (type, value);
	}

	/* 
	 * Sets direction of Form
	 * direction can be used for backstab checks, or aiming attack etc
	 * also changes the sprites accordingly
	 */

	public void changeDirection(int dir){
		if (myGraphics) {

				if (dir == 7 || dir == 6 || dir == 5) {
					skin.flipX = true;
				}

				if (dir == 2 || dir == 1 || dir == 3) {
					skin.flipX = false;
				}

				if(dir == 3 || dir == 4 || dir == 5){
					myGraphics.SetBool ("backwards", false);
				}

				if(dir == 1 || dir == 0 || dir == 7){
					myGraphics.SetBool ("backwards", true);
				}
		}
		direction = dir;
		if (sprites.Length > 1) {
			if (dir % 2 != 1) {
				if (skin) {
					if (dir > 0) {
						dir /= 2;
					}
					skin.sprite = sprites [dir];
				}
			}
		}
	}

	/*
	 * changes direction of form to best point towards a specific loacation
	 */

	public void changeDirection(Vector2Int dir){
		if (myGraphics) {
			if (lastX != 0 && dir.x != 0 && dir.x != lastX) {
				lastX = dir.x;
			}
			if (lastY != 0 && dir.y != 0 && dir.y != lastY) {
				myGraphics.SetBool ("backward", !myGraphics.GetBool ("backward"));
				lastY = dir.y;
			}
		}
		if (Mathf.Abs (dir.x) > Mathf.Abs (dir.y)) {
			if (dir.x > 0) {
				changeDirection (6);
			} else {
				changeDirection (2);
			}
		} else {
			if (dir.y > 0) {
				changeDirection (0);
			} else {
				changeDirection (4);
			}
		}
	}

	/*
	 * checks whetehr or not this form can interact with other Form through collisions
	 * can't collide with parent form || the other children of that parent || its child form || etheral forms 
	 * in more complicated Forms && movements we want to prevent it from colliding with itself
	 */

	bool checkCol(Form other){
		bool canCollide = false;
		if (other != parent && other.parent != this && !other.etheral && other != this && other != this) {
			if (!parent || parent != other.parent) {
				canCollide = true;
			}
		}
		return canCollide;
	}

	/*
	 * moves Form in a specific direction based on 8 directions, N NW W SW S SE E NE
	 */

	public void move(int dir){
		if (speedCounter == speed) {
			speedCounter = 0;
			if (!dead) {
				//checks for valid direction
				if (dir > -1 && dir < checks.Length) {
					if (!moving) {
						moving = true;
						for(int hs = 0; hs < hyperSpeed; hs++){
							// removes body from map
							erasePresence ();

							changeDirection (dir);

							bool col = false; // bool to determine whether move is legal

							// first we check if the direction is free to be moved into
							for (int i = 0; i < checks [dir].Length; i++) {
							
								int x = centerPoint.x + checks [dir] [i].x;
								int y = centerPoint.y + checks [dir] [i].y;
								// is destination within the map
								if (x > -1 && x < xEdge && y > -1 && y < yEdge) {
									if (!etheral) {
										col = checkSide (dir);
									}
								} else {
									col = true;
								}
							}
							if (!col) {
								// if legal move iterate the originPoint and centerPoint by direction
								switch (dir) {
								case 0:
									originPoint.y += 1;
									centerPoint.y += 1;
									break;
								case 1:
									originPoint.y += 1;
									centerPoint.y += 1;
									originPoint.x -= 1;
									centerPoint.x -= 1;
									break;
								case 2:
									originPoint.x -= 1;
									centerPoint.x -= 1;
									break;
								case 3:
									originPoint.x -= 1;
									centerPoint.x -= 1;
									originPoint.y -= 1;
									centerPoint.y -= 1;
									break;
								case 4:
									originPoint.y -= 1;
									centerPoint.y -= 1;
									break;
								case 5:
									originPoint.y -= 1;
									centerPoint.y -= 1;
									originPoint.x += 1;
									centerPoint.x += 1;
									break;
								case 6:
									originPoint.x += 1;
									centerPoint.x += 1;
									break;
								case 7:
									originPoint.x += 1;
									centerPoint.x += 1;
									originPoint.y += 1;
									centerPoint.y += 1;
									break;
								}
								transform.position = new Vector3 (originPoint.x, originPoint.y, 0); // keeps GameObject synced with collision map to make it easier to find in the inspector
							}
						}
						drawBody ();
						moving = false;
					}


				} else {
					Debug.LogError ("invalid direction " + dir);
				}
			}
		} else {
			speedCounter++;
		}
	}

	/*
	 * teleports Form to a new location
	 */

	public void move(Vector2Int destination){
		if (speedCounter == speed) {
			speedCounter = 0;
			if (!dead) {
				for (int hs = 0; hs < hyperSpeed; hs++) {
					//check that ultimate destination is within the map
					if (destination.x > -1 && destination.x < xEdge && destination.y > -1 && destination.y < yEdge) {

						// removes body from map
						erasePresence ();
					
						changeDirection (destination - originPoint);

						bool col = false;

						for (int i = 0; i < body.Length; i++) {
							int x = body [i].x + destination.x;
							int y = body [i].y + destination.y;
							// check that body is still within map
							if (x > -1 && x < xEdge && y > -1 && y < yEdge) {
								if (!etheral) {
									col = checkBody (destination, true);
								}
							} 
						}

						// if we didn't collide iterate the body
						if (!col) {
							originPoint = destination;
							centerPoint = originPoint + new Vector2Int (width / 2, length / 2);

						}
						// populate Map with body in new position
						drawBody ();
					}
				}
			} else {
				Debug.Log (gameObject.name + " is already dead dont make it move");
			}
		} else {
			speedCounter++;
		}
	}

	/*
	 * Removes body of the Form from the map
	 */

	public void erasePresence(){
		for (int i = 0; i < body.Length; i++) {
			curMap [originPoint.x + body [i].x, originPoint.y + body [i].y].formLeave (this);
		}
	}



	public bool checkSide(int dir){
		bool col = false;

		if (dir < checks.Length && dir > -1) {
			for (int i = 0; i < checks[dir].Length; i++) {
				int x = centerPoint.x + checks[dir] [i].x;
				int y = centerPoint.y + checks[dir] [i].y;
				// is destination within the map
				if (x > -1 && x < xEdge && y > -1 && y < yEdge) {
					Form co;
					// can we collide with anything in the Cell
					if (curMap [x, y].height >= height) {
						for (int k = 0; k < curMap [x, y].within.Count; k++) {
							co = curMap [x, y].within [k];
							// checks if its a valid Form to collide with
							if (checkCol(co)) {
								col = true;
								if (!curCollided.Contains (curMap [x, y].within [k])) {
									curCollided.Add (co);
									collide (co);
									co.collide (this);
								}
							}
						}
					} else { // if not we collide with Forms within the Cell
						for (int k = 0; k < curMap [x, y].within.Count; k++) {
							co = curMap [x, y].within [k];
							// can we collide with the form
							if (checkCol(co)) {
								// enter Cell and call actions on Forms in Cell
								enter (co);
								co.enter (this);
							}
						}
					}
				}
			}
		} else {
			Debug.LogError (gameObject + "invalid direction " + dir);
		}
		return col;
	}

	public bool checkBody(Vector2Int pos, bool colliding){

		bool col = false;

		for (int i = 0; i < body.Length; i++) {
			int x = body [i].x + pos.x;
			int y = body [i].y + pos.y;
			// is destination within the map
			if (x > -1 && x < xEdge && y > -1 && y < yEdge) {
				Form co;
				// can we collide with anything in the Cell
				if (curMap [x, y].height >= height) {
					
					for (int k = 0; k < curMap [x, y].within.Count; k++) {
						co = curMap [x, y].within [k];
						// checks if its a valid Form to collide with
						if (checkCol (co)) {
							col = true;
							if (!curCollided.Contains (curMap [x, y].within [k])) {
								
								curCollided.Add (co);
								if (colliding) {
									co.collide (this);
									collide (co);
								}
							}
						}
					}
				} else {
					// if we can enter Cell call the Forms within enter actions
					for (int k = 0; k < curMap [x, y].within.Count; k++) {
						co = curMap [x, y].within [k];
						// can we collide with the form
						if (checkCol(co)) {
							// enter Cell and call actions on Forms in Cell
							enter (co);
							co.enter (this);
						}
					}
				}
			}
		}

		return col;
	}

	public void collide(Form col){
		for (int i = 0; i < colActions.Length; i++) {
			colActions [i].callAction (col);
		}
	}

	public void enter(Form col){
		for (int i = 0; i < entActions.Length; i++) {
			entActions [i].callAction (col);
		}
	}

	/*
	 * populates Cells in Map with references to this Form
	 */

	public bool drawBody(){
		for(int i = 0; i < body.Length; i++){
			int x = body [i].x + originPoint.x;
			int y = body [i].y + originPoint.y;
			if (x > -1 && x < xEdge && y > -1 && y < yEdge) {
				curMap [x, y].within.Add (this);
				if (!etheral) {
					curMap [x, y].height = height;
				}
			} else {
				Debug.LogError (gameObject.name + " is trying to draw itself out of bounds");
				return false;
			}
		}
		// changes sprite order to create illusion of depth
		if (skin) {
			skin.sortingOrder = Map.S.world [originPoint.x, originPoint.y].heightMod;
		}
		transform.position = new Vector3 (originPoint.x, originPoint.y, 0);
		return true;
	}

	/*
	 * Spawns object into the world
	 */
	public bool spawn(int posX, int posY){
		bool freeSpace = false;
		// checks that pos is within world
		if (posX > -1 && posX < xEdge && posY > -1 && posY < yEdge) {
			// checks that we have a body to spawn
			if (body.Length > 0) {
				Vector2Int p = new Vector2Int (posX, posY);
				// if pos is free set up body
				if (!checkBody (p, false)) {
					freeSpace = true;
					transform.position = new Vector3 (posX, posY, 0);
					originPoint = p;
					centerPoint = originPoint + new Vector2Int (width / 2, length / 2);
					drawBody ();
				} else {
					Debug.Log ("check body fail");
				}
			} else {
				Debug.Log ("body.Length bad = " + body.Length);
			}
		}
		return freeSpace;
	}

	/*
	 * creates a square hit box for the Form
	 * sets up the different directional checks so we dont have to check the whole body everytime we want t omove
	 */

	public void squareBody(){
		// check if dimensions have been set
		if (width > 0 && length > 0) {

			body = new Vector2Int[width * length];
			int cornLen = width + length + 1;
			int count = 0;
			upCheck = new Vector2Int[width];
			int uc = 0;
			nwCheck = new Vector2Int[cornLen];
			int nc = 0;
			leftCheck = new Vector2Int[length];
			int lc = 0;
			wsCheck = new Vector2Int[cornLen];
			int wc = 0;
			downCheck = new Vector2Int[width];
			int dc = 0;
			seCheck = new Vector2Int[cornLen];
			int sc = 0;
			rightCheck = new Vector2Int[length];
			int rc = 0;
			enCheck = new Vector2Int[cornLen];
			int ec = 0;


			int xPosMod = width % 2;
			int yPosMod = length % 2;
			// movs through the whole square body, finding edges
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < length; y++) {
					body [count] = new Vector2Int (x, y);
					count++;
					if (y == length - 1) {
						upCheck [uc] = new Vector2Int (x - (width / 2), length / 2 + yPosMod);
						uc++;
						nwCheck[nc] = new Vector2Int (x - (width / 2), length / 2 + yPosMod);
						nc++;
						enCheck[ec] = new Vector2Int (x - (width / 2), length / 2 + yPosMod);
						ec++;
					}
					if (x == 0) {
						leftCheck [lc] = new Vector2Int (x - (1 + width / 2), y - (length / 2));
						lc++;
						wsCheck[wc] = new Vector2Int (x - (1 + width / 2), y - (length / 2));
						wc++;
						nwCheck[nc] = new Vector2Int (x - (1 + width / 2), y - (length / 2));
						nc++;
					}
					if (y == 0) {
						downCheck [dc] = new Vector2Int (x - (width / 2), y - (1 + length / 2));
						dc++;
						seCheck[sc]= new Vector2Int (x - (width / 2), y - (1 + length / 2));
						sc++;
						wsCheck[wc] = new Vector2Int (x - (width / 2), y - (1 + length / 2));
						wc++;
					}
					if (x == width - 1) {
						rightCheck [rc] = new Vector2Int (xPosMod + width / 2, y - (length / 2));
						rc++;
						seCheck[sc] = new Vector2Int (xPosMod + width / 2, y - (length / 2));
						sc++;
						enCheck[ec] = new Vector2Int (xPosMod + width / 2, y - (length / 2));
						ec++;
					}
				}
			}
			nwCheck [nc] = new Vector2Int (-(1 + width / 2), yPosMod + length / 2);
			wsCheck [wc] = new Vector2Int (-(1 + width / 2), -(1 + length / 2));
			seCheck [sc] = new Vector2Int (xPosMod + width / 2, -(1 + length / 2));
			enCheck [ec] = new Vector2Int (xPosMod + width / 2, yPosMod + length / 2);

			checks = new Vector2Int[8][];
			checks [0] = upCheck;
			checks [1] = nwCheck;
			checks [2] = leftCheck;
			checks [3] = wsCheck;
			checks [4] = downCheck;
			checks [5] = seCheck;
			checks [6] = rightCheck;
			checks [7] = enCheck;

			if (skin) {
				if (!etheral) {
					skin.sortingOrder = Map.S.world [originPoint.x, originPoint.y].heightMod;
				} else {
					skin.sortingOrder = 1000;
				}
			}
		} else {
			Debug.LogError (gameObject.name + " size invalid");
		}
	}
}
