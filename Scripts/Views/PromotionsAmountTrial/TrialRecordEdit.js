var isChecked = 0; 
editFunction = function (rowID, store) {
    var row = null;
    if (rowID != null) {
        edit_TrialApplyStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_TrialApplyStore.getAt(0);
                editWin.show();
            }
        });
    }
    else {
        editWin.show();
    }
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/PromotionsAmountTrial/TrialRecordUpdate',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: RECORDID,
            id: 'record_id',
            name: 'record_id',
            submitValue: true,
            readOnly: true,
            hidden: true
        }, 
        {
            xtype: 'textfield',
            fieldLabel: HDID,
            id: 'trial_id',
            name: 'trial_id',
            submitValue: true,
            readOnly: true,
            hidden: true
        },
          {
              xtype: 'textfield',
              name: 'user_email',
              id: 'user_email',
              submitValue: true,
              allowBlank: false,
              readOnly:true,
              fieldLabel: USEREMAIL
          },
        {
            xtype: 'radiogroup',
            hidden: false,
            id: 'status',
            name: 'status',
            fieldLabel: STATUS,
            colName: 'status',
            defaults: {
                name: 'status',
                margin: '0 8 0 0'
            },
            columns:3,
            vertical: true,
            items: [
            { boxLabel: APPLYSTATUS1, id: 'st1', inputValue: '1', width: 150 },
            {
                boxLabel: APPLYSTATUS2, id: 'st2', inputValue: '2',
                listeners: {
                    change: function () {
                        Ext.Ajax.request({
                            url: '/PromotionsAmountGift/FirstSaveGift',
                            method: 'post',
                            params: {
                                
                            },
                            success: function (form, action) {

                            },
                            failure: function () {
                                Ext.Msg.alert("提示信息", "操作失敗");
                            }
                        });
                    }

                 
                }
            },
            { boxLabel: APPLYSTATUS3, id: 'st3', inputValue: '3' }
            ],
            listeners: {
                change: function () {
                    if (isChecked != 0) {
                        var trial_id = Ext.getCmp('trial_id').getValue();
                        var status = Ext.getCmp('status').getValue().status;
                        if (status == 2) {
                            Ext.Ajax.request({
                                url: '/PromotionsAmountTrial/VerifyMaxCount',
                                params: {
                                    trial_id: trial_id,
                                    status: status
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText)
                                    if (result.success) {
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "已達試用最大上線！");
                                        Ext.getCmp('save').setDisabled(true);
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert("提示信息", "已達試用最大上線！");
                                }
                            });
                        }
                        else {
                            Ext.getCmp('save').setDisabled(false);
                        }
                    }

                }
            }
        }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            id:'save',
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            user_email: Ext.htmlEncode(Ext.getCmp('user_email').getValue()),
                            record_id: Ext.htmlEncode(Ext.getCmp('record_id').getValue()),
                            status: Ext.htmlEncode(Ext.getCmp('status').getValue().status)
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                TrialApplyStore.load({
                                    params: {
                                        trial_id: Ext.htmlEncode(Ext.getCmp('trial_id').getValue())
                                    }
                                });
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                TrialApplyStore.load({
                                    params: {
                                        trial_id: Ext.htmlEncode(Ext.getCmp('trial_id').getValue())
                                    }
                                });
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            TrialApplyStore.load();
                            editWin.close();
                            TrialApplyStore.load();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: TRIALRECORD,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
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
                         isChecked = 0;
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話
                } else {
                    editFrm.getForm().loadRecord(row);
                    if (row.data.status == 1) {
                        Ext.getCmp("st1").setValue(true);
                        isChecked = 1;
                    }
                    else if (row.data.status == 2) {
                        Ext.getCmp("st2").setValue(true);
                        isChecked = 1;
                    } else {
                        Ext.getCmp("st3").setValue(true);
                        isChecked = 1;
                    }
                }
            }
        }
    });
    //editWin.show();
}