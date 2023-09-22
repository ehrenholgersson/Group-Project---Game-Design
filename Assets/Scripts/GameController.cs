using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Character[] _characters;
    GameObject[] _levels;
    [SerializeField] TextMeshProUGUI _levelList;
    [SerializeField] TextMeshProUGUI _levelName;
    [SerializeField] TextMeshProUGUI _characterList;
    [SerializeField] GameObject[] _menus;
    int _selectedmenu = 0;
    List<int>[] _playerSelection = new List<int>[] {new List<int>(),new List<int>() };
    Image[,] _levelBox;
    [SerializeField] Image[] _playerBox;
    [SerializeField] SelectionBox[] _selectionboxes1 = new SelectionBox[2];
    [SerializeField] SelectionBox[] _selectionboxes2 = new SelectionBox[2];
    List<Player> _players = new List<Player>();
    public List<Player> Players { get => _players; }
    List<bool> _readyplayers = new List<bool>();
    int _selectedLevel = 0;
    GameObject _background;
    GameObject _characterScene;
    public static State GameState = State.Menu;
    public static GameController Instance;
    [SerializeField] GameObject _winScreen;
    [SerializeField] GameObject[] _ready = new GameObject[4];
    [SerializeField] bool _allowSinglePlayer = false;

    float _menutimer;

    public enum State {Game, Menu, WinScreen}


    private IDisposable _eventListener; // don't know if this is still needed?
    
#region Menu Controls


    public void AddPlayer(Player player)
    {
        _players.Add(player);
        _readyplayers.Add(false);
        player.ChangeCharacter(_characters[0]);
        _playerSelection[0].Add(_players.Count);
        if (GameState == State.Menu && _selectedmenu == 0)
        {
            ChangeMenu(1);
        }
        UpdateSelection();
    }

    void OnButtonPressed(InputControl button)
    {
        if (GameState == State.Game)
            return;
        if (GameState == State.WinScreen && (Time.time - _menutimer > 0.3f))
        {
            _winScreen.SetActive(false);
            GameState = State.Menu;
            ResetMenu();
        }
        else if (_selectedmenu == 0 && (Time.time - _menutimer > 0.3f))
        {
            ChangeMenu(1);
        }

    }

    void  ChangeMenu(int index)
    {
        for (int i = 0; i< _readyplayers.Count; i++)
        {
            _readyplayers[i] = false;
        }
        _menutimer = Time.time;
        _selectedmenu = index;
        for (int i = 0; i < _menus.Length; i++)
        {
            if (i == index&&GameState==State.Menu)
            {
                _menus[i].SetActive(true);
                
            }
            else
            {
                _menus[i].SetActive(false);
            }
        }
        _playerSelection[0].Clear();
        _playerSelection[1].Clear();
        foreach (Player p in _players) 
        {
            _playerSelection[0].Add(p.PlayerNumber + 1);
        }
        UpdateSelection();
        Debug.Log("swapped to menu " + index);
    }

    public void MenuDown(int player)
    {
        if (GameState == State.Game)
            return;
        if (_playerSelection[0].Contains(player))
        {          
            _playerSelection[0].Remove(player);
            _playerSelection[1].Add(player);
        }
        else if (_playerSelection[1].Contains(player))
        {
            _playerSelection[1].Remove(player);
            _playerSelection[0].Add(player);
            if (_playerSelection[1].Contains(player))
                Debug.Log("failed to remove player from selection");
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
        if (GameState == State.Game)
            return;
        if (_selectedmenu == 2)
        {
            if (_playerSelection[0].Contains(player))
            {
                if (_characters.Contains(_players[player - 1].PlayerCharacter))
                {
                    int selChar = Array.IndexOf(_characters, _players[player - 1].PlayerCharacter)-1;
                    if (selChar < 0)
                        selChar = _characters.Length - 1;
                    _players[player - 1].ChangeCharacter(_characters[selChar]);
                }
            }
            if (_playerSelection[1].Contains(player))
            {
                _selectedLevel--;
                if (_selectedLevel<0)
                    _selectedLevel = _levels.Length - 1;
                _selectedLevel = _selectedLevel % _levels.Length;
                Debug.Log("selected level " + _selectedLevel);
                Destroy(_background);
                _background = Instantiate(_levels[_selectedLevel]);
            }
        }
        UpdateSelection();
    }
    public void MenuRight(int player)
    {
        if (GameState == State.Game)
            return;
        if (_selectedmenu == 2)
        {
            if (_playerSelection[0].Contains(player))
            {
                if (_characters.Contains(_players[player-1].PlayerCharacter))
                    _players[player-1].ChangeCharacter(_characters[(Array.IndexOf(_characters, _players[player-1].PlayerCharacter) + 1) % _characters.Length]);                       
            }
            if (_playerSelection[1].Contains(player))
            {
                _selectedLevel++;
                _selectedLevel = _selectedLevel% _levels.Length;
                Debug.Log("selected level " + _selectedLevel);
                Destroy(_background);
                _background = Instantiate(_levels[_selectedLevel]);
            }
        }
        UpdateSelection();
    }
    public void MenuSelect(int player)
    {
        if (GameState == State.Game)
            return;
        if (_selectedmenu == 0)
        {
                ChangeMenu(1);
        }
        if (GameState == State.WinScreen)
        {
            _winScreen.SetActive(false);
            GameState = State.Menu;
            ResetMenu();
        }
        if (_selectedmenu == 1)
        {
            if (_playerSelection[0].Contains(player))
            {
                if (Time.time - _menutimer > 0.3f)
                    ChangeMenu(2);
            }
            else if (_playerSelection[1].Contains(player))
            {
                Application.Quit();
                Debug.Log("quit");
            }
        }
            if (( _selectedmenu == 2 && Time.time - _menutimer > 0.3f))
        {
            _readyplayers[player -1] = true;
            UpdateSelection();
            if (_readyplayers.Count > 1||_allowSinglePlayer)
            {
                foreach(bool ready in _readyplayers)
                {
                    if (!ready)
                        return;
                }
                BeginRound();
            }

        }
    }
    public void MenuBack(int player)
    {
        if (GameState == State.Game)
            return;
        if (_selectedmenu == 2)
        {
            if (_readyplayers[player - 1] == false)
                ChangeMenu(1);
            else
                _readyplayers[player - 1] = false;
        }
        UpdateSelection();
    }

    public void CheckWin()
    {
        int count = 0;
        foreach (Player p in _players)
        {
            if (!p.Dead)
                count++;
        }
        if (count > 1)
            return;
        foreach (Player p in _players)
        {
            if (!p.Dead)
            {
                Debug.Log("player " + p.PlayerNumber + " won");
                GameState = State.WinScreen;
                _winScreen.SetActive(true);
                _winScreen.GetComponent<WinScreen>().SetWinner(p.PlayerNumber+1);
                _menutimer = Time.time;
            }
        }

        //ResetMenu();
    }
    void UpdateSelection()
    { 
        if (_selectedmenu == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                _playerBox[i].color = new Color(_playerBox[i].color.r, _playerBox[i].color.g, _playerBox[i].color.b, 0);
                for (int k = 0; k < _levelBox.GetLength(1); k++)
                    _levelBox[i, k].color = new Color(0, 0, 0, 0);
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
            _selectionboxes2[1].Set(_playerSelection[1]);
            //int j = 0;
            //foreach (int i in _playerSelection[1])
            //{
            //    switch (i)
            //    {
            //        case 1:
            //            for (int k = 0; k < _levelBox.GetLength(1); k++)
            //                _levelBox[j, k].color = Color.red;
            //            break;

            //        case 2:
            //            for (int k = 0; k < _levelBox.GetLength(1); k++)
            //                _levelBox[j, k].color = Color.blue;
            //            break;

            //        case 3:
            //            for (int k = 0; k < _levelBox.GetLength(1); k++)
            //                _levelBox[j, k].color = Color.green;
            //            break;

            //        case 4:
            //            for (int k = 0; k < _levelBox.GetLength(1); k++)
            //                _levelBox[j, k].color = Color.yellow;
            //            break;
            //    }
            //    j++;
            //}
        }
        if (_selectedmenu == 1)
        {
            _selectionboxes1[0].Set(_playerSelection[0]);
            _selectionboxes1[1].Set(_playerSelection[1]);
        }
        _levelName.text = _levels[_selectedLevel].name;
        int count = 0;
        foreach (GameObject ready in _ready)
        {
            if (_readyplayers.Count > count && _readyplayers[count])
            {
                _ready[count].SetActive(true);
            }
            else
                _ready[count].SetActive(false);
            count++;
        }
    }


    #endregion
    void OnDisable()
    {
        _eventListener.Dispose();
    }

    void BeginRound()
    {
        PlayerInputManager.instance.DisableJoining();
        GameState = State.Game;
        _characterScene.SetActive(false);
        _background.transform.Find("Spawn").gameObject.SetActive(true);
        //"change" character to trigger a respawn
        foreach (Player p in _players)
        {
            p.ChangeCharacter(p.PlayerCharacter);
        }
        ChangeMenu(0);
    }

    public void ResetMenu()
    {
        // boot players
        foreach(GameObject p in GameObject.FindGameObjectsWithTag("Player_Character"))
        {
            Destroy(p);
        }
        _players.Clear();
        _readyplayers.Clear();
        // reenable stage for player select rendertexture and enable joining
        PlayerInputManager.instance.EnableJoining();
        _characterScene.SetActive(true);
        // goto first menu screen
        ChangeMenu(0);

        // clear player menu selection
        _playerSelection[0].Clear();
        _playerSelection[1].Clear();

        // change to random level
        Destroy(_background);
        _selectedLevel = UnityEngine.Random.Range(0, _levels.Length);
        _background = Instantiate(_levels[_selectedLevel]);
        _menutimer = Time.time;
    }
    void OnEnable()
    {
        _eventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
    }

    void Start()
    {
        Instance = this;
        _characterScene = GameObject.Find("Characters");
        _levelBox = new Image[4, 4];
        for (int i = 1;i<5;i++)
        {
            GameObject box = GameObject.Find("LevelSelectBox" + i);
            int j = 0;
            foreach (Image s in  box.GetComponentsInChildren<Image>())
            {
                _levelBox[i-1, j] = s;
                Debug.Log("added " + i + "," + j);
                j++;
            }

        }

        UnityEngine.Object[] loadArray = Resources.LoadAll("Characters", typeof(Character));
        _characters = new Character[loadArray.Length];
        loadArray.CopyTo( _characters, 0 );
        Debug.Log("Found " + loadArray.Length + " characters");
        //_characters = (Resources.LoadAll("Characters/Selectable", typeof(Character)) as Character[]).ToList();
        loadArray = Resources.LoadAll("Levels", typeof(GameObject));
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

        ChangeMenu(0);
        _selectedLevel = UnityEngine.Random.Range(0, _levels.Length);
        _background = Instantiate(_levels[_selectedLevel]);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
