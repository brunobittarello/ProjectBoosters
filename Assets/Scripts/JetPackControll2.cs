﻿using UnityEngine;
using System.Collections;
using Assets.Scripts;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CustomThirdPersonCharacter))]
    public class JetPackControll2 : MonoBehaviour
    {
        CustomThirdPersonCharacter ThirdPersonCharacter;

        public Transform TurbineRight;
        public Transform TurbineLeft;

        public Transform FireRight;
        public Transform FireLeft;

        Rigidbody RigidBody;
        public Vector3 RightForceDirection;
        public Vector3 LeftForceDirection;

        public float TurbineMaxAngle;
        public float TurbineMaxForce;
        public float RotationVelocity;

        public float MaxVelocity;


        public JetPackControll2()
        {
            RightForceDirection = new Vector3(-0.4f, 0.6f, 0);
            LeftForceDirection = new Vector3(0.4f, 0.6f, 0);
            TurbineMaxAngle = 30;
            TurbineMaxForce = 5;
            RotationVelocity = 3;
            MaxVelocity = 70;
        }

        // Use this for initialization
        void Start()
        {
            ThirdPersonCharacter = GetComponent<CustomThirdPersonCharacter>();
            RigidBody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (ThirdPersonCharacter.m_IsGrounded)
            {
                if (ThirdPersonCharacter.m_Rigidbody.velocity.magnitude > 1)
                {
                    Direction(0, 1, TurbineLeft);
                    Direction(0, 1, TurbineRight);
                }
                else
                {
                    Direction(0, 0, TurbineLeft);
                    Direction(0, 0, TurbineRight);
                }
            }
            else
            {
                if (Input.GetAxis("RotateCharacter") != 0)
                {
                    //if (Input.GetAxis("RotateCharacter") > 0)
                    // {
                    Direction(Input.GetAxis("RotateCharacter"), 0, TurbineLeft);
                    //    Direction(0, 0, TurbineRight);
                    //}
                    // else
                    //{
                    //    Direction(0, 0, TurbineLeft);
                    Direction(Input.GetAxis("RotateCharacter"), 0, TurbineRight);
                    // }
                }
                else
                {
                    var AbsHorizontal = Mathf.Abs(Input.GetAxis("Horizontal"));
                    if (AbsHorizontal > Mathf.Abs(Input.GetAxis("Vertical")))
                    {
                        Direction(0, AbsHorizontal, TurbineLeft);
                        Direction(0, AbsHorizontal, TurbineRight);
                    }
                    else
                    {
                        Direction(0,  Mathf.Abs(Input.GetAxis("Vertical")), TurbineLeft);
                        Direction(0,  Mathf.Abs(Input.GetAxis("Vertical")), TurbineRight);
                    }
                    //Direction(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), TurbineLeft);
                    //Direction(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), TurbineRight);
                }
            }

            //TurbineRight.transform.localRotation = TurbineLeft.transform.localRotation;

            UpdateJetPack(Input.GetAxis("RightTurbine"), Input.GetAxis("LeftTurbine"));
            VerifyMaxVelocity();
        }

        void UpdateJetPack(float rightPower, float leftPower)
        {
            Vector3 turbineDirectionL = Vector3.zero;
            Vector3 turbineDirectionR = Vector3.zero;

            if (rightPower > 0)
            {
                turbineDirectionR = transform.InverseTransformDirection((TurbineRight.up + RightForceDirection).normalized) * (rightPower * TurbineMaxForce);

                FireRight.gameObject.GetComponent<MeshRenderer>().enabled = true;
                FireRight.parent.localScale = new Vector3(1, rightPower, 1);
            }
            else
                FireRight.gameObject.GetComponent<MeshRenderer>().enabled = false;

            if (leftPower > 0)
            {
                turbineDirectionL = transform.InverseTransformDirection((TurbineLeft.up + LeftForceDirection).normalized) * (leftPower * TurbineMaxForce);

                FireLeft.gameObject.GetComponent<MeshRenderer>().enabled = true;
                FireLeft.parent.localScale = new Vector3(1, leftPower, 1);
            }
            else
                FireLeft.gameObject.GetComponent<MeshRenderer>().enabled = false;

            UIDebugger.JetPackForce = (turbineDirectionR + turbineDirectionL);
            RigidBody.AddRelativeForce((turbineDirectionR + turbineDirectionL));
        }

        void Direction(float horizontal, float vertical, Transform transform)
        {
            float tiltAroundZ = horizontal * TurbineMaxAngle;
            float tiltAroundY = vertical * TurbineMaxAngle;
            Quaternion target = Quaternion.Euler(tiltAroundY, 0, tiltAroundZ);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, 0.5f);
        }

        void Rotate()
        {
            transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("RotateCharacter") * RotationVelocity, Vector3.up);
        }

        void VerifyMaxVelocity()
        {
            if (RigidBody.velocity.magnitude > MaxVelocity)
                RigidBody.velocity = RigidBody.velocity.normalized * MaxVelocity;
        }
    }
}