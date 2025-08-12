using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using LZQuaternions;
using UnityEngine;
using VNyanInterface;
using static Leap_Motion_Fixer.LeapFixerLayer;
using static UnityEngine.Random;
using static UnityEngine.TouchScreenKeyboard;


namespace Leap_Motion_Fixer
{
    class LeapFixerSettings
    {
        // Layer On/Off Setting
        public static bool layerActive = true; // flag for layer being active or not (when inactive, vnyan will stop reading from the rotations entirely)
        public void setLayerOnOff(float val) => layerActive = (val == 1f) ? true : false;

        private float timeout = 2000f; // ms, will be divided by 1000
        private float sensitivity = 5f; // number of events

        private float transitionTime = 400f;

        private float slerpAmount = 10f;
        private float slerpAmount2 = 5f;
        private float slerpBoost = 0f;

        /// <summary>
        /// Rescale the settings range and inverting the diretion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float rescaleInvertSpeed(float value, float newScale)
        {
            return newScale * (1 - value / 100f);
        }

        /// <summary>
        /// Sets the main Slerp value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpAmount(float val, float scale) => slerpAmount = rescaleInvertSpeed(val, scale);
        /// <summary>
        /// Sets the secondary Slerp value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpAmount2(float val, float scale) => slerpAmount2 = rescaleInvertSpeed(val, scale);
        /// <summary>
        /// Sets the slerp boost value
        /// </summary>
        /// <param name="val"></param>
        public void setSlerpBoost(float val, float scale) => slerpBoost = val * scale;

        public void setTimeout(float val) => timeout = val;
        public void setTransitionTime(float val) => transitionTime = val;
        public void setSensitivity(float val) => sensitivity = val;

        public float getSlerpAmount() => slerpAmount;
        public float getSlerpAmount2() => slerpAmount2;
        public float getSlerpBoost() => slerpBoost;

        public float getTimeout() => timeout / 1000f;
        public float getTransitionTime() => transitionTime / 1000f;
        public float getSensitivity() => sensitivity;

        

        // States & Statuses
        private float leftState = 0f;
        private float leftStatus = 0f;
        private float rightState = 0f;
        private float rightStatus = 0f;
        private float mirror = 0f;

        private float leftStatusTimer = 0f;
        private float rightStatusTimer = 0f;


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
        public float getLeftState() => leftState;
        public float getLeftStatus() => leftStatus;
        public float getRightState() => rightState;
        public float getRightStatus() => rightStatus;

        public float getLeftStatusTimer() => leftStatusTimer;
        public float getRightStatusTimer() => rightStatusTimer;

        public void increaseLeftStatusTimer(float var) => leftStatusTimer += (var);
        public void increaseRightStatusTimer(float var) => rightStatusTimer += (var);

        public void resetLeftStatusTimer() => leftStatusTimer = 0f;
        public void resetRightStatusTimer() => rightStatusTimer = 0f;
        public void getVNyanLeapStatus()
        {
            setLeftStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LeapStatusL"));
            setRightStatus(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LeapStatusR"));
        }

        public float getVNyanLeftStatusTrack()
        {
            return VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixer_LeapStatusTrackL");
        }

        public float getVNyanRightStatusTrack()
        {
            return VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixer_LeapStatusTrackR");
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
                if (Rotations_In.ContainsKey(boneNum))
                {
                    setCurrentBone(boneNum, Rotations_In[boneNum]);
                }
            }
        }

        public void setTargetBones(List<int> BoneList, Dictionary<int, VNyanQuaternion> Rotations_In)
        {
            foreach (int boneNum in BoneList)
            {
                if (Rotations_In.ContainsKey(boneNum))
                {
                    setTargetBone(boneNum, Rotations_In[boneNum]);
                }
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

                if (!(current == target))
                {
                    setCurrentBone(boneNum, adaptiveSlerp(current, target, slerpAmount, 0f));
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

        public List<int> LeftArmBones = settings.getLeftArmBones();
        public List<int> RightArmBones = settings.getRightArmBones();
        public List<int> AllBones = settings.getAllBones();

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

        /// <summary>
        /// Updates vnyan avatar rotations in Bone Rotations
        /// </summary>
        /// <param name="BoneList"></param>
        public void updateBoneRotations(List<int> BoneList)
        {
            foreach (int boneNum in BoneList)
            {
                if (BoneRotations.ContainsKey(boneNum))
                {
                    BoneRotations[boneNum] = getSettings().getCurrentBone(boneNum);
                }
            }
        }

        public abstract class LayerState : State
        {
            public override void setCurrentDict(PoseLayerFrame Frame) => settings.setCurrentBones(getBoneList(), Frame.BoneRotation);
            public override void setTargetDict(PoseLayerFrame Frame) => settings.setTargetBones(getBoneList(), Frame.BoneRotation);
            public override void setTargetDictLastLeap() => settings.setTargetBones(getBoneList(), settings.getLastLeapBones());
            public override void updateLastLeapDict() => settings.updateLastLeapBones(getStatus(), getBoneList());
            public override void rotateTowardsTarget(float slerp, float boost) => settings.rotateTowardsTarget(getBoneList(), slerp, boost);

            public override float getSlerp() => settings.getSlerpAmount();
            public override float getSlerpUnstable() => settings.getSlerpAmount2();
            public override float getSlerpBoost() => settings.getSlerpBoost();
            public override float getTimeout() => settings.getTimeout();
            public override float getSensitivity() => settings.getSensitivity();
            public override float getTransitionTime() => settings.getTransitionTime();
        } 

        public class LeftState : LayerState
        {
            public override List<int> getBoneList() => settings.getLeftArmBones();
            public override float getState() => settings.getLeftState();
            public override float getStatus() => settings.getLeftStatus();
            public override float getStatusTimer() => settings.getLeftStatusTimer();
            public override float getStatusTrack() => settings.getVNyanLeftStatusTrack();
            public override void increaseStatusTimer(float time) => settings.increaseLeftStatusTimer(Time.deltaTime);
            public override void resetStatusTimer() => settings.resetLeftStatusTimer();
            public override void setState(float state) => settings.setLeftState(state);
        }

        public class RightState : LayerState
        {
            public override List<int> getBoneList() => settings.getRightArmBones();
            public override float getState() => settings.getRightState();
            public override float getStatus() => settings.getRightStatus();
            public override float getStatusTimer() => settings.getRightStatusTimer();
            public override float getStatusTrack() => settings.getVNyanRightStatusTrack();
            public override void increaseStatusTimer(float time) => settings.increaseRightStatusTimer(Time.deltaTime);
            public override void resetStatusTimer() => settings.resetRightStatusTimer();
            public override void setState(float state) => settings.setRightState(state);
        }

        ILeapState LeftStateManager = new LeftState();
        ILeapState RightStateManager = new RightState();

        public void doUpdate(in PoseLayerFrame LeapFixerFrame)
        {
            BoneRotations = LeapFixerFrame.BoneRotation;
            BonePositions = LeapFixerFrame.BonePosition;
            BoneScales = LeapFixerFrame.BoneScaleMultiplier;
            RootPos = LeapFixerFrame.RootPosition;
            RootRot = LeapFixerFrame.RootRotation;

            getSettings().getVNyanLeapStatus();

            LeftStateManager.ManageState(LeapFixerFrame);
            LeftStateManager.setStateVNyan("LZ_LeapFixer_LeftStateManager");

            RightStateManager.ManageState(LeapFixerFrame);
            RightStateManager.setStateVNyan("LZ_LeapFixer_RightStateManager");

            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("leapfixer_lefttimer", getSettings().getLeftStatusTimer());
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("leapfixer_righttimer", getSettings().getRightStatusTimer());

            // Apply bones if they exist in bonerotations
            updateBoneRotations(LeftArmBones);
            updateBoneRotations(RightArmBones);
        }
    }
}