using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Player))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IsMovingAnimatorPrp = Animator.StringToHash("isMoving");
    private static readonly int IsSprintingAnimatorPrp = Animator.StringToHash("isSprinting");
    private static readonly int IsSneakingAnimatorPrp = Animator.StringToHash("isSneaking");

    private Player _player;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_player.Network.IsOwner) return;
        _animator.SetBool(IsMovingAnimatorPrp, _player.IsMoving);
        _animator.SetBool(IsSprintingAnimatorPrp, _player.IsSprinting);
        _animator.SetBool(IsSneakingAnimatorPrp, _player.IsSneaking);
    }
    
}
