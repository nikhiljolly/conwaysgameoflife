# Conway's Game of Life

*Simulation in Unity3D  
By: Nikhil Jolly*

## Brief:

This is a simulation project made in Unity3D based on the Conway's Game of Life.  
You can read more about it on the [wiki page](https://en.wikipedia.org/wiki/Conway's_Game_of_Life "Conway's Game of Life"):

## Instructions:

*Open the "Game" scene in Unity.*

1. Create: Used to create a board of cells with specific dimensions.  
  Caution: Keep them close to 100*100 (depending upon the system), otherwise you will face memory issues. No 'hardcore' optimizations have been implemented yet.

2. Create a custom board: After generating a board, touch/cick on the cells to change there states. Clear the board if required.

3. Next: Used to move to next generation of cells based on Conway's rules.

4. Start/Stop: Used to start and stop a simulation of the board, creating next generation cells.

5. Clear: Used to clear the board (kill all the cells).

6. Camera: Slider to vary the size of camera.

7. Speed: Slider to vary the speed of simulation.

8. Gosper's Glider Gun: Used to initiate the cells with seed of Gosper's GG.


## Misc:

1. Shows generation number for each generation of cells.

## For further understanding:

### Basic Flow:

1. Board creation:
	- Board is created as per the dimensions passed whilst using the offset for apt position.
	- Each cell is populated with an array representing all of it's neighbours.
	- Sets neighbour as well as state of cells randomly (with 50% probability of dead and alive).
	- The generation counter is set to 0.

2. Next generation:
	- Makes a copy of the state of the cells and evaluates the next generation of cells whilst modifying any changes to the new copy.
	- Modifies the current cells based on the changes stored in the new copy of states.
	- Also increments the generation counter.

3. Simulation:
	- Calls next generation again and again after a delay whilst incrementing the generation counter.

4. Custom board:
	- The board can be modified at any time by clicking/touching a cell, which results in change of state.

5. Map management (Beta):
	- Can only be used in editor to save and load maps for testing of any map of cells.
	- By default contains a string representing a map of "Gosper's Glider Gun" which can be loaded via a button in the UI.


### Misc notes:

1. Cell:
	- Each is either dead or alive represented by the boolean values as follows; Dead: False, Alive: True
	- Each cell contains an array of neighbours (created on creating the board)
	- Doesn't use MonoBehaviour, instead is represented a custom class (minor memory optimization).
	- Cells start from (0, 0) to (sizeX-1, sizeY-1)

2. Rules followed to evaluate a cell:
	- If cell is alive
		- <2 (0 or 1) alive neighbours: Cell dies
		- >3 alive neighbours: Cell dies
		- 2 or 3 alive neighbours: Cell lives
	- If cell is dead
		- 3 alive neighbours: Cell comes to life
		- Else stays dead

3. Each script is well documented. Kindly refer to them if a part is not understandable.

4. The code isn't highly optimized for handling large amount of dimensions for the board. Keep them close to 100x100 (depending upon the system).


Regards,  
Nikhil Jolly
