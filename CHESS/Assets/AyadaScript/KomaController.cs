using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KomaController : MonoBehaviour {

    //マス目サイズ
    private const int Mass_Size = 8;

    private GameObject getobj; //クリックされた駒格納

    public int mCount = 0;//ルーク、クイーン、ビショップが移動できるかできないかの判定用

    //別スクリプト取得用
    private GameObject PlayerControlle;
    private GameObject MoveControlle;
    private GameObject ClickControlle;
    private GameObject PawnControlle;
    private GameObject KingControlle;
    private GameObject KnightControlle;
    private GameObject BishopControlle;
    private GameObject QueenControlle;
    private GameObject RookControlle;
    private GameObject FieldControlle;
    private GameObject NetControlle;
    private GameObject AiControlle;
    PlayerController playercontrolle;
    MoveController movecontrolle;
    CastlingController cast;
    ClickController clickcontrolle;    
    Pawn Pcontrolle;
    King Kcontrolle; 
    Knight Kncontrolle;
    RookScript Rcontrolle;
    BishopScript Bcontrolle;
    QueenScript Qcontrolle;
    PieceScript Piece;
    NetController netcontrolle;
    A_AIController a_aicontrolle;
    H_AIController h_aicontrolle;

    private GameObject TurnText;
    private GameObject E_TurnText;

    public bool KomaOnOffFlag; //駒がクリックされている状態か？

    // Use this for initialization
    void Start () {
        getobj = null;
        KomaOnOffFlag = false;

        //別スクリプト取得
        PlayerControlle = GameObject.Find("PlayerController");
        MoveControlle = GameObject.Find("MoveController");
        ClickControlle = GameObject.Find("ClickController");
        PawnControlle = GameObject.Find("PawnController");
        KingControlle = GameObject.Find("KingController");
        KnightControlle = GameObject.Find("KnightController");
        RookControlle = GameObject.Find("RookController");
        BishopControlle = GameObject.Find("BishopController");
        QueenControlle = GameObject.Find("QueenController");
        FieldControlle = GameObject.Find("GameSetting/FieldController");
        NetControlle= GameObject.Find("NetController");
        AiControlle = GameObject.Find("AiController");

        playercontrolle = PlayerControlle.GetComponent<PlayerController>();
        movecontrolle = MoveControlle.GetComponent<MoveController>();
        cast = MoveControlle.GetComponent<CastlingController>();
        clickcontrolle = ClickControlle.GetComponent<ClickController>();   
        Pcontrolle = PawnControlle.GetComponent<Pawn>();      
        Kcontrolle = KingControlle.GetComponent<King>();  
        Kncontrolle = KnightControlle.GetComponent<Knight>();
        Rcontrolle = RookControlle.GetComponent<RookScript>();
        Bcontrolle = BishopControlle.GetComponent<BishopScript>();
        Qcontrolle = QueenControlle.GetComponent<QueenScript>();
        Piece = FieldControlle.GetComponent<PieceScript>();
        netcontrolle = NetControlle.GetComponent<NetController>();
        a_aicontrolle = AiControlle.GetComponent<A_AIController>();
        h_aicontrolle = AiControlle.GetComponent<H_AIController>();

        TurnText = GameObject.Find("Canvas/TurnText");
        E_TurnText = GameObject.Find("Canvas/E_TurnText");
        TurnText.SetActive(false);
        E_TurnText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //自分のターンなら駒を動かせる
        if (RoomController.getplayerflag() == playercontrolle.PlayerCheck() && !RoomController.getAi_BattleFlag())
        {
            PieceSelect();

            //   右クリックでリセット
            if (clickcontrolle.getRightClick() && !movecontrolle.MoveFlag)
                Reset();

            //CPUとの対戦では表示させない
            if (!RoomController.getAyada_Aiflag())
            {
                TurnText.SetActive(true);
                E_TurnText.SetActive(false);
            }
        }
        //綾田AIの行動
        else if (RoomController.getAyada_Aiflag() && RoomController.getA_Ai_Playerflag() == playercontrolle.PlayerCheck())
        {
            if (!a_aicontrolle.A_AiFlag && !CheckScript.fcheckmate_draw)
            {
                a_aicontrolle.A_AiFlag = true;
                a_aicontrolle.CheckArea();
                a_aicontrolle.AiMoveSearch();
            }
        }
        //久井AIの行動
        else if (RoomController.getH_Aiflag() && RoomController.getH_Ai_Playerflag() == playercontrolle.PlayerCheck())
        {
            if (!h_aicontrolle.H_AiFlag && !CheckScript.fcheckmate_draw)
            {
                h_aicontrolle.H_AiFlag = true;
                h_aicontrolle.CheckArea();
                h_aicontrolle.AiMoveSearch();
            }
        }
        //相手のターン中は相手の行動が終わるまで相手のサーバのデータチェック
        else
        {
            netcontrolle.CheckNet();
            TurnText.SetActive(false);
            E_TurnText.SetActive(true);
        }
        //PieceSelect();
        //if (clickcontrolle.getRightClick() && !movecontrolle.MoveFlag)
        //    Reset();
    }

    //クリックされた駒ごとのスクリプト呼ぶ
    private void PieceSelect()
    {         
        getobj = clickcontrolle.getClickObject();

        //駒がクリックされている&&ポーン進化のフラグがおちている
        if (getobj != null && !movecontrolle.EvolutionFlag)
        {
            //どちらのターンかチェック
            if (playercontrolle.PlayerCheck() && getobj.transform.parent.gameObject.tag == "P1")
            {
            }
            else if (!playercontrolle.PlayerCheck() && getobj.transform.parent.gameObject.tag == "P2")
            {
            }
            else
            {
                Reset();
                return;
            }

            movecontrolle.MoveObject = getobj;
            switch (getobj.tag)
            {
                //移動できない駒の場合falseを返す　その場合はリセット
                case "Pawn":
                    if (!Pcontrolle.Move(getobj))
                        Reset();
                    break;
                case "Rook":
                    if (!Rcontrolle.Move(getobj))
                        Reset();
                    break;
                case "Knight":
                    if (!Kncontrolle.Move(getobj))
                        Reset();
                    break;
                case "Bishop":
                    if (!Bcontrolle.Move(getobj))
                        Reset();
                    break;
                case "Queen":
                    if (!Qcontrolle.Move(getobj))
                        Reset();
                    break;
                case "King":
                    if (!Kcontrolle.Move(getobj))
                        Reset();
                    break;
                default:
                    Debug.Log("null");
                    break;
            }
        }
    }

    //駒がどこにいるかサーチ
    public int SearchZ(GameObject k)
    {
        int Z = 0;
         
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                if (Piece.Piece_Obj[z, x] == k)
                {
                    Z = z;
                }
            }
        }
        return Z;
    }

    //駒がどこにいるかサーチ
    public int SearchX(GameObject k)
    {
        int X = 0;
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                if (Piece.Piece_Obj[z, x] == k)
                {
                    X = x;
                }
            }
        }
        return X;
    }

    //駒が移動完了または右クリックでリセット
    public void Reset()
    {
        GameObject board;    

        getobj = null;
        if (!movecontrolle.EvolutionFlag)
            movecontrolle.MoveObject = null;
        movecontrolle.board = null;
        KomaOnOffFlag = false;
        mCount = 0;
        //Castlingしたならフラグを落とす
        if (cast.CastFlag)
            cast.CastFlag = false;

        //boardのlayer,tagをリセット
        for (var z = 0; z < Mass_Size; z++)
        {
            for (var x = 0; x < Mass_Size; x++)
            {
                board = GameObject.Find("Base[" + z + "," + x + "]");
                board.layer = 0;
                if (board.tag == "Enpassant" && !movecontrolle.En_Flag)
                    board.tag = "Untagged";
                else if (board.tag == "LeftCastling" || board.tag == "RightCastling")
                    board.tag = "Untagged";
            }
        }
    }

}
