using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Name : MonoBehaviour {

    public string name;
    public ChessClient.ChessClient client;
    string HOST = "http://localhost:57286/api/";

    public async void ConnectToGame()
    {
        string nameFrom = PlayerPrefs.GetString("name").Replace('"',' ').Trim();
        client = new ChessClient.ChessClient(HOST, nameFrom);
        try
        {
            await client.ConnectToGame(name);
            SceneManager.LoadScene(1);
        }
        catch
        {

        }
       
    }
}
