using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckScript : MonoBehaviour
{
    //マス目サイズ
    private const int Mass_Size = 8;
    //チェスフィールド値
    private const int mField01 = 100;
    private const int mField02 = 200;

    //テキスト表示用
    private GameObject gClickController;
    ClickController c_cntl;

    //キャスリング
    private GameObject gCastlingController;
    CastlingController CCntl;

    //再チェック
    CheckChange C_CheckCntl;
    CheckChangePawn C_CheckPCntl;
    //再フラグ付け
    CheckKing C_KCntl;
    CheckQueen C_QCntl;
    CheckBishop C_BCntl;
    CheckKnight C_KnCntl;
    CheckRook C_RCntl;
    CheckPawn C_PCntl;

    Result ResultCntl;

    ///<summary>勝敗判定用</summary>
    public static byte[,] Check_flag =
        new byte[Mass_Size, Mass_Size];

    ///<summary>勝敗判定用</summary>
    public static byte[,] Copy_Check_flag =
        new byte[Mass_Size, Mass_Size];

    ///<summary>駒数値格納</summary>
    public static int[,] Copy_Piece_Num =
        new int[Mass_Size, Mass_Size];

    public static bool fcheckmate_draw;

    // Use this for initialization
    void Start()
    {
        //テキスト表示用
        gClickController = GameObject.Find("ClickController");
        c_cntl = gClickController.GetComponent<ClickController>();
        //キャスリング
        gCastlingController = GameObject.Find("MoveController");
        CCntl = gCastlingController.GetComponent<CastlingController>();

        //再判定用
        C_CheckCntl = GetComponent<CheckChange>();
        C_CheckPCntl = GetComponent<CheckChangePawn>();
        //再フラグ付け
        C_KCntl = GetComponent<CheckKing>();
        C_QCntl = GetComponent<CheckQueen>();
        C_BCntl = GetComponent<CheckBishop>();
        C_KnCntl = GetComponent<CheckKnight>();
        C_RCntl = GetComponent<CheckRook>();
        C_PCntl = GetComponent<CheckPawn>();

        CopyNum();  //配列種別値コピー
        Reset();    // 8x8の配列を0で初期化

        ResultCntl = GetComponent<Result>();
        fcheckmate_draw = false;
    }

    public void Check_Chess(bool player)
    {
        //チェスフィールド判定値
        int chessNum = 0;

        //Player2からみて相手のフィールド値
        if (!player) { chessNum = mField01; }
        //Player1からみて相手のフィールド値
        else { chessNum = mField02; }

        c_cntl.checkText.SetActive(false);
        c_cntl.mateText.SetActive(false);
        Reset();
        CopyNum();

        int fJudge = 0;                   //勝敗判定

        //相手PlayerのKingの位置
        int nking = isChecked(chessNum);   //King配列合算値
        if (nking == mField01)
        {
            Debug.Log("ERROR01\nKingがありません");
            if (player) { fJudge = 1; }
            else { fJudge = 2; }
        }
        int nk_z = nking / Mass_Size;      //KingZ値
        int nk_x = nking % Mass_Size;      //KingX値
        
        CopyFlag();
        Check_AreaFlag(player);

        //Check確認
        //1.Kingが存在するかどうか
        //2.相手のキングが現在のPlayerの攻撃範囲内にいるか判定
        //3.キャスリングフラグ
        if ((nking != mField01) && Check_King(nk_z, nk_x) && !CCntl.CastFlag)
        {
            c_cntl.checkText.SetActive(true);
            CCntl.Kcast_Check = true;
            //CheckMate確認(移動可能範囲確認)
            if (canMoveObj(player))
            {
				c_cntl.checkText.SetActive(false);
                c_cntl.mateText.SetActive(true);
                
                if (player) { fJudge = 1; }
                else { fJudge = 2; }
                fcheckmate_draw = true;
            }
        }
        else
        {
            c_cntl.checkText.SetActive(false);
            c_cntl.mateText.SetActive(false);
            CCntl.Kcast_Check = false;
        }
        //Check_AreaFlag(player);
        //Debug-----
        ArrayView();
        //----------
        ResultCntl.ChangeJudge(fJudge);
    }

    ///// <summary>指定時間後に実行する</summary>
    //IEnumerator DelayMethod()
    //{
    //    yield return new WaitForSeconds(3.0f); // 3秒待つ
    //}

    ///<summary>Playerの移動範囲のフラグ付け</summary>
    public void Check_AreaFlag(bool player)
    {
        //Player1，2移動後確認
        for (sbyte z = 0; z < Mass_Size; z++)
        {
            for (sbyte x = 0; x < Mass_Size; x++)
            {
                //-------------------------------------------
                //Playerの移動範囲のフラグ付け
                //Player1
                if (player)
                {
                    //---------------------------------------
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[z, x])
                    {
                        //King
                        case mField01:
                            C_KCntl.MoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Queen
                        case mField01 + 1:
                            C_QCntl.MoveSearch(z , x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Bishop
                        case mField01 + 2:
                            C_BCntl.MoveSearch(z , x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Knight
                        case mField01 + 3:
                            C_KnCntl.MoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Rook
                        case mField01 + 4:
                            C_RCntl.MoveSearch(z , x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Pawn
                        case mField01 + 5:
                            C_PCntl.MoveSearch(z , x, FieldScript.Piece_Num[z, x]);
                            break;                       
                        default:
                            break;
                    }
                    //---------------------------------------
                }
                //-------------------------------------------
                //Player2
                else
                {
                    //---------------------------------------
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[z, x])
                    {
                        //King
                        case mField02:
                            C_KCntl.MoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Queen
                        case mField02 + 1:
                            C_QCntl.MoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Bishop
                        case mField02 + 2:
                            C_BCntl.MoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Knight
                        case mField02 + 3:
                            C_KnCntl.MoveSearch(z , x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Rook
                        case mField02 + 4:
                            C_RCntl.MoveSearch(z , x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Pawn
                        case mField02 + 5:
                            C_PCntl.MoveSearch(z , x, FieldScript.Piece_Num[z, x]);
                            break;
                        default:
                            break;
                    }
                    //---------------------------------------
                }
                //-------------------------------------------
            }
        }
    }

    ///<summary>各playerのキング位置</summary>
    public int isChecked(int chessNum)
    {
        for (sbyte z = 0; z < Mass_Size; z++)
        {
            for (sbyte x = 0; x < Mass_Size; x++)
            {
                if (FieldScript.Piece_Num[z, x] == chessNum)
                {
                    return z * Mass_Size + x;
                }
            }
        }
        return mField01;
    }

    ///<summary>チェックされているかどうか確認</summary>
    private bool Check_King(int nk_z, int nk_x)
    {
        //Kingボード上の位置
        if (Check_flag[nk_z, nk_x] >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    ///<summary>King移動可能範囲からチェックの確認</summary>
    private bool CheckMate_Around(int nk_z, int nk_x)
    {
        //Check確認フラグ
        bool cflag = false;
        //------------------------------------
        //King周囲Check
        for (sbyte i = -1; i < 2; i++)
        {
            if ((nk_z + i < Mass_Size) && (nk_z + i > -1))
            {
                for (sbyte j = -1; j < 2; j++)
                {
                    if ((nk_x + j < Mass_Size) && (nk_x + j > -1))
                    {
                        //自軍の位置確認
                        if (!(FieldScript.Piece_Num[(nk_z + i), (nk_x + j)] >
                            FieldScript.Piece_Num[nk_z, nk_x] &&
                            FieldScript.Piece_Num[(nk_z + i), (nk_x + j)] <
                            FieldScript.Piece_Num[nk_z, nk_x] + Mass_Size))
                        {
                            if (Check_flag[(nk_z + i), (nk_x + j)] >= 1)
                            {
                                cflag = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------
        return cflag;
    }

    ///<summary>総当り(1つずつ移動チェック確認)</summary>
    private bool canMoveObj(bool player)
    {
        ///<summary>移動後のフラグ</summary>        
        bool moveFlag = false;
        int nking;              //King配列合算値
        int nk_z;               //KingZ値
        int nk_x;               //KingX値
        //Knight移動先
        int[,] knight_num = {
            { -2, -1 }, { -2, 1 },
            { -1,-2}, { -1, 2 },
            { 1, -2 }, { 1, 2 },
            { 2, -1 }, { 2, 1 } };
        //Pawn移動先
        int[,] pawn_num = {
            { 1, 0 }, { 2, 0 },
            { 1,-1}, { 1, 1 },
            { -1, 0 }, { -2, 0 },
            { -1,-1}, { -1, 1 }};

        //Player1，2移動後確認
        for (int movez = 0; movez < Mass_Size; movez++)
        {
            for (int movex = 0; movex < Mass_Size; movex++)
            {
                //仮移動確認配列初期化
                CopyNum();
                Reset();
                int number = 0;     //Field値格納用
                //-------------------------------------------
                //相手のKingをチェックしている時
                //相手駒の動きからチェックメイトしているか確認
                if (!player)
                {
                    number = mField01;
                    //---------------------------------------
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[movez, movex])
                    {
                        //King
                        case mField01:
                            //--------------------------------------------------------------
                            //全方位確認
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if (!(i == 0 && j == 0))
                                        {
                                            Reset();
                                            C_CheckCntl.MoveCheck(movez, movex, i, j,
                                                Copy_Piece_Num[movez, movex]);
                                            Copy_Check_AreaFlag(player);
                                            //Debug.Log(mField01); ArrayView();//@@@@@--------------------------------------------@@@@@
                                            //相手PlayerKingの位置
                                            nking = reisChecked(number);    //King配列合算値
                                            nk_z = nking / Mass_Size;       //KingZ値
                                            nk_x = nking % Mass_Size;       //KingX値
                                            //移動した後でもチェックされている場合.又.King獲られる場合
                                            if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                            //仮移動確認配列初期化
                                            CopyNum();
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Queen
                        case mField01 + 1:
                            //--------------------------------------------------------------
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if (!(i == 0 && j == 0))
                                        {
                                            int sumi = i, sumj = j;
                                            //全方位確認
                                            do
                                            {
                                                moveFlag = C_CheckCntl.MoveCheck(
                                                    movez, movex,
                                                    sumi, sumj,
                                                    Copy_Piece_Num[movez, movex]);
                                                Reset();
                                                Copy_Check_AreaFlag(player);
                                                //Debug.Log(mField01 + 1); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                //相手PlayerKingの位置
                                                nking = reisChecked(number);  //King配列合算値
                                                nk_z = nking / Mass_Size;     //KingZ値
                                                nk_x = nking % Mass_Size;     //KingX値
                                                //移動した後でもチェックされている場合.又.King獲られる場合
                                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                                sumi = sumi + i;
                                                sumj = sumj + j;
                                                //仮移動確認配列初期化
                                                CopyNum();
                                            } while (moveFlag);
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Bishop
                        case mField01 + 2:
                            //--------------------------------------------------------------
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if (i != 0 && j != 0)
                                        {
                                            int sumi = i, sumj = j;
                                            //全方位確認

                                            do
                                            {
                                                moveFlag = C_CheckCntl.MoveCheck(
                                                    movez, movex,
                                                    sumi, sumj,
                                            Copy_Piece_Num[movez, movex]);
                                                Reset();
                                                Copy_Check_AreaFlag(player);
                                                //Debug.Log(mField01 + 2); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                //相手PlayerKingの位置
                                                nking = reisChecked(number);  //King配列合算値
                                                nk_z = nking / Mass_Size;     //KingZ値
                                                nk_x = nking % Mass_Size;     //KingX値
                                                //移動した後でもチェックされている場合.又.King獲られる場合
                                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                                sumi = sumi + i;
                                                sumj = sumj + j;
                                                //仮移動確認配列初期化
                                                CopyNum();
                                            } while (moveFlag);
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Knight
                        case mField01 + 3:
                            //--------------------------------------------------------------
                            //全方位確認
                            for (int i = 0; i < Mass_Size; i++)
                            {
                                C_CheckCntl.MoveCheck(movez, movex, knight_num[i, 0], knight_num[i, 1],
                                     Copy_Piece_Num[movez, movex]);
                                Reset();
                                Copy_Check_AreaFlag(player);
                               // Debug.Log(mField01 + 3); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                                     //相手PlayerKingの位置
                                nking = reisChecked(number);  //King配列合算値
                                nk_z = nking / Mass_Size;     //KingZ値
                                nk_x = nking % Mass_Size;     //KingX値
                                //移動した後でもチェックされている場合.又.King獲られる場合
                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                //仮移動確認配列初期化
                                CopyNum();
                            }
                            //--------------------------------------------------------------
                            break;
                        //Rook
                        case mField01 + 4:
                            //--------------------------------------------------------------
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if ((!(i == 0 && j == 0)) &&
                                            (i == 0 || j == 0))
                                        {
                                            int sumi = i, sumj = j;
                                            //全方位確認
                                            do
                                            {
                                                moveFlag = C_CheckCntl.MoveCheck(
                                                movez, movex,
                                                sumi, sumj,
                                                Copy_Piece_Num[movez, movex]);
                                                Reset();
                                                Copy_Check_AreaFlag(player);
                                               // Debug.Log(mField01 + 4); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                //相手PlayerKingの位置
                                                nking = reisChecked(number);  //King配列合算値
                                                nk_z = nking / Mass_Size;     //KingZ値
                                                nk_x = nking % Mass_Size;     //KingX値
                                                //移動した後でもチェックされている場合.又.King獲られる場合
                                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                                sumi = sumi + i;
                                                sumj = sumj + j;
                                                //仮移動確認配列初期化
                                                CopyNum();
                                            } while (moveFlag);
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Pawn
                        case mField01 + 5:
                            //--------------------------------------------------------------
                            //全方位確認
                            for (int i = 0; i < 4; i++)
                            {
                                C_CheckPCntl.MoveCheck(
                                    movez, movex,
                                    pawn_num[i, 0], pawn_num[i, 1],
                                    Copy_Piece_Num[movez, movex]);
                                Reset();
                                Copy_Check_AreaFlag(player);
                                //Debug.Log(mField01 + 5); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                                     //相手PlayerKingの位置
                                nking = reisChecked(number);  //King配列合算値
                                nk_z = nking / Mass_Size;     //KingZ値
                                nk_x = nking % Mass_Size;     //KingX値
                                                              //移動した後でもチェックされている場合.又.King獲られる場合
                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                //仮移動確認配列初期化
                                CopyNum();
                            }
                            //--------------------------------------------------------------
                            break;
                        default:
                            break;
                    }
                    //---------------------------------------
                }
                //-------------------------------------------
                //Player2
                else
                {
                    number = mField02;
                    //---------------------------------------
                    //King,Queen,Bishop,Knight,Rook,Pown
                    switch (FieldScript.Piece_Num[movez, movex])
                    {
                        //King
                        case mField02:
                            //--------------------------------------------------------------
                            //全方位確認
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if (!(i == 0 && j == 0))
                                        {
                                            C_CheckCntl.MoveCheck(
                                                movez, movex,
                                                i, j,
                                                Copy_Piece_Num[movez, movex]);
                                            Reset();
                                            Copy_Check_AreaFlag(player);
                                            //Debug.Log(mField02); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                                             //相手PlayerKingの位置
                                            nking = reisChecked(number);    //King配列合算値
                                            nk_z = nking / Mass_Size;       //KingZ値
                                            nk_x = nking % Mass_Size;       //KingX値
                                                                            //移動した後でもチェックされている場合.又.King獲られる場合
                                            if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }

                                            //仮移動確認配列初期化
                                            CopyNum();
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Queen
                        case mField02 + 1:
                            //--------------------------------------------------------------
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if (!(i == 0 && j == 0))
                                        {
                                            int sumi = i, sumj = j;
                                            //全方位確認
                                            do
                                            {
                                                moveFlag = C_CheckCntl.MoveCheck(
                                                    movez, movex,
                                                    sumi, sumj,
                                                Copy_Piece_Num[movez, movex]);
                                                Reset();
                                                Copy_Check_AreaFlag(player);
                                                //Debug.Log(mField02 + 1); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                //相手PlayerKingの位置
                                                nking = reisChecked(number);  //King配列合算値
                                                nk_z = nking / Mass_Size;     //KingZ値
                                                nk_x = nking % Mass_Size;     //KingX値
                                                //どこかに移動した時にチェックされているか確認
                                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                                sumi = sumi + i;
                                                sumj = sumj + j;
                                                //仮移動確認配列初期化
                                                CopyNum();
                                            } while (moveFlag);
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Bishop
                        case mField02 + 2:
                            //--------------------------------------------------------------
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if (i != 0 && j != 0)
                                        {
                                            int sumi = i, sumj = j;
                                            //全方位確認
                                            do
                                            {
                                                moveFlag = C_CheckCntl.MoveCheck(
                                                    movez, movex,
                                                    sumi, sumj,
                                                Copy_Piece_Num[movez, movex]);
                                                Reset();
                                                Copy_Check_AreaFlag(player);
                                                //Debug.Log(mField02 + 2); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                //相手PlayerKingの位置
                                                nking = reisChecked(number);  //King配列合算値
                                                nk_z = nking / Mass_Size;     //KingZ値
                                                nk_x = nking % Mass_Size;     //KingX値
                                                //どこかに移動した時にチェックされているか確認
                                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                                sumi = sumi + i;
                                                sumj = sumj + j;
                                                //仮移動確認配列初期化
                                                CopyNum();
                                            } while (moveFlag);
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Knight
                        case mField02 + 3:
                            //--------------------------------------------------------------
                            //全方位確認
                            for (int i = 0; i < Mass_Size; i++)
                            {
                                C_CheckCntl.MoveCheck(movez, movex, knight_num[i, 0], knight_num[i, 1],
                                    Copy_Piece_Num[movez, movex]);
                                Reset();
                                Copy_Check_AreaFlag(player);
                               // Debug.Log(mField02 + 3); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                                     //相手PlayerKingの位置
                                nking = reisChecked(number);  //King配列合算値
                                nk_z = nking / Mass_Size;     //KingZ値
                                nk_x = nking % Mass_Size;     //KingX値
                                                              //どこかに移動した時にチェックされているか確認
                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                //仮移動確認配列初期化
                                CopyNum();
                            }
                            //--------------------------------------------------------------
                            break;
                        //Rook
                        case mField02 + 4:
                            //--------------------------------------------------------------
                            for (int i = -1; i < 2; i++)
                            {
                                if ((movez + i < Mass_Size) && (movez + i > -1))
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if ((!(i == 0 && j == 0)) &&
                                            (i == 0 || j == 0))
                                        {
                                            int sumi = i, sumj = j;
                                            //全方位確認
                                            do
                                            {
                                                moveFlag = C_CheckCntl.MoveCheck(
                                                    movez, movex,
                                                    sumi, sumj,
                                                Copy_Piece_Num[movez, movex]);
                                                Reset();
                                                Copy_Check_AreaFlag(player);
                                                //Debug.Log(mField02 + 4); ArrayView();//@@@@@--------------------------------------------@@@@@
                                                //相手PlayerKingの位置
                                                nking = reisChecked(number);  //King配列合算値
                                                nk_z = nking / Mass_Size;     //KingZ値
                                                nk_x = nking % Mass_Size;     //KingX値
                                                //どこかに移動した時にチェックされているか確認
                                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                                sumi = sumi + i;
                                                sumj = sumj + j;
                                                //仮移動確認配列初期化
                                                CopyNum();
                                            } while (moveFlag);
                                        }
                                    }
                                }
                            }
                            //--------------------------------------------------------------
                            break;
                        //Pawn
                        case mField02 + 5:
                            //--------------------------------------------------------------
                            //全方位確認
                            for (int i = 0; i < 4; i++)
                            {
                                C_CheckPCntl.MoveCheck(movez, movex, pawn_num[i, 0], pawn_num[i, 1],
                                    Copy_Piece_Num[movez, movex]);
                                Reset();
                                Copy_Check_AreaFlag(player);
                                //Debug.Log(mField02 + 5); ArrayView();//@@@@@--------------------------------------------@@@@@
                                //相手PlayerKingの位置
                                nking = reisChecked(number);  //King配列合算値
                                nk_z = nking / Mass_Size;     //KingZ値
                                nk_x = nking % Mass_Size;     //KingX値
                                                              //どこかに移動した時にチェックされているか確認
                                if (!Check_King(nk_z, nk_x) || nking == mField01) { return false; }
                                //仮移動確認配列初期化
                                CopyNum();
                            }
                            //--------------------------------------------------------------
                            break;
                        default:
                            break;
                    }
                    //---------------------------------------
                }
                //-------------------------------------------
            }
        }
        return true;
    }

    ///<summary>各playerのキング位置 コピーからチェック</summary>
    private int reisChecked(int chessNum)
    {
        for (sbyte z = 0; z < Mass_Size; z++)
        {
            for (sbyte x = 0; x < Mass_Size; x++)
            {
                if (Copy_Piece_Num[z, x] == chessNum)
                {
                    return z * Mass_Size + x;
                }
            }
        }
        return mField01;
    }

    ///<summary>チェック判定用配列のリセット</summary>
    public void Reset()
    {
        // 8x8の配列をfalseで初期化
        for (int i = 0; i < Check_flag.GetLength(0); i++)
        {
            for (int j = 0; j < Check_flag.GetLength(1); j++)
            {
                Check_flag[i, j] = 0;
            }
        }
    }

    ///<summary>元の駒の状態(配列の数値)のコピー</summary>
    private void CopyNum()
    {
        for (int z = 0; z < Mass_Size; z++)
        {
            for (int x = 0; x < Mass_Size; x++)
            {
                Copy_Piece_Num[z, x] = FieldScript.Piece_Num[z, x];
            }
        }
    }

    ///<summary>元の駒の状態(配列の数値)のリセット</summary>
    public void CopyFlag()
    {
        for (int z = 0; z < Mass_Size; z++)
        {
            for (int x = 0; x < Mass_Size; x++)
            {
                Copy_Check_flag[z, x] = 0;
            }
        }
    }

    ///<summary>Playerの移動範囲のフラグ付け </summary>
    public void Copy_Check_AreaFlag(bool player)
    {
        //-------------------------------------------
        //Player1，2移動後確認
        for (int z = 0; z < Mass_Size; z++)
        {
            for (int x = 0; x < Mass_Size; x++)
            {
                //-------------------------------------------
                //Playerの移動範囲のフラグ付け
                //Player1
                if (player)
                {
                    //---------------------------------------
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (Copy_Piece_Num[z, x])
                    {
                        //King
                        case mField01:
                            C_KCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Queen
                        case mField01 + 1:
                            C_QCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Bishop
                        case mField01 + 2:
                            C_BCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Knight
                        case mField01 + 3:
                            C_KnCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Rook
                        case mField01 + 4:
                            C_RCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Pawn
                        case mField01 + 5:
                            C_PCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        default:
                            break;
                    }
                    //---------------------------------------
                }
                //-------------------------------------------
                //Player2
                else
                {
                    //---------------------------------------
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (Copy_Piece_Num[z, x])
                    {
                        //King
                        case mField02:
                            C_KCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Queen
                        case mField02 + 1:
                            C_QCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Bishop
                        case mField02 + 2:
                            C_BCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Knight
                        case mField02 + 3:
                            C_KnCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Rook
                        case mField02 + 4:
                            C_RCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        //Pawn
                        case mField02 + 5:
                            C_PCntl.MoveSearch(z, x, Copy_Piece_Num[z, x]);
                            break;
                        default:
                            break;
                    }
                    //---------------------------------------
                }
                //-------------------------------------------
            }
        }
        //-------------------------------------------
    }

    ///<summary>King移動可能範囲からチェックの確認</summary>
    public bool Re_CheckMate_Around(int nk_z, int nk_x)
    {
        //Check確認フラグ
        bool cflag = false;
        //------------------------------------
        //King周囲Check
        for (sbyte i = -1; i < 2; i++)
        {
            if ((nk_z + i < Mass_Size) && (nk_z + i > -1))
            {
                for (sbyte j = -1; j < 2; j++)
                {
                    if ((nk_x + j < Mass_Size) && (nk_x + j > -1))
                    {
                        //自軍の位置確認
                        if (!(Copy_Piece_Num[(nk_z + i), (nk_x + j)] >
                            Copy_Piece_Num[nk_z, nk_x] &&
                            Copy_Piece_Num[(nk_z + i), (nk_x + j)] <
                            Copy_Piece_Num[nk_z, nk_x] + Mass_Size))
                        {
                            if (Check_flag[(nk_z + i), (nk_x + j)] >= 1)
                            {
                                cflag = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------
        return cflag;
    }

    ///<summary>配列可視化参照</summary>
    private void ArrayView()
    {
        string print_array = "";
        //Debug.Log("Copy_Check_flag");
        //for (int i = Copy_Check_flag.GetLength(0) - 1; i >= 0; i--)
        //{
        //    for (int j = 0; j < Copy_Check_flag.GetLength(1); j++)
        //    {
        //        print_array += Copy_Check_flag[i, j].ToString("D2") + ":";
        //    }
        //    print_array += "\n";
        //}
        //Debug.Log(print_array);
        //print_array = "";
        Debug.Log("Piece_Num");
        for (int i = FieldScript.Piece_Num.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < FieldScript.Piece_Num.GetLength(1); j++)
            {
                print_array += FieldScript.Piece_Num[i, j].ToString("D3") + ":";
            }
            print_array += "\n";
        }
        Debug.Log(print_array);
    }
}