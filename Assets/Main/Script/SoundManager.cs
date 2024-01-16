using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

/// <summary>
/// 音源管理クラス
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // BGM管理
    public enum BGM_Type
    {
        // BGM用の列挙子をゲームに合わせて登録
        Bgm00Title,
        Bgm01Battle,
        Bgm02Battle,
        Bgm03Battle,
        Bgm04Battle,
        Bgm05Battle,
        Bgm06Battle,
        Bgm07Battle,
        Bgm08Battle,
        Bgm09Battle,
        Bgm10Battle,
        Bgm11Battle,
        Bgm01PartyEdit,
        Bgm02PartyEdit,
        Bgm03PartyEdit,
        Bgm04PartyEdit,
        Bgm05PartyEdit,
        Bgm06PartyEdit,
        Bgm07PartyEdit,
        Bgm08PartyEdit,
        Bgm09PartyEdit,
        Bgm10PartyEdit,
        BgmLose01,
        BgmLose02,
        BgmLose03,
        Bgm01Farm,
        Bgm02Farm,
        Bgm03Farm,
        Bgm04Farm,
        Bgm05Farm,
        fanfare01,
        fanfare02,
        MenuBgm01,




        SILENCE = 999,        // 無音状態をBGMとして作成したい場合には追加しておく。それ以外は不要
    }

    // SE管理
    public enum SE_Type
    {
        // SE用の列挙子をゲームに合わせて登録
        Se01AttackSowd,
        Se02CarrotDrillRide,
        Se03CarrotDroll,
        Se04CarrotAttack2,
        Se05PumpkinAttack,
        Se06EggplantAttack,
        Se07EggplantAttack2,
        Se08EggplantAttack3,
        Se09TomatoAttack,
        Se10TomatoAttack2,
        Se11EggplantAttack2,
        Se12DefenseUpBuff,
        Se13Debuff,
        Se14MagicAttack,
        Se15AttackUpBuff,
        Se16TomatoAttac3,
        Se17TomatoAttac4,
        Se18MagicHeal,
        Se19StrongEnemyAttack,
        Se20StrongEnemyAttack2,
        Se21WeakEnemyAttack,
        Se22WeakEnemyAttack2,
        Se23HitByaBullet,
        Se24EnemyHit,
        Se25Decision,
        Se26Decision2,
        Se27Cancel,
        Se28Cancel2,
        Se29Roll,
        Se30Choice,
        Se31PopUpDisplay,
        Se32LosePopUp,
        Se33LosePopUp2,
        Se34SkillActivation,
        Se35LevelUp,
        Se36WinPopUp,
        Se37DeathOfAnAlly,
        Se38DeathOfEnemy,
        Se39Knife,
        Se40Knife,
        Se41Earthquake,
        Se42Fire,
        Se43Heal,
        Se44Heal2,
        Se45PeachAttack,
        Se46rRotation,
        Se47TomatoFront,
        Se48PeachMiddle,
        Se49PeachBack,
        Se49Resuscitation,
        Se50PotatoMiddle,
        Se51PotatoFront,
        Se52IceMagic,
        Se53Lightning,
        Se54Lightning2,
        Se55Lightning3,
        Se56Button,
        Se57Button2,
        Se58fireworks,
        Se59flingingupandaway,
        Se60Button,
        Se61TakingPictureWithCamera,
        Se62AutoHeal,
        Se63AssaultTrumpet,
        Se64ReturnStage,
    }

    // クロスフェード時間
    public const float CROSS_FADE_TIME = 1.0f;

    // ボリューム関連
    public float BGM_Volume = 1.0f;
    public float SE_Volume = 1.0f;
    public bool Mute = false;

    // === AudioClip ===
    public AudioClip[] BGM_Clips;
    public AudioClip[] SE_Clips;

    // SE用AudioMixer  未使用
    public AudioMixer audioMixer;


    // === AudioSource ===
    private AudioSource[] BGM_Sources = new AudioSource[2];
    private AudioSource[] SE_Sources = new AudioSource[15];

    private bool isCrossFading;

    private int currentBgmIndex = 999;

    void Awake()
    {
        // シングルトンかつ、シーン遷移しても破棄されないようにする
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // BGM用 AudioSource追加
        BGM_Sources[0] = gameObject.AddComponent<AudioSource>();
        BGM_Sources[1] = gameObject.AddComponent<AudioSource>();

        // SE用 AudioSource追加
        for (int i = 0; i < SE_Sources.Length; i++)
        {
            SE_Sources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // ボリューム設定
        if (!isCrossFading)
        {
            BGM_Sources[0].volume = BGM_Volume;
            BGM_Sources[1].volume = BGM_Volume;
        }

        foreach (AudioSource source in SE_Sources)
        {
            source.volume = SE_Volume;
        }
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="bgmType"></param>
    /// <param name="loopFlg"></param>
    public void PlayBGM(BGM_Type bgmType, bool loopFlg = true)
    {
        // BGMなしの状態にする場合
        if ((int)bgmType == 999)
        {
            StopBGM();
            return;
        }

        int index = (int)bgmType;
        currentBgmIndex = index;

        if (index < 0 || BGM_Clips.Length <= index)
        {
            return;
        }

        // 同じBGMの場合は何もしない
        if (BGM_Sources[0].clip != null && BGM_Sources[0].clip == BGM_Clips[index])
        {
            return;
        }
        else if (BGM_Sources[1].clip != null && BGM_Sources[1].clip == BGM_Clips[index])
        {
            return;
        }

        // フェードでBGM開始
        if (BGM_Sources[0].clip == null && BGM_Sources[1].clip == null)
        {
            BGM_Sources[0].loop = loopFlg;
            BGM_Sources[0].clip = BGM_Clips[index];
            BGM_Sources[0].Play();
        }
        else
        {
            // クロスフェード処理
            StartCoroutine(CrossFadeChangeBMG(index, loopFlg));
        }
    }

    /// <summary>
    /// BGMのクロスフェード処理
    /// </summary>
    /// <param name="index">AudioClipの番号</param>
    /// <param name="loopFlg">ループ設定。ループしない場合だけfalse指定</param>
    /// <returns></returns>
    private IEnumerator CrossFadeChangeBMG(int index, bool loopFlg)
    {
        isCrossFading = true;
        if (BGM_Sources[0].clip != null)
        {
            // [0]が再生されている場合、[0]の音量を徐々に下げて、[1]を新しい曲として再生
            BGM_Sources[1].volume = 0;
            BGM_Sources[1].clip = BGM_Clips[index];
            BGM_Sources[1].loop = loopFlg;
            BGM_Sources[1].Play();
            BGM_Sources[1].DOFade(1.0f, CROSS_FADE_TIME).SetEase(Ease.Linear);
            BGM_Sources[0].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);

            yield return new WaitForSeconds(CROSS_FADE_TIME);
            BGM_Sources[0].Stop();
            BGM_Sources[0].clip = null;
        }
        else
        {
            // [1]が再生されている場合、[1]の音量を徐々に下げて、[0]を新しい曲として再生
            BGM_Sources[0].volume = 0;
            BGM_Sources[0].clip = BGM_Clips[index];
            BGM_Sources[0].loop = loopFlg;
            BGM_Sources[0].Play();
            BGM_Sources[0].DOFade(1.0f, CROSS_FADE_TIME).SetEase(Ease.Linear);
            BGM_Sources[1].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);

            yield return new WaitForSeconds(CROSS_FADE_TIME);
            BGM_Sources[1].Stop();
            BGM_Sources[1].clip = null;
        }
        isCrossFading = false;
    }

    /// <summary>
    /// BGM完全停止
    /// </summary>
    public void StopBGM()
    {
        BGM_Sources[0].Stop();
        BGM_Sources[1].Stop();
        BGM_Sources[0].clip = null;
        BGM_Sources[1].clip = null;
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SE_Type seType)
    {
        int index = (int)seType;
        if (index < 0 || SE_Clips.Length <= index)
        {
            return;
        }

        // 再生中ではないAudioSourceをつかってSEを鳴らす
        foreach (AudioSource source in SE_Sources)
        {

            // 再生中の AudioSource の場合には次のループ処理へ移る
            if (source.isPlaying)
            {
                continue;
            }

            // 再生中でない AudioSource に Clip をセットして SE を鳴らす
            source.clip = SE_Clips[index];
            source.Play();
            break;
        }
    }

    /// <summary>
    /// SE停止
    /// </summary>
    public void StopSE()
    {
        // 全てのSE用のAudioSourceを停止する
        foreach (AudioSource source in SE_Sources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    /// <summary>
    /// BGM一時停止
    /// </summary>
    public void MuteBGM()
    {
        BGM_Sources[0].Stop();
        BGM_Sources[1].Stop();
    }

    /// <summary>
    /// 一時停止した同じBGMを再生(再開)
    /// </summary>
    public void ResumeBGM()
    {
        BGM_Sources[0].Play();
        BGM_Sources[1].Play();
    }

    //! Buttonコンポーネントアタッチ用Method(デバック用)
    public void PlayBGMNo01()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Bgm00Title);
    }

    public void PlayBGMNo02()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Bgm01Farm);
    }

    public void PlayBGMNo03()
    {
        PlayBGM(BGM_Type.Bgm02Battle);
    }

    public void PlayBGMNo04()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Bgm03Battle);
    }

    public void PlaySE01()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se04CarrotAttack2);
    }

    //! Sliderコンポーネントアタッチ用Method
    public void SetBgmVolume(float BGMVolume)
    {
        this.BGM_Volume = BGMVolume;
    }

    public void SetSeVolume(float SEVolume)
    {
        this.SE_Volume = SEVolume;
    }

    ////* 未使用 *////

    /// <summary>
    /// AudioMixer設定
    /// </summary>
    /// <param name="vol"></param>
    public void SetAudioMixerVolume(float vol)
    {
        if (vol == 0)
        {
            audioMixer.SetFloat("volumeSE", -80);
        }
        else
        {
            audioMixer.SetFloat("volumeSE", 0);
        }
    }

    // internal class instance
    // {
    //     internal class PlayBGM
    //     {
    //     }
    // }
}