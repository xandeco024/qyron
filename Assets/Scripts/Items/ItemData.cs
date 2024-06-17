using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewItem", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite sprite;
    public GameObject textPrefab;

    [Header("Buffs")]
    public int health;
}
