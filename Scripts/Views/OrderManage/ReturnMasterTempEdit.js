editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/OrderManage/UpdateReturnMaster',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '暫存退貨單號',
            id: 'user_return_id',
            margin: '5,0,5,0',
            name: 'user_return_id',
            submitValue: true

        },
        {
            xtype: 'displayfield',
            name: 'detail_id',
            id: 'detail_id',
            submitValue: true,
            margin: '5,0,5,0',
            maxLength: 25,
            fieldLabel: '購物單號'
        },
        {
            xtype: 'displayfield',
            name: 'return_reason',
            id: 'return_reason',
            margin: '5,0,5,0',
            allowBlank: false,
            submitValue: true,
            fieldLabel: '退貨原因'
        },
        {
            xtype: 'fieldcontainer',
            fieldLabel: '狀態',
            id: 'validate',
            margin: '5,0,5,0',
            defaultType: 'radiofield',
            submitValue: true,
            defaults: {
                flex: 1
            },
            layout: 'hbox',
            items: [
                {
                boxLabel: '未歸檔',
                name: 'temp_status',
                inputValue: '0',
                id: 'radio1',
                checked: true
                },
                {
                    boxLabel: '歸檔',
                    name: 'temp_status',
                    inputValue: '1',
                    id: 'radio2'
                },
                {
                    boxLabel: '作廢不處理',
                    name: 'temp_status',
                    inputValue: '2',
                    id: 'radio3'
                }]
        },
        {
            xtype: 'textareafield',
            readOnly: true,
            name: 'user_note',
            id: 'user_note',
            allowBlank: true,
            margin: '5,0,5,0',
            fieldLabel: '備註',
            submitValue: true
        },
        {
            xtype: 'displayfield',
            name: 'user_return_createdates',
            id: 'user_return_createdates',
            fieldLabel: '建立時間'
        },
        {
            xtype: 'displayfield',
            fieldLabel: '更新時間',
            id: 'user_return_updatedates'
        },
        {
            xtype: 'label',
            style: 'color:red',
            text: '*歸檔後即產生實際退貨單，流程不可逆，請小心使用'
        },
        {
            xtype: 'displayfield',
            id: "slave_status",
            name: "slave_status",
            hidden: true
        },
        {
            xtype: 'displayfield',
            id: "return_id",
            name: "return_id",
            hidden: true
        },
        {
            xtype: 'displayfield',
            id: "order_id",
            name: "order_id",
            hidden: true
        },
        {
            xtype: 'displayfield',
            id: "item_vendor_id",
            name: "item_vendor_id"
            ,
            hidden: true
        },
        {
            xtype: 'displayfield',
            id: "return_zip",
            name: "return_zip",
            hidden: true
        },
        {
            xtype: 'displayfield',
            id: "return_address",
            name: "return_address",
            hidden: true
        },
        {
            xtype: 'displayfield',
            id: "return_ipfrom",
            name: "return_ipfrom",
            hidden: true
        }
        ],
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
                                user_return_id: Ext.htmlEncode(Ext.getCmp('user_return_id').getValue()),
                                detail_id: Ext.htmlEncode(Ext.getCmp('detail_id').getValue()),
                                return_reason: Ext.htmlEncode(Ext.getCmp('return_reason').getValue()),
                                user_note: Ext.htmlEncode(Ext.getCmp('user_note').getValue()),
                                user_return_createdates: Ext.htmlEncode(Ext.getCmp('user_return_createdates').getValue()),
                                user_return_updatedates: Ext.htmlEncode(Ext.getCmp('user_return_updatedates').getValue()),
                                order_id: Ext.htmlEncode(Ext.getCmp('order_id').getValue()),
                                item_vendor_id: Ext.htmlEncode(Ext.getCmp('item_vendor_id').getValue()),
                                return_zip: Ext.htmlEncode(Ext.getCmp('return_zip').getValue()),
                                return_address: Ext.htmlEncode(Ext.getCmp('return_address').getValue()),
                                slave_status: Ext.htmlEncode(Ext.getCmp('slave_status').getValue()),
                                radio1: Ext.getCmp('radio1').getValue(),
                                radio2: Ext.getCmp('radio2').getValue(),
                                radio3: Ext.getCmp('radio3').getValue()
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
                                        OrderTempReturnListStore.load();
                                        editWin.close();
                                    }
                                } else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
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
        title: '暫存退貨單',
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
                editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();
}