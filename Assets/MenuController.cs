using System;
using System.Collections.Generic;
using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics.Tracing;
//using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    Character[] _characters;
    GameObject[] _levels;
    [SerializeField] TextMeshProUGUI _levelList;
    [SerializeField] TextMeshProUGUI _characterList;
    [SerializeField] GameObject[] _menus;
    int _selectedmenu = 0;
    List<int>[] _playerSelection = new List<int>[] {new List<int>(),new List<int>() };
    Image[,] _levelBox;
    [SerializeField] Image[] _playerBox;
    List<Player> _players = new List<Player>();

    private IDisposable _eventListener;
    // Start is called before the first frame update
#region Menu Controls

    void OnEnable()
    {
        _eventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
        _playerSelection[0].Add(1);
        _playerSelection[0].Add(2);
        _playerSelection[0].Add(3);
        _playerSelection[0].Add(4);
        

    }

    void OnDisable()
    {
        _eventListener.Dispose();
    }

    void AddPlayer(Player player)
    {
        _players.Add(player);
    }

    void OnButtonPressed(InputControl button)
    {
        Debug.Log(button.name);
        if (_selectedmenu == 0)
        {
            ChangeMenu(1);
        }
        // check if player was spawned

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
        UpdateSelection();

    }

    public void MenuDown(int player)
    {
        if (_playerSelection[0].Contains(player))
        {
            Debug.Log("move to 1");
            _playerSelection[0].Remove(player);
            _playerSelection[1].Add(player);
        }
        else if (_playerSelection[1].Contains(player))
        {
            Debug.Log("move to 0");
            _playerSelection[1].Remove(player);
            _playerSelection[0].Add(player);
        }
        UpdateSelection();

    }
    public void MenuUp(int player)
    {
        MenuDown(player);
        //only 2 options in either menu :)
    }
    public void MenuLeft(int player)
    {
        for (int x = 0; x < _playerSelection.GetLength(0); x++)
            for (int y = 0; y < _playerSelection.GetLength(1); y++)
            {
                if (_characters.Contains(_players[player].PlayerCharacter))
                {

                }
            }
    }
    public void MenuRight(int player)
    {
        for (int x = 0; x < _playerSelection.GetLength(0); x++)
            for (int y = 0; y < _playerSelection.GetLength(1); y++)
            {
                //if (_playerSelection[x, y] == player)
                //{
                //    if (x == 0 && _selectedmenu == 1)
                //    {
                //        // next character 
                //    }
                //    else if (x == 1 && _selectedmenu == 1)
                //    {
                //        //next level
                //    }
                //}
            }
    }
    public void MenuSelect(int player)
    {
        
    }
    public void MenuBack(int player)
    {

    }
    void UpdateSelection()
    { 
        if (_selectedmenu == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                _playerBox[i].color = new Color(_playerBox[i].color.r, _playerBox[i].color.g, _playerBox[i].color.b, 0);
                for (int k = 0; k < _levelBox.GetLength(1); k++)
                    _levelBox[i, k].color = new Color(0,0,0,0);
            }
            foreach (int i in _playerSelection[0])
            {
                switch (i)
                {
                    case 1:
                        _playerBox[0].color = new Color(_playerBox[0].color.r, _playerBox[0].color.g, _playerBox[0].color.b, 1);
                        break;

                    case 2:
                        _playerBox[1].color = new Color(_playerBox[1].color.r, _playerBox[1].color.g, _playerBox[1].color.b, 1);
                        break;

                    case 3:
                        _playerBox[2].color = new Color(_playerBox[2].color.r, _playerBox[2].color.g, _playerBox[2].color.b, 1);
                        break;

                    case 4:
                        _playerBox[3].color = new Color(_playerBox[3].color.r, _playerBox[3].color.g, _playerBox[3].color.b, 1);
                        break;
                }
            }
            int j = 0;
            foreach (int i in _playerSelection[1])
            {
                switch (i)
                {
                    case 1:
                        for (int k = 0; k < _levelBox.GetLength(1); k++)
                            _levelBox[j, k].color = Color.red;
                        break;

                    case 2:
                        for (int k = 0; k < _levelBox.GetLength(1); k++)
                            _levelBox[j, k].color = Color.blue;
                        break;

                    case 3:
                        for (int k = 0; k < _levelBox.GetLength(1); k++)
                            _levelBox[j, k].color = Color.green;
                        break;

                    case 4:
                        for (int k = 0; k < _levelBox.GetLength(1); k++)
                            _levelBox[j, k].color = Color.yellow;
                        break;
                }
            }
        }
    }


    #endregion


    void Start()
    {
        _levelBox = new Image[4, 4];
        for (int i = 1;i<5;i++)
        {
            GameObject box = GameObject.Find("LevelSelectBox" + i);
            //for (int j = 1; j < 3; j++)
            int j = 0;
            foreach (Image s in  box.GetComponentsInChildren<Image>())
            {
                _levelBox[i-1, j] = s;
                Debug.Log("added " + i + "," + j);
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
