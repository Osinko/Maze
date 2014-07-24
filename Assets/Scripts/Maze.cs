using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
		public IntVector2 size;
		public MazeCell cellPrefab;
		public MazePassage passagePrefab;	//通路
		public MazeWall wallPrefab;			//壁

		MazeCell[,] cells;

		public float generationStepDelay;	//遅延時間

		public IEnumerator Generate ()
		{
				WaitForSeconds delay = new WaitForSeconds (generationStepDelay);
				cells = new MazeCell[size.x, size.z];
				List<MazeCell> activeCells = new List<MazeCell> ();
				DoFirstGenerationStep (activeCells);
				
				while (activeCells.Count >0) {
						yield return delay;
						DoNextGenerationStep (activeCells);
				}
		}

		void DoFirstGenerationStep (List<MazeCell> activeCells)
		{
				activeCells.Add (CreateCell (IntVector2.RandomVector (size)));
		}

		void DoNextGenerationStep (List<MazeCell> activeCells)
		{
				int currentIndex = activeCells.Count - 1;				//リストの後ろから
				MazeCell currentCell = activeCells [currentIndex];

				if (currentCell.IsFullyInitialized) {					//４方向の情報が揃っていた場合
						activeCells.RemoveAt (currentIndex);			//アクティブセルから外す
						return;
				}

				MazeDirection direction = currentCell.RandomUninitializedDirection;
				IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector ();
				if (coordinates.Contains (size)) {
						MazeCell neighbor = GetCell (coordinates);
						if (neighbor == null) {
								neighbor = CreateCell (coordinates);
								CreatePassage (currentCell, neighbor, direction);
								activeCells.Add (neighbor);
						} else {
								CreateWall (currentCell, neighbor, direction);	//生産力を失うと同時に壁になる
						}
				} else {
						CreateWall (currentCell, null, direction);	//指定された範囲外は全て外壁になる
				}
		}

		void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction)
		{
				MazePassage passage = Instantiate (passagePrefab) as MazePassage;
				passage.Initialize (cell, otherCell, direction);
				passage = Instantiate (passagePrefab) as MazePassage;
				passage.Initialize (otherCell, cell, direction.GetOpposite ());	//otherCellとcellが入れ替わっている点に注意
		}

		void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction)
		{
				MazeWall wall = Instantiate (wallPrefab) as MazeWall;
				wall.Initialize (cell, otherCell, direction);
				if (otherCell != null) {
						wall = Instantiate (wallPrefab) as MazeWall;
						wall.Initialize (otherCell, cell, direction.GetOpposite ());
				}
		}


		public MazeCell GetCell (IntVector2 coordinates)
		{
				return cells [coordinates.x, coordinates.z];
		}

		MazeCell CreateCell (IntVector2 coordinates)
		{
				MazeCell newCell = Instantiate (cellPrefab) as MazeCell;	//プレハブより複製
				cells [coordinates.x, coordinates.z] = newCell;
				newCell.coordinates = coordinates;		
				newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
				newCell.transform.parent = transform;
				newCell.transform.localPosition = new Vector3 (coordinates.x - size.x * 0.5f + 0.5f, 0, coordinates.z - size.z * 0.5f + 0.5f);
				return newCell;
		}
}
