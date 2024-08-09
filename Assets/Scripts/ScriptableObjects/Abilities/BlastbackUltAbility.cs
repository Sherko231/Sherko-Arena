using System.Collections;
using Sherko.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "BlastbackUltAbility", menuName = "Abilities/BlastbackUltAbility", order = 0)]
public class BlastbackUltAbility : PlayerAbility
{
    [SerializeField] private float ultDamageMultiplier = 3f;
    [SerializeField] private float ultVignetteIntensity, ultVignetteSmoothness;
    [SerializeField] private Color ultVignetteColor = new(1f, 0.58f, 0.02f);
    [Title("References")]
    [SerializeField] private VolumeProfile volume;
    
    private float _initVignetteIntensity, _initVignetteSmoothness;
    private Color _initVignetteColor;

    public override bool CanBeUsed(PlayerStamina stamina)
    {
        Consumer consumer = stamina.Consumer;
        ShotgunRecoilHandler recoilHandler = stamina.Player.CharacterSpecial.GetComponent<ShotgunRecoilHandler>();
        bool hasEnoughStamina = consumer.Current >= staminaCost;
        return hasEnoughStamina && CanUse && !recoilHandler.IsUlt;
    }

    protected override void ExecuteAbility(Player player)
    {
        player.FX.BlastbackUltVFX.Play();
        if (player.Network.IsOwner)
        {
            ShotgunRecoilHandler recoilHandler = player.CharacterSpecial.GetComponent<ShotgunRecoilHandler>();
            recoilHandler.IsUlt = true;
            
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
        
        player.FX.BlastbackUltVFX.Stop();
        ShotgunRecoilHandler recoilHandler = player.CharacterSpecial.GetComponent<ShotgunRecoilHandler>();
        recoilHandler.IsUlt = false;
        
        base.TerminateAbility(player);
    }
}
