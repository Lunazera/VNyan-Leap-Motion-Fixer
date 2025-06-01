using System;
using VNyanInterface;
using UnityEngine;
using System.Collections.Generic;

namespace Leap_Motion_Fixer
{
    public class LeapFixerPlugin : MonoBehaviour
    {
        /*
        public string PluginName { get; } = "LeapFixer";
        public string Version { get; } = "1.0";
        public string Title { get; } = "Leap Motion Fixer";
        public string Author { get; } = "Lunazera";
        public string Website { get; } = "https://github.com/Lunazera/Leap-Motion-Fixer";
        public void InitializePlugin()
        {
            Debug.Log("Initialized!");

        }
        */


        IPoseLayer LeapFixer = new LeapFixerLayer();

        public string paramNameLayerActive = "LZ_LeapFixerActive";
        private float LayerActive = 1f;
        private float LayerActive_new = 1f;

        public string paramNameSpeed = "LZ_LeapFixerSpeed";
        private float Speed = 10f;
        private float Speed_new = 10f;

        private bool stateLeftTimeout = false;
        private bool stateRightTimeout = false;

        private float stateTimeout = 500f;

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

            LeapFixerLayer.settings.setLayerOnOff(0f);
        }
        

        public void Update()
        {


            /*
            // TODO 
            // Fix logic here, clean up and think through this a bit
            // we also want to switch the layer code to get the state info directly
            // maybe renamte these state vs status because its confusing
            if (stateLeftTimeout)
            {
                if ((Time.time - timeSinceLostLeft) > stateTimeout) // if we exceed our timeout
                {
                    if (LeapFixerLayer.settings.getLeftStatus() == 0f) // if we find tracking again
                    {
                        LeapFixerLayer.settings.setLeftState(1f);
                    }
                    else
                    {
                        LeapFixerLayer.settings.setLeftState(0f);
                    }
                    stateLeftTimeout = false;
                }
            } 
            else
            {
                
                if (LeapFixerLayer.settings.getLeftState() == 0f) // If current state is off
                {
                    // but leap motion is now detected
                    if (LeapFixerLayer.settings.getLeftStatus() == 1f)
                    {
                        // turn state on
                        LeapFixerLayer.settings.setLeftState(1f);
                    }
                    else
                    {
                        LeapFixerLayer.settings.setLeftState(0f);
                    }
                }
                else if (LeapFixerLayer.settings.getLeftState() == 1f) // if current state is on + Stable
                {
                    // but leap motion is lost
                    if (LeapFixerLayer.settings.getLeftStatus() == 0f)
                    {
                        timeSinceLostLeft = Time.time; // record current timestamp for this side
                        stateLeftTimeout = true; // turn on timeout flag
                        LeapFixerLayer.settings.setLeftState(2f); // turn on our unstable state
                    }
                }




                // If we ever lose left tracking status
                if (LeapFixerLayer.settings.getLeftStatus() == 0f)
                {
                    timeSinceLostLeft = Time.time; // record current timestamp for this side
                    stateLeftTimeout = true; // turn on timeout flag
                    LeapFixerLayer.settings.setLeftState(2f); // turn on our unstable state
                }
                else
                {
                    LeapFixerLayer.settings.setLeftState(1f);
                }
            }

            if (stateRightTimeout)
            {
                if ((Time.time - timeSinceLostRight) > stateTimeout) // if we exceed our timeout
                {
                    if (LeapFixerLayer.settings.getRightStatus() == 0f) // if we find tracking again
                    {
                        LeapFixerLayer.settings.setRightState(1f);
                    }
                    else
                    {
                        LeapFixerLayer.settings.setRightState(0f);
                    }
                    stateRightTimeout = false;
                }
            }
            else
            {
                // If we ever lose Right tracking status
                if (LeapFixerLayer.settings.getRightStatus() == 0f)
                {
                    timeSinceLostRight = Time.time; // record current timestamp for this side
                    stateRightTimeout = true; // turn on timeout flag
                    LeapFixerLayer.settings.setRightState(2f); // turn on our unstable state
                }
                else
                {
                    LeapFixerLayer.settings.setRightState(1f);
                }
            }
            */

            // Parameter management //
            // Layer Toggle
            LayerActive_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameLayerActive);
            if (!(LayerActive_new == LayerActive))
            {
                LayerActive = LayerActive_new;
                LeapFixerLayer.settings.setLayerOnOff(LayerActive);
            }
            // Speed, Slerp Amount
            Speed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameSpeed);
            if (!(Speed_new == Speed))
            {
                Speed = Speed_new;
                LeapFixerLayer.settings.setSlerpAmount(Speed);
            }
        }
    }
}
