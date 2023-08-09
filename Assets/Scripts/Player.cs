using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

// currently multiplying things by transform.localscale.x to change facing direction,this works but could cause problems later

public class Player : MonoBehaviour, IDamage
{
    Rigidbody2D _rb;
    bool _grounded;
    bool _doubleJump = false;
    bool _dashAvailable = false;
    bool _facingright = true;
    bool _jump;
    float _speed = 40;
    Vector2 _inputDirection;
    int _busyJobs;
    [SerializeField] GameObject _meleeBox;
    [SerializeField] Character _character;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
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
                    if (_rb.velocity.y < 30) // why dis? I don't remember, may remove
                    {
                        _rb.velocity += new Vector2(0, 12);

                        _doubleJump = false;
                    }
                _jump = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
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
    private void OnCollisionExit2D(Collision2D collision)// kinda redundant doing this for trigger and collision exit - but it seems to catch some edge cases
    {
        _grounded = false;
        _rb.drag = 0.2f;
    }
    private void OnTriggerExit2D(Collider2D collision) // kinda redundant doing this for trigger and collision exit - but it seems to catch some edge cases
    {
        Debug.Log("Trigger Exit");
        _grounded = false;
        _rb.drag = 0.2f;
    }

    public void ApplyDamage(float damage)
    {
        Debug.Log(gameObject.name + " is hit for " + damage + " damage.");
    }

    void Flip()
    {
        _facingright = (!_facingright);
        transform.localScale = (new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z));
    }

    #region Inputs
    public void OnMove(InputValue value)
    {
        _inputDirection = value.Get<Vector2>();
    }
    public void OnJump(InputValue value)
    {
        _jump = true;
       
    }

    public void OnMoveAction(InputValue value)
    {
        if (_dashAvailable&& !(_busyJobs > 0))
            if (_inputDirection.magnitude > 0.1f)
                MovetoPoint(_inputDirection * 4, 0.1f,0,0,true);
            else
                MovetoPoint(new Vector2(transform.localScale.x * 4, 0), 0.1f,0,0,true);
        Dodge(0.2f, 0f, 0f, false);
        _dashAvailable = false;
    }

    public void OnAction1(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Input1);
    }

    public void OnAction2(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Input2);
    }

    public void OnAction3(InputValue value) 
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Input3);
    }

    public void OnAction4(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Input4);
    }

    public void OnAction5(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Input5);
    }

    public void OnAction6(InputValue value)
    {
        if (!(_busyJobs > 0))
            ProcessActions(_character.Input6);
    }

    # endregion Inputs

    public void ProcessActions(List<Action> actions)
    {
        foreach (Action a in actions)
        {
            if (a.PlayerState == Action.State.Both || (a.PlayerState == Action.State.Grounded && _grounded) || (a.PlayerState == Action.State.Airborne && !_grounded))
                switch (a.Type)
                {
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
            await Task.Delay(25);
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
        hitBox.SetActive(false); // in case there is some delay in the destroy function
        Destroy(hitBox);
        // hitbox trigger was causing player to be incorrectly in grounded state, there is probably a better solution than this but it works
        _grounded = false;
        _rb.drag = 0.2f;

        // End Delay
        timer = Time.time;
        while (Time.time < timer + EndDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    async void Shoot(Vector2 direction, float damage, float knockback, Vector2 knockBackDirection, Vector2 startPoint, float speed, float startDelay, float EndDelay, GameObject prefab,bool busy)
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
            projectile.setup(direction, speed,damage,knockback, new Vector2(knockBackDirection.x * transform.localScale.x, knockBackDirection.y),1f,gameObject);
        }

        // End Delay
        _timer = Time.time;
        while (Time.time < _timer + EndDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;
    }

    #region Movement 

    async void MovetoPoint(Vector2 destination, float attackTime, float startDelay, float endDelay,bool busy) // todo -- if player is moved by some other means then also move the destination so that they do not "snap" back in weird way, also this currently has a timeout of 2 seconds, could make this an action defined variable but should use distance to destination vs speed to come up with a more suitable value
    {
        if (busy)
            _busyJobs++;
        float speed = destination.magnitude/attackTime;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        float timer = Time.time;
        while (Time.time < timer + startDelay)
            await Task.Delay(25);
        timer = Time.time;
        Vector2 direction = (destination).normalized;
        //        Vector2 setVelocity = _rb.position;
        //Debug.Log("Move started to " + destination + "at "+_rb.position);
        Debug.Log("speed is " + speed);
        while (Time.time < timer + attackTime)//(Mathf.Abs((destination - _rb.position).magnitude) > 1f && (Time.time < timer + 2))
        {
            _rb.velocity = direction * speed;//(destination - _rb.position).normalized * speed;
            await Task.Delay(25);
        }
        //Debug.Log("Move complete at "+_rb.position);
        _rb.gravityScale = 1;
        _rb.velocity = direction * (speed/4); // this should be based on some vale provided in args
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
        //Initial Delay
        float timer = Time.time;
        while (Time.time < timer + startDelay)
            await Task.Delay(25);
        gameObject.layer = 7;
        timer = Time.time;
        while (Time.time < timer + attackTime)
            await Task.Delay(25);
        gameObject.layer = 3;
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
            await Task.Delay(25);
        _rb.AddForce(force);
        timer = Time.time;
        while (Time.time < timer + endDelay)
            await Task.Delay(25);
        if (busy)
            _busyJobs--;

    }

    #endregion Movement 

    #endregion Action Functions/Methods
}
