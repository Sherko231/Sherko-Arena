using Sherko.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private PlayerControllerStats stats;
    
    public bool ControlsDisabled { get; set; }
    public Vector2 MoveVec => _moveVec;

    private readonly IntervalRunner _shootIntervalRunner = new(true);
    
    private Player _player;
    private Vector2 _moveVec;
    private Vector2 _mouseVec;
    private bool _jumpPressed;
    private bool _sprintPressed;
    private bool _sneakPressed;
    private bool _shootPressed;

    public void OnPause(InputAction.CallbackContext c)
    { 
        if (!c.started) return;
        if (!_player.Network.IsOwner) return;
        SherkoUtils.ToggleCursor(!SherkoUtils.IsCursorVisible);
        NetworkManager.Singleton.Shutdown();
        JoinUI.Instance.ToggleUI(!JoinUI.Instance.Toggled);
    }
    
    public void OnMove(InputAction.CallbackContext c)
    {
        _moveVec = c.ReadValue<Vector2>();
    }

    public void OnMouse(InputAction.CallbackContext c)
    {
        _mouseVec = c.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext c)
    {
        _jumpPressed = !c.canceled;
    }

    public void OnSprint(InputAction.CallbackContext c)
    {
        _sprintPressed = !c.canceled;
    }

    public void OnSneak(InputAction.CallbackContext c)
    {
        _sneakPressed = !c.canceled;
    }

    public void OnShoot(InputAction.CallbackContext c)
    {
        _shootPressed = !c.canceled;
    }

    public void OnReload(InputAction.CallbackContext c)
    {
        if (!c.started) return;
        if (!_player.Network.IsOwner) return;
        
        if (_player.Health.IsAlive) _player.Gun.Reload();
    }

    public void OnCostedAbility1(InputAction.CallbackContext c)
    {
        if (!c.started) return;
        if (!_player.Network.IsOwner) return;
        _player.PlayerAbilities.ExecuteAbility(0);
    }
    
    public void OnCostedAbility2(InputAction.CallbackContext c)
    {
        if (!c.started) return;
        if (!_player.Network.IsOwner) return;
        _player.PlayerAbilities.ExecuteAbility(1);
    }
    
    public void OnCostedAbility3(InputAction.CallbackContext c)
    {
        if (!c.started) return;
        if (!_player.Network.IsOwner) return;
        _player.PlayerAbilities.ExecuteAbility(2);
    }

    private void Awake()
    {
        _player = GetComponent<Player>();
        SherkoUtils.ToggleCursor(false);
    }

    private void Update()
    {
        if (ControlsDisabled) BlockInputProcess();
        if (_player.Network.IsOwner)
        {   
            _player.Rotate(_mouseVec, stats.MouseSensitivity);
            if (_player.Health.IsAlive && _player.Gun != null) _shootIntervalRunner.RunAtIntervals(() => _player.Gun.Shoot(), _player.Gun.Stats.ShootInterval, _shootPressed);
        }

        _player.SyncHeadRot();
    }

    private void FixedUpdate()
    {
        if (_player.Network.IsOwner)
        {
            _player.Sprint(_sprintPressed);
            _player.Sneak(_sneakPressed);
            _player.Move(_moveVec);
            if (_jumpPressed) _player.Jump();
        }
    }

    private void BlockInputProcess()
    {
        _moveVec = Vector2.zero;
        _sprintPressed = _sneakPressed = _jumpPressed = false;
    }
}
