﻿using System.Collections;
using UnityEngine;

namespace Ennemis
{
    public class Scr_EnnemisBehaviour_Soldat : MonoBehaviour
    {
        [Header("Data")]
        public Transform _mySelf = null;
        public GameObject _projectile;
        public Scr_EnnemisLifeSystem _lifeSyst = null;
        public Rigidbody2D _myBody = null;

        [Header("Shoot")]
        public float _detectionShootingRange = 18f;
        public float _shootingAllonge = 2f;
        public float _shootingRepos = 0.3f;
        public float _shootingCooldown = 0.9f;

        [Header("Charge")]
        public float _detectionChargingRange = 8f;
        public float _chargeSpeed = 35f;
        public float _chargeDuration = 2f;
        public float _chargeRepos = 0.3f;
        public float _chargeCooldown = 1.9f;

        [Header("Parameter detecting")]
        public bool _haveDetected = false;
        public bool _inDanger = false;

        [Header("Parameter shooting")]
        public bool _canShoot = true;
        public bool _isShooting = false;

        [Header("Parameter charging")]
        public bool _canCharge = true;
        public bool _isCharging = false;

        [Header("Target")]
        [SerializeField] private Transform _target = null;

        [HideInInspector] public Vector3 _targetDirection = Vector2.zero;
        private float _targetDistance = 0;

        private void Start()
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _mySelf = this.transform;
            _myBody = this.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _targetDirection = (_target.position - _mySelf.position);
            _targetDistance = Vector2.Distance(_mySelf.position, _target.position);
            _haveDetected = PlayerInShootingRange(_targetDistance, _detectionShootingRange, _detectionChargingRange);
            _inDanger = PlayerInChargingRange(_targetDistance, _detectionChargingRange);
        }

        private void FixedUpdate()
        {
            if(_haveDetected && !_lifeSyst._isTakingDamage)
            {
                if (_canShoot && !_isCharging)
                {
                    StartCoroutine(Shoot(_targetDirection, _shootingRepos, _shootingCooldown));
                }
            }

            if(_inDanger && !_lifeSyst._isTakingDamage)
            {
                if (_canCharge)
                {
                    StartCoroutine(Charge(_targetDirection, _chargeSpeed, _chargeDuration, _chargeRepos, _chargeCooldown));
                }
            }

            if (!_isCharging)
            {
                _myBody.velocity = Vector2.zero;
            }
        }

        public IEnumerator Shoot(Vector2 shootDirection, float _shootingRepos, float _shootingCooldown)
        {
            _canShoot = false;
            _isShooting = true;
            Instantiate(_projectile, _mySelf.position + _targetDirection.normalized * _shootingAllonge, _mySelf.rotation, _mySelf);

            yield return new WaitForSeconds(_shootingRepos);

            _isShooting = false;

            yield return new WaitForSeconds(_shootingCooldown);

            _canShoot = true;
        }

        public IEnumerator Charge(Vector2 chargeDirection, float chargeSpeed, float chargeDuration, float chargeRepos, float chargeCooldown)
        {
            _canCharge = false;
            _isCharging = true;

            while (0 < chargeDuration)
            {
                chargeDuration -= Time.deltaTime;

                _myBody.velocity = chargeDirection.normalized * chargeSpeed;

                yield return new WaitForEndOfFrame();
            }

            _myBody.velocity = Vector2.zero;

            yield return new WaitForSeconds(chargeRepos);

            _isCharging = false;

            yield return new WaitForSeconds(chargeCooldown);

            _canCharge = true;
        }

        #region Tools

        protected bool PlayerInShootingRange(float playerDistance, float testedShootingRange, float testedChargingRange)
        {
            if(playerDistance <= testedShootingRange && playerDistance >= testedChargingRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected bool PlayerInChargingRange(float playerDistance, float testedRealChargingRange)
        {
            if(playerDistance <= testedRealChargingRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
