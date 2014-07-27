﻿using UnityEngine;
using System.Collections;

public class MazeWall : MazeCellEdge
{
		public Transform wall;	//自分自身の壁オブジェクトを指定する

		public override void Initialize (MazeCell cell, MazeCell otherCell, MazeDirection direction)
		{
				base.Initialize (cell, otherCell, direction);
				wall.GetComponent<Renderer> ().material = cell.room.settings.wallMaterial;	//そのセルのMazeRoomに割り当てられている色を拾ってくる
		}
}
