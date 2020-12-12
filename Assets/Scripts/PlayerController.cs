using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float torque;
    Rigidbody rb;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public void sendUpdate()
    {
        NetworkClient.PositionUpdate(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector3(xMov, rb.velocity.y, zMov) * speed;
        float turn = Input.GetAxis("Fire1");
        rb.AddRelativeTorque(Vector3.up * torque * turn);
    }

    
}
