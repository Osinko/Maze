using UnityEngine;
using System.Collections;

public class MazeDoor : MazePassage
{

		public Transform hinge;

		MazeDoor OtherSideDoor {
				get {
						//対面側のセルエッジを拾う
						return otherCell.GetEdge (direction.GetOpposite ()) as MazeDoor;
				}
		}

		public override void Initialize (MazeCell primary, MazeCell otherCell, MazeDirection direction)
		{
				base.Initialize (primary, otherCell, direction);
				if (OtherSideDoor != null) {
						hinge.localScale = new Vector3 (-1, 1, 1);	//ドア部分を逆反転
						Vector3 p = hinge.localPosition;
						p.x = -p.x;
						hinge.localPosition = p;					//ヒンジの位置を変更
				}
				//childCountはカレントオブジェクトの子の数全てが計算される
				//hinge以降幾つかのcubeでオブジェクトが作成されているがプレハブの段階では一つのHingeにまとめられている
				for (int i = 0; i < transform.childCount; i++) {
						Transform child = transform.GetChild (i);
						if (child != hinge) {						//hingeだけは無視される
								child.GetComponent<Renderer> ().material = cell.room.settings.wallMaterial;	//MazeRoomに従って色書き換え
						}
				}
		}

}
