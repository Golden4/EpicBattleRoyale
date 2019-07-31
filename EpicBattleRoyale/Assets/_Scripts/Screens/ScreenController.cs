using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour {
	public static ScreenController Ins;

	public enum GameScreen {
		Menu,
		Shop,
		BuyCoin,
		TileSize,
		UI,
		GameOver,
		Pause,
		Continue,
		Challeges,
		ChallegesResult
	}

	public enum CurScene {
		Menu,
		Game
	}

	public CurScene curScene;

	public List<ScreenList> screensList = new List<ScreenList> ();

	[System.Serializable]
	public class ScreenList {
		public GameScreen screenType;
		public ScreenBase screen;
	}

	public static GameScreen curActiveScreen;

	public static event System.Action<ScreenBase> OnChangeScreenEvent;

	void Awake ()
	{
		Ins = this;

		for (int i = 0; i < screensList.Count; i++) {
			screensList [i].screen.Init ();
		}
	}

	void Start ()
	{
		Time.timeScale = 1;
		ActivateScreen (0);
	}

	public static T GetScreen<T> () where T : ScreenBase
	{
		for (int i = 0; i < Ins.screensList.Count; i++) {
			if (Ins.screensList [i].GetType () == typeof(T)) {
				return (T)Ins.screensList [i].screen;
			}
		}

		Debug.LogError ("Not Found Screen: " + typeof(T).Name);

		return default(T);
	}

	/*	public static void ActivateScreen<T> (T screen) where T:ScreenBase
	{
		for (int i = 0; i < Ins.screensList.Length; i++) {
			if (typeof(T) == Ins.screensList [i].GetType ()) {
				ActivateScreen (i);
			}
		}
	}*/

	public void ActivateScreen (GameScreen screen)
	{
		curActiveScreen = (GameScreen)screen;
		int index = screensList.FindIndex (x => x.screenType == screen);
		ActivateScreen (index);
	}

	public void ActivateScreen (int screen)
	{
		for (int i = 0; i < screensList.Count; i++) {
			if (i == screen) {
				screensList [i].screen.Activate ();
			} else {
				screensList [i].screen.Deactivate ();
			}
		}

		if (OnChangeScreenEvent != null) {
			OnChangeScreenEvent.Invoke (screensList [screen].screen);
		}

	}

	void OnDestroy ()
	{
		for (int i = 0; i < screensList.Count; i++) {
			screensList [i].screen.OnCleanUp ();
		}
	}

	/*	void OnApplicationQuit ()
	{
	}*/

	/*	void OnApplicationFocus (bool pause)
	{
		if (pause && curActiveScreen == GameScreen.UI && curScene == CurScene.Game)
			ActivateScreen (GameScreen.Pause);
	}

	void OnApplicationPause (bool pause)
	{
		if (!pause && curActiveScreen == GameScreen.UI && curScene == CurScene.Game) {
			ActivateScreen (GameScreen.Pause);
		}
	}*/

}
