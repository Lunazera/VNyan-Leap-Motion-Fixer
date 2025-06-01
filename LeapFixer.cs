using System;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;
using static UnityEngine.Random;
using static UnityEngine.TouchScreenKeyboard;

namespace Leap_Motion_Fixer
{
    class LeapFixerSettings
    {
        // Layer On/Off Setting
        public static bool layerActive = true; // flag for layer being active or not (when inactive, vnyan will stop reading from the rotations entirely)
        public void setLayerOnOff(float val) => layerActive = (val == 1f) ? true : false;

        public static float slerpAmount = 10f;

        public void setSlerpAmount(float val) => slerpAmount = val;

        public float getSlerpAmount() => slerpAmount;

        // Arm States:
        // 0 = off,
        // 1 = stable,
        // 2 = unstable

        // Arm Status:
        // 0 = LeapOff
        // 1 = LeapOn

        // Mirror
        // 0 = unmirrored, 1 = mirrored

        private float leftState = 0f;
        private float rightState = 0f;
        private float rightStatus = 0f;
        private float leftStatus = 0f;
        private float mirror = 0f;

        // Mirror set
        public void setMirror(float val) => mirror = val;
        // Left Sets
        public void setLeftState(float val) => leftState = val;
        public void setLeftStatus(float val) => leftStatus = val;
        // Right Sets
        public void setRightState(float val) => rightState = val;
        public void setRightStatus(float val) => rightStatus = val;
        
        // Gets
        // These take into account the mirror setting, so it doesn't need to be handled in VNyan
        public float getMirror() => mirror;
        public float getLeftState() => (getMirror() != 1f) ? leftState : rightState;
        public float getLeftStatus() => (getMirror() != 1f) ? leftStatus : rightStatus;
        public float getRightState() => (getMirror() != 1f) ? rightState : leftState;
        public float getRightStatus() => (getMirror() != 1f) ? rightStatus : leftStatus;

        //VNyan Getters
        public void getVNyanLeapMirror()
        {
            setMirror(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapMirror"));
        }
        public void getVNyanLeapState()
        {
            setLeftState(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixerStateL"));
            setRightState(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixerStateR"));
        }
        public void getVNyanLeapStatus()
        {
            setLeftStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapStatusL"));
            setRightStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapStatusR"));
        }

        /// <summary>
        /// Creates VNyanQuaternion Dictionaries given a list of ints
        /// </summary>
        /// <param name="boneList"></param>
        /// <returns></returns>
        private static Dictionary<int, VNyanQuaternion> createQuaternionDictionary(List<int> boneList)
        {
            Dictionary<int, VNyanQuaternion> rotDic = new Dictionary<int, VNyanQuaternion>();
            foreach (var ele in boneList)
            {
                rotDic.Add(ele, new VNyanQuaternion { });
            }
            return rotDic;
        }

        // Bone dictionaries
        private static List<int> LeftArm = new List<int> { 11, 13, 15, 17, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 };
        private static List<int> RightArm = new List<int> { 12, 14, 16, 18, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53 };

        public List<int> getLeftArmBones()
        {
            return LeftArm;
        }
        public List<int> getRightArmBones()
        {
            return RightArm;
        }

        /* We keep three dictionaries to maintain our setup
         * "Current" = Internally keeps bone rotations that we will overwrite onto VNyan's bone rotations
         * "Target" = Rotations we want to smoothly SLERP the current rotations towards
         * "LastLeap" = We will keep the incoming VNyan rotations here when leap motion is working well. When unstable, we will stop reading into this
         */
        private Dictionary<int, VNyanQuaternion> LeftArmCurrent = createQuaternionDictionary(LeftArm);
        private Dictionary<int, VNyanQuaternion> LeftArmTarget = createQuaternionDictionary(LeftArm);
        private Dictionary<int, VNyanQuaternion> LeftArmLastLeap = createQuaternionDictionary(LeftArm);

        private Dictionary<int, VNyanQuaternion> RightArmCurrent = createQuaternionDictionary(RightArm);
        private Dictionary<int, VNyanQuaternion> RightArmTarget = createQuaternionDictionary(RightArm);
        private Dictionary<int, VNyanQuaternion> RightArmLastLeap = createQuaternionDictionary(RightArm);


        /* Setters for Target and LastLeap bone rotations
         */
        public void setLeftArmCurrent(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in LeftArm)
            {
                LeftArmCurrent[boneNum] = Rotations_In[boneNum];
            }
        }
        public void setRightArmCurrent(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in RightArm)
            {
                RightArmCurrent[boneNum] = Rotations_In[boneNum];
            }
        }
        public void setLeftArmLastLeap(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in LeftArm)
            {
                LeftArmLastLeap[boneNum] = Rotations_In[boneNum];
            }
        }
        public void setRightArmLastLeap(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in RightArm)
            {
                RightArmLastLeap[boneNum] = Rotations_In[boneNum];
            }
        }

        public void setLeftArmTarget(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in LeftArm)
            {
                LeftArmTarget[boneNum] = Rotations_In[boneNum];
            }
        }
        public void setRightArmTarget(Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in RightArm)
            {
                RightArmTarget[boneNum] = Rotations_In[boneNum];
            }
        }

        //
        public VNyanQuaternion getLeftArmCurrent(int boneNum)
        {
            return LeftArmCurrent[boneNum];
        }
        public VNyanQuaternion getRightArmCurrent(int boneNum)
        {
            return RightArmCurrent[boneNum];
        }
        public Dictionary<int,VNyanQuaternion> getLeftArmLastLeap()
        {
            return LeftArmLastLeap;
        }
        public Dictionary<int, VNyanQuaternion> getRightArmLastLeap()
        {
            return RightArmLastLeap;
        }

        public Dictionary<int, VNyanQuaternion> getLeftArmTarget()
        {
            return LeftArmTarget;
        }

        public Dictionary<int, VNyanQuaternion> getRightArmTarget()
        {
            return RightArmTarget;
        }

        public void rotateTowardsTargetLeft(float smoothing)
        {
            foreach (int boneNum in getLeftArmBones())
            {
                VNyanQuaternion target = LeftArmTarget[boneNum];
                VNyanQuaternion current = LeftArmCurrent[boneNum];

                if ( current != target )
                {
                    Quaternion newRotation = Quaternion.Slerp(VNyanExtra.QuaternionMethods.convertQuaternionV2U(current), VNyanExtra.QuaternionMethods.convertQuaternionV2U(target), smoothing * Time.deltaTime);
                    LeftArmCurrent[boneNum] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(newRotation);
                }
            }
        }

        public void rotateTowardsTargetRight(float smoothing)
        {
            foreach (int boneNum in getRightArmBones())
            {
                VNyanQuaternion target = RightArmTarget[boneNum];
                VNyanQuaternion current = RightArmCurrent[boneNum];

                if (!(current == target))
                {
                    Quaternion newRotation = Quaternion.Slerp(VNyanExtra.QuaternionMethods.convertQuaternionV2U(current), VNyanExtra.QuaternionMethods.convertQuaternionV2U(target), smoothing * Time.deltaTime);
                    RightArmCurrent[boneNum] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(newRotation);
                }
            }
        }
    }

    class LeapFixerLayer : IPoseLayer
    {
        public static LeapFixerSettings settings = new LeapFixerSettings();

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

            settings.setMirror(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("mirror"));
            settings.getVNyanLeapState();
            settings.getVNyanLeapStatus();


            // 2. Check Status: If Leap status is on, read into "last leap rotations".
            // This will always get the Target rotations from the previous frame
            if (settings.getLeftStatus() == 1f )
            {
                settings.setLeftArmLastLeap(settings.getLeftArmTarget());
            }
            if (settings.getRightStatus() == 1f)
            {
                settings.setRightArmLastLeap(settings.getRightArmTarget());
            }

            // 3. Check state
            // 3a. if off, just pass through our rotations
            // 3a. If state is stable, read the incoming Qs into the target Q's
            // 3c. If unstable, use smoothing

            // Left Arm //


            if (settings.getLeftState() == 0f )
            {
                // System is Off/unused, passthrough but keep Current updated
                settings.setLeftArmCurrent(BoneRotations);
            }
            else if (settings.getLeftState() == 1f )
            {
                // System is On/Stable, 
                settings.setLeftArmTarget(BoneRotations); // Set target from last leap motion rotations read
                settings.rotateTowardsTargetLeft(settings.getSlerpAmount());
                foreach (int boneNum in settings.getLeftArmBones())
                {
                    BoneRotations[boneNum] = settings.getLeftArmCurrent(boneNum);
                }
            }
            else if (settings.getLeftState() == 2f )
            {
                // System is On/UNSTABLE
                settings.setLeftArmTarget(settings.getLeftArmLastLeap());
                settings.rotateTowardsTargetLeft(5);
                foreach (int boneNum in settings.getLeftArmBones())
                {
                    BoneRotations[boneNum] = settings.getLeftArmCurrent(boneNum);
                }
            }
            else if (settings.getLeftState() == 3f )
            {
                // Transition state between on/off
                settings.setLeftArmTarget(BoneRotations); // Set target from last leap motion rotations read
                settings.rotateTowardsTargetLeft(5);
                foreach (int boneNum in settings.getLeftArmBones())
                {
                    BoneRotations[boneNum] = settings.getLeftArmCurrent(boneNum);
                }
            }


            if (settings.getRightState() == 0f)
            {
                // System is Off/unused, passthrough but keep Current updated
                settings.setRightArmCurrent(BoneRotations);
            }
            else if (settings.getRightState() == 1f)
            {
                // System is On/Stable, 
                settings.setRightArmTarget(BoneRotations); // Set target from last leap motion rotations read
                settings.rotateTowardsTargetRight(settings.getSlerpAmount());
                foreach (int boneNum in settings.getRightArmBones())
                {
                    BoneRotations[boneNum] = settings.getRightArmCurrent(boneNum);
                }
            }
            else if (settings.getRightState() == 2f)
            {
                // System is On/UNSTABLE
                settings.setRightArmTarget(settings.getRightArmLastLeap());
                settings.rotateTowardsTargetRight(5);
                foreach (int boneNum in settings.getRightArmBones())
                {
                    BoneRotations[boneNum] = settings.getRightArmCurrent(boneNum);
                }
            }
            else if (settings.getRightState() == 3f)
            {
                // Transition state between on/off
                settings.setRightArmTarget(BoneRotations); // Set target from last leap motion rotations read
                settings.rotateTowardsTargetRight(5);
                foreach (int boneNum in settings.getRightArmBones())
                {
                    BoneRotations[boneNum] = settings.getRightArmCurrent(boneNum);
                }
            }
        }
    }
}
