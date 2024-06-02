using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using KanKikuchi.AudioManager;

public class GameManager : MonoBehaviour
{
    //静的マネージャー宣言
    public static GameManager instance { get; private set; }

    // 設定等
    public MyAppSetting setting;

    // ポーズ
    public bool isPauseNow = false;

    private void Awake()
    {
        //60FPSで固定する
        //UnityEngine.Application.targetFrameRate = 60;

        //シングルトン設定
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {

    }

    void Update()
    {
    }

    private IEnumerator JumpScene(string name, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);

        //SetCursor(MyAppSetting.CursorType.Default);
        SEManager.Instance.Stop();
        BGMManager.Instance.Stop();

        SceneManager.LoadScene(name);
    }

    public void SetCursor(MyAppSetting.CursorType type)
    {
        if ((int)type >= setting.cursorTexs.Length) return;
        Cursor.SetCursor(setting.cursorTexs[(int)type], Vector2.zero, CursorMode.Auto);
    }
}
