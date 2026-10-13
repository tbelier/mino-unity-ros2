
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class TwistRobotController : MonoBehaviour
{
    public string topic = "/mino/control/u";
    public float linearScale = 1f;
    public float angularScale = 1f;
    public float maxSpeed = 2f;
    public float maxRotation = 180f;

    private ROSConnection ros;
    private float linear;
    private float angular;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TwistMsg>(topic, OnTwist);
    }

    void OnTwist(TwistMsg msg)
    {
        linear = (float)msg.linear.x;
        angular = (float)msg.angular.z;
    }

    void Update()
    {
        float v = Mathf.Clamp(
            linear * linearScale, -maxSpeed, maxSpeed);

        float w = Mathf.Clamp(
            angular * angularScale * Mathf.Rad2Deg,
            -maxRotation, maxRotation);

        transform.position += transform.forward * v * Time.deltaTime;
        transform.Rotate(Vector3.up, w * Time.deltaTime, Space.World);
    }
}
