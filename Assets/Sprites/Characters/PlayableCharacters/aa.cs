using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.Collections;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class aa : MonoBehaviour
{
    // script to autofill spritelibraryasset inherit category labels with the correct sprites

    public Sprite[] sprites;
    public SpriteLibraryAsset spriteLibraryAsset;

    void Start()
    {
        SpriteLibraryAsset newLibraryAsset = Instantiate(spriteLibraryAsset);
        Replace(newLibraryAsset);
        spriteLibraryAsset = newLibraryAsset;
        spriteLibraryAsset.name = spriteLibraryAsset.name + "New";
        //save the asset
        string path = "Assets/Sprites/Characters/PlayableCharacters/" + spriteLibraryAsset.name + ".asset";
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Replace(SpriteLibraryAsset sLA)
    {
        foreach (string category in sLA.GetCategoryNames())
        {
            Debug.Log(category);
            foreach (string label in sLA.GetCategoryLabelNames(category))
            {
                Debug.Log("changing " + label + " to " + sprites[0]);
                //find the label number in the sprite array
                sLA.RemoveCategoryLabel(category ,label, false);
                sLA.AddCategoryLabel(sprites[0], category, label);
            }
        }
    }
}
