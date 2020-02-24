﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Management;

namespace Game
{
    public class Movement_2D_TopDown : MonoBehaviour
    {
        [SerializeField] 
        private Rigidbody2D body = null;
        [SerializeField] 
        private InputManager input = null;
        [Space(10)]
        public Data_PlayerForme _actualForme = null;
        [Space(10)]
        [SerializeField] 
        private float _activeSpeed = 0f;

        private float _accTimer = 0f;
        private float _decTimer = 0f;
        private float _RunDeadZone = 0.5f;
        
        void Update()
        {
            Run();
        }

        void Run()
        {
            if (input._stickMagnitude > _RunDeadZone)
            {
                //incrémentation du timer en fonction du temps
                _accTimer += Time.deltaTime;


                //determine la vitesse
                if (_accTimer < _actualForme._accelerationCurve.keys[_actualForme._accelerationCurve.length - 1].time) //regarde le temps de la dernière key
                {
                    _activeSpeed = _actualForme._maxSpeed * _actualForme._accelerationCurve.Evaluate(_accTimer);
                }
                else
                {
                    _activeSpeed = _actualForme._maxSpeed * _actualForme._topSpeedCurve.Evaluate(_accTimer);
                }

                //applique la vitesse
                body.velocity = input._CharacterDirection * _activeSpeed;

                //Reset decceleration timer
                if (_decTimer != 0)
                {
                    _decTimer = 0f;
                }
            }
            else
            {
                //incrémentation du timer en fonction du temps
                _decTimer += Time.deltaTime;

                //determine la vitesse
                _activeSpeed = _actualForme._maxSpeed * _actualForme._deccelerationCurve.Evaluate(_decTimer);

                //applique la vitesse
                body.velocity = input._CharacterDirection * _activeSpeed;

                //Reset decceleration timer
                if (_accTimer != 0)
                {
                    _accTimer = 0f;
                }
            }
        }
    }
}