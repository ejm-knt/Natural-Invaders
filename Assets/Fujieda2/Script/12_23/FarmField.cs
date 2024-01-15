using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmField : MonoBehaviour
{
    public bool isEnpty;
    public int farmFieldPos;

    public void FarmNotSeed()//ボタンを押せなくするメソッド
    {
        if (isEnpty == false)//true （種が植えてある）畑はボタンが押せなくなる
        {
            Button btn = GetComponent<Button>();
            btn.interactable = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FarmNotSeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnpty == false)//true （種が植えてある）畑はボタンが押せなくなる
        {
            FarmNotSeed();
        }
    }

}
