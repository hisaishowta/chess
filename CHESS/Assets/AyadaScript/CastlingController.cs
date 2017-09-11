using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlingController : MonoBehaviour
{
    //マス目サイズ
    private const int Mass_Size = 8;

    public GameObject CasObj;


    private GameObject FieldControlle;
    private GameObject MoveControlle;
    private GameObject PlayerControlle;
    private GameObject NetControlle;
    PieceScript Piece;
    FieldScript field;
    MoveController movecontrolle;
    PlayerController playercontrolle;
    NetController netcontrolle;

    //Castling処理用変数
    public bool P1Cas_King = true; //キングが初期位置にいるかの判定用
    public bool P2Cas_King = true;
    public bool P1Cas_RightR = true; //ルークが初期位置にいるかの判定用
    public bool P1Cas_LeftR = true;
    public bool P2Cas_RightR = true;
    public bool P2Cas_LeftR = true;

    public bool CastFlag = false;//Castling中かどうか
    public bool Kcast_Check = false; //KingがCHECKされているかどうか

    // Use this for initialization
    void Start()
    {
        FieldControlle = GameObject.Find("GameSetting/FieldController");
        MoveControlle = GameObject.Find("MoveController");
        PlayerControlle = GameObject.Find("PlayerController");
        NetControlle = GameObject.Find("NetController");
        field = FieldControlle.GetComponent<FieldScript>();
        Piece = FieldControlle.GetComponent<PieceScript>();
        movecontrolle = MoveControlle.GetComponent<MoveController>();
        playercontrolle = PlayerControlle.GetComponent<PlayerController>();
        netcontrolle = NetControlle.GetComponent<NetController>();
    }

    //KingとRookが初期位置から動いた場合該当する駒用のフラグをfalseに
    public void CastCheck()
    {
        if (movecontrolle.MoveObject.name == "P1_King")
            P1Cas_King = false;
        else if (movecontrolle.MoveObject.name == "P2_King")
            P2Cas_King = false;
        if (movecontrolle.MoveObject.name == "P1_Rook0")
            P1Cas_LeftR = false;
        else if (movecontrolle.MoveObject.name == "P1_Rook1")
            P1Cas_RightR = false;
        else if (movecontrolle.MoveObject.name == "P2_Rook0")
            P2Cas_LeftR = false;
        else if (movecontrolle.MoveObject.name == "P2_Rook1")
            P2Cas_RightR = false;
    }

    //左側Castlingの処理
    public void LCas_Process(int fZ, int fX, GameObject moveobj, GameObject board)
    {
        CastFlag = true;

        //自分が行ったCastlingならサーバに上げるCastlingデータ設定
        if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
            netcontrolle.personvalue.castid = 1;

        if (playercontrolle.PlayerCheck())
            P1Cas_King = false;
        else
            P2Cas_King = false;

        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                if (field.Base_Obj[z, x] == board)
                {
                    CasObj = Piece.Piece_Obj[z, x];  //移動するもう一つの駒格納

                    //自分がCastlingを行った場合は駒のサーバデータをセット
                    if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
                    {
                        netcontrolle.personvalue.fromZid = fZ;
                        netcontrolle.personvalue.fromXid = fX;
                        netcontrolle.personvalue.toZid = z;
                        netcontrolle.personvalue.toXid = x;
                    }
                }
            }
        }

        //クリックしているのがKingの時
        if (moveobj.tag == "King")
        {
            board = GameObject.Find("Base[" + fZ + "," + (fX - 2) + "]");
            movecontrolle.target = new Vector3(board.transform.position.x, 1.5f, board.transform.position.z);
            movecontrolle.Uptarget = new Vector3(moveobj.transform.position.x, 1.5f, moveobj.transform.position.z);
            movecontrolle.Downtarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            board = GameObject.Find("Base[" + fZ + "," + (fX - 1) + "]");
            movecontrolle.Castarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            //---------------------------------------------------
            //Piece配列入れ替え
            Piece.Piece_Obj[fZ, fX - 2] = moveobj;
            Piece.Piece_Obj[fZ, fX - 1] = CasObj;
            Piece.Piece_Obj[fZ, fX - 4] = null;
            //---------------------------------------------------
            //Num配列入れ替え
            FieldScript.Piece_Num[fZ, fX - 2] = FieldScript.Piece_Num[fZ, fX];
            FieldScript.Piece_Num[fZ, fX] = (int)FieldScript.Chess_Field.None;
            FieldScript.Piece_Num[fZ, fX - 1] = FieldScript.Piece_Num[fZ, fX - 4];
            FieldScript.Piece_Num[fZ, fX - 4] = (int)FieldScript.Chess_Field.None;
            //---------------------------------------------------
        }
        //クリックしているのがRookの時
        else
        {
            board = GameObject.Find("Base[" + fZ + "," + (fX + 3) + "]");
            movecontrolle.target = new Vector3(board.transform.position.x, 1.5f, board.transform.position.z);
            movecontrolle.Uptarget = new Vector3(moveobj.transform.position.x, 1.5f, moveobj.transform.position.z);
            movecontrolle.Downtarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            board = GameObject.Find("Base[" + fZ + "," + (fX + 2) + "]");
            movecontrolle.Castarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            //---------------------------------------------------
            //Piece配列入れ替え
            Piece.Piece_Obj[fZ, fX + 3] = moveobj;
            Piece.Piece_Obj[fZ, fX + 2] = CasObj;
            Piece.Piece_Obj[fZ, fX + 4] = null;
            //---------------------------------------------------
            //Num配列入れ替え
            FieldScript.Piece_Num[fZ, fX + 3] = FieldScript.Piece_Num[fZ, fX];
            FieldScript.Piece_Num[fZ, fX] = (int)FieldScript.Chess_Field.None;
            FieldScript.Piece_Num[fZ, fX + 2] = FieldScript.Piece_Num[fZ, fX + 4];
            FieldScript.Piece_Num[fZ, fX + 4] = (int)FieldScript.Chess_Field.None;
            //---------------------------------------------------
        }
    }

    //右側Castlingの処理
    public void RCas_Process(int fZ, int fX, GameObject moveobj, GameObject board)
    {
        CastFlag = true;

        //自分が行ったCastlingならサーバに上げるCastlingデータ設定
        if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
            netcontrolle.personvalue.castid = 2;

        if (playercontrolle.PlayerCheck())
            P1Cas_King = false;
        else
            P2Cas_King = false;

        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                if (field.Base_Obj[z, x] == board)
                {
                    CasObj = Piece.Piece_Obj[z, x];  //移動するもう一つの駒格納

                    //自分がCastlingを行った場合は駒のサーバデータをセット
                    if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
                    {
                        netcontrolle.personvalue.fromZid = fZ;
                        netcontrolle.personvalue.fromXid = fX;
                        netcontrolle.personvalue.toZid = z;
                        netcontrolle.personvalue.toXid = x;
                    }
                }
            }
        }

        //クリックしているのがKingの時
        if (moveobj.tag == "King")
        {
            board = GameObject.Find("Base[" + fZ + "," + (fX + 2) + "]");
            movecontrolle.target = new Vector3(board.transform.position.x, 1.5f, board.transform.position.z);
            movecontrolle.Uptarget = new Vector3(moveobj.transform.position.x, 1.5f, moveobj.transform.position.z);
            movecontrolle.Downtarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            board = GameObject.Find("Base[" + fZ + "," + (fX + 1) + "]");
            movecontrolle.Castarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            //---------------------------------------------------
            //Piece配列入れ替え
            Piece.Piece_Obj[fZ, fX + 2] = moveobj;
            Piece.Piece_Obj[fZ, fX + 1] = CasObj;
            Piece.Piece_Obj[fZ, fX + 3] = null;
            //---------------------------------------------------
            //Num配列入れ替え
            FieldScript.Piece_Num[fZ, fX + 2] = FieldScript.Piece_Num[fZ, fX];
            FieldScript.Piece_Num[fZ, fX] = (int)FieldScript.Chess_Field.None;
            FieldScript.Piece_Num[fZ, fX + 1] = FieldScript.Piece_Num[fZ, fX + 3];
            FieldScript.Piece_Num[fZ, fX + 3] = (int)FieldScript.Chess_Field.None;
            //---------------------------------------------------
        }
        //クリックしているのがRookの時
        else
        {
            board = GameObject.Find("Base[" + fZ + "," + (fX - 2) + "]");
            movecontrolle.target = new Vector3(board.transform.position.x, 1.5f, board.transform.position.z);
            movecontrolle.Uptarget = new Vector3(moveobj.transform.position.x, 1.5f, moveobj.transform.position.z);
            movecontrolle.Downtarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            board = GameObject.Find("Base[" + fZ + "," + (fX - 1) + "]");
            movecontrolle.Castarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);
            //---------------------------------------------------
            //Piece配列入れ替え
            Piece.Piece_Obj[fZ, fX - 2] = moveobj; 
            Piece.Piece_Obj[fZ, fX - 1] = CasObj;
            Piece.Piece_Obj[fZ, fX - 3] = null;
            //---------------------------------------------------
            //Num配列入れ替え
            FieldScript.Piece_Num[fZ, fX - 2] = FieldScript.Piece_Num[fZ, fX];
            FieldScript.Piece_Num[fZ, fX] = (int)FieldScript.Chess_Field.None;
            FieldScript.Piece_Num[fZ, fX - 1] = FieldScript.Piece_Num[fZ, fX - 3];
            FieldScript.Piece_Num[fZ, fX - 3] = (int)FieldScript.Chess_Field.None;
            //---------------------------------------------------
        }
    }
}
