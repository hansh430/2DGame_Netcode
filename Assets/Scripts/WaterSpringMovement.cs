using UnityEngine;

public class WaterSpringMovement : MonoBehaviour
{
    public float velocity = 0;
    public float force = 0;
    public float height = 0f;
    public float targetHeight = 0f;

    public void WaveSprintUpdate(float springStiffness, float dampening)
    {
        height = transform.localPosition.y;
        var x = height - targetHeight;
        var loss = -dampening * velocity;
        force = -springStiffness * x + loss;
        velocity += force;
        var y = transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, y + velocity, transform.localPosition.z);
    }
}
