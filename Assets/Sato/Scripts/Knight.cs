// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Knight : CharacterScript
// {   //grapeに後々書き換える
//     GameObject enemyObject;     //Enemyオブジェクト
//     Enemy1 enemyScript;         //EnemyScript 要更新
//     [SerializeField] GameObject frontEffectPrefab;  //前列Action
//     [SerializeField] GameObject backEffectPrefab;   //後列Action
//     ParticleSystem frontEffect;
//     ParticleSystem backEffect;

//     // Start is called before the first frame update
//     void Start()
//     {
//         MaxCharacterLife = 30;
//         CharacterLife = 30;
//         // enemyObject = GameObject.FindWithTag("Enemy");
//         // enemyScript = enemyObject.GetComponent<Enemy1>();
//         // // 前列Actionのインスタンスを作成
//         // GameObject frontEffectInstance = Instantiate(frontEffectPrefab);
//         // ParticleSystem frontEffect = frontEffectInstance.GetComponent<ParticleSystem>();
//         // GameObject backEffectInstance = Instantiate(backEffectPrefab);
//         // ParticleSystem backEffect = frontEffectInstance.GetComponent<ParticleSystem>();
//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }

//     public override void FrontAction()
//     {
//         // frontEffect.Play();
//         base.FrontAction();
//     }

//     public override void MiddleAction()
//     {
//         CharacterLife -= 3;
//         base.MiddleAction();
//     }

//     public override void BackAction()
//     {
//         // backEffect.Play();
//         base.BackAction();
//     }
// }
