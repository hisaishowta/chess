using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class BishopScript : MonoBehaviour
{
    //マス目サイズ
    private const int Mass_Size = 8;
    //クリックの位置
    private int nowX;
    private int nowZ;

    [SerializeField]
    private GameObject KomaCntl;
    KomaController koma_Cntl = null;

    [SerializeField]
    private GameObject FieldCntl;
    PieceScript piece = null;

    // Use this for initialization
    void Start()
    {
        koma_Cntl = KomaCntl.GetComponent<KomaController>();
        piece = FieldCntl.GetComponent<PieceScript>();
    }

    public bool Move(GameObject BishopObj)
    {
        //クリックの位置(z,x)
        nowZ = koma_Cntl.SearchZ(BishopObj);
        nowX = koma_Cntl.SearchX(BishopObj);
        //移動範囲サーチ
        return MoveSearch(nowZ, nowX, BishopObj);
    }

    //移動範囲サーチ
    public bool MoveSearch(int checkZ, int checkX, GameObject obj)
    { 
        //移動できるボードのレイヤーを変更
        //--------------------------------------------------------------
        for (int i = -1; i < 2; i++)
        {
            if ((checkZ + i < Mass_Size) && (checkZ + i > -1))
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i != 0 && j != 0)
                    {
                        int sumi = i, sumj = j;
                        //上左右・下左右確認
                        while (MoveCheck(checkZ, checkX, sumi, sumj, obj))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
        //移動数フラグ
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
                if (obj.transform.parent.gameObject.tag !=
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