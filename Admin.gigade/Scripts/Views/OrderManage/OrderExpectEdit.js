editFunction = function (row, store) {
    //物流業者
    Ext.define("gigade.paraModel", {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'parameterCode', type: 'string' },
            { name: 'parameterName', type: 'string' }
        ]
    });
    var DeliverStore = Ext.create("Ext.data.Store", {
        model: 'gigade.paraModel',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/OrderManage/QueryPara?paraType=Deliver_Store',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });



    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/OrderManage/OrderExpectModify',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '出貨流水號',
            id: 'expect_id',
            margin: '5,0,5,0',
            name: 'expect_id',
            submitValue: true

        },
        {
            xtype: 'displayfield',
            name: 'slave_id',
            id: 'slave_id',
            submitValue: true,
            margin: '5,0,5,0',
            maxLength: 25,
            fieldLabel: '廠商出貨單號'
        },
        {
            xtype: 'radiogroup',
            id: 'e_status',
            name: 'e_status',
            margin: '5,0,5,0',
            fieldLabel: '狀態',
            colName: 'e_status',
            defaults: {
                name: 'e_status'
            },
            columns: 3,
            vertical: true,
            items: [
                    { boxLabel: '未出貨', id: 's1', inputValue: '0', checked: true },
                    { boxLabel: '已出貨', id: 's2', inputValue: '1' },
                    { boxLabel: '異常', id: 's3', inputValue: '2' }
            ]
        },
        {
            xtype: 'combobox',
            allowBlank: true,
            fieldLabel: '物流業者' + '<font color="red" > ※</font>',
            hidden: false,
            id: 'deliver_id',
            name: 'deliver_id',
            store: DeliverStore,
            displayField: 'parameterName',
            margin: '5,0,5,0',
            valueField: 'parameterCode',
            typeAhead: true,
            forceSelection: false,
            editable: false,
            value: 99
        },
        {
            xtype: 'textfield',
            name: 'code',
            id: 'code',
            margin: '5,0,5,0',
            allowBlank: false,
            submitValue: true,
            fieldLabel: '物流單號' + '<font color="red" > ※</font>'
        },
        {
            xtype: "datefield",
            fieldLabel: '出貨時間' + '<font color="red" > ※</font>',
            margin: '5,0,5,0',
            id: 'startdate',
            name: 'startdate',
            editable: false,
            format: 'Y-m-d',
            allowBlank: false,
            submitValue: true,
            value: Tomorrow(),
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("startdate");
                }
            }
        },
        {
            xtype: 'textareafield',
            name: 'note',
            id: 'note',
            allowBlank: true,
            margin: '5,0,5,0',
            fieldLabel: '出貨單備註',
            submitValue: true,
            maxLength: 100
        },
        {
            html: '<font color=red>※表示必填，提供會員查詢用</font>',
        }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            expect_id: Ext.htmlEncode(Ext.getCmp('expect_id').getValue()),
                            slave_id: Ext.htmlEncode(Ext.getCmp('slave_id').getValue()),
                            deliver_id: Ext.htmlEncode(Ext.getCmp('deliver_id').getValue()),
                            code: Ext.htmlEncode(Ext.getCmp('code').getValue()),
                            stime: Ext.Date.format(new Date(Ext.getCmp('startdate').getValue()), 'Y-m-d H:i:s'),
                            note: Ext.htmlEncode(Ext.getCmp('note').getValue()),
                            e_status: Ext.htmlEncode(Ext.getCmp("e_status").getValue().e_status),
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "保存成功! ");
                                OrderExpectListStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(INFORMATION, "保存失敗! ");
                        }
                    });

                }

            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '出貨單',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
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
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    Ext.getCmp('note').setValue("");
                    Ext.getCmp('code').setValue("");
                }
                else {
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

}

