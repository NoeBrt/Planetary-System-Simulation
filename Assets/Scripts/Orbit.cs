using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[ExecuteInEditMode]
public class Orbit : MonoBehaviour
{

    [Header("Space Object")]
    [SerializeField] private float size = 1f;
    [Header("Orbit")]
    [SerializeField] private Transform orbitTo;
    [SerializeField] private float orbitRadius = 50f;
    [SerializeField] private float orbitSpeed = 30f;
    [SerializeField][Range(0f, 360f)] private float orbitStartAngle;
    [SerializeField] private Vector3 orbitOrientation;
    [SerializeField] private Vector3 orbitOffset;
    private Vector3 orbitDirection;
    private Quaternion orbitRotation;
    public static bool isFreeze { get; set; } = false;

    [Header("Ellipsis")]
    [SerializeField][Range(0.1f, 5)] float xElipticLength = 1.0f;
    [SerializeField][Range(0.1f, 5)] float zElipticLength = 1.0f;
    [SerializeField][Range(-50, 50)] float elipticOffset;
    public float ElipticOffsetBound { get; set; }

    [Header("Rotation")]
    [SerializeField] private bool isGravitLocked = false;
    [SerializeField] private bool isRotateSelf = false;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Trajectory path")]
    [SerializeField] LineRenderer trajectoryLine;
    [SerializeField] int tajectorySidesCount = 50;
    static public bool IsGuizmoVisible { get; set; } = true;

    [Header("UI")]
    GeneralUiManager UiManager;
    //Orbit speed alpha
    private float alpha = 0.0f;
    const float GRAVITY = 68.1f;
    //
    // Champ capsule 
    public float Size { get => size; set => size = value; }
    public float OrbitRadius { get => orbitRadius; set => orbitRadius = value; }
    public float OrbitSpeed { get => orbitSpeed; set => orbitSpeed = value; }
    public float OrbitStartAngle { get => orbitStartAngle; set => orbitStartAngle = value; }
    public Vector3 OrbitOrientation { get => orbitOrientation; set => orbitOrientation = value; }
    public Vector3 OrbitOffset { get => orbitOffset; set => orbitOffset = value; }
    public float XElipticLength { get => xElipticLength; set => xElipticLength = value; }
    public float ZElipticLength { get => zElipticLength; set => zElipticLength = value; }
    public float ElipticOffset { get => elipticOffset; set => elipticOffset = value; }
    public bool IsGravitLocked { get => isGravitLocked; set => isGravitLocked = value; }
    public bool IsRotateSelf { get => isRotateSelf; set => isRotateSelf = value; }
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    public Transform ObritTo { get => orbitTo; set => orbitTo = value; }
    //capsule

    private void Awake()
    {
        isFreeze = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        UiManager = GameObject.Find("Canvas").GetComponent<GeneralUiManager>();
        if (orbitTo != null)
        {
            trajectoryLine = Instantiate(trajectoryLine, Vector3.zero, Quaternion.identity);
        }
        transform.localScale = Vector3.one * Size;
        UpdateStartValue();
    }

    void UpdateStartValue()
    {
        if (orbitTo != null)
        {
            float xDirection = Mathf.Sin(Mathf.Deg2Rad * orbitStartAngle) * (orbitRadius * xElipticLength);
            float zDirection = Mathf.Cos(Mathf.Deg2Rad * orbitStartAngle) * (orbitRadius * zElipticLength);
            orbitDirection = new Vector3(xDirection, 0, zDirection);
            transform.position = orbitTo.position + (orbitRotation * (orbitDirection + orbitOffset + translateBetweenEliptFoci()));
            transform.rotation = orbitRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * Size;
        OnCollision();
        orbitRotation = Quaternion.Euler(orbitOrientation);
        if (!Application.isPlaying)
        {
            UpdateStartValue();
        }
        else if (orbitTo != null && transform.GetComponent<Camera>() == null)
        {
            OrbitMovement();
            Rotation();

        }
    }
    private void LateUpdate()
    {
        if (orbitTo != null)
        {
            if (transform.GetComponent<Camera>() != null)
            {
                OrbitMovement();
                Rotation();
            }
            else
            {
                DrawTrajectoryLine();

            }
        }
    }

    void Rotation()
    {
        if (isRotateSelf)
            SelfRotations();
        else if (isGravitLocked)
        {
            GravitLocked();
        }
        else
        {
            transform.rotation = orbitRotation;
        }
    }

    void OnCollision()
    {
        if (Physics.OverlapSphere(transform.position, (size / 2.5f)).Length > 1 && transform.GetComponent<Camera>() == null && !isFreeze)
        {
            isFreeze = true;
            UiManager.DislayCollisionTextText(true);
            Time.timeScale = 0;
            Debug.Log("collision " + Physics.OverlapSphere(transform.position, size / 2f)[0].gameObject.name + " | " + Physics.OverlapSphere(transform.position, size / 2f)[1].gameObject.name);

        }
    }

    void OrbitMovement()
    {
        // G*1/R * sign of orbitSpeed if he negative
        alpha += Mathf.Sqrt(Mathf.Abs(2 * GRAVITY / OrbitRadius)) * orbitSpeed * Time.deltaTime;
        float currentAngle = (orbitStartAngle + alpha);
        float xDirection = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * (orbitRadius * xElipticLength);
        float zDirection = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * (orbitRadius * zElipticLength);
        orbitDirection = new Vector3(xDirection, 0, zDirection);
        transform.position = orbitTo.position + (orbitRotation * (orbitDirection + orbitOffset + translateBetweenEliptFoci()));
    }


    Vector3 translateBetweenEliptFoci()
    {
        if (xElipticLength >= zElipticLength)
        {
            ElipticOffsetBound = Mathf.Abs((orbitRadius * xElipticLength) - orbitTo.localScale.x / zElipticLength);
            return Vector3.right * elipticOffset;
        }
        else
        {
            ElipticOffsetBound = Mathf.Abs((orbitRadius * zElipticLength) - orbitTo.localScale.z / xElipticLength);

            return Vector3.forward * elipticOffset;
        }
    }

    void SelfRotations()
    {
        //  transform.rotation = orbitRotation;
        transform.Rotate((orbitRotation * Vector3.up) * rotationSpeed * Time.deltaTime, Space.World);
    }
    void GravitLocked()
    {
        Vector3 newUp = orbitRotation * Vector3.up;
        Vector3 relativePos = orbitTo.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, newUp);
        transform.rotation = rotation;
    }
    public void UpdateOrbit(Transform orbitTo, float orbitRadius, float orbitSpeed, float orbitStartAngle, Vector3 orbitOrientation, Vector3 orbitOffset)
    {
        this.orbitStartAngle = orbitStartAngle;
        this.orbitRadius = orbitRadius;
        this.orbitSpeed = orbitSpeed;
        this.orbitOrientation = orbitOrientation;
        this.orbitOffset = orbitOffset;
        this.orbitTo = orbitTo;
    }
    public void UpdateOrbit(float orbitRadius, float orbitSpeed, float orbitStartAngle, Vector3 orbitOrientation, Vector3 orbitOffset)
    {
        this.orbitStartAngle = orbitStartAngle;
        this.orbitRadius = orbitRadius;
        this.orbitSpeed = orbitSpeed;
        this.orbitOrientation = orbitOrientation;
        this.orbitOffset = orbitOffset;
    }

    public void UpdateEllipsis(float xElipticLength, float zElipticLength, float elipticOffset)
    {
        this.xElipticLength = xElipticLength;
        this.zElipticLength = zElipticLength;
        this.elipticOffset = elipticOffset;
    }
    public void UpdateOjectRotation(bool isGravitLocked, bool isRotateSelf, float rotationSpeed)
    {
        this.isGravitLocked = isGravitLocked;
        this.isRotateSelf = isRotateSelf;
        this.rotationSpeed = rotationSpeed;
    }

    void OnDrawGizmos()
    {
        if (orbitTo != null && IsGuizmoVisible)
        {
            Gizmos.matrix = Matrix4x4.TRS(orbitTo.position + (Quaternion.Euler(orbitOrientation) * (orbitOffset + translateBetweenEliptFoci())), Quaternion.Euler(orbitOrientation), new Vector3(1f, 1f, 1f));

            Gizmos.DrawLine(Vector3.down * orbitRadius - translateBetweenEliptFoci(), Vector3.up * orbitRadius - translateBetweenEliptFoci());

            float previousX = 0;
            float previousZ = 0;
            float angle = 0f;

            for (int i = 0; i < tajectorySidesCount + 1; i++)
            {
                angle += (360f / tajectorySidesCount);

                float x = Mathf.Sin(Mathf.Deg2Rad * angle) * (orbitRadius * xElipticLength);
                float z = Mathf.Cos(Mathf.Deg2Rad * angle) * (orbitRadius * zElipticLength);
                if (i != 0)
                {
                    Gizmos.DrawLine(new Vector3(previousX, 0f, previousZ), new Vector3(x, 0f, z));
                }
                previousX = x;
                previousZ = z;

            }

        }
    }
    void DrawTrajectoryLine()
    {
        if (orbitTo != null && trajectoryLine.isVisible)
        {
            Vector3 pos = orbitTo.position + (Quaternion.Euler(orbitOrientation) * (orbitOffset + translateBetweenEliptFoci()));
            Quaternion q = Quaternion.Euler(orbitOrientation);
            trajectoryLine.positionCount = 51;
            trajectoryLine.startWidth = 1f;
            trajectoryLine.endWidth = 1f;
            float x;
            float z;
            float angle = 0f;
            for (int i = 0; i < tajectorySidesCount + 1; i++)
            {
                angle += (360f / tajectorySidesCount);
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * (orbitRadius * xElipticLength);
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * (orbitRadius * zElipticLength);
                trajectoryLine.SetPosition(i, q * (new Vector3(x, 0, z)) + pos);
            }

        }
    }
    public void showTrajectoryLine(bool isShow)
    {

        trajectoryLine.gameObject.SetActive(isShow);
    }
    private void OnDestroy()
    {
        if (trajectoryLine != null)
        {
            trajectoryLine.gameObject.SetActive(false);
        }
    }

}
