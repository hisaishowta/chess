using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class ParticleScript : MonoBehaviour
{
    [SerializeField]
    private GameObject move_prt;    //移動先参考用パーティクル
    [SerializeField]
    private GameObject move_prt2;   //チェック参考用パーティクル
    [SerializeField]
    private GameObject p_baseObj;   //パーティクルの親
    //パーティクル格納 P1,P2
    private GameObject p_obj;   

    //playerの判定
    PlayerController playercntl;

    // Use this for initialization
    void Start()
    {
        //別スクリプト取得
        playercntl = GameObject.Find("PlayerController").GetComponent<PlayerController>();
    }
        
    // Update is called once per frame
    void Update()
    {
        //生成削除
        if (p_baseObj.transform.childCount == 0)
        {
            Particle_Create();
        }
        else if (p_baseObj.transform.childCount == 1)
        {
            Particle_Destroy();
        }
    }

    public void Particle_Create()
    {
        //Layer確認 && Player確認(true OR false)
        if (p_baseObj.layer == LayerMask.NameToLayer("Board") && playercntl.PlayerCheck())
        {
            //パーティクル出現
            //パーティクル,Baseと同じ位置,角度 
            p_obj = GameObject.Instantiate(
                move_prt,
                transform.position,
                Quaternion.identity);
            //作成するパーティクル名
            p_obj.name = "B_Part";
            //作成した空オブジェクトを親にする
            p_obj.transform.parent = p_baseObj.transform;
        }
        else if (p_baseObj.layer == LayerMask.NameToLayer("Board"))
        {
            //パーティクル出現
            //パーティクル,Baseと同じ位置,角度 
            p_obj = GameObject.Instantiate(
                move_prt2,
                transform.position,
                Quaternion.identity);
            //作成するパーティクル名
            p_obj.name = "B_Part2";
            //作成した空オブジェクトを親にする
            p_obj.transform.parent = p_baseObj.transform;
        }
    }

    public void Particle_Destroy()
    {
        //Layer確認
        if (p_baseObj.layer != LayerMask.NameToLayer("Board"))
        {
            if (p_obj != null)
            {
                //パーティクル削除
                Destroy(p_obj);
            }
        }
    }
}
