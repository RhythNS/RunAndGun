using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Button_PlayGame() {
        // TODO match making / find game screen
    }

    public void Btn_HostGame() {
        // TODO start hosted game
    }

    public void Btn_Options() {
        // TODO OptionScreen
    }

    public void Btn_QuitGame() {
        // TODO ask user if really wants to quit
        Application.Quit(0);
    }
}
