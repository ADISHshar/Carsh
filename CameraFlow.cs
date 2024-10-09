using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The car's transform
    public Vector3 offset; // The offset between the camera and the car
    public float positionSmoothSpeed = 0.125f;
    public float rotationSmoothSpeed = 0.05f;

    private Vector3 velocity = Vector3.zero; // Used for smooth damping

    private void LateUpdate()
    {
        // Calculate the desired position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        // Smoothly interpolate the position of the camera using SmoothDamp
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothSpeed);
        transform.position = smoothedPosition;

        // Calculate the desired rotation
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, Vector3.up);

        // Smoothly interpolate the rotation of the camera using Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed);
    }
}
