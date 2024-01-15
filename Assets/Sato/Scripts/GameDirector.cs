using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public List<GameObject> AllCharacters = new List<GameObject>(); //全キャラリスト
    public List<GameObject> PartyMembers = new List<GameObject>(); //全パーティメンバーリスト
    public List<GameObject> PlantedSeedCharacters = new List<GameObject>(); //植えた種リスト
    // [SerializeField] GameObject Character1;     //動作確認用
    // [SerializeField] GameObject Character2;
    // [SerializeField] GameObject Character3;
    // [SerializeField] GameObject Character4;
    // [SerializeField] GameObject Character5;
    // [SerializeField] GameObject Character6;
    // [SerializeField] GameObject Character7;
    public List<CharacterScript> allcharaCs = new List<CharacterScript>();

    public static GameDirector Instance { get; private set; }   //シングルトンインスタンス

    private void Awake()
    {
        // インスタンスが既に存在していた場合は破棄する
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // このインスタンスをシングルトンインスタンスとして登録
        Instance = this;

        // シーン遷移時に破棄されないように設定
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // フレームレートを60に
        Application.targetFrameRate = 60;
        // AllCharacters.Add(Character1);   //デバッグ用
        // AllCharacters.Add(Character2);
        // AllCharacters.Add(Character3);
        // AllCharacters.Add(Character4);
        // AllCharacters.Add(Character5);
        // AllCharacters.Add(Character6);
        // AllCharacters.Add(Character7);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RemoveNull()   //partyMemberで死亡した時にAllCharactersにnullが出てしまうためその要素を削除する
    {
        AllCharacters.RemoveAll(character => character == null);
    }

    public int FRESHCOUNT = 1;//戦闘終了時に鮮度が進む(定数)
    // public CharacterScript[] characters;//動作確認用のキャラクター配列

    public void NextFreshCount() //キャラクターの鮮度が進む(藤枝農場用)
    {
        Debug.Log("フレッシュカウントメソッドに入った");
        //gameDirector(js)の取得
        // gameDirectorObject = GameObject.Find("GameDirector");
        // gameDirector = gameDirectorObject.GetComponent<FarmGameDirector>();
        SetCharacterScriptList();
        foreach(CharacterScript cs in allcharaCs){
            if(cs.fresh <= 0 && cs.isPlanted == false){
                continue;
            }else if (cs.fresh <= 0 && cs.isPlanted == false){
                cs.fresh += FRESHCOUNT;
            }else{
                cs.fresh += FRESHCOUNT;
            }

        }

        // for (int i = 0; i < allcharaCs.Count; i++)
        // {
        //     if (AllCharacters[i].fresh <= 0 && AllCharacters[i].isPlanted == false)//鮮度0以下かつ畑に植えていない
        //     {
        //         continue;//スキップ
        //     }
        //     else if (AllCharacters[i].fresh <= 0 && AllCharacters[i].isPlanted == true)//鮮度0以下かつ畑に植えている
        //     {
        //         AllCharacters[i].fresh += FRESHCOUNT;//鮮度を1進める
        //     }
        //     else//鮮度1以上
        //     {
        //         AllCharacters[i].fresh += FRESHCOUNT;//鮮度を1進める
        //     }
        // }
    }
    public void SetCharacterScriptList(){
        foreach(GameObject obj in AllCharacters){
            allcharaCs.Add(obj.GetComponent<CharacterScript>());
        }
    }
}
