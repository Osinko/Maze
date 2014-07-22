using UnityEngine;
using System.Collections;

public class Maze : MonoBehaviour
{
		public IntVector2 size;
		public MazeCell cellPrefab;
		MazeCell[,] cells;

		void Start ()
		{
	
		}
	
		void Update ()
		{
	
		}

		public float generationStepDelay;

		public IEnumerator Generate ()
		{
				WaitForSeconds delay = new WaitForSeconds (generationStepDelay);
				cells = new MazeCell[size.x, size.z];
				for (int x = 0; x < size.x; x++) {
						for (int z = 0; z < size.z; z++) {
								yield return delay;
								CreateCell (new IntVector2 (x, z));
						}
				}
		}

		void CreateCell (IntVector2 coordinates)
		{
				MazeCell newCell = Instantiate (cellPrefab) as MazeCell;
				cells [coordinates.x, coordinates.z] = newCell;
				newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
				newCell.transform.parent = transform;
				newCell.transform.localPosition = new Vector3 (coordinates.x - size.x * 0.5f + 0.5f, 0, coordinates.z - size.z * 0.5f + 0.5f);
		}
}
