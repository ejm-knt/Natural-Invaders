using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmGameDirector : MonoBehaviour
{
    //このクラスの役割
    //ゲームオブジェクト(オールキャラクターリスト)を所持(インスタンスは（仮:FujiedaTomato）)
    //植えた種リストを所持

    public List<FujiedaTomato> AllCharacters = new List<FujiedaTomato>(); //全キャラリスト
    //public List<GameObject> PartyMembers = new List<GameObject>(); //全パーティメンバーリスト
    public List<FujiedaTomato> PlantedSeedCharacters = new List<FujiedaTomato>(); //植えた種リスト

    [SerializeField] FujiedaTomato Character1;//動作確認用
    [SerializeField] FujiedaTomato Character2;
    [SerializeField] FujiedaTomato Character3;
    [SerializeField] FujiedaTomato Character4;
    [SerializeField] FujiedaTomato Character5;
    [SerializeField] FujiedaTomato Character6;
    [SerializeField] FujiedaTomato Character7;

    public static FarmGameDirector Instance { get; private set; }   //シングルトンインスタンス

    // Start is called before the first frame update
    void Awake()//FarmDirectorより先に動いてもらう
    {
        //フレームレートを60に
        Application.targetFrameRate = 60;
        AllCharacters.Add(Character1);
        AllCharacters.Add(Character2);
        AllCharacters.Add(Character3);
        AllCharacters.Add(Character4);
        AllCharacters.Add(Character5);
        AllCharacters.Add(Character6);
        AllCharacters.Add(Character7);
    }
    void Update()
    {

    }

}
