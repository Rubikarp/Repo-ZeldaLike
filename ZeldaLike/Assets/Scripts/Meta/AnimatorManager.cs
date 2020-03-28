﻿using UnityEngine;
using Game;

namespace Management
{
    public class AnimatorManager : MonoBehaviour
    {

        [Header("INPUT")]
        [SerializeField] SpriteRenderer _spritRend = null;
        [SerializeField] Animator _animator = null;

        [SerializeField] InputManager _input = null;
        [SerializeField] Scr_PlayerLifeSystem _playerLife = null;

        [SerializeField] Scr_AttributionTouches_Transfo _formeManager = null;
        //Humain
        [SerializeField] Actif_KnifeThrowing _humainAttack = null;
        public bool _isHumain = false;
        //lourd
        [SerializeField] Actif_AgileAttack _agileAttack = null;
        public bool _isAgile = false;

        //Agile
        [SerializeField] Actif_HeavyAttack _heavyAttack = null;
        public bool _isLourd = false;




        void Start()
        {
            _input = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputManager>();
        }
        
        void Update()
        {
            //Flip Horizontal
            if(_input._CharacterDirection.x < 0)
            {
                _spritRend.flipX = true;
            }
            else if (_input._CharacterDirection.x > 0)
            {
                _spritRend.flipX = false;
            }

            //Valeur des actions
            _animator.SetFloat("MouvY", _input._CharacterDirection.y);
            _animator.SetFloat("MouvSpeed", _input._stickDirection.magnitude);
            _animator.SetBool("IsTakingDamage", _playerLife._isTakingDamage);

            //Bool pour la forme
            if(_formeManager.actualForm == Scr_AttributionTouches_Transfo.Forme.Lourd && !_isLourd)
            {
                _spritRend.color = Color.red;

                _animator.SetBool("IsLourd", true);
                _animator.SetBool("IsAgile", false);
                _animator.SetBool("IsHuman", false);

                _isHumain = false;
                _isAgile = false;
                _isLourd = true;

                _animator.SetTrigger("GoLourd");
            }
            else
            if (_formeManager.actualForm == Scr_AttributionTouches_Transfo.Forme.Agile && !_isAgile)
            {
                _spritRend.color = Color.white;

                _animator.SetBool("IsLourd", false);
                _animator.SetBool("IsAgile", true);
                _animator.SetBool("IsHuman", false);

                _isAgile = true;
                _isHumain = false;
                _isLourd = false;

                _animator.SetTrigger("GoAgile");

            }
            else
            if(_formeManager.actualForm == Scr_AttributionTouches_Transfo.Forme.Humain && !_isHumain)
            {
                _spritRend.color = Color.white;

                _animator.SetBool("IsLourd", false);
                _animator.SetBool("IsAgile", false);
                _animator.SetBool("IsHuman", true);

                _isHumain = true;
                _isAgile = false;
                _isLourd = false;

                _animator.SetTrigger("GoHumain");

            }

        }

        //AttackTrigger
        public void TriggerAttack()
        {
            _animator.SetTrigger("Attack");
        }

        public void TriggerDeath()
        {
            _animator.SetTrigger("Die");
        }

    }
}