using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 120.0f;

    private Rigidbody body;
    private float translationInput;
    private float rotationInput;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update ne fait que lire le clavier ; le déplacement physique se fait dans FixedUpdate.
    void Update()
    {
        translationInput = Input.GetAxis("Vertical");
        rotationInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Vector3 velocity = transform.forward * translationInput * speed;
        body.linearVelocity = new Vector3(velocity.x, body.linearVelocity.y, velocity.z);

        Quaternion turn = Quaternion.Euler(0.0f, rotationInput * rotationSpeed * Time.fixedDeltaTime, 0.0f);
        body.MoveRotation(body.rotation * turn);
    }

    void OnDisable()
    {
        translationInput = 0.0f;
        rotationInput = 0.0f;
        if (body != null) body.linearVelocity = new Vector3(0.0f, body.linearVelocity.y, 0.0f);
    }
}
