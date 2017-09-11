using System.Collections;
using System.Text;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private Person person;

    private GameObject StartButton;
    private GameObject resetButton;
    private GameObject PlayerText;

    public static bool playerflag=true;
    private bool PlayerSearch = false;
    private bool SetEnd = false;
    private bool SearchFlag = false;
    private bool ResetFlag = false;
    private bool PushFlag = false;
    
    //綾田AI関係
    public static bool Ayada_AiFlag = false;
    public static bool A_Ai_PlayerFlag = false;
    //久井AI関係
    public static bool Hisai_AiFlag = false;
    public static bool H_Ai_PlayerFlag = false;

    public static bool Ai_BattleFlag = false;


    //自分がPlayer1ならtrue ::  Player2ならfalse を返す
    public static bool getplayerflag()
    {
        return playerflag;
    }

    public static bool getAi_BattleFlag()
    {
        return Ai_BattleFlag;
    }

    //綾田Ai関係-------------------------------
    public static bool getAyada_Aiflag()
    {
        return Ayada_AiFlag;
    }

    public static bool getA_Ai_Playerflag()
    {
        return A_Ai_PlayerFlag;
    }
    //----------------------------------------

    //久井Ai関係-------------------------------
    public static bool getH_Aiflag()
    {
        return Hisai_AiFlag;
    }

    public static bool getH_Ai_Playerflag()
    {
        return H_Ai_PlayerFlag;
    }
    //----------------------------------------

    // Use this for initialization
    void Start()
    {
        person = new Person();
        StartButton = GameObject.Find("Canvas/btnRoom");
        resetButton = GameObject.Find("Canvas/ResetButton");
        PlayerText = GameObject.Find("Canvas/PlayerText");
        StartButton.SetActive(false);
        PlayerText.SetActive(false);
        resetButton.SetActive(false);
        StartCoroutine(SetURL());
    }

    // Update is called once per frame
    void Update()
    {
        if (!SearchFlag && !ResetFlag && PushFlag)
        {
            if (PlayerSearch && playerflag)
            {
                StartCoroutine(P1Search());
            }
            else if (PlayerSearch && !playerflag)
            {
                StartCoroutine(P2Search());
            }
            else if (Ayada_AiFlag || Hisai_AiFlag)
            {
                StartButton.SetActive(true);
            }
            resetButton.SetActive(true);
        }
    }

    //P1ボタンが押されたら
    public void Player1()
    {
        if (!PlayerSearch && SetEnd && !PushFlag)
        {
            StartCoroutine(P1Set());
            PushFlag = true;
        }
    }
    //P2ボタンが押されたら
    public void Player2()
    {
        if (!PlayerSearch && SetEnd && !PushFlag)
        {
            StartCoroutine(P2Set());
            PushFlag = true;
        }
    }

    //Ayadaボタンが押されたら
    public void Ayada_Ai()
    {
        if (!PushFlag)
        {
            Ayada_AiFlag = true;
            PushFlag = true;
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                playerflag = true;
                A_Ai_PlayerFlag = false;
            }
            else
            {
                playerflag = false;
                A_Ai_PlayerFlag = true;
            }
            Debug.Log(A_Ai_PlayerFlag);
        }
    }

    //Hisaiボタンが押されたら
    public void Hisai_Ai()
    {
        if (!PushFlag)
        {
            Hisai_AiFlag = true;
            PushFlag = true;
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                playerflag = true;
                H_Ai_PlayerFlag = false;
            }
            else
            {
                playerflag = false;
                H_Ai_PlayerFlag = true;
            }
            Debug.Log(H_Ai_PlayerFlag);
        }
    }

    //AIBattleボタンが押されたら
    public void Ai_Battle()
    {
        if (!PushFlag)
        {
            Ayada_AiFlag = true;
            Hisai_AiFlag = true;      
            PushFlag = true;
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                A_Ai_PlayerFlag = true;
                H_Ai_PlayerFlag = false;
                Debug.Log("ayada" + A_Ai_PlayerFlag);
                Debug.Log("hisai" + H_Ai_PlayerFlag);
            }
            else
            {
                A_Ai_PlayerFlag = false;
                H_Ai_PlayerFlag = true;
                Debug.Log("ayada" + A_Ai_PlayerFlag);
                Debug.Log("hisai" + H_Ai_PlayerFlag);
            }
            Ai_BattleFlag = true;
        }
    }

    public void ResetButton()
    {
        PlayerSearch = false;
        ResetFlag = true;
        Ai_BattleFlag = false;
        StartCoroutine(ResetURL());
        Ayada_AiFlag = false;
        Hisai_AiFlag = false;
    }

    IEnumerator ResetURL()
    {
        if (!Ayada_AiFlag && !Hisai_AiFlag)
        {
            if (playerflag)
            {
                WWW P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_0");
                yield return P1URL;

                person = JsonUtility.FromJson<Person>(P1URL.text.Trim('[', ']'));
                Debug.Log(person.value);
                var personValue = JsonUtility.FromJson<PersonValue>(person.value);
                Debug.Log("roomid:" + personValue.roomid);

                personValue.roomid = 0;

                string json = JsonUtility.ToJson(personValue);

                //P1のroomid更新
                P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_0&value=" + json);
                yield return P1URL;
                if (!string.IsNullOrEmpty(P1URL.error))
                {
                    Debug.Log(P1URL.error);
                }
                else
                {
                    Debug.Log("resetP1room:" + P1URL.text);
                }
            }
            else
            {
                WWW P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_1");
                yield return P2URL;

                person = JsonUtility.FromJson<Person>(P2URL.text.Trim('[', ']'));
                Debug.Log(person.value);
                var personValue = JsonUtility.FromJson<PersonValue>(person.value);
                Debug.Log("roomid:" + personValue.roomid);

                personValue.roomid = 0;

                string json = JsonUtility.ToJson(personValue);

                //P2のroomid更新
                P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_1&value=" + json);
                yield return P2URL;
                if (!string.IsNullOrEmpty(P2URL.error))
                {
                    Debug.Log(P2URL.error);
                }
                else
                {
                    Debug.Log("resetP2room:" + P2URL.text);
                }
            }
        }
        StartButton.SetActive(false);
        resetButton.SetActive(false);
        PlayerText.SetActive(false);
        PushFlag = false;
        ResetFlag = false;
    }

    //Player1とPlayer2のサーバのデータに初期値をセット
    IEnumerator SetURL()
    {
        //URLの中身を初期化
        {
            WWW P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_0&value={\"roomid\":0,\"fromZid\":0,\"fromXid\":0,\"toZid\":0,\"toXid\":0,\"turnid\":0,\"castid\":0,\"enid\":0,\"proid\":0}");
            yield return P1URL;
            if (!string.IsNullOrEmpty(P1URL.error))
            {
                // print(w.error);
                Debug.Log(P1URL.error);
            }
            else
            {
                Debug.Log("P1setValueWithKey Finished:" + P1URL.text);
            }
        }


        //URLの中身を初期化
        {
            WWW P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_1&value={\"roomid\":0,\"fromZid\":0,\"fromXid\":0,\"toZid\":0,\"toXid\":0,\"turnid\":0,\"castid\":0,\"enid\":0,\"proid\":0}");
            yield return P2URL;
            if (!string.IsNullOrEmpty(P2URL.error))
            {
                // print(w.error);
                Debug.Log(P2URL.error);
            }
            else
            {
                Debug.Log("P2setValueWithKey Finished:" + P2URL.text);
            }
        }

        SetEnd = true;
    }


    IEnumerator P1Set()
    {
        playerflag = true;
        PlayerSearch = true;

        WWW P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_0");
        yield return P1URL;

        person = JsonUtility.FromJson<Person>(P1URL.text.Trim('[', ']'));
        Debug.Log(person.value);
        var personValue = JsonUtility.FromJson<PersonValue>(person.value);
        Debug.Log("roomid:" + personValue.roomid);

        personValue.roomid = 1;

        string json = JsonUtility.ToJson(personValue);

        //P1のroomid更新
        P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_0&value=" + json);
        yield return P1URL;
        if (!string.IsNullOrEmpty(P1URL.error))
        {
            Debug.Log(P1URL.error);
        }
        else
        {
            Debug.Log("setP1room:" + P1URL.text);
        }
    }

    IEnumerator P1Search()
    {
        SearchFlag = true;
        //P2のデータを取得
        WWW P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_1");
        yield return P2URL;
        if (!string.IsNullOrEmpty(P2URL.error))
        {
            Debug.Log(P2URL.error);
        }
        else
        {
            Debug.Log("setP2room:" + P2URL.text);
        }

        person = JsonUtility.FromJson<Person>(P2URL.text.Trim('[', ']'));
        Debug.Log(person.value);
        var personValue = JsonUtility.FromJson<PersonValue>(person.value);
        Debug.Log("P2roomid:" + personValue.roomid);

        if (personValue.roomid == 1)
        {
            StartButton.SetActive(true);
            PlayerText.SetActive(false);
        }
        else
        {
            StartButton.SetActive(false);
            PlayerText.SetActive(true);
        }
        SearchFlag = false;
    }

    IEnumerator P2Set()
    {

        playerflag = false;
        PlayerSearch = true;

        WWW P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_1");
        yield return P2URL;

        person = JsonUtility.FromJson<Person>(P2URL.text.Trim('[', ']'));
        Debug.Log(person.value);
        var personValue = JsonUtility.FromJson<PersonValue>(person.value);
        Debug.Log("roomid:" + personValue.roomid);

        personValue.roomid = 1;

        string json = JsonUtility.ToJson(personValue);

        //P2のroomid更新
        P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_1&value=" + json);
        yield return P2URL;
        if (!string.IsNullOrEmpty(P2URL.error))
        {
            Debug.Log(P2URL.error);
        }
        else
        {
            Debug.Log("setP2room:" + P2URL.text);
        }
    }

    IEnumerator P2Search()
    {
        SearchFlag = true;
        //P1のデータを取得
        WWW P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_0");
        yield return P1URL;
        if (!string.IsNullOrEmpty(P1URL.error))
        {
            Debug.Log(P1URL.error);
        }
        else
        {
            Debug.Log("setP1room:" + P1URL.text);
        }

        person = JsonUtility.FromJson<Person>(P1URL.text.Trim('[', ']'));
        Debug.Log(person.value);
        var personValue = JsonUtility.FromJson<PersonValue>(person.value);
        Debug.Log("P1roomid:" + personValue.roomid);

        if (personValue.roomid == 1)
        {
            StartButton.SetActive(true);
            PlayerText.SetActive(false);
        }
        else
        {
            StartButton.SetActive(false);
            PlayerText.SetActive(true);
        }
        SearchFlag = false;
    }


    [Serializable]
    public class Person
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class PersonValue
    {
        public int roomid;
        public int fromZid;
        public int fromXid;
        public int toZid;
        public int toXid;
        public int turnid;
        public int castid;
        public int enid;
        public int proid;
    }
}
