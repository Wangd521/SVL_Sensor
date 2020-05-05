/**
 * Copyright (c) 2020 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 */

using System;
using Simulator.Bridge;
using Simulator.Bridge.Ros;

namespace Simulator.Sensors
{
    public class LaneFollowingDataConverters : IDataConverter<LaneFollowingData>
    {
        public Func<LaneFollowingData, object> GetConverter(IBridge bridge)
        {
            if (bridge.GetType() == typeof(Bridge.Ros.Bridge))
            {
                return (data) =>
                {
                    return new TwistStamped()
                    {
                        header = new Header()
                        {
                            stamp = Conversions.ConvertTime(data.Time),
                            seq = data.Sequence,
                            frame_id = data.Frame,
                        },
                        twist = new Twist()
                        {
                            linear = new Simulator.Bridge.Ros.Vector3()
                            {
                                x = 0.0,
                                y = 0.0,
                                z = 0.0,
                            },
                            angular = new Simulator.Bridge.Ros.Vector3()
                            {
                                x = data.SteerInput.GetValueOrDefault(),
                                y = 0.0,
                                z = 0.0,
                            }
                        }
                    };
                };
            }

            throw new System.Exception("LaneFollowingSensor not implemented for this bridge type!");
        }

        public Type GetOutputType(IBridge bridge)
        {
            if (bridge.GetType() == typeof(Bridge.Ros.Bridge))
            {
                return typeof(TwistStamped);
            }

            throw new System.Exception("LaneFollowingSensor not implemented for this bridge type!");
        }
    }

    public class LaneFollowingData
    {
        public string Name;
        public string Frame;
        public double Time;
        public uint Sequence;
        public float? SteerInput;
    }

    static class Conversions
    {
        public static Simulator.Bridge.Ros.Time ConvertTime(double unixEpochSeconds)
        {
            long nanosec = (long)(unixEpochSeconds * 1e9);

            return new Simulator.Bridge.Ros.Time()
            {
                secs = nanosec / 1000000000,
                nsecs = (uint)(nanosec % 1000000000),
            };
        }
    }
}
