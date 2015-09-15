editFunction = function (row, store) {
    var editBlackListFrm = Ext.create('Ext.form.Panel', {
        id: 'editBlackListFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Edm/UpdateBlackList',
        defaults: { anchor: "90%" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: '黑名單信箱',
            id: 'mail',
            name: 'mail',
            vtype: 'email',
            submitValue: true,
            labelWidth:70,
            margin:'5 5 0 10',
            allowBlank: false
        }

        ],
        buttonAlign: 'right',
        buttons: [{
            text: '送出',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            mail: Ext.htmlEncode(Ext.getCmp('mail').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);

                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                BlackListStore.load();
                                editWin.close();
                            }
                            else {
                                if (result.msg == '0') {
                                    Ext.Msg.alert(INFORMATION, "該郵箱用戶不是會員，無法加入黑名單");
                                }
                                else if (result.msg == '1') {
                                    Ext.Msg.alert(INFORMATION, "該信箱已存在黑名單中，請勿再次添加");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            }
                        },
                        failure: function (form, action) {
                            if (Ext.decode(action.response.responseText).msg == 0) {
                                Ext.Msg.alert(INFORMATION, "該郵箱用戶不是會員，無法加入黑名單");
                                Ext.getCmp('mail').reset();
                            }
                            else if (Ext.decode(action.response.responseText).msg == 1) {
                                Ext.Msg.alert(INFORMATION, "該信箱已存在黑名單中，請勿再次添加");
                                Ext.getCmp('mail').reset();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }
                    })
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '新增黑名單',
        id: 'editWin',
        width: 430,
        height: 120,
        layout: 'fit',
        iconCls: "icon-user-edit",
        bodyStyle: 'padding:5px 5px 5px 5px',
        items: [editBlackListFrm],
        constrain: true,
        labelWidth: 60,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
             {
                 type: 'close',
                 qtip: '是否關閉',
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
                editBlackListFrm.getForm().loadRecord(row); //如果是編輯的話
            }
        }
    });
    editWin.show();
}
