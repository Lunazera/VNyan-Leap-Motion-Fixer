using System;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;

namespace LZLeapMotionFixer
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
        private float sensitivity = 5f;

        [SerializeField] private string paramNameTimeout = "LZ_LeapFixer_Timeout";
        private float timeout = 800f;

        [SerializeField] private string paramNameTransitionTime = "LZ_LeapFixer_TransitionTime";
        private float transitionTime = 400f;

        [SerializeField] private string paramNameMirror = "LZ_LeapMirror";
        private float mirror = 0f;

        [Header("Smoothing Settings")]
        [SerializeField] private string paramNameSmoothing = "LZ_LeapFixer_Smoothing";
        private float smoothing = 40f;
        [SerializeField] private string paramNameSmoothingUnstable = "LZ_LeapFixer_SmoothingUnstable";
        private float smoothing2 = 80f;
        [Tooltip("New scale for Smoothing slider (rescales 0-100 slider -> value-0)")]
        [SerializeField] private float smoothingScale = 10f;

        [SerializeField] private string paramNameBoost = "LZ_LeapFixer_Boost";
        private float boost = 50f;
        [Tooltip("Scale for Boost slider (Should be small, like < 0.1f)")]
        [SerializeField] private float boostScale = 0.01f;

        private LeapFixerLayer zLeapFixer = new LeapFixerLayer();

        /// <summary>
        /// Access to Leap Fixer Layer settings. Runs the getSettings() method from LeapFixer.
        /// </summary>
        /// <returns>LeapFixerSettings settings</returns>
        public LeapFixerSettings getLayerSettings()
        {
            return zLeapFixer.getSettings();
        }

        /// <summary>
        /// Access to LeapFixerPlugin's instantiation of the leap fixer layer
        /// </summary>
        /// <returns></returns>
        public LeapFixerLayer getFixerLayer()
        {
            return zLeapFixer;
        }

        public void setInitialValue(string paramName, float value)
        {
            float checkValue = value;
            if (LZUIManager.getSettingsDict().TryGetValue(paramName, out string checkVal))
            {
                checkValue = Convert.ToSingle(value);
            }
            else
            {
                LZUIManager.addSettingsDictFloat(paramName, checkValue);
            }
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramName, checkValue);
        }

        /// <summary>
        /// Checks if current VNyan parameter is different than 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public bool checkForNewValue(string paramName, float currentValue)
        {
            return (!(currentValue == LZUIManager.getSettingsDictFloat(paramName)));
        }

        public void Start()
        {
            if (!Application.isEditor)
            {
                // Register pose layer
                VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(zLeapFixer);

                setInitialValue(paramNameLayerActive, LayerActive);
                setInitialValue(paramNameTimeout, timeout);
                setInitialValue(paramNameTransitionTime, transitionTime);
                setInitialValue(paramNameSensitivity, sensitivity);
                setInitialValue(paramNameSmoothing, smoothing);
                setInitialValue(paramNameSmoothingUnstable, smoothing2);
                setInitialValue(paramNameBoost, boost);
                setInitialValue(paramNameMirror, mirror);
            }

            getLayerSettings().setLayerOnOff(LayerActive);
            getLayerSettings().setTimeout(timeout);
            getLayerSettings().setTransitionTime(transitionTime);
            getLayerSettings().setSensitivity(sensitivity);
            getLayerSettings().setSlerpAmount(smoothing, smoothingScale);
            getLayerSettings().setSlerpAmount2(smoothing2, smoothingScale);
            getLayerSettings().setSlerpBoost(boost, boostScale);
            getLayerSettings().setMirror(mirror);
        }

        public void Update()
        {
            if (!Application.isEditor)
            {
                // Parameter management //
                // Layer Toggle

                if (checkForNewValue(paramNameMirror, mirror))
                {
                    mirror = LZUIManager.getSettingsDictFloat(paramNameMirror);
                    getLayerSettings().setMirror(mirror);
                }

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

                if (checkForNewValue(paramNameTransitionTime, transitionTime))
                {
                    transitionTime = LZUIManager.getSettingsDictFloat(paramNameTransitionTime);
                    getLayerSettings().setTransitionTime(transitionTime);
                }

                if (checkForNewValue(paramNameSensitivity, sensitivity))
                {
                    sensitivity = LZUIManager.getSettingsDictFloat(paramNameSensitivity);
                    getLayerSettings().setSensitivity(sensitivity);
                }

                if (checkForNewValue(paramNameSmoothing, smoothing))
                {
                    smoothing = LZUIManager.getSettingsDictFloat(paramNameSmoothing);
                    getLayerSettings().setSlerpAmount(smoothing, smoothingScale);
                }

                if (checkForNewValue(paramNameSmoothingUnstable, smoothing2))
                {
                    smoothing2 = LZUIManager.getSettingsDictFloat(paramNameSmoothingUnstable);
                    getLayerSettings().setSlerpAmount2(smoothing2, smoothingScale);
                }

                if (checkForNewValue(paramNameBoost, boost))
                {
                    boost = LZUIManager.getSettingsDictFloat(paramNameBoost);
                    getLayerSettings().setSlerpBoost(boost, boostScale);
                }
            }
        }
    }
}
