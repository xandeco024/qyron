using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayableCharacterData", menuName = "PlayableCharacterData")]
public class PlayableCharacterData : CharacterData
{
    public Color color;

    [Header("Sprites")]
    public Sprite[] idle1;
    public Sprite[] idle2;
    public Sprite[] running;
    public Sprite[] jumping;
    public Sprite[] falling;
    public Sprite[] dash;
    public Sprite[] lightAttack1;
    public Sprite[] lightAttack2;
    public Sprite[] heavyAttack1;
    public Sprite[] heavyAttack2;
    public Sprite[] grabIdle;
    public Sprite[] grabWalking;
    public Sprite[] downed;
    public Sprite[] damage;



    [Header("Light Combos")]
    public Sprite[] LLLLCombo;
    public Sprite[] HLLLCombo;
    public Sprite[] HHLLCombo;
    public Sprite[] HHHLCombo;
    public Sprite[] LLHLCombo;
    public Sprite[] LHHLCombo;



    [Header("Heavy Combos")]
    public Sprite[] HHHHCombo;
    public Sprite[] LHHHCombo;
    public Sprite[] HHLHCombo;
    public Sprite[] HLLHCombo;
    public Sprite[] LLLHCombo;
    public Sprite[] LLHHCombo;

    [Header("Grab Combos")]
    public Sprite[] GLCombo;
    public Sprite[] GHCombo;
    public Sprite[] HGHCombo;
    public Sprite[] LGLCombo;
    public Sprite[] HHGHCombo;
    public Sprite[] LHGLCombo;
    public Sprite[] LLGLCombo;
}
