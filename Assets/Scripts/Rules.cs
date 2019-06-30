using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;
using ChessClient;
using UnityEngine.UI;

public class Rules : MonoBehaviour {

    const string HOST = "http://localhost:57286/api/";
    string user;
    bool isPlaying = true;
    string yourColor;
    string currentColor;
    bool isYourTurn;
    bool makeMove = false;
    bool offerDraw = false;

    public GameObject g;
    public GameObject v;

    public GameObject Draw;

    public GameObject AcceptDraw;
    public GameObject DeclineDraw;
    public GameObject DrawText;

    public GameObject gameOverCanvas;
    public Text gameOverText;

    DragAndDrop dad;
    Chess.Chess chess;
    ChessClient.ChessClient create;
    public ChessClient.ChessClient refBoard;

    bool flag = true;
    public bool youOfferDraw = false;



	// Use this for initialization
	IEnumerator Start ()
    {
        user = PlayerPrefs.GetString("name");
        user = user.Replace('"', ' ').Trim();
        dad = new DragAndDrop();


        create = new ChessClient.ChessClient(HOST, user);
        refBoard = new ChessClient.ChessClient(HOST, user);
        try
        {
            CreateGame();
        }
        catch
        {
            GameInfo info = refBoard.GetCurrentGame().Result;
            chess = new Chess.Chess(info.FEN);
        }
        
        if (isPlaying)
        {
            while (true)
            {
                yield return new WaitForSeconds(0.7f);
                Refresh();
            }
        } 
    }
    private void CreateGame()
    {
        GameInfo info = create.CreateOnce().Result;
        
        chess = new Chess.Chess(info.FEN);

    }

    private async void Refresh()
    {
        if (isPlaying)
        {
            GameInfo info = await refBoard.GetCurrentGame();
            if(info.Statuse == "play")
            {
                Draw.SetActive(true);
            }
            if (info.Statuse == "wait")
            { 
                g.SetActive(true);
                Draw.SetActive(false);
                v.SetActive(true);
            }
            else
            {
                g.SetActive(false);

                v.SetActive(false);
            }
            if (info.Statuse == "done")
            {
                isPlaying = false;
                gameOverText.text = "Вы проиграли!!!";
                gameOverCanvas.SetActive(true);
                Draw.SetActive(false);
            }
            if (info.Statuse == "draw")
            {
                isPlaying = false;
                gameOverText.text = "Ничья!!!";
                Draw.SetActive(false);
                gameOverCanvas.SetActive(true);
            }
            if (info.Statuse == "offerDraw")
            {
                offerDraw = true;
                Draw.SetActive(false);
                if (!youOfferDraw)
                {
                    AcceptDraw.SetActive(true);
                    DeclineDraw.SetActive(true);
                    DrawText.SetActive(true);
                }
            }
            else
            {
                youOfferDraw = false;
                offerDraw = false;
                AcceptDraw.SetActive(false);
                DeclineDraw.SetActive(false);
                DrawText.SetActive(false);
            }
            chess = new Chess.Chess(info.FEN);
            if(yourColor == null)
            {
                yourColor = await refBoard.GetColorPlayer(info);
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        MarkSquare(x, y, false);
            }
            
            if (dad.state != DragAndDrop.State.drag && flag)
                ShowFigures();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (dad.Action())
        {
            string from = GetSquare(dad.pickPosition);
            string to = GetSquare(dad.putPosition);
            string figure = chess.GetFigureAt(from).ToString();
            string move = figure + from + to;
            Debug.Log(move);
            //chess = new Chess.Chess(client.SendMove(move).FEN);
            if (isPlaying || !offerDraw)
                MakeMove(move);

            //ShowFigures();
        }
       else if(!dad.isPicking && !makeMove)
       {
            if (yourColor == currentColor)
            {
                MarkValidFigures();
            }
            else
            {
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        MarkSquare(x, y, false);
            }
        }
        else
        {
            if (yourColor == currentColor && !makeMove)
            {
                string from = GetSquare(dad.pickPosition);
                string figure = chess.GetFigureAt(from).ToString();
                MarkFigureMove(figure + from);
            }
        }
        currentColor = chess.fen.Split()[1] == "w" ? "white" : "black";
    }

    void MarkFigureMove(string figure)
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                MarkSquare(x, y, false);
        foreach (string moves in chess.GetAllMoves())
        {
            string temp = moves.Substring(0, 3);
            if(figure == temp)
            {
                int x, y;
                string str = moves.Substring(3, 2);
                GetCoord(str, out x, out y);
                MarkSquare(x, y, true);
                GetCoord(figure.Substring(1,2), out x, out y);
                MarkSquare(x, y, true);
            }
        }
    }

    async void MakeMove(string move)
    {
        makeMove = true;
        flag = false;
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                MarkSquare(x, y, false);
        GameInfo info = await refBoard.SendMove(move);
        chess = new Chess.Chess(info.FEN);
        currentColor = chess.fen.Split()[1] == "w" ? "white" : "black";
        if (chess.IsCheckAndMate())
        {
            isPlaying = false;
            gameOverText.text = "Вы победили!!!";
            gameOverCanvas.SetActive(true);
            Draw.SetActive(false);
        }
        ShowFigures();
        makeMove = false;
        flag = true;
    }

    private string GetSquare(Vector2 putPosition)
    {
        int x = Convert.ToInt32(putPosition.x / 2.0);
        int y = Convert.ToInt32(putPosition.y / 2.0);
        return ((char)('a' + x)).ToString() + (y + 1).ToString();
    }

    void ShowFigures()
    {
        int nr = 0;
        for(int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                string figure = chess.GetFigureAt(x, y).ToString();
                if (figure == ".") continue;
                PlaceFigure("box" + nr, figure, x, y);
                nr++;
            }
        for(; nr < 32; nr++)
        {
            PlaceFigure("box" + nr, "q", 9, 9);
        }
    }

    void MarkValidFigures()
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                MarkSquare(x, y, false);
        foreach (string moves in chess.GetAllMoves())
        {
            int x, y;
            GetCoord(moves.Substring(1, 2), out x, out y);
            MarkSquare(x, y, true);
        }
    }

    public void GetCoord(string e2, out int x, out int y)
    {
        x = 9;
        y = 9;
        if (e2.Length == 2 &&
            e2[0] >= 'a' && e2[0] <= 'h' &&
            e2[1] >= '1' && e2[1] <= '8')
        {
            x = e2[0] - 'a';
            y = e2[1] - '1';
        }
    }

    void PlaceFigure (string box, string figure, int x, int y)
    {
        //Debug.Log(box + " " + figure + " " + x + y);
        GameObject goBox = GameObject.Find(box);
        GameObject goFigure = GameObject.Find(figure);
        GameObject goSquare = GameObject.Find("" + y + x);

        var spriteFigure = goFigure.GetComponent<SpriteRenderer>();
        var spriteBox = goBox.GetComponent<SpriteRenderer>();
        spriteBox.sprite = spriteFigure.sprite;

        goBox.transform.position = goSquare.transform.position;
    }

    void MarkSquare(int x, int y, bool isMarked)
    {
        GameObject goSquare = GameObject.Find("" + y + x);
        GameObject goCell;
        string color = (x + y) % 2 == 0 ? "Black" : "White";
        if (isMarked)
            goCell = GameObject.Find(color + "SquareMarked");
        else
            goCell = GameObject.Find(color + "Square");
        var spriteSquare = goSquare.GetComponent<SpriteRenderer>();
        var spriteCell = goCell.GetComponent<SpriteRenderer>();
        spriteSquare.sprite = spriteCell.sprite;
    }
}

class DragAndDrop
{
    
    public enum State
    {
        none,
        drag
    }

    public Vector2 pickPosition { get; private set; }
    public Vector2 putPosition{ get; private set; }
    public bool isPicking = false;

    public State state;
    GameObject item;
    Vector2 offset;


    public DragAndDrop()
    {
        state = State.none;
        item = null;
    }

    public bool Action()
    {
        switch (state)
        {
            case State.none:
                if (IsMousedPressed())
                {
                    PickUp();
                    isPicking = true;
                }
                    

                break;
            case State.drag:
                if (IsMousedPressed())
                    Drag();
                else
                {
                    Drop();
                    isPicking = false;
                    return true;
                }
                break;
        }
        return false;
    }


        public bool IsMousedPressed()
    {
        return Input.GetMouseButton(0);
    }

    public void PickUp()
    {
        Vector2 clickPosition = GetClickPosition();
        Transform clickeditem = GetItemAt(clickPosition);
        if (clickeditem == null) return;
        pickPosition = clickeditem.position;
        item = clickeditem.gameObject;
        state = State.drag;
        offset = pickPosition - clickPosition;
        Debug.Log("Picked up " + item.name);
    }

    Transform GetItemAt(Vector2 position)
    {
        RaycastHit2D[] figures = Physics2D.RaycastAll(position, position, 0.5f);
        if (figures.Length == 0)
            return null;
        return  figures[0].transform;
    }

    void Drag()
    {
        item.transform.position = GetClickPosition() + offset;
    }

    void Drop()
    {
        putPosition = item.transform.position;
        state = State.none;
        item = null;
    }

    Vector2 GetClickPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    

}