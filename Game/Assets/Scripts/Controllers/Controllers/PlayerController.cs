using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class PlayerController : MonoBehaviour
{
    CharacterController _controller;
    Animator anim;


    //이동
    public float _walkSpeed = 3.0f;
    public float _runSpeed = 6.0f;

    //회전
    public float _turnSpeed = 10.0f;
    public float _cameraMoveSpeed = 2f;
    public float _maxUpAngle = 80f;
    public float _maxDownAngle = 30f;
    private float _xRotation = 0f;
    private float _yRotation = 0f;

    public float _offset = 0.1f;
    public float _minDistance = 2f;
    public float _defaultCameraDistance = 6f;


    public enum PlayerState
    {
        Idle, 
        Walk,
        Run,
        Interact
    }

    PlayerState _state = PlayerState.Idle;
    public PlayerState State { get { return _state; } set { _state = value; }}

    void UpdateIdle()
    {
        Move(_walkSpeed);
        Rotate();
        anim.SetFloat("move_speed", 0);
    }

    void UpdateWalk()
    {
        Move(_walkSpeed);
        Rotate();
        anim.SetFloat("move_speed", _walkSpeed);
    }

    void UpdateRun()
    {
        Move(_runSpeed);
        Rotate();
        anim.SetFloat("move_speed", _runSpeed);
        if(Input.GetKey(KeyCode.W))
            Managers.Energy.DecreaseEnergy(0.3f * Time.deltaTime);

    }

    void UpdateInteract()
    {
        anim.SetFloat("move_speed", 0);
    }


    void Move(float speed) //키보드 입력에 따른 플레이어 이동
    {
        if (_controller.enabled == false)
            return;
        //키 입력 이벤트 -> 키보드 이동 처리
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        anim.SetFloat("vertical", vertical);
        anim.SetFloat("horizontal", horizontal);

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        _controller.Move(transform.TransformDirection(moveDirection * speed * Time.deltaTime));

        //움직임이 있으면 Walk 없으면 Idle
        if (moveDirection.magnitude > 0.0001f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _state = PlayerState.Run;
            else
                _state = PlayerState.Walk;
        }    
        else
            _state = PlayerState.Idle;


    }

    void Rotate()//마우스 이동에 따른 시야 회전
    {
        //플레이어와 카메라 사이에 벽이 있는지 판단하여 거리 조절
        float cameraDistance;

        Vector3 lookAtPosition = transform.position + new Vector3(0f, _controller.height * 2 / 3, 0f);
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 rayOrigin = lookAtPosition + (cameraPos - lookAtPosition).normalized * _defaultCameraDistance;
        float rayDistance = Vector3.Distance(rayOrigin, lookAtPosition) - 0.5f;


        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, (lookAtPosition - rayOrigin).normalized, out hit, rayDistance))
        {
            if (!hit.transform.gameObject.CompareTag("NPC") && hit.collider.isTrigger == false)
                cameraDistance = Mathf.Max(Vector3.Distance(hit.point, lookAtPosition) - _offset, 0f);
            else
                cameraDistance = _defaultCameraDistance;
        }
        else
        {
            cameraDistance = _defaultCameraDistance;
        }

        //마우스 이동에 따른 카메라 회전, 이동
        float mouseX = Input.GetAxis("Mouse X") * _turnSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * _turnSpeed;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, _maxDownAngle, _maxUpAngle);
        _yRotation += mouseX;

        Quaternion yRotation = Quaternion.Euler(0f, _yRotation, 0f);
        Quaternion xRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        transform.localRotation = yRotation;
        Camera.main.transform.localRotation = xRotation * yRotation;

        float y = Mathf.Sin(_xRotation * Mathf.Deg2Rad) * cameraDistance;
        float z = Mathf.Cos(_xRotation * Mathf.Deg2Rad) * cameraDistance;

        Vector3 targetPosition = transform.position + transform.TransformVector(0f, y, -z);
        Vector3 currentPosition = Camera.main.transform.position;
        Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * _cameraMoveSpeed);
        Camera.main.transform.position = newPosition;
        Camera.main.transform.LookAt(lookAtPosition);
    }


    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //플레이어 상태 변화
        switch (_state)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Walk:
                UpdateWalk();
                break;
            case PlayerState.Run:
                UpdateRun();
                break;
            case PlayerState.Interact:
                UpdateInteract();
                break;
        }
    }

}
