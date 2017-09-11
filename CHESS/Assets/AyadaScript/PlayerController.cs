using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool PlayerFlag; //P1,P2ターン管理フラグ　true=P1,false=P2

    public GameObject P1Camera;
    public GameObject P2Camera;

    //引き分け
    private GameObject DrawCntl;
    Draw mDraw;
    //テキスト表示用
    private GameObject gClickController;
    ClickController c_cntl;

    Result ResultCntl;

    // Use this for initialization
    void Start()
    {
        PlayerFlag = true;

        if (!RoomController.getplayerflag())
        {
            P1Camera.SetActive(false);
            P2Camera.SetActive(true);
        }

        Debug.Log(RoomController.getplayerflag());

        //テキスト表示用
        gClickController = GameObject.Find("ClickController");
        c_cntl = gClickController.GetComponent<ClickController>();

        DrawCntl = GameObject.Find("CheckController");
        mDraw = DrawCntl.GetComponent<Draw>();
        ResultCntl = DrawCntl.GetComponent<Result>();
    }

    //ターン管理フラグのPlayerFlagを返す
    public bool PlayerCheck()
    {
        return PlayerFlag;
    }

    //ターン切り替え
    public void TurnState()
    {
        if (PlayerFlag)
            PlayerFlag = false;
        else
            PlayerFlag = true;

        if (!mDraw.Draw_check() ||
            !mDraw.Draw_rule50() ||
            !mDraw.Draw_needchess() ||
            !mDraw.Draw_same())
        {
            //if (Result.number == 0)
            //{
                c_cntl.Draw.SetActive(true);
                ResultCntl.ChangeJudge(3);
                CheckScript.fcheckmate_draw = true;
            //}
        }
    }
}
