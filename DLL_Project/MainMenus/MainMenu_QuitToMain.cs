using Verse;
using UnityEngine.SceneManagement;

namespace CommunityCoreLibrary
{

    public class MainMenu_QuitToMain : MainMenu
    {

        public override bool RenderNow( bool anyMapFiles )
        {
            return (
                ( Current.ProgramState == ProgramState.MapPlaying ) &&
                ( !Current.Game.Info.permadeathMode )
            );
        }

        public override void ClickAction()
        {
            Find.WindowStack.Add( (Window)new Dialog_Confirm(
                "ConfirmQuit".Translate(),
                () =>
                    {
                        SceneManager.LoadScene( "Entry" );
                    },
                false
            ) );
        }

    }

}