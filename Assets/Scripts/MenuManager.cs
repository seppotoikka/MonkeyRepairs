using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject buttons;
    public GameObject tutorial;
    public GameObject credits;

    public TextMeshProUGUI players;

    bool quit;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();
    }

    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }

    public void Credits(bool show)
    {
        credits.SetActive(show);
        buttons.SetActive(!show);
    }

    public void Quit()
    {
        if (!quit)
        {
            quit = true;
            buttons.SetActive(false);
            Application.Quit();
        }
    }

    public void NumberOfPlayers()
    {
        GameManager.numberOfPlayers = GameManager.numberOfPlayers % 2 + 1;
        players.text = GameManager.numberOfPlayers == 1 ? "1 Player" : "2 Players";
    }

    IEnumerator FadeToQuit()
    {
        Fader.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }

    public void Tutorial(bool show)
    {
        tutorial.SetActive(show);
        buttons.SetActive(!show);
    }

    IEnumerator LoadGame()
    {
        Fader.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }
}
