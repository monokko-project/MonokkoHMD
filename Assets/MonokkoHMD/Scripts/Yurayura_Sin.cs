using UnityEngine;

public class Yurayura_Sin : MonoBehaviour
{
    [SerializeField] float posSpeed = 1f;
    [SerializeField] float rotSpeed = 1f;
    [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float posAmount = 1f;
    [SerializeField] float rotAmount = 1f;
    [SerializeField, Range(0f, 1f)] float scaleAmount = 1f;

    Vector3 initialPos;
    Vector3 initialRot;
    Vector3 initialScale;
    float xPosDiff;
    float yPosDiff;
    float zPosDiff;
    float xRotDiff;
    float yRotDiff;
    float zRotDiff;
    float xScaleDiff;
    float yScaleDiff;
    float zScaleDiff;

    void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation.eulerAngles;
        initialScale = transform.localScale;
        xPosDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        yPosDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        zPosDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        xRotDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        yRotDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        zRotDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        xScaleDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        yScaleDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
        zScaleDiff = Random.Range(-1f, 1f) * Mathf.PI * 2f;
    }

    void Update()
    {
        float posTime = Time.time * posSpeed;
        float rotTime = Time.time * rotSpeed;
        float scaleTime = Time.time * scaleSpeed;


        float xPos = Mathf.Sin(posTime + xPosDiff);
        float yPos = Mathf.Sin(posTime + yPosDiff);
        float zPos = Mathf.Sin(posTime + zPosDiff);
        Vector3 pos = new Vector3(xPos, yPos, zPos) * posAmount;
        transform.localPosition = initialPos + pos;

        float xRot = Mathf.Sin(rotTime + xRotDiff);
        float yRot = Mathf.Sin(rotTime + yRotDiff);
        float zRot = Mathf.Sin(rotTime + zRotDiff);
        Vector3 rot = initialRot + (new Vector3(xRot, yRot, zRot) * rotAmount);
        transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);

        float xScale = Mathf.Sin(scaleTime + xScaleDiff) * scaleAmount;
        float yScale = Mathf.Sin(scaleTime + yScaleDiff) * scaleAmount;
        float zScale = Mathf.Sin(scaleTime + zScaleDiff) * scaleAmount;
        Vector3 scale = new Vector3(initialScale.x + xScale, initialScale.y + yScale, initialScale.z + zScale);
        transform.localScale = scale;
    }
}
