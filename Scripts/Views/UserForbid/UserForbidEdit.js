var boolstatus = true;
editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/UserForbid/SaveUserFoid',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: '流水號',
            id: 'forbid_id',
            name: 'forbid_id',
            hidden: true
        },
        {
            xtype: 'textfield',
            fieldLabel: '禁用IP',
            name: 'forbid_ip',
            id: 'forbid_ip',
            allowBlank: false,
            submitValue: true
        }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (Ext.getCmp('forbid_ip').getValue() != "") {
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                forbid_id: Ext.htmlEncode(Ext.getCmp('forbid_id').getValue()),
                                forbid_ip: Ext.htmlEncode(Ext.getCmp('forbid_ip').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.msg == '1') {
                                        Ext.getCmp('forbid_ip').setValue("");
                                        Ext.Msg.alert("提示信息", "該IP已經禁用！");
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        UserForbidStore.load();
                                        editWin.close();
                                    }

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

            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '設置IP黑名單',
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 420,
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
             qtip: '關閉',
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
                }
            }
        }
    });
    editWin.show();
}
