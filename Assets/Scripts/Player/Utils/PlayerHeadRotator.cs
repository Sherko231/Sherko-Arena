using UnityEngine;

public class PlayerHeadRotator : MonoBehaviour
{
    public void Sync(Quaternion playerCamRot)
    {
        transform.rotation = playerCamRot;
    }
}
