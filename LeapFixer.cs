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
        public void setSlerpBoost(float val) => slerpBoost = rescaleInvertSpeed(val / 10);
        public void setTimeout(float val) => timeout = val;
        public void setSensitivity(float val) => sensitivity = val;

        public float getSlerpAmount() => slerpAmount;
        public float getSlerpAmount2() => slerpAmount2;
        public float getSlerpBoost() => slerpBoost;

        public float getTimeout() => timeout / 1000f;
        public float getSensitivity() => sensitivity;

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

        //public float getState(int Side)
        //{
        //    switch (Side)
        //    {
        //        default:
        //            return getLeftState();
        //        case 2:
        //            return getRightState();
        //    }    
        //}

        //public void setState(int Side, float val)
        //{
        //    switch (Side)
        //    {
        //        case 1:
        //            setLeftState(val);
        //            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("teststateLeft", val);
        //            break;
        //        case 2:
        //            setRightState(val);
        //            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("teststateRight", val);
        //            break;
        //    }
        //}

        //VNyan Getters
        //public void getVNyanLeapMirror()
        //{
        //    setMirror(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LeapMirror"));
        //}
        //public void getVNyanLeapState()
        //{
        //    setLeftState(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixerStateL"));
        //    setRightState(VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("LZ_LeapFixerStateR"));
        //}
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="status"></param>
        /// <param name="BoneList"></param>
        public void LeapStateProcess(float state, float status, List<int> BoneList, Dictionary<int, VNyanQuaternion> VNyanBoneRotations)
        {
            switch (state)
            {
                case 0f:
                    setCurrentBones(BoneList, VNyanBoneRotations);
                    break;

                case 1f:
                    updateLastLeapBones(status, BoneList);
                    setTargetBones(BoneList, VNyanBoneRotations);
                    rotateTowardsTarget(BoneList, getSlerpAmount(), 0f);
                    break;

                case 2f:
                    if (status == 1f)
                    {
                        setTargetBones(BoneList, getLastLeapBones());
                    }
                    rotateTowardsTarget(BoneList, getSlerpAmount2(), 0f);
                    break;

                case 3f:
                    updateLastLeapBones(status, BoneList);
                    setTargetBones(BoneList, VNyanBoneRotations);
                    rotateTowardsTarget(BoneList, getSlerpAmount2(), 0f);
                    break;
            }
        }

        public void LeapStateLeft()
        {
            switch (getLeftState())
            {
                case 0f:
                    if (getLeftStatus() == 1f)
                    {
                        // If status is now on, change state to "On"
                        setLeftState(1);
                    }
                    break;
                case 1f:

                    if (getVNyanLeftStatusTrack() >= getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        setLeftState(2);
                    }
                    else if (getLeftStatus() == 0f)
                    {
                        // If status is lost, change state to "Off"
                        setLeftState(0);
                    }
                    break;
                case 2f:
                    increaseLeftStatusTimer(Time.deltaTime);
                    if (getLeftStatusTimer() >= getTimeout())
                    {
                        // if we are past our timer amount 
                        resetLeftStatusTimer();
                        // when timer elapses
                        if (getVNyanLeftStatusTrack() < getSensitivity())
                        {
                            setLeftState(3);
                        }
                    }
                    break;

                case 3f:
                    increaseLeftStatusTimer(Time.deltaTime);
                    if (getLeftStatusTimer() >= getTimeout())
                    {
                        resetLeftStatusTimer();
                        if (getVNyanLeftStatusTrack() < 2f)
                        {
                            setLeftState(1);
                        }
                        else if (getVNyanLeftStatusTrack() >= getSensitivity())
                        {
                            setLeftState(2);
                        }
                    }
                    break;
            }
        }

        public void LeapStateRight()
        {
            switch (getRightState())
            {
                case 0f:
                    if (getRightStatus() == 1f)
                    {
                        // If status is now on, change state to "On"
                        setRightState(1);
                    }
                    break;

                case 1f:
                    if (getVNyanRightStatusTrack() >= getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        setRightState(2);
                    }
                    if (getRightStatus() == 0f)
                    {
                        // If status is lost, change state to "Off"
                        setRightState(0);
                    }
                    break;

                case 2f:
                    increaseRightStatusTimer(Time.deltaTime);
                    if (getRightStatusTimer() >= getTimeout())
                    {
                        // if we are past our timer amount 
                        resetRightStatusTimer();
                        // when timer elapses
                        if (getVNyanRightStatusTrack() < getSensitivity())
                        {
                            setRightState(3);
                        }
                    }
                    break;

                case 3f:
                    increaseRightStatusTimer(Time.deltaTime);
                    if (getRightStatusTimer() >= getTimeout())
                    {
                        resetRightStatusTimer();
                        if (getVNyanRightStatusTrack() < 2f)
                        {
                            setRightState(1);
                        }
                        else if (getVNyanRightStatusTrack() >= getSensitivity())
                        {
                            setRightState(2);
                        }
                    }
                    break;
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

        public class LeftState : State
        {
            public override float getSensitivity() => settings.getSensitivity();
            public override float getState() => settings.getLeftState();
            public override float getStatus() => settings.getLeftStatus();
            public override float getStatusTimer() => settings.getLeftStatusTimer();
            public override float getStatusTrack() => settings.getVNyanLeftStatusTrack();
            public override float getTimeout() => settings.getTimeout();
            public override void increaseStatusTimer(float time) => settings.increaseLeftStatusTimer(Time.deltaTime);
            public override void resetStatusTimer() => settings.resetLeftStatusTimer();
            public override void setState(float state) => settings.setLeftState(state);
        }

        public class RightState : State
        {
            public override float getSensitivity() => settings.getSensitivity();
            public override float getState() => settings.getRightState();
            public override float getStatus() => settings.getRightStatus();
            public override float getStatusTimer() => settings.getRightStatusTimer();
            public override float getStatusTrack() => settings.getVNyanRightStatusTrack();
            public override float getTimeout() => settings.getTimeout();
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

            // getSettings().getVNyanLeapMirror();
            // getSettings().getVNyanLeapState();
            getSettings().getVNyanLeapStatus();

            // getSettings().LeapStateLeft();
            // getSettings().LeapStateRight();

            LeftStateManager.ProcessState();
            LeftStateManager.setStateVNyan("LZ_LeapFixer_LeftStateManager");

            RightStateManager.ProcessState();
            RightStateManager.setStateVNyan("LZ_LeapFixer_RightStateManager");

            

            getSettings().LeapStateProcess(getSettings().getLeftState(), getSettings().getLeftStatus(), LeftArmBones, BoneRotations);
            getSettings().LeapStateProcess(getSettings().getRightState(), getSettings().getRightStatus(), RightArmBones, BoneRotations);

            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("leapfixer_lefttimer", getSettings().getLeftStatusTimer());
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("leapfixer_righttimer", getSettings().getRightStatusTimer());

            // Apply bones if they exist in bonerotations
            updateBoneRotations(LeftArmBones);
            updateBoneRotations(RightArmBones);
        }
    }
}