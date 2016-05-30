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
	public class GatewayManagement
	{
		IBMWIoTP.GatewayManagement testClient;
		string orgId,deviceType,deviceID,authmethod,authtoken;
		
		[SetUp]
		public void Setup() 
		{
			testClient = new IBMWIoTP.GatewayManagement("../../Resource/Gatewayprop.txt",true);
		}
		[Test]
		public void GatewayManagementObjectCreationWithFilePath()
		{
			testClient = new IBMWIoTP.GatewayManagement("../../Resource/Gatewayprop.txt",true);
		}
		[Test]
		public void GatewayManagementObjectCreationWithParams()
		{
			Dictionary<string,string> data = IBMWIoTP.DeviceClient.parseFile("../../Resource/Gatewayprop.txt","## Gateway Registration detail");
	    	if(	!data.TryGetValue("Organization-ID",out orgId)||
	    		!data.TryGetValue("Device-Type",out deviceType)||
	    		!data.TryGetValue("Device-ID",out deviceID)||
	    		!data.TryGetValue("Authentication-Method",out authmethod)||
	    		!data.TryGetValue("Authentication-Token",out authtoken) )
	    	{
	    		throw new Exception("Invalid property file");
	    	}
			testClient = new IBMWIoTP.GatewayManagement(orgId,deviceType,deviceID,authmethod,authtoken,true);
		}
		[Test, ExpectedException]
		public void GatewayManagementObjectCreationWithinvalidFilePath()
		{
			testClient = new IBMWIoTP.GatewayManagement("propInvalid.text",true);
		}
		[Test]
		public void GatewayManagementManagedConnect()
		{
			testClient.connect();
			Assert.IsTrue(testClient.connectionStatus());
		}
		[Test]
		public void GatewayManagementManagedRequest()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			IBMWIoTP.DeviceInfo simpleDeviceInfo = new IBMWIoTP.DeviceInfo();
		    simpleDeviceInfo.description = "My gateway";
		    simpleDeviceInfo.deviceClass = "My gateway class";
		    simpleDeviceInfo.manufacturer = "My gateway manufacturer";
		    simpleDeviceInfo.fwVersion = "gateway Firmware Version";
		    simpleDeviceInfo.hwVersion = "gateway HW Version";
		    simpleDeviceInfo.model = "My gateway model";
		    simpleDeviceInfo.serialNumber = "12345";
		    simpleDeviceInfo.descriptiveLocation ="My gateway location";
			testClient.deviceInfo = simpleDeviceInfo;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			uid = testClient.managedGateway(4000,true,true,new{type="gateway"});
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		
		[Test]
		public void GatewayManagementAddErrorCodeRequest()
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
			 uid = testClient.addGatewayErrorCode(12);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementClearErrorCodeRequest()
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
			 uid = testClient.clearGatewayErrorCode();
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementAddLogRequest()
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
			 uid = testClient.addGatewayLog("test","data",1);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementClearLogRequest()
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
			 uid = testClient.clearGatewayLog();
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementSetLocationRequest()
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
			 uid = testClient.setGatewayLocation(77.5667,12.9667, 0,10);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementUnManagedRequest()
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
			 uid = testClient.unmanagedGateway();
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementManagedRequestForDevice()
		{
			var status ="";
			var uid ="";
			var recivedId ="";
			var wasCalled = false;
			IBMWIoTP.DeviceInfo simpleDeviceInfo = new IBMWIoTP.DeviceInfo();
		    simpleDeviceInfo.description = "My connected device";
		    simpleDeviceInfo.deviceClass = "My connected device class";
		    simpleDeviceInfo.manufacturer = "My connected device manufacturer";
		    simpleDeviceInfo.fwVersion = "Connected Device Firmware Version";
		    simpleDeviceInfo.hwVersion = "Connected Device HW Version";
		    simpleDeviceInfo.model = "My Connected device model";
		    simpleDeviceInfo.serialNumber = "12345";
		    simpleDeviceInfo.descriptiveLocation ="My Connected device location";
			testClient.deviceInfo = simpleDeviceInfo;
			if(!testClient.isConnected())
				testClient.connect();
			testClient.mgmtCallback += (reqId,sts) => {
				recivedId= reqId;
				status =sts;
				wasCalled = true;
			};
			uid = testClient.managedDevice("CsharpTestGatewayConnectedDevice","csharpgwdev0",4000,true,true,simpleDeviceInfo,new{type="connected device"});
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		
		[Test]
		public void GatewayManagementAddErrorCodeRequestForDevice()
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
			 uid = testClient.addDeviceErrorCode("CsharpTestGatewayConnectedDevice","csharpgwdev0",12);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementClearErrorCodeRequestForDevice()
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
			 uid = testClient.clearDeviceErrorCode("CsharpTestGatewayConnectedDevice","csharpgwdev0");
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementAddLogRequestForDevice()
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
			 uid = testClient.addDeviceLog("CsharpTestGatewayConnectedDevice","csharpgwdev0","test","data",1);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementClearLogRequestForDevice()
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
			 uid = testClient.clearDeviceLog("CsharpTestGatewayConnectedDevice","csharpgwdev0");
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementSetLocationRequestForDevice()
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
			 uid = testClient.setDeviceLocation("CsharpTestGatewayConnectedDevice","csharpgwdev0",77.5667,12.9667, 0,10);
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
		[Test]
		public void GatewayManagementUnManagedRequestForDevice()
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
			 uid = testClient.unmanagedDevice("CsharpTestGatewayConnectedDevice","csharpgwdev0");
			Assert.IsTrue(wasCalled);
			Assert.AreEqual(status,"200");
			Assert.AreEqual(recivedId,uid);
		}
	}
}
