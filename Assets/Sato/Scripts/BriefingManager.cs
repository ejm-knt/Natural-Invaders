using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
// using UnityEditor.Animations;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class BriefingManager : MonoBehaviour
{
    GameObject gameDirectorObject;
    GameDirector gameDirector;

    [SerializeField] GameObject memberWindowPrefab;
    [SerializeField] Transform scrollViewContent;
    [SerializeField] GameObject battleMemberAlert;
    [SerializeField] GameObject gridParent;
    [SerializeField] GameObject grid1;
    [SerializeField] GameObject grid2;
    [SerializeField] GameObject grid3;
    [SerializeField] GameObject grid4;
    [SerializeField] GameObject grid5;
    [SerializeField] GameObject grid6;
    [SerializeField] GameObject grid7;
    [SerializeField] GameObject grid8;
    [SerializeField] GameObject grid9;
    [SerializeField] GameObject partyInfoSumObject;    //PTキャラの合計情報
    private Text partySumInfo;
    List<GridCheck> grids;  //gridのリスト
    List<GameObject> frontLine;     //前列のGridObject
    List<GameObject> middleLine;    //中列のGridObject
    private int preSum; //パーティ情報の合計直前情報
    [SerializeField] GameObject[] allcharaPrefabs;  //全キャラ種類リスト

    void Awake()
    {
        //gameDirector(js)の取得
        gameDirectorObject = GameObject.Find("GameDirector");
        gameDirector = gameDirectorObject.GetComponent<GameDirector>();
        List<GameObject> allCharacterList = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            int num = Random.Range(0, allcharaPrefabs.Length);
            GameObject obj = Instantiate(allcharaPrefabs[num]);
            allCharacterList.Add(obj);
        }
        gameDirector.AllCharacters = allCharacterList;
    }

    void Start()
    {
        // PT情報表示用Textの取得
        partySumInfo = partyInfoSumObject.GetComponent<Text>();
        //すべてのキャラクターの分だけインスタンス生成
        foreach (GameObject character in gameDirector.AllCharacters)
        {
            // 可視化
            character.SetActive(true);
            // MemberWindowのPrefabからインスタンスを作成
            GameObject memberWindow = Instantiate(memberWindowPrefab, scrollViewContent);
            GameObject characterWindow = memberWindow.transform.GetChild(0).gameObject;
            // キャラクターのGameObjectをCharacterWindowの子要素として設定
            character.transform.SetParent(characterWindow.transform);
            // キャラクターのGameObjectを特定の位置に移動
            character.transform.localPosition = new Vector3(0, -90, 0);
            character.transform.localScale = new Vector3(14, 14, 14);
            character.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            // すべてのDragObjにUpdatePartySumInfoを追加
            DragObj dragObj = characterWindow.GetComponent<DragObj>();
            dragObj.OnPartySumInfoChanged += UpdatePartySumInfo;
            // キャラクターwindowのTextに値を設定
            Text text = memberWindow.transform.Find("info").GetComponent<Text>();
            CharacterScript cs = character.GetComponent<CharacterScript>();
            text.text = $"Life{cs.MaxCharacterLife}\nAtk{cs.CharacterAtk}\nMatk{cs.CharacterMatk}";
        }
        //gridのリストを作成
        grids = new List<GridCheck>(){
            grid1.GetComponent<GridCheck>(),
            grid2.GetComponent<GridCheck>(),
            grid3.GetComponent<GridCheck>(),
            grid4.GetComponent<GridCheck>(),
            grid5.GetComponent<GridCheck>(),
            grid6.GetComponent<GridCheck>(),
            grid7.GetComponent<GridCheck>(),
            grid8.GetComponent<GridCheck>(),
            grid9.GetComponent<GridCheck>()
        };
        // 前列中列のリスト作成
        frontLine = new List<GameObject> { grid1, grid2, grid3 };
        middleLine = new List<GameObject> { grid4, grid5, grid6 };
        // SoundManager.instance.PlayBGM(SoundManager.BGM_Type.titel);
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Bgm04PartyEdit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdatePartySumInfo()
    {
        int AtkSum = 0;
        int MatkSum = 0;
        int DefASum = 0;
        int AtkASum = 0;
        // パーティのステータス合計
        List<CharacterScript> frontCsList = new List<CharacterScript>();
        foreach (GameObject frontObj in frontLine)
        {
            CharacterScript cs = frontObj.GetComponentInChildren<CharacterScript>();
            if (cs != null)
            {
                frontCsList.Add(cs);
            }
        }
        List<CharacterScript> middleCsList = new List<CharacterScript>();
        foreach (GameObject middleObj in middleLine)
        {
            CharacterScript cs = middleObj.GetComponentInChildren<CharacterScript>();
            if (cs != null)
            {
                middleCsList.Add(cs);
            }
        }
        foreach (CharacterScript cs in frontCsList)
        {
            AtkSum += cs.CharacterAtk;
            MatkSum += cs.CharacterMatk;
        }
        foreach (CharacterScript cs in middleCsList)
        {
            // DefASum += cs.CharacterDaux;
            // AtkASum += cs.CharacterAaux;
        }
        partySumInfo.text =
        $"前列\nATK {AtkSum}\nMatk {MatkSum}\n中列補助\nDefA {DefASum}\nAtkA {AtkASum}";
    }

    public void OnClickBattleStartButton()
    {
        //5人以下かどうかチェック
        int battleMemberCount = 0;
        foreach (GridCheck grid in grids)
        {
            if (grid.attached)
            {
                battleMemberCount++;
            }
        }
        if (battleMemberCount <= 5)
        {
            GetChara getChara = gridParent.GetComponent<GetChara>();
            List<GameObject> list = getChara.Get();
            gameDirector.PartyMembers = list;
            foreach (GameObject partyCharacter in gameDirector.PartyMembers)
            {
                CharacterScript cs = partyCharacter.GetComponent<CharacterScript>();
                GridCheck gc = partyCharacter.GetComponentInParent<GridCheck>();
                cs.position = gc.position;
                partyCharacter.transform.parent = null;
                DontDestroyOnLoad(partyCharacter);
            }
            foreach (GameObject allchara in gameDirector.AllCharacters)
            {
                allchara.transform.parent = null;
                DontDestroyOnLoad(allchara);
                allchara.SetActive(false);
            }
            foreach (GameObject ptchara in gameDirector.PartyMembers)
            {
                ptchara.SetActive(true);
            }
            SceneManager.LoadScene("Battle");
        }
        else
        {
            //警告文
            battleMemberAlert.SetActive(true);
        }
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);

    }

    public void OnClickSortButton()
    {
        //5人以下かどうかチェック
        int battleMemberCount = 0;
        foreach (GridCheck grid in grids)
        {
            if (grid.attached)
            {
                battleMemberCount++;
            }
        }
        if (battleMemberCount <= 5)
        {
            GetChara getChara = gridParent.GetComponent<GetChara>();
            List<GameObject> list = getChara.Get();
            gameDirector.PartyMembers = list;
            foreach (GameObject partyCharacter in gameDirector.PartyMembers)
            {
                CharacterScript cs = partyCharacter.GetComponent<CharacterScript>();
                GridCheck gc = partyCharacter.GetComponentInParent<GridCheck>();
                cs.position = gc.position;
                partyCharacter.transform.parent = null;
                DontDestroyOnLoad(partyCharacter);
            }
            foreach (GameObject allchara in gameDirector.AllCharacters)
            {
                allchara.transform.parent = null;
                DontDestroyOnLoad(allchara);
                allchara.SetActive(false);
            }
            foreach (GameObject ptchara in gameDirector.PartyMembers)
            {
                ptchara.SetActive(true);
            }
            SceneManager.LoadScene("Battle1");
        }
        else
        {
            //警告文
            battleMemberAlert.SetActive(true);
        }
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);


    }
}
