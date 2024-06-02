using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class ARCameraViewer : MonoBehaviour
{
    private Texture2D tempBuffer = null;
    public Texture2D m_Texture = null;
    public Texture2D CurrentArImage => m_Texture;
    [SerializeField]ARCameraManager cameraManager = null;
    private XRCpuImage.ConversionParams currentConversionParam;

    [SerializeField] TextMeshProUGUI stateText;
    void OnEnable()
    {
        //cameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        //cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    public unsafe void OnCameraFrameReceived()
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return;

        currentConversionParam.inputRect = new RectInt(0, 0, image.width, image.height);
        currentConversionParam.outputDimensions = new Vector2Int(image.width, image.height);
        currentConversionParam.outputFormat = TextureFormat.RGBA32;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // iOS device
            currentConversionParam.transformation = XRCpuImage.Transformation.MirrorY;
        }
        
        

        // See how many bytes you need to store the final image.
        int size = image.GetConvertedDataSize(currentConversionParam);

        // Allocate a buffer to store the image.
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data
        image.Convert(currentConversionParam, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so you can dispose of the XRCpuImage. You must do this or it will leak resources.
        image.Dispose();
        

        // At this point, you can process the image, pass it to a computer vision algorithm, etc.
        // In this example, you apply it to a texture to visualize it.

        // You've got the data; let's put it into a texture so you can visualize it.
        ReCreateTexture(ref tempBuffer, currentConversionParam.outputDimensions, currentConversionParam.outputFormat);

        tempBuffer.LoadRawTextureData(buffer);
        tempBuffer.Apply();

        if (Application.platform == RuntimePlatform.IPhonePlayer || true)
        {
            // iOS device
            Vector2Int dimension = Vector2Int.zero;
            dimension.x = currentConversionParam.outputDimensions.y;
            dimension.y = currentConversionParam.outputDimensions.x;
            ReCreateTexture(ref m_Texture, dimension, currentConversionParam.outputFormat);

            // 90° rotate

            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    m_Texture.SetPixel(image.height - y - 1, x, tempBuffer.GetPixel(x, y));
                }
            }
            m_Texture.Apply();
        }
        else
        {
            Destroy(m_Texture);
            m_Texture = tempBuffer;
        }
        

        /*
        if (UnityEngine.Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            Vector2Int dimension = Vector2Int.zero;
            dimension.x = currentConversionParam.outputDimensions.y;
            dimension.y = currentConversionParam.outputDimensions.x;
            ReCreateTexture(ref m_Texture, dimension, currentConversionParam.outputFormat);

            // 90° rotate
            
            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    m_Texture.SetPixel(image.height - y - 1, x,  tempBuffer.GetPixel(x, y));
                }
            }
            m_Texture.Apply();
        }
        else if (UnityEngine.Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            ReCreateTexture(ref m_Texture, currentConversionParam.outputDimensions, currentConversionParam.outputFormat);

            // 180° rotate
            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    m_Texture.SetPixel(image.width - x - 1, image.height - y - 1, tempBuffer.GetPixel(x, y));
                }
            }
            m_Texture.Apply();
        }
        else if (UnityEngine.Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        {
            Vector2Int dimension = Vector2Int.zero;
            dimension.x = currentConversionParam.outputDimensions.y;
            dimension.y = currentConversionParam.outputDimensions.x;
            ReCreateTexture(ref m_Texture, dimension, currentConversionParam.outputFormat);

            // 270° rotate
            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    m_Texture.SetPixel(y, image.width - x - 1, tempBuffer.GetPixel(x, y));
                }
            }
            m_Texture.Apply();
        }
        else
        {
            m_Texture = tempBuffer;
        }*/
        //m_Texture = tempBuffer;
        
        // Done with your temporary data, so you can dispose it.
        buffer.Dispose();
    }

    protected void ReCreateTexture(ref Texture2D tex, in Vector2Int size, TextureFormat foramt)
    {
        // Check necessity
        if (tex != null
            && Mathf.RoundToInt(tex.texelSize.x) == size.x
            && Mathf.RoundToInt(tex.texelSize.y) == size.y)
        {
            return;
        }
        
        if (tex != null )
        {
            var prev = tex;
            tex = null;
            Destroy(prev);
        }
        tex = new Texture2D(
            size.x,
            size.y,
            foramt,
            false);
    }

    public void Camera2()
    {
        // Acquire an XRCpuImage
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return;

        // Set up our conversion params
        var conversionParams = new XRCpuImage.ConversionParams
        {
            // Convert the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Output at full resolution
            outputDimensions = new Vector2Int(image.width, image.height),

            // Convert to RGBA format
            outputFormat = TextureFormat.RGBA32,
        };
        // Flip across the vertical axis (mirror image)
        //if (Application.platform == RuntimePlatform.IPhonePlayer) currentConversionParam.transformation = XRCpuImage.Transformation.MirrorY;

        // Create a Texture2D to store the converted image
        var texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

        // Texture2D allows us write directly to the raw texture data as an optimization
        var rawTextureData = texture.GetRawTextureData<byte>();
        try
        {
            unsafe
            {
                // Synchronously convert to the desired TextureFormat
                image.Convert(
                    conversionParams,
                    new IntPtr(rawTextureData.GetUnsafePtr()),
                    rawTextureData.Length);
            }
        }
        finally
        {
            // Dispose the XRCpuImage after we're finished to prevent any memory leaks
            image.Dispose();
        }

        // Apply the converted pixel data to our texture
        texture.Apply();

        // 反映
        Destroy(m_Texture);
        m_Texture = texture;
    }

    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;
        int iRotated, iOriginal;
        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }
        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        Destroy(originalTexture);//@@@　メモリーリーク注意
        return rotatedTexture;
    }

    
}