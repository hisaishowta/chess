﻿using UnityEngine;

#pragma warning disable 0649

public class CheckKing : MonoBehaviour
{
    //マス目サイズ
    private const int Mass_Size = 8;
    //チェスフィールド値
    private const int mField01 = 100;
    private const int mField02 = 200;
    //移動範囲サーチ
    public void     MoveSearch(int checkZ, int checkX, int obj_num)
    {
        //全方位確認
        for (int i = -1; i < 2; i++)
        {
            if ((checkZ + i < Mass_Size) && (checkZ + i > -1))
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        MoveCheck(checkZ, checkX, i, j, obj_num);
                    }
                }
            }
        }
    }

    private void MoveCheck(int checkZ, int checkX, int nextZ, int nextX, int obj_num)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //駒がある場合
            if (CheckScript.Copy_Piece_Num[watchZ, watchX] != 0)
            {
                //敵味方共にフラグ
                CheckScript.Check_flag[watchZ, watchX]++;
                CheckScript.Copy_Check_flag[watchZ, watchX]++;
            }
            //駒がない場合
            else if (CheckScript.Copy_Piece_Num[watchZ, watchX] == 0)
            {
                CheckScript.Check_flag[watchZ, watchX]++;
                CheckScript.Copy_Check_flag[watchZ, watchX]++;
            }
        }
    }
    public bool Move(int nowZ, int nowX, int obj_num)
    {
        //全方位確認
        for (int i = -1; i < 2; i++)
        {
            if ((nowZ + i < Mass_Size) && (nowZ + i > -1))
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        if (MoveCount(nowZ, nowX, i, j, obj_num))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool MoveCount(int nowZ, int nowX, int nextZ, int nextX, int obj_num)
    {
        //参照先配列値Z,X
        int watchZ = nowZ + nextZ;
        int watchX = nowX + nextX;

        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //駒がある場合
            if (CheckScript.Copy_Piece_Num[watchZ, watchX] != 0)
            {
                //敵駒の場合
                if (!(obj_num + Mass_Size > CheckScript.Copy_Piece_Num[watchZ, watchX] &&
                    obj_num - Mass_Size < CheckScript.Copy_Piece_Num[watchZ, watchX]))
                {
                    return true;
                }
            }
            //駒がない場合
            else if (CheckScript.Copy_Piece_Num[watchZ, watchX] == 0)
            {
                if (CheckScript.Copy_Check_flag[watchZ, watchX]==0)
                {
                return true;
                }
            }
        }
        return false;
    }
}