using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuicaParent : MonoBehaviour
{
    public GameObject nextPrefab;
    SuicaDirector suica;
    public bool isGameOverTrigger = false;
    public int addScoreValue;
    // Start is called before the first frame update
    void Start()
    {
        GameObject suicaDirector = GameObject.Find("SuicaDirector");
        suica = suicaDirector.GetComponent<SuicaDirector>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == gameObject.tag)
        {
            if (this.gameObject.GetInstanceID() > other.gameObject.GetInstanceID())
            {//インスタンスIDが大きいほうだけがこのメソッドを動かします。
                // 当たったGameObjectを非表示にしてQueueに戻します。
                suica.ReturnObject(this.gameObject);
                suica.ReturnObject(other.gameObject);

                //新しい場所に生成します
                suica.CreateUnionObject(nextPrefab, this.gameObject.transform.position, other.gameObject.transform.position);
                suica.Score += addScoreValue;
            }
        }
    }
}
