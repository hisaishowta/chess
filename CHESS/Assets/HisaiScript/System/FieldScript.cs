using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScript : MonoBehaviour
{
    //作成したQuadを入れるGameObject
    private GameObject Base_Folder;
    //Quadプレファブ格納用
    public GameObject Prefab_Base;
    //Quadサイズ
    private Vector3 Base_Size = new Vector3(1, 1, 1);
    //マス目サイズ
    private const int Mass_Size = 8;
    //マス目定義
    public enum Chess_Field
    {
        //空
        None,
        //Player01使用
        P1_King = 100,
        P1_Queen,
        P1_Bishop,
        P1_Knight,
        P1_Rook,
        P1_Pawn,
        //Player02使用
        P2_King = 200,
        P2_Queen,
        P2_Bishop,
        P2_Knight,
        P2_Rook,
        P2_Pawn,
    }
   
    //駒数値格納
    public static int[,] Piece_Num =
        new int[Mass_Size, Mass_Size];

    //Baseオブジェクト格納
    public GameObject[,] Base_Obj =
        new GameObject[Mass_Size, Mass_Size];

    //Quadフォルダ生成・名前の設定
    void Awake()
    {
        Base_Folder = new GameObject();
        Base_Folder.name = "Base_Folder";
    }

    // Use this for initialization
    void Start()
    {
        Piece_Arrangement();   //駒設定
        Base_Arrangement();     //Quad設定
        PieceScript bs = GetComponent<PieceScript>();
        bs.Piece_Respawn();
        BaseObj_Arramgement();
    }

    //駒配置
    private void Piece_Arrangement()
    {
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                //-------------------------------------------------
                //駒初期化
                //ルーク
                if ((z == 0 && x == 0) || (z == 0 && x == 7))
                {
                    Piece_Num[z, x] = (int)Chess_Field.P1_Rook;
                }
                else if ((z == 7 && x == 0) || (z == 7 && x == 7))
                {
                    Piece_Num[z, x] = (int)Chess_Field.P2_Rook;
                }
                //ナイト
                else if ((z == 0 && x == 1) || (z == 0 && x == 6))
                {
                    Piece_Num[z, x] = (int)Chess_Field.P1_Knight;
                }
                else if ((z == 7 && x == 1) || (z == 7 && x == 6))
                {
                    Piece_Num[z, x] = (int)Chess_Field.P2_Knight;
                }
                //ビショップ
                else if ((z == 0 && x == 2) || (z == 0 && x == 5))
                {
                    Piece_Num[z, x] = (int)Chess_Field.P1_Bishop;
                }
                else if ((z == 7 && x == 2) || (z == 7 && x == 5))
                {
                    Piece_Num[z, x] = (int)Chess_Field.P2_Bishop;
                }
                //クイーン
                else if (z == 0 && x == 3)
                {
                    Piece_Num[z, x] = (int)Chess_Field.P1_Queen;
                }
                else if (z == 7 && x == 3)
                {
                    Piece_Num[z, x] = (int)Chess_Field.P2_Queen;
                }
                //キング
                else if (z == 0 && x == 4)
                {
                    Piece_Num[z, x] = (int)Chess_Field.P1_King;
                }
                else if (z == 7 && x == 4)
                {
                    Piece_Num[z, x] = (int)Chess_Field.P2_King;
                }
                //ポーン
                else if (z == 1)
                {
                    Piece_Num[z, x] = (int)Chess_Field.P1_Pawn;
                }
                else if (z == 6)
                {
                    Piece_Num[z, x] = (int)Chess_Field.P2_Pawn;
                }
                //空
                else
                {
                    Piece_Num[z, x] = (int)Chess_Field.None;
                }
                //-------------------------------------------------
            }
        }
    }

    //Quad配置
    private void Base_Arrangement()
    {
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                //-------------------------------------------------
                //Quad初期化
                //Quad配置場所
                Vector3 Base_Pos = new Vector3(
                    Base_Size.x * ((x - Mass_Size / 2) + 0.5f),
                    0,
                    Base_Size.z * ((z - Mass_Size / 2) + 0.5f));
                GameObject Base = Instantiate(
                    Prefab_Base,
                    Base_Pos,
                    new Quaternion(1f, 0f, 0f, 1f)) as GameObject;
                //作成するQuadの名前
                Base.name = "Base[" + z + "," + x + "]";
                //作成した空オブジェクトを親にする
                Base.transform.parent = Base_Folder.transform;
                //-------------------------------------------------
            }
        }
    }

    //Baseを配列に格納
    private void BaseObj_Arramgement()
    {
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                GameObject baseobj;
                baseobj = GameObject.Find("Base[" + z + "," + x + "]");
                Base_Obj[z, x] = baseobj;
            }
        }
    }
}