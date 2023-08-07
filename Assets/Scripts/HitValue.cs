using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HitValue : MonoBehaviour
{
    Text _text;
    float _speed = 3;
    float _timer;
    Vector3 _direction;
    // Start is called before the first frame update
    void Start()
    {
        _text = transform.GetComponentInChildren<Text>();
        _timer = Time.time;
        _direction = new Vector3(Random.Range(-0.4f, 0.4f),1,0).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Lerp(1, 0, (Time.time - _timer) / 2));
        transform.position += _direction * _speed * Time.deltaTime;
        if (Time.time > _timer + 2)
            Destroy(gameObject);
    }
}
