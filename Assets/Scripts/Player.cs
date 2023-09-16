using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// currently multiplying things by transform.localscale.x to change facing direction,this works but could cause problems later

public class Player : MonoBehaviour, IDamage
{
    Rigidbody2D _rb;
    bool _grounded;
    bool _doubleJump = false;
    bool _dashAvailable = false;
    bool _facingright = true;
    bool _jump;
    bool _blockState, _blockInput;
    bool _interrupt = false;
    float _speed = 40;
    public float Health {get; private set;}
    Vector2 _inputDirection;
    int _busyJobs;
    [SerializeField] GameObject _meleeBox;
    [SerializeField] float _hitStunTime;
    [SerializeField] float _blockStunTime;
    public Character PlayerCharacter { get; private set; }
    Image _healthbar;
    public int PlayerNumber { get; private set;}
    Animator _animator;
    GameController _gameController;
    SpriteRenderer _playerSprite;
    public bool Dead {get; private set;}


    void OnEnable()
    {

        
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupCharacter();
        Application.targetFrameRate = 120;
        _rb = GetComponent<Rigidbody2D>();
        PlayerNumber = GetComponent<PlayerInput>().playerIndex;

        if (GameController.GameState == GameController.State.Game)
        {
            PlayerUI.Player[PlayerNumber].SetActive(true);
            _healthbar = PlayerUI.Player[PlayerNumber].GetComponentInChildren<Image>();
        }
        else if (GameController.GameState == GameController.State.Menu) // maybe move to start?
        {
            _gameController = GameObject.Find("GameController").GetComponent<GameController>();
            //MenuController.Players = GetComponent<PlayerInput>().playerIndex;
            _gameController.AddPlayer(this);
        }
        PlayerSpawn();
    }

    void PlayerSpawn()
    {
        Vector3 newpos = GameObject.Find("P" + (PlayerNumber + 1) + "Spawn").transform.position;
        Debug.Log("player spawned at " + newpos.x + "," + newpos.y);
        //transform.position = GameObject.Find("P"+ (PlayerNumber + 1) + "Spawn").transform.position;
        transform.position = new Vector3(newpos.x, newpos.y, 0);
        
    }

    public void ChangeCharacter(Character character)
    {
        Debug.Log("Change character");
        PlayerCharacter = character;
        SetupCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if (_blockState!=_blockInput)
        {
            ToggleBlock();
        }
        if (!(_busyJobs > 0))
        {
            if ((_inputDirection.x < 0 && _facingright) || (_inputDirection.x > 0 && !_facingright))
                Flip();
            if (_grounded)
            {
                // using RigidBody2D.velocity so that rigidbody mass only effects knockback for easier management of character stats (speed = speed and mass = knockback amount without having any interplay between the two)
                _rb.velocity += new Vector2(_inputDirection.x * _speed * Time.deltaTime, 0);
            }
            if (_jump)
            {
                if (_grounded)
                    _rb.velocity += new Vector2(0, 12);
                else if (_doubleJump)
                    if (_rb.velocity.y < 30) // why? I don't remember, may remove
                    {
                        _rb.velocity += new Vector2(0, 12);

                        _doubleJump = false;
                    }
                _jump = false;
            }
        }
        if (_animator!=null)
        {
            _animator.SetFloat("XSpeed", _rb.velocity.x*transform.localScale.x);
            _animator.SetFloat("YSpeed", _rb.velocity.y);
            _animator.SetBool("Grounded", _grounded);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 6 && PlayerCharacter != null) 
        {
            _grounded = true;
            if (PlayerCharacter.DoubleJump)
                _doubleJump = true;
            _dashAvailable = true;
            _rb.drag = 3;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)// kinda redundant doing this for trigger and collision exit - but it seems to catch some edge cases
    {
        _grounded = false;
        _rb.drag = 0.2f;
    }
    private void OnTriggerExit2D(Collider2D collision) // kinda redundant doing this for trigger and collision exit - but it seems to catch some edge cases
    {
        _grounded = false;
        _rb.drag = 0.2f;
    }

    public void ApplyDamage(float damage)
    {
        if (!_blockState)
        {
            Health -= damage;
            Debug.Log(gameObject.name + " is hit for " + damage + " damage.");
            _healthbar.fillAmount = Health / PlayerCharacter.Health;
            //Animate("Hit", _hitStunTime);
            Interrupt();
            FlashColor(new Color(1, 0.5f, 0.5f), 0.2f);
            Stun(_hitStunTime);
            if (Health <=0 )
            {
                Dead = true;
                Interrupt();
                //_animator.Play("KO");
                gameObject.layer = 7;
                _busyJobs++;
                _gameController.CheckWin();
            }
        }
        else
        {
            FlashColor(new Color(0.5f, 0.5f, 1), 0.2f);
            //Animate("BlockHit",_blockStunTime)
            Stun(_blockStunTime);
        }
    }

    void Flip()
    {
        _facingright = (!_facingright);
        transform.localScale = (new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z));
    }

    //Instantiate skin/animation prefab specified by character object and declare _animator 
    public void SetupCharacter()
    {
        if (PlayerCharacter != null && PlayerCharacter.Appearance != null)
        {

            _speed = PlayerCharacter.Speed;
            _rb.mass = PlayerCharacter.Mass;
            Health = PlayerCharacter.Health;
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {

                if (t.tag == "Skin")
                    Destroy(t.gameObject);
            }
            GameObject gO = Instantiate(PlayerCharacter.Appearance);
            gO.transform.position = transform.position;
            gO.transform.SetParent(transform, true);
            _animator = gO.GetComponentInChildren<Animator>();
            _playerSprite = gO.GetComponentInChildren<SpriteRenderer>();
            //enabled/disable movement based on game state
            if (GameController.GameState == GameController.State.Game)
                _busyJobs = 0;
            else
                _busyJobs = 1;

            if (PlayerCharacter.Mirror)
            {
                gO.GetComponentInChildren<SpriteRenderer>().flipX = true;
                Debug.Log("Mirror");
            }
        }
        if (GameController.GameState == GameController.State.Game)
        {
            PlayerUI.Player[PlayerNumber].SetActive(true);
            _healthbar = PlayerUI.Player[PlayerNumber].GetComponentInChildren<Image>();
            _healthbar.fillAmount = Health / PlayerCharacter.Health;
            gameObject.layer = 3;
            _busyJobs = 0;
            Dead = false;
        }
        PlayerSpawn();
    }

    private void OnDestroy()
    {
        if (PlayerUI.Player!=null&& PlayerUI.Player[PlayerNumber]!=null)
            PlayerUI.Player[PlayerNumber].SetActive(false);
    }

    #region Inputs
    public void OnMove(InputValue value)
    {
        if (_gameController!=null)
        {
            if (value.Get<Vector2>().y < -0.5f)
            {
                _gameController.MenuDown(PlayerNumber+1);
                Debug.Log("Player - MenuUp");
            }
            if (value.Get<Vector2>().y > 0.5f)
            {
                _gameController.MenuUp(PlayerNumber+1);
                Debug.Log("Player - MenuDown");
            }
            if (value.Get<Vector2>().x > 0.5f)
            {
                _gameController.MenuRight(PlayerNumber + 1);
            }
            if (value.Get<Vector2>().x < -0.5f)
            {
                _gameController.MenuLeft(PlayerNumber + 1);
            }
        }
        _inputDirection = value.Get<Vector2>();
        // constrain to 8 directions
        if (MathF.Abs(_inputDirection.x) > 0.2f && MathF.Abs(_inputDirection.y) > 0.2f)
            _inputDirection = new Vector2(0.5f * (MathF.Abs(_inputDirection.x) / _inputDirection.x), 0.5f * (MathF.Abs(_inputDirection.y) / _inputDirection.y)) * _inputDirection.magnitude;
        else if (MathF.Abs(_inputDirection.x) > MathF.Abs(_inputDirection.y))
            _inputDirection = new Vector2(_inputDirection.x, 0);
        else _inputDirection = new Vector2(0,_inputDirection.y);
    }
    public void OnJump(InputValue value)
    {
        if (_gameController!= null && GameController.GameState == GameController.State.Menu)
            _gameController.MenuSelect(PlayerNumber+1);
        _jump = true;
    }


    public void OnMoveAction(InputValue value)
    {
        if (GameController.GameState == GameController.State.Menu)
            _gameController.MenuSelect(PlayerNumber + 1);

        if (_dashAvailable&& !(_busyJobs > 0))
            if (_inputDirection.magnitude > 0.1f)
                MovetoPoint(_inputDirection * 4, 0.1f,0,0,true);
            else
                MovetoPoint(new Vector2(transform.localScale.x * 4, 0), 0.1f,0,0,true);
        Dodge(0.2f, 0f, 0f, false);
        _dashAvailable = false;
    }

    public void OnBlock(InputValue value)
    {
        if (value.Get<float>()>0.2f)
            _blockInput = true;
        else
            _blockInput = false;
    }

    public void OnAction1(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(PlayerCharacter.Input1);
    }

    public void OnAction2(InputValue value)
    {
        if (GameController.GameState == GameController.State.Menu)
            _gameController.MenuBack(PlayerNumber + 1);
        if (!(_busyJobs > 0))
            ProcessActions(PlayerCharacter.Input2);
    }

    public void OnAction3(InputValue value) 
    {
        if (!(_busyJobs > 0))
            ProcessActions(PlayerCharacter.Input3);
    }

    public void OnAction4(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(PlayerCharacter.Input4);
    }

    public void OnAction5(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(PlayerCharacter.Input5);
    }

    public void OnAction6(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(PlayerCharacter.Input6);
    }

    # endregion Inputs
    
    public void ProcessActions(List<Action> actions)
    {
        foreach (Action a in actions)
        {
            if (a.PlayerState == Action.State.Both || (a.PlayerState == Action.State.Grounded && _grounded) || (a.PlayerState == Action.State.Airborne && !_grounded))
                switch (a.Type)
                {
                    case Action.ActionType.Animation:
                        Animate(a.AnimationName, a.AttackTime);
                        break;
                    case Action.ActionType.Melee:
                        Melee(a.HitPoints, a.Damage, a.KnockBack, a.KnockBackDirection, a.MaxCombo, a.AttackTime, a.HitSize, a.StartDelay, a.EndDelay, a.Busy);
                        break;
                    case Action.ActionType.Projectile:
                        switch (a.Direction)
                        {
                            case Action.DirectionType.InputDirection:
                                if (_inputDirection.normalized.magnitude < 0.2f) // shoot forward if input is 0
                                    Shoot(new Vector2(transform.localScale.x, 0), a.Damage, a.KnockBack, a.KnockBackDirection, new Vector2(a.ProjectileStart.x * transform.localScale.x, a.ProjectileStart.y), a.ProjectileSpeed, a.StartDelay, a.EndDelay, a.ProjectilePrefab, a.Busy);
                                else
                                    Shoot(_inputDirection.normalized, a.Damage, a.KnockBack, a.KnockBackDirection, new Vector2(a.ProjectileStart.x * transform.localScale.x, a.ProjectileStart.y), a.ProjectileSpeed, a.StartDelay, a.EndDelay, a.ProjectilePrefab, a.Busy);
                                //Debug.Log("fire projectile in input direction");
                                break;
                            case Action.DirectionType.Custom:
                                Shoot(new Vector2(a.CustomDirection.x * transform.localScale.x, a.CustomDirection.y).normalized, a.Damage, a.KnockBack, a.KnockBackDirection, new Vector2(a.ProjectileStart.x * transform.localScale.x, a.ProjectileStart.y), a.ProjectileSpeed, a.StartDelay, a.EndDelay, a.ProjectilePrefab, a.Busy);
                                //Debug.Log("Fire Projectile in Custom direction " + (a.CustomDirection.x * transform.localScale.x) + "," + a.CustomDirection.y);
                                break;
                            case Action.DirectionType.FacingDirection:
                                Shoot(new Vector2(transform.localScale.x, 0), a.Damage, a.KnockBack, a.KnockBackDirection, new Vector2(a.ProjectileStart.x * transform.localScale.x, a.ProjectileStart.y), a.ProjectileSpeed, a.StartDelay, a.EndDelay, a.ProjectilePrefab, a.Busy);
                                //Debug.Log("Fire Projectile in forward direction");
                                break;
                        }
                        break;
                    case Action.ActionType.Movement:
                        switch (a.Movement)
                        {
                            case Action.MovementType.AddForce:
                                MoveByAddForce(a.Force, a.StartDelay, a.EndDelay,a.Busy);
                                break;
                            case Action.MovementType.Dodge:
                                Dodge(a.AttackTime, a.StartDelay, a.EndDelay, a.Busy);
                                break;
                            case Action.MovementType.MoveToPoint:
                                MovetoPoint(new Vector2(a.Destination.x * transform.localScale.x, a.Destination.y), a.AttackTime, a.StartDelay, a.EndDelay, a.Busy);
                                break;
                        }
                        break;
                }
        }
        
    }

    #region Action Functions/Methods
    async void Melee(List<Vector2> hitPoints,float damage, float knockBack,Vector2 knockBackDirection, float combo, float attackTime, float hitSize, float startDelay, float EndDelay, bool busy) //missing currently - Damage and knockback values
    {
        if (hitPoints.Count < 1)
            return;
        if (busy)
            _busyJobs++;
        //Initial Delay
        float timer = Time.time;
        while (Time.time < timer + startDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        //Attack
        GameObject hitBox = Instantiate(_meleeBox);
        if (hitBox.TryGetComponent<MeleeHitBox>(out MeleeHitBox m))
        {
            m.Damage = damage;
            m.KnockBack = knockBack;
            m.KnockBackDirection = new Vector2(knockBackDirection.x * transform.localScale.x, knockBackDirection.y);
            m.ComboCount = combo;
        }
        hitBox.transform.SetParent(transform, false);
        hitBox.transform.localPosition = hitPoints[0];
        hitBox.transform.localScale = new Vector3(hitSize, hitSize, 1);
        timer = Time.time;
        if (hitPoints.Count == 1)
        {
            while (Time.time < timer + attackTime)
            {
                if (_interrupt)
                {
                    if (busy)
                        _busyJobs--;
                    return;
                }
                await Task.Delay(25);
            }
        }
        else
        {
            for (int i = 0; i < hitPoints.Count - 1; i++)
                while (Time.time < timer + (attackTime / (hitPoints.Count - 1)) * (i + 1))
                {
                    if (_interrupt)
                    {
                        if (busy)
                            _busyJobs--;
                        return;
                    }
                    hitBox.transform.localPosition = Vector2.MoveTowards(hitPoints[i], hitPoints[i + 1], (Time.time - (timer + (attackTime / (hitPoints.Count - 1)) * i)) / (attackTime / (hitPoints.Count - 1)));
                    await Task.Delay(25);
                }
        }
        hitBox.SetActive(false); // in case there is some delay in the destroy function
        Destroy(hitBox);
        // hitbox trigger was causing player to be incorrectly in grounded state, there is probably a better solution than this but it works
        _grounded = false;
        _rb.drag = 0.2f;

        // End Delay
        timer = Time.time;
        while (Time.time < timer + EndDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        if (busy)
            _busyJobs--;
    }

    async void Shoot(Vector2 direction, float damage, float knockback, Vector2 knockBackDirection, Vector2 startPoint, float speed, float startDelay, float EndDelay, GameObject prefab,bool busy)
    {
        if (busy)
            _busyJobs++;
        //Initial Delay
        float timer = Time.time;
        while (Time.time < timer + startDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        // Shooty shoot
        GameObject gameObj = Instantiate(prefab);
        gameObj.transform.position = _rb.position + startPoint;
        if (gameObj.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.setup(direction, speed,damage,knockback, new Vector2(knockBackDirection.x * transform.localScale.x, knockBackDirection.y),1f,gameObject);
        }
        // End Delay
        timer = Time.time;
        while (Time.time < timer + EndDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        if (busy)
            _busyJobs--;
    }

    async void Animate(String name, float time)
    {
        float timer = Time.time;
        if (_animator != null)
        {
            _animator.Play(name);
        }

        while (Time.time < timer + time)
        {
            if (_interrupt) // exit early 
            {
                if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == name) // return to idle unless we are already playing another animation
                    _animator.Play("Idle");
                return;
            }
            await Task.Delay(25);
        }

        if (_animator != null)
        {
            _animator.Play("Idle");
        }
    }

    public async void FlashColor(Color color, float time)
    {
        float timer = Time.time;
        _playerSprite.color = color;
        while (Time.time < timer + time)
        {
            await Task.Delay(25);
        }
        _playerSprite.color = Color.white;
    }

    #region Movement 

    async void MovetoPoint(Vector2 destination, float attackTime, float startDelay, float endDelay,bool busy) // todo -- if player is moved by some other means then also move the destination so that they do not "snap" back in weird way
    {
        if (busy)
            _busyJobs++;
        float speed = destination.magnitude/attackTime;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        //Initial Delay
        float timer = Time.time;
        while (Time.time < timer + startDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        //Movement
        timer = Time.time;
        Vector2 direction = (destination).normalized;
        Debug.Log("speed is " + speed);
        while (Time.time < timer + attackTime)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                _rb.gravityScale = 1;
                _rb.velocity = direction * (speed / 4);
                return;
            }
            _rb.velocity = direction * speed;
            await Task.Delay(25);
        }
        _rb.gravityScale = 1;
        _rb.velocity = direction * (speed/4); // maybe this should be based on some vale provided in args
        // End Delay
        timer = Time.time;
        while (Time.time < timer + endDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    void ToggleBlock()
    {
        if (_blockInput)
            _busyJobs++;
        else
            _busyJobs--;

        _blockState = !_blockState;
    }

    async void Dodge(float attackTime, float startDelay, float endDelay, bool busy)
    {
        if (busy)
            _busyJobs++;
        //Initial Delay
        float timer = Time.time;
        while (Time.time < timer + startDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        // move player to layer 7 for specified time
        gameObject.layer = 7;
        timer = Time.time;
        while (Time.time < timer + attackTime)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                gameObject.layer = 3;
                return;
            }
            await Task.Delay(25);
        }
        gameObject.layer = 3;
        // End Delay
        timer = Time.time;
        while (Time.time < timer + endDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    async void MoveByAddForce(Vector2 force, float startDelay, float endDelay,bool busy)
    {
        if (busy)
            _busyJobs++;
        //Initial Delay
        float timer = Time.time;
        while (Time.time < timer + startDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }

        _rb.AddForce(new Vector2(force.x * transform.localScale.x, force.y));

        //end delay
        timer = Time.time;
        while (Time.time < timer + endDelay)
        {
            if (_interrupt)
            {
                if (busy)
                    _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        if (busy)
            _busyJobs--;

    }

    async void Stun(float time) // just sets the player as "busy" for specified time, for use with hit/block stun 
    {
        float timer = Time.time;
        _busyJobs++;
        while (Time.time < timer + time)
        {
            if (_interrupt)
            {
                _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        _busyJobs--;
    }
    async void Stun(float time, bool Interruptable) // just sets the player as "busy" for specified time, for use with hit/block stun, option fo no interrupt so we can still use if interruping other actions
    {
        float timer = Time.time;
        _busyJobs++;
        while (Time.time < timer + time)
        {
            if (_interrupt && Interruptable)
            {
                _busyJobs--;
                return;
            }
            await Task.Delay(25);
        }
        _busyJobs--;
    }

    async void Interrupt()
    {
        _interrupt = true;
        await Task.Delay(26);
        _interrupt = false;
    }

    #endregion Movement 

    #endregion Action Functions/Methods
}
