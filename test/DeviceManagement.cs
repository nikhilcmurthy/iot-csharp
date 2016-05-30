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
using IBMWIoTP;
using NUnit.Framework;

namespace test
{
	[TestFixture]
	public class DeviceManagement
	{
		IBMWIoTP.DeviceManagement testClient;
		string orgId,deviceType,deviceID,authmethod,authtoken;
		
		[SetUp]
		public void Setup() 
		{
			testClient = new IBMWIoTP.DeviceManagement("../../Resource/prop.txt",true);
		}
		[Test]
		public void DeviceManagementObjectCreationWithFilePath()
		{
			testClient = new IBMWIoTP.DeviceManagement("../../Resource/prop.txt",true);
		}
		[Test]
		public void DeviceManagementObjectCreationWithParams()
		{
			Dictionary<string,string> data = IBMWIoTP.DeviceClient.parseFile("../../Resource/prop.txt","## Device Registration detail");
	    	if(	!data.TryGetValue("Organization-ID",out orgId)||
	    		!data.TryGetValue("Device-Type",out deviceType)||
	    		!data.TryGetValue("Device-ID",out deviceID)||
	    		!data.TryGetValue("Authentication-Method",out authmethod)||
	    		!data.TryGetValue("Authentication-Token",out authtoken) )
	    	{
	    		throw new Exception("Invalid property file");
	    	}
			testClient = new IBMWIoTP.DeviceManagement(orgId,deviceType,deviceID,authmethod,authtoken,true);
		}
		[Test, ExpectedException]
		public void DeviceManagementObjectCreationWithinvalidFilePath()
		{
			testClient = new IBMWIoTP.DeviceManagement("propInvalid.text",true);
		}
		[Test]
		public void DeviceManagementManagedConnect()
		{
			testClient.connect();
			Assert.IsTrue(testClient.connectionStatus());
		}
		[Test]
		public void DeviceManagementManagedRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			DeviceInfo simpleDeviceInfo = new DeviceInfo();
		    simpleDeviceInfo.description = "My device";
		    simpleDeviceInfo.deviceClass = "My device class";
		    simpleDeviceInfo.manufacturer = "My device manufacturer";
		    simpleDeviceInfo.fwVersion = "Device Firmware Version";
		    simpleDeviceInfo.hwVersion = "Device HW Version";
		    simpleDeviceInfo.model = "My device model";
		    simpleDeviceInfo.serialNumber = "12345";
		    simpleDeviceInfo.descriptiveLocation ="My device location";
			testClient.deviceInfo = simpleDeviceInfo;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};

			
			uid = testClient.manage(4000,true,true,new {test=1});
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		
		[Test]
		public void DeviceManagementAddErrorCodeRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			 uid = testClient.addErrorCode(12);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void DeviceManagementClearErrorCodeRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			 uid = testClient.clearErrorCode();
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void DeviceManagementAddLogRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			 uid = testClient.addLog("test","data",1);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void DeviceManagementClearLogRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			 uid = testClient.clearLog();
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void DeviceManagementSetLocationRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			 uid = testClient.setLocation(77.5667,12.9667, 0,10);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void DeviceManagementUnManagedRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			 uid = testClient.unmanage();
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
	
		
	}
}
