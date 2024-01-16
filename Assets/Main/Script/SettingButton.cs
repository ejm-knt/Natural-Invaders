using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject settingCanvas;
    public void OnSettingButton()
    {
        settingCanvas = GameObject.Find("SettingCanvas");
        var canvas = settingCanvas.transform.Find("PopUpContent").gameObject;
        Debug.Log(canvas);
        canvas.SetActive(true);
    }
}
