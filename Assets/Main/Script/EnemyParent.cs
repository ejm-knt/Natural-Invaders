using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum EnemyKind
{
    //! 職業Enum
    ant,        //! アリ
    mantis,     //! カマキリ
    bee,        //! ハチ
    beetle,     //! カブトムシ
}

public abstract class EnemyParent : MonoBehaviour
{
    //! Enemyステータス群
    private string enemyName;   //! 名前
    private int enemyAttack;    //! 攻撃力
    private int enemyLife;      //! 現在HP
    private int enemyMaxLife;   //! 最大HP(超過回復防止用)
    [SerializeField] private GameObject atkPrefab;    //! 攻撃ParticleSystem
    [SerializeField] private GameObject healPrefab;   //! 回復ParticleSystem
    [SerializeField] private GameObject chargePrefab; //! 「溜める」ParticleSystem
    public List<CharacterScript> characters = new();

    //! Enemy詳細ステータス群
    private bool enemyChargeFlag = false;           //!「溜める」管理フラグ
    private int enemyChargeMagnification = 2;       //!「溜める」倍率
    [SerializeField] private int enemyHealValue;    //! 回復値

    //! 各getter,setter
    public string EnemyName { get => enemyName; set => enemyName = value; }
    public int EnemyAttack { get => enemyAttack; set => enemyAttack = value; }
    public int EnemyMaxLife { get => enemyMaxLife; set => enemyMaxLife = value; }
    public int EnemyChargeMagnification { get => enemyChargeMagnification; set => enemyChargeMagnification = value; }
    public int EnemyHealValue { get => enemyHealValue; set => enemyHealValue = value; }
    public int EnemyLife
    {
        get => enemyLife;
        set
        {
            enemyLife = value;
            if (OnLifeChanged != null)
            {
                OnLifeChanged.Invoke(); //着火しまーす！
            }
        }
    }
    GameObject atkObject;
    GameObject chargeObject;
    GameObject healObject;
    private void Start()
    {
        atkObject = Instantiate(atkPrefab);
        chargeObject = Instantiate(chargePrefab);
        healObject = Instantiate(healPrefab);
    }
    //! HPバーアニメーション同期
    public delegate void OnLifeChangedDelegate();
    public event OnLifeChangedDelegate OnLifeChanged;

    //! Enemy初期化時のステータス管理
    public void Initialize(EnemyKind enemyKind, int enemyHealValue)
    {
        switch (enemyKind)
        {
            case EnemyKind.ant:
                EnemyName = "ant";
                EnemyLife = 500;
                EnemyMaxLife = EnemyLife;
                EnemyAttack = 100;
                EnemyHealValue = enemyHealValue;
                break;

            case EnemyKind.mantis:
                EnemyName = "mantis";
                EnemyLife = 1000;
                EnemyMaxLife = EnemyLife;
                EnemyAttack = 200;
                EnemyHealValue = enemyHealValue;
                break;

            case EnemyKind.bee:
                EnemyName = "bee";
                EnemyLife = 1500;
                EnemyMaxLife = EnemyLife;
                EnemyAttack = 300;
                EnemyHealValue = enemyHealValue;
                break;

            case EnemyKind.beetle:
                EnemyName = "beetle";
                EnemyLife = 20;
                EnemyMaxLife = EnemyLife;
                EnemyAttack = 4;
                EnemyHealValue = enemyHealValue;
                break;

            default:
                Debug.Log("その敵は存在しない");
                break;
        }
    }

    //! 「行動パターン」分岐 -> 各クラスに記述を投げる
    public abstract void EnemyAction(int turnCount);

    //! 「溜める」行動
    public void EnemyCharge()
    {
        chargeObject.transform.position = transform.position;
        ParticleSystem ch = chargeObject.GetComponent<ParticleSystem>();
        ch.Play(); //* 溜めるEffect再生
        EnemyAttack *= EnemyChargeMagnification; //* 2倍Atk
        enemyChargeFlag = true;
    }

    //! 「溜める」解除_ヘルパー関数
    private void EnemyChargeInvalid()
    {
        ParticleSystem ch = chargeObject.GetComponent<ParticleSystem>();
        ch.Stop(); //* 溜めるEffect再生
        EnemyAttack /= EnemyChargeMagnification; //* 2倍Atkを1倍に戻す
        enemyChargeFlag = false;
    }

    //! 「回復」行動
    public void EnemyHeal()
    {
        healObject.transform.position = transform.position;
        ParticleSystem heal = healObject.GetComponent<ParticleSystem>();
        heal.Play(); //* 回復Effect再生
        int tempHpHeal = EnemyLife + EnemyHealValue;
        if (tempHpHeal > EnemyMaxLife)
        {   //! 回復超過。HPMAXLifeを代入
            EnemyLife = EnemyMaxLife;
        }
        else
        {   //! 回復未超過。HPに回復量を加算
            EnemyLife += EnemyHealValue;
        }
    }

    //!「最前列ランダム単体攻撃」行動
    public void EnemySingleAttack(List<CharacterScript> characters)
    {
        List<CharacterScript> targetGroup = SelectTargetGroups(characters);   //! ScriptListから特定列の配列を新規作成
        CharacterScript targetCharacter = SelectCharacterFromRow(targetGroup);//! 特定列の配列から単一のScriptを選択
        atkObject.transform.position = targetCharacter.transform.position;
        ParticleSystem atk = atkObject.GetComponent<ParticleSystem>();
        // testatk.transform.position = targetCharacter.transform.position;
        atk.Play();
        // atkParticleObject.transform.position = targetCharacter.transform.position;
        // atkParticleObject.Play();
        targetCharacter.CharacterLife -= enemyAttack - targetCharacter.CharacterDef;

        //? 攻撃行動後にEnemyが「溜める」状態だった場合、「溜める」解除。
        if (enemyChargeFlag == true)
        {
            EnemyChargeInvalid();
        }
    }

    public IEnumerator EnemyGroupAttack(List<CharacterScript> characters)
    {
        List<CharacterScript> targetGroup = SelectTargetGroups(characters);

        foreach (CharacterScript Group in targetGroup)
        {
            Group.CharacterLife -= enemyAttack - Group.CharacterDef; //* ダメージ計算処理

            atkObject.transform.position = Group.transform.position; //* Effect発生位置確定
            ParticleSystem atk = atkObject.GetComponent<ParticleSystem>();
            atk.Play();
            yield return new WaitUntil(() => atk.isStopped);
        }

        //? 攻撃行動後にEnemyが「溜める」状態だった場合、「溜める」解除。
        if (enemyChargeFlag == true)
        {
            EnemyChargeInvalid();
        }
    }

    //! 全キャラクターのリストから最前列のみのリストを新規作成する関数
    public List<CharacterScript> SelectTargetGroups(List<CharacterScript> characters)
    {
        //? 前列にいるキャラクターだけを含む新しいリストを作成
        List<CharacterScript> Groups = characters.Where(c => c.position <= 3).ToList();
        if (Groups.Count == 0)
        {
            //? 中列にいるキャラクターだけを含む新しいリストを作成
            Groups = characters.Where(c => c.position >= 4 && c.position <= 6).ToList();
        }
        if (Groups.Count == 0)
        {
            //? 後列にいるキャラクターだけを含む新しいリストを作成
            Groups = characters.Where(c => c.position >= 7 && c.position <= 9).ToList();
        }
        return Groups; //* toListで生成したいずれかのListを返す
    }

    //! 最前列内のランダムなキャラクタークラスを返す関数_ヘルパー関数
    private CharacterScript SelectCharacterFromRow(List<CharacterScript> Groups)
    {
        int minPosition = Groups.Min(c => c.position); //* 最前列リスト内の最小値を取得
        int maxPosition = Groups.Max(c => c.position); //* 最前列リスト内の最大値を取得
        while (true)
        {
            int randCharacterPosition = UnityEngine.Random.Range(minPosition, maxPosition + 1); //* 最小値と最大値の間で乱数を生成
            foreach (CharacterScript character in Groups)
            {
                if (character.position == randCharacterPosition) //* 生成した乱数がキャラクターの位置と一致する場合
                {
                    return character; //* 合致したキャラクタークラスを返す
                }
            }
        }
    }

    public void SetDeathAnime()
    {
        Animator anime = gameObject.GetComponent<Animator>();
        anime.SetBool("death", true);
    }
}