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
using IBMWIoTP;
using System.Web.Script.Serialization;

namespace ApiClient
{
	class Program
	{
		public static void Main(string[] args)
		{
			string apiKey ="";
			string authToken ="";
			if(string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(authToken)){
				Console.Write("please Enter API Key : ");
				apiKey = Console.ReadLine();
				Console.WriteLine();
				Console.Write("Please Enter Auth Token : ");
				authToken = Console.ReadLine();
			}
			IBMWIoTP.ApiClient client = new IBMWIoTP.ApiClient(apiKey,authToken);

			try {
			
				Console.WriteLine("get org info");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetOrganizationDetail()));
				Console.WriteLine("get all device list");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetAllDevices()));
				
				IBMWIoTP.RegisterDevicesInfo [] bulk = new IBMWIoTP.RegisterDevicesInfo[1];
				var info = new IBMWIoTP.RegisterDevicesInfo();
				info.deviceId="123qwe";
				info.typeId = "demotest";
				info.authToken ="1qaz2wsx3edc4rfv";
				info.deviceInfo = new IBMWIoTP.DeviceInfo();
				info.location = new IBMWIoTP.LocationInfo();
				info.metadata = new {};
				
				bulk[0] = info;
				
				Console.WriteLine("get Bulk reg ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.RegisterMultipleDevices(bulk)));
				IBMWIoTP.DeviceListElement [] removeBulk = new IBMWIoTP.DeviceListElement[1];
				var del = new IBMWIoTP.DeviceListElement();
				del.deviceId ="123qwe";
				del.typeId="demotest";
				
				removeBulk[0]=del;
				Console.WriteLine("get Bulk remove ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.DeleteMultipleDevices(removeBulk)));
				
				Console.WriteLine("GetAllDeviceTypes ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetAllDeviceTypes()));
	//
				DeviceTypeInfo devty = new DeviceTypeInfo();
				devty.classId="Gateway";
				devty.deviceInfo = new DeviceInfo();
				devty.id = "gatewaypi";
				devty.metadata= new {};
				Console.WriteLine("RegisterDeviceType");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.RegisterDeviceType(devty)));
				
				Console.WriteLine("GetDeviceType ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDeviceType("gatewaypi")));
				
				Console.WriteLine("UpdateDeviceType ");
				var u = new IBMWIoTP.DeviceTypeInfoUpdate();
				u.description="test";
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.UpdateDeviceType("gatewaypi",u)));
				
				Console.WriteLine("GetDeviceType ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDeviceType("gatewaypi")));
	
				Console.WriteLine("DeleteDeviceType ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.DeleteDeviceType("gatewaypi")));
				
				Console.WriteLine("Problem ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDeviceConnectionLogs("demotest","1qaz2wsx")));
				
				Console.WriteLine("data usage ");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDataUsage("2016","2016",false)));
				
				
				Console.WriteLine("Service Status");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetServiceStatus()));	
				
				Console.WriteLine("lastEvents");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetLastEvents("demotest","1qaz2wsx")));
				
				Console.WriteLine("lastEvents of type");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetLastEventsByEventType("demotest","1qaz2wsx","test")));
	
			
				Console.WriteLine("GetAllDeviceManagementRequests");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetAllDeviceManagementRequests()));
	
				Console.WriteLine("InitiateDeviceManagementRequest");
				IBMWIoTP.DeviceMgmtparameter [] param = new IBMWIoTP.DeviceMgmtparameter[1];
				IBMWIoTP.DeviceMgmtparameter p = new IBMWIoTP.DeviceMgmtparameter();
				p.name="rebootAfter";
				p.value = "100";
				param[0] = p;
				IBMWIoTP.DeviceListElement [] deviceList = new IBMWIoTP.DeviceListElement[1];
				IBMWIoTP.DeviceListElement ele = new IBMWIoTP.DeviceListElement();
				ele.typeId = "demotest";
				ele.deviceId= "1234";
				deviceList[0] = ele;
				var responce =client.InitiateDeviceManagementRequest("device/reboot",param,deviceList);//new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<dynamic>(jsonResponse);
				Console.WriteLine(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(responce));
				string id = responce["reqId"];
				
				Console.WriteLine("GetDeviceManagementRequest");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDeviceManagementRequest(id)));
	
				Console.WriteLine("GetDeviceManagementRequestStatus");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDeviceManagementRequestStatus(id)));
	
				Console.WriteLine("GetDeviceManagementRequestStatus");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDeviceManagementRequestStatus(id,ele.typeId,ele.deviceId)));
	
				Console.WriteLine("DeleteDeviceManagementRequest");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.DeleteDeviceManagementRequest(id)));
	
				Console.WriteLine("weather");
				Console.WriteLine( new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(client.GetDeviceLocationWeather("testgwdev","1234")));
	
			
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
			Console.Write("Press any key to exit . . . ");
			Console.ReadKey();
		}
		
	}
}