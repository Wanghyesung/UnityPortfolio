using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eObjectStat
{
    HP,
    MP,
    Speed,
    Defend,
    Attack,
    None,
}

public class ObjectInfo : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int hp;
    [SerializeField] private int maxHp;
   
    public void AddHP(int _value)
    {
        hp += _value;
        if (hp >= maxHp)
            hp = maxHp;
        else if(hp <=0)
            hp = 0;
    }
    public void AddMaxHP(int _value)
    {
        maxHp += _value;
    }
    public void RestHP()
    {
        hp = maxHp;
    }
   

    public int HP => hp;
    public int MaxHp => maxHp;

    [Header("MP")]
    [SerializeField] private int mp;
    [SerializeField] private int maxMp;

    public int MP => mp;
    public int MaxMp => maxMp;

    public void AddMP(int _value)
    {
        mp += _value;
        if (mp >= maxMp)
            mp = maxMp;
        else if (mp <= 0)
            mp = 0;
    }
    public void RestMP()
    {
        mp = maxMp;
    }


    [Header("Speed")]
    [SerializeField] private int speed;
    [SerializeField] private int maxSpeed;
   


    public int Speed => speed;
    public int MaxSpeed => maxSpeed;

    public void AddSpeed(int _value)
    {
        speed += _value;
        if (speed >= maxSpeed)
            speed = maxSpeed;
        else if (speed <= 0)
            speed = 1;
    }


    [Header("Defense")]
    [SerializeField] private int defense;
    [SerializeField] private int maxDefense;
    public void AddDefense(int _value)
    {
        defense += _value;
        if (defense >= maxDefense)
            defense = maxDefense;
        else if (defense <= 0)
            defense = 1;
    }

    public int Defense => defense;
    public int MaxDefense => maxDefense;

    [Header("Attack")]
    [SerializeField] private int attack;
    [SerializeField] private int maxAttack;

    public int Attack => attack;
    public int MaxAttack => maxAttack;

    public void UpAttack(int _value)
    {
        attack += _value;
        if(attack >= maxAttack)
            attack = maxAttack; 
    }


    public int GetStatValue(eObjectStat _eStat)
    {
        switch (_eStat)
        {
            case eObjectStat.HP:
                return maxHp;
            case eObjectStat.MP:
                return maxMp;
            case eObjectStat.Speed:
                return speed;
            case eObjectStat.Defend:
                return defense;
            case eObjectStat.Attack:
                return attack;
            default:
                return 0;
        }
    }

}
