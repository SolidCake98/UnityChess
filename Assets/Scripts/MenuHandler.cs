using ChessClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour {

    public GameObject signInPanel;
    public GameObject signUpPanel;
    public GameObject button;


    public void SignUp()
    {
        button.SetActive(false);
        signUpPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SignIn()
    {
        button.SetActive(false);
        signInPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void LogOut()
    {
        PlayerPrefs.SetString("token", "");
        InitGame init = GameObject.Find("Main Camera").GetComponent<InitGame>();
        init.Init();
    }

    public void StartPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
