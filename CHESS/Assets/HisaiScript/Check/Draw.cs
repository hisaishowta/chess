using System.IO;
using System.Text;
using UnityEngine;

public class Draw : MonoBehaviour
{
    /// <summary> マス目サイズ </summary> 
    private const int Mass_Size = 8;
    //チェスフィールド値
    private const int mField01 = 100;
    private const int mField02 = 200;

    private GameObject Playercntl;
    PlayerController player;

    //各駒のスクリプト利用
    private GameObject gChess;
    CheckKing kcntl;
    CheckQueen qcntl;
    CheckBishop bcntl;
    CheckKnight kncntl;
    CheckRook rcntl;
    CheckPawn pcntl;

    //textファイル場所
    static private string stCurDir;
    public string wtxt = "";

    // Use this for initialization
    void Start()
    {
        Playercntl = GameObject.Find("PlayerController");
        player = Playercntl.GetComponent<PlayerController>();

        gChess = GameObject.Find("CheckController");
        kcntl = gChess.GetComponent<CheckKing>();
        qcntl = gChess.GetComponent<CheckQueen>();
        bcntl = gChess.GetComponent<CheckBishop>();
        kncntl = gChess.GetComponent<CheckKnight>();
        rcntl = gChess.GetComponent<CheckRook>();
        pcntl = gChess.GetComponent<CheckPawn>();

        fileRemake();
    }

    /// <summary>チェックされていない状態で動けない場合</summary>
    /// <returns>動けないのであればFALSE,動ける駒があればTRUE</returns>
    public bool Draw_check()
    {
        //動ける時true
        bool flag = false;
        bool b_turn = player.PlayerCheck();

        for (int i = 0; i < Mass_Size; ++i)
        {
            for (int j = 0; j < Mass_Size; ++j)
            {
                if (b_turn)
                {
                    switch (FieldScript.Piece_Num[i, j])
                    {
                        //移動できない駒の場合falseを返す
                        case mField01:
                            if (kcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField01 + 1:
                            if (qcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; };
                            break;
                        case mField01 + 2:
                            if (bcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField01 + 3:
                            if (kncntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField01 + 4:
                            if (rcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField01 + 5:
                            if (pcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        default:
                            break;
                    }
                    if (flag) { break; }
                }
                else {
                    switch (FieldScript.Piece_Num[i, j])
                    {
                        //移動できない駒の場合falseを返す
                        case mField02:
                            if (kcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField02 + 1:
                            if (qcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; };
                            break;
                        case mField02 + 2:
                            if (bcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField02 + 3:
                            if (kncntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField02 + 4:
                            if (rcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        case mField02 + 5:
                            if (pcntl.Move(i, j, FieldScript.Piece_Num[i, j]))
                            { flag = true; }
                            break;
                        default:
                            break;
                    }
                    if (flag) { break; }
                }
            }
            if (flag) { break; }
        }
        return flag;
    }

    /// <summary>50手ルール判定用</summary>
    /// <returns>両者50超えていればFALSE,なければTRUE</returns>
    public bool Draw_rule50()
    {
        //自身50+相手50が合計して超えているかどうか
        Debug.Log(MoveController.rule50);
        if (MoveController.rule50 > 99) { return false; }
        return true;
    }

    /// <summary>どちらかがkingのみの状態で必要な駒</summary>
    /// <returns>チェックできる駒があればTRUE,なければFALSE</returns>
    public bool Draw_needchess()
    {
        //プレイヤーの確認
        bool b_turn = player.PlayerCheck();

        //駒確認(King,Queen,Bishop,Knight,Rook,Pawn順)
        int[] p1chess = new int[] { 0, 0, 0, 0, 0, 0 };
        int[] p2chess = new int[] { 0, 0, 0, 0, 0, 0 };
        //チェックメイトに必要な駒の確認用
        int[,] cneed = new int[,] {
            { 1, 1, 0, 0, 0, 0 },   //キング＋クイーン
            { 1, 0, 2, 0, 0, 0 },   //キング＋ビショップ＋ビショップ
            { 1, 0, 1, 1, 0, 0 },   //キング＋ビショップ＋ナイト
            { 1, 0, 0, 0, 1, 0 },   //キング＋ルーク
            { 1, 0, 0, 0, 0, 1 }    //キング＋ポーン
        };
        //駒の数確認用
        int p1count = 0, p2count = 0;

        //-------------------------------------------
        //持ち駒確認
        for (int i = 0; i < Mass_Size; ++i)
        {
            for (int j = 0; j < Mass_Size; ++j)
            {
                //-------------------------------------------
                switch (FieldScript.Piece_Num[i, j])
                {
                    //相手の駒確認 P1確認
                    case mField01:
                        p1chess[0]++; break;
                    case mField01 + 1:
                        p1chess[1]++; break;
                    case mField01 + 2:
                        p1chess[2]++; break;
                    case mField01 + 3:
                        p1chess[3]++; break;
                    case mField01 + 4:
                        p1chess[4]++; break;
                    case mField01 + 5:
                        p1chess[5]++; break;
                    //相手の駒確認 P2確認
                    case mField02:
                        p2chess[0]++; break;
                    case mField02 + 1:
                        p2chess[1]++; break;
                    case mField02 + 2:
                        p2chess[2]++; break;
                    case mField02 + 3:
                        p2chess[3]++; break;
                    case mField02 + 4:
                        p2chess[4]++; break;
                    case mField02 + 5:
                        p2chess[5]++; break;
                    default:
                        break;
                }
                //-------------------------------------------
            }
        }
        //-------------------------------------------

        //---------------------
        //配列可視化参照
        string print_array = "";
        for (int i = 0; i < p1chess.Length; i++)
        {
            print_array += p1chess[i].ToString("D1") + ":";
        }
        Debug.Log(print_array);
        //---------------------
        print_array = "";
        for (int i = 0; i < p2chess.Length; i++)
        {
            print_array += p2chess[i].ToString("D1") + ":";
        }
        Debug.Log(print_array);
        //---------------------

        for (int i = 0; i < p1chess.Length; i++)
        {
            if (p1chess[i] != 0) { p1count++; }
            if (p2chess[i] != 0) { p2count++; }
        }
        bool flag = true;
        //p1の場合
        if (b_turn)
        {
            if (p2count == 1)
            {
                //必要際定数の駒の確認
                for (int i = 0; i < cneed.GetLength(0); i++)
                {
                    if (p1chess[0] >= cneed[i, 0] &&
                        p1chess[1] >= cneed[i, 1] &&
                        p1chess[2] >= cneed[i, 2] &&
                        p1chess[3] >= cneed[i, 3] &&
                        p1chess[4] >= cneed[i, 4] &&
                        p1chess[5] >= cneed[i, 5])
                    {
                        return true;
                    }
                    else
                    {
                        flag= false;
                    }
                }
                return flag;
            }
        }
        //p2の場合
        else
        {
            if (p1count == 1)
            {
                //必要際定数の駒の確認
                for (int i = 0; i < cneed.GetLength(0); i++)
                {
                    if (p2chess[0] >= cneed[i, 0] &&
                        p2chess[1] >= cneed[i, 1] &&
                        p2chess[2] >= cneed[i, 2] &&
                        p2chess[3] >= cneed[i, 3] &&
                        p2chess[4] >= cneed[i, 4] &&
                        p2chess[5] >= cneed[i, 5])
                    {
                        return true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                return flag;
            }
        }
        if (p1count == 1 && p2count == 1)
        {
            return false;
        }
        return true;
    }

    //textリセット
    private void fileRemake()
    {
        //カレントディレクトリを取得
        stCurDir = Directory.GetCurrentDirectory() + "/Assets/Resources/TXT";
        //text削除
        if (Directory.Exists(stCurDir))
        {
            File.Delete(stCurDir + "/board.txt");
        }
        //TXTファイルを作成
        if (!Directory.Exists(stCurDir))
        {
            Directory.CreateDirectory(stCurDir);
        }
        BoardListwrite();
    }

    /// <summary>3回同じ盤面確認用</summary>
    /// <returns>3回目FALSE,1.2回目TRUE</returns>
    public bool Draw_same()
    {
        //盤面書き出し
        BoardListwrite();
        //盤面読み込み確認
        int count = textRead();
        if (count >= 3) { return false; }
        return true;
    }

    //text追加分
    public void BoardListwrite()
    {
        wtxt = "";
        for (int z = 0; z < Mass_Size; z++)
        {
            for (int x = 0; x < Mass_Size; x++)
            {
                wtxt += FieldScript.Piece_Num[z, x].ToString();
            }
        }
        textSave(wtxt);
    }

    //text書き出し
    private void textSave(string txt)
    {
        using (StreamWriter sw = new StreamWriter(stCurDir + "/board.txt", true))
        {
            sw.WriteLine(txt);// ファイルに書き出したあと改行
            sw.Flush();       // StreamWriterのバッファに書き出し残しがないか確認
            sw.Close();       // ファイルを閉じる
        }
    }

    private int textRead()
    {
        int count = 0;
        int fre = 0;
        using (StreamReader file = new StreamReader(stCurDir + "/board.txt", Encoding.Default))
        {
            string rtxt = "";
            while (file.Peek() > -1)
            {
                // 1 行ずつ読み込み
                rtxt = file.ReadLine();
                if (wtxt == rtxt)
                {
                    ++count;
                    Debug.Log(count);
                    if (fre == 4) { fre = 0; }
                    else if (fre > 4) { count = 0; }
                }
                if (count > 0)
                {
                    ++fre;
                }
            }
            // ファイルを閉じる
            file.Close();
        }
        return count;
    }
}