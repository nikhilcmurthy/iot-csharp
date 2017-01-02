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
		string orgId,appID,apiKey,authToken;
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
		}
		
		[Test]
		public void GetOrganizationDetail()
		{
			var result = client.GetOrganizationDetail();
			StringAssert.Contains(orgId,result.Id);
		}
	}
}
