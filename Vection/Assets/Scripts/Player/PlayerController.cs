using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
public class PlayerController : MonoBehaviour
{
    public CharacterController2D.CharacterCollisionState2D flags;

    // Private Variables
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController2D _characterController;
    private BoxCollider2D _collider;
    private Animator _animator;
    private bool _canJump;

    // Player Parameters
    [SerializeField] private float _gravity = 10.0f;
    [SerializeField] private float _runSpeed = 10.0f;
    [SerializeField] private float _jumpHeight = 2.0f;

    // Player State Variables
    private bool _isFacingRight;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isRunning;
    private bool _isSlowingTime;

    
    // Player Abilities
    [SerializeField] private bool _canFrontFlip;
    [SerializeField] private bool _canBackFlip;

    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
        if(_characterController == null){
            Debug.Log("Character controller is null");
        }
        _collider = GetComponent<BoxCollider2D>();
        if(_collider == null){
            Debug.Log("Box collider is null");
        }
        _animator = GetComponent<Animator>();
        if(_animator == null){
            Debug.Log("Animator is null");
        }
    }

    void Update()
    {

        // Slow Motion
        if(Input.GetKeyDown(KeyCode.Mouse1)){
            _isSlowingTime = !_isSlowingTime;
            Time.timeScale = _isSlowingTime ? 1F : 0.3F;
        }

        // Is Grounded Logic
        flags = _characterController.collisionState;
        _isGrounded = flags.below;
        if(_isGrounded){
            // Debug.Log("Is grounded");
            _canJump = true;
            _moveDirection.y = 0;
            float horizontal = Input.GetAxis("Horizontal");
            _moveDirection.x = horizontal;
            // If the player is moving right
            if(horizontal > 0){
                _isFacingRight = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            // The player is moving left
            else if(horizontal < 0){
                _isFacingRight = false;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }

            //Jumping Logic
            if(Input.GetButtonDown("Jump") && _canJump){
                _canJump = false;
                _moveDirection.y = _jumpHeight;
            }
        }
        // In Air Logic
        else{
            // Debug.Log("not grounded");
            _moveDirection.y -= _gravity * Time.deltaTime;
        }
        // Move the Character
        _characterController.move(_moveDirection * _runSpeed * Time.deltaTime);
        Animate();
    }

    private void Animate(){
        _animator.SetBool("isJumping", _isJumping);
        _animator.SetFloat("speed", _moveDirection.x == 0 ? 0 : 1);
    }

    // IEnumerator WallJumpWaiter(){
    //     // wallJumped = true;
    //     // yield return new WaitForSeconds(0.5f);
    //     // wallJumped = false;
    // }

    // IEnumerator WallRunWaiter(){
    //     isWallRunning = true;
    //     yield return new WaitForSeconds(1.5f);
    //     isWallRunning = false;
    // }
}

