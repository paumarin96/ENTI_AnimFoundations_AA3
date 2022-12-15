using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


namespace OctopusController
{
    public enum TentacleMode { LEG, TAIL, TENTACLE };

    public class MyOctopusController 
    {
        
        MyTentacleController[] _tentacles = new  MyTentacleController[4];

        Transform _currentRegion;
        Transform _target;
        
        private Vector3[] tpos;

        Transform[] _randomTargets;// = new Transform[4];
        float[] _theta, _sin, _cos;

        float _twistMin = -20, _twistMax = 20;
        float _swingMin, _swingMax;

        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin {  set => _swingMin = value; }
        public float SwingMax { set => _swingMax = value; }

        private bool _done;

        private float timer = 3;
        
        // Max number of tries before the system gives up (Maybe 10 is too high?)
        [SerializeField]
        private int _mtries = 10;
        // The number of tries the system is at now
        [SerializeField]
        private int[] _tries = {0};
        
        // the range within which the target will be assumed to be reached
        readonly float _epsilon = 0.1f;
        private int nearestTentacle = 0;
        private bool goToBall = false; 

        public void TestLogging(string objectName)
        {

           
            Debug.Log("hello, I am initializing my Octopus Controller in object "+objectName);

            
        }
        public static Quaternion GetSwing(Quaternion rot)
        {
            //QSwing = qTwist^-1 * qr
            return (rot * Quaternion.Inverse(new Quaternion(0, rot.y, 0, rot.w).normalized));
            
        }
        public static Quaternion GetTwist(Quaternion rot)
        {
            //QTwist = (0,qy,0,qw) * (1/sqrt(qw^2 + qy^2)))
            return GetSwing(rot) * (new Quaternion(0,rot.y, 0, rot.w).normalized);

        }
        public void Init(Transform[] tentacleRoots, Transform[] randomTargets)
        {
            _tentacles = new MyTentacleController[tentacleRoots.Length];

            // foreach (Transform t in tentacleRoots)
            for(int i = 0;  i  < tentacleRoots.Length; i++)
            {

                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i],TentacleMode.TENTACLE);
             

            }
            _theta = new float[_tentacles[0].Bones.Length];
            _sin = new float[_tentacles[0].Bones.Length];
            _cos = new float[_tentacles[0].Bones.Length];
            tpos = new Vector3[_tentacles.Length];
            
            _tries = new int[_tentacles.Length]; 

            _randomTargets = randomTargets;

        }

              
        public void NotifyTarget(Transform target, Transform region)
        {
            _currentRegion = region;
            _target = target;
        }

        public void NotifyShoot() {
            Debug.Log("Shoot");
            float minDist = 1000;
            for (int i = 0; i < _tentacles.Length; i++)
            {
                float dist = (_tentacles[i].Bones[_tentacles[i].Bones.Length - 1].position - _target.position).magnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestTentacle = i;
                }
            }

            goToBall = true;


        }


        public void UpdateTentacles()
        {
            if (goToBall)
                timer -= Time.deltaTime;

            if (timer <= 0.0f && goToBall)
            {
                goToBall = false;
                timer = 3;
            }
 
                
            for (int i = 0; i < _tentacles.Length; i++)
            {
                if (goToBall && i == nearestTentacle)
                {
                    
                    update_ccd(i, _target.transform.position);
                }
                else
                {
                    update_ccd(i, _randomTargets[i].transform.position);  
                }
                
            }
            
            
            
        }




        #endregion


        #region private and internal methods

        void update_ccd(int tentacleIndex, Vector3 target) {

            Transform[] joints = _tentacles[tentacleIndex].Bones;

            if (!_done)
            {

                if(_tries[tentacleIndex] <= _mtries)
                {
                    
                    for (int i = _tentacles[tentacleIndex].Bones.Length - 2; i >= 0; i--)
                    {
                        Vector3 r1 = joints[joints.Length - 1].transform.position - joints[i].transform.position;
                        r1.Normalize();

                        Vector3 r2 = target - joints[i].transform.position;
                        r2.Normalize();

                        if (r1.magnitude * r2.magnitude <= 0.001f)
                        {
                            _cos[i] = 1;
                            _sin[i] = 0;

                        }
                        else
                        {
                            // find the components using dot and cross product
                            _cos[i] = Vector3.Dot(r1, r2);
                            _sin[i] = Vector3.Cross(r1, r2).magnitude;
                        }


                        // The axis of rotation 
                        Vector3 axis = Vector3.Cross(r1, r2).normalized;

                        // find the angle between r1 and r2 (and clamp values if needed avoid errors)
                        var clampTheta = Mathf.Clamp(_cos[i], -1, 1);
                        _theta[i] = Mathf.Acos(clampTheta) * Mathf.Rad2Deg;

                        // correct angles, depending on angles invert angle if sin component is negative
                        if (_sin[i] < 0)
                            _theta[i] = -_theta[i];
                        
                       var finalRotQuat =  Quaternion.AngleAxis(_theta[i], axis) * joints[i].transform.rotation;
                       var finalRotQuatTwist = new Quaternion(0, finalRotQuat.y, 0, finalRotQuat.w).normalized;
                       var finalRotQuatSwing = Quaternion.Inverse(finalRotQuatTwist) * finalRotQuat;

                       var twistAngle = 0.0f;
                       Vector3 twistAxis;
                           finalRotQuatSwing.ToAngleAxis(out twistAngle, out twistAxis);
                           twistAngle = Mathf.Clamp(twistAngle, _twistMin, _twistMax);

                           finalRotQuatSwing = Quaternion.AngleAxis(twistAngle, twistAxis);
                           



                       joints[i].transform.rotation = finalRotQuatSwing;

                    }

                    _tries[tentacleIndex]++;
                }
                
                

            }
            float diff = (target - joints[joints.Length - 1].transform.position).magnitude;
                
            // if target is within reach (within epsilon) then the process is done
            if (diff < 0.01)
            {
                _done = true;
            }
            // if it isn't, then the process should be repeated
            else
            {
                _done = false;
            }
		
            // the target has moved, reset tries to 0 and change tpos
            if(target != tpos[tentacleIndex])
            {
                _tries[tentacleIndex] = 0;
                tpos[tentacleIndex] = target;
            }

        }


        

        #endregion






    }
}
