using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviourControllable : Controllable
{
    [OSCProperty]
    public float Speed;

    [OSCProperty]
    public bool DoomMode;

    public override void Awake()
    {
        usePanel = true;//Black magic, enabled by default on every controllable but miraculously not on this script
        base.Awake();
	}
}
