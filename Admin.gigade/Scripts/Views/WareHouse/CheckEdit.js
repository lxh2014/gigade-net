CheckEditFunction = function (row, store)
{
    var IpoFrm = Ext.create('Ext.form.Panel', {
        id: 'IpoFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/WareHouse/UpdateIpodCheck',
        defaults: { anchor: "99%" },
        items: [
        {
            xtype: 'textfield',
            id: 'row_id',
            name: 'row_id',
            hidden: true,
            allowBlank: true,
            submitValue: true
        },
        {
            xtype: 'displayfield',
            id: 'prod_id',
            name: 'prod_id',
            fieldLabel: '商品細項編號',
            allowBlank: false,
            submitValue: true
        },
        {
            xtype: 'displayfield',
            id: 'product_name',
            name: 'product_name',
            fieldLabel: '商品名稱',
            allowBlank: false,
            submitValue: true
        },
        {
            xtype: 'displayfield',
            id: 'qty_ord',
            name: 'qty_ord',
            fieldLabel: '下單採購量',
            allowBlank: false,
            submitValue: true
        },
        {
            xtype: 'displayfield',
            id: 'item_stock',
            name: 'item_stock',
            fieldLabel: '前台庫存',
            allowBlank: false
        },
        {
            xtype: 'numberfield',
            name: 'qty_damaged',
            id: 'qty_damaged',
            maxValue: 99999,
            allowBlank: false,
            allowNegative: false,
            allowDecimals: false,
            minValue: 0,
            fieldLabel: '不允收的量'
        },
        {
            xtype: 'numberfield',
            name: 'qty_claimed',
            id: 'qty_claimed',
            maxValue: 99999,
            allowBlank: false,
            allowNegative: false,
            allowDecimals: false,
            minValue: 0,
            fieldLabel: '允收數量'
        },
        {
            xtype: 'datefield',
            id: 'made_date',
            name: 'made_date',
            fieldLabel: '製造日期',
            format: 'Y-m-d',
            allowBlank: false,
            submitValue: true,
            editable: false,
            listeners: {
                select: function (a, b, c)
                {
                    var start = Ext.getCmp("made_date");
                    var end = Ext.getCmp("cde_dt");
                    var date = new Date(start.getValue());
                    end.setValue(new Date(date.setDate(date.getDate() + row.data.cde_dt_incr)));
                }
            }
        },
        {
            xtype: 'datefield',
            id: 'cde_dt',
            name: 'cde_dt',
            fieldLabel: '有效日期',
            format: 'Y-m-d',
            allowBlank: false,
            submitValue: true,
            editable: false,
            listeners: {
                select: function (a, b, c)
                {
                    var start = Ext.getCmp("made_date");
                    var end = Ext.getCmp("cde_dt");
                    var date = new Date(end.getValue());
                    start.setValue(new Date(date.setDate(date.getDate() - row.data.cde_dt_incr)));
                }
            }
        },
         
        ],
        buttons: [
        {
            xtype: 'button',
            vtype: 'submit',
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function ()
            {
                var form = this.up('form').getForm();
                if (form.isValid())
                {
                    Ext.Msg.confirm("提示信息", "確認是否保存？", function (btn)
                    {
                        if (btn == "yes")
                        {
                            var qty_damaged = Ext.getCmp('qty_damaged').getValue();
                            var qty_claimed = Ext.getCmp('qty_claimed').getValue();
                            var qty_ord = Ext.getCmp('qty_ord').getValue();
                            var item_stock = parseInt(Ext.getCmp('item_stock').getValue()) + parseInt(qty_claimed);

                            if (parseInt(qty_claimed) == 0 && parseInt(qty_damaged) == 0)
                            {
                                ;
                            }
                            else if (parseInt(qty_claimed) > parseInt(qty_ord))
                            {
                                Ext.Msg.alert("錯誤提示", "允收數量不能大於下單採購量,保存失敗！");
                                return false;
                            }

                            form.submit({
                                params: {
                                    row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                                    qty_damaged: Ext.htmlEncode(Ext.getCmp('qty_damaged').getValue()),
                                    qty_claimed: Ext.htmlEncode(Ext.getCmp('qty_claimed').getValue()),
                                    item_stock: item_stock,
                                    plst_id: "F",
                                    made_date: Ext.getCmp('made_date').getValue(),
                                    cde_dt: Ext.getCmp('cde_dt').getValue(),
                                    item_id: Ext.getCmp('prod_id').getValue(),
                                    ipo_id: row.data.po_id,
                                    //out_qty:
                                    //com_qty:
                                    cde_dt:Ext.getCmp('cde_dt').getValue(),
                                    made_date:Ext.getCmp('made_date').getValue()


                                },
                                success: function (form, action)
                                {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success)
                                    {
                                        Ext.Msg.alert("提示信息", "驗收成功!");
                                        IpodStore.load();
                                        editDisKeyWordsWin.close();
                                    }
                                    else
                                    {
                                        Ext.Msg.alert("提示信息", "驗收失敗!");
                                        IpodStore.load();
                                    }

                                },
                                failure: function ()
                                {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }


                            });
                        }
                    });
                }
            }
        }]
    });
    var editDisKeyWordsWin = Ext.create('Ext.window.Window', {
        title: '採購單驗收',
        iconCls: 'icon-user-edit',
        id: 'editDisKeyWordsWin',
        width: 420,
        height: 320,
        layout: 'fit',
        items: [IpoFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
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
                        Ext.getCmp('editDisKeyWordsWin').destroy();
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
                    IpoFrm.getForm().loadRecord(row);
                    if (row.data.made_date == "0001-01-01")
                    {
                        Ext.getCmp('made_date').setValue("");
                    }
                    if (row.data.cde_dt == "0001-01-01")
                    {
                        Ext.getCmp('cde_dt').setValue("");
                    }
                    if (row.data.pwy_dte_ctl != "Y")
                    {
                        Ext.getCmp('made_date').setDisabled(true);
                        Ext.getCmp('cde_dt').setDisabled(true);
                    }
                }
            }
        }
    });
    editDisKeyWordsWin.show();
}