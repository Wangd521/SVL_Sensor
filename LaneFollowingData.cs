/**
 * Copyright (c) 2020 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 */

using Simulator.Bridge;
using Simulator.Bridge.Ros.Ros;

namespace Simulator.Sensors
{
    public class LaneFollowingData
    {
        public string Name;
        public string Frame;
        public double Time;
        public uint Sequence;
        public float? SteerInput;
    }

    public class LaneFollowingDataBridgePlugin : ISensorBridgePlugin
    {
        public void Register(IBridgePlugin plugin)
        {
            if (plugin.Factory is Bridge.Ros.RosBridgeFactoryBase)
            {
                plugin.Factory.RegPublisher(plugin,
                    (LaneFollowingData data) =>
                        new TwistStamped()
                        {
                            header = new Header()
                            {
                                stamp = Bridge.Ros.Conversions.ConvertTime(data.Time),
                                seq = data.Sequence,
                                frame_id = data.Frame,
                            },
                            twist = new Twist()
                            {
                                linear = new Vector3()
                                {
                                    x = 0.0,
                                    y = 0.0,
                                    z = 0.0,
                                },
                                angular = new Vector3()
                                {
                                    x = data.SteerInput.GetValueOrDefault(),
                                    y = 0.0,
                                    z = 0.0,
                                }
                            }
                        }
                );
            }
        }
    }
}
