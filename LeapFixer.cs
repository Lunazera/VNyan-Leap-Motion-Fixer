using System;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;
using static UnityEngine.Random;
using static UnityEngine.TouchScreenKeyboard;

namespace Leap_Motion_Fixer
{
    public class LeapFixerSettings
    {
        // Layer On/Off Setting
        public static bool layerActive = true; // flag for layer being active or not (when inactive, vnyan will stop reading from the rotations entirely)
        public static void setLayerOnOff(float val) => layerActive = (val == 1f) ? true : false;

        public static float slerpAmount = 10f;

        public static void setSlerpAmount(float val) => slerpAmount = val;

        // Arm States:
        // 0 = off,
        // 1 = stable,
        // 2 = unstable

        // Arm Status:
        // 0 = LeapOff
        // 1 = LeapOn

        // Mirror
        // 0 = unmirrored, 1 = mirrored

        public static Dictionary<string, float> bonesSettings = new Dictionary<string, float>
        {
            { "leftStatus", 0 },
            { "leftState", 0 },
            { "rightStatus", 0 },
            { "rightState", 0 },
            { "mirror", 0 }
        };
        // Mirror set
        public static void setMirror(float state) => bonesSettings["mirror"] = state;
        // Left Sets
        public static void setLeftState(float state) => bonesSettings["leftState"] = state;
        public static void setLeftStatus(float state) => bonesSettings["leftStatus"] = state;
        // Right Sets
        public static void setRightState(float state) => bonesSettings["rightState"] = state;
        public static void setRightStatus(float state) => bonesSettings["rightStatus"] = state;
        
        // Gets
        // These take into account the mirror setting, so it doesn't need to be handled in VNyan
        public static float getMirror() => bonesSettings["mirror"];
        public static float getLeftState() => bonesSettings["leftState"];
        public static float getLeftStatus() => bonesSettings["leftStatus"];
        public static float getRightState() => bonesSettings["rightState"];
        public static float getRightStatus() => bonesSettings["rightStatus"];


        //VNyan Getters
        public static void getVNyanLeapMirror()
        {
            setMirror(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapMirror"));
        }
        public static void getVNyanLeapState()
        {
            setLeftState(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixerStateL"));
            setRightState(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixerStateR"));
        }
        public static void getVNyanLeapStatus()
        {
            setLeftStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapStatusL"));
            setRightStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapStatusR"));
        }
        

        // Bone dictionaries
        public static List<int> LeftArm = new List<int> { 11, 13, 15, 17, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 };
        public static List<int> RightArm = new List<int> { 12, 14, 16, 18, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53 };

        // Current Rotations
        public static Dictionary<int, VNyanQuaternion> LeftArmCurrent = new Dictionary<int, VNyanQuaternion> {
            { 11, new VNyanQuaternion{} },
            { 13, new VNyanQuaternion{} },
            { 15, new VNyanQuaternion{} },
            { 17, new VNyanQuaternion{} },
            { 24, new VNyanQuaternion{} },
            { 25, new VNyanQuaternion{} },
            { 26, new VNyanQuaternion{} },
            { 27, new VNyanQuaternion{} },
            { 28, new VNyanQuaternion{} },
            { 29, new VNyanQuaternion{} },
            { 30, new VNyanQuaternion{} },
            { 31, new VNyanQuaternion{} },
            { 32, new VNyanQuaternion{} },
            { 33, new VNyanQuaternion{} },
            { 34, new VNyanQuaternion{} },
            { 35, new VNyanQuaternion{} },
            { 36, new VNyanQuaternion{} },
            { 37, new VNyanQuaternion{} },
            { 38, new VNyanQuaternion{} },
        };
        public static Dictionary<int, VNyanQuaternion> RightArmCurrent = new Dictionary<int, VNyanQuaternion> {
            { 12, new VNyanQuaternion{} },
            { 14, new VNyanQuaternion{} },
            { 16, new VNyanQuaternion{} },
            { 18, new VNyanQuaternion{} },
            { 39, new VNyanQuaternion{} },
            { 40, new VNyanQuaternion{} },
            { 41, new VNyanQuaternion{} },
            { 42, new VNyanQuaternion{} },
            { 43, new VNyanQuaternion{} },
            { 44, new VNyanQuaternion{} },
            { 45, new VNyanQuaternion{} },
            { 46, new VNyanQuaternion{} },
            { 47, new VNyanQuaternion{} },
            { 48, new VNyanQuaternion{} },
            { 49, new VNyanQuaternion{} },
            { 50, new VNyanQuaternion{} },
            { 51, new VNyanQuaternion{} },
            { 52, new VNyanQuaternion{} },
            { 53, new VNyanQuaternion{} }
        };

        // Target Rotations
        public static Dictionary<int, VNyanQuaternion> LeftArmTarget = new Dictionary<int, VNyanQuaternion> {
            { 11, new VNyanQuaternion{} },
            { 13, new VNyanQuaternion{} },
            { 15, new VNyanQuaternion{} },
            { 17, new VNyanQuaternion{} },
            { 24, new VNyanQuaternion{} },
            { 25, new VNyanQuaternion{} },
            { 26, new VNyanQuaternion{} },
            { 27, new VNyanQuaternion{} },
            { 28, new VNyanQuaternion{} },
            { 29, new VNyanQuaternion{} },
            { 30, new VNyanQuaternion{} },
            { 31, new VNyanQuaternion{} },
            { 32, new VNyanQuaternion{} },
            { 33, new VNyanQuaternion{} },
            { 34, new VNyanQuaternion{} },
            { 35, new VNyanQuaternion{} },
            { 36, new VNyanQuaternion{} },
            { 37, new VNyanQuaternion{} },
            { 38, new VNyanQuaternion{} },
        };
        public static Dictionary<int, VNyanQuaternion> RightArmTarget = new Dictionary<int, VNyanQuaternion> {
            { 12, new VNyanQuaternion{} },
            { 14, new VNyanQuaternion{} },
            { 16, new VNyanQuaternion{} },
            { 18, new VNyanQuaternion{} },
            { 39, new VNyanQuaternion{} },
            { 40, new VNyanQuaternion{} },
            { 41, new VNyanQuaternion{} },
            { 42, new VNyanQuaternion{} },
            { 43, new VNyanQuaternion{} },
            { 44, new VNyanQuaternion{} },
            { 45, new VNyanQuaternion{} },
            { 46, new VNyanQuaternion{} },
            { 47, new VNyanQuaternion{} },
            { 48, new VNyanQuaternion{} },
            { 49, new VNyanQuaternion{} },
            { 50, new VNyanQuaternion{} },
            { 51, new VNyanQuaternion{} },
            { 52, new VNyanQuaternion{} },
            { 53, new VNyanQuaternion{} }
        };

        // Last Leap Rotations
        public static Dictionary<int, VNyanQuaternion> LeftArmLastLeap = new Dictionary<int, VNyanQuaternion> {
            { 11, new VNyanQuaternion{} },
            { 13, new VNyanQuaternion{} },
            { 15, new VNyanQuaternion{} },
            { 17, new VNyanQuaternion{} },
            { 24, new VNyanQuaternion{} },
            { 25, new VNyanQuaternion{} },
            { 26, new VNyanQuaternion{} },
            { 27, new VNyanQuaternion{} },
            { 28, new VNyanQuaternion{} },
            { 29, new VNyanQuaternion{} },
            { 30, new VNyanQuaternion{} },
            { 31, new VNyanQuaternion{} },
            { 32, new VNyanQuaternion{} },
            { 33, new VNyanQuaternion{} },
            { 34, new VNyanQuaternion{} },
            { 35, new VNyanQuaternion{} },
            { 36, new VNyanQuaternion{} },
            { 37, new VNyanQuaternion{} },
            { 38, new VNyanQuaternion{} },
        };
        public static Dictionary<int, VNyanQuaternion> RightArmLastLeap = new Dictionary<int, VNyanQuaternion> {
            { 12, new VNyanQuaternion{} },
            { 14, new VNyanQuaternion{} },
            { 16, new VNyanQuaternion{} },
            { 18, new VNyanQuaternion{} },
            { 39, new VNyanQuaternion{} },
            { 40, new VNyanQuaternion{} },
            { 41, new VNyanQuaternion{} },
            { 42, new VNyanQuaternion{} },
            { 43, new VNyanQuaternion{} },
            { 44, new VNyanQuaternion{} },
            { 45, new VNyanQuaternion{} },
            { 46, new VNyanQuaternion{} },
            { 47, new VNyanQuaternion{} },
            { 48, new VNyanQuaternion{} },
            { 49, new VNyanQuaternion{} },
            { 50, new VNyanQuaternion{} },
            { 51, new VNyanQuaternion{} },
            { 52, new VNyanQuaternion{} },
            { 53, new VNyanQuaternion{} }
        };


        public static void setLeftArmLastLeap(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in LeftArm)
            {
                LeftArmLastLeap[boneNum] = Rotations_In[boneNum];
            }
        }
        public static void setRightArmLastLeap(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in RightArm)
            {
                RightArmLastLeap[boneNum] = Rotations_In[boneNum];
            }
        }

        public static void setLeftArmTarget(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in LeftArm)
            {
                LeftArmTarget[boneNum] = Rotations_In[boneNum];
            }
        }
        public static void setRightArmTarget(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in RightArm)
            {
                RightArmTarget[boneNum] = Rotations_In[boneNum];
            }
        }

        public static void rotateTowardsTargetLeft()
        {
            foreach (int boneNum in LeftArm)
            {
                VNyanQuaternion target = LeftArmTarget[boneNum];
                VNyanQuaternion current = LeftArmCurrent[boneNum];

                if ( !(current == target) )
                {
                    Quaternion newRotation = Quaternion.Slerp(VNyanExtra.QuaternionMethods.convertQuaternionV2U(current), VNyanExtra.QuaternionMethods.convertQuaternionV2U(target), slerpAmount * Time.deltaTime);
                    LeftArmCurrent[boneNum] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(newRotation);
                }
            }
        }

        public static void rotateTowardsTargetRight()
        {
            foreach (int boneNum in RightArm)
            {
                VNyanQuaternion target = RightArmTarget[boneNum];
                VNyanQuaternion current = RightArmCurrent[boneNum];

                if (!(current == target))
                {
                    Quaternion newRotation = Quaternion.Slerp(VNyanExtra.QuaternionMethods.convertQuaternionV2U(current), VNyanExtra.QuaternionMethods.convertQuaternionV2U(target), slerpAmount * Time.deltaTime);
                    RightArmCurrent[boneNum] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(newRotation);
                }
            }
        }
    }

    public class LeapFixerLayer : IPoseLayer
    {
        // Set up our frame-by-frame information
        public PoseLayerFrame LeapFixerFrame = new PoseLayerFrame();

        // Create containers to load pose data each frame
        public Dictionary<int, VNyanQuaternion> BoneRotations;
        public Dictionary<int, VNyanVector3> BonePositions;
        public Dictionary<int, VNyanVector3> BoneScales;
        public VNyanVector3 RootPos;
        public VNyanQuaternion RootRot;

        // VNyan Get Methods, VNyan uses these to get the pose after doUpdate()
        VNyanVector3 IPoseLayer.getBonePosition(int i)
        {
            return BonePositions[i];
        }
        VNyanQuaternion IPoseLayer.getBoneRotation(int i)
        {
            return BoneRotations[i];
        }
        VNyanVector3 IPoseLayer.getBoneScaleMultiplier(int i)
        {
            return BoneScales[i];
        }
        VNyanVector3 IPoseLayer.getRootPosition()
        {
            return RootPos;
        }
        VNyanQuaternion IPoseLayer.getRootRotation()
        {
            return RootRot;
        }

        // Pose Toggle Method, can be used to activate
        bool IPoseLayer.isActive()
        {
            return LeapFixerSettings.layerActive;
        }

        // doUpdate is how we get all current bone values and where we lay out the calculation/work each frame
        public void doUpdate(in PoseLayerFrame LeapFixerFrame)
        {
            // Get all current Bone and Root values up to this point from our Layer Frame, and load them in our holdover values.
            BoneRotations = LeapFixerFrame.BoneRotation;
            BonePositions = LeapFixerFrame.BonePosition;
            BoneScales = LeapFixerFrame.BoneScaleMultiplier;
            RootPos = LeapFixerFrame.RootPosition;
            RootRot = LeapFixerFrame.RootRotation;

            // logic order
            // for left and right sides separately
            // 1. Check Leap Motion Current State from VNyan
            LeapFixerSettings.setMirror(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("mirror"));
            LeapFixerSettings.setLeftStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapStatusL"));
            LeapFixerSettings.setRightStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapStatusR"));


            // 2. Check Status: If Leap status is on, read into "last leap rotations"
            if ( LeapFixerSettings.getLeftStatus() == 1f )
            {
                LeapFixerSettings.setLeftArmLastLeap(BoneRotations);
            }
            if (LeapFixerSettings.getRightStatus() == 1f)
            {
                LeapFixerSettings.setRightArmLastLeap(BoneRotations);
            }

            // 3. Check state
            // 3a. if off, just pass through our rotations
            // 3a. If state is stable, read the incoming Qs into the target Q's
            // 3c. If unstable, use smoothing

            // Left Arm //

            
            if (LeapFixerSettings.getLeftState() == 1f) // on + Stable
            {
                // set Target to come from our incoming bone rotations
                LeapFixerSettings.setLeftArmTarget(BoneRotations);

                // Rotate with smoothing the current towards the target
                LeapFixerSettings.rotateTowardsTargetLeft();

                foreach (int boneNum in LeapFixerSettings.LeftArm)
                {
                    BoneRotations[boneNum] = LeapFixerSettings.LeftArmCurrent[boneNum];
                }
            }
            else if (LeapFixerSettings.getLeftState() == 2f) // on + Unstable
            {
                // set Target to come from our last leap rotations
                LeapFixerSettings.setLeftArmTarget(LeapFixerSettings.LeftArmLastLeap);

                // Rotate with smoothing the current towards the target
                LeapFixerSettings.rotateTowardsTargetLeft();

                foreach (int boneNum in LeapFixerSettings.LeftArm)
                {
                    BoneRotations[boneNum] = LeapFixerSettings.LeftArmCurrent[boneNum];
                }
            }
            else
            {

            }
            
        }
    }
}
