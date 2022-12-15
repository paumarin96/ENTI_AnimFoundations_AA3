using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;


namespace OctopusController
{
      public delegate float ErrorFunction(Vector3 target, float[] solution);

      public struct PositionRotation
      {
          Vector3 position;
          Quaternion rotation;

          public PositionRotation(Vector3 position, Quaternion rotation)
          {
              this.position = position;
              this.rotation = rotation;
          }

          // PositionRotation to Vector3
          public static implicit operator Vector3(PositionRotation pr)
          {
              return pr.position;
          }
          // PositionRotation to Quaternion
          public static implicit operator Quaternion(PositionRotation pr)
          {
              return pr.rotation;
          }
      }
    public class MyScorpionController
    {
        
        
        //TAIL
        Transform tailTarget;
        Transform tailEndEffector;
        MyTentacleController _tail;
        float animationRange;
        private ErrorFunction ErrorFunction;

        private Vector3[] Axis = null;
        private Vector3[] StartOffset = null;
        private Quaternion firstBoneInitialRot;
        public float[] Solution = null;
        private float StopThreshold = 0.1f;
        private float LearningRate = 5f;
        private float DeltaGradient = 0.1f;

        //LEGS
        Transform[] legTargets;
        Transform[] legFutureBases;
        MyTentacleController[] _legs = new MyTentacleController[6];
        private Vector3[] _oldBasePositions;
        private List<bool> _legTooLongFlag = new List<bool>();
        private List<float> _legTimers = new List<float>();
        private List<int> _legOffsetCounter = new List<int>();

        private float _legAnimDuration = 0.7f;
        
        
        private float distanceThreshold = 0.4f;
        
        private List<Vector3[]> copy = new List<Vector3[]>();
        private List<float[]> distances = new List<float[]>();
        private List<bool> done = new List<bool>();
        
        #region public
        public void InitLegs(Transform[] LegRoots,Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];
            legTargets = LegTargets;
            legFutureBases = LegFutureBases;
             _oldBasePositions = new Vector3[_legs.Length];
            //Legs init
            for(int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);

                distances.Add(new float[_legs[i].Bones.Length - 1]);
                copy.Add(new Vector3[_legs[i].Bones.Length]);
                done.Add(false);
                _legTooLongFlag.Add(false); 
                _legTimers.Add(0);
                _legOffsetCounter.Add(0);

            }
            
            for (int i = 0; i < distances.Count; i++)
            {
                for (int j = 0; j < distances[i].Length; j++)
                {
                    distances[i][j] = (_legs[i].Bones[j + 1].transform.position - _legs[i].Bones[j].transform.position).magnitude;

                }
                
            }

        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);
            ErrorFunction = DistanceFromTarget;
            Solution = new float[_tail.Bones.Length];
            Axis = new Vector3[_tail.Bones.Length];
            StartOffset = new Vector3[_tail.Bones.Length];
            tailEndEffector = _tail.Bones[_tail.Bones.Length - 1];

            firstBoneInitialRot = _tail.Bones[0].localRotation;
            
            for (int i = 0; i < _tail.Bones.Length - 1; i++)
            {

                if(i == 0)
                {
                    Axis[i] = Vector3.forward;
                    Solution[i] = _tail.Bones[i].transform.localEulerAngles.z;
                    StartOffset[i] = _tail.Bones[i + 1].position - _tail.Bones[i].position;
                    StartOffset[i] = Quaternion.Inverse(_tail.Bones[i].rotation) * StartOffset[i]; 

                }

                else
                {
                    Axis[i] = Vector3.right;
                    Solution[i] = _tail.Bones[i].transform.localEulerAngles.x;
                    
                    StartOffset[i] = _tail.Bones[i + 1].position - _tail.Bones[i].position;
                
                    StartOffset[i] = Quaternion.Inverse(_tail.Bones[i].rotation) * StartOffset[i]; 
                }
                

               
            }
             
        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            tailTarget = target;
        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {

        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {
            updateTail();
            updateLegs();
            updateLegPos();
        }
        #endregion


        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos()
        {
            //check for the distance to the futureBase, then if it's too far away start moving the leg towards the future base position
            //
            for (int i = 0; i < _legs.Length; i++)
            {
               
                if (Vector3.Distance(_legs[i].Bones[0].transform.position,
                        legFutureBases[i].transform.position) > distanceThreshold && !_legTooLongFlag[i])
                {

                    _legTimers[i] = 0;
                    _legOffsetCounter[i]++;
           
                        _oldBasePositions[i] =  _legs[i].Bones[0].transform.position;
                        _legTooLongFlag[i] = true;
                    


                }

                if (_legTooLongFlag[i])
                {
                    _legTimers[i] += Time.deltaTime;

                    float offset = 0;
                    if (i == 0 || i == 3 || i == 4)
                    {
                        offset = _legOffsetCounter[i] % 2 == 0 ? 0.25f : -0.25f;
                    }
                    else
                    {
                        offset = _legOffsetCounter[i] % 2 == 0 ? -0.25f : 0.25f;
                    }
                    
                    
                   
                    _legs[i].Bones[0].transform.position = Vector3.Lerp(_oldBasePositions[i],
                        legFutureBases[i].transform.position + new Vector3(0,0, offset), _legTimers[i] / _legAnimDuration);
                    
                    if (_legTimers[i] >= _legAnimDuration)
                    {
                        _legTooLongFlag[i] = false;
                    }
                }
            

            }
            
        }

        private void updateTail()
        {
            ErrorFunction = DistanceFromTarget;
            if (ErrorFunction(tailTarget.position, Solution) > StopThreshold)
            {
                ApproachTarget(tailTarget.position);
            }

            Debug.DrawLine(tailEndEffector.position, tailTarget.position, Color.green);

        }

        //TODO: implement fabrik method to move legs 
        private void updateLegs()
        {
            for (int i = 0; i < copy.Count; i++)
            {
                for (int j = 0; j < copy[i].Length; j++)
                {
                    copy[i][j] = _legs[i].Bones[j].transform.position;
                    
                    if (Vector3.Distance(copy[i][copy[i].Length - 1], legTargets[i].position) < 0.1f)
                    {
                        done[i] = true;
                    }
                    else
                    {
                        done[i] = false;
                    }
                }
                
 

                if (!done[i])
                {
                    float targetRootDist = Vector3.Distance(copy[i][0], legTargets[i].position);
                    float targetEndEffectorDist = Vector3.Distance(copy[i][copy[i].Length - 1], legTargets[i].position);
                    
                    // Update joint positions
                    if (targetRootDist > distances[i].Sum()){
                
                
                        done[i] = true;
                
                        copy[i][0] = _legs[i].Bones[0].transform.position;
                        Vector3 direction = legTargets[i].position - copy[i][0];
                        direction.Normalize();
                        for (int j = 0; j < copy[i].Length - 1; ++j)
                        {
                            copy[i][j + 1] = copy[i][j] + direction * distances[i][j];
                        }
                
                    }  else
                    {
                        // The target is reachable
                        while (targetEndEffectorDist > 0.1f)
                        {
                            // STAGE 1: FORWARD REACHING
                            copy[i][copy[i].Length-1] = legTargets[i].position;
                            for (int j = copy[i].Length - 2; j > 0; --j)
                            {
                                copy[i][j] = copy[i][j + 1] + (copy[i][j] - copy[i][j + 1]).normalized * distances[i][j];
                            }
                
                            copy[i][0] = _legs[i].Bones[0].transform.position;
                            // STAGE 2: BACKWARD REACHING
                            for (int j = 1; j <= copy[i].Length - 1; j++)
                            {
                                copy[i][j] = copy[i][j - 1] + (copy[i][j] - copy[i][j - 1]).normalized * distances[i][j - 1];
                
                            }
                    
                            targetRootDist = Vector3.Distance(copy[i][0], legTargets[i].position);
                            targetEndEffectorDist = Vector3.Distance(copy[i][copy[i].Length - 1], legTargets[i].position);
                            
                        }
                
                
                    }
                    // Update original joint rotations
                    for (int j = 0; j <= _legs[i].Bones.Length - 2; j++)
                    {
                
                        Vector3 copyDirection = copy[i][j + 1] - copy[i][j];
                        Vector3 jointDirection = _legs[i].Bones[j + 1].position - _legs[i].Bones[j].position;
                
                
                        Vector3 axis = Vector3.Cross(jointDirection.normalized, copyDirection.normalized).normalized;
                        float angle = Mathf.Acos(Vector3.Dot(jointDirection.normalized, copyDirection.normalized)) * Mathf.Rad2Deg;
                
                        if(angle > 0.1f)
                            _legs[i].Bones[j].rotation = Quaternion.AngleAxis(angle, axis) *  _legs[i].Bones[j].rotation;
                        
                        
                    }
                }
            }
            
        }
        
        
        public float DistanceFromTarget(Vector3 target, float[] Solution)
        {
            Vector3 point = ForwardKinematics(Solution);
            
            return Vector3.Distance(point, target);
        }
        
        public PositionRotation ForwardKinematics(float[] Solution)
        {
             Vector3 prevPoint = _tail.Bones[0].transform.position;

             Quaternion rotation = _tail.Bones[0].transform.parent.rotation;

             Vector3 nextPoint; 

             for (int i = 0; i < Solution.Length; i++)
             {
                 rotation = rotation * Quaternion.AngleAxis(Solution[i], Axis[i]);
                 nextPoint = prevPoint + rotation * StartOffset[i];
                 Debug.DrawLine(prevPoint, nextPoint, Color.magenta);
                 prevPoint = nextPoint; 

             }

         
            return new PositionRotation(prevPoint, rotation);
        }

        public float CalculateGradient(Vector3 target, float[] Solution, int i, float delta)
        {
            float gradient = 0;

            Solution[i] += delta;

            var distancePlusDelta = DistanceFromTarget(target, Solution);

            Solution[i] -= delta;

            gradient = (distancePlusDelta - DistanceFromTarget(target, Solution)) / delta;

            return gradient;
        }

        public void ApproachTarget(Vector3 target)
        {
            for (int i = 0; i < Solution.Length; i++)
            {
                Solution[i] = Solution[i] - (LearningRate * CalculateGradient(target, Solution, i, DeltaGradient));
            }

            for (int i = 0; i < _tail.Bones.Length; i++)
            {
                _tail.Bones[i].localRotation = Quaternion.AngleAxis(Solution[i], Axis[i]);

            }
        }


        #endregion
    }
}
