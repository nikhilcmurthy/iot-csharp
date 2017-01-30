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
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

using log4net;

namespace IBMWIoTP
{
	/// <summary>
	/// Description of ApiClient.
	/// </summary>
	public class ApiClient
	{
		ILog log = log4net.LogManager.GetLogger(typeof(ApplicationClient));
		private static string BaseURL = "https://{0}.internetofthings.ibmcloud.com/api/v0002";
		
		private static string OrgInfo = "/";
		
		private static string BulkGet = "/bulk/devices";
		private static string BulkAdd = "/bulk/devices/add";
		private static string BulkRemove = "/bulk/devices/remove";
		
		private static string DeviceTypes = "/device/types";
		private static string DeviceTypesIndigvual = "/device/types/{0}";
		
		private static string Device = "/device/types/{0}/devices";
		private static string DeviceIndigvual = "/device/types/{0}/devices/{1}";
		private static string DeviceGatewayList = "/device/types/{0}/devices/{1}/devices";
		private static string DeviceLocation = "/device/types/{0}/devices/{1}/location";
		private static string DeviceMgmtInfo = "/device/types/{0}/devices/{1}/mgmt";
		
		private static string DeviceLogs = "/device/types/{0}/devices/{1}/diag/logs";
		private static string DeviceLog = "/device/types/{0}/devices/{1}/diag/logs/{2}";
		private static string DeviceErrorCode = "/device/types/{0}/devices/{1}/diag/errorCodes";
	
		private static string Problem = "/logs/connection";
		
		private static string DeviceMgmt = "/mgmt/requests";
		private static string DeviceMgmtInd = "/mgmt/requests/{0}";
		private static string DeviceMgmtStatus = "/mgmt/requests/{0}/deviceStatus";
		private static string DeviceMgmtStatusInd = "/mgmt/requests/{0}/deviceStatus/{1}/{2}";
		
		private static string DeviceMgmtCustom = "/mgmt/custom/bundle";
		private static string DeviceMgmtCustomInd = "/mgmt/custom/bundle/{0}";
		
		private static string Usage = "/usage/data-traffic";
		
		private static string ServiceStatus = "/service-status";
		
		private static string LastEventCache = "/device/types/{0}/devices/{1}/events";
		private static string LastEventCacheInd = "/device/types/{0}/devices/{1}/events/{2}";
		
		private static string Weather = "/devices/types/{0}/devices/{1}/exts/twc/ops/geocode";
		
		
		private string _apiKey , _authToken,_orgId;
		
		public ApiClient(string apiKey , string authToken)
		{
			if(apiKey.Split('-').Length >0)
			{
				_apiKey = apiKey;
				_authToken = authToken;
				_orgId = apiKey.Split('-')[1];
			}
			else{
				throw new Exception("Invalid api key");
			}
		}
		protected dynamic RestHandler(string methord,string urlSuffix ,object param , bool isJsonResponce){	//,bool validResponceCode){
		
			string url =string.Format(BaseURL,_orgId);
			url = url+urlSuffix;
			bool isQuerry = false;
			if(methord == "GET" && param  is string)
			{
				url +=param; 
				isQuerry = true;
			}
			System.Net.WebRequest request = WebRequest.Create(url);
			request.Method = methord; 
			byte[] credentialBuffer = new UTF8Encoding().GetBytes(_apiKey + ":" +_authToken);
			request.Headers["Authorization"] ="Basic " + Convert.ToBase64String(credentialBuffer);
			request.PreAuthenticate = true;
			request.Timeout = Timeout == null ?  100000 :Timeout;
			log.Info("Request " +methord+"  "+url);
			
			if(param != null & !isQuerry){
				request.ContentType = "application/json";
				
				using (var streamWriter = new StreamWriter(request.GetRequestStream()))
				{
				    string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(param);
					log.Info("Request Json " +json);
				
				    streamWriter.Write(json);
				    streamWriter.Flush();
				    streamWriter.Close();
				}
			}
			// Get the response.
			HttpWebResponse  response =(HttpWebResponse) request.GetResponse();
			int responseCode =(int) response.StatusCode;
			log.Info("responce " +response.ResponseUri +"  " +responseCode);
			
			using (var responseStream = response.GetResponseStream())
			{
				if (responseStream != null)
				using (var reader = new StreamReader(responseStream))
				{	
					var JsonResponce = reader.ReadToEnd();
					if(isJsonResponce){
						return new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<dynamic>(JsonResponce);
					}
				}
			}
			
			return new {};
		}
		public int Timeout { get ; set ; }

		public dynamic GetOrganizationDetail(){
			return  RestHandler("GET",OrgInfo,null,true);
		}
		
		
		
		public dynamic GetAllDevices(){
			return RestHandler("GET",BulkGet,null,true);
		}
		
		public dynamic RegisterMultipleDevices(RegisterDevicesInfo[] info){
			return RestHandler("POST",BulkAdd,info,true);
		}
		
		public dynamic DeleteMultipleDevices(DeviceListElement[] info){
			return RestHandler("POST",BulkRemove,info,true);
		}
		
		
		
		public dynamic GetAllDeviceTypes(){
			return RestHandler("GET",DeviceTypes,null,true);
		}
		
		public dynamic RegisterDeviceType(DeviceTypeInfo info){
			return RestHandler("POST",DeviceTypes,info,true);
		}
		
		public dynamic GetDeviceType(string type){
			return RestHandler("GET",string.Format(DeviceTypesIndigvual,type),null,true);
		}
		
		public dynamic DeleteDeviceType(string type){
			return RestHandler("DELETE",string.Format(DeviceTypesIndigvual,type),null,false);
		}
		
		public dynamic UpdateDeviceType(string type,DeviceTypeInfoUpdate info){
			return RestHandler("PUT",string.Format(DeviceTypesIndigvual,type),info,true);
		}
		
		
		
		public dynamic ListDevices(string type){
			return RestHandler("GET",string.Format(Device,type),null,true);
		}
		
		public dynamic RegisterDevice(string type, RegisterSingleDevicesInfo info){
			return RestHandler("POST",string.Format(Device,type),info,true);
		}
		
		public dynamic UnregisterDevice(string type, string deviceId){
			return RestHandler("DELETE",string.Format(DeviceIndigvual,type,deviceId),null,false);
		}
		
		public dynamic GetDeviceInfo(string type, string deviceId){
			return RestHandler("GET",string.Format(DeviceIndigvual,type,deviceId),null,true);
		}
		
		public dynamic UpdateDeviceInfo(string type,string deviceId, UpdateDevicesInfo info){
			return RestHandler("PUT",string.Format(DeviceIndigvual,type,deviceId),info,true);
		}
		
		public dynamic GetGatewayConnectedDevice(string type, string deviceId){
			return RestHandler("GET",string.Format(DeviceGatewayList,type,deviceId),null,true);
		}
		
		public dynamic GetDeviceLocationInfo(string type, string deviceId){
			return RestHandler("GET",string.Format(DeviceLocation,type,deviceId),null,true);
		}
		
		public dynamic UpdateDeviceLocationInfo(string type, string deviceId,LocationInfo info){
			return RestHandler("PUT",string.Format(DeviceLocation,type,deviceId),info,false);
		}
		
		public dynamic GetDeviceManagementInfo(string type, string deviceId){
			return RestHandler("GET",string.Format(DeviceMgmtInfo,type,deviceId),null,true);
		}
		
		
		
		public dynamic GetAllDiagnosticLogs(string type, string deviceId){
			return RestHandler("GET",string.Format(DeviceLogs,type,deviceId),null,true);
		}
		public dynamic ClearAllDiagnosticLogs(string type, string deviceId){
			return RestHandler("DELETE",string.Format(DeviceLogs,type,deviceId),null,false);
		}
		public dynamic AddDeviceDiagLogs(string type, string deviceId,LogInfo info){
			return RestHandler("POST",string.Format(DeviceLogs,type,deviceId),info,true);
		}
		public dynamic GetDiagnosticLog(string type, string deviceId,string logId){
			return RestHandler("GET",string.Format(DeviceLog,type,deviceId,logId),null,true);
		}
		public dynamic DeleteDiagnosticLog(string type, string deviceId,string logId){
			return RestHandler("DELETE",string.Format(DeviceLog,type,deviceId,logId),null,false);
		}
		
		
		public dynamic GetDeviceErrorCodes(string type, string deviceId){
			return RestHandler("GET",string.Format(DeviceErrorCode,type,deviceId),null,true);
		}
		public dynamic ClearDeviceErrorCodes(string type, string deviceId){
			return RestHandler("DELETE",string.Format(DeviceErrorCode,type,deviceId),null,false);
		}
		public dynamic AddErrorCode(string type, string deviceId,ErrorCodeInfo err){
			return RestHandler("POST",string.Format(DeviceErrorCode,type,deviceId),err,false);
		}
		
		public dynamic GetDeviceConnectionLogs(string type, string deviceId){
			return RestHandler("GET",Problem,"?typeId="+type+"&deviceId="+deviceId,true);
		}
		
		public dynamic GetAllDeviceManagementRequests(){
			return RestHandler("GET",DeviceMgmt,null,true);
		}
		
		public dynamic InitiateDeviceManagementRequest(string action,DeviceMgmtparameter [] parameters,DeviceListElement [] devices){
			return RestHandler("POST",DeviceMgmt,new { action = action,parameters =parameters ,devices=devices},true);
		}
		
		public dynamic DeleteDeviceManagementRequest(string requestId){
			return RestHandler("DELETE",string.Format(DeviceMgmtInd,requestId),null,false);
		}
		
		public dynamic GetDeviceManagementRequest(string requestId){
			return RestHandler("GET",string.Format(DeviceMgmtInd,requestId),null,true);
		}
		
		public dynamic GetDeviceManagementRequestStatus(string requestId){
			return RestHandler("GET",string.Format(DeviceMgmtStatus,requestId),null,true);
		}
		public dynamic GetDeviceManagementRequestStatus(string requestId,string type,string deviceId){
			return RestHandler("GET",string.Format(DeviceMgmtStatusInd,requestId,type,deviceId),null,true);
		}
		
		public dynamic GetDataUsage(string start, string end,bool detail){
			return RestHandler("GET",Usage,"?start="+start+"&end="+end+"&detail="+detail,true);
		}
		
		public dynamic GetServiceStatus(){
			return RestHandler("GET",ServiceStatus,null,true);
		}
		
		public dynamic GetLastEvents(string type, string deviceId){
			return RestHandler("GET",string.Format(LastEventCache,type,deviceId),null,true);
		}
		public dynamic GetLastEventsByEventType(string type, string deviceId,string eventType){
			return RestHandler("GET",string.Format(LastEventCacheInd,type,deviceId,eventType),null,true);
		}
		
		public dynamic GetDeviceLocationWeather (string type, string deviceId){
			return RestHandler("GET",string.Format(Weather,type,deviceId),null,true);
		}
		
		//TODO :All Extention functionalities

	}
}
