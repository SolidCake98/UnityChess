using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour {

    public GameObject board;
    Rules rules;
    ChessClient.ChessClient client;

	public void OfferDraw()
    {
        rules = board.GetComponent<Rules>();
        client = rules.refBoard;
        rules.youOfferDraw = true;
        client.OfferDraw();
    }

    public void AcceptDraw()
    {
        rules = board.GetComponent<Rules>();
        client = rules.refBoard;
        client.AcceptDraw();
        rules.AcceptDraw.SetActive(false);
        rules.DeclineDraw.SetActive(false);
    }

    public void DeclineDraw()
    {
        rules = board.GetComponent<Rules>();
        client = rules.refBoard;
        client.DeclineDraw();
        rules.AcceptDraw.SetActive(false);
        rules.DeclineDraw.SetActive(false);
    }
}
