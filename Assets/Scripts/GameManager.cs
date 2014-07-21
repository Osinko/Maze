using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

		void Start ()
		{
				BeginGame ();
		}
	
		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Space)) {
						RestartGame ();
				}
		}

		public Maze mazePrefab;
		Maze mazeInstance;

		void BeginGame ()
		{
				mazeInstance = Instantiate (mazePrefab) as Maze;
		}

		void RestartGame ()
		{
				Destroy (mazeInstance);
				BeginGame ();
		}
}
