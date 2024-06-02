using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateImage : MonoBehaviour
{
    private RectTransform rect;
    private RectTransform parent;

    [SerializeField] RawImage rawImage;
    [SerializeField] Camera cam;
    private RenderTexture rt;

    void Start()
    {
        // 自分のRectTransformを取得
        rect = GetComponent<RectTransform>();
        // 親のRectTransformを取得
        parent = transform.parent.GetComponent<RectTransform>();

        if (Application.platform == RuntimePlatform.IPhonePlayer || true)
        {
            // iOS device
            // 90度回転
            rect.rotation = Quaternion.Euler(0, 0, 90);
            // 左右上下反転
            rect.localScale = new Vector3(-1, 1, 1);

            // サイズを親のRectTransformのサイズに合わせる
            rect.sizeDelta = new Vector2(parent.sizeDelta.y, parent.sizeDelta.x);
        }
        else
        {
            // サイズを親のRectTransformのサイズに合わせる
            rect.sizeDelta = parent.sizeDelta;
        }
        // カメラのRenderTextureのサイズをスクリーンサイズに合わせる
        cam.enabled = true;
        rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
    }

    public Texture2D CreateRotateTexture2D(Texture2D original)
    {
        //Texture2Dを作成
        Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, false);

        //subCameraにRenderTextureを入れる
        //cam.targetTexture = rt;

        // Imageに反映
        rawImage.texture = original;

        //手動でカメラをレンダリングします
        cam.Render();

        RenderTexture.active = rt;
        texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2D.Apply();

        return texture2D;
    }
}
