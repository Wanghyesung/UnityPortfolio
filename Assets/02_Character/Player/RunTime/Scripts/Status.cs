using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackInfo
{
    public float Damage;
    public float AttackVariance = 0.0f;
    public float Power;
    public Vector3 AttackerPosition;
    public Vector3 HitPoint;
    public bool Down;

    public SOAudio HitSound = null;
}

public interface IHealth
{
    public int CurrentHP { get; }
    public int MaxHP { get; }

    public void TakeDamage(AttackInfo _pAttackInfo);
    public void Heal(int _iAmount);
    public bool IsDead();
}



