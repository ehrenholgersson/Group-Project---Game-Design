using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static GameObject[] Player{ get; private set;}

    private void OnEnable()
    {
        Player = new GameObject[4] 
        {
        transform.Find("Player1").gameObject,
        transform.Find("Player2").gameObject,
        transform.Find("Player3").gameObject,
        transform.Find("Player4").gameObject
        };
    }
}
