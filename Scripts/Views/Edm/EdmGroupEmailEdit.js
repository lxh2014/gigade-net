editFunction = function (rowID, store, group_id, group_name) {
   
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Edm/EdmGroupEmailEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 xtype: 'displayfield',
                 fieldLabel: '群組編號',
                 id: 'group_id',
                 name: 'group_id',
             },
              {
                  xtype: 'displayfield',
                  fieldLabel: '群組名稱',
                  id: 'group_name',
                  width:300,
                  name: 'group_name',
              },
               {
                   xtype: 'displayfield',
                   fieldLabel: '郵件編號',
                   id: 'email_id',
                   name: 'email_id',
                   hidden:false
               },
            {
                xtype: 'textfield',
                fieldLabel: '電子郵件',
                id: 'email_address',
                name: 'email_address',
                allowBlank: false,
                vtype: 'email'
            },
            {
                xtype: 'textfield',
                fieldLabel: '姓名',
                id: 'email_name',
                name: 'email_name'
                
            },
            {
                xtype: 'radiogroup',
                fieldLabel: '訂閱狀態',
                id: 'email_status',
                name: 'email_status',
                columns: 2,
                vertical: true,
                items: [
                   { boxLabel: '訂閱', id: 'rdo1', name: 'email_status', inputValue: '1',checked:true },
                   { boxLabel: '取消', id: 'rdo2', name: 'email_status', inputValue: '2' }
                ]
            },
            {
                xtype: 'displayfield',
                fieldLabel: '建立日期',
                id: 'email_createdate_tostring',
                name: 'email_createdate_tostring',
                hidden:false
            },
            {
                xtype: 'displayfield',
                fieldLabel: '修改日期',
                id: 'email_updatedate_tostring',
                name: 'email_updatedate_tostring',
                hidden: false
            }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            email_id: Ext.htmlEncode(Ext.getCmp('email_id').getValue()),
                            email_address: Ext.htmlEncode(Ext.getCmp('email_address').getValue()),
                            email_name: Ext.htmlEncode(Ext.getCmp('email_name').getValue()),
                            email_status: Ext.htmlEncode(Ext.getCmp('email_status').getValue().email_status)
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == 0) {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "操作失敗!");
                                }
                                load();
                                EdmGroupEmailStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
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
        title: '電子報群組新增/編輯',
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 400,
        height:300,
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
                qtip: CLOSE,
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
            }
        ],
        listeners: {
            'show': function () {
                if (row != null) {
                    editFrm.getForm().loadRecord(row);
                    var email_status = row.data.email_status.toString();
                    if (email_status == 1) {
                        Ext.getCmp('rdo1').setValue(true);
                    } else if (email_status == 2) {
                        Ext.getCmp('rdo2').setValue(true);
                    }
                }
                else {
                    editFrm.getForm().reset();
                    Ext.getCmp("group_id").setValue(group_id);
                    Ext.getCmp("group_name").setValue(group_name);
                    Ext.getCmp("email_id").hide(true);
                    Ext.getCmp("email_createdate_tostring").hide(true);
                    Ext.getCmp("email_updatedate_tostring").hide(true);
                }
            }
        }
    });
    //  editWin.show();
    var row = null;
    if (rowID != null) {
        Edit_EdmGroupEmailStore.load({
            params: { group_id: group_id, email_id: rowID },
            callback: function () {
                row = Edit_EdmGroupEmailStore.getAt(0);
                editWin.show();
            }
        });
    }
    else {
        editWin.show();
    }
}