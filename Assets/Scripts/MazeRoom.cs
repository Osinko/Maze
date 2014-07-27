using UnityEngine;
using System.Collections.Generic;

public class MazeRoom : ScriptableObject
{
		public int settingsIndex;			//この番号により部屋が塗り分けられる
		public MazeRoomSetting settings;	//例えば上記が0ならFloor1、Wall1で塗られる

		List<MazeCell> cells = new List<MazeCell> ();	//その"部屋"に属するセルのリスト

		public void Add (MazeCell cell)
		{
				cell.room = this;	//発行側のroomにこのMazeRoomを登録
				cells.Add (cell);	//自分自身のリストにセルを登録
		}
}
