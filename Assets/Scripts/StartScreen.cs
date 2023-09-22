using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class StartScreen : MonoBehaviour
{
    private IDisposable _eventListener;
    GameController _mController;

    void OnEnable()
    {
        _eventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
    }

    void OnDisable()
    {
        _eventListener.Dispose();
    }

    void OnButtonPressed(InputControl button)
    {
        Debug.Log(button.name);
    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {           

    }
}
