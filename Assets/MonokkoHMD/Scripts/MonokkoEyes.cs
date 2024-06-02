using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonokkoEyes : MonoBehaviour
{
    private Vector3 targetPos;

    [SerializeField] float smoothTimePos = 0.3f;
    private float veloX;
    private float veloY;
    private float veloZ;
    
    [SerializeField] Transform lookAtPos;
    [SerializeField] float smoothTimeRot = 3f;

    [SerializeField] Transform eyeL;
    [SerializeField] Transform eyeR;
    private Vector3 eyeLInitialPos;
    private Vector3 eyeRInitialPos;

    // 生成時
    public void Initialize(Vector3 targetPos, Transform lookAt)
    {
        // 位置を設定
        this.targetPos = targetPos;

        // カメラの方向を向く
        this.lookAtPos = lookAt;
        transform.LookAt(lookAt);
    }

    // 位置更新
    public void UpdatePosition(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        
        // 今の座標とtargetPosが0.5m以上離れていたら、瞬時に移動
        if (Vector3.Distance(transform.position, targetPos) > 0.5f)
        {
            transform.position = targetPos;
        }
    }

    void Update()
    {
        // SmoothDampで滑らかに移動
        float x = Mathf.SmoothDamp(transform.position.x, targetPos.x, ref veloX, smoothTimePos);
        float y = Mathf.SmoothDamp(transform.position.y, targetPos.y, ref veloY, smoothTimePos);
        float z = Mathf.SmoothDamp(transform.position.z, targetPos.z, ref veloZ, smoothTimePos);
        transform.position = new Vector3(x, y, z);

        // 滑らかにLookAtPosに向く
        Vector3 vector3 = lookAtPos.position - transform.position;
        // Quaternion(回転値)を取得
		Quaternion quaternion = Quaternion.LookRotation(vector3);
		// 取得した回転値をこのゲームオブジェクトのrotationに代入
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, quaternion, Time.deltaTime * smoothTimeRot);
    }
}
