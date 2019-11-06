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
    private bool _isFacingRight;
    private bool _isGrounded;

    // Player Parameters
    [SerializeField] private float _gravity = 10.0f;
    [SerializeField] private float _runSpeed = 10.0f;
    [SerializeField] private float _jumpHeight = 10.0f;

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
    }

    void Update()
    {

        // Is Grounded Logic
        flags = _characterController.collisionState;
        _isGrounded = flags.below;
        if(_isGrounded){
            // Debug.Log("Is grounded");
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
            _characterController.move(_moveDirection * _runSpeed * Time.deltaTime);
        }
        // In Air Logic
        else{
            // Debug.Log("not grounded");
            _moveDirection.y = -_gravity;
            _characterController.move(_moveDirection * Time.deltaTime);
        }
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

