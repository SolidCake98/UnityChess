using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Messages
{
    public string Message;
}

[Serializable]
public class AuthRes
{
    public string Item1;
    public string Item2;
}


public class Authorization : MonoBehaviour
{
    public GameObject panel;
    public InputField inputNameRegister;
    public InputField inputPasswordRegister;
    public InputField inputEmailRegister;
    public GameObject load;
    public Text message;

    ChessClient.ChessClient client;
    string host = "http://localhost:57286/api/";

    string pattern = @".+@.+\..+$";

    public async void Register()
    {
        string name = inputNameRegister.text;
        string password = inputPasswordRegister.text;
        string email = inputEmailRegister.text;

        client = new ChessClient.ChessClient(host, name);
        try
        {
            load.SetActive(true);
            if(!Regex.IsMatch(email, pattern))
            {
                message.text = "Неверный email";
                return;
            }
            await client.RegisterUser(host + "Users", name, password, email);
            load.SetActive(false);
            message.gameObject.SetActive(true);
            message.text = "Вы успешно зарегистрованы";
            panel.SetActive(false);
            InitGame init = GameObject.Find("Main Camera").GetComponent<InitGame>();
            init.Init("Вы успешно зарегистрованы");
        }
        catch (WebException ex)
        {
            if (ex.Response == null)
            {
                message.text = "Инет вруби";
                load.SetActive(false);
                return;
            }
            Stream errorResponse = ex.Response.GetResponseStream();
            StreamReader reader = new StreamReader(errorResponse, Encoding.UTF8);
            string responseString = reader.ReadToEnd();
            try
            {
                Messages mes = JsonUtility.FromJson<Messages>(responseString);
                load.SetActive(false);
                message.text = mes.Message;
            }
            catch
            {
                message.text = ex.Message;
                load.SetActive(false);
            }
        }
    }


    public async void Auth()
    {
        string name = inputNameRegister.text;
        string password = inputPasswordRegister.text;

        client = new ChessClient.ChessClient(host, name);
        try
        {
            load.SetActive(true);
            string response = await client.Authentication(host + "Token/gettoken/", name, password);
            AuthRes authRes = JsonUtility.FromJson<AuthRes>(response);
            load.SetActive(false);
            message.text = authRes.Item2 + " успешно авторизован";
            PlayerPrefs.SetString("name", authRes.Item2);
            PlayerPrefs.SetString("token", "Bearer " + authRes.Item1);
            panel.SetActive(false);
            InitGame init = GameObject.Find("Main Camera").GetComponent<InitGame>();
            init.Init();
        }
        catch (WebException ex)
        {
            if (ex.Response == null)
            {
                message.text = "Инет вруби";
                load.SetActive(false);
                return;
            }
                message.text = "Неправильно введен логин или пароль";
                load.SetActive(false);
        }
    }
}
