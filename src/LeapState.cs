using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    public abstract class State : ILeapState
    {
        public abstract float getSensitivity();
        public abstract float getState();
        public abstract float getStatus();
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

        public void ProcessState()
        {
            switch (getState())
            {
                case 0f:
                    if (getStatus() == 1f)
                    {
                        // If status is now on, change state to "On"
                        setState(1);
                    }
                    break;
                case 1f:

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
