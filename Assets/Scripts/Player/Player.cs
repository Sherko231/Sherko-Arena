using Sherko.MonoBehaviours;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerAnimator), typeof(PlayerFX))]
public class Player : MonoBehaviour
{
    public static Player OwnerSingleton { get; private set; }

    [Header("References")] 
    [SerializeField] private CharactersRegistry charactersRegistry;
    [SerializeField] private Transform camPoint;
    [SerializeField] private GroundChecker groundChecker;
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;
    private PlayerHeadRotator _headRotator;
    private Character _characterData;

    public event Action OnKillChange;
    public event Action OnInitilized;

    public Transform CamPoint => camPoint;
    public Gun Gun { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerFX FX { get; private set; }
    public PlayerHealth Health { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerStamina Stamina { get; private set; }
    public PlayerNetwork Network { get; private set; }
    public PlayerNetworkRpcManager RpcManager { get; private set; }
    public PlayerAbilities PlayerAbilities { get; private set; }
    public Rigidbody Rb { get; private set; }
    
    public GameObject Mesh { get; set; }
    public GameObject GunHolder { get; set; }
    public GameObject CharacterSpecial { get; set; }

    public int Kills { get; private set; }
    public bool IsGrounded => groundChecker.Value;
    public bool IsMoving => Rb.velocity.magnitude > 0.1f && IsGrounded;
    public bool IsSprinting { get; private set; }
    public bool IsSneaking { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool CanMove { get; set; } = true;

    public Vector3 MovingVec
    {
        get
        {
            Transform trans = transform;
            return (Controller.MoveVec.x * trans.right) + (Controller.MoveVec.y * trans.forward);
        }
    }

    public Vector3 LookingVec => CamPoint.forward;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        RpcManager = GetComponent<PlayerNetworkRpcManager>();
        Animator = GetComponent<PlayerAnimator>();
        FX = GetComponent<PlayerFX>();
        Health = GetComponent<PlayerHealth>();
        Controller = GetComponent<PlayerController>();
        Stamina = GetComponent<PlayerStamina>();
        Network = GetComponent<PlayerNetwork>();
    }

    private void OnEnable()
    {
        Network.OnPlayerNetworkSpawn += OnNetworkSpawn;
        Network.OnPlayerNetworkDespawn += OnNetworkDespawn;
        Network.CharacterType.OnValueChanged += OnSelectionSet;
    }

    private void OnDisable()
    {
        Network.OnPlayerNetworkSpawn -= OnNetworkSpawn;
        Network.OnPlayerNetworkDespawn -= OnNetworkDespawn;
        Network.CharacterType.OnValueChanged -= OnSelectionSet;
        StopAllCoroutines();
        PlayerAbilities.TerminateAll();
    }

    private void Update()
    {
        LimitVelocity();
    }

    public void Respawn()
    {
        transform.position = SpawnPointsRegistry.Instance.GetRandomSpawnPoint();
        Health.Fill();
        Gun.Ammo.Full();
        RpcManager.SendReviveClientRpc(Network.OwnerClientId);
    }

    public void Move(Vector2 moveVec)
    {
        if (!CanMove) return;
        Transform t = transform;

        float speed = 
            IsSprinting ? stats.MoveSpeed * stats.SprintMultiplier :
            IsSneaking ? stats.MoveSpeed * stats.SneakMultiplier :
            stats.MoveSpeed;

        Vector3 moveX = speed * moveVec.x * t.right;
        Vector3 moveZ = speed * moveVec.y * t.forward;

        Rb.velocity = new Vector3(0, Rb.velocity.y, 0) + moveX + moveZ;
    }

    public void Rotate(Vector2 mouseVec, float sensitivity)
    {
        Vector2 fixedMouseVec = mouseVec * (sensitivity * Time.deltaTime);
        UpdateXRotation(fixedMouseVec);
        UpdateYRotation(fixedMouseVec);
        PlayerCamera.Instance.SmoothFollow(camPoint.transform);
    }

    public void SyncHeadRot()
    {
        if (_headRotator != null) _headRotator.Sync(camPoint.transform.rotation);
    }

    public void Jump()
    {
        if (!IsGrounded) return;
        
        Vector3 velo = Rb.velocity; //reset y velo
        Rb.velocity = new(velo.x, 0, velo.z);
        Rb.AddForce(Vector3.up * stats.JumpForce, ForceMode.Impulse);
    }

    public void Sprint(bool isSprinting)
    {
        IsSprinting = isSprinting;
    }

    public void Sneak(bool isSneaking)
    {
        IsSneaking = isSneaking;
    }

    public void IncreaseKill()
    {
        Kills++;
        OnKillChange?.Invoke();
    }

    public void ResetKills()
    {
        Kills = 0;
        OnKillChange?.Invoke();
    }

    private void LimitVelocity()
    {
        if (Rb.velocity.magnitude > stats.MaxSpeed)
        {
            Rb.velocity = Rb.velocity.normalized * stats.MaxSpeed;
        }
    }

    private void UpdateYRotation(Vector2 fixedMouseVec)
    {
        Quaternion rot = transform.rotation;
        transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + fixedMouseVec.x, rot.eulerAngles.z);
    }

    private void UpdateXRotation(Vector2 fixedMouseVec)
    {
        Quaternion camRot = camPoint.transform.rotation;
        float xRot = camRot.eulerAngles.x;
        if (xRot > 90) xRot -= 360;
        xRot = Mathf.Clamp(xRot - fixedMouseVec.y, -90, 90);
        camPoint.transform.rotation = Quaternion.Euler(xRot, camRot.eulerAngles.y, camRot.eulerAngles.z);
    }
    
    private void OnNetworkSpawn()
    {
        Controller.ControlsDisabled = true;
        StartCoroutine(Init(3));
    }

    private IEnumerator Init(float delay)
    {
        yield return new WaitForSeconds(delay);
        _characterData = charactersRegistry.ProvideWithType(Network.CharacterType.Value);
        _characterData.Init(this);
        
        PlayerAbilities = CharacterSpecial.GetComponent<PlayerAbilities>();
        
        Gun = GunHolder.GetComponentInChildren<Gun>(true);
        Gun.Init(this);
        
        _headRotator = Mesh.GetComponentInChildren<PlayerHeadRotator>();
        
        GunLinker gunLinker = GunHolder.GetComponentInChildren<GunLinker>();
        gunLinker.Head = _headRotator.transform;
        gunLinker.Init();
        
        Health.DisabledObjectsOnDeath.Add(Mesh);
        Health.DisabledObjectsOnDeath.Add(Gun.transform.GetChild(0).gameObject); //TASLEEK :)
        
        Network.Init();
        
        if (Network.IsOwner)
        {
            OwnerSingleton = this;
            KillsUI.Instance.InitUI();
            PingUI.Instance.InitUI();
            HealthUI.Instance.InitUI();
            AmmoUI.Instance.InitUI();
            StaminaUI.Instance.InitUI();
            foreach (AbilityUI ability in AbilityUI.Instances) ability.InitUI();
        }
        
        Controller.ControlsDisabled = false;
        IsInitialized = true;
        OnInitilized?.Invoke();
    }

    private void OnSelectionSet(CharacterType value, CharacterType newValue)
    {
        Debug.Log(value + "->" + newValue);

    }

    private void OnNetworkDespawn()
    {
        if (Network.IsOwner) OwnerSingleton = null;
    }
}
