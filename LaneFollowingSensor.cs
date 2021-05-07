/**
 * Copyright (c) 2020-2021 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 */

using UnityEngine;
using Simulator.Bridge;
using Simulator.Utilities;
using Simulator.Sensors.UI;
using System.Collections.Generic;
using Simulator.Bridge.Data;

namespace Simulator.Sensors
{
    [SensorType("Lane Following", new [] { typeof(VehicleControlData) })]
    public class LaneFollowingSensor : SensorBase, IVehicleInputs
    {
        [SensorParameter]
        [Range(0f, 30f)]
        public float CruiseSpeed = 0f;

        [SensorParameter]
        [Range(0f, 2f)]
        public float SteeringSensitivity = 1f;

        [AnalysisMeasurement(MeasurementType.Velocity)]
        public float MaxSpeed = 0f;

        [AnalysisMeasurement(MeasurementType.Count)]
        public int ControlMessages = 0;

        [AnalysisMeasurement(MeasurementType.Input)]
        public float LargestSteerInput = 0f;

        [AnalysisMeasurement(MeasurementType.Duration)]
        public double AverageInferenceTime = 0f;

        private IVehicleDynamics Dynamics;
        private IAgentController Controller;

        public float SteerInput { get; private set; } = 0f;
        public float AccelInput { get; private set; } = 0f;
        public float BrakeInput { get; private set; } = 0f;

        private BridgeInstance Bridge;

        private float ADSteerInput = 0f;
        private float MaxSteerInput = 0f;
        private double InferenceTime = 0f;
        private double TotalInferenceTime = 0f;
        private double LastControlUpdate = 0f;

        private void Awake()
        {
            LastControlUpdate = SimulatorManager.Instance.CurrentTime;
            Dynamics = GetComponentInParent<IVehicleDynamics>();
            Controller = GetComponentInParent<IAgentController>();
        }

        public override void OnBridgeSetup(BridgeInstance bridge)
        {
            Bridge = bridge;
            Bridge.AddSubscriber<VehicleControlData>(Topic, data =>
            {
                if (Time.timeScale == 0f)
                    return;

                LastControlUpdate = SimulatorManager.Instance.CurrentTime;
                ADSteerInput = data.SteerInput.GetValueOrDefault();
                InferenceTime = data.TimeStampSec.GetValueOrDefault();

                ControlMessages++;
                TotalInferenceTime += InferenceTime;
                AverageInferenceTime = TotalInferenceTime / (float)ControlMessages;
            });
        }

        public void Update()
        {
            if (SimulatorManager.Instance.CurrentTime - LastControlUpdate >= 0.5)
            {
                ADSteerInput = SteerInput = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (Bridge != null && Bridge.Status == Status.Connected)
            {
                if (SimulatorManager.Instance.CurrentTime - LastControlUpdate < 0.5f)
                {
                    if (MaxSteerInput < Mathf.Abs(ADSteerInput))
                    {
                        MaxSteerInput = Mathf.Abs(ADSteerInput);
                        LargestSteerInput = ADSteerInput;
                    }

                    SteerInput = ADSteerInput * SteeringSensitivity;
                }
            }

            if (Controller.AccelInput >= 0)
            {
                AccelInput = Dynamics.Velocity.magnitude < CruiseSpeed ? 1f : 0f;
            }

            MaxSpeed = Mathf.Max(MaxSpeed, Dynamics.Velocity.magnitude);
        }

        public override void OnVisualize(Visualizer visualizer)
        {
            Debug.Assert(visualizer != null);
            var graphData = new Dictionary<string, object>()
            {
                {"Cruise Speed", CruiseSpeed},
                {"Speed", Dynamics.Velocity.magnitude},
                {"AD Steer Input", ADSteerInput},
                {"Largest AD Steer Input", LargestSteerInput},
                {"Steering Sensitivity", SteeringSensitivity},
                {"Number of Control Messages", ControlMessages},
                {"Inference Time", InferenceTime},
                {"Average Inference Time", AverageInferenceTime},
                {"Last Control Update", LastControlUpdate},
            };

            visualizer.UpdateGraphValues(graphData);
        }

        public override void OnVisualizeToggle(bool state)
        {
            //
        }
    }
}
