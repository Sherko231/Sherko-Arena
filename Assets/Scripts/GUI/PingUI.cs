using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PingUI : MonoBehaviour
{
    public static PingUI Instance { get; private set; }

    [SerializeField] private float refreshRate = 2f;
    private TMP_Text _tmp;

    public void InitUI()
    {
        StartCoroutine(UpdatePing());
    }

    private void Awake()
    {
        Instance = this;
        _tmp = GetComponent<TMP_Text>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdatePing()
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshRate);
            _tmp.text = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(0) + " m/s";
        }
    }
}
