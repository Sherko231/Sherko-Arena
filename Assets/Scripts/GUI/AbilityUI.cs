using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public static AbilityUI[] Instances { get; } = new AbilityUI[3];
    [SerializeField] [Range(0, 2)] private int index;
    private Image _image;
    private Player _player;

    private Color _deactivatedImageColor;
    private Color _activatedImageColor;

    private bool _IsUlt => index == 2;

    private void Awake()
    {
        Instances[index] = this;
        _image = GetComponent<Image>();
        _activatedImageColor = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
        _deactivatedImageColor = new Color(_image.color.r, _image.color.g, _image.color.b, 0.2f);
    }

    public void InitUI()
    {
        _player = Player.OwnerSingleton;
        _player.PlayerAbilities.Abilities[index].OnOwnerExecution += OnAbilityExecution;
        _player.PlayerAbilities.Abilities[index].OnReactivate += OnAbilityReactivate;
        _player.Stamina.Consumer.OnConsumerFull += OnUltActivate;
        _player.Stamina.Consumer.OnValueChange += OnUltDeactivate;
    }

    private void OnDisable()
    {
        _player.PlayerAbilities.Abilities[index].OnOwnerExecution -= OnAbilityExecution;
        _player.PlayerAbilities.Abilities[index].OnReactivate -= OnAbilityReactivate;
        _player.Stamina.Consumer.OnConsumerFull -= OnUltActivate;
        _player.Stamina.Consumer.OnValueChange -= OnUltDeactivate;
    }

    private void OnAbilityExecution()
    {
        _image.color = _deactivatedImageColor;
    }

    private void OnAbilityReactivate()
    {
        _image.color = _activatedImageColor;
    }

    private void OnUltActivate()
    {
        if (!_IsUlt) return;
        _image.color = _activatedImageColor;
    }

    private void OnUltDeactivate()
    {
        if (!_IsUlt || _player.Stamina.Consumer.Current >= _player.Stamina.Consumer.Capacity) return;
        _image.color = _deactivatedImageColor;
    }
}
