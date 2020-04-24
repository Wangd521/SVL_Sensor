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
using Simulator.Bridge.Ros;
using System.Collections.Generic;

namespace Simulator.Sensors
{
    [SensorType("Lane Following", new System.Type[] { })]
    public class LaneFollowingSensor : SensorBase, IVehicleInputs
    {
        private float ADSteerInput = 0f;
        private double LastControlUpdate = 0f;

        public float SteerInput { get; private set; } = 0f;
        public float AccelInput { get; private set; } = 0f;
        public float BrakeInput { get; private set; } = 0f;

        private void Awake()
        {
            LastControlUpdate = SimulatorManager.Instance.CurrentTime;
        }

        public override void OnBridgeSetup(IBridge bridge)
        {
            bridge.AddReader<TwistStamped>(Topic, data =>
            {
                LastControlUpdate = SimulatorManager.Instance.CurrentTime;
                ADSteerInput = (float)data.twist.angular.x;
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
            if (SimulatorManager.Instance.CurrentTime - LastControlUpdate < 0.5f)
            {
                SteerInput = ADSteerInput;
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
