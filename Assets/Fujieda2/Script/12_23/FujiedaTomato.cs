using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FujiedaTomato : CharacterScript
{
    public FujiedaTomato() : base("Tomato", 16, 5, 0, 10, 0, 10)
    {
        // 親クラス(CharacterScript)のコンストラクタを呼び出す
        // ここで追加の初期化などを行うこともできます
    }

    public override void FrontAction()
    {
        base.FrontAction();
        // 固有キャラの前列行動の処理を追加
    }
    private void FrontActionSound()
    {
        // サウンド再生のロジック
        // audioSource.PlayOneShot(sampleSound);  //固有キャラのAudioClip
    }
    private void FrontActionEffect()
    {
        // エフェクトプレハブの生成と再生
        // GameObject effectInstance = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
        // Destroy(effectInstance, particleSystem.main.duration);  // エフェクトが終了したら削除
    }

    public override void MiddleAction()
    {
        base.MiddleAction();
        // Potato独自の中列行動の処理を追加
    }
    public override void BackAction()
    {
        base.BackAction();
        // Potato独自の後列行動の処理を追加
    }

    public override void AutoHeal()
    {
        base.AutoHeal();
    }


    private void MiddleActionSound()
    {
        // サウンド再生のロジック
        // audioSource.PlayOneShot(sampleSound);  // 固有キャラのAudioClip
    }
    // private void MiddleActionEffect()
    // {
    //     // エフェクトプレハブの生成と再生
    //     GameObject effectInstance = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
    //     Destroy(effectInstance, particleSystem.main.duration);  // エフェクトが終了したら削除
    // }

    private void BackActionSound()
    {
        // サウンド再生のロジック
        // audioSource.PlayOneShot(sampleSound);  // 固有キャラのAudioClip
    }
    private void BackActionnEffect()
    {
        // エフェクトプレハブの生成と再生
        // GameObject effectInstance = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
        // Destroy(effectInstance, particleSystem.main.duration);  // エフェクトが終了したら削除
    }

    public override void Death()
    {
        base.Death();
        // キャラの生死判定
    }

    //Start is called before the first frame update
    void Start()
    {
        //main統合用にコメントアウト、ウネの判定に持たせる
        if (isPlanted == true)
        {
            Button btn = GetComponent<Button>();
            btn.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //main統合用にコメントアウト、ウネの判定に持たせる
        if (isPlanted == true)
        {
            Button btn = GetComponent<Button>();
            btn.interactable = false;
        }

    }
}
