﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Ennemis
{
    public class Scr_BossPhase1 : MonoBehaviour
    {
        [Header("General")]
        private bool _actionActive; //Pour savoir si une action est en cours.
        public float _delayBetweenActions;  //Délai avant une nouvelle action.
        private float _delay;
        private bool _canGoDelay; //Permet de savoir si on lance le délai entre deux actions.
        private int _randomAction;  //Détermine l'aléatoire des actions.
        public Transform _mySelf;  //Le Boss.
        private GameObject _player; //Le PJ.
        public float _fightDistance;  //Distance à laquelle le Boss arrête d'avancer.
        public float _moveSpeed;  //Vitesse de déplacement du Boss.
        public Vector3 _bossDirection;  //Direction du Boss vers le PJ.
        public float _retreatDistance;  //Distance à laquelle le Boss recule.
        private SoundManager sound; //Le son
        private bool _canWalk;
        public GameObject _spawnFX;
        private bool _spriteFliped;
        private Rigidbody2D _myBody;

        public AnimatorController_BossP1 b = null;

        [Header("Renforts")]
        public List<GameObject> _renforts;  //Liste des ennemis à faire spawn.
        public List<Transform> _renfortsSpawns;  //Liste des emplacements de spawn des ennemis.
        public GameObject ennemisContainer = null;

        [Header("Tir de Couverture et Fou de la Gachette")]
        public GameObject _bullet; //Le projectile.
        public float _couvertureSpeed;  //La vitesse de déplacement du Boss lors de la couverture.
        private float _couvertureDuration; //La durée du déplacement du Boss lors de la couverture.
        public float _couvertureDurationOrigin;
        public Transform _bulletContainer;  //Parent des projectiles du Boss.
        public float _shootingAllonge;  // Distance à laquelle apparaissent les projectile (pour pas qu'ils sortent du bide du boss).
        public float _delayBetweenShotsOrigin;  
        private float _delayBetweenShots;  // Délai entre les balles de Fou de la Gachette.
        public List<Vector3> _bulletFury;  // Liste des trajectoires des projectiles de Fou de la Gachette.
        [HideInInspector] public Vector3 _currentTarget = Vector2.zero;  //Direction actuellement ciblée par le Boss lors de Fou de la Gachette.
        private bool _couverture;

        [Header("Attaque au CaC")]
        public float _attackCast;  //Délai avant l'attaque.
        public GameObject _attackZone; //Zone touchée par l'attaque.
        public GameObject _attackPos;  //Point d'apparition de l'attaque.

        [Header("Grenade")]
        public GameObject _grenade;  //Projectile de Grenade.
        public Vector3 _grenadeTarget; //Point ciblé par la Grenade.


        // Start is called before the first frame update
        void Start()
        {
            _myBody = GetComponent<Rigidbody2D>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _actionActive = false;
            _delay = _delayBetweenActions;
            _canGoDelay = false;
            _couvertureDuration = _couvertureDurationOrigin;
            _delayBetweenShots = _delayBetweenShotsOrigin;
            _couverture = false;
            _canWalk = true;
            _spriteFliped = false;
        }
        void Awake()
        {
            sound = SoundManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            _bossDirection = (_player.transform.position - _mySelf.position);

            //Déplacement du Boss entre les actions.
            if (_canGoDelay == true)
            {
                if (_delay > 0)
                {
                    _delay -= Time.deltaTime;

                    if (Vector2.Distance(_mySelf.position, _player.transform.position) > _fightDistance && _canWalk == true)
                    {
                        b._canFlip = true;
                        _mySelf.position = Vector2.MoveTowards(_mySelf.position, _player.transform.position, _moveSpeed * Time.deltaTime);
                        b.animator.SetBool("IsWalking", true);
                        b._canShowBackVertical = false;
                        //sound.PlaySound("BOSS P1_Pas");
                        if (_spriteFliped == true)
                        {
                            _spriteFliped = false;
                            b.SpriteFlip(true);
                        }
                    }
                    else if (Vector2.Distance(_mySelf.position, _player.transform.position) < _retreatDistance && Vector2.Distance(_mySelf.position, _player.transform.position) < _fightDistance && _canWalk == true)
                    {
                        b._canFlip = false;
                        if (_spriteFliped == false)
                        {
                            b.SpriteFlip(true);
                            _spriteFliped = true;
                        }
                        _mySelf.position = Vector2.MoveTowards(_mySelf.position, _player.transform.position, -_moveSpeed * Time.deltaTime);
                        b.animator.SetBool("IsWalking", true);
                        b._canShowBackVertical = true;
                        //sound.PlaySound("BOSS P1_Pas");
                    }
                    else if (Vector2.Distance(_mySelf.position, _player.transform.position) < _fightDistance && Vector2.Distance(_mySelf.position, _player.transform.position) > _retreatDistance)
                    {
                        b._canFlip = true;
                        _mySelf.position = _mySelf.position;
                        b.animator.SetBool("IsWalking", false);
                        b._canShowBackVertical = false;
                        if (_spriteFliped == true)
                        {
                            _spriteFliped = false;
                            b.SpriteFlip(true);
                        }
                    }
                }
                else if (_delay <= 0)
                {
                    if (_spriteFliped == true)
                    {
                        b.SpriteFlip(true);
                    }
                    _delay = _delayBetweenActions;
                    _canGoDelay = false;
                    _actionActive = false;
                    b._canFlip = true;
                    _spriteFliped = false;
                }
            }

            //Choix et application des actions.
            if (_actionActive == false)
            {
                _randomAction = Random.Range(1, 10);

                if (_randomAction == 1)
                {
                    StartCoroutine(Renforts());
                    _actionActive = true;
                    Debug.Log("Renforts");
                }
                else if (_randomAction == 2 || _randomAction == 3)
                {
                    StartCoroutine(TirDeCouverture());
                    _actionActive = true;
                    Debug.Log("TirDeCouverture");
                }
                else if (_randomAction == 4 || _randomAction == 5)
                {
                    StartCoroutine(Grenade());
                    _actionActive = true;
                    Debug.Log("Grenade");
                }
                else if (_randomAction == 6 || _randomAction == 7)
                {
                    StartCoroutine(AttaqueCaC());
                    _actionActive = true;
                    Debug.Log("AttaqueCaC");
                }
                else if (_randomAction == 8 || _randomAction == 9)
                {
                    StartCoroutine(FouDeLaGachette());
                    _actionActive = true;
                    Debug.Log("FouDeLaGachette");
                }
            }

                //Déplacement de "Tir de couverture".
            if (_couverture == true && _couvertureDuration > 0)
            {
                _couvertureDuration -= Time.deltaTime;

                b._canFlip = false;
                _mySelf.position = Vector2.MoveTowards(_mySelf.position, _player.transform.position, -_couvertureSpeed * Time.deltaTime);
                if (_spriteFliped == false)
                {
                    b.SpriteFlip(true);
                    _spriteFliped = true;
                }
                b.animator.SetBool("IsWalking", true);
                b._canShowBackVertical = true;
            }
            else if (_couverture == true && _couvertureDuration <= 0)
            {
                _couvertureDuration = _couvertureDurationOrigin;
                _couverture = false;
                _canGoDelay = true;
                _canWalk = true;
                b._canShowBackVertical = false;
                _spriteFliped = false;
                b._canFlip = true;
            }
        }

        //Effet de "Renforts".
        private IEnumerator Renforts()
        {
            _canWalk = false;
            b._canShowBackVertical = false;
            b._canFlip = false;
            b.animator.SetBool("IsWalking", false);
            b.animator.SetTrigger("isCallingHelp");
            sound.PlaySound("Renforts");
            yield return new WaitForSeconds(0.5f);
            sound.PlaySound("Spawn_Ennemis");
            for (int i = 0; i < _renforts.Count; i++)
            {
                Instantiate(_spawnFX, _renfortsSpawns[i].position, Quaternion.identity, _renfortsSpawns[i]);
            }

            yield return new WaitForSeconds(0.25f);

            for (int ii = 0; ii < _renforts.Count; ii++)
            {
                Instantiate(_renforts[ii], _renfortsSpawns[ii].position, Quaternion.identity, _renfortsSpawns[ii]);
            }

            yield return new WaitForSeconds(0.5f);
            _canGoDelay = true;
            _canWalk = true;
            b._canFlip = true;
        }

        //Effet de "Tir de couverture".
        private IEnumerator TirDeCouverture()
        {
            _canWalk = false;
            b._canShowBackVertical = false;
            b._canFlip = false;
            b.animator.SetBool("IsWalking", false);
            b.animator.SetTrigger("isShooting");
            yield return new WaitForSeconds(0.5f);
            _currentTarget = (_player.transform.position - _mySelf.position);

            Instantiate(_bullet, _mySelf.position + _currentTarget.normalized * _shootingAllonge, _mySelf.rotation, _bulletContainer);
            sound.PlaySound("Tir Soldat");
            yield return new WaitForSeconds(0.25f);
            b._canFlip = true;
            _couverture = true; //Permet de lancer le déplacement de "Tir de Couverture" qui se trouve dans l'Update.
        }

        //Effet de "Grenade".
        private IEnumerator Grenade()
        {
            _canWalk = false;
            b._canShowBackVertical = false;
            b._canFlip = false;
            b.animator.SetBool("IsWalking", false);
            b.animator.SetTrigger("isGrenading");
            _grenadeTarget = _player.transform.position;
            yield return new WaitForSeconds(0.25f);

            Instantiate(_grenade, _mySelf.position + _grenadeTarget.normalized * _shootingAllonge, _mySelf.rotation, _bulletContainer);
            sound.PlaySound("Création Bombe");
            _canGoDelay = true;
            _canWalk = true;
            b._canFlip = true;
        }

        //Effet de "AttaqueCaC".
        private IEnumerator AttaqueCaC()
        {
            _canWalk = false;
            b._canShowBackVertical = false;
            b._canFlip = false;
            b.animator.SetBool("IsWalking", false);
            b.animator.SetTrigger("isAttacking");
            b._canTurn = false;
            yield return new WaitForSeconds(_attackCast);

            Instantiate(_attackZone, _attackPos.transform.position + _bossDirection.normalized * 2, _attackPos.transform.rotation, _attackPos.transform);
            _attackPos.GetComponent<Scr_BossP1AttackPos>()._attackSet = true;
            sound.PlaySound("BOSS_Attaque CaC");
            b.animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(0.5f);

            _canGoDelay = true;
            b._canFlip = true;
            b._canTurn = true;
            _canWalk = true;
        }

        //Effet de "Fou de la Gachette".
        private IEnumerator FouDeLaGachette()
        {
            _canWalk = false;
            b._canShowBackVertical = false;
            b._canFlip = false;
            b._spritRend.flipX = false;
            b.animator.SetBool("IsWalking", false);
            b.animator.SetTrigger("IsGunSlinger");

            for (int i = 0; i < _bulletFury.Count; i++)
            {
                _currentTarget = (_bulletFury[i] - _mySelf.position);
                Instantiate(_bullet, _mySelf.position + _currentTarget.normalized * _shootingAllonge, _mySelf.rotation, _bulletContainer);
                sound.PlaySound("Tir Demultiplie");
                yield return new WaitForSeconds(_delayBetweenShots);
            }

            yield return new WaitForSeconds(0.5f);
            b._canFlip = true;
            _canGoDelay = true;
            _canWalk = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Environment"))
            {
                _mySelf.position = _mySelf.position;
                _couvertureDuration = 0;
            }
        }


    }
}

