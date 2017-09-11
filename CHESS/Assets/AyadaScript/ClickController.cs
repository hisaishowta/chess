using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour {

   // private GameObject getobj;

    //別スクリプト取得
    private GameObject KomaControlle;
    private GameObject MoveControlle;
    private GameObject FieldControlle;
    private GameObject NetControlle;
    private GameObject PlayerControlle;
    KomaController komacontrolle;
    MoveController movecontrolle;
    PieceScript Piece;
    NetController netcontrolle;
    PlayerController playercontrolle;

    //クリックボタン関係
    public GameObject pawnText;//進化時テキスト
    public GameObject knightbtn;//進化時のナイトボタン
    public GameObject queenbtn;//進化時のクイーンボタン
    public GameObject rookbtn;//進化時のルークボタン
    public GameObject bishopbtn;//進化時のビショップボタン

    //Checkテキスト
    public GameObject checkText; //CheckText
    public GameObject mateText;  //CheckText
    public GameObject Draw; //Draw

    //Bishopプレファブ格納用
    public GameObject Prefab_Bishop_P1;
    public GameObject Prefab_Bishop_P2;
    //Castleプレファブ格納用
    public GameObject Prefab_Rook_P1;
    public GameObject Prefab_Rook_P2;
    //Knightプレファブ格納用
    public GameObject Prefab_Knight_P1;
    public GameObject Prefab_Knight_P2;
    //Queenプレファブ格納用
    public GameObject Prefab_Queen_P1;
    public GameObject Prefab_Queen_P2;

    //check判定
    private GameObject CheckCntl;

    /// <summary>
    /// promotionフラグ
    /// </summary>
    public static bool fPromotion;

    // Use this for initialization
    void Start () {
        //別スクリプト取得
        KomaControlle = GameObject.Find("KomaController");
        MoveControlle = GameObject.Find("MoveController");
        FieldControlle = GameObject.Find("GameSetting/FieldController");
        NetControlle = GameObject.Find("NetController");
        komacontrolle = KomaControlle.GetComponent<KomaController>();
        movecontrolle = MoveControlle.GetComponent<MoveController>();
        Piece = FieldControlle.GetComponent<PieceScript>();
        netcontrolle = NetControlle.GetComponent<NetController>();

        //クリックボタン関係
        pawnText = GameObject.Find("Canvas/PawnText");
        knightbtn = GameObject.Find("Canvas/KnightButton");
        queenbtn = GameObject.Find("Canvas/QueenButton");
        rookbtn = GameObject.Find("Canvas/RookButton");
        bishopbtn = GameObject.Find("Canvas/BishopButton");
        
        pawnText.SetActive(false);
        knightbtn.SetActive(false);
        queenbtn.SetActive(false);
        rookbtn.SetActive(false);
        bishopbtn.SetActive(false);

        //CheckText関係
        checkText = GameObject.Find("Canvas/CheckText");
        checkText.SetActive(false);
        mateText = GameObject.Find("Canvas/MateText");
        mateText.SetActive(false);
        Draw = GameObject.Find("Canvas/DrawText");
        Draw.SetActive(false);
        
        fPromotion = false;
    }

    //クリックしたオブジェクトを返す
    public GameObject getClickObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        GameObject result = null;

        if (Input.GetMouseButtonDown(0) && !komacontrolle.KomaOnOffFlag)
        {
            if (Physics.Raycast(ray, out hit, 100f, 1 << 8))
            {      
                result = hit.collider.gameObject;
                komacontrolle.KomaOnOffFlag = true;        
            }
        }
        return result;
    }

    //クリックしたボードを返す
    public GameObject getClickBoard()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        GameObject Board = null;

        if (Input.GetMouseButtonDown(0) && komacontrolle.KomaOnOffFlag)
        {
            if (Physics.Raycast(ray, out hit, 100f, 1 << 9))
            {
                Board = hit.collider.gameObject;
            }
        }
        return Board;
    }

    //左クリックしたかの判定
    public bool getLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }

    //右クリックしたかの判定
    public bool getRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            return true;
        }
        return false;
    }

//ポーンPromotion関係------------------------------------------------------------------------------------------------------

    //進化時のボタンで起動
    public void ButtonDown()
    {
        pawnText.SetActive(false);
        knightbtn.SetActive(false);
        queenbtn.SetActive(false);
        rookbtn.SetActive(false);
        bishopbtn.SetActive(false);
    }

    //クイーンにPromotion
    public void QueenPawn()
    {
        int fromZ, fromX;
        fromZ = komacontrolle.SearchZ(movecontrolle.MoveObject);                             //ポーンのポジション保存
        fromX = komacontrolle.SearchX(movecontrolle.MoveObject);                             //
        GameObject PwPiece; //プロモーションするオブジェクト格納
        //P1,P2判定
        if (movecontrolle.MoveObject.transform.parent.gameObject.tag == "P1")
        {
            PwPiece = Instantiate(
                          Prefab_Queen_P1,
                          movecontrolle.MoveObject.transform.position,
                         Quaternion.identity) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player01").transform;
            PwPiece.name = "P1_Queen2";                //オブジェクト名
            //ポーンがいた配列にQueen代入
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P1_Queen;
        }
        else {
            PwPiece = Instantiate(
                          Prefab_Queen_P2,
                          movecontrolle.MoveObject.transform.position,
                         Quaternion.identity) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player02").transform;
            PwPiece.name = "P2_Queen2";                //オブジェクト名
            //ポーンがいた配列にQueen代入
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P2_Queen;
        }
        Piece.Piece_Obj[fromZ, fromX].SetActive(false);
        Piece.Piece_Obj[fromZ, fromX] = PwPiece;  //ポーンがいた配列に新たな駒代入
        movecontrolle.EvolutionFlag = false;//進化中フラグおとす
       // if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
            netcontrolle.personvalue.proid = 1;
    }

    //ナイトにPromotion
    public void KnightPawn()
    {
        int fromZ, fromX;
        fromZ = komacontrolle.SearchZ(movecontrolle.MoveObject);                             //ポーンのポジション保存
        fromX = komacontrolle.SearchX(movecontrolle.MoveObject);                             //
        GameObject PwPiece; //プロモーションするオブジェクト格納
        //P1,P2判定後オブジェクト生成
        if (movecontrolle.MoveObject.transform.parent.gameObject.tag == "P1")
        {
            PwPiece = Instantiate(
                          Prefab_Knight_P1,
                          movecontrolle.MoveObject.transform.position,
                         Quaternion.identity) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player01").transform;
            PwPiece.name = "P1_Knight3";                //オブジェクト名
            //ポーンがいた配列にKnight代入
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P1_Knight; 
        }
        else
        {
            PwPiece = Instantiate(
                          Prefab_Knight_P2,
                          movecontrolle.MoveObject.transform.position,
                          new Quaternion(0f, 1f, 0f, 0f)) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player02").transform;
            PwPiece.name = "P2_Knight3";                //オブジェクト名
            //ポーンがいた配列にKnight代入
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P2_Knight;
        }
        Piece.Piece_Obj[fromZ, fromX].SetActive(false);
        Piece.Piece_Obj[fromZ, fromX] = PwPiece;    //ポーンがいた配列に新たな駒代入           
        movecontrolle.EvolutionFlag = false;//進化中フラグおとす
       // if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
            netcontrolle.personvalue.proid = 4;
    }

    //ビショップにPromotion
    public void BishopPawn()
    {
        int fromZ, fromX;
        fromZ = komacontrolle.SearchZ(movecontrolle.MoveObject);                             //ポーンのポジション保存
        fromX = komacontrolle.SearchX(movecontrolle.MoveObject);                             //
        GameObject PwPiece; //プロモーションするオブジェクト格納
        //P1,P2判定
        if (movecontrolle.MoveObject.transform.parent.gameObject.tag == "P1")
        {
            PwPiece = Instantiate(
                          Prefab_Bishop_P1,
                          movecontrolle.MoveObject.transform.position,
                         Quaternion.identity) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player01").transform;
            PwPiece.name = "P1_Bishop3";                //オブジェクト名
            //ポーンがいた配列にBishop代入
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P1_Bishop;
        }
        else {
            PwPiece = Instantiate(
                         Prefab_Bishop_P2,
                         movecontrolle.MoveObject.transform.position,
                        Quaternion.identity) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player02").transform;
            PwPiece.name = "P2_Bishop3";                //オブジェクト名
            //ポーンがいた配列にBishop代入
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P2_Bishop;
        }
        Piece.Piece_Obj[fromZ, fromX].SetActive(false);
        Piece.Piece_Obj[fromZ, fromX] = PwPiece;  //ポーンがいた配列に新たな駒代入
        movecontrolle.EvolutionFlag = false;//進化中フラグおとす
       // if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
            netcontrolle.personvalue.proid = 3;
    }
    
    //ルークにPromotion
    public void RookPawn()
    {
        int fromZ, fromX;
        fromZ = komacontrolle.SearchZ(movecontrolle.MoveObject);                             //ポーンのポジション保存
        fromX = komacontrolle.SearchX(movecontrolle.MoveObject);                             //
        GameObject PwPiece; //プロモーションするオブジェクト格納
        //P1,P2判定
        if (movecontrolle.MoveObject.transform.parent.gameObject.tag == "P1")
        {
            PwPiece = Instantiate(
                          Prefab_Rook_P1,
                          movecontrolle.MoveObject.transform.position,
                         Quaternion.identity) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player01").transform;
            PwPiece.name = "P1_Rook3";                //オブジェクト名
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P1_Rook;
        }
        else {
            PwPiece = Instantiate(
                          Prefab_Rook_P2,
                          movecontrolle.MoveObject.transform.position,
                         Quaternion.identity) as GameObject;
            PwPiece.transform.parent = GameObject.Find("Player02").transform;
            PwPiece.name = "P2_Rook3";                //オブジェクト名
            //ポーンがいた配列にRook代入
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.P2_Rook;
        }
        Piece.Piece_Obj[fromZ, fromX].SetActive(false);
        Piece.Piece_Obj[fromZ, fromX] = PwPiece;  //ポーンがいた配列に新たな駒代入
        movecontrolle.EvolutionFlag = false;//進化中フラグおとす
       // if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
            netcontrolle.personvalue.proid = 2;
    }

   
}
