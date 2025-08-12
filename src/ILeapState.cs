using System;
using System.Collections.Generic;
using System.Text;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    public interface ILeapState
    {
        public List<int> getBoneList();

        public float getState();
        public float getStatus();
        public float getStatusTrack();
        public float getStatusTimer();
        public float getTimeout();
        public float getSensitivity();

        public float getSlerp();
        public float getSlerpUnstable();
        public float getSlerpBoost();

        public void increaseStatusTimer(float time);
        public void resetStatusTimer();
        public void setState(float state);

        public void setStateVNyan(string ParamName) { }
        public void ManageState(PoseLayerFrame Frame) { }

        public void setCurrentDict() { }
        public void setTargetDict() { }
        public void setTargetDictLastLeap() { }
        public void updateLastLeapDict() { }
        public void rotateTowardsTarget() { }

        public void OffState(PoseLayerFrame Frame);
        public void OnOffTransitionState(PoseLayerFrame Frame);
        public void OnState(PoseLayerFrame Frame);
        public void UnstableState(PoseLayerFrame Frame);
        public void RecoveryState(PoseLayerFrame Frame);
    }
}
