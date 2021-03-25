define("qrtRequest1Section", ["ProcessModuleUtilities"], function(ProcessModuleUtilities) {
	return {
		entitySchemaName: "qrtRequest",
		attributes: {
			"canActiveButton": {
				"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"dataValueType": this.Terrasoft.DataValueType.BOOLEAN,
				"value": true,
			},
			"Collection": {
				"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"dataValueType": this.Terrasoft.DataValueType.COLLECTION,
				"value": null
			},
			"IsInsurance": {
				"dataValueType": Terrasoft.BOOLEAN,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": false
			},
			"qrtStatusRecord": {
				"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"dataValueType": this.Terrasoft.DataValueType.TEXT,
				"value": null,
			},
			"qrtOwnerRecord": {
				"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"dataValueType": this.Terrasoft.DataValueType.TEXT,
				"value": null,
			},
			"isFile": {
				"dataValueType": Terrasoft.TEXT,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": null
			},
			"CargoValue": {
				"dataValueType": Terrasoft.TEXT,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": null
			},
			
			"PlacesCount": {
				"dataValueType": Terrasoft.TEXT,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": null
			},
			"Volume": {
				"dataValueType": Terrasoft.TEXT,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": null
			},
			"GrossWeight": {
				"dataValueType": Terrasoft.TEXT,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": null
			},
		},
		messages: {
			"updateActionElem": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.SUBSCRIBE
			},
			"PublishDetailRequestCalculation": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.SUBSCRIBE
			},
		},
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"parentName": "CombinedModeActionButtonsCardLeftContainer",
				"propertyName": "items",
				"name": "actions2",
				"values": {
					"itemType": Terrasoft.ViewItemType.BUTTON,
					//"caption": {"bindTo": "Resources.Strings.ActionButtonCaption"},
					"caption": "Отправить запрос на расчет",
					"classes": {
						"textClass": ["actions-button-margin-right"],
						"wrapperClass": ["actions-button-margin-right"]
					},
					"menu": {
						"items": {"bindTo": "ActionsButtonMenuItems2"}
					},
					"visible": true
				}
			},
		]/**SCHEMA_DIFF*/,
		methods: {
			getActions: function() {
				var actionMenuItems = this.callParent(arguments);
				actionMenuItems.addItem(this.getButtonMenuItem({ 
					Type: "Terrasoft.MenuSeparator",
					Caption: ""
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Caption": "Отправка КП",
					"Tag": "sendKp",
					"Enabled": {"bindTo": "canActiveButton"}
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Caption": { "bindTo": "Resources.Strings.CreateOrderButtonCaption" },
					"Click": { "bindTo": "onButtonClick" },
					"Enabled": {"bindTo": "canActiveButton"}
				}));
				return actionMenuItems;
			},
			canActiveButtonStatus: function(recordId, self){
				if(recordId){
					var esq = self.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "qrtRequest"
						});
					esq.addColumn("qrtStage");

					esq.getEntity(recordId, function(result) {
						if (!result.success) {
							// обработка/логирование ошибки, например
							self.showInformationDialog("Ошибка запроса данных");
							return;
						}
						var status = result.entity.get("qrtStage");
						if (status){
							//Создана Заявка
							//Заявка выполнена
							//Отклонен
							//Отложен
							
							if (status.value !== "0fd00a33-4ee3-412c-8294-03d5159b5e6b" &&
								status.value !== "fc973230-2d8f-45c5-86e8-d84d3cc61ad9" &&
								status.value !== "3b5344da-dc30-49b4-a69d-2665704a9e05" &&
								status.value !== "42b71095-936e-4c8c-b4fe-ff93f7b16d4e"){
								self.set("canActiveButton", true);
							} else {
								self.set("canActiveButton", false);
							}
						}
					}, this);
				}
				
			},
			updateRequestContractDetailFilter: function(qrtService, qrtCustomer, qrtPayer){
				debugger;
				
				var customerId = qrtCustomer;
				var serviceList = qrtService;
				var player = qrtPayer;
				
				var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
						rootSchemaName: "qrtRequestContract"
					});
				
				esq.addColumn("qrtContract.qrtType", "Type");
				esq.addColumn("qrtContract", "Contract");
				if (customerId){
					if (serviceList.indexOf("VED") !== -1 || 
						serviceList.indexOf("FIN") !== -1 || 
						serviceList.indexOf("WHS") !== -1) {
							var esqFilter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
								"qrtRequest", this.get("ActiveRow"));
							esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
							esq.filters.add("esqFilter", esqFilter);
						
					} else if (serviceList === "CCL"){
						//Заказчик
						var esqFilter1 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
							"qrtContract.qrtCustomer", customerId.value);
						//Исполнитель = Абипа Кастомс (DEV/PROD)
						var esqFilter2 = esq.createColumnInFilterWithParameters(
							"qrtContract.qrtSupplier", ["b6d3752b-2903-4634-9998-e51992c63d62", "ef62ce35-9d5e-4270-891a-7925da40c95d"]);
						//Согласован,Подписан
						var esqFilter3 = esq.createColumnInFilterWithParameters(
							"qrtContract.qrtState", ["5ed15f38-4eb6-46f0-9137-8459468b334c", "6173800d-8966-487e-aeb7-d68b278c58f8"]);
						
						esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
						esq.filters.add("esqFilter1", esqFilter1);
						esq.filters.add("esqFilter2", esqFilter2);
						esq.filters.add("esqFilter3", esqFilter3);
						
					} else if (serviceList.indexOf("CCL") === -1 && (serviceList.indexOf("AIR") !== -1 || 
					 			serviceList.indexOf("FTL") !== -1 || serviceList.indexOf("LTL") !== -1 || 
					 			serviceList.indexOf("BIG") !== -1 || serviceList.indexOf("CHA") !== -1 || 
					 			serviceList.indexOf("FCL") !== -1 || serviceList.indexOf("LCL") !== -1 || 
					 			serviceList.indexOf("LCS") !== -1 || serviceList.indexOf("RLR") !== -1)){
						//Исполнитель = Абипа (DEV/PROD)
						var esqFilter4 = esq.createColumnInFilterWithParameters(
							"qrtContract.qrtSupplier", ["e59055ed-4501-4d41-b370-562cbdbd270d", "e308b781-3c5b-4ecb-89ef-5c1ed4da488e"]);
						//Согласован,Подписан
						var esqFilter5 = esq.createColumnInFilterWithParameters(
							"qrtContract.qrtState", ["5ed15f38-4eb6-46f0-9137-8459468b334c", "6173800d-8966-487e-aeb7-d68b278c58f8"]);
						//Плательщик
						if (player){
							var esqFilter6 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
								"qrtContract.qrtCustomer", player.value);
							esq.filters.add("esqFilter6", esqFilter6);
						}
						
						esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
						esq.filters.add("esqFilter4", esqFilter4);
						esq.filters.add("esqFilter5", esqFilter5);
					} else {
						//Исполнитель = Абипа Кастомс (DEV/PROD)/Абипа (DEV/PROD)
						
						var esqFilter7 = esq.createColumnInFilterWithParameters(
							"qrtContract.qrtSupplier", ["b6d3752b-2903-4634-9998-e51992c63d62", "ef62ce35-9d5e-4270-891a-7925da40c95d",
											"e59055ed-4501-4d41-b370-562cbdbd270d", "e308b781-3c5b-4ecb-89ef-5c1ed4da488e"]);
											
						//Согласован,Подписан
						var esqFilter8 = esq.createColumnInFilterWithParameters(
							"qrtContract.qrtState", ["5ed15f38-4eb6-46f0-9137-8459468b334c", "6173800d-8966-487e-aeb7-d68b278c58f8"]);
						
						//Заказчик
						var esqFilter9 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
							"qrtContract.qrtCustomer", customerId.value);
						
						esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
						esq.filters.add("esqFilter7", esqFilter7);
						esq.filters.add("esqFilter8", esqFilter8);
						esq.filters.add("esqFilter9", esqFilter9);
					}
				}
				
				var esqFilter10 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
						"qrtRequest", this.get("ActiveRow"));
				esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
				esq.filters.add("esqFilter10", esqFilter10);
				
				esq.getEntityCollection(function (result) {
					if (!result.success) {
						this.showInformationDialog("Ошибка запроса данных");
						return;
					}
					if (result.collection.collection.length > 0){
							var existsContactsCollection = [];
							var self = this;
							result.collection.each(function (item, index, array) {
								var selfId = item.get("Id");
								var type = item.get("Type");
								//Разовый "21453f48-1adc-4171-a160-d7a152f9cb76"
								if (type && type.value === "21453f48-1adc-4171-a160-d7a152f9cb76"){
									var contact = item.get("Contract");
									//----------------------------------------------
									var esq2 = self.Ext.create("Terrasoft.EntitySchemaQuery", {
										rootSchemaName: "qrtRequestContract"
									});
									
									var esq2Filter = esq2.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
										"qrtContract", contact.value);
									var esq2Filter2 = esq.createColumnIsNotNullFilter("qrtOrder");
									esq2.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
									esq2.filters.add("esq2Filter", esq2Filter);
									esq2.filters.add("esq2Filter2", esq2Filter2);
									
									esq2.getEntityCollection(function (result2) {
										if (!result2.success) {
											self.showInformationDialog("Ошибка запроса данных");
											return;
										}
										if (result2.collection.collection.length === 0){
											existsContactsCollection.push(selfId);
										}
									}, self);
									//----------------------------------------------
								} else {
									existsContactsCollection.push(selfId);
								}
							});
							this.set("Collection", existsContactsCollection);

						}
				}, this);
			},
			onButtonClick: function() {
				var controlConfig = {};
				Terrasoft.utils.inputBox("Проверьте, что все расчеты, подлежащие переносу в Заявки, имеют статус \"Утвержден\"",
					function(buttonCode, controlData) {
						if (buttonCode === "yes") {
							//Что-то делаем
							this.createRequestButton();
						} else {
							return;
						}
					},
					[
						{
							className: "Terrasoft.Button",
							caption: "Продолжить создание Заявки",
							returnCode: "yes"
						},
						{
							className: "Terrasoft.Button",
							caption: "Отменить",
							returnCode: "no"
						}
					],
					this,
					controlConfig,
					{defaultButton: 0}
				);
				
			},
			createRequestButton: function(){
				var activeRow = this.get("ActiveRow");
				var contactId = null;
              	var requestCompany = null;
              	
              	Terrasoft.chain(
                  function (next) {
                    if (activeRow){
                      var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "qrtRequest"
						});
                      esq.addColumn("qrtService");
                      esq.addColumn("qrtOurCompany");
                      esq.addColumn("qrtContact");
                      
                      esq.getEntity(activeRow, function(result) {
                        if (!result.success) {
                          // обработка/логирование ошибки, например
                          this.showInformationDialog("Ошибка запроса данных");
                          return;
                        }
                        contactId = result.entity.get("qrtContact");
                        requestCompany = result.entity.get("qrtOurCompany");
                        next();
                      }, this);
                    }
                    
                  },
                  function (next) {
                    if (contactId){
                      var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {rootSchemaName: "ContactCommunication"});
                      esq.addColumn("Number");
                      
                      esq.filters.add("filter01", this.Terrasoft.createColumnFilterWithParameter(
                        this.Terrasoft.ComparisonType.EQUAL,"Contact", contactId.value));
                      //Тип = Email
                      esq.filters.add("filter02", this.Terrasoft.createColumnFilterWithParameter(
                        this.Terrasoft.ComparisonType.EQUAL,"CommunicationType", "ee1c85c3-cfcb-df11-9b2a-001d60e938c6"));
                      
                      esq.getEntityCollection(function (result) {
                        if (!result.success) {
                          // обработка/логирование ошибки, например
                          this.showInformationDialog("Ошибка запроса данных:\n" + "\nErrorCode: " + result.errorInfo.errorCode + "\nMessage: " + result.errorInfo.message);
                          return;
                        }
                        if (result.collection.collection.length > 0){
                          var mail = "";
                          result.collection.each(function(item) {
                            mail = item.get("Number");
                            return;
                          });
                          if (Ext.isEmpty(mail)){
                            this.showInformationDialog("Внесите Email контакта");
                            return;
                          } else {
                            this.set("qrtEmail", mail);
                          	next();
                          }
                        } else {
                          this.showInformationDialog("Заполните Email контакта");
                        }
                      }, this);
                    } else {
                      this.showInformationDialog("Заполните поле \"Контактное лицо\"");
                    }
                    
                  },
                  function (next) {
                    if (activeRow) {
                      var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
                          rootSchemaName: "qrtRequestCalculation"
                          });

                      esq.addColumn("Id");
                      esq.addColumn("qrtTypeService");
                      esq.addColumn("qrtOurCompany");//*

                      esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;

                      var esqFilter1 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
                          "qrtRequest", activeRow);
                      esq.filters.addItem(esqFilter1);
                      //Статус = Утвержден
                      var esqFilter2 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
                          "qrtStatus", "c9c87bbd-b105-4066-be61-e711f04f4e67");
                      esq.filters.addItem(esqFilter2);

                      esq.getEntityCollection(function(result) {
                          if (!result.success) {
                              this.showInformationDialog("Что то пошло не так");
                              return;
                          }
                          if (result.collection.getCount() === 0) {
                              this.showInformationDialog("Для создания Заявки требуется утвердить по одному расчету на ТО и транспорт");
                              return;
                          }
                          var listCalc = [];
                          result.collection.each(function (item, index, array) {
                              var nameService = item.get("qrtTypeService");
                              listCalc.push(item.get("Id"));
                              var company = item.get("qrtOurCompany");

                              //!CCL
                              if (nameService && nameService.value !== "33607268-f494-497d-993a-016e0670ae85"){
                                  if(company && requestCompany && company.value === requestCompany.value && listCalc.length > 1){
                                      [listCalc[0], listCalc[listCalc.length - 1]] = [listCalc[listCalc.length - 1], listCalc[0]];
                                  }
                              }
                          });

                          window.console.log(listCalc);
                          this.Terrasoft.SysSettings.querySysSettingsItem("qrtOrderLastNumber", function(value) {
                              //Значение системной настройки
                              var countSettings = value + 1;
                              var args = {
                                  sysProcessName: "qrtCreateOrderFromRequest_4352e6c",
                                  parameters: {
                                      RequestID: activeRow,
                                      ProcessCount: countSettings,
                                      ProcessListCalc: listCalc //*
                                  }
                              };

                              ProcessModuleUtilities.executeProcess(args);
                              return;
                          }, this);
                      }, this);
                  }
                  }, this);
			},
			init: function(){
				this.callParent(arguments);
				this.console.log("Init");
				this.sandbox.subscribe("updateActionElem",  function(args){
					this.console.log(args);
					this.initActionButton(args, this);
				}, this, ["qrtRequest1Section"]);
			},
			// Выполняется после отрисовки страницы Запроса.
			onCardRendered: function() {
				this.callParent();
				// Данные реестра.
				var gridData = this.get("GridData");
				var activeRow = this.get("ActiveRow");
				if (activeRow) {
					this.initActionButton(activeRow, this);
					this.canActiveButtonStatus(activeRow, this);
					
				}
				// После потери активной записи ее нужно восстановить
				// и для нее проверить наличие Заявки.
				else {
					var historyState = this.sandbox.publish("GetHistoryState");
					var hash = historyState.hash;
					if (hash && hash.valuePairs) {
						activeRow = hash.valuePairs[0].name;
						// Восстановление активной записи.
						this.set("ActiveRow", activeRow);
						// Сохранение контекста в локальную переменную.
						var self = this;
						// Подписка на событие полной загрузки данных в вертикальный реестр.
						gridData.on("dataloaded", function() {
							self.initActionButton(activeRow, self);
							self.canActiveButtonStatus(activeRow, self);
						});
					}
				}
				// Подписка на событие изменения активной записи реестра.
				gridData.on("itemchanged", function() {
					this.initActionButton(activeRow, this);
					this.canActiveButtonStatus(activeRow, this);
				}, this);
			},
			// Определяет наличие Заявки для активной записи реестра.
			isOrderExist: function(id) {
				var order = this.get("GridData").get(id).get("qrtRequest");
				return (order) ? true : false;
			},
			initActionButton: function(activeRow, context) {
				if (activeRow) {
					//window.open(window.location, "_self");
					
					var esq = context.Ext.create("Terrasoft.EntitySchemaQuery", {
						    rootSchemaName: "qrtRequest"
						});
					esq.addColumn("qrtPayer");
					esq.addColumn("qrtCustomer");
					esq.addColumn("qrtService");
					esq.addColumn("qrtOwner");
					esq.addColumn("qrtFileDimension");
					esq.addColumn("qrtVolume", "qrtVolume");
					esq.addColumn("qrtCargoValue", "qrtCargoValue");
					esq.addColumn("qrtGrossWeight", "qrtGrossWeight");
					esq.addColumn("qrtPlacesCount", "qrtPlacesCount");
					esq.addColumn("qrtIsInsurance", "qrtIsInsurance");

					esq.getEntity(activeRow, function(result) {
						if (!result.success) {
							// обработка/логирование ошибки, например
							this.showInformationDialog("Ошибка запроса данных");
							return;
						} else {
							var actionMenuItems2 = this.Ext.create("Terrasoft.BaseViewModelCollection");
							var answer = "";
							if (result.entity.get("qrtService")){
								answer = result.entity.get("qrtService").split(/, /g);
							}
							
							this.set("CargoValue", result.entity.get("qrtCargoValue"));
							this.set("PlacesCount", result.entity.get("qrtPlacesCount"));
							this.set("Volume", result.entity.get("qrtVolume"));
							this.set("GrossWeight", result.entity.get("qrtGrossWeight"));
							
							var isFile = result.entity.get("qrtFileDimension");
							this.set("isFile", isFile);
							var ownerRecord = result.entity.get("qrtOwner");
							this.set("IsInsurance", result.entity.get("qrtIsInsurance"));
							if (ownerRecord){
								this.set("qrtOwnerRecord", ownerRecord.value);
							}
							if (answer.length > 0) {
								answer.forEach(function(item){
									this.console.log(item, isFile);
									actionMenuItems2.addItem(context.getButtonMenuItem({
										"Caption": item,
										"Tag": item + "," + isFile,
										"Click": {"bindTo": "sendRequest"},
										"Enabled": {"bindTo": "canEntityBeOperated"}
									}));
								});
								
							} else {
								actionMenuItems2.addItem(context.getButtonMenuItem({
									"Caption": "Добавить",
									"Tag": null,
									"Click": {"bindTo": "sendRequest"},
									"Enabled": {"bindTo": "canEntityBeOperated"}
								}));
							}

							context.set("ActionsButtonMenuItems2", actionMenuItems2);
							context.console.log(actionMenuItems2);
							context.consol.log(context.get("ActionsButtonMenuItems2"));
							context.updateRequestContractDetailFilter(
								result.entity.get("qrtService"), 
								result.entity.get("qrtCustomer"),
								result.entity.get("qrtPayer"));
						}
					}, this);
				}
			},
			getDetailNameFromWhoOpen: function() {
				return {
					param1: this.get("ActiveRow"),
					param2: this.get("qrtStatusRecord"),
					param3: this.get("qrtOwnerRecord"),
					param4: this.get("IsInsurance"),
					param0: this.get("Collection"),
				};
			},
			sendRequest: function(args){
                var activeRow = this.get("ActiveRow");
                this.console.log(activeRow);
				if (activeRow) {
					this.initActionButton(activeRow, this);
					this.canActiveButtonStatus(activeRow, this);
                }
             
                this.console.log(args + "111");
				var recordId = this.get("ActiveRow");
				if(recordId){
					var esq2 = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "qrtRequest"
						});
					esq2.addColumn("qrtLead");
					esq2.addColumn("qrtCustomer");
					esq2.addColumn("qrtCityLoading");
					esq2.addColumn("qrtCountryLoading");
					esq2.addColumn("qrtCityUnloading");
					esq2.addColumn("qrtCountryUnloading");
					esq2.addColumn("qrtProductType");
					esq2.addColumn("qrtDateRequirement");
					esq2.addColumn("qrtProductDescription");
					esq2.addColumn("qrtCustomMode");
					esq2.addColumn("qrtNumberFEA");
					
					esq2.addColumn("qrtPlacesCount");
					esq2.addColumn("qrtPaidWeight");
					esq2.addColumn("qrtVolume");
					esq2.addColumn("qrtGrossWeight");
					esq2.addColumn("qrtNotes");
                  	
					esq2.getEntity(recordId, function(result2) {
						if (!result2.success) {
							// обработка/логирование ошибки, например
							this.showInformationDialog("Ошибка запроса данных");
							return;
						}
						
						this.set("qrtStatusRecord", null);
						var button = String(args).split(",");
						var сargoValue = this.get("CargoValue");
						var fileDimension = this.get("isFile");
		
						var placesCount = this.get("PlacesCount");
						var volume = this.get("Volume");
						var grossWeight = this.get("GrossWeight");
						
						//
						var сityLoading = result2.entity.get("qrtCityLoading");
						if (Ext.isEmpty(сityLoading)){
							this.showInformationDialog("Поле \"Город загрузки\': Необходимо указать значение");
							return;
						}
						
						var сountryLoading = result2.entity.get("qrtCountryLoading");
						if (!сountryLoading){
							this.showInformationDialog("Поле \"Страна загрузки\': Необходимо указать значение");
							return;
						}
						
						var сityUnloading = result2.entity.get("qrtCityUnloading");
						if (!сityUnloading){
							this.showInformationDialog("Поле \"Город выгрузки\': Необходимо указать значение");
							return;
						}
						
						var сountryUnloading = result2.entity.get("qrtCountryUnloading");
						if (!сountryUnloading){
							this.showInformationDialog("Поле \"Страна выгрузки\': Необходимо указать значение");
							return;
						}
						
						var сroductType = result2.entity.get("qrtProductType");
						if (!сroductType){
							this.showInformationDialog("Поле \"Направление перевозки\': Необходимо указать значение");
							return;
						}
						
						var dateRequirement = result2.entity.get("qrtDateRequirement");
						if (!dateRequirement){
							this.showInformationDialog("Поле \"Требования к датам\': Необходимо указать значение");
							return;
						}
						
						var productDescription = result2.entity.get("qrtProductDescription");
						if (Ext.isEmpty(productDescription)){
							this.showInformationDialog("Поле \"Описание груза\': Необходимо указать значение");
							return;
						}
						
						if (Ext.isEmpty(сargoValue) || сargoValue === 0){
							this.showInformationDialog("Поле \"Стоимость груза\': Необходимо указать значение");
							return;
						}
						
						if (button[0] === "CCL") {
							
							var numberFEA = result2.entity.get("qrtNumberFEA");
							if (Ext.isEmpty(numberFEA)){
								this.showInformationDialog("Поле \"Кол-во ТН ВЭД\': Необходимо указать значение");
								return;
							}
							
							var customMode = result2.entity.get("qrtCustomMode");
							if (!customMode){
								this.showInformationDialog("Поле \"Таможенный режим\': Необходимо указать значение");
								return;
							}
						}
						//
						
						if (fileDimension && ((Ext.isEmpty(placesCount) || placesCount === 0) || 
						(Ext.isEmpty(volume) || volume === 0) || (Ext.isEmpty(grossWeight) || grossWeight === 0))){
							this.showInformationDialog("Укажите итоговые параметры по габаритам груза");
							return;
						}
						
						var defaultValues = [];
						var placesCount2 = result2.entity.get("qrtPlacesCount");
						if (placesCount2){
							defaultValues.push({
								name: "qrtPlacesCount",
								value: placesCount2
							});
						}
						var paidWeight2 = result2.entity.get("qrtPaidWeight");
						if (paidWeight2){
							defaultValues.push({
								name: "qrtPaidWeight",
								value: paidWeight2
							});
						}
						var volume2 = result2.entity.get("qrtVolume");
						if (volume2){
							defaultValues.push({
								name: "qrtVolume",
								value: volume2
							});
						}
						var grossWeight2 = result2.entity.get("qrtGrossWeight");
						if (grossWeight2){
							defaultValues.push({
								name: "qrtGrossWeight",
								value: grossWeight2
							});
						}
						
						var customer = result2.entity.get("qrtCustomer");
						if (customer){
							defaultValues.push({
								name: "qrtCustomer",
								value: customer.value
							});
						}
						var lead = result2.entity.get("qrtLead");
						if (lead){
							defaultValues.push({
								name: "qrtLead",
								value: lead.value
							});
						}
						
						defaultValues.push({
							name: "qrtRequest",
							value: this.get("ActiveRow")
						});
						
						defaultValues.push({
							name: "ModifiedOn",
							value: new Date()
						});
						
                      	defaultValues.push({
							name: "qrtNotes",
							value: result2.entity.get("qrtNotes")
						});
                      	
						if (button[0] === ("CCL" || "FTL" ||"FCL")) {
							this.set("qrtStatusRecord", button[0]);
							this.sandbox.subscribe("PublishDetailRequestCalculation", this.getDetailNameFromWhoOpen, this);
							this.openCardInChain({
								schemaName: "qrtRequestCalculation1Page",
								operation: "add",
								moduleId: "00000000-0000-0000-0000-000000000000",
								//Автоматически устанавливаем значения в открываемой карточке
								defaultValues: defaultValues
							});
						} else {
							var esq = Ext.create("Terrasoft.EntitySchemaQuery", {rootSchemaName: "qrtRequestDimensions"});
							var esq1Filter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,"qrtRequest", recordId);
							esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
							esq.filters.add("esq1Filter", esq1Filter);
		
							esq.getEntityCollection(function(result) {
								//Общее число активных ежедневных изданий
								var dataCount = result.collection.collection.length;
								
								if (dataCount > 0 || button[1] === "true") {
									this.set("qrtStatusRecord", button[0]);
									this.sandbox.subscribe("PublishDetailRequestCalculation", this.getDetailNameFromWhoOpen, this);
									this.openCardInChain({
										schemaName: "qrtRequestCalculation1Page",
										operation: "add",
										moduleId: "00000000-0000-0000-0000-000000000000",
										//Автоматически устанавливаем значения в открываемой карточке
										defaultValues: defaultValues
									});
								} else {
									this.showInformationDialog("Для создания Расчета обязательно ввести габариты груза или вложить файл с габаритами и сделать отметку «Габариты в файле»");
								}
							}, this);
						}
					}, this);
				}
			},
			setButtonVisible: function(activeRow, context) {
				if (activeRow) {
					context.set("ButtonVisible", false);
				}
			},
		}
	};
});
