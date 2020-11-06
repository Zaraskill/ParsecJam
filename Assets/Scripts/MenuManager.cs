using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject creditsMenu;
    public GameObject tutoMenu;
    public GameObject creditsBut;
    public GameObject menuBut;
    public GameObject toto;

    private int sceneIndex;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPlay()
    {
        mainMenu.SetActive(false);
        tutoMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(toto);
        AudioManager.instance.PlayButton();
    }

    public void OnClickCredits(bool activate)
    {
        if (activate)
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(creditsBut);
        }
        else
        {
            mainMenu.SetActive(true);
            creditsMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(menuBut);
        }
        AudioManager.instance.PlayButton();
    }

    public void OnClickQuit()
    {
        AudioManager.instance.PlayButton();
        Application.Quit();
    }

    public void OnClickStart()
    {
        AudioManager.instance.PlayButton();
        //sceneIndex = Random.Range(0, SceneManager.sceneCount);
        SceneManager.LoadScene(1);
    }
}
