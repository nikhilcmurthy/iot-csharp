/*
 *  Copyright (c) $(YEAR) IBM Corporation and other Contributors.
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
using System.Collections.Generic;
using NUnit.Framework;

namespace test
{
	[TestFixture]
	public class ApiClient
	{
		IBMWIoTP.ApiClient client = null;
		string orgId,appID,apiKey,authToken,dmReqId,gatewayType,gatwayId,deviceType,deviceId,logId;
		[SetUp]
		public void Setup() 
		{
			
			Dictionary<string,string> data = IBMWIoTP.ApplicationClient.parseFile("../../Resource/AppProp.txt","## Application Registration detail");
        	if(	!data.TryGetValue("Organization-ID",out orgId)||
        		!data.TryGetValue("App-ID",out appID)||
        		!data.TryGetValue("Api-Key",out apiKey)||
        		!data.TryGetValue("Authentication-Token",out authToken) )
        	{
        		throw new Exception("Invalid property file");
        	}
			client = new IBMWIoTP.ApiClient(apiKey,authToken);
			client.Timeout=100000;
			Dictionary<string,string> device = IBMWIoTP.DeviceClient.parseFile("../../Resource/prop.txt","## Device Registration detail");
			device.TryGetValue("Device-Type",out deviceType);
			device.TryGetValue("Device-ID",out deviceId);
			Dictionary<string,string> gw = IBMWIoTP.GatewayClient.parseFile("../../Resource/Gatewayprop.txt","## Gateway Registration detail");
			gw.TryGetValue("Device-Type",out gatewayType);
			gw.TryGetValue("Device-ID",out gatwayId);
		}
		
		//Organization
		[Test]
		public void GetOrganizationDetail()
		{
			var result = client.GetOrganizationDetail();
			StringAssert.AreEqualIgnoringCase( orgId,result["id"]);
		}
		
		//Bulk Operations
		
		[Test]
		public void Bulk_a_Register()
		{
			IBMWIoTP.RegisterDevicesInfo [] bulk = new IBMWIoTP.RegisterDevicesInfo[1];
			var info = new IBMWIoTP.RegisterDevicesInfo();
			info.deviceId="bulk1";
			info.typeId = deviceType;
			info.authToken ="12345678";
			info.deviceInfo = new IBMWIoTP.DeviceInfo();
			info.location = new IBMWIoTP.LocationInfo();
			info.metadata = new {};
			
			bulk[0] = info;
			var result = client.RegisterMultipleDevices(bulk);
			int length = result.Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		[Test]
		public void Bulk_b_GetAll()
		{
			var result = client.GetAllDevices();
			int length = result["results"].Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		[Test]
		public void Bulk_c_UnRegister()
		{
			IBMWIoTP.DeviceListElement [] removeBulk = new IBMWIoTP.DeviceListElement[1];
			var del = new IBMWIoTP.DeviceListElement();
			del.deviceId ="bulk1";
			del.typeId=deviceType;
			removeBulk[0]=del;
		
			var result = client.DeleteMultipleDevices(removeBulk);
			StringAssert.AreEqualIgnoringCase( result[0]["typeId"],deviceType);
			StringAssert.AreEqualIgnoringCase( result[0]["deviceId"],"bulk1");
			Assert.IsTrue( result[0]["success"]);
		}
		
		//Device Types 
		[Test]
		public void DeviceType_a_Register()
		{
			
			IBMWIoTP.DeviceTypeInfo devty = new IBMWIoTP.DeviceTypeInfo();
			devty.classId="Gateway";
			devty.deviceInfo = new IBMWIoTP.DeviceInfo();
			devty.id = "CsharpTestType";
			devty.metadata= new {};
			var result = client.RegisterDeviceType(devty);
			StringAssert.AreEqualIgnoringCase( devty.id,result["id"]);
		}
		
		[Test]
		public void DeviceType_b_Get()
		{
			var result = client.GetDeviceType("CsharpTestType");
			StringAssert.AreEqualIgnoringCase( "CsharpTestType",result["id"]);
		}
		
		[Test]
		public void DeviceType_c_GetAll()
		{
			var result = client.GetAllDeviceTypes();
			int length = result["results"].Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		
		[Test]
		public void DeviceType_d_Update()
		{
			var u = new IBMWIoTP.DeviceTypeInfoUpdate();
			u.description="test";
			var result = client.UpdateDeviceType("CsharpTestType",u);
			StringAssert.AreEqualIgnoringCase( u.description,result["description"]);
		}	
		[Test]
		public void DeviceType_e_Delete()
		{
			var u = new IBMWIoTP.DeviceTypeInfoUpdate();
			u.description="test";
			var result = client.DeleteDeviceType("CsharpTestType");
		}
		
		//Device
		[Test]
		public void Device_a_Register()
		{
			var newdevice = new IBMWIoTP.RegisterSingleDevicesInfo();
			newdevice.deviceId = "csharp1";
			newdevice.authToken = "testtest";
			newdevice.deviceInfo = new IBMWIoTP.DeviceInfo();
			newdevice.location = new IBMWIoTP.LocationInfo();
			newdevice.location.latitude=121;
			newdevice.metadata = new {};	
			var result = client.RegisterDevice(deviceType,newdevice);
			StringAssert.AreEqualIgnoringCase(deviceType,result["typeId"]);
		}
		[Test]
		public void Device_b_ListDevices()
		{
			var u = new IBMWIoTP.DeviceTypeInfoUpdate();
			u.description="test";
			var result = client.ListDevices(deviceType);
			int length = result["results"].Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		[Test]
		public void Device_c_UpdateDevicesInfo()
		{
			var update  = new IBMWIoTP.UpdateDevicesInfo();
			update.deviceInfo =new IBMWIoTP.DeviceInfo();
			update.deviceInfo.description="updatedinfo";
			var result = client.UpdateDeviceInfo(deviceType,"csharp1",update);
			StringAssert.AreEqualIgnoringCase("updatedinfo",result["deviceInfo"]["description"]);
		}
		[Test]
		public void Device_d_GetDeviceInfo()
		{
			var result = client.GetDeviceInfo(deviceType,"csharp1");
			StringAssert.AreEqualIgnoringCase("updatedinfo",result["deviceInfo"]["description"]);
		}

		[Test]
		public void Device_e_UpdateDeviceLocationInfo()
		{
			var loc = new IBMWIoTP.LocationInfo();
			loc.accuracy=1;
			loc.measuredDateTime = DateTime.Now.ToString("o");
				
			var result = client.UpdateDeviceLocationInfo(deviceType,"csharp1",loc);
		}
		[Test]
		public void Device_f_GetDeviceLocationInfo()
		{
			var result = client.GetDeviceLocationInfo(deviceType,"csharp1");
			Assert.AreEqual(1,result["accuracy"]);
		}
		[Test]
		public void Device_z_UnregisterDevice()
		{
			var result = client.UnregisterDevice(deviceType,"csharp1");
		}
		
		//gateway and management info
		
		[Test]
		public void GetGatewayConnectedDevice()
		{
			var result = client.GetGatewayConnectedDevice(gatewayType,gatwayId);
			int length = result["results"].Length;
			Assert.That(length,Is.GreaterThanOrEqualTo(0));
		}
		
		
		[Test]
		public void GetDeviceManagementInfo()
		{
			var result = client.GetDeviceManagementInfo(deviceType,deviceId);
			Assert.IsTrue(result["supports"]["deviceActions"]);
		}
		
		//log
		[Test]
		public void Log_a_AddDeviceDiagLogs()
		{
			var log =new IBMWIoTP.LogInfo();
			log.message="test";
			log.severity =1;
				
			var result = client.AddDeviceDiagLogs(deviceType,deviceId,log);
		}
		
		[Test]
		public void Log_b_GetAllDiagnosticLogs()
		{
			var logResult= client.GetAllDiagnosticLogs(deviceType,deviceId);
			logId = logResult[0]["id"];
			Assert.IsNotNullOrEmpty(logId);
		}
		
		[Test]
		public void Log_c_GetDiagnosticLog()
		{
			var result= client.GetDiagnosticLog(deviceType,deviceId,logId);
			StringAssert.AreEqualIgnoringCase(logId,result["id"]);
			StringAssert.AreEqualIgnoringCase(deviceType,result["typeId"]);
			StringAssert.AreEqualIgnoringCase(deviceId,result["deviceId"]);
			StringAssert.AreEqualIgnoringCase("test",result["message"]);
			Assert.AreEqual(1,result["severity"]);
		}
		
		
		[Test]
		public void Log_d_DeleteDiagnosticLog()
		{
			var result= client.DeleteDiagnosticLog(deviceType,deviceId,logId);
			
		}
		[Test]
		public void Log_e_ClearAllDiagnosticLogs()
		{
			var result= client.ClearAllDiagnosticLogs(deviceType,deviceId);
		}
		
		
		//error
		
		[Test]
		public void ErrorCode_a_AddErrorCode()
		{
			var err =  new IBMWIoTP.ErrorCodeInfo();
			err.errorCode = 0;
			err.timestamp =  "";
			var result= client.AddErrorCode(deviceType,deviceId,err);
		}
		
		[Test]
		public void ErrorCode_b_GetDeviceErrorCodes()
		{
			var result= client.GetDeviceErrorCodes(deviceType,deviceId);
			var err= result[0];
			Assert.AreEqual(0,err["errorCode"]);
		}
		
		[Test]
		public void ErrorCode_c_ClearDeviceErrorCodes()
		{

			var result= client.ClearDeviceErrorCodes(deviceType,deviceId);
		}
		
		//Status
		[Test]
		public void GetDeviceConnectionLogs()
		{
			var result = client.GetDeviceConnectionLogs(deviceType,deviceId);
			int length = result.Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		
		[Test]
		public void GetDataUsage()
		{
			var result = client.GetDataUsage("2016","2016",false);
			StringAssert.AreEqualIgnoringCase( result["start"],"2016-01-01");
			StringAssert.AreEqualIgnoringCase( result["end"],"2016-12-31");
		}
		
		[Test]
		public void GetServiceStatus()
		{
			var result = client.GetServiceStatus();
			StringAssert.AreEqualIgnoringCase( result["us"]["messaging"],"green");
		}
		
		[Test]
		public void GetLastEvents()
		{
			var result = client.GetLastEvents(deviceType,deviceId);
			int length = result.Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		
		[Test]
		public void GetLastEventsByEventType()
		{
			var result = client.GetLastEventsByEventType(deviceType,deviceId,"test");
			StringAssert.AreEqualIgnoringCase( result["typeId"],deviceType);
			StringAssert.AreEqualIgnoringCase( result["deviceId"],deviceId);
			StringAssert.AreEqualIgnoringCase( result["eventId"],"test");
		}
		//device management
		[Test]
		public void DeviceManagementRequests_a_Initiate()
		{
			var testClient = new IBMWIoTP.DeviceManagement("../../Resource/prop.txt",true);
			testClient.connect();
			testClient.manage(4000,true,true,new {});
			
			IBMWIoTP.DeviceMgmtparameter [] param = new IBMWIoTP.DeviceMgmtparameter[1];
			IBMWIoTP.DeviceMgmtparameter p = new IBMWIoTP.DeviceMgmtparameter();
			p.name="rebootAfter";
			p.value = "100";
			param[0] = p;
			IBMWIoTP.DeviceListElement [] deviceList = new IBMWIoTP.DeviceListElement[1];
			IBMWIoTP.DeviceListElement ele = new IBMWIoTP.DeviceListElement();
			ele.typeId = deviceType;
			ele.deviceId= deviceId;
			deviceList[0] = ele;
			var result =client.InitiateDeviceManagementRequest("device/reboot",param,deviceList);
			dmReqId = result["reqId"];
			Assert.IsNotNullOrEmpty(dmReqId);
		}
		
		[Test]
		public void DeviceManagementRequests_b_GetAll()
		{
			var result = client.GetAllDeviceManagementRequests();
			int length = result["results"].Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		
		[Test]
		public void DeviceManagementRequests_c_Get()
		{
			var result = client.GetDeviceManagementRequest(dmReqId);
			StringAssert.AreEqualIgnoringCase( result["action"],"device/reboot");
		}
		
		[Test]
		public void DeviceManagementRequests_d_GetStatus()
		{
			var result = client.GetDeviceManagementRequestStatus(dmReqId);
			int length = result["results"].Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		
		[Test]
		public void DeviceManagementRequests_e_GetStatus()
		{
			var result = client.GetDeviceManagementRequestStatus(dmReqId,deviceType,deviceId);
			int status = result["status"];
			Assert.That(status,Is.EqualTo(1));
		}
	
		[Test]
		public void DeviceManagementRequests_f_Delete()
		{
			var result = client.DeleteDeviceManagementRequest(dmReqId);
		}
		//weather
		
		[Test]
		public void GetDeviceLocationWeather()
		{
			var result = client.GetDeviceLocationWeather(deviceType,deviceId);
		}
	}
}
