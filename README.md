C# Client Library - Introduction
============================================

This C# Client Library can be used to simplify interactions with the [IBM Watson IoT Platform] (https://internetofthings.ibmcloud.com). The documentation is divided into following sections:  

- The [Device section] (docs/Device.rst) contains information on how devices publish events and handle commands using the C# IBMWIoTP Client Library.
- The [Managed Device section] (docs/DeviceManagement.rst) contains information on how devices can connect to the Watson IoT Platform Device Management service using C# IBMWIoTP Client Library and perform device management operations like firmware update, location update, and diagnostics update.
- The [Gateway section] (docs/Gateway.rst) contains information on how gateways publish events and handle commands for itself and for the attached devices using the C# IBMWIoTP Client Library.
- The [Gateway Management section] (docs/GatewayManagement.rst) contains information on how to connect the gateway as Managed Gateway to IBM Watson IoT Platform and manage the attached devices.
- The [Application section] (docs/Application.rst) details how applications can use the C# IBMWIoTP Client Library to interact with devices.

-----

Supported Features
------------------

| Feature   |      Supported?      |
|----------|:-------------:|
| Device connectivity |  &#10004; |
| Gateway connectivity |    &#10004;   |
| Application connectivity | &#10004; |
| Watson IoT API | &#10004; |
| SSL/TLS | &#10004; |
| Client side Certificate based authentication | &#10008; |
| Device Management | &#10004; |
| Device Management Extension(DME) | &#10008; |
| Scalable Application | &#10004; |
| Auto reconnect | &#10008; |
| Websocket | &#10008; |
| Event/Command publish using MQTT| &#10004; |
| Event/Command publish using HTTP| &#10008; |

NuGet Package
--------------------------------
 C# library is available in [nuget](https://www.nuget.org/packages/IBMWIoTP/)
 
To install IBMWIoTP, run the following command in the Package Manager Console 

```
  PM> Install-Package IBMWIoTP 
```


----
Dependencies
-------------------------------------------------------------------------------

-  [Paho M2MQTT] (https://www.nuget.org/packages/M2Mqtt/) - provides a client class which enable applications to connect to an MQTT broker
-  [log4net] (https://www.nuget.org/packages/log4net/) - library for creating log.

----

License
-----------------------

The library is shipped with Eclipse Public License and refer to the [License file] (LICENSE) for more information about the licensing.
