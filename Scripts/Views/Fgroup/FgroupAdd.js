
var addFrm = Ext.create('Ext.form.Panel', {
    id: 'addFrm',
    frame: true,
    plain: true,
    defaultType: 'textfield',
    layout: 'anchor',
    labelWidth: 45,
    url: '/Fgroup/Add',
    defaults: { anchor: "95%", msgTarget: "side" },
    items: [
        {
            fieldLabel: GROUPNAME,
            id: 'aGroupName',
            allowBlank: false,
            submitValue: false
        }, {
            fieldLabel: GROUPCODE,
            id: 'aGroupCode',
            allowBlank: false,
            submitValue: false
        }, {
            fieldLabel: REMARK,
            id: 'aRemark',
            allowBlank: false,
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
                        groupName: Ext.htmlEncode(Ext.getCmp('aGroupName').getValue()),
                        groupCode: Ext.htmlEncode(Ext.getCmp('aGroupCode').getValue()),
                        remark: Ext.htmlEncode(Ext.getCmp('aRemark').getValue())
                    },
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        Ext.Msg.alert(INFORMATION, result.msg);
                        if (result.success) {
                            FgroupStore.load();
                            addWin.hide();
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

var addWin = Ext.create('Ext.window.Window', {
    title: SAVETITLE,
    iconCls: 'icon-user-add',
    width: 400,
    height: document.documentElement.clientHeight * 260 / 783,
    layout: 'fit',
    items: [addFrm],
    closeAction: 'hide',
    modal: true,
    resizable: false,
    labelWidth: 60,
    bodyStyle: 'padding:5px 5px 5px 5px',
    listeners: {
        'show': function () {
            addFrm.getForm().reset();
        }
    }
});

