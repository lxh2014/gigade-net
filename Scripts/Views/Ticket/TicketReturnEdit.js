editFunction = function (row, store) {

    Ext.define('GIGADE.reason_type', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'parameterCode', type: 'string' },
            { name: 'parameterName', type: 'string' }
        ]
    });
    var reasonTypeStore = Ext.create("Ext.data.Store", {
        autoDestroy: true,
        model: 'GIGADE.reason_type',
        proxy: {
            type: 'ajax',
            url: '/Ticket/GetReasonTypeStore',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        url: '/Ticket/SaveTicketReturn',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '編號',
                id: 'tr_id',
                name: 'tr_id',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '訂單編號',
                id: 'ticket_master_id',
                name: 'ticket_master_id',
            },
            {
                xtype: 'textfield',
                fieldLabel: '備註',
                id: 'tr_note',
                name: 'tr_note',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '銀行資訊',
                id: 'tr_bank_note',
                name: 'tr_bank_note',
                value: 0,

            },
             {
                 xtype: 'displayfield',
                 fieldLabel: '來源IP',
                 id: 'tr_ipfrom',
                 name: 'tr_ipfrom',
                 allowBlank: false
             },
            {
                xtype: 'numberfield',
                fieldLabel: '退款金額',
                id: 'tr_money',
                name: 'tr_money',
                minValue:0,
                allowDecimals:false,
                allowBlank: false,
            },
             {
                 xtype: 'numberfield',
                 fieldLabel: '原本金額',
                 id: 'trcl_last_money',
                 name: 'trcl_last_money',
                 hidden:true,
             },

               {
                   xtype: 'combobox',
                   fieldLabel: '退款類型',
                   id: 'tr_reason_type',
                   name: 'tr_reason_type',
                   store: reasonTypeStore,
                   valueField: 'parameterCode',
                   displayField: 'parameterName',
                   value: '1',
                   editable: false,
                   allowBlank:false,
               },
               {
                   xtype: 'displayfield',
                   fieldLabel:'退款狀態',
                   id: 'tr_status',
                   name: 'tr_status',
                   hidden: true
               },
                 {
                     xtype: 'displayfield',
                     fieldLabel: '原本狀態',
                     id: 'trcl_last_status',
                     name: 'trcl_last_status',
                      hidden: true
                 },
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            form.submit({
                                params: {
                                    tr_id: Ext.htmlEncode(Ext.getCmp('tr_id').getValue()),
                                    ticket_master_id: Ext.htmlEncode(Ext.getCmp('ticket_master_id').getValue()),
                                    tr_note: Ext.htmlEncode(Ext.getCmp('tr_note').getValue()),
                                    tr_bank_note: Ext.htmlEncode(Ext.getCmp('tr_bank_note').getValue()),
                                    tr_ipfrom: Ext.htmlEncode(Ext.getCmp('tr_ipfrom').getValue()),
                                    tr_money: Ext.htmlEncode(Ext.getCmp('tr_money').getValue()),
                                    trcl_last_money: Ext.htmlEncode(Ext.getCmp('trcl_last_money').getValue()),
                                    tr_status: Ext.htmlEncode(Ext.getCmp('tr_status').getValue()),
                                    trcl_last_status: Ext.htmlEncode(Ext.getCmp('trcl_last_status').getValue()),
                                    tr_reason_type: Ext.htmlEncode(Ext.getCmp('tr_reason_type').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                        EdmContentStore.load();
                                        editWin.close();
                                    }
                                }
                            });
                        }
                    }
            },
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '退款單編輯',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        height: 239,
        width: 350,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //  resizable: false,
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
         }
        ],
        listeners: {
            'beforerender': function () {
                if (row) {
                }
            },
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    initRow(row);
                }
                else {
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function initRow(row) {
        var money = Ext.getCmp('tr_money').getValue();
        var status = Ext.getCmp('tr_status').getValue();
        Ext.getCmp('trcl_last_money').setValue(money);
        Ext.getCmp('trcl_last_status').setValue(status);
    }
}