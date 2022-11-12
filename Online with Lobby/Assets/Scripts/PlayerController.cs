using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, Hittable 
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 20f;

    private PlayerControls playerControls;
    private Rigidbody2D rb;

    public float playerVelocitySpeed = 50;
    public float rotationSpeed = 10;

    private Vector2 move, look;
    private Vector3 currentEulerAngles;
    private float angle;

    private float shoot;
    private bool canShoot = true;
    public float shootDelay = 0.5f;

    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private float weaponRange = 200;
    [SerializeField] private Animator muzzleFlashAnimator;
    [SerializeField] private LayerMask hitLayers;

    //photon - control only my character
    PhotonView view;
    

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
       
    }

    private void Update()
    {
        if (view.IsMine)
        {
            moveFunc();
            rotate();
        
            Shoot();

        }
    }


    private void moveFunc()
    {
        move = playerControls.Player.Move.ReadValue<Vector2>();
        if (move != Vector2.zero)
            rb.velocity = new Vector2(move.x * playerVelocitySpeed, move.y * playerVelocitySpeed);
        else
            rb.velocity = Vector2.zero;
    }

    private void rotate()
    {
        look = playerControls.Player.Look.ReadValue<Vector2>();
        angle = Vector3.SignedAngle(look - Vector2.zero, transform.up, Vector3.forward);
        currentEulerAngles += new Vector3(0, 0, -angle) * Time.deltaTime * rotationSpeed;
        transform.eulerAngles = currentEulerAngles;

    }

    private void Shoot()
    {
        shoot =  playerControls.Player.Shoot.ReadValue<float>();

        if (shoot == 1 && canShoot)
        {
            
            
            muzzleFlashAnimator.SetTrigger("Shoot");

            var hit = Physics2D.Raycast(
                shootingPoint.position,
                transform.up,
                weaponRange, hitLayers
                );

            var trail = Instantiate(
                bulletTrail, 
                shootingPoint.position, 
                transform.rotation
                );

            var trailScript = trail.GetComponent<BulletTrail>();

            if(hit.collider != null)
            {
                trailScript.SetTargetPos(hit.point);

                var hittable = hit.collider.GetComponent<Hittable>();
                hittable?.Hit(damage);
                
                Debug.Log(hittable);
               
            }
            else
            {
                var endPos = shootingPoint.position + transform.up * weaponRange;
                trailScript.SetTargetPos(endPos);
                Debug.Log(hit);
            }

            shoot = 0;
            canShoot = false;

            StartCoroutine(ShootDelay());
        }
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    public void Hit(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(this.gameObject);
        
    }


}
