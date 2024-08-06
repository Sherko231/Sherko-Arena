using Sherko.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerFX : MonoBehaviour
{
    [Title("Breeze")] 
    [SerializeField] private ParticleSystem wallRunVFX;
    [SerializeField] private ParticleSystem breezeUltVFX;
    [Title("Blastback")]
    [SerializeField] private ParticleSystem blastbackUltVFX;
    [Title("Spidey")]
    [SerializeField] private ParticleSystem spideyUltVFX;
    [Title("SprintFX")]
    [SerializeField] private float fovChangeSpeed = 2f;
    [SerializeField] private float fovChangeMultiplier = 1.3f;
    private ValueTransferer _sprintTransferer;
    
    private Player _player;

    public ParticleSystem BreezeUltVFX => breezeUltVFX;
    public ParticleSystem BlastbackUltVFX => blastbackUltVFX;
    public ParticleSystem SpideyUltVfx => spideyUltVFX;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _sprintTransferer = new(PlayerCamera.Cam.m_Lens.FieldOfView, fovChangeSpeed);
    }

    private void Update()
    {
        if (_player.Network.IsOwner) ToggleSprintFX(_player.IsSprinting);    
    }

    private void ToggleSprintFX(bool isSprinting)
    {
        PlayerCamera.Cam.m_Lens.FieldOfView = isSprinting ? _sprintTransferer.SmoothTransfer(_sprintTransferer.InitValue * fovChangeMultiplier) : _sprintTransferer.SmoothReset();
    }
    
    public void ToggleWallRunVFX(bool toggled)
    {
        if (toggled) wallRunVFX.Play();
        else wallRunVFX.Stop();
    }
}
