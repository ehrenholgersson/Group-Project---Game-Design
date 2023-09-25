using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    SpriteRenderer _indicator;
    CircleCollider2D _circleCollider;
    float _comboTimer;
    [SerializeField] float _comboDelay = 0.1f;
    public float ComboCount;
    public float Damage;
    public float KnockBack;
    public Vector2 KnockBackDirection;
    public bool KnockAway;
    public void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _indicator = GetComponent<SpriteRenderer>();
        if (!Application.isEditor)
        {
            _indicator.enabled = false;
        }

    }

  public void Update()
    {
        if (!_circleCollider.enabled&&Time.time > _comboTimer+ _comboDelay && ComboCount > 1)
        {
            ComboCount--;
            _circleCollider.enabled = true;
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (/*collision.gameObject.tag.Contains("Character") && */collision.gameObject != transform.parent.gameObject)
        //{
            if (collision.gameObject != transform.parent.gameObject && collision.gameObject.TryGetComponent<IDamage>(out IDamage target))
            {
                target.ApplyDamage(Damage);
            if (collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRb))
            {
                if (!KnockAway)
                    targetRb.AddForce(KnockBackDirection * KnockBack);
                else
                {
                    if (transform.parent.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rB))
                        targetRb.AddForce((targetRb.worldCenterOfMass - rB.worldCenterOfMass).normalized * KnockBack);
                    else
                        Debug.Log("rigidbody not found");
                }
                
            }
                _circleCollider.enabled = false;
                _comboTimer = Time.time;
            }
       // }
    }
}
