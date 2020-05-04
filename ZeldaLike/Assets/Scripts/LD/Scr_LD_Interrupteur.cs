﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Scr_LD_Interrupteur : MonoBehaviour
    {
        public bool _isActive;
        public GameObject _thingToActivate;
        public bool _isBig;

        // Start is called before the first frame update
        void Start()
        {
            _isActive = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isActive == false)
            {
                _thingToActivate.GetComponent<Scr_LD_ActiveState>()._isActive = false;
            }
            else if (_isActive == true)
            {
                _thingToActivate.GetComponent<Scr_LD_ActiveState>()._isActive = true;
            }
        }


        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Environment"))
            {
                
                if (collision.gameObject.GetComponent<Scr_HeavyMovable>()._isBig == true)
                {
                    _isActive = true;
                }

                else if (collision.gameObject.GetComponent<Scr_HeavyMovable>()._isBig == false && _isBig == false)
                {
                    _isActive = true;
                }
            }
            else if (collision.gameObject.transform.parent.parent.CompareTag("Player"))
            {
                _isActive = true;
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Environment"))
            {
                _isActive = false;
            }
            else if (collision.gameObject.transform.parent.parent.CompareTag("Player"))
            {
                _isActive = false;
            }
        }
    }
}

