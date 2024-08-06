using Sherko.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "SpideyUltAbility", menuName = "Abilities/SpideyUltAbility")]
public class SpideyUltAbility : PlayerAbility
{
    [Title("VFX Settings")]
    [SerializeField] private VolumeProfile volume;
    [SerializeField] private float ultVignetteIntensity, ultVignetteSmoothness;
    [SerializeField] private Color ultVignetteColor = Color.magenta;
    
    private float _initVignetteIntensity, _initVignetteSmoothness;
    private Color _initVignetteColor;

    public override bool CanBeUsed(PlayerStamina stamina)
    {
        Consumer consumer = stamina.Consumer;
        bool hasEnoughStamina = consumer.Current >= staminaCost;
        return hasEnoughStamina && CanUse && !stamina.IsUnlimited;
    }

    protected override void ExecuteAbility(Player player)
    {
        player.FX.SpideyUltVfx.Play();
        if (player.Network.IsOwner)
        {
            player.Stamina.IsUnlimited = true;
            if (volume.TryGet(out Vignette vignette))
            {
                _initVignetteIntensity = vignette.intensity.value;
                _initVignetteSmoothness = vignette.smoothness.value;
                _initVignetteColor = vignette.color.value;
                vignette.intensity.value = ultVignetteIntensity;
                vignette.smoothness.value = ultVignetteSmoothness;
                vignette.color.value = ultVignetteColor;
            }
        }
        
    }

    public override void TerminateAbility(Player player)
    {
        if (player.Network.IsOwner && volume.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = _initVignetteIntensity;
            vignette.smoothness.value = _initVignetteSmoothness;
            vignette.color.value = _initVignetteColor;
        }
        player.Stamina.IsUnlimited = false;
        player.FX.SpideyUltVfx.Stop();
        base.TerminateAbility(player);
    }
    
}
