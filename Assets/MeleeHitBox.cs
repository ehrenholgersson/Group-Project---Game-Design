using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    SpriteRenderer _indicator;
    CircleCollider2D _circleCollider;
    public void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _indicator = GetComponent<SpriteRenderer>();
    }

/*    public void Update()
    {

    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Character") && collision.gameObject != transform.parent.gameObject)
        {
            if (TryGetComponent<IDamage>(out IDamage target))
                target.ApplyDamage(1);
        }
    }
}
