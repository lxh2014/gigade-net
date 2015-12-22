var conditionID = "";   //條件設定ID
editFunction = function (row, store) {
    VipGroupStore.load();
    if (row != null) {  //編輯時
        conditionID = row.data["condition_id"];
    } else if (row == null) {   //新增時
        conditionID = "";
        paymentStore.load();
    }

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/PromotionsAccumulateBonus/SavePromotionsAccumulateBonus',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'ID', 
                id: 'id',
                name: 'id',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: ACTIVENAME,
                name: 'name',
                id: 'name',
                allowBlank: false
            },
            {
                xtype: 'textarea',
                fieldLabel: ACTIVEDESC,
                name: 'event_desc',
                id: 'event_desc',
                allowBlank: false
            },
            {
                xtype: 'fieldset',
                defaults: {
                    labelWidth: 89,
                    anchor: '100%',
                    layout: {
                        type: 'hbox',
                        defaultMargins: { top: 50, right: 5, bottom: 0, left: 0 }
                    }
                },
                items: [
                    {
                        //會員群組
                        xtype: 'fieldcontainer',
                        fieldLabel: VIPGROUP,
                        combineErrors: true,
                        //margins: '0 200 0 0',
                        layout: 'hbox',
                        defaults: {
                            flex: 1,
                            hideLabel: true
                        },
                        items: [
                            {
                                xtype: 'radiofield',
                                name: 'us',
                                inputValue: "u_group",
                                id: "us1",
                                checked: true,
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
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
                                //                                allowBlank: false,
                                editable: false,
                                hidden: false,
                                id: 'group_name',
                                name: 'group_name',
                                hiddenName: 'group_name',
                                colName: 'group_name',
                                store: VipGroupStore,
                                displayField: 'group_name',
                                valueField: 'group_id',
                                typeAhead: true,
                                forceSelection: false,
                                lastQuery: '',
                                value: "0"
                                //                                emptyText: SELECT
                            }
                        ]
                    },
                    {
                        //會員條件
                        xtype: 'fieldcontainer',
                        fieldLabel: VIPCONDITION,
                        combineErrors: true,
                        layout: 'hbox',
                        margins: '0 200 0 0',
                        defaults: {
                            flex: 1,
                            //width: 120,
                            hideLabel: true
                        },
                        items: [
                            {
                                xtype: 'radiofield',
                                name: 'us',
                                inputValue: "u_groupset",
                                id: "us2",
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        var com_group = Ext.getCmp("group_name");
                                        var btn_group = Ext.getCmp("condi_set");
                                        if (newValue) {
                                            com_group.setValue("0");
                                            com_group.allowBlank = true;
                                            com_group.setValue("");
                                            com_group.isValid();
                                            btn_group.setDisabled(false);
                                            com_group.setDisabled(true);
                                            if (condition_id != "" && condition_id != 0) {
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
                                handler: onAddabClick
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
                    }
                ]
            },
            {
                xtype: 'fieldset',
                defaults: {
                    labelWidth: 89,
                    anchor: '100%',
                    bodyStyle: 'padding:5px 5px 0',
                    layout: {
                        type: 'hbox',
                        //defaultMargins: { top: 50, right: 5, bottom: 0, left: 0 }
                        padding: { top: 10, right: 10, bottom: 10, left: 10 }
                    }
                },
                items: [
                    {
                        //倍率
                        xtype: 'fieldcontainer',
                        fieldLabel: RATE,
                        combineErrors: true,
                        //margins: '0 200 0 0',
                        layout: 'hbox',
                        defaults: {
                            flex: 1,
                            hideLabel: true
                        },
                        items: [
                            {
                                xtype: 'radiofield',
                                name: 'bonusCount',
                                inputValue: "u_group",
                                id: "us3",
                                checked: true,
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        var rdo_group = Ext.getCmp("us3");
                                        var rdo_groppset = Ext.getCmp("us4");
                                        var txt_bonus = Ext.getCmp("bonus_rate");
                                        var txt_extraBonus = Ext.getCmp("extra_point");
                                        if (newValue) {
                                            txt_bonus.setDisabled(false);
                                            txt_extraBonus.setValue("");
                                            txt_extraBonus.setDisabled(true);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'numberfield', //倍率
                                name: 'bonus_rate',
                                id: 'bonus_rate',
                                minValue: 0,
                                allowDecimals:false ,
                                allowBlank: true
                            }
                        ]
                    },
                    {
                        //額外加贈購物金
                        xtype: 'fieldcontainer',
                        fieldLabel: EXTRABONUS,
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
                                name: 'bonusCount',
                                inputValue: "u_groupset",
                                id: "us4",
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        var rdo_group = Ext.getCmp("us3");
                                        var rdo_groppset = Ext.getCmp("us4");
                                        var txt_bonus = Ext.getCmp("bonus_rate");
                                        var txt_extraBonus = Ext.getCmp("extra_point");
                                        if (newValue) {
                                            txt_bonus.setDisabled(true);
                                            txt_extraBonus.setDisabled(false);
                                            txt_bonus.setValue("");
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'numberfield', //額外加贈購物金
                                name: 'extra_point',
                                id: 'extra_point',
                                minValue: 0,
                                disabled: true,
                                allowDecimals: false,
                                allowBlank: true
                            }
                        ]
                    }
                ]
            },
             {
                 xtype: 'combobox',
                 fieldLabel: PAYTYPE,
                 id: 'payment_code',
                 multiSelect: true, //支持多選
                 queryMode: 'local',
                 name: 'payment',
                 allowBlank: false,
                 editable: false,
                 typeAhead: true,
                 forceSelection: false,
                 store: paymentStore,
                 displayField: 'parameterName',
                 valueField: 'parameterCode',
                 emptyText: SELECT
             },
            {
                xtype: 'numberfield',
                fieldLabel: BONUSEXPIREDAY,
                name: 'bonus_expire_day',
                id: 'bonus_expire_day',
                allowDecimals: false,
                minValue: 0,
                allowBlank: false
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: TIMELIMIT,
                combineError: true,
                defaultType: 'radiofield',
                layout: 'hbox',
                defaults: {
                    flex: 1
                },
                items: [
                   {
                       boxLabel: NOTHING,
                       name: 'repeatPresent',
                       id: 'rdoNoTimesLimit',
                       inputValue: "0",
                       checked: true,
                       listeners: {
                           change: function (radio, newValue, oldValue) {
                               var txtPresentTimes = Ext.getCmp("present_time");
                               if (newValue) {
                                   txtPresentTimes.setValue(0);
                                   txtPresentTimes.setDisabled(true);
                                   //txtPresentTimes.allowBlank = false;
                               }
                           }
                       }
                   },
                   {
                       boxLabel: BYORDER,
                       name: 'repeatPresent',
                       id: 'rdoByOrder',
                       inputValue: "1",
                       listeners: {
                           change: function (radio, newValue, oldValue) {
                               var txtPresentTimes = Ext.getCmp("present_time");
                               if (newValue) {
                                   txtPresentTimes.setValue(0);
                                   txtPresentTimes.setDisabled(false);
                                   // txtPresentTimes.allowBlank = true;
                               }
                           }
                       }
                   },
                   {
                       boxLabel: BYMEMBER,
                       name: 'repeatPresent',
                       id: 'rdoByMember',
                       inputValue: "2",
                       listeners: {
                           change: function (radio, newValue, oldValue) {
                               var txtPresentTimes = Ext.getCmp("present_time");
                               if (newValue) {
                                   txtPresentTimes.setValue(0);
                                   txtPresentTimes.setDisabled(false);
                                   // txtPresentTimes.allowBlank = true;
                               }
                           }
                       }
                   }
                ]
            },
            {
                xtype: 'numberfield',
                fieldLabel: PRESENTTIMES,
                name: 'present_time',
                id: 'present_time',
                minValue: 0,
                value: 0,
                allowDecimals: false,
                disabled: true
                //,
                //labelAlign: 'right'
            },

             {
                 xtype: "datetimefield",
                 fieldLabel: BEGINTIME,
                 editable: false,
                 id: 'startTime',
                 name: 'startTime',
                 format: 'Y-m-d H:i:s',
                 width: 150,
                 allowBlank: false,
                 submitValue: true,
                 value: Tomorrow(),
                 time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                 listeners: {
                     select: function (a, b, c) {
                         var start = Ext.getCmp("startTime");
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
                          time: { hour: 23, min: 59, sec: 59 },
                          width: 150,
                          allowBlank: false,
                          submitValue: true, //
                          value: setNextMonth(Tomorrow(), 1),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("startTime");
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
                //                if ((Ext.getCmp("present_time").getValue() == "" || Ext.getCmp("present_time").getValue() == 0) && Ext.getCmp("rdoNoTimesLimit").getValue() == true) {
                //                    alert('贈送次數不能為空！');
                //                    return;
                //                }

                var selStartTime = Ext.getCmp("startTime").getValue();
                var selEndTime = Ext.getCmp("end").getValue();
                if (selEndTime <= selStartTime) {
                    alert("結束時間不能早于開始時間!");
                    Ext.getCmp("end").setValue("");
                    return;
                }
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            rowid: Ext.htmlEncode(Ext.getCmp('id').getValue()),
                            name: Ext.getCmp('name').getValue(),
                            event_desc: Ext.getCmp('event_desc'),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                            bonus_expire_day: Ext.htmlEncode(Ext.getCmp('bonus_expire_day').getValue()),
                            payment: Ext.htmlEncode(Ext.getCmp('payment_code').getValue()),
                            condition_id: condition_id
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                PromotionsAccumulateBonusStore.load();
                                editWin.close();
                            } else {
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
        title: ADDBONUSACTIVITYMANAGEMENT,
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 400,
        y: 0,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
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
                    // editFrm.getForm().loadRecord(row); //如果是添加的話
                    editFrm.getForm().reset();
                } else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                    var _grop = row.data.group_name;

                    if (row.data.condition_id != 0) { //針對會員選定條件
                        Ext.getCmp('us1').setValue(false);
                        Ext.getCmp('us2').setValue(true);
                    } else {
                        Ext.getCmp('us1').setValue(true);
                        Ext.getCmp('us2').setValue(false);
                        Ext.getCmp('userInfo').hide();
                        if (row.data.group_name == "") {
                            Ext.getCmp('group_name').setValue(BUFEN);
                        }
                    }

                    var _br = row.data.bonus_rate; //倍率與額外加贈購物金
                    if (_br == null || _br == "0" || _br == "") {
                        Ext.getCmp('us3').setValue(false);
                        Ext.getCmp('us4').setValue(true);
                    } else {
                        Ext.getCmp('us3').setValue(true);
                        Ext.getCmp('us4').setValue(false);
                    }

                    var _rpt = row.data.repeat; //針對贈送次數限制
                    if (_rpt == "0") {
                        Ext.getCmp('rdoNoTimesLimit').setValue(true);
                        //Ext.getCmp('present_time').setValue(true);
                        Ext.getCmp('rdoByOrder').setValue(false);
                        Ext.getCmp('rdoByMember').setValue(false);
                    } else if (_rpt == "1") {
                        Ext.getCmp('rdoNoTimesLimit').setValue(false);
                        // Ext.getCmp('present_time').setValue(false);
                        Ext.getCmp('rdoByOrder').setValue(true);
                        Ext.getCmp('rdoByMember').setValue(false);
                        Ext.getCmp("present_time").setValue(row.data.present_time);
                    } else if (_rpt == "2") {
                        Ext.getCmp('rdoNoTimesLimit').setValue(false);
                        Ext.getCmp('rdoByOrder').setValue(false);
                        Ext.getCmp('rdoByMember').setValue(true);
                        Ext.getCmp('present_time').setValue(row.data.present_time);
                    }


                    paymentStore.load({
                        callback: function () {
                            var paymentids = row.data.payment_code.toString().split(',');
                            var arrTemp = new Array();
                            for (var i = 0; i < paymentids.length; i++) {
                                arrTemp.push(paymentStore.getAt(paymentStore.find("parameterCode", paymentids[i])));
                            }
                            Ext.getCmp('payment_code').setValue(arrTemp);

                        }
                    })

                    //修改時對payment賦值
                    //var paymentids = row.data.payment_code.toString().split(',');
                    //var paymentcombobox = Ext.getCmp('payment_code');
                    //var paymentstore = paymentcombobox.store;
                    //var paymentarrTemp = new Array();
                    //for (var i = 0; i < paymentids.length; i++) {
                    //    paymentarrTemp.push(paymentstore.getAt(paymentstore.find("parameterCode", paymentids[i])));
                    //}
                    //paymentcombobox.setValue(paymentarrTemp);


                }
            }
        }
    });
    editWin.show();
}

onAddabClick = function () {
    showUserSetForm(null, conditionID, "userInfo");
}
