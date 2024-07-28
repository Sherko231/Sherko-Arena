using UnityEngine;

public class ShotgunRecoilHandler : MonoBehaviour
{
    [SerializeField] private float maxRecoilDist = 10f;
    [SerializeField] private float recoilPower = 14f;
    [SerializeField] [Range(1, 10f)] private float ultMultiplier = 3;
    private Player _player;
    private Gun _gun;
    
    public bool IsUlt { get; set; }

    private bool _CanRecoil
    {
        get
        {
            bool isHit = Physics.Raycast(_player.transform.position, _player.LookingVec,out RaycastHit hit , maxRecoilDist);
            return isHit && hit.normal == Vector3.up;
        }
    }

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _player.OnInitilized += Init;
    }

    private void Init()
    {
        _gun = _player.Gun;
        _gun.OnShootOwner += OnShoot;
    }

    private void OnDisable()
    {
        _gun.OnShootOwner -= OnShoot;
        _player.OnInitilized -= Init;
    }

    private void OnShoot()
    {
        if (!_CanRecoil) return;
        Vector3 recoilDir = -_player.LookingVec;
        Vector3 playerVelo = _player.Rb.velocity;
        float targetRecoilPower = IsUlt ? recoilPower * ultMultiplier : recoilPower;
        _player.Rb.velocity = new Vector3(playerVelo.x, 0, playerVelo.z);
        _player.Rb.AddForce(recoilDir * targetRecoilPower, ForceMode.Impulse);
    }
}
