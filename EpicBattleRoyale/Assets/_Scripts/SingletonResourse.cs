using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonResourse<T> : MonoBehaviour where T : MonoBehaviour {
	static T _Ins;

	static object _lock = new object ();

	public static bool isInit;

	public static T Ins {
		get {
			lock (_lock) {
				if (_Ins == null) {
					GameObject manager = Resources.Load ("Prefabs/" + typeof(T).Name) as GameObject;
					T IstManager = Instantiate (manager).GetComponent<T> ();

					_Ins = IstManager;

					if (!isInit) {
						_Ins.GetComponent <SingletonResourse<T>> ().OnInit ();
						isInit = true;
					}

				}

				return _Ins;
			}
		}
	}

	public void Awake ()
	{
		if (_Ins == null)
			_Ins = this.GetComponent <T> ();
		else if (_Ins != this) {
				Destroy (gameObject);
				return;
			}

		if (!isInit) {
			OnInit ();
			isInit = true;
		}

	}

	public virtual void OnInit ()
	{
		
	}

}
