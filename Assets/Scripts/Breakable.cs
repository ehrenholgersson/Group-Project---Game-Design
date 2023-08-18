using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamage
{
    [SerializeField] float _health = 10;

    public void ApplyDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
            Destroy(gameObject);
    }
}
