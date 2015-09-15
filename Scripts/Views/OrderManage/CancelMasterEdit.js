editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/OrderManage/ModifyOrderCancelMasterList',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'displayfield',
            fieldLabel: '編號',
            id: 'cancel_id',
            name: 'cancel_id',
            submitValue: true

        },
        {
            xtype: 'displayfield',
            name: 'order_id',
            id: 'order_id',
            submitValue: true,
            allowBlank: false,
            fieldLabel: '付款單號'
        },
        {
            xtype: 'radiogroup',
            id: 'cancel_status',
            labelWidth: 80,
            colName: 'cancel_status',
            defaults: {
                name: 'Cancel_Status'
            },
            fieldLabel: '狀態',
            width: 500,
            margin: '5 10 0 5',
            columns: 2,
            items: [
                { id: 'id1', boxLabel: "待歸檔", name: 'cancel_status', inputValue: '0' },
                { id: 'id2', boxLabel: "歸檔 ", name: 'cancel_status', inputValue: '1' }
            ]
        },
        {
            xtype: 'textareafield',
            name: 'cancel_note',
            id: 'cancel_note',
            grow: true,
            submitValue: true,
            fieldLabel: '備註'
        },
        {
            xtype: 'textareafield',
            name: 'bank_note',
            id: 'bank_note',
            grow: true,
            submitValue: true,
            fieldLabel: '退款資訊'
        },
        {
            xtype: 'datefield',
            name: 'cancel_createdate',
            id: 'cancel_createdate',
            format: 'Y-m-d H:i:s',
            allowBlank: false,
            submitValue: true,
            fieldLabel: '建立時間',
            value: new Date(),
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("cancel_createdate");
                    var end = Ext.getCmp("cancel_updatedate");
                    var s_date = new Date(start.getValue());
                    if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert("提示信息", "歸檔時間不能早於建立時間");
                    }
                }
            }
        },
        {
            xtype: 'datefield',
            name: 'cancel_updatedate',
            id: 'cancel_updatedate',
            submitValue: true,
            allowBlank: false,
            format: 'Y-m-d H:i:s',
            fieldLabel: '歸檔時間',
            value: new Date(),
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("cancel_createdate");
                    var end = Ext.getCmp("cancel_updatedate");
                    var s_date = new Date(start.getValue());
                    if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert("提示信息", "歸檔時間不能早於建立時間");
                    }
                }
            }
        },
        {
            xtype: 'displayfield',
            name: 'cancel_ipfrom',
            id: 'cancel_ipfrom',
            submitValue: true,
            fieldLabel: '修改來源'
        }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                // alert(Ext.htmlEncode(Ext.getCmp('group_id').getValue()));
                var form = this.up('form').getForm();
                var start = Ext.getCmp("cancel_createdate");
                var end = Ext.getCmp("cancel_updatedate");
                if (end.getValue() < start.getValue()) {
                    Ext.Msg.alert("提示信息", "歸檔時間不能早於建立時間");
                    return;
                }
                if (form.isValid()) {
                    form.submit({
                        params: {
                            cancel_id: Ext.htmlEncode(Ext.getCmp('cancel_id').getValue()),
                            order_id: Ext.htmlEncode(Ext.getCmp('order_id').getValue()),
                            cancel_status: Ext.htmlEncode(Ext.getCmp('cancel_status').getValue().Cancel_Status),
                            cancel_note: Ext.htmlEncode(Ext.getCmp('cancel_note').getValue()),
                            bank_note: Ext.htmlEncode(Ext.getCmp('bank_note').getValue()),
                            scancel_createdate: Ext.htmlEncode(Ext.getCmp('cancel_createdate').getValue()),
                            scancel_updatedate: Ext.htmlEncode(Ext.getCmp('cancel_updatedate').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg != undefined) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                    editWin.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    FaresStore.load();
                                    editWin.close();
                                }
                            } else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);

                            Ext.Msg.alert(INFORMATION, "修改失敗!");
                        }
                    });

                }

            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '取消單歸檔',
        iconCls: 'icon-user-edit',
        width: 600,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
                var status = row.data.cancel_status.toString();
                if (status == "0") {
                    Ext.getCmp('id1').setValue(true);
                    Ext.getCmp('id2').setValue(false);
                }
                else if (status == "1") {
                    Ext.getCmp('id1').setValue(false);
                    Ext.getCmp('id2').setValue(true);
                }

            }
        }
    });
    editWin.show();

}