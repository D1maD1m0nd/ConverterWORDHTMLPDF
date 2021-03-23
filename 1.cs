namespace Terrasoft.Configuration
{
		using System;
	using System.Reflection;
	using System.Configuration;
	using System.ServiceModel;
	using System.ServiceModel.Web;
	using System.ServiceModel.Activation;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Web.Common;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;
	using System.Web;
	using System.Collections.ObjectModel;
	using System.Data;
	using Newtonsoft.Json;
	using Terrasoft.Core.DB;
	using System.Runtime.Serialization;
	using System.Xml;
	using AddressDadataFor1C;
	using AddressFor1c;
	using DocMoneyReceipt;
	using ActsAndImplementationOfServices;
	using InvoiceIssued;
	using DocumentInvoice;
	using System.Globalization;
	using Integration1СKommitent;
	using RubSum;
	using ServiceKommitent1C;
	using Contract1C;
	using Invoice1C;
	using Service1C;
	using Users1C;
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class WebService1C : BaseService {
		
		public WebService1C() {}
		public string Link1C = "";
		public WebService1C(UserConnection userConnection) {
			this._userConnection = userConnection;
		}
		
		private UserConnection _userConnection;
		protected override UserConnection UserConnection {
			get {
				if (_userConnection != null) {
					return _userConnection;
				}
				_userConnection = CurrentHttpContext.Session ["UserConnection"] as UserConnection;
				if (_userConnection != null) {
					return _userConnection;
				}
				var appConnection = (AppConnection)CurrentHttpContext.Application ["AppConnection"];
				_userConnection = appConnection.SystemUserConnection;
				return _userConnection;
			}
		}
        
        private HttpContextBase _httpContext;
		protected virtual HttpContextBase CurrentHttpContext {
			get { return _httpContext ?? (_httpContext = new HttpContextWrapper (HttpContext.Current)); }
			set { _httpContext = value; }
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostAccount1C(Guid idAcccount, bool itsContract = false){
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string accountName = "";
			string accountNameFull = "";
			string innStart = "";
			string inn1 = "";
			string inn2 = "";
			string kppStart = "";
			string kpp1 = "";
			string kpp2 = "";
			Guid countryId = new Guid();
			Guid typeId = new Guid();
			Guid statusAccountId = new Guid();
			string typeName1c = "";
			string countryId1C = "";
			string countryCode = "";
			string statusAccountName = "";
			string code1CinCRM = "";
			string guid1C = "";
			string method = "";
			string dopPath = "";
			string ogrn = "";
			string directorSID = "";
			string seriesAndNoOfRegistrationCertificate = "";
			DateTime dateRegSer = new DateTime();
			Guid primaryContactId = new Guid();
			string dateName = "";
			string historyKpp = "";
			string mobilePhone = "";
			string fax = "";
			
			string historyName = "";//todos
			string[] list = null;//todos
			string legalAddress = "";//todos
			
			var esqAccount = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Account");
			esqAccount.AddAllSchemaColumns();
			esqAccount.Filters.Add(esqAccount.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idAcccount));
			var accounts = esqAccount.GetEntityCollection(UserConnection);
			foreach (var accountItem in accounts) {
				accountName = accountItem.GetTypedColumnValue<string>("Name").Replace("\"", @"\" + "\"");
			//	AddRecordJournal($"{accountName} {idAcccount}",200);
				innStart = accountItem.GetTypedColumnValue<string>("qrtINN");
				kppStart =  accountItem.GetTypedColumnValue<string>("qrtKPP");
				accountNameFull = accountItem.GetTypedColumnValue<string>("AlternativeName").Replace("\"", @"\" + "\"");
				countryId = accountItem.GetTypedColumnValue<Guid>("CountryId");
				typeId = accountItem.GetTypedColumnValue<Guid>("TypeId");
				statusAccountId = accountItem.GetTypedColumnValue<Guid>("OwnershipId");
				code1CinCRM = accountItem.GetTypedColumnValue<string>("qrtCode1C");
				guid1C = accountItem.GetTypedColumnValue<string>("qrtGuid1c");
				ogrn = accountItem.GetTypedColumnValue<string>("qrtOGRN");
				directorSID = accountItem.GetTypedColumnValue<string>("qrtDirectorSID");
				seriesAndNoOfRegistrationCertificate = accountItem.GetTypedColumnValue<string>("qrtSeriesAndNoOfRegistrationCertificate");
				dateRegSer = accountItem.GetTypedColumnValue<DateTime>("qrtDateOfIssueOfCertificateOfRegistrationOfIP");
				primaryContactId = accountItem.GetTypedColumnValue<Guid>("PrimaryContactId");
				mobilePhone = accountItem.GetTypedColumnValue<string>("Phone");
				fax = accountItem.GetTypedColumnValue<string>("Fax");
			}
			
			var esqCountry = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Country");
			esqCountry.AddAllSchemaColumns();
			esqCountry.Filters.Add(esqCountry.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", countryId));
			var countryes = esqCountry.GetEntityCollection(UserConnection);
			foreach (var countryItem in countryes) {
				countryCode = countryItem.GetTypedColumnValue<string>("Code");
			}
			if(code1CinCRM != String.Empty){
				if(itsContract){
					
					if(String.IsNullOrEmpty(guid1C)){
						guid1C = GetJson("Catalog_Контрагенты", $"?$format=json&$filter=Code eq '{code1CinCRM}'");
						int indexOfChar = guid1C.IndexOf("Ref_Key");
					    guid1C = guid1C.Substring(indexOfChar + 11, 36).Replace("</d", "");
						foreach(var acc in accounts){
							acc.SetColumnValue("qrtGuid1cAbipa", guid1C);
						}
					}
						return guid1C;
				}
				method = "PATCH";
				//todos
				//Добавить ИНН контрагента в проверку
			//	AddRecordJournal($"Код црм {code1CinCRM}",200);
				
				string answer = GetAccountRecord(code1CinCRM, "Catalog_Контрагенты"); 
				list = answer.Split(new string[] {"*-"}, StringSplitOptions.None); // Парсинг символов
			//	AddRecordJournal($"List14  {list[14]}",200);

			//	AddRecordJournal($"{innStart} == {list[13]}",200);
//AddRecordJournal(string.Join(" ", list),200);
				
				if(string.IsNullOrEmpty(guid1C) || guid1C != list[0]){
					guid1C = list[0];	
				
				}
				
					//guid1C = GetIdRecord(code1CinCRM, "Catalog_Контрагенты");
					//return "Return: " + guid1C;
				
				
				if(guid1C.Length < 36){
					return "PostAccount1C: no GUID";
				}
				dopPath = "(guid'" + guid1C + "')";
			}
			else{
				method = "POST";
			}
			countryId1C = GetCountry(countryCode);
			if (countryId1C.Length != 36){
				return ".PostAccount1C: " + countryId1C;
			}
			
			if(accountName != "" && countryId1C.Length == 36){
				switch(typeId.ToString()){
					//Клиент
					case "03a75490-53e6-df11-971b-001d60e938c6":
						typeName1c = "9dcbcad5-c9fb-11e3-967a-902b349bd4bd";
						break;
					//Поставщик
					case "d34b9da2-53e6-df11-971b-001d60e938c6":
						typeName1c = "f1808275-c5fd-11e3-bead-902b341ad718";
						break;
					//Наша компания
					case "0d1b6593-500b-11e6-80d1-3640b5ae4b2f":
						typeName1c = "0d1b6593-500b-11e6-80d1-3640b5ae4b2f";
						break;
					//Партнер
					case "f2c0ce97-53e6-df11-971b-001d60e938c6":
						typeName1c = "5dbf5cdd-fe36-11ea-80cd-0cc47a8176e9";
						break;
					default:
						typeName1c = "00000000-0000-0000-0000-000000000000";
						break;
				}
				
				if(statusAccountId.ToString() == "321c60a3-689e-46f5-8bdf-a843cb9169a3"){
					statusAccountName = "ФизическоеЛицо";
				}
				else if(statusAccountId.ToString() == "86727d9e-68bd-4a69-bac8-cc7b857042ae"){
					statusAccountName = "ЮридическоеЛицо";
				}
				if(countryId.ToString() == "a570b005-e8bb-df11-b00f-001d60e938c6"){
					inn1 = innStart;
					kpp1 = kppStart;
					kpp2 = ogrn;
				}
				else{
					inn2 = innStart;
					kpp2 = kppStart;
				}
				if(dateRegSer.ToString() != ""){
					dateName = dateRegSer.ToString("yyyy-MM-ddT00:00:00");
				}
				
				string informationDelimeterFor1c = "";
				string contactInformation = "";
				Guid coountyId = new Guid();
				string coountyName = "";
				Guid cityId = new Guid();
				string cityName = "";
				Guid regionId = new Guid();
				string regionName = "";
				string adres = "";
				int countContactInformation = 1;
				string typeContactInformation = "";
				Guid typeAdres = new Guid();
				string dateAddress = "";
				string historyContactInfo = "";
				DateTime dateTodeyPart1 = DateTime.Now;
				string dateTodeyPart2 = dateTodeyPart1.ToString("yyyy-MM-ddT00:00:00");
				
				var esqContactInformation = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountAddress");
				esqContactInformation.AddAllSchemaColumns();
				esqContactInformation.Filters.Add(esqContactInformation.CreateFilterWithParameters(FilterComparisonType.Equal, "Account", idAcccount));
				esqContactInformation.Filters.Add(esqContactInformation.CreateFilterWithParameters(FilterComparisonType.Equal, "Primary", true));
				var contactInformations = esqContactInformation.GetEntityCollection(UserConnection);
				foreach (var contactInformationItem in contactInformations) {
					coountyId = contactInformationItem.GetTypedColumnValue<Guid>("CountryId");
					cityId = contactInformationItem.GetTypedColumnValue<Guid>("CityId");
					regionId = contactInformationItem.GetTypedColumnValue<Guid>("RegionId");
					adres = contactInformationItem.GetTypedColumnValue<string>("Address");
					typeAdres = contactInformationItem.GetTypedColumnValue<Guid>("AddressTypeId");
					informationDelimeterFor1c = contactInformationItem.GetTypedColumnValue<string>("qrtBreakdownAddress1C");
					dateAddress = contactInformationItem.GetTypedColumnValue<DateTime>("qrtValidFromDate").ToString("yyyy-MM-ddT00:00:00");
					var esqCityInformation = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "City");
					esqCityInformation.AddAllSchemaColumns();
					esqCityInformation.Filters.Add(esqCityInformation.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", cityId));
					var cityInformations = esqCityInformation.GetEntityCollection(UserConnection);
					foreach (var cityInformationItem in cityInformations) {
						cityName = cityInformationItem.GetTypedColumnValue<string>("Name");
					}
					var esqRegionInformation = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Region");
					esqRegionInformation.AddAllSchemaColumns();
					esqRegionInformation.Filters.Add(esqRegionInformation.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", regionId));
					var regionInformations = esqRegionInformation.GetEntityCollection(UserConnection);
					foreach (var regionInformationItem in regionInformations) {
						regionName = regionInformationItem.GetTypedColumnValue<string>("Name");
					}
					var esqCoountyInformation = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Country");
					esqCoountyInformation.AddAllSchemaColumns();
					esqCoountyInformation.Filters.Add(esqCoountyInformation.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", coountyId));
					var countryInformations = esqCoountyInformation.GetEntityCollection(UserConnection);
					foreach (var countryInformationItem in countryInformations) {
						coountyName = countryInformationItem.GetTypedColumnValue<string>("Name");
					}
					if(typeAdres.ToString() == "770bf68c-4b6e-df11-b988-001d60e938c6"){
						typeContactInformation = "b910ecef-9cc8-44e8-b0df-bc705acb83ed"; //id из 1с
					}
					else if(typeAdres.ToString() == "780bf68c-4b6e-df11-b988-001d60e938c6"){
						typeContactInformation = "4e115265-2211-42de-bc59-7d7deec26cde"; //id из 1с
					}
					if(typeContactInformation != String.Empty){
						contactInformation += GenerateAddressDataFor1C(informationDelimeterFor1c, adres,countContactInformation, typeContactInformation,coountyName);
						//contactInformation += "{\"LineNumber\": \"" + countContactInformation.ToString() + "\", \"Тип\": \"" + "Адрес" + "\", \"Страна\": \"" + coountyName + "\", \"Регион\": \"" + regionName + "\", \"Город\": \"" + cityName + "\", \"Представление\": \"" + adres + "\", \"ВидДляСписка_Key\": \"" + typeContactInformation + "\", \"Вид_Key\": \"" + typeContactInformation + "\"},";
						if(typeAdres.ToString() == "770bf68c-4b6e-df11-b988-001d60e938c6" && historyContactInfo == String.Empty){
							legalAddress = adres;//todo
						}
						countContactInformation++;
					}
				}
				
				Guid typeEmailId = new Guid("ee1c85c3-cfcb-df11-9b2a-001d60e938c6");
				string emailAccount = "";
				
				var esqAccountCommunication = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountCommunication");
				esqAccountCommunication.AddAllSchemaColumns();
				esqAccountCommunication.Filters.Add(esqAccountCommunication.CreateFilterWithParameters(FilterComparisonType.Equal, "CommunicationType", typeEmailId));
				esqAccountCommunication.Filters.Add(esqAccountCommunication.CreateFilterWithParameters(FilterComparisonType.Equal, "Account", idAcccount));
				var acountCommunications = esqAccountCommunication.GetEntityCollection(UserConnection);
				foreach (var acountCommunicationItem in acountCommunications) {
					emailAccount = acountCommunicationItem.GetTypedColumnValue<string>("Number");
				}
				
				contactInformation += "{\"LineNumber\": \"" + countContactInformation.ToString() + "\", \"Тип\": \"" + "Телефон" + "\", \"ВидДляСписка_Key\": \"" + "c5bb357f-c3b0-48ba-8a12-a42cbf99a845" + "\", \"Вид_Key\": \"" + "c5bb357f-c3b0-48ba-8a12-a42cbf99a845" + "\", \"Представление\": \"" + mobilePhone + "\"}, {\"LineNumber\": \"" + (countContactInformation + 1).ToString() + "\", \"Тип\": \"" + "Факс" + "\", \"ВидДляСписка_Key\": \"" + "3b6d7c29-7790-4139-8f2f-e22857acc623" + "\", \"Вид_Key\": \"" + "3b6d7c29-7790-4139-8f2f-e22857acc623" + "\", \"Представление\": \"" + fax + "\"}, {\"LineNumber\": \"" + (countContactInformation + 2).ToString() + "\", \"Тип\": \"" + "АдресЭлектроннойПочты" + "\", \"ВидДляСписка_Key\": \"" + "9448865d-d77c-4a6c-a459-44aa1eeae7b1" + "\", \"Вид_Key\": \"" + "9448865d-d77c-4a6c-a459-44aa1eeae7b1" + "\", \"Представление\": \"" + emailAccount + "\"}"; //id из 1с
				
				string DATA = "";
				
			
				if (method == "PATCH"){
					
					if (list[1] != accountNameFull){
						if (list[4] == "1"){
							historyName = "{\"LineNumber\": \"1\", \"Период\": \"0001-01-02T00:00:00\", \"НаименованиеПолное\": \"" + list[1] + "\"},{\"LineNumber\": \"2\", \"Период\": \"" + dateTodeyPart2 + "\", \"НаименованиеПолное\": \"" + accountNameFull + "\"}";
						} else if (list[1] != "1" && list[12] != "" && list[12] != dateTodeyPart2){
							int i = int.Parse(list[4]);
							historyName = list[6] + ",{\"LineNumber\": \"" + i + "\", \"Период\": \"" + dateTodeyPart2 + "\", \"НаименованиеПолное\": \"" + accountNameFull + "\"}";
						}
					}
					if (list[2] != kppStart && countryId.ToString() == "a570b005-e8bb-df11-b00f-001d60e938c6"){
						if (list[3] == "1"){
							historyKpp = "{\"LineNumber\": \"1\", \"Период\": \"0001-01-02T00:00:00\", \"КПП\": \"" + list[2] + "\"}, {\"LineNumber\": \"2\", \"Период\": \"" + dateTodeyPart2 + "\", \"КПП\": \"" + kppStart + "\"}";
						} else if (list[3] != "1" && list[11] != "" && list[11] != dateTodeyPart2){
							int i = int.Parse(list[3]);
							historyKpp = list[5] + ",{\"LineNumber\": \"" + i + "\", \"Период\": \"" + dateTodeyPart2 + "\", \"КПП\": \"" + kppStart + "\"}";
						}
					}
					
						if (list[7] != legalAddress){
							dateAddress = dateAddress == "0001-01-01T00:00:00" ? DateTime.Now.ToString("yyyy-MM-ddT00:00:00"): dateAddress;
							if (list[8] == "1"){
								
								historyContactInfo = (list[14] + ", {\"LineNumber\": \"2\", \"Период\": \"" + dateAddress + "\", \"Представление\": \"" + adres + "\", \"Вид_Key\": \"" + "b910ecef-9cc8-44e8-b0df-bc705acb83ed" + "\"}").Replace(",\r\n,",",");;
								// AddRecordJournal(list[14],200);
								// AddRecordJournal(historyContactInfo,200);
								// AddRecordJournal("---------------------Тесе",200);
							} else if (list[8] != "1" && list[10] != "" && list[10] != dateAddress){
		
								historyContactInfo = list[9] + ",{\"LineNumber\": \"" + list[8] + "\", \"Период\": \"" + dateAddress + "\", \"Представление\": \"" + legalAddress + "\", \"Страна\":\"" + coountyName + "\",\"Вид_Key\": \"" + "b910ecef-9cc8-44e8-b0df-bc705acb83ed" + "\"}";
							}
					}
				}//todos
               if(method == "PATCH"){
                	DATA = "{";
                	if(!String.IsNullOrEmpty(historyContactInfo)){
                		DATA += "\"КонтактнаяИнформация\": [" + contactInformation + "],";
                		DATA += "\"ИсторияКонтактнойИнформации\": [" + historyContactInfo + "\r\n],";
                		//AddRecordJournal("Значение контактной информации " + historyContactInfo, 200);
                	}
                	if(!String.IsNullOrEmpty(historyKpp)){
                		DATA += " \"РасширенноеПредставлениеКПП\": \"" + kpp1 + "\", \"ИНН\": \"" + list[13] + "\", \"КПП\": \"" + kpp1 + "\",";
                		DATA += "\"ИсторияКПП\": [" + historyKpp + "]}";
                	}
                	if(!String.IsNullOrEmpty(historyName)){
                		DATA += "\"ИсторияНаименований\": [" + historyName + "],";
                		DATA += "\"НаименованиеПолное\": \"" + accountNameFull + "\",";
                	}
                	int idx = DATA.LastIndexOf(",");
                	DATA = DATA.Remove(idx) + "}";
                	DATA = HistoryIncludeCorrect(DATA);
                }
	  			else{
	  				DATA = "{\"Description\": \"" + accountName.Replace(@"\",@"\\") + "\"," +
					" \"НаименованиеПолное\": \"" + accountNameFull + "\"," +
					" \"Predefined\":\"true\", " +
					"\"НалоговыйНомер\": \"" + inn2 + "\"," +
					" \"СвидетельствоСерияНомер\": \"" + seriesAndNoOfRegistrationCertificate + "\", " +
					"\"ДокументУдостоверяющийЛичность\": \"" + directorSID + "\", " +
					"\"СвидетельствоДатаВыдачи\": \"" + dateName + "\", " +
					"\"Parent_Key\": \"" + typeName1c + "\", " +
					"\"ЮридическоеФизическоеЛицо\": \"" + statusAccountName + "\", " +
					"\"РегистрационныйНомер\": \"" + kpp2 + "\", " +
					"\"СтранаРегистрации_Key\": \"" + countryId1C + "\", " +
					"\"РасширенноеПредставлениеИНН\": \"" + inn1 + "\", " +
					"\"РасширенноеПредставлениеКПП\": \"" + kpp1 + "\", \"ИНН\": \"" + inn1 + "\", \"КПП\": \"" + kpp1 + "\"," +
					" \"ИсторияНаименований\": [" + historyName + "]," +
					" \"КонтактнаяИнформация\": [" + contactInformation + "]," +
					"\"ИсторияКПП\": [" + historyKpp + "]}";
					
	  			}
	  	// 		AddRecordJournal("КонтактнаяИнформация",200);
				// AddRecordJournal(contactInformation,200);
				// AddRecordJournal("КонтактнаяИнформация",200);
				
				// AddRecordJournal("DATA",200);
	  	 		AddRecordJournal(DATA,200);
	  	// 		AddRecordJournal("DATA",200);
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Контрагенты" + dopPath);
				
			    request.Credentials = CredentialCache.DefaultCredentials;
			    request.Method = method;
			    request.ContentType = "application/json; charset=UTF-8";
			    request.Accept = "application/json";
			    request.Credentials = new NetworkCredential(login, password);
			    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
			    requestWriter.Write(DATA);
			    requestWriter.Close();
			    try
			    {
					WebResponse webResponse = request.GetResponse();
					Stream webStream = webResponse.GetResponseStream();
					StreamReader responseReader = new StreamReader(webStream);
					string response = responseReader.ReadToEnd();
					int indexOfChar = response.IndexOf("Ref_Key");
					string textRef = response.Substring(indexOfChar + 11, 36).Replace("</d", "");
					int indexCode = response.IndexOf("Code");
					string textCode = response.Substring(indexCode + 8, 9).Replace("</d", "");
					responseReader.Close();
				    
				    //
					HttpWebResponse we = (HttpWebResponse)request.GetResponse();
					string message = "BPM -> 1C: " + "Контрагент отправлен (" + (int)we.StatusCode + ")";
					//AddRecordJournal(message,(int)we.StatusCode);
					//
				    
				    var esqAccount2 = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Account");
					esqAccount2.AddAllSchemaColumns();
					esqAccount2.Filters.Add(esqAccount2.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idAcccount));
					var accounts2 = esqAccount2.GetEntityCollection(UserConnection);
					foreach (var accountItem2 in accounts2) {
						accountItem2.SetColumnValue("qrtCode1C", textCode);
						accountItem2.SetColumnValue("qrtGuid1c", textRef);
						accountItem2.Save(false);
					}
					// string contactCustom = "";
					// if(primaryContactId != Guid.Empty && textRef != String.Empty){
					// 	contactCustom = GetContact(primaryContactId, textRef);
					// }
					return textRef;
			    }
			    catch (WebException e)
			    {
			    	//
					var we = e.Response as HttpWebResponse;
						string message = "BPM -> 1C: " + "Контрагент не отправлен (" + (int)we.StatusCode + ") " + accountName + " " + idAcccount.ToString() + " " + new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
					AddRecordJournal(message,(int)we.StatusCode);
					//
					string error = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
					return "Error:" + error + "//" + DATA;
			    }
			}
		    return "-1";
		}
		//-
		public string HistoryIncludeCorrect(string response)
        {
            //Срез истории контактной информации
            string soStartHistoryContactInfo = "\"ИсторияКонтактнойИнформации\": [";
            string soEndHistoryContactInfo = "}\r\n]";
            int startIndexHistoryContactInfo = response.IndexOf(soStartHistoryContactInfo);
             if (startIndexHistoryContactInfo == -1)
            {
                return response;
            }
            string subStrHistoryContactInfo = response.Substring(startIndexHistoryContactInfo);
            int endHistoryContactInfo = subStrHistoryContactInfo.IndexOf(soEndHistoryContactInfo);
            if(endHistoryContactInfo == -1){
            	return response;
            }
            //Console.WriteLine(endHistoryContactInfo);
            string historySubstring = response.Substring(startIndexHistoryContactInfo, endHistoryContactInfo + soEndHistoryContactInfo.Length);
            string[] hisortObj = historySubstring.Split(new string[] { "LineNumber" }, StringSplitOptions.None);
            //срез контактной информации
            string soStart = "<КонтактнаяИнформация xmlns";
            string soEnd = "</КонтактнаяИнформация>";
            foreach (var item in hisortObj)
            {
                
              

                int startIndex = item.IndexOf(soStart);
                if (startIndex == -1)
                {
                    continue;
                }
                string subStr = item.Substring(startIndex);

                int endIndex = subStr.IndexOf(soEnd);
                string resultStr = item.Substring(startIndex, endIndex + soEnd.Length);
                string contactHistoryReplace = resultStr.Replace("http://www.v8.1c.ru/ssl/contactinfo", WebUtility.UrlEncode("http://www.v8.1c.ru/ssl/contactinfo"))
                                                        .Replace("http://www.w3.org/2001/XMLSchema", WebUtility.UrlEncode("http://www.w3.org/2001/XMLSchema"))
                                                        .Replace("http://www.w3.org/2001/XMLSchema-instance", WebUtility.UrlEncode("http://www.w3.org/2001/XMLSchema-instance"))
                                                        .Replace(@"\r\n", string.Empty)
                                                        .Replace(@"\", string.Empty)
                                                        .Replace("\"", "\\\"");
                response = response.Replace(resultStr, contactHistoryReplace);
            }
            
            //AddRecordJournal("----------------------Ответ" + response,200);
            return response;
        }
		//-@code - код из 1с
		// @page - страница для поиска
		// @copyCode - Случай если есть дубли по коду 1с
		// @inn - инн контрагента, используется только в случае, если найдены дубли кодов 1с
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetAccountRecord(string code, string page) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string[] nameCount;

			string parametrs = "";
			string responseCashe = "";
			string response = "";

		    parametrs = $"?$format=json&$filter=Code eq '{code}'";
		    var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Контрагенты" + parametrs);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Credentials = new NetworkCredential(login, password);
		    WebResponse webResponse = request.GetResponse();
		    Stream webStream = webResponse.GetResponseStream();
		    StreamReader responseReader = new StreamReader(webStream);
		    response = responseReader.ReadToEnd();
		    responseReader.Close();
			//response = HistoryIncludeCorrect(response);
			
		    responseCashe = response.Replace(": ", ",");
		    
		    nameCount = responseCashe.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			//get Ref_Key
			if (nameCount.Length > 9){

                
                string contactInfo = "";
                //Первая запись контактной информации, нужна что бы передавать значение в историю при второй передаче
                if (response.IndexOf("КонтактнаяИнформация\": []") == -1 )
                {
                    contactInfo = response.Split(new string[] { "КонтактнаяИнформация\": [" },
                        StringSplitOptions.RemoveEmptyEntries)[1]; // Парсинг символов
                    string refKeyContactInfoSubstring = contactInfo.Substring(contactInfo.IndexOf("{\r\n\"Ref_Key\": \""), 12);
                    contactInfo = contactInfo.Split(new string[] { "{\r\n\"Ref_Key\"" }, StringSplitOptions.None)[1];
                    contactInfo = refKeyContactInfoSubstring + contactInfo;
                }
                
                //История ИсторияНаименований
                string nameList = response.Split(new string[] { "ИсторияНаименований\": [" }, StringSplitOptions.RemoveEmptyEntries)[1]; // Парсинг символов
                nameList = nameList.Split(new string[] { "]" }, StringSplitOptions.None)[0];
                int historyNameCount = nameList.Split(new string[] { "LineNumber" }, StringSplitOptions.None).Length;
                //История КПП
                string array = response.Split(new string[] { "ИсторияКПП\": [" }, StringSplitOptions.RemoveEmptyEntries)[1]; // Парсинг символов
                array = array.Split(new string[] { "]" }, StringSplitOptions.None)[0];
                int historyKPPcount = array.Split(new string[] { "LineNumber" }, StringSplitOptions.None).Length;
                //ИсторияКонтактнойИнформации  
                string historyInfo = response.Split(new string[] { "ИсторияКонтактнойИнформации\": [" }, StringSplitOptions.RemoveEmptyEntries)[1]; // Парсинг символов
                historyInfo = historyInfo.Split(new string[] { "}\r\n]," }, StringSplitOptions.None)[0] + "}";
                int historyInfoCount = historyInfo.Split(new string[] { "LineNumber" }, StringSplitOptions.None).Length;
                //Адрес
                string address = "";
                string period = "";
                if (historyInfoCount != 1)
                {
                    address = historyInfo.Split(
                        new string[] {"LineNumber\": \"" + (historyInfoCount - 1).ToString() + "\","},
                        StringSplitOptions.None)[1];
                    address = address.Split(new string[] {"\"Представление\": \""}, StringSplitOptions.None)[1];
                    address = address.Split(new string[] {"\","}, StringSplitOptions.None)[0];

                    //Период
                    
                    if (historyInfo.IndexOf("Период") != -1)
	                {
	                    period = historyInfo.Split(
	                        new string[] { "LineNumber\": \"" + (historyInfoCount - 1).ToString() + "\"," },
	                        StringSplitOptions.None)[1];
	                    period = period.Split(new string[] { "\"Период\": \"" }, StringSplitOptions.None)[1];
	                    period = period.Split(new string[] { "\"," }, StringSplitOptions.None)[0];
	                }
                }

                //Период ИсторияКПП
                string period2 = "";
                if (historyKPPcount > 1)
                {
                    period2 = array.Split(new string[] { "LineNumber\": \"" + (historyKPPcount - 1).ToString() + "\"," }, StringSplitOptions.None)[1];
                    period2 = period2.Split(new string[] { "\"Период\": \"" }, StringSplitOptions.None)[1];
                    period2 = period2.Split(new string[] { "\"," }, StringSplitOptions.None)[0];
                }
                //Период ИсторияНаименований
                string period3 = "";
                if (historyNameCount > 1)
                {
                    period3 = nameList.Split(new string[] { "LineNumber\": \"" + (historyNameCount - 1).ToString() + "\"," }, StringSplitOptions.None)[1];
                    period3 = period3.Split(new string[] { "\"Период\": \"" }, StringSplitOptions.None)[1];
                    period3 = period3.Split(new string[] { "\"," }, StringSplitOptions.None)[0];
                }
                //НаименованиеПолное
                string nameCompany = response.Split(new string[] { "НаименованиеПолное\": \"" }, StringSplitOptions.None)[1];
                nameCompany = nameCompany.Split(new string[] { "\"," }, StringSplitOptions.None)[0];
                //Ref_Key
                string refKey = nameCount[4].Trim(new char[] { '"' });
                //КПП
                string kpp = nameCount[30].Trim(new char[] { '"' });
                //ИНН 
                string inn = nameCount[28].Trim(new char[] { '"' });
                array = array.Replace("\r\n", "");
                nameList = nameList.Replace("\r\n", "");
                historyInfo = historyInfo.Replace("\r\n", "");

                //Ref_Key, НаименованиеПолное, КПП, ИсторияКПП(int), ИсторияНаименований(int),ИсторияКПП(list), ИсторияНаименований(list), ИсторияКонтактнойИнформации, ИсторияКонтактнойИнформации(int), ИсторияКонтактнойИнформации(list), Период
                string list = refKey + "*-" + nameCompany + "*-" + kpp + "*-" + historyKPPcount + "*-" +
                historyNameCount + "*-" + array + "*-" + nameList + "*-" + address + "*-" +
                historyInfoCount + "*-" + historyInfo + "*-" + period + "*-" + period2 + "*-" + period3 + "*-" + inn + "*-" + contactInfo;
				return list;
				
			}
			
			/*
			
			int indexRef = response.IndexOf("Ref_Key");
			if(indexRef != -1){
				string textRef = response.Substring(indexRef + 11, 36);
				return textRef;
			}
			*/
			return "-1";
		}
		//-
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostAccount1CForJs(string idAcccountStr){
			Guid id = new Guid(idAcccountStr);
			string result = PostAccount1C(id);
			return result;
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostAccountBankInvoice1C(string idAccount1c, string idBankInvoice){
			
				string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
				string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
				string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
				
	  			string DATA = "{\"ОсновнойБанковскийСчет_Key\": \"" + idBankInvoice + "\"}";
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Контрагенты" + "(guid'" + idAccount1c + "')");
			    request.Credentials = CredentialCache.DefaultCredentials;
			    request.Method = "PATCH";
			    request.ContentType = "application/json; charset=UTF-8";
			    request.Accept = "application/json";
			    request.Credentials = new NetworkCredential(login, password);
			    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
			    requestWriter.Write(DATA);
			    requestWriter.Close();
			    try
			    {
					WebResponse webResponse = request.GetResponse();
					Stream webStream = webResponse.GetResponseStream();
					StreamReader responseReader = new StreamReader(webStream);
					string response = responseReader.ReadToEnd();
					int indexOfChar = response.IndexOf("Ref_Key");
					string textRef = response.Substring(indexOfChar + 11, 36);
					responseReader.Close();
					
					//
					HttpWebResponse we = (HttpWebResponse)request.GetResponse();
					string message = "BPM -> 1C: " + "Контрагент обновлен (" + (int)we.StatusCode + ")";
					AddRecordJournal(message,(int)we.StatusCode);
					//
					
					return textRef;
			    }
			    catch (WebException e)
			    {
			    	//
					var we = e.Response as HttpWebResponse;
					string message = "BPM -> 1C: " + "Контрагент не обновлен (" + (int)we.StatusCode + ")";
					AddRecordJournal(message,(int)we.StatusCode);
					//
			      //return e.Message;
			      return "error";
			    }
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetIdRecord(string code, string page) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs = "?$filter=Code eq '" + code + "'" + "&$format=json";
			var request = (HttpWebRequest)WebRequest.Create(path + page + parametrs);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			responseReader.Close();
			int indexRef = response.IndexOf("Ref_Key");
			if(indexRef != -1){
				string textRef = response.Substring(indexRef + 11, 36);
				return textRef;
			}
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetContractForJs(string idContractStr) {
			Guid id = new Guid(idContractStr);
			string result = GetContract(id);
			return result;
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetContract(Guid idContract) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string contractGuidName = "";
			string contractCodeName = "";
			
			var esqContract = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
			esqContract.AddAllSchemaColumns();
			esqContract.Filters.Add(esqContract.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idContract));
			var contractes = esqContract.GetEntityCollection(UserConnection);
			foreach (var contractItem in contractes) {
				contractGuidName = contractItem.GetTypedColumnValue<string>("qrtGuid1c");
				contractCodeName = contractItem.GetTypedColumnValue<string>("qrt1SCode");
			}
			
			if(contractCodeName != String.Empty){
				if(contractGuidName == String.Empty){
					contractGuidName = GetIdRecord(contractCodeName, "Catalog_ДоговорыКонтрагентов");
				}
				/*
				if(contractGuidName == "-1"){
					return "-1";
				}
				*/
				if(contractGuidName.Length != 36){
					return "contractGuidName: " + contractGuidName;
				}
				string parametrs = "(guid'" + contractGuidName + "')";
				var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_ДоговорыКонтрагентов" + parametrs);
				request.Credentials = CredentialCache.DefaultCredentials;
	            request.Credentials = new NetworkCredential(login, password);
	            WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				
				//
				responseReader.Close();
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Договор получен (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				//
				
	      		if(indexRef != -1){
	      			string textRef = PostContract(idContract, "PATCH", contractGuidName);
	      			return "1)" + textRef;
	      		}
	      		else{
	      			string newTextRef = PostContract(idContract, "POST", "");
					return "2)" + newTextRef;
	      		}
			}
			else{
				string newTextRef2 = PostContract(idContract, "POST", "");
				return "3)" + newTextRef2;
			}
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostContract(Guid idContract, string method, string guid1C) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string abipa = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtAbipa1C");
			string abipaCustom = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtAbipaCustom1C");
			
			Guid customer = new Guid();
			Guid supplier = new Guid();
			Guid typeContract = new Guid();
			Guid paymentCurrency = new Guid();
			Guid defermentOfPayment = new Guid();
			Guid fioMainContract = new Guid();
			Guid genderId = new Guid();
			Guid stateContract = new Guid();
			Guid cyrrencyPayment2 = new Guid();
			Guid AccountOther = new Guid();
			Guid AccountOtherPaymant = new Guid();
			DateTime date = new DateTime();
			DateTime dateEnd = new DateTime();
			string number = "";
			string posotionContract = "";
			string basedCContract = "";
			string genderName = "";
			string refCustomer = "";
			string refSupplier = "";
			string fioMainContractName = "";
			string calculationsInConditionalUnits = "";
			string dateName = "";
			string dateEndName = "";
			string defermentOfPaymentName = "";
			string currenCurrencyy = "";
			string phifixName = "";
			string resultAccount = "";
			string resultOrganization = "";
			string typeContractFor1c = "";
			string subjectStr = "";
			string paymantInCurrency = "";
			string theContractSigned = "";
			string currencyRateMarkup = "";
			string fioAccountId = ""; //Todo
			
			//Золотарев(Catalog_ФизическиеЛица)
			string phifixName1 = "fe051d9a-91ee-11e6-80d1-8ac3d8faa466"; //id из 1с
			//Андросов(Catalog_ФизическиеЛица)
			string phifixName2 = "777fb6ef-bf02-11e3-9621-d850e63b96c4"; //id из 1с
			
			var esqContract = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
			esqContract.AddAllSchemaColumns();
			esqContract.Filters.Add(esqContract.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idContract));
			var contractes = esqContract.GetEntityCollection(UserConnection);
			foreach (var contractItem in contractes) {
				customer = contractItem.GetTypedColumnValue<Guid>("qrtCustomerId");
				supplier = contractItem.GetTypedColumnValue<Guid>("qrtSupplierId");
				typeContract = contractItem.GetTypedColumnValue<Guid>("qrtCategoryId");
				paymentCurrency = contractItem.GetTypedColumnValue<Guid>("qrtPaymentCurrencyId");
				defermentOfPayment = contractItem.GetTypedColumnValue<Guid>("qrtDefermentOfPaymentId");
				fioMainContract = contractItem.GetTypedColumnValue<Guid>("qrtFIOId");
				stateContract = contractItem.GetTypedColumnValue<Guid>("qrtStateId");
				cyrrencyPayment2 = contractItem.GetTypedColumnValue<Guid>("qrtCurrencyId");
				AccountOther = contractItem.GetTypedColumnValue<Guid>("qrtAccountTheThirdSideId");
				AccountOtherPaymant = contractItem.GetTypedColumnValue<Guid>("qrtThirdPartyCounterpartyDetailsId");
				date = contractItem.GetTypedColumnValue<DateTime>("qrtDate");
				dateEnd = contractItem.GetTypedColumnValue<DateTime>("qrtEndDate");
				number = contractItem.GetTypedColumnValue<string>("qrtName");
				posotionContract = contractItem.GetTypedColumnValue<string>("qrtPosition");
				basedCContract = contractItem.GetTypedColumnValue<string>("qrtBased");
				subjectStr = contractItem.GetTypedColumnValue<string>("qrtSubjectStr");
				currencyRateMarkup = (contractItem.GetTypedColumnValue<float>("qrtCurrencyRateMarkup")).ToString();
			}
			
			if(stateContract.ToString() == "6173800d-8966-487e-aeb7-d68b278c58f8"){
				theContractSigned = "true";
			}
			else{
				theContractSigned = "false";
			}
			
			if (fioMainContract != Guid.Empty){ //todo
				var esqContact = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
				esqContact.AddAllSchemaColumns();
				esqContact.Filters.Add(esqContact.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", fioMainContract));
				var contates = esqContact.GetEntityCollection(UserConnection);
				foreach (var contactItem in contates) {
					genderId = contactItem.GetTypedColumnValue<Guid>("GenderId");
					fioMainContractName = contactItem.GetTypedColumnValue<string>("Name");
					fioAccountId = contactItem.GetTypedColumnValue<string>("AccountId"); //Todo
				}
				
				var esqGender = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Gender");
				esqGender.AddAllSchemaColumns();
				esqGender.Filters.Add(esqGender.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", genderId));
				var genders = esqGender.GetEntityCollection(UserConnection);
				foreach (var genderItem in genders) {
					genderName = genderItem.GetTypedColumnValue<string>("Name");
				}
				
				string fioAccountResult = PostAccount1C(new Guid(fioAccountId), true); //Todo
				string contactREsult = GetContact(fioMainContract, fioAccountResult); //Todo
				
				var esqDefermentOfPayment = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtDefermentOfPayment");
				esqDefermentOfPayment.AddAllSchemaColumns();
				esqDefermentOfPayment.Filters.Add(esqDefermentOfPayment.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", defermentOfPayment));
				var esqDefermentOfPayments = esqDefermentOfPayment.GetEntityCollection(UserConnection);
				foreach (var defermentOfPaymentItem in esqDefermentOfPayments) {
					defermentOfPaymentName = defermentOfPaymentItem.GetTypedColumnValue<string>("Name");
				}
			}//todo end
			
			string dopPath = "";
			if(method == "PATCH"){
				dopPath = "(guid'" + guid1C + "')";
			}
			
			refCustomer = PostAccount1C(customer, true);
			
			refSupplier = PostAccount1C(supplier, true);
			
			if(refCustomer.Length == 36){
				string statusGetBankInvoiceCustomer = GetBankInvoice(idContract, 1, refCustomer, "qrtContract");
				if(statusGetBankInvoiceCustomer != "-1" && statusGetBankInvoiceCustomer != "error" && customer.ToString() != abipa && customer.ToString() != abipaCustom){
					string PostAccountBankInvoice1CResultCustomer = PostAccountBankInvoice1C(refCustomer, statusGetBankInvoiceCustomer);
				}
			}
			else{
				return "PostContract.Customer: " + refCustomer;
			}
			
			if(refSupplier.Length == 36){
				string statusGetBankInvoiceSupplier = GetBankInvoice(idContract, 2, refSupplier,"qrtContract");
				if(statusGetBankInvoiceSupplier != "-1" && statusGetBankInvoiceSupplier != "error" && supplier.ToString() != abipa && supplier.ToString() != abipaCustom){
					string PostAccountBankInvoice1CResultSupplier = PostAccountBankInvoice1C(refSupplier, statusGetBankInvoiceSupplier);
				}
			}
			else{
				return "PostContract.Supplier: " + refSupplier;
			}
			//"5fb76920-53e6-df11-971b-001d60e938c6" - Рубль
			if(paymentCurrency.ToString() == "5fb76920-53e6-df11-971b-001d60e938c6" && cyrrencyPayment2.ToString() == "5fb76920-53e6-df11-971b-001d60e938c6"){
				calculationsInConditionalUnits = "false";
				currenCurrencyy = "false";
				paymantInCurrency = "false";
			}
			else if(paymentCurrency.ToString() != "5fb76920-53e6-df11-971b-001d60e938c6" && cyrrencyPayment2.ToString() != "5fb76920-53e6-df11-971b-001d60e938c6"){
				calculationsInConditionalUnits = "false";
				currenCurrencyy = "true";
				paymantInCurrency = "true";
			}
			else{
				calculationsInConditionalUnits = "true";
				currenCurrencyy = "true";
				paymantInCurrency = "true";
			}
			
			if(date.ToString() != ""){
				dateName = date.ToString("yyyy-MM-ddT00:00:00");
			}
			
			if(dateEnd.ToString() != ""){
				dateEndName = dateEnd.ToString("yyyy-MM-ddT00:00:00");
			}
			//С покупателем, С комитентом (принципалом) на закупку
			if(typeContract.ToString() == "9e085bd0-b7a3-46dc-a76f-ed7f9b3dc9fe" || typeContract.ToString() == "084958ff-522d-44b1-a9e5-2b9a5c0aa89f"){
				resultAccount = refCustomer;
				
				if(supplier.ToString() == abipa){
					resultOrganization = "becb74a9-a84a-11e7-af3b-ccb0daad0804"; //id из 1с
					phifixName = phifixName1;
				}
				else if(supplier.ToString() == abipaCustom){
					resultOrganization = "77050c9a-befb-11e3-9621-d850e63b96c4"; //id из 1с
					phifixName = phifixName2;
				}
				if(typeContract.ToString() == "9e085bd0-b7a3-46dc-a76f-ed7f9b3dc9fe"){
					typeContractFor1c = "СПокупателем";
				}
				else{
					typeContractFor1c = "СКомитентомНаЗакупку";
				}
			}
			else{
				resultAccount = refSupplier;
				if(customer.ToString() == abipa){
					//(Catalog_Организации) ООО "АБИПА"
					resultOrganization = "becb74a9-a84a-11e7-af3b-ccb0daad0804";
					phifixName = phifixName1;
				}
				else if(customer.ToString() == abipaCustom){
					//(Catalog_Организации) ООО "АБИПА КАСТОМС"
					resultOrganization = "77050c9a-befb-11e3-9621-d850e63b96c4";
					phifixName = phifixName2;
				}
				switch (typeContract.ToString()){
					case "c7a2112b-6351-4d68-b24f-9963cb0b27c9":
						typeContractFor1c = "СПоставщиком";
					
						break;
					case "24900c3f-686d-4f1d-9c34-b9fd82dcd69f":
						typeContractFor1c = "СКомитентом";
						break;
					case "9f0c7db3-d4b9-45af-93fb-0e2348b052ee":
						typeContractFor1c = "СКомиссионеромНаЗакупку";
						break;
					default:
						typeContractFor1c = "Прочее";
						break;
				}
				// //С поставщиком
				// if(typeContract.ToString() == "c7a2112b-6351-4d68-b24f-9963cb0b27c9"){
				// 	typeContractFor1c = "СПоставщиком";
				// }
				// //С комитентом (принципалом) на продажу
				// else if(typeContract.ToString() == "24900c3f-686d-4f1d-9c34-b9fd82dcd69f"){
				// 	typeContractFor1c = "СКомитентом";
				// }
				// //С комиссионером (агентом) на закупку
				// else if(typeContract.ToString() == "9f0c7db3-d4b9-45af-93fb-0e2348b052ee"){
				// 	typeContractFor1c = "СКомиссионеромНаЗакупку";
				// }
				// else{
				// 	typeContractFor1c = "Прочее";
				// }
			}
			
			string typeOfSettlementName = "";
			if(subjectStr != String.Empty){
				string typeOfSettlementId = GetTypeOfSettlement(subjectStr);
				if(typeOfSettlementId != "-1" || typeOfSettlementId != "error"){
					typeOfSettlementName = typeOfSettlementId;
				}
				else{
					typeOfSettlementName = "00000000-0000-0000-0000-000000000000";
				}
			}
			else{
				typeOfSettlementName = "00000000-0000-0000-0000-000000000000";
			}
			
			string currencyCodeName = "";
			string currencyCodeName2 = "";
			string refCurrency = "";
			string currencyRateMarkup2 = "";
			
			var esqCurrency = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Currency");
			esqCurrency.AddAllSchemaColumns();
			esqCurrency.Filters.Add(esqCurrency.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", cyrrencyPayment2));
			var currencyes = esqCurrency.GetEntityCollection(UserConnection);
			foreach (var currencyItem in currencyes) {
				currencyCodeName = currencyItem.GetTypedColumnValue<string>("ShortName");
			}

			if((!string.IsNullOrEmpty(currencyRateMarkup) && currencyRateMarkup != "0") && (cyrrencyPayment2.ToString() != "5fb76920-53e6-df11-971b-001d60e938c6")){
			    currencyRateMarkup = currencyRateMarkup.Replace(",", ".");
			    int indexComma = currencyRateMarkup.IndexOf(".");
			    int currencyRateMarkupLength = currencyRateMarkup.Length;
			    if(indexComma != -1 && currencyRateMarkupLength  - (indexComma + 1) == 2){
			      currencyRateMarkup2 = currencyRateMarkup.Substring(0, currencyRateMarkupLength-1);
			    }
			    else{
			    	currencyRateMarkup2 = currencyRateMarkup;
			    }

				currencyCodeName2 = currencyCodeName + "+" + currencyRateMarkup2 + "%";

				
			}
		
			
			if(currencyCodeName2 != String.Empty){
				refCurrency = GetCurrencyDescription(currencyCodeName2);
				if(refCurrency == "-1"){
					if(currencyCodeName != String.Empty){
						refCurrency = GetCurrencyDescription(currencyCodeName);
						if(refCurrency == "-1"){
							refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
						}
					}
					else{
						refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
					}
				}
			}
			else{
				if(currencyCodeName != String.Empty){
					refCurrency = GetCurrencyDescription(currencyCodeName);
					if(refCurrency == "-1"){
						refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
					}
				}
				else{
					refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
				}
			}
			
			
			string dateName2 = date.ToString("dd.MM.yyyy");
			////Случай обработки системного символа \ при формировании json в номере договора
			
			int resultSearchSystemCharNumber = number.IndexOf(@"\");
			if(resultSearchSystemCharNumber != -1){
				number = number.Replace(@"\", @"\\");
			}
			//string DATA = "{\"Description\": \"" + number + " от " + dateName + "\", \"ВидДоговора\": \"" + typeContractFor1c + "\", \"НДСПоСтавкам4и2\": \"" + "false" + "\", \"ДолжностьРуководителяКонтрагента\": \"" + posotionContract + "\", \"Валютный\": \"" + currenCurrencyy + "\", \"Номер\": \"" + number + "\", \"УчетАгентскогоНДС\": \"" + "false" + "\", \"ПроцентКомиссионногоВознаграждения\": \"" + "0" + "\", \"Руководитель_Key\": \"" + phifixName + "\", \"IsFolder\": \"" + "false" + "\", \"DeletionMark\": \"" + "false" + "\", \"ДолжностьРуководителя_Key\": \"" + "7d02b19c-192d-11e3-a1cc-c86000df0d7b" + "\", \"Owner_Key\": \"" + resultAccount + "\", \"Организация_Key\": \"" + resultOrganization + "\"}";
			string DATA = "{\"Description\": \"" + number + " от " + dateName2 + "\","+
			"\"ВидДоговора\": \"" + typeContractFor1c + "\","+
			"\"НДСПоСтавкам4и2\": \"" + "false" + "\","+
			"\"ВидВзаиморасчетов_Key\": \"" + typeOfSettlementName + "\","+
			"\"ПолРуководителяКонтрагента\": \"" + genderName + "\","+
			"\"СчетаФактурыОтИмениОрганизации\": \"" + "false" + "\","+
			"\"ПлатежныйАгент\": \"" + "false" + "\","+
			"\"ДоговорПодписан\": \"" + theContractSigned + "\","+
			"\"ДолжностьРуководителяКонтрагента\": \"" + posotionContract + "\","+
			"\"РуководительКонтрагента\": \"" + fioMainContractName + "\","+
			"\"ЗаРуководителяКонтрагентаПоПриказу\": \"" + basedCContract + "\","+
			"\"ВалютаВзаиморасчетов_Key\": \"" + refCurrency + "\","+
			"\"Валютный\": \"" + currenCurrencyy + "\","+
			"\"ОплатаВВалюте\": \"" + paymantInCurrency + "\","+
			"\"Дата\": \"" + dateName + "\","+
			"\"СрокДействия\": \"" + dateEndName + "\","+
			"\"СрокОплаты\": \"" + defermentOfPaymentName + "\","+
			"\"Номер\": \"" + number + "\","+
			"\"УчетАгентскогоНДС\": \"" + "false" + "\","+
			"\"УстановленСрокОплаты\": \"" + "true" + "\","+
			"\"УдалитьРеализацияНаЭкспорт\": \"" + "false" + "\","+
			"\"ИспользуетсяПриОбменеДанными\": \"" + "false" + "\","+
			"\"Predefined\": \"" + "false" + "\","+
			"\"РасчетыВУсловныхЕдиницах\":  \"" + calculationsInConditionalUnits + "\","+
			"\"СпособЗаполненияСтавкиНДС\": \"" + "Автоматически" + "\","+
			"\"ПроцентКомиссионногоВознаграждения\": \"" + "0" + "\","+
			"\"Руководитель_Key\": \"" + phifixName + "\","+
			"\"ДолжностьРуководителя_Key\": \"" + "7d02b19c-192d-11e3-a1cc-c86000df0d7b" + "\","+
			"\"IsFolder\": \"" + "false" + "\","+
			"\"DeletionMark\": \"" + "false" + "\","+
			"\"Owner_Key\": \"" +  resultAccount+ "\","+
			"\"Организация_Key\": \"" +  resultOrganization + "\"}";
		    
		    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_ДоговорыКонтрагентов" + dopPath);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = method;
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
		      WebResponse webResponse = request.GetResponse();
		      Stream webStream = webResponse.GetResponseStream();
		      StreamReader responseReader = new StreamReader(webStream);
		      string response = responseReader.ReadToEnd();
		      int indexRef = response.IndexOf("Ref_Key");
			  string textRef = response.Substring(indexRef + 11, 36);
			  int indexCode = response.IndexOf("Code");
			  string textCode = response.Substring(indexCode + 8, 9);
		      responseReader.Close();
		      
		      var esqContractVn = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
			  esqContractVn.AddAllSchemaColumns();
			  esqContractVn.Filters.Add(esqContractVn.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idContract));
			  var contractesVn = esqContractVn.GetEntityCollection(UserConnection);
			  foreach (var contractVnItem in contractesVn) {
				  contractVnItem.SetColumnValue("qrt1SCode", textCode);
				  contractVnItem.SetColumnValue("qrtGuid1c", textRef);
				  contractVnItem.Save(false);
			  }
			  
			  string codeTechnoContract = "";
			  string commenttechnoContract = "";
			  string guid1cTechnoContract = "";
			  string result = "";
			  string nameContractTechno = "";
			  Guid technoContractCategoryFor1C = new Guid();
			  Guid cyrrencyTechnoContract = new Guid();
			  Guid idTechno = new Guid();
			  
			  var esqContractTechno = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
			  esqContractTechno.AddAllSchemaColumns();
			  esqContractTechno.Filters.Add(esqContractTechno.CreateFilterWithParameters(FilterComparisonType.Equal, "qrtContract", idContract));
			  var contractesTechno = esqContractTechno.GetEntityCollection(UserConnection);
			  foreach (var contractTechnoItem in contractesTechno) {
			  	  nameContractTechno = contractTechnoItem.GetTypedColumnValue<string>("qrtName");
				  codeTechnoContract = contractTechnoItem.GetTypedColumnValue<string>("qrtCode1C");
				  commenttechnoContract = contractTechnoItem.GetTypedColumnValue<string>("qrtCommentsFor1C");
				  guid1cTechnoContract = contractTechnoItem.GetTypedColumnValue<string>("qrtGuid1c");
				  technoContractCategoryFor1C = contractTechnoItem.GetTypedColumnValue<Guid>("qrtContractCategoryFor1CId");
				  cyrrencyTechnoContract = contractTechnoItem.GetTypedColumnValue<Guid>("qrtCurrencyId");
				  idTechno = contractTechnoItem.GetTypedColumnValue<Guid>("Id");
				  
				  if(codeTechnoContract != String.Empty){
				  	if(guid1cTechnoContract == String.Empty){
					  	guid1cTechnoContract = GetIdRecord(codeTechnoContract, "Catalog_ДоговорыКонтрагентов");
					}
					if(guid1cTechnoContract == "-1"){
						return "-1";
					}
					result = PostContractTecho(idContract, "PATCH", commenttechnoContract, technoContractCategoryFor1C, cyrrencyTechnoContract, paymentCurrency, guid1cTechnoContract, idTechno, AccountOther, AccountOtherPaymant,nameContractTechno);
				  }
				  else{
					result = PostContractTecho(idContract, "POST", commenttechnoContract, technoContractCategoryFor1C, cyrrencyTechnoContract, paymentCurrency, "", idTechno, AccountOther, AccountOtherPaymant,nameContractTechno);
				  }
			  }
			  //
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM -> 1C: " + "Договор отправлен (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				//
			  return result;
			  //return textRef;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Договор не отправлен (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		    	return "PostContract: " + e.Message + "/" + DATA;
		    	//return "error";
		    }
			return "-1";
		}
		public string  UpdateContractDate(Guid idContract){
			string DATA = "";
			var esqContract = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
			esqContract.AddAllSchemaColumns();
			esqContract.Filters.LogicalOperation = LogicalOperationStrict.And;
			esqContract.Filters.Add(esqContract.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idContract));

			var contractes = esqContract.GetEntityCollection(UserConnection);
			foreach (var contractItem in contractes) {
				var endDate = contractItem.GetTypedColumnValue<DateTime>("qrtEndDate");
				var id = contractItem.GetTypedColumnValue<Guid>("Id");
				var guid1cAbipaK = contractItem.GetTypedColumnValue<string>("qrtGuid1c");
				var newDate = endDate.ToString("yyyy-MM-ddT00:00:00");
				DATA = "{" + $"\"СрокДействия\": \"{newDate}\"" + "}";
				var parametrs = $"Catalog_ДоговорыКонтрагентов(guid'{guid1cAbipaK}')?$format=json";
				WriteData(parametrs, DATA);
				var esqTechnoContract = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
				esqTechnoContract.AddAllSchemaColumns();
				esqTechnoContract.Filters.LogicalOperation = LogicalOperationStrict.And;

				esqTechnoContract.Filters.Add(esqTechnoContract.CreateFilterWithParameters(FilterComparisonType.Equal, "qrtContract", id));
				var contractesTechno = esqTechnoContract.GetEntityCollection(UserConnection);
				foreach(var itemTechno in contractesTechno){
					var guid1cAbipaKTechno = itemTechno.GetTypedColumnValue<string>("qrtGuid1c");
					parametrs = $"Catalog_ДоговорыКонтрагентов(guid'{guid1cAbipaKTechno}')?$format=json";
					WriteData(parametrs, DATA);
					//AddRecordJournal(parametrs, 200);
				}
				
			}
			return DATA;
		}
		public string WriteData(string parametrs, string data){
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + parametrs);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = "PATCH";
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(data);
		    requestWriter.Close();
		    try
		    {
		      WebResponse webResponse = request.GetResponse();
		      Stream webStream = webResponse.GetResponseStream();
		      StreamReader responseReader = new StreamReader(webStream);
		      responseReader.Close();

			  HttpWebResponse we = (HttpWebResponse)request.GetResponse();
		      string message = "BPM -> 1C: " + "Договор отправлен (" + (int)we.StatusCode + ")";
			  AddRecordJournal(message ,(int)we.StatusCode);
				//
			  return message;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Договор не отправлен (" + (int)we.StatusCode + ")";
				AddRecordJournal(new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString(),500);
		    	return "PostContract: " + e.Message;
		    }
		}
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string UpdateContractTechno(string method, string nameContractTechno, string guid1C){
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string dopPath = "(guid'" + guid1C + "')";
			if(string.IsNullOrEmpty(nameContractTechno) || string.IsNullOrEmpty(dopPath) || string.IsNullOrEmpty(guid1C)){
				return "-1";
			}
			
			string DATA = "{\"Description\": \"" + nameContractTechno + "\"}";
		   
		    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_ДоговорыКонтрагентов" + dopPath +"?$format=json");
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = method;
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
			try
		    {
		      WebResponse webResponse = request.GetResponse();
		      Stream webStream = webResponse.GetResponseStream();
		      StreamReader responseReader = new StreamReader(webStream);
		      string response = responseReader.ReadToEnd();
		      int indexRef = response.IndexOf("Ref_Key");
			  string textRef = response.Substring(indexRef + 11, 36);
			  int indexCode = response.IndexOf("Code");
			  string textCode = response.Substring(indexCode + 8, 9);
		      responseReader.Close();

			  HttpWebResponse we = (HttpWebResponse)request.GetResponse();
			  string message = "BPM -> 1C: " + "Технические договоры отправлены (" + (int)we.StatusCode + ")";
			  //AddRecordJournal(message,(int)we.StatusCode);
				//
		      return textRef;
			    }
			catch (WebException e)
			    {
					//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Технические договоры не отправлены (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		    	return "PostContractTecho " + e.Message;
		    }
				return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostContractTecho(Guid idContract, string method, string comment, Guid typeContract, Guid paymentCurrency, Guid paymentCurrency2, string guid1C, Guid idTechnoContract, Guid AccountOther, Guid AccountOtherPaymant, string nameContractTechno){
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string abipa = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtAbipa1C");
			string abipaCustom = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtAbipaCustom1C");
			string threeVed = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtThreeVedCustom");
			
			Guid customer = new Guid();
			Guid supplier = new Guid();
			Guid defermentOfPayment = new Guid();
			Guid fioMainContract = new Guid();
			Guid genderId = new Guid();
			Guid stateContract = new Guid();
			Guid form = new Guid();
			Guid ThreeSideFio = new Guid();
			DateTime date = new DateTime();
			DateTime dateEnd = new DateTime();
			string posotionContract = "";
			string basedCContract = "";
			string genderName = "";
			string refCustomer = "";
			string refSupplier = "";
			string fioMainContractName = "";
			string calculationsInConditionalUnits = "";
			string dateName = "";
			string dateEndName = "";
			string defermentOfPaymentName = "";
			string currenCurrencyy = "";
			string phifixName = "";
			string resultAccount = "";
			string resultOrganization = "";
			string typeContractFor1c = "";
			string subjectStr = "";
			string number = "";
			string paymantInCurrency = "";
			string ThreeSideBased = "";
			string ThreeSidePosition = "";
			string theContractSigned = "";
			string basedCContractResult = "";
			string posotionContractResult = "";
			string currencyRateMarkup = "";
			string currencyTechnoContract = "";
			//Золотарев(Catalog_ФизическиеЛица)
			string phifixName1 = "fe051d9a-91ee-11e6-80d1-8ac3d8faa466"; //id из 1с
			//Андросов(Catalog_ФизическиеЛица)
			string phifixName2 = "777fb6ef-bf02-11e3-9621-d850e63b96c4"; //id из 1с
			
			var esqContract = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
			esqContract.AddAllSchemaColumns();
			esqContract.Filters.Add(esqContract.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idContract));
			var contractes = esqContract.GetEntityCollection(UserConnection);
			foreach (var contractItem in contractes) {
				customer = contractItem.GetTypedColumnValue<Guid>("qrtCustomerId");
				supplier = contractItem.GetTypedColumnValue<Guid>("qrtSupplierId");
				defermentOfPayment = contractItem.GetTypedColumnValue<Guid>("qrtDefermentOfPaymentId");
				fioMainContract = contractItem.GetTypedColumnValue<Guid>("qrtFIOId");
				stateContract = contractItem.GetTypedColumnValue<Guid>("qrtStateId");
				form = contractItem.GetTypedColumnValue<Guid>("qrtFormId");
				ThreeSideFio = contractItem.GetTypedColumnValue<Guid>("qrtNameOfThirdPartySignatureId");
				date = contractItem.GetTypedColumnValue<DateTime>("qrtDate");
				dateEnd = contractItem.GetTypedColumnValue<DateTime>("qrtEndDate");
				posotionContract = contractItem.GetTypedColumnValue<string>("qrtPosition");
				basedCContract = contractItem.GetTypedColumnValue<string>("qrtBased");
				subjectStr = contractItem.GetTypedColumnValue<string>("qrtSubjectStr");
				number = contractItem.GetTypedColumnValue<string>("qrtName");
				ThreeSideBased = contractItem.GetTypedColumnValue<string>("qrtBasedOnForAThirdParty");
				ThreeSidePosition = contractItem.GetTypedColumnValue<string>("qrtThirdPartySignaturePosition");
				currencyRateMarkup = (contractItem.GetTypedColumnValue<float>("qrtCurrencyRateMarkup")).ToString();
			}
			
			if(stateContract.ToString() == "6173800d-8966-487e-aeb7-d68b278c58f8"){
				theContractSigned = "true";
			}
			else{
				theContractSigned = "false";
			}
			
			var esqContact = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
			esqContact.AddAllSchemaColumns();
			if(form.ToString() == threeVed && typeContract.ToString() == "c7a2112b-6351-4d68-b24f-9963cb0b27c9"){
				esqContact.Filters.Add(esqContact.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", ThreeSideFio));
				basedCContractResult = ThreeSideBased;
				posotionContractResult = ThreeSidePosition;
			}
			else{
				esqContact.Filters.Add(esqContact.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", fioMainContract));
				basedCContractResult = basedCContract;
				posotionContractResult = posotionContract;
			}
			var contates = esqContact.GetEntityCollection(UserConnection);
			foreach (var contactItem in contates) {
				genderId = contactItem.GetTypedColumnValue<Guid>("GenderId");
				fioMainContractName = contactItem.GetTypedColumnValue<string>("Name");
			}
			
			var esqGender = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Gender");
			esqGender.AddAllSchemaColumns();
			esqGender.Filters.Add(esqGender.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", genderId));
			var genders = esqGender.GetEntityCollection(UserConnection);
			foreach (var genderItem in genders) {
				genderName = genderItem.GetTypedColumnValue<string>("Name");
			}
			
			var esqDefermentOfPayment = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtDefermentOfPayment");
			esqDefermentOfPayment.AddAllSchemaColumns();
			esqDefermentOfPayment.Filters.Add(esqDefermentOfPayment.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", defermentOfPayment));
			var esqDefermentOfPayments = esqDefermentOfPayment.GetEntityCollection(UserConnection);
			foreach (var defermentOfPaymentItem in esqDefermentOfPayments) {
				defermentOfPaymentName = defermentOfPaymentItem.GetTypedColumnValue<string>("Name");
			}
			
			string dopPath = "";
			if(method == "PATCH"){
				dopPath = "(guid'" + guid1C + "')";
			}
			
			if(form.ToString() == threeVed && typeContract.ToString() == "c7a2112b-6351-4d68-b24f-9963cb0b27c9"){
				refCustomer = PostAccount1C(customer, true);
				refSupplier = PostAccount1C(AccountOther, true);
			//	AddRecordJournal($"AccountOther {refSupplier}",200);
				paymentCurrency2 = paymentCurrency;
			}
			else{
				refCustomer = PostAccount1C(customer, true);
				refSupplier = PostAccount1C(supplier, true);
			}
			
			if(refCustomer.Length != 36){
				return "Customer: " + refCustomer;
			}
			
			if(refSupplier.Length != 36){
				return "Supplier: " + refSupplier;
			}
			if(paymentCurrency.ToString() == "5fb76920-53e6-df11-971b-001d60e938c6" && paymentCurrency2.ToString() == "5fb76920-53e6-df11-971b-001d60e938c6"){
				calculationsInConditionalUnits = "false";
				currenCurrencyy = "false";
				paymantInCurrency = "false";
				//AddRecordJournal($"Я выполнился когда из техно рубль {paymantInCurrency} , а в основном {paymentCurrency2}",200);
			}
			else if(paymentCurrency.ToString() != "5fb76920-53e6-df11-971b-001d60e938c6" && paymentCurrency2.ToString() != "5fb76920-53e6-df11-971b-001d60e938c6"){
				calculationsInConditionalUnits = "false";
				currenCurrencyy = "true";
				paymantInCurrency = "true";
				//AddRecordJournal($"Условие #2 Я выполнился когда из техно не рубль {paymantInCurrency} , а в основном не {paymentCurrency2}",200);
			}
			else{
				calculationsInConditionalUnits = "true";
				currenCurrencyy = "true";
				paymantInCurrency = "false";
			//	AddRecordJournal($"Я выполнился когда из техно не рубль {paymantInCurrency} , а в основном рубль {paymentCurrency2}",200);
			}

			if(date.ToString() != ""){
				dateName = date.ToString("yyyy-MM-ddT00:00:00");
			}
			
			if(dateEnd.ToString() != ""){
				dateEndName = dateEnd.ToString("yyyy-MM-ddT00:00:00");
			}
			
			if(typeContract.ToString() == "9e085bd0-b7a3-46dc-a76f-ed7f9b3dc9fe" || typeContract.ToString() == "084958ff-522d-44b1-a9e5-2b9a5c0aa89f"){
				resultAccount = refCustomer;
				if(supplier.ToString() == abipa){
					resultOrganization = "becb74a9-a84a-11e7-af3b-ccb0daad0804"; //id из 1с
					phifixName = phifixName1;
				}
				else if(supplier.ToString() == abipaCustom){
					resultOrganization = "77050c9a-befb-11e3-9621-d850e63b96c4"; //id из 1с
					phifixName = phifixName2;
				}
				if(typeContract.ToString() == "9e085bd0-b7a3-46dc-a76f-ed7f9b3dc9fe"){
					typeContractFor1c = "СПокупателем";
				}
				else{
					typeContractFor1c = "СКомитентомНаЗакупку";
				}
			}
			else{
				resultAccount = refSupplier;
				if(form.ToString() != threeVed && customer.ToString() == abipa){
					resultOrganization = "becb74a9-a84a-11e7-af3b-ccb0daad0804";
					phifixName = phifixName1;
				}
				else if(form.ToString() != threeVed && customer.ToString() == abipaCustom){
					resultOrganization = "77050c9a-befb-11e3-9621-d850e63b96c4";
					phifixName = phifixName2;
				}
				else if(form.ToString() == threeVed && supplier.ToString() == abipa){
					resultOrganization = "becb74a9-a84a-11e7-af3b-ccb0daad0804"; //id из 1с
					phifixName = phifixName1;
				}
				else if(form.ToString() == threeVed && supplier.ToString() == abipaCustom){
					resultOrganization = "77050c9a-befb-11e3-9621-d850e63b96c4"; //id из 1с
					phifixName = phifixName2;
				}
				if(typeContract.ToString() == "c7a2112b-6351-4d68-b24f-9963cb0b27c9"){
					typeContractFor1c = "СПоставщиком";
				}
				else if(typeContract.ToString() == "24900c3f-686d-4f1d-9c34-b9fd82dcd69f"){
					typeContractFor1c = "СКомитентом";
				}
				else if(typeContract.ToString() == "9f0c7db3-d4b9-45af-93fb-0e2348b052ee"){
					typeContractFor1c = "СКомиссионеромНаЗакупку";
				}
				else{
					typeContractFor1c = "Прочее";
				}
			}
			
			string typeOfSettlementName = "";
			if(subjectStr != String.Empty){
				string typeOfSettlementId = GetTypeOfSettlement(subjectStr);
				if(typeOfSettlementId != "-1" || typeOfSettlementId != "error"){
					typeOfSettlementName = typeOfSettlementId;
				}
				else{
					typeOfSettlementName = "00000000-0000-0000-0000-000000000000";
				}
			}
			else{
				typeOfSettlementName = "00000000-0000-0000-0000-000000000000";
			}
			
			string currencyCodeName = "";
			string currencyCodeName2 = "";
			string refCurrency = "";
			string currencyRateMarkup2 = "";
			
			var esqCurrency = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Currency");
			esqCurrency.AddAllSchemaColumns();
			esqCurrency.Filters.Add(esqCurrency.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", paymentCurrency));
			var currencyes = esqCurrency.GetEntityCollection(UserConnection);
			foreach (var currencyItem in currencyes) {
				currencyCodeName = currencyItem.GetTypedColumnValue<string>("ShortName");
			}
				if((!string.IsNullOrEmpty(currencyRateMarkup) && currencyRateMarkup != "0") && (currencyTechnoContract != "5fb76920-53e6-df11-971b-001d60e938c6")){
					currencyRateMarkup = currencyRateMarkup.Replace(",", ".");
				    int indexComma = currencyRateMarkup.IndexOf(".");
				    int currencyRateMarkupLength = currencyRateMarkup.Length;
				    if(indexComma != -1 && currencyRateMarkupLength  - (indexComma + 1) == 2){
				      currencyRateMarkup2 = currencyRateMarkup.Substring(0, currencyRateMarkupLength-1);
				    }
				    else{
				    	currencyRateMarkup2 = currencyRateMarkup;
				    }
				    if (paymentCurrency.ToString() != "5fb76920-53e6-df11-971b-001d60e938c6"){
						currencyCodeName2 = currencyCodeName + "+" + currencyRateMarkup2 + "%";
				    }
				}

				
			if(currencyCodeName2 != String.Empty){
				refCurrency = GetCurrencyDescription(currencyCodeName2);
				if(refCurrency == "-1"){
					if(currencyCodeName != String.Empty){
						refCurrency = GetCurrencyDescription(currencyCodeName);
						if(refCurrency == "-1"){
							refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
						}
					}
					else{
						refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
					}
				}
			}
			else{
				if(currencyCodeName != String.Empty){
					refCurrency = GetCurrencyDescription(currencyCodeName);
					if(refCurrency == "-1"){
						refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
					}
				}
				else{
					refCurrency ="32919840-192d-11e3-a1cc-c86000df0d7b";
				}
			}
			
			string dateName2 = date.ToString("dd.MM.yyyy");
			AddRecordJournal($"Оплата в валюте {paymantInCurrency} Валюта {currencyCodeName} или {currencyCodeName2}",200);
			string DATA = "";
		//	AddRecordJournal($"Аккаунт перед отправкой {resultAccount}",200);
			 DATA = "{\"Description\": \"" + nameContractTechno + "\"," +
                   " \"ВидДоговора\": \"" + typeContractFor1c + "\"," +
                   " \"НДСПоСтавкам4и2\": \"" + "false" + "\", " +
                   "\"ВидВзаиморасчетов_Key\": \"" + typeOfSettlementName + "\"," +
                   " \"ПолРуководителяКонтрагента\": \"" + genderName + "\"," +
                   " \"СчетаФактурыОтИмениОрганизации\": \"" + "false" + "\"," +
                   " \"ПлатежныйАгент\": \"" + "false" + "\", " +
                   "\"ДолжностьРуководителяКонтрагента\": \"" + posotionContractResult + "\"," +
                   " \"РуководительКонтрагента\": \"" + fioMainContractName + "\"," +
                   " \"ЗаРуководителяКонтрагентаПоПриказу\": \"" + basedCContractResult + "\"," +
                   " \"ВалютаВзаиморасчетов_Key\": \"" + refCurrency + "\", " +
                   "\"ДоговорПодписан\": \"" + theContractSigned + "\", " +
                   "\"Валютный\": \"" + currenCurrencyy + "\", \"ОплатаВВалюте\": \"" + paymantInCurrency + "\", " +
                   "\"Дата\": \"" + dateName + "\", " +
                   "\"Комментарий\": \"" + comment + "\"," +
                   " \"СрокДействия\": \"" + dateEndName + "\"," +
                   " \"СрокОплаты\": \"" + defermentOfPaymentName + "\", " +
                   "\"Номер\": \"" + number + "\"," +
                   " \"УчетАгентскогоНДС\": \"" + "false" + "\", " +
                   "\"УстановленСрокОплаты\": \"" + "true" + "\"," +
                   " \"УдалитьРеализацияНаЭкспорт\": \"" + "false" + "\", " +
                   "\"ИспользуетсяПриОбменеДанными\": \"" + "false" + "\", " +
                   "\"Predefined\": \"" + "false" + "\"," +
                   " \"РасчетыВУсловныхЕдиницах\":  \"" + calculationsInConditionalUnits + "\"," +
                   " \"СпособЗаполненияСтавкиНДС\": \"" + "Автоматически" + "\"," +
                   " \"ПроцентКомиссионногоВознаграждения\": \"" + "0" + "\"," +
                   " \"Руководитель_Key\": \"" + phifixName + "\"," +
                   " \"ДолжностьРуководителя_Key\": \"" + "7d02b19c-192d-11e3-a1cc-c86000df0d7b" + "\", " +
                   "\"IsFolder\": \"" + "false" + "\"," +
                   " \"DeletionMark\": \"" + "false" + "\", " +
                   "\"Owner_Key\": \"" + resultAccount + "\", " +
                   "\"Организация_Key\": \"" + resultOrganization + "\"}";

		    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_ДоговорыКонтрагентов" + dopPath +"?$format=json");
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = method;
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
		      WebResponse webResponse = request.GetResponse();
		      Stream webStream = webResponse.GetResponseStream();
		      StreamReader responseReader = new StreamReader(webStream);
		      string response = responseReader.ReadToEnd();
		      int indexRef = response.IndexOf("Ref_Key");
			  string textRef = response.Substring(indexRef + 11, 36);
			  int indexCode = response.IndexOf("Code");
			  string textCode = response.Substring(indexCode + 8, 9);
		      responseReader.Close();
		      
		      var esqContractVn = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
			  esqContractVn.AddAllSchemaColumns();
			  esqContractVn.Filters.Add(esqContractVn.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idTechnoContract));
			  var contractesVn = esqContractVn.GetEntityCollection(UserConnection);
			  foreach (var contractVnItem in contractesVn) {
				  contractVnItem.SetColumnValue("qrtCode1C", textCode);
				  contractVnItem.SetColumnValue("qrtGuid1c", textRef);
				  contractVnItem.Save(false);
			  }
			  //
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM -> 1C: " + "Технические договоры отправлены (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
			  return textRef;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Технические договоры не отправлены (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		    	return "PostContractTecho " + e.Message;
		    	//return "error";
		    }
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetTypeOfSettlement(string typeOfSettement) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs = "?$filter=Description eq '" + typeOfSettement.Trim() + "'" + "&$format=json";
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_ВидыВзаиморасчетов" + parametrs);
			request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			int indexRef = response.IndexOf("Ref_Key");
      		if(indexRef != -1){
      			//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Вид Взаиморасчетов получен (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
      			string textRef = response.Substring(indexRef + 11, 36);
      			return textRef;
      		}
      		else{
      			string newTextRef = PostTypeOfSettlement(typeOfSettement);
				return newTextRef;
      		}
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostTypeOfSettlement(string typeOfSettement) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string DATA = "{\"Description\": \"" + typeOfSettement + "\"}";
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_ВидыВзаиморасчетов");
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = "POST";
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				string textRef = response.Substring(indexRef + 11, 36);
				responseReader.Close();
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Вид Взаиморасчетов создан (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
				return textRef;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Вид Взаиморасчетов не создан (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		      //return e.Message;
		      return "error";
		    }
		    return "-1";
		}

		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetBankInvoice(Guid Id, int readBillingInfo, string guidAccount, string Record ) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			Guid customerBillingInfo = new Guid();
			Guid supplierBillingInfo = new Guid();
			Guid readGuidBillingInfo = new Guid();
			
			var esqContract = new EntitySchemaQuery(UserConnection.EntitySchemaManager, Record);
			esqContract.AddAllSchemaColumns();
			esqContract.Filters.Add(esqContract.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));
			var contractes = esqContract.GetEntityCollection(UserConnection);
			foreach (var contractItem in contractes) {
				customerBillingInfo = contractItem.GetTypedColumnValue<Guid>("qrtCustomerBillingInfoId");
				supplierBillingInfo = contractItem.GetTypedColumnValue<Guid>("qrtSupplierBillingInfoId");
			}
		//	AddRecordJournal($"supplierBillingInfo {supplierBillingInfo} customerBillingInfo {customerBillingInfo}",200);
			
			if(readBillingInfo == 1){
				if(customerBillingInfo != Guid.Empty){
					readGuidBillingInfo = customerBillingInfo;
				}
			}
			else{
				if(supplierBillingInfo != Guid.Empty){
					readGuidBillingInfo = supplierBillingInfo;
				}
			}
			
			if(readGuidBillingInfo == Guid.Empty){
				return "-1";
			}
			
			string codeBillingInfo = "";
			string guidBillingInfo = "";
			Guid countryId = new Guid();
			var esqAccountBillingInfo = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
			esqAccountBillingInfo.AddAllSchemaColumns();
			esqAccountBillingInfo.Filters.Add(esqAccountBillingInfo.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", readGuidBillingInfo));
			var accountBillingInfos = esqAccountBillingInfo.GetEntityCollection(UserConnection);
			foreach (var accountBillingInfoItem in accountBillingInfos) {
				codeBillingInfo = accountBillingInfoItem.GetTypedColumnValue<string>("qrtCode1c");
				guidBillingInfo = accountBillingInfoItem.GetTypedColumnValue<string>("qrtGuid1c");
				countryId = accountBillingInfoItem.GetTypedColumnValue<Guid>("CountryId");
			}
			if (countryId.ToString() != "a570b005-e8bb-df11-b00f-001d60e938c6"){
				return "-1";
			}
			if(codeBillingInfo != String.Empty){
				if(guidBillingInfo == String.Empty){
					guidBillingInfo = GetIdRecord(codeBillingInfo, "Catalog_БанковскиеСчета");
				}
				if(guidBillingInfo == "-1"){
					return "-1";
				}
				string parametrs = "(guid'" + guidBillingInfo + "')" + "?$format=json";
				var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_БанковскиеСчета" + parametrs);
				AddRecordJournal(path + "Catalog_БанковскиеСчета" + parametrs,200);
				request.Credentials = CredentialCache.DefaultCredentials;
	            request.Credentials = new NetworkCredential(login, password);
	            WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
	      		if(indexRef != -1){
	      			//
					HttpWebResponse we = (HttpWebResponse)request.GetResponse();
					string message = "BPM <- 1C: " + "Банковский Счет получен (" + (int)we.StatusCode + ")";
					AddRecordJournal(message,(int)we.StatusCode);
					//
	      			
					//string textRef = PostBankInvoice(readGuidBillingInfo, "PATCH", guidBillingInfo, guidAccount);
	      			
					string textRef = response.Substring(indexRef + 11, 36);
	      			return textRef;
	      		}
	      		else{
	      			string newTextRef = PostBankInvoice(readGuidBillingInfo, "POST", "", guidAccount);
					return newTextRef;
	      		}
			}
			else{
				string newTextRef2 = PostBankInvoice(readGuidBillingInfo, "POST", "", guidAccount);
				return newTextRef2;
			}
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostBankInvoice(Guid billingInfo, string method, string guid1C, string guidAccount) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string dopPath = "";
			
			if(method == "PATCH"){
				dopPath = "(guid'" + guid1C + "')";
			}
			
			Guid currencyId = new Guid();
			
			string bankNameRus = "";
			string bankAccount = "";
			string bik = "";
			string swift = "";
			
			var esqAccountBillingInfo = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
			esqAccountBillingInfo.AddAllSchemaColumns();
			esqAccountBillingInfo.Filters.Add(esqAccountBillingInfo.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", billingInfo));
			var accountBillingInfos = esqAccountBillingInfo.GetEntityCollection(UserConnection);
			foreach (var accountBillingInfoItem in accountBillingInfos) {
				bankNameRus = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBank").Replace("\"", @"\" + "\"");
				bankAccount = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBankAccount");
				bik = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBIC");
				swift = accountBillingInfoItem.GetTypedColumnValue<string>("qrtSWIFT");
				currencyId = accountBillingInfoItem.GetTypedColumnValue<Guid>("qrtCurrencyId");
				
			}
			
			string bollCurrency = "";
			
			if(currencyId.ToString() == "5fb76920-53e6-df11-971b-001d60e938c6"){
				bollCurrency = "false";
			}
			else{
				bollCurrency = "true";
			}
			
			string currencyName = "";
			
			var esqCurrency = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Currency");
			esqCurrency.AddAllSchemaColumns();
			esqCurrency.Filters.Add(esqCurrency.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", currencyId));
			var currencyes = esqCurrency.GetEntityCollection(UserConnection);
			foreach (var currencyItem in currencyes) {
				currencyName = currencyItem.GetTypedColumnValue<string>("Code");
			}
			
			string refCurrency = GetCurrency(currencyName);
			if(refCurrency == "-1"){
				return "error";
			}
			
			string bankid1c = GetBank(billingInfo);
			if(bankid1c == "-1" || bankid1c == "error"){
				return "error";
			}
			
			string DATA = "{\"Owner\": \"" + guidAccount + "\", \"Owner_Type\": \"" + "StandardODATA.Catalog_Контрагенты" + "\", \"Description\": \"" + bankAccount + ", " + bankNameRus + "\", \"НомерСчета\": \"" + bankAccount + "\", \"СуммаБезКопеек\": \"" + "false" + "\", \"ВсегдаУказыватьКПП\": \"" + "true" + "\", \"МесяцПрописью\": \"" + "false" + "\", \"Банк_Key\": \"" + bankid1c + "\", \"ВалютаДенежныхСредств_Key\": \"" + refCurrency + "\", \"Валютный\": \"" + bollCurrency + "\"}";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_БанковскиеСчета" + dopPath);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = method;
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				string textRef = response.Substring(indexRef + 11, 36);
				int indexCode = response.IndexOf("Code");
      			string textCode = response.Substring(indexCode + 8, 9);
				responseReader.Close();
				
				var esqAccountBillingInfoVn = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
				esqAccountBillingInfoVn.AddAllSchemaColumns();
				esqAccountBillingInfoVn.Filters.Add(esqAccountBillingInfoVn.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", billingInfo));
				var accountBillingInfosVn = esqAccountBillingInfoVn.GetEntityCollection(UserConnection);
				foreach (var accountBillingInfoItemVn in accountBillingInfosVn) {
					accountBillingInfoItemVn.SetColumnValue("qrtGuid1c", textRef);
					accountBillingInfoItemVn.SetColumnValue("qrtCode1c", textCode);
					accountBillingInfoItemVn.Save();
				}
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM -> 1C: " + "Банковский Счет создан/обновлен (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
				return textRef;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Банковский Счет не создан (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		      //return e.Message;
		      return "error";
		    }
		    return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetCurrency(string code) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs =  "?$filter=Code eq '" + code + "'" + "&$format=json";
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Валюты" + parametrs);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			int indexRef = response.IndexOf("Ref_Key");
			responseReader.Close();
			HttpWebResponse we = (HttpWebResponse)request.GetResponse();
			if(indexRef != -1){
				//
				string message = "BPM <- 1C: " + "Валюта получена (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				//
				string textRef = response.Substring(indexRef + 11, 36);
				return textRef;
			}
			else{
				//
				string message = "BPM <- 1C: " + "Валюта не получена (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
				return "-1";
			}
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetCurrencyDescription(string name) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs =  "?$filter=Description eq '" + name + "'" + "&$format=json";
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Валюты" + parametrs);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			int indexRef = response.IndexOf("Ref_Key");
			responseReader.Close();
			if(indexRef != -1){
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Валюта получена (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				//
				string textRef = response.Substring(indexRef + 11, 36);
				return textRef;
			}
			else{
				//Todo create currency
				string textId = PostCurrency(name);
				return textId;
			}
		}
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostCurrency(string name) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			Random rnd = new Random();
			int code = rnd.Next(000,999);
			
			string markup = name.Split(new char[] {'+', '%'})[1];
			
			string DATA = "{\"Description\":\"" + name + "\",\"Code\":\"" + code + "\",\"НаименованиеПолное\":\"" + name + "\",\"Наценка\":\"" + markup + "\"}";
			string URL = path + "Catalog_Валюты";
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
			
			request.Credentials = CredentialCache.DefaultCredentials;
			
			request.Method = "POST";
			request.ContentType = "application/json; charset=UTF-8";
			request.Accept = "application/json";
			request.Credentials = new NetworkCredential(login, password);
			StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
			requestWriter.Write(DATA);
			requestWriter.Close();
			
			try
			{
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				string textRef = response.Substring(indexRef + 11, 36);
				responseReader.Close();
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM -> 1C: " + "Валюта создана/обновлена (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
				return textRef;
			}
			catch (WebException e)
			{
				//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Валюта не создана (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
				return "-1";
			}
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetBank(Guid billingInfo) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string bik = "";
			string swift = "";
			string guidBank = "";
			
			var esqAccountBillingInfo = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
			esqAccountBillingInfo.AddAllSchemaColumns();
			esqAccountBillingInfo.Filters.Add(esqAccountBillingInfo.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", billingInfo));
			var accountBillingInfos = esqAccountBillingInfo.GetEntityCollection(UserConnection);
			foreach (var accountBillingInfoItem in accountBillingInfos) {
				bik = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBIC");
				swift = accountBillingInfoItem.GetTypedColumnValue<string>("qrtSWIFT");
				guidBank = accountBillingInfoItem.GetTypedColumnValue<string>("qrtGuid1cBank");
			}
			
			string parametrs = "?$filter=СВИФТБИК eq '" + swift + "' and Code eq '" + bik + "'" + "&$format=json";
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Банки" + parametrs);
			request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			int indexRef = response.IndexOf("Ref_Key");
			//
			HttpWebResponse we = (HttpWebResponse)request.GetResponse();
			string message = "BPM <- 1C: " + "Банк получен (" + (int)we.StatusCode + ")";
			//AddRecordJournal(message,(int)we.StatusCode);
			//
			if(indexRef != -1){
				if(guidBank == String.Empty){
					guidBank = GetIdRecord(bik, "Catalog_Банки");
				}
				if(guidBank == "-1"){
					return "-1";
				}
      			string textRef = response.Substring(indexRef + 11, 36);
      			return textRef;
      		}
      		else{
      			string newTextRef = PostBank(billingInfo, "POST", "");
				return newTextRef;
      		}
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostBank(Guid billingInfo, string method, string guid1C) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string dopPath = "";
			
			if(method == "PATCH"){
				dopPath = "(guid'" + guid1C + "')";
			}
			
			string bankNameRus = "";
			string bankAccount = "";
			string bik = "";
			string swift = "";
			string adres = "";
			string correspondentAccount = "";
			Guid countryId = new Guid();
			
			var esqAccountBillingInfo = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
			esqAccountBillingInfo.AddAllSchemaColumns();
			esqAccountBillingInfo.Filters.Add(esqAccountBillingInfo.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", billingInfo));
			var accountBillingInfos = esqAccountBillingInfo.GetEntityCollection(UserConnection);
			foreach (var accountBillingInfoItem in accountBillingInfos) {
				bankNameRus = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBank").Replace("\"", @"\" + "\"");
				bankAccount = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBankAccount");
				bik = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBIC");
				swift = accountBillingInfoItem.GetTypedColumnValue<string>("qrtSWIFT");
				adres = accountBillingInfoItem.GetTypedColumnValue<string>("qrtBankAddressRU");
				correspondentAccount = accountBillingInfoItem.GetTypedColumnValue<string>("qrtCorrespondentAccount");
				countryId = accountBillingInfoItem.GetTypedColumnValue<Guid>("CountryId");
			}
			//Если только свифт без бика , то банк не создается
			if (string.IsNullOrEmpty(bik)){
				return "-1";
			}
			string countryCode = "";
			string countryId1C = "";
			
			var esqCountry = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Country");
			esqCountry.AddAllSchemaColumns();
			esqCountry.Filters.Add(esqCountry.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", countryId));
			var countryes = esqCountry.GetEntityCollection(UserConnection);
			foreach (var countryItem in countryes) {
				countryCode = countryItem.GetTypedColumnValue<string>("Code");
			}
			countryId1C = GetCountry(countryCode);
			if (countryId1C.Length != 36){
				return "PostBank: " + countryId1C;
			}
			
			string groupResult = "";
			string group = SearchGroupForBank(bik);
			if(group != "-1"){
				groupResult = group;
			}
			else{
				groupResult = "00000000-0000-0000-0000-000000000000";
			}
			
			string DATA = "{\"Parent_Key\": \"" + groupResult + "\", \"Code\": \"" + bik + "\", \"Description\": \"" + bankNameRus + "\", \"КоррСчет\": \"" + correspondentAccount + "\", \"Адрес\": \"" + adres + "\", \"СВИФТБИК\": \"" + swift + "\", \"Страна_Key\": \"" + countryId1C + "\"}";
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Банки" + dopPath);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = method;
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				string textRef = response.Substring(indexRef + 11, 36);
				responseReader.Close();
				
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM -> 1C: " + "Банк создан/обновлен (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
				
				var esqAccountBillingInfoVn = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
				esqAccountBillingInfoVn.AddAllSchemaColumns();
				esqAccountBillingInfoVn.Filters.Add(esqAccountBillingInfoVn.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", billingInfo));
				var accountBillingInfosVn = esqAccountBillingInfoVn.GetEntityCollection(UserConnection);
				foreach (var accountBillingInfoItemVn in accountBillingInfosVn) {
					accountBillingInfoItemVn.SetColumnValue("qrtGuid1cBank", textRef);
					accountBillingInfoItemVn.Save();
				}
				
				return textRef;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Банк не создан (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		      //return e.Message;
		      return "error";
		    }
		    return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string SearchGroupForBank(string bik) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			if(bik.Length < 4){
				return "-1";
			}
			
			string search = bik[2].ToString() + bik[3].ToString();
			string result = "";
			
			string parametrs = "?$filter=Code eq '" + search + "'" + "&$format=json";
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Банки" + parametrs);
			request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			int indexRef = response.IndexOf("Ref_Key");
			if(indexRef != -1){
				
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Банк получен (" + (int)we.StatusCode + ")";
				///AddRecordJournal(message,(int)we.StatusCode);
				//
				
      			string textRef = response.Substring(indexRef + 11, 36);
      			return textRef;
      		}
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetContact(Guid contact, string account) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string contactGuidName = "";
			string contactCodeName = "";
			
			var esqContact = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
			esqContact.AddAllSchemaColumns();
			esqContact.Filters.Add(esqContact.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contact));
			var contates = esqContact.GetEntityCollection(UserConnection);
			foreach (var contactItem in contates) {
				contactGuidName = contactItem.GetTypedColumnValue<string>("qrtGuid1c");
				contactCodeName = contactItem.GetTypedColumnValue<string>("qrt1SCode");
			}
			
			if(contactCodeName != String.Empty){
				if(contactGuidName == String.Empty){
					contactGuidName = GetIdRecord(contactCodeName, "Catalog_КонтактныеЛица");
				}
				if(contactGuidName == "-1"){
					return "-1";
				}
				string parametrs = "(guid'" + contactGuidName + "')" + "?$format=json";
				var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_КонтактныеЛица" + parametrs);
				request.Credentials = CredentialCache.DefaultCredentials;
	            request.Credentials = new NetworkCredential(login, password);
	            WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
	      		if(indexRef != -1){
	      			//
					HttpWebResponse we = (HttpWebResponse)request.GetResponse();
					string message = "BPM <- 1C: " + "Контактные Лица получены (" + (int)we.StatusCode + ")";
					//AddRecordJournal(message,(int)we.StatusCode);
					//
	      			string textRef = PostContact(contact, account, "PATCH", contactGuidName);
	      			return textRef;
	      		}
	      		else{
	      			string newTextRef = PostContact(contact, account, "POST", "");
					return newTextRef;
	      		}
			}
			else{
				string newTextRef2 = PostContact(contact, account, "POST", "");
				return newTextRef2;
			}
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostContact(Guid contact, string account, string method, string guid1C) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string dopPath = "";
			
			if(method == "PATCH"){
				dopPath = "(guid'" + guid1C + "')";
			}
			
			string fio = "";
			string JobName = "";
			string mobilePhone = "";
			string jobPhpne = "";
			string email = "";
			
			var esqContact = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
			esqContact.AddAllSchemaColumns();
			esqContact.Filters.Add(esqContact.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contact));
			var contates = esqContact.GetEntityCollection(UserConnection);
			foreach (var contactItem in contates) {
				fio = contactItem.GetTypedColumnValue<string>("Name");
				JobName = contactItem.GetTypedColumnValue<string>("JobTitle");
				mobilePhone = contactItem.GetTypedColumnValue<string>("MobilePhone");
				jobPhpne = contactItem.GetTypedColumnValue<string>("Phone");
				email = contactItem.GetTypedColumnValue<string>("Email");
			}

			string name = "";
			string fam = "";
			string oth = "";
			
			fio = fio.Trim();
			
			var massFio = fio.Split(' ');
			if(massFio.Length >= 1){
				fam = massFio[0];
			}
			if(massFio.Length >= 2){
				name = massFio[1];
			}
			if(massFio.Length == 3){
				oth = massFio[2];
			}
			
			//string contactInformation = "{\"LineNumber\": \"" + "1" + "\", \"Тип\": \"" + "Телефон" + "\", \"ВидДляСписка_Key\": \"" + "f9e52726-2faf-4dca-85d0-559b80df2c53" + "\", \"Вид_Key\": \"" + "f9e52726-2faf-4dca-85d0-559b80df2c53" + "\", \"Представление\": \"" + mobilePhone + "\"},{\"LineNumber\": \"" + "2" + "\", \"Тип\": \"" + "Телефон" + "\", \"ВидДляСписка_Key\": \"" + "feb95df3-9e07-4bb2-a4a8-cc02c31f8a37" + "\", \"Вид_Key\": \"" + "feb95df3-9e07-4bb2-a4a8-cc02c31f8a37" + "\", \"Представление\": \"" + jobPhpne + "\"},{\"LineNumber\": \"" + "3" + "\", \"Тип\": \"" + "АдресЭлектроннойПочты" + "\", \"ВидДляСписка_Key\": \"" + "125756c6-d55f-4411-b2d0-e8cd53187d7f" + "\", \"Вид_Key\": \"" + "125756c6-d55f-4411-b2d0-e8cd53187d7f" + "\", \"Представление\": \"" + email + "\"}, {\"LineNumber\": \"" + "4" + "\", \"Тип\": \"" + "Адрес" + "\", \"ВидДляСписка_Key\": \"" + "e06d2b1f-c480-485d-a278-190de6d465a6" + "\", \"Вид_Key\": \"" + "e06d2b1f-c480-485d-a278-190de6d465a6" + "\", \"Страна\": \"" + "Россия" + "\", \"Представление\": \"" +  + "\"}";
			string contactInformation = "{\"LineNumber\": \"" + "1" + "\", \"Тип\": \"" + "Телефон" + "\", \"ВидДляСписка_Key\": \"" + "f9e52726-2faf-4dca-85d0-559b80df2c53" + "\", \"Вид_Key\": \"" + "f9e52726-2faf-4dca-85d0-559b80df2c53" + "\", \"Представление\": \"" + mobilePhone + "\"},{\"LineNumber\": \"" + "2" + "\", \"Тип\": \"" + "Телефон" + "\", \"ВидДляСписка_Key\": \"" + "feb95df3-9e07-4bb2-a4a8-cc02c31f8a37" + "\", \"Вид_Key\": \"" + "feb95df3-9e07-4bb2-a4a8-cc02c31f8a37" + "\", \"Представление\": \"" + jobPhpne + "\"},{\"LineNumber\": \"" + "3" + "\", \"Тип\": \"" + "АдресЭлектроннойПочты" + "\", \"ВидДляСписка_Key\": \"" + "125756c6-d55f-4411-b2d0-e8cd53187d7f" + "\", \"Вид_Key\": \"" + "125756c6-d55f-4411-b2d0-e8cd53187d7f" + "\", \"Представление\": \"" + email + "\"}"; //id из 1с
			string DATA = "{\"Description\": \"" + fio + ", " + JobName + "\", \"Фамилия\": \"" + fam + "\", \"Должность\": \"" + JobName + "\", \"Отчество\": \"" + oth + "\", \"ОбъектВладелец\": \"" + account + "\", \"ОбъектВладелец_Type\": \"" + "StandardODATA.Catalog_Контрагенты" + "\", \"ВидКонтактногоЛица\": \"" + "КонтактноеЛицоКонтрагента" + "\", \"Имя\": \"" + name + "\", \"КонтактнаяИнформация\" : [" + contactInformation + "]}";
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_КонтактныеЛица" + dopPath);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = method;
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				string textRef = response.Substring(indexRef + 11, 36);
				int indexCode = response.IndexOf("Code");
      			string textCode = response.Substring(indexCode + 8, 9);
				responseReader.Close();
				
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM -> 1C: " + "Контактные Лица созданы/обновлены (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				//
				
				var esqContactVn = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
				esqContactVn.AddAllSchemaColumns();
				esqContactVn.Filters.Add(esqContactVn.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contact));
				var contatesVn = esqContactVn.GetEntityCollection(UserConnection);
				foreach (var contactVnItem in contatesVn) {
					contactVnItem.SetColumnValue("Name", fio);
					contactVnItem.SetColumnValue("qrt1SCode", textCode);
					contactVnItem.SetColumnValue("qrtGuid1c", textRef);
					contactVnItem.SetColumnValue("TypeId", "806732ee-f36b-1410-a883-16d83cab0980");
					contactVnItem.Save();
				}
				return textRef;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Контактные Лица не созданы/обновлены (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		      //return e.Message;
		      return "error";
		    }
		    return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetCountry(string code) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			
			string parametrs = "?$format=json&$filter=startswith(КодАльфа3,'" + code +"')";
			string fullPath = path + "Catalog_СтраныМира" + parametrs;
			
			try
			{
				var request = (HttpWebRequest)WebRequest.Create(fullPath);
				request.Credentials = CredentialCache.DefaultCredentials;
				request.Credentials = new NetworkCredential(login, password);
				request.ReadWriteTimeout  = 300000;
				var response = (HttpWebResponse)request.GetResponse();
				var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
				string[] nameCount = responseString.Split(new char[] { '"' });
				
				if (nameCount.Length > 9){
					//
					HttpWebResponse we = (HttpWebResponse)request.GetResponse();
					string message = "BPM <- 1C: " + "Страны Мира получены (" + (int)we.StatusCode + ")";
					//AddRecordJournal(message,(int)we.StatusCode);
					//
					return nameCount[9];
					//Console.WriteLine(nameCount[9]);
				} else {
					string newTextRef = Postountry(code);
					return newTextRef;
				}
			}
			catch (WebException e)
			{
				//
				var we = e.Response as HttpWebResponse;
				string message = "BPM <- 1C: " + "Страны Мира не получены (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
				string error = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
				return error;
				//Console.WriteLine(error);
			}
			/*
			string parametrs = "?$format=json&$filter=startswith(КодАльфа3,'" + code +"')";
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_СтраныМира" + parametrs);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			int indexRef = response.IndexOf("Ref_Key");
			responseReader.Close();
			if(indexRef != -1){
				string textRef = response.Substring(indexRef + 11, 36);
				return textRef;
			}
			else{
				string newTextRef = Postountry(code);
				return newTextRef;
			}
			*/
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string Postountry(string code) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string countryName = "";
			
			var esqCountry = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Country");
			esqCountry.AddAllSchemaColumns();
			esqCountry.Filters.Add(esqCountry.CreateFilterWithParameters(FilterComparisonType.Equal, "Code", code));
			var countryes = esqCountry.GetEntityCollection(UserConnection);
			foreach (var countryItem in countryes) {
				countryName = countryItem.GetTypedColumnValue<string>("Name");
			}
			
			string DATA = "{\"Description\": \"" + countryName + "\", \"НаименованиеПолное\": \"" + countryName + "\", \"КодАльфа3\": \"" + code + "\"}";
				
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_СтраныМира");
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = "POST";
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				string textRef = response.Substring(indexRef + 11, 36);
				responseReader.Close();
				
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM -> 1C: " + "Страны Мира отправлены (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				//
				
				return textRef;
		    }
		    catch (WebException e)
		    {
		    	//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Страны Мира не отправлены (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
		      //return e.Message;
		      return ".Postountry: " + e.Message;
		    }
		    return "-1";
		}
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetUptdatesContactName(string code){
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			//Запрос к odata по коду контактного лица
			string parametrs = "?$format=json&$filter="+ "Code " +"eq" + "'" + code + "'";
			//AddRecordJournal(path + "Catalog_КонтактныеЛица" + parametrs,200);
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_КонтактныеЛица" + parametrs);
			request.Credentials = CredentialCache.DefaultCredentials;
			request.Credentials = new NetworkCredential(login, password);
			WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			responseReader.Close();
			//int indexComma = response.IndexOf("\",", indexDescription);
			int indexRef = response.IndexOf("Ref_Key");
			int delMark = response.IndexOf("DeletionMark");
			
			if(indexRef != -1 && response.Substring(delMark + 15, 5) == "false"){
				
				string s = "Description";
	            int idxDescription = response.IndexOf("Description") + s.Length + 4;
	            int endSymbol = response.Substring(idxDescription).IndexOf("\"");
	            int idxComma = response.Substring(idxDescription).IndexOf(",");
	            endSymbol = idxComma != -1 ? idxComma : endSymbol;
				string description = response.Substring(idxDescription, endSymbol);
				
				
				string textRef = response.Substring(indexRef + 11, 36);
				
				
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Контакты получены (" + (int)we.StatusCode + ")";
				
	
				return description + "_" + textRef;

			}
			
			return "-1";
		}
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]

		public void UpdatesContactName()
		{

			string id1с = "";
			string code1c = "";
			string newAccountName = "";

			var esqAccount = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
			esqAccount.AddAllSchemaColumns();
			var accounts = esqAccount.GetEntityCollection(UserConnection);
			foreach (var accountItem in accounts)
			{
				code1c = accountItem.GetTypedColumnValue<string>("qrt1SCode");
				if (code1c != String.Empty)
				{
					string response = GetUptdatesContactName(code1c);
					
					if(response != "-1"){
						var massResponse = response.Split('_');
						
						
						accountItem.SetColumnValue("Name", (massResponse[0]).Replace(@"\" + "\"", "\""));
						accountItem.SetColumnValue("qrtGuid1c", massResponse[1]);
						accountItem.Save(false);

					}
				}
			}
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetUpdatesAccountName(string code, string inn, Guid countryId, string countryId1C) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs = "";
			int searchColumn = SearchCountIncludeStr(code, "DataVersion");
            if (searchColumn > 1)
            {
                if (!string.IsNullOrEmpty(inn))
                {
                    parametrs = $"?$format=json&$filter=Code eq '{code}' and ИНН eq '{inn}'";
				}
                else
                {
					if(countryId.ToString() != "a570b005-e8bb-df11-b00f-001d60e938c6" && countryId != Guid.Empty){
						parametrs = $"?$format=json&$filter=Code eq '{code}' and СтранаРегистрации_Key eq (guid'{countryId1C}')";
					}
				}
            }else{
            	parametrs = $"?$format=json&$filter=Code eq '{code}'";
            }
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Контрагенты" + parametrs);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			
			responseReader.Close();
			int indexRef = response.IndexOf("Ref_Key");
			int delMark = response.IndexOf("DeletionMark");
			
			//int delMark = response.IndexOf("DeletionMark");
			if(indexRef != -1 && response.Substring(delMark+15) != "false"){
				int indexDescription = response.IndexOf("Description");
				int indexComma = response.IndexOf("\",", indexDescription);
				string textRef = response.Substring(indexRef + 11, 36);
				//
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Контрагенты получены (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				
				
				return response.Substring(indexDescription+15, indexComma-indexDescription-15) + "_" + textRef;
			}
			else{
				return "-1";
			}
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void UpdatesAccountName() {
			
			string id1с = "";
			string code1c = "";
			string newAccountName = "";
			string inn = "";
			string countryCode = "";
			string countryId1C = "";
			Guid countryId = new Guid();
			
			
			var esqAccount = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Account");
			esqAccount.AddAllSchemaColumns();
			var accounts = esqAccount.GetEntityCollection(UserConnection);
			foreach (var accountItem in accounts) {
				code1c = accountItem.GetTypedColumnValue<string>("qrtCode1C");
				inn = accountItem.GetTypedColumnValue<string>("qrtINN");
				countryId = accountItem.GetTypedColumnValue<Guid>("CountryId");
				if(code1c != String.Empty){
					var esqCountry = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Country");
					esqCountry.AddAllSchemaColumns();
					esqCountry.Filters.Add(esqCountry.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", countryId));
					var countryes = esqCountry.GetEntityCollection(UserConnection);
					foreach (var countryItem in countryes) {
						countryCode = countryItem.GetTypedColumnValue<string>("Code");
					}
					countryId1C = GetCountry(countryCode);
					
					string response = GetUpdatesAccountName(code1c, inn, countryId, countryId1C);
					if(response != "-1"){
						var massResponse = response.Split('_');
						if(massResponse[0] != String.Empty && massResponse[1] != String.Empty){
							accountItem.SetColumnValue("Name", massResponse[0].Replace(@"\" + "\"", "\""));
							accountItem.SetColumnValue("qrtGuid1c", massResponse[1]);
							accountItem.Save(false);
						}
					}
				}
			}
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetUpdatesBankNameAndAdres(string bik, string swift) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs = "";
			
			if(swift == String.Empty || swift == null){
				parametrs = "?$filter=Code eq '" + bik + "'" + "&$format=json";
			}
			else if(bik == String.Empty || bik == null){
				parametrs = "?$filter=СВИФТБИК eq '" + swift + "'" + "&$format=json";
			}
			else if((bik == String.Empty || bik == null)  && (swift == String.Empty || swift == null)){
				return "-1";
			}
			else{
				parametrs = "?$filter=СВИФТБИК eq '" + swift + "' and Code eq '" + bik + "'" + "&$format=json";
			}
			
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Банки" + parametrs);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            request.KeepAlive = false;
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			responseReader.Close();
			int indexRef = response.IndexOf("Ref_Key");
			if(indexRef != -1){
				string res = bik + "_" + swift;
				if(res == "_"){
					return "-1";
				}
				try{
					int indexDescription = response.IndexOf("Description");
					int indexCommaDescription = response.IndexOf("\",", indexDescription);
					int indexAdres = response.IndexOf("Адрес");
					int indexCommaAdres = response.IndexOf("\",", indexAdres);
					int indexCity = response.IndexOf("Город");
					int indexCommaCity = response.IndexOf("\",", indexCity);
					int indexCode = response.IndexOf("Code");
					int indexCommaCode = response.IndexOf("\",", indexCode);
					int indexSwift = response.IndexOf("СВИФТБИК");
					int indexCommaSwift = response.IndexOf("\",", indexSwift);
					int indexKoorInvoice= response.IndexOf("КоррСчет");
					int indexCommaKoorInvoice = response.IndexOf("\",", indexKoorInvoice);
					string textRef = response.Substring(indexRef + 11, 36);
					return response.Substring(indexDescription+15, indexCommaDescription-indexDescription-15).Replace(@"\" + "\"", "\"") + "_" + response.Substring(indexAdres+9, indexCommaAdres-indexAdres-9) + "_" + textRef + "_" + response.Substring(indexCity+9, indexCommaCity-indexCity-9) + "_" + response.Substring(indexCode+8, indexCommaCode-indexCode-8) + "_" + response.Substring(indexSwift+12, indexCommaSwift-indexSwift-12) + "_" + response.Substring(indexKoorInvoice+12, indexCommaKoorInvoice-indexKoorInvoice-12);
				}
				catch (Exception e){
					return "-1";
				}
			}
			else{
				return "-1";
			}	
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void UpdatesBankNameAndAdresOne(Guid idBank) {
			
			string bik = "";
			string swift = "";
			string currencyName = "";
			string BillingInfoNameRu = "";
			string BillingInfoNameEng = "";
			string bankAccount = "";
			Guid currencyId = new Guid();
			
			var esqBank = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
			esqBank.AddAllSchemaColumns();
			esqBank.Filters.Add(esqBank.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idBank));
			var banks = esqBank.GetEntityCollection(UserConnection);
			foreach (var bankItem in banks) {
				bik = bankItem.GetTypedColumnValue<string>("qrtBIC");
				swift = bankItem.GetTypedColumnValue<string>("qrtSWIFT");
				//Расчетный счет
				bankAccount = bankItem.GetTypedColumnValue<string>("qrtBankAccount");
				//
				currencyId = bankItem.GetTypedColumnValue<Guid>("qrtCurrencyId");
				if(currencyId != Guid.Empty){
					var esqCurrency = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Currency");
					esqCurrency.AddAllSchemaColumns();
					esqCurrency.Filters.Add(esqCurrency.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", currencyId));
					var currencyes = esqCurrency.GetEntityCollection(UserConnection);
					foreach (var currencyItem in currencyes) {
						currencyName = currencyItem.GetTypedColumnValue<string>("Name");
					}
				}
				string response = GetUpdatesBankNameAndAdres(bik, swift);
				if(response != "-1"){
					var massResponse = response.Split('_');
					if(massResponse[0] != String.Empty && massResponse[1] != String.Empty && massResponse[2] != String.Empty){
						BillingInfoNameRu = "Наименование банка: " + massResponse[0] + "\nТекущий счет: " + currencyName + "\nТранзитный счет: " + currencyName + "\nSWIFT: " + swift + "\nБИК: " + bik + "\nАдрес банка: " + massResponse[3] + ", " + massResponse[1];
						BillingInfoNameEng = "Bank name: " + massResponse[0] + "\nCurrent account in: " + currencyName + "\nTransit account in: " + currencyName + "\nIBAN: " + "" + "\nSWIFT: " + swift + "\nBank address: " + massResponse[3] + ", " + massResponse[1];
						bankItem.SetColumnValue("qrtBank", massResponse[0]);
						bankItem.SetColumnValue("qrtBankENG", massResponse[0]);
						bankItem.SetColumnValue("qrtBankAddressRU", massResponse[3] + ", " + massResponse[1]);
						bankItem.SetColumnValue("qrtBankAddressENG", massResponse[3] + ", " + massResponse[1]);
						bankItem.SetColumnValue("qrtGuid1cBank", massResponse[2]);
						bankItem.SetColumnValue("Name", bankAccount + "," + massResponse[0].Replace(@"\" + "\"", "\"") + "," + currencyName);
						bankItem.SetColumnValue("BillingInfo", BillingInfoNameRu);
						bankItem.SetColumnValue("qrtBillingInfoENG", BillingInfoNameEng);
						bankItem.SetColumnValue("qrtBIC", massResponse[4]);
						bankItem.SetColumnValue("qrtSWIFT", massResponse[5]);
						bankItem.Save(false);
					}
				}
			}
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void UpdatesBankNameAndAdresAll() {
			
			string guid1cBank = "";
			Guid id = new Guid();
			
			var esqBank = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountBillingInfo");
			esqBank.AddAllSchemaColumns();
			var banks = esqBank.GetEntityCollection(UserConnection);
			foreach (var bankItem in banks) {
				guid1cBank = bankItem.GetTypedColumnValue<string>("qrtGuid1cBank");
				id = bankItem.GetTypedColumnValue<Guid>("Id");
				if(guid1cBank != String.Empty){
					UpdatesBankNameAndAdresOne(id);
				}
			}
		}
		
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string UpdatesBankNameAndAdresOneForJs(string bik, string swift) {

			string response = GetUpdatesBankNameAndAdres(bik, swift);
			return response;
			if(response != "-1"){
				var massResponse = response.Split('_');
				if(massResponse[0] != String.Empty && massResponse[1] != String.Empty && massResponse[2] != String.Empty){
					return response;
				}
				else{
					return "-1";
				}
			}
			else{
				return "-1";
			}
		}
		
			/*
		@descrypt пишет значение в справочник лог 1С

		@param message - сообщение, тектосокове многострочное поле, общее пишется буквально все
		@param link1c - ссылка на объект к которому получали доступ
		@param statusLog - статус, тут хранится сообщение об ошибках или успешных действиях
		@param dataStructure - счет
		@param contract - договор
		@param account - контрагент

		@return возвращает статус ввиде строки выполнено
		*/
		public string AddRecordJournal (
			string message,
		 	int status,
			string link1c = "",
			string dataStructure = "",
		    string statusLog = "",
			string structureOutData = "",
			Guid invoice = default,
			Guid account = default,
			Guid contract = default,
			Guid technoContract = default,
			Guid logEvent = default) {
			
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			var esq = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtConnection1CAbipa");
			var connectionId = esq.AddColumn("qrtODataAddress");
			var recordId = esq.AddColumn("Id");
			
			var releaseFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "qrtODataAddress", path);
			esq.Filters.Add(releaseFilter);
			
			var entityCollection = esq.GetEntityCollection(UserConnection);
			if (entityCollection.Count > 0){
				string resultId = entityCollection[0].GetTypedColumnValue<string>(recordId.Name);
				
				if(resultId != ""){
					
				
					var record = new Terrasoft.Configuration.qrtLog1C(UserConnection);
					record.SetDefColumnValues();
					record.SetColumnValue("qrtNumber", "01");
					record.SetColumnValue("qrtMessege", message);
					record.SetColumnValue("CreatedOn", DateTime.Now);
					record.SetColumnValue("qrtLinkOnObject1C", link1c);
					record.SetColumnValue("qrtStructureSendData", dataStructure);
					record.SetColumnValue("qrtStatusLog", statusLog);
					record.SetColumnValue("qrtStructureOutData", structureOutData);
					if(logEvent != default && logEvent != Guid.Empty){
						record.SetColumnValue("qrtLog1CEventId",logEvent);
					}
					if(invoice != default && invoice != Guid.Empty){
						record.SetColumnValue("qrtInvoiceId", invoice);
					}
					if(account != default && account != Guid.Empty){
						record.SetColumnValue("qrtAccountId", account);
					}
					if(contract != default && contract != Guid.Empty){
						record.SetColumnValue("qrtContractId", contract);
					}
					if(technoContract != default && technoContract != Guid.Empty){
						record.SetColumnValue("qrtTechnoContractId", technoContract);
					}
					if (status == 200 || status == 201){
						record.SetColumnValue("qrtTypeMessegeLogId", "4e636c1b-48bf-4e1d-9913-50f1135b6da0");
					} else {
						//error
						record.SetColumnValue("qrtTypeMessegeLogId", "053a6c9c-ecbd-47e2-b9f8-9095db906f76");
					}
					
					record.SetColumnValue("qrtConnection1CId", resultId);
					record.Save(false);
					return "AddRecordJournal: complied";
				}
				return "resultId = null";
			}
			return path;
		}
		public int SearchCountIncludeStr(string code, string substring){
            string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs = $"?$format=json&$filter=Code eq '{code}'";
			var request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Контрагенты" + parametrs);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(login, password);
            WebResponse webResponse = request.GetResponse();
			Stream webStream = webResponse.GetResponseStream();
			StreamReader responseReader = new StreamReader(webStream);
			string response = responseReader.ReadToEnd();
			responseReader.Close();
			
            int idx = response.IndexOf(substring,0);
			int count = 0;

            while (idx > -1)
            {
                count++;
                idx = response.IndexOf(substring, idx + substring.Length);
            }
            return count;
		}
		
		public string getUser1C(Guid contact,string nameUser1C){
			
		

			string parametrs = $"?$format=json&$filter=Description eq '{nameUser1C}'";
			string response = GetJson("Catalog_Пользователи", parametrs);
		
			if(response.IndexOf($"{nameUser1C}") == -1){
				return "-1";
			}
			var user1C = User1C.FromJson(response);
			string refKey = "";
			foreach (var item in user1C.Value)
			{
				if(!item.DeletionMark && !item.Valid && item.Description == nameUser1C)
				{
					refKey = item.RefKey;	
				}
			}
			if(refKey == ""){
				return "-1";
			}
			var esqOwner = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"Contact");
			esqOwner.AddAllSchemaColumns();
			esqOwner.Filters.Add(esqOwner.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contact));
			var ownerCollection = esqOwner.GetEntityCollection(UserConnection);
			foreach(var ownerItem in ownerCollection){
					ownerItem.SetColumnValue("qrtGuidUser1CAbipa",refKey);
					ownerItem.Save(false);
			}
			return refKey;
			
		}
			[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string PostInvoice(Guid idInvoice, string method, string guid1C) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");

			string dopPath = "";
			string pathDoc = "";
			
			string sender = "qrtPreDateProcessInvoice";
			string messageText = "";
			//параметры лога
			string message1C = "";
			string statusLog = "";
			string dataStructure = "";
			string outData = "";
			Guid logEvent = new Guid("4462860a-37e4-4569-8e70-316b706d4c42");
			Guid accountLog = new Guid();
			
			//Заявка 
			Guid order = new Guid();
			//Номер счета
			string number = "";
			//Тип счета
			Guid typeInvoice = new Guid();
			//Дата выставления 
			DateTime startDate = new DateTime();
			//Оплата До
			DateTime dueDatePlan = new DateTime();
			//Номер счета поставщика
			string vendorAccountNumber = "";
			//Договор технический
			Guid contractFromTechno1C = new Guid();
			//Договор основной
			Guid contractFrom1C = new Guid();
			//ндс
			Guid typeVAT = new Guid();

			//Дата выставления входящего
			DateTime dateIncoming = new DateTime();
			//guid 1c
			string guid1c = "";
			//Заказчик исполнитель
			Guid customer = new Guid();
			Guid supplier = new Guid();
			//ответственный
			Guid responsibleContactId = new Guid();
			//закрывающие документы
			string numberAct = "";
			string numberInvoiceCloser = "";
			
			//СуммаДокумента
			double sumDoc = 0;

			//Состояние оплаты
			Guid paymentStatus = new Guid();
			string numberInvoiceVendor = "";
			//Читаем ключевые поля с счета
			var esqInvoice = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"qrtInvoice");
			esqInvoice.AddAllSchemaColumns();
			esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idInvoice));
			var invoiceCollection = esqInvoice.GetEntityCollection(UserConnection);
			foreach(var invoiceItem in invoiceCollection){
               
				number = invoiceItem.GetTypedColumnValue<string>("qrtName");
				numberAct = invoiceItem.GetTypedColumnValue<string>("qrtNumberAct1C");
				numberInvoiceCloser = invoiceItem.GetTypedColumnValue<string>("qrtNumberInvoice1C");
				numberInvoiceVendor = invoiceItem.GetTypedColumnValue<string>("qrtVendorAccountNumber");
				startDate = invoiceItem.GetTypedColumnValue<DateTime>("qrtStartDate");
				vendorAccountNumber = invoiceItem.GetTypedColumnValue<string>("qrtVendorAccountNumber");
				contractFromTechno1C = invoiceItem.GetTypedColumnValue<Guid>("qrtContract1CId");
				contractFrom1C = invoiceItem.GetTypedColumnValue<Guid>("qrtContractId");
				guid1c = invoiceItem.GetTypedColumnValue<string>("qrtGuid1cAbipa");
				customer = invoiceItem.GetTypedColumnValue<Guid>("qrtCustomerId");
				supplier = invoiceItem.GetTypedColumnValue<Guid>("qrtSupplierId");
				dateIncoming = invoiceItem.GetTypedColumnValue<DateTime>("qrtDateInvoiceSupplier");
				typeInvoice = invoiceItem.GetTypedColumnValue<Guid>("qrtTypeId");
				paymentStatus = invoiceItem.GetTypedColumnValue<Guid>("qrtPaymentStatusId");
				dueDatePlan = invoiceItem.GetTypedColumnValue<DateTime>("qrtDueDatePlan");
				responsibleContactId = invoiceItem.GetTypedColumnValue<Guid>("qrtOwnerId");
				typeVAT = invoiceItem.GetTypedColumnValue<Guid>("qrtVatTypeId");
				order = invoiceItem.GetTypedColumnValue<Guid>("qrtOrderId");
			}
			
			if(method == "PATCH"){
				if(!numberAct.Equals(string.Empty) && !numberInvoiceCloser.Equals(string.Empty)){
					messageText = "Изменение счета из црм запрещено, так как в 1С имеются закрывающие документы. Для проведения операции обратитесь в бухгалтерию";
					MsgChannelUtilities.PostMessage(_userConnection,sender, messageText);
					return "-1";
				}
				dopPath = "(guid'" + guid1c + "')";
			}
			
			//Читаем ответственного
			string owner = "";
			string nameUser1C = "";
			var esqOwner = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"Contact");
			esqOwner.AddAllSchemaColumns();
			esqOwner.Filters.Add(esqOwner.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", responsibleContactId));
			var ownerCollection = esqOwner.GetEntityCollection(UserConnection);
			foreach(var ownerItem in ownerCollection){
				owner = ownerItem.GetTypedColumnValue<string>("qrtGuidUser1CAbipaK");
				nameUser1C = ownerItem.GetTypedColumnValue<string>("qrtUserName1CAbipaK");
			}
			if(owner.Equals(String.Empty)){
				owner = getUser1C(responsibleContactId,nameUser1C);
			}
			
			if(owner.Length != 36){
				owner = "38c93148-ab21-11ea-80c8-0cc47a8176e9";
			}
			
			//Переменные для хранения рефов контрагентов
			string refCustomer = PostAccount1C(customer,true);
			string refSupplier = PostAccount1C(supplier, true);
			//Хранение рефов банковских счетов
			string refAccountBankInvoice1CResultCustomer = "";
			string refAccountBankInvoice1CResultSupplier = "";
			
			//Проверка на правильность вернувшегося значения по контрагенту и присвоение рефа банковского счета
			if(refCustomer.Length == 36){
				refAccountBankInvoice1CResultCustomer = GetBankInvoice(idInvoice, 1, refCustomer, "qrtInvoice");
			}
			else{
				return "PostContract.Customer: " + refCustomer;
			}
			
			if(refSupplier.Length == 36){
				refAccountBankInvoice1CResultSupplier = GetBankInvoice(idInvoice, 2, refSupplier,"qrtInvoice");
			}
			else{
				return "PostContract.Supplier: " + refSupplier;
			}
			
			string guidContract = "";
			//читаем договора
			if(contractFrom1C != Guid.Empty && contractFromTechno1C != Guid.Empty){
				//Читаем технический
				var esqContractFrom1CTecnho = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
				esqContractFrom1CTecnho.AddAllSchemaColumns();
				esqContractFrom1CTecnho.Filters.Add(esqContractFrom1CTecnho.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", contractFromTechno1C));
				var contractesTechno = esqContractFrom1CTecnho.GetEntityCollection(UserConnection);

					foreach(var contract in contractesTechno){
							guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
							if(string.IsNullOrEmpty(guidContract)){
								var code1cAbipaK = contract.GetTypedColumnValue<string>("qrtCode1C");
							if(code1cAbipaK.Equals(string.Empty)){
								continue;
							}

							string json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
							if(!json.Contains(code1cAbipaK)){
								continue;
							}
							var contractParse =  Contract.FromJson(json);
							var newRef = contractParse.Value[0].RefKey;
							guidContract = newRef;
							contract.SetColumnValue("qrtGuid1c", newRef);
							contract.Save(false);
						}
				}
			}
			else if(contractFrom1C != Guid.Empty){
				//Читаем основной
				var esqContractFrom1C = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
				esqContractFrom1C.AddAllSchemaColumns();
				esqContractFrom1C.Filters.Add(esqContractFrom1C.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", contractFrom1C));
				var contractes = esqContractFrom1C.GetEntityCollection(UserConnection);
				foreach(var contract in contractes){
					guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
					if(string.IsNullOrEmpty(guidContract)){
						var code1cAbipaK = contract.GetTypedColumnValue<string>("qrt1SCode");
						if(code1cAbipaK.Equals(string.Empty)){
							continue;
						}

						string json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
						if(!json.Contains(code1cAbipaK)){
							continue;
						}
						var contractParse =  Contract.FromJson(json);
						var newRef = contractParse.Value[0].RefKey;
						guidContract = newRef;
						contract.SetColumnValue("qrtGuid1c", newRef);
						contract.Save(false);
					}
				}
			}
			else if(contractFromTechno1C != Guid.Empty){
				{
				//Читаем технический
				var esqContractFrom1CTecnho = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
				esqContractFrom1CTecnho.AddAllSchemaColumns();
				esqContractFrom1CTecnho.Filters.Add(esqContractFrom1CTecnho.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", contractFromTechno1C));
				var contractesTechno = esqContractFrom1CTecnho.GetEntityCollection(UserConnection);

					foreach(var contract in contractesTechno){
							guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
							if(string.IsNullOrEmpty(guidContract)){
								var code1cAbipaK = contract.GetTypedColumnValue<string>("qrtCode1C");
							if(code1cAbipaK.Equals(string.Empty)){
								continue;
							}

							string json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
							if(!json.Contains(code1cAbipaK)){
								continue;
							}
							var contractParse =  Contract.FromJson(json);
							var newRef = contractParse.Value[0].RefKey;
							guidContract = newRef;
							contract.SetColumnValue("qrtGuid1c", newRef);
							contract.Save(false);
						}
				}
			}
			}
			else{
                messageText = "\"Не удалось получить договор\"";
						MsgChannelUtilities.PostMessage(_userConnection,sender, messageText);
						return "-1";
				return "-1";
			}
			//Читаем валюту
		 	string contractData = GetJson("Catalog_ДоговорыКонтрагентов", $"(guid'{guidContract}')?$format=json");
            string refCurrency = contractData.Substring(contractData.IndexOf("\"ВалютаВзаиморасчетов_Key\":") + ("\"ВалютаВзаиморасчетов_Key\": \"").Length, 36);
            
            //Читаем Курс
            string responseCatalogValute = GetJson("InformationRegister_КурсыВалют/SliceLast()",$"?$format=json&$filter=Валюта_Key eq (guid'{refCurrency}')");
            string course = ParseValuteIn1C(responseCatalogValute);
            
			

			//Читаем заявку
			string nameOrder = "";
			var esqOrder = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtOrder");
			esqOrder.AddAllSchemaColumns();
			esqOrder.Filters.Add(esqOrder.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", order));
			var orders = esqOrder.GetEntityCollection(UserConnection);
			foreach(var item in orders){
				nameOrder = item.GetTypedColumnValue<string>("qrtName");
			}
			//Читаем ставку НДС
			double tax = 0;
			Guid VAT = new Guid();
			//Название
			string productName = "";
			//id для 1С
			Guid productId = new Guid();
			string productId1c = "";
			//Номер строки
			string lineNumber = "";
			//Количество
			double count = 0;
			//Цена
			double priceProduct = 0;
			//Сумма
			double sumProduct = 0;
			//Сумма без ндс
			double amountVAT = 0;
			//Список товаров
			List<string> products = new List<string>();
			
			var esqInvoiceService = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtServicesInTheAccountForTheAccountingSystem");
			esqInvoiceService.AddAllSchemaColumns();
			esqInvoiceService.Filters.Add(esqInvoiceService.CreateFilterWithParameters(FilterComparisonType.Equal, "qrtInvoice", idInvoice));
			var invoiceServices = esqInvoiceService.GetEntityCollection(UserConnection);
			foreach(var invoiceServiceItem in invoiceServices){
				lineNumber = invoiceServiceItem.GetTypedColumnValue<string>("qrtName");
				productId = invoiceServiceItem.GetTypedColumnValue<Guid>("qrtNomenclature1CId");
				VAT = invoiceServiceItem.GetTypedColumnValue<Guid>("qrtTaxId");
				//AddRecordJournal(lineNumber.ToString(),200);
				var esqProduct = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtNomenclature1C");
				esqProduct.AddAllSchemaColumns();
				esqProduct.Filters.Add(esqProduct.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", productId));
				var collectProd = esqProduct.GetEntityCollection(UserConnection);
				foreach(var item in collectProd){
					productId1c = item.GetTypedColumnValue<string>("qrtGuidAbipaK");
				}
				
				productName = invoiceServiceItem.GetTypedColumnValue<string>("qrtCustomerName");
				count = invoiceServiceItem.GetTypedColumnValue<double>("qrtNumber");
				amountVAT = invoiceServiceItem.GetTypedColumnValue<double>("qrtVATamount");
				priceProduct =invoiceServiceItem.GetTypedColumnValue<double>("qrtAmount");
				sumProduct = invoiceServiceItem.GetTypedColumnValue<double>("qrtPriceVAT");
				
				//Читаем ставку НДС

				if(!VAT.ToString().Equals("f0b2a7c0-52e6-df11-971b-001d60e938c6") &&
				 !VAT.ToString().Equals("d019db79-8ec8-41e5-a4f8-e4a4c847a8f3")){
					var esqVat = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Tax");
					esqVat.AddAllSchemaColumns();
					esqVat.Filters.Add(esqVat.CreateFilterWithParameters(FilterComparisonType.Equal,"Id",VAT));
					var taxes = esqVat.GetEntityCollection(UserConnection);
					foreach(var taxitem in taxes){
						tax = taxitem.GetTypedColumnValue<double>("Percent");
					}
					if(typeVAT.ToString() != "1d9e4496-77ba-4d1e-8f77-18a24e0cd0c8"){
						sumProduct = Math.Round(sumProduct / (1 + (double)tax/100),2);
					}
					products.Add("{"+$"\"LineNumber\":\"{lineNumber}\"," +
		                         $" \"Номенклатура_Key\":\"{productId1c}\"," +
		                         $"\"Содержание\": \"{productName}\", " +
		                         $"\"Количество\": {count}," +
		                         $" \"Цена\":{priceProduct.ToString().Replace(",",".")}, " +
		                         $"\"Сумма\":{sumProduct.ToString().Replace(",",".")}, " +
		                         $"\"СуммаНДС\":{amountVAT.ToString().Replace(",",".")},"+
		                         $"\"СтавкаНДС\":\"НДС{tax}\"" + "}");	
				}else{
					products.Add("{"+$"\"LineNumber\":\"{lineNumber}\"," +
		                         $" \"Номенклатура_Key\":\"{productId1c}\"," +
		                         $"\"Содержание\": \"{productName}\", " +
		                         $"\"Количество\": {count}," +
		                         $" \"Цена\":{priceProduct.ToString().Replace(",",".")}, " +
		                         $"\"Сумма\":{sumProduct.ToString().Replace(",",".")}, " +
		                         $"\"СуммаНДС\":{amountVAT.ToString().Replace(",",".")},"+
		                         $"\"СтавкаНДС\":\"БезНДС\"" + "}");
				}
			}
				

			//Формирование даты для 1С
			string dateName = dueDatePlan.ToString("yyyy-MM-ddT00:00:00");
			DateTime dateDoc = DateTime.Now;;
			
			//Перебор всех товаров в коллекции и запись в их результирующую строку
			string productsRes = "";
			int prodCount = products.Count;
			for(int i = 0; i < prodCount; i++){

				if(prodCount == 1){
					productsRes+=products[i];
				}
				else if(i != prodCount -1){
					productsRes += products[i] + ",";
				}
				else{
					productsRes+=products[i];
				}
  	 		}
  	 		string resultOrganization = ""; 
			string resultAccount = "";
			string resultBankInvoice = "";
			string postAddress = "";
			string date1C = startDate.ToString("yyyy-MM-ddT00:00:00");
			string DATA = "";
			string includeVAT = (typeVAT.ToString() == "1d9e4496-77ba-4d1e-8f77-18a24e0cd0c8") ? "true":"false";
			
			//Определяем Определяем поля для входящих и исходящих счетов, а также состав отправляемых данных
			//Входящий
			if(typeInvoice.ToString() == "0a59591a-94a7-4e4c-a594-d4e553af9604"){
					resultOrganization = "77050c9a-befb-11e3-9621-d850e63b96c4";
					resultBankInvoice = refAccountBankInvoice1CResultCustomer;
					if(resultBankInvoice.Equals("-1")){
						messageText = "\"Укажите в счете реквизиты Исполнителя и Плательщика\"";
						MsgChannelUtilities.PostMessage(_userConnection,sender, messageText);
						return "-1";
					}
					resultAccount = refSupplier;
					pathDoc = "Document_СчетНаОплатуПоставщика";
					DATA =  "{" +	"\"Организация_Key\": \"" + resultOrganization + "\"," +
									"\"Date\":\"" + date1C + "\"," +
									"\"Контрагент_Key\":\"" + resultAccount + "\","+
									"\"Комментарий\": \"" + nameOrder + "\","+
									"\"Ответственный_Key\":\"" + owner + "\"," +
									"\"Posted\":true," + 
									"\"НомерВходящегоДокумента\":\"" +  numberInvoiceVendor + "\"," + 
									"\"ДатаВходящегоДокумента\":\"" + dateIncoming.ToString("yyyy-MM-dd") + "\"," +
									"\"КурсВзаиморасчетов\":" + course + "," +
									"\"ДоговорКонтрагента_Key\":\"" + guidContract + "\"," + 
									"\"СуммаВключаетНДС\":" + includeVAT  + "," +
									"\"ДокументБезНДС\":" + "false" + "," +
									"\"СтруктурнаяЕдиница_Key\":\"" + resultBankInvoice + "\"," + 
									"\"ВалютаДокумента_Key\":\"" + refCurrency + "\","+
									"\"Склад_Key\":\"636323cd-192d-11e3-a1cc-c86000df0d7b\","+
									"\"Руководитель_Key\":\"777fb6ef-bf02-11e3-9621-d850e63b96c4\","+
									"\"ГлавныйБухгалтер_Key\":\"d075f4b3-021d-11e6-80c6-3640b5ae4b2f\","+
									"\"Товары\":" + "[" + productsRes + "]" + "}";
			}
			//Исходящий
			else if(typeInvoice.ToString() == "808d6728-5dd7-4c06-a843-f8beec0e4ade"){
				
				resultOrganization = "77050c9a-befb-11e3-9621-d850e63b96c4";
				resultBankInvoice = refAccountBankInvoice1CResultSupplier;
				if(resultBankInvoice.Equals("-1")){
					messageText = "\"Укажите в счете реквизиты Исполнителя и Плательщика\"";
					MsgChannelUtilities.PostMessage(_userConnection,sender, messageText);
					return "-1";
				}
				pathDoc = "Document_СчетНаОплатуПокупателю";
				resultAccount = refCustomer;
				Guid typeAdr = new Guid("db1fed51-aab0-49fb-9680-c28cb12a3abd");
				var esqContactInformation = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "AccountAddress");
				esqContactInformation.AddAllSchemaColumns();
				esqContactInformation.Filters.Add(esqContactInformation.CreateFilterWithParameters(FilterComparisonType.Equal, "Account", customer));
				esqContactInformation.Filters.Add(esqContactInformation.CreateFilterWithParameters(FilterComparisonType.Equal, "AddressType", typeAdr));
				var contactInformations = esqContactInformation.GetEntityCollection(UserConnection);
				foreach (var contactInformationItem in contactInformations) {
					postAddress = contactInformationItem.GetTypedColumnValue<string>("Address");
				}
				DATA =  "{" +		"\"Организация_Key\": \"" + resultOrganization + "\"," +
									"\"Контрагент_Key\":\"" + resultAccount + "\","+
									"\"Date\":\"" + date1C + "\"," +
									"\"ДоговорКонтрагента_Key\":\"" + guidContract + "\"," +
									"\"Комментарий\": \"" + nameOrder + "\","+
									"\"Ответственный_Key\":\"" + owner + "\"," +
									"\"Posted\":true," + 
									"\"СуммаВключаетНДС\":" + includeVAT  + "," +
									"\"ДокументБезНДС\":" + "false" + "," +
									"\"СтруктурнаяЕдиница_Key\":\"" + resultBankInvoice + "\"," +
									"\"КурсВзаиморасчетов\":" + course + "," +
									"\"ВалютаДокумента_Key\":\"" + refCurrency + "\","+
									"\"Склад_Key\":\"636323cd-192d-11e3-a1cc-c86000df0d7b\","+
									"\"Руководитель_Key\":\"777fb6ef-bf02-11e3-9621-d850e63b96c4\","+
									"\"ГлавныйБухгалтер_Key\":\"d075f4b3-021d-11e6-80c6-3640b5ae4b2f\","+
									"\"ОрганизацияПолучатель_Key\":\"" + resultOrganization + "\"," +
									"\"АдресДоставки\":\"" + postAddress + "\","+
									"\"Товары\":" + "[" + productsRes + "]" + "}";
			}
			dataStructure = DATA;
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + pathDoc + dopPath);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = method;
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
			
			 try
		    {
		      WebResponse webResponse = request.GetResponse();
		      Stream webStream = webResponse.GetResponseStream();
		      StreamReader responseReader = new StreamReader(webStream);
		      string response = responseReader.ReadToEnd();
		      var invoiceCustomerSuplier = InvoiceCustomerSuplier.FromJson(response);
			  string textRef = invoiceCustomerSuplier.RefKey;
			  string number1C = invoiceCustomerSuplier.Number;
		      responseReader.Close();
			 
			  if(method == "POST"){
			  	 bool isValid = Guid.TryParse(textRef, out var guidOutput);
				  if(!isValid || number1C.Equals(string.Empty)){
				  	//Входящий
				  	if(typeInvoice.ToString() == "0a59591a-94a7-4e4c-a594-d4e553af9604"){
				  		string json = GetJson("Document_СчетНаОплатуПоставщика", $"?$format=json&filter=Number eq '{number1C}' and Date eq {date1C}");
				  		var invoiceParse = Invoice.FromJson(json);
				  		if(!invoiceParse.Value.Length.Equals(0)){
				  			foreach(var itemInvoice in invoiceParse.Value){
				  				textRef = itemInvoice.RefKey.ToString();
				  				number1C = itemInvoice.NumberInvoice;
				  			}
				  		}
				  	}
				  	//Исходящий
				  	else{
				  		string json = GetJson("Document_СчетНаОплатуПокупателю", $"?$format=json&filter=Number eq '{number1C}' and Date eq {date1C}");
				  		var invoiceParse = Invoice.FromJson(json);
				  		if(!invoiceParse.Value.Length.Equals(0)){
				  			foreach(var itemInvoice in invoiceParse.Value){
				  				textRef = itemInvoice.RefKey.ToString();
				  				number1C = itemInvoice.NumberInvoice;
				  			}
				  		}
				  	}
			  	}
			  	foreach (var invoiceItem in invoiceCollection) {
					  invoiceItem.SetColumnValue("qrtGuidAbipaK", textRef);
					  invoiceItem.SetColumnValue("qrtCodeAbipaK", number1C);
					  if(typeInvoice.ToString() != "0a59591a-94a7-4e4c-a594-d4e553af9604"){
					  	invoiceItem.SetColumnValue("qrtVendorAccountNumber",DescriptionCode(number1C));
					  }
					  invoiceItem.Save(false);
				 }
			  }
			  
			
			HttpWebResponse we = (HttpWebResponse)request.GetResponse();
			string message = "BPM -> 1C: " + "Счет отправлен(" + (int)we.StatusCode + ")";
			//PostDatePayDocInInformationRegister(textRef, resultOrganization, dateName, pathDoc);
			AddRecordJournal(message,(int)we.StatusCode);
			AddRecordJournal(
			message1C,
			(int)we.StatusCode,
			Link1C,
			dataStructure,
			message,
			outData,
			idInvoice,
			default,
			contractFrom1C,
			contractFromTechno1C,
			logEvent);
			MsgChannelUtilities.PostMessage(_userConnection,sender, message);
			return textRef;
		    }
		    catch (WebException e)
		    {
				//
				var we = e.Response as HttpWebResponse;
                string errorMessage = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
				string message = string.Empty;
				int status = (int)we.StatusCode;
                switch(status){
                    case 400:
                        if(errorMessage.IndexOf("LineNumber") != -1){
                            message = "Счет не отправлен (" + (int)we.StatusCode + ") " + " " + "Проверьте правильность нумерации на детали Услуги в счете для учетной системы";
                        }else{
                            message = "Счет не отправлен (" + (int)we.StatusCode + ") " + " " + errorMessage;
                        }
                        break;
					case 500:
						if(errorMessage.IndexOf("Непредвиденный символ при чтении JSON") != -1){
                            message = "Счет не отправлен (" + (int)we.StatusCode + ") " + " " + "Невозможно прочитать системный символ \\. Проверьте его наличие в данных, которые отправляете. Название контрагента, счет, деталь услуги для учетной системы";
                        }else{
                            message = "Счет не отправлен (" + (int)we.StatusCode + ") " + " " + errorMessage;
                        }
						break;
                    case 1:
                        message = "Счет не отправлен (" + (int)we.StatusCode + ") " + " " + "невозможно прочитать Guid у отправляемых данных";
                        break;
                    default:
                        message = "Счет не отправлен (" + (int)we.StatusCode + ") " + " " + errorMessage;
                        break;
                }
				
				AddRecordJournal(
				message1C,
				(int)we.StatusCode,
				Link1C,
				dataStructure,
				message,
				outData,
				idInvoice,
				default,
				contractFrom1C,
				contractFromTechno1C,
				logEvent);
				
				MsgChannelUtilities.PostMessage(_userConnection,sender, message);
		    	return "PostInvoice " + e.Message;
		    }
			return "-1";
		}
		
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetInvoice(Guid idInvoice) {
			
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string invoiceGuidName = "";
			string invoiceCodeName = "";
			string document = "";
			Guid typeInvoice = new Guid();
			DateTime startDate = new DateTime();
			var esqInvoice = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtInvoice");
			esqInvoice .AddAllSchemaColumns();
			esqInvoice .Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idInvoice));
			var invoices  = esqInvoice.GetEntityCollection(UserConnection);
			foreach (var invoiceItem in invoices) {
				invoiceGuidName = invoiceItem.GetTypedColumnValue<string>("qrtGuidAbipaK");
				invoiceCodeName = invoiceItem.GetTypedColumnValue<string>("qrtCodeAbipaK");
				startDate = invoiceItem.GetTypedColumnValue<DateTime>("qrtStartDate");
				typeInvoice = invoiceItem.GetTypedColumnValue<Guid>("qrtTypeId");
			}
			if(DateTime.Today > startDate.Date){
				string sender = "qrtPreDateProcessInvoice";
				string messageText = "\"Отправка счета в 1С возможна только с текущей датой счета.Если нужно отправить другой датой, обратитесь к гл.бухгалтеру.\"";
				MsgChannelUtilities.PostMessage(_userConnection,sender, messageText);
				return "-1";
			}
			if(invoiceCodeName != String.Empty){
					//Входящий
				if(typeInvoice.ToString() == "0a59591a-94a7-4e4c-a594-d4e553af9604"){
					document = "Document_СчетНаОплатуПоставщика";
				}
				//Исходящий
				else if(typeInvoice.ToString() == "808d6728-5dd7-4c06-a843-f8beec0e4ade"){
					document = "Document_СчетНаОплатуПокупателю";
				}
				
				if(invoiceGuidName.Equals(String.Empty)){
					string date1C = startDate.ToString("yyyy-MM-ddT00:00:00");
					invoiceGuidName = checkGuidInvoice(document,typeInvoice,invoiceGuidName,invoiceCodeName,date1C);
					//invoiceGuidName = GetIdRecord(invoiceCodeName, "Document_СчетНаОплатуПокупателю");
					
				}
				if(invoiceGuidName == "-1"){
					return "-1";
				}
				string parametrs = "(guid'" + invoiceGuidName + "')";
				var request = (HttpWebRequest)WebRequest.Create(path + document + parametrs);
				request.Credentials = CredentialCache.DefaultCredentials;
	            request.Credentials = new NetworkCredential(login, password);
	            WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				int indexRef = response.IndexOf("Ref_Key");
				//
				responseReader.Close();
				HttpWebResponse we = (HttpWebResponse)request.GetResponse();
				string message = "BPM <- 1C: " + "Счет получен (" + (int)we.StatusCode + ")";
				AddRecordJournal(message,(int)we.StatusCode);
				//
	      		if(indexRef != -1){
	      			string textRef = PostInvoice(idInvoice, "PATCH", invoiceGuidName);
	      			return textRef;
	      		}
	      		else{
	      			string newTextRef = PostInvoice(idInvoice, "POST", "");
					return newTextRef;
	      		}
			}
			else{
				string newTextRef2 = PostInvoice(idInvoice, "POST", "");
				return newTextRef2;
			}
			return "-1";
		}
		public string checkGuidInvoice(string document,Guid typeInvoice,string textRef, string number1C, string date1C){
			 bool isValid = Guid.TryParse(textRef, out var guidOutput);
			  if(!isValid || number1C.Equals(string.Empty)){
			  	//Входящий
			  	if(typeInvoice.ToString() == "0a59591a-94a7-4e4c-a594-d4e553af9604"){
			  		string json = GetJson("Document_СчетНаОплатуПоставщика", $"?$format=json&filter=Number eq '{number1C}' and Date eq {date1C}");
			  		var invoiceParse = Invoice.FromJson(json);
			  		if(!invoiceParse.Value.Length.Equals(0)){
			  			foreach(var itemInvoice in invoiceParse.Value){
			  				textRef = itemInvoice.RefKey.ToString();
			  				number1C = itemInvoice.NumberInvoice;
			  			}
			  		}
			  	}
			  	//Исходящий
			  	else{
			  		string json = GetJson("Document_СчетНаОплатуПокупателю", $"?$format=json&filter=Number eq '{number1C}' and Date eq {date1C}");
			  		var invoiceParse = Invoice.FromJson(json);
			  		if(!invoiceParse.Value.Length.Equals(0)){
			  			foreach(var itemInvoice in invoiceParse.Value){
			  				textRef = itemInvoice.RefKey.ToString();
			  				number1C = itemInvoice.NumberInvoice;
			  			}
			  		}
			  	}
			}
			if(textRef.Length.Equals(36)){
				return textRef;
			}
			return "-1";
		}
		public string[] getDataPayment(string catalog,Guid docMainId, Guid docTechnId, string idInvoice){
			string guidContract = string.Empty;
			bool lockT = true;
			bool lockM = true;
			string[] contractData = new string[2];
			while(lockM || lockT){
				//читаем договора
				if(docTechnId != Guid.Empty && lockT){
					//Читаем технический
					var esqContractFrom1CTecnho = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
					esqContractFrom1CTecnho.AddAllSchemaColumns();
					esqContractFrom1CTecnho.Filters.Add(esqContractFrom1CTecnho.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", docTechnId));
					var contractesTechno = esqContractFrom1CTecnho.GetEntityCollection(UserConnection);
					foreach(var contract in contractesTechno){
						guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
						
						if(string.IsNullOrEmpty(guidContract)){
							lockT = false;
							continue;
						}
						string parametrs = $"?$format=json&$filter=ДоговорКонтрагента_Key eq (guid'{guidContract}')";
						string response = GetJson(catalog, parametrs);
						if(!string.IsNullOrEmpty(guidContract)){
							
							if(response.Contains(idInvoice)){
								contractData[0] = response;
                                contractData[1] = guidContract;
								return contractData;
							}
							lockT = false;
						}else{
							var code1cAbipaK = contract.GetTypedColumnValue<string>("qrtCode1C");
							if(code1cAbipaK.Equals(string.Empty)){
								lockT = false;
								continue;
							}

							string json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
							if(!json.Contains(code1cAbipaK)){
								lockT = false;
								continue;
							}
							var contractParse =  Contract.FromJson(json);
							var newRef = contractParse.Value[0].RefKey;
							guidContract = newRef;
							contract.SetColumnValue("qrtGuid1c", newRef);
							contract.Save(false);

							parametrs = $"?$format=json&$filter=ДоговорКонтрагента_Key eq (guid'{guidContract}')";
							response = GetJson(catalog, parametrs);
							if(response.Contains(idInvoice.ToString())){
								contractData[0] = response;
                                contractData[1] = guidContract;
								return contractData;
							}
							lockT = false;
						}
						
					}
				}
				else if(docMainId != Guid.Empty && lockM){
					//Читаем основной
					var esqContractFrom1C = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
					esqContractFrom1C.AddAllSchemaColumns();
					esqContractFrom1C.Filters.Add(esqContractFrom1C.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", docMainId));
					var contractes = esqContractFrom1C.GetEntityCollection(UserConnection);
					foreach(var contract in contractes){
						guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
						string parametrs = $"?$format=json&$filter=ДоговорКонтрагента_Key eq (guid'{guidContract}')";
						string response = GetJson(catalog, parametrs);
						if(!string.IsNullOrEmpty(guidContract)){
							if(response.Contains(idInvoice)){
								contractData[0] = response;
                                contractData[1] = guidContract;
								return contractData;
							}
							lockM = false;
						}else{
							var code1cAbipaK = contract.GetTypedColumnValue<string>("qrt1SCode");
							if(code1cAbipaK.Equals(string.Empty)){
								lockM = false;
								continue;
							}
							
							string json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
							if(!json.Contains(code1cAbipaK)){
								lockM = false;
								continue;
							}

							var contractParse =  Contract.FromJson(json);
							var newRef = contractParse.Value[0].RefKey;
							guidContract = newRef;
							contract.SetColumnValue("qrtGuid1c", newRef);
							contract.Save(false);
							parametrs = $"?$format=json&$filter=ДоговорКонтрагента_Key eq (guid'{guidContract}')";
							response = GetJson(catalog, parametrs);
							if(response.Contains(idInvoice.ToString())){
								contractData[0] = response;
                                contractData[1] = guidContract;
								return contractData;
							}
							lockM = false;
						}
					}
				}else{
					return contractData;
				}
			}
			
			return contractData;
		}
		public void UpdateStatusInvoice(bool oncerecord = false, Guid idInvoice = default){
				//параметры лога
				string message1C = "";
				string messageText = "";
				string statusLog = "";
				string dataStructure = "";
				string outData = "";
				Guid logEvent = new Guid("4aca41a1-52f7-4433-99ed-b46e2e1df6a7");
				Guid accountLog = new Guid();

                string[] dataContract = new string[2];
				string guid1c = "";
				string  code1c = "";
				string guidContract = "";
				string guid1CAbipa = "";
				Guid oldStatus = new Guid();
				Guid docMainId = new Guid();
				Guid docTechnId = new Guid();
				string invoice1C = "";
				string newStatus = "";
				string status1C = "";
				Guid typeInvoice = new Guid();
				string response = "";
				double sumDocWithVAT = 0;
				double sumDocPartial = 0;
				double remainderSum = 0;
				double amountWithVAT = 0;
				//Читаем ключевые поля с счета
				var esqInvoice = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"qrtInvoice");
				esqInvoice.AddAllSchemaColumns();
				if(oncerecord){
					esqInvoice .Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idInvoice));
				}
				var invoiceCollection = esqInvoice.GetEntityCollection(UserConnection);
				foreach(var invoiceItem in invoiceCollection){
					idInvoice = invoiceItem.GetTypedColumnValue<Guid>("Id");
					amountWithVAT = invoiceItem.GetTypedColumnValue<double>("qrtAmountWichVAT");
					typeInvoice = invoiceItem.GetTypedColumnValue<Guid>("qrtTypeId");
					oldStatus = invoiceItem.GetTypedColumnValue<Guid>("qrtPaymentStatusId");
					guid1c = invoiceItem.GetTypedColumnValue<string>("qrtGuid1cAbipa");
					sumDocWithVAT = invoiceItem.GetTypedColumnValue<double>("qrtAmountPaymentWichVAT");
					docMainId = invoiceItem.GetTypedColumnValue<Guid>("qrtContractId");
					docTechnId = invoiceItem.GetTypedColumnValue<Guid>("qrtContract1CId");

					//определяем тип счета
					if(typeInvoice.ToString().Equals("808d6728-5dd7-4c06-a843-f8beec0e4ade")){
						response = GetJson("InformationRegister_СтатусыДокументов",$"?$format=json&$filter=Документ eq cast(guid'{guid1c}', 'Document_СчетНаОплатуПокупателю')");	
                        dataContract = getDataPayment("Document_ПоступлениеНаРасчетныйСчет",docMainId,docTechnId,guid1c );
					}
					else{
						response = GetJson("InformationRegister_СтатусыДокументов",$"?$format=json&$filter=Документ eq cast(guid'{guid1c}', 'Document_СчетНаОплатуПоставщика')");
                        dataContract =  getDataPayment("Document_СписаниеСРасчетногоСчета",docMainId,docTechnId,guid1c );
                    }
                    guidContract = dataContract[1];
					status1C = ParseStatusPayment(response);
					
					if(status1C.Equals("-1")){
						messageText = "Не удалось получить статус";
						AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										idInvoice,
										default,
										docMainId,
										docTechnId,
										logEvent);
						
						continue;
					}
					outData += "Текущий статус 1С "  + status1C + "\n";
					newStatus = JuxtapositionCrmAt1CStatus(status1C, oldStatus.ToString());
				
					if(newStatus.Equals("-1")){
						messageText = "Статус не изменился";
						AddRecordJournal(
										message1C,
										200,
										Link1C,
										dataStructure,
										messageText,
										outData,
										idInvoice,
										default,
										docMainId,
										docTechnId,
										logEvent);
						Link1C = "";
						outData = "";
						messageText = "";
						continue;
					}
					outData += "Новый статус 1С "  + newStatus + "\n";
					if(newStatus.Equals("13d69a11-4994-4372-813f-831c985fa943") || newStatus.Equals("bf5ce88e-aefd-47b4-841d-2eb7d6f18fd4")){
						Guid currencyPay = new Guid();
						double rateDoc = 0;
						if(typeInvoice.ToString().Equals("808d6728-5dd7-4c06-a843-f8beec0e4ade")){
							//Читаем валюту
						 	string contractData = GetJson("Catalog_ДоговорыКонтрагентов", $"(guid'{guidContract}')?$format=json");
							if(!contractData.Contains(contractData)){
								continue;
							}
				            string refCurrency = contractData.Substring(contractData.IndexOf("\"ВалютаВзаиморасчетов_Key\":") + ("\"ВалютаВзаиморасчетов_Key\": \"").Length, 36);
				          	outData += "Guid валюты 1С "  + refCurrency + "\n";
							response = dataContract[0];
							if(response == "-1"){
                                messageText = "Проблема с суммой документа";
					            	AddRecordJournal(
										message1C,
										200,
										Link1C,
										dataStructure,
										messageText,
										outData,
										idInvoice,
										default,
										docMainId,
										docTechnId,
										logEvent);
                                        Link1C = "";
                                        outData = "";
                                        messageText = "";
							}
							
							var moneyReceipt = MoneyReceipt.FromJson(response);
							sumDocPartial = SumDocPartialOutgoing(guid1c,moneyReceipt);
							outData += "Частичная оплата 1С "  + sumDocPartial + "\n";
							rateDoc = GetCurrencyDocPay(moneyReceipt, guid1c);
							outData += "Курс 1С "  + rateDoc + "\n";
							currencyPay = DeterminateExchangeRate(refCurrency);
							outData += "Представление валюты 1С "  + currencyPay + "\n";
						
						}
						else{
							string contractData = GetJson("Catalog_ДоговорыКонтрагентов", $"(guid'{guidContract}')?$format=json");
				            string refCurrency = contractData.Substring(contractData.IndexOf("\"ВалютаВзаиморасчетов_Key\":") + ("\"ВалютаВзаиморасчетов_Key\": \"").Length, 36);
							string parametrs = $"?$format=json&$filter=ДоговорКонтрагента_Key eq (guid'{guidContract}')";
							response = dataContract[0];
							
							if(response == "-1"){
                                AddRecordJournal(
										message1C,
										200,
										Link1C,
										dataStructure,
										messageText,
										outData,
										idInvoice,
										default,
										docMainId,
										docTechnId,
										logEvent);
                                        Link1C = "";
                                        outData = "";
                                        messageText = "";
								continue;
							}
							var moneyReceipt = MoneyReceipt.FromJson(response);
							sumDocPartial = SumDocPartialIncoming(guid1c,moneyReceipt);
							outData += "Частичная оплата 1С "  + sumDocPartial + "\n";
							rateDoc = GetCurrencyDocPay(moneyReceipt, guid1c);
							outData += "Курс 1С "  + rateDoc + "\n";
							currencyPay = DeterminateExchangeRate(refCurrency);
							outData += "Представление валюты 1С "  + currencyPay + "\n";
						}
						remainderSum = amountWithVAT - sumDocPartial;

						outData += $"б.в остаток{remainderSum} * {rateDoc} = {remainderSum * rateDoc}" + "\n";
						
						invoiceItem.SetColumnValue("qrtPrimaryRemainderPaid",Math.Round(remainderSum * rateDoc, 2));
						
						outData += $"Остаток к оплате {amountWithVAT} - {sumDocPartial}" + "\n";
						invoiceItem.SetColumnValue("qrtRemainderPaid",remainderSum);
						
						invoiceItem.SetColumnValue("qrtBillPaymentCurrencyId", currencyPay);

						outData += $"Курс из 1с 1/{rateDoc}" + Math.Round(1/rateDoc,4) + "\n";
						invoiceItem.SetColumnValue("qrtCalculateCurrency1CSumDocPay",Math.Round(1/rateDoc,4));
						
						outData += "Курс документа "  + rateDoc + "\n";
						invoiceItem.SetColumnValue("qrtRateDocPay",rateDoc);
						
						outData += "Сумма документа "  + sumDocPartial + "\n";
						invoiceItem.SetColumnValue("qrtAmountPaymentWichVAT",sumDocPartial);
						
						outData += $"б.в валюта {sumDocPartial} * {rateDoc} = {sumDocPartial * rateDoc}" + "\n";
						invoiceItem.SetColumnValue("qrtPrimaryAmountPaymentWichVAT",Math.Round(sumDocPartial * rateDoc, 2));
						
						outData += $"Дата оплаты факт " + DateTime.Now.ToString() + "\n";
						invoiceItem.SetColumnValue("qrtDueDateFact",DateTime.Now);	
					
					}
					invoiceItem.SetColumnValue("qrtPaymentStatusId", Guid.Parse(newStatus));
					AddRecordJournal(
									message1C,
									200,
									Link1C,
									dataStructure,
									messageText,
									outData,
									idInvoice,
									default,
									docMainId,
									docTechnId,
									logEvent);
					invoiceItem.Save(false);
					
					
					Link1C = "";
					outData = "";
					messageText = "";

				}
			
		}
		
		public string JuxtapositionCrmAt1CStatus(string  status1C, string statusCrm){
			switch(status1C){
				case "Оплачен":
					if(statusCrm.Equals("bf5ce88e-aefd-47b4-841d-2eb7d6f18fd4")){
						return "-1";
					}
					return "bf5ce88e-aefd-47b4-841d-2eb7d6f18fd4";
				case "ОплаченЧастично":

					return "13d69a11-4994-4372-813f-831c985fa943";
				case "НеОплачен":
					if(statusCrm.Equals("185f4a47-f5b0-4fb9-85bc-676c5c9b9809")){
						return "-1";
					}
					return "185f4a47-f5b0-4fb9-85bc-676c5c9b9809";
				case "Отменен":
					if(statusCrm.Equals("f7314e30-54ce-4a1b-8d86-24dfcb9625d7")){
						return "-1";
					}
					return "f7314e30-54ce-4a1b-8d86-24dfcb9625d7";
				default:
					return "-1";
			}
		}
		
		public string ParseStatusPayment(string response){
			int idxStatusStart = response.IndexOf("Статус\":");
			if(idxStatusStart == -1){
				return "-1";
			}
			string subStatusString = response.Substring(idxStatusStart + 10);
			AddRecordJournal("1" + subStatusString,200);
			int idxStatusEnd = subStatusString.IndexOf("\",");
			string status = response.Substring(idxStatusStart + 10, idxStatusEnd);
			return status;
		}
		public double GetCurrencyDocPay(MoneyReceipt moneyReceipt, string idInvoice){
			AddRecordJournal(Link1C,200);
			var collection = moneyReceipt.Value;
			double rate = 1;
			foreach(var itemMoneyReceipt in collection){
				foreach(var itemDecrypt in itemMoneyReceipt.DescyptPay){
						if(itemDecrypt.AccountPaymentKey.Equals(idInvoice)){
							rate = itemDecrypt.ExchangeRate;
						}
						
					}
			}
			return rate;
		}
		public double SumDocPartialIncoming(string invoiceGuidName, MoneyReceipt moneyReceipt){


			var collection = moneyReceipt.Value;
			double sumPartialDoc = 0; 
			foreach(var itemMoneyReceipt in collection){
				foreach (var itemDecrypt in itemMoneyReceipt.DescyptPay)
				{
					if(itemDecrypt.AccountPaymentKey.Equals(invoiceGuidName)){
						if(!itemDecrypt.SumCurrencyDocument.Equals(0)){
							sumPartialDoc += itemDecrypt.SumCurrencyDocument;
						}else{
							sumPartialDoc += itemDecrypt.SumPaymentRub;
						}
					}
				}

				
			}
			
            return sumPartialDoc;
		}

		
		public double SumDocPartialOutgoing (string invoiceGuidName, MoneyReceipt moneyReceipt){
			var collection = moneyReceipt.Value;
			double sumPartialDoc = 0; 
			foreach(var itemMoneyReceipt in collection){
				foreach (var itemDecrypt in itemMoneyReceipt.DescyptPay)
				{
					if(itemDecrypt.AccountPaymentKey.Equals(invoiceGuidName)){
						if(!itemDecrypt.SumCurrencyDocument.Equals(0)){
							sumPartialDoc += itemDecrypt.SumCurrencyDocument;
						}else{
							sumPartialDoc += itemDecrypt.SumPaymentRub;
						}
					}
					
				}
			}
			
            return sumPartialDoc;
		}
		
		public void GetАrticle1С(Guid invoice, Services[] services){
			AddRecordJournal($"Длина {services.Length}",200);
			var esqService = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"qrtServicesInTheAccountForTheAccountingSystem");
			esqService.Filters.LogicalOperation = LogicalOperationStrict.And;
			esqService.Filters.Add(esqService.CreateFilterWithParameters(FilterComparisonType.Equal, "qrtInvoice",invoice));
			esqService.AddAllSchemaColumns();
			var servicesCollection = esqService.GetEntityCollection(UserConnection);
			foreach(var item in servicesCollection){
				
				var line = item.GetTypedColumnValue<string>("qrtName");
				var priceExVAT = item.GetTypedColumnValue<double>("qrtTotalPrice");
				var sum = item.GetTypedColumnValue<double>("qrtPriceVAT");
				AddRecordJournal($"Строка {line}",200);
				foreach(var service in services){
					AddRecordJournal($"Строка из 1С {service.LineNumber}",200);
					AddRecordJournal($"Суммы из 1С {service.Price} {service.Sum}",200);
					AddRecordJournal($"1c {service.LineNumber} crm{line}",200);
					if(line[0] == '0'){
						line = line.Remove(0,1);
					}
					if (service.LineNumber.Equals(line)){
							AddRecordJournal($"Суммы из 1С {service.Price} {service.Sum}",200);
							item.SetColumnValue("qrtTotalPrice",service.Price);
							item.SetColumnValue("qrtPriceVAT",service.Sum);
						item.Save(false);
						break;
					}
				}
			}
		}
		public void GetColumnOfActsAndImplementationAndInvoieIssued(bool oncerecord = false, Guid idInvoice = default){
				//параметры лога
				string message1C = "";
				string messageText = "";
				string statusLog = "";
				string dataStructure = "";
				string outData = "";

				Guid logEvent = new Guid("1f52f666-1016-4ad2-b43f-013b2d7d98b6");
				Guid accountLog = new Guid();
				
				string guid1c = "";
				string json = "";
				string refActs = "";
				string typeInvoice = "";
                string typeVAT = "";
                string numberInvoice1C = "";
				string order = "";
                Guid id = new Guid();
				double sumAct = 0;
				string guidContract = string.Empty;
				Guid contract1 = new Guid();
				Guid contractCategoryFor1C = new Guid();
				Guid executor = new Guid();
				//Читаем ключевые поля с счета
				var esqInvoice = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"qrtInvoice");
				esqInvoice.Filters.LogicalOperation = LogicalOperationStrict.And;
				//esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.NotEqual,"qrtContract1C.qrtContractCategoryFor1C.Id", "084958ff-522d-44b1-a9e5-2b9a5c0aa89f"));
				var columnName= esqInvoice.AddColumn("qrtOrder.qrtName").Name;
				esqInvoice.AddAllSchemaColumns();
				if(oncerecord){
					esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idInvoice));
				}
				var invoiceCollection = esqInvoice.GetEntityCollection(UserConnection);
				foreach(var invoiceItem in invoiceCollection){
					contract1 = invoiceItem.GetTypedColumnValue<Guid>("qrtContract1CId");
					guid1c = invoiceItem.GetTypedColumnValue<string>("qrtGuidAbipaK");
					sumAct = invoiceItem.GetTypedColumnValue<double>("qrtActAmount");
					typeInvoice = invoiceItem.GetTypedColumnValue<Guid>("qrtTypeId").ToString();
					typeVAT = invoiceItem.GetTypedColumnValue<Guid>("qrtVatTypeId").ToString();
                    executor = invoiceItem.GetTypedColumnValue<Guid>("qrtSupplierId");
					order = invoiceItem.GetTypedColumnValue<string>(columnName);
                    numberInvoice1C = invoiceItem.GetTypedColumnValue<string>("qrtNumberInvoice1C");
                    id = invoiceItem.GetTypedColumnValue<Guid>("Id");
                    var docMainId = invoiceItem.GetTypedColumnValue<Guid>("qrtContractId");
					var docTechnId = invoiceItem.GetTypedColumnValue<Guid>("qrtContract1CId");

					if(!sumAct.Equals(0) && !numberInvoice1C.Equals(string.Empty)){
						continue;
					}
					if(String.IsNullOrEmpty(guid1c)){
                        messageText = "Guid счета не заполнен";
						AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
						
						continue;
					}
					if(!contract1.Equals(default) ||  contract1 != Guid.Empty){
					
						var esqContract = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"qrtInvoice");
						esqContract.Filters.Add(esqContract.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", contract1));
						esqContract.AddAllSchemaColumns();
						var contractCollection = esqContract.GetEntityCollection(UserConnection);
						foreach(var contractItem in contractCollection){
							contractCategoryFor1C = contractItem.GetTypedColumnValue<Guid>("qrtContractCategoryFor1CId");
						}
					}
					if(contractCategoryFor1C.ToString() == "084958ff-522d-44b1-a9e5-2b9a5c0aa89f"){
                        messageText = "проверьте категорию договора";
						AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
					
						continue;
					}
					
					
				
					if(typeInvoice.Equals("808d6728-5dd7-4c06-a843-f8beec0e4ade")){
						json = GetJson("Document_РеализацияТоваровУслуг", $"?$format=json&$filter=СчетНаОплатуПокупателю_Key eq (guid'{guid1c}')");
					
						if(!json.Contains(guid1c) ){	
								messageText = "Не удалось найти документ реализации товаров и услуг";
								AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
													
								continue;
							}
					}else{
						json = GetJson("Document_ПоступлениеТоваровУслуг", $"?$format=json&$filter=СчетНаОплатуПоставщика_Key eq (guid'{guid1c}')");
						if(!json.Contains(guid1c) ){
							//читаем договора
							if(docMainId != Guid.Empty && docTechnId != Guid.Empty){
								//Читаем технический
								var esqContractFrom1CTecnho = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
								esqContractFrom1CTecnho.AddAllSchemaColumns();
								esqContractFrom1CTecnho.Filters.Add(esqContractFrom1CTecnho.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", docTechnId));
								var contractesTechno = esqContractFrom1CTecnho.GetEntityCollection(UserConnection);
								foreach(var contract in contractesTechno){
									guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
									if(string.IsNullOrEmpty(guidContract)){
										var code1cAbipaK = contract.GetTypedColumnValue<string>("qrtCode1C");
										if(code1cAbipaK.Equals(string.Empty)){
											messageText = "Технический договор:пустой код 1с Абипа";
											AddRecordJournal(
												message1C,
												400,
												Link1C,
												dataStructure,
												messageText,
												outData,
												idInvoice,
												default,
												default,
												default,
												logEvent);
											continue;
										}

										json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
										if(!json.Contains(code1cAbipaK)){
											messageText = "По заданным параметрам не удалось получить договор " + $"?$format=json&$filter=Code eq '{code1cAbipaK}'";
											AddRecordJournal(
												message1C,
												400,
												Link1C,
												dataStructure,
												messageText,
												outData,
												idInvoice,
												default,
												docMainId,
												docTechnId,
												logEvent);
											continue;
										}
										var contractParse =  Contract.FromJson(json);
										var newRef = contractParse.Value[0].RefKey;
										guidContract = newRef;
										contract.SetColumnValue("qrtGuid1c", newRef);
										contract.Save(false);
									}
									
								}
							}
							else if(docMainId != Guid.Empty){
								//Читаем основной
								var esqContractFrom1C = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtContract");
								esqContractFrom1C.AddAllSchemaColumns();
								esqContractFrom1C.Filters.Add(esqContractFrom1C.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", docMainId));
								var contractes = esqContractFrom1C.GetEntityCollection(UserConnection);
								foreach(var contract in contractes){
									guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
									if(string.IsNullOrEmpty(guidContract)){
										var code1cAbipaK = contract.GetTypedColumnValue<string>("qrt1SCode");
										if(code1cAbipaK.Equals(string.Empty)){
												messageText = "По заданным параметрам не удалось получить договор " + $"?$format=json&$filter=Code eq '{code1cAbipaK}'";
											AddRecordJournal(
												message1C,
												400,
												Link1C,
												dataStructure,
												messageText,
												outData,
												idInvoice,
												default,
												docMainId,
												docTechnId,
												logEvent);
												Link1C = "";
												outData = "";
												messageText = "";
											continue;
										}

										json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
										if(!json.Contains(code1cAbipaK)){
											messageText = "Основной договор:пустой код 1С Абипа";
											AddRecordJournal(
												message1C,
												400,
												Link1C,
												dataStructure,
												messageText,
												outData,
												idInvoice,
												default,
												docMainId,
												docTechnId,
												logEvent);
											continue;
										}
										var contractParse =  Contract.FromJson(json);
										var newRef = contractParse.Value[0].RefKey;
										guidContract = newRef;
										contract.SetColumnValue("qrtGuid1c", newRef);
										contract.Save(false);
									}
								}
							}
							else if(docTechnId != Guid.Empty){
								var esqContractFrom1CTecnho = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "qrtTechnicalContracts1C");
								esqContractFrom1CTecnho.AddAllSchemaColumns();
								esqContractFrom1CTecnho.Filters.Add(esqContractFrom1CTecnho.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", docTechnId));
								var contractesTechno = esqContractFrom1CTecnho.GetEntityCollection(UserConnection);
								foreach(var contract in contractesTechno){
									guidContract = contract.GetTypedColumnValue<string>("qrtGuid1c");
									if(string.IsNullOrEmpty(guidContract)){
										var code1cAbipaK = contract.GetTypedColumnValue<string>("qrtCode1C");
										if(code1cAbipaK.Equals(string.Empty)){
												messageText = "Технический договор:пустой код 1с Абипа";
											AddRecordJournal(
												message1C,
												400,
												Link1C,
												dataStructure,
												messageText,
												outData,
												idInvoice,
												default,
												docMainId,
												docTechnId,
												logEvent);
											
											continue;
										}

										json = GetJson("Catalog_ДоговорыКонтрагентов", $"?$format=json&$filter=Code eq '{code1cAbipaK}'");
										if(!json.Contains(code1cAbipaK)){
											AddRecordJournal(
												message1C,
												400,
												Link1C,
												dataStructure,
												messageText,
												outData,
												idInvoice,
												default,
												docMainId,
												docTechnId,
												logEvent);
											continue;
										}
										var contractParse =  Contract.FromJson(json);
										var newRef = contractParse.Value[0].RefKey;
										guidContract = newRef;
										contract.SetColumnValue("qrtGuid1c", newRef);
										contract.Save(false);
									}
									
								}
							}
							order = order.Trim(' ');
							json = GetJson("Document_ПоступлениеТоваровУслуг",$"?$format=json&$filter=ДоговорКонтрагента_Key eq (guid'{guidContract}') and startswith(Комментарий,'{order}_') eq true");
				
							if(!json.Contains(order)){
									json = GetJson("Document_ПоступлениеТоваровУслуг",$"?$format=json&$filter=ДоговорКонтрагента_Key eq (guid'{guidContract}') and like(Комментарий,'%{order}%')");
									if(!json.Contains(order)){
										messageText = "Не удалось найти документ поступление товаров и услуг";

										AddRecordJournal(
												message1C,
												400,
												Link1C,
												dataStructure,
												messageText,
												outData,
												id,
												default,
												default,
												default,
												logEvent);
											continue;
									}
								}
						}
							
						
					}
					
					var actsAndImplementation = ActsAndImplementation.FromJson(json);
					var collectionActsAndImplementation  = actsAndImplementation.Value;
					foreach(var itemActsAndImplementation in collectionActsAndImplementation){
						outData += $"Получаем курс {itemActsAndImplementation.ExchangeRate}\n";
						invoiceItem.SetColumnValue("qrtActAmountCalculationRate", itemActsAndImplementation.ExchangeRate);

						outData += $"Получаем курс относительно рубля {itemActsAndImplementation.ExchangeRate}\n";
						invoiceItem.SetColumnValue("qrtCalculateCurrency1C",Math.Round(1/itemActsAndImplementation.ExchangeRate,4));
						
						outData += $"Сумма акта с НДС {itemActsAndImplementation.SumDoc}\n";
						invoiceItem.SetColumnValue("qrtActAmountVAT", itemActsAndImplementation.SumDoc);
						double sumExVAT = 0;
						if((executor.ToString() == "0b6363fd-24bb-40d4-b291-de8a2b05ddbb" ||
						   executor.ToString() == "6e8a99e7-c6e7-417a-9987-a284e18db373") &&
						   !typeInvoice.Equals("808d6728-5dd7-4c06-a843-f8beec0e4ade")){

							foreach(var service in itemActsAndImplementation.AgentServices){
							
								if(typeVAT == "1d9e4496-77ba-4d1e-8f77-18a24e0cd0c8"){
									sumExVAT+= service.Sum - service.SumVAT;
									continue;
								}
								sumExVAT+= service.Sum;
							}
						}else{
							foreach(var service in itemActsAndImplementation.Services){
							
								if(typeVAT == "1d9e4496-77ba-4d1e-8f77-18a24e0cd0c8"){
									sumExVAT+= service.Sum - service.SumVAT;
									continue;
								}
								sumExVAT+= service.Sum;
							}
						}
						outData += $"Сумма акта без НДС {sumExVAT}\n";
						invoiceItem.SetColumnValue("qrtActAmount", sumExVAT);
						//invoiceItem.SetColumnValue("qrtPrimaryActAmount", Math.Round(itemActsAndImplementation.SumDoc * itemActsAndImplementation.ExchangeRate, 2));
						
						outData += $"Номер акта {itemActsAndImplementation.Number}\n";
						invoiceItem.SetColumnValue("qrtActNumber", DescriptionCode(itemActsAndImplementation.Number));
						invoiceItem.SetColumnValue("qrtNumberAct1C", itemActsAndImplementation.Number);
						outData += $"Дата акта {itemActsAndImplementation.DateAct}\n";
						invoiceItem.SetColumnValue("qrtActDate", itemActsAndImplementation.DateAct);
						refActs = itemActsAndImplementation.RefKey;
						
						var currencyForCrm = DeterminateExchangeRate(itemActsAndImplementation.CurrencyDocKey);
						outData += "Получаем валюту " + currencyForCrm + "\n";
						if(!currencyForCrm.Equals(default)){
							outData += $"Валюта {currencyForCrm}" + "\n";
							invoiceItem.SetColumnValue("qrtActCurrencyId",currencyForCrm);
						}
						double primarySum = 0;
						
						//получаем сумму в базовой валюте
						if(typeInvoice.Equals("808d6728-5dd7-4c06-a843-f8beec0e4ade")){

							json = GetJson("AccumulationRegister_РеализацияУслуг",
										$"?$format=json&$filter=Recorder eq cast(guid'{refActs}', 'Document_РеализацияТоваровУслуг')");
							
							if(!json.Contains(refActs)){
								outData += "Не удалось получить рализацию по акту \n";
								messageText = "Не удалось найти реализацию услуг по акту";
									AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
								invoiceItem.Save(false);
								
							}else{
								var realizationService1C = RealizationService1C.FromJson(json);
								var collection = realizationService1C.Value;
								
								foreach (var i in collection)
								{
									var collectionnew = i.RecordSet;
									foreach (var j in collectionnew)
									{
										GetArticleRub(id, actsAndImplementation.Value[0].Services, j);
										primarySum += j.Total;
									}
								}
							}
							
						}

						else{
							if(executor.ToString() == "0b6363fd-24bb-40d4-b291-de8a2b05ddbb" ||
						   		executor.ToString() == "6e8a99e7-c6e7-417a-9987-a284e18db373"){
								json = GetJson("AccumulationRegister_ЗакупленныеТоварыКомитентов",
								$"?$format=json&$filter=Recorder eq cast(guid'{refActs}', 'Document_ПоступлениеТоваровУслуг')");
								if(!json.Contains(refActs)){
									invoiceItem.Save(false);
									messageText = "Не удалось найти ЗакупленныеТоварыКомитентов услуг по акту";
									AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
										// Link1C = "";
										// outData = "";
									
								}else{
									var payServiceesKommitent = PayServiceesKommitent.FromJson(json);
									var collection = payServiceesKommitent.Value;
								
									foreach (var i in collection)
									{
										var collectionnew = i.RecordSet;
										foreach (var j in collectionnew)
										{
					
											primarySum += j.Total;
										}
									}
								}
						   }else{
							   json = GetJson("InformationRegister_РублевыеСуммыДокументовВВалюте",
										$"?$format=json&$filter=Recorder eq cast(guid'{refActs}', 'Document_ПоступлениеТоваровУслуг')");
								if(!json.Contains(refActs)){
									invoiceItem.Save(false);
									messageText = "Не удалось найти РублевыеСуммыДокументовВВалюте услуг по акту";
									AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
										// Link1C = "";
										// outData = "";
									
								}   
								else{
									var rubSumInCurrency = RubSumInCurrency.FromJson(json);
									var collection = rubSumInCurrency.Value;
								
									foreach (var i in collection)
									{
										var collectionnew = i.RecordSet;
										foreach (var j in collectionnew)
										{
											primarySum += j.Total;
										}
									}
								}

						   }
				
						}
						outData = "Сумма в базовой валюте " + primarySum + "\n";
						outData += currencyForCrm.ToString() + "==" + "5fb76920-53e6-df11-971b-001d60e938c6" + "Валюта" + (currencyForCrm.ToString() == "5fb76920-53e6-df11-971b-001d60e938c6") + "\n";
						if(currencyForCrm.ToString() == "5fb76920-53e6-df11-971b-001d60e938c6"){
							outData += "Зашли в условие";
							if(primarySum ==  0){
								outData += "Зашли в условие 1\n";
								outData += $"Сумма в базовой валюте  НДС {sumExVAT}\n";
								invoiceItem.SetColumnValue("qrtPrimaryActAmount", sumExVAT);
								outData += $"Сумма в базовой валюте без НДС {itemActsAndImplementation.SumDoc}\n";
								invoiceItem.SetColumnValue("qrtPrimaryActAmountVAT",itemActsAndImplementation.SumDoc );
							}
							else{
								outData += "Зашли в условие 2\n";
								outData += $"Сумма в базовой валюте  НДС {primarySum}\n";
								invoiceItem.SetColumnValue("qrtPrimaryActAmount", primarySum);
								outData += $"Сумма в базовой валюте без НДС {primarySum}\n";
								invoiceItem.SetColumnValue("qrtPrimaryActAmountVAT",itemActsAndImplementation.SumDoc );
							}
							
						}else{
							outData += "Зашли в условие 3\n";
							outData += $"Сумма в базовой валюте  НДС {primarySum}\n";
							invoiceItem.SetColumnValue("qrtPrimaryActAmount", primarySum);
							outData += $"Сумма в базовой валюте без НДС {primarySum}\n";
							invoiceItem.SetColumnValue("qrtPrimaryActAmountVAT",primarySum);
						}
					}
					
					
					if(typeInvoice.Equals("808d6728-5dd7-4c06-a843-f8beec0e4ade")){
						json = GetJson("Document_СчетФактураВыданный", $"?$format=json&$filter=ДокументОснование eq cast(guid'{refActs}', 'Document_РеализацияТоваровУслуг')" );
					
						
					}
					else{
						json = GetJson("Document_СчетФактураПолученный", $"?$format=json&$filter=ДокументОснование eq cast(guid'{refActs}', 'Document_ПоступлениеТоваровУслуг')");
						
					}
					
					if(!json.Contains(refActs)){
						messageText = "Не удалось найти счет фактуру услуг по акту 1";
						AddRecordJournal(
										message1C,
										400,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
						invoiceItem.Save(false);
					
						// Link1C = "";
						// outData = "";
						continue;
					}
			
					var invoicecesIssued = InvoicecesIssued.FromJson(json);
					var collectionInvoiceIssued = invoicecesIssued.Value;
					foreach(var itemInvoiceIssued in collectionInvoiceIssued){
						outData += $"Номер счет фактуры 1{itemInvoiceIssued.Number}\n";
						invoiceItem.SetColumnValue("qrtNumberInvoice1C", itemInvoiceIssued.Number);
						invoiceItem.SetColumnValue("qrtInvoiceNumber", itemInvoiceIssued.DescriptionNumber);
						outData += $"Дата счет фактуры 1 {itemInvoiceIssued.DateInvoice}\n";
						invoiceItem.SetColumnValue("qrtInvoiceDate", itemInvoiceIssued.DateInvoice);
					}
					
					invoiceItem.Save(false);
					messageText = "Данные по актам получены успешно";
					AddRecordJournal(
										message1C,
										200,
										Link1C,
										dataStructure,
										messageText,
										outData,
										id,
										default,
										default,
										default,
										logEvent);
					
				}
		}
		public string DescriptionCode(string code) {
			string result = "";
			var codeExPrefix = code.Split('-')[1];
			//Метод удаляет ноли, которые не формируют номер
			foreach(var rune in codeExPrefix)
            {
				if (rune != '0') {
					var idx = code.IndexOf(rune);
					return code.Substring(idx);
				}
            }
			return "-1";
		}
		public void GetColumnOfReportKommmitentAndImplementationAndInvoieIssued(bool oncerecord = false, Guid idInvoice = default){
				string guid1c = "";
				string json = "";
				string refActs = "";
				string typeInvoice = "";
                string typeVAT = "";
                string numberInvoice1C = "";
				double sumAct = 0;
				string order = "";
                string outData = "";
				Guid guidKommitent = new Guid("bfe0ddb9-7d8f-47e6-9267-bd80deb41c36");
				//Читаем ключевые поля с счета
				var esqInvoice = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"qrtInvoice");
				esqInvoice.Filters.LogicalOperation = LogicalOperationStrict.And;
				esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.IsNotNull, "qrtGuidAbipaK"));
				esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.IsNotNull, "qrtCodeAbipaK"));
				esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.IsNull, "qrtActNumber"));
				esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.Equal,"qrtContract1C.qrtContractCategoryFor1C.Id", "084958ff-522d-44b1-a9e5-2b9a5c0aa89f"));
				esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.Equal, "qrtType","808d6728-5dd7-4c06-a843-f8beec0e4ade"));
				var columnName= esqInvoice.AddColumn("qrtOrder.qrtName").Name;
				esqInvoice.AddAllSchemaColumns();
				if(oncerecord){
					esqInvoice.Filters.Add(esqInvoice.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idInvoice));
				}
				var invoiceCollection = esqInvoice.GetEntityCollection(UserConnection);
				foreach(var invoiceItem in invoiceCollection){
					guid1c = invoiceItem.GetTypedColumnValue<string>("qrtGuidAbipaK");
					sumAct = invoiceItem.GetTypedColumnValue<double>("qrtActAmount");
					typeInvoice = invoiceItem.GetTypedColumnValue<Guid>("qrtTypeId").ToString();
                    typeVAT = invoiceItem.GetTypedColumnValue<Guid>("qrtVatTypeId").ToString();
                    numberInvoice1C = invoiceItem.GetTypedColumnValue<string>("qrtNumberInvoice1C");
					order = invoiceItem.GetTypedColumnValue<string>(columnName);
					idInvoice = invoiceItem.GetTypedColumnValue<Guid>("Id");
					if(!sumAct.Equals(0) && !numberInvoice1C.Equals(string.Empty)){
						continue;
					}
					if(String.IsNullOrEmpty(guid1c)){
						continue;
					}
					json = GetJson("Document_ОтчетКомитентуОПродажах",$"?$format=json&$filter=startswith(Комментарий,'{order}_') eq true");
					
					if(!json.Contains("Комментарий")){
						json = GetJson("Document_ОтчетКомитентуОПродажах",$"?$format=json&$filter=like(Комментарий,'{order}')");
						if(!json.Contains(order)){
							continue;
						}
					}
					AddRecordJournal(json,200);
					var kommitent = Kommitent.FromJson(json);
					var collect = kommitent.Value;
					double sumVat = 0;
					double sumPremial = 0;
					double sumDoc = 0;
					double course = 0;


					string actNumber1C = string.Empty;
					string actNumber = string.Empty;
					string currencyDocKey = string.Empty;
					string numberSF = string.Empty;
					string descriptionNumberSf = string.Empty;

					DateTime dateAct = new DateTime();
					DateTime dateSF = new DateTime();

					foreach (var item in collect)
					{
						actNumber1C += item.Number + ",";
						actNumber += DescriptionCode(item.Number) + ",";
						sumDoc += item.SumDoc;
						sumPremial += item.SumPremial;
						sumVat += item.SumDoc + item.SumPremial;
						course = item.Course;
						currencyDocKey = item.CurrencyDocKey;
						refActs = item.RefKey;
						dateAct = item.Date;
						json = GetJson("Document_СчетФактураВыданный", $"?$format=json&$filter=ДокументОснование eq cast(guid'{refActs}', 'Document_ОтчетКомитентуОПродажах')" );
						if(!json.Contains(refActs)){
							continue;
						}
	
						var invoicecesIssued = InvoicecesIssued.FromJson(json);
						var collectionInvoiceIssued = invoicecesIssued.Value;
						foreach(var itemInvoiceIssued in collectionInvoiceIssued){
							numberSF += itemInvoiceIssued.Number + ",";
							descriptionNumberSf += itemInvoiceIssued.DescriptionNumber + ",";
							dateSF = itemInvoiceIssued.DateInvoice;
						}
					}
					
					
					outData += $"Курс {course}" + "\n";
					invoiceItem.SetColumnValue("qrtActAmountCalculationRate", course);
					invoiceItem.SetColumnValue("qrtCalculateCurrency1C", Math.Round(1/course,4));

					outData += $"Сумма с НДС {sumVat}" + "\n";
					invoiceItem.SetColumnValue("qrtActAmountVAT", sumVat);
					invoiceItem.SetColumnValue("qrtActAmount", sumVat);
					outData += $"Сумма с вознагрождения {sumPremial}" + "\n";
					invoiceItem.SetColumnValue("qrtSumPremial",sumPremial);
					outData += $"Сумма с отчета коммитенту(qrtSumReportKommitent) { sumDoc}" + "\n";
					invoiceItem.SetColumnValue("qrtSumReportKommitent", sumDoc);
					var currencyForCrm = DeterminateExchangeRate(currencyDocKey);
					if(!currencyForCrm.Equals(default)){
						outData += $"Валюта {currencyForCrm}" + "\n";
						invoiceItem.SetColumnValue("qrtActCurrencyId",currencyForCrm);
					}
					outData += $"Номер акта {actNumber}" + "\n";
					invoiceItem.SetColumnValue("qrtActNumber", actNumber.TrimEnd(new char[] {',',' '}));
					invoiceItem.SetColumnValue("qrtNumberAct1C", actNumber1C.TrimEnd(new char[] {',',' '}));
					outData += $"Дата акта {dateAct}" + "\n";
					invoiceItem.SetColumnValue("qrtActDate", dateAct);
					outData += $"Сумма в базовой валюте  НДС {sumVat}\n";
					invoiceItem.SetColumnValue("qrtPrimaryActAmount", sumVat);
					outData += $"Сумма в базовой валюте без НДС {sumVat}\n";
					invoiceItem.SetColumnValue("qrtPrimaryActAmountVAT",sumVat );
					
					outData += $"Номер СФ 1С {numberSF}" + "\n";
					invoiceItem.SetColumnValue("qrtNumberInvoice1C", numberSF.TrimEnd(new char[] {',',' '}));
					invoiceItem.SetColumnValue("qrtInvoiceNumber", descriptionNumberSf.TrimEnd(new char[] {',',' '}));
					outData += $"Номер СФ 1С {dateSF}" + "\n";
					invoiceItem.SetColumnValue("qrtInvoiceDate", dateSF);
					invoiceItem.Save(false);
					AddRecordJournal(
										"message1C",
										200,
										Link1C,
										"dataStructure",
										"",
										outData,
										idInvoice,
										default,
										default,
										default,
										guidKommitent);
				}
		}
        public void GetArticleRub(Guid invoice, Services[] services,Service1C.RecordSet record){
			var esqService = new EntitySchemaQuery(UserConnection.EntitySchemaManager,"qrtServicesInTheAccountForTheAccountingSystem");
			esqService.Filters.LogicalOperation = LogicalOperationStrict.And;
			esqService.Filters.Add(esqService.CreateFilterWithParameters(FilterComparisonType.Equal, "qrtInvoice",invoice));
			esqService.AddAllSchemaColumns();
			var servicesCollection = esqService.GetEntityCollection(UserConnection);
			foreach(var item in servicesCollection){
				
				var line = item.GetTypedColumnValue<string>("qrtName");
				if(line[0] == '0'){
					line = line.Replace("0","");
				}
            
				if(line != record.LineNumber){
					continue;
				}
			
				var priceExVAT = item.GetTypedColumnValue<double>("qrtTotalPrice");
				var sum = item.GetTypedColumnValue<double>("qrtPriceVAT");
		
				foreach(var service in services){
					if(record.LineNumber == service.LineNumber){
					
						var countService = service.Count;
						var total = record.Total;
						
						item.SetColumnValue("qrtPriceRealization", total/countService);
						item.SetColumnValue("qrtSumRealization",total);
						item.Save(false);
						break;
					}
				}
				break;
			}
		}
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void PostDatePayDocInInformationRegister(string doc, string organization, string datePay, string typeDoc){
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			string register = "InformationRegister_СрокиОплатыДокументов";
			string DATA = 	"{\"Организация_Key\": \"" + organization + "\"," + 
							"\"Документ\": \"" + doc + "\"," +
							"\"Документ_Type\":" + "\"StandardODATA." + typeDoc + "\"," +
							"\"СрокОплаты\":\"" + datePay + "\"}";
	//		AddRecordJournal(DATA,200);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + register);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = "POST";
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
			
			 try
		    {
		       WebResponse webResponse = request.GetResponse();
		       Stream webStream = webResponse.GetResponseStream();
		       StreamReader responseReader = new StreamReader(webStream);
		       responseReader.Close();
			
			   HttpWebResponse we = (HttpWebResponse)request.GetResponse();
			   string message = "BPM -> 1C: " + "Дата записана(" + (int)we.StatusCode + ")";
			   AddRecordJournal(message,(int)we.StatusCode);
				//
		    }
		    catch (WebException e)
		    {
				//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Дата не записана (" + (int)we.StatusCode + ") " + " " + new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
		//		AddRecordJournal(message,(int)we.StatusCode);
				AddRecordJournal(message,(int)we.StatusCode);
				//
		    }

			
		}
		public string ParseValuteIn1C(string response)
        {
           	int idxCourse = response.IndexOf("Курс\":") + ("Курс\": ").Length;
            if(idxCourse == ("Курс\": ").Length - 1){
            	return "1";
            }
            AddRecordJournal(response,200);
			string courseString = response.Substring(idxCourse);
			int idxCourseEnd = courseString.IndexOf(",");
            string course = response.Substring(idxCourse, idxCourseEnd);
            return course.Replace(",",".");
        }
        public Guid DeterminateExchangeRate(string refCurrency){
        	if(String.IsNullOrEmpty(refCurrency)){
        		return default;
        	}
        	string json = GetJson($"Catalog_Валюты(guid'{refCurrency}')","?$format=json");
        	if(json.Contains("RUB")){
        		return new Guid("5fb76920-53e6-df11-971b-001d60e938c6");
        	}
        	if(json.Contains("EUR")){
        		return new Guid("c0057119-53e6-df11-971b-001d60e938c6");
        	}
        	if(json.Contains("USD")){
        		return new Guid("915e8a55-98d6-df11-9b2a-001d60e938c6");
        	}
        	return default;
        }
		[OperationContract]
		[WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetJson(string catalog, string parametrs){
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			try
		    {
		    	//AddRecordJournal(path + catalog + parametrs,200);
				Link1C += path + catalog + parametrs;
				var request = (HttpWebRequest)WebRequest.Create(path + catalog + parametrs);
				request.Credentials = CredentialCache.DefaultCredentials;
		        request.Credentials = new NetworkCredential(login, password);
		        WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				return responseReader.ReadToEnd();
		    }
		    catch (WebException e)
		    {
				//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Контрагент не получен (" + (int)we.StatusCode + ")";
				//AddRecordJournal(message,(int)we.StatusCode);
				//
		    	return "PostAddress " + e.Message;
		    }
		}
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string UpdateAddressIn1CofDadata(string jsonDadata, string guid1cAbipa,string description){
			
			if(string.IsNullOrEmpty(guid1cAbipa)){
				return "-1";
			}
			//jsonDadata = jsonDadata.Replace("null", "\"\"");
			AddRecordJournal(jsonDadata,200);
			string login =  (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cLogin");
			string password = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrt1cPassword");
			string path = (string)Terrasoft.Core.Configuration.SysSettings.GetDefValue(UserConnection, "qrtPath1C");
			
			string parametrs = "(guid'" + guid1cAbipa + "')?$format=json";
			//Получаем данные по контрагенту
			string response = GetJson("Catalog_Контрагенты", parametrs);
			//AddRecordJournal(response,200);
			string DATA = "";
			DATA =  "{" + "\"КонтактнаяИнформация\": " + "[" + GenerateAddressDataFor1C(jsonDadata, description, 1,"b910ecef-9cc8-44e8-b0df-bc705acb83ed","РОССИЯ") + "]" + "}";

			AddRecordJournal(DATA,200);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path + "Catalog_Контрагенты" + parametrs);
		    request.Credentials = CredentialCache.DefaultCredentials;
		    request.Method = "PATCH";
		    request.ContentType = "application/json; charset=UTF-8";
		    request.Accept = "application/json";
		    request.Credentials = new NetworkCredential(login, password);
		    StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
		    requestWriter.Write(DATA);
		    requestWriter.Close();
		    try
		    {
		      WebResponse webResponse = request.GetResponse();
		      Stream webStream = webResponse.GetResponseStream();
		      StreamReader responseReader = new StreamReader(webStream);
		      responseReader.Close();
		      HttpWebResponse we = (HttpWebResponse)request.GetResponse();
			  string message = "BPM -> 1C: " + "Адрес обновлен (" + (int)we.StatusCode + ")";
			  AddRecordJournal(message,(int)we.StatusCode);
			  return "PostAddress" + message;
		    }
		    catch (WebException e)
		    {
				//
				var we = e.Response as HttpWebResponse;
				string message = "BPM -> 1C: " + "Адрес не отправлен (" + (int)we.StatusCode + ") " + new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
				AddRecordJournal(message,(int)we.StatusCode);
				//
		    	return "PostAddress " + e.Message;
		    }
			return "-1";
		}
		public string ReplaceNullOnStringForJson(string json){
			return json.Replace("null","\"\"");
		}
			public string GenerateAddressDataFor1C(string json, string descriptionCRM, int number, string typeAddress, string countryOther){
			//Десериализация json для данных dadata
				string DATA = "";
				string structureAddress = "";
                string valueStructure = "";
				json = ReplaceNullOnStringForJson(json);
				if(!json.Equals(string.Empty)){
					var addressDadata = AddressDadata.FromJson(json);
                    valueStructure = GenerateAccountAddressJson(addressDadata);
					structureAddress = GenerateContractAddressXml(addressDadata);
					DATA ="{" + $"\"LineNumber\": \"{number}\"," + 
				 			  	  $"\"Тип\": \"Адрес\"," +
				 				  $"\"Вид_Key\":\"{typeAddress}\","+
				 				  $"\"Страна\":\"{addressDadata.Data.Country}\"," + 
								  $"\"Представление\":\"{addressDadata.Value}\","  + 
								  $"\"ЗначенияПолей\": \"{structureAddress}\","+
								  $"\"Значение\":\"{valueStructure}\"" + "},";
				}
				else{
                    AddRecordJournal("International address", 200);
					var address1CValue = new Address1C();
					address1CValue.Value = descriptionCRM;
		            address1CValue.Type = "Адрес";
		            address1CValue.AddressType = "ВСвободнойФорме";
		            address1CValue.Country = countryOther;
		            string value = AddressFor1c.Serialize.ToJson(address1CValue).Replace("\"", "\\\"");
		            
					DATA = "{" + $"\"LineNumber\": \"{number}\"," + 
			 					$"\"Тип\": \"Адрес\"," +
			 					$"\"Вид_Key\":\"{typeAddress}\","+
			 					$"\"Страна\":\"{countryOther}\","+
		 						$"\"Представление\":\"{descriptionCRM}\","  + 
		 						$"\"Значение\":\"{value}\"" + "},";
		 			
				}
				return DATA;
			}
			
			
			public string ChoiceFlatTypeFull(string flatTypeFull){
				switch(flatTypeFull){
					case "помещение":
						return "2020";
					case "комната":
						return "2050";
					case "офис":
						return "2030";
					default:
						return "";
				}
			}
			
			public string ChoiceBlockTypeFull(string blockTypeFull){
				switch(blockTypeFull){
					case "корпус":
						return "1050";
					case "строение":
						return "1060 ";
					default:
						return "";
				}
			}
			
			public string ChoiceTypeNumberHome(string typeHouse){
				switch(typeHouse){
					case "дом":
						return "1010";
					case "владение":
						return "1020";
						break;
					case "домовладение":
						return "1030";
					case "гараж":
						return "1040";
					case "здание":
						return "1050";
					case "шахта":
						return "1060";
					case "pем. участок":
						return "1070";
					default:
						return "";
				}
		}

        public  string GenerateAccountAddressJson(AddressDadata addressData)
        {

            var address1CValue = new Address1C();
            //Парс объекта
            string Description = addressData.Value;
            string Country = addressData.Data.Country;

            string Area = addressData.Data.Region;
            string AreaType = addressData.Data.RegionType;

            string City = addressData.Data.City;
            string CityType = addressData.Data.CityType;

            string Street = addressData.Data.Street;
            string StreetType = addressData.Data.StreetType;
            string Index = addressData.Data.PostalCode;

            string Flat = addressData.Data.Flat;
            string FlatTypeFull = addressData.Data.FlatTypeFull;

            string Block = addressData.Data.Block;
            string BlockTypeFull = addressData.Data.BlockTypeFull;

            string HouseTypeFull = addressData.Data.HouseTypeFull;
            string NumberHouse = addressData.Data.House;

            List<ApartmentOrBuilding> apartmentList = new List<ApartmentOrBuilding>();
            List<ApartmentOrBuilding> buidList = new List<ApartmentOrBuilding>();

            address1CValue.Type = "Адрес";
            address1CValue.AddressType = "Административно-территориальный";
            //Дом владение здание корпус строение литера литер сооружение участок
            if (HouseTypeFull.Contains("дом") || HouseTypeFull.Contains("владение") ||
                HouseTypeFull.Contains("Сооружение"))
            {
                HouseTypeFull = HouseTypeFull.ToUpper()[0] + HouseTypeFull.Substring(1);
                address1CValue.HouseType = HouseTypeFull;
                address1CValue.HouseNumber = NumberHouse;
            }else if (HouseTypeFull.Contains("корпус") || HouseTypeFull.Contains("строение")
                                                       || HouseTypeFull.Contains("литера") 
                                                       || HouseTypeFull.Contains("литер") 
                                                       || HouseTypeFull.Contains("Сооружение") 
                                                       || HouseTypeFull.Contains("участок"))
            {

                
                HouseTypeFull = HouseTypeFull.ToUpper()[0] + HouseTypeFull.Substring(1);
                buidList.Add(new ApartmentOrBuilding() { Number = NumberHouse, Type = HouseTypeFull });
                address1CValue.Buildings = buidList;
            }

            if (FlatTypeFull.Contains("помещение")
                || FlatTypeFull.Contains("помещение")
                || FlatTypeFull.Contains("комната")
                || FlatTypeFull.Contains("офис")
                || FlatTypeFull.Contains("квартира"))
            {
                FlatTypeFull = FlatTypeFull.ToUpper()[0] + FlatTypeFull.Substring(1);
                string[] variationIncorrectNameBuildings = {"оф ", "ком ", "комн ", "помещ ", "кв "};
                string[] variationCorrectNameBuildings = {"офис", "комната", "помещение", "кваритира"};
                string[] correctValueFlat = CorrectJsonAddressPropertyBuilding(FlatTypeFull, Flat,
                    variationIncorrectNameBuildings, variationCorrectNameBuildings);
                if (correctValueFlat.Length.Equals(3))
                {

                    
                    apartmentList.Add(new ApartmentOrBuilding() {Number = correctValueFlat[0], Type = FlatTypeFull });
                    string subFlatTypeFull = correctValueFlat[1].ToUpper()[0] + correctValueFlat[1].Substring(1);
                    apartmentList.Add(new ApartmentOrBuilding() {Number = correctValueFlat[2], Type = correctValueFlat[1]});
                    address1CValue.Apartments = apartmentList;


                }
                else
                {
                    apartmentList.Add(new ApartmentOrBuilding() { Number = Flat, Type = FlatTypeFull });
                    address1CValue.Apartments = apartmentList;
                }
            }

            if (BlockTypeFull.Contains("корпус") || BlockTypeFull.Contains("корпус")
                                                 || BlockTypeFull.Contains("строение")
                                                 || BlockTypeFull.Contains("литер"))
            {
                apartmentList.Add(new ApartmentOrBuilding() { Number = Block, Type = BlockTypeFull });
                address1CValue.Apartments = apartmentList;
            }
            //Регион
            address1CValue.Area = Area;
            address1CValue.AreaType = AreaType;
            //Представление
            address1CValue.Value = Description;
            address1CValue.Country = Country;
            address1CValue.City = City;
            address1CValue.CityType = CityType;
            address1CValue.Street = Street;
            address1CValue.StreetType = StreetType;
            address1CValue.ZiPcode = Index;

            return AddressFor1c.Serialize.ToJson(address1CValue).Replace("\"", "\\\"");
            
        }		
	public string GenerateContractAddressXml(AddressDadata addressData)
        {
            //Парс объекта
            string Description = addressData.Value;
            string Country = addressData.Data.Country;
            string SubjectRF = addressData.Data.RegionWithType;
            string City = addressData.Data.City;
            string Street = addressData.Data.Street;
            string Index = addressData.Data.PostalCode;

            string HouseTypeFull = addressData.Data.HouseTypeFull;
            string NumberHouse = addressData.Data.House;

            string FlatType = addressData.Data.FlatTypeFull;
            string Flat = addressData.Data.Flat;
           

            string BlockType = addressData.Data.BlockTypeFull;
            string Block = addressData.Data.Block;

            string SettlementType = addressData.Data.SettlementTypeFull;
            string Settlement = addressData.Data.Settlement;
            //создание главного объекта документа
            XmlDocument XmlDoc = new XmlDocument();
            /*<database name="abc"></database>*/
            //создание корневого элемента 
            XmlElement ContactInfo = XmlDoc.CreateElement("КонтактнаяИнформация");
            //создание атрибута
            ContactInfo.SetAttribute("xmlns", "http://www.v8.1c.ru/ssl/contactinfo");
            ContactInfo.SetAttribute("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
            ContactInfo.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            ContactInfo.SetAttribute("Представление", Description);
            //Добавление в документ
            XmlDoc.AppendChild(ContactInfo);

            XmlElement compressionRootElement = XmlDoc.CreateElement("Состав");
            compressionRootElement.SetAttribute("xsi:type", "Адрес");
            compressionRootElement.SetAttribute("Страна", Country);
            ContactInfo.AppendChild(compressionRootElement);

            XmlElement compressionChElement = XmlDoc.CreateElement("Состав");
            compressionChElement.SetAttribute("xsi:type", "АдресРФ");
            compressionRootElement.AppendChild(compressionChElement);


            
            if (!string.IsNullOrEmpty(SubjectRF))
            {
                XmlElement subjectRF = XmlDoc.CreateElement("СубъектРФ");
                subjectRF.InnerText = SubjectRF;
                compressionChElement.AppendChild(subjectRF);
            }


            
            if (!string.IsNullOrEmpty(City))
            {
                XmlElement city = XmlDoc.CreateElement("Город");
                city.InnerText = City;
                compressionChElement.AppendChild(city);
            }


            
            if (!string.IsNullOrEmpty(Street))
            {
                XmlElement street = XmlDoc.CreateElement("Улица");
                street.InnerText = Street;
                compressionChElement.AppendChild(street);
            }

            XmlElement subAdrAlOdinary = XmlDoc.CreateElement("ДопАдрЭл");
            subAdrAlOdinary.SetAttribute("ТипАдрЭл", "10100000");
            compressionChElement.AppendChild(subAdrAlOdinary);

            XmlElement subAdrAl = XmlDoc.CreateElement("ДопАдрЭл");
            subAdrAl.SetAttribute("ТипАдрЭл", "10100000");
            subAdrAl.SetAttribute("Значение", Index);
            compressionChElement.AppendChild(subAdrAl);

            if (!string.IsNullOrEmpty(HouseTypeFull))
            {
                string numberType = ChoiceTypeNumberHome(HouseTypeFull);
                XmlElement number = XmlDoc.CreateElement("Номер");
                number.SetAttribute("Тип", numberType);
                number.SetAttribute("Значение", NumberHouse);
                compressionChElement.AppendChild(number);
            }

            if (!string.IsNullOrEmpty(FlatType))
            {
                string[] variationIncorrectNameBuildings =  {"оф ", "ком ", "комн ","помещ ",  "кв " };
                string[] variationCorrectNameBuildings = {"офис", "комната", "помещение", "кваритира"};
                string[] correctValueFlat = CorrectJsonAddressPropertyBuilding(FlatType, Flat,
                    variationIncorrectNameBuildings, variationCorrectNameBuildings);
                if (correctValueFlat.Length.Equals(3))
                {
                    var numberType = ChoiceFlatTypeFull(FlatType);
                    var numberFlat = correctValueFlat[0];
                    XmlElement number = XmlDoc.CreateElement("Номер");
                    number.SetAttribute("Тип", numberType);
                    number.SetAttribute("Значение", numberFlat);
                    compressionChElement.AppendChild(number);


                    var numberSubType = ChoiceFlatTypeFull(correctValueFlat[1]);
                    var numberSubFlat = correctValueFlat[2];
                    XmlElement numberSub = XmlDoc.CreateElement("Номер");
                    number.SetAttribute("Тип", numberSubType);
                    number.SetAttribute("Значение", numberSubFlat);
                    compressionChElement.AppendChild(numberSub);
                }
                else
                {
                    string numberType = ChoiceFlatTypeFull(FlatType);
                    XmlElement number = XmlDoc.CreateElement("Номер");
                    number.SetAttribute("Тип", numberType);
                    number.SetAttribute("Значение", Flat);
                    compressionChElement.AppendChild(number);
                }
                
            }


            if (!string.IsNullOrEmpty(BlockType))
            {
                string[] variationIncorrectNameBuildings = { "оф ", "ком ", "комн ", "помещ ", "кв " };
                string[] variationCorrectNameBuildings = { "офис", "комната", "помещение", "кваритира" };
                string[] correctValueBlock = CorrectJsonAddressPropertyBuilding(FlatType, Flat,
                    variationIncorrectNameBuildings, variationCorrectNameBuildings);
                if (correctValueBlock.Length.Equals(3))
                {
                    string numberType = ChoiceBlockTypeFull(BlockType);
                    string numberBlock = correctValueBlock[0];
                    XmlElement number = XmlDoc.CreateElement("Номер");
                    number.SetAttribute("Тип", numberType);
                    number.SetAttribute("Значение", numberBlock);
                    compressionChElement.AppendChild(number);


                    var numberSubType = ChoiceFlatTypeFull(correctValueBlock[1]);
                    var numberSubBlock = correctValueBlock[2];
                    XmlElement numberSub = XmlDoc.CreateElement("Номер");
                    number.SetAttribute("Тип", numberSubType);
                    number.SetAttribute("Значение", numberSubBlock);
                    compressionChElement.AppendChild(numberSub);
                }
                else
                {
                    string numberType = ChoiceBlockTypeFull(BlockType);
                    XmlElement number = XmlDoc.CreateElement("Номер");
                    number.SetAttribute("Тип", numberType);
                    number.SetAttribute("Значение", Block);
                    compressionChElement.AppendChild(number);
                }
               
            }

            if (!string.IsNullOrEmpty(SettlementType))
            {
                string numberType = ChoiceTypeNumberHome(HouseTypeFull);
                XmlElement number = XmlDoc.CreateElement("Номер");
                number.SetAttribute("Тип", numberType);
                number.SetAttribute("Значение", NumberHouse);
                compressionChElement.AppendChild(number);
            }


            return XmlDoc.OuterXml.Replace("\"", "\\\"");
	}
     public string[] CorrectJsonAddressPropertyBuilding(string typeBuilding, string numberBuilding, string[] variationsBuildingsNames, string[] correctBuildingNames)
        {
            var resultBuildings = numberBuilding.Split(' ');
            foreach (var buildingName in variationsBuildingsNames)
            {
                if (!resultBuildings.Length.Equals(3))
                {
                    break;
                }
                    if (resultBuildings[1] == buildingName.Replace(" ", ""))
                    {
                        Console.WriteLine(1);
                        foreach (var correctBuildingName in correctBuildingNames)
                        {
                            if (correctBuildingName.Contains(buildingName.Replace(" ", "")))
                            {
                                resultBuildings[1] = correctBuildingName;
                            }
                        }
                        
                    }
            }

            return resultBuildings;
        }
    }
}