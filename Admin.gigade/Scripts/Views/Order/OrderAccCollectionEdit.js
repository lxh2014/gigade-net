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
        url: '/OrderAccountCollection/SaveOrEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  xtype: 'textfield',
                  fieldLabel: '',
                  id: 'row_id',
                  name: 'row_id',
                  hidden: true
              },
            {
                xtype: 'textfield',
                fieldLabel: '訂單',
                id: 'order_id',
                name: 'order_id',
                allowBlank: false,
                regex: /^[-+]?([1-9]\d*|0)$/,
                regexText: '請輸入數字',
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('order_id').getValue();
                        Ext.Ajax.request({
                            url: "/OrderAccountCollection/YesOrNoOrderId",
                            method: 'post',
                            type: 'text',
                            params: {
                                order_id: Ext.getCmp('order_id').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    Ext.getCmp('order_status').setValue("訂單存在");
                                    if (result.msg == 1 && Ext.getCmp("row_id").getValue() == "") {
                                        Ext.MessageBox.confirm(CONFIRM, "該筆訂單已入賬，是否關閉窗口？", function (btn) {
                                            if (btn == "yes") {
                                                Ext.getCmp('editWin').destroy();
                                            }
                                            else {
                                                Ext.getCmp('order_id').setValue("");
                                                return false;
                                            }
                                        });
                                    }
                                }
                                else {
                                    Ext.getCmp('order_status').setValue("訂單不存在");

                                }
                            }
                        });
                    }
                }
            },
             {
                 xtype: 'displayfield',
                 name: 'order_status',
                 id: 'order_status',
                 fieldLabel: '訂單狀態',
                 allowBlank: false
             },
        {
            xtype: 'datetimefield',
            name: 'account_collection_time',
            id: 'account_collection_time',
            allowBlank: false,
            format: 'Y-m-d',
            maxLength: 25,
            value: new Date(),
            fieldLabel: '入賬時間'
        },
        {
            xtype: 'numberfield',
            name: 'account_collection_money',
            id: 'account_collection_money',
            allowDecimals: false,
            allowBlank: false,
            value: 0,
            fieldLabel: '入賬金額'
        },
        //{
        //    xtype: 'numberfield',
        //    name: 'amount_account_collection_money',
        //    id: 'amount_account_collection_money',
        //    allowDecimals: false,
        //    allowBlank: false,
        //    value: 0,
        //    fieldLabel: '入賬總額'
        //},
        //{
        //    xtype: 'numberfield',
        //    name: 'amount_invoice',
        //    id: 'amount_invoice',
        //    allowDecimals: false,
        //    allowBlank: false,
        //    value: 0,
        //    fieldLabel: '發票總額'
        //},
        {
            xtype: 'numberfield',
            name: 'poundage',
            id: 'poundage',
            allowDecimals: false,
            allowBlank: false,
            value: 0,
            fieldLabel: '手續費'
        },


         {
             xtype: 'datetimefield',
             name: 'return_collection_time',
             id: 'return_collection_time',
             format: 'Y-m-d',
             maxLength: 25,
             fieldLabel: '退貨時間'
         },
        {
            xtype: 'numberfield',
            name: 'return_collection_money',
            id: 'return_collection_money',
            allowDecimals: false,
            value: 0,
            fieldLabel: '退貨金額'
        },
         {
             xtype: 'numberfield',
             name: 'return_poundage',
             id: 'return_poundage',
             allowDecimals: false,
             value: 0,
             fieldLabel: '退貨手續費'
         },
        {
            xtype: 'textfield',
            name: 'remark',
            id: 'remark',
            allowBlank: true,
            submitValue: true,
            fieldLabel: '備註'
        }],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                if (row == null)//空表示新增
                {
                    if (Ext.getCmp('order_status').getValue() == "訂單存在") {
                        if (form.isValid()) {
                            form.submit({
                                params: {
                                    row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                                    order_id: Ext.htmlEncode(Ext.getCmp('order_id').getValue()),
                                    account_collection_time: Ext.htmlEncode(Ext.getCmp('account_collection_time').getValue()),
                                    account_collection_money: Ext.htmlEncode(Ext.getCmp('account_collection_money').getValue()),
                                    //amount_account_collection_money: Ext.htmlEncode(Ext.getCmp('amount_account_collection_money').getValue()),
                                    //amount_invoice: Ext.htmlEncode(Ext.getCmp('amount_invoice').getValue()),
                                    poundage: Ext.htmlEncode(Ext.getCmp('poundage').getValue()),
                                    return_collection_time: Ext.htmlEncode(Ext.getCmp('return_collection_time').getValue()),
                                    return_collection_money: Ext.htmlEncode(Ext.getCmp('return_collection_money').getValue()),
                                    return_poundage: Ext.htmlEncode(Ext.getCmp('return_poundage').getValue()),
                                    remark: Ext.htmlEncode(Ext.getCmp('remark').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, "保存成功! ");
                                        editWin.close();
                                        OrderMasterExportStore.load();

                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                        editWin.close();
                                        OrderMasterExportStore.load();
                                    }
                                },
                                failure: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                }
                            });
                        }
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, "訂單不存在");
                    }
                }
                else {//如果是編輯
                    if (Ext.getCmp('order_id').getValue() == row.data.order_id) {
                        if (form.isValid()) {
                            form.submit({
                                params: {
                                    row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                                    order_id: Ext.htmlEncode(Ext.getCmp('order_id').getValue()),
                                    account_collection_time: Ext.htmlEncode(Ext.getCmp('account_collection_time').getValue()),
                                    account_collection_money: Ext.htmlEncode(Ext.getCmp('account_collection_money').getValue()),
                                    //amount_account_collection_money: Ext.htmlEncode(Ext.getCmp('amount_account_collection_money').getValue()),
                                    //amount_invoice: Ext.htmlEncode(Ext.getCmp('amount_invoice').getValue()),
                                    poundage: Ext.htmlEncode(Ext.getCmp('poundage').getValue()),
                                    return_collection_time: Ext.htmlEncode(Ext.getCmp('return_collection_time').getValue()),
                                    return_collection_money: Ext.htmlEncode(Ext.getCmp('return_collection_money').getValue()),
                                    return_poundage: Ext.htmlEncode(Ext.getCmp('return_poundage').getValue()),
                                    remark: Ext.htmlEncode(Ext.getCmp('remark').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, "保存成功! ");
                                        editWin.close();
                                        OrderMasterExportStore.load();

                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                        editWin.close();
                                        OrderMasterExportStore.load();
                                    }
                                },
                                failure: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                }
                            });
                        }
                    }
                    else {
                        if (Ext.getCmp('order_status').getValue() == "訂單存在") {
                            if (form.isValid()) {
                                form.submit({
                                    params: {
                                        row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                                        order_id: Ext.htmlEncode(Ext.getCmp('order_id').getValue()),
                                        account_collection_time: Ext.htmlEncode(Ext.getCmp('account_collection_time').getValue()),
                                        account_collection_money: Ext.htmlEncode(Ext.getCmp('account_collection_money').getValue()),
                                        poundage: Ext.htmlEncode(Ext.getCmp('poundage').getValue()),
                                        return_collection_time: Ext.htmlEncode(Ext.getCmp('return_collection_time').getValue()),
                                        return_collection_money: Ext.htmlEncode(Ext.getCmp('return_collection_money').getValue()),
                                        return_poundage: Ext.htmlEncode(Ext.getCmp('return_poundage').getValue()),
                                        remark: Ext.htmlEncode(Ext.getCmp('remark').getValue())
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert(INFORMATION, "保存成功! ");
                                            editWin.close();
                                            OrderMasterExportStore.load();

                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                            editWin.close();
                                            OrderMasterExportStore.load();
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                    }
                                });
                            }
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "訂單不存在");
                        }
                    }
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '新增/修改',
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
                if (row != null) {
                    editFrm.getForm().loadRecord(row);
                }

            }
        }
    });
    editWin.show();
}