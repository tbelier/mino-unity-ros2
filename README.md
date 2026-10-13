# ROS 2 Unity Maze Robot

**Learn ROS 2 by programming a virtual mobile robot in a maze.**

ROS 2 Unity Maze Robot is an educational robotics simulator built with Unity and the ROS-TCP-Connector. It provides a virtual mobile robot equipped with multiple sensors, a maze environment, and a ROS 2 interface for developing and testing autonomous robotics software.

The goal is to let anyone experiment with ROS 2, from basic topic communication to high-level robot autonomy, without requiring access to physical hardware.

The project is accompanied by video tutorials covering the development of the simulation and the setup of the ROS 2 communication interface.

## Features

- **Virtual maze environment:** experiment with robot navigation in a simulated world.
- **Four 1D LiDAR sensors:** front, rear, left, and right distance measurements.
- **Multiple cameras:** access the robot's camera views for perception and visualization experiments.
- **ROS 2 integration:** exchange messages with the simulator through the Unity ROS-TCP-Connector.
- **Velocity control:** control the robot by publishing ROS 2 messages.
- **Standalone application:** run the simulator without opening the Unity Editor.
- **Open development environment:** build your own ROS 2 nodes, algorithms, and high-level software architecture.

## Getting started

### 1. Download the simulator

Download the latest standalone build from the [GitHub Releases](../../releases) page.

Extract the archive and launch the application for your operating system.

You do not need to install Unity to run a standalone build.

### 2. Set up ROS 2

Install a supported ROS 2 distribution and configure the ROS-TCP-Endpoint on your ROS 2 machine.

The Unity simulator uses the ROS-TCP-Connector to communicate with ROS 2 over a TCP connection.

Make sure that:

- The ROS-TCP-Endpoint is running.
- Unity is configured with the correct IP address and port of the ROS-TCP-Endpoint.
- The required TCP port is reachable from the machine running the simulator.

### 3. Start developing

Create your own ROS 2 package and implement nodes that interact with the virtual robot.

For example, you can:

- Subscribe to the LiDAR measurements.
- Publish velocity commands.
- Implement keyboard teleoperation.
- Develop a wall-following algorithm.
- Build a maze-solving or autonomous navigation system.
- Design a complete high-level robotics architecture.

The simulator provides the virtual hardware interface. Your ROS 2 nodes implement the robot's intelligence.

## ROS 2 interface

The robot is identified by the namespace `/mino`.

### Topics

| Topic | Message type | Direction | Description |
|---|---|---|---|
| `/mino/lidar/scan` | `std_msgs/msg/Float32MultiArray` | Simulator → ROS 2 | LiDAR measurements |
| `/mino/control/u` | `geometry_msgs/msg/Twist` | ROS 2 → Simulator | Robot velocity commands |

The LiDAR topic provides measurements from the four 1D sensors. Consult [`docs/ros2_interface.md`](docs/ros2_interface.md) for the exact ordering of the sensors, array layout, units, and measurement conventions.

The control topic accepts velocity commands through a `Twist` message. Check the interface documentation for the supported linear and angular components.

### Inspect the topics

Once the simulator and ROS-TCP-Endpoint are running, use the ROS 2 command-line tools to inspect the available topics:

```bash
ros2 topic list
ros2 topic info /mino/lidar/scan
ros2 topic echo /mino/lidar/scan
ros2 topic info /mino/control/u
```

Check that the message types and topic names match the interface described above.

### Publish a velocity command

For a basic communication test, publish a `Twist` message:

```bash
ros2 topic pub --once /mino/control/u geometry_msgs/msg/Twist \
"{linear: {x: 0.1}, angular: {z: 0.0}}"
```

This sends a forward velocity command. Ensure the simulation is running and the robot's control interface is active.

To stop the robot, publish a zero-velocity command:

```bash
ros2 topic pub --once /mino/control/u geometry_msgs/msg/Twist \
"{linear: {x: 0.0}, angular: {z: 0.0}}"
```

## Video tutorials

Follow the video series to learn how the simulator works and how to build a ROS 2 robotics architecture.

0. **Introduction and project overview** — ▶️ [Watch video](https://youtu.be/6t4ymVSfwPo)
2. **Installing Unity, ROS2 and ROS-TCP-Connector** — ⏳ Coming soon
4. **Subscribing to Laser messages** — ⏳ Coming soon
5. **Publishing Twist Commands** — ⏳ Coming soon
6. **Creating a Server to get Camera Images** — ⏳ Coming soon
7. **Developing autonomous navigation algorithms** — ⏳ Coming soon

## Source code

The Unity project is located in [`unity/`](unity/).

The ROS 2 workspace contains example nodes and packages in [`ros2_ws/`](ros2_ws/).

To modify the simulator, open the Unity project using a compatible Unity Editor version. To develop your own autonomy stack, create a ROS 2 package in your workspace and connect it to the running simulator.

## License

This project is licensed under the MIT License.

You are free to use, copy, modify, merge, publish, distribute, sublicense, and sell copies of the software, subject to the terms of the license.

The MIT License requires the original copyright notice and license text to be included with copies or substantial portions of the software.

## Acknowledgements

Built with [Unity](https://unity.com/) and the [Unity Robotics ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector).

The project is intended for education, experimentation, and rapid prototyping of ROS 2 robotics software.
