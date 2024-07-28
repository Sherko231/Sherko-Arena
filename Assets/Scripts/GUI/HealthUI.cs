using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public static HealthUI Instance { get; private set; }

    [SerializeField] private TMP_Text healthTmp;
    private Slider _slider;
    private Player _player;
    private Player _player2;
    
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        Instance = this;
    }
    
    public void InitUI()
    {
        _player = Player.OwnerSingleton;
        _player.Health.OnHealthUpdate += UpdateUI;
    }

    private void OnDisable()
    {
        _player.Health.OnHealthUpdate -= UpdateUI;
    }
    
    private void UpdateUI()
    {
        _slider.value = _player.Health.CurrentHealth / _player.Health.MaxHealth;
        healthTmp.text = _player.Health.CurrentHealth  + "";
    }
}
