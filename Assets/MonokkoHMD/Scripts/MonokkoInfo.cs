using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonokkoInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI classText;

    void Start()
    {
        
    }

    public void SetInfo(Detect detect)
    {
        classText.text = detect.classId;

        if(detect.classId == "book")
        {
            nameText.text = "ブックン";
        }
        else if(detect.classId == "cup")
        {
            nameText.text = "カップン";
        }

    }
}
