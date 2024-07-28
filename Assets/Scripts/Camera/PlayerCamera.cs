using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;
    public static CinemachineVirtualCamera Cam;

    [SerializeField] private float lerpTimeMultiplier = 40;

    private void Awake()
    {
        Instance = this;
        Cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void SmoothFollow(Transform target)
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * lerpTimeMultiplier);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * lerpTimeMultiplier);
    }
}
