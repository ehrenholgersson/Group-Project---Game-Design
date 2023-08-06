using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBag : MonoBehaviour, IDamage
{
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void ApplyDamage(float damage)
    {
        Debug.Log(gameObject.name + " is hit for " + damage + " damage.");
    }
}
