// Assets/Scripts/Characters/CharacterDefinition.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterDefinition",menuName = "Characters/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    public string characterName;
    public GameObject characterPrefab;   // <‑‑ your EXISTING prefab
    public float baseMoveSpeed = 5;
    public float baseJumpHeight = 5;
    public Vector2 baseWallJumpingPower = new Vector2(4f, 8f);
    public float baseMaxWallJumps = 4f; // Maximum number of wall jumps
    public float baseGravityScale = 2f; // Gravity scale for the player


    [TextArea] public string description;
    public Sprite portrait;
}
