using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    public interface ILeapState
    {
        public List<int> getBoneList(LeapFixerSettings settings);

        public float getState(LeapFixerSettings settings);
        public float getStatus(LeapFixerSettings settings);
        public float getStatusTrack(LeapFixerSettings settings);
        public float getStatusTimer(LeapFixerSettings settings);

        public void increaseStatusTimer(LeapFixerSettings settings, float time);
        public void resetStatusTimer(LeapFixerSettings settings);
        public void setState(LeapFixerSettings settings, float state);

        public void setStateVNyan(LeapFixerSettings settings, string ParamName) { }
        public void ManageState(LeapFixerSettings settings, PoseLayerFrame Frame) { }

        public void OffState(LeapFixerSettings settings, PoseLayerFrame Frame);
        public void OnStateTransition(LeapFixerSettings settings, PoseLayerFrame Frame);
        public void OffStateTransition(LeapFixerSettings settings, PoseLayerFrame Frame);
        public void OnState(LeapFixerSettings settings, PoseLayerFrame Frame);
        public void UnstableState(LeapFixerSettings settings, PoseLayerFrame Frame);
        public void RecoveryState(LeapFixerSettings settings,PoseLayerFrame Frame);
    }
}
