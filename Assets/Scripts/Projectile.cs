using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D _rb;
    Vector2 _velocity;
    //float _speed;
    float _damage;
    float _knockback;
    float _lifeTime;

    public void Start()
    {
        if (_lifeTime==0)
            _lifeTime = Time.time + 4;
    }
    public void setup(Vector2 direction,float speed,float damage,float knockback,float scale)
    {
        _velocity = direction * speed;
        if (gameObject.TryGetComponent<Rigidbody2D>(out _rb))
            _rb.velocity = _velocity;
    }

    public void setup(Vector2 direction, float speed)
    {
        _velocity = direction*speed;
        if(gameObject.TryGetComponent<Rigidbody2D>(out _rb))
            _rb.velocity = _velocity;
    }
    private void Update()
    {
        // may not require rigidbody if projectile is just meant to move in a line, if so we move it here
        if (_rb == null) 
        {
            transform.position += new Vector3(_velocity.x,_velocity.y,0) * Time.deltaTime;
        }
        if (Time.time > _lifeTime)
            Destroy(gameObject);
    }
}
