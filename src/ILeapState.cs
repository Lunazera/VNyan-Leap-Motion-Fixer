using System;
using System.Collections.Generic;
using System.Text;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    public interface ILeapState
    {
        public float getState();
        public float getStatus();
        public float getStatusTrack();
        public float getStatusTimer();
        public float getTimeout();
        public float getSensitivity();

        public void increaseStatusTimer(float time);
        public void resetStatusTimer();
        public void setState(float state);

        public void setStateVNyan(string ParamName) { }
        public void ProcessState() { }
    }
}
