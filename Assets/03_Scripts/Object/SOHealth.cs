using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "SO/Item/Effect/Health")]
public class SOHealth : ScriptableObject
{
    public int MaxHP = 100;

    public int Defense = 1;

    public int MoveSpeed = 5;

    public int AttackPower = 10;
}
