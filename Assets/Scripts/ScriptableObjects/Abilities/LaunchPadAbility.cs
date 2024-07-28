using UnityEngine;

[CreateAssetMenu(fileName = "LaunchPadAbility", menuName = "Abilities/LaunchPadAbility", order = 0)]
public class LaunchPadAbility : PlayerAbility
{
    [Header("Settings")]
    [SerializeField] private float placementDistance = 6f;
    [SerializeField] private LayerMask nonPlayersLayers;
    [Header("References")]
    [SerializeField] private GameObject launchpadPrefab;
    
    protected override void ExecuteAbility(Player player)
    {
        Ray placementRay = new(player.CamPoint.position, player.LookingVec);
        bool isHitGround = Physics.Raycast(placementRay, out RaycastHit hit, placementDistance, nonPlayersLayers);

        Vector3 launchpadSpawnPos;
        if (isHitGround) launchpadSpawnPos = hit.point;
        else launchpadSpawnPos = player.CamPoint.position + (player.LookingVec * placementDistance);
        Instantiate(launchpadPrefab, launchpadSpawnPos, Quaternion.identity);
    }
}
