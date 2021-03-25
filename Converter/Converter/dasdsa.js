define("qrtInvoice1Section", ["MenuUtilities", "ProcessModuleUtilities", "PrintReportUtilities"],
	function(MenuUtilities, ProcessModuleUtilities) {
	return {
		entitySchemaName: "qrtInvoice",
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		mixins: {
			PrintReportUtilities : "Terrasoft.PrintReportUtilities"
		},
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "qrtPrintButton",
				"values": {
					"itemType": Terrasoft.ViewItemType.BUTTON,
					"caption": {
						"bindTo": "Resources.Strings.PrintButtonCaption"
					},
					"click": {
						"bindTo": "onClickPrintButton"
					},
					"style": "default",
					"layout": {
						"column": 1,
						"row": 6,
						"colSpan": 1
					}
				},
				"parentName": "CombinedModeActionButtonsCardRightContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "EmailButton",
				"values": {
					itemType: Terrasoft.ViewItemType.BUTTON,
					caption: {bindTo:
					"Resources.Strings.EmailButtonCaption"},
					click: {bindTo: "onEmailButtonClick"},
					"style": Terrasoft.controls.ButtonEnums.style.BLUE,
					"layout": {
						"column": 0,
						"row": 0,
						"colSpan": 1,
						"rowSpan": 1
					}
				},
				"parentName": "CombinedModeActionButtonsCardLeftContainer",
				"propertyName": "items",
			}
		]/**SCHEMA_DIFF*/,
		methods: {
			getActions: function() {//расширяем метод для коллекции кнопок действий
				var actionMenuItems = this.callParent(arguments);
				actionMenuItems.addItem(this.getButtonMenuItem({ //добавляем разделитель
					Type: "Terrasoft.MenuSeparator",
					Caption: ""
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({ //добавляем кнопку
					"Caption": "Сформировать услуги для передачи в учетную систему", //текст кнопки
					"Tag": "CreateServicesForTransfer", //имя метода, который запустит процесс, см. ниже
					"Enabled": true //делать кнопку неактивной, если запись еще не создана
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Caption": "Обновить фактические суммы в Заявках",
					"Tag": "0a59591a-94a7-4e4c-a594-d4e553af9604",
					"Click": { "bindTo": "updateSumInOrder" },
					"Enabled": {"bindTo": "canEntityBeOperated"}
				}));
				return actionMenuItems;
			},
			updateSumInOrder: function(arg){
				var recordId = this.get("ActiveRow");
				if (recordId){
						
					var processArgs = {
						sysProcessName: "qrtProcess_e6fefc4",// название бизнесс процесса
						parameters: {
							ProcessInvoice: recordId, //название параметра в БП
						}
					};
					ProcessModuleUtilities.executeProcess(processArgs);
					this.showInformationDialog("Данные в Заявках обновлены");
				}
			},
			onEmailButtonClick: function() {
				var invoiceId = this.get("ActiveRow");
				var title = "";
				var body = "";
				
				var customer = null;
				var supplier = null;
				var service = null;
				var isPrintInvoice = false;
				var isPrintAct = false;
				var isPrintPackage = false;
				var nameInvoice = "";
				var orderName = "";
				var actNumber = "";
				var invoiceNumber = "";
				var route = "";
				var kgFact = 0;
				var seatsFact = 0;
				var order = null;
				var contractName = "";
				var datePlan = null;
				var paymentStatus = null;
				var serviceDescription = "";
				var type = null;
                var paymentCurrency = null;
              	var InvoiceBool = false;
              	var actReport = false;
              	
              	message = "";
              	messageId = "";
              	
				if (invoiceId) {
					Terrasoft.chain(
					function (next) {
						var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
							rootSchemaName: "qrtInvoice"
						});
						esq.addColumn("qrtVendorAccountNumber");//Номер счета
						esq.addColumn("qrtActNumber");//№ акта
						esq.addColumn("qrtInvoiceNumber");//№ счет-фактуры
						esq.addColumn("qrtType");//Тип счета
                      	
                      	esq.addColumn("qrtActNumber");//Номер акта 1С
                      	esq.addColumn("qrtActDate");//Дата акта
                      	esq.addColumn("qrtPrimaryActAmount");//Сумма акта без НДС б.в
                      	
                      	esq.addColumn("qrtInvoiceNumber");//№ счет-фактуры 1С
                      	esq.addColumn("qrtInvoiceDate");//Дата счет фактуры
                      
                      	esq.addColumn("qrtContract.qrtPaymentCurrency","PaymentCurrency");//Валюта платежа
						esq.addColumn("qrtDueDatePlan");//Дата оплаты план
						esq.addColumn("qrtPaymentStatus");//Состояние оплаты
						esq.addColumn("qrtCustomer");//Заказчик
						esq.addColumn("qrtSupplier");//Исполнитель
						esq.addColumn("qrtOrder");//Заявка
						esq.addColumn("qrtOrder.qrtService", "Service");//Сервис
						esq.addColumn("qrtOrder.qrtName", "OrderName");//Номер
						esq.addColumn("qrtOrder.qrtRoute", "qrtRoute");//Маршрут
						esq.addColumn("qrtOrder.qrtService.qrtServiceDescription", "ServiceDescription");//Маршрут
						esq.addColumn("qrtOrder.qrtGrossWeightKgTotalFact", "qrtGrossWeightKgTotalFact");//Вес факт
						esq.addColumn("qrtOrder.qrtNumberOfSeatsTotalFact", "qrtNumberOfSeatsTotalFact");//Кол мест факт
						
                      	esq.addColumn("qrtVendorAccountNumber");//№ счета поставщика
						esq.addColumn("qrtIsPrintInvoice");//Печать счета
						esq.addColumn("qrtIsPrintAct");//Печать акта
						esq.addColumn("qrtIsPrintPackage");//Печать пакета
						esq.addColumn("qrtInvoiceBool");//Инвойс
						esq.addColumn("qrtActReport");//Отчет комитенту
                      
						esq.getEntity(invoiceId, function(result) {
							if (!result.success) {
								// обработка/логирование ошибки, например
								this.showInformationDialog("Ошибка запроса данных");
								return;
							}
							customer = result.entity.get("qrtCustomer");
							supplier = result.entity.get("qrtSupplier");
							service = result.entity.get("Service");
							type = result.entity.get("qrtType");
                          	paymentCurrency = result.entity.get("PaymentCurrency");
                          
							isPrintInvoice = result.entity.get("qrtIsPrintInvoice");
                          	if (isPrintInvoice){
                              //TODO...
                              var vendor = result.entity.get("qrtVendorAccountNumber");
                              if (!vendor){
                                this.showInformationDialog("Выполните отправку счета в 1С и далее повторите действие для отправки счета клиенту");
                                return;
                              }
                            }
							isPrintAct = result.entity.get("qrtIsPrintAct");
                            if (isPrintAct){
                              var actNumber2 = result.entity.get("qrtActNumber");
                              var actDate = result.entity.get("qrtActDate");
                              var actAmount = result.entity.get("qrtPrimaryActAmount");
                              if (!actNumber2 || !actDate || (!actAmount || actAmount === 0)){
                                this.showInformationDialog("Не достаточно данных в счете для формирования выделенных документов. Заполните данные и повторите операцию.");
                                return;
                              }
                            }
							isPrintPackage = result.entity.get("qrtIsPrintPackage");
                            if (isPrintPackage){
                              var printNumber = result.entity.get("qrtInvoiceNumber");
                              var printDate = result.entity.get("qrtActDate");
                              if (!printNumber || !printDate){
                                this.showInformationDialog("Не достаточно данных в счете для формирования выделенных документов. Заполните данные и повторите операцию.");
                                return;
                              }
                            }
							InvoiceBool = result.entity.get("qrtInvoiceBool");
                          	actReport = result.entity.get("qrtActReport");
                          	
							nameInvoice = result.entity.get("qrtVendorAccountNumber");
							orderName = result.entity.get("OrderName");
							actNumber = result.entity.get("qrtActNumber");
							invoiceNumber = result.entity.get("qrtInvoiceNumber");
							route = result.entity.get("qrtRoute");
							kgFact = result.entity.get("qrtGrossWeightKgTotalFact");
							seatsFact = result.entity.get("qrtNumberOfSeatsTotalFact");
							order = result.entity.get("qrtOrder");
							datePlan = result.entity.get("qrtDueDatePlan");
							if (!datePlan){
								datePlan = new Date();
							}
							paymentStatus = result.entity.get("qrtPaymentStatus");
							serviceDescription = result.entity.get("ServiceDescription");
                          
							var isActive = [isPrintInvoice, isPrintAct, isPrintPackage, InvoiceBool, actReport];
                            var activeCount = isActive.filter(active => active === true);
                            if (activeCount.length === 0){
                                var controlConfig = {};
                                Terrasoft.utils.inputBox("Вы не выбрали ни один документ для вложения. \nПродолжить без вложении?",
                                    function(buttonCode, controlData) {
                                        if (buttonCode === "yes") {
                                            //Что-то делаем
                                            next();
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
                                    //["ok", "cancel"],
                                    this,
                                    controlConfig,
                                    {defaultButton: 0}
                                );
                            } else {
                              next();
                            }
						}, this);
					}, 
					function (next) {
						var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {rootSchemaName: "qrtRequestContract"});
						esq.addColumn("qrtContract.qrtName", "ContractName");
						if (order){
							esq.filters.add("filter01",
								this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
									"qrtOrder", order.value));
						}
						//С клиентом/Покупателем
						esq.filters.add("filter02",
								this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
									"qrtContract.qrtCategory", "9e085bd0-b7a3-46dc-a76f-ed7f9b3dc9fe"));
						esq.getEntityCollection(function (result) {
							if (result.success) {
								if (result.collection.collection.length > 0){
									contractName = result.collection.collection.items[0].get("ContractName");
								}
							}
							next();
						}, this);
					},
					function (next) {
                      var isActive = [isPrintInvoice, isPrintAct, isPrintPackage, InvoiceBool, actReport];
                      var activeCount = isActive.filter(active => active === true);
						//33607268-f494-497d-993a-016e0670ae85 - CCL
						if (service && service.value !== "33607268-f494-497d-993a-016e0670ae85"){
							if (isPrintInvoice === true && activeCount.length === 1){
								title = supplier.displayValue + ". Счет на оплату №" + nameInvoice + "  по Заявке №" + orderName;
								body = "<p>Добрый день!<br>Направляем Вам Счет по Заявке №" + orderName +
								".<br>Параметры Заявки:<br>- " + serviceDescription +
								"<br>- Маршрут: " + route + 
								"<br>- Вес: " + kgFact + 
								"<br>- Кол-во мест: " + seatsFact +
								"<br>Просим осуществить оплату согласно условиям Договора №" + contractName + 
                                "<br>Срок оплаты - " + datePlan.toLocaleDateString("ru-RU") + 
								"<br>Если возникнут вопросы, пожалуйста, свяжитесь со мной.<br></p>";
							} else if (!isPrintInvoice && activeCount.length === 0){
								this.showInformationDialog(
									"Установите индикатор для формирования вложения и текста в письма и выполните действие снова");
							} else {
                                if(InvoiceBool){
                                    title = ((supplier.displayValue.includes("АБИПА")) ? "ABIPA": "ABIPA KASTOMS") + `, Invoice №${this.get("qrtName")}, order №${orderName}`;
                                    body = "Dear Sirs,<br>In the attached file you can find payment documents for Order №" + orderName +
                                    ".<br>Order details:" +
                                    "<br>- Route: " + route +
                                    "<br>- Weight: " + kgFact +
                                    "<br>- Nr of pieces: " + seatsFact +
                                    "<br>Please, make the payment according to payment terms " + contractName +
                                    "<br>Agreement - " + datePlan.toLocaleDateString("eu-EU") +
                                    "<br>If you have any questions, please do not hesitate to contact me.<br></p>";
                                }else{
                                    title = supplier.displayValue + ".Закрывающие документы по Заявке №" + orderName + " (ТО)";
                                    body = "Добрый день!<br>Направляем Вам закрывающие документы по Заявке №" + orderName +" 										на таможенное оформление." +
                                    ".<br>Параметры Заявки:" +
                                    "<br>- Маршрут: " + route +
                                    "<br>- Вес: " + kgFact +
                                    "<br>- Кол-во мест: " + seatsFact +
                                    "<br>Просим осуществить оплату согласно условиям Договора №" + contractName +
                                    "<br>Срок оплаты - " + datePlan.toLocaleDateString("ru-RU") +
                                    "<br>Если возникнут вопросы, пожалуйста, свяжитесь со мной.<br></p>";
                                }
                            }
						} else {
							if (isPrintInvoice === true && activeCount.length === 1){
								title = supplier.displayValue + ". Счет на оплату №" + nameInvoice + "  по Заявке №" + orderName + " (ТО)";
								body = "<p>Добрый день!<br>Направляем Вам Счет по Заявке №" + orderName + " на таможенное формление." +
								".<br>Параметры Заявки:" +
								"<br>- Маршрут: " + route + 
								"<br>- Вес: " + kgFact + 
								"<br>Просим осуществить оплату согласно условиям Договора №" + contractName + 
                                "<br>Срок оплаты - " + datePlan.toLocaleDateString("ru-RU") + 
								"<br>Если возникнут вопросы, пожалуйста, свяжитесь со мной.<br></p>";
							} else if (!isPrintInvoice && activeCount.length >= 1){
                                if(InvoiceBool){
                                    title = ((supplier.displayValue.includes("АБИПА")) ? "ABIPA": "ABIPA KASTOMS") + `, Invoice №${this.get("qrtName")}, order №${orderName}`;
                                    body = "Dear Sirs,<br>In the attached file you can find payment documents for Order №" + orderName +
                                    ".<br>Order details:" +
                                    "<br>- Route: " + route +
                                    "<br>- Weight: " + kgFact +
                                    "<br>- Nr of pieces: " + seatsFact +
                                    "<br>Please, make the payment according to payment terms " + contractName +
                                    "<br>Agreement - " + datePlan.toLocaleDateString("eu-EU") +
                                    "<br>If you have any questions, please do not hesitate to contact me.<br></p>";
                                }else{
                                    title = supplier.displayValue + ".Закрывающие документы по Заявке №" + orderName + " (ТО)";
                                    body = "Добрый день!<br>Направляем Вам закрывающие документы по Заявке №" + orderName +" 										на таможенное оформление." +
                                    ".<br>Параметры Заявки:" +
                                    "<br>- Маршрут: " + route +
                                    "<br>- Вес: " + kgFact +
                                    "<br>- Кол-во мест: " + seatsFact +
                                    "<br>Просим осуществить оплату согласно условиям Договора №" + contractName +
                                    "<br>Срок оплаты - " + datePlan.toLocaleDateString("ru-RU") +
                                    "<br>Если возникнут вопросы, пожалуйста, свяжитесь со мной.<br></p>";
                                }
							} else {
								this.showInformationDialog(
									"Установите индикатор для формирования вложения и текста в письма и выполните действие снова");
							}
						}
						next();
					},
                      function (next) {
						//TODO
                        if (type){
                          type = type.value;
                        }
                        if (paymentCurrency){
                          paymentCurrency = paymentCurrency.value;
                        }
                        var ourCompany;
                        //Исходящий
                        if (type === "808d6728-5dd7-4c06-a843-f8beec0e4ade"){
                          ourCompany = supplier;//Исполнитель
                        } else {
                          ourCompany = customer;//Заказчик
                        }
                        if (ourCompany){
                          ourCompany = ourCompany.value;
                        }
                        var IsName = [];
                        var message2 = [];
                        //Печать счета
                        if (isPrintInvoice == true){
                          IsName.push("qrtIsPrintInvoice");
                          message2.push("Печать счета");
                        }
                        //Печать акта
                        if (isPrintAct == true){
                          IsName.push("qrtIsPrintAct");
                          message2.push("Печать акта");
                        }
                        //Печать СФ
                        if (isPrintPackage == true){
                          IsName.push("qrtIsPrintPackage");
                          message2.push("Печать СФ");
                        }
                        //Инвойс
                        if (InvoiceBool == true){
                          IsName.push("qrtInvoiceBool");
                          message2.push("Инвойс");
                        }
                        //Отчет комитенту
                        if (actReport == true){
                          IsName.push("qrtActReport");
                          message2.push("Отчет комитенту");
                        }
                        
                        for (var i = 0; i < IsName.length; i++){
                          var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
                            rootSchemaName:"qrtInvoicePrintForm"
                          });
                          esq.addColumn("qrtPrintForm");
                          esq.addColumn("Name");
                          esq.filters.add("filter01",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtAction", "a24f516a-309b-4cfb-80aa-b56ea201344b"));
                          esq.filters.add("filter02",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtInvoiceType", type));
                          esq.filters.add("filter03",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtOurCompany", ourCompany));
                          esq.filters.add("filter04",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtCurrencyPay", paymentCurrency));
                          
                          if (IsName[i] === "qrtIsPrintInvoice"){
                            esq.filters.add("filter05",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtIsPrintInvoice", true));
                          }
                          if (IsName[i] === "qrtIsPrintAct"){
                            esq.filters.add("filter06",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtIsPrintAct", true));
                          }
                          if (IsName[i] === "qrtIsPrintPackage"){
                            esq.filters.add("filter07",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtIsPrintPackage", true));
                          }
                          if (IsName[i] === "qrtInvoiceBool"){
                            esq.filters.add("filter08",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtIsInvoice", true));
                          }
                          if (IsName[i] === "qrtActReport"){
                            esq.filters.add("filter09",
                              this.Terrasoft.createColumnFilterWithParameter(this.Terrasoft.ComparisonType.EQUAL,
                                  "qrtActReport", true));
                          }
                          esq.getEntityCollection(function (result) {
                              if (!result.success) {
                                  // обработка/логирование ошибки, например
                                  this.showInformationDialog("Ошибка запроса данных:\n" + "\nErrorCode: " + result.errorInfo.errorCode + "\nMessage: " + result.errorInfo.message);
                                  return;
                              }
                          	  if (result.collection.collection.length > 0){
                                  next();
                              } else {
                                var controlConfig = {};
                                Terrasoft.utils.inputBox("Для следующих документов(" + message2.toString() + ") печатная форма не найдена. \nПродолжить?",
                                    function(buttonCode, controlData) {
                                        if (buttonCode === "yes") {
                                            //Что-то делаем
                                            next();
                                        } else {
                                            window.console.log("no");
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
                                    //["ok", "cancel"],
                                    this,
                                    controlConfig,
                                    {defaultButton: 0}
                                );
                              }
                              
                          }, this);
                        }
                        
					},
					function (next) {
						var args = {
							sysProcessName: "qrtInvoiceSendEmail",
							parameters: {
								invoiceId: invoiceId,
								ProcessBody: body,
								ProcessTitle: title,
								
							}
						};
						ProcessModuleUtilities.executeProcess(args);
					}, this);
				}
			},
			getCardPrintButtonVisible: function() {
				return false;
				var cardPrintFormsCollection = this.get(this.moduleCardPrintFormsCollectionName);
				var result = MenuUtilities.getMenuVisible(cardPrintFormsCollection, this);
				this.set("IsCardPrintButtonVisible", result);
				return result;
			},
			onClickPrintButton: function() {
				this.sandbox.publish("saveAndRunPrintProc", "saveAndRunPrintProc", ["saveAndRunPrintProc"]);
			}
		},
		messages: {
			"saveAndRunPrintProc": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.PUBLISH
			}
		}
	};
});
