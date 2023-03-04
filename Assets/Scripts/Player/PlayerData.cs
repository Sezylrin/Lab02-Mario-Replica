using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    //PHYSICS
        [Header("Gravity")]
    public float gravityScale; //Overrides rb.gravityScale
    public float fallGravityMult;
    public float maxFallSpeed;

    [Header("Drag")]
    public float dragAmount; //Drag in air
    public float frictionAmount; //Drag on ground

        [Header("Other Physics")]
    [Range(0, 0.5f)] public float coyoteTime; //Grace time to Jump after the player has fallen off ground


    //GROUND
        [Header("Walk")]
    public float walkMaxSpeed;
    public float walkAccel;
    public float walkDecel;
    [Range(0, 1)] public float accelInAir;
    [Range(0, 1)] public float decelInAir;
    [Space(5)]
    [Range(0.5f, 2f)] public float accelPower;
    [Range(0.5f, 2f)] public float stopPower;
    [Range(0.5f, 2f)] public float turnPower;


    //JUMP
        [Header("Jump")]
    public float jumpForce;
    [Range(0, 1)] public float jumpCutMultiplier;
    [Space(10)]
    [Range(0, 0.5f)] public float jumpBufferTime; // Time after pressing jump where a jump will be performed when ready.


    //OTHER
    [Header("Other Settings")]
    public bool doKeepWalkMomentum; // Player Movement will not decrease if above maxSpeed, letting only drag do so. Allows for conservation of momentum
}