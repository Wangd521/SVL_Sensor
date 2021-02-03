/**
 * Copyright (c) 2020 LG Electronics, Inc.
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
        public float SteerInput { get; private set; } = 0f;
        public float AccelInput { get; private set; } = 0f;
        public float BrakeInput { get; private set; } = 0f;

        private BridgeInstance Bridge;

        private float ADSteerInput = 0f;
        private double LastControlUpdate = 0f;

        private void Awake()
        {
            LastControlUpdate = SimulatorManager.Instance.CurrentTime;
        }

        public override void OnBridgeSetup(BridgeInstance bridge)
        {
            Bridge = bridge;
            Bridge.AddSubscriber<VehicleControlData>(Topic, data =>
            {
                LastControlUpdate = SimulatorManager.Instance.CurrentTime;
                ADSteerInput = data.SteerInput.GetValueOrDefault();
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
                    SteerInput = ADSteerInput;
                }
            }
        }

        public override void OnVisualize(Visualizer visualizer)
        {
            Debug.Assert(visualizer != null);
            var graphData = new Dictionary<string, object>()
            {
                {"AD Steer Input", ADSteerInput},
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
