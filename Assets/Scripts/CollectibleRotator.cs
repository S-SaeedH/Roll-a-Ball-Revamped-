using UnityEngine;

public class CollectibleRotator : MonoBehaviour
{
    [Header("Rotation")]
    public float spinSpeed = 90f;
    public float tiltAngle = 15f;

    [Header("Bob")]
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;

    [Header("Glow")]
    public Renderer collectibleRenderer;
    public Color glowColor = new Color(0f, 1f, 1f, 1f); // cyan

    private Vector3 startPosition;
    private float bobTime;
    private Material mat;

    void Start()
    {
        startPosition = transform.position;
        bobTime = Random.Range(0f, Mathf.PI * 2f);

        if (collectibleRenderer != null)
        {
            mat = collectibleRenderer.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", glowColor * 1.5f);
        }
    }

    void Update()
    {
        // Spin
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

        // Tilt wobble
        float tilt = Mathf.Sin(Time.time * bobSpeed) * tiltAngle;
        transform.localRotation = Quaternion.Euler(
            tilt,
            transform.localEulerAngles.y,
            tilt * 0.5f
        );

        // Bob up and down
        bobTime += Time.deltaTime * bobSpeed;
        float newY = startPosition.y + Mathf.Sin(bobTime) * bobHeight;
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }
}
