﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class RookScript : MonoBehaviour
{

    //マス目サイズ
    private const int Mass_Size = 8;
    //クリックの位置
    private int nowX;
    private int nowZ;

    private bool CastCheck = false;//Castlingチェック時の無限ループ防ぐ用

    [SerializeField]
    private GameObject KomaCntl;
    KomaController koma_Cntl = null;

    [SerializeField]
    private GameObject FieldCntl;
    PieceScript piece = null;

    [SerializeField]
    private GameObject MoveCntl;
    CastlingController cast = null;
    MoveController move_Cntl = null;

    [SerializeField]
    private GameObject CheckCntl;
    CheckScript check_Cntl = null;

    [SerializeField]
    private GameObject PlayerCntl;
    PlayerController player_Cntl = null;

    // Use this for initialization
    void Start()
    {
        koma_Cntl = KomaCntl.GetComponent<KomaController>();
        piece = FieldCntl.GetComponent<PieceScript>();
        cast = MoveCntl.GetComponent<CastlingController>();
        move_Cntl = MoveCntl.GetComponent<MoveController>();
        check_Cntl = CheckCntl.GetComponent<CheckScript>();
        player_Cntl = PlayerCntl.GetComponent<PlayerController>();
    }

    public bool Move(GameObject RookObj)
    {

        //クリックの位置(z,x)
        nowZ = koma_Cntl.SearchZ(RookObj);
        nowX = koma_Cntl.SearchX(RookObj);
        //移動範囲サーチ
        return MoveSearch(nowZ, nowX, RookObj);
    }

    //移動範囲サーチ
    public bool MoveSearch(int checkZ, int checkX, GameObject obj)
    {
        GameObject board;　//Castlingチェック用
        
        //移動できるボードのレイヤーを変更
        //--------------------------------------------------------------
        for (int i = -1; i < 2; i++)
        {
            if ((checkZ + i < Mass_Size) && (checkZ + i > -1))
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((!(i == 0 && j == 0)) &&
                        (i == 0 || j == 0))
                    {
                        int sumi = i, sumj = j;
                        //上下左右確認
                        while (MoveCheck(checkZ, checkX, sumi, sumj, obj))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //Castlingチェック

        //オブジェクトがnullでない && 敵の駒にチェックされてない && Check_Chessでの無限ループ防ぐフラグ
        if (move_Cntl.MoveObject != null && !cast.Kcast_Check && !CastCheck)
        {
            //初期位置から動いていない&&クリックされたのが右側のルークなら 
            if (((cast.P1Cas_RightR && cast.P1Cas_King) && (move_Cntl.MoveObject.name == "P1_Rook1"))
                || ((cast.P2Cas_RightR && cast.P2Cas_King) && (move_Cntl.MoveObject.name == "P2_Rook1")))
            {
                int check = 0;
                //左側に駒がいないかチェック　
                for (int i = 1; i < 3; i++)
                {
                    if ((board = GameObject.Find("Base[" + checkZ + "," + (checkX - i) + "]")) != null)
                    {
                        if (piece.Piece_Obj[checkZ, checkX - i] == null)
                        {
                            check++;
                            if (check == 2)
                            {
                                CastCheck = true;
                                bool flag = player_Cntl.PlayerCheck();
                                check_Cntl.Reset();
                                check_Cntl.Check_AreaFlag(!flag);
                                //Castling時の移動マスが敵駒の攻撃範囲に入ってないかチェック 大丈夫ならボードのタグを変更
                                if (CheckScript.Check_flag[checkZ, checkX - 1] == 0 &&
                                    CheckScript.Check_flag[checkZ, checkX - 2] == 0)
                                {
                                    board = GameObject.Find("Base[" + checkZ + "," + (checkX - 3) + "]");
                                    board.tag = "RightCastling";
                                    board.layer = 9;
                                }
                            }
                        }
                    }
                }
            }
            //初期位置から動いていない&&クリックされたのが左側のルークなら　ボードのタグを変更
            else if (((cast.P1Cas_LeftR && cast.P1Cas_King) && (move_Cntl.MoveObject.name == "P1_Rook0"))
                || ((cast.P2Cas_LeftR && cast.P2Cas_King) && (move_Cntl.MoveObject.name == "P2_Rook0")))
            {
                int check = 0;
                //右側に駒がいないかチェック　
                for (int i = 1; i < 4; i++)
                {
                    if ((board = GameObject.Find("Base[" + checkZ + "," + (checkX + i) + "]")) != null)
                    {
                        if (piece.Piece_Obj[checkZ, checkX + i] == null)
                        {
                            check++;
                            if (check == 3)
                            {
                                if (!CastCheck)
                                {
                                    CastCheck = true;
                                    bool flag = player_Cntl.PlayerCheck();
                                    check_Cntl.Reset();
                                    check_Cntl.Check_AreaFlag(!flag);
                                }
                                //Castling時の移動マスが敵駒の攻撃範囲に入ってないかチェック 大丈夫ならボードのタグを変更
                                if (CheckScript.Check_flag[checkZ, checkX + 1] == 0 && 
                                    CheckScript.Check_flag[checkZ, checkX + 2] == 0 && 
                                    CheckScript.Check_flag[checkZ, checkX + 3] == 0)
                                {
                                    board = GameObject.Find("Base[" + checkZ + "," + (checkX + 4) + "]");
                                    board.tag = "LeftCastling";
                                    board.layer = 9;
                                }
                            }
                        }
                    }
                }
            }
            CastCheck = false;
        }

        //-----------------------------------------------------------------------------------------------------

        if (koma_Cntl.mCount > 0)
            return true;
        else
            return false;
    }

    private bool MoveCheck(int checkZ, int checkX, int nextZ, int nextX, GameObject obj)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;
        //参照Baseオブジェクト格納
        GameObject board;

        //移動できるボードのレイヤーを変更
        //--------------------------------------------------------------
        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //参照先確認
            if (piece.Piece_Obj[watchZ, watchX] != null)
            {
                //参照Baseオブジェクト検出
                board = GameObject.Find("Base[" + watchZ + "," + watchX + "]");
                //クリックオブジェクトの親タグと進行方向にいるオブジェクトの親タグを比較
                if (/*!cast.CastFlag &&*/ obj.transform.parent.gameObject.tag !=
                    piece.Piece_Obj[watchZ, watchX].transform.parent.gameObject.tag)
                {
                    board.layer = 9;
                    
                    koma_Cntl.mCount++;
                    return false;
                }
            }
            else if ((board = GameObject.Find("Base[" + watchZ + "," + watchX + "]")) != null)
            {
                board.layer = 9;
                
                koma_Cntl.mCount++;
                return true;
            }
        }
        return false;
    }
}