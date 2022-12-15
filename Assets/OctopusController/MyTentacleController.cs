using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;




namespace OctopusController
{

    
    internal class MyTentacleController

    //MAINTAIN THIS CLASS AS INTERNAL
    {

        TentacleMode tentacleMode;
        Transform[] _bones;
        Transform _endEffectorSphere;

        public Transform[] Bones { get => _bones; }

        //Exercise 1.
        public Transform[] LoadTentacleJoints(Transform root, TentacleMode mode)
        {
            //TODO: add here whatever is needed to find the bones forming the tentacle for all modes
            //you may want to use a list, and then convert it to an array and save it into _bones
            tentacleMode = mode;

            switch (tentacleMode){
                case TentacleMode.LEG:
                    //TODO: in _endEffectorsphere you keep a reference to the base of the leg
                    
                    Transform legBone = root.GetChild(0);
                    
                    List<Transform> legBones = new List<Transform>();
                    while (legBone.childCount > 0)
                    {
                        legBones.Add(legBone);
                       
                        legBone = legBone.GetChild(1);

                    }
                    legBones.Add(legBone);
                    
                    
                    _bones = legBones.ToArray();
                    _endEffectorSphere = legBones[legBones.Count-1];
                    Debug.Log(legBones.Count);
                    break;
                case TentacleMode.TAIL:
                    Transform tailBone = root;

                    List<Transform> tailBones = new List<Transform>();
                    while (tailBone.childCount > 0)
                    {
                        tailBones.Add(tailBone);
                       
                        tailBone = tailBone.GetChild(1);

                    }
                    tailBones.Add(tailBone);
                    
                    
                    _bones = tailBones.ToArray();
                    _endEffectorSphere = tailBones[tailBones.Count-1];
              

                    break;
                case TentacleMode.TENTACLE:
                    Transform bone = root.GetChild(0).GetChild(0);
                   
                    List<Transform> bones = new List<Transform>();
                    while (bone.childCount > 0)
                    {
                        bones.Add(bone);
                        bone = bone.GetChild(0);

                    }

                    _bones = bones.ToArray();
                    break;
            }
            return Bones;
        }
    }
}
