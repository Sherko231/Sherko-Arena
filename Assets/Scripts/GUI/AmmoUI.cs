using Sherko.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI Instance { get; private set; }
    private TMP_Text _tmp;
    private Consumer _gunAmmo;
    
    private void Awake()
    {
        Instance = this;
        _tmp = GetComponent<TMP_Text>();
    }
    
    public void InitUI()
    {
        _gunAmmo = Player.OwnerSingleton.Gun.Ammo;
        _gunAmmo.OnValueChange += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        if (_gunAmmo != null) _gunAmmo.OnValueChange -= UpdateUI;
    }
    
    private void UpdateUI()
    {
        _tmp.text = _gunAmmo.Current + "";
    }
}
