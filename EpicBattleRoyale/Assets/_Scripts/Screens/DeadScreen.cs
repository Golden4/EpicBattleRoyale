using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DeadScreen : Menu<DeadScreen>
{
    public Text numberPlaceText;
    public Text killsText;
    public Text rewardText;

    public override void OnShow()
    {
    }

    public static void Show(int killsAmount, int placeNumber, int rewardAmount)
    {
        Open();

        Instance.numberPlaceText.text = string.Format("#{0}<size=20><color=grey>/{1}</color></size>", placeNumber, GameController.CHARACTERS_COUNT_MAX);

        Instance.killsText.text = killsAmount.ToString();
    }

    public void Retry()
    {
        SceneController.LoadScene(SceneController.Scene.Game);
    }

    public void MainMenu()
    {
        SceneController.LoadScene(SceneController.Scene.Menu);
    }
}
