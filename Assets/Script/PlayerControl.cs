using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

//TODO: 
//Colisions when jump
//Camera collisions
//Fall

public class PlayerControl : MonoBehaviour
{
    public GameObject player;
    public GameObject mainCamera;

    public Vector3 cameraOffset;

    public Animator animator;
    public InputAction move;
    public InputAction look;
    public InputAction jump;
    public InputAction crouch;

    private float sensitivityX = 8.0f;
    private float sensitivityY = 7.0f;
    private float timeCount = 0.0f;
    private float playerSpeed = 0.0f;
    private float playerVelocity = 10.0f;
    private float maxSpeed = 10.0f;
    private bool changeLookPos = false;
    private bool isColiding = false;
    private Vector2 moveCamera;
    private Vector3 playerTranslate;
    private Vector3 lookPoint;
    private GameObject headLook;
    private Quaternion rotationTo;
    private Quaternion rotationFrom;  
    private CapsuleCollider playerCollider;

    void Awake()
    {
        jump.performed += ctx => Jump();
        crouch.performed += ctx => Crouch();
    }

    // Start is called before the first frame update
    void Start()
    {        
        headLook = new GameObject("HeadLook");
        playerTranslate = player.transform.position;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  
        moveCamera = new Vector2(0,0);
        mainCamera.transform.position = player.transform.position + cameraOffset;    
        lookPoint = player.transform.position;
        rotationFrom = player.transform.rotation;
        rotationTo = player.transform.rotation;
        maxSpeed = 10.0f;
        headLook.transform.eulerAngles = new Vector3(0, mainCamera.transform.eulerAngles.y,0);         
        playerCollider = player.GetComponents<CapsuleCollider>()[0];
    }

    // Update is called once per frame
    void Update(){
        headLook.transform.position = player.transform.position;
        headLook.transform.eulerAngles = new Vector3(0, mainCamera.transform.eulerAngles.y,0); 

        Vector2 playerMoviment = move.ReadValue<Vector2>();
        if ((!changeLookPos) && (playerMoviment != Vector2.zero)) {   
            headLook.transform.Translate(playerMoviment.y*Vector3.forward*Time.deltaTime*10);
            headLook.transform.Translate(playerMoviment.x*Vector3.right*Time.deltaTime*10);
            rotationTo.SetLookRotation(headLook.transform.position - player.transform.position);
            rotationFrom = player.transform.rotation;
            changeLookPos = true;            
        }

        if (changeLookPos){            
            player.transform.rotation = Quaternion.Slerp(rotationFrom, rotationTo, timeCount);
            timeCount += Time.deltaTime * 4;
            if ((timeCount >= 1) || (player.transform.rotation.Compare(rotationTo, 10))){
                player.transform.rotation = rotationTo;
                timeCount = 0;
                changeLookPos = false;
            }            
        } 
        
        if (playerMoviment != Vector2.zero) {
            playerSpeed += Mathf.Abs(playerMoviment.x*Time.deltaTime*playerVelocity);
            playerSpeed += Mathf.Abs(playerMoviment.y*Time.deltaTime*playerVelocity);
            if (playerSpeed > maxSpeed) {
                playerSpeed = maxSpeed;
            }
        } else {
            if (playerSpeed > -1) {
                playerSpeed -= 1* Time.deltaTime*playerVelocity;
            } else {
                playerSpeed = -1;
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("mjump")) {
            if (!isColiding) {
                player.transform.Translate(Vector3.forward * Time.deltaTime * playerVelocity);
                playerSpeed = maxSpeed;
            }
        }

        animator.SetFloat("Speed", playerSpeed);

     }

    void LateUpdate()
    {              
        playerTranslate = (player.transform.position - playerTranslate);
        if (playerTranslate != Vector3.zero) {
            mainCamera.transform.position += playerTranslate;
        }
        playerTranslate = Vector3.zero;
        moveCamera = look.ReadValue<Vector2>()*Time.deltaTime;    
        Vector3 alturaRelativa = cameraOffset.y * Vector3.up;

        Vector3 rotatePointY = player.transform.position + (cameraOffset.y * Vector3.up);
        Vector3 camPosAnterior = mainCamera.transform.position;

        if (moveCamera.y != 0) {
            mainCamera.transform.RotateAround(rotatePointY, mainCamera.transform.right, 
                moveCamera.y * sensitivityY);
        }

        if ((mainCamera.transform.eulerAngles.x > 45) && (mainCamera.transform.eulerAngles.x < 315)) {
            mainCamera.transform.position = camPosAnterior;
        }


        //move camera horizontally
        Vector3 rotatePointX = mainCamera.transform.position.y * Vector3.up + 
            new Vector3(player.transform.position.x,0, player.transform.position.z);

        if (moveCamera.x != 0) { 
            mainCamera.transform.RotateAround(rotatePointX, 
                Vector3.up, moveCamera.x * sensitivityX);
        }
        
        mainCamera.transform.Translate(cameraOffset.x*Vector3.left);
        mainCamera.transform.LookAt(player.transform.position + (cameraOffset.y*Vector3.up), Vector3.up);
        playerTranslate = player.transform.position;        
        mainCamera.transform.Translate(cameraOffset.x*Vector3.right);    
    }

    void OnEnable()
    {
        move.Enable();
        look.Enable();
        jump.Enable();
        crouch.Enable();
    }

    void OnDisable()
    {
        move.Disable();
        look.Disable();
        jump.Disable();
        crouch.Disable();
    }

    void Jump() {
        animator.SetFloat("Motion", 0);
        animator.SetTrigger("Jump");
    }

    void Crouch() {
        animator.SetBool("Crouch", !(animator.GetBool("Crouch")));
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y == 0) {
                isColiding = true;
            }
            Debug.Log(contact.point.ToString() + ", " + contact.normal.ToSafeString());
        }
    }

    void OnCollisionExit(Collision other)
    {
      if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("mjump")) {
          isColiding = false;
      }   
    }
 }
