using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchingBag : MonoBehaviour, IDamage
{
    [SerializeField] GameObject HitText;
    public void ApplyDamage(float damage)
    {
        Debug.Log(gameObject.name + " is hit for " + damage + " damage.");
        GameObject damageValue = Instantiate(HitText);
        damageValue.transform.position = transform.position + new Vector3(0,0.6f,0);
        damageValue.transform.GetComponentInChildren<Text>().text = damage.ToString();
    }
}
