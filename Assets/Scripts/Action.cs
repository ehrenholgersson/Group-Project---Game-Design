using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "ProductiveBludgers/Action")]
public class Action : ScriptableObject
{
    #region Enumerators
    public enum ActionType {Melee,Projectile,Movement,Animation}
    public enum DirectionType { FacingDirection, InputDirection, Custom }
    public enum MovementType { AddForce, MoveToPoint, Dodge }
    public enum State { Airborne, Grounded, Both}

    public enum KBType { FacingDirection, Away, Custom }

    #endregion 

    #region General Values
    public string Name;
    public ActionType Type;
    public float Damage;
    public float KnockBack;
    public KBType KBDirection = KBType.FacingDirection;
    public Vector2 CustomKnockBackDirection = Vector2.right;
    public float AttackTime;
    public float StartDelay;
    public float EndDelay;
    public DirectionType Direction;
    public Vector2 CustomDirection;
    public bool Busy = true;
    public State PlayerState = State.Both;
    public int MaxCombo;
    #endregion

    #region Melee Specific Values
    public List<Vector2> HitPoints;
    public float HitSize = 0.2f;
    #endregion

    #region Projectile Specific Values
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed;
    public Vector2 ProjectileStart;
    #endregion

    #region Movement Specific Values
    public MovementType Movement;
    public Vector2 Force;
    public Vector2 Destination;
    public float Speed; // could probs use same float for movement and projectile speed, but doesn't exactly matter

    #endregion

    #region Animation Specific Values
    public string AnimationName;
    #endregion
}
