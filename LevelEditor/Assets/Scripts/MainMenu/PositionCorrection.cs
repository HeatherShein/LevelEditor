using UnityEngine;

public class PositionCorrection : MonoBehaviour
{
    [SerializeField]
    GameObject level;

    [SerializeField]
    float offsetX;

    [SerializeField]
    float offsetY;

    [SerializeField]
    float offsetZ;

    public void CorrectPosition()
    {
        Vector3 pos = new Vector3(offsetX, offsetY,offsetZ);
        this.level.transform.position = this.level.transform.position + pos;
    }
}
