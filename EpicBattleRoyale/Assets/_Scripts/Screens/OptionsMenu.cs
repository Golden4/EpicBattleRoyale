using UnityEngine.UI.Extensions;

public class OptionsMenu : SimpleMenu<OptionsMenu>
{
    public override void OnBackPressed()
    {
        MainMenu.Show();
    }
}
