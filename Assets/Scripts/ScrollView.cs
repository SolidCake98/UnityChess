using ChessClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour {

    const string HOST = "http://localhost:57286/api/"   ;
    public RectTransform prefab;
    public ScrollRect scrollView;
    public RectTransform content;
    public GameObject pan;
    List<ItemView> views = new List<ItemView>();
    List<ItemViewWait> viewsWait = new List<ItemViewWait>();
    bool isActive = false;

    ChessClient.ChessClient client = new ChessClient.ChessClient(HOST, "");

    public void InitScroll()
    {
        isActive = !isActive;
        scrollView.gameObject.SetActive(isActive);
        pan.SetActive(isActive);
        if(isActive)
            RatingListInit(result => OnReceivedModels(result));
    }

    public void InitWaitScroll()
    {
        isActive = !isActive;
        scrollView.gameObject.SetActive(isActive);
        WaitListInit(result => OnReciewedWaitModels(result));
    }

    void OnReciewedWaitModels(List<GameInfo> result)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
        views.Clear();
        foreach (var model in result)
        {
            var instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
            instance.SetActive(true);
            instance.transform.SetParent(content, false);
            var view = InitialiseItemView(instance, model);
            viewsWait.Add(view);
        }
    }

    private ItemViewWait InitialiseItemView(GameObject instance, GameInfo model)
    {
        ItemViewWait view = new ItemViewWait(instance.transform);
        int id = model.White;
        UserInfo u = client.GetUserById(id).Result;
        view.name.text = u.Name;
        var comp = view.connect.GetComponent<Name>();
        comp.name = u.Name;
        return view;
    }

    void WaitListInit(Action<List<GameInfo>> onDone)
    {
        List<GameInfo> userRating = client.GetWaitingGames(HOST).Result;
        onDone(userRating);
    }

    void RatingListInit(Action<List<UserInfo>> onDone)
    {
        var client = new ChessClient.ChessClient(HOST, "");
        List<UserInfo> userRating = client.GetRating(HOST).Result;
        onDone(userRating);
    }

    void OnReceivedModels(List<UserInfo> models)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
        views.Clear();
        foreach(var model in models)
        {
            var instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
            instance.SetActive(true);
            instance.transform.SetParent(content, false);
            var view = InitialiseItemView(instance, model);
            views.Add(view);
        }
    }

    ItemView InitialiseItemView(GameObject viewGameObject, UserInfo model)
    {
        ItemView view = new ItemView(viewGameObject.transform);
        view.name.text = model.Name;
        view.rating.text = model.Rating.ToString();
        return view;
    }
}

public class ItemView
{
    public Text name;
    public Text rating;

    public ItemView(Transform rootView)
    {
        name = rootView.Find("Name").GetComponent<Text>();
        rating = rootView.Find("Rating").GetComponent<Text>();
    }
}

public class ItemViewWait
{
    public Text name;
    public Button connect;

    public ItemViewWait(Transform rootView)
    {
        name = rootView.Find("Name").GetComponent<Text>();
        connect = rootView.Find("Button").GetComponent<Button>();
    }
}