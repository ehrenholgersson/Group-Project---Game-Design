using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float _damageMultiplier, _knockBack;
    Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) //applt damage based on magnitude of attached rigidbody velocity projected along direction to the collider
    {
        float impulse = Vector3.Project(_rb.velocity,(collision.transform.position - transform.position).normalized).magnitude;
        if ( collision.collider.TryGetComponent<IDamage>(out IDamage target)&&impulse > 0)
        {
            float damage = Mathf.Floor(impulse * _damageMultiplier);
            target.ApplyDamage(damage);
            Debug.Log("SpikeBall dealt " + (impulse * _damageMultiplier) + " damage to " + collision.collider.gameObject.name);
            if (damage > 0)
                if (collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRB))
                {
                    Debug.Log("pushing player");
                    targetRB.AddForce((targetRB.worldCenterOfMass - _rb.worldCenterOfMass).normalized * _knockBack);
                }
        }
    }
}
