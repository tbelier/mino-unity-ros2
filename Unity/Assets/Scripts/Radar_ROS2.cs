
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

[ExecuteAlways]
public class Radar_ROS2 : MonoBehaviour
{
    [Header("Azimut (horizontal)")]
    [Range(1f, 360f)] public float azimuthFOV = 360f;
    [Range(1, 720)] public int azimuthLasers = 360;

    [Header("Élévation (vertical)")]
    [Range(1f, 180f)] public float elevationFOV = 10f;
    [Range(1, 64)] public int elevationLasers = 2;

    [Header("Radar Physics")]
    public float maxDistance = 10f;
    public float I0 = 220f;

    [Header("Laser Display")]
    public bool showLasers = true;
    public Color laserColor = Color.cyan;
    [Range(0f, 1f)] public float opacity = 0.7f;
    [Range(0.001f, 0.1f)] public float laserWidth = 0.01f;

    [Header("ROS-TCP Connector")]
    public string topicName = "/m4ze/radar/scan";

    [Header("Timing")]
    public float refreshRate = 0.5f;

    private ROSConnection ros;
    private float lastScanTime;
    private float lastEditorUpdate;
    private Material laserMaterial;
    private readonly List<LineRenderer> lasers = new();

    void OnEnable()
    {
        EnsureLasers();
        UpdateLaserVisibility();

        if (Application.isPlaying)
        {
            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<Float32MultiArrayMsg>(topicName);
        }
    }

    void Start()
    {
        if (!Application.isPlaying) return;

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32MultiArrayMsg>(topicName);
    }

    void OnValidate()
    {
        if (!isActiveAndEnabled) return;

        EnsureLasers();
        UpdateLaserVisibility();
    }

    void Update()
    {
        if (!showLasers)
        {
            UpdateLaserVisibility();
        }
        else
        {
            float now = Application.isPlaying
                ? Time.time
                : Time.realtimeSinceStartup;

            float interval = Application.isPlaying
                ? Mathf.Max(refreshRate, 0.02f)
                : 0.2f;

            if (now - lastEditorUpdate >= interval)
            {
                lastEditorUpdate = now;
                EnsureLasers();
                Sweep(Application.isPlaying &&
                      now - lastScanTime >= interval);

                if (Application.isPlaying &&
                    now - lastScanTime >= interval)
                    lastScanTime = now;
            }
        }
    }

    void OnDisable()
    {
        if (Application.isPlaying)
            return;

        UpdateLaserVisibility();
    }

    void OnDestroy()
    {
        if (laserMaterial != null)
        {
            if (Application.isPlaying)
                Destroy(laserMaterial);
            else
                DestroyImmediate(laserMaterial);
        }
    }

    static float Angle(int i, int n, float fov)
    {
        if (n == 1) return 0f;

        float step = fov >= 360f
            ? fov / n
            : fov / (n - 1);

        return -fov / 2f + i * step;
    }

    void EnsureLasers()
    {
        int total = azimuthLasers * elevationLasers;

        lasers.Clear();

        foreach (Transform child in transform)
        {
            if (child.name != "RadarLaser") continue;

            LineRenderer line = child.GetComponent<LineRenderer>();

            if (line != null)
                lasers.Add(line);
        }

        while (lasers.Count > total)
        {
            LineRenderer line = lasers[lasers.Count - 1];
            lasers.RemoveAt(lasers.Count - 1);

            if (Application.isPlaying)
                Destroy(line.gameObject);
            else
                DestroyImmediate(line.gameObject);
        }

        if (laserMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");

            if (shader != null)
                laserMaterial = new Material(shader);
        }

        while (lasers.Count < total)
        {
            GameObject obj = new GameObject("RadarLaser");
            obj.transform.SetParent(transform, false);

            LineRenderer line = obj.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.sharedMaterial = laserMaterial;
            line.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;

            lasers.Add(line);
        }

        UpdateLaserVisibility();
    }


    void UpdateLaserVisibility()
    {
        foreach (LineRenderer line in lasers)
        {
            if (line != null)
                line.enabled = showLasers;
        }
    }

    void Sweep(bool publish)
    {
        EnsureLasers();

        List<float> data = publish
            ? new List<float>(azimuthLasers * elevationLasers * 4)
            : null;

        Vector3 origin = transform.position;
        Color c = laserColor;
        c.a = opacity;

        int k = 0;

        for (int a = 0; a < azimuthLasers; a++)
        {
            float az = Angle(a, azimuthLasers, azimuthFOV);

            for (int e = 0; e < elevationLasers; e++)
            {
                float el = Angle(e, elevationLasers, elevationFOV);

                Vector3 dir = transform.TransformDirection(
                    Quaternion.Euler(-el, az, 0f) *
                    Vector3.forward);

                bool hit = Physics.Raycast(
                    origin, dir, out RaycastHit h, maxDistance) &&
                    !h.collider.CompareTag("Intangible");

                float d = hit ? h.distance : maxDistance;

                LineRenderer line = lasers[k++];

                if (showLasers && line != null)
                {
                    line.enabled = true;
                    line.sharedMaterial = laserMaterial;
                    line.startWidth = laserWidth;
                    line.endWidth = laserWidth;
                    line.startColor = c;
                    line.endColor = c;
                    line.SetPosition(0, origin);
                    line.SetPosition(1, origin + dir * d);
                }

                if (publish)
                {
                    data.Add(az);
                    data.Add(el);
                    data.Add(d);
                    data.Add(hit
                        ? I0 / Mathf.Max(d * d, 0.001f)
                        : 0f);
                }
            }
        }

        if (publish && ros != null)
        {
            ros.Publish(topicName,
                new Float32MultiArrayMsg
                {
                    data = data.ToArray()
                });
        }
    }
}
