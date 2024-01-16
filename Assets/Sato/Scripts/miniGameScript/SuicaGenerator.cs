using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuicaGenerator : MonoBehaviour
{
    [SerializeField] SuicaDirector suicaDirector;
    [SerializeField] public GameObject[] previewImages;
    [SerializeField] public GameObject[] prefabs;
    private GameObject nextObject;
    private GameObject nextImage;
    [SerializeField] GameObject crane;
    private int num;    //ランダム用数値
    [SerializeField] private float height;  //クレーンからの高さ

    public void SetNextObject()
    {   //プレビューにランダムイメージをセット
        num = Random.Range(0, previewImages.Length);
        nextImage = previewImages[num];
        nextImage.SetActive(true);
    }

    public void SetObjectToCacther()
    {   //クレーンにObjectを作成して配置。numをもとに生成
        nextObject = suicaDirector.GetObject(prefabs[num]);
        nextObject.transform.SetParent(crane.transform);
        nextObject.transform.localPosition = new Vector2(0, height);
        Rigidbody2D rb = nextObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
        }
        // Debug.Log(nextObject.transform.position);
        nextImage.SetActive(false); //nextImageを非表示にして
        SetNextObject();    //新しいのを作成
    }


}
