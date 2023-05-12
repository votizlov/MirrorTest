using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


    enum SpaceshipButtons
    {
        Fire = 0,
    }

    public struct PlayerInput : INetworkInput
    {
        public float HorizontalInput;
        public float VerticalInput;
        public NetworkButtons Buttons;
    }
