using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=GRSOrkmasMM

public class JumpDetector : MonoBehaviour
{
    public Transform head;
    public Transform leftArm;
    public Transform rightArm;
    
    public int crouchThreshold = 50;

    private static int NumberOfTrackedPositions = 100;
    private CyclicArray<Vector3> headPositions;
    private CyclicArray<Transform> leftArmPositions;
    private CyclicArray<Transform> rightArmPositions;

    private bool isJumping = false;
    private bool isCrouching = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        headPositions = new CyclicArray<Vector3>(NumberOfTrackedPositions);
        leftArmPositions = new CyclicArray<Transform>(NumberOfTrackedPositions);
        rightArmPositions = new CyclicArray<Transform>(NumberOfTrackedPositions);
    }


    // Update is called once per frame
    void Update()
    {
    }


    void FixedUpdate()
    {
        if (!isJumping) {
            TrackPositions();
            if (isCrouching) {
                if (WantsToJump()) {
                    Jump();
                } else {
                    isCrouching = head.position.y < crouchThreshold;
                }
            }
        }
    }


    void TrackPositions() {
        headPositions.Add(head.position);
        leftArmPositions.Add(leftArm);
        rightArmPositions.Add(rightArm);
    }

    bool WantsToJump() {
        return head.position.y >= crouchThreshold;
    }

    void Jump() {
        // GO BANANA
        isJumping = true;
    }
}
