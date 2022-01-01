using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouveau mot", menuName = "Mot")]
public class Mot : ScriptableObject
{
   /* public enum Difficulty
    {
        easy = 0,
        medium = 1,
        hard = 2
    }*/

    public List<Graphème> graphèmes;

    public Sprite image;

    public AudioClip audio;
}
