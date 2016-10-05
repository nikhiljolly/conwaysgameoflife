using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// The board that holds each cell for the Conway's Game of Life
/// </summary>
public class Board : MonoBehaviour {
	public static Board board;

	#region Public variables

	public float simDelay;	//Simulation delay (speed) - Managed via UI
	public float factor;	//Used to offset the position of cells when board is created (adds gap in b/w cells)
	public GameObject cellPrefab;	//The main prefab for a cell
	public Color dead,alive;	//Colors for the dead and alive cells - customize via Editor

	#endregion

	#region Private

	/// <summary>
	/// Dimensions of the board in terms of no of horizontal and vertical cells
	/// Usually assigned while the board is created
	/// </summary>
	int sizeX,sizeY;

	/// <summary>
	/// Maintains the generation no. of the cells
	/// </summary>
	int gen;

	/// <summary>
	/// maintains the current state of each cell on the board.
	/// </summary>
	Cell[,] cells;
	/// <summary>
	/// maintains the new state of each cell on the board, used while creating new generation.
	/// </summary>
	bool[,] newCells;

	#endregion

	#region Default Unity fxns

	void Awake(){
		board = this;
	}

	void Update(){

		//Handle the input
		HandleInput ();

		#if UNITY_EDITOR //Map saving and loading for testing in editor
		//Save & load map
		if (Input.GetKeyDown (KeyCode.S))
			SaveMap ();
		if (Input.GetKeyDown (KeyCode.L))
			LoadMap ();
		#endif

	}

	#endregion

	#region Board Creation

	/// <summary>
	/// Creates a (x)*(y) board comprising of cells with offset 'factor' for gaps
	/// Assigns the state of cells randomly
	/// </summary>
	/// <param name="x">Size horizontally.</param>
	/// <param name="y">Size vertically.</param>
	public void CreateBoard(int x, int y){

		//Set board size values
		sizeX = x;
		sizeY = y;

		//Temp
		Cell temp;

		//Destroy previous cells if any
		if (cells != null) {

			//Destroy each cell
			foreach (Cell c in cells)
				Destroy (c.gameObject);

		}

		cells=new Cell[sizeX,sizeY];

		//Horizontal
		for (int p = 0; p < sizeX; p++) {

			//Vertical
			for (int q = 0; q < sizeY; q++) {

				//Instantiate gameobject of cell
				temp=new Cell(Instantiate (cellPrefab) as GameObject);

				//Add to array
				cells[p,q]=temp;

				//Change name (used to get Cell object when using raycast for custom board)
				temp.gameObject.name=+p+"x"+q;


				//Change state: If random no is 1: True(Alive) else no is 0: False(Dead)
				temp.Alive=(Random.Range(0,2)==1);

				//Move to Board Gameobject (for organizing in hierarchy) & position relative to parent
				temp.transform.parent = transform;
				temp.transform.localPosition = new Vector2 (GetPosX (p,sizeX), GetPosY (q,sizeY));

			}

		}

		//Set the neighbours of each cell
		SetNeighbours();

		//Update generation to 0
		Generation(0);

	}

	/// <summary>
	/// Used by CreateBoard to get the x position of cell based on the offset
	/// </summary>
	float GetPosX(int p,int sizeX){
		//1. Find position of tile
		//2. Adjust position to center the board
		return factor * p - (factor / 2f * (sizeX - 1));
	}
	/// <summary>
	/// Used by CreateBoard to get the y position of cell based on the offset
	/// </summary>
	float GetPosY(int q,int sizeY){
		//1. Find position of tile
		//2. Adjust position to center the board
		return -factor * q + (factor / 2f * (sizeY - 1));
	}
	/// <summary>
	/// Used by CreateBoard to set the neighbours of each cell
	/// </summary>
	void SetNeighbours(){

		int size;

		//Horizontal
		for (int p = 0; p < sizeX; p++) {

			//Vertical
			for (int q = 0; q < sizeY; q++) {

				size = 0;

				//Note:
				//The first element is 0,0
				//The last element is sizeX-1,sizeY-1

				//Horizontal (Including H+V)
				if (p + 1 < sizeX) {
					size++;

					if (q + 1 < sizeY)
						size++;
					if (q - 1 >= 0)
						size++;
				}

				if (p - 1 >= 0) {
					size++;

					if (q + 1 < sizeY) 
						size++;
					if (q - 1 >= 0)
						size++;
				}

				//only Vertical
				if (q + 1 < sizeY)
					size++;
				if (q - 1 >= 0)
					size++;

				cells [p, q].neighbours = new Cell[size];

				size = 0;

				//Horizontal (Including H+V)
				if (p + 1 < sizeX) {
					cells [p, q].neighbours [size++] = cells [p + 1, q];

					if (q + 1 < sizeY) 
						cells [p, q].neighbours [size++] = cells [p + 1, q + 1];
					if (q - 1 >= 0)
						cells [p, q].neighbours[size++] = cells[p+1,q-1];
				}
				
				if (p - 1 >= 0) {
					cells [p, q].neighbours [size++] = cells [p - 1, q];

					if (q + 1 < sizeY) 
						cells [p, q].neighbours [size++] = cells [p - 1, q + 1];
					if (q - 1 >= 0)
						cells [p, q].neighbours[size++] = cells[p-1,q-1];
				}

				//only Vertical
				if (q + 1 < sizeY)
					cells [p, q].neighbours [size++] = cells [p, q + 1];
				if (q - 1 >= 0)
					cells [p, q].neighbours[size++] = cells[p,q-1];
				

			}


		}

	}
	#endregion

	#region Public

	/// <summary>
	/// Creates the next generation of cells via evaluating each cell based on Conway's rules.
	/// </summary>
	public void NextGeneration(){

		// Make a copy of state of cells
		newCells = new bool[sizeX,sizeY];

		//Copy cell states to newStates
		for (int p = 0; p < sizeX; p++) {

			for (int q = 0; q < sizeY; q++) {
				newCells [p, q] = cells [p, q].Alive;
			}

		}

		//Evaluate each cell based on location and change the cell value in the newCells
		//(Maintain the original cells in cells for evaluations)
		for (int p = 0; p < sizeX; p++) {

			for (int q = 0; q < sizeY; q++) {
				Evaluate (p,q);
			}

		}

		//Apply new state to the actual cells
		for (int p = 0; p < sizeX; p++) {

			for (int q = 0; q < sizeY; q++) {
				cells[p,q].Alive = newCells[p,q];
			}

		}

		//Increment the generation
		Generation(gen+1);

	}
		
	/// <summary>
	/// Toggles the simulation. Also, returns the result after toggling.
	/// </summary>
	/// <returns><c>true</c>, if simulation was toggled, <c>false</c> otherwise.</returns>
	public bool ToggleSimulation(){
		
		if (simulation == null) {
			
			//Start simulation
			simulation = StartCoroutine (Simulation ());
			return true;

		} else {

			//Stop simulation
			StopCoroutine (simulation);
			simulation = null;
			return false;
		}

	}

	/// <summary>
	/// Clears the board (Kill all the cells..ZEHAHA!)
	/// </summary>
	public void ClearBoard(){

		if (cells == null)
			return;

		//Kill each cell
		foreach (Cell c in cells) {
			c.Alive = false;
		}

		//Set generation to 0
		Generation(0);

	}

	#endregion

	#region Private - Mainly helpers

	//Coroutine used for simulation with delay(speed)
	public Coroutine simulation;
	IEnumerator Simulation(){

		//Keep simulating endlessly
		while (true) {

			//Waits a sec
			yield return new WaitForSeconds (simDelay);
			//Shows the next generation
			NextGeneration ();

		}

	}

	/// <summary>
	/// Evaluate a cell based on it's position in the cells array.
	/// Apply changes to the value of cell state in the newCells array.
	/// Uses Conway's rule to evaluate.
	/// </summary>
	/// <param name="cell">Cell.</param>
	void Evaluate(int x,int y){

		int no = cells[x,y].GetAliveNeighbours ();
		//print ("Cell "+x+"|"+y+": "+no);

		//Cell is alive
		if (cells[x,y].Alive) {

			// <2 (0 or 1) alive neigh,
			// OR >3 alive neigh : Dies
			if (no < 2 || no > 3) {
				newCells[x,y] = false;
			}
			// else (2 or 3 alive neigh) : stays alive

		
		}
		//Cell is dead
		else {
			
			// Exactly 3 alive neigh : Reborn
			if(no==3)
				newCells[x,y]=true;
			
			// else : stays dead

		}

	}

	/// <summary>
	/// Handles mouse input to perform action on any cell.
	/// </summary>
	void HandleInput(){

		//To avoid touching both on the ui and the cell
		if (EventSystem.current.IsPointerOverGameObject (-1))
			return;

		//Mouse down
		if (Input.GetMouseButtonDown (0)) {
			//print ("Mouse down!");

			//Using raycast to detect if any cell is touched
			Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

			//Did raycast hit a cell?
			if(hitInfo && hitInfo.collider.tag=="Cell"){

				//Get cell by name and perform action on it
				GetCell(hitInfo.transform.name).Touched();

			}

		}
	}

	/// <summary>
	/// The cell name is used to extract the location of the cell in the 2d array.
	/// Example cell name: 0x1
	/// </summary>
	/// <returns>The cell.</returns>
	/// <param name="name">Name.</param>
	Cell GetCell(string name){
		//print (name);

		int i=0;
		string strX="", strY="";

		//Get pos x
		for (i=0;i<name.Length;i++) {

			if (name[i] == 'x')	//Break when we find 'x'
				break;
			
			strX += name[i];
				
		}

		//Get pos y (continues i from where we left off)
		for (++i;i<name.Length;i++) {

			strY += name[i];

		}

		//print (strX+"|"+strY);

		return cells [int.Parse(strX),int.Parse(strY)];
	}

	/// <summary>
	/// Updates the generation of cells on the board
	/// </summary>
	/// <param name="no">No.</param>
	void Generation(int no){

		gen = no;
		UIManager.manager.Generation(no);

	}

	#endregion

	#region Maps for testing

	[System.Serializable]
	public struct Map{
		public bool[] mapCells;
	}
	Map map;

	/// <summary>
	/// Used to load gosper's gg in general (assigned json of gospers gg via editor)
	///	Otherwise used to save and load maps for testing in editor
	/// </summary>
	public string mapString;

	/// <summary>
	/// Currently used to save maps for testing in editor
	/// </summary>
	void SaveMap(){

		// Make a copy of state of cells in a flat array
		map.mapCells = new bool[sizeX*sizeY];

		//Copy cell states to mapCell
		int i=0;
		for (int p = 0; p < sizeX; p++) {

			for (int q = 0; q < sizeY; q++) {
				map.mapCells [i++] = cells [p, q].Alive;
			}

		}

		//Convert to string
		mapString = JsonUtility.ToJson (map);

		print (mapString);

	}

	/// <summary>
	/// Used to load gosper's gg in general via mapString
	///	Otherwise used to load maps for testing in editor
	/// </summary>
	public void LoadMap(){

		//Load map from mapString
		map=JsonUtility.FromJson<Map>(mapString);

		//Create the board based on the size of the mapCells
		CreateBoard((int)Mathf.Sqrt(map.mapCells.Length),(int)Mathf.Sqrt(map.mapCells.Length));

		int i = 0;
		//Update the life state of cells based on the map
		for (int p = 0; p < sizeX; p++) {

			for (int q = 0; q < sizeY; q++) {

				cells[p,q].Alive=map.mapCells[i++];

			}
		}

	}

	#endregion


}
