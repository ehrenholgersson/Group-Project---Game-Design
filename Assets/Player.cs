using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
//using static UnityEngine.ParticleSystem;

public class Player : MonoBehaviour, IDamage
{
    Rigidbody2D rb;
    bool grounded;
    bool jump;
    ParticleSystem particle;
    //AnimationCurve curve = new AnimationCurve();
    bool doubleJump = false;
    [SerializeField] GameObject giblet;
    Vector3 _force;
    float _dashpower;
    bool _isBusy; // could use state machine/enum instead?
    CircleCollider2D _meleeBox; // yes, its a circle - need to change this to the gameobject or possibly MeleeHitBox

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        rb = GetComponent<Rigidbody2D>();
        _meleeBox = transform.GetComponentInChildren<CircleCollider2D>(true);
    }

    public void ApplyDamage(float damage)
    {
        Debug.Log(gameObject.name + " is hit for " + damage + " damage.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBusy)
        {
            if (grounded)
            {
                if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y < 10)
                {
                    rb.velocity += new Vector2(0, 8);

                }
                if (Input.GetKey("a") && rb.velocity.x > -10)
                {
                    rb.velocity += new Vector2(-35, 0) * Time.deltaTime;
                }
                else if (Input.GetKey("d") && rb.velocity.y < 10)
                {
                    rb.velocity += new Vector2(35, 0) * Time.deltaTime;
                }
            }
            else if (doubleJump)
                if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y < 10)
                {
                    rb.velocity += new Vector2(0, 8);

                    doubleJump = false;
                }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _dashpower+=Time.deltaTime *200;
            }
            else if (_dashpower > 0)
            {
                Dash();
            }
            if (Input.GetKey(KeyCode.Tab))
                Melee(new Vector2(0.6f, 0), new Vector2(1, 1), 0.3f, 0.2f, 0f, 0f);
        }

    }

    void FixedUpdate()
    {
        rb.AddForce(_force);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer != 6)
        {
            grounded = true;
            doubleJump = true;
            rb.drag = 3;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
        rb.drag = 0.2f;
    }

    async void Dash()
    {

        _isBusy = true;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        Vector2 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("dash started to " + destination);
        while (Mathf.Abs((destination - rb.position).magnitude) > 1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, _dashpower*Time.deltaTime);
            await Task.Delay(25);
        }
        Debug.Log("Dash complate");
        _dashpower = 0;
        _isBusy = false;
        rb.gravityScale = 1;

    }
    async void Melee(Vector2 hitStart, Vector2 hitEnd, float attackTime,float hitSize, float startDelay,float EndDelay) //missing currently - Damage and knockback values
    {
        _isBusy = true;
        //Initial Delay
        float _timer = Time.time;
        while(Time.time < _timer + startDelay)
            await Task.Delay(25);
        //Attack
        _meleeBox.gameObject.SetActive(true);
        _meleeBox.transform.localPosition = hitStart;
        _meleeBox.transform.localScale = new Vector3(hitSize, hitSize,1);
        _timer = Time.time;
        while(Time.time < _timer + attackTime)
        {
            _meleeBox.transform.localPosition = Vector2.MoveTowards(hitStart, hitEnd,(Time.time-_timer)/attackTime);
            await Task.Delay(5);
        }
        _meleeBox.gameObject.SetActive(false);
        _timer = Time.time;
        while (Time.time < _timer + EndDelay)
            await Task.Delay(25);
        _isBusy = false;
    }
}
