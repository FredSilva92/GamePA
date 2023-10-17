using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected bool _isDead = false;
    protected bool _isShooting;

    public bool IsDead
    {
        get { return _isDead; }
    }

    public bool IsShooting
    {
        get { return _isShooting; }
    }
}
