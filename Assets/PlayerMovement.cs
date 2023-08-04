using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
//using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlayerMovement : MonoBehaviour//, IKillable
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
    bool _isDashing; // could replace with more general purpose flag such as "Busy"?

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        rb = GetComponent<Rigidbody2D>();
        particle = GetComponent<ParticleSystem>();
        //var psMain = partical.main;


        //curve.AddKey(0.0f, 1.0f);
        //curve.AddKey(1.0f, 0.0f);
        particle.Play();
    }

    // Update is called once per frame
    void Update()
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

    public void Damage()
    {
        Debug.Log("The player has died");
        //GameControl.Instance.player.SetActive(false);
        //GameControl.Instance.gameOver.SetActive(true);
        for (int i = 0; i < 9; i++)
        {
            GameObject go = Instantiate(giblet);
            go.GetComponent<Rigidbody2D>().velocity = rb.velocity + new Vector2(Random.Range(-5, 15), Random.Range(-5, 15));
            go.transform.position = transform.position;
        }
    }

    async void Dash()
    {
        _isDashing = true;

    }
}
