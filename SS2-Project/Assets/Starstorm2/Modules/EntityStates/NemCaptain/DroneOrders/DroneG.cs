﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStates.NemCaptain.Weapon
{
    public class DroneG : CallDroneBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("drone g!");
        }
    }
}
