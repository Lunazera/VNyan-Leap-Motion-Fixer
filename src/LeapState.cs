using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    public abstract class State : ILeapState
    {
        public abstract List<int> getBoneList();
        public abstract float getState();
        public abstract float getStatus();
        public abstract float getSensitivity();

        public abstract float getSlerp();
        public abstract float getSlerpUnstable();
        public abstract float getSlerpBoost();

        public abstract float getStatusTimer();
        public abstract float getStatusTrack();
        public abstract float getTimeout();

        public abstract void increaseStatusTimer(float time);
        public abstract void resetStatusTimer();
        public abstract void setState(float state);

        public void setStateVNyan(string ParamName) 
        { 
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(ParamName, getState());
        }

        public virtual void setCurrentDict(PoseLayerFrame Frame) { }
        public virtual void setTargetDict(PoseLayerFrame Frame) { }
        public virtual void setTargetDictLastLeap() { }
        public virtual void updateLastLeapDict() { }
        public virtual void rotateTowardsTarget(float slerp) { }

        // States

        public void OffState(PoseLayerFrame Frame)
        {
            setCurrentDict(Frame);
        }

        public virtual void OnOffTransitionState(PoseLayerFrame Frame) { }

        public void OnState(PoseLayerFrame Frame)
        {
            updateLastLeapDict();
            setTargetDict(Frame);
            rotateTowardsTarget(getSlerp());
        }

        public void UnstableState(PoseLayerFrame Frame)
        {
            if (getStatus() == 1f)
            {
                setTargetDictLastLeap();
            }
            rotateTowardsTarget(getSlerpUnstable());
        }

        public void RecoveryState(PoseLayerFrame Frame)
        {
            updateLastLeapDict();
            setTargetDict(Frame);
            rotateTowardsTarget(getSlerpUnstable());
        }

        // State manager

        public void ManageState(PoseLayerFrame Frame)
        {
            switch (getState())
            {
                case 0f:
                    OffState(Frame);
                    if (getStatus() == 1f)
                    {
                        // If status is now on, change state to "On"
                        setState(1);
                    }
                    break;
                case 1f:
                    OnState(Frame);
                    if (getStatusTrack() >= getSensitivity())
                    {
                        // If our status tracker has gone above our sensitivity, we'll move to our unstable state
                        setState(2);
                    }
                    else if (getStatus() == 0f)
                    {
                        // If status is lost, change state to "Off"
                        setState(0);
                    }
                    break;
                case 2f:
                    UnstableState(Frame);
                    increaseStatusTimer(Time.deltaTime);
                    if (getStatusTrack() >= getTimeout())
                    {
                        // if we are past our timer amount 
                        resetStatusTimer();
                        // when timer elapses
                        if (getStatusTrack() < getSensitivity())
                        {
                            setState(3);
                        }
                    }
                    break;

                case 3f:
                    RecoveryState(Frame);
                    increaseStatusTimer(Time.deltaTime);
                    if (getStatusTrack() >= getTimeout())
                    {
                        resetStatusTimer();
                        if (getStatusTrack() < 2f)
                        {
                            setState(1);
                        }
                        else if (getStatusTrack() >= getSensitivity())
                        {
                            setState(2);
                        }
                    }
                    break;
            }
        }
    }
}
