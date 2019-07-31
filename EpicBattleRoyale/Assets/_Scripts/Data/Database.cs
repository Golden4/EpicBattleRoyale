using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Database {

	static GameAssets data;

	static bool Loaded;

	public static GameAssets Get {
		get {
			if (data == null && !Loaded) {
				Loaded = true;
				GameAssets pi = Resources.Load<GameAssets> ("Data/Data");

				data = pi;
			}

			return data;
		}
	}

}
