using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;



public class CharacterScript : MonoBehaviour
{
    public GameObject enemyObject;
    public EnemyParent enemy;
    protected string characterName; // キャラクター名
    protected int characterLife; // キャラクターのHP
    protected int characterAtk; // キャラクタの攻撃力
    protected int characterMatk; // キャラクターの中列攻撃力
    protected int characterSpd; // キャラクターの素早さ
    protected int characterHeal; // キャラクターの回復力
    protected int characterAaux; // キャラクターの攻撃補助力
    protected int characterDaux; // キャラクターの防御補助力
    protected int characterPosition; // キャラクターの場所指定
    protected int characterDef = 0; //キャラクターの防御力 基本0、敵キャラクターのダメージ計算用
    public int position = 0; // キャラクターの現在位置
    public bool isDead = false; // キャラクターの死亡判定
    protected int enemyLife;  // enemyLifeをCharacterScriptのフィールドとして追加
    protected int maxCharacterLife;
    private GameObject effectManager;   //エフェクトマネージャー
    public Image image;
    public bool isPlanted = false;
    public int fresh = -5;

    private AudioSource audioSource;  // AudioSourceコンポーネント
    public GameObject attackEffectPrefab;  // 前列攻撃のエフェクトプレハブ
    public AudioClip attackSound;

    public int CharacterLife
    {
        get { return characterLife; }
        set
        {
            // 任意の制御や処理を追加できます
            characterLife = Mathf.Clamp(value, 0, MaxCharacterLife);
            if (OnLifeChanged != null)
            {
                OnLifeChanged.Invoke(); //着火しまーす！
            }
        }
    }
    public int MaxCharacterLife
    {
        get { return maxCharacterLife; }
        set { maxCharacterLife = value; }
    }

    public int CharacterDef { get => characterDef; set => characterDef = value; }
    public int CharacterAtk { get => characterAtk; set => characterAtk = value; }
    public int CharacterMatk { get => characterMatk; set => characterMatk = value; }
    public int CharacterAaux { get => characterAaux; set => characterAaux = value; }
    public int CharacterDaux { get => characterDaux; set => characterDaux = value; }

    public delegate void OnLifeChangedDelegate();
    public event OnLifeChangedDelegate OnLifeChanged;

    public CharacterScript(
        string name = "DefaultName",
        int life = 50,
        int atk = 5,
        int spd = 5,
        int heal = 5,
        int aaux = 5,
        int daux = 5,
        int matk = 5)
    {
        characterName = name;
        characterLife = life;
        maxCharacterLife = life;
        CharacterAtk = atk;
        characterSpd = spd;
        characterHeal = heal;
        characterAaux = aaux;
        characterDaux = daux;
        CharacterMatk = matk;
    }




    void Start()
    {
        // DontDestroyOnLoad(gameObject);
        // audioSource = GetComponent<AudioSource>();
        // particleSystem = GetComponent<ParticleSystem>();
    }

    public void GetEnemyParent()
    {
        enemyObject = GameObject.FindWithTag("Enemy");
        enemy = enemyObject.GetComponent<EnemyParent>();
    }

    public void DisplayCharacterInfo() // キャラクターの共通の何かを追加することがあれば。
    {
        Debug.Log($"Character Name: {characterName}, Life: {characterLife}");
    }

    // public virtual IEnumerator FrontAction() 要修正
    public virtual void FrontAction()
    {

        // int enemyLife = enemy.enemyLife;
        // int enemyLifeDecrease = CharacterAtk;
        // enemyLife -= enemyLifeDecrease;  //enemyLifeを減少させる
        enemy.EnemyLife -= this.characterAtk;
        Debug.Log("前列攻撃");
        // FrontActionSound(); // サウンド再生
        // FrontActionEffect(); // エフェクト再生
        // yield return new WaitForSeconds(0.5f);要修正
        //dmg 計算
    }

    public virtual void MiddleAction()
    {
        // int enemyLife = enemy.enemyLife;
        // int enemyLifeDecrease = CharacterMatk;
        // enemyLife -= enemyLifeDecrease;  // enemyLifeを減少させる
        // Debug.Log("中列攻撃");

        // MiddleActionSound(); // サウンド再生の呼び出し
        // MiddleActionEffect(); // エフェクト再生
    }


    public virtual void BackAction()
    {

        // // characterHealの値をcharacterLifeに追加する
        // int addedLife = characterLife + characterHeal;

        // // 最大値を超えないように調整
        // characterLife = Mathf.Clamp(addedLife, 0, maxCharacterLife);

        // // ここでcharacterLifeの値が更新された状態
        // Debug.Log("後列行動");
        // Debug.Log($"Character Life after BackAction: {characterLife}");

        // BackActionSound(); // サウンド再生の呼び出し
        // BackActionnEffect(); // エフェクト再生

    }

    public virtual void ResurrectionHP1()
    {
        isDead = false;
        characterLife = 1;
        Debug.Log("蘇生した");
        gameObject.SetActive(true);
    }

    public virtual void AutoHeal()
    {
        if (effectManager == null)
        {
            GetEffectManager();
        }
        PlayableDirector[] effects = effectManager.GetComponentsInChildren<PlayableDirector>();
        int gainLife = (int)(characterLife * 0.2);
        CharacterLife += gainLife;
        foreach (PlayableDirector effect in effects)
        {
            if (effect.state != PlayState.Playing)
            {
                Vector3 pos = this.gameObject.transform.position;
                pos.y -= 0.5f;
                effect.gameObject.transform.position = pos;
                effect.Play();
                break;
            }
        }
        WaitSeconds(1.5f);
        //SoundDirectorから再生
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se61TakingPictureWithCamera);
        Debug.Log($"{gainLife}回復した");
    }

    public void ModifyCharacterLife(int amount)
    {
        characterLife -= amount;
        // characterLife が 0 以下になった場合、isDead を false に設定
        if (characterLife <= 0)
        {
            characterLife = 0;
            isDead = false;
        }
    }

    public virtual void Death()
    {
        if (!isDead)
        {
            isDead = true;
            Debug.Log("死亡した");

            // キャラクターが死亡した際の処理
            // DeathSound();
            // DeathEffect();

        }
    }

    // public void SeedSelectButton()
    // {
    //     Debug.Log("SeedSelectbutton");//動作確認用
    //     FarmClickManager.SetSelectSeed(this);//ClickManagerクラスのSetSelectSeed()へインスタンスを渡す
    // }

    private void DeathSound()
    {
        // サウンド再生のロジック
        // audioSource.PlayOneShot(sampleSound);  // 固有キャラのAudioClip
    }
    // private void DeathEffect()
    // {
    //     // エフェクトプレハブの生成と再生
    //     GameObject effectInstance = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
    //     Destroy(effectInstance, particleSystem.main.duration);  // エフェクトが終了したら削除
    // }


    // public void SetEnemyLife(int life)
    // {
    //     enemyLife = life;
    // }
    private void GetEffectManager()
    {
        effectManager = GameObject.Find("EffectManager");
    }
    public IEnumerable WaitSeconds(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
