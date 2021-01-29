using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float speed = 2.0f;
    private static int CURRENT = 0;
    private static int PREVIOUS = 1;

    private Vector3[] movement_axes = { new Vector3(), new Vector3() };
    private Vector3 MovementAxes{ get { return movement_axes[CURRENT]; } }
    private float view_angle = 0.0f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(movement_axes[CURRENT].magnitude > 0.0f)
            movement_axes[PREVIOUS] = movement_axes[CURRENT];

        view_angle = Vector3.SignedAngle(Vector3.forward, movement_axes[PREVIOUS], Vector3.up);

        movement_axes[CURRENT].x = Input.GetAxis("Horizontal");
        movement_axes[CURRENT].z = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        rb.velocity = MovementAxes * speed;
        transform.localRotation = Quaternion.Euler(0, view_angle, 0);
    }
}
