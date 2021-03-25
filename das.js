define("qrtRequest1Page", ["ProcessModuleUtilities", "MoneyModule", "MultiCurrencyEdit", "MultiCurrencyEditUtilities"],
	function (ProcessModuleUtilities, MoneyModule, MultiCurrencyEdit,
		MultiCurrencyEditUtilities, BusinessRuleModule, ConfigurationConstants) {
		return {
			entitySchemaName: "qrtRequest",
			attributes: {
				"qrtAddressUnloading": {
					lookupListConfig: {
						filters: [
							function () {
								var customer = this.get("qrtConsignee");

								var filterGroup = Ext.create("Terrasoft.FilterGroup");
								filterGroup.add("agentFilter",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.EQUAL,
										"Account",
										customer.value));
								filterGroup.add("agentFilter1",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.IS_NOT_NULL,
										"Address"));
								filterGroup.add("agentFilter3",
									Terrasoft.createColumnIsNotNullFilter("Address"));
								filterGroup.add("agentFilter5",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.IS_NOT_NULL,
										"AddressType"));
								filterGroup.add("agentFilter6",
									Terrasoft.createColumnIsNotNullFilter("AddressType"));
								return filterGroup;
							}
						]
					},
					dependencies: [
						{
							columns: ["qrtAddressUnloading"],
							methodName: "setAddressUnloading"
						}
					]
				},
				"qrtAddressLoading": {
					lookupListConfig: {
						filters: [
							function () {
								var customer = this.get("qrtSender");

								var filterGroup = Ext.create("Terrasoft.FilterGroup");
								filterGroup.add("agentFilter",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.EQUAL,
										"Account",
										customer.value));

								filterGroup.add("agentFilter1",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.IS_NOT_NULL,
										"Address"));
								filterGroup.add("agentFilter3",
									Terrasoft.createColumnIsNotNullFilter("Address"));
								filterGroup.add("agentFilter5",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.IS_NOT_NULL,
										"AddressType"));
								filterGroup.add("agentFilter6",
									Terrasoft.createColumnIsNotNullFilter("AddressType"));
								return filterGroup;
							}
						]
					},
					dependencies: [
						{
							columns: ["qrtAddressLoading"],
							methodName: "setAddressLoading"
						}
					]
				},
				"qrtContactLoading": {
					// Тип данных атрибута.
					"dataValueType": Terrasoft.DataValueType.LOOKUP,
					dependencies: [
						{
							columns: ["qrtContactLoading"],
							methodName: "onContactLoadingPhone"
						}
					],
					// Конфигурационный объект атрибута типа LOOKUP.
					"lookupListConfig": {
						// Массив фильтров, применяемых к запросу для формирования данных поля-справочника.
						"filters": [
							function () {
								var filterGroup = Ext.create("Terrasoft.FilterGroup");
								// Добавление фильтра "IsUser" в результирующую коллекцию фильтров.
								// Выбирает все записи из корневой схемы Contact, к которой присоединена
								// колонка Id из схемы SysAdminUnit, для которых Id не равен null.
								filterGroup.add("IsAccount",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.EQUAL,
										"Account",
										this.get("qrtSender").value,
										true));
								// // Добавление фильтра "IsActive" в результирующую коллекцию фильтров.
								// // Выбирает все записи из корневой схемы Contact, к которой присоединена
								// // колонка Active из схемы SysAdminUnit, для которых Active=true.
								// filterGroup.add("IsType",
								// 	Terrasoft.createColumnFilterWithParameter(
								// 		Terrasoft.ComparisonType.EQUAL,
								// 		"Type",
								// 		"a1a0a5f6-4feb-4402-8f93-b8eee25e31ce",
								// 		true));
								return filterGroup;
							}
						]
					}
				},
				"qrtContactUnloading": {
					// Тип данных атрибута.
					"dataValueType": Terrasoft.DataValueType.LOOKUP,
					dependencies: [
						{
							columns: ["qrtContactUnloading"],
							methodName: "onContactUnloadingPhone"
						}
					],
					// Конфигурационный объект атрибута типа LOOKUP.
					"lookupListConfig": {
						// Массив фильтров, применяемых к запросу для формирования данных поля-справочника.
						"filters": [
							function () {
								var filterGroup = Ext.create("Terrasoft.FilterGroup");
								// Добавление фильтра "IsUser" в результирующую коллекцию фильтров.
								// Выбирает все записи из корневой схемы Contact, к которой присоединена
								// колонка Id из схемы SysAdminUnit, для которых Id не равен null.
								filterGroup.add("IsAccount",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.EQUAL,
										"Account",
										this.get("qrtConsignee").value,
										true));
								// Добавление фильтра "IsActive" в результирующую коллекцию фильтров.
								// Выбирает все записи из корневой схемы Contact, к которой присоединена
								// колонка Active из схемы SysAdminUnit, для которых Active=true.
								// filterGroup.add("IsType",
								// 	Terrasoft.createColumnFilterWithParameter(
								// 		Terrasoft.ComparisonType.EQUAL,
								// 		"Type",
								// 		"6d0ab62a-787e-49de-9daa-002a782d2a47",
								// 		true));
								return filterGroup;
							}
						]
					}
				},
				"canActiveButton": {
					"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
					"dataValueType": this.Terrasoft.DataValueType.BOOLEAN,
					"value": true,
				},
				"qrtTSWDirectory": {
					dependencies: [
						{
							columns: ["qrtTSWDirectory"],
							methodName: "changeTSWDirectory"
						},
					]
				},
				"qrtBlockField": {
					"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
					"dataValueType": this.Terrasoft.DataValueType.BOOLEAN,
					"value": true,
				},
				"Collection": {
					"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
					"dataValueType": this.Terrasoft.DataValueType.COLLECTION,
					"value": null
				},
				"qrtOrder": {
					"lookupListConfig": {
						"columns": ["qrtPayer"], //Нужное поле
					},
					dependencies: [
						{
							columns: ["qrtOrder"],
							methodName: "onRequestStage"
						}
					]
				},
				"qrtPreviousOwner": {
					"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
					"dataValueType": this.Terrasoft.DataValueType.TEXT,
					"value": null,
				},
				"qrtOurCompany": {
					dependencies: [
						{
							columns: ["qrtOurCompany"],
							methodName: "changeOurCompany"
						},
					]
				},
				"qrtOwner": {
					dependencies: [
						{
							columns: ["qrtOwner"],
							methodName: "changeOwner"
						},
					]
				},
				"qrtAnotherPayer": {
					dependencies: [
						{
							columns: ["qrtAnotherPayer"],
							methodName: "setPlayer"
						},
					]
				},
				"qrtCustomer": {
					dependencies: [
						{
							columns: ["qrtCustomer"],
							methodName: "setPlayer"
						},
					]
				},
				"qrtCustomer2": {
					dependencies: [
						{
							columns: ["qrtCustomer"],
							methodName: "updateRequestContractDetailFilter"
						},
					]
				},
				"qrtCityLoading": {
					"dependencies": [
						{
							"columns": ["qrtCityLoading"],
							"methodName": "setCityLoading"
						}
					]
				},
				"qrtCityUnloading": {
					"dependencies": [
						{
							"columns": ["qrtCityUnloading"],
							"methodName": "setCityUnloading"
						}
					]
				},
				"qrtTypeService": {
					"dependencies": [
						{
							"columns": ["qrtTypeService"],
							"methodName": "saveTypeService"
						}
					]
				},
				"qrtSetPropuctType": {
					"dependencies": [
						{
							"columns": ["qrtCountryLoading", "qrtCountryUnloading"],
							"methodName": "setPropuctType"
						}
					]
				},
				"qrtColorCityLoading": {
					"dependencies": [
						{
							"columns": ["qrtCityLoading"],
							"methodName": "colorCityLoading"
						}
					]
				},
				"qrtColorCityUnloading": {
					"dependencies": [
						{
							"columns": ["qrtCityUnloading"],
							"methodName": "colorCityUnloading"
						}
					]
				},
				"qrtColorCountryLoading": {
					"dependencies": [
						{
							"columns": ["qrtCountryLoading"],
							"methodName": "colorCountryLoading"
						}
					]
				},
				"qrtColorCountryUnloading": {
					"dependencies": [
						{
							"columns": ["qrtCountryUnloading"],
							"methodName": "colorCountryUnloading"
						}
					]
				},
				"qrtService": {
					"dependencies": [
						{
							"columns": ["qrtService"],
							"methodName": "checkService"
						}
					]
				},
				"qrtFileDimension": {
					"dependencies": [
						{
							"columns": ["qrtFileDimension"],
							"methodName": "fileIsSave"
						}
					]
				},
				"qrtStatusRecord": {
					"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
					"dataValueType": this.Terrasoft.DataValueType.TEXT,
					"value": null,
				},
				"qrtStage": {
					lookupListConfig: {
						// Дополнительные колонки.
						columns: ["qrtStage"],
						// Колонка сортировки.
						orders: [{ columnPath: "Description" }],
					},
					"dependencies": [
						{
							"columns": ["qrtStage"],
							"methodName": "requestStage"
						}
					]
				},
				"qrtStage2": {
					"dependencies": [
						{
							"columns": ["qrtStage"],
							"methodName": "canActiveButtonStatus"
						}
					]
				},
				"qrtSendKp": {
					"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
					"dataValueType": this.Terrasoft.DataValueType.BOOLEAN,
					"value": false,
				},
				"qrtInsurancePremium": {
					"dataValueType": this.Terrasoft.DataValueType.FLOAT,
					"dependencies": [
						{
							"columns": ["qrtCurrencyRate", "qrtCurrency"],
							"methodName": "recalculateAmount3"
						}
					]
				},
				"qrtPrimaryInsurancePremium": {
					"dependencies": [
						{
							"columns": ["qrtInsurancePremium"],
							"methodName": "recalculatePrimaryAmount3"
						}
					]
				},
				"qrtArticleTransportation": {
					"dataValueType": this.Terrasoft.DataValueType.FLOAT,
					"dependencies": [
						{
							"columns": ["qrtCurrencyRate", "qrtCurrency"],
							"methodName": "recalculateAmount2"
						}
					]
				},
				"qrtPrimaryArticleTransportation": {
					"dependencies": [
						{
							"columns": ["qrtArticleTransportation"],
							"methodName": "recalculatePrimaryAmount2"
						}
					]
				},
				"qrtContact": {
					dependencies: [
						{
							columns: ["qrtContact"],
							methodName: "onContactPhone"
						},
					],
					// Конфигурационный объект атрибута типа LOOKUP.
					"lookupListConfig": {
						// Массив фильтров, применяемых к запросу для формирования данных поля-справочника.
						"filters": [
							function () {
								var records = this.get("qrtCustomer");
								if (!records) {
									records = "";
								} else {
									records = records.value;
								}
								var filterGroup = Ext.create("Terrasoft.FilterGroup");
								filterGroup.add("IsAccount",
									Terrasoft.createColumnFilterWithParameter(
										Terrasoft.ComparisonType.EQUAL,
										"[ContactCareer:Contact].Account",
										records,
										true));
								return filterGroup;
							}
						]
					}
				},
				"qrtCustomMode": {
					lookupListConfig: {
						// Дополнительные колонки.
						columns: ["qrtCustomMode"],
						// Колонка сортировки.
						orders: [{ columnPath: "Description" }],
					}
				},
				"qrtRoute": {
					"dependencies": [
						{
							"columns": ["qrtCityLoading", "qrtCountryLoading", "qrtCrossing",
								"qrtTSWDirectory", "qrtCityUnloading", "qrtCountryUnloading"],
							"methodName": "setRoute"
						}
					]
				},
				// Валюта.
				"qrtCurrency": {
					// Тип данных атрибута — справочник.
					"dataValueType": this.Terrasoft.DataValueType.LOOKUP,
					// Конфигурация справочнка валют.
					"lookupListConfig": {
						"columns": ["Division", "Symbol"]
					}
				},
				// Курс.
				"qrtCurrencyRate": {
					"dataValueType": this.Terrasoft.DataValueType.FLOAT,
					// Зависимости атрибута.
					"dependencies": [
						{
							// Колонки, от которых зависит атрибут.
							"columns": ["qrtCurrency"],
							// Метод-обработчик.
							"methodName": "setCurrencyRate"
						}
					]
				},
				// Сумма.
				"qrtCargoValue": {
					"dataValueType": this.Terrasoft.DataValueType.FLOAT,
					"dependencies": [
						{
							"columns": ["qrtCurrencyRate", "qrtCurrency"],
							"methodName": "recalculateAmount"
						}
					]
				},
				// Сумма в базовой валюте.
				"qrtPrimaryCargoValue": {
					"dependencies": [
						{
							"columns": ["qrtCargoValue"],
							"methodName": "recalculatePrimaryAmount"
						}
					]
				},
				// Валюта — виртуальная колонка для совместимости с модулем MultiCurrencyEditUtilities.
				"Currency": {
					"type": this.Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
					"dataValueType": this.Terrasoft.DataValueType.LOOKUP,
					"lookupListConfig": {
						"columns": ["Division"]
					},
					"dependencies": [
						{
							"columns": ["Currency"],
							"methodName": "onVirtualCurrencyChange"
						}
					]
				},
				// Коллекция курсов валют
				"CurrencyRateList": {
					dataValueType: this.Terrasoft.DataValueType.COLLECTION,
					value: this.Ext.create("Terrasoft.Collection")
				},
				// Коллекция для кнопки выбора валюты
				"CurrencyButtonMenuList": {
					dataValueType: this.Terrasoft.DataValueType.COLLECTION,
					value: this.Ext.create("Terrasoft.BaseViewModelCollection")
				}
			},
			// Миксины модели представления.
			mixins: {
				// Миксин управления мультивалютностью на странице редактирования.
				MultiCurrencyEditUtilities: "Terrasoft.MultiCurrencyEditUtilities"
			},
			messages: {
				"updateActionElem": {
					mode: Terrasoft.MessageMode.PTP,
					direction: Terrasoft.MessageDirectionType.PUBLISH
				},
				"NeedBlockFieldInRequest": {
					mode: Terrasoft.MessageMode.BROADCAST,
					direction: Terrasoft.MessageDirectionType.SUBSCRIBE
				},
				"StatusUpdatedGrid": {
					mode: Terrasoft.MessageMode.BROADCAST,
					direction: Terrasoft.MessageDirectionType.SUBSCRIBE
				},
				"NeedUpdatedGridDocument": {
					mode: Terrasoft.MessageMode.PTP,
					direction: Terrasoft.MessageDirectionType.SUBSCRIBE
				},
				"PublishDetailRequestCalculation": {
					mode: Terrasoft.MessageMode.PTP,
					direction: Terrasoft.MessageDirectionType.SUBSCRIBE
				},
				//Имя сообщения.
				"NeedUpdatedGridFile": {
					"mode": Terrasoft.MessageMode.BROADCAST,
					"direction": Terrasoft.MessageDirectionType.SUBSCRIBE
				},
				//Имя сообщения.
				"NeedUpdatedGrid2": {
					"mode": Terrasoft.MessageMode.BROADCAST,
					"direction": Terrasoft.MessageDirectionType.SUBSCRIBE
				},
			},
			modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
			details: /**SCHEMA_DETAILS*/{
			"Files": {
				"schemaName": "FileDetailV2",
				"entitySchemaName": "qrtRequestFile",
				"filter": {
					"masterColumn": "Id",
					"detailColumn": "qrtRequest"
				}
			},
			"qrtSchema26Detail25b3cd76": {
				"schemaName": "qrtSchema26Detail",
				"entitySchemaName": "qrtRequestAdress",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			},
			"qrtSchema29Detail692255ee": {
				"schemaName": "qrtSchema29Detail",
				"entitySchemaName": "qrtTransportationParameter",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			},
			"qrtSchema30Detail677d2578": {
				"schemaName": "qrtSchema30Detail",
				"entitySchemaName": "qrtRequestDimensions",
				"subscriber": {
					"methodName": "changeParametersCargo"
				},
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			},
			"qrtCompetitorDealDetail67319f93": {
				"schemaName": "qrtCompetitorDealDetail",
				"entitySchemaName": "qrtCompetitorDeal",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			},
			"qrtSchema14Detaila65f1cc1": {
				"schemaName": "qrtSchema14Detail",
				"entitySchemaName": "qrtRequest",
				"filter": {
					"detailColumn": "qrtParentRequest",
					"masterColumn": "Id"
				}
			},
			"qrtSchemaff4028e6Detail0fe3de31": {
				"schemaName": "qrtSchemaff4028e6Detail",
				"entitySchemaName": "Reminding",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			},
			"qrtSchema5Detail7ea4023f": {
				"schemaName": "qrtSchema5Detail",
				"entitySchemaName": "qrtRequestCalculation",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			},
			"qrtSchema95a37e5cDetailf0646aa3": {
				"schemaName": "qrtSchema95a37e5cDetail",
				"entitySchemaName": "qrtOrder",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			},
			"qrtSchema6Detail": {
				"schemaName": "qrtSchema6Detail",
				"entitySchemaName": "qrtRequestContract",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				},
				"filterMethod": "requestContractDetailFilter"
			},
			"EmailDetailV28e461933": {
				"schemaName": "EmailDetailV2",
				"entitySchemaName": "Activity",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				},
				"defaultValues": {
					"Contact": {
						"masterColumn": "qrtContact"
					},
					"Account": {
						"masterColumn": "qrtCustomer"
					}
				},
				"filterMethod": "getEmailDetailFilter"
			},
			"qrtSchemaee79419eDetail8c2633c8": {
				"schemaName": "qrtSchemaee79419eDetail",
				"entitySchemaName": "qrtChangeRequest",
				"filter": {
					"detailColumn": "qrtRequest",
					"masterColumn": "Id"
				}
			}
		}/**SCHEMA_DETAILS*/,
			businessRules: /**SCHEMA_BUSINESS_RULES*/{
				"qrtArticleTransportation": {
					"e44df9cf-c8b8-4abc-9a0e-a4d68412ac77": {
						"uId": "e44df9cf-c8b8-4abc-9a0e-a4d68412ac77",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsCarriage"
								},
								"rightExpression": {
									"type": 0,
									"value": false,
									"dataValueType": 12
								}
							},
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsCalculationPayments"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtInsurancePremium": {
					"e44df9cf-c8b8-4abc-9a0e-a4d68412ac99": {
						"uId": "e44df9cf-c8b8-4abc-9a0e-a4d68412ac99",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsCarriage"
								},
								"rightExpression": {
									"type": 0,
									"value": false,
									"dataValueType": 12
								}
							},
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsCalculationPayments"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtTSW": {
					"1ba09edb-5160-47c1-8267-610a17430805": {
						"uId": "1ba09edb-5160-47c1-8267-610a17430805",
						"enabled": true,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Industry",
						"comparisonType": 3,
						"type": 0,
						"value": "5159b08b-18f7-4550-ba99-b9fdad67f68d",
						"dataValueType": 10
					}
				},
				"qrtOurCompany": {
					"c91e1e0c-29c8-486a-ae17-0827c73de020": {
						"uId": "c91e1e0c-29c8-486a-ae17-0827c73de020",
						"enabled": true,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Type",
						"comparisonType": 3,
						"type": 0,
						"value": "57412fad-53e6-df11-971b-001d60e938c6",
						"dataValueType": 10
					}
				},
				"qrtSEMT": {
					"d1e0939b-9e66-4236-ab74-c605da25c64e": {
						"uId": "d1e0939b-9e66-4236-ab74-c605da25c64e",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 1,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtOverload"
								},
								"rightExpression": {
									"type": 0,
									"value": false,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtOverload": {
					"bbe7bdc7-5e13-4881-92cf-aadef2ed9c35": {
						"uId": "bbe7bdc7-5e13-4881-92cf-aadef2ed9c35",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 1,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtSEMT"
								},
								"rightExpression": {
									"type": 0,
									"value": false,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtThirdPartySC": {
					"c065454a-b9b4-4374-910f-502e356a3306": {
						"uId": "c065454a-b9b4-4374-910f-502e356a3306",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsInsurance"
								},
								"rightExpression": {
									"type": 0,
									"value": false,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtDateReadyCargo": {
					"5d1caf8b-481e-4731-8ee6-dc7144d66cb7": {
						"uId": "5d1caf8b-481e-4731-8ee6-dc7144d66cb7",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 1,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtDateRequirement"
								},
								"rightExpression": {
									"type": 0,
									"value": "fb9ae086-fbf1-4dd4-bf5a-ad4915e9d285",
									"dataValueType": 10
								}
							},
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtDateRequirement"
								},
								"rightExpression": {
									"type": 0,
									"value": "dfb73111-0040-4e8a-a177-30f467acdfc7",
									"dataValueType": 10
								}
							}
						]
					},
					"0112c418-0401-45f8-aada-50a75d97dc4c": {
						"uId": "0112c418-0401-45f8-aada-50a75d97dc4c",
						"enabled": false,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtDateOption"
								},
								"rightExpression": {
									"type": 0,
									"value": "d0262831-065f-474b-ab95-594091f123e5",
									"dataValueType": 10
								}
							}
						]
					}
				},
				"qrtDeliveryDate": {
					"6538eddc-de91-4948-b019-7c98f7d7b521": {
						"uId": "6538eddc-de91-4948-b019-7c98f7d7b521",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 1,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtDateRequirement"
								},
								"rightExpression": {
									"type": 0,
									"value": "2a648aff-1361-493a-a82e-b081e527bd74",
									"dataValueType": 10
								}
							},
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtDateRequirement"
								},
								"rightExpression": {
									"type": 0,
									"value": "dfb73111-0040-4e8a-a177-30f467acdfc7",
									"dataValueType": 10
								}
							}
						]
					},
					"595e0c4f-9a87-4cd7-9c87-af79d18b8600": {
						"uId": "595e0c4f-9a87-4cd7-9c87-af79d18b8600",
						"enabled": false,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtDateOption"
								},
								"rightExpression": {
									"type": 0,
									"value": "d0262831-065f-474b-ab95-594091f123e5",
									"dataValueType": 10
								}
							}
						]
					}
				},
				"qrtTemperatureRegime": {
					"fee879f2-ac87-492f-ab78-535d54fbc757": {
						"uId": "fee879f2-ac87-492f-ab78-535d54fbc757",
						"enabled": true,
						"removed": true,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsPerishable"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtNonTariffDocument": {
					"5f9ebfd0-3fd7-4faf-ae11-1972aeb63c0b": {
						"uId": "5f9ebfd0-3fd7-4faf-ae11-1972aeb63c0b",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtDocumentAvailability"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtHazardClassLookup": {
					"9f7a041f-6589-4356-ba3d-e65476c21e12": {
						"uId": "9f7a041f-6589-4356-ba3d-e65476c21e12",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsDangerous"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtPattern": {
					"5423b244-5ca7-4533-afc3-3ffcb7b101e2": {
						"uId": "5423b244-5ca7-4533-afc3-3ffcb7b101e2",
						"enabled": true,
						"removed": true,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 1,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtOrder"
								}
							}
						]
					}
				},
				"qrtUN": {
					"e44df9cf-c8b8-4abc-9a0e-a4d68412ac88": {
						"uId": "e44df9cf-c8b8-4abc-9a0e-a4d68412ac88",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtIsDangerous"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtTemperature": {
					"513cb65b-62ca-45a0-970b-4b442ad836a3": {
						"uId": "513cb65b-62ca-45a0-970b-4b442ad836a3",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtTemperatureMode"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtDateRequirement": {
					"e368bfec-63ca-422e-a29e-949481d61d58": {
						"uId": "e368bfec-63ca-422e-a29e-949481d61d58",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 2,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtCityUnloading"
								}
							},
							{
								"comparisonType": 2,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtCityLoading"
								}
							}
						]
					}
				},
				"qrtLead": {
					"92fbcb70-2c1f-4211-9b8f-4c1066dbeed0": {
						"uId": "92fbcb70-2c1f-4211-9b8f-4c1066dbeed0",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 1,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 2,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtLead"
								}
							}
						]
					}
				},
				"qrtPayer": {
					"582fc2fe-a05d-48d5-8b7f-864df72cdce1": {
						"uId": "582fc2fe-a05d-48d5-8b7f-864df72cdce1",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 1,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtAnotherPayer"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					},
					"b943018b-b436-4e8f-bb55-188a8792289c": {
						"uId": "b943018b-b436-4e8f-bb55-188a8792289c",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtAnotherPayer"
								},
								"rightExpression": {
									"type": 0,
									"value": true,
									"dataValueType": 12
								}
							}
						]
					}
				},
				"qrtRejectionReason": {
					"a17210f1-6c0d-4975-9957-594d80b20a60": {
						"uId": "a17210f1-6c0d-4975-9957-594d80b20a60",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 0,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtStage"
								},
								"rightExpression": {
									"type": 0,
									"value": "3b5344da-dc30-49b4-a69d-2665704a9e05",
									"dataValueType": 10
								}
							}
						]
					},
					"3ce8f8c6-0f59-497f-827c-65983a76ba11": {
						"uId": "3ce8f8c6-0f59-497f-827c-65983a76ba11",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 3,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtStage"
								},
								"rightExpression": {
									"type": 0,
									"value": "3b5344da-dc30-49b4-a69d-2665704a9e05",
									"dataValueType": 10
								}
							}
						]
					}
				},
				"qrtHolderClientContract": {
					"5400cf26-4910-4c8d-a1f7-1d73da3ec8c8": {
						"uId": "5400cf26-4910-4c8d-a1f7-1d73da3ec8c8",
						"enabled": true,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Type",
						"comparisonType": 3,
						"type": 0,
						"value": "57412fad-53e6-df11-971b-001d60e938c6",
						"dataValueType": 10
					}
				},
				"qrtOwner": {
					"0269a6aa-b3bc-4ba5-b458-5ae60d9058f4": {
						"uId": "0269a6aa-b3bc-4ba5-b458-5ae60d9058f4",
						"enabled": true,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Type",
						"comparisonType": 3,
						"autoClean": false,
						"autocomplete": false,
						"type": 0,
						"value": "60733efc-f36b-1410-a883-16d83cab0980",
						"dataValueType": 10
					}
				},
				"qrtCityUnloading": {
					"e9b1309a-bfb8-432b-80d8-1988bd364e5b": {
						"uId": "e9b1309a-bfb8-432b-80d8-1988bd364e5b",
						"enabled": false,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Country",
						"comparisonType": 3,
						"autoClean": false,
						"autocomplete": false,
						"type": 1,
						"attribute": "qrtCountryUnloading"
					}
				},
				"qrtCityLoading": {
					"896ceffc-524c-480e-9760-6f9e18c3e99b": {
						"uId": "896ceffc-524c-480e-9760-6f9e18c3e99b",
						"enabled": false,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Country",
						"comparisonType": 3,
						"autoClean": false,
						"autocomplete": false,
						"type": 1,
						"attribute": "qrtCountryLoading"
					}
				},
				"qrtCountryUnloading": {
					"766260c4-235e-4dbf-b967-5d0649323dbe": {
						"uId": "766260c4-235e-4dbf-b967-5d0649323dbe",
						"enabled": false,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Code",
						"comparisonType": 3,
						"autoClean": false,
						"autocomplete": false,
						"type": 1,
						"attribute": "qrtCityUnloading",
						"attributePath": "Country.Code"
					}
				},
				"qrtCountryLoading": {
					"d54fd3ab-b631-475f-8123-290529c0db94": {
						"uId": "d54fd3ab-b631-475f-8123-290529c0db94",
						"enabled": false,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Code",
						"comparisonType": 3,
						"autoClean": false,
						"autocomplete": false,
						"type": 1,
						"attribute": "qrtCityLoading",
						"attributePath": "Country.Code"
					}
				},
				"qrtContactUnloading": {
					"cc1c29bd-b61a-4cd7-a1f3-d300c1ec141a": {
						"uId": "cc1c29bd-b61a-4cd7-a1f3-d300c1ec141a",
						"enabled": false,
						"removed": false,
						"ruleType": 1,
						"baseAttributePatch": "Account",
						"comparisonType": 3,
						"autoClean": false,
						"autocomplete": false,
						"type": 1,
						"attribute": "qrtConsignee"
					}
				},
				"qrtProductType": {
					"c2321f08-11c2-4058-bc6d-d30d9546fe48": {
						"uId": "c2321f08-11c2-4058-bc6d-d30d9546fe48",
						"enabled": true,
						"removed": false,
						"ruleType": 0,
						"property": 2,
						"logical": 0,
						"conditions": [
							{
								"comparisonType": 2,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtCountryLoading"
								}
							},
							{
								"comparisonType": 2,
								"leftExpression": {
									"type": 1,
									"attribute": "qrtCountryUnloading"
								}
							}
						]
					}
				}
			}/**SCHEMA_BUSINESS_RULES*/,
			methods: {
				canActiveButtonStatus: function () {
					var status = this.get("qrtStage");
					if (status) {
						//Создана Заявка
						//Заявка выполнена
						//Отклонен
						//Отложен
						if (status.value !== "0fd00a33-4ee3-412c-8294-03d5159b5e6b" &&
							status.value !== "fc973230-2d8f-45c5-86e8-d84d3cc61ad9" &&
							status.value !== "3b5344da-dc30-49b4-a69d-2665704a9e05" &&
							status.value !== "42b71095-936e-4c8c-b4fe-ff93f7b16d4e") {
							this.set("canActiveButton", true);
						} else {
							this.set("canActiveButton", false);
						}
					}
				},
				setCurrencyRate: function () {
					var currency = this.get("qrtCurrency");
					this.console.log(this.get("qrtCurrency"));
					//Доллар
					if (currency && currency.value === "915e8a55-98d6-df11-9b2a-001d60e938c6" && this.get("qrtCurrencyRateUSD")) {
						this.set("qrtCurrencyRate", 1 / this.get("qrtCurrencyRateUSD"));
					}
					//Евро
					else if (currency && currency.value === "c0057119-53e6-df11-971b-001d60e938c6" && this.get("qrtCurrencyRateEUR")) {
						this.set("qrtCurrencyRate", 1 / this.get("qrtCurrencyRateEUR"));
					} else {
						//Загружает курс валют на дату начала проекта.
						MoneyModule.LoadCurrencyRate.call(this, "qrtCurrency", "qrtCurrencyRate", this.get("StartDate"));
					}
					this.set("qrtCargoBufRate", "qrtCurrencyRate");
				},
				changeTSWDirectory: function () {
					var directory = this.get("qrtTSWDirectory");
					if (directory) {
						var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "Account"
						});
						//СВХ
						var esqFilter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
							"qrtTSWDirectory", directory.value);
						esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
						esq.filters.add("esqFilter", esqFilter);

						esq.getEntityCollection(function (result) {
							if (!result.success) {
								this.showInformationDialog("Ошибка запроса данных");
								return;
							} else if (result.success) {
								if (result.collection.collection.length > 0) {
									var item = result.collection.collection.items[0].get("Id");
									this.loadLookupDisplayValue("qrtTSW", item);
								} else {
									this.set("qrtTSW", null);
								}
							}
						}, this);
					} else {
						this.set("qrtTSW", null);
					}
				},
				blockField2: function () {
					var idRequest = this.get("Id");
					var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", { rootSchemaName: "qrtRequestDimensions" });

					esq.filters.add("filter01",
						this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL, "qrtRequest", idRequest));
					esq.getEntityCollection(function (result) {
						if (!result.success) {
							this.showInformationDialog("Ошибка запроса данных");
							return;
						}
						if (result.collection.collection.length > 0) {
							this.set("qrtBlockField", false);
						} else {
							this.set("qrtBlockField", true);
						}
					}, this);

				},
				blockField: function () {
					this.set("qrtGrossWeight", 0.00);
					this.set("qrtPlacesCount", "0");
					this.set("qrtVolume", 0.00);
					this.set("qrtBlockField", true);
				},
				getEmailDetailFilter: function () {
					var recordId = this.get("Id");
					var filterGroup = new Terrasoft.createFilterGroup();
					filterGroup.add("qrtRequest", Terrasoft.createColumnFilterWithParameter(
						Terrasoft.ComparisonType.EQUAL, "qrtRequest", recordId));
					//Email
					filterGroup.add("ActivityType", Terrasoft.createColumnFilterWithParameter(
						Terrasoft.ComparisonType.EQUAL, "Type", "e2831dec-cfc0-df11-b00f-001d60e938c6"));
					return filterGroup;
				},
				updateRequestContractDetailFilter: function () {
					debugger;
					var customerId = this.get("qrtCustomer");
					var serviceList = this.get("qrtService");
					var orderId = this.get("qrtPayer");

					var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
						rootSchemaName: "qrtRequestContract"
					});

					esq.addColumn("qrtContract.qrtType", "Type");
					esq.addColumn("qrtContract", "Contract");

					if (customerId) {
						if (serviceList.indexOf("VED") !== -1 ||
							serviceList.indexOf("FIN") !== -1 ||
							serviceList.indexOf("WHS") !== -1) {
							var esqFilter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
								"qrtRequest", this.get("Id"));
							esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
							esq.filters.add("esqFilter", esqFilter);

						} else if (serviceList === "CCL") {
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
							serviceList.indexOf("LCS") !== -1 || serviceList.indexOf("RLR") !== -1)) {
							//Исполнитель = Абипа (DEV/PROD)
							var esqFilter4 = esq.createColumnInFilterWithParameters(
								"qrtContract.qrtSupplier", ["e59055ed-4501-4d41-b370-562cbdbd270d", "e308b781-3c5b-4ecb-89ef-5c1ed4da488e"]);
							//Согласован,Подписан
							var esqFilter5 = esq.createColumnInFilterWithParameters(
								"qrtContract.qrtState", ["5ed15f38-4eb6-46f0-9137-8459468b334c", "6173800d-8966-487e-aeb7-d68b278c58f8"]);
							//Плательщик
							if (orderId) {
								var esqFilter6 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
									"qrtContract.qrtCustomer", orderId.value);
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
						"qrtRequest", this.get("Id"));
					esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
					esq.filters.add("esqFilter10", esqFilter10);

					esq.getEntityCollection(function (result) {
						if (!result.success) {
							this.showInformationDialog("Ошибка запроса данных");
							return;
						}
						if (result.collection.collection.length > 0) {
							var existsContactsCollection = [];
							var self = this;
							result.collection.each(function (item, index, array) {
								var selfId = item.get("Id");
								var type = item.get("Type");
								//Разовый "21453f48-1adc-4171-a160-d7a152f9cb76"
								if (type && type.value === "21453f48-1adc-4171-a160-d7a152f9cb76") {
									var contact = item.get("Contract");

									if (contact) {
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
												this.showInformationDialog("Ошибка запроса данных");
												return;
											}
											if (result2.collection.collection.length === 0) {
												existsContactsCollection.push(selfId);
												window.console.log(existsContactsCollection);
											}
										}, this);
										//----------------------------------------------
									}
								} else {
									existsContactsCollection.push(selfId);
								}
							});
							this.set("Collection", existsContactsCollection);
							window.console.log(existsContactsCollection);

						}
					}, this);
				},
				requestContractDetailFilter: function () {
					var filterGroup = new this.Terrasoft.createFilterGroup();
					if (!Ext.isEmpty(this.get("Collection"))) {
						filterGroup.add("ContractStatus", this.Terrasoft.createColumnInFilterWithParameters(
							"Id", this.get("Collection")));
					} else {
						filterGroup.add("ContractStatu1s", this.Terrasoft.createColumnFilterWithParameter(
							Terrasoft.ComparisonType.EQUAL,
							"qrtRequest", this.get("Id")));
					}
					return filterGroup;
				},
				changeOurCompany: function () {
					if (!this.get("qrtHolderClientContract") && this.get("qrtOurCompany")) {
						this.set("qrtHolderClientContract", this.get("qrtOurCompany"));
					}
				},
				changeOwner: function () {
					var owner = this.get("qrtOwner");
					var previousOwner = this.get("qrtPreviousOwner");

					if (owner) {
						var processArgs = {
							sysProcessName: "qrtProcess_323c7d5",// название бизнесс процесса
							parameters: {
								ProcessId: this.get("Id"), //название параметра в БП
								ProcessOwner: owner.value,
								ProcessPreviousOwner: previousOwner,
							}
						};
						ProcessModuleUtilities.executeProcess(processArgs);
					}
				},
				requestStage: function () {
					var stage = this.get("qrtStage");
					var name = this.get("qrtName");
					if (stage) {
						//Отложен
						if (stage.value === "42b71095-936e-4c8c-b4fe-ff93f7b16d4e") {
							this.showInformationDialog("Чтобы актуализировать Запрос в дату, оговоренную с клиентом создайте для себя " +
								"активность «Связаться с клиентом, актуализировать запрос №_" + name + "» с указанием даты звонка.");
						}
					}
				},
				setPlayer: function () {
					var anotherPlayer = this.get("qrtAnotherPayer");

					if (anotherPlayer) {
						this.set("qrtPayer", null);
					} else {
						var customer = this.get("qrtCustomer");
						this.set("qrtPayer", customer);
					}
				},
				setCityLoading: function () {
					var recordId = this.get("qrtCityLoading");
					if (recordId) {
						var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "City"
						});
						esq.addColumn("Country");

						esq.getEntity(recordId.value, function (result) {
							if (!result.success) {
								// обработка/логирование ошибки, например
								this.showInformationDialog("Ошибка запроса данных");
								return;
							}
							var country = result.entity.get("Country");

							this.set("qrtCountryLoading", country);
						}, this);
					}
				},
				setCityUnloading: function () {
					var recordId = this.get("qrtCityUnloading");
					if (recordId) {
						var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "City"
						});
						esq.addColumn("Country");

						esq.getEntity(recordId.value, function (result) {
							if (!result.success) {
								// обработка/логирование ошибки, например
								this.showInformationDialog("Ошибка запроса данных");
								return;
							}
							var country = result.entity.get("Country");

							this.set("qrtCountryUnloading", country);
						}, this);
					}
				},
				saveTypeService: function () {
					this.save({ isSilent: true });
				},
				setPropuctType: function () {
					var recordId = this.get("qrtOurCompany");
					var countryLoading = this.get("qrtCountryLoading");
					var countryUnloading = this.get("qrtCountryUnloading");

					if (countryLoading && countryUnloading) {
						if (recordId) {
							var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
								rootSchemaName: "Account"
							});
							esq.addColumn("Country");

							esq.getEntity(recordId.value, function (result) {
								if (!result.success) {
									// обработка/логирование ошибки, например
									this.showInformationDialog("Ошибка запроса данных");
									return;
								}
								var country = result.entity.get("Country");
								//RU
								if (country && country.value === "a570b005-e8bb-df11-b00f-001d60e938c6") {

									if (countryLoading.value === "a570b005-e8bb-df11-b00f-001d60e938c6"
										&& countryUnloading.value === "a570b005-e8bb-df11-b00f-001d60e938c6") {
										this.loadLookupDisplayValue("qrtProductType", "55cb2961-77d2-439b-9b1e-ffc41afe77a6");
									} else if (countryLoading.value === "a570b005-e8bb-df11-b00f-001d60e938c6"
										&& countryUnloading.value !== "a570b005-e8bb-df11-b00f-001d60e938c6") {
										this.loadLookupDisplayValue("qrtProductType", "d3a7e452-1290-43b2-a543-770d0ad8eef4");
									} else if (countryLoading.value !== "a570b005-e8bb-df11-b00f-001d60e938c6"
										&& countryUnloading.value === "a570b005-e8bb-df11-b00f-001d60e938c6") {
										this.loadLookupDisplayValue("qrtProductType", "918dae28-5c4b-4cbd-bb15-647e7ddeb932");
									} else if (countryLoading.value !== "a570b005-e8bb-df11-b00f-001d60e938c6"
										&& countryUnloading.value !== "a570b005-e8bb-df11-b00f-001d60e938c6") {
										this.loadLookupDisplayValue("qrtProductType", "3c190a1d-98bc-4270-9bf8-ff2fcd8430f7");
									}
								} else if (country && country.value !== "a570b005-e8bb-df11-b00f-001d60e938c6") {

									if (countryLoading.value === "ab862349-ba25-43f8-8669-ccb757dec63e"
										&& countryUnloading.value === "ab862349-ba25-43f8-8669-ccb757dec63e") {
										this.loadLookupDisplayValue("qrtProductType", "55cb2961-77d2-439b-9b1e-ffc41afe77a6");
									} else if (countryLoading.value === "ab862349-ba25-43f8-8669-ccb757dec63e"
										&& countryUnloading.value !== "ab862349-ba25-43f8-8669-ccb757dec63e") {
										this.loadLookupDisplayValue("qrtProductType", "d3a7e452-1290-43b2-a543-770d0ad8eef4");
									} else if (countryLoading.value !== "ab862349-ba25-43f8-8669-ccb757dec63e"
										&& countryUnloading.value === "ab862349-ba25-43f8-8669-ccb757dec63e") {
										this.loadLookupDisplayValue("qrtProductType", "918dae28-5c4b-4cbd-bb15-647e7ddeb932");
									} else if (countryLoading.value !== "ab862349-ba25-43f8-8669-ccb757dec63e"
										&& countryUnloading.value !== "ab862349-ba25-43f8-8669-ccb757dec63e") {
										this.loadLookupDisplayValue("qrtProductType", "3c190a1d-98bc-4270-9bf8-ff2fcd8430f7");
									}
								} else {
									this.showInformationDialog("Заполните поле \"Страны\" у Нашей компаний");
								}


							}, this);
						} else {
							this.showInformationDialog("Заполните поле \"Наша Компания\"");
						}
					}
				},
				colorCountryLoading: function () {
					var selector = Ext.get("qrtRequest1PageqrtCountryLoadinga0132271-e1d9-415e-872d-7baae374f8d2LookupEdit-el");
					var countryLoading = this.get("qrtCountryLoading");
					var countryLoadingColor = (Ext.isEmpty(countryLoading)) ? "#DCDCDC" : "#FFFFFF";
					selector.setStyle("background-color", countryLoadingColor);
				},
				colorCityLoading: function () {
					var selector = Ext.get("qrtRequest1PageqrtCityLoading1ed3e4ff-f28b-4e63-9412-a2b5c3de6af5LookupEdit-el");
					var cityLoading = this.get("qrtCityLoading");
					var cityLoadingColor = (Ext.isEmpty(cityLoading)) ? "#DCDCDC" : "#FFFFFF";
					selector.setStyle("background-color", cityLoadingColor);
				},
				colorCountryUnloading: function () {
					var selector = Ext.get("qrtRequest1PageqrtCountryUnloading6a6640c1-ca51-4230-8b67-c37bc9ebdae0LookupEdit-el");
					var countryUnloading = this.get("qrtCountryUnloading");
					var countryUnloadingColor = (Ext.isEmpty(countryUnloading)) ? "#DCDCDC" : "#FFFFFF";
					selector.setStyle("background-color", countryUnloadingColor);
				},
				colorCityUnloading: function () {
					var selector = Ext.get("qrtRequest1PageqrtCityUnloadingeb5f342e-96cd-40ec-84c3-8c707011b4e3LookupEdit-el");
					var cityUnloading = this.get("qrtCityUnloading");
					var cityUnloadingColor = (Ext.isEmpty(cityUnloading)) ? "#DCDCDC" : "#FFFFFF";
					selector.setStyle("background-color", cityUnloadingColor);
				},
				checkService: function () {
					var result = "";
					var answer = false;

					if (this.get("qrtService")) {
						result = this.get("qrtService").split(/, /g);
						result.forEach(function (item) {
							if (item !== "CCL") {
								answer = true;
								return;
							}
						}, this);
					}
					this.set("qrtIsCarriage", answer);
				},
				onButtonClick: function () {
					debugger;
					var controlConfig = {};
					Terrasoft.utils.inputBox("Проверьте, что все расчеты, подлежащие переносу в Заявки, имеют статус \"Утвержден\"",
						function (buttonCode, controlData) {
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
						{ defaultButton: 0 }
					);

				},
				createRequestButton: function () {
					var activeRow = this.get("Id");
					var contactId = this.get("qrtContact");
					Terrasoft.chain(
						function (next) {
							if (contactId) {
								var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", { rootSchemaName: "ContactCommunication" });
								esq.addColumn("Number");

								esq.filters.add("filter01", this.Terrasoft.createColumnFilterWithParameter(
									this.Terrasoft.ComparisonType.EQUAL, "Contact", contactId.value));
								//Тип = Email
								esq.filters.add("filter02", this.Terrasoft.createColumnFilterWithParameter(
									this.Terrasoft.ComparisonType.EQUAL, "CommunicationType", "ee1c85c3-cfcb-df11-9b2a-001d60e938c6"));

								esq.getEntityCollection(function (result) {
									if (!result.success) {
										// обработка/логирование ошибки, например
										this.showInformationDialog("Ошибка запроса данных:\n" + "\nErrorCode: " + result.errorInfo.errorCode + "\nMessage: " + result.errorInfo.message);
										return;
									}
									if (result.collection.collection.length > 0) {
										var mail = "";
										result.collection.each(function (item) {
											mail = item.get("Number");
											return;
										});
										if (Ext.isEmpty(mail)) {
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

								esq.getEntityCollection(function (result) {
									if (!result.success) {
										this.showInformationDialog("Что то пошло не так");
										return;
									}
									if (result.collection.getCount() === 0) {
										this.showInformationDialog("Для создания Заявки требуется утвердить по одному расчету на ТО и транспорт");
										return;
									}
									var listCalc = [];
									var requestCompany = this.get("qrtOurCompany");
									result.collection.each(function (item, index, array) {
										var nameService = item.get("qrtTypeService");
										listCalc.push(item.get("Id"));
										var company = item.get("qrtOurCompany");

										//!CCL
										if (nameService && nameService.value !== "33607268-f494-497d-993a-016e0670ae85") {
											if (company && requestCompany && company.value === requestCompany.value && listCalc.length > 1) {
												[listCalc[0], listCalc[listCalc.length - 1]] = [listCalc[listCalc.length - 1], listCalc[0]];
											}
										}
									});

									window.console.log(listCalc);
									this.Terrasoft.SysSettings.querySysSettingsItem("qrtOrderLastNumber", function (value) {
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
				fileIsSave: function () {
					var self = this; // промежуточная перменная
					setTimeout(function () {
						self.save({
							isSilent: true, callback: function () {
								var actionMenuItems2 = self.getActions2();
								self.console.log("ДО");
								self.console.log(self.get("ActionsButtonMenuItems2"));
								self.set("ActionsButtonMenuItems2", actionMenuItems2);
								self.console.log("ПОСЛЕ");
								self.console.log(self.get("ActionsButtonMenuItems2"));
								self.sandbox.publish("updateActionElem", self.get("Id"), ["qrtRequest1Section"]); 
								setTimeout(function () {
									
									self.onRender();
                                                     
								}, 100);
							}
						});
					}, 100);

				},
				isFirstCount: function () {

					var recordId = this.get("Id");
					var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
						rootSchemaName: "qrtRequestCalculation"
					});

					esq.addColumn("qrtTypeService", "qrtTypeService");

					var esq1Filter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
						"qrtRequest", recordId);
					esq.filters.add("esq1Filter", esq1Filter);

					esq.getEntityCollection(function (result) {
						if (!result.success) {
							this.showInformationDialog("Ошибка запроса данных");
							return;
						}
						var isCarriage = false;
						result.collection.each(function (item) {
							if (item.get("qrtTypeService").value !== "33607268-f494-497d-993a-016e0670ae85") {
								isCarriage = true;
							}
						});
					}, this);
				},
				getActions: function () {
					var actionMenuItems = this.callParent(arguments);
					actionMenuItems.addItem(this.getButtonMenuItem({
						Type: "Terrasoft.MenuSeparator",
						Caption: ""
					}));
					actionMenuItems.addItem(this.getButtonMenuItem({
						"Caption": "Отправка КП",
						"Tag": "sendKp",
						"Enabled": { "bindTo": "canActiveButton" }
					}));
					actionMenuItems.addItem(this.getButtonMenuItem({
						"Caption": { "bindTo": "Resources.Strings.CreateOrderButtonCaption" },
						"Click": { "bindTo": "onButtonClick" },
						"Enabled": { "bindTo": "canActiveButton" }
					}));
					return actionMenuItems;
				},
				getActions2: function () {
					var actionMenuItems = this.Ext.create("Terrasoft.BaseViewModelCollection");
					var answer = "";
					if (this.get("qrtService")) {
						answer = this.get("qrtService").split(/, /g);
					}
					var isFile = this.get("qrtFileDimension");
					this.console.log("Файл " + isFile);
					this.console.log("Сервисы " + answer);
					var self = this;
					if (answer.length > 0) {
						answer.forEach(function (item) {
							this.console.log(item + " ," + isFile);
							actionMenuItems.addItem(self.getButtonMenuItem({
								"Caption": item,
								"Tag": item + "," + isFile,
								"Click": { "bindTo": "sendRequest" },
								"Enabled": { "bindTo": "canEntityBeOperated" }
							}));
						});
					} else {
						actionMenuItems.addItem(this.getButtonMenuItem({
							"Caption": "Добавить",
							//"Caption": {"bindTo": "Resources.Strings.AddButtonCaption"},//текст кнопки
							"Tag": null,
							"Click": { "bindTo": "sendRequest" },
							"Enabled": { "bindTo": "canEntityBeOperated" }
						}));
					}


					return actionMenuItems;
				},
				getDetailNameFromWhoOpen: function () {
					return {
						param1: this.get("Id"),
						param2: this.get("qrtStatusRecord"),
						param3: this.get("qrtOwner").value,
						param4: this.get("qrtIsInsurance"),
						param0: this.get("Collection"),
					};
				},
				sendRequest: function (args) {
                    
					this.set("qrtStatusRecord", null);
					var button = String(args).split(",");
                    
					var сargoValue = this.get("qrtCargoValue");
					var fileDimension = this.get("qrtFileDimension");
                    this.console.log(button);
                    this.console.log(args);
                    this.console.log(fileDimension);
                   
					//
					var сityLoading = this.get("qrtCityLoading");
					if (!сityLoading) {
						this.showInformationDialog("Поле \"Город загрузки\': Необходимо указать значение");
						return;
					}

					var сountryLoading = this.get("qrtCountryLoading");
					if (!сountryLoading) {
						this.showInformationDialog("Поле \"Страна загрузки\': Необходимо указать значение");
						return;
					}

					var сityUnloading = this.get("qrtCityUnloading");
					if (!сityUnloading) {
						this.showInformationDialog("Поле \"Город выгрузки\': Необходимо указать значение");
						return;
					}

					var сountryUnloading = this.get("qrtCountryUnloading");
					if (!сountryUnloading) {
						this.showInformationDialog("Поле \"Страна выгрузки\': Необходимо указать значение");
						return;
					}

					var сroductType = this.get("qrtProductType");
					if (!сroductType) {
						this.showInformationDialog("Поле \"Направление перевозки\': Необходимо указать значение");
						return;
					}

					var dateRequirement = this.get("qrtDateRequirement");
					if (!dateRequirement) {
						this.showInformationDialog("Поле \"Требования к датам\': Необходимо указать значение");
						return;
					}

					var productDescription = this.get("qrtProductDescription");
					if (!productDescription) {
						this.showInformationDialog("Поле \"Описание груза\': Необходимо указать значение");
						return;
					}

					if (Ext.isEmpty(сargoValue) || сargoValue === 0) {
						this.showInformationDialog("Поле \"Стоимость груза\": Необходимо указать значение");
						return;
					}

					if (button[0] === "CCL") {

						var numberFEA = this.get("qrtNumberFEA");
						if (Ext.isEmpty(numberFEA)) {
							this.showInformationDialog("Поле \"Кол-во ТН ВЭД\": Необходимо указать значение");
							return;
						}

						var customMode = this.get("qrtCustomMode");
						if (!customMode) {
							this.showInformationDialog("Поле \"Таможенный режим\": Необходимо указать значение");
							return;
						}

                        var incoterms = this.get("qrtIncoterms");
                        if(!incoterms){
                            this.showInformationDialog("Поле \"Инкотермс\": Необходимо указать значение");
							return;
                        }
					}
					//
					var defaultValues = [];
					var placesCount = this.get("qrtPlacesCount");
					if (placesCount) {
						defaultValues.push({
							name: "qrtPlacesCount",
							value: placesCount
						});
					}
					var paidWeight = this.get("qrtPaidWeight");
					if (paidWeight) {
						defaultValues.push({
							name: "qrtPaidWeight",
							value: paidWeight
						});
					}
					var volume = this.get("qrtVolume");
					if (volume) {
						defaultValues.push({
							name: "qrtVolume",
							value: volume
						});
					}
					var grossWeight = this.get("qrtGrossWeight");
					if (grossWeight) {
						defaultValues.push({
							name: "qrtGrossWeight",
							value: grossWeight
						});
					}

					var customer = this.get("qrtCustomer");
					if (customer) {
						defaultValues.push({
							name: "qrtCustomer",
							value: customer.value
						});
					}
					var lead = this.get("qrtLead");
					if (lead) {
						defaultValues.push({
							name: "qrtLead",
							value: lead.value
						});
					}
					defaultValues.push({
						name: "qrtRequest",
						value: this.get("Id")
					});
					defaultValues.push({
						name: "qrtTransitTime",
						value: ""
					});
					this.console.log(defaultValues);
					defaultValues.push({
						name: "ModifiedOn",
						value: new Date()
					});
					defaultValues.push({
						name: "qrtNotes",
						value: this.get("qrtNotes")
					});
                    //Если габариты в файле заполнены то проверяем значение эти полей
					if (fileDimension && ((Ext.isEmpty(this.get("qrtPlacesCount")) || this.get("qrtPlacesCount") === 0) || (Ext.isEmpty(this.get("qrtVolume")) || this.get("qrtVolume") === 0) || (Ext.isEmpty(this.get("qrtGrossWeight")) || this.get("qrtGrossWeight") === 0))) {
						this.showInformationDialog("Укажите итоговые параметры по габаритам груза");
						return;
					}

					if (button[0] === ("CCL" || "FTL" || "FCL")) {
						this.set("qrtStatusRecord", button[0]);
						this.sandbox.subscribe("PublishDetailRequestCalculation", this.getDetailNameFromWhoOpen, this);
						this.save({
							isSilent: true, callback: function () {
								this.openCardInChain({
									schemaName: "qrtRequestCalculation1Page",
									operation: "add",
									moduleId: "00000000-0000-0000-0000-000000000000",
									//Автоматически устанавливаем значения в открываемой карточке
									defaultValues: defaultValues
								});
								return;
							}
						});

					} else {
                        
						var recordId = this.get("Id");

						var esq = Ext.create("Terrasoft.EntitySchemaQuery", { rootSchemaName: "qrtRequestDimensions" });
						var esq1Filter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL, "qrtRequest", recordId);
						esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
						esq.filters.add("esq1Filter", esq1Filter);

						esq.getEntityCollection(function (result) {
							//Общее число активных ежедневных изданий
							var dataCount = result.collection.collection.length;

							if (dataCount > 0 || fileDimension) {
								this.set("qrtStatusRecord", button[0]);
								this.sandbox.subscribe("PublishDetailRequestCalculation", this.getDetailNameFromWhoOpen, this);
								this.save({
									isSilent: true, callback: function () {
										this.openCardInChain({
											schemaName: "qrtRequestCalculation1Page",
											operation: "add",
											moduleId: "00000000-0000-0000-0000-000000000000",
											//Автоматически устанавливаем значения в открываемой карточке
											defaultValues: defaultValues
										});
										return;
									}
								});
							} //отказались от данного функционала 
							else {
							this.showInformationDialog("Для создания Расчета обязательно ввести " + 
							"габариты груза или вложить файл с габаритами и сделать отметку «Габариты в файле»");
							}
						}, this);
					}
				},
				sendKp: function () {
					var recordId = this.get("qrtOwner");
					var esq = Ext.create("Terrasoft.EntitySchemaQuery", { rootSchemaName: "VwSysAdminUnit" });
					esq.addColumn("Contact", "Contact");
					var esq1Filter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL, "Contact", recordId.value);
					esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
					esq.filters.add("esq1Filter", esq1Filter);

					esq.getEntityCollection(function (result) {
						if (!result.success) {
							// обработка/логирование ошибки, например
							this.showInformationDialog("Ошибка запроса данных");
							return;
						}
						if (result.collection.collection.length > 0) {
							var sysAdminUnitId = result.collection.collection.items[0].get("Id");
							//----------------------------------------------
							var esq2 = Ext.create("Terrasoft.EntitySchemaQuery", { rootSchemaName: "SysAdminUnitGrantedRight" });
							esq2.addColumn("GranteeSysAdminUnit.Contact.Id", "GranteeSysAdminUnit");// получает права
							esq2.addColumn("GrantorSysAdminUnit", "GrantorSysAdminUnit");// раздает права
							var esq2Filter = esq.createColumnFilterWithParameter(
								Terrasoft.ComparisonType.EQUAL, "GrantorSysAdminUnit", sysAdminUnitId);
							esq2.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
							esq2.filters.add("esq2Filter", esq2Filter);

							esq2.getEntityCollection(function (result2) {
								if (!result2.success) {
									// обработка/логирование ошибки, например
									this.showInformationDialog("Ошибка запроса данных");
									return;
								}
								var isCurrent = false;
								for (var i = 0; i < result2.collection.collection.length; i++) {
									if (result2.collection.collection.items[i].get("GranteeSysAdminUnit") ===
										Terrasoft.SysValue.CURRENT_USER_CONTACT.value) {
										isCurrent = true;
									}
								}
								if (!isCurrent && recordId.value !== Terrasoft.SysValue.CURRENT_USER_CONTACT.value &&
									Terrasoft.SysValue.CURRENT_USER_CONTACT.value !== "410006e1-ca4e-4502-a9ec-e54d922d2c00") {
									this.showInformationDialog("У вас нет прав на перевод в статус Проверен. Обратитесь к автору Запроса.");
									return;
								}
								if (isCurrent || recordId.value === Terrasoft.SysValue.CURRENT_USER_CONTACT.value ||
									Terrasoft.SysValue.CURRENT_USER_CONTACT.value === "410006e1-ca4e-4502-a9ec-e54d922d2c00") {
									var config = {
										entitySchemaName: "qrtRequestCalculation",
										columnName: "qrtRequestCalculation",
										multiSelect: true,
									};
									this.set("qrtSendKp", true);
									this.openLookup(config, this.addCallBack, this);
								}

							}, this);
						} else {
							if (Terrasoft.SysValue.CURRENT_USER_CONTACT.value === "410006e1-ca4e-4502-a9ec-e54d922d2c00") {
								var config = {
									entitySchemaName: "qrtRequestCalculation",
									columnName: "qrtRequestCalculation",
									multiSelect: true,
								};
								this.set("qrtSendKp", true);
								this.openLookup(config, this.addCallBack, this);
							} else {
								this.showInformationDialog("У вас нет прав на перевод в статус Проверен. Обратитесь к автору Запроса.");
							}

						}
					}, this);
				},
				init: function () {
					this.callParent(arguments);
					this.getActions2();
					this.sandbox.subscribe("NeedBlockFieldInRequest", this.blockField, this);
					this.sandbox.subscribe("NeedUpdatedGridFile", this.updateFiles, this);
					this.sandbox.subscribe("NeedUpdatedGridDocument", this.myUpdateMethod, this);
					this.sandbox.subscribe("StatusUpdatedGrid", this.statusUpdate, this);
					this.sandbox.subscribe("NeedUpdatedGrid2", this.myGridUpdate2, this);
				},
				updateFiles: function (args) {
					this.updateDetail({ detail: "Files", realoadAll: true });
				},
				statusUpdate: function (args) {
					var self = this;
					this.save({
						isSilent: true, callback: function () {

							
							self.onReloadCard();
						}
					});

				},
				myGridUpdate2: function (args) {
					this.showInformationDialog("Заявка (-и) успешно созданы");
					this.loadLookupDisplayValue("qrtStage", "0fd00a33-4ee3-412c-8294-03d5159b5e6b", function () {
						this.save({
							isSilent: true, callback: function () {
								this.onReloadCard();
							}
						});
					});
				},
				typeFilter: function () {
					var filterGroup = new this.Terrasoft.createFilterGroup();
					window.console.log(filterGroup);
					filterGroup.logicalOperation = this.Terrasoft.LogicalOperatorType.AND;
					filterGroup.add("DocumentBFilter", this.Terrasoft.createColumnFilterWithParameter(
						Terrasoft.ComparisonType.EQUAL, "qrtIsTO", false));
					return filterGroup;
				},
				typeFilterTO: function () {
					var filterGroup = new this.Terrasoft.createFilterGroup();
					window.console.log(filterGroup);
					filterGroup.logicalOperation = this.Terrasoft.LogicalOperatorType.AND;
					filterGroup.add("DocumentAFilter", this.Terrasoft.createColumnFilterWithParameter(
						Terrasoft.ComparisonType.EQUAL, "qrtIsTO", true));
					return filterGroup;
				},
				setRoute: function () {
					var text = "";

					var cityLoading = this.get("qrtCityLoading");
					var countryLoading = this.get("qrtCountryLoading");
					var borderCrossing = this.get("qrtCrossing");
					var cityUnloading = this.get("qrtCityUnloading");
					var countryUnloading = this.get("qrtCountryUnloading");
					var tsw = this.get("qrtTSWDirectory");

					if (cityLoading) {
						text += cityLoading.displayValue;

						var recordId = this.get("qrtCityLoading").value;
						var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "City"
						});
						esq.addColumn("Country", "Country");

						esq.getEntity(recordId, function (result) {
							if (!result.success) {
								// обработка/логирование ошибки, например
								this.showInformationDialog("Ошибка запроса данных");
								return;
							}
							if (countryLoading) {
								text += (text !== "") ? ", " + countryLoading.displayValue : countryLoading.displayValue;
							}

							if (borderCrossing) {
								text += (text !== "") ? " - " + borderCrossing : borderCrossing;
							}

							if (tsw) {
								text += (text !== "") ? " - " + tsw.displayValue : tsw.displayValue;
							}

							if (cityUnloading) {
								text += (text !== "") ? " - " + cityUnloading.displayValue : cityUnloading.displayValue;

								var recordId3 = this.get("qrtCityUnloading").value;

								var esq3 = this.Ext.create("Terrasoft.EntitySchemaQuery", {
									rootSchemaName: "City"
								});
								esq3.addColumn("Description", "Description");
								esq3.addColumn("Country", "Country");

								esq3.getEntity(recordId3, function (result3) {
									if (!result3.success) {
										// обработка/логирование ошибки, например
										this.showInformationDialog("Ошибка запроса данных");
										return;
									}

									if (countryUnloading) {
										text += (text !== "") ? ", " + countryUnloading.displayValue : countryUnloading.displayValue;
									}

									this.set("qrtRoute", text);
									if (!this.isNewMode()) {
										var self = this; // промежуточная перменная
										setTimeout(function () { self.save({ isSilent: true }); }, 100);
									}
								}, this);
							}
							else {
								if (countryUnloading) {
									text += (text !== "") ? " - " + countryUnloading.displayValue : countryUnloading.displayValue;
								}

								this.set("qrtRoute", text);
								if (!this.isNewMode()) {
									var self = this; // промежуточная перменная
									setTimeout(function () { self.save({ isSilent: true }); }, 100);
								}
							}
						}, this);
					}
					else {
						if (countryLoading) {
							text += (text !== "") ? " - " + countryLoading.displayValue : countryLoading.displayValue;
						}

						if (borderCrossing) {
							text += (text !== "") ? " - " + borderCrossing : borderCrossing;
						}

						if (tsw) {
							text += (text !== "") ? " - " + tsw.displayValue : tsw.displayValue;
						}

						if (cityUnloading) {
							text += (text !== "") ? " - " + cityUnloading.displayValue : cityUnloading.displayValue;

							var recordId2 = this.get("qrtCityUnloading").value;

							var esq2 = this.Ext.create("Terrasoft.EntitySchemaQuery", {
								rootSchemaName: "City"
							});
							esq2.addColumn("Description", "Description");
							esq2.addColumn("Country", "Country");

							esq2.getEntity(recordId2, function (result2) {
								if (!result2.success) {
									// обработка/логирование ошибки, например
									this.showInformationDialog("Ошибка запроса данных");
									return;
								}

								if (result2.entity.get("Country").value !== "a570b005-e8bb-df11-b00f-001d60e938c6"
									&& result2.entity.get("Description")) {
									text += "(" + result2.entity.get("Description") + ")";
								}

								if (countryUnloading) {
									text += (text !== "") ? ", " + countryUnloading.displayValue : countryUnloading.displayValue;
								}

								this.set("qrtRoute", text);
								if (!this.isNewMode()) {
									var self = this; // промежуточная перменная
									setTimeout(function () { self.save({ isSilent: true }); }, 100);
								}
							}, this);
						}
						else {
							if (countryUnloading) {
								text += (text !== "") ? " - " + countryUnloading.displayValue : countryUnloading.displayValue;
							}

							this.set("qrtRoute", text);
							if (!this.isNewMode()) {
								var self = this; // промежуточная перменная
								setTimeout(function () { self.save({ isSilent: true }); }, 100);
							}
						}
					}
				},
				// Переопределение базового метода Terrasoft.BasePageV2.onEntityInitialized().
				onEntityInitialized: function () {
					// Вызов родительской реализации метода onEntityInitialized.
					this.callParent(arguments);
					this.set("Currency", this.get("qrtCurrency"), { silent: true });
					// Инициализация миксина управления мультивалютностью.
					this.mixins.MultiCurrencyEditUtilities.init.call(this);

					if (this.get("qrtCargoValue") === "0,00") {
						this.set("qrtCargoValue", 0);
					}
					if (this.get("qrtPrimaryCargoValue") === "0,00") {
						this.set("qrtPrimaryCargoValue", 0);
					}
					if (this.get("qrtCurrencyRate") === "0,00") {
						this.set("qrtCurrencyRate", 0);
					}
					if (this.get("qrtGrossWeight") === "0,00") {
						this.set("qrtGrossWeight", 0);
					}
					if (this.get("qrtPaidWeight") === "0,00") {
						this.set("qrtPaidWeight", 0);
					}
					if (this.get("qrtVolume") === "0,00") {
						this.set("qrtVolume", 0);
					}

					this.isFirstCount();
					var actionMenuItems2 = this.getActions2();
					this.set("ActionsButtonMenuItems2", actionMenuItems2);

					if (this.isNewMode()) {
						var recordId = Terrasoft.SysValue.CURRENT_USER_CONTACT.value;

						var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "Contact"
						});
						esq.addColumn("Account");

						esq.getEntity(recordId, function (result) {
							if (!result.success) {
								// обработка/логирование ошибки, например
								this.showInformationDialog("Ошибка запроса данных");
								return;
							}
							var accountId = result.entity.get("Account");
							if (accountId) {
								this.set("qrtOurCompany", accountId);
							}
						}, this);
					}

					var owner = this.get("qrtOwner");
					if (owner) {
						this.set("qrtPreviousOwner", owner.value);
					}
					this.updateRequestContractDetailFilter();
					this.blockField2();
					this.canActiveButtonStatus();
				},
				onCurrencyRateUSD: function () {
					var today = this.get("qrtCreatedOn");

					//************************************************
					var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
						rootSchemaName: "CurrencyRate"
					});
					esq.addColumn("StartDate", "StartDate");
					esq.addColumn("Currency", "Currency");
					esq.addColumn("Rate", "Rate");

					// Фильтр
					var esqFirstFilter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL, "StartDate", today);
					var esqFirstFilter2 = esq.createColumnFilterWithParameter(
						Terrasoft.ComparisonType.EQUAL, "Currency", "c0057119-53e6-df11-971b-001d60e938c6"); //EUR

					esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
					esq.filters.add("esqFirstFilter", esqFirstFilter);
					esq.filters.add("esqFirstFilter2", esqFirstFilter2);

					// В данную коллекцию попадут объекты - результаты запроса, отфильтрованные по двум фильтрам.
					esq.getEntityCollection(function (result) {
						var rateEUR = null;
						if (result.success) {
							result.collection.each(function (item) {
								rateEUR = item.get("Rate");
							});

							if (rateEUR) {
								this.set("qrtCurrencyRateEUR", 1 / rateEUR);
							}
							else {
								this.set("qrtCurrencyRateEUR", 0);
							}
						}
					}, this);
					//************************************************
					var esq2 = this.Ext.create("Terrasoft.EntitySchemaQuery", {
						rootSchemaName: "CurrencyRate"
					});
					esq2.addColumn("StartDate", "StartDate");
					esq2.addColumn("Currency", "Currency");
					esq2.addColumn("Rate", "Rate");

					// Фильтр
					var esqFirstFilter3 = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL, "StartDate", today);
					var esqFirstFilter4 = esq.createColumnFilterWithParameter(
						Terrasoft.ComparisonType.EQUAL, "Currency", "915e8a55-98d6-df11-9b2a-001d60e938c6"); //USD

					esq2.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
					esq2.filters.add("esqFirstFilter3", esqFirstFilter3);
					esq2.filters.add("esqFirstFilter4", esqFirstFilter4);

					// В данную коллекцию попадут объекты - результаты запроса, отфильтрованные по двум фильтрам.
					esq2.getEntityCollection(function (result2) {
						var rateUSD = null;
						if (result2.success) {
							result2.collection.each(function (item2) {
								rateUSD = item2.get("Rate");
							});

							if (rateUSD) {
								this.set("qrtCurrencyRateUSD", 1 / rateUSD);
							}
							else {
								this.set("qrtCurrencyRateUSD", 0);
							}
						}
					}, this);
				},
				// Пересчитывает сумму.
				recalculateAmount: function () {
					var currency = this.get("qrtCurrency");
					var division = currency ? currency.Division : null;
					MoneyModule.RecalcCurrencyValue.call(this, "qrtCurrencyRate", "qrtCargoValue", "qrtPrimaryCargoValue", division);
				},
				// Пересчитывает сумму в базовой валюте.
				recalculatePrimaryAmount: function () {
					var currency = this.get("qrtCurrency");
					var division = currency ? currency.Division : null;
					MoneyModule.RecalcBaseValue.call(this, "qrtCurrencyRate", "qrtCargoValue", "qrtPrimaryCargoValue", division);
				},
				recalculateAmount2: function () {
					var currency = this.get("qrtCurrency");
					var division = currency ? currency.Division : null;
					MoneyModule.RecalcCurrencyValue.call(
						this, "qrtCurrencyRate", "qrtArticleTransportation", "qrtPrimaryArticleTransportation", division);
				},
				recalculatePrimaryAmount2: function () {
					var currency = this.get("qrtCurrency");
					var division = currency ? currency.Division : null;
					MoneyModule.RecalcBaseValue.call(
						this, "qrtCurrencyRate", "qrtArticleTransportation", "qrtPrimaryArticleTransportation", division);
				},
				recalculateAmount3: function () {
					var currency = this.get("qrtCurrency");
					var division = currency ? currency.Division : null;
					MoneyModule.RecalcCurrencyValue.call(
						this, "qrtCurrencyRate", "qrtInsurancePremium", "qrtPrimaryInsurancePremium", division);
				},
				recalculatePrimaryAmount3: function () {
					var currency = this.get("qrtCurrency");
					var division = currency ? currency.Division : null;
					MoneyModule.RecalcBaseValue.call(
						this, "qrtCurrencyRate", "qrtInsurancePremium", "qrtPrimaryInsurancePremium", division);
				},
				// Обработчик изменения виртуальной колонки валюты.
				onVirtualCurrencyChange: function () {
					var currency = this.get("Currency");
					this.set("qrtCurrency", currency);
				},
				openLookup: function (config, callback, scope) {
					if (config.columnName === "qrtTypeService") {
						var service = this.get("qrtService");

						if (service) {
							var list = service.split(/, /g);
							var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
								rootSchemaName: "qrtTypeService"
							});

							var esqFirstFilter = esq.createColumnInFilterWithParameters("Name", list);
							esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.OR;
							esq.filters.add("esqFirstFilter", esqFirstFilter);

							esq.getEntityCollection(function (result) {
								if (!result.success) {
									this.showInformationDialog("Ошибка запроса данных");
									return;
								} else if (result.success) {
									var existsContactsCollection = [];
									result.collection.each(function (item) {
										existsContactsCollection.push(item.get("Id"));
									});

									config.multiSelect = true;
									//Если ничего не выбрано, то снимаем все метки
									config.selectedRows = (list[0] !== "") ? existsContactsCollection : null;
									Terrasoft.LookupUtilities.open({
										"lookupConfig": config,
										"sandbox": this.sandbox,
										"keepAlive": config.keepAlive,
										"lookupModuleId": config.lookupModuleId,
										"lookupPageName": config.lookupPageName,
									}, this.addCallBack, this);
								}
							}, this);
						} else {
							config.multiSelect = true;
							Terrasoft.LookupUtilities.open({
								"lookupConfig": config,
								"sandbox": this.sandbox,
								"keepAlive": config.keepAlive,
								"lookupModuleId": config.lookupModuleId,
								"lookupPageName": config.lookupPageName,
							}, this.addCallBack, this);
						}
					}
					//---------------->
					else if (config.columnName === "qrtRequestCalculation") {
						debugger;
						var esq2 = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "qrtRequestCalculation"
						});
						var esq2Filter = esq2.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
							"qrtRequest", this.get("Id"));
						//Проверен, На согласовании у клиента, Утвержден, Частично выполнен
						var esq2Filter2 = esq2.createColumnInFilterWithParameters(
							"qrtStatus", ["9ee36036-dd40-4eca-862b-df0b983de156", "01de507a-06b5-4960-ae60-38f204620b13",
							"c9c87bbd-b105-4066-be61-e711f04f4e67", "2d64aa9c-6d1d-4497-ae3b-89c1eabdeb0d"]);
						esq2.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
						esq2.filters.add("esq2Filter", esq2Filter);
						esq2.filters.add("esq2Filter2", esq2Filter2);

						esq2.getEntityCollection(function (result2) {
							if (!result2.success) {
								this.showInformationDialog("Ошибка запроса данных");
								return;
							} else if (result2.success) {
								var existsContactsCollection = [];
								result2.collection.each(function (item) {
									existsContactsCollection.push(item.get("Id"));
								});

								config.multiSelect = true;
								if (result2.collection.collection.length > 0) {
									var existsFilter = this.Terrasoft.createColumnInFilterWithParameters("Id",
										existsContactsCollection);
									existsFilter.comparisonType = this.Terrasoft.ComparisonType.EQUAL;
									existsFilter.Name = "existsFilter";
									config.filters = existsFilter;

									Terrasoft.LookupUtilities.open({
										"lookupConfig": config,
										"sandbox": this.sandbox,
										"keepAlive": config.keepAlive,
										"lookupModuleId": config.lookupModuleId,
										"lookupPageName": config.lookupPageName,
									}, this.addCallBack, this);
								} else {
									this.showInformationDialog("Нет подходящих расчетов для отправки");
								}
							}
						}, this);
						//<----------------
					} else {
						Terrasoft.LookupUtilities.open({
							"lookupConfig": config,
							"sandbox": this.sandbox,
							"keepAlive": config.keepAlive,
							"lookupModuleId": config.lookupModuleId,
							"lookupPageName": config.lookupPageName,
						}, this.addCallBack, this);
					}
				},
				addCallBack: function (args) {
					this.selectedRows = args.selectedRows.getItems();
					this.selectedItems = [];
					var selectedItems2 = [];
					var count = 0;

					this.selectedRows.forEach(function (item) {
						this.selectedItems.push(item.displayValue);
						selectedItems2.push(item.value);
						if (item.displayValue === "VED") {
							count = this.selectedItems.length - 1;
						}
						this.loadLookupDisplayValue(args.columnName, item.value);
					}, this);

					if (args.columnName === "qrtTypeService") {
						if (count !== 0) {
							this.selectedItems[count] = this.selectedItems[0];
							this.selectedItems[0] = "VED";
						}
						this.set("qrtService", String(this.selectedItems).replace(/,/g, ", "));

						var actionMenuItems2 = this.getActions2();
						this.set("ActionsButtonMenuItems2", actionMenuItems2);
						return;
					}
					//---------------->
					else if (args.columnName === "qrtRequestCalculation") {
						//Открываем окно Email
						if (this.get("qrtSendKp") === true) {
							//To do...
							//----------------------------------------------
							var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
								rootSchemaName: "qrtRequestCalculation"
							});
							esq.addColumn("qrtTypeService");
							esq.addColumn("qrtName");
							esq.addColumn("Id");

							var esqFilter = esq.createColumnInFilterWithParameters(
								"Id", selectedItems2);
							esq.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
							esq.filters.add("esqFilter", esqFilter);

							esq.getEntityCollection(function (result) {
								if (!result.success) {
									this.showInformationDialog("Ошибка запроса данных");
									return;
								} else if (result.success) {
									var serviceCollection = [];
									var requestCalculationCollection = [];
									var requestCalculationNameCollection = [];
									result.collection.each(function (item) {
										var service = item.get("qrtTypeService");
										if (service) {
											serviceCollection.push(service.value);
											requestCalculationCollection.push(item.get("Id"));
											requestCalculationNameCollection.push(item.get("qrtName"));
										}
									});
									if (result.collection.collection.length > 0) {
										//----------------------------------------------
										var contactId = this.get("qrtContact");
										if (contactId) {
											var esq2 = this.Ext.create("Terrasoft.EntitySchemaQuery", {
												rootSchemaName: "Contact"
											});
											esq2.addColumn("Language");

											esq2.getEntity(contactId.value, function (result2) {
												if (!result2.success) {
													// обработка/логирование ошибки, например
													this.showInformationDialog("Ошибка запроса данных");
													return;
												}
												var language = result2.entity.get("Language");
												if (language) {
													//----------------------------------------------
													var esq3 = this.Ext.create("Terrasoft.EntitySchemaQuery", {
														rootSchemaName: "qrtRequestCalculationPrintForm"
													});

													esq3.addColumn("qrtService");

													var esq3Filter = esq3.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
														"qrtAction", "a24f516a-309b-4cfb-80aa-b56ea201344b");
													var esq3Filter2 = esq3.createColumnInFilterWithParameters(
														"qrtService", serviceCollection);
													var esq3Filter3 = esq3.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
														"qrtLanguage", language.value);

													esq3.filters.logicalOperation = Terrasoft.LogicalOperatorType.AND;
													esq3.filters.add("esq3Filter", esq3Filter);
													esq3.filters.add("esq3Filter2", esq3Filter2);
													esq3.filters.add("esq3Filter3", esq3Filter3);

													esq3.getEntityCollection(function (result3) {
														if (!result3.success) {
															this.showInformationDialog("Ошибка запроса данных");
															return;
														} else if (result3.success) {
															var resultCollection = [];
															var resultNameCollection = [];
															result3.collection.each(function (item2) {
																var service = item2.get("qrtService");
																if (service) {
																	for (var i = 0; i < requestCalculationNameCollection.length; i++) {
																		if (service.value === serviceCollection[i]) {
																			resultCollection.push(requestCalculationCollection[i]);
																			resultNameCollection.push(requestCalculationNameCollection[i]);
																		}
																	}
																}
															});

															if (resultCollection.length === 0) {
																var controlConfig = {};
																Terrasoft.utils.inputBox("Нет шаблона (ов) КП." + "\n" + "Продолжить формирование письма без вложений?",
																	function (buttonCode, controlData) {
																		if (buttonCode === "yes") {
																			var processArgs3 = {
																				sysProcessName: "qrtProcess_b1a38e0",

																				parameters: {
																					ProcessGuids: resultCollection,
																					ProcessCount: resultCollection.length,
																					ProcessRequestId: this.get("Id"),
																					ProcessHeading: "ABIPA_Коммерческое предложение № " +
																						String(resultNameCollection).replace(/,/g, ", "),
																				}

																			};
																			ProcessModuleUtilities.executeProcess(processArgs3);

																			this.set("qrtSendKp", false);
																		}
																	},
																	[
																		{
																			className: "Terrasoft.Button",
																			caption: "Да",
																			returnCode: "yes"
																		},
																		{
																			className: "Terrasoft.Button",
																			caption: "Нет",
																			returnCode: "no"
																		}
																	],
																	this,
																	controlConfig,
																	{ defaultButton: 0 }
																);
															} else if (resultCollection.length < selectedItems2.length) {
																this.showInformationDialog(
																	"Не для всех выбранных расчетов найдены шаблоны КП. В письмо будут вложены не все КП");

																var processArgs = {
																	sysProcessName: "qrtProcess_b1a38e0",

																	parameters: {
																		ProcessGuids: resultCollection,
																		ProcessCount: resultCollection.length,
																		ProcessRequestId: this.get("Id"),
																		ProcessHeading: "ABIPA_Коммерческое предложение № " + String(resultNameCollection).replace(/,/g, ", "),
																	}

																};
																ProcessModuleUtilities.executeProcess(processArgs);

																this.set("qrtSendKp", false);
															} else {
																var processArgs2 = {
																	sysProcessName: "qrtProcess_b1a38e0",

																	parameters: {
																		ProcessGuids: selectedItems2,
																		ProcessCount: selectedItems2.length,
																		ProcessRequestId: this.get("Id"),
																		ProcessHeading: "ABIPA_Коммерческое предложение № " + String(this.selectedItems).replace(/,/g, ", "),
																	}

																};
																ProcessModuleUtilities.executeProcess(processArgs2);

																this.set("qrtSendKp", false);
															}
														}
													}, this);
													//----------------------------------------------
												} else {
													this.showInformationDialog("Заполните \'Язык общения\' у Контактного лица");
												}
											}, this);
										} else {
											this.showInformationDialog("Поле \'Контактное лицо\' не заполнено");
										}
										//----------------------------------------------
									}
								}
							}, this);
							//----------------------------------------------
						}
					}
					//<----------------
				},
				round: function (value, decimals) {
					return Number(Math.round(value + "e" + decimals) + "e-" + decimals);
				},
				changeParametersCargo: function () {
					var idRequest = this.get("Id");
					var Weight = 0.00;
					var Volume = 0.00;
					var PaidWeight = 0.00;
					var PackagesNumber = 0;
					Terrasoft.chain(
						function (next) {
							var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", { rootSchemaName: "qrtRequestDimensions" });
							esq.addColumn("qrtWeight");
							esq.addColumn("qrtVolume");
							esq.addColumn("qrtPaidWeight");
							esq.addColumn("qrtPackagesNumber");
							esq.addColumn("qrtIsStack");
							esq.filters.add("filter01",
								this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL, "qrtRequest", idRequest));
							esq.getEntityCollection(function (result) {
								if (result.success) {
									result.collection.each(function (item) {
										if (!item.get("qrtIsStack")) {
											Weight += item.get("qrtWeight");
											Volume += item.get("qrtVolume");
											PaidWeight += item.get("qrtPaidWeight");
											PackagesNumber += item.get("qrtPackagesNumber");
										}
									});
								}
								next();
							});
						},
						function (next) {
							window.console.log(Weight);
							this.set("qrtGrossWeight", this.round(Weight, 2));
							this.set("qrtVolume", Volume);
							this.set("qrtPaidWeight", PaidWeight);
							this.set("qrtPlacesCount", String(PackagesNumber));
							this.blockField2();
							this.save({ isSilent: true });
						}, this);
				},
			},
			dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
			diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "actions2",
				"values": {
					"itemType": 5,
					"caption": "Отправить запрос на расчет",
					"classes": {
						"textClass": [
							"actions-button-margin-right"
						],
						"wrapperClass": [
							"actions-button-margin-right"
						]
					},
					"menu": {
						"items": {
							"bindTo": "ActionsButtonMenuItems2"
						}
					},
					"visible": true
				},
				"parentName": "LeftContainer",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "merge",
				"name": "qrtDated7d6df1f-9480-4087-b977-df4d870266b0",
				"values": {
					"enabled": false
				}
			},
			{
				"operation": "merge",
				"name": "qrtOwner83a259eb-2ce6-4b13-8cad-02b774f40b41",
				"values": {
					"enabled": true,
					"contentType": 5
				}
			},
			{
				"operation": "insert",
				"name": "LOOKUPc4705c4d-2e98-4ea2-9a5f-eb5809f1c800",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "qrtOurCompany",
					"enabled": true,
					"contentType": 3
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "LOOKUPf2c03132-fc8f-4470-ad2b-c832a3b741a9",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 8,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "qrtTypeService",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "STRING63af6586-6e69-40ac-9c32-23fa83029f0c",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 16,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "qrtService",
					"enabled": false
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "BOOLEANf5d4f1b5-101b-419c-9d66-516f8ae1c79e",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 18,
						"row": 2,
						"layoutName": "Header"
					},
					"bindTo": "qrtProject",
					"enabled": true
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "LOOKUP76dc8569-7b3e-436b-8164-b75475484d5d",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "Header"
					},
					"bindTo": "qrtLead",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "merge",
				"name": "qrtStage512e9b08-d5a6-4e4e-ab88-8220571ee214",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 8,
						"row": 2,
						"layoutName": "Header"
					}
				}
			},
			{
				"operation": "move",
				"name": "qrtStage512e9b08-d5a6-4e4e-ab88-8220571ee214",
				"parentName": "Header",
				"propertyName": "items",
				"index": 8
			},
			{
				"operation": "insert",
				"name": "qrtCustomer520871ef-a65a-4a00-9bd1-262683c59313",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "Header"
					},
					"bindTo": "qrtCustomer"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 9
			},
			{
				"operation": "merge",
				"name": "qrtContact6c991e4a-358b-4d5c-8e35-e79bae775efd",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 8,
						"row": 3,
						"layoutName": "Header"
					},
					"enabled": true,
					"contentType": 5
				}
			},
			{
				"operation": "merge",
				"name": "qrtEmaild39ce0e2-3c21-4afe-8192-6250c0db96b3",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 16,
						"row": 3,
						"layoutName": "Header"
					}
				}
			},
			{
				"operation": "move",
				"name": "qrtEmaild39ce0e2-3c21-4afe-8192-6250c0db96b3",
				"parentName": "Header",
				"propertyName": "items",
				"index": 11
			},
			{
				"operation": "insert",
				"name": "LOOKUP422bbaed-ab13-471e-a481-ad2f99ec0013",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 8,
						"row": 4,
						"layoutName": "Header"
					},
					"bindTo": "qrtRejectionReason",
					"enabled": true,
					"contentType": 3
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 12
			},
			{
				"operation": "merge",
				"name": "qrtContactPhone3f70fca9-aeaf-48fd-8d68-cc98a75a7c2d",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 16,
						"row": 4,
						"layoutName": "Header"
					}
				}
			},
			{
				"operation": "insert",
				"name": "Tab8e87459eTabLabelGroup304245e2",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tab8e87459eTabLabelGroup304245e2GroupCaption"
					},
					"itemType": 15,
					"markerValue": "added-group",
					"items": []
				},
				"parentName": "Tab8e87459eTabLabel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Tab8e87459eTabLabelGridLayout824a4083",
				"values": {
					"itemType": 0,
					"items": []
				},
				"parentName": "Tab8e87459eTabLabelGroup304245e2",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtCountryLoadinga0132271-e1d9-415e-872d-7baae374f8d2",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtCountryLoading",
					"afterrender": {
						"bindTo": "colorCountryLoading"
					},
					"afterrerender": {
						"bindTo": "colorCountryLoading"
					},
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtCityLoading1ed3e4ff-f28b-4e63-9412-a2b5c3de6af5",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 6,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtCityLoading",
					"afterrender": {
						"bindTo": "colorCityLoading"
					},
					"afterrerender": {
						"bindTo": "colorCityLoading"
					},
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "qrtCountryUnloading6a6640c1-ca51-4230-8b67-c37bc9ebdae0",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 12,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtCountryUnloading",
					"afterrender": {
						"bindTo": "colorCountryUnloading"
					},
					"afterrerender": {
						"bindTo": "colorCountryUnloading"
					},
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "qrtCityUnloadingeb5f342e-96cd-40ec-84c3-8c707011b4e3",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 18,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtCityUnloading",
					"afterrender": {
						"bindTo": "colorCityUnloading"
					},
					"afterrerender": {
						"bindTo": "colorCityUnloading"
					},
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "STRING9ce4a067-a7cb-4a07-bee3-44b256c85e9e",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 6,
						"row": 1,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtLoadingIndex",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "STRING0837a7f2-f921-4e57-8f65-a9b213c59165",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 18,
						"row": 1,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtUnloadingIndex",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "qrtSenderd1637964-8c6e-4f9a-99ba-5e7f3d50724e",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtSender",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "qrtConsigneede2099bb-5879-42ed-af9d-3d713ee871dd",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 2,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtConsignee",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "insert",
				"name": "qrtAddressLoading357f6a6b-46f1-4fe9-a8e9-46cb464937db",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtAddressLoading"
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 8
			},
			{
				"operation": "insert",
				"name": "qrtAddressUnloading6560be3d-755b-497e-a13b-2b5f50deb7a2",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 3,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtAddressUnloading"
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 9
			},
			{
				"operation": "insert",
				"name": "qrtContactLoadingec347d08-6121-4691-bf28-8918e124b347",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 4,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtContactLoading",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 10
			},
			{
				"operation": "insert",
				"name": "qrtContactUnloadingb3a8eb20-1992-4aef-be52-8ca0af303d5b",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 4,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtContactUnloading",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 11
			},
			{
				"operation": "insert",
				"name": "qrtPhoneContactLoading4461a22f-52df-4764-a36f-cc819f1cec95",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 5,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtPhoneContactLoading",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 12
			},
			{
				"operation": "insert",
				"name": "qrtPhoneContactUnloade9490453-0270-4028-abfe-6cf4ac1af2a9",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 5,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtPhoneContactUnload"
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 13
			},
			{
				"operation": "insert",
				"name": "qrtComments8e45281c-4b0c-49ca-bbf7-b5c57740b5d5",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 6,
						"layoutName": "Tab8e87459eTabLabelGridLayout824a4083"
					},
					"bindTo": "qrtComments",
					"enabled": true,
					"labelConfig": {
						"caption": {
							"bindTo": "Resources.Strings.qrtComments8e45281c4b0c49cabbf7b5c57740b5d5LabelCaption"
						}
					},
					"contentType": 0
				},
				"parentName": "Tab8e87459eTabLabelGridLayout824a4083",
				"propertyName": "items",
				"index": 14
			},
			{
				"operation": "insert",
				"name": "Tab8e87459eTabLabelGroup11afd486",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tab8e87459eTabLabelGroup11afd486GroupCaption"
					},
					"itemType": 15,
					"markerValue": "added-group",
					"items": []
				},
				"parentName": "Tab8e87459eTabLabel",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Tab8e87459eTabLabelGridLayout75f63734",
				"values": {
					"itemType": 0,
					"items": []
				},
				"parentName": "Tab8e87459eTabLabelGroup11afd486",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtIsInsurance31f4639b-cbb7-4095-b7a5-823d7e7a6a43",
				"values": {
					"layout": {
						"colSpan": 4,
						"rowSpan": 1,
						"column": 20,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtIsInsurance"
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtIncoterms0b3abd48-2589-4c6b-855e-0d233634b2ac",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 6,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtIncoterms"
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "LOOKUP6f1d7667-b13f-41cd-aa6b-8d54d1c8295d",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 11,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtCustomsRegime",
					"enabled": true,
					"contentType": 3
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "qrtProductType38156bf0-7843-41cb-a58e-b54934ccf34d",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtProductType",
					"enabled": true,
					"contentType": 3
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "BOOLEANa3d1f737-4f66-4435-ac07-d536ed60cbdb",
				"values": {
					"layout": {
						"colSpan": 4,
						"rowSpan": 1,
						"column": 20,
						"row": 1,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtPickup",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "qrtDateReadyCargo286c71f9-257f-4a62-b13e-743ca08624c4",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 10,
						"row": 1,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtDateReadyCargo",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "qrtDateRequirementfe756766-db90-4a4c-96cc-dbea9686c76f",
				"values": {
					"layout": {
						"colSpan": 10,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtDateRequirement",
					"enabled": true,
					"contentType": 3
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "BOOLEAN5659367e-e356-447c-91e1-a6f8cfe43cbf",
				"values": {
					"layout": {
						"colSpan": 4,
						"rowSpan": 1,
						"column": 20,
						"row": 2,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtInvoiceReplacement",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "insert",
				"name": "qrtDeliveryDate10315b08-b2af-4bc5-bbe3-d57e4145c339",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 10,
						"row": 2,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtDeliveryDate",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 8
			},
			{
				"operation": "insert",
				"name": "STRING37c32658-7d0d-4ca3-927c-96178dd74fa1",
				"values": {
					"layout": {
						"colSpan": 10,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtTransitTime",
					"enabled": true
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 9
			},
			{
				"operation": "insert",
				"name": "STRING05154e74-4d3d-4e47-983e-088037e3bd3c",
				"values": {
					"layout": {
						"colSpan": 16,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "Tab8e87459eTabLabelGridLayout75f63734"
					},
					"bindTo": "qrtSpecialCommentary",
					"labelConfig": {
						"caption": {
							"bindTo": "Resources.Strings.STRING05154e744d3d4e47983e088037e3bd3cLabelCaption"
						}
					},
					"enabled": true,
					"contentType": 0
				},
				"parentName": "Tab8e87459eTabLabelGridLayout75f63734",
				"propertyName": "items",
				"index": 10
			},
			{
				"operation": "insert",
				"name": "qrtSchema26Detail25b3cd76",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tab8e87459eTabLabel",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "qrtSchema29Detail692255ee",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tab8e87459eTabLabel",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "qrtSchemaee79419eDetail8c2633c8",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tab8e87459eTabLabel",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "Taba5b0ad95TabLabel",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Taba5b0ad95TabLabelTabCaption"
					},
					"items": [],
					"order": 1
				},
				"parentName": "Tabs",
				"propertyName": "tabs",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Taba5b0ad95TabLabelGroup064d2369",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Taba5b0ad95TabLabelGroup064d2369GroupCaption"
					},
					"itemType": 15,
					"markerValue": "added-group",
					"items": []
				},
				"parentName": "Taba5b0ad95TabLabel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"values": {
					"itemType": 0,
					"items": []
				},
				"parentName": "Taba5b0ad95TabLabelGroup064d2369",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "BOOLEAN4e36e747-20f5-432a-8425-d1df3c28268f",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 13,
						"row": 0,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtTemperatureMode",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtIsDangerouse3ee1dc0-e3d6-4622-88c2-364ae0473806",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 18,
						"row": 0,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtIsDangerous",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "qrtProductDescriptionc0cfab19-63c5-4250-a277-ced782a48590",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtProductDescription",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "STRINGb2eabba2-43ad-4f1a-874b-2e0e4da85be8",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 13,
						"row": 1,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtTemperature",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "qrtCargoValuedb011269-176c-44c5-b022-f4bd1b2b0ccc",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtCargoValue",
					"primaryAmount": "qrtPrimaryCargoValue",
					"currency": "qrtCurrency",
					"rate": "qrtCurrencyRate",
					"primaryAmountEnabled": false,
					"generator": "MultiCurrencyEditViewGenerator.generate",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "qrtGrossWeight913ffa32-bec2-4994-9dc2-1af192b157e5",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 6,
						"row": 1,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtGrossWeight",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "LOOKUP2e455903-efcc-45c3-9645-17b9b82a78d9",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 18,
						"row": 1,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtHazardClassLookup",
					"enabled": true,
					"contentType": 3
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "qrtPlacesCount1dac625b-cfdc-4369-b8d9-ced9feb80d7c",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtPlacesCount",
					"enabled": {
						"bindTo": "qrtBlockField"
					}
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "insert",
				"name": "qrtVolumea05132bc-b1ef-49f2-af8e-4286bcfee19f",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 6,
						"row": 2,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtVolume",
					"enabled": {
						"bindTo": "qrtBlockField"
					}
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 8
			},
			{
				"operation": "insert",
				"name": "STRING534e2ba8-1e34-4a6d-aed9-571cd02cfe58",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 18,
						"row": 2,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtUN",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 9
			},
			{
				"operation": "insert",
				"name": "qrtOverload284bdd71-ae0a-42b7-8a93-f4b981bd2120",
				"values": {
					"layout": {
						"colSpan": 4,
						"rowSpan": 1,
						"column": 12,
						"row": 3,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtOverload"
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 10
			},
			{
				"operation": "insert",
				"name": "qrtSEMT7214fc4b-b365-471b-aec1-b35e5ada8f59",
				"values": {
					"layout": {
						"colSpan": 4,
						"rowSpan": 1,
						"column": 16,
						"row": 3,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtSEMT"
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 11
			},
			{
				"operation": "insert",
				"name": "BOOLEANc0ee14a7-f03d-4d29-a4d2-f8226b205dae",
				"values": {
					"layout": {
						"colSpan": 4,
						"rowSpan": 1,
						"column": 20,
						"row": 3,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtFileDimension",
					"enabled": true
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 12
			},
			{
				"operation": "insert",
				"name": "qrtLoadingType2388ca2c-86e1-4297-915f-ccb9318956ef",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtLoadingType"
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 13
			},
			{
				"operation": "insert",
				"name": "qrtRepackagingLabelingbe1185c2-70e3-40f9-a523-aed9ea38f60b",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 6,
						"row": 3,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtRepackagingLabeling",
					"enabled": true,
					"contentType": 0
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 14
			},
			{
				"operation": "insert",
				"name": "qrtCommentCargo1e2f17ec-d2a6-40f2-ae84-690ba8843abf",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 4,
						"layoutName": "Taba5b0ad95TabLabelGridLayouta5b76cf1"
					},
					"bindTo": "qrtCommentCargo",
					"enabled": true,
					"contentType": 0
				},
				"parentName": "Taba5b0ad95TabLabelGridLayouta5b76cf1",
				"propertyName": "items",
				"index": 15
			},
			{
				"operation": "insert",
				"name": "qrtSchema30Detail677d2578",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Taba5b0ad95TabLabel",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Tab74da62c8TabLabel",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tab74da62c8TabLabelTabCaption"
					},
					"items": [],
					"order": 2
				},
				"parentName": "Tabs",
				"propertyName": "tabs",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Tab74da62c8TabLabelGroup6b60b2d5",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tab74da62c8TabLabelGroup6b60b2d5GroupCaption"
					},
					"itemType": 15,
					"markerValue": "added-group",
					"items": []
				},
				"parentName": "Tab74da62c8TabLabel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"values": {
					"itemType": 0,
					"items": []
				},
				"parentName": "Tab74da62c8TabLabelGroup6b60b2d5",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtIsInsurance35a39e10-c7c1-496c-864c-aafb977ca02d",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 12,
						"row": 0,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtIsInsurance",
					"enabled": true
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "FLOATa41889d8-7367-4d83-8a94-45da0de90df0",
				"values": {
					"layout": {
						"colSpan": 7,
						"rowSpan": 1,
						"column": 17,
						"row": 0,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtInsurancePremium",
					"primaryAmount": "qrtPrimaryInsurancePremium",
					"currency": "qrtCurrency",
					"rate": "qrtCurrencyRate",
					"primaryAmountEnabled": false,
					"generator": "MultiCurrencyEditViewGenerator.generate",
					"enabled": true
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "LOOKUP8862b0cb-f2e6-41dd-9ba9-a2464ff104ba",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtCustomMode",
					"enabled": true,
					"contentType": 3
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "BOOLEAN1ab189e5-02d1-4909-94cc-01dfe9d5c045",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 12,
						"row": 1,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtIsCarriage",
					"enabled": false
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "qrtIncotermse252ef4f-68f3-4618-8c80-1c6559e140d5",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtIncoterms"
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "insert",
				"name": "qrtCargoValue762ccb7a-4e2c-480b-8bf7-957b5e373a96",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 6,
						"row": 1,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtCargoValue",
					"primaryAmount": "qrtPrimaryCargoValue",
					"currency": "qrtCurrency",
					"rate": "qrtCurrencyRate",
					"primaryAmountEnabled": false,
					"generator": "MultiCurrencyEditViewGenerator.generate",
					"enabled": true
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "FLOATe782c38e-1e6c-4c86-ad40-487168af4613",
				"values": {
					"layout": {
						"colSpan": 7,
						"rowSpan": 1,
						"column": 17,
						"row": 1,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtArticleTransportation",
					"primaryAmount": "qrtPrimaryArticleTransportation",
					"currency": "qrtCurrency",
					"rate": "qrtCurrencyRate",
					"primaryAmountEnabled": false,
					"generator": "MultiCurrencyEditViewGenerator.generate",
					"enabled": true
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "insert",
				"name": "BOOLEAN7610b89f-c324-4484-9d4d-1fbc264028ea",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 12,
						"row": 2,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtIsCalculationPayments",
					"enabled": true
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 7
			},
			{
				"operation": "insert",
				"name": "qrtCountryLoadingf4e51168-181b-4ee8-bcc7-c55885a9badc",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtCountryLoading"
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 8
			},
			{
				"operation": "insert",
				"name": "qrtNumberFEA8231d93b-0fc3-4e27-b64d-899d1c9a53e5",
				"values": {
					"layout": {
						"colSpan": 6,
						"rowSpan": 1,
						"column": 6,
						"row": 2,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtNumberFEA"
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 9
			},
			{
				"operation": "insert",
				"name": "qrtEDSclientfe68f122-96d5-4c3b-ac70-9709f85d32e4",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 12,
						"row": 3,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtEDSclient"
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 10
			},
			{
				"operation": "insert",
				"name": "BOOLEAN77cf3752-5280-4144-8e8f-6560bb953392",
				"values": {
					"layout": {
						"colSpan": 7,
						"rowSpan": 1,
						"column": 17,
						"row": 3,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtForeignTradeContract",
					"enabled": true
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 11
			},
			{
				"operation": "insert",
				"name": "qrtManufacturer8cfac419-510f-452c-9fec-a04f2004822a",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtManufacturer"
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 12
			},
			{
				"operation": "insert",
				"name": "BOOLEANb3297657-4e23-40f9-bb49-a6953e02d6d8",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 1,
						"row": 4,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtDocumentAvailability",
					"enabled": true
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 13
			},
			{
				"operation": "insert",
				"name": "STRING4f619293-3f41-4c69-9470-835e85856152",
				"values": {
					"layout": {
						"colSpan": 18,
						"rowSpan": 1,
						"column": 6,
						"row": 4,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtNonTariffDocument",
					"enabled": true,
					"contentType": 0
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 14
			},
			{
				"operation": "insert",
				"name": "qrtCommentaryCustom7cc21228-13c8-4142-9a1c-06f4d70a9a2d",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 5,
						"layoutName": "Tab74da62c8TabLabelGridLayoutf9c84a23"
					},
					"bindTo": "qrtCommentaryCustom",
					"enabled": true,
					"labelConfig": {
						"caption": {
							"bindTo": "Resources.Strings.qrtCommentaryCustom7cc2122813c841429a1c06f4d70a9a2dLabelCaption"
						}
					}
				},
				"parentName": "Tab74da62c8TabLabelGridLayoutf9c84a23",
				"propertyName": "items",
				"index": 15
			},
			{
				"operation": "merge",
				"name": "Tabfc48e9daTabLabel",
				"values": {
					"order": 3
				}
			},
			{
				"operation": "insert",
				"name": "Tabfc48e9daTabLabelGroup3a581294",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tabfc48e9daTabLabelGroup3a581294GroupCaption"
					},
					"itemType": 15,
					"markerValue": "added-group",
					"items": []
				},
				"parentName": "Tabfc48e9daTabLabel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Tabfc48e9daTabLabelGridLayout998d9884",
				"values": {
					"itemType": 0,
					"items": []
				},
				"parentName": "Tabfc48e9daTabLabelGroup3a581294",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtAnotherPayer507b3c50-2a76-4864-b68b-637c1b72941e",
				"values": {
					"layout": {
						"colSpan": 4,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Tabfc48e9daTabLabelGridLayout998d9884"
					},
					"bindTo": "qrtAnotherPayer"
				},
				"parentName": "Tabfc48e9daTabLabelGridLayout998d9884",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtPayer373ef92a-3601-4508-95b5-6c25ce328abc",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 4,
						"row": 0,
						"layoutName": "Tabfc48e9daTabLabelGridLayout998d9884"
					},
					"bindTo": "qrtPayer",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tabfc48e9daTabLabelGridLayout998d9884",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "LOOKUPd0e8c9b3-27a1-4737-bcb2-a82ba1325e0a",
				"values": {
					"layout": {
						"colSpan": 9,
						"rowSpan": 1,
						"column": 15,
						"row": 0,
						"layoutName": "Tabfc48e9daTabLabelGridLayout998d9884"
					},
					"bindTo": "qrtHolderClientContract",
					"enabled": true,
					"contentType": 5
				},
				"parentName": "Tabfc48e9daTabLabelGridLayout998d9884",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "qrtSchema5Detail7ea4023f",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tabfc48e9daTabLabel",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "merge",
				"name": "qrtCurrencyRateEUR33d4bee2-9815-4635-a8cd-fbdeb96f613c",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Tabfc48e9daTabLabelGridLayoutbee6e453"
					},
					"labelConfig": {
						"caption": {
							"bindTo": "Resources.Strings.qrtCurrencyRateEUR33d4bee298154635a8cdfbdeb96f613cLabelCaption"
						}
					},
					"enabled": true
				}
			},
			{
				"operation": "merge",
				"name": "qrtCurrencyRateUSD2379d5dd-e7c4-459e-ae40-5eef9326887e",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 8,
						"row": 0,
						"layoutName": "Tabfc48e9daTabLabelGridLayoutbee6e453"
					},
					"labelConfig": {
						"caption": {
							"bindTo": "Resources.Strings.qrtCurrencyRateUSD2379d5dde7c4459eae405eef9326887eLabelCaption"
						}
					},
					"enabled": true
				}
			},
			{
				"operation": "insert",
				"name": "qrtCurrencyRated02b0bcd-8689-4363-8df1-a81c9ef7ef14",
				"values": {
					"layout": {
						"colSpan": 8,
						"rowSpan": 1,
						"column": 16,
						"row": 0,
						"layoutName": "Tabfc48e9daTabLabelGridLayoutbee6e453"
					},
					"bindTo": "qrtCurrencyRate"
				},
				"parentName": "Tabfc48e9daTabLabelGridLayoutbee6e453",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "merge",
				"name": "Tabc2918efaTabLabel",
				"values": {
					"order": 4
				}
			},
			{
				"operation": "insert",
				"name": "qrtCompetitorDealDetail67319f93",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tabc2918efaTabLabel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "qrtSchemaff4028e6Detail0fe3de31",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tabc2918efaTabLabel",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "qrtSchema95a37e5cDetailf0646aa3",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tabc2918efaTabLabel",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "Tabc2918efaTabLabelGroup83de7214",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tabc2918efaTabLabelGroup83de7214GroupCaption"
					},
					"itemType": 15,
					"markerValue": "added-group",
					"items": []
				},
				"parentName": "Tabc2918efaTabLabel",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "insert",
				"name": "Tabc2918efaTabLabelGridLayoutb7c56dd9",
				"values": {
					"itemType": 0,
					"items": []
				},
				"parentName": "Tabc2918efaTabLabelGroup83de7214",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "INTEGERb4ccab94-5ff2-45e0-830a-7d7a03e10045",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Tabc2918efaTabLabelGridLayoutb7c56dd9"
					},
					"bindTo": "qrtMinute",
					"enabled": false
				},
				"parentName": "Tabc2918efaTabLabelGridLayoutb7c56dd9",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "INTEGER53991e79-1423-418a-bacf-52403d6c5887",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 5,
						"row": 0,
						"layoutName": "Tabc2918efaTabLabelGridLayoutb7c56dd9"
					},
					"bindTo": "qrtHour",
					"enabled": false
				},
				"parentName": "Tabc2918efaTabLabelGridLayoutb7c56dd9",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "INTEGERadb84630-6829-465d-bf6c-80ac3552f979",
				"values": {
					"layout": {
						"colSpan": 5,
						"rowSpan": 1,
						"column": 10,
						"row": 0,
						"layoutName": "Tabc2918efaTabLabelGridLayoutb7c56dd9"
					},
					"bindTo": "qrtDay",
					"enabled": false
				},
				"parentName": "Tabc2918efaTabLabelGridLayoutb7c56dd9",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "qrtSchema14Detaila65f1cc1",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "Tabc2918efaTabLabel",
				"propertyName": "items",
				"index": 6
			},
			{
				"operation": "merge",
				"name": "NotesAndFilesTab",
				"values": {
					"order": 5
				}
			},
			{
				"operation": "merge",
				"name": "ESNTab",
				"values": {
					"order": 6
				}
			},
			{
				"operation": "remove",
				"name": "qrtCustomer942f2ef1-ad14-44ea-8ff4-7cfa905650c1"
			},
			{
				"operation": "remove",
				"name": "qrtOrderd3c326d0-a787-4d97-9288-307a8baaba4d"
			},
			{
				"operation": "remove",
				"name": "qrtServiceType27ebfc5b-b272-43a1-8821-303a3a62b6e3"
			},
			{
				"operation": "remove",
				"name": "qrtSourceRequest39a7e01d-af41-46c7-ad33-5d996abc4a6e"
			},
			{
				"operation": "remove",
				"name": "qrtSourceaaac70e0-5304-4919-bf02-1b3bc1ec1268"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGroupf84ce8ca"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGridLayoutd9c760fd"
			},
			{
				"operation": "remove",
				"name": "qrtSenderbe70fbc4-7d99-4e02-883c-919fcd8a358a"
			},
			{
				"operation": "remove",
				"name": "qrtConsignee30dcc034-5403-47c4-974e-51bfb213f051"
			},
			{
				"operation": "remove",
				"name": "qrtNatureCargof5b6470f-6f5f-4b4a-8baa-bbd2c9a388a0"
			},
			{
				"operation": "remove",
				"name": "qrtPatterncd793d6b-bf4c-4848-9501-2a3c52cb7937"
			},
			{
				"operation": "remove",
				"name": "qrtTSWe231b845-8b7d-431c-9bd8-3c8afc0c9193"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGroup72a8faea"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGridLayoutf8831f06"
			},
			{
				"operation": "remove",
				"name": "qrtIsInsurance420ad833-69bb-40ee-89aa-95b7ce34f0bd"
			},
			{
				"operation": "remove",
				"name": "qrtRoute3afb7dea-34ba-4b4d-be68-582497f3e5de"
			},
			{
				"operation": "remove",
				"name": "qrtCrossinga342bbbf-d3a0-42df-89a4-8b6af7bedfcb"
			},
			{
				"operation": "remove",
				"name": "qrtIsExportDeclarationd31812a0-378e-4014-9085-a352200d0001"
			},
			{
				"operation": "remove",
				"name": "qrtCargoReadinessDate7c1a0228-8114-47e6-8f13-a6e2ce8b22ee"
			},
			{
				"operation": "remove",
				"name": "qrtTransportationTypebb757bf3-2d5d-4835-9e93-d2de90687894"
			},
			{
				"operation": "remove",
				"name": "qrtDeliveryTimePland64e978d-98e1-4d5b-93f8-0e411ed55840"
			},
			{
				"operation": "remove",
				"name": "qrtCountryLoading52f9035a-7f9f-4743-a909-d853278ecc0e"
			},
			{
				"operation": "remove",
				"name": "qrtCityLoadingcc44a65b-b105-48a8-add9-6b798afed073"
			},
			{
				"operation": "remove",
				"name": "qrtAddressLoading980f0d01-a8ff-4ce2-a293-a8a4b011e62d"
			},
			{
				"operation": "remove",
				"name": "qrtContactLoading477aa3de-0e6d-4c7d-b079-ec30443218e0"
			},
			{
				"operation": "remove",
				"name": "qrtPhoneContactLoading9b074861-868a-41f1-a677-d0ec7b443947"
			},
			{
				"operation": "remove",
				"name": "qrtCountryUnloadinge62276a2-c1d5-46ea-9270-24f793d5fc1b"
			},
			{
				"operation": "remove",
				"name": "qrtCityUnloading4e512596-f16e-4870-9256-befa3b810264"
			},
			{
				"operation": "remove",
				"name": "qrtAddressUnloading5a26d49f-2c31-47c4-9e13-aec9bb626651"
			},
			{
				"operation": "remove",
				"name": "qrtContactUnloading08405d3e-826f-4176-9754-62faf589b7ac"
			},
			{
				"operation": "remove",
				"name": "qrtPhoneContactUnload2403f868-5113-4ed1-a86d-55cb45e6a3eb"
			},
			{
				"operation": "remove",
				"name": "qrtComments6ab8674b-86c8-452e-a9de-7cf3b3a35ad0"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGroup1233cf77"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGridLayoutb938c208"
			},
			{
				"operation": "remove",
				"name": "qrtPlaceFilingb98355f0-75b2-4275-8f8d-0db04c4be8d9"
			},
			{
				"operation": "remove",
				"name": "qrtTOCommentsfe8940e0-7ace-48fb-8828-07da8966a7d2"
			},
			{
				"operation": "remove",
				"name": "qrtSchema2Detail011ade85"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGroupa8f725ad"
			},
			{
				"operation": "remove",
				"name": "Tab8e87459eTabLabelGridLayoutdce159bb"
			},
			{
				"operation": "remove",
				"name": "qrtNotea00d0828-4255-484d-8f87-8b813f87240e"
			},
			{
				"operation": "remove",
				"name": "Tabcee09868TabLabel"
			},
			{
				"operation": "remove",
				"name": "Tabcee09868TabLabelGroup07e40894"
			},
			{
				"operation": "remove",
				"name": "Tabcee09868TabLabelGridLayout5c3034f6"
			},
			{
				"operation": "remove",
				"name": "qrtCargoTypea05cf584-4d6f-4bbe-8f1d-6231007ffb8d"
			},
			{
				"operation": "remove",
				"name": "qrtVehicleType71e46358-3b7d-4ede-945b-748ca934515b"
			},
			{
				"operation": "remove",
				"name": "qrtNumberVehiclesf6406c0d-2cdb-4d85-a1a9-f62a06e702b2"
			},
			{
				"operation": "remove",
				"name": "qrtCargoWeightec6170c8-f721-498f-8768-4722cf55be14"
			},
			{
				"operation": "remove",
				"name": "qrtIncoterms3d7adf27-b523-472e-80a6-523cfc9ff139"
			},
			{
				"operation": "remove",
				"name": "qrtCargoValueb2e79511-2c4a-4e4d-918f-07243d06aaea"
			},
			{
				"operation": "remove",
				"name": "qrtIsDangerous78520fec-149f-4d42-9db8-5468da7610be"
			},
			{
				"operation": "remove",
				"name": "qrtIsPerishable5c0ab9bd-e78d-4552-807a-66a8e4636edf"
			},
			{
				"operation": "remove",
				"name": "qrtLoad2a3dacf0-596b-42e8-af4b-58680a38ebab"
			},
			{
				"operation": "remove",
				"name": "qrtSchema3Detailec7ea69e"
			},
			{
				"operation": "remove",
				"name": "qrtSchema5Detaild554c9ae"
			},
			{
				"operation": "move",
				"name": "ActivityDetailV2109a3f17",
				"parentName": "Tabc2918efaTabLabel",
				"propertyName": "items",
				"index": 7
			}
		]/**SCHEMA_DIFF*/
		};
	});
