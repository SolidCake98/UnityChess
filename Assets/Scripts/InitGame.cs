using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitGame : MonoBehaviour {

    public Text message;
    public GameObject signIn;
    public GameObject signUp;

    public GameObject findGame;
    public GameObject connect;
    public GameObject rating;
    public GameObject logout;

    public GameObject panelAuth;

    public ChessClient.ChessClient client;
    string HOST = "http://localhost:57286/api/Games";

    // Use this for initialization
    void Start () {
        Init();
	}
	
    public void Init(string message="")
    {
        client = new ChessClient.ChessClient(HOST, "");
        string token = PlayerPrefs.GetString("token");
        if (token == "")
        {
            this.message.text = message==""? "Авторизуйтесь, пожалуйста": message;
            signIn.SetActive(true);
            signUp.SetActive(true);
            panelAuth.SetActive(true);
            findGame.SetActive(false);
            rating.SetActive(false);
            connect.SetActive(false);
            

        }
        else
        {
            try
            {
                string name = client.GetNameByToken(HOST, token);
                this.message.text = "Hello, " + name;
                PlayerPrefs.SetString("name", name  );
                findGame.SetActive(true);
                rating.SetActive(true);
                connect.SetActive(true);
                signIn.SetActive(false);
                signUp.SetActive(false);
                logout.SetActive(true);
            }
            catch
            {
                this.message.text = message == "" ? "Авторизуйтесь, пожалуйста" : message;
                panelAuth.SetActive(true);
                signIn.SetActive(true);
                signUp.SetActive(true);
                findGame.SetActive(false);
                rating.SetActive(false);
                logout.SetActive(false);
                connect.SetActive(false);
            }
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
