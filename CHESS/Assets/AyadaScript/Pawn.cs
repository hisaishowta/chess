using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{

    //マス目サイズ
    private const int Mass_Size = 8;

    //別スクリプト取得用
    private GameObject FieldControlle;
    private GameObject KomaControlle;
    private GameObject KnightControlle;
    private GameObject QueenControlle;
    private GameObject BishopControlle;
    private GameObject RookControlle;
    private GameObject MoveControlle;
    KomaController komacontrolle;
    PieceScript Piece;
    Knight Kncontrolle;
    RookScript Rcontrolle;
    BishopScript Bcontrolle;
    QueenScript Qcontrolle;
    MoveController movecontrolle;

    private int X;
    private int Z;

    // Use this for initialization
    void Start()
    {
        FieldControlle = GameObject.Find("GameSetting/FieldController");
        KomaControlle = GameObject.Find("GameSetting/KomaController");
        KnightControlle = GameObject.Find("KnightController");
        RookControlle = GameObject.Find("RookController");
        BishopControlle = GameObject.Find("BishopController");
        QueenControlle = GameObject.Find("QueenController");
        MoveControlle = GameObject.Find("MoveController");
        komacontrolle = KomaControlle.GetComponent<KomaController>();
        Piece = FieldControlle.GetComponent<PieceScript>();
        Kncontrolle = KnightControlle.GetComponent<Knight>();
        Rcontrolle = RookControlle.GetComponent<RookScript>();
        Bcontrolle = BishopControlle.GetComponent<BishopScript>();
        Qcontrolle = QueenControlle.GetComponent<QueenScript>();
        movecontrolle = MoveControlle.GetComponent<MoveController>();
    }

    //Pawnの駒が押されているときマイフレーム動く
    public bool Move(GameObject pawn)
    {
        Z = komacontrolle.SearchZ(pawn);   //クリックされた駒の位置取得
        X = komacontrolle.SearchX(pawn);   //
        return MoveSearch(Z, X, pawn);　　 //移動範囲サーチ  移動できるマスがあればtrueを返す
    }

    //移動範囲サーチ
    public bool MoveSearch(int Z, int X, GameObject obj)
    {
        GameObject board;
        bool search = false;

        //P1Pawn
        if (obj != null && obj.transform.parent.gameObject.tag == "P1")
        {
            //前方に何もいなければ
            if (Z < 7 && Piece.Piece_Obj[Z + 1, X] == null)
            {
                board = GameObject.Find("Base[" + (Z + 1) + "," + X + "]");
                board.layer = 9;

                //初期位置ならば
                if (Z == 1 && Piece.Piece_Obj[Z + 2, X] == null)
                {
                    board = GameObject.Find("Base[" + (Z + 2) + "," + X + "]");

                    board.layer = 9;
                    board.tag = "Enpassant";
                }
            }

            //右上に敵がいれば
            if ((board = GameObject.Find("Base[" + (Z + 1) + "," + (X + 1) + "]")) != null)
            {
                if (/*X < 7 &&*/ Piece.Piece_Obj[Z + 1, X + 1] != null || board.tag == "Enpassant")
                {
                    if (board.tag == "Enpassant" || obj.transform.parent.gameObject.tag !=
                         Piece.Piece_Obj[Z + 1, X + 1].transform.parent.gameObject.tag)
                    {
                        board.layer = 9;
                    }
                }
            }


            //左上に敵がいれば
            if ((board = GameObject.Find("Base[" + (Z + 1) + "," + (X - 1) + "]")) != null)
            {
                if (/*X > 0 &&*/ Piece.Piece_Obj[Z + 1, X - 1] != null || board.tag == "Enpassant")
                {
                    if (board.tag == "Enpassant" || obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z + 1, X - 1].transform.parent.gameObject.tag)
                    {
                        board.layer = 9;
                    }
                }
            }
        }

        //P2Pawn
        if (obj != null && obj.transform.parent.gameObject.tag == "P2")
        {
            //前方に何もなければ
            if (Z > 0 && Piece.Piece_Obj[Z - 1, X] == null)
            {
                board = GameObject.Find("Base[" + (Z - 1) + "," + X + "]");
                board.layer = 9;

                //初期位置ならば
                if (Z == 6 && Piece.Piece_Obj[Z - 2, X] == null)
                {
                    board = GameObject.Find("Base[" + (Z - 2) + "," + X + "]");
                    board.layer = 9;
                    board.tag = "Enpassant";
                }
            }
            //右下に敵がいれば
            if ((board = GameObject.Find("Base[" + (Z - 1) + "," + (X + 1) + "]")) != null)
            {
                if (/*X < 7 && */Piece.Piece_Obj[Z - 1, X + 1] != null || board.tag == "Enpassant")
                {
                    if (board.tag == "Enpassant" || obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z - 1, X + 1].transform.parent.gameObject.tag)
                    {
                        board.layer = 9;
                    }
                }
            }


            //左下に敵がいれば
            if ((board = GameObject.Find("Base[" + (Z - 1) + "," + (X - 1) + "]")) != null)
            {
                if (/*X > 0 &&*/Piece.Piece_Obj[Z - 1, X - 1] != null || board.tag == "Enpassant")
                {
                    if (board.tag == "Enpassant" || obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z - 1, X - 1].transform.parent.gameObject.tag)
                    {
                        board.layer = 9;
                    }
                }
            }
        }
        //移動できるマスがあればtrueを返す
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                board = GameObject.Find("Base[" + z + "," + x + "]");
                if (board.layer == 9)
                {
                    search = true;
                    return search;
                }
            }
        }
        return search;
    }
}