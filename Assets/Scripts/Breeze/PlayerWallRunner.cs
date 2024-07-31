using Sherko.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWallRunner : MonoBehaviour
{
    [FormerlySerializedAs("wallCheckerL")] [SerializeField] private SideWallChecker sideWallCheckerL;
    [FormerlySerializedAs("wallCheckerR")] [SerializeField] private SideWallChecker sideWallCheckerR;
    [SerializeField] private float tiltAmount = 25f;
    [SerializeField] private float tiltSpeed = 5f;
    [SerializeField] private float wallRunSpeed = 1.5f;
    private Player _player;
    private ValueTransferer _camRotZTransferer;
    private bool _isParticlePlaying;

    private bool _IsTouchingAnyWall => sideWallCheckerL.IsWalled || sideWallCheckerR.IsWalled;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        sideWallCheckerL.Init(_player);
        sideWallCheckerR.Init(_player);
        _camRotZTransferer = new(_player.CamPoint.rotation.eulerAngles.z, tiltSpeed);
    }

    private void Update()
    {
        HandleCamTilting();
        HandleWallRunVFX();
    }

    private void FixedUpdate()
    {
        if (_IsTouchingAnyWall) HandleWallStick();

    }

    private void HandleWallStick()
    {
        if (_player.Rb.velocity.y <= 0) _player.Rb.velocity = new Vector3(_player.Rb.velocity.x, 0, _player.Rb.velocity.z) * wallRunSpeed;
    }
    
    private void HandleCamTilting()
    {
        float eulerRotZ;
        if (sideWallCheckerL.IsWalled) eulerRotZ = _camRotZTransferer.SmoothTransfer(-tiltAmount);
        else if (sideWallCheckerR.IsWalled) eulerRotZ = _camRotZTransferer.SmoothTransfer(tiltAmount);
        else eulerRotZ = _camRotZTransferer.SmoothReset();
        SetZRot(eulerRotZ);
    }

    private void SetZRot(float z)
    {
        Quaternion rotation = _player.CamPoint.rotation;
        rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, z);
        _player.CamPoint.rotation = rotation;
    }

    private void HandleWallRunVFX()
    {
        if (_IsTouchingAnyWall && !_isParticlePlaying)
        {
            _isParticlePlaying = true;
            _player.FX.ToggleWallRunVFX(true);
            _player.RpcManager.SendWallRunVfxClientRpc(_player.Network.OwnerClientId,true);
        }
        else if (!_IsTouchingAnyWall && _isParticlePlaying)
        {
            _isParticlePlaying = false;
            _player.FX.ToggleWallRunVFX(false);
            _player.RpcManager.SendWallRunVfxClientRpc(_player.Network.OwnerClientId, false);
        }
    }


}
