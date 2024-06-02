using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARSubsystems;

public class ARSaveTexture : MonoBehaviour
{
    [SerializeField] ARCameraBackground arCamBg;
    [SerializeField] ARCameraManager arCameraManager;
    public RenderTexture capturedTex;

    void Start()
    {
        // スクリーンサイズで初期化
        capturedTex = new RenderTexture(Screen.width, Screen.height, 0);
    }

    void Update()
    {
        SaveTex();
    }

    public void SaveTex()
    {
        if (arCamBg.material != null)
        {
            // RenderTextureに deep copy
            Graphics.Blit(null, capturedTex, arCamBg.material);
        }
    }

    void OnEnable()
    {
        arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        foreach(var texture in eventArgs.textures)
        {
            Graphics.Blit(texture, capturedTex, arCamBg.material);
        }
    }
}