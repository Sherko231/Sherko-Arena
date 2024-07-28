using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector Instance { get; private set; }
    public CharacterType Selection { get; set; } = CharacterType.Breeze;
    
    private void Awake() => Instance = this;
}
