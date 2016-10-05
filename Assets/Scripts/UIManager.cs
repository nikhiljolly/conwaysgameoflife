using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager manager;

	public InputField inputX,inputY;	//Input fields that are used to gather dimensions via UI

	//simulation: Represents the text on the sim button (Start/Stop)
	//generation: Represents the generation no. of the cells
	public Text simulation,generation;

	void Awake(){
		manager = this;
	}

	#region Used by ui

	/// <summary>
	/// Creates the board using the size from the dimensions field in the ui
	/// </summary>
	public void CreateBoard(){

		int sizeX=0, sizeY=0;
		int.TryParse (inputX.text,out sizeX);
		int.TryParse (inputY.text,out sizeY);

		Board.board.CreateBoard (sizeX, sizeY);

	}

	/// <summary>
	/// Used to toggle the simulation, on or off.
	/// Also updates the text of the simulation button.
	/// </summary>
	public void ToggleSimulation(){

		if(Board.board.ToggleSimulation())	//Simulation is running
			simulation.text="Stop";
		else	//Simulation is stopeed
			simulation.text="Start";

	}

	/// <summary>
	/// Used to load the next generation of cells.
	/// </summary>
	public void NextGeneration(){

		Board.board.NextGeneration ();

	}

	/// <summary>
	/// Used to vary the orthographicSize of camera to zoom in and zoom out.
	/// Called via the camera slider.
	/// </summary>
	/// <param name="size">Size.</param>
	public void CameraSize(float size){
		Camera.main.orthographicSize = size;
	}

	/// <summary>
	/// Used to vary the simulation speed.
	/// Called via the speed slider.
	/// </summary>
	/// <param name="delay">Delay.</param>
	public void SimSpeed(float delay){
		#if UNITY_EDITOR	//To remove bug of null reference in editor (called by speed slider)
		if (Board.board!=null)
		#endif
		Board.board.simDelay = delay;
	}

	/// <summary>
	/// Clears the board. Kills all the cells. AAAH!
	/// </summary>
	public void ClearBoard(){
		Board.board.ClearBoard ();
	}

	/// <summary>
	/// Updates the generation no. of cells
	/// </summary>
	public void Generation(int no){
		generation.text = "Generation\n" + no;
	}

	/// <summary>
	/// Used to load the Gosper's GG.
	/// </summary>
	public void GospersGliderGun(){

		Board.board.LoadMap ();	//By default mapString contains json of gospers gg

	}

	#endregion


}
