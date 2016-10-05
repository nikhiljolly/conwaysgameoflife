using System;
using UnityEngine;

/// <summary>
/// Custom class to represent a cell. Maintains the position, neighbours and state of the cell on the board.
/// Doesn't use the MonoBehaviour class (minor memory optimization). Thereby, isn't attached to each Cell.
/// </summary>
public class Cell{

	#region	Public

	public Cell[] neighbours;	//All the neighbours of the cell
	public GameObject gameObject;
	public Transform transform;

	#endregion

	#region Private

	bool alive;	//Alive: True, Dead: False
	SpriteRenderer spriteRen;

	#endregion

	#region Custom

	/// <summary>
	/// Initializes a new instance of the <see cref="Cell"/> class.
	/// Constructor: Used to pass the gameobject through which sprite renderer and transform references are gathered for later use.
	/// </summary>
	/// <param name="g">The gameobject.</param>
	public Cell(GameObject g){
		gameObject = g;
		spriteRen = g.GetComponent<SpriteRenderer> ();
		transform = g.GetComponent<Transform> ();
	}

	/// <summary>
	///	Returns the no. of alive neighbours of the cell.
	/// </summary>
	/// <returns>The alive neighbours.</returns>
	public int GetAliveNeighbours(){

		int no = 0;

		foreach (Cell c in neighbours) {

			if (c.Alive) {
				//Debug.Log (c.gameObject.name+" alive!");
				no++;
			}

		}


		return no;

	}

	//Getter & Setter for alive variable
	public bool Alive{
		get{
			return alive;
		}
		set{
			//Sets the state of the cell to either alive or dead
			//Alive: True-Black and Dead: False-White
			alive = value;
			if (value) {
				//Debug.Log ("set "+gameObject.name+" alive");
				spriteRen.color = Board.board.alive;
			}
			else
				spriteRen.color = Board.board.dead;
		}
	}
		
	//Called when cell is touched
	public void Touched(){

		//Toggle cell life
		Alive = !alive;

	}

	#endregion

}

