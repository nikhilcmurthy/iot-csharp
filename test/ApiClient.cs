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
		string orgId,appID,apiKey,authToken,dmReqId;
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
			info.typeId = "CsharpTestDevice";
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
			del.typeId="CsharpTestDevice";
			removeBulk[0]=del;
		
			var result = client.DeleteMultipleDevices(removeBulk);
			StringAssert.AreEqualIgnoringCase( result[0]["typeId"],"CsharpTestDevice");
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
		public void GetDeviceConnectionLogs()
		{
			var result = client.GetDeviceConnectionLogs("CsharpTestDevice","csharp0");
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
			var result = client.GetLastEvents("CsharpTestDevice","csharp0");
			int length = result.Length;
			Assert.That(length,Is.Not.EqualTo(0));
		}
		
		[Test]
		public void GetLastEventsByEventType()
		{
			var result = client.GetLastEventsByEventType("CsharpTestDevice","csharp0","test");
			StringAssert.AreEqualIgnoringCase( result["typeId"],"CsharpTestDevice");
			StringAssert.AreEqualIgnoringCase( result["deviceId"],"csharp0");
			StringAssert.AreEqualIgnoringCase( result["eventId"],"test");
		}
		
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
			ele.typeId = "CsharpTestDevice";
			ele.deviceId= "csharp0";
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
			var result = client.GetDeviceManagementRequestStatus(dmReqId,"CsharpTestDevice","csharp0");
			int status = result["status"];
			Assert.That(status,Is.EqualTo(1));
		}
	
		[Test]
		public void DeviceManagementRequests_f_Delete()
		{
			var result = client.DeleteDeviceManagementRequest(dmReqId);
		}
		
	}
}
