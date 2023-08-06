using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Character", menuName = "ProductiveBludgers/Character")]
public class Character : ScriptableObject
{
    public string Name;
    public float Health;
    public float Speed;
    public float Mass;
    public List<Action> MovementAction;
    public List<Action> Action1;
    public List<Action> Action2;
    public List<Action> Action3;
    public List<Action> Action4;
    public List<Action> Action5;
    public List<Action> Action6;
    public bool DoubleJump;
}
