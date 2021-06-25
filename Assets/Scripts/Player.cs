using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnGameWon;
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    bool disabled;
    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardSpotted += Disable;
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if(!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Finish")
        {
            Disable();
            if(OnGameWon!=null)
            {
                OnGameWon();
            }
        }
    }

    void Disable()
    {
        disabled = true;
    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    private void OnDestroy()
    {
        Guard.OnGuardSpotted -= Disable;
    }
}