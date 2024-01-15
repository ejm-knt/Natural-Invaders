using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicaController : MonoBehaviour
{
    private Vector3 mousePos;
    private Vector3 mouseFirstPos;
    private Vector3 mouseMoveAmount;
    private Vector3 firstCranePos;
    [SerializeField] GameObject crane;
    private float lastTapTime = 0f;
    private const float doubleTapTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            // 現在の時間を取得
            float currentTime = Time.time;
            // 前回のタップから0.5秒以内ならダブルタップと判定
            if (currentTime - lastTapTime <= doubleTapTime)
            {
                // ダブルタップ時の処理（果物を落とす処理）をここに書く
            }
            // タップ時間を更新
            lastTapTime = currentTime;
            mouseFirstPos = Input.mousePosition;
            // カメラからオブジェクトまでの距離を計算
            float distance = Camera.main.transform.position.z - crane.transform.position.z;
            // distanceをZ座標として使用
            mouseFirstPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseFirstPos.x, mouseFirstPos.y, -distance));
            firstCranePos = crane.transform.position;
        }

        if (Input.GetMouseButton(0))
        {
            mousePos = Input.mousePosition;
            // カメラからオブジェクトまでの距離を計算
            float distance = Camera.main.transform.position.z - crane.transform.position.z;
            // distanceをZ座標として使用
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -distance));
            Debug.Log("test");
            Debug.Log(mousePos);
            mouseMoveAmount = mousePos - mouseFirstPos;
            float craneX = firstCranePos.x + mouseMoveAmount.x;
            craneX = Mathf.Clamp(craneX, -3, 3);
            crane.transform.position = new Vector3(craneX, crane.transform.position.y, crane.transform.position.z);
        }


        // if (Input.GetMouseButtonUp(0))
        // {
        //     isDrag = false;
        // }
    }
}
