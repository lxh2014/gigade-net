editFunction = function (rowID, store, group_id) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Edm/EdmTestAddorEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '郵箱編號',
            id: 'email_id',
            name: 'email_id',
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
            id: 'test_username',
            name: 'test_username',
            allowBlank: false
        },
        {
            xtype: 'radiogroup',
            fieldLabel: '訂閱狀態',
            id: 'test_status',
            name: 'test_status',
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: '訂閱', id: 'rdo1', name: 'test_status', inputValue: '1' },
            { boxLabel: '取消', id: 'rdo2', name: 'test_status', inputValue: '2' }
            ]
        },
        {
            xtype: 'displayfield',
            fieldLabel: '建立日期',
            id: 'test_createdate',
            name: 'test_createdate'
        },
        {
            xtype: 'displayfield',
            fieldLabel: '修改日期',
            id: 'test_updatedate',
            name: 'test_updatedate'
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
                            email_id: Ext.htmlEncode(Ext.getCmp('email_id').getValue()),
                            email_address: Ext.htmlEncode(Ext.getCmp('email_address').getValue()),
                            test_username: Ext.htmlEncode(Ext.getCmp('test_username').getValue()),
                            test_status: Ext.htmlEncode(Ext.getCmp('test_status').getValue().status)
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                EdmTestStore.load();
                                editWin.close();
                            }
                            else {
                                if (result.msg == "0") {
                                    Ext.Msg.alert(INFORMATION, "該郵箱已加入測試名單，請查看后再做修改");
                                } else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg == "0") {
                                Ext.Msg.alert(INFORMATION, "該郵箱已加入測試名單，請查看后再做修改");
                            } else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '測試名單新增',
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
                    var test_status = row.data.test_status.toString();
                    if (test_status == 1) {
                        Ext.getCmp('rdo1').setValue(true);
                    } else if (test_status == 2) {
                        Ext.getCmp('rdo2').setValue(true);
                    }
                }
                else {
                    editFrm.getForm().reset();
                    Ext.getCmp("email_id").hide(true);
                }
            }
        }
    });
    // editWin.show();
    var row = null;
    if (rowID != null) {
        Edit_EdmTestStore.load({
            params: { email_id: rowID },
            callback: function () {
                row = Edit_EdmTestStore.getAt(0);
                editWin.show();
            }
        })
    }
    else {
        editWin.show();
    }
}
addFunction = function (store) {
    var addFrm = Ext.create('Ext.form.Panel', {
        id: 'addFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Edm/EdmTestAddorEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: '電子郵件',
            id: 'EmailAddress',
            name: 'EmailAddress',
            allowBlank: false,
            vtype: 'email'
        },
        {
            xtype: 'textfield',
            fieldLabel: '姓名',
            id: 'TestUsername',
            name: 'TestUsername',
            allowBlank: false
        },
        {
            xtype: 'radiogroup',
            fieldLabel: '訂閱狀態',
            id: 'Status',
            name: 'Status',
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: '訂閱', id: 'status1', name: 'status', inputValue: '1', checked: true },
            { boxLabel: '取消', id: 'status2', name: 'status', inputValue: '2' }
            ]
        }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            email_address: Ext.htmlEncode(Ext.getCmp('EmailAddress').getValue()),
                            test_username: Ext.htmlEncode(Ext.getCmp('TestUsername').getValue()),
                            test_status: Ext.htmlEncode(Ext.getCmp('Status').getValue().status)
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                EdmTestStore.load();
                                addWin.close();
                            }
                            else if (result.msg == "1") { 
                                Ext.Msg.alert(INFORMATION, "該郵箱已加入測試名單，請查看后再添加!");
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg == "1") {
                                Ext.Msg.alert(INFORMATION, "該郵箱已加入測試名單，請查看后再添加");
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }
                    });
                }
            }
        }]
    });
    var addWin = Ext.create('Ext.window.Window', {
        title: '測試名單新增',
        id: 'addWin',
        iconCls: 'icon-user-edit',
        width: 420,
        layout: 'fit',
        items: [addFrm],
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
                        Ext.getCmp('addWin').destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ],
        //listeners: {
        //    'show': function () {
        //        addFrm.getForm().reset(); //如果是添加的話              
        //    }
        //}
    });
    addWin.show();
}