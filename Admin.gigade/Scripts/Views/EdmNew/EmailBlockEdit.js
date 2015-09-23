editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/EdmNew/AddorEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '郵箱位址',
            id: 'email_address',
            name: 'email_address',
        },
        {
            xtype: 'textareafield',
            fieldLabel: '擋信原因',
            id: 'block_reason',
            name: 'block_reason',
            allowBlank: false
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
                            email_address: Ext.htmlEncode(Ext.getCmp('email_address').getValue()),
                            block_reason: Ext.htmlEncode(Ext.getCmp('block_reason').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                EmailBlockListStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '擋信名單修改',
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
                editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();
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
        url: '/EdmNew/AddorEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: '郵箱位址',
            id: 'EmailAddress',
            name: 'EmailAddress',
            allowBlank: false,
            vtype: 'email'
        },
        {
            xtype: 'textareafield',
            fieldLabel: '擋信原因',
            id: 'reason',
            name: 'reason,',
            allowBlank: false
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
                            reason: Ext.htmlEncode(Ext.getCmp('reason').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                EmailBlockListStore.load();
                                addWin.close();
                            }
                            else if (result.msg == "0") {
                                Ext.Msg.alert(INFORMATION, "該郵箱已加入擋信名單，請查看后再添加!");
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg == "0") {
                                Ext.Msg.alert(INFORMATION, "該郵箱已加入擋信名單，請查看后再添加!");
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
        title: '擋信名單管理',
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
        ]      
    });
    addWin.show();
}
unBlockFunction = function (row, store) {
    var unBlockFrm = Ext.create('Ext.form.Panel', {
        id: 'unBlockFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/EdmNew/UnBlock',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '郵箱位址',
            id: 'email_address',
            name: 'email_address',
        },
        {
            xtype: 'textareafield',
            fieldLabel: '解除原因',
            id: 'unblock_reason',
            name: 'unblock_reason',
            allowBlank: false
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
                            email_address: Ext.htmlEncode(Ext.getCmp('email_address').getValue()),
                            unblock_reason: Ext.htmlEncode(Ext.getCmp('unblock_reason').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                EmailBlockListStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]
    });
    var unBlockWin = Ext.create('Ext.window.Window', {
        title: '擋信名單修改',
        id: 'unBlockWin',
        iconCls: 'icon-user-edit',
        width: 420,
        layout: 'fit',
        items: [unBlockFrm],
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
                        Ext.getCmp('unBlockWin').destroy();
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
                unBlockFrm.getForm().loadRecord(row);
            }
        }
    });
    unBlockWin.show();
}