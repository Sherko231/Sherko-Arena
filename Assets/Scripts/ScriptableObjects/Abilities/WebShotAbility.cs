using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WebShotAbility", menuName = "Abilities/WebShotAbility", order = 0)]
public class WebShotAbility : PlayerAbility
{
    [Title("References")] 
    [SerializeField] private WebProjectile webPrefab;
    [Title("Settings")]
    [SerializeField] private float launchPower = 10f;

    protected override void ExecuteAbility(Player player)
    {
        Vector3 shootPoint = player.CamPoint.position + player.transform.forward * 3;
        Vector3 shootDir = player.LookingVec;
        
        WebProjectile instance = Instantiate(webPrefab, shootPoint, player.transform.rotation);
        instance.ThrowerClientId = player.Network.OwnerClientId;
        instance.Launch(shootDir, launchPower);
    }
}
