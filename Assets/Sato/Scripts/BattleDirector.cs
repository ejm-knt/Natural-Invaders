using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BattleDirector : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    private float characterGroundLevel = 0.9f;  //キャラクターのY座標の高さ
    Vector3 pos1;
    Vector3 pos2;
    Vector3 pos3;
    Vector3 pos4;
    Vector3 pos5;
    Vector3 pos6;
    Vector3 pos7;
    Vector3 pos8;
    Vector3 pos9;
    GameObject gameDirectorObject;
    private GameDirector gameDirector;
    [SerializeField] int turnCount = 0;

    [SerializeField] GameObject winEffect;  //勝利文字
    [SerializeField] GameObject loseEffect; //敗北文字
    [SerializeField] GameObject battleStartEffect; //出撃文字
    [SerializeField] GameObject playerActionPopup; //プレイヤーアクションポップアップ
    [SerializeField] UnityEngine.UI.Button skill1Button;
    [SerializeField] UnityEngine.UI.Button skill2Button;
    [SerializeField] UnityEngine.UI.Button skill3Button;
    List<UnityEngine.UI.Button> skillButtons = new List<UnityEngine.UI.Button>();

    public List<CharacterScript> characterScripts = new List<CharacterScript>();
    EnemyParent enemyScript;
    bool isWin = false;
    bool isLose = false;
    List<Vector3> allPositions;
    bool isSkillUsed = false;   //スキルを使ったかどうか、使ってたらtrue
    bool playerHasActed = false;    //プレイヤーが行動したかどうか
    [SerializeField] GameObject win1ParticlePrefab;
    [SerializeField] GameObject win2ParticlePrefab;
    public List<Vector3> AllPositions { get => allPositions; set => allPositions = value; }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        // POSの定義
        pos1 = new(-1, characterGroundLevel, 1);
        pos2 = new(0, characterGroundLevel, 1);
        pos3 = new(1, characterGroundLevel, 1);
        pos4 = new(-1, characterGroundLevel, 0);
        pos5 = new(0, characterGroundLevel, 0);
        pos6 = new(1, characterGroundLevel, 0);
        pos7 = new(-1, characterGroundLevel, -1);
        pos8 = new(0, characterGroundLevel, -1);
        pos9 = new(1, characterGroundLevel, -1);
        // GameDirectorの取得
        gameDirectorObject = GameObject.Find("GameDirector");
        gameDirector = gameDirectorObject.GetComponent<GameDirector>();
        // タイルのポジションをリスト化
        AllPositions = new List<Vector3> { pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9 };
        //キャラクター1～5に場所を割り当てる
        foreach (GameObject character in gameDirector.PartyMembers)
        {   //パーティーメンバーの初期配置
            CharacterScript cs = character.GetComponent<CharacterScript>();
            characterScripts.Add(cs);
            character.transform.position = AllPositions[cs.position - 1];
            character.transform.localRotation = Quaternion.Euler(-90, 0, 180);
        }
        // ボタンリストに追加
        skillButtons.Add(skill1Button);
        skillButtons.Add(skill2Button);
        skillButtons.Add(skill3Button);
        // enemyにenemyを割り当てる
        enemy = GameObject.FindWithTag("Enemy");
        // enemyという名前のスクリプトをenemyScriptへ代入
        enemyScript = enemy.GetComponent<EnemyParent>();
        // enemyにパーティリストを送る
        enemyScript.characters = characterScripts;
        // PartyにEnemyを送る
        foreach (CharacterScript cs in characterScripts)
        {
            cs.GetEnemyParent();
        }
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Bgm03Battle);
        // バトルの開始
        yield return StartCoroutine(BattleStart());
        StartCoroutine(BattleMainLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // RotateFormation();
        }
    }

    IEnumerator BattleMainLoop()   //バトルの基本ループ
    {
        while (isWin == false && isLose == false)
        {
            turnCount++;
            yield return new WaitForSeconds(0.3f);
            //味方のターン        
            yield return StartCoroutine(ActionPlayerTurn());
            if (isWin || isLose)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
            //敵のターン
            yield return StartCoroutine(ActionEnemyTurn());
            Judge();
            //3列目回復
            yield return StartCoroutine(BackLineHeal());
            if (!(isWin == false && isLose == false))
            {
                break;
            }
            yield return new WaitForSeconds(1.0f);
            yield return StartCoroutine(PlayerAction());
        }
    }

    IEnumerator ActionPlayerTurn()
    {
        foreach (CharacterScript cs in characterScripts)
        {
            if (cs.position <= 3 && cs.isDead == false)
            {
                // yield return StartCoroutine(cs.FrontAction());
                cs.FrontAction();
                yield return new WaitForSeconds(1.5f);
                Judge();
            }
        }
        foreach (CharacterScript cs in characterScripts)
        {
            if (cs.position >= 4 && cs.position <= 6 && cs.isDead == false)
            {
                // yield return StartCoroutine(cs.FrontAction());
                cs.MiddleAction();
                yield return new WaitForSeconds(1.5f);
                Judge();
            }
        }
        foreach (CharacterScript cs in characterScripts)
        {
            if (cs.position >= 7 && cs.position <= 9 && cs.isDead == false)
            {
                // yield return StartCoroutine(cs.FrontAction());
                cs.BackAction();
                yield return new WaitForSeconds(1.5f);
                Judge();
                yield return null;
            }
        }
        // RaycastHit hit;
        // foreach (Vector3 pos in allPositions)   //全部のタイルから
        // {
        //     Vector3 groundPos = new Vector3(pos.x, 0, pos.z); // y座標を0に設定
        //     if (Physics.Raycast(groundPos, Vector3.up, out hit))
        //     {
        //         CharacterScript characterScript = hit.transform.GetComponent<CharacterScript>();
        //         if (characterScript != null)
        //         {
        //             // posのz座標(Line)によってアクションを決定
        //             if (pos.z == 1)
        //             {
        //                 characterScript.FrontAction();
        //             }
        //             else if (pos.z == 0)
        //             {
        //                 characterScript.MiddleAction();
        //             }
        //             else // pos.z == -1
        //             {
        //                 characterScript.BackAction();
        //             }
        //             Judge();    //生死判定
        //             if (isWin || isLose) // 勝利または敗北が確定した場合
        //             {
        //                 break; // ループを抜ける
        //             }
        //             yield return new WaitForSeconds(0.2f);  //0.2秒待って
        //         }
        //     }
        // }
    }

    IEnumerator BackLineHeal()  //3列目の回復行動
    {
        foreach (CharacterScript cs in characterScripts)
        {
            if (cs.position >= 7 && cs.position <= 9 && cs.isDead == false)
            {
                cs.AutoHeal();
                yield return new WaitForSeconds(0.2f);  //0.2秒待って
            }
        }
        // RaycastHit hit;
        // foreach (Vector3 pos in allPositions)
        // {
        //     if (pos.z == -1)    //最後列かどうか
        //     {
        //         Vector3 groundPos = new Vector3(pos.x, 0, pos.z); // y座標を0に設定
        //         if (Physics.Raycast(groundPos, Vector3.up, out hit))
        //         {
        //             CharacterScript characterScript = hit.transform.GetComponent<CharacterScript>();
        //             if (characterScript != null)    //そこにキャラクターがいるなら
        //             {
        //                 characterScript.AutoHeal();
        //                 yield return new WaitForSeconds(0.2f);  //0.2秒待って
        //             }
        //         }
        //     }
        // }
    }


    IEnumerator ActionEnemyTurn()
    {
        enemyScript.EnemyAction(turnCount);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator BattleStart()
    {
        //バトルスタート演出
        battleStartEffect.SetActive(true);
        // n秒待つ
        yield return new WaitForSeconds(2);
        // n秒後にSetActiveをfalseにする
        battleStartEffect.SetActive(false);
    }

    void Judge()    //生死判定
    {
        if (enemyScript.EnemyLife <= 0)  //敵のライフが0なら即勝利
        {
            isWin = true;
            enemyScript.SetDeathAnime();
            EndingStage();
        }
        else
        {
            int deathCount = 0;
            foreach (CharacterScript cs in characterScripts)
            {
                if (cs.CharacterLife <= 0)
                {
                    cs.Death();
                    deathCount++;
                }
            }
            if (deathCount >= characterScripts.Count)  //もし全員Deadなら負け、characterScriptsがPTMと同じ要素数であることが前提条件
            {
                isLose = true;
                EndingStage();
            }
        }
    }

    void EndingStage()  //ステージエンド用
    {
        //タグの削除と
        //すべてのキャラクタースクリプトのPosを0に初期化する処理を行う
        if (isWin == true)
        {
            winEffect.SetActive(true);
            GameObject win1Particle = Instantiate(win1ParticlePrefab);
            GameObject win2Particle = Instantiate(win1ParticlePrefab);
            win1Particle.transform.position = new Vector3(-2, 5, 2); // 再生位置を設定
            win2Particle.transform.position = new Vector3(2, 5, 2); // 再生位置を設定
            ParticleSystem win1ParticleSystem = win1Particle.GetComponent<ParticleSystem>();
            ParticleSystem win2ParticleSystem = win2Particle.GetComponent<ParticleSystem>();
            win1ParticleSystem.Play();
            win2ParticleSystem.Play();
            SoundManager.instance.PlayBGM(SoundManager.BGM_Type.fanfare02);
        }
        if (isLose == true)
        {
            loseEffect.SetActive(true);
        }
        GameDirector.Instance.NextFreshCount();
    }

    IEnumerator PlayerAction()
    {
        playerActionPopup.SetActive(true);
        SkillButtonCheck();
        // プレイヤーが行動を選択するまで待つ
        while (!playerHasActed)
        {
            yield return null;
        }
        // プレイヤーが行動を選択した後の処理
        playerActionPopup.SetActive(false);
        playerHasActed = false;
    }

    void SkillButtonCheck()
    {
        foreach (UnityEngine.UI.Button button in skillButtons)
        {
            if (isSkillUsed)
            {
                Color color = button.image.color;
                color.a = 0.8f; // 透明度を80%に設定
                button.image.color = color;
                button.interactable = false; // ボタンを押下不可能に設定
            }
            else
            {
                Color color = button.image.color;
                color.a = 1f; // 透明度を100%に設定
                button.image.color = color;
                button.interactable = true; // ボタンを押下可能に設定
            }
        }
    }


    IEnumerator MoveOverTime(GameObject character, Vector3 targetPosition, float duration, float jumpHeight)
    {
        Vector3 startPosition = character.transform.position;
        // スタートポジションとエンドポジションの中間地点を計算
        Vector3 midPosition = (startPosition + targetPosition) / 2;
        // Y座標の高さが最大値になるようにピークポジションを設定
        Vector3 peakPosition = midPosition + new Vector3(0, jumpHeight, 0);
        Vector3 endPosition = targetPosition;
        endPosition.y = characterGroundLevel;
        float elapsed = 0;  //日本語で経過時間
        // durationは日本語で間隔、経過時間が指定した時間の半分に満ちたら走るwhile文が変わるよ
        // 上昇
        while (elapsed < duration / 2)
        {
            character.transform.position = Vector3.Lerp(startPosition, peakPosition, elapsed / (duration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0;
        // 下降
        while (elapsed < duration / 2)
        {
            character.transform.position = Vector3.Lerp(peakPosition, targetPosition, elapsed / (duration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        character.transform.position = endPosition;
    }
    public void RotateFormation()  //ポジションロール
    {
        foreach (GameObject Character in gameDirector.PartyMembers)
        {
            CharacterScript cs = Character.GetComponent<CharacterScript>();
            cs.position = ((cs.position + 2) % 9) + 1;  //characterscriptのposition更新
            float posz = AllPositions[cs.position - 1].z;   //allPositionのz座標
            //前列が中列へ移動～後列は前列へ
            Vector3 targetPosition = new Vector3(Character.transform.position.x, Character.transform.position.y, posz);
            float jumpHeight = (posz == 1) ? 0.8f : 0.3f;   //左が最後列から前へ飛ぶ高さ、右がその他の列の高さ posz1は位置番手前の列
            StartCoroutine(MoveOverTime(Character, targetPosition, 0.2f, jumpHeight));
            SoundManager.instance.PlaySE(SoundManager.SE_Type.Se34SkillActivation);
        }
    }

    public void playerActed()
    {
        playerHasActed = true;
    }
    public void OnButtonClickSkill1()
    {
        StartCoroutine(Skill1DmgEnemy());
        isSkillUsed = true;
        playerActed();
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
    }
    IEnumerator Skill1DmgEnemy()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
        int dmg = (int)(enemyScript.EnemyLife * 0.1);
        if (dmg <= 0) { dmg = 1; };
        enemyScript.EnemyLife -= dmg;
        //何かしらのエフェクト、効果音
        yield return new WaitForSeconds(0.5f); //エフェクトや効果音の再生時間
    }

    public void OnButtonClickSkill2()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
        StartCoroutine(Skill2Resurrection());
        isSkillUsed = true;
        playerActed();
    }
    IEnumerator Skill2Resurrection()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
        foreach (CharacterScript cs in characterScripts)
        {
            if (cs.isDead == true)
            {
                cs.ResurrectionHP1();
            }
        }
        yield return new WaitForSeconds(0.5f); //エフェクトや効果音の再生時間
    }

    public void OnButtonClickSkill3()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
        StartCoroutine(Skill3AutoHealAll());
        isSkillUsed = true;
        playerActed();
    }
    IEnumerator Skill3AutoHealAll()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
        foreach (CharacterScript cs in characterScripts)
        {
            cs.AutoHeal();
            yield return new WaitForSeconds(0.1f);
        }
        // RaycastHit hit;
        // foreach (Vector3 pos in AllPositions)
        // {
        //     Vector3 groundPos = new Vector3(pos.x, 0, pos.z); // y座標を0に設定
        //     if (Physics.Raycast(groundPos, Vector3.up, out hit))
        //     {
        //         CharacterScript characterScript = hit.transform.GetComponent<CharacterScript>();
        //         if (characterScript != null)    //そこにキャラクターがいるなら
        //         {
        //             characterScript.AutoHeal();
        //             yield return new WaitForSeconds(0.1f);  //0.2秒待って
        //         }
        //     }
        // }
    }

    public void onButtonToStageSelect()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
        //PTMをdontDestroyしてfalseにする
        SetTransitionPtm();
        SceneManager.LoadScene("SelectStageScene");
    }
    public void onButtonToFarm()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
        //PTMをdontDestroyしてfalseにする
        SetTransitionPtm();
        SceneManager.LoadScene("farm");
    }

    public void SetTransitionPtm()
    {
        foreach (GameObject ptm in GameDirector.Instance.PartyMembers)
        {
            DontDestroyOnLoad(ptm);
            ptm.SetActive(false);
        }
    }
}