using UnityEngine;

public class ClientController : MonoBehaviour
{
    [SerializeField] private int maxFPS = 200;

    private void Awake()
    {
        Application.targetFrameRate = maxFPS;
    }
}
