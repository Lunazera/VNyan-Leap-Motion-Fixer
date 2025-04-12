using System;
using VNyanInterface;
using UnityEngine;

namespace Leap_Motion_Fixer
{
    public class LeapFixerPlugin : MonoBehaviour
    {
        IPoseLayer LeapFixer = new LeapFixerLayer();

        public string paramNameLayerActive = "LZ_LeapFixerActive";
        private float LayerActive = 1f;
        private float LayerActive_new = 1f;

        public string paramNameSpeed = "LZ_LeapFixerSpeed";
        private float Speed = 10f;
        private float Speed_new = 10f;

        private bool stateLeftTimeout = false;
        private bool stateRightTimeout = false;

        private float stateTimeout = 500;

        private float timeSinceLostRight = 0f;
        private float timeSinceLostLeft = 0f;


        public void Start()
        {
            VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(LeapFixer);

            // Parameter management //
            // Layer Toggle
            if (LZ_UI.settings.ContainsKey(paramNameLayerActive))
            {
                LayerActive = Convert.ToSingle(LZ_UI.settings[paramNameLayerActive]);
            }
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameLayerActive, LayerActive);

            // Speed, Slerp Amount
            if (LZ_UI.settings.ContainsKey(paramNameSpeed))
            {
                Speed = Convert.ToSingle(LZ_UI.settings[paramNameSpeed]);
            }
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameSpeed, Speed);

            LeapFixerSettings.setLayerOnOff(0f);
        }

        public void Update()
        {

            // TODO 
            // Fix logic here, clean up and think through this a bit
            // we also want to switch the layer code to get the state info directly
            // maybe renamte these state vs status because its confusing
            if (stateLeftTimeout)
            {
                if ((Time.time - timeSinceLostLeft) > stateTimeout) // if we exceed our timeout
                {
                    if (LeapFixerSettings.getLeftStatus() == 0f) // if we find tracking again
                    {
                        LeapFixerSettings.setLeftState(1f);
                    }
                    else
                    {
                        LeapFixerSettings.setLeftState(0f);
                    }
                    stateLeftTimeout = false;
                }
            } 
            else
            {
                
                if (LeapFixerSettings.getLeftState() == 0f) // If current state is off
                {
                    // but leap motion is now detected
                    if (LeapFixerSettings.getLeftStatus() == 1f)
                    {
                        // turn state on
                        LeapFixerSettings.setLeftState(1f);
                    }
                    else
                    {
                        LeapFixerSettings.setLeftState(0f);
                    }
                }
                else if (LeapFixerSettings.getLeftState() == 1f) // if current state is on + Stable
                {
                    // but leap motion is lost
                    if (LeapFixerSettings.getLeftStatus() == 0f)
                    {
                        timeSinceLostLeft = Time.time; // record current timestamp for this side
                        stateLeftTimeout = true; // turn on timeout flag
                        LeapFixerSettings.setLeftState(2f); // turn on our unstable state
                    }
                }




                // If we ever lose left tracking status
                if (LeapFixerSettings.getLeftStatus() == 0f)
                {
                    timeSinceLostLeft = Time.time; // record current timestamp for this side
                    stateLeftTimeout = true; // turn on timeout flag
                    LeapFixerSettings.setLeftState(2f); // turn on our unstable state
                }
                else
                {
                    LeapFixerSettings.setLeftState(1f);
                }
            }

            if (stateRightTimeout)
            {
                if ((Time.time - timeSinceLostRight) > stateTimeout) // if we exceed our timeout
                {
                    if (LeapFixerSettings.getRightStatus() == 0f) // if we find tracking again
                    {
                        LeapFixerSettings.setRightState(1f);
                    }
                    else
                    {
                        LeapFixerSettings.setRightState(0f);
                    }
                    stateRightTimeout = false;
                }
            }
            else
            {
                // If we ever lose Right tracking status
                if (LeapFixerSettings.getRightStatus() == 0f)
                {
                    timeSinceLostRight = Time.time; // record current timestamp for this side
                    stateRightTimeout = true; // turn on timeout flag
                    LeapFixerSettings.setRightState(2f); // turn on our unstable state
                }
                else
                {
                    LeapFixerSettings.setRightState(1f);
                }
            }

            // Parameter management //
            // Layer Toggle
            LayerActive_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameLayerActive);
            if (!(LayerActive_new == LayerActive))
            {
                LayerActive = LayerActive_new;
                LeapFixerSettings.setLayerOnOff(LayerActive);
            }
            // Speed, Slerp Amount
            Speed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameSpeed);
            if (!(Speed_new == Speed))
            {
                Speed = Speed_new;
                LeapFixerSettings.setSlerpAmount(Speed);
            }
        }
    }
}
