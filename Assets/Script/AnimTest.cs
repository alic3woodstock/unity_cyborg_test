using UnityEngine;
using UnityEngine.InputSystem;

public class AnimTest : MonoBehaviour
{
    public InputAction jump;
    public InputAction motion;
    public Animator animator;
    public Rigidbody gravityControl;
    public CapsuleCollider playerCollider;

    private float position;
    private float colliderDiff = 0.96f;  
    private bool initCjump = false;

    void Awake()
    {
        jump.performed += ctx => Jump();        
    }

    // Start is called before the first frame update
    void Start()
    {
        position = 0;
        animator.SetFloat("Motion",2.1f);
    }

    void FixedUpdate()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsTag("cjump")) {
            gravityControl.useGravity = animator.GetFloat("Motion") > 2;

            if (animator.GetFloat("Motion") <= 1) {
                colliderDiff = info.normalizedTime * 1.14f + 0.96f;
                if (colliderDiff > 2.1f) {
                    colliderDiff = 2.1f;
                }
            } else {
                colliderDiff = (1.0f - info.normalizedTime) * 1.14f + 0.96f;
                if (colliderDiff < 9.6f) {
                    colliderDiff = 9.6f;
                }
            }

            playerCollider.center = playerCollider.center.x * Vector3.right 
                + playerCollider.center.z * Vector3.forward + colliderDiff * Vector3.up;
            initCjump = true;
        } else if (initCjump){
            Debug.Log(info.IsTag("cjump").ToString());
            playerCollider.center = 0.96f * Vector3.up;
            colliderDiff = 0.96f;
            gravityControl.useGravity = true;
            initCjump = false;
        }
        Debug.Log(colliderDiff.ToString() + ", " + animator.GetFloat("Motion").ToString() + ", "
            + playerCollider.gameObject.transform.position.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        position += motion.ReadValue<float>() * 0.01f;
        animator.SetFloat("Motion", position);
    }

    void OnEnable()
    {
        jump.Enable();  
        motion.Enable();      
    }

    void OnDisable() {
        jump.Disable();
        motion.Disable();
    }

    void Jump() {
        animator.SetFloat("Motion", 0);
        animator.SetTrigger("Start Jump");
    }
}
