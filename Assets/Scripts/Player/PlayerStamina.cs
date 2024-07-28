using Sherko.Utils;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float gainPerKill = 10f;
    private Player _player;

    public Player Player => _player;
    public Consumer Consumer { get; } = new(100f);
    public bool IsUnlimited { get; set; }

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.OnKillChange += OnPlayerKillIncrease;
    }

    private void OnDisable()
    {
        _player.OnKillChange -= OnPlayerKillIncrease;
    }

    private void OnPlayerKillIncrease()
    {
        Consumer.Fill(gainPerKill);
    }
}
