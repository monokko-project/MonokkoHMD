using System;
using UnityEngine;

public class CameraAspect : MonoBehaviour
{
    //参照: https://note.com/suzukijohnp/n/n3ba0969ea002

    [SerializeField] Camera cam;
    [SerializeField] float baseWidth = 9.0f;
    [SerializeField] float baseHeight = 16.0f;
    //true:false = 2D:3D
    [SerializeField] bool is2D = true;
    [SerializeField] Mode mode = default;

    // ウィンドウサイズ可変時
    [SerializeField] bool canResizeWindow = false;
    private int screenWidth;
    private int screenHeight;
    private bool screenStartedResizing = false;
    private int updateCounter = 0;
    [SerializeField] int numberOfUpdatesToRunXFunction = 1;

    public enum Mode
    {
        //高さ固定+アスペクト外も可視,高さ固定+アスペクト外は黒帯
        height_fixed,
        height_fixed_black_band,
        //幅固定+アスペクト外も可視,幅固定+アスペクト外は黒帯
        width_fixed,
        width_fixed_black_band,
        //アスペクト固定+アスペクト外も可視,アスペクト固定+アスペクト外は黒帯
        aspect_fixed,
        aspect_fixed_black_band,
        //アスペクト外は映らないようにする
        fit,

    }

    private void Awake()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Set();
    }

    private void Update()
    {
        if (canResizeWindow) if (CheckWindowResize()) Set();
    }

    private void Set()
    {
        float scale = 0f;
        float width = 0f;
        float height = 0f;
        float scaleWidth = 0f;
        float scaleRatio = 0f;

        if (is2D)
        {
            switch (mode)
            {
                case Mode.height_fixed:
                    // デフォルト
                    break;

                case Mode.height_fixed_black_band:
                    // 高さ固定+幅上限あり
                    scale = Mathf.Min(Screen.height / this.baseHeight, Screen.width / this.baseWidth);
                    width = (this.baseWidth * scale) / Screen.width;
                    this.cam.rect = new Rect((1.0f - width) * 0.5f, 0, width, 1.0f);
                    break;

                case Mode.width_fixed:
                    // 幅固定+高さ可変
                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    this.cam.orthographicSize *= scaleWidth;
                    break;

                case Mode.width_fixed_black_band:
                    // 幅固定+高さ上限あり
                    scale = Mathf.Min(Screen.height / this.baseHeight, Screen.width / this.baseWidth);
                    height = (this.baseHeight * scale) / Screen.height;
                    this.cam.rect = new Rect(0, (1.0f - height) * 0.5f, 1.0f, height);

                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    scaleRatio = Mathf.Min(scaleWidth, 1.0f);
                    this.cam.orthographicSize *= scaleRatio;
                    break;

                case Mode.aspect_fixed:
                    // ベース維持
                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    scaleRatio = Mathf.Max(scaleWidth, 1.0f);
                    this.cam.orthographicSize *= scaleRatio;
                    break;

                case Mode.aspect_fixed_black_band:
                    // アスペクト比固定
                    scale = Mathf.Min(Screen.height / this.baseHeight, Screen.width / this.baseWidth);
                    width = (this.baseWidth * scale) / Screen.width;
                    height = (this.baseHeight * scale) / Screen.height;
                    this.cam.rect = new Rect((1.0f - width) * 0.5f, (1.0f - height) * 0.5f, width, height);
                    break;

                case Mode.fit:
                    // フィット
                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    scaleRatio = Mathf.Min(scaleWidth, 1.0f);
                    this.cam.orthographicSize *= scaleRatio;
                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case Mode.height_fixed:
                    // デフォルト
                    break;

                case Mode.height_fixed_black_band:
                    // 高さ固定+幅上限あり
                    scale = Mathf.Min(Screen.height / this.baseHeight, Screen.width / this.baseWidth);
                    width = (this.baseWidth * scale) / Screen.width;
                    this.cam.rect = new Rect((1.0f - width) * 0.5f, 0, width, 1.0f);
                    break;

                case Mode.width_fixed:
                    // 幅固定+高さ可変
                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    this.cam.fieldOfView = Mathf.Atan(Mathf.Tan(this.cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * scaleWidth) * 2.0f * Mathf.Rad2Deg;
                    break;

                case Mode.width_fixed_black_band:
                    // 幅固定+高さ上限あり
                    scale = Mathf.Min(Screen.height / this.baseHeight, Screen.width / this.baseWidth);
                    height = (this.baseHeight * scale) / Screen.height;
                    this.cam.rect = new Rect(0, (1.0f - height) * 0.5f, 1.0f, height);

                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    scaleRatio = Mathf.Min(scaleWidth, 1.0f);
                    this.cam.fieldOfView = Mathf.Atan(Mathf.Tan(this.cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
                    break;

                case Mode.aspect_fixed:
                    // ベース維持
                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    scaleRatio = Mathf.Max(scaleWidth, 1.0f);
                    this.cam.fieldOfView = Mathf.Atan(Mathf.Tan(this.cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
                    break;

                case Mode.aspect_fixed_black_band:
                    // アスペクト比固定
                    scale = Mathf.Min(Screen.height / this.baseHeight, Screen.width / this.baseWidth);
                    width = (this.baseWidth * scale) / Screen.width;
                    height = (this.baseHeight * scale) / Screen.height;
                    this.cam.rect = new Rect((1.0f - width) * 0.5f, (1.0f - height) * 0.5f, width, height);
                    break;

                case Mode.fit:
                    // フィット
                    scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
                    scaleRatio = Mathf.Min(scaleWidth, 1.0f);
                    this.cam.fieldOfView = Mathf.Atan(Mathf.Tan(this.cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
                    break;
            }
        }
    }

    private bool CheckWindowResize()
    {
        if ((Screen.width != screenWidth) || (Screen.height != screenHeight))
        {
            updateCounter = 0;
            screenStartedResizing = true;
            screenWidth = Screen.width;
        }

        if (screenStartedResizing)
        {
            updateCounter += 1;
        }

        if (updateCounter == numberOfUpdatesToRunXFunction && screenStartedResizing)
        {
            screenStartedResizing = false;
            return true;
        }
        return false;
    }
}
