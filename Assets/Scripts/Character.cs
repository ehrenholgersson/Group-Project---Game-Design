using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Character", menuName = "ProductiveBludgers/Character")]
public class Character : ScriptableObject
{
    public string Name;
    public float Health = 100;
    public float Speed;
    public float Mass;
    public GameObject Appearance;
    public List<Action> MovementInput;
    public List<Action> Input1;
    public List<Action> Input2;
    public List<Action> Input3;
    public List<Action> Input4;
    public List<Action> Input5;
    public List<Action> Input6;
    public bool DoubleJump = true;
}
