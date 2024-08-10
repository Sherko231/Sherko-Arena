using System;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    [SerializeField] private int maxFPS = 200;

    private void Awake()
    {
        Application.targetFrameRate = maxFPS;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) Player.OwnerSingleton.Stamina.Consumer.Full();
    }
}
