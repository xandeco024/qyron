using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    public new string name;
    public Color color;
    public float maxHealth;
    public float resistance;
    public float damageReduction;
    public float attackDamage;
    public float criticalChance;
    public float speed;
    public float dodgeChance;
    public float moveSpeed;
    public float jumpForce;
    public float respect;
    public Sprite sprite;
}
