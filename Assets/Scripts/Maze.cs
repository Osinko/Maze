using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
		public IntVector2 size;
		public MazeCell cellPrefab;
		public MazePassage passagePrefab;	//通路
		public MazeWall[] wallPrefabs;		//壁

		MazeCell[,] cells;

		public float generationStepDelay;	//遅延時間

		public MazeRoomSetting[] roomSetting;

		//MazeCell[] test;	//Debug用

		//ここからすべてを生成している
		public IEnumerator Generate ()
		{
				WaitForSeconds delay = new WaitForSeconds (generationStepDelay);
				cells = new MazeCell[size.x, size.z];
				List<MazeCell> activeCells = new List<MazeCell> ();
				DoFirstGenerationStep (activeCells);
				
				while (activeCells.Count >0) {
						//test = activeCells.ToArray ();	//Debug用
						yield return delay;
						DoNextGenerationStep (activeCells);
				}
		}

		//起源
		void DoFirstGenerationStep (List<MazeCell> activeCells)
		{
				MazeCell newCell = CreateCell (IntVector2.RandomVector (size));
				newCell.Initialize (CreateRoom (-1));		//除外するインデックスを-1にすることでどんなランダム値でも通る事になりMazeRoomをセルに登録
				activeCells.Add (newCell);
		}


		//追加、アルゴリズムの選択コード
		public enum ChangeAlgorithm
		{
				First,
				Mid,
				Last,
				Rand,
		}

		public ChangeAlgorithm changeAlgorithm;

		delegate int FunctionDelegate (List<MazeCell> activeCells);
		static FunctionDelegate[] Func = {
			First,
			Mid,
			Last,
			Rand
		};

		static int First (List<MazeCell> activeCells)
		{
				return 0;
		}

		static int Mid (List<MazeCell> activeCells)
		{
				return activeCells.Count / 2;
		}

		static int Last (List<MazeCell> activeCells)
		{
				return activeCells.Count - 1;
		}

		static int Rand (List<MazeCell> activeCells)
		{
				return Random.Range (0, activeCells.Count - 1);
		}

		//迷路の成長
		void DoNextGenerationStep (List<MazeCell> activeCells)
		{
				FunctionDelegate f = Func [(int)changeAlgorithm];

				int currentIndex = f (activeCells);				//リストの後ろから
				//int currentIndex = activeCells.Count - 1;				//リストの後ろから
				MazeCell currentCell = activeCells [currentIndex];

				if (currentCell.IsFullyInitialized) {					//４方向の情報が揃っていた場合
						activeCells.RemoveAt (currentIndex);			//アクティブセルから外す
						return;											//反対に４方向が埋まっていないセルなら、そこから迷路が成長を始める
				}

				MazeDirection direction = currentCell.RandomUninitializedDirection;		//空きのあるエッジをランダムで探し進行方向を得る
				IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector ();
				if (coordinates.Contains (size)) {						//迷路の端をこえているかどうか？
						MazeCell neighbor = GetCell (coordinates);		//進行方向先のセルを取得
						if (neighbor == null) {
								neighbor = CreateCell (coordinates);	//なかったら進行方向先にセルを作って
								CreatePassage (currentCell, neighbor, direction);	//まず通路を作る
								activeCells.Add (neighbor);				//隣接セルをアクティブリストに登録
						} else {
								CreateWall (currentCell, neighbor, direction);
						}
				} else {
						CreateWall (currentCell, null, direction);	//迷路の端をこえていたら全て外壁になる
				}
		}


		public MazeDoor doorPrefab;
		[Range(0,1)]
		public float
				doorProbavility;

		//通路の作成	
		void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction)
		{
				MazePassage prefab = Random.value < doorProbavility ? doorPrefab : passagePrefab;	//ランダムで通路かドアプレハブを生成
				MazePassage passage = Instantiate (prefab) as MazePassage;	//カレントセルにpassagePrefabを作成
				passage.Initialize (cell, otherCell, direction);
				passage = Instantiate (prefab) as MazePassage;				//進行方向位置にpassagePrefabを作成

				//部屋はドアを契機として変更している
				//otherCellなので進行方向先のセルからMazeRoomを変更している
				if (passage is MazeDoor) {												//もし生成したものがドアなら
						otherCell.Initialize (CreateRoom (cell.room.settingsIndex));	//そこから別の部屋として初期化（現在のセルが所属する部屋は除外される）
				} else {
						otherCell.Initialize (cell.room);								//自セルが属する部屋に設定。つまり今までの部屋と同じとして初期化
				}

				passage.Initialize (otherCell, cell, direction.GetOpposite ());		//進入方向にエッジの方向を定め方向を反転
		}


		void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction)
		{
				MazeWall wall = Instantiate (wallPrefabs [Random.Range (0, wallPrefabs.Length)]) as MazeWall;	//カレントセルにwallPrefabを作成
				wall.Initialize (cell, otherCell, direction);
				if (otherCell != null) {								//nullは迷路の外壁を意味する。外壁の場合は進行方向に作成しない
						wall = Instantiate (wallPrefabs [Random.Range (0, wallPrefabs.Length)]) as MazeWall;	//進行方向にwallPrefabを作成
						wall.Initialize (otherCell, cell, direction.GetOpposite ());	//進入方向にエッジの方向を定め方向を反転
				}
		}

		//部屋の島のリスト
		List<MazeRoom> rooms = new List<MazeRoom> ();

		//部屋の作成
		MazeRoom CreateRoom (int indexToExclude)	//引数に除外するインデックを指定する
		{
				MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom> ();
				newRoom.settingsIndex = Random.Range (0, roomSetting.Length);	//その部屋の色をランダムで決める
				if (newRoom.settingsIndex == indexToExclude) {
						//隣の部屋と同じ色設定であったなら１ずらし、上限値をこえたらラッピングする
						newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSetting.Length;
				}
				newRoom.settings = roomSetting [newRoom.settingsIndex];	//色を実装
				rooms.Add (newRoom);	//部屋の島を登録
				return newRoom;
		}




		//指定した座標のセルを得る
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
