editFunction = function (rowID) {
    var row;
    edit_SmsStore.load({
        params: { relation_id: rowID },
        callback: function () {
            row = edit_SmsStore.getAt(0);
            editWins.show();
        }
    });
    var SmsEditFrm = Ext.create('Ext.form.Panel', {
        id: 'SmsEditFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        url: '/Service/updateSms',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: "編號",
                id: 'id',
                labelWidth: 60,
                name: 'id',
                submitValue: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: "訂單編號",
                 id: 'order_id',
                 labelWidth: 60,
                 name: 'order_id',
                 submitValue: true
             },
            {
                xtype: 'displayfield',
                fieldLabel: "行動電話",
                id: 'mobile',
                labelWidth: 60,
                name: 'mobile',
                submitValue: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: "主旨",
                 id: 'subject',
                 labelWidth: 60,
                 name: 'subject',
                 submitValue: true
             },
              {
                  xtype: 'textarea',
                  fieldLabel: "內容",
                  labelWidth: 60,
                  id: 'content',
                  name: 'content',
                  submitValue: true
              },
                      {
                          xtype: 'fieldcontainer',
                          combineErrors: true,
                          layout: 'hbox',
                          labelWidth: 30,
                          items: [
                            { xtype: 'displayfield', value: "狀態:" },
                            {
                                xtype: 'radiofield',
                                boxLabel: '未發送',
                                margin: '0 0 0 20',
                                name: 'state',
                                id: 'state1',
                                value: true
                            },
                               {
                                   xtype: 'radiofield',
                                   boxLabel: '取消',
                                   name: 'state',
                                   margin: '0 0 0 20',
                                   id: 'state2'
                               }
                          ]
                      }
        ],
        buttons: [

            {
                formBind: true,
                disabled: true,
                text: "確定修改",
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                id: Ext.getCmp('id').getValue(),
                                content: Ext.getCmp('content').getValue(),

                                state1: Ext.getCmp('state1').getValue(),
                                state2: Ext.getCmp('state2').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    SmsStore.load();
                                    editWins.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                    SmsStore.load();
                                    editWins.close();
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                SmsStore.load();
                                editWins.close();
                            }
                        });
                    }
                }
            }
        ]
    });
    var editWins = Ext.create('Ext.window.Window', {
        title: "修改簡訊狀態",
        iconCls: 'icon-user-edit',
        id: 'editWins',
        width: 400,
        height: 300,
        layout: 'fit',
        items: [SmsEditFrm],
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
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                //Ext.getCmp('id').setValue(row.data.id); 
                //Ext.getCmp('order_id').setValue(b);
                //Ext.getCmp('mobile').setValue(c);
                //Ext.getCmp('subject').setValue(d);
                //Ext.getCmp('content').setValue(e);
                SmsEditFrm.getForm().loadRecord(row); //如果是編輯的話
                Ext.getCmp('state1').setValue(true);
            }
        }
    });
    // editWins.show();
}

