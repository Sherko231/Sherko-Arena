using Sherko.Utils;
using Unity.Netcode;
using UnityEngine;

public class EditorController : MonoBehaviour
{
    [SerializeField] private int maxFPS = 200;

    private void Awake()
    {
        Application.targetFrameRate = maxFPS;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) Player.OwnerSingleton.Stamina.Consumer.Fill(100f);
        if (Input.GetKeyDown(KeyCode.H)) Player.OwnerSingleton.Stamina.Consumer.Consume(10f);
    }
}
