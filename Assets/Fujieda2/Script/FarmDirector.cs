using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class FarmDirector : MonoBehaviour
{//このクラスの役割
 //FindメソッドでFind TestGameDirectorを取得
 //画像を所持しており、自身の画像を表示(植えた時には種画像を書き換える)
 //植えた種リストを生成(シーン起動時に判定し、成長したかどうかを判定して消す、消したらnullにしておく)


    GameObject gameDirectorObject;
    public FarmGameDirector farmGameDirector;
    [SerializeField] FarmClickManager farmClickManager;
    [SerializeField] GameObject seedWindowPrefab;
    [SerializeField] Transform scrollViewContent;
    List<FujiedaTomato> SeedCharacters = new List<FujiedaTomato>();

    // Start is called before the first frame update
    void Start()
    {
        //FarmシーンのBGMを再生
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Bgm01Farm);

        //gameDirector(js)の取得
        gameDirectorObject = GameObject.Find("FarmGameDirector");
        farmGameDirector = gameDirectorObject.GetComponent<FarmGameDirector>();
        //すべてのキャラクターの分だけインスタンス生成
        for (int i = 0; i < farmGameDirector.AllCharacters.Count; i++)
        {
            if (farmGameDirector.AllCharacters[i].fresh > 1 && farmGameDirector.AllCharacters[i].isPlanted == true)
            {//植えていて鮮度が1以上になったら、植える判定をfalse
                farmGameDirector.AllCharacters[i].isPlanted = false;
            }
            else if (farmGameDirector.AllCharacters[i].fresh > 1) { continue; }
            else
            {
                Debug.Log("test");

                SeedCharacters.Add(farmGameDirector.AllCharacters[i]);
                // 可視化
                farmGameDirector.AllCharacters[i].gameObject.SetActive(true);

                //Instantiateの使い方
                //戻り値・・・既存オブジェクトのクローン
                //第1引数(original)・・・ コピーしたい既存オブジェクト

                // MemberWindowのPrefabからインスタンスを作成
                GameObject seedWindow = Instantiate(seedWindowPrefab, scrollViewContent);
                seedWindow.GetComponent<Button>().onClick.AddListener(farmClickManager.SeedSelectButton);
                GameObject characterWindowImage = seedWindow.transform.GetChild(0).gameObject;
                GameObject characterWindowText = seedWindow.transform.GetChild(1).gameObject;

                // キャラクターのGameObjectをCharacterWindowの子要素として設定
                SeedCharacters[i].transform.SetParent(characterWindowImage.transform);

                //キャラクター自身の画像を表示
                characterWindowImage.GetComponent<Image>().sprite = SeedCharacters[i].image.sprite;


                //キャラクター自身のステータスをテキストで表示
                characterWindowText.GetComponent<Text>().text =
                     $"Fresh:{-1 * SeedCharacters[i].fresh} \nName:\n{SeedCharacters[i].name} ";//キャラのステータスを取得Life:{ SeedCharacters[i].life } 

                // キャラクターのGameObjectを特定の位置に移動
                SeedCharacters[i].transform.localPosition = new Vector3(180, -10, 0);
                SeedCharacters[i].transform.localScale = new Vector3(250, 490, 0);
                //SeedCharacters[i].transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
