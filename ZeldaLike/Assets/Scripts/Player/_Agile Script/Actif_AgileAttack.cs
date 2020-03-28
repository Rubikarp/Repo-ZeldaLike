﻿using System.Collections;
using UnityEngine;
using Management;

namespace Game
{
    public class Actif_AgileAttack : MonoBehaviour
    {
        [SerializeField] private InputManager _input = null;
        [SerializeField] private GameObject _Avatar = null;
        [SerializeField] private GameObject _HurtBox = null;
        [SerializeField] private AnimatorManager _animator = null;


        private Rigidbody2D _rgb = null;
        private Transform _myTranfo = null;

        [Header("Attaque data")]
        public GameObject _attackObj;

        public Transform _attackContainer;
        public Transform _attackPos;
        [SerializeField] private bool _canAttack = true;

        [Header("Bond")]
        public GameObject _bondObj;
        public float _jumpRange = 25f;
        public float _jumpLargeur = 2f;
        private bool _isBleeding = false;

        public float _bondSpeed = 30;
        public float _invulnerabiltyTimer = 1f;
        public float _invulnerabiltyTime;

        public float _canAttackTimer = 0.2f;
        public float _canAttackTime;

        public float _bondMaxDur = 1;
        public float _bondEndDist = 3;
        public float _attackCooldown = 1;

        private void Start()
        {
            _myTranfo = this.transform;
            _rgb = _Avatar.GetComponent<Rigidbody2D>();
            _input = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputManager>();
        }

        private void Update()
        {
            if (_input._attack && _canAttack)
            {
                //calcul angle
                float rotZ = Mathf.Atan2(_input._CharacterDirection.y, _input._CharacterDirection.x) * Mathf.Rad2Deg;
                RaycastHit2D hit = Physics2D.BoxCast(_rgb.position + _input._CharacterDirection * 5 - Vector2.Perpendicular(_input._CharacterDirection) * (_jumpLargeur/2), new Vector2(_jumpRange, _jumpLargeur), rotZ, _Avatar.transform.position);

                if (hit.collider != null)
                {
                    //Est ce que je touche un ennemi ?
                    if (hit.transform.CompareTag("Ennemis"))
                    {
                        _canAttack = false;
                        _canAttackTime = _canAttackTimer;

                        GameObject _actualTarget = Physics2D.BoxCast(_rgb.position + _input._CharacterDirection * 5, new Vector2(_jumpRange, _jumpLargeur), rotZ, _Avatar.transform.position).transform.gameObject;
                        
                        _isBleeding = _actualTarget.GetComponentInChildren<Int_EnnemisLifeSystem>().IsBleeding;
                        if (!_isBleeding)
                        {
                            _isBleeding = _actualTarget.GetComponent<Int_EnnemisLifeSystem>().IsBleeding;
                        }

                        //saigne t'il?
                        if (_isBleeding)
                        {
                            StartCoroutine(Bond(_Avatar.transform.position, _actualTarget.transform.position, _bondSpeed, _bondMaxDur));
                            _isBleeding = false;
                        }
                        //attaque classique
                        else
                        {
                            Instantiate(_attackObj, _attackPos.position, _attackPos.rotation, _Avatar.transform);
                            _animator.TriggerAttack();

                        }
                    }
                    else
                    {
                        //attaque classique
                        Instantiate(_attackObj, _attackPos.position, _attackPos.rotation, _Avatar.transform);
                        _animator.TriggerAttack();
                        _canAttack = false;
                    }
                }
                //attaque classique dans le vide
                else
                {
                    //attaque classique
                    Instantiate(_attackObj, _attackPos.position, _attackPos.rotation, _Avatar.transform);
                    _animator.TriggerAttack();
                    _canAttack = false;


                }
            }
            else
            {
                //invulnerability timer
                _invulnerabiltyTime -= Time.deltaTime;
                if (_invulnerabiltyTime <= 0)
                {
                    _HurtBox.SetActive(true);
                }

                //invulnerability timer
                _canAttackTime -= Time.deltaTime;
                if (_canAttackTime <= 0)
                {
                    _canAttack = true;
                }
            }
        }

        private IEnumerator Bond(Vector3 me, Vector3 cible, float speed, float maxDuration)
        {
            float distance = Vector2.Distance(_rgb.position, cible);
            Vector2 direction = cible - me;

            GameObject bond = Instantiate(_bondObj, _attackPos.position, _attackPos.rotation, _Avatar.transform);

            _HurtBox.SetActive(false);
            _invulnerabiltyTime = _invulnerabiltyTimer;

            while (_bondEndDist < distance) // boucle durant la durée du dash
            {
                _rgb.position += direction * speed * Time.deltaTime;

                distance = Vector2.Distance(_rgb.position, cible);
                maxDuration -= Time.deltaTime;
                //Debug.Log("distance = " + distance + "maxDuration = " + maxDuration);

                if (0 > maxDuration)
                {
                    break;
                }

                // Retour à la prochaine frame
                yield return new WaitForEndOfFrame();
            }

            yield return null;

            Destroy(bond);
            _canAttack = true;
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(_rgb.position + _input._CharacterDirection * 3 - Vector2.Perpendicular(_input._CharacterDirection) * (_jumpLargeur / 2), _input._CharacterDirection.normalized.normalized * _jumpRange, Color.blue);
            Debug.DrawRay(_rgb.position + _input._CharacterDirection * 3 + Vector2.Perpendicular(_input._CharacterDirection) * (_jumpLargeur / 2), _input._CharacterDirection.normalized.normalized * _jumpRange, Color.blue);
        }

        private void OnDisable()
        {
            _HurtBox.SetActive(true);
        }

        private void OnEnable()
        {
            _canAttack = true;
        }
    }
}