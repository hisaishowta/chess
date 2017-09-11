using UnityEngine;

public class CheckChange : MonoBehaviour
{
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
            //駒がある場合
            if (CheckScript.Copy_Piece_Num[watchZ, watchX] != 0)
            {
                //敵駒の場合
                if (!(num + Mass_Size > CheckScript.Copy_Piece_Num[watchZ, watchX] &&
                    num - Mass_Size < CheckScript.Copy_Piece_Num[watchZ, watchX]))
                {
                    CheckScript.Copy_Piece_Num[watchZ, watchX] = CheckScript.Copy_Piece_Num[checkZ, checkX];
                    CheckScript.Copy_Piece_Num[checkZ, checkX] = 0;
                    return false;
                }
            }
            //駒がない場合
            else if (CheckScript.Copy_Piece_Num[watchZ, watchX] == 0)
            {
                CheckScript.Copy_Piece_Num[watchZ, watchX] = CheckScript.Copy_Piece_Num[checkZ, checkX];
                CheckScript.Copy_Piece_Num[checkZ, checkX] = 0;
                return true;
            }
        }
        return false;
    }
}