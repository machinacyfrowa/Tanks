using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float engineForce, rotationForce;
    private bool isGrounded;
    private Transform tank, crosshair, tower;
    private Dictionary<String, Transform> wheels;
    Vector2 inputVector;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        tank = this.transform.Find("Tank");
        crosshair = this.transform.Find("Crosshair");
        tower = tank.Find("Tower");
        wheels = new Dictionary<String, Transform>();
        wheels.Add("FL", tank.Find("FLWheel"));
        wheels.Add("FR", tank.Find("FRWheel"));
        wheels.Add("BL", tank.Find("BLWheel"));
        wheels.Add("BR", tank.Find("BRWheel"));
    }

    // Update is called once per frame
    void Update()
    {
        //get mouse position on terain
        RaycastHit hit;
        
        Vector3 mousePositionOnScreen = Mouse.current.position.ReadValue();
        Ray cursorRay= Camera.main.ScreenPointToRay(mousePositionOnScreen);
        Physics.Raycast(cursorRay, out hit, Mathf.Infinity);
        //Debug.Log(hit.point);
        crosshair.position = hit.point;
        Quaternion targetTowerRotation = Quaternion.LookRotation(crosshair.position - transform.position, Vector3.forward);
        targetTowerRotation.x = 0;
        targetTowerRotation.z = 0;
        tower.rotation = Quaternion.Slerp(tower.rotation, targetTowerRotation, Time.deltaTime * 8);

    }

    private void FixedUpdate()  
    {
        if(isGrounded)
        {
            Vector3 wheelRotation = new Vector3(rb.velocity.magnitude*3*inputVector.y, 0, 0);
            foreach (Transform wheel in wheels.Values)
            {
                wheel.Rotate(wheelRotation);
            }
            if (inputVector.y != 0)
            {
                // accelerate to front or back
                rb.AddRelativeForce(Vector3.forward * inputVector.y * engineForce, ForceMode.Acceleration);
                
                
            }
            if (inputVector.x != 0)
            {
                // apply rotation to object
                rb.AddRelativeTorque(Vector3.up * inputVector.x * rotationForce, ForceMode.Acceleration);
            }
        }
        
    }

    void OnMove(InputValue inputValue)
    {
        inputVector = inputValue.Get<Vector2>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
            isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
            isGrounded = false;
    }
}
