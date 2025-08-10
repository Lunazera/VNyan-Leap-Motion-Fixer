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
        private float slerpAmount = 10f;
        private float slerpAmount2 = 5f;
        private float slerpBoost = 0f;

        /// <summary>
        /// Sets the main Slerp value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpAmount(float val) => slerpAmount = rescaleInvertSpeed(val);
        /// <summary>
        /// Sets the secondary Slerp value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpAmount2(float val) => slerpAmount2 = rescaleInvertSpeed(val);
        /// <summary>
        /// Sets the slerp boost value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpBoost(float val) => slerpBoost = rescaleInvertSpeed(val/10);

        public float getSlerpAmount() => slerpAmount;
        public float getSlerpAmount2() => slerpAmount2;
        public float getSlerpBoost() => slerpBoost;

        /// <summary>
        /// Rescale the settings range and inverting the diretion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float rescaleInvertSpeed(float value)
        {
            return 50f * (1 - value / 100f);
        }

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
        /// <param name="BoneList"></param>
        /// <returns></returns>
        private static Dictionary<int, VNyanQuaternion> createQuaternionDictionary(List<int> BoneList)
        {
            Dictionary<int, VNyanQuaternion> rotDic = new Dictionary<int, VNyanQuaternion>();
            foreach (var ele in BoneList)
            {
                rotDic.Add(ele, new VNyanQuaternion { });
            }
            return rotDic;
        }

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

        private static List<int> AllBones = new List<int>
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
            (int)HumanBodyBones.RightShoulder,
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

        public List<int> getLeftArmBones() => LeftArm;
        public List<int> getRightArmBones() => RightArm;
        public List<int> getAllBones() => AllBones;

        /* We keep three dictionaries to maintain our setup
         * "Current" = Internally keeps bone rotations that we will overwrite onto VNyan's bone rotations
         * "Target" = Rotations we want to smoothly SLERP the current rotations towards
         * "LastLeap" = We will keep the incoming VNyan rotations here when leap motion is working well. When unstable, we will stop reading into this
         */
        private static Dictionary<int, VNyanQuaternion> armsCurrent = createQuaternionDictionary(AllBones);
        private static Dictionary<int, VNyanQuaternion> armsTarget = createQuaternionDictionary(AllBones);
        private static Dictionary<int, VNyanQuaternion> armsLastLeap = createQuaternionDictionary(AllBones);

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
                setCurrentBone(boneNum, Rotations_In[boneNum]);
            }
        }

        public void setTargetBones(List<int> BoneList, Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in BoneList)
            {
                setTargetBone(boneNum, Rotations_In[boneNum]);
            }
        }

        public void updateLastLeapBones(float status, List<int> BoneList)
        {
            if (status == 1f)
            {
                foreach (int boneNum in BoneList)
                {
                    setLastLeapBone(boneNum, getCurrentBone(boneNum));
                }
            }
        }

        public VNyanQuaternion getCurrentBone(int boneNum) => armsCurrent[boneNum];
        public VNyanQuaternion getTargetBone(int boneNum) => armsTarget[boneNum];
        public VNyanQuaternion getLastLeapBone(int boneNum) => armsLastLeap[boneNum];

        /// <summary>
        /// Gets the Last Leap rotations dictionary for left arm.
        /// </summary>
        /// <returns>LeftArmLastLeap</returns>
        public Dictionary<int, VNyanQuaternion> getLastLeapBones() => armsLastLeap;

        /// <summary>
        /// Calculates a multiplier based on the angle between two quaternions and a scale.
        /// </summary>
        /// <param name="qFrom"></param>
        /// <param name="qTo"></param>
        /// <param name="adaptiveScale"></param>
        /// <returns></returns>
        public float setAdaptiveAngle(Quaternion qFrom, Quaternion qTo, float adaptiveScale)
        {
            return adaptiveScale * Quaternion.Angle(qFrom, qTo);
        }

        /// <summary>
        /// Applies Quaternion Slerp method, linearly scaling the slerp amount by the angle between the current and target bones.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="slerpAmount"></param>
        /// <param name="adaptiveScale"></param>
        /// <returns></returns>
        public VNyanQuaternion adaptiveSlerp(VNyanQuaternion current, VNyanQuaternion target, float slerpAmount, float adaptiveScale)
        {
            Quaternion currentUnityQ = QuaternionMethods.convertQuaternionV2U(current);
            Quaternion targetUnityQ = QuaternionMethods.convertQuaternionV2U(target);

            float angleSpeed = setAdaptiveAngle(currentUnityQ, targetUnityQ, adaptiveScale);

            return QuaternionMethods.convertQuaternionU2V(Quaternion.Slerp(currentUnityQ, targetUnityQ, (slerpAmount + angleSpeed) * Time.deltaTime));
        }

        public void rotateTowardsTarget(List<int> BoneList, float slerpAmount, float angleScale)
        {
            foreach (int boneNum in BoneList)
            {
                VNyanQuaternion target = getTargetBone(boneNum);
                VNyanQuaternion current = getCurrentBone(boneNum);

                if ( !(current == target) )
                {
                    setCurrentBone(boneNum, adaptiveSlerp(current, target, slerpAmount, angleScale));
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

            switch (getSettings().getLeftState())
            {
                case 0f:
                    settings.setCurrentBones(settings.getLeftArmBones(), BoneRotations);
                    break;
                case 1f:
                    settings.updateLastLeapBones(settings.getLeftStatus(), settings.getLeftArmBones());
                    settings.setTargetBones(settings.getLeftArmBones(), BoneRotations);
                    settings.rotateTowardsTarget(settings.getLeftArmBones(),settings.getSlerpAmount(), 0f);
                    break;
                case 2f:
                    if (settings.getLeftStatus() == 1f)
                    {
                        settings.setTargetBones(settings.getLeftArmBones(), settings.getLastLeapBones());
                    }
                    settings.rotateTowardsTarget(settings.getLeftArmBones(),settings.getSlerpAmount2(), 0f);
                    break;
                case 3f:
                    settings.updateLastLeapBones(settings.getLeftStatus(), settings.getLeftArmBones());
                    settings.setTargetBones(settings.getLeftArmBones(), BoneRotations);
                    settings.rotateTowardsTarget(settings.getLeftArmBones(),settings.getSlerpAmount2(), 0f);
                    break;
            }

            switch (getSettings().getRightState())
            {
                case 0f:
                    settings.setCurrentBones(settings.getRightArmBones(), BoneRotations);
                    break;
                case 1f:
                    settings.updateLastLeapBones(settings.getRightStatus(), settings.getRightArmBones());
                    settings.setTargetBones(settings.getRightArmBones(), BoneRotations);
                    settings.rotateTowardsTarget(settings.getRightArmBones(), settings.getSlerpAmount(), 0f);
                    break;
                case 2f:
                    if (settings.getRightStatus() == 1f)
                    {
                        settings.setTargetBones(settings.getRightArmBones(), settings.getLastLeapBones());
                    }
                    settings.rotateTowardsTarget(settings.getRightArmBones(), settings.getSlerpAmount2(), 0f);
                    break;
                case 3f:
                    settings.updateLastLeapBones(settings.getRightStatus(), settings.getRightArmBones());
                    settings.setTargetBones(settings.getRightArmBones(), BoneRotations);
                    settings.rotateTowardsTarget(settings.getRightArmBones(), settings.getSlerpAmount2(), 0f);
                    break;
            }

            // Apply "current bone" dictionary into pose layer
            foreach (int boneNum in settings.getAllBones())
            {
                BoneRotations[boneNum] = settings.getCurrentBone(boneNum);
            }
        }
    }
}
