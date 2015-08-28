//會員群組Model
Ext.define("gigade.Parametersrc", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }]
});
//會員群組store
var GroupTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Parametersrc',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Redirect/GetGroupType",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

function editFunction(RowID, Store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Redirect/SaveGroup',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '編號',
            id: 'group_id',
            name: 'group_id',
            editable: false,
            hidden: true
        },
        {
            xtype: 'textfield',
            fieldLabel: "群組名稱",
            allowBlank: false,
            id: 'group_name',
            name: 'group_name'
        },
        {
            xtype: 'combobox', //來源
            editable: false,
            fieldLabel: '來源類型',
            allowBlank: false,
            id: 'group_type',
            name: 'group_type',
            hiddenName: 'group_type',
            store: GroupTypeStore,
            displayField: 'parameterName',
            valueField: 'ParameterCode',
            emptyText: '請選擇...',
            typeAhead: true,
            forceSelection: false
        },
        {//建立時間
            xtype: 'displayfield',
            fieldLabel: "建立時間",
            hidden: true,
            editable: false,
            id: 'createdate',
            name: 'createdate'
        },
        {//更新時間
            xtype: 'displayfield',
            fieldLabel: "更新時間",
            hidden: true,
            editable: false,
            id: 'updatedate',
            name: 'updatedate'
        },
        {//來源ip
            xtype: 'displayfield',
            fieldLabel: "來源ip",
            hidden: true,
            editable: false,
            id: 'group_ipfrom',
            name: 'group_ipfrom'
        }
        ],
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            group_id: Ext.htmlEncode(Ext.getCmp("group_id").getValue()),
                            group_name: Ext.htmlEncode(Ext.getCmp("group_name").getValue()),
                            group_type: Ext.getCmp("group_type").getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                Store.load();
                                editWin.close();
                            } else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    })
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "群組新增/編輯",
        id: 'editWin',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        width: 400,
        height: 400,
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
         }],
        listeners: {
            'show': function () {
                if (RowID) {
                    if (RowID) {
                        editFrm.getForm().loadRecord(RowID);//為控件賦值
                        Ext.getCmp('group_id').show();
                        Ext.getCmp('createdate').show();
                        Ext.getCmp('updatedate').show();
                        Ext.getCmp('group_ipfrom').show();
                    }
                    else {
                        editFrm.getForm().reset();
                    }

                }
            }
        }
    });
    editWin.show();
}
