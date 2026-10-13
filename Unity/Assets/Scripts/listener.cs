
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class TalkerListener : MonoBehaviour
{
    void Start()
    {
        ROSConnection.GetOrCreateInstance()
            .Subscribe<StringMsg>("/chatter", OnMessage);
    }

    void OnMessage(StringMsg msg)
    {
        Debug.Log("Message ROS 2 reçu : " + msg.data);
    }
}
