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
        [Header("Main Settings")]
        [SerializeField] private string paramNameLayerActive = "LZ_LeapFixerActive";
        private float LayerActive = 1f;

        [SerializeField] private string paramNameSensitivity = "LZ_LeapFixer_Sensitivity";
        private float sensitivity = 1f;

        [SerializeField] private string paramNameTimeout = "LZ_LeapFixer_Timeout";
        private float timeout = 500f;

        [Header("Smoothing Settings")]
        [SerializeField] private string paramNameSmoothing = "LZ_LeapFixer_Smoothing";
        private float smoothing = 10f;

        [SerializeField] private string paramNameSmoothingUnstable = "LZ_LeapFixer_SmoothingUnstable";
        private float smoothing2 = 10f;

        [SerializeField] private string paramNameBoost = "LZ_LeapFixer_Boost";
        private float boost = 10f;
       
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
                setInitialValue(paramNameTimeout, timeout);
                setInitialValue(paramNameSensitivity, sensitivity);
                setInitialValue(paramNameSmoothing, smoothing);
                setInitialValue(paramNameSmoothingUnstable, smoothing2);
                setInitialValue(paramNameBoost, boost);
            }

            LeapFixerLayer.settings.setLayerOnOff(LayerActive);
            LeapFixerLayer.settings.setTimeout(timeout);
            LeapFixerLayer.settings.setSensitivity(sensitivity);
            LeapFixerLayer.settings.setSlerpAmount(smoothing);
            LeapFixerLayer.settings.setSlerpAmount2(smoothing2);
            LeapFixerLayer.settings.setSlerpBoost(boost);
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
                if (checkForNewValue(paramNameTimeout, timeout))
                {
                    timeout = LZUIManager.getSettingsDictFloat(paramNameTimeout);
                    getLayerSettings().setTimeout(timeout);
                }

                if (checkForNewValue(paramNameSensitivity, sensitivity))
                {
                    sensitivity = LZUIManager.getSettingsDictFloat(paramNameSensitivity);
                    getLayerSettings().setSensitivity(sensitivity);
                }

                if (checkForNewValue(paramNameSmoothing, smoothing))
                {
                    smoothing = LZUIManager.getSettingsDictFloat(paramNameSmoothing);
                    getLayerSettings().setSlerpAmount(smoothing);
                }

                if (checkForNewValue(paramNameSmoothingUnstable, smoothing2))
                {
                    smoothing2 = LZUIManager.getSettingsDictFloat(paramNameSmoothingUnstable);
                    getLayerSettings().setSlerpAmount2(smoothing2);
                }

                if (checkForNewValue(paramNameBoost, boost))
                {
                    boost = LZUIManager.getSettingsDictFloat(paramNameBoost);
                    getLayerSettings().setTimeout(boost);
                }
            }
        }
    }
}
