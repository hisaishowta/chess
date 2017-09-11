using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAi : MonoBehaviour {

    //駒をとる場合の点数
    private const int KiPoint = 200;
    private const int Queen_P = 15;
    private const int Rook_P = 10;
    private const int Bishop_P = 10;
    private const int Knight_P = 5;
    private const int Pawn_P = 2;

    //駒を取られる場合の点数
    private const int MyKiPoint = 200;
    private const int MyQueen_P = 10;
    private const int MyRook_P = 7;
    private const int MyBishop_P = 7;
    private const int MyKnight_P = 3;
    private const int MyPawn_P = 1;

    //マス目サイズ
    private const int Mass_Size = 8;

    private bool moreFlag;
    public bool A_AiFlag = false;

    //チェスフィールド値
    private const int mField01 = 100;
    private const int mField02 = 200;

    public int[,] Piece_Ai_P = new int[Mass_Size, Mass_Size];

    private int[] PieceSet = { 0, 0, 0, 0, 0, 0, 0 }; //点数,fromZ,fromX,toZ,toX,obj_num,enid

    //Pawn移動先
    private int[,] pawn_num = {
            { 1,-1}, { 1, 1 },
            { -1,-1}, { -1, 1 },
            { 1, 0 }, { -1, 0 },
            { 2, 0 }, { -2, 0 } };

    //Knight移動先
    private int[,] knight_num = {
            { -2, -1 }, { -2, 1 },
            { -1,-2}, { -1, 2 },
            { 1, -2 }, { 1, 2 },
            { 2, -1 }, { 2, 1 } };

    private GameObject PlayerControlle;
    private GameObject CheckControlle;
    private GameObject MoveControlle;
    PlayerController playercontrolle;
    CheckScript checkcontrolle;
    MoveController movecontrolle;

    // Use this for initialization
    void Start()
    {
        PlayerControlle = GameObject.Find("PlayerController");
        CheckControlle = GameObject.Find("CheckController");
        MoveControlle = GameObject.Find("MoveController");
        playercontrolle = PlayerControlle.GetComponent<PlayerController>();
        checkcontrolle = CheckControlle.GetComponent<CheckScript>();
        movecontrolle = MoveControlle.GetComponent<MoveController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //相手の駒の範囲チェック
    public void CheckArea()
    {
        bool flag = playercontrolle.PlayerCheck();
        // checkcontrolle.Reset();
        checkcontrolle.Check_AreaFlag(!flag);
        for (int i = 0; i < PieceSet.Length; i++)
            PieceSet[i] = -1;
    }

    public void AiMoveSearch()
    {
        //Player1，2移動後確認
        for (sbyte z = 0; z < Mass_Size; z++)
        {
            for (sbyte x = 0; x < Mass_Size; x++)
            {
                //-------------------------------------------
                //Player1
                if (RoomController.getH_Ai_Playerflag())
                {
                    //---------------------------------------
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[z, x])
                    {
                        //King
                        case mField01:
                            KingMoveSearch(z, x);
                            break;
                        //Queen
                        case mField01 + 1:
                            QueenMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Bishop
                        case mField01 + 2:
                            BishopMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Knight
                        case mField01 + 3:
                            KnightMoveSearch(z, x);
                            break;
                        //Rook
                        case mField01 + 4:
                            RookMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Pawn
                        case mField01 + 5:
                            PawnMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        default:
                            break;
                    }
                }
                //Player2
                else
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[z, x])
                    {
                        //King
                        case mField02:
                            KingMoveSearch(z, x);
                            break;
                        //Queen
                        case mField02 + 1:
                            QueenMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Bishop
                        case mField02 + 2:
                            BishopMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Knight
                        case mField02 + 3:
                            KnightMoveSearch(z, x);
                            break;
                        //Rook
                        case mField02 + 4:
                            RookMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        //Pawn
                        case mField02 + 5:
                            PawnMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        //promotionの場合
        if (PieceSet[5] != 0)
            movecontrolle.E_PlayerSetTarget(PieceSet[1], PieceSet[2], PieceSet[3], PieceSet[4], 0, 0, 1);
        else if (PieceSet[6] != 0)
            movecontrolle.E_PlayerSetTarget(PieceSet[1], PieceSet[2], PieceSet[3], PieceSet[4], 0, 1, 0);
        else
            movecontrolle.E_PlayerSetTarget(PieceSet[1], PieceSet[2], PieceSet[3], PieceSet[4], 0, 0, 0);
    }


    //Kingの移動範囲サーチ---------------------------------------------------------------------------
    public void KingMoveSearch(int checkZ, int checkX)
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
                        KingMoveCheck(checkZ, checkX, i, j);
                    }
                }
            }
        }
    }

    private void KingMoveCheck(int checkZ, int checkX, int nextZ, int nextX)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;
        int AiPoint = 0;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //現在のマスが敵の駒の範囲に入っている場合
            if (CheckScript.Check_flag[checkZ, checkX] != 0)
                AiPoint += MyKiPoint;

            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                if (RoomController.getH_Ai_Playerflag())
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField02:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField02 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += Pawn_P;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                else
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField01:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField01 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += Pawn_P;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyKiPoint;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyKiPoint;
            }
            if (PieceSet[0] < AiPoint)
            {
                PieceSet[0] = AiPoint;
                PieceSet[1] = checkZ;
                PieceSet[2] = checkX;
                PieceSet[3] = watchZ;
                PieceSet[4] = watchX;
                PieceSet[5] = 0;
                PieceSet[6] = 0;
            }
            else if (PieceSet[0] == AiPoint)
            {
                if (Random.Range(0, 2) == 0)
                {
                    PieceSet[1] = checkZ;
                    PieceSet[2] = checkX;
                    PieceSet[3] = watchZ;
                    PieceSet[4] = watchX;
                    PieceSet[5] = 0;
                    PieceSet[6] = 0;
                }
            }
        }
    }
    //-----------------------------------------------------------------------------------------------------


    //Queen,Bishp,Rookの移動範囲サーチ---------------------------------------------------------------------------------
    public void QueenMoveSearch(int checkZ, int checkX, int obj_num)
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
                        moreFlag = false;
                        int sumi = i, sumj = j;
                        while (Q_B_R_MoveCheck(checkZ, checkX, sumi, sumj, obj_num))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
    }

    //Bishop移動範囲サーチ
    public void BishopMoveSearch(int checkZ, int checkX, int obj_num)
    {
        //全方位確認
        for (int i = -1; i < 2; i++)
        {
            if ((checkZ + i < Mass_Size) && (checkZ + i > -1))
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i != 0 && j != 0)
                    {
                        moreFlag = false;
                        int sumi = i, sumj = j;
                        while (Q_B_R_MoveCheck(checkZ, checkX, sumi, sumj, obj_num))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
    }

    //Rook移動範囲サーチ
    public void RookMoveSearch(int checkZ, int checkX, int obj_num)
    {
        //全方位確認
        for (int i = -1; i < 2; i++)
        {
            if ((checkZ + i < Mass_Size) && (checkZ + i > -1))
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((!(i == 0 && j == 0)) &&
                        (i == 0 || j == 0))
                    {
                        moreFlag = false;
                        int sumi = i, sumj = j;
                        while (Q_B_R_MoveCheck(checkZ, checkX, sumi, sumj, obj_num))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
    }

    //Queen,Bishop,Rookの移動判定
    private bool Q_B_R_MoveCheck(int checkZ, int checkX, int nextZ, int nextX, int obj_num)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;
        int AiPoint = 0;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {

            //現在のマスが敵の駒の範囲に入っている場合
            if (CheckScript.Check_flag[checkZ, checkX] != 0)
            {
                switch (obj_num)
                {
                    //Queen
                    case mField01 + 1:
                    case mField02 + 1:
                        AiPoint += MyQueen_P;
                        break;
                    //Bishop
                    case mField01 + 2:
                    case mField02 + 2:
                        AiPoint += MyBishop_P;
                        break;
                    //Rook
                    case mField01 + 4:
                    case mField02 + 4:
                        AiPoint += MyRook_P;
                        break;
                }
            }


            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                if (RoomController.getH_Ai_Playerflag())
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField02:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField02 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += Pawn_P;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                else
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField01:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField01 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += Pawn_P;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                {
                    switch (obj_num)
                    {
                        //Queen
                        case mField01 + 1:
                        case mField02 + 1:
                            AiPoint -= MyQueen_P;
                            break;
                        //Bishop
                        case mField01 + 2:
                        case mField02 + 2:
                            AiPoint -= MyBishop_P;
                            break;
                        //Rook
                        case mField01 + 4:
                        case mField02 + 4:
                            AiPoint -= MyRook_P;
                            break;
                    }
                }

                if (PieceSet[0] < AiPoint)
                {
                    PieceSet[0] = AiPoint;
                    PieceSet[1] = checkZ;
                    PieceSet[2] = checkX;
                    PieceSet[3] = watchZ;
                    PieceSet[4] = watchX;
                    PieceSet[5] = 0;
                    PieceSet[6] = 0;
                }
                else if (PieceSet[0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        PieceSet[1] = checkZ;
                        PieceSet[2] = checkX;
                        PieceSet[3] = watchZ;
                        PieceSet[4] = watchX;
                        PieceSet[5] = 0;
                        PieceSet[6] = 0;
                    }
                }
                return false;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                {
                    switch (obj_num)
                    {
                        //Queen
                        case mField01 + 1:
                        case mField02 + 1:
                            AiPoint -= MyQueen_P;
                            break;
                        //Bishop
                        case mField01 + 2:
                        case mField02 + 2:
                            AiPoint -= MyBishop_P;
                            break;
                        //Rook
                        case mField01 + 4:
                        case mField02 + 4:
                            AiPoint -= MyRook_P;
                            break;
                    }
                }

                if (PieceSet[0] < AiPoint)
                {
                    PieceSet[0] = AiPoint;
                    PieceSet[1] = checkZ;
                    PieceSet[2] = checkX;
                    PieceSet[3] = watchZ;
                    PieceSet[4] = watchX;
                    PieceSet[5] = 0;
                    PieceSet[6] = 0;
                }
                else if (PieceSet[0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        PieceSet[1] = checkZ;
                        PieceSet[2] = checkX;
                        PieceSet[3] = watchZ;
                        PieceSet[4] = watchX;
                        PieceSet[5] = 0;
                        PieceSet[6] = 0;
                    }
                }
                return true;
            }
        }
        return false;
    }

    //---------------------------------------------------------------------------------------

    //Knight移動範囲サーチ---------------------------------------------------------------------------
    public void KnightMoveSearch(int checkZ, int checkX)
    {
        //全方位確認
        for (int i = 0; i < Mass_Size; i++)
        {
            KnightMoveCheck(checkZ, checkX, knight_num[i, 0], knight_num[i, 1]);
        }
    }

    private void KnightMoveCheck(int checkZ, int checkX, int nextZ, int nextX)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;
        int AiPoint = 0;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //現在のマスが敵の駒の範囲に入っている場合
            if (CheckScript.Check_flag[checkZ, checkX] != 0)
                AiPoint += MyKnight_P;

            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                if (RoomController.getH_Ai_Playerflag())
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField02:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField02 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += Pawn_P;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                else
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField01:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField01 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += Pawn_P;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyKnight_P;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyKnight_P;
            }
            if (PieceSet[0] < AiPoint)
            {
                PieceSet[0] = AiPoint;
                PieceSet[1] = checkZ;
                PieceSet[2] = checkX;
                PieceSet[3] = watchZ;
                PieceSet[4] = watchX;
                PieceSet[5] = 0;
                PieceSet[6] = 0;
            }
            else if (PieceSet[0] == AiPoint)
            {
                if (Random.Range(0, 2) == 0)
                {
                    PieceSet[1] = checkZ;
                    PieceSet[2] = checkX;
                    PieceSet[3] = watchZ;
                    PieceSet[4] = watchX;
                    PieceSet[5] = 0;
                    PieceSet[6] = 0;
                }
            }
        }
    }


    //Pawn移動範囲サーチ---------------------------------------------------------------------------
    public void PawnMoveSearch(int checkZ, int checkX, int obj_num)
    {
        //全方位確認
        for (int i = 0; i < pawn_num.GetLength(0); i++)
        {
            PawnMoveCheck(checkZ, checkX, pawn_num[i, 0], pawn_num[i, 1], obj_num);
        }
    }

    private void PawnMoveCheck(int checkZ, int checkX, int nextZ, int nextX, int obj_num)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;
        int AiPoint = 0;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //現在のマスが敵の駒の範囲に入っている場合
            if (CheckScript.Check_flag[checkZ, checkX] != 0)
                AiPoint += MyPawn_P;

            //Player01の場合
            if (RoomController.getH_Ai_Playerflag() && nextZ >= 1)
            {
                //駒がある場合
                if (nextX != 0 && FieldScript.Piece_Num[watchZ, watchX] != 0)
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField02:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField02 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += Pawn_P;
                            break;
                        //自分の駒
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                //駒がない場合
                else if (nextX != 0 && FieldScript.Piece_Num[watchZ, watchX] == 0)
                {
                    AiPoint -= 1000;
                }
                else if (nextZ == 2 && ((FieldScript.Piece_Num[watchZ, watchX] != 0) || (FieldScript.Piece_Num[watchZ - 1, watchX] != 0)) || (checkZ != 1))
                {
                    AiPoint -= 1000;
                }
                else if (nextX == 0 && FieldScript.Piece_Num[watchZ, watchX] != 0)
                {
                    AiPoint -= 1000;
                }
                else if (watchZ == 7)
                {
                    AiPoint += 10;
                }

                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyPawn_P;

                //現在の移動候補と比較
                if (PieceSet[0] < AiPoint)
                {
                    PieceSet[0] = AiPoint;
                    PieceSet[1] = checkZ;
                    PieceSet[2] = checkX;
                    PieceSet[3] = watchZ;
                    PieceSet[4] = watchX;
                    if (watchZ == 7)
                        PieceSet[5] = obj_num;
                    else
                        PieceSet[5] = 0;
                    if (nextZ == 2)
                        PieceSet[6] = 1;
                    else
                        PieceSet[6] = 0;
                }
                else if (PieceSet[0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        PieceSet[1] = checkZ;
                        PieceSet[2] = checkX;
                        PieceSet[3] = watchZ;
                        PieceSet[4] = watchX;
                        if (watchZ == 7)
                            PieceSet[5] = obj_num;
                        else
                            PieceSet[5] = 0;
                        if (nextZ == 2)
                            PieceSet[6] = 1;
                        else
                            PieceSet[6] = 0;
                    }
                }

            }
            //Player02の場合
            else if (!RoomController.getH_Ai_Playerflag() && nextZ == -1)
            {
                //駒がある場合
                if (nextX != 0 && FieldScript.Piece_Num[watchZ, watchX] != 0)
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[watchZ, watchX])
                    {
                        //King
                        case mField01:
                            AiPoint += KiPoint;
                            break;
                        //Queen
                        case mField01 + 1:
                            AiPoint += Queen_P;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += Bishop_P;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += Knight_P;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += Rook_P;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += Pawn_P;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                //駒がない場合
                else if (nextX != 0 && FieldScript.Piece_Num[watchZ, watchX] == 0)
                {
                    AiPoint -= 1000;
                }
                else if (nextZ == -2 && (FieldScript.Piece_Num[watchZ, watchX] != 0) || (FieldScript.Piece_Num[watchZ + 1, watchX] != 0) || (checkZ != 6))
                {
                    AiPoint -= 1000;
                }
                else if (nextX == 0 && FieldScript.Piece_Num[watchZ, watchX] != 0)
                {
                    AiPoint -= 1000;
                }
                else if (watchZ == 0)
                {
                    AiPoint += 10;
                }

                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyPawn_P;

                //現在の移動候補と比較
                if (PieceSet[0] < AiPoint)
                {
                    PieceSet[0] = AiPoint;
                    PieceSet[1] = checkZ;
                    PieceSet[2] = checkX;
                    PieceSet[3] = watchZ;
                    PieceSet[4] = watchX;
                    if (watchZ == 0)
                        PieceSet[5] = obj_num;
                    else
                        PieceSet[5] = 0;
                    if (nextZ == -2)
                        PieceSet[6] = 1;
                    else
                        PieceSet[6] = 0;
                }
                else if (PieceSet[0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        PieceSet[1] = checkZ;
                        PieceSet[2] = checkX;
                        PieceSet[3] = watchZ;
                        PieceSet[4] = watchX;
                        if (watchZ == 0)
                            PieceSet[5] = obj_num;
                        else
                            PieceSet[5] = 0;
                        if (nextZ == -2)
                            PieceSet[6] = 1;
                        else
                            PieceSet[6] = 0;
                    }
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------
}
