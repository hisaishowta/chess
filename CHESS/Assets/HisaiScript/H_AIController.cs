using UnityEngine;

public class H_AIController : MonoBehaviour
{
    //駒をとる場合の点数
    private const int KiPoint = 100;
    private const int QuPoint = 10;
    private const int RoPoint = 7;
    private const int BiPoint = 7;
    private const int KnPoint = 5;
    private const int PaPoint = 2;
    //駒を取られる場合の点数
    private const int MyKiPoint = 100;
    private const int MyQuPoint = 9;
    private const int MyRoPoint = 5;
    private const int MyBiPoint = 5;
    private const int MyKnPoint = 3;
    private const int MyPaPoint = 1;
    //マス目サイズ
    private const int Mass_Size = 8;
    //チェスフィールド値
    private const int mField01 = 100;
    private const int mField02 = 200;
    //検索値
    private const int SearchSize = 10;
    //Pawn移動先
    private int[,] pawn_num = {
            { 1,-1}, { 1, 1 },
            { -1,-1}, { -1, 1 },
            { 1, 0 }, { -1, 0 },
            { 2, 0 }, { -2, 0 } };
    private int[,] Mypawn_num = {
            { 1,-1}, { 1, 1 },
            { -1,-1}, { -1, 1 }};
    //Knight移動先
    private int[,] knight_num = {
            { -2, -1 }, { -2, 1 },
            { -1,-2}, { -1, 2 },
            { 1, -2 }, { 1, 2 },
            { 2, -1 }, { 2, 1 } };

    private bool now_player;    //現在のplayer
    private bool moreFlag;
    public bool H_AiFlag = false;

    private int firstPoint = 0;
    private int num = 0;
    
    private int MyKingZ = 0;
    private int MyKingX = 0;

    //相手の駒の位置格納用
    private int KingZ = -1,KingX = -1;
    private int QueenZ = -1,QueenX = -1;
    private int BishopZ = -1,BishopX = -1,BishopZ2 = -1,BishopX2 = -1;
    private int RookZ = -1,RookX = -1,RookZ2 = -1, RookX2 = -1;
    private int KnightZ = -1,KnightX = -1, KnightZ2 = -1,KnightX2 = -1;

    private int[,] PieceSelect = new int[10, 7];        //移動先候補格納
    private int[] PieceSet = { 0, 0, 0, 0, 0, 0, 0 };   //点数,fromZ,fromX,toZ,toX,obj_num,enid

    public int[,] Piece_Ai_P = new int[Mass_Size, Mass_Size];
    public static int[,] CheckMy_flag = new int[Mass_Size, Mass_Size];

    private GameObject scPlayerController;
    PlayerController playercntl;
    private GameObject scCheckScript;
    CheckScript checkcntl;
    private GameObject scMoveController;
    MoveController movecntl;

    // Use this for initialization
    void Start()
    {
        scPlayerController = GameObject.Find("PlayerController");
        playercntl = scPlayerController.GetComponent<PlayerController>();
        scCheckScript = GameObject.Find("CheckController");
        checkcntl = scCheckScript.GetComponent<CheckScript>();
        scMoveController = GameObject.Find("MoveController");
        movecntl = scMoveController.GetComponent<MoveController>();
    }

    //相手の駒の範囲チェック
    public void CheckArea()
    {
        checkcntl.CopyFlag();                  //Copy_Check_flagリセット
        checkcntl.Reset();                     //Check_flagリセット  
        now_player = playercntl.PlayerCheck(); //現在のplayer確認
        checkcntl.Check_AreaFlag(!now_player); //相手攻撃範囲の確認
        //移動先･フラグ初期化
        for (int i = 0; i < PieceSelect.GetLength(0); i++)
        {
            for (int j = 0; j < PieceSelect.GetLength(1); j++)
            {
                PieceSelect[i, j] = -100;
            }
        }
        for (int i = 0; i < PieceSet.Length; i++) { PieceSet[i] = -1; }

        //相手の駒の位置情報初期化
        KingZ = KingX = QueenZ = QueenX = BishopZ = BishopX = BishopZ2 = BishopX2 = -1;
        RookZ = RookX = RookZ2 = RookX2 = KnightZ = KnightX = KnightZ2 = KnightX2 = -1;

        //自分のKing,相手のKing,Queenの位置確認
        for (int z = 0; z < Mass_Size; z++)
        {
            for (int x = 0; x < Mass_Size; x++)
            {
                //Player2
                if (!RoomController.getH_Ai_Playerflag())
                {
                    //King,Queen,Bishop,Knight,Rook,Pawn
                    switch (FieldScript.Piece_Num[z, x])
                    {
                        //自分のKing
                        case mField02:
                            MyKingZ = z; MyKingX = x;
                            break;
                        //King
                        case mField01:
                            KingZ = z; KingX = x;
                            break;
                        //Queen
                        case mField01 + 1:
                            QueenZ = z; QueenX = x;
                            break;
                        //Bishop
                        case mField01 + 2:
                            //二体分別用IF文
                            if (BishopZ == -1) { BishopZ = z; BishopX = x; }
                            else { BishopZ2 = z; BishopX2 = x; }
                            break;
                        //Rook
                        case mField01 + 4:
                            //二体分別用IF文
                            if (RookZ == -1) { RookZ = z; RookX = x; }
                            else { RookZ2 = z; RookX2 = x; }
                            break;
                        //Knight
                        case mField01 + 3:
                            //二体分別用IF文
                            if (KnightZ == -1) { KnightZ = z; KnightX = x; }
                            else { KnightZ2 = z; KnightX2 = x; }
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
                        //自分のKing
                        case mField01:
                            MyKingZ = z; MyKingX = x;
                            break;
                        //King
                        case mField02:
                            KingZ = z; KingX = x;
                            break;
                        //Queen
                        case mField02 + 1:
                            QueenZ = z; QueenX = x;
                            break;
                        //Bishop
                        case mField02 + 2:
                            //二体分別用IF文
                            if (BishopZ == -1) { BishopZ = z; BishopX = x; }
                            else { BishopZ2 = z; BishopX2 = x; }
                            break;
                        //Rook
                        case mField02 + 4:
                            //二体分別用IF文
                            if (RookZ == -1) { RookZ = z; RookX = x; }
                            else { RookZ2 = z; RookX2 = x; }
                            break;
                        //Knight
                        case mField02 + 3:
                            //二体分別用IF文
                            if (KnightZ == -1) { KnightZ = z; KnightX = x; }
                            else { KnightZ2 = z; KnightX2 = x; }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
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
                            KnightMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
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
                            KnightMoveSearch(z, x, FieldScript.Piece_Num[z, x]);
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
        //検索先が同じならカウントアップ
        int count = 0;
        for (int j = 1; j < SearchSize; j++)
        {
            if (PieceSelect[0, 0] == PieceSelect[j, 0]) {count++; }
        }
        //参照先が異なる場合
        if (count != 0)
        {
            int random = Random.Range(0, count + 1);
            for (int i = 1; i < 7; i++) { PieceSet[i] = PieceSelect[random, i]; }
        }
        //同じ参照先の場合
        else
        {
            for (int i = 1; i < 7; i++) { PieceSet[i] = PieceSelect[0, i]; }
        }

        //promotionの場合
        if (PieceSet[5] != 0)
        {
            movecntl.E_PlayerSetTarget(PieceSet[1], PieceSet[2], PieceSet[3], PieceSet[4], 0, 0, 1);
        }
        //enpasantの場合
        else if (PieceSet[6] != 0)
        {
            movecntl.E_PlayerSetTarget(PieceSet[1], PieceSet[2], PieceSet[3], PieceSet[4], 0, 1, 0);
        }
        else {
            movecntl.E_PlayerSetTarget(PieceSet[1], PieceSet[2], PieceSet[3], PieceSet[4], 0, 0, 0);
        }
        firstPoint++;
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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += PaPoint;
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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += PaPoint;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                { AiPoint -= MyKiPoint; }
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                { AiPoint -= MyKiPoint; }
            }

            if (PieceSelect[0, 0] < AiPoint)
            {
                PieceSelect[0, 0] = AiPoint;
                PieceSelect[0, 1] = checkZ;
                PieceSelect[0, 2] = checkX;
                PieceSelect[0, 3] = watchZ;
                PieceSelect[0, 4] = watchX;
                PieceSelect[0, 5] = 0;
                PieceSelect[0, 6] = 0;
                num = 1;
            }
            else if (PieceSelect[0, 0] == AiPoint)
            {
                if (Random.Range(0, 2) == 0 || PieceSelect[0, 0] != PieceSelect[SearchSize - 1, 0])
                {
                    int setnum = num % SearchSize;
                    PieceSelect[setnum, 0] = AiPoint;
                    PieceSelect[setnum, 1] = checkZ;
                    PieceSelect[setnum, 2] = checkX;
                    PieceSelect[setnum, 3] = watchZ;
                    PieceSelect[setnum, 4] = watchX;
                    PieceSelect[setnum, 5] = 0;
                    PieceSelect[setnum, 6] = 0;
                    if (PieceSelect[0, 0] == PieceSelect[SearchSize - 1, 0])
                        num = Random.Range(0, SearchSize);
                    else
                        num += 1;
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

            //移動後の自分の攻撃範囲検索
            int fromobj;
            fromobj = FieldScript.Piece_Num[watchZ, watchX];
            FieldScript.Piece_Num[watchZ, watchX] = obj_num;
            FieldScript.Piece_Num[checkZ, checkX] = 0;
            CheckReset();
            CheckMy_AreaFlag(now_player);
            FieldScript.Piece_Num[checkZ, checkX] = obj_num;
            FieldScript.Piece_Num[watchZ, watchX] = fromobj;
            //移動後の自分の攻撃範囲でチェックできるならプラス
            if (KingZ != -1 && CheckMy_flag[KingZ, KingX] >= 1)
                AiPoint += 2;
            //移動後の自分の攻撃範囲にQueenがいるならプラス
            if (QueenZ != -1 && CheckMy_flag[QueenZ, QueenX] >= 1)
                AiPoint += 1;
            //移動後の自分の攻撃範囲にBishopがいるならプラス
            if (BishopZ != -1)
            {
                if (CheckMy_flag[BishopZ, BishopX] >= 1)
                    AiPoint += 1;
                if (BishopZ2 != -1)
                {
                    if (CheckMy_flag[BishopZ2, BishopX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分の攻撃範囲にRookがいるならプラス
            if (RookZ != -1)
            {
                if (CheckMy_flag[RookZ, RookX] >= 1)
                    AiPoint += 1;
                if (RookZ2 != -1)
                {
                    if (CheckMy_flag[RookZ2, RookX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分の攻撃範囲にKnightがいるならプラス
            if (KnightZ != -1)
            {
                if (CheckMy_flag[KnightZ, KnightX] >= 1)
                    AiPoint += 1;
                if (KnightZ2 != -1)
                {
                    if (CheckMy_flag[KnightZ2, KnightX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分のマスが別の自分の駒の攻撃範囲になっていたらプラス
            if (CheckMy_flag[watchZ, watchX] >= 1)
                AiPoint += 2;

            //移動後の相手の攻撃範囲検索
            fromobj = FieldScript.Piece_Num[watchZ, watchX];
            FieldScript.Piece_Num[watchZ, watchX] = obj_num;
            FieldScript.Piece_Num[checkZ, checkX] = 0;
            CheckReset();
            CheckMy_AreaFlag(!now_player);
            FieldScript.Piece_Num[checkZ, checkX] = obj_num;
            FieldScript.Piece_Num[watchZ, watchX] = fromobj;
            if (CheckMy_flag[MyKingZ, MyKingX] >= 1)
                AiPoint -= 200;

            //現在のマスが敵の駒の範囲に入っている場合
            if (CheckScript.Check_flag[checkZ, checkX] != 0)
            {
                switch (obj_num)
                {
                    //Queen
                    case mField01 + 1:
                    case mField02 + 1:
                        AiPoint += MyQuPoint;
                        break;
                    //Bishop
                    case mField01 + 2:
                    case mField02 + 2:
                        AiPoint += MyBiPoint;
                        break;
                    //Rook
                    case mField01 + 4:
                    case mField02 + 4:
                        AiPoint += MyRoPoint;
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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += PaPoint;
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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += PaPoint;
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
                            AiPoint -= MyQuPoint;
                            break;
                        //Bishop
                        case mField01 + 2:
                        case mField02 + 2:
                            AiPoint -= MyBiPoint;
                            break;
                        //Rook
                        case mField01 + 4:
                        case mField02 + 4:
                            AiPoint -= MyRoPoint;
                            break;
                    }
                }

                if (PieceSelect[0, 0] < AiPoint)
                {
                    PieceSelect[0, 0] = AiPoint;
                    PieceSelect[0, 1] = checkZ;
                    PieceSelect[0, 2] = checkX;
                    PieceSelect[0, 3] = watchZ;
                    PieceSelect[0, 4] = watchX;
                    PieceSelect[0, 5] = 0;
                    PieceSelect[0, 6] = 0;
                    num = 1;
                }
                else if (PieceSelect[0, 0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0 || PieceSelect[0, 0] != PieceSelect[SearchSize - 1, 0])
                    {
                        int setnum = num % SearchSize;
                        PieceSelect[setnum, 0] = AiPoint;
                        PieceSelect[setnum, 1] = checkZ;
                        PieceSelect[setnum, 2] = checkX;
                        PieceSelect[setnum, 3] = watchZ;
                        PieceSelect[setnum, 4] = watchX;
                        PieceSelect[setnum, 5] = 0;
                        PieceSelect[setnum, 6] = 0;
                        if (PieceSelect[0, 0] == PieceSelect[SearchSize - 1, 0])
                            num = Random.Range(0, SearchSize);
                        else
                            num += 1;
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
                            AiPoint -= MyQuPoint;
                            break;
                        //Bishop
                        case mField01 + 2:
                        case mField02 + 2:
                            AiPoint -= MyBiPoint;
                            break;
                        //Rook
                        case mField01 + 4:
                        case mField02 + 4:
                            AiPoint -= MyRoPoint;
                            break;
                    }
                }

                if (PieceSelect[0, 0] < AiPoint)
                {
                    PieceSelect[0, 0] = AiPoint;
                    PieceSelect[0, 1] = checkZ;
                    PieceSelect[0, 2] = checkX;
                    PieceSelect[0, 3] = watchZ;
                    PieceSelect[0, 4] = watchX;
                    PieceSelect[0, 5] = 0;
                    PieceSelect[0, 6] = 0;
                    num = 1;
                }
                else if (PieceSelect[0, 0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0 || PieceSelect[0, 0] != PieceSelect[SearchSize - 1, 0])
                    {
                        int setnum = num % SearchSize;
                        PieceSelect[setnum, 0] = AiPoint;
                        PieceSelect[setnum, 1] = checkZ;
                        PieceSelect[setnum, 2] = checkX;
                        PieceSelect[setnum, 3] = watchZ;
                        PieceSelect[setnum, 4] = watchX;
                        PieceSelect[setnum, 5] = 0;
                        PieceSelect[setnum, 6] = 0;
                        if (PieceSelect[0, 0] == PieceSelect[SearchSize - 1, 0])
                            num = Random.Range(0, SearchSize);
                        else
                            num += 1;
                    }
                }
                return true;
            }
        }
        return false;
    }

    //---------------------------------------------------------------------------------------

    //Knight移動範囲サーチ---------------------------------------------------------------------------
    public void KnightMoveSearch(int checkZ, int checkX, int obj_num)
    {
        //全方位確認
        for (int i = 0; i < Mass_Size; i++)
        {
            KnightMoveCheck(checkZ, checkX, knight_num[i, 0], knight_num[i, 1], obj_num);
        }
    }

    private void KnightMoveCheck(int checkZ, int checkX, int nextZ, int nextX, int obj_num)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;
        int AiPoint = 0;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //移動後の自分の攻撃範囲検索
            int fromobj;
            fromobj = FieldScript.Piece_Num[watchZ, watchX];
            FieldScript.Piece_Num[watchZ, watchX] = obj_num;
            FieldScript.Piece_Num[checkZ, checkX] = 0;
            bool flag = playercntl.PlayerCheck();
            CheckReset();
            CheckMy_AreaFlag(flag);
            FieldScript.Piece_Num[checkZ, checkX] = obj_num;
            FieldScript.Piece_Num[watchZ, watchX] = fromobj;
            //移動後の自分の攻撃範囲でチェックできるならプラス
            if (KingZ != -1 && CheckMy_flag[KingZ, KingX] >= 1)
                AiPoint += 2;
            //移動後の自分の攻撃範囲にクイーンがいるならプラス
            if (QueenZ != -1 && CheckMy_flag[QueenZ, QueenX] >= 1)
                AiPoint += 1;
            //移動後の自分の攻撃範囲にBishopがいるならプラス
            if (BishopZ != -1)
            {
                if (CheckMy_flag[BishopZ, BishopX] >= 1)
                    AiPoint += 1;
                if (BishopZ2 != -1)
                {
                    if (CheckMy_flag[BishopZ2, BishopX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分の攻撃範囲にRookがいるならプラス
            if (RookZ != -1)
            {
                if (CheckMy_flag[RookZ, RookX] >= 1)
                    AiPoint += 1;
                if (RookZ2 != -1)
                {
                    if (CheckMy_flag[RookZ2, RookX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分の攻撃範囲にKnightがいるならプラス
            if (KnightZ != -1)
            {
                if (CheckMy_flag[KnightZ, KnightX] >= 1)
                    AiPoint += 1;
                if (KnightZ2 != -1)
                {
                    if (CheckMy_flag[KnightZ2, KnightX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分のマスが別の自分の駒の攻撃範囲になっていたらプラス
            if (CheckMy_flag[watchZ, watchX] >= 1)
                AiPoint += 2;

            //移動後の相手の攻撃範囲検索
            fromobj = FieldScript.Piece_Num[watchZ, watchX];
            FieldScript.Piece_Num[watchZ, watchX] = obj_num;
            FieldScript.Piece_Num[checkZ, checkX] = 0;
            CheckReset();
            CheckMy_AreaFlag(!now_player);
            FieldScript.Piece_Num[checkZ, checkX] = obj_num;
            FieldScript.Piece_Num[watchZ, watchX] = fromobj;
            if (CheckMy_flag[MyKingZ, MyKingX] >= 1)
                AiPoint -= 200;

            //現在のマスが敵の駒の範囲に入っている場合
            if (CheckScript.Check_flag[checkZ, checkX] != 0)
                AiPoint += MyKnPoint;

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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += PaPoint;
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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += PaPoint;
                            break;
                        default:
                            AiPoint -= 1000;
                            break;
                    }
                }
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyKnPoint;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //移動後のマスが敵の駒の範囲に入っている場合
                if (CheckScript.Check_flag[watchZ, watchX] != 0)
                    AiPoint -= MyKnPoint;
            }
            if (PieceSelect[0, 0] < AiPoint)
            {
                PieceSelect[0, 0] = AiPoint;
                PieceSelect[0, 1] = checkZ;
                PieceSelect[0, 2] = checkX;
                PieceSelect[0, 3] = watchZ;
                PieceSelect[0, 4] = watchX;
                PieceSelect[0, 5] = 0;
                PieceSelect[0, 6] = 0;
                num = 1;
            }
            else if (PieceSelect[0, 0] == AiPoint)
            {
                if (Random.Range(0, 2) == 0 || PieceSelect[0, 0] != PieceSelect[SearchSize - 1, 0])
                {
                    int setnum = num % SearchSize;
                    PieceSelect[setnum, 0] = AiPoint;
                    PieceSelect[setnum, 1] = checkZ;
                    PieceSelect[setnum, 2] = checkX;
                    PieceSelect[setnum, 3] = watchZ;
                    PieceSelect[setnum, 4] = watchX;
                    PieceSelect[setnum, 5] = 0;
                    PieceSelect[setnum, 6] = 0;
                    if (PieceSelect[0, 0] == PieceSelect[SearchSize - 1, 0])
                        num = Random.Range(0, SearchSize);
                    else
                        num += 1;
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
            //最初の数ターンはポーンの点数を高くする
            if (firstPoint <= 3)
                AiPoint += 3;

            //移動後の自分の攻撃範囲検索
            int fromobj;
            fromobj = FieldScript.Piece_Num[watchZ, watchX];
            FieldScript.Piece_Num[watchZ, watchX] = obj_num;
            FieldScript.Piece_Num[checkZ, checkX] = 0;
            bool flag = playercntl.PlayerCheck();
            CheckReset();
            CheckMy_AreaFlag(flag);
            FieldScript.Piece_Num[checkZ, checkX] = obj_num;
            FieldScript.Piece_Num[watchZ, watchX] = fromobj;
            //移動後の自分の攻撃範囲でチェックできるならプラス
            if (KingZ != -1 && CheckMy_flag[KingZ, KingX] >= 1)
                AiPoint += 2;
            //移動後の自分の攻撃範囲にクイーンがいるならプラス
            if (QueenZ != -1 && CheckMy_flag[QueenZ, QueenX] >= 1)
                AiPoint += 1;
            //移動後の自分の攻撃範囲にBishopがいるならプラス
            if (BishopZ != -1)
            {
                if (CheckMy_flag[BishopZ, BishopX] >= 1)
                    AiPoint += 1;
                if (BishopZ2 != -1)
                {
                    if (CheckMy_flag[BishopZ2, BishopX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分の攻撃範囲にRookがいるならプラス
            if (RookZ != -1)
            {
                if (CheckMy_flag[RookZ, RookX] >= 1)
                    AiPoint += 1;
                if (RookZ2 != -1)
                {
                    if (CheckMy_flag[RookZ2, RookX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分の攻撃範囲にKnightがいるならプラス
            if (KnightZ != -1)
            {
                if (CheckMy_flag[KnightZ, KnightX] >= 1)
                    AiPoint += 1;
                if (KnightZ2 != -1)
                {
                    if (CheckMy_flag[KnightZ2, KnightX2] >= 1)
                        AiPoint += 1;
                }
            }
            //移動後の自分のマスが別の自分の駒の攻撃範囲になっていたらプラス
            if (CheckMy_flag[watchZ, watchX] >= 1 && firstPoint > 3)
                AiPoint += 2;

            //移動後の相手の攻撃範囲検索
            fromobj = FieldScript.Piece_Num[watchZ, watchX];
            FieldScript.Piece_Num[watchZ, watchX] = obj_num;
            FieldScript.Piece_Num[checkZ, checkX] = 0;
            CheckReset();
            CheckMy_AreaFlag(!now_player);
            FieldScript.Piece_Num[checkZ, checkX] = obj_num;
            FieldScript.Piece_Num[watchZ, watchX] = fromobj;
            if (CheckMy_flag[MyKingZ, MyKingX] >= 1)
                AiPoint -= 200;

            //現在のマスが敵の駒の範囲に入っている場合
            if (CheckScript.Check_flag[checkZ, checkX] != 0)
                AiPoint += MyPaPoint;

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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField02 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField02 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField02 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField02 + 5:
                            AiPoint += PaPoint;
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
                    AiPoint -= MyPaPoint;

                //現在の移動候補と比較
                if (PieceSelect[0, 0] < AiPoint)
                {
                    PieceSelect[0, 0] = AiPoint;
                    PieceSelect[0, 1] = checkZ;
                    PieceSelect[0, 2] = checkX;
                    PieceSelect[0, 3] = watchZ;
                    PieceSelect[0, 4] = watchX;
                    if (watchZ == 7)
                        PieceSelect[0, 5] = obj_num;
                    else
                        PieceSelect[0, 5] = 0;
                    if (nextZ == 2)
                        PieceSelect[0, 6] = 1;
                    else
                        PieceSelect[0, 6] = 0;
                    num = 1;
                }
                else if (PieceSelect[0, 0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0 || PieceSelect[0, 0] != PieceSelect[SearchSize - 1, 0])
                    {
                        int setnum = num % SearchSize;
                        PieceSelect[setnum, 0] = AiPoint;
                        PieceSelect[setnum, 1] = checkZ;
                        PieceSelect[setnum, 2] = checkX;
                        PieceSelect[setnum, 3] = watchZ;
                        PieceSelect[setnum, 4] = watchX;
                        if (watchZ == 7)
                            PieceSelect[setnum, 5] = obj_num;
                        else
                            PieceSelect[setnum, 5] = 0;
                        if (nextZ == 2)
                            PieceSelect[setnum, 6] = 1;
                        else
                            PieceSelect[setnum, 6] = 0;
                        if (PieceSelect[0, 0] == PieceSelect[SearchSize - 1, 0])
                            num = Random.Range(0, SearchSize);
                        else
                            num += 1;
                    }
                }

            }
            //Player02の場合
            else if (!RoomController.getH_Ai_Playerflag() && nextZ <= -1)
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
                            AiPoint += QuPoint;
                            break;
                        //Bishop
                        case mField01 + 2:
                            AiPoint += BiPoint;
                            break;
                        //Knight
                        case mField01 + 3:
                            AiPoint += KnPoint;
                            break;
                        //Rook
                        case mField01 + 4:
                            AiPoint += RoPoint;
                            break;
                        //Pawn
                        case mField01 + 5:
                            AiPoint += PaPoint;
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
                else if (nextZ == -2 && ((FieldScript.Piece_Num[watchZ, watchX] != 0) || (FieldScript.Piece_Num[watchZ + 1, watchX] != 0)) || (checkZ != 6))
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
                    AiPoint -= MyPaPoint;

                //現在の移動候補と比較
                if (PieceSelect[0, 0] < AiPoint)
                {
                    PieceSelect[0, 0] = AiPoint;
                    PieceSelect[0, 1] = checkZ;
                    PieceSelect[0, 2] = checkX;
                    PieceSelect[0, 3] = watchZ;
                    PieceSelect[0, 4] = watchX;
                    if (watchZ == 0)
                        PieceSelect[0, 5] = obj_num;
                    else
                        PieceSelect[0, 5] = 0;
                    if (nextZ == -2)
                        PieceSelect[0, 6] = 1;
                    else
                        PieceSelect[0, 6] = 0;
                    num = 1;
                }
                else if (PieceSelect[0, 0] == AiPoint)
                {
                    if (Random.Range(0, 2) == 0 || PieceSelect[0, 0] != PieceSelect[SearchSize - 1, 0])
                    {
                        int setnum = num % SearchSize;
                        PieceSelect[setnum, 0] = AiPoint;
                        PieceSelect[setnum, 1] = checkZ;
                        PieceSelect[setnum, 2] = checkX;
                        PieceSelect[setnum, 3] = watchZ;
                        PieceSelect[setnum, 4] = watchX;
                        if (watchZ == 0)
                            PieceSelect[setnum, 5] = obj_num;
                        else
                            PieceSelect[setnum, 5] = 0;
                        if (nextZ == -2)
                            PieceSelect[setnum, 6] = 1;
                        else
                            PieceSelect[setnum, 6] = 0;
                        if (PieceSelect[0, 0] == PieceSelect[SearchSize - 1, 0])
                            num = Random.Range(0, SearchSize);
                        else
                            num += 1;
                    }
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------



    //以下自分の駒&&相手の駒の範囲取得関係-----------------------------------------------------------------------------------------
    public void CheckMy_AreaFlag(bool player)
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
                            K_MoveSearch(z, x);
                            break;
                        //Queen
                        case mField01 + 1:
                            Q_MoveSearch(z, x);
                            break;
                        //Bishop
                        case mField01 + 2:
                            B_MoveSearch(z, x);
                            break;
                        //Knight
                        case mField01 + 3:
                            Kn_MoveSearch(z, x);
                            break;
                        //Rook
                        case mField01 + 4:
                            R_MoveSearch(z, x);
                            break;
                        //Pawn
                        case mField01 + 5:
                            P_MoveSearch(z, x, FieldScript.Piece_Num[z, x]);
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
                            K_MoveSearch(z, x);
                            break;
                        //Queen
                        case mField02 + 1:
                            Q_MoveSearch(z, x);
                            break;
                        //Bishop
                        case mField02 + 2:
                            B_MoveSearch(z, x);
                            break;
                        //Knight
                        case mField02 + 3:
                            Kn_MoveSearch(z, x);
                            break;
                        //Rook
                        case mField02 + 4:
                            R_MoveSearch(z, x);
                            break;
                        //Pawn
                        case mField02 + 5:
                            P_MoveSearch(z, x, FieldScript.Piece_Num[z, x]);
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

    public void CheckReset()
    {
        // 8x8の配列をfalseで初期化
        for (int i = 0; i < CheckMy_flag.GetLength(0); i++)
        {
            for (int j = 0; j < CheckMy_flag.GetLength(1); j++)
            {
                CheckMy_flag[i, j] = 0;
            }
        }
    }

    //移動範囲サーチ
    public void K_MoveSearch(int checkZ, int checkX)
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
                        K_MoveCheck(checkZ, checkX, i, j);
                    }
                }
            }
        }
    }

    private void K_MoveCheck(int checkZ, int checkX, int nextZ, int nextX)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                CheckMy_flag[watchZ, watchX]++;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                CheckMy_flag[watchZ, watchX]++;
            }
        }
    }

    //移動範囲サーチ
    public void Q_MoveSearch(int checkZ, int checkX)
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
                        while (Q_MoveCheck(checkZ, checkX, sumi, sumj))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
    }

    private bool Q_MoveCheck(int checkZ, int checkX, int nextZ, int nextX)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                //敵味方共にフラグ
                if (!moreFlag)
                {
                    CheckMy_flag[watchZ, watchX]++;
                }
                return false;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //敵味方共にフラグ
                if (!moreFlag)
                {
                    CheckMy_flag[watchZ, watchX]++;
                }
                return true;
            }
        }
        return false;
    }


    //移動範囲サーチ
    public void B_MoveSearch(int checkZ, int checkX)
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
                        while (B_MoveCheck(checkZ, checkX, sumi, sumj))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
    }

    private bool B_MoveCheck(int checkZ, int checkX, int nextZ, int nextX)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                //敵味方共にフラグ
                if (!moreFlag)
                {
                    CheckMy_flag[watchZ, watchX]++;
                }
                return false;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //敵味方共にフラグ
                if (!moreFlag)
                {
                    CheckMy_flag[watchZ, watchX]++;
                }
                return true;
            }
        }
        return false;
    }

    //移動範囲サーチ
    public void R_MoveSearch(int checkZ, int checkX)
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
                        while (R_MoveCheck(checkZ, checkX, sumi, sumj))
                        {
                            sumi = sumi + i;
                            sumj = sumj + j;
                        }
                    }
                }
            }
        }
    }

    private bool R_MoveCheck(int checkZ, int checkX, int nextZ, int nextX)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                //敵味方共にフラグ
                if (!moreFlag)
                {
                    CheckMy_flag[watchZ, watchX]++;
                }
                return false;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                //敵味方共にフラグ
                if (!moreFlag)
                {
                    CheckMy_flag[watchZ, watchX]++;
                }
                return true;
            }
        }
        return false;
    }


    //移動範囲サーチ
    public void Kn_MoveSearch(int checkZ, int checkX)
    {
        //全方位確認
        for (int i = 0; i < Mass_Size; i++)
        {
            Kn_MoveCheck(checkZ, checkX, knight_num[i, 0], knight_num[i, 1]);
        }
    }

    private void Kn_MoveCheck(int checkZ, int checkX, int nextZ, int nextX)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //駒がある場合
            if (FieldScript.Piece_Num[watchZ, watchX] != 0)
            {
                CheckMy_flag[watchZ, watchX]++;
            }
            //駒がない場合
            else if (FieldScript.Piece_Num[watchZ, watchX] == 0)
            {
                CheckMy_flag[watchZ, watchX]++;
            }
        }
    }

    //移動範囲サーチ
    public void P_MoveSearch(int checkZ, int checkX, int obj_num)
    {
        //全方位確認
        for (int i = 0; i < Mypawn_num.GetLength(0); i++)
        {
            P_MoveCheck(checkZ, checkX, Mypawn_num[i, 0], Mypawn_num[i, 1], obj_num);
        }
    }

    private void P_MoveCheck(int checkZ, int checkX, int nextZ, int nextX, int obj_num)
    {
        //参照先配列値Z,X
        int watchZ = checkZ + nextZ;
        int watchX = checkX + nextX;

        //配列外除外
        if (watchZ < Mass_Size && watchZ > -1 &&
            watchX < Mass_Size && watchX > -1)
        {
            //Player01の場合
            if (obj_num < mField02 && nextZ == 1)
            {
                CheckMy_flag[watchZ, watchX]++;
            }
            //Player02の場合
            else if (obj_num > mField02 && nextZ == -1)
            {
                CheckMy_flag[watchZ, watchX]++;
            }
        }
    }

}
