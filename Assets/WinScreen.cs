using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    private TextMeshProUGUI _text;

    // Start is called before the first frame update
    void OnEnable()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SetWinner(int PlayerIndex)
    {
        switch (PlayerIndex)
        {
            case 1:
                _text.text = "Player One Wins";
                _text.color = Color.red;
                break;
            case 2:
                _text.text = "Player Two Wins";
                _text.color = Color.blue;
                break;
            case 3:
                _text.text = "Player Three Wins";
                _text.color = Color.green;
                break;
            case 4:
                _text.text = "Player Three Wins";
                _text.color = Color.green;
                break;
        }
    }

    private void Update()
    {
        
    }
}
