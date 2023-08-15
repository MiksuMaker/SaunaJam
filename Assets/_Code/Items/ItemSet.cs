using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemSet")]
public class ItemSet : ScriptableObject
{
    public int water = 0;
    public int wood = 0;
    public int stone = 0;
    public int write = 0;
    public int sauna = 0;
}
