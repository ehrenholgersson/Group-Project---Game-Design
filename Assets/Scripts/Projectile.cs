using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D _rb;
    Vector2 _velocity;
    float _damage;
    float _knockback;
    Vector2 _knockBackDirection;
    float _lifeTime;
    float _comboTimer;
    float _comboCount;
    GameObject _owner;
    CircleCollider2D _circleCollider;

    public void Start()
    {
        if (_lifeTime==0)
            _lifeTime = Time.time + 4;
        _circleCollider = GetComponent<CircleCollider2D>();
    }
    public void setup(Vector2 direction,float speed,float damage,float knockback,Vector2 knockBackDirection, float scale,GameObject owner)
    {
        _velocity = direction * speed;
        if (gameObject.TryGetComponent<Rigidbody2D>(out _rb))
            _rb.velocity = _velocity;
        _knockback = knockback;
        _knockBackDirection = knockBackDirection;
        _damage = damage;   
        _owner = owner;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Character") && collision.gameObject != _owner)
        {
            if (collision.gameObject.TryGetComponent<IDamage>(out IDamage target))
            {
                target.ApplyDamage(_damage);
                if (collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRb))
                    //targetRb.AddForce((targetRb.position - new Vector2(transform.parent.position.x, transform.parent.position.y)).normalized * KnockBack);
                    targetRb.AddForce(_knockBackDirection * _knockback);
                _circleCollider.enabled = false;
                _comboTimer = Time.time;
                if (_comboCount<=1)
                    Destroy(gameObject);
            }
        }
    }
}
