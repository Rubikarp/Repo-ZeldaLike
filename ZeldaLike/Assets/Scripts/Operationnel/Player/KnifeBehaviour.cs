﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Management;

public class KnifeBehaviour : MonoBehaviour
{
    public float _knifeSpeed;
    public float _knifeDamage;
    private Rigidbody2D _knifeBody;
    private bool _ephemerate;
    public float _knifeLifetime;
    GameObject _player;
    Vector2 _playerOrientation;

    // Start is called before the first frame update
    void Start()
    {
        _knifeBody = GetComponent<Rigidbody2D>();
        _ephemerate = true;
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerOrientation = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputManager>()._CharacterDirection;
    }

    // Update is called once per frame
    void Update()
    {
        _knifeBody.velocity = _playerOrientation * _knifeSpeed;

        if (_ephemerate == true)
        {
            _ephemerate = false;
            StartCoroutine(DestroyKnife());
        }
    }

    IEnumerator DestroyKnife()
    {
        yield return new WaitForSeconds(_knifeLifetime);
        Destroy(gameObject);
    }
}
