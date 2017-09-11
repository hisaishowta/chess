using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEff : MonoBehaviour
{
    public GameObject explotion;
    public Transform spawn;
    private int count;

    // Use this for initialization
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject obj;
        if (count++ > 30)
        {
            // 左クリックが押されたら
            obj = Instantiate(explotion, spawn.position, spawn.rotation) as GameObject;
            Destroy(obj, 3.0f); // 三秒後に削除
            count = 0;
        }
    }
}