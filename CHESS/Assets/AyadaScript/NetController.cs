using System.Collections;
using System.Text;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetController : MonoBehaviour
{
    public Person person;
    public PersonValue personvalue;

    private bool E_P_DataSearch = false; //EnemyPlayerのデータサーチフラグ
    public bool E_P_TurnEnd = false;

    public bool C_DataSetEnd = false;
    public bool C_DataSetStart = false;

    public bool EvolutionCheck = false;

    private GameObject MoveControlle;
    MoveController movecontrolle;

    // Use this for initialization
    void Start()
    {
        MoveControlle = GameObject.Find("MoveController");
        movecontrolle = MoveControlle.GetComponent<MoveController>();
        person = new Person();
    }

    // Update is called once per frame
    void Update()
    {    
    }

    public void CheckNet()
    {
        //連続で入らないように制御
        if (!E_P_DataSearch && !E_P_TurnEnd)
        {
            //自分のプレイヤーの方に入る
            if (RoomController.getplayerflag())
            {
                StartCoroutine(P1Net());
            }
            else
            {
                StartCoroutine(P2Net());
            }
        }
    }

    IEnumerator P1Net()
    {
        E_P_DataSearch = true;
      
        WWW P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_1");

        yield return P2URL;
        if (!string.IsNullOrEmpty(P2URL.error))
        {
            Debug.Log(P2URL.error);
        }
        else
        {
            Debug.Log("getValueWithKey Finished:" + P2URL.text);
        }

        person = JsonUtility.FromJson<Person>(P2URL.text.Trim('[', ']'));
        Debug.Log(person.value);
        var personValue = JsonUtility.FromJson<PersonValue>(person.value);

        //相手ターンが終了していたら相手が動かした駒の位置をもとに関数を呼び動かす
        if (personValue.turnid == 1)
        {
            int fz, fx, tz, tx,cast,en,pro;
            fz = personValue.fromZid; fx = personValue.fromXid; tz =personValue.toZid; tx = personValue.toXid;
            cast = personValue.castid; en = personValue.enid; pro = personValue.proid;
            movecontrolle.E_PlayerSetTarget(fz, fx, tz, tx, cast, en, pro);
            E_P_TurnEnd = true;

            //データを受け取ったらデータを初期化
            P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_1&value={\"roomid\":0,\"fromZid\":0,\"fromXid\":0,\"toZid\":0,\"toXid\":0,\"turnid\":0,\"castid\":\"0\",\"enid\":0,\"proid\":0}");
            yield return P2URL;
            if (!string.IsNullOrEmpty(P2URL.error))
            {
                Debug.Log(P2URL.error);
            }
            else
            {
                Debug.Log("P2データ初期化:" + P2URL.text);

            }
            
        }
        Debug.Log(person.value);
        E_P_DataSearch = false;
    }

    IEnumerator P2Net()
    {
        E_P_DataSearch = true;
       
        WWW P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=getValueWithKey&apriId=devChess&key=userdata_0");

        yield return P1URL;
        if (!string.IsNullOrEmpty(P1URL.error))
        {
            Debug.Log(P1URL.error);
        }
        else
        {
            Debug.Log("getValueWithKey Finished:" + P1URL.text);
        }

        person = JsonUtility.FromJson<Person>(P1URL.text.Trim('[', ']'));
        Debug.Log(person.value);
        var personValue = JsonUtility.FromJson<PersonValue>(person.value);
        Debug.Log("roomid:" + personValue.roomid);

        //相手ターンが終了していたら相手が動かした駒の位置をもとに関数を呼び動かす
        if (personValue.turnid == 1)
        {
            int fz, fx, tz, tx,cast, en, pro;
            fz = personValue.fromZid; fx = personValue.fromXid; tz = personValue.toZid; tx = personValue.toXid;
            cast = personValue.castid; en = personValue.enid; pro = personValue.proid;
            movecontrolle.E_PlayerSetTarget(fz, fx, tz, tx, cast, en, pro);
            E_P_TurnEnd = true;

            //データを受け取ったらデータを初期化
            P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_0&value={\"roomid\":0,\"fromZid\":0,\"fromXid\":0,\"toZid\":0,\"toXid\":0,\"turnid\":0,\"castid\":\"0\",\"enid\":0,\"proid\":0}");
            yield return P1URL;
            if (!string.IsNullOrEmpty(P1URL.error))
            {
                Debug.Log(P1URL.error);
            }
            else
            {
                Debug.Log("P1データ初期化:" + P1URL.text);

            }
        }
        Debug.Log(person.value);
        E_P_DataSearch = false;
    }


    //自分の駒を動かした後サーバのデータを更新 
    public IEnumerator C_DataSet()
    {
        C_DataSetStart = true;
        //駒のデータをセット
        personvalue.turnid = 1;
        string json = JsonUtility.ToJson(personvalue);

        //駒データを更新
        {

            //自分プレイヤーの方のデータを更新
            if (RoomController.getplayerflag())
            {
                WWW P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_0&value=" + json);
                yield return P1URL;
                if (!string.IsNullOrEmpty(P1URL.error))
                {
                    Debug.Log(P1URL.error);
                }
                else
                {
                    Debug.Log("P1TurnidState:" + P1URL.text);

                }

            }
            else
            {
                WWW P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_1&value=" + json);
                yield return P2URL;
                if (!string.IsNullOrEmpty(P2URL.error))
                {
                    Debug.Log(P2URL.error);
                }
                else
                {
                    Debug.Log("P2TurnidState:" + P2URL.text);

                }
            }

        }


        //相手プレイヤーの方のデータをリセットする
        {

            if (RoomController.getplayerflag())
            {
                WWW P2URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_1&value={\"roomid\":0,\"fromZid\":0,\"fromXid\":0,\"toZid\":0,\"toXid\":0,\"turnid\":0,\"castid\":\"0\",\"enid\":0,\"proid\":0}");
                yield return P2URL;
                if (!string.IsNullOrEmpty(P2URL.error))
                {
                    Debug.Log(P2URL.error);
                }
                else
                {
                    Debug.Log("P2データ初期化:" + P2URL.text);

                }

            }
            else
            {
                WWW P1URL = new WWW("http://www16306uf.sakura.ne.jp/Chess/ChessIndex.php?proc=setValueWithKey&apriId=devChess&key=userdata_0&value={\"roomid\":0,\"fromZid\":0,\"fromXid\":0,\"toZid\":0,\"toXid\":0,\"turnid\":0,\"castid\":\"0\",\"enid\":0,\"proid\":0}");
                yield return P1URL;
                if (!string.IsNullOrEmpty(P1URL.error))
                {
                    Debug.Log(P1URL.error);
                }
                else
                {
                    Debug.Log("P1データ初期化:" + P1URL.text);

                }

            }

        }

        //personvalueの中身を0にリセット
        {
            personvalue.roomid = 0;
            personvalue.turnid = 0;
            personvalue.castid = 0;
            personvalue.enid = 0;
            personvalue.proid = 0;
        }
        
        C_DataSetEnd = true;

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
        public int roomid;  //0:入ってない 1:入室中
        public int fromZid; //相手プレイヤーが動かした駒の移動前のZ位置
        public int fromXid; //相手プレイヤーが動かした駒の移動前のX位置
        public int toZid;   //相手プレイヤーが動かした駒の移動後のZ位置
        public int toXid; 　//相手プレイヤーが動かした駒の移動後のX位置
        public int turnid;  //0:思考中 1:ターンエンド
        public int castid;  //0:実行なし 1:左側実行 2:右側実行
        public int enid;
        public int proid;   //0:実行なし 1:クイーン 2:ルーク 3:ビショップ 4:ナイト
    }
}
