editFunction = function (row, store) {
    var editKeyWordsFrm = Ext.create('Ext.form.Panel', {
        id: 'editKeyWordsFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/DispatchingSystem/AddOrEdit',
        defaults: { anchor: "99%" },
        items: [
             {
                 xtype: 'displayfield',
                 fieldLabel: '編號',
                 id: 'dk_id',
                 name: 'dk_id',
                 hidden: true
             },
        {
            xtype: 'displayfield',
            fieldLabel: '禁用資料關鍵字'
        },
        {
            xtype: 'textareafield',
            id: 'dk_string',
            name: 'dk_string',
            autoScroll: true,
            height: 200,
            allowBlank: false,
            submitValue: true
        }
        ],
        buttons: [
        {
            xtype: 'button',
            vtype: 'submit',
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    Ext.Ajax.request({
                        url: "/DispatchingSystem/GetCount",
                        params: {
                            dk_string: Ext.getCmp('dk_string').getValue()
                        },
                        success: function (response) {
                            var result = eval("(" + response.responseText + ")");
                            if (result.success == true) {
                                if (result.msg > 0) {
                                    Ext.getCmp('dk_string').setValue("");
                                    Ext.Msg.alert(INFORMATION, "此關鍵字已存在!");
                                    return;
                                }
                                else {
                                    form.submit({
                                        params: {
                                            dk_string: Ext.htmlEncode(Ext.getCmp('dk_string').getValue()),
                                            dk_id: Ext.htmlEncode(Ext.getCmp('dk_id').getValue())
                                        },
                                        success: function (form, action) {
                                            var result = Ext.decode(action.response.responseText);
                                            if (result.success) {
                                                if (result.msg == 0) {
                                                    Ext.Msg.alert(INFORMATION, "新增成功!");
                                                }
                                                else if (result.msg == 1) {
                                                    Ext.Msg.alert(INFORMATION, "編輯成功");
                                                }
                                                DisKeyWordsStore.load();
                                                editDisKeyWordsWin.close();
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
                    });

                }
            }
        }
        ]
    });
    var editDisKeyWordsWin = Ext.create('Ext.window.Window', {
        title: '新增禁用關鍵字',
        iconCls: 'icon-user-edit',
        id: 'editDisKeyWordsWin',
        width: 430,
        height: 320,
        layout: 'fit',
        items: [editKeyWordsFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: '是否關閉',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                    if (btn == "yes") {
                        Ext.getCmp('editDisKeyWordsWin').destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }],
        listeners: {
            'show': function () {
                editKeyWordsFrm.getForm().loadRecord(row);
            }
        }
    });
    editDisKeyWordsWin.show();
}