using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class fade : MonoBehaviour
{
    [SerializeField] float fadeDistance, movementSpeed, movementAmplitude;
    [SerializeField] bool isText;
    private PlayableCharacter[] players = new PlayableCharacter[4];
    private PlayableCharacter[] foundPlayers;
    private SpriteRenderer SR;
    private TextMeshPro textMeshPro;
    private Vector3 startPos;

    void Start()
    {
        foundPlayers = FindObjectsOfType<PlayableCharacter>();

        for (int i = 0; i < players.Length; i++)
        {
            if (i < foundPlayers.Length)
            {
                players[i] = foundPlayers[i];
            }
            else
            {
                players[i] = null;
            }
        }

        if(!isText) SR = GetComponent<SpriteRenderer>();
        if(isText) textMeshPro = GetComponent<TextMeshPro>();
        startPos = transform.position;
    }

    void Update()
    {
        FadeWithDistance();
        FloatEffect();
    }

    void FadeWithDistance()
    {
        float closestDistance = ClosestPlayerDistance();
        float alpha = Mathf.InverseLerp(fadeDistance, 0, closestDistance);
        alpha = Mathf.Clamp(alpha, 0, 1);

        if (isText)
        {
            Color color = textMeshPro.color;
            color.a = alpha;
            textMeshPro.color = color;
        }
        else if (!isText)
        {
            Color color = SR.color;
            color.a = alpha;
            SR.color = color;
        }
    }

    void FloatEffect()
    {
        Vector3 pos = startPos;
        pos.y += Mathf.Sin(Time.time * movementSpeed) * movementAmplitude;
        transform.position = pos;
    }

    float ClosestPlayerDistance ()
    {
        float closestDistance = float.MaxValue;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                float distance = Vector3.Distance(players[i].transform.position, transform.position);
                if (distance < closestDistance) closestDistance = distance;
            }
        }

        return closestDistance;
    }
}
