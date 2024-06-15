using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewPlayableCharacterData", menuName = "PlayableCharacterData")]
public class PlayableCharacterData : CharacterData
{
    public Color color;
    public SpriteLibraryAsset spriteLibraryAsset;
}
