using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class test : MonoBehaviour
{

	void Start ()
	{
		string folder = Application.dataPath;    //これだけでunityの実行ファイルがあるフォルダがわかる
		List<string> strList = new List<string> ();
		Factor (5, strList);

		SaveText (folder, @"\test.txt", strList.ToArray ());

	}

	int Factor ( int n ,List<string> strList)
	{

		print ("call"+n);
		strList.Add("call"+n);
		if (n == 1)
			return 1;
		
		int answ = Factor (n - 1,strList) * n;
		print ("run " + n + "= " + answ);
		strList.Add("run " + n + "= " + answ);
		
		return answ;
	}

	public void SaveText (string fileFolder, string filename, string[] dataStr)
	{
		using (StreamWriter w = new StreamWriter(fileFolder+filename)) {
			foreach (var item in dataStr) {
				w.WriteLine (item);
			}
		}
	}
}
