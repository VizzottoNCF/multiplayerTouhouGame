using Unity.Mathematics;
using UnityEngine;

public class CircleBullets : MonoBehaviour
{
    [SerializeField] Transform bullet0;
    [SerializeField] Transform bullet1;
    [SerializeField] Transform bullet2;
    [SerializeField] Transform bullet3;
    [SerializeField] Transform bullet4;
    [SerializeField] Transform bullet5;
    [SerializeField] float spinDirection = 1f;
    [SerializeField] float rotationSpeed = 5f;

    private void Update()
    {
        Quaternion desiredRot = quaternion.Euler(0f, 0f, transform.rotation.z + spinDirection);

        // rotates towards desired rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, Time.deltaTime * rotationSpeed);
    }
}
