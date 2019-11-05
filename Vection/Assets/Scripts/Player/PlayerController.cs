using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
public class PlayerController : MonoBehaviour
{
    public CharacterController2D.CharacterCollisionState2D flags;
    public float walkSpeed = 6.0f;
    public float jumpSpeed = 10.0f;
    public float gravity = 20.0f;
    public float doubleJumpSpeed = 4.0f;
    public float wallJumpAmount = 1.5f;
    public float wallRunAmount = 2.0f;
    public float slopeSlideSpeed = 4.0f;
    public float glideAmount = 2.0f;
    public float glideTimer = 2.0f;
    public float creepSpeed = 3.0f;
    //Player Abilites toggle
    public bool canDoubleJump = true;
    public bool canWallJump = true;
    public bool canWallRun = true;
    public bool canGlide = true;
    public LayerMask layerMask;
    //Player State Variables
    public bool isGrounded;
    public bool isJumping;
    private bool doubleJumped;
    public bool isFacingRight;
    public bool wallJumped;
    public bool isWallRunning;
    public bool isSlopeSliding;
    public bool isCrouching;
    public bool isCreeping;
    public bool isGliding;
    // Private Variables
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController2D _characterController;
    private bool _lastJumpWasLeft;
    private float _slopeAngle;
    private Vector3 _slopeGradient = Vector3.zero;
    private bool _startGlide;
    private float _currentGlideTimer;
    private BoxCollider2D _collider;
    private Vector2 _originalColliderSize;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
        if(_characterController == null){
            Debug.Log("Character controller is null");
        }
        _currentGlideTimer = glideTimer;
        _collider = GetComponent<BoxCollider2D>();
        _originalColliderSize = _collider.size;
    }

    // Update is called once per frame
    void Update()
    {
        if(!wallJumped){
            _moveDirection.x = Input.GetAxis("Horizontal");
            _moveDirection.x *= walkSpeed;
        }

        // _moveDirection.x = Input.GetAxis("Horizontal");
        // _moveDirection.x *= walkSpeed;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, 2f, layerMask);
        if(hit){
            _slopeAngle = Vector2.Angle(hit.normal, Vector2.up); // Gets the angle of the slope
            _slopeGradient = hit.normal;

            if(_slopeAngle > _characterController.slopeLimit){
                isSlopeSliding = true;
            }else{
                isSlopeSliding = false;
            }
        }

        if(isGrounded){
            _currentGlideTimer = glideTimer;
            doubleJumped = false;
            isJumping = false;
            _moveDirection.y = 0;
            if(_moveDirection.x < 0){
                isFacingRight = false;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if(_moveDirection.x > 0){
                isFacingRight = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if(isSlopeSliding){
                _moveDirection = new Vector3(_slopeGradient.x * slopeSlideSpeed, -_slopeGradient.y * slopeSlideSpeed, 0f);
            }

            if(Input.GetButtonDown("Jump")){
                _moveDirection.y = jumpSpeed;
                isJumping = true;
                isWallRunning = true;
            }
        }else{
            // if the player is in the air
            if(Input.GetButtonUp("Jump")){
                if(_moveDirection.y > 0){
                    _moveDirection.y = _moveDirection.y / 2;
                }
            }
            if(Input.GetButtonDown("Jump") && !doubleJumped && canDoubleJump){
                _moveDirection.y = doubleJumpSpeed;
                doubleJumped = true;
            }
        }
        //  Gliding
        if(canGlide && Input.GetAxis("Vertical") > 0.5f && _characterController.velocity.y < 0.2f){
            if(_currentGlideTimer > 0){
                isGliding = true;
                if(_startGlide){
                    _moveDirection.y = 0;
                    _startGlide = false;
                }
                _moveDirection.y -= glideAmount * Time.deltaTime;
                _currentGlideTimer -= Time.deltaTime;
            }else{
                isGliding = false;
                _moveDirection.y -= gravity * Time.deltaTime;
            }
        }else{
            // Gravity
            _startGlide = true;
            isGliding = false;
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        _characterController.move(_moveDirection * Time.deltaTime);
        flags = _characterController.collisionState;

        isGrounded = flags.below;

        // Crouching or creeping
        RaycastHit2D hitCeiling = Physics2D.Raycast(transform.position, Vector2.up, 2f, layerMask); // Detects if an object is above the head

        if(Input.GetAxis("Vertical") < 0 && _moveDirection.x == 0){
            if(!isCrouching && !isCreeping){
                _collider.size = new Vector2(_collider.size.x, _collider.size.y / 2);
                transform.position = new Vector3(transform.position.x, transform.position.y - (_originalColliderSize.y / 4), 0);
                _characterController.recalculateDistanceBetweenRays();
            }
            isCrouching = true;
            isCreeping = false;
        }else if(Input.GetAxis("Vertical") < 0 && (_moveDirection.x > 0 || _moveDirection.x < 0)){
            if(!isCrouching && !isCreeping){
                _collider.size = new Vector2(_collider.size.x, _collider.size.y / 2);
                transform.position = new Vector3(transform.position.x, transform.position.y + (_originalColliderSize.y / 4), 0);
                _characterController.recalculateDistanceBetweenRays();
            }
            isCrouching = false;
            isCreeping = true;
        }else{
            if(!hitCeiling.collider && (isCrouching || isCreeping)){
                _collider.size = _originalColliderSize;
                transform.position = new Vector3(transform.position.x, transform.position.y + (_originalColliderSize.y / 4), 0);
                _characterController.recalculateDistanceBetweenRays();
                isCreeping = false;
                isCrouching = false;
            }
        }

        if(flags.above){
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        // Wall jumping and running
        if(flags.left || flags.right){
            if(canWallJump){
                if(Input.GetAxis("Vertical") > 0 && isWallRunning){
                    _moveDirection.y = jumpSpeed / wallRunAmount;
                    StartCoroutine(WallRunWaiter());
                }
            }
            if(canWallJump){
                // Checks for jump button not wall jumped and that the character is not grounded
                if(Input.GetButtonDown("Jump") && !wallJumped && !isGrounded){
                    if(_moveDirection.x < 0){
                        _moveDirection.x = jumpSpeed * wallJumpAmount;
                        _moveDirection.y = jumpSpeed * wallJumpAmount;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        _lastJumpWasLeft = false;
                    }
                    else if(_moveDirection.x > 0){
                        _moveDirection.x = -jumpSpeed * wallJumpAmount;
                        _moveDirection.y = jumpSpeed * wallJumpAmount;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        _lastJumpWasLeft = true;
                    }

                    StartCoroutine(WallJumpWaiter());
                }
            }
        }
    }

    IEnumerator WallJumpWaiter(){
        wallJumped = true;
        yield return new WaitForSeconds(0.5f);
        wallJumped = false;
    }

    IEnumerator WallRunWaiter(){
        isWallRunning = true;
        yield return new WaitForSeconds(1.5f);
        isWallRunning = false;
    }
}

