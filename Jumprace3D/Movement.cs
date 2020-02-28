using UnityEngine;
using System.Collections;

// A very simplistic car driving on the x-z plane.

public class Movement : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    public Joystick verJoystick;
    public Joystick horJoyStick;

    void Update() {
        if (GameManager.Instance.gameIsComplete)
            return;
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;


        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);

        // Move translation along the object's z-axis
        if (translation < 0) return;
        transform.Translate(0, 0, translation);


        // Mobile input
        float mTranslation = verJoystick.Vertical * speed;
        float mRotation = horJoyStick.Horizontal * rotationSpeed;

        mTranslation *= Time.deltaTime;
        mRotation *= Time.deltaTime;

        transform.Rotate(0,mRotation,0);

        if(mTranslation < 0) return;
        transform.Translate(0,0,mTranslation);
        
    }
}