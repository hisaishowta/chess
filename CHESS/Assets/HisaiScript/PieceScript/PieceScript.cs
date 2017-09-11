using UnityEngine;

public class PieceScript : MonoBehaviour
{
    //マス目サイズ
    private const int Mass_Size = 8;
    //駒オブジェクト格納
    public GameObject[,] Piece_Obj =
        new GameObject[Mass_Size, Mass_Size];

    //駒サイズ
    private Vector3 Piece_Size = new Vector3(1, 2, 1);

    //Pawnプレファブ格納用
    public GameObject Prefab_Pawn;
    public GameObject Prefab_Pawn2;
    //Bishopプレファブ格納用
    public GameObject Prefab_Bishop;
    public GameObject Prefab_Bishop2;
    //Castleプレファブ格納用
    public GameObject Prefab_Rook;
    public GameObject Prefab_Rook2;
    //Kingプレファブ格納用
    public GameObject Prefab_King;
    public GameObject Prefab_King2;
    //Knightプレファブ格納用
    public GameObject Prefab_Knight;
    public GameObject Prefab_Knight2;
    //Queenプレファブ格納用
    public GameObject Prefab_Queen;
    public GameObject Prefab_Queen2;


    //プレファブ格納用
    private GameObject Prefab_Box;


    //駒配置
    public void Piece_Respawn()
    {
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                Vector3 Piece_Pos = Vector3.zero;  //駒配置場所
                sbyte num = 0;                      //同一の駒判定用値
                string str = null;                  //オブジェクト名

                if (FieldScript.Piece_Num[z, x] != (int)FieldScript.Chess_Field.None)
                {
                    //-------------------------------------------------
                    //Player01
                    if (FieldScript.Piece_Num[z, x] < 200)
                    {
                        //-------------------------------------------------
                        //駒初期配置場所
                        switch (FieldScript.Piece_Num[z, x])
                        {
                            //ルーク
                            case (int)FieldScript.Chess_Field.P1_Rook:
                                if (x == 0)
                                {
                                    num = 1 - Mass_Size;
                                    str = "P1_Rook" + 0;
                                }
                                else
                                {
                                    num = Mass_Size - 1;
                                    str = "P1_Rook" + 1;
                                }
                                Prefab_Box = Prefab_Rook;
                                break;
                            //ナイト
                            case (int)FieldScript.Chess_Field.P1_Knight:
                                if (x == 1)
                                {
                                    num = 3 - Mass_Size;
                                    str = "P1_Knight" + 0;
                                }
                                else
                                {
                                    num = Mass_Size - 3;
                                    str = "P1_Knight " + 1;
                                }
                                Prefab_Box = Prefab_Knight;
                                break;
                            //ビショップ
                            case (int)FieldScript.Chess_Field.P1_Bishop:
                                if (x == 2)
                                {
                                    num = 5 - Mass_Size;
                                    str = "P1_Bishop" + 0;
                                }
                                else
                                {
                                    num = Mass_Size - 5;
                                    str = "P1_Bishop" + 1;
                                }
                                Prefab_Box = Prefab_Bishop;
                                break;
                            //クイーン
                            case (int)FieldScript.Chess_Field.P1_Queen:
                                num = -1;
                                Prefab_Box = Prefab_Queen;
                                str = "P1_Queen";
                                break;
                            //キング
                            case (int)FieldScript.Chess_Field.P1_King:
                                num = 1;
                                Prefab_Box = Prefab_King;
                                str = "P1_King";
                                break;
                            default:
                                break;
                        }
                        //駒座標指定
                        Piece_Pos = new Vector3(
                                    Piece_Size.x * num / 2.0f,
                                    0.0f,
                                    Piece_Size.z * (1.0f - Mass_Size) / 2.0f);
                        //ポーン
                        if (FieldScript.Piece_Num[z, x] ==
                            (int)FieldScript.Chess_Field.P1_Pawn)
                        {
                            Piece_Pos = new Vector3(
                                    Piece_Size.x * ((x - Mass_Size / 2.0f) + 0.5f),
                                    0.0f,
                                    Piece_Size.z * (3.0f - Mass_Size) / 2.0f);
                            Prefab_Box = Prefab_Pawn;
                            str = "P1_Pawn" + x;
                        }
                        GameObject Piece = Instantiate(
                            Prefab_Box,
                            Piece_Pos,
                             Quaternion.identity) as GameObject;
                        Piece.name = str;
                        Piece.transform.parent = GameObject.Find("Player01").transform;
                        Piece_Obj[z, x] = Piece;
                        //-------------------------------------------------
                    }
                    //-------------------------------------------------
                    //Player02
                    else
                    {
                        //-------------------------------------------------
                        //駒初期配置場所
                        switch (FieldScript.Piece_Num[z, x])
                        {
                            //ルーク
                            case (int)FieldScript.Chess_Field.P2_Rook:
                                if (x == 0)
                                {
                                    num = 1 - Mass_Size;
                                    str = "P2_Rook" + 0;
                                }
                                else
                                {
                                    num = Mass_Size - 1;
                                    str = "P2_Rook" + 1;
                                }
                                Prefab_Box = Prefab_Rook2;
                                break;
                            //ナイト
                            case (int)FieldScript.Chess_Field.P2_Knight:
                                if (x == 1)
                                {
                                    num = 3 - Mass_Size;
                                    str = "P2_Knight" + 0;
                                }
                                else
                                {
                                    num = Mass_Size - 3;
                                    str = "P2_Knight" + 1;
                                }
                                Prefab_Box = Prefab_Knight2;
                                break;
                            //ビショップ
                            case (int)FieldScript.Chess_Field.P2_Bishop:
                                if (x == 2)
                                {
                                    num = 5 - Mass_Size;
                                    str = "P2_Bishop" + 0;
                                }
                                else
                                {
                                    num = Mass_Size - 5;
                                    str = "P2_Bishop" + 1;
                                }
                                Prefab_Box = Prefab_Bishop2;
                                break;
                            //クイーン
                            case (int)FieldScript.Chess_Field.P2_Queen:
                                num = -1;
                                Prefab_Box = Prefab_Queen2;
                                str = "P2_Queen";
                                break;
                            //キング
                            case (int)FieldScript.Chess_Field.P2_King:
                                num = 1;
                                Prefab_Box = Prefab_King2;
                                str = "P2_King";
                                break;
                            default:
                                break;
                        }
                        //駒座標指定
                        Piece_Pos = new Vector3(
                                    Piece_Size.x * num / 2.0f,
                                    0.0f,
                                    Piece_Size.z * (Mass_Size - 1.0f) / 2.0f);
                        //ポーン
                        if (FieldScript.Piece_Num[z, x] ==
                            (int)FieldScript.Chess_Field.P2_Pawn)
                        {
                            Piece_Pos = new Vector3(
                                    Piece_Size.x * ((x - Mass_Size / 2.0f) + 0.5f),
                                    0.0f,
                                    Piece_Size.z * (Mass_Size - 3.0f) / 2.0f);
                            Prefab_Box = Prefab_Pawn2;
                            str = "P2_Pawn" + x;
                        }
                        GameObject Piece = Instantiate(
                            Prefab_Box,
                            Piece_Pos,
                            Quaternion.identity) as GameObject;
                        Piece.name = str;
                        Piece.transform.parent = GameObject.Find("Player02").transform;
                        Piece_Obj[z, x] = Piece;
                        //-------------------------------------------------
                    }
                    //-------------------------------------------------
                }
            }
        }
    }
}