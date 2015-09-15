
var SaveToolWin = function (row, trow) {
    var ToolForm = Ext.create('Ext.form.Panel', {
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
            fieldLabel: TOOL_NAME,
            allowBlank: false,
            id: 'FunctionName',
            Name: 'FunctionName',
            submitValue: false
        }, {
            fieldLabel: TOOL_CODE,
            allowBlank: false,
            id: 'FunctionCode',
            Name: 'FunctionCode',
            submitValue: false
        }, {
            xtype: 'fieldcontainer',
            defaultType: 'checkboxfield',
            items: [
                {
                    boxLabel: SET_ISAUTHORIZED,
                    name: 'topping',
                    inputValue: '1',
                    id: 'ckIsEdit'
                }]
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
                            Name: Ext.htmlEncode(Ext.getCmp('FunctionName').getValue()),
                            Code: Ext.htmlEncode(Ext.getCmp('FunctionCode').getValue()),
                            IsEdit: Number(Ext.getCmp('ckIsEdit').getValue()),
                            Group: Ext.htmlEncode(row.data.FunctionName),
                            TopValue: row.data.RowId,
                            Type: 2
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                ToolStore.load({
                                    params: { TopValue: Ext.htmlEncode(row.data.RowId), Type: 2 }
                                });
                                Ext.getCmp('SaveToolWin').close();
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
        id: 'SaveToolWin',
        title: TOOL_SAVE,
        items: [ToolForm],
        width: 290,
        height: 220,
        layout: 'fit',
        labelWidth: 100,
        closeAction: 'destroy',
        resizable: false,
        modal: 'true',
        iconCls: trow ? "icon-edit" : "icon-add",
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            "show": function () {
                if (trow) {
                    ToolForm.getForm().loadRecord(trow);
                    Ext.getCmp("ckIsEdit").setValue(trow.data.IsEdit);
                }
                else {
                    ToolForm.getForm().reset();
                }
            }
        }
    }).show();
}