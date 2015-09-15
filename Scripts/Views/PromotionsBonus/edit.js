

editFunction = function (row, store) {
    var conditionID = "";
    if (row != null) {
        conditionID = row.data["condition_id"];
    }
    VipGroupStore.load();
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/PromotionsBonus/Save',
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
                        inputValue: "u_group",
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
                        //                        allowBlank: true,
                        editable: false,
                        hidden: false,
                        id: 'group_name',
                        name: 'group_name',
                        hiddenName: 'group_name',
                        colName: 'group_id',
                        store: VipGroupStore,
                        displayField: 'group_name',
                        valueField: 'group_id',
                        typeAhead: true,
                        forceSelection: false,
                        lastQuery: '',
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
                        inputValue: "u_groupset",
                        id: "us2",
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var rdo_group = Ext.getCmp("us1");
                                var rdo_groppset = Ext.getCmp("us2");
                                var com_group = Ext.getCmp("group_name");
                                var btn_group = Ext.getCmp("condi_set");
                                if (newValue) {
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
                fieldLabel: BONUSMONEY,
                name: 'amount',
                id: 'amount',
                allowBlank: false,
                minValue: 0
            },
            {
                xtype: 'radiogroup',
                fieldLabel: BOUNSTYPE,
                id: 'bonus_type',
                colName: 'bonus_type',
                width: 200,
                defaults: {
                    name: 'Bonus_Type'
                },
                columns: 2,
                vertical: true,
                items: [
                    { id: 'bt1', boxLabel: BONUS_ONE, name: 'bonus_type', inputValue: '1' },
                    { id: 'bt2', boxLabel: BONUS_TWO, name: 'bonus_type', inputValue: '2', checked: true }
                ],
                listeners: {
                    change: function (a, b, c) {
                        if (b.bonus_type == "1") {
                            Ext.getCmp("days").setValue("90");
                        } else {
                            Ext.getCmp("days").setValue("30");
                        }
                    }
                }
            },
            {
                xtype: 'radiogroup',
                fieldLabel: FUHAOCHONGFUSHIYONG,
                id: 'hmcfsy',
                colName: 'hmcfsy',
                width: 200,
                defaults: {
                    name: 'HmcfSy'
                },
                columns: 2,
                vertical: true,
                items: [
                    { id: 'hmcfsy1', boxLabel: NO, name: 'hmcfsy', inputValue: '0' },
                    { id: 'hmcfsy2', boxLabel: YES, name: 'hmcfsy', inputValue: '1', checked: true }
                ]
            },
            {
                xtype: 'radiogroup',
                fieldLabel: SHIYONGXIANGTONG,
                id: 'sydzxh',
                colName: 'sydzxh',
                width: 200,
                defaults: {
                    name: 'SydzXh'
                },
                columns: 2,
                vertical: true,
                items: [
                    { id: 'sydzxh1', boxLabel: NO, name: 'sydzxh', inputValue: '0' },
                    { id: 'sydzxh2', boxLabel: YES, name: 'sydzxh', inputValue: '1', checked: true }
                ]
            },
            {
                xtype: 'numberfield',
                fieldLabel: '使用期限',
                name: 'days',
                id: 'days',
                allowBlank: false,
                value: '30',
                minValue: 1
            },


             {
                 xtype: "datetimefield",
                 fieldLabel: BEGINTIME,
                 editable: false,
                 id: 'startbegin',
                 name: 'startbegin',
                 anchor: '95%',
                 format: 'Y-m-d H:i:s',
                 width: 150,
                 allowBlank: false,
                 submitValue: true,
                 value: Tomorrow(),
                 time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                 listeners: {
                     select: function (a, b, c) {
                         var start = Ext.getCmp("startbegin");
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
                          anchor: '95%',
                          name: 'end',
                          format: 'Y-m-d H:i:s',
                          width: 150,
                          allowBlank: false,
                          submitValue: true, //
                          value: setNextMonth(Tomorrow(), 1),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("startbegin");
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
                var selStartBegin = Ext.getCmp("startbegin").getValue();
                var selEndTime = Ext.getCmp("end").getValue();
                if (selEndTime <= selStartBegin) {
                    alert(TIMETIP);
                    Ext.getCmp("end").setValue("");
                    return;
                }
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            rowid: Ext.htmlEncode(Ext.getCmp('id').getValue()),
                            name: Ext.htmlEncode(Ext.getCmp('name').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                            amount: Ext.htmlEncode(Ext.getCmp('amount').getValue()),
                            bonus_type: Ext.htmlEncode(Ext.getCmp("bonus_type").getValue().Bonus_Type),
                            hmcfsy: Ext.htmlEncode(Ext.getCmp("hmcfsy").getValue().HmcfSy),
                            sydzxh: Ext.htmlEncode(Ext.getCmp('sydzxh').getValue().SydzXh),
                            startbegin: Ext.htmlEncode(Ext.getCmp('startbegin').getValue()),
                            end: Ext.htmlEncode(Ext.getCmp('end').getValue()),
                            condition_id: condition_id,
                            days: Ext.htmlEncode(Ext.getCmp('days').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION,SUCCESS);
                                FaresStore.load();
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
        title: XHDH,
        iconCls: 'icon-user-edit',
        width: 400, id: 'editWin',
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
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
                    editFrm.getForm().reset(); //如果是編輯的話
                } else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話

                    if (row.data.condition_id != 0) {
                        Ext.getCmp('us1').setValue(false);
                        Ext.getCmp('us2').setValue(true);
                    }
                    else {
                        Ext.getCmp('us1').setValue(true);
                        Ext.getCmp('us2').setValue(false);
                        if (row.data.group_name == "") {
                            Ext.getCmp('group_name').setValue(BUFEN);
                        }
                    }

                    var _bt = row.data.type; //針對點數類型
                    if (_bt == "1") {
                        Ext.getCmp('bt1').setValue(true);
                        Ext.getCmp('bt2').setValue(false);
                    } else if (_bt == "2") {
                        Ext.getCmp('bt1').setValue(false);
                        Ext.getCmp('bt2').setValue(true);
                    }

                    var _rpt = row.data.repeat; //針對序號重複使用
                    if (_rpt == "true") {
                        Ext.getCmp('hmcfsy1').setValue(false);
                        Ext.getCmp('hmcfsy2').setValue(true);
                    } else if (_rpt == "false") {
                        Ext.getCmp('hmcfsy1').setValue(true);
                        Ext.getCmp('hmcfsy2').setValue(false);
                    }

                    var _mpl = row.data.multiple; //針對使用多組序號
                    if (_mpl == "true") {
                        Ext.getCmp('sydzxh1').setValue(false);
                        Ext.getCmp('sydzxh2').setValue(true);
                    } else if (_mpl == "false") {
                        Ext.getCmp('sydzxh1').setValue(true);
                        Ext.getCmp('sydzxh2').setValue(false);
                    }
                    Ext.getCmp('days').setValue(row.data.days);

                }
            }
        }
    });
    editWin.show();
}

