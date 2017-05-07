using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        private Rigidbody m_rb;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            m_rb = GetComponent<Rigidbody>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            // Arin added the code below :)
            Vector3 localVelocity = transform.InverseTransformDirection(m_rb.velocity);
            if(localVelocity.z > 20) {
                m_Car.SetMaxSteerAngle(15f);
            } else {
                m_Car.SetMaxSteerAngle(45f);
            }
        }
    }
}
