
editFunction = function (row, store) {
    var mytype = 0;
    var conditionID = "";
    VipGroupStore.load();
    if (row != null) {
        conditionID = row.data["condition_id"];
    }


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45, 
        url: '/PromotionsDeductRate/Save',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: 'Id',
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
                    //                    allowBlank: true,
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
            minValue: 0,
            allowBlank: false
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
                    minValue: 0
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
                    minValue: 0
                }
            ]
        },
        {
            xtype: 'radiogroup',
            id: 'bonus_type',
            name: 'bonus_type',
            fieldLabel: POINTTYPE,
            colName: 'bonus_type',
            width: 150,
            defaults: {
                name: 'Tax_Type'
            },
            columns: 3,
            vertical: true,
            items: [
                { id: 'id1', boxLabel: BONUS_ONE, inputValue: '1', checked: true },
                { id: 'id2', boxLabel: BONUS_TWO, inputValue: '2' },
                { id: 'id3', boxLabel: BONUS_THREE, inputValue: '3' }
            ]
        },
        {
            xtype: 'numberfield',
            fieldLabel: POINTTOP,
            name: 'rate',
            id: 'rate',
            allowBlank: false,
            minValue: 0,
            maxValue: 100,
            value: 100,
            regex: /^[-+]?([1-9]\d*|0)$/,
            regexText: VENDERTIP,
            submitValue: true
        },

         {
             xtype: "datetimefield",
             fieldLabel: BEGINTIME,
             editable: false,
             id: 'startdate',
             name: 'startdate',
             anchor: '95%',
             format: 'Y-m-d H:i:s',
             width: 150,
             allowBlank: false,
             submitValue: true,
             value: Tomorrow(),
             time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
             listeners: {
                 select: function (a, b, c) {
                     var start = Ext.getCmp("startdate");
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
                                  var start = Ext.getCmp("startdate");
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
                      }],
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
                var selStartDate = Ext.getCmp("startdate").getValue();
                var selEndTime = Ext.getCmp("end").getValue();
                if (selEndTime <= selStartDate) {
                    Ext.Msg.alert(INFORMATION, TIMETIP);
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
                            point: Ext.htmlEncode(Ext.getCmp('point').getValue()),
                            dollar: Ext.htmlEncode(Ext.getCmp('dollar').getValue()),
                            rate: Ext.htmlEncode(Ext.getCmp('rate').getValue()),
                            startdate: Ext.htmlEncode(Ext.getCmp('startdate').getValue()),
                            end: Ext.htmlEncode(Ext.getCmp('end').getValue()),
                            bonus_type: Ext.htmlEncode(Ext.getCmp("bonus_type").getValue().Tax_Type),
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
        title: DSDY,
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 420,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
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

                } else {

                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                    var _type = row.data.bonus_type;

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
                    if (_type == '1') {
                        Ext.getCmp('id1').setValue(true);
                        Ext.getCmp('id2').setValue(false);
                        Ext.getCmp('id3').setValue(false);
                    } else if (_type == '2') {
                        Ext.getCmp('id1').setValue(false);
                        Ext.getCmp('id2').setValue(true);
                        Ext.getCmp('id3').setValue(false);
                    } else if (_type == "3") {
                        Ext.getCmp('id1').setValue(false);
                        Ext.getCmp('id2').setValue(false);
                        Ext.getCmp('id3').setValue(true);
                    }

                }
            }
        }
    });

    editWin.show();
}
