using System;
using TMPro;
using UnityEngine;

public class CodeUI : MonoBehaviour
{
    private TMP_Text _tmp;

    private void Awake()
    {
        _tmp = GetComponent<TMP_Text>();
        GameRelay.Instance.OnJoin += SetCodeUI;
        GameRelay.Instance.OnHost += SetCodeUI;
    }

    private void OnDisable()
    {
        GameRelay.Instance.OnJoin -= SetCodeUI;
        GameRelay.Instance.OnHost -= SetCodeUI;
    }

    private void SetCodeUI()
    {
        _tmp.text = "Code : " + GameRelay.Instance.JoinCode;
    }
}
