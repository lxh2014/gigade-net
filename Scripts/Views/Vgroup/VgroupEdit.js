editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/Vgroup/Edit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            name: 'rowid',
            hidden: true
        }, {
            fieldLabel: GROUPNAME,
            name: 'groupName',
            id: 'groupName',
            submitValue: false,
            allowBlank: false
        }, {
            fieldLabel: GROUPCODE,
            name: 'groupCode',
            id: 'groupCode',
            submitValue: false,
            allowBlank: false
        }, {
            fieldLabel: REMARK,
            name: 'remark',
            id: 'remark',
            submitValue: false
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
                            groupName: Ext.htmlEncode(Ext.getCmp('groupName').getValue()),
                            groupCode: Ext.htmlEncode(Ext.getCmp('groupCode').getValue()),
                            remark: Ext.htmlEncode(Ext.getCmp('remark').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, result.msg);
                            if (result.success) {
                                FgroupStore.load();
                                editWin.close();
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
        title: SAVETITLE,
        iconCls: 'icon-user-edit',
        width: 400,
        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
            }
        }
    });

    editWin.show();
}