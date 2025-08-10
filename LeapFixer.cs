using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using LZQuaternions;
using UnityEngine;
using VNyanInterface;


namespace Leap_Motion_Fixer
{
    class LeapFixerSettings
    {
        // Layer On/Off Setting
        public static bool layerActive = true; // flag for layer being active or not (when inactive, vnyan will stop reading from the rotations entirely)
        public void setLayerOnOff(float val) => layerActive = (val == 1f) ? true : false;

        // SLERP 1 amount
        private static float slerpAmount = 10f;

        /// <summary>
        /// Sets the main Slerp value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpAmount(float val) => slerpAmount = val;

        /// <summary>
        /// Gets the current main Slerp value used for smoothing.
        /// </summary>
        /// <returns>float of Slerp amount</returns>
        public float getSlerpAmount() => slerpAmount;

        // SLERP 2 amount
        private float slerpAmount2 = 5f;

        /// <summary>
        /// Sets the secondary Slerp value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpAmount2(float val) => slerpAmount2 = val;

        /// <summary>
        /// Gets the current secondary Slerp value used for smoothing.
        /// </summary>
        /// <returns>float of Slerp amount</returns>
        public float getSlerpAmount2() => slerpAmount2;

        // Force Smoothing Setting
        private bool forceSmoothingL = false;
        private bool forceSmoothingR = false;
        public void setForceSmoothingL(bool state) => forceSmoothingL = state;
        public void setForceSmoothingR(bool state) => forceSmoothingR = state;

        // States & Statuses
        private float leftState = 0f;
        private float leftStatus = 0f;
        private float rightState = 0f;
        private float rightStatus = 0f;
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
        public float getLeftState() => (mirror != 1f) ? leftState : rightState;
        public float getLeftStatus() => (mirror != 1f) ? leftStatus : rightStatus;
        public float getRightState() => (mirror != 1f) ? rightState : leftState;
        public float getRightStatus() => (mirror != 1f) ? rightStatus : leftStatus;

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
        
        // Bone Lists
        private static List<int> LeftArm = new List<int>
        {   (int)HumanBodyBones.LeftShoulder,
            (int)HumanBodyBones.LeftUpperArm,
            (int)HumanBodyBones.LeftLowerArm,
            (int)HumanBodyBones.LeftHand,
            (int)HumanBodyBones.LeftThumbProximal,
            (int)HumanBodyBones.LeftThumbIntermediate,
            (int)HumanBodyBones.LeftThumbDistal,
            (int)HumanBodyBones.LeftIndexProximal,
            (int)HumanBodyBones.LeftIndexIntermediate,
            (int)HumanBodyBones.LeftIndexDistal,
            (int)HumanBodyBones.LeftMiddleProximal,
            (int)HumanBodyBones.LeftMiddleIntermediate,
            (int)HumanBodyBones.LeftMiddleDistal,
            (int)HumanBodyBones.LeftRingProximal,
            (int)HumanBodyBones.LeftRingIntermediate,
            (int)HumanBodyBones.LeftRingDistal,
            (int)HumanBodyBones.LeftLittleProximal,
            (int)HumanBodyBones.LeftLittleIntermediate,
            (int)HumanBodyBones.LeftLittleDistal,
        };

        private static List<int> RightArm = new List<int>
        {   (int)HumanBodyBones.RightShoulder,
            (int)HumanBodyBones.RightUpperArm,
            (int)HumanBodyBones.RightLowerArm,
            (int)HumanBodyBones.RightHand,
            (int)HumanBodyBones.RightThumbProximal,
            (int)HumanBodyBones.RightThumbIntermediate,
            (int)HumanBodyBones.RightThumbDistal,
            (int)HumanBodyBones.RightIndexProximal,
            (int)HumanBodyBones.RightIndexIntermediate,
            (int)HumanBodyBones.RightIndexDistal,
            (int)HumanBodyBones.RightMiddleProximal,
            (int)HumanBodyBones.RightMiddleIntermediate,
            (int)HumanBodyBones.RightMiddleDistal,
            (int)HumanBodyBones.RightRingProximal,
            (int)HumanBodyBones.RightRingIntermediate,
            (int)HumanBodyBones.RightRingDistal,
            (int)HumanBodyBones.RightLittleProximal,
            (int)HumanBodyBones.RightLittleIntermediate,
            (int)HumanBodyBones.RightLittleDistal,
        };

        private static List<int> AllBones = LeftArm.Concat(RightArm).ToList();

        public List<int> getLeftArmBones() => LeftArm;
        public List<int> getRightArmBones() => RightArm;
        public List<int> getAllBones() => AllBones;

        /* We keep three dictionaries to maintain our setup
         * "Current" = Internally keeps bone rotations that we will overwrite onto VNyan's bone rotations
         * "Target" = Rotations we want to smoothly SLERP the current rotations towards
         * "LastLeap" = We will keep the incoming VNyan rotations here when leap motion is working well. When unstable, we will stop reading into this
         */
        //private Dictionary<int, VNyanQuaternion> LeftArmCurrent = createQuaternionDictionary(LeftArm);
        //private Dictionary<int, VNyanQuaternion> LeftArmTarget = createQuaternionDictionary(LeftArm);
        //private Dictionary<int, VNyanQuaternion> LeftArmLastLeap = createQuaternionDictionary(LeftArm);

        //private Dictionary<int, VNyanQuaternion> RightArmCurrent = createQuaternionDictionary(RightArm);
        //private Dictionary<int, VNyanQuaternion> RightArmTarget = createQuaternionDictionary(RightArm);
        //private Dictionary<int, VNyanQuaternion> RightArmLastLeap = createQuaternionDictionary(RightArm);

        private static Dictionary<int, VNyanQuaternion> armsCurrent = createQuaternionDictionary(AllBones);
        private static Dictionary<int, VNyanQuaternion> armsTarget = createQuaternionDictionary(AllBones);
        private static Dictionary<int, VNyanQuaternion> armsLastLeap = createQuaternionDictionary(AllBones);



        /* Setters for Target and LastLeap bone rotations
         */
        public void setCurrentBone(int boneNum, VNyanQuaternion bone)
        {
            armsCurrent[boneNum] = bone;
        }

        public void setTargetBone(int boneNum, VNyanQuaternion bone)
        {
            armsTarget[boneNum] = bone;
        }

        public void setLastLeapBone(int boneNum, VNyanQuaternion bone)
        {
            armsLastLeap[boneNum] = bone;
        }


        public void setCurrentBones(List<int> BoneList, Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in BoneList)
            {
                armsCurrent[boneNum] = Rotations_In[boneNum];
            }
        }

        public void setLastLeapBones(List<int> BoneList, Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in BoneList)
            {
                armsLastLeap[boneNum] = Rotations_In[boneNum];
            }
        }

        /// <summary>
        /// Sets the Left arm's target dictionary
        /// </summary>
        /// <param name="Rotations_In"></param>
        public void setTargetBones(List<int> BoneList, Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in BoneList)
            {
                setTargetBone(boneNum, Rotations_In[boneNum]);
            }
        }

        // Getters
        public VNyanQuaternion getCurrentBone(int boneNum) => armsCurrent[boneNum];
        public Dictionary<int, VNyanQuaternion> getCurrentBones() => armsCurrent;

        /// <summary>
        /// Gets the Last Leap rotations dictionary for left arm.
        /// </summary>
        /// <returns>LeftArmLastLeap</returns>
        public Dictionary<int, VNyanQuaternion> getLastLeapBones() => armsLastLeap;


        /// <summary>
        /// Gets the Target rotation's dictionary for left arm
        /// </summary>
        /// <returns>LeftArmTarget</returns>
        public Dictionary<int, VNyanQuaternion> getTargetBones() => armsTarget;

        ///// <summary>
        ///// Record tracking from Current into LastLeap Dict if Left Leap Status is on.
        ///// </summary>
        //public void updateLastLeapBones(float status, List<int> BoneList)
        //{
        //    if (status == 1f)
        //    {
        //        foreach (int boneNum in BoneList)
        //        {
        //            armsLastLeap[boneNum] = armsCurrent[boneNum];
        //        }
        //    }
        //}

        /// <summary>
        /// Uses Slerp method to rotate Left arm's Current dictionary rotations towards the target rotations, writing result back into Current dictionary.
        /// </summary>
        /// <param name="smoothing">float of Slerp amount</param>
        public void rotateTowardsTarget(int boneNum, float slerpAmount, float angleScale)
        {
            VNyanQuaternion target = armsTarget[boneNum];
            VNyanQuaternion current = armsCurrent[boneNum];

            if ( !(current == target) )
            {
                Quaternion target_U = QuaternionMethods.convertQuaternionV2U(target);
                Quaternion current_U = QuaternionMethods.convertQuaternionV2U(current);

                Quaternion newRotation = Quaternion.Slerp(LZQuaternions.QuaternionMethods.convertQuaternionV2U(current), LZQuaternions.QuaternionMethods.convertQuaternionV2U(target), slerpAmount * Time.deltaTime);
                setCurrentBone(boneNum, LZQuaternions.QuaternionMethods.convertQuaternionU2V(newRotation));
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

        /// <summary>
        /// Gets the layer settings, giving access to all dictionaries and methods.
        /// </summary>
        /// <returns>LeapFixerSettings settings</returns>
        public LeapFixerSettings getSettings()
        {
            return settings;
        }

        public void updateTargetBones(List<int> BoneList)
        {
            foreach (int boneNum in BoneList)
            {
                if (BoneRotations.TryGetValue(boneNum, out VNyanQuaternion vnyanCurrent))
                {
                    // 1. update target
                    settings.setTargetBone(boneNum, vnyanCurrent);
                }
            }
        }

        public void processBoneRotations(List<int> BoneList, float slerpAmount, float adaptiveAmount)
        {
            foreach (int boneNum in BoneList)
            {
                // 2. rotate current towards target
                settings.rotateTowardsTarget(boneNum, slerpAmount, adaptiveAmount);
            }
        }

        public void updateBoneRotations(Dictionary<int, VNyanQuaternion> newRotations, List<int> BoneList)
        {
            foreach (int boneNum in BoneList)
            {
                if (BoneRotations.ContainsKey(boneNum))
                {
                    BoneRotations[boneNum] = newRotations[boneNum];
                }
            }
        }




        // doUpdate is how we get all current bone values and where we lay out the calculation/work each frame
        public void doUpdate(in PoseLayerFrame LeapFixerFrame)
        {
            BoneRotations = LeapFixerFrame.BoneRotation;
            BonePositions = LeapFixerFrame.BonePosition;
            BoneScales = LeapFixerFrame.BoneScaleMultiplier;
            RootPos = LeapFixerFrame.RootPosition;
            RootRot = LeapFixerFrame.RootRotation;

            getSettings().getVNyanLeapMirror();
            getSettings().getVNyanLeapState();
            getSettings().getVNyanLeapStatus();

            ///* Logic:
            // * 0. When it's off, 
            // * - - we don't do anything to avatar, 
            // * - - we record where the current bone rotations should be
            // * 1. When it's On + Stable, 
            // * - - we take incoming bone rotations as our target, and rotate current towards with tiny smoothing
            // * - - we record the last frame's 'current' rotations to our "last leap"
            // * 2. When it's On + Unstable, 
            // * - - we use the "last leap" as our target, and rotate current towards with slower slerp
            // * - - we *don't* update "last leap"
            // * 3. When it's transitioning from unstable to stable
            // *  - - we take incoming bone rotations as our target, and rotate current towards
            // *  - - we record the last frame's 'current' rotations to our "last leap"
            // */

            //switch(getSettings().getLeftState())
            //{
            //    case 0f:
            //        settings.setCurrentBones(settings.getLeftArmBones(), BoneRotations);
            //        break;
            //    case 1f:

            //        break;
            //    case 2f:
            //        break;
            //    case 3f:
            //        break;
            //}


            // Left Arm //
            if (settings.getLeftState() == 0f) // State 0: complete off
            {
                settings.setCurrentBones(settings.getLeftArmBones(), BoneRotations);
            }
            else if (settings.getLeftState() == 1f) // State 1: on
            {
                settings.setLastLeapBones(settings.getLeftArmBones(), settings.getCurrentBones());
                updateTargetBones(settings.getLeftArmBones());
                processBoneRotations(settings.getLeftArmBones(), settings.getSlerpAmount(), 0f);
            }
            else if (settings.getLeftState() == 2f) // State 2: on + unstable, paused
            {
                if (settings.getLeftStatus() == 1f)
                {
                    settings.setTargetBones(settings.getLeftArmBones(), settings.getLastLeapBones());
                }
                
                processBoneRotations(settings.getLeftArmBones(), settings.getSlerpAmount2(), 0f);
            }
            else if (settings.getLeftState() == 3f ) // State 3: on + stabilizing, returning.
            {
                settings.setTargetBones(settings.getLeftArmBones(), settings.getCurrentBones());
                updateTargetBones(settings.getLeftArmBones());
                processBoneRotations(settings.getLeftArmBones(), settings.getSlerpAmount2(), 0f);
            }

            //// Right Arm //
            //if (settings.getRightState() == 0f) // State 0: complete off
            //{
            //    settings.setRightArmCurrent(BoneRotations);
            //}
            //else if (settings.getRightState() == 1f) // State 1: on
            //{
            //    settings.recordLastLeapRight();
            //    settings.setRightArmTarget(BoneRotations);
            //    settings.rotateTowardsTargetRight(settings.getSlerpAmount());
            //}
            //else if (settings.getRightState() == 2f) // State 2: on + unstable, paused
            //{
            //    if (settings.getRightStatus() == 1f)
            //    {
            //        settings.setRightArmTarget(settings.getRightArmLastLeap());
            //    }
            //    settings.rotateTowardsTargetRight(settings.getSlerpAmount2());
            //}
            //else if (settings.getRightState() == 3f) // State 3: on + stabilizing, returning.
            //{
            //    settings.recordLastLeapRight();
            //    settings.setRightArmTarget(BoneRotations);
            //    settings.rotateTowardsTargetRight(settings.getSlerpAmount2());
            //}

            updateBoneRotations(settings.getCurrentBones(), settings.getAllBones());

            //// Apply "current bone" dictionary into pose layer
            //foreach (int boneNum in settings.getLeftArmBones())
            //{
            //    BoneRotations[boneNum] = settings.getLeftArmCurrentBone(boneNum);
            //}

            //foreach (int boneNum in settings.getRightArmBones())
            //{
            //    BoneRotations[boneNum] = settings.getRightArmCurrentBone(boneNum);
            //}
        }
    }
}
