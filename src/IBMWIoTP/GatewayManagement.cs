/*
 *  Copyright (c) 2016 IBM Corporation and other Contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html 
 *
 * Contributors:
 *   Hari hara prasad Viswanathan  - Initial Contribution
 */
using System;
using System.Text;
using System.Dynamic;
using System.Threading;
using System.Collections.Generic;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using log4net;

namespace IBMWIoTP
{
	/// <summary>
	/// A managed Gateway class, used by Gateway, to connect the Gateway and devices behind the Gateway as managed devices 
	/// to IBM Watson IoT Platform and enables the Gateway to perform one or more Device Management operations.
	/// 
	/// The device management feature enhances the IBM Watson IoT Platform service with new capabilities 
	/// for managing devices and Gateways.
	///
	/// 
	/// 1)Connect devices behind the Gateway to IBM Watson IoT Platform
	/// 2)Send and receive its own sensor data like a directly connected device,
	/// 3)Send and receive data on behalf of the devices connected to it
	/// 4)Perform Device management operations like, manage, unmanage, firmware update, reboot, 
	///    update location, Diagnostics informations, Factory Reset and etc.. 
	///    for both Gateway and devices connected to the Gateway
	/// 
	/// </summary>
	public class GatewayManagement : GatewayClient
	{
		string MANAGE_TOPIC = "iotdevice-1/type/{0}/id/{1}/mgmt/manage";
		string UNMANAGE_TOPIC = "iotdevice-1/type/{0}/id/{1}/mgmt/unmanage";
		string UPDATE_LOCATION_TOPIC = "iotdevice-1/type/{0}/id/{1}/device/update/location"; 
		string ADD_ERROR_CODE_TOPIC = "iotdevice-1/type/{0}/id/{1}/add/diag/errorCodes";
		string CLEAR_ERROR_CODES_TOPIC = "iotdevice-1/type/{0}/id/{1}/clear/diag/errorCodes";
		string NOTIFY_TOPIC = "iotdevice-1/type/{0}/id/{1}/notify";
		string RESPONSE_TOPIC = "iotdevice-1/type/{0}/id/{1}/response";
		string ADD_LOG_TOPIC = "iotdevice-1/type/{0}/id/{1}/add/diag/log";
		string CLEAR_LOG_TOPIC = "iotdevice-1/type/{0}/id/{1}/clear/diag/log";
		
		// Subscribe MQTT topics
		const string DM_RESPONSE_TOPIC = "iotdm-1/type/{0}/id/{1}/response";
		const string DM_OBSERVE_TOPIC = "iotdm-1/type/{0}/id/{1}/observe";
		const string DM_REBOOT_TOPIC = "iotdm-1/type/{0}/id/{1}/mgmt/initiate/device/reboot";
		const string DM_FACTORY_RESET ="iotdm-1/type/{0}/id/{1}/mgmt/initiate/device/factory_reset";
		const string DM_UPDATE_TOPIC = "iotdm-1/type/{0}/id/{1}/device/update";
		const string DM_CANCEL_OBSERVE_TOPIC = "iotdm-1/type/{0}/id/{1}/cancel";
		const string DM_FIRMWARE_DOWNLOAD_TOPIC = "iotdm-1/type/{0}/id/{1}/mgmt/initiate/firmware/download";
		const string DM_FIRMWARE_UPDATE_TOPIC = "iotdm-1/type/{0}/id/{1}/mgmt/initiate/firmware/update";
		
		//ResponceCode 
		public static int RESPONSECODE_FUNCTION_NOT_SUPPORTED = 501;
		public static int RESPONSECODE_ACCEPTED = 202;
		public static int RESPONSECODE_INTERNAL_ERROR = 500;
		public static int RESPONSECODE_BAD_REQUEST = 400;
		
		public static int UPDATESTATE_IDLE = 0;
		public static int UPDATESTATE_DOWNLOADING = 1;
		public static int UPDATESTATE_DOWNLOADED = 2;
		public static int UPDATESTATE_SUCCESS = 0;
		public static int UPDATESTATE_IN_PROGRESS = 1;
		public static int UPDATESTATE_OUT_OF_MEMORY = 2;
		public static int UPDATESTATE_CONNECTION_LOST = 3;
		public static int UPDATESTATE_VERIFICATION_FAILED = 4;
		public static int UPDATESTATE_UNSUPPORTED_IMAGE = 5;
		public static int UPDATESTATE_INVALID_URI = 6;
		
		public DeviceInfo deviceInfo = new DeviceInfo();
		public LocationInfo locationInfo = new LocationInfo();
		
		List<DMRequest> collection = new List<DMRequest>();
		ManualResetEvent oSignalEvent = new ManualResetEvent(false);
		bool isSync = false;
		ILog log = log4net.LogManager.GetLogger(typeof(GatewayManagement));
		DeviceFirmware fw = null;
		
		/// <summary>
		/// Constructor of gateway management, helps to connect as a managed gateway to Watson IoT 
		/// </summary>
        /// <param name="orgId">object of String which denotes your organization Id</param>
        /// <param name="gatewayDeviceType">object of String which denotes your gateway type</param>
        /// <param name="gatewayDeviceID">object of String which denotes gateway Id</param>
        /// <param name="authMethod">object of String which denotes your authentication method</param>
        /// <param name="authToken">object of String which denotes your authentication token</param>
		public GatewayManagement(string orgId, string gatewayDeviceType, string gatewayDeviceID, string authmethod, string authtoken):
			base(orgId,gatewayDeviceType,gatewayDeviceID,authmethod,authtoken)
		{
						                             
		}
		/// <summary>
		/// Constructor of gateway management, helps to connect as a managed gateway to Watson IoT 
		/// </summary>
        /// <param name="orgId">object of String which denotes your organization Id</param>
        /// <param name="gatewayDeviceType">object of String which denotes your gateway type</param>
        /// <param name="gatewayDeviceID">object of String which denotes gateway Id</param>
        /// <param name="authMethod">object of String which denotes your authentication method</param>
        /// <param name="authToken">object of String which denotes your authentication token</param>
		/// <param name="isSync"> Boolean value to represent the device management request and response in synchronize mode or Async mode</param>
		public GatewayManagement(string orgId, string gatewayDeviceType, string gatewayDeviceID, string authmethod, string authtoken,bool isSync):
			base(orgId,gatewayDeviceType,gatewayDeviceID,authmethod,authtoken)
		{
			this.isSync = isSync;
		}
		/// <summary>
		/// Constructor of gateway management, helps to connect as a managed gateway to Watson IoT 
		/// </summary>
		/// <param name="filePath">object of String which denotes file path that contains gateway credentials in specified format</param>
		/// <param name="isSync"> Boolean value to represent the device management request and response in synchronize mode or Async mode</param>
		public GatewayManagement(string filePath ,bool isSync):
			base(filePath)
		{
			this.isSync = isSync;
		}
		/// <summary>
		/// Constructor of gateway management, helps to connect as a managed gateway to Watson IoT 
		/// </summary>
		/// <param name="filePath">object of String which denotes file path that contains gateway credentials in specified format</param>
		public GatewayManagement(string filePath):
			base(filePath)
		{
		}
		/// <summary>
		/// To connect the device to the Watson IoT Platform
		/// </summary>
		public override void connect()
		{
			base.connect();
			suscribeTOManagedTopics();
		}
		
		class DMRequest
		{
			public  DMRequest()
			{
			}
			public  DMRequest(string reqId, string topic ,string json)
			{
				this.reqID = reqId;
				this.topic = topic;
				this.json =json;
			}
			public string reqID {get;set;}
			public string topic {get;set;}
			public string json {get;set;}
		}
		
		class DMResponce
		{
			public DMResponce()
			{
			}
			public string reqId {get;set;}
			public string rc {get;set;}
			
		}

		class DMField {
			public DMField(){
			}
		
			public string field {get;set;}
			public DeviceFirmware value {get;set;}
		}
		class DMFields {
			public DMFields(){
			}
			public DMField [] fields;
		}
		class DeviceActionReq{
		
			public DeviceActionReq(){
			}
			public string reqId {get;set;}
			public DMFields d {get;set;}
		}
		
		private void suscribeTOManagedTopics(){
			if(mqttClient.IsConnected)
			{
				
				string[] topics = { "iotdm-1/#"};
				byte[] qos = {1};
				mqttClient.Subscribe(topics, qos);
				mqttClient.MqttMsgPublishReceived -= subscriptionHandler;
				mqttClient.MqttMsgPublishReceived += subscriptionHandler;
				log.Info("Subscribes to topic [" +topics[0] + "]");
			}
		}		
		
		public void subscriptionHandler(object sender, MqttMsgPublishEventArgs e)
        {
			try{
				
	            string result = System.Text.Encoding.UTF8.GetString(e.Message);
	            var serializer  = new System.Web.Script.Serialization.JavaScriptSerializer();
	            DeviceActionReq fwData;
				int rc;
				string msg;
				
				if(e.Topic == string.Format(DM_RESPONSE_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID) ){
			            var responce = serializer.Deserialize<DMResponce>(result);
			            var itm =  collection.Find( x => x.reqID == responce.reqId );
			            if( itm is DMRequest)
			            {
			            	log.Info("["+responce.rc+"]  "+itm.topic+" of ReqId  "+ itm.reqID);
			            	if(this.mgmtCallback !=null)
			            		this.mgmtCallback(itm.reqID,responce.rc);
			            	collection.Remove(itm);
			            }
			             if(this.isSync){
				            oSignalEvent.Set();
			            	oSignalEvent.Dispose();
			            	oSignalEvent = new ManualResetEvent(false);
			            }
					}
				
				if(e.Topic ==  string.Format(DM_REBOOT_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID) ){
				 	var res = serializer.Deserialize<DMResponce>(result);
					log.Info("Device Rebot action called with ReqId : " +res.reqId );
					if(this.actionCallback != null)
						this.actionCallback(res.reqId , "reboot");
					}
	
				if(e.Topic ==  string.Format(DM_FACTORY_RESET,this.gatewayDeviceType,this.gatewayDeviceID) ){
				 	var resetResponce = serializer.Deserialize<DMResponce>(result);
					log.Info("Device Factory rest action called with ReqId : " +resetResponce.reqId );
					if(this.actionCallback != null)
						this.actionCallback(resetResponce.reqId , "reset");
					}
				if(e.Topic ==  string.Format(DM_UPDATE_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID) ){
					 fwData = serializer.Deserialize<DeviceActionReq>(result);
					var fields = fwData.d.fields;
					for (int i = 0; i < fields.Length; i++) {
						if(fields[i].field == "mgmt.firmware" ){
							this.fw = fields[i].value;
							break;
						}
					}
					if(this.fw != null)
					{
						sendResponce(fwData.reqId,204,"");
					}
	
					}
					
				if(e.Topic ==  string.Format(DM_OBSERVE_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID) ){
					fwData = serializer.Deserialize<DeviceActionReq>(result);
					sendResponce(fwData.reqId,200,"");
					}
					
				if(e.Topic ==  string.Format(DM_FIRMWARE_DOWNLOAD_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID) ){
					fwData = serializer.Deserialize<DeviceActionReq>(result);
					 rc = RESPONSECODE_ACCEPTED;
					 msg ="";
					if(this.fw.state != UPDATESTATE_IDLE){
						rc = RESPONSECODE_BAD_REQUEST;
						msg = "Cannot download as the device is not in idle state";							
					}
					if(this.fwCallback != null)
					{
						this.fwCallback("download",this.fw);
					}
					
					}
				if(e.Topic ==  string.Format(DM_FIRMWARE_UPDATE_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID) ){
					fwData = serializer.Deserialize<DeviceActionReq>(result);
					 rc = RESPONSECODE_ACCEPTED;
					 msg ="";
					if(this.fw.state != UPDATESTATE_DOWNLOADED){
						rc = RESPONSECODE_BAD_REQUEST;
						msg = "Firmware is still not successfully downloaded.";							
					}
					if(this.fwCallback != null)
					{
						this.fwCallback("update",this.fw);
					}
					
					}
				
				if(e.Topic ==  string.Format(DM_CANCEL_OBSERVE_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID) ){
					fwData = serializer.Deserialize<DeviceActionReq>(result);
					sendResponce(fwData.reqId,200,"");
					}

				            
			}
        	catch(Exception ex)
        	{
        		log.Error("Execption has occer in subscriptionHandler ",ex);
        	}

        }
		/// <summary>
		///  To know the current connection status of the device
		/// </summary>
		/// <returns> bool value of status of connection </returns>
		public bool connectionStatus()
		{
			return mqttClient.IsConnected;
		}
		private void publishDM (string type,string id,string topic , object message,string reqId)
		{
			var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(message);
			topic =  string.Format(topic,type,id);
			log.Info("Device Management Request For Topic " + topic +" with payload " +json);
			collection.Add(new DMRequest(reqId,topic,json));
			mqttClient.Publish(topic, Encoding.UTF8.GetBytes(json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE , false);
			if(this.isSync)
				oSignalEvent.WaitOne();
		}
		/// <summary>
		/// To register the gateway as an managed device to Watson IoT Platform
		/// </summary>
		/// <param name="lifeTime">Time period of the device to remain as managed device</param>
		/// <param name="supportDeviceActions">bool value to represent the support for device actions for the device</param>
		/// <param name="supportFirmwareActions">bool value to represent the support for firmware action for the device</param>
		/// <returns>unique id of the current request</returns>
		public string managedGateway(int lifeTime,bool supportDeviceActions,bool supportFirmwareActions)
		{
			try{
				string uid =Guid.NewGuid().ToString();
				var payload = new {
					lifetime =  lifeTime,
					supports = new {
						deviceActions =  supportDeviceActions,
						firmwareActions = supportFirmwareActions
					},
					deviceInfo = this.deviceInfo ,
					metadata = new {}
				};
				var message = new { reqId = uid , d = payload };
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,MANAGE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		
		}
		/// <summary>
		/// To register the gateway as an managed device to Watson IoT Platform
		/// </summary>
		/// <param name="lifeTime">Time period of the device to remain as managed device</param>
		/// <param name="supportDeviceActions">bool value to represent the support for device actions for the device</param>
		/// <param name="supportFirmwareActions">bool value to represent the support for firmware action for the device</param>
		/// <param name="metaData"> Object that represent the meta information of the device </param>
		/// <returns>unique id of the current request</returns>
		public string managedGateway(int lifeTime,bool supportDeviceActions,bool supportFirmwareActions, Object metaData)
		{
			try{
				string uid =Guid.NewGuid().ToString();
				var payload = new {
					lifetime =  lifeTime,
					supports = new {
						deviceActions =  supportDeviceActions,
						firmwareActions = supportFirmwareActions
					},
					deviceInfo = this.deviceInfo ,
					metadata = metaData
				};
				var message = new { reqId =uid , d = payload };
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,MANAGE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
			
		}
		/// <summary>
		///  To register the gateway as an Unmanaged device to Watson Iot Platform
		/// </summary>
		/// <returns></returns>
		public string unmanagedGateway()
		{
			try{
				string uid =Guid.NewGuid().ToString();
				var message = new { reqId =uid };
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,UNMANAGE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		/// <summary>
		///  To Add error code to the Watson IoT platform for the gateway 
		/// </summary>
		/// <param name="errorCode">integer value of device error code </param>
		/// <returns>unique id of the current request</returns>
		public string addGatewayErrorCode(int errorCode)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var payload = new{
					errorCode = errorCode
				};
				var message = new { reqId =uid ,d = payload};
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,ADD_ERROR_CODE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		
		/// <summary>
		///  To Clear the error code added by the gateway in the Watson IoT Platform
		/// </summary>
		/// <returns>unique id of the current request</returns>
		public string clearGatewayErrorCode()
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var message = new { reqId =uid};
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,CLEAR_ERROR_CODES_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		/// <summary>
		/// To Add log of the gateway status to Watson IoT Platform 
		/// </summary>
		/// <param name="msg">String value of the log message </param>
		/// <param name="dataAsString"> String value of base64-encoded binary diagnostic data </param>
		/// <param name="severityValue">intiger value of severity (0: informational, 1: warning, 2: error)</param>
		/// <returns>unique id of the current request</returns>
		public string addGatewayLog(string msg, string dataAsString,int severityValue)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var payload = new{
					message = msg,
					timestamp = DateTime.UtcNow.ToString("o"),
					data = dataAsString,
					severity = severityValue
				};
				var message = new { reqId =uid ,d = payload};
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,ADD_LOG_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		
		/// <summary>
		///  To clear the log present in Watson IoT platform for the gateway
		/// </summary>
		/// <returns>unique id of the current request</returns>
		public string clearGatewayLog()
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var message = new { reqId =uid};
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,CLEAR_LOG_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		
		/// <summary>
		/// To Update the current location of the device to the Watson IoT Platform
		/// </summary>
		/// <param name="longitude">double value of longitude of the device </param>
		/// <param name="latitude">double value of latitude of the device</param>
		/// <param name="elevation">double value of elevation of the device</param>
		/// <param name="accuracy">double value of accuracy of the reading</param>
		/// <returns>unique id of the current request</returns>
		public string setGatewayLocation( double  longitude,double latitude,double elevation,double accuracy)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				this.locationInfo.longitude = longitude;
				this.locationInfo.latitude = latitude;
				this.locationInfo.elevation = elevation;
				this.locationInfo.accuracy =accuracy;
				this.locationInfo.measuredDateTime = DateTime.Now.ToString("o");
				//this.locationInfo.updatedDateTime = this.locationInfo.measuredDateTime;
				var message = new { reqId =uid ,d = this.locationInfo };
				publishDM(this.gatewayDeviceType,this.gatewayDeviceID,UPDATE_LOCATION_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		/// <summary>
		///  To register the connected device as an managed device to Watson IoT Platform
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <param name="lifeTime">Time period of the device to remain as managed device</param>
		/// <param name="supportDeviceActions">bool value to represent the support for device actions for the device</param>
		/// <param name="supportFirmwareActions">bool value to represent the support for firmware action for the device</param>
		/// <returns>unique id of the current request</returns>
		public string managedDevice(string deviceType,string deviceID,int lifeTime,bool supportDeviceActions,bool supportFirmwareActions)
		{
			try{
				string uid =Guid.NewGuid().ToString();
				var payload = new {
					lifetime =  lifeTime,
					supports = new {
						deviceActions =  supportDeviceActions,
						firmwareActions = supportFirmwareActions
					},
					deviceInfo = new {} ,
					metadata = new {}
				};
				var message = new { reqId = uid , d = payload };
				publishDM(deviceType,deviceID,MANAGE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		
		}
		/// <summary>
		///  To register the connected device as an managed device to Watson IoT Platform
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <param name="lifeTime">Time period of the device to remain as managed device</param>
		/// <param name="supportDeviceActions">bool value to represent the support for device actions for the device</param>
		/// <param name="supportFirmwareActions">bool value to represent the support for firmware action for the device</param>
		/// <param name="info">Device Info object that represents the basic device informations </param>
		/// <returns>unique id of the current request</returns>
		public string managedDevice(string deviceType,string deviceID,int lifeTime,bool supportDeviceActions,bool supportFirmwareActions,DeviceInfo info)
		{
			try{
				string uid =Guid.NewGuid().ToString();
				var payload = new {
					lifetime =  lifeTime,
					supports = new {
						deviceActions =  supportDeviceActions,
						firmwareActions = supportFirmwareActions
					},
					deviceInfo = info ,
					metadata = new {}
				};
				var message = new { reqId = uid , d = payload };
				publishDM(deviceType,deviceID,MANAGE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		
		}
			/// <summary>
		///  To register the connected device as an managed device to Watson IoT Platform
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <param name="lifeTime">Time period of the device to remain as managed device</param>
		/// <param name="supportDeviceActions">bool value to represent the support for device actions for the device</param>
		/// <param name="supportFirmwareActions">bool value to represent the support for firmware action for the device</param>
		/// <param name="info">Device Info object that represents the basic device informations </param>
		/// <param name="metaData">Object that represent the meta information of the device</param>
		/// <returns>unique id of the current request</returns>
		public string managedDevice(string deviceType,string deviceID,int lifeTime,bool supportDeviceActions,bool supportFirmwareActions,DeviceInfo info, Object metaData)
		{
			try{
				string uid =Guid.NewGuid().ToString();
				var payload = new {
					lifetime =  lifeTime,
					supports = new {
						deviceActions =  supportDeviceActions,
						firmwareActions = supportFirmwareActions
					},
					deviceInfo = info ,
					metadata = metaData
				};
				var message = new { reqId =uid , d = payload };
				publishDM(deviceType,deviceID,MANAGE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
			
		}
		
		/// <summary>
		///  To register the connected device as an Unmanaged device to Watson Iot Platform
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <returns>unique id of the current request</returns>
		public string unmanagedDevice(string deviceType,string deviceID)
		{
			try{
				string uid =Guid.NewGuid().ToString();
				var message = new { reqId =uid };
				publishDM(deviceType,deviceID,UNMANAGE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		/// <summary>
		/// To Add error code to the Watson IoT platform for the connected device
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <param name="errorCode">integer value of device error code </param>
		/// <returns>unique id of the current request</returns>
		public string addDeviceErrorCode(string deviceType,string deviceID,int errorCode)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var payload = new{
					errorCode = errorCode
				};
				var message = new { reqId =uid ,d = payload};
				publishDM(deviceType,deviceID,ADD_ERROR_CODE_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		
		/// <summary>
		///  To Clear the error code added by the connected device in the Watson IoT Platform
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <returns>unique id of the current request</returns>
		public string clearDeviceErrorCode(string deviceType,string deviceID)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var message = new { reqId =uid};
				publishDM(deviceType,deviceID,CLEAR_ERROR_CODES_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		/// <summary>
		///  To Add log of the connected device status to Watson IoT Platform 
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <param name="msg">String value of the log message </param>
		/// <param name="dataAsString"> String value of base64-encoded binary diagnostic data </param>
		/// <param name="severityValue">intiger value of severity (0: informational, 1: warning, 2: error)</param>
		/// <returns>unique id of the current request</returns>
		public string addDeviceLog(string deviceType,string deviceID,string msg, string dataAsString,int severityValue)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var payload = new{
					message = msg,
					timestamp = DateTime.UtcNow.ToString("o"),
					data = dataAsString,
					severity = severityValue
				};
				var message = new { reqId =uid ,d = payload};
				publishDM(deviceType,deviceID,ADD_LOG_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		
		/// <summary>
		///  To clear the log present in Watson IoT platform for the connected device
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <returns>unique id of the current request</returns>
		public string clearDeviceLog(string deviceType,string deviceID)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				var message = new { reqId =uid};
				publishDM(deviceType,deviceID,CLEAR_LOG_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		
		/// <summary>
		/// To Update the current location of the connected device to the Watson IoT Platform
		/// </summary>
		/// <param name="deviceType">object of String which denotes your device type</param>
		/// <param name="deviceID">object of String which denotes device Id</param>
		/// <param name="longitude">double value of longitude of the device </param>
		/// <param name="latitude">double value of latitude of the device</param>
		/// <param name="elevation">double value of elevation of the device</param>
		/// <param name="accuracy">double value of accuracy of the reading</param>
		/// <returns>unique id of the current request</returns>
		public string setDeviceLocation(string deviceType,string deviceID,double  longitude,double latitude,double elevation,double accuracy)
		{
			try
			{
				string uid =Guid.NewGuid().ToString();
				this.locationInfo.longitude = longitude;
				this.locationInfo.latitude = latitude;
				this.locationInfo.elevation = elevation;
				this.locationInfo.accuracy =accuracy;
				this.locationInfo.measuredDateTime = DateTime.Now.ToString("o");
				//this.locationInfo.updatedDateTime = this.locationInfo.measuredDateTime;
				var message = new { reqId =uid ,d = this.locationInfo };
				publishDM(deviceType,deviceID,UPDATE_LOCATION_TOPIC,message,uid);
				return uid;
			}
        	catch(Exception e)
        	{
        		log.Error("Execption has occer in manage ",e);
        		return "";
        	}
		}
		public void sendResponce ( string reqId, int rc ,string msg){
			var message = new { reqId =reqId , rc = Convert.ToString(rc) , message = msg};
			var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(message);
			log.Info("Sending DM responce with payload " +json);
			mqttClient.Publish(string.Format(RESPONSE_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID), Encoding.UTF8.GetBytes(json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE , false);
			
		}
		
		public void setState(int state){
			if(this.fw ==null)
				return;
			object[] field = new object[1];
			field[0]= new {
					        	field="mgmt.firmware",
					        	value= new {
					        		state=state
					        	}
			           };
			var notify = new {
				d = new {
					fields=field
					}
				};
			this.fw.state = state;
			string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(notify);
			mqttClient.Publish(string.Format(NOTIFY_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID), Encoding.UTF8.GetBytes(json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE , false);
		
		}
		
		public void setUpdateState(int updateState){
			if(this.fw ==null)
				return;
			object[] field = new object[1];
			field[0]= new {
					        	field="mgmt.firmware",
					        	value= new {
					        		state=UPDATESTATE_IDLE,
					        		updateStatus = updateState
					        	}
			           };
			var notify = new {
				d = new {
					fields=field
					}
				};
			
			this.fw.state = UPDATESTATE_IDLE;
			this.fw.updateStatus = updateState;
			string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(notify);
			mqttClient.Publish(string.Format(NOTIFY_TOPIC,this.gatewayDeviceType,this.gatewayDeviceID), Encoding.UTF8.GetBytes(json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE , false);
		
		}
		
        public delegate void processMgmtResponce( string reqestId, string responceCode);

        public event processMgmtResponce mgmtCallback;
        
        public delegate void processDeviceAction( string reqestId,string action);

        public event processDeviceAction actionCallback;
        
        public delegate void processFirmwareAction(string action,DeviceFirmware firmware);

        public event processFirmwareAction fwCallback;
    
	
		
	}
	
}
