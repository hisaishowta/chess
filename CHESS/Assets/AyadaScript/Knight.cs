using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{

    //マス目サイズ
    private const int Mass_Size = 8;

    //別スクリプト取得用
    private GameObject FieldControlle;
    private GameObject KomaControlle;
    KomaController komacontrolle;
    PieceScript Piece;

    private int X;
    private int Z;

    // Use this for initialization
    void Start()
    {
        FieldControlle = GameObject.Find("GameSetting/FieldController");
        KomaControlle = GameObject.Find("KomaController");
        komacontrolle = KomaControlle.GetComponent<KomaController>();
        Piece = FieldControlle.GetComponent<PieceScript>();
    }

    //Knightの駒が押されているときマイフレーム動く
    public bool Move(GameObject knight)
    {
        Z = komacontrolle.SearchZ(knight);   //クリックされた駒の位置取得
        X = komacontrolle.SearchX(knight);   //
        return MoveSearch(Z, X, knight);　　　　　　　　 //移動範囲サーチ
    }


    //移動範囲サーチ
    public bool MoveSearch(int Z, int X, GameObject obj)
    {
        GameObject board;
        bool search = false;

        //移動できるボードのレイヤーを変更
        //左上
        if (Z < 6 && X > 0 && Piece.Piece_Obj[Z + 2, X - 1] != null)
        {
            board = GameObject.Find("Base[" + (Z + 2) + "," + (X - 1) + "]");
            //クリックオブジェクトの親タグと進行方向にいるオブジェクトの親タグを比較
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z + 2, X - 1].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z + 2) + "," + (X - 1) + "]")) != null)
        {
            board.layer = 9;
        }

        //右上
        if (Z < 6 && X < 7 && Piece.Piece_Obj[Z + 2, X + 1] != null)
        {
            board = GameObject.Find("Base[" + (Z + 2) + "," + (X + 1) + "]");
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z + 2, X + 1].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z + 2) + "," + (X + 1) + "]")) != null)
        {
            board.layer = 9;
        }

        //右上
        if (Z < 7 && X < 6 && Piece.Piece_Obj[Z + 1, X + 2] != null)
        {
            board = GameObject.Find("Base[" + (Z + 1) + "," + (X + 2) + "]");
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z + 1, X + 2].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z + 1) + "," + (X + 2) + "]")) != null)
        {
            board.layer = 9;
        }

        //右下
        if (Z > 0 && X < 6 && Piece.Piece_Obj[Z - 1, X + 2] != null)
        {
            board = GameObject.Find("Base[" + (Z - 1) + "," + (X + 2) + "]");
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z - 1, X + 2].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z - 1) + "," + (X + 2) + "]")) != null)
        {
            board.layer = 9;
        }

        //右下
        if (Z > 1 && X < 7 && Piece.Piece_Obj[Z - 2, X + 1] != null)
        {
            board = GameObject.Find("Base[" + (Z - 2) + "," + (X + 1) + "]");
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z - 2, X + 1].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z - 2) + "," + (X + 1) + "]")) != null)
        {
            board.layer = 9;
        }

        //左下
        if (Z > 1 && X > 0 && Piece.Piece_Obj[Z - 2, X - 1] != null)
        {
            board = GameObject.Find("Base[" + (Z - 2) + "," + (X - 1) + "]");
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z - 2, X - 1].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z - 2) + "," + (X - 1) + "]")) != null)
        {
            board.layer = 9;
        }

        //左下
        if (Z > 0 && X > 1 && Piece.Piece_Obj[Z - 1, X - 2] != null)
        {
            board = GameObject.Find("Base[" + (Z - 1) + "," + (X - 2) + "]");
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z - 1, X - 2].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z - 1) + "," + (X - 2) + "]")) != null)
        {
            board.layer = 9;
        }

        //左上
        if (Z < 7 && X > 1 && Piece.Piece_Obj[Z + 1, X - 2] != null)
        {
            board = GameObject.Find("Base[" + (Z + 1) + "," + (X - 2) + "]");
            if (obj.transform.parent.gameObject.tag != Piece.Piece_Obj[Z + 1, X - 2].transform.parent.gameObject.tag)
            {
                board.layer = 9;
            }
        }
        else if ((board = GameObject.Find("Base[" + (Z + 1) + "," + (X - 2) + "]")) != null)
        {
            board.layer = 9;
        }

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