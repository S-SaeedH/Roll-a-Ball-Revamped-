using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Movement")]
    public float ballSpeed  = 8.5f;
    public float jumpForce  = 4.5f;

    [Header("SFX Clips")]
    public AudioClip jumpSFX;
    public AudioClip rollSFX;

    [Header("SFX Volume")]
    [Range(0f, 1f)] public float jumpVolume = 0.6f;
    [Range(0f, 1f)] public float rollVolume = 0.2f;

    private Transform      cameraTransform;
    private AudioSource    rollAudioSource;
    private AudioSource    sfxAudioSource;
    private Rigidbody      rb;
    private LayerMask      groundMask;

    private float xInput, zInput;
    private bool  hasJump;

    private const float    rollThreshold = 0.5f;
    private Vector3        spawnPosition;
    private Quaternion     spawnRotation;

    void Start()
    {
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;

        groundMask    = LayerMask.GetMask("Ground");
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        rb            = GetComponent<Rigidbody>();

        cameraTransform = Camera.main.transform;

        AudioSource[] sources = GetComponents<AudioSource>();
        sfxAudioSource  = sources[0];
        rollAudioSource = sources.Length < 2
            ? gameObject.AddComponent<AudioSource>()
            : sources[1];

        rollAudioSource.clip         = rollSFX;
        rollAudioSource.loop         = true;
        rollAudioSource.volume       = 0f;
        rollAudioSource.playOnAwake  = false;
        rollAudioSource.spatialBlend = 0f;

        sfxAudioSource.loop          = false;
        sfxAudioSource.playOnAwake   = false;
        sfxAudioSource.spatialBlend  = 0f;
    }

    void OnMove(InputValue v)
    {
        Vector2 input = v.Get<Vector2>();
        xInput = input.x;
        zInput = input.y;
    }

    void OnJump()
    {
        if (!hasJump) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        hasJump = false;
        if (jumpSFX != null)
            sfxAudioSource.PlayOneShot(jumpSFX, jumpVolume);
    }

    void FixedUpdate()
    {
        hasJump = false;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight   = cameraTransform.right;
        camForward.y = 0f;
        camRight.y   = 0f;
        camForward.Normalize();
        camRight.Normalize();

        rb.AddForce((camForward * zInput + camRight * xInput) * ballSpeed);
    }

    void OnCollisionEnter(Collision collision) => HandleGroundCollision(collision);
    void OnCollisionStay(Collision collision)  => HandleGroundCollision(collision);

    void HandleGroundCollision(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            foreach (ContactPoint cp in collision.contacts)
            {
                if (cp.normal.y > 0.5f) { hasJump = true; break; }
            }
        }
        else if (collision.gameObject.CompareTag("Death"))
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            hasJump = false;
        }
    }

    void Update() => HandleRollSound();

    void HandleRollSound()
    {
        if (rollSFX == null) return;

        float speed      = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        bool  shouldRoll = speed > rollThreshold && hasJump;

        if (!rollAudioSource.isPlaying) rollAudioSource.Play();

        float targetVolume = shouldRoll ? Mathf.Lerp(0f, rollVolume, speed / 8f)         : 0f;
        float targetPitch  = shouldRoll ? Mathf.Lerp(0.65f, 1.25f,  speed / 15f)         : 0.8f;

        rollAudioSource.volume = Mathf.Lerp(rollAudioSource.volume, targetVolume, Time.deltaTime * 8f);
        rollAudioSource.pitch  = Mathf.Lerp(rollAudioSource.pitch,  targetPitch,  Time.deltaTime * 6f);
    }
}
