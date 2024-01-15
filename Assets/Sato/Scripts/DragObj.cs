using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObj : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Vector3 prevPos;
    private Vector3 startPos;   //オブジェクトの開始時点のローカル座標
    [SerializeField] float changeScrollDragThreshold = 0.5f;    //ドラッグへ切り替えるしきい値
    private GameObject parentObj;   //親情報取得用
    private GameObject startParentObject;   //最初の親情報
    private bool isSelfDrag = false;    //自分自身がドラッグの対象かどうか
    private Vector3 offset; //マウスカーソルとオブジェクトの中心との差分
    private Vector2 startSize;  //開始時点のサイズ
    private RectTransform rectTransform;    //自身のrectTransform
    private GameObject canvas3D;
    private GridCheck gridCheck;    //取得したgridCheck

    void Start()
    {
        //最初の親情報の取得
        startParentObject = this.transform.parent.gameObject;
        //最初のローカル座標の取得
        startPos = this.transform.localPosition;
        // 最初のサイズ
        rectTransform = GetComponent<RectTransform>();
        startSize = rectTransform.sizeDelta;
        // Canvasの指定
        canvas3D = GameObject.Find("Canvas3D");
    }

    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("クリックされた");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ドラッグ前の位置を記憶しておく
        prevPos = transform.position;
        // ドラッグ前の親情報を取得しておく
        parentObj = this.transform.parent.gameObject;
        // ドラッグ開始時にマウスカーソルとオブジェクトの中心との差分を計算
        offset = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z)) - transform.position;
        // ドラッグ開始時にサイズを変更widthとheightを150へ
        rectTransform.sizeDelta = new Vector2(150, 150);
        // gridCheckを所持していれば移動時にアタッチを外す
        if (gridCheck != null)
        {
            gridCheck.attached = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ方向の判定
        if (Mathf.Abs(prevPos.y - eventData.position.y) >= changeScrollDragThreshold)
        {
            isSelfDrag = true;
        }
        if (isSelfDrag)
        {
            this.gameObject.transform.SetParent(canvas3D.transform, false);
            this.gameObject.transform.SetAsLastSibling();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        // ドラッグ中は位置を更新する
        Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
        transform.position = worldPosition - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ドラッグが終了した座標を取得
        Vector3 screenPosition = new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z);
        Vector3 endDragPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        // ドラッグが終了した座標にあるオブジェクトを検出
        RaycastHit hit;
        Vector3 rayDirection = endDragPosition - Camera.main.transform.position;
        Debug.DrawRay(Camera.main.transform.position, rayDirection, Color.green);
        // 特定のレイヤーのみを検出するレイヤーマスクを作成
        int layerMask = 1 << LayerMask.NameToLayer("Grid");
        bool shouldReset = true;
        if (Physics.Raycast(Camera.main.transform.position, rayDirection, out hit, Mathf.Infinity, layerMask))
        {
            // レイがヒットした場合は緑色で描画
            Debug.DrawRay(Camera.main.transform.position, rayDirection, Color.green);
            // ヒットしたGameObjectの名前を取得
            string hitObjectName = hit.collider.gameObject.name;
            gridCheck = hit.transform.GetComponent<GridCheck>();
            if (gridCheck != null && !gridCheck.attached)
            {
                // gridCheckを持ち、かつアタッチされてなければ子要素に切り替える
                transform.SetParent(hit.transform);
                gameObject.transform.localPosition = new Vector3(0.01f, 0, 0.4f);
                gridCheck.attached = true;
                shouldReset = false;
            }
            // gridCheck.attached = false;
            // gridCheck = null;
        }
        if (shouldReset)
        {
            ResetDefault();
        }
        if (OnPartySumInfoChanged != null)
        {
            OnPartySumInfoChanged.Invoke(); //着火しまーす！
        }
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Se59flingingupandaway);
    }

    public delegate void OnPartySumInfoChangedDelegate();
    public event OnPartySumInfoChangedDelegate OnPartySumInfoChanged;


    private void ResetDefault() //最初の場所に戻す
    {
        this.transform.SetParent(startParentObject.transform);
        rectTransform.sizeDelta = startSize;
        transform.localPosition = startPos;
        gridCheck = null;
        isSelfDrag = false;
    }
}
