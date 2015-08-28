

editFunction = function (row, store) {
    var conditionID = "";
    VipGroupStore.load();
    if (row != null) {
        conditionID = row.data.condition_id;
    }
    else {
        paymentStore.load();
    }

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true, 
        autoScroll: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/PromotionsAccumulateRate/Save',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: ACTIVENAME,
            name: 'name',
            id: 'name',
            allowBlank: false
        },
        {
            xtype: 'textfield',
            fieldLabel: 'Id',
            id: 'id',
            name: 'id',
            hidden: true
        },
        {
            xtype: 'fieldcontainer',
            fieldLabel: VIPGROUP,
            combineErrors: true,
            margins: '0 200 0 0',
            layout: 'hbox',
            defaults: {
                flex: 1,
                hideLabel: true
            },
            items: [
                {
                    xtype: 'radiofield',
                    name: 'us',
                    inputValue: "1",
                    id: "us1",
                    checked: true,
                    listeners: {
                        change: function (radio, newValue, oldValue) {
                            var rdo_group = Ext.getCmp("us1");
                            var rdo_groppset = Ext.getCmp("us2");
                            var com_group = Ext.getCmp("group_name");
                            var btn_group = Ext.getCmp("condi_set");
                            if (newValue) {
                                btn_group.setDisabled(true);
                                com_group.setValue("0");
                                com_group.setDisabled(false);
                                com_group.allowBlank = false;
                                Ext.getCmp("userInfo").hide();
                            }
                        }
                    }
                },
                {
                    xtype: 'combobox', //會員群組
                    editable: false, //是否能夠編輯,默認為false
                    hidden: false,
                    id: 'group_name',
                    name: 'group_name',
                    hiddenName: 'group_name',
                    store: VipGroupStore,
                    displayField: 'group_name',
                    valueField: 'group_id',
                    typeAhead: true,
                    lastQuery: '',
                    forceSelection: false, //默認是false
                    value: "0"
                }
            ]
        },
        {
            xtype: 'fieldcontainer',
            fieldLabel: VIPCONDITION,
            combineErrors: true,
            layout: 'hbox',
            margins: '0 200 0 0',
            defaults: {
                flex: 1,
                width: 120,
                hideLabel: true
            },
            items: [
                {
                    xtype: 'radiofield',
                    name: 'us',
                    inputValue: "2",
                    id: "us2",
                    listeners: {
                        change: function (radio, newValue, oldValue) {
                            var rdo_group = Ext.getCmp("us1");
                            var rdo_groppset = Ext.getCmp("us2");
                            var com_group = Ext.getCmp("group_name");
                            var btn_group = Ext.getCmp("condi_set");
                            if (newValue) {
                                //                                com_group.setValue("0");
                                com_group.allowBlank = true;
                                com_group.setValue("");
                                com_group.isValid();

                                btn_group.setDisabled(false);
                                com_group.setDisabled(true);
                                if (condition_id != "") {
                                    Ext.getCmp("userInfo").show();
                                }
                            }
                        }
                    }
                },
                {
                    xtype: 'button',
                    text: CONDINTION,
                    disabled: true,
                    width: 120,
                    id: 'condi_set',
                    colName: 'condi_set',
                    name: 'condi_set',
                    handler: function () {
                        if (conditionID != "") {
                            showUserSetForm(null, conditionID, "userInfo");
                        } else {
                            showUserSetForm(null, condition_id, "userInfo");
                        }
                    }
                }
            ]
        },
        {
            xtype: 'textareafield',
            name: 'textarea1',
            id: 'userInfo',
            anchor: '100%',
            hidden: true,
            value: ShowConditionData(conditionID, "userInfo"),
            listeners: {
                change: function (textarea, newValue, oldValue) {
                    var textArea = Ext.getCmp("userInfo");
                    if (newValue != "" && oldValue != "") {
                        textArea.show();
                    }
                }
            }
        },
        {
            xtype: 'numberfield',
            fieldLabel: ALLMONEY,
            name: 'amount',
            id: 'amount',
            allowBlank: false,
            minValue: 1
        },
        {
            xtype: 'fieldcontainer',
            fieldLabel: DOLLARPOINT,
            combineErrors: true,
            layout: 'hbox',
            defaults: {
                flex: 1,
                hideLabel: true
            },
            items: [
                {
                    xtype: "numberfield",
                    id: 'dollar',
                    name: 'dollar',
                    allowBlank: false,
                    minValue: 1
                },
                {
                    xtype: 'displayfield',
                    value: FENGE,
                    margin: '0'
                },
                {
                    xtype: "numberfield",
                    id: 'point',
                    name: 'point',

                    allowBlank: false,
                    minValue: 1
                }
            ]
        },
        {
            xtype: 'radiogroup',
            id: 'bonus_type',
            fieldLabel: BOUNSTYPE,
            colName: 'bonus_type',
            width: 200,
            defaults: {
                name: 'Tax_Type'
            },
            columns: 1,
            vertical: true,
            items: [
                { boxLabel: BONUS_ONE, inputValue: '1', checked: true }
            ]
        },
        {
            xtype: "combobox",
            allowBlank: false,
            editable: false,
            hidden: false,
            id: 'payment_type_rid',
            name: 'payment_type_rid',
            store: paymentStore,
            fieldLabel: PAYTYPE,
            displayField: 'parameterName',
            valueField: 'parameterCode',
            multiSelect: true,
            emptyText: SELECT,
            typeAhead: true,
            queryMode: 'local',
            forceSelection: false //默認是false           

        },
        {
            xtype: "datetimefield",
            fieldLabel: BEGINTIME,
            editable: false,
            id: 'newstart',
            name: 'newstart',
            format: 'Y-m-d H:i:s',
            width: 150,
            allowBlank: false,
            submitValue: true,
            value: Tomorrow(),
            time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("newstart");
                    var end = Ext.getCmp("end");
                    if (end.getValue() == null) {
                        end.setValue(setNextMonth(start.getValue(), 1));
                    }
                    else if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                        start.setValue(setNextMonth(end.getValue(), -1));
                    }
                }
            }
        },
                      {
                          xtype: "datetimefield",
                          fieldLabel: ENDTIME,
                          editable: false,
                          id: 'end',
                          name: 'end',
                          format: 'Y-m-d H:i:s',
                          width: 150,
                          allowBlank: false,
                          submitValue: true, //
                          value: setNextMonth(Tomorrow(), 1),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("newstart");
                                  var end = Ext.getCmp("end");
                                  if (start.getValue() != "" && start.getValue() != null) {
                                      if (end.getValue() < start.getValue()) {
                                          Ext.Msg.alert(INFORMATION, DATA_TIP);
                                          end.setValue(setNextMonth(start.getValue(), 1));
                                      }

                                  }
                                  else {
                                      start.setValue(setNextMonth(end.getValue(), -1));
                                  }
                              }
                          }
                      }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
               
                var form = this.up('form').getForm();
                if ((condition_id == "" || condition_id == "0") && (Ext.getCmp("group_name").getValue() == null || Ext.getCmp("group_name").getValue() == "")) {
                    Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                    return;
                }
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            //裡面的數據錯誤。會導致無法顯示各種數據
                            rowid: Ext.htmlEncode(Ext.getCmp('id').getValue()),
                            name: Ext.getCmp('name').getValue(),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                            amount: Ext.htmlEncode(Ext.getCmp('amount').getValue()),
                            bonus_type: Ext.htmlEncode(Ext.getCmp("bonus_type").getValue().Tax_Type),
                            payment_id: Ext.htmlEncode(Ext.getCmp('payment_type_rid').getValue()),
                            dollar: Ext.htmlEncode(Ext.getCmp('dollar').getValue()),
                            newstart: Ext.htmlEncode(Ext.getCmp('newstart').getValue()),
                            end: Ext.htmlEncode(Ext.getCmp('end').getValue()),
                            points: Ext.htmlEncode(Ext.getCmp('point').getValue()),
                            us2: Ext.htmlEncode(Ext.getCmp("us2").getValue().us),
                            condition_id: condition_id
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                FaresStore.load();
                                editWin.close();
                            }
                            else {
                                alert(ERRORSHOW + result.success);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]

    });
    var editWin = Ext.create('Ext.window.Window', {
        title: DSLJ,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        y: 200,
        closeAction: 'destroy',
        modal: true,
        constrain: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話


                }
                else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話



                    if (row.data.condition_id != 0) {
                        Ext.getCmp('us1').setValue(false);
                        Ext.getCmp('us2').setValue(true);
                    }
                    else {
                        Ext.getCmp('us1').setValue(true);
                        Ext.getCmp('us2').setValue(false);
                        Ext.getCmp('userInfo').hide();
                        if (row.data.group_name == "") {
                            Ext.getCmp('group_name').setValue(BUFEN);

                        }

                    }
                    paymentStore.load({
                        callback: function () {
                            var paymentIDs = row.data.payment_type_rid.toString().split(',');
                            var arrTemp = new Array();
                            for (var i = 0; i < paymentIDs.length; i++) {
                                arrTemp.push(paymentStore.getAt(paymentStore.find("parameterCode", paymentIDs[i])));
                            }
                            Ext.getCmp('payment_type_rid').setValue(arrTemp);

                        }
                    })


                    //var paymentIDs = row.data.payment_type_rid.toString().split(',');
                    //var paymentcombobox = Ext.getCmp('payment_type_rid');
                    //var paymentstore = paymentcombobox.store;
                    //var paymentarrTemp = new Array();
                    //var length = paymentstore.count();
                    //for (var i = 0; i < paymentIDs.length; i++) {
                    //    paymentarrTemp.push(paymentstore.getAt(paymentstore.find("parameterCode", paymentIDs[i])));
                    //}
                    //paymentcombobox.setValue(paymentarrTemp);
                }

            }
        }
    });
    editWin.show();


}
