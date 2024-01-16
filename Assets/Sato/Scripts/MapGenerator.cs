using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements.Experimental;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject waterTile;
    [SerializeField] GameObject grassTile;
    [SerializeField] GameObject dirtTile;
    const int mapHeigt = 3; //縦何マス
    const int mapWidth = 3; //横何マス
    const int mapSize = 1;  //1つのマップの大きさ
    const int waterRatio = 20;
    const int grassRatio = 70;
    const int dirtRatio = 10;
    List<GameObject> tiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int ratioTotal = 0; //合計確率 100分率でなくても可、でもわかりやすさ大事
        Dictionary<GameObject, int> tileSeeds = new Dictionary<GameObject, int>()   //確率とタイル種類をDictionary化
        {
            { waterTile, waterRatio },
            { grassTile, grassRatio },
            { dirtTile, dirtRatio }
        };
        foreach (KeyValuePair<GameObject, int> kvp in tileSeeds)    //確率合計値出してます
        {
            ratioTotal += kvp.Value;
        }
        float offsetX = mapHeigt / 2.0f - mapSize / 2.0f;  //マップ開始位置の調整
        float offsetZ = mapWidth / 2.0f - mapSize / 2.0f;
        for (int i = 0; i < mapHeigt; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                int flagNum = Random.Range(1, ratioTotal + 1);  //ランダム数値の生成
                foreach (KeyValuePair<GameObject, int> kvp in tileSeeds)
                {
                    if (flagNum <= kvp.Value)   //フラグが立ったらそのブロックを生成
                    {
                        GameObject tile = Instantiate(kvp.Key, new Vector3(i - offsetX, 0, j - offsetZ), Quaternion.identity);
                        tiles.Add(tile);
                        break;
                    }
                    else    //立たなかったら次の配列へループ。ランダム数値を調整して次のループへ
                    {
                        flagNum -= kvp.Value;
                    }
                }
            }
        }
    }

    public void DontDestroyTile()
    {
        foreach (GameObject tile in tiles)
        {
            DontDestroyOnLoad(tile);
        }
    }
}
