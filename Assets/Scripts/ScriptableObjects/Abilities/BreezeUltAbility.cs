using System.Collections;
using Sherko.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "BreezeUltAbility", menuName = "Abilities/BreezeUltAbility")]
public class BreezeUltAbility : PlayerAbility
{
    [SerializeField] [Tooltip("In Sec")] 
    private float duration = 10f;
    [Title("VFX Settings")]
    [SerializeField] private VolumeProfile volume;
    [SerializeField] private float ultVignetteIntensity, ultVignetteSmoothness;
    [SerializeField] private Color ultVignetteColor = Color.cyan;
    
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
        player.FX.BreezeUltVFX.Play();
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

        player.StartCoroutine(StartAbilityTimer(player));
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
        player.FX.BreezeUltVFX.Stop();
        base.TerminateAbility(player);
    }

    private IEnumerator StartAbilityTimer(Player player)
    {
        yield return new WaitForSeconds(duration);
        TerminateAbility(player);
    }
}
