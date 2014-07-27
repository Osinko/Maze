using UnityEngine;
using System;

//このシリアライズをしっかり反映させるにはMonoBehaviourを継承させてはいけない
[Serializable]
public class MazeRoomSetting
{
		public Material floorMaterial, wallMaterial;

}
