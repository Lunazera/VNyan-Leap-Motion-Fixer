using System;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;

namespace Leap_Motion_Fixer
{
    /// <summary>
    /// Maintains main plugin parameters and settings
    /// </summary>
    class LeapFixerPlugin : MonoBehaviour
    {
        [Header("Plugin Settings")]
        [SerializeField] private string paramNameLayerActive = "LZ_LeapFixerActive";
        [SerializeField] private float LayerActive = 1f;

        [SerializeField] private string paramNameSpeed = "LZ_LeapFixer_Speed";
        [SerializeField] private float Speed = 10f;

        [SerializeField] private string paramNameSpeed2 = "LZ_LeapFixer_Speed2";
        [SerializeField] private float Speed2 = 10f;

        [SerializeField] private string paramNameTimeout = "LZ_LeapFixer_Timeout";
        [SerializeField] private float stateTimeout = 500f;

        private float StatusL;
        private float StatusR;

        private bool stateLeftTimeout = false;
        private bool stateRightTimeout = false;

        private float timeSinceLostRight = 0f;
        private float timeSinceLostLeft = 0f;

        private static LeapFixerLayer LeapFixer = new LeapFixerLayer();


        /// <summary>
        /// Access to Leap Fixer Layer settings. Runs the getSettings() method from LeapFixer.
        /// </summary>
        /// <returns>LeapFixerSettings settings</returns>
        public static LeapFixerSettings getLayerSettings()
        {
            return LeapFixer.getSettings();
        }

        /// <summary>
        /// Access to LeapFixerPlugin's instantiation of the leap fixer layer
        /// </summary>
        /// <returns></returns>
        public static LeapFixerLayer getFixerLayer()
        {
            return LeapFixer;
        }

        public static void setInitialValue(string paramName, float value)
        {
            float checkValue = value;
            checkValue = LZUIManager.getSettingsDictFloat(paramName, value);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramName, checkValue);
        }

        /// <summary>
        /// Checks if current VNyan parameter is different than 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static bool checkForNewValue(string paramName, float currentValue)
        {
            return (!(currentValue == LZUIManager.getSettingsDictFloat(paramName)));
        }


        public void Start()
        {
            if (!Application.isEditor)
            {
                // Register pose layer
                VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(LeapFixer);

                setInitialValue(paramNameLayerActive, LayerActive);
                setInitialValue(paramNameSpeed, Speed);
                setInitialValue(paramNameSpeed2, Speed2);
            }

            LeapFixerLayer.settings.setLayerOnOff(LayerActive);
            LeapFixerLayer.settings.setSlerpAmount(Speed);
            LeapFixerLayer.settings.setSlerpAmount2(Speed2);
        }

        public void Update()
        {
            if (!Application.isEditor)
            {
                // Parameter management //
                // Layer Toggle

                if (checkForNewValue(paramNameLayerActive, LayerActive))
                {
                    LayerActive = LZUIManager.getSettingsDictFloat(paramNameLayerActive);
                    getLayerSettings().setLayerOnOff(LayerActive);
                }

                if (checkForNewValue(paramNameSpeed, Speed))
                {
                    Speed = LZUIManager.getSettingsDictFloat(paramNameSpeed);
                    getLayerSettings().setSlerpAmount(Speed);
                }

                if (checkForNewValue(paramNameSpeed2, Speed2))
                {
                    Speed2 = LZUIManager.getSettingsDictFloat(paramNameSpeed2);
                    getLayerSettings().setSlerpAmount2(Speed2);
                }

                if (checkForNewValue(paramNameTimeout, stateTimeout))
                {
                    stateTimeout = LZUIManager.getSettingsDictFloat(paramNameTimeout);
                    //getLayerSettings().setSlerpAmount2(stateTimeout);
                }
            }
        }
    }
}
