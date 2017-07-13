using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        public bool autoAgent;

        private float horizontalSteering, verticalAcceleration;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        private void Start()
        {
            horizontalSteering = 0f;
            verticalAcceleration = 1f;
        }

        private void FixedUpdate()
        {

            if (autoAgent)
            {
                m_Car.Move(horizontalSteering, verticalAcceleration, verticalAcceleration, 0f);
            } else {
                // pass the input to the car!
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
        #if !MOBILE_INPUT
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");
                m_Car.Move(h, v, v, handbrake);
        #else
                m_Car.Move(h, v, v, 0f);
        #endif
            }
        }

        public void SetSteering(float s)
        {
            Debug.Log("GilLog - CarUserControl::SetSteering - s " + s + " ");
            horizontalSteering = 2*s - 1f;
        }

        public void SetAcceleration(float s)
        {
            Debug.Log("GilLog - CarUserControl::SetAcceleration - s " + s + " ");
            verticalAcceleration = 2*s - 1f;
        }

    }
}
