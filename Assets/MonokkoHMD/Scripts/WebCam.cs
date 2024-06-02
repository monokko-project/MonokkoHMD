using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCam : MonoBehaviour
{
    public WebCamTexture webcamTexture;
    RawImage rawImage;
    public Texture2D resizedTexture;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        var webcamTextureOrg = new WebCamTexture(devices[0].name);
        webcamTexture = new WebCamTexture(devices[0].name, webcamTextureOrg.width, webcamTextureOrg.height, 30);
        this.rawImage = GetComponent<RawImage>();
        //this.rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

 
    public Texture2D ResizeTexture2D(Texture2D source, int newWidth, int newHeight)
    {
        // フィルタモードをBilinearに設定
        source.filterMode = FilterMode.Bilinear;

        // 新しいサイズのRenderTextureを作成
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;

        // RenderTextureをアクティブにし、元のテクスチャを描画
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);

        // 新しいサイズのTexture2Dを作成し、ピクセルデータを読み込む
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight);
        resizedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resizedTexture.Apply();

        // RenderTextureを解放
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return resizedTexture;
    }

    private void Update()
    {
        resizedTexture = ResizeTexture2D(ConvertWebCamTextureToTexture2D(webcamTexture), 640, 640);
        //rawImage.texture = resizedTexture;
    }

    public Texture2D ConvertWebCamTextureToTexture2D(WebCamTexture webCamTexture)
    {
        // 新しいTexture2Dを作成
        Texture2D texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);

        // WebCamTextureのピクセルデータをTexture2Dにコピー
        texture2D.SetPixels(webCamTexture.GetPixels());
        texture2D.Apply();

        return texture2D;
    }

}