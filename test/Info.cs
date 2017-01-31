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
using IBMWIoTP;
using NUnit.Framework;

namespace test
{
	[TestFixture]
	public class Info
	{
		[Test]
		public void DeviceInfoObjectTest()
		{
			DeviceInfo simpleDeviceInfo = new DeviceInfo();
		    simpleDeviceInfo.description = "My device";
		    simpleDeviceInfo.deviceClass = "My device class";
		    simpleDeviceInfo.manufacturer = "My device manufacturer";
		    simpleDeviceInfo.fwVersion = "Device Firmware Version";
		    simpleDeviceInfo.hwVersion = "Device HW Version";
		    simpleDeviceInfo.model = "My device model";
		    simpleDeviceInfo.serialNumber = "12345";
		    simpleDeviceInfo.descriptiveLocation ="My device location";
		    
		    Assert.AreEqual(simpleDeviceInfo.description , "My device");
		    Assert.AreEqual(simpleDeviceInfo.deviceClass , "My device class");
		    Assert.AreEqual(simpleDeviceInfo.manufacturer , "My device manufacturer");
		    Assert.AreEqual(simpleDeviceInfo.fwVersion , "Device Firmware Version");
		    Assert.AreEqual(simpleDeviceInfo.hwVersion , "Device HW Version");
		    Assert.AreEqual(simpleDeviceInfo.model , "My device model");
		    Assert.AreEqual(simpleDeviceInfo.serialNumber , "12345");
		    Assert.AreEqual(simpleDeviceInfo.descriptiveLocation ,"My device location");
		}
		[Test]
		public void DeviceFirmwareObjectTest()
		{
			DeviceFirmware sample = new DeviceFirmware();
			sample.version = "1.0.0"; 
			sample.name = "test"; 
			sample.url = "http://url.com"; 
			sample.uri = "http://url.com"; 
			sample.verifier = "1q2w4e"; 
			sample.state = 0; 
			sample.updateStatus = 0; 
			sample.updatedDateTime = "time"; 
			
			Assert.AreEqual(sample.version , "1.0.0"); 
			Assert.AreEqual(sample.name , "test"); 
			Assert.AreEqual(sample.url , "http://url.com"); 
			Assert.AreEqual(sample.uri , "http://url.com"); 
			Assert.AreEqual(sample.verifier , "1q2w4e"); 
			Assert.AreEqual(sample.state ,0); 
			Assert.AreEqual(sample.updateStatus , 0); 
			Assert.AreEqual(sample.updatedDateTime , "time"); 
		}
		
		[Test]
		public void LocationInfoObjectTest()
		{
			LocationInfo sample = new LocationInfo();
			sample.latitude = 10.213;
			sample.longitude = 145.213;
			sample.measuredDateTime="now";
			sample.elevation = 0;
			sample.accuracy=6;
			
			Assert.AreEqual(sample.latitude , 10.213);
			Assert.AreEqual(sample.longitude , 145.213);
			Assert.AreEqual(sample.measuredDateTime,"now");
			Assert.AreEqual(sample.elevation , 0);
			Assert.AreEqual(sample.accuracy,6);
		}
		
		[Test]
		public void GatewayErrorObjectTest()
		{
			GatewayError sample = new GatewayError();
			sample.Request = "my request";
			sample.Time = "now";
			sample.Topic = "my topic";
			sample.Type = "my type";
			sample.Id = "my id";
			sample.Client = "my client";
			sample.RC = "200";
			sample.Message = "test msg";
			
			Assert.AreEqual(sample.Request , "my request");
			Assert.AreEqual(sample.Time , "now");
			Assert.AreEqual(sample.Topic , "my topic");
			Assert.AreEqual(sample.Type , "my type");
			Assert.AreEqual(sample.Id , "my id");
			Assert.AreEqual(sample.Client , "my client");
			Assert.AreEqual(sample.RC , "200");
			Assert.AreEqual(sample.Message , "test msg");
		}
		
		
	}
}
