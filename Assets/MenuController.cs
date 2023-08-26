using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics.Tracing;
//using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.TextCore.Text;

public class MenuController : MonoBehaviour
{
    Character[] _characters;
    GameObject[] _levels;
    [SerializeField] TextMeshProUGUI _levelList;
    [SerializeField] TextMeshProUGUI _characterList;
    [SerializeField] GameObject[] _menus;
    int _selectedmenu = 0;
    int[,] _playerSelection = new int[2, 4];
    int _players;
    public static int Players = 0;
    SpriteRenderer[,] _levelBox;

    private IDisposable _eventListener;
    // Start is called before the first frame update
#region Menu Controls

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
        if (_selectedmenu == 0)
        {
            ChangeMenu(1);
        }
        // check if player was spawned
        if (Players < _players)
        {
            _players++;
        }
        //if (button.name == )
    }

    void ChangeMenu(int index)
    {
        for (int i = 0; i < _menus.Length; i++)
        {
            if (i == index)
            {
                _menus[i].gameObject.SetActive(true);
                _selectedmenu = i;
            }
            else
            {
                _menus[i].gameObject.SetActive(false);
            }
        }
        
    }

    void MenuDown(int player)
    {
        for (int x = 0;x<_playerSelection.GetLength(0);x++)
            for (int y = 0;y<_playerSelection.GetLength(1);y++)
            {
                if (_playerSelection[x,y] == player)
                {
                    _playerSelection[x , y] = 0;
                    _playerSelection[x--% _playerSelection.GetLength(0), y] = player;
                    UpdateSelection();
                    return;
                }
            }
    }
    void MenuUp(int player)
    {
        for (int x = 0; x < _playerSelection.GetLength(0); x++)
            for (int y = 0; y < _playerSelection.GetLength(1); y++)
            {
                if (_playerSelection[x, y] == player)
                {
                    _playerSelection[x, y] = 0;
                    _playerSelection[x++ % _playerSelection.GetLength(0), y] = player;
                    UpdateSelection();
                    return;
                }
            }
    }
    void MenuLeft(int player)
    {
        for (int x = 0; x < _playerSelection.GetLength(0); x++)
            for (int y = 0; y < _playerSelection.GetLength(1); y++)
            {
                if (_playerSelection[x, y] == player)
                {
                    if (x == 0 && _selectedmenu ==1)
                    {
                        // previous character 
                    }
                    else if (x==1 && _selectedmenu ==1)
                    {
                        //previous level
                    }
                }
            }
    }
    void MenuRight(int player)
    {
        for (int x = 0; x < _playerSelection.GetLength(0); x++)
            for (int y = 0; y < _playerSelection.GetLength(1); y++)
            {
                if (_playerSelection[x, y] == player)
                {
                    if (x == 0 && _selectedmenu == 1)
                    {
                        // next character 
                    }
                    else if (x == 1 && _selectedmenu == 1)
                    {
                        //next level
                    }
                }
            }
    }
    void MenuSelect(int player)
    {
        
    }
    void MenuBack(int player)
    {

    }
    void UpdateSelection()
    { 

    }


    #endregion


    void Start()
    {
        _levelBox = new SpriteRenderer[4, 4];
        for (int i = 1;i<5;i++)
        {
            GameObject box = GameObject.Find("LevelSelectBox" + i);
            //for (int j = 1; j < 3; j++)
            int j = 0;
            foreach (SpriteRenderer s in  box.GetComponentsInChildren<SpriteRenderer>())
            {
                _levelBox[i, j] = s;
                j++;
            }

        }
        ChangeMenu(0);

        UnityEngine.Object[] loadArray = Resources.LoadAll("Characters", typeof(Character));
        _characters = new Character[loadArray.Length];
        loadArray.CopyTo( _characters, 0 );
        Debug.Log("Found " + loadArray.Length + " characters");
        //_characters = (Resources.LoadAll("Characters/Selectable", typeof(Character)) as Character[]).ToList();
        loadArray = Resources.LoadAll("", typeof(GameObject));
        _levels = new GameObject[loadArray.Length];
        loadArray.CopyTo(_levels, 0 );

        Debug.Log("Found " + loadArray.Length + " Levels");

        foreach (Character character in _characters)
        {
            _characterList.text += character.name + "<br>";
        }
        foreach (GameObject level in _levels)
        {
            _levelList.text += level.name + "<br>";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
