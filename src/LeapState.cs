using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using UnityEngine;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    public abstract class State : ILeapState
    {
        // Getters
        //public abstract LeapFixerSettings getSettings();

        public abstract List<int> getBoneList(LeapFixerSettings settings);
        public abstract float getState(LeapFixerSettings settings);
        public abstract float getStatus(LeapFixerSettings settings);

        public abstract float getStatusTimer(LeapFixerSettings settings);
        public abstract float getStatusTrack(LeapFixerSettings settings);

        // Timing Management
        public abstract void increaseStatusTimer(LeapFixerSettings settings, float time);
        public abstract void resetStatusTimer(LeapFixerSettings settings);

        // States

        public void OffState(LeapFixerSettings settings, PoseLayerFrame Frame)
        {
            settings.setCurrentBones(getBoneList(settings), Frame.BoneRotation);
        }

        public void OnStateTransition(LeapFixerSettings settings, PoseLayerFrame Frame) 
        {
            settings.updateLastLeapBones(getStatus(settings), getBoneList(settings));
            settings.setTargetBones(getBoneList(settings), Frame.BoneRotation);
            settings.rotateTowardsTarget(getBoneList(settings), settings.getSlerpAmount2(), settings.getSlerpBoost());
        }

        public void OffStateTransition(LeapFixerSettings settings, PoseLayerFrame Frame)
        {
            settings.updateLastLeapBones(getStatus(settings), getBoneList(settings));
            settings.setTargetBones(getBoneList(settings), Frame.BoneRotation);
            settings.rotateTowardsTarget(getBoneList(settings), settings.getSlerpAmount2(), settings.getSlerpBoost());
        }

        public void OnState(LeapFixerSettings settings, PoseLayerFrame Frame)
        {
            settings.updateLastLeapBones(getStatus(settings), getBoneList(settings));
            settings.setTargetBones(getBoneList(settings), Frame.BoneRotation);
            settings.rotateTowardsTarget(getBoneList(settings), settings.getSlerpAmount(), settings.getSlerpBoost());
        }

        public void UnstableState(LeapFixerSettings settings, PoseLayerFrame Frame)
        {
            if (getStatus(settings) == 1f)
            {
                settings.setTargetBones(getBoneList(settings), settings.getLastLeapBones());
            }
            settings.rotateTowardsTarget(getBoneList(settings), settings.getSlerpAmount2(), 0f);
        }

        public void RecoveryState(LeapFixerSettings settings, PoseLayerFrame Frame)
        {
            settings.updateLastLeapBones(getStatus(settings), getBoneList(settings));
            settings.setTargetBones(getBoneList(settings), Frame.BoneRotation);
            settings.rotateTowardsTarget(getBoneList(settings), settings.getSlerpAmount2(), settings.getSlerpBoost());
        }


        // State Management

        public abstract void setState(LeapFixerSettings settings, float state);

        public void setStateVNyan(LeapFixerSettings settings, string ParamName)
        {
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(ParamName, getState(settings));
        }

        public void ManageState(LeapFixerSettings settings, PoseLayerFrame Frame)
        {
            switch (getState(settings))
            {
                case 0f: // Off
                    resetStatusTimer(settings);
                    if (getStatus(settings) == 1f)
                    {
                        // If status is now on, change state to "On"
                        resetStatusTimer(settings);
                        setState(settings, 4f);
                    }
                    OffState(settings, Frame);
                    break;

                case 1f: // Stable
                    resetStatusTimer(settings);
                    if (getStatusTrack(settings) >= settings.getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        setState(settings, 2f);
                    }
                    else if (getStatus(settings) == 0f)
                    {
                        // If status is lost, change state to "Off"
                        resetStatusTimer(settings);
                        setState(settings, 5f);
                    }
                    OnState(settings, Frame);
                    break;

                case 2f: // Unstable
                    UnstableState(settings, Frame);
                    increaseStatusTimer(settings, Time.deltaTime);
                    if (getStatusTimer(settings) >= settings.getTimeout())
                    {
                        // if we are past our timer amount 
                        resetStatusTimer(settings);
                        // when timer elapses
                        if (getStatusTrack(settings) < settings.getSensitivity())
                        {
                            setState(settings, 3f);
                        }
                    }
                    break;

                case 3f: // Recovery
                    RecoveryState(settings, Frame);
                    increaseStatusTimer(settings, Time.deltaTime);
                    if (getStatusTimer(settings) >= settings.getTimeout())
                    {
                        resetStatusTimer(settings);
                        if (getStatusTrack(settings) < 2f)
                        {
                            setState(settings, 1f);
                        }
                        else if (getStatusTrack(settings) >= settings.getSensitivity())
                        {
                            setState(settings, 2f);
                        }
                    }
                    break;

                case 4f: // Transition to On
                    OnStateTransition(settings, Frame);
                    increaseStatusTimer(settings, Time.deltaTime);
                    if (getStatusTrack(settings) >= settings.getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        resetStatusTimer(settings);
                        setState(settings, 2f);
                    }
                    else if (getStatus(settings) == 0f)
                    {
                        resetStatusTimer(settings);
                        setState(settings, 5f);
                    }
                    else if (getStatusTimer(settings) >= settings.getTransitionTime())
                    {
                        if (getStatus(settings) == 1f)
                        {
                            setState(settings, 1f);
                        } 
                        else
                        {
                            setState(settings, 0f);
                        }
                    }
                    break;

                case 5f: // Transition to Off
                    OffStateTransition(settings, Frame);
                    increaseStatusTimer(settings, Time.deltaTime);
                    if (getStatusTrack(settings) >= settings.getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        resetStatusTimer(settings);
                        setState(settings, 2f);
                    } 
                    else if (getStatus(settings) == 1f)
                    {
                        resetStatusTimer(settings);
                        setState(settings, 4f);
                    }
                    else if (getStatusTimer(settings) >= settings.getTransitionTime())
                    {
                        if (getStatus(settings) == 0f)
                        {
                            setState(settings, 0f);
                        }
                        else
                        {
                            setState(settings, 1f);
                        }
                    }
                    break;

            }
        }
    }

    public class LeftState : State
    {
        public override List<int> getBoneList(LeapFixerSettings settings) => settings.getLeftArmBones();
        public override float getState(LeapFixerSettings settings) => settings.getLeftState();
        public override float getStatus(LeapFixerSettings settings) => settings.getLeftStatus();
        public override float getStatusTimer(LeapFixerSettings settings) => settings.getLeftStatusTimer();
        public override float getStatusTrack(LeapFixerSettings settings) => settings.getVNyanLeftStatusTrack();
        public override void increaseStatusTimer(LeapFixerSettings settings, float time) => settings.increaseLeftStatusTimer(Time.deltaTime);
        public override void resetStatusTimer(LeapFixerSettings settings) => settings.resetLeftStatusTimer();
        public override void setState(LeapFixerSettings settings, float state) => settings.setLeftState(state);
    }

    public class RightState : State
    {
        public override List<int> getBoneList(LeapFixerSettings settings) => settings.getRightArmBones();
        public override float getState(LeapFixerSettings settings) => settings.getRightState();
        public override float getStatus(LeapFixerSettings settings) => settings.getRightStatus();
        public override float getStatusTimer(LeapFixerSettings settings) => settings.getRightStatusTimer();
        public override float getStatusTrack(LeapFixerSettings settings) => settings.getVNyanRightStatusTrack();
        public override void increaseStatusTimer(LeapFixerSettings settings, float time) => settings.increaseRightStatusTimer(Time.deltaTime);
        public override void resetStatusTimer(LeapFixerSettings settings) => settings.resetRightStatusTimer();
        public override void setState(LeapFixerSettings settings, float state) => settings.setRightState(state);
    }
}
