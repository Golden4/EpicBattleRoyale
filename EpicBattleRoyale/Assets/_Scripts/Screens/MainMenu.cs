using UnityEngine.UI.Extensions;

public class MainMenu : SimpleMenu<MainMenu>
{
    public void OnPlayPressed()
    {
        SceneController.LoadScene(SceneController.Scene.Game);
    }

    public void OnOptionsPressed()
    {
        OptionsMenu.Show();
    }

    public override void OnBackPressed()
    {
    }
}
