using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmScript : MonoBehaviour
{
    public bool farmField;

    // Start is called before the first frame update
    void Start()
    {
        if (farmField == false)//true （種が植えてある）畑はボタンが押せなくなる
        {
            Button btn = GetComponent<Button>();
            btn.interactable = false;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (farmField == false)//true （種が植えてある）畑はボタンが押せなくなる
        {
            Button btn = GetComponent<Button>();
            btn.interactable = false;
        }
    }
}
