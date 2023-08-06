using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

// currently multiplying things by transform.localscal.x to change facing direction,this works but could cause problems later

public class Player : MonoBehaviour, IDamage
{
    Rigidbody2D _rb;
    bool _grounded;
    bool _doubleJump = false;
    bool _dashAvailable = false;
    bool _facingright = true;
    float _speed = 40;
    Vector2 _inputDirection;
    //Vector3 _force;
    float _dashpower;
    bool _isBusy; // could use state machine/enum instead?
    int _busyJobs;
    [SerializeField] GameObject _meleeBox; // yes, its a circle - need to change this to the gameobject or possibly MeleeHitBox
    //[SerializeField] GameObject _hitBox;
    [SerializeField] Character _character;

    //[SerializeField] Action[] _action = new Action[4];

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        _rb = GetComponent<Rigidbody2D>();
        //_meleeBox = transform.GetComponentInChildren<CircleCollider2D>(true);
    }

    public void ApplyDamage(float damage)
    {
        Debug.Log(gameObject.name + " is hit for " + damage + " damage.");
    }

    public void OnMove(InputValue value)
    { 
        _inputDirection = value.Get<Vector2>();
    }

    void flip()
    {
        _facingright = (!_facingright);
        transform.localScale = (new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z));
    }
    public void OnJump(InputValue value)
    {
        if(!(_busyJobs>0))
        {
            if (_grounded)
                _rb.velocity += new Vector2(0, 12);
            else if (_doubleJump)
                if (_rb.velocity.y < 30) // why dis? I don't remember, may remove
                {
                    _rb.velocity += new Vector2(0, 12);

                    _doubleJump = false;
                }
        }
    }

    public void OnMoveAction(InputValue value)
    {
        if (_dashAvailable&& !(_busyJobs > 0))
            if (_inputDirection.magnitude > 0.1f)
                MovetoPoint(_rb.position + _inputDirection * 4, 50,0,0,true);
            else
                MovetoPoint(_rb.position + new Vector2(transform.localScale.x * 4, 0), 50,0,0,true);
        _dashAvailable = false;
    }

    public void OnAction1(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Action1);
    }

    public void OnAction2(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Action2);
    }

    public void OnAction3(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Action3);
    }

    public void OnAction4(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Action4);
    }

    public void OnAction5(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Action5);
    }

    public void OnAction6(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Action6);
    }

    public void ProcessActions(List<Action> actions)
    {
        foreach (Action a in actions)
        {
            if(a.PlayerState==Action.State.Both||(a.PlayerState == Action.State.Grounded&&_grounded)||(a.PlayerState == Action.State.Airborne&&!_grounded))
                switch (a.Type)
                {
                    case Action.ActionType.Melee:
                        Melee(a.HitPoints, a.Damage, a.KnockBack,a.KnockBackDirection, a.MaxCombo, a.AttackTime, a.HitSize, a.StartDelay, a.EndDelay, a.Busy);
                        break;
                    case Action.ActionType.Projectile:
                        switch (a.Direction)
                        {
                            case Action.DirectionType.InputDirection:
                                Shoot(_inputDirection.normalized, new Vector2(a.ProjectileStart.x * transform.localScale.x, a.ProjectileStart.y), a.ProjectileSpeed, a.StartDelay, a.EndDelay, a.ProjectilePrefab, a.Busy);
                                Debug.Log("fire projectile in input direction");
                                break;
                            case Action.DirectionType.Custom:
                                Shoot(new Vector2(a.CustomDirection.x * transform.localScale.x, a.CustomDirection.y).normalized, new Vector2(a.ProjectileStart.x * transform.localScale.x, a.ProjectileStart.y), a.ProjectileSpeed, a.StartDelay, a.EndDelay, a.ProjectilePrefab, a.Busy);
                                Debug.Log("Fire Projectile in Custom direction " + (a.CustomDirection.x * transform.localScale.x) + "," + a.CustomDirection.y);
                                break;
                            case Action.DirectionType.FacingDirection:
                                Shoot(new Vector2(transform.localScale.x, 0), new Vector2(a.ProjectileStart.x * transform.localScale.x, a.ProjectileStart.y), a.ProjectileSpeed, a.StartDelay, a.EndDelay, a.ProjectilePrefab, a.Busy);
                                Debug.Log("Fire Projectile in forward direction");
                                break;
                        }
                        break;
                    case Action.ActionType.Movement:
                        switch (a.Movement)
                        {
                            case Action.MovementType.AddForce:
                                MoveByAddForce(a.Force, a.StartDelay, a.EndDelay);
                                break;
                            case Action.MovementType.Dodge:
                                Dodge(a.AttackTime, a.StartDelay, a.EndDelay, a.Busy);
                                break;
                            case Action.MovementType.MoveToPoint:
                                MovetoPoint(_rb.position + new Vector2(a.Destination.x * transform.localScale.x, a.Destination.y), a.Speed, a.StartDelay, a.EndDelay, a.Busy);
                                break;
                        }
                        break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!(_busyJobs>0))
        {
            if ((_inputDirection.x < 0 && _facingright) || (_inputDirection.x > 0 && !_facingright))
                flip();
            if (_grounded)
            {
                // using RigidBody2D.velocity so that rigisbody mass only effects knockback for easier management of charachter stat (speed = speed and mass = knockback amount without having any interplay between these)
                _rb.velocity += new Vector2(_inputDirection.x * _speed * Time.deltaTime, 0);
            }
        }

        #region Old Movement Code
        // -- Old code, replacing with new Input system
        //if(!(_busyJobs>0))
        //{
        //    if (_grounded)
        //    {
        //        if (Input.GetKeyDown(KeyCode.Space) && _rb.velocity.y < 10)
        //        {
        //            _rb.velocity += new Vector2(0, 8);

        //        }
        //        if (Input.GetKey(KeyCode.A) && _rb.velocity.x > -10)
        //        {
        //            _rb.velocity += new Vector2(-35, 0) * Time.deltaTime;
        //        }
        //        else if (Input.GetKey(KeyCode.D) && _rb.velocity.y < 10)
        //        {
        //            _rb.velocity += new Vector2(35, 0) * Time.deltaTime;
        //        }
        //    }
        //    else if (_doubleJump)
        //        if (Input.GetKeyDown(KeyCode.Space) && _rb.velocity.y < 10)
        //        {
        //            _rb.velocity += new Vector2(0, 8);

        //            _doubleJump = false;
        //        }
        //    if (Input.GetKeyDown(KeyCode.LeftControl))
        //    {
        //        //_dashpower+=Time.deltaTime *200;
        //        Dash(_rb.position + _rb.velocity.normalized * 10, 60);
        //    }
        //    /*else if (_dashpower > 0)
        //    {
        //        Dash();
        //    }*/
        //    if (Input.GetKeyDown(KeyCode.Tab))
        //        //Melee(new List<Vector2> { new Vector2(0,0),new Vector2(1,0),new Vector2(1,1) }, 0.15f, 0.2f, 0.05f, 0.2f);
        //        Shoot(Vector2.right, Vector2.right, 30, 0.1f, 0.3f, _projectile);
        //    if(Input.GetKeyDown(KeyCode.F))
        //    {
        //        switch(_action[0].Type)
        //        {
        //            case Action.ActionType.Melee:
        //                Melee(_action[0].HitPoints, _action[0].AttackTime, _action[0].HitSize, _action[0].StartDelay, _action[0].EndDelay);
        //                break;
        //            case Action.ActionType.Projectile:
        //                Shoot(Vector2.right, Vector2.right, _action[0].ProjectileSpeed, _action[0].StartDelay, _action[0].EndDelay, _action[0].ProjectilePrefab);
        //                break;
        //        }
        //    }
        //        //Melee(new List<Vector2> { new Vector2(0,0),new Vector2(1,0),new Vector2(1,1) }, 0.4f, 0.2f, 0.2f, 0.1f);
        //}
        #endregion

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer != 6)
        {
            _grounded = true;
            if (_character.DoubleJump)
                _doubleJump = true;
            _dashAvailable = true;
            _rb.drag = 3;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _grounded = false;
        _rb.drag = 0.2f;
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
            await Task.Delay(25);
        //Attack
        //hitBox.SetActive(true);
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
                await Task.Delay(25);
        }
        else
        {
            for (int i = 0; i < hitPoints.Count - 1; i++)
                while (Time.time < timer + (attackTime / (hitPoints.Count - 1)) * (i + 1))
                {
                    hitBox.transform.localPosition = Vector2.MoveTowards(hitPoints[i], hitPoints[i + 1], (Time.time - (timer + (attackTime / (hitPoints.Count - 1)) * i)) / (attackTime / (hitPoints.Count - 1)));
                    await Task.Delay(25);
                }
        }
        hitBox.SetActive(false);
        Destroy(hitBox);
        // End Delay
        timer = Time.time;
        while (Time.time < timer + EndDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    async void Shoot(Vector2 direction, Vector2 startPoint, float speed, float startDelay, float EndDelay, GameObject prefab,bool busy)
    {
        if (busy)
            _busyJobs++;
        //Initial Delay
        float _timer = Time.time;
        while (Time.time < _timer + startDelay)
            await Task.Delay(25);
        // Shooty shoot
        GameObject gameO = Instantiate(prefab);
        gameO.transform.position = _rb.position + startPoint;
        if (gameO.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.setup(direction, speed);
        }

        // End Delay
        _timer = Time.time;
        while (Time.time < _timer + EndDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    #region Movement 

    async void MovetoPoint(Vector2 destination, float speed, float startDelay, float endDelay,bool busy) // todo -- if player is moved by some other means the also move the destination so that they do not "snap" back in weird way, also this currently has a timeout of 2 seconds, could make this an action defined variable or could use distance to destination vs speed to come up with a more suitable value for this
    {
        if (busy)
            _busyJobs++;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        float timer = Time.time;
        while (Time.time < timer + startDelay)
            await Task.Delay(25);
        timer = Time.time;
        Vector2 direction = (destination - _rb.position).normalized;
        //        Vector2 setVelocity = _rb.position;
        Debug.Log("dash started to " + destination);
        while (Mathf.Abs((destination - _rb.position).magnitude) > 1f && (Time.time < timer + 2))
        {
            //if (direction.y < 0 && _grounded)
            //{
            //    _busyJobs--;
            //    _rb.gravityScale = 1;
            //    return;
            //}
            //transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            _rb.velocity = (destination - _rb.position).normalized * speed;
            await Task.Delay(25);
        }
        Debug.Log("Dash complete");
        _dashpower = 0;
        _rb.gravityScale = 1;
        _rb.velocity = direction * 8; // this should be based on some vale provided in args
        timer = Time.time;
        while (Time.time < timer + endDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    async void Dodge(float attackTime, float startDelay, float endDelay, bool busy)
    {
        if (busy)
            _busyJobs++;
        float timer = Time.time;
        while (Time.time < timer + startDelay)
            await Task.Delay(25);
        gameObject.layer = 7;
        timer = Time.time;
        while (Time.time < timer + attackTime)
            await Task.Delay(25);
        timer = Time.time;
        while (Time.time < timer + endDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    async void MoveByAddForce(Vector2 force, float startDelay, float endDelay)
    {
        _busyJobs++;
        //Initial Delay
        float timer = Time.time;
        while (Time.time < timer + startDelay)
            await Task.Delay(25);
        _rb.AddForce(force);
        timer = Time.time;
        while (Time.time < timer + endDelay)
            await Task.Delay(25);

    }

    #endregion Movement 

    #endregion Action Functions/Methods

    #region Unused Functions/Methods

    //async void Dash(Vector2 destination,float speed) // currently has a timeout of 2 seconds, could make this an action defined variable or could use distance to destination vs speed to come up with a more suitable value for this
    //{
    //    _busyJobs++;
    //    _rb.gravityScale = 0;
    //    _rb.velocity = Vector2.zero;
    //    float startTime = Time.time;
    //    Vector2 direction = (destination - _rb.position).normalized;
    //    Debug.Log("dash started to " + destination);
    //    while (Mathf.Abs((destination - _rb.position).magnitude) > 1f&& (Time.time < startTime + 2))
    //    {
    //        if (direction.y < 0 && _grounded)
    //        {
    //            _busyJobs--;
    //            _rb.gravityScale = 1;
    //            return;
    //        }
    //        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    //        await Task.Delay(25);
    //    }
    //    Debug.Log("Dash complete");
    //    _dashpower = 0;
    //    _busyJobs--;
    //    _rb.gravityScale = 1;
    //    _rb.velocity = direction *8; // this should be based on some vale provided in args
    //}

    async void Dash()
    {

        _busyJobs++;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        Vector2 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("dash started to " + destination);
        while (Mathf.Abs((destination - _rb.position).magnitude) > 1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, _dashpower * Time.deltaTime);
            await Task.Delay(25);
        }
        Debug.Log("Dash complete");
        _dashpower = 0;
        _busyJobs--;
        _rb.gravityScale = 1;

    }
    #endregion Unused Functions/Methods
}
