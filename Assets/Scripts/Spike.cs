using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] float _damage, _knockBack;
    Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<IDamage>(out IDamage target))
        {
            target.ApplyDamage(_damage);
            if (collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRB))
                targetRB.AddForce((targetRB.worldCenterOfMass - _rb.worldCenterOfMass).normalized * _knockBack);
        }
    }
}
