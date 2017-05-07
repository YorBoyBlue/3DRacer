using UnityEngine;

public class ChaseCamera : MonoBehaviour {
    private Rigidbody m_CarRB;
	public Transform m_car;
    public float m_distance;
    public float m_height;
    public float m_rotationDamping = 3f;
    public float m_heightDamping;
    public float m_desiredAngle = 0;
    private float m_fadeTimer = 5f;

    void Start() {
        m_CarRB = m_car.GetComponent<Rigidbody>();
    }
    
    void LateUpdate() {
        float currentAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;
        //m_desiredAngle = m_car.eulerAngles.y;
        float desiredHeight = m_car.position.y + m_height;

        currentAngle = Mathf.LerpAngle(currentAngle, m_desiredAngle, m_rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, m_heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);

        // Set the new position
        Vector3 finalDestination = new Vector3(m_car.position.x, m_car.position.y + 3, m_car.position.z);
        Vector3 finalPosition = m_car.position - (currentRotation * Vector3.forward * m_distance);
        finalPosition.y = currentHeight;
        transform.position = finalPosition;

        transform.LookAt(finalDestination);
    }

    void Update() { 
        if(m_fadeTimer > -5f) {
            m_fadeTimer -= Time.deltaTime;
        }
        if(m_fadeTimer > 0) {
            m_heightDamping = 1f;
        } else {
            if(m_heightDamping < 50f) {
                m_heightDamping += 5f * Time.deltaTime;
            }
        }
    }

    void FixedUpdate() {
        m_desiredAngle = m_car.eulerAngles.y;

        Vector3 localVelocity = m_car.InverseTransformDirection(m_CarRB.velocity);
        
        if(localVelocity.z < -0.5f && Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.E) && localVelocity.z > -0.5f) {
            m_desiredAngle += 180;
        }
    }
}
