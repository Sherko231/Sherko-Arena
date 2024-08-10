using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [InfoBox("index 0 = E\nindex 1 = Q\nindex 2 = X (ULT)", SdfIconType.InfoCircle)]
    public PlayerAbility[] Abilities = new PlayerAbility[3];
    private Player _player;
    private PlayerStamina _stamina;
    
    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _stamina = _player.Stamina;
    }

    public void ExecuteAbility(int index)
    {
        PlayerAbility ability = Abilities[index];

        if (!ability.CanBeUsed(_stamina))
        {
            return;
        }
        
        if (!_stamina.IsUnlimited) _stamina.Consumer.Consume(ability.StaminaCost);
        Abilities[index].Execute(_player);
    }

    public void ForceExecuteAbility(int index)
    {
        Abilities[index].Execute(_player);
    }

    public void TerminateAll()
    {
        foreach (PlayerAbility ability in Abilities) ability.TerminateAbility(_player);
    }
}
