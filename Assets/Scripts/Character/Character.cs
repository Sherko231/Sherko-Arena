using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 0)]
public class Character : ScriptableObject
{
    [EnumToggleButtons] [SerializeField] private CharacterType type;
    [SerializeField] private GameObject mesh;
    [SerializeField] private GameObject gunHolder;
    [SerializeField] private GameObject characterSpecial;

    public CharacterType Type => type;

    public void Init(Player player)
    {
        Transform playerTrans = player.transform;
        player.Mesh = Instantiate(mesh, playerTrans);
        player.GunHolder = Instantiate(gunHolder, playerTrans);
        player.CharacterSpecial = Instantiate(characterSpecial, playerTrans);
    }

}
