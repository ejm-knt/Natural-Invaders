using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuicaDirector : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private int poolSize = 3;
    [SerializeField] GameObject crane;
    [SerializeField] SuicaGenerator suicaGenerator;
    private bool isAlive = true;
    SuicaParent suicaParent;
    [SerializeField] Text scoreText;
    private int score;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            // スコアが変更されたときにscoreTextを更新
            scoreText.text = "Score\n" + score.ToString();
        }
    }
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();   //poolの辞書型を作成
        foreach (var prefab in prefabs) //prefabsの配列分新しいPoolを作成
        {
            var objectPool = new Queue<GameObject>(poolSize);
            InitialPool(objectPool, prefab);
            poolDictionary.Add(prefab.name, objectPool); //作ったペアをpoolDictionaryへ
        }

        StartCoroutine(MainLoop());
        //BGMの再生
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Bgm01Farm);
    }
    public void InitialPool(Queue<GameObject> queue, GameObject prefab)
    {
        for (int i = 0; i < poolSize; i++)//プールのサイズだけ作成し不可視にしてQueueへ保存
        {
            GameObject obj = Instantiate(prefab);
            obj.name = prefab.name;
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    public void CreateUnionObject(GameObject prefab, Vector2 pos1, Vector2 pos2)
    {
        StartCoroutine(CreateUnion(prefab, pos1, pos2));
    }
    IEnumerator CreateUnion(GameObject prefab, Vector2 pos1, Vector2 pos2)
    {
        yield return new WaitForSeconds(0.1f);
        // 新しいGameObjectを取得して表示します。
        GameObject obj = GetObject(prefab);
        // 新しいGameObjectの位置を二つのGameObjectの中間点に設定します。
        obj.transform.position = (pos1 + pos2) / 2;
        SuicaParent objScript = obj.GetComponent<SuicaParent>();
        objScript.isGameOverTrigger = true;
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
    }

    public GameObject GetObject(GameObject prefab)
    {//Queueから可視化して取り出す。もし足りなければ生成してそのObjectをreturnする
        if (poolDictionary.TryGetValue(prefab.name, out var queue))
        {//引数のGameObjectからPoolのqueueを取得、なければnull
            if (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject newObj = Instantiate(prefab);
                newObj.name = prefab.name;
                return newObj;
            }
        }
        return null;
    }

    public void ReturnObject(GameObject obj)
    {//引数のGameObjectを不可視にしてQueueへ戻す
        if (poolDictionary.TryGetValue(obj.name, out var queue))
        {//objのpollを探して、それがあれば不可視にして戻す。
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    IEnumerator MainLoop()
    {   //メインループ、オブジェクトの作成と配置を繰り返す
        suicaGenerator.SetNextObject(); //最初の設定
        while (isAlive)
        {
            //プレイヤーが生きている間、かつ、Craneの子要素にSuicaParentのあるGameObjectが存在しない場合
            suicaParent = crane.GetComponentInChildren<SuicaParent>();
            if (suicaParent == null)
            {
                suicaGenerator.SetObjectToCacther();
                suicaGenerator.SetNextObject();
            }
            //プレイヤーが敗北するか、SuicaParentがNullになったら再度ループを行う
            yield return new WaitUntil(() => suicaParent == null || !isAlive);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StartFallObject()
    {   //親要素から切り離し
        suicaParent = crane.GetComponentInChildren<SuicaParent>();
        if (suicaParent != null)
        {
            suicaParent.transform.SetParent(null);
            Rigidbody2D rb = suicaParent.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1.5f;
            suicaParent = null;
        }
    }

    public void OnBackButton()
    {
        SceneManager.LoadScene("SelectStageScene");
    }
}
