using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MonokkoVision : MonoBehaviour
{
    [SerializeField] int detectionInterval = 300;
    public YOLOv8 yOLOv8;
    [SerializeField] Transform mainCamera;
    public ARCameraViewer arCameraViewer;
    [SerializeField] ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField] RectTransform uiCanvas;
    [SerializeField] GameObject boundingBoxUI;
    [SerializeField] GameObject monokkoInfoUI;
    private List<GameObject> infoUIs = new List<GameObject>();

    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] TextMeshProUGUI debugText2;

    public RawImage debugARCameraImage;
    [SerializeField] RotateImage rotateImage;
    //public ARSaveTexture aRSaveTexture;

    // モノッコの目オブジェクト
    [SerializeField] GameObject monokkoEye;
    // モノッコとして認識するラベル
    public List<string> monokkoLabels = new List<string> { "book", "cup" };
    // モノッコのラベルごとの目オブジェクト
    public Dictionary<string, MonokkoEyes> monokkoEyeDict = new Dictionary<string, MonokkoEyes>();

    void Start()
    {
        //MonokkoDetection(destroyCancellationToken).Forget();
        StartCoroutine(MonokkoDetection(new CancellationToken()));

        // ダミーのDetectからDetectMonokkoを呼び出す
        /*
        Detect detect = new Detect();
        detect.x1 = 200;
        detect.x2 = 600;
        detect.y1 = 600;
        detect.y2 = 400;
        detect.classId = "book";
        DetectMonokko(detect);
        */
    }

    void Update()
    {
        //if(aRSaveTexture.capturedTex != null)
        // {
        //    rawImage.texture = aRSaveTexture.capturedTex;
        //}
        
        if (arCameraViewer.CurrentArImage != null)
        {
            //debugARCameraImage.texture = arCameraViewer.CurrentArImage;
            //yOLOv8.Execute(ResizeTexture2D(arCameraViewer.CurrentArImage, 640, 640));
        }

        /*
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            debugText2.text = "touch: " + touch.position.ToString();
            if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                Instantiate(monokkoEye, hitPose.position, Quaternion.identity);
            }
        }
        */
    }

    private IEnumerator MonokkoDetection(CancellationToken ct)
    {
        // 1秒ごとにモノッコを検出
        while (true)
        {
            //await UniTask.Delay(detectionInterval, cancellationToken: ct);
            yield return new WaitForSeconds(detectionInterval / 1000f);

            arCameraViewer.Camera2();
            if (arCameraViewer.CurrentArImage != null)
            {
                

                //if (Application.platform == RuntimePlatform.IPhonePlayer && false)
                //    debugARCameraImage.texture = RotateTexture(arCameraViewer.CurrentArImage, true);
                //else
                //    debugARCameraImage.texture = arCameraViewer.CurrentArImage;
                
               
                // 1フレーム待つ
                //await UniTask.Yield(PlayerLoopTiming.Update, ct);

                // rotateCameraのRenderTextureを取得
                Texture2D cameraTex = rotateImage.CreateRotateTexture2D(arCameraViewer.CurrentArImage);

                Destroy(debugARCameraImage.texture);
                debugARCameraImage.texture = cameraTex;
                
                Texture2D resizedTex = ResizeTexture2D(cameraTex, 640, 640);
                
                Debug.Log("Execute Start");
                //List<Detect> detects = await yOLOv8.Execute(resizedTex);
                
                // 実行中は待つ
                while (yOLOv8.isExecuting) yield return null;
                
                yield return yOLOv8.Execute(resizedTex);
                List<Detect> detects = yOLOv8.detects;
                Debug.Log("Execute End");

                Destroy(resizedTex);
                //Destroy(cameraTex);
                
                
                // 以前のUIを削除
                foreach (var ui in infoUIs) Destroy(ui);
                infoUIs.Clear();

                foreach(var detect in detects)
                {
                    if (monokkoLabels.Contains(detect.classId))
                    {
                        DetectMonokko(detect);
                    }
                }
                
            }
        }
    }

    private void DetectMonokko(Detect detect)
    {
        // 座標をスクリーン座標 → CanvasのUI座標に変換
        Vector2 screen1 = new Vector2(detect.x1, detect.y1);
        Vector2 screen2 = new Vector2(detect.x2, detect.y2);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvas, screen1, null, out var ui1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvas, screen2, null, out var ui2);

        // canvasのreferenceResolutionは800*600
        float x1UI = ui1.x;
        float x2UI = ui2.x;
        float y1UI = ui1.y;
        float y2UI = ui2.y;

        Debug.Log("detect: " + detect.classId + " " + x1UI + " " + x2UI + " " + y1UI + " " + y2UI);

        // BoundingBoxを生成・表示
        RectTransform boundingBox = Instantiate(boundingBoxUI, uiCanvas).GetComponent<RectTransform>();
        boundingBox.localPosition = new Vector2((x1UI + x2UI) / 2f, (y1UI + y2UI) / 2f);
        boundingBox.sizeDelta = new Vector2(Mathf.Abs(x1UI - x2UI), Mathf.Abs(y1UI - y2UI));

        // モノッコの情報を表示
        RectTransform monokkoInfo = Instantiate(monokkoInfoUI, uiCanvas).GetComponent<RectTransform>();
        monokkoInfo.localPosition = new Vector2((x1UI + x2UI) / 2f, Mathf.Max(y1UI, y2UI) + 10f);
        monokkoInfo.GetComponent<MonokkoInfo>().SetInfo(detect);

        // リストに追加(次フレームで削除するため)
        infoUIs.Add(boundingBox.gameObject);
        infoUIs.Add(monokkoInfo.gameObject);

        RaycastAndSetObj(detect);
    }


    private void RaycastAndSetObj(Detect detect)
    {
        Vector2 detectCenterPos = new Vector2((detect.x1 + detect.x2) / 2, (detect.y1 + detect.y2) / 2);

        //debugText.text = "detect: " + detect.classId + " " + detectCenterPos.ToString();

        if (arRaycastManager.Raycast(detectCenterPos, hits, TrackableType.PlaneWithinPolygon))
        {
            // 0番目が最も近い場所でヒットした情報
            var hitPose = hits[0].pose;
            monokkoEye.transform.position = hitPose.position;

            // モノッコの目をラベルごとに生成/更新
            if (monokkoEyeDict.ContainsKey(detect.classId))
            {
                if (monokkoEyeDict[detect.classId] != null)
                {
                    // 更新
                    monokkoEyeDict[detect.classId].UpdatePosition(hitPose.position);
                }
                else
                {
                    // 生成(なぜかキーは存在しているがnullの場合)
                    MonokkoEyes monokkoEyes = Instantiate(monokkoEye, hitPose.position, Quaternion.identity).GetComponent<MonokkoEyes>();
                    monokkoEyes.Initialize(hitPose.position, mainCamera);
                    monokkoEyeDict.Add(detect.classId, monokkoEyes);
                }
            }
            else
            {
                // 新規生成
                MonokkoEyes monokkoEyes = Instantiate(monokkoEye, hitPose.position, Quaternion.identity).GetComponent<MonokkoEyes>();
                monokkoEyes.Initialize(hitPose.position, mainCamera);
                monokkoEyeDict.Add(detect.classId, monokkoEyes);
            }
        }
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
