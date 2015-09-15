Ext.define('GIGADE.Group', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'FunctionGroup', type: 'string' }
    ]
})
var GroupStore = Ext.create('Ext.data.Store', {
    autoLoad: true,
    model: 'GIGADE.Group',
    proxy: {
        type: 'ajax',
        url: '/Function/GetGroup',
        actionMethods: 'post',
        render: {
            type: 'json'
        }
    }
})

var SaveWin = function (row) {

    var FunctionForm = Ext.create('Ext.form.Panel', {
        id: 'FunctionFrm',
        autoScroll: true,
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/Function/SaveFunction',
        defaults: { anchor: "95%", msgTarget: "side", labelAlign: 'top' },
        items: [{
            hidden: true,
            name: 'RowId'
        }, {
            xtype: 'combobox',
            store: GroupStore,
            displayField: 'FunctionGroup',
            valueField: 'FunctionGroup',
            queryMode: 'local',
            typeAhead: true,
            fieldLabel: GROUP,
            allowBlank: false,
            id: 'FunctionGroup',
            Name: 'FunctionGroup',
            submitValue: false
        }, {
            fieldLabel: NAME,
            allowBlank: false,
            id: 'FunctionName',
            Name: 'FunctionName',
            submitValue: false
        }, {
            fieldLabel: CODE,
            allowBlank: false,
            id: 'FunctionCode',
            Name: 'FunctionCode',
            submitValue: false
        }, {
            fieldLabel: ICONCLS,
            id: 'IconCls',
            Name: 'IconCls',
            submitValue: false
        }, {
            xtype: 'textarea',
            fieldLabel: REMARK,
            id: 'Remark',
            name: 'Remark',
            submitValue: false
        }, {
            xtype: 'checkbox',
            boxLabel: CHILD_ONLY,
            id: 'child_only',
            name: 'child_only'
        }],
        buttonAlign: 'center',
        buttons: [
        {
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            Group: Ext.htmlEncode(Ext.getCmp('FunctionGroup').getValue()),
                            Name: Ext.htmlEncode(Ext.getCmp('FunctionName').getValue()),
                            Code: Ext.htmlEncode(Ext.getCmp('FunctionCode').getValue()),
                            IconCls: Ext.htmlEncode(Ext.getCmp('IconCls').getValue()),
                            Remark: Ext.htmlEncode(Ext.getCmp('Remark').getValue()),
                            IsEdit: 1
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                FunStore.load();
                                GroupStore.load();
                                Ext.getCmp('SaveWin').close();
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


    Ext.create('Ext.window.Window', {
        id: 'SaveWin',
        title: SAVETITLE,
        items: [FunctionForm],
        width: 390,
        height: document.documentElement.clientHeight * 400 / 783,
        layout: 'fit',
        labelWidth: 100,
        closeAction: 'destroy',
        resizable: false,
        modal: 'true',
        iconCls: row ? "icon-edit" : "icon-add",
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            "show": function () {
                if (row) {
                    FunctionForm.getForm().loadRecord(row);
                    Ext.getCmp('child_only').setValue(row.data.FunctionType == 3);
                }
                else {
                    FunctionForm.getForm().reset();
                }
            }
        }
    }).show();
}