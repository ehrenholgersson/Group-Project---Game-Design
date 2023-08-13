using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Action))]
[CanEditMultipleObjects]
public class ActionEditor : Editor
{
    SerializedProperty Name;
    SerializedProperty Type;
    SerializedProperty HitPoints;
    SerializedProperty AttackTime;
    SerializedProperty HitSize;

    SerializedProperty StartDelay;
    SerializedProperty EndDelay;
    SerializedProperty ProjectilePrefab;
    SerializedProperty ProjectileSpeed;
    SerializedProperty Direction;
    SerializedProperty CustomDirection;
    SerializedProperty StartPosition;
    SerializedProperty Busy;

    SerializedProperty Movement;
    SerializedProperty Force;
    SerializedProperty Destination;
    SerializedProperty Speed;

    SerializedProperty MaxCombo;
    SerializedProperty Damage;
    SerializedProperty KnockBack;
    SerializedProperty KnockBackDirection;

    SerializedProperty PlayerState;
    SerializedProperty AnimationName;

    private void OnEnable()
    {
        Name = serializedObject.FindProperty("Name"); 
        Type = serializedObject.FindProperty("Type");
        HitPoints = serializedObject.FindProperty("HitPoints");
        AttackTime = serializedObject.FindProperty("AttackTime");
        HitSize = serializedObject.FindProperty("HitSize");
        StartDelay = serializedObject.FindProperty("StartDelay");
        EndDelay = serializedObject.FindProperty("EndDelay");
        ProjectileSpeed = serializedObject.FindProperty("ProjectileSpeed");
        ProjectilePrefab = serializedObject.FindProperty("ProjectilePrefab");
        Direction = serializedObject.FindProperty("Direction");
        CustomDirection = serializedObject.FindProperty("CustomDirection");
        StartPosition = serializedObject.FindProperty("ProjectileStart");
        Busy = serializedObject.FindProperty("Busy");
        MaxCombo = serializedObject.FindProperty("MaxCombo");
        Damage = serializedObject.FindProperty("Damage");
        KnockBack = serializedObject.FindProperty("KnockBack");
        KnockBackDirection = serializedObject.FindProperty("KnockBackDirection");

        AnimationName = serializedObject.FindProperty("AnimationName");

        Movement = serializedObject.FindProperty("Movement");
        Force = serializedObject.FindProperty("Force");
        Destination = serializedObject.FindProperty("Destination");
        Speed = serializedObject.FindProperty("Speed");
        PlayerState = serializedObject.FindProperty("PlayerState");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(Name);
        if (Type.enumNames[Type.intValue] != "Animation")
        {
            EditorGUILayout.PropertyField(Busy);
        }
        EditorGUILayout.PropertyField(PlayerState);
        EditorGUILayout.PropertyField(Type);
        switch (Type.enumNames[Type.intValue])
        {
            case "Melee":
                EditorGUILayout.PropertyField(Damage);
                EditorGUILayout.PropertyField(KnockBack);
                EditorGUILayout.PropertyField(KnockBackDirection);
                EditorGUILayout.PropertyField(MaxCombo);
                EditorGUILayout.PropertyField(HitPoints);
                EditorGUILayout.PropertyField(AttackTime);
                EditorGUILayout.PropertyField(StartDelay);
                EditorGUILayout.PropertyField(EndDelay);
                EditorGUILayout.PropertyField(HitSize);
                break;
            case "Projectile":
                EditorGUILayout.PropertyField(Damage);
                EditorGUILayout.PropertyField(KnockBack);
                EditorGUILayout.PropertyField(KnockBackDirection);
                EditorGUILayout.PropertyField(MaxCombo);
                EditorGUILayout.PropertyField(StartDelay);
                EditorGUILayout.PropertyField(EndDelay);
                EditorGUILayout.PropertyField(ProjectileSpeed);
                EditorGUILayout.PropertyField(ProjectilePrefab);
                EditorGUILayout.PropertyField(StartPosition);
                EditorGUILayout.PropertyField(Direction);
                if (Direction.enumNames[Direction.intValue]=="Custom")
                    EditorGUILayout.PropertyField(CustomDirection);
                break;
            case "Movement":
                EditorGUILayout.PropertyField(StartDelay);
                
                EditorGUILayout.PropertyField(EndDelay);
                EditorGUILayout.PropertyField(Movement);
                switch(Movement.enumNames[Movement.intValue])
                {
                    case "AddForce":
                        EditorGUILayout.PropertyField(Force);
                        break;
                    case "MoveToPoint":
                        EditorGUILayout.PropertyField(Destination);
                        EditorGUILayout.PropertyField(AttackTime);
                        break;
                    case "Dodge":
                        EditorGUILayout.PropertyField(AttackTime);
                        break;
                }
                break;
            case "Animation":
                EditorGUILayout.PropertyField(AnimationName);
                EditorGUILayout.PropertyField(AttackTime);
                break;

        }
        serializedObject.ApplyModifiedProperties();

    }
}



    // enum ActionType {Melee,Projectile,Movement}
    //[SerializeField] string Name;
    //[SerializeField] ActionType Type;

    //#region General Values
    //[SerializeField] float StartDelay;
    //[SerializeField] float EndDelay;
    //#endregion

    //#region Melee Specific Values
    //[ConditionalField("Type", ActionType.Melee)] List<Vector2> HitPoints;
    //[SerializeField] float AttackTime;
    //[SerializeField] float HitSize;
    //#endregion

    //#region Projectile Specific Values
    //[SerializeField] GameObject ProjectilePrefab;
    //[SerializeField] float ProjectileSpeed; 
 