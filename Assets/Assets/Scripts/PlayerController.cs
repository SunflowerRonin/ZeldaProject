using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;

    [Header("Config Player")]
    public float movementSpeed = 3f;

    private Vector3 direction;
    private bool isWalk;

    //Input
    private float horizontal;
    private float vertical;

    [Header("Attack Config")]
    public ParticleSystem fxAttack;
    [SerializeField]
    private bool isAttack;
    public Transform hitBox;
    public int amountDmg;
    //[Range(n1,n2)] cria uma barra deslizante entre os valores dados
    [Range(0.2f, 1f)]
    public float hitRange = 0.5f;

    public LayerMask hitMask;

    public Collider[] hitInfo;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();

        MoveCharacter();

        UpdateAnimator();
    }

    #region MEUS METODOS

    void Inputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && isAttack == false)
        {
            Attack();
        }
    }

    void Attack()
    {
            isAttack = true;
            anim.SetTrigger("Attack");
            fxAttack.Emit(1);

        hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);


        foreach (Collider c in hitInfo)
        {
            c.gameObject.SendMessage("GetHit", amountDmg, SendMessageOptions.DontRequireReceiver);
        }

    }

    void MoveCharacter()
    {
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }

        controller.Move(direction * movementSpeed * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        anim.SetBool("IsWalk", isWalk);
    }

    void AttackIsDone()
    {
        isAttack = false;
    }
    #endregion 

    private void OnDrawGizmosSelected()
    {
        if (hitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitBox.position, hitRange);
        }
    }
}
