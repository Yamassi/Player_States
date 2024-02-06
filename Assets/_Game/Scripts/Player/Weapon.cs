using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Hand _handL, _handR;

    private void Awake()
    {
        _handL.gameObject.SetActive(false);
        _handR.gameObject.SetActive(false);
    }

    public void ToggleHandL(bool value) => _handL.gameObject.SetActive(value);

    public void ToggleHandR(bool value) => _handR.gameObject.SetActive(value);
}