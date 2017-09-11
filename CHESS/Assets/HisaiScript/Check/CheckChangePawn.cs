using UnityEngine;

public class CheckChangePawn : MonoBehaviour
{
    //King値
    private const int mKing01 = 100;
    private const int mKing02 = 200;

    //マス目サイズ
    private const int Mass_Size = 8;

    //移動範囲サーチ
    public bool MoveCheck(int checkZ, int checkX, int nextZ, int nextX, int num)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //P1Pawn P2Pawn
            if ((nextZ == 1 && num < mKing02) || (nextZ == -1 && num > mKing02))
            {
                //一つ前方(P1Pawn P2Pawn)
                if (nextX == 0)
                {
                    if ((checkZ < 7 && checkZ > 0) && CheckScript.Copy_Piece_Num[watchZ, watchX] == 0)
                    {
                        CheckScript.Copy_Piece_Num[watchZ, checkX] = CheckScript.Copy_Piece_Num[checkZ, checkX];
                        CheckScript.Copy_Piece_Num[checkZ, checkX] = 0;
                        return true;
                    }
                    return false;
                }
                //左上_右上_左下_右下(P1Pawn P2Pawn)
                if (nextX == -1 || nextX == 1)
                {
                    if ((checkZ < 7 && checkZ > 0) && CheckScript.Copy_Piece_Num[watchZ, watchX] > 0)
                    {
                        //敵駒の場合
                        if (!(num + Mass_Size > CheckScript.Copy_Piece_Num[watchZ, watchX] &&
                            num - Mass_Size < CheckScript.Copy_Piece_Num[watchZ, watchX]))
                        {
                            CheckScript.Copy_Piece_Num[watchZ, watchX] = CheckScript.Copy_Piece_Num[checkZ, checkX];
                            CheckScript.Copy_Piece_Num[checkZ, checkX] = 0;
                            return true;
                        }
                    }
                }
                return false;
            }
            //初期位置の場合(P1Pawn P2Pawn)
            else if ((nextZ == 2 && num < mKing02) || (nextZ == -2 && num > mKing02))
            {
                //前方と2マス先に何もいない場合(P1Pawn P2Pawn)
                if (CheckScript.Copy_Piece_Num[watchZ, watchX] == 0 &&
                    CheckScript.Copy_Piece_Num[watchZ - 1, watchX] == 0)
                {
                    if ((num < 200 && checkZ == 1) ||
                        (num > 200 && checkZ == 6))
                    {
                        CheckScript.Copy_Piece_Num[watchZ, watchX] = CheckScript.Copy_Piece_Num[checkZ, checkX];
                        CheckScript.Copy_Piece_Num[checkZ, checkX] = 0;
                        return true;
                    }
                }
            }
        }
        return false;
    }
}