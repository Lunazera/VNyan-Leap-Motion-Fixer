using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    public abstract class State : ILeapState
    {
        // Getters
        public abstract List<int> getBoneList();
        public abstract float getState();
        public abstract float getStatus();
        public abstract float getSensitivity();

        public abstract float getSlerp();
        public abstract float getSlerpUnstable();
        public abstract float getSlerpBoost();

        public abstract float getStatusTimer();
        public abstract float getStatusTrack();

        // Timing Management
        public abstract float getTimeout();
        public abstract void increaseStatusTimer(float time);
        public abstract void resetStatusTimer();
        public abstract float getTransitionTime();
        

        // Dictionary Management

        public virtual void setCurrentDict(PoseLayerFrame Frame) { }
        public virtual void setTargetDict(PoseLayerFrame Frame) { }
        public virtual void setTargetDictLastLeap() { }
        public virtual void updateLastLeapDict() { }
        public virtual void rotateTowardsTarget() { }


        // States

        public void OffState(PoseLayerFrame Frame)
        {
            setCurrentDict(Frame);
        }

        public virtual void OnStateTransition(PoseLayerFrame Frame) 
        {
            updateLastLeapDict();
            setTargetDict(Frame);
            rotateTowardsTarget();
        }

        public virtual void OffStateTransition(PoseLayerFrame Frame)
        {
            updateLastLeapDict();
            setTargetDict(Frame);
            rotateTowardsTarget();
        }

        public void OnState(PoseLayerFrame Frame)
        {
            updateLastLeapDict();
            setTargetDict(Frame);
            rotateTowardsTarget();
        }

        public void UnstableState(PoseLayerFrame Frame)
        {
            if (getStatus() == 1f)
            {
                setTargetDictLastLeap();
            }
            rotateTowardsTarget();
        }

        public void RecoveryState(PoseLayerFrame Frame)
        {
            updateLastLeapDict();
            setTargetDict(Frame);
            rotateTowardsTarget();
        }


        // State Management

        public abstract void setState(float state);

        public void setStateVNyan(string ParamName)
        {
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(ParamName, getState());
        }

        public void ManageState(PoseLayerFrame Frame)
        {
            switch (getState())
            {
                case 0f: // Off
                    resetStatusTimer();
                    if (getStatus() == 1f)
                    {
                        // If status is now on, change state to "On"
                        resetStatusTimer();
                        setState(4f);
                    }
                    OffState(Frame);
                    break;

                case 1f: // Stable
                    resetStatusTimer();
                    if (getStatusTrack() >= getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        setState(2f);
                    }
                    else if (getStatus() == 0f)
                    {
                        // If status is lost, change state to "Off"
                        resetStatusTimer();
                        setState(5f);
                    }
                    OnState(Frame);
                    break;

                case 2f: // Unstable
                    UnstableState(Frame);
                    increaseStatusTimer(Time.deltaTime);
                    if (getStatusTimer() >= getTimeout())
                    {
                        // if we are past our timer amount 
                        resetStatusTimer();
                        // when timer elapses
                        if (getStatusTrack() < getSensitivity())
                        {
                            setState(3f);
                        }
                    }
                    break;

                case 3f: // Recovery
                    RecoveryState(Frame);
                    increaseStatusTimer(Time.deltaTime);
                    if (getStatusTimer() >= getTimeout())
                    {
                        resetStatusTimer();
                        if (getStatusTrack() < 2f)
                        {
                            setState(1f);
                        }
                        else if (getStatusTrack() >= getSensitivity())
                        {
                            setState(2f);
                        }
                    }
                    break;

                case 4f: // Transition to On
                    OnStateTransition(Frame);
                    increaseStatusTimer(Time.deltaTime);
                    if (getStatusTrack() >= getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        resetStatusTimer();
                        setState(2f);
                    }
                    else if (getStatus() == 0f)
                    {
                        resetStatusTimer();
                        setState(5f);
                    }
                    else if (getStatusTimer() >= getTransitionTime())
                    {
                        if (getStatus() == 1f)
                        {
                            setState(1f);
                        } 
                        else
                        {
                            setState(0f);
                        }
                    }
                    break;

                case 5f: // Transition to Off
                    OffStateTransition(Frame);
                    increaseStatusTimer(Time.deltaTime);
                    if (getStatusTrack() >= getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        resetStatusTimer();
                        setState(2f);
                    } 
                    else if (getStatus() == 1f)
                    {
                        resetStatusTimer();
                        setState(4f);
                    }
                    else if (getStatusTimer() >= getTransitionTime())
                    {
                        if (getStatus() == 0f)
                        {
                            setState(0f);
                        }
                        else
                        {
                            setState(1f);
                        }
                    }
                    break;

            }
        }
    }
}
