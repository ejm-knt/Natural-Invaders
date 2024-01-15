using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//機能として収穫までの時間をバーにして表示する
public class FreshCountVer : MonoBehaviour
{
    public Slider slider;//Sliderを入れる
    GameObject gameDirectorObject;
    FarmGameDirector gameDirector;
    public FujiedaTomato tomato;

    void Start()
    {
        //Sliderを満タンにする。
        slider.value = -4;
        //現在のfreshを入れる。
        gameDirectorObject = GameObject.Find("FarmGameDirector");
        gameDirector = gameDirectorObject.GetComponent<FarmGameDirector>();
    }

    void Update()
    {
        if (tomato != null)
        {
            slider.value = tomato.fresh;
        }
    }
}
