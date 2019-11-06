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
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
        if(_characterController == null){
            Debug.Log("Character controller is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

