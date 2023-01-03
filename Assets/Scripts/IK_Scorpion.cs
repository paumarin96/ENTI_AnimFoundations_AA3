﻿using System;
using System.Collections;
using System.Collections.Generic;
using OctopusController;
using UnityEngine;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController= new MyScorpionController();

    public IK_tentacles _myOctopus;

    public TailBodyTargetAnimation tailBody;

    [Header("Body")]
    float animTime;
    public float animDuration = 5;
    bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    public ShootForce shootForce;
    readonly float bodyHeightSpeed = 20f;
    private float bodyYOffset = 0f;

    public delegate void StartWalk();

    public bool isPlayerControlling;

    public event StartWalk OnStartWalk;
    
    // Start is called before the first frame update
    void Start()
    {
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);

        float y = 0;
        for (int i = 0; i < legs.Length; i++)
        {
            y += legs[i].GetChild(0).position.y;
        }

        y /= legs.Length;

        bodyYOffset = Body.position.y - y; 
    }

    // Update is called once per frame
    void Update()
    {
        if(animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shootForce.StartForceSlider();
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            shootForce.StopForceSlider();
            
            NotifyStartWalk();
            animTime = 0;
            animPlaying = true;
        }
        
        if (animTime < animDuration && !isPlayerControlling)
        {
            Body.position = Vector3.Lerp(StartPos.position, EndPos.position, animTime / animDuration);
        }
        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position;
            animPlaying = false;
            NotifyFinishedWalk();
        }

        _myController.UpdateIK();
        UpdateBodyPosition();
        UpdateBodyRotation();
    }

    
    //Updates the body position depending on the height of the legs
    private void UpdateBodyPosition()
    {
        
        float y = 0;
        for (int i = 0; i < futureLegBases.Length; i++)
        {
            RaycastHit groundHit;
            Physics.Raycast(futureLegBases[i].transform.position, Vector3.down, out groundHit, 10);
            y += groundHit.point.y;
        }

        y /= legs.Length;

        //Body.position = new Vector3(Body.position.x, bodyYOffset + y, Body.position.z); 
       Body.position  = Vector3.Lerp(Body.position, new Vector3(Body.position.x, bodyYOffset + y, Body.position.z),
            Time.deltaTime * 30);
    }

    private void UpdateBodyRotation()
    {
       
        Vector3 legAvgNormal1 = Vector3.Cross( legs[0].GetChild(0).transform.position - legs[3].GetChild(0).transform.position,
            legs[4].GetChild(0).transform.position - legs[3].GetChild(0).transform.position).normalized;

        Vector3 legAvgNormal2 = Vector3.Cross(legs[5].GetChild(0).transform.position - legs[2].GetChild(0).transform.position,
            legs[1].GetChild(0).transform.position - legs[2].GetChild(0).transform.position).normalized;

        Vector3 avgNormal = (legAvgNormal1 + legAvgNormal2) / 2;
        avgNormal.Normalize();
        
        Body.up = Vector3.Lerp(Body.up,avgNormal, Time.deltaTime * 10);

    }
    
    
    private void NotifyFinishedWalk()
    {
        tailBody.StartAnimation();
    }

    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        _myController.NotifyTailTarget(tailTarget);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {
        if(OnStartWalk != null)
            OnStartWalk();
        _myController.NotifyStartWalk();
    }

    private void OnDrawGizmos()
    {
        _myController.DrawGizmos();
    }
}
