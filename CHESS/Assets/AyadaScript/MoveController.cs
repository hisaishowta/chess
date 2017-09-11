using UnityEngine;

public class MoveController : MonoBehaviour
{
    //マス目サイズ
    private const int Mass_Size = 8;

    private int ProId;

    public int TurnCount = 0;

    public GameObject MoveObject;//クリックされている駒格納
    public GameObject board; //クリックされているボード格納

    //アンパッサン処理関係
    private GameObject En_pawn; //アンパッサン時のポーン格納用
    private GameObject En_board;//アンパッサン時のボード格納用
    public bool En_Flag = false;


    //別スクリプト取得用
    private GameObject PlayerControlle;
    private GameObject KomaControlle;
    private GameObject ClickControlle;
    private GameObject FieldControlle;
    private GameObject MoveControlle;
    private GameObject NetControlle;
    private GameObject AiControlle;
    PlayerController playercontrolle;
    KomaController komacontrolle;
    ClickController clickcontrolle;
    FieldScript field;
    PieceScript Piece;
    CastlingController cast;
    NetController netcontrolle;
    A_AIController a_aicontrolle;
    H_AIController h_aicontrolle;

    private GameObject CheckCntl;
    CheckScript check;

    public bool ClickMoveFlag = false;//ボードがクリックされて目的地が設定されているか
    public bool EvolutionFlag = false;//ポーン進化中のフラグ 
    public bool MoveFlag = false;//移動中かどうかのフラグ
    private bool UpFlag = false;
    public bool DownFlag = false;
    public float movementPeriod = 0.5f; //指定位置までの駒の移動秒数


    public Vector3 target;//クリック位置格納変数
    public Vector3 Uptarget;
    public Vector3 Downtarget;
    public Vector3 Castarget;
    Vector3 fromPosition;
    Vector3 toPosition;
    Vector3 CfromPosition;
    Vector3 CtoPosition;
    float movementTime = 0;

    /// <summary>50手ルール</summary>
    public static int rule50;

    // Use this for initialization
    void Start()
    {
        MoveObject = null;
        board = null;
        //別スクリプト取得
        PlayerControlle = GameObject.Find("PlayerController");
        KomaControlle = GameObject.Find("KomaController");
        ClickControlle = GameObject.Find("ClickController");
        FieldControlle = GameObject.Find("GameSetting/FieldController");
        MoveControlle = GameObject.Find("MoveController");
        NetControlle = GameObject.Find("NetController");
        AiControlle = GameObject.Find("AiController");
        playercontrolle = PlayerControlle.GetComponent<PlayerController>();
        komacontrolle = KomaControlle.GetComponent<KomaController>();
        clickcontrolle = ClickControlle.GetComponent<ClickController>();
        field = FieldControlle.GetComponent<FieldScript>();
        Piece = FieldControlle.GetComponent<PieceScript>();
        cast = MoveControlle.GetComponent<CastlingController>();
        netcontrolle = NetControlle.GetComponent<NetController>();
        a_aicontrolle = AiControlle.GetComponent<A_AIController>();
        h_aicontrolle = AiControlle.GetComponent<H_AIController>();

        CheckCntl = GameObject.Find("CheckController");
        check = CheckCntl.GetComponent<CheckScript>();
    }

    // Update is called once per frame
    void Update()
    {
        PieceMove();
    }

    //目標地点に駒を移動させる
    private void PieceMove()
    {

        if (MoveObject != null)
        {
            //上に移動
            if (UpFlag)
            {
                MoveFlag = true;
                movementTime += Time.deltaTime;                                     // 経過時間を増やす
                float ratio2 = Mathf.Lerp(0, 1, movementTime / movementPeriod);     // 移動の時間に対する今の経過時間の割合

                // 移動
                MoveObject.transform.position = Vector3.Lerp(fromPosition, toPosition, ratio2);

                // 移動終了時に各パラメータを初期化。フラグを下ろす。
                if (ratio2 == 1)
                {
                    ClickMoveFlag = true;
                    UpFlag = false;
                    fromPosition = MoveObject.transform.position;                          // 移動前のポジション
                    MoveObject.transform.position = target;
                    toPosition = MoveObject.transform.position;                            // 移動後のポジション
                    MoveObject.transform.position = fromPosition;                          // Positionを移動前に戻す
                    movementTime = 0;                                           　　　　　　// 移動中の経過時間を0に
                    if (cast.CastFlag)
                    {
                        CfromPosition = cast.CasObj.transform.position;                          // 移動前のポジション
                        cast.CasObj.transform.position = Castarget;
                        CtoPosition = cast.CasObj.transform.position;                            // 移動後のポジション
                        cast.CasObj.transform.position = CfromPosition;                          // Positionを移動前に戻す
                    }
                }
            }
            //平行移動
            else if (ClickMoveFlag)
            {
                movementTime += Time.deltaTime;                                     // 経過時間を増やす
                float ratio2 = Mathf.Lerp(0, 1, movementTime / movementPeriod);     // 移動の時間に対する今の経過時間の割合

                // 移動
                MoveObject.transform.position = Vector3.Lerp(fromPosition, toPosition, ratio2);
                if (cast.CastFlag)
                    cast.CasObj.transform.position = Vector3.Lerp(CfromPosition, CtoPosition, ratio2);

                // 移動終了時に各パラメータを初期化。フラグを下ろす。
                if (ratio2 == 1)
                {
                    ClickMoveFlag = false;
                    DownFlag = true;
                    fromPosition = MoveObject.transform.position;                          // 移動前のポジション
                    MoveObject.transform.position = Downtarget;
                    toPosition = MoveObject.transform.position;                            // 移動後のポジション
                    MoveObject.transform.position = fromPosition;                          // Positionを移動前に戻す
                    movementTime = 0;
                }
            }
            //下に移動
            else if (DownFlag)
            {
                movementTime += Time.deltaTime;                                     // 経過時間を増やす
                float ratio2 = Mathf.Lerp(0, 1, movementTime / movementPeriod);     // 移動の時間に対する今の経過時間の割合

                // 移動
                MoveObject.transform.position = Vector3.Lerp(fromPosition, toPosition, ratio2);

                // 移動終了時に各パラメータを初期化。フラグを下ろす。
                if (ratio2 == 1)
                {
                    //駒移動後ポーンなら進化判定
                    if (MoveObject.tag == "Pawn")
                    {
                        int pawnZ;
                        pawnZ = komacontrolle.SearchZ(MoveObject);
                        if ((pawnZ == 0 || pawnZ == 7))
                        {
                            if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
                            {
                                clickcontrolle.pawnText.SetActive(true);
                                clickcontrolle.knightbtn.SetActive(true);
                                clickcontrolle.queenbtn.SetActive(true);
                                clickcontrolle.rookbtn.SetActive(true);
                                clickcontrolle.bishopbtn.SetActive(true);
                            }
                            EvolutionFlag = true;
                        }
                    }
                    DownFlag = false;
                    movementTime = 0;                                           　　　　　　// 移動中の経過時間を0に
                }
            }
            else
            {
                if (board == null)
                    SetTarget();

                //相手がpromotionしたとき入る
                if (EvolutionFlag && RoomController.getplayerflag() != playercontrolle.PlayerCheck())
                {
                    //相手のproidのデータからどれにpromotionするか判定
                    if (ProId == 1)
                        clickcontrolle.QueenPawn();
                    else if (ProId == 2)
                        clickcontrolle.RookPawn();
                    else if (ProId == 3)
                        clickcontrolle.BishopPawn();
                    else if (ProId == 4)
                        clickcontrolle.KnightPawn();
                }

                //移動中&&駒が選択されている&&プロモーション中でない   移動完了で入る
                if (MoveFlag && komacontrolle.KomaOnOffFlag && !EvolutionFlag)
                {
                    //自分のターンなら入り、サーバのデータ更新してからリセットする
                    if (RoomController.getplayerflag() == playercontrolle.PlayerCheck() && !RoomController.getAyada_Aiflag())
                    {
                        if (!netcontrolle.C_DataSetStart)
                            StartCoroutine(netcontrolle.C_DataSet());//データ更新
                        if (netcontrolle.C_DataSetEnd)
                        {
                            netcontrolle.E_P_TurnEnd = false;

                            //-------------------------------------------
                            //Check判定
                            bool flag = playercontrolle.PlayerCheck();
                            //チェックフラグ付け

                            check.Check_Chess(flag);
                            check.Reset();

                            playercontrolle.TurnState(); 
                            //-------------------------------------------
                            komacontrolle.Reset();

                            netcontrolle.C_DataSetStart = false;
                            netcontrolle.C_DataSetEnd = false;
                            MoveFlag = false;
                        }
                    }
                    //自分のターンでないなら更新せずそのままリセット
                    else
                    {
                        netcontrolle.E_P_TurnEnd = false;

                        //-------------------------------------------
                        //Check判定
                        bool flag = playercontrolle.PlayerCheck();
                        //チェックフラグ付け

                        check.Check_Chess(flag);
                        check.Reset();

                        playercontrolle.TurnState(); 

                        a_aicontrolle.A_AiFlag = false;
                        h_aicontrolle.H_AiFlag = false;
                        //-------------------------------------------
                        komacontrolle.Reset();

                        MoveFlag = false;
                        TurnCount++;
                    }
                }
            }
        }

    }

    //目標地点の設定 + Castling,Enpassantチェック  自ターンの移動で入る関数
    private void SetTarget()
    {
        int fromZ, fromX;//クリックオブジェクトの移動前のポジション判定用

        //左クリックされているか　&&　駒がクリックされている状態か　　
        if (clickcontrolle.getLeftClick() && komacontrolle.KomaOnOffFlag)
        {
            board = clickcontrolle.getClickBoard();//クリックされたボード取得

            if (board != null)
            {
                // hitしたボードの位置を格納する           
                target = new Vector3(board.transform.position.x, 0.5f, board.transform.position.z);
                Uptarget = new Vector3(MoveObject.transform.position.x, 0.5f, MoveObject.transform.position.z);
                Downtarget = new Vector3(board.transform.position.x, 0, board.transform.position.z);

                UpFlag = true;

                fromZ = komacontrolle.SearchZ(MoveObject);                             //移動する駒の移動前のポジション保存
                fromX = komacontrolle.SearchX(MoveObject);                             //

                Piece.Piece_Obj[fromZ, fromX] = null;                                  //移動前のポジションをnullに

                //Castling処理------------------------------------------
                cast.CastCheck();
                if (board.tag == "LeftCastling")
                {
                    cast.LCas_Process(fromZ, fromX, MoveObject, board);
                }
                else if (board.tag == "RightCastling")
                {
                    cast.RCas_Process(fromZ, fromX, MoveObject, board);
                }
                //------------------------------------------------------

                fromPosition = MoveObject.transform.position;                          // 移動前のポジション
                MoveObject.transform.position = Uptarget;
                toPosition = MoveObject.transform.position;                            // 移動後のポジション
                MoveObject.transform.position = fromPosition;                          // Positionを移動前に戻す
                movementTime = 0;                                           　　　　　　// 移動中の経過時間を0に

                //Enpassantの条件下の時入る-----------------

                if (board.tag == "Enpassant" || En_Flag)
                    En_Process(fromZ, fromX);

                //-----------------------------------------

                //Castlingの時処理しない Castlingの方の関数で入れ替え
                if (!cast.CastFlag)
                {
                    for (var z = 0; z < Mass_Size; z++)
                    {
                        for (var x = 0; x < Mass_Size; x++)
                        {
                            if (field.Base_Obj[z, x] == board)
                            {

                                if (Piece.Piece_Obj[z, x] != null)
                                {
                                    //Castlingなら消さない 基本全体のオブジェクトOFF
                                    Piece.Piece_Obj[z, x].SetActive(false);

                                    rule50 = 0;
                                }
                                else if (MoveObject.tag != "Pawn") { rule50++; }

                                //駒のデータをセット
                                netcontrolle.personvalue.fromZid = fromZ;
                                netcontrolle.personvalue.fromXid = fromX;
                                netcontrolle.personvalue.toZid = z;
                                netcontrolle.personvalue.toXid = x;

                                Piece.Piece_Obj[z, x] = MoveObject;       //移動先の配列にいれる

                                //---------------------------------------------------
                                //配列入れ替え
                                FieldScript.Piece_Num[z, x] = FieldScript.Piece_Num[fromZ, fromX];
                                FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.None;
                                //---------------------------------------------------
                            }
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    //相手のサーバのデータをもとに駒を自動で動かす   相手ターンの移動の時入る関数
    public void E_PlayerSetTarget(int fromZ, int fromX, int toZ, int toX, int castid, int enid, int proid)
    {
        //クリックオブジェクト取得
        MoveObject = Piece.Piece_Obj[fromZ, fromX];

        board = field.Base_Obj[toZ, toX];

        ProId = proid;

        //相手がCastlingしていなければ移動先のボードの位置を格納する    
        if (castid == 0)
        {
            target = new Vector3(field.Base_Obj[toZ, toX].transform.position.x, 0.5f, field.Base_Obj[toZ, toX].transform.position.z);
            Uptarget = new Vector3(Piece.Piece_Obj[fromZ, fromX].transform.position.x, 0.5f, Piece.Piece_Obj[fromZ, fromX].transform.position.z);
            Downtarget = new Vector3(field.Base_Obj[toZ, toX].transform.position.x, 0, field.Base_Obj[toZ, toX].transform.position.z);
        }

        UpFlag = true;

        Piece.Piece_Obj[fromZ, fromX] = null;                                  //移動前のポジションをnullに

        //Castling処理------------------------------------------
        if (castid == 1)
        {
            if (MoveObject.tag == "King")
                board = field.Base_Obj[fromZ, fromX - 4];
            else
                board = field.Base_Obj[fromZ, fromX + 4];
            cast.LCas_Process(fromZ, fromX, MoveObject, board);
        }
        else if (castid == 2)
        {
            if (MoveObject.tag == "King")
                board = field.Base_Obj[fromZ, fromX + 3];
            else
                board = field.Base_Obj[fromZ, fromX - 3];
            cast.RCas_Process(fromZ, fromX, MoveObject, board);
        }
        //------------------------------------------------------

        fromPosition = MoveObject.transform.position;                          // 移動前のポジション
        MoveObject.transform.position = Uptarget;
        toPosition = MoveObject.transform.position;                            // 移動後のポジション
        MoveObject.transform.position = fromPosition;                          // Positionを移動前に戻す
        movementTime = 0;                                                      // 移動中の経過時間を0に

        //Enpassantの条件下の時入る-----------------

        if (enid == 1 || En_Flag)
            En_Process(fromZ, fromX);

        //-----------------------------------------

        //Castlingしてない時のみ入る
        if (castid == 0)
        {
            if (Piece.Piece_Obj[toZ, toX] != null)
            {
                Piece.Piece_Obj[toZ, toX].SetActive(false);

                rule50 = 0;
            }
            else if (MoveObject.tag != "Pawn") { rule50++; }
            Piece.Piece_Obj[toZ, toX] = MoveObject;       //移動先の配列にいれる
            //---------------------------------------------------
            //配列入れ替え
            FieldScript.Piece_Num[toZ, toX] = FieldScript.Piece_Num[fromZ, fromX];
            FieldScript.Piece_Num[fromZ, fromX] = (int)FieldScript.Chess_Field.None;
            //---------------------------------------------------
        }
        komacontrolle.KomaOnOffFlag = true;
    }

    //Enpassantの処理
    private void En_Process(int fromZ, int fromX)
    {
        if (En_Flag && MoveObject.tag == "Pawn")
        {
            En_Flag = false;
            if (board == En_board)
            {
                En_pawn.SetActive(false);
                for (var z = 0; z < Mass_Size; z++)
                {
                    for (var x = 0; x < Mass_Size; x++)
                    {
                        if (Piece.Piece_Obj[z, x] == En_pawn)
                        {
                            Piece.Piece_Obj[z, x] = null;
                            //---------------------------------------------------
                            //配列入れ替え
                            FieldScript.Piece_Num[z, x] = 0;
                            //---------------------------------------------------
                        }
                    }
                }
            }
            En_board.tag = "Untagged";
            En_board = null;
            En_pawn = null;
        }
        else if (En_Flag)
        {
            En_board.tag = "Untagged";
            En_Flag = false;
            En_board = null;
            En_pawn = null;
        }
        //ポーンが2マス移動後P1,P2を判定し1つ後方のマスをEnpassantできるよう処理
        else if (playercontrolle.PlayerCheck())
        {
            En_pawn = MoveObject;
            En_board = GameObject.Find("Base[" + (fromZ + 1) + "," + fromX + "]");
            En_Flag = true;
            board.tag = "Untagged";
            En_board.tag = "Enpassant";
            if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
                netcontrolle.personvalue.enid = 1;
        }
        else if (!playercontrolle.PlayerCheck())
        {
            En_pawn = MoveObject;
            En_board = GameObject.Find("Base[" + (fromZ - 1) + "," + fromX + "]");
            En_Flag = true;
            board.tag = "Untagged";
            En_board.tag = "Enpassant";
            if (RoomController.getplayerflag() == playercontrolle.PlayerCheck())
                netcontrolle.personvalue.enid = 1;
        }
        rule50 = 0;
    }
}