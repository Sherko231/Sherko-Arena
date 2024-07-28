using TMPro;
using UnityEngine;

public class KillsUI : MonoBehaviour
{
    public static KillsUI Instance { get; private set; }
    private TMP_Text _tmp;
    private Player _player;

    private void Awake()
    {
        Instance = this;
        _tmp = GetComponent<TMP_Text>();
    }

    public void InitUI()
    {
        _player = Player.OwnerSingleton;
        _player.OnKillChange += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        if (_player != null) _player.OnKillChange -= UpdateUI;
    }

    private void UpdateUI()
    {
        _tmp.text = $"Kills : {_player.Kills}";
    }
}
