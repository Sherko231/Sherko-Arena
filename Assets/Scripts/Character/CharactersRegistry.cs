using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "CharacterRegistry", menuName = "CharacterRegistry", order = 0)]
public class CharactersRegistry : ScriptableObject
{
    [SerializeField] private Character[] characters;

    public Character ProvideWithType(CharacterType type)
    {
        Character target = null;
        foreach (Character character in characters.Where(character => character.Type == type))
        {
            target = character;
        }

        return target;
    }
}
