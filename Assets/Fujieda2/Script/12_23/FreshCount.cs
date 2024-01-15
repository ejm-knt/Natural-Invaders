using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreshCount : MonoBehaviour
{
    //このクラスの役割:戦闘終了時に動くクラス(メソッド)
    //戦闘終了時に鮮度値(FRESHCOUNT)を進める
    //種(field == true)の種は(FRESHCOUNTが1になったら)種=>野菜になり仲間に加わる(畑画面から外れる)

    public int FRESHCOUNT = 1;//戦闘終了時に鮮度が進む(定数)
    public CharacterScript[] characters;//動作確認用のキャラクター配列
    GameObject gameDirectorObject;
    FarmGameDirector gameDirector;

    public void NextFreshCount() //キャラクターの鮮度が進む
    {
        //動作確認用
        Debug.Log("NextFreshCount");
        //gameDirector(js)の取得
        gameDirectorObject = GameObject.Find("FarmGameDirector");
        gameDirector = gameDirectorObject.GetComponent<FarmGameDirector>();

        for (int i = 0; i < gameDirector.PlantedSeedCharacters.Count; i++)
        {
            if (gameDirector.PlantedSeedCharacters[i] != null)
            {
                gameDirector.PlantedSeedCharacters[i].fresh += FRESHCOUNT;//鮮度を1進める
                                                                          //isPlanted == falseの判定変更を入れる予定(リストから外す)
                                                                          //ステータステキストを書き換える               
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
