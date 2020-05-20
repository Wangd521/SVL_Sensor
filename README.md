# LaneFollowing Sensor

This repository contains the code and assets for a custom Sensor Plugin - LaneFollowing Sensor.

To use this Sensor Plugin:

1) clone the Sensor Plugin repo into Assets/External/Sensors as Assets/External/Sensors/LaneFollowingSensor inside of your Simulator Unity Project

2) build the Sensor Plugin for use with the Simulator, navigate to the `Simulator -> Build Sensors` Unity Editor menu item. Clicking on it will build every custom sensor in the Assets/External/Sensors directory and will output built Sensor Plugin bundles to the AssetBundles/Sensors folder

3) on simulation startup, the Simulator will load all custom Sensor Plugin bundles in AssetBundles/Sensors directory and will be a valid sensor in a vehicle's configuration JSON

4) add json configuration (see below) to vehicle of your choosing and launch simulation

LaneFollowing Sensor will subscribe to predicted steering commands from the model and publish EGO vehicle commands for data collection for training

# Parameters

Topic: A topic name to subscribe for predicted steering commands

ControlCommandTopic: A topic name to publish EGO vehicle commands for training

Example sensor config JSON:

```json
{
    "type": "Lane Following",
    "name": "Lane Following Sensor",
    "params": {
        "Topic": "/lanefollowing/steering_cmd",
        "ControlCommandTopic": "/simulator/control/command"
    }
}
```

## Copyright and License

Copyright (c) 2020 LG Electronics, Inc.

This software contains code licensed as described in LICENSE.
