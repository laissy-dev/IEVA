using UnityEngine;

// Controls the player character with the keyboard (arrows or ZQSD/WASD).
// Vertical axis moves forward/backward, Horizontal axis rotates left/right.
// Axes can be changed in File > Project Settings > Input Manager.
public class PlayerController : MonoBehaviour
{
    // Movement speed in units per second
    public float speed = 5.0f;
    // Rotation speed in degrees per second
    public float rotationSpeed = 120.0f;

    void Update()
    {
        // GetAxis returns a value between -1 and 1
        float translation = Input.GetAxis("Vertical");
        float rotation = Input.GetAxis("Horizontal");

        // Multiply by deltaTime so the movement does not depend on the frame rate
        translation *= speed * Time.deltaTime;
        rotation *= rotationSpeed * Time.deltaTime;

        // Translate is relative to the object's own axes: it moves where it faces
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}
