using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public static StaminaUI Instance { get; private set; }

    [SerializeField] private TMP_Text staminaTmp;
    private Slider _slider;
    private Player _player;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        Instance = this;
    }

    public void InitUI()
    {
        _player = Player.OwnerSingleton;
        _player.Stamina.Consumer.OnValueChange += UpdateUI;
    }

    private void OnDisable()
    {
        _player.Stamina.Consumer.OnValueChange -= UpdateUI;
    }

    private void UpdateUI()
    {
        _slider.value = _player.Stamina.Consumer.Current / _player.Stamina.Consumer.Capacity;
        staminaTmp.text = _player.Stamina.Consumer.Current + "";
    }
}
