using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    /// <summary>Result値格納</summary>
    public static int number;
    /// <summary>win/lose/draw格納</summary>
    private GameObject[] resultText = new GameObject[3];
    /// <summary>Player</summary>
    private GameObject[] playerText = new GameObject[2];
    /// <summary>花火</summary>
    public GameObject Eff;
    
    private bool gameFlag = false;

    enum ResultChange : int
    {
        nothing,
        win1,
        win2,
        draw,
    }

    // Use this for initialization
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Result")
        {
            resultText[0] = GameObject.Find("Canvas/Result/tWin");
            resultText[0].SetActive(false);
            resultText[1] = GameObject.Find("Canvas/Result/tLose");
            resultText[1].SetActive(false);
            resultText[2] = GameObject.Find("Canvas/Result/tDraw");
            resultText[2].SetActive(false);
            playerText[0] = GameObject.Find("Canvas/Player/tPlayer1");
            playerText[0].SetActive(false);
            playerText[1] = GameObject.Find("Canvas/Player/tPlayer2");
            playerText[1].SetActive(false);

            Eff.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameMain")
        {
            if (gameFlag)
            {
                StartCoroutine("coRoutine");
                gameFlag = false;
            }
        }
        if (SceneManager.GetActiveScene().name == "Result")
        {
            ResultText();
        }
    }

    IEnumerator coRoutine()
    {
        yield return new WaitForSeconds(5); // 5秒待機
        SceneManager.LoadScene("Result");
        CheckScript.fcheckmate_draw = false;
    }

    public void ChangeJudge(int player)
    {
        switch (player)
        {
            case (int)ResultChange.nothing:
                break;
            case (int)ResultChange.win1:
            case (int)ResultChange.win2:
            case (int)ResultChange.draw:
                number = player;
                gameFlag = true;
                break;
            default:
                break;
        }
    }

    private void ResultText()
    {
        //利用中のplayer
        bool player = RoomController.getplayerflag();
        //player1が勝利した場合
        if (number == (int)ResultChange.win1)
        {
            if (player)
            {
                resultText[0].SetActive(true);
                playerText[0].SetActive(true);
                Eff.SetActive(true);
            }
            else
            {
                resultText[1].SetActive(true);
                playerText[1].SetActive(true);
            }
        }
        //player2が勝利した場合
        if (number == (int)ResultChange.win2)
        {
            if (!player)
            {
                resultText[0].SetActive(true);
                playerText[1].SetActive(true);
                Eff.SetActive(true);
            }
            else
            {
                resultText[1].SetActive(true);
                playerText[0].SetActive(true);
            }
        }
        //引き分けの場合
        else if (number == (int)ResultChange.draw)
        {
            resultText[2].SetActive(true);
        }
    }
}
