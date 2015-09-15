editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        //defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 50,
        url: '/WareHouse/SaveIpod',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 xtype: 'textfield',
                 fieldLabel: "序號",
                 id: 'row_id',
                 name: 'row_id',
                 hidden: true
             },
             {
                 xtype: 'textfield',
                 name: 'po_id',
                 id: 'po_id',
                 maxLength: 25,
                 allowBlank: false,
                 submitValue: true,
                 fieldLabel: '採購單單號'
             },
            {
                xtype: 'textfield',
                name: 'prod_id',
                id: 'prod_id',
                maxLength: 25,
                allowBlank: false,
                submitValue: true,
                maxLength: 6,
                minLength: 6,
                regex: /^[0-9]*$/,
                regexText:"請輸入數字",
                fieldLabel: '商品細項編號'
            },
            {
                xtype: 'combobox', //類型
                editable: false,
                id: 'ParameterCode',
                fieldLabel: "收貨狀態",
                name: 'ParameterCode',
                store: PlstIdStore,
               // margin: '20 0 0 0',
                lastQuery: '',
                displayField: 'parameterName',
                valueField: 'ParameterCode',
                allowBlank: false,
                emptyText: "请选择收貨狀態",
            },
            {
                xtype: 'numberfield',
                name: 'qty_ord',
                id: 'qty_ord',
                maxLength: 25,
                allowBlank: false,
                submitValue: true,
                allowNegative: false,
                allowDecimals: false,
                minValue: 0,
                fieldLabel: '下單採購量'
            },
            {
                xtype: 'radiogroup',
                id: 'bkord_allow',
                name: 'bkord_allow',
                fieldLabel: "是否允許多次收貨",
                colName: 'bkord',
                defaults: {
                    name: 'bkord'
                },
                columns: 2,
                vertical: true,
                items: [
                    { boxLabel: '是', id: 'bk1', inputValue: '1'},
                    { boxLabel: '否', id: 'bk2', inputValue: '0', checked: true  }
                   
                ]
            },
             {
                 xtype: 'numberfield',
                 name: 'qty_damaged',
                 id: 'qty_damaged',
                 maxLength: 25,
                 allowNegative:false,
                 allowBlank: false,
                 minValue: 0,
                 allowDecimals:false,
                 submitValue: true,
                 fieldLabel: '不允收的量'
             },
              {
                  xtype: 'numberfield',
                  name: 'qty_claimed',
                  id: 'qty_claimed',
                  maxLength: 25,
                  allowBlank: false,
                  submitValue: true,
                  allowNegative: false,
                  allowDecimals: false,
                  minValue: 0,
                  fieldLabel: '實際收貨量'
              },
                {
                    xtype: 'radiogroup',
                    fieldLabel: "是否有效期控管",
                    id: 'pwy_dte_ctl',
                    colName: 'pwy_dte_ctl',
                    name: 'pwy_dte_ctl',
                    defaults: {
                        name: 'pwy_Dte'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                    {
                        id: 'pw1',
                        boxLabel: YES,
                       
                        inputValue: '1',
                        width: 100,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var cde_dt_incr = Ext.getCmp("cde_dt_incr");//有效期天數
                                var cde_dt_var = Ext.getCmp("cde_dt_var");//允收天數
                                var cde_dt_shp = Ext.getCmp("cde_dt_shp");//允出天數
                                if (newValue) {
                                    cde_dt_incr.allowBlank = false;
                                    cde_dt_incr.show();
                                    cde_dt_var.allowBlank = false;
                                    cde_dt_var.show();
                                    cde_dt_shp.allowBlank = false;
                                    cde_dt_shp.show();
                                  
                                }
                            }
                        }
                    },
                    {
                        id: 'pw2',
                        boxLabel: NO,
                        checked: true,
                        inputValue: '0',
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var cde_dt_incr = Ext.getCmp("cde_dt_incr");//有效期天數
                                var cde_dt_var = Ext.getCmp("cde_dt_var");//允收天數
                                var cde_dt_shp = Ext.getCmp("cde_dt_shp");//允出天數
                                if (newValue) {
                                   
                                    cde_dt_incr.setValue("0");
                                    cde_dt_incr.allowBlank = true;
                                    cde_dt_incr.hide();
                                    cde_dt_var.setValue("0");
                                    cde_dt_var.allowBlank = true;
                                    cde_dt_var.hide();
                                    cde_dt_shp.setValue("0");
                                    cde_dt_shp.allowBlank = true;
                                    cde_dt_shp.hide();
                                }
                            }
                        }
                    }]
                },
            {
                xtype: 'combobox', //類型
                editable: false,
                id: 'promo_invs_flg',
                fieldLabel: "品項庫存用途",
                name: 'promo_invs_flg',
                store: PromoInvsFlgStore,
                lastQuery: '',
                displayField: 'parameterName',
                valueField: 'ParameterCode',
                listeners: {
                    beforerender: function () {
                        PromoInvsFlgStore.load({
                            callback: function () {
                                PromoInvsFlgStore.insert(0, { ParameterCode: ' ', parameterName: '請選擇' });
                                if (row.data.promo_invs_flg.trim() != "") {
                                    Ext.getCmp("promo_invs_flg").setValue(row.data.promo_invs_flg);
                                } else {
                                    Ext.getCmp("promo_invs_flg").setValue(PromoInvsFlgStore.data.items[0].data.ParameterCode);
                                }
                            }
                        });
                    }
                }
                
            },
             {
                 xtype: 'numberfield',
                 name: 'req_cost',
                 id: 'req_cost',
                 maxLength: 25,
                 allowBlank: false,
                 submitValue: true,
                 minValue: 0,
                
                 fieldLabel: '原訂單價格'
             },
              {
                  xtype: 'numberfield',
                  name: 'off_invoice',
                  id: 'off_invoice',
                  maxLength: 25,
                  allowBlank: false,
                  minValue: 0,
                  maxValue: 100,
                  submitValue: true,
                
                  fieldLabel: '折扣率'
              },
             {
                   xtype: 'numberfield',
                   name: 'freight_price',
                   id: 'freight_price',
                   maxLength: 25,
                   minValue: 0,
                   allowBlank: false,
                   submitValue: true,
                   allowDecimals: false,
                   fieldLabel: '運費'
               },
                {
                    xtype: 'numberfield',
                    name: 'cde_dt_incr',
                    id: 'cde_dt_incr',
                    maxLength: 25,
                    allowBlank: true,
                    allowNegative: false,
                    allowDecimals: false,
                    minValue: 0,
                    submitValue: true,
                    fieldLabel: '有效期天數',
                    hidden: true
                },
                {
                    xtype: 'numberfield',
                    name: 'cde_dt_var',
                    id: 'cde_dt_var',
                    maxLength: 25,
                    allowBlank: true,
                    allowNegative: false,
                    allowDecimals: false,
                    submitValue: true,
                    fieldLabel: '允收天數',
                    minValue: 0,
                    hidden: true
                },
                {
                    xtype: 'numberfield',
                    name: 'cde_dt_shp',
                    id: 'cde_dt_shp',
                    maxLength: 25,
                    allowBlank: true,
                    allowNegative: false,
                    allowDecimals: false,
                    minValue: 0,
                    submitValue: true,
                    fieldLabel: '允出天數',
                    hidden: true
                }
            
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: SAVE,
                handler: function () {
                    var form = this.up('form').getForm();
                   
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                                po_id: Ext.htmlEncode(Ext.getCmp('po_id').getValue()),
                                prod_id: Ext.htmlEncode(Ext.getCmp('prod_id').getValue()),

                                plst_id: Ext.htmlEncode(Ext.getCmp('ParameterCode').getValue()),
                                qty_ord: Ext.htmlEncode(Ext.getCmp('qty_ord').getValue()),
                                bkord: Ext.htmlEncode(Ext.getCmp("bkord_allow").getValue().bkord),
                                qty_damaged: Ext.htmlEncode(Ext.getCmp('qty_damaged').getValue()),
                                qty_claimed: Ext.htmlEncode(Ext.getCmp('qty_claimed').getValue()),
                                pwy_dt: Ext.htmlEncode(Ext.getCmp("pwy_dte_ctl").getValue().pwy_Dte),

                                promo_invs_flg: Ext.htmlEncode(Ext.getCmp('promo_invs_flg').getValue()),

                                req_cost: Ext.htmlEncode(Ext.getCmp('req_cost').getValue()),
                                off_invoice: Ext.htmlEncode(Ext.getCmp('off_invoice').getValue()),
                                freight_price: Ext.htmlEncode(Ext.getCmp('freight_price').getValue()),
                                cde_dt_incr: Ext.htmlEncode(Ext.getCmp('cde_dt_incr').getValue()),
                                cde_dt_var: Ext.htmlEncode(Ext.getCmp('cde_dt_var').getValue()),
                                cde_dt_shp: Ext.htmlEncode(Ext.getCmp('cde_dt_shp').getValue()),
                                
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);  //Ext.decode 把json格式的字符串轉化為json對象
                                if (result.success) {
                                    if (result.msg != undefined) {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                    }
                                    editWin.close();
                                    IpodStore.load();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, result.Msg);
                                    IpodStore.load();
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.msg != undefined) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            }
                        });
                    }
                }
            }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '採購單單身編輯',
        iconCls: row ? 'icon-user-edit' : 'icon-user-add',
        id: 'editWin',
        width: 400,
        height: 450,
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
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (row != null) {
                    editFrm.getForm().loadRecord(row);
                    Ext.getCmp('po_id').setDisabled(true);
                    if (row.data.bkord_allow == "Y") {
                        Ext.getCmp('bk1').setValue(true);
                        Ext.getCmp('bk2').setValue(false);
                    } else
                    {
                        Ext.getCmp('bk1').setValue(false);
                        Ext.getCmp('bk2').setValue(true);
                    }
                       
                    if (row.data.pwy_dte_ctl == "Y")//有效期控管
                    {
                        Ext.getCmp('pw1').setValue(true);
                        Ext.getCmp('pw2').setValue(false);
                        var cde_dt_incr = Ext.getCmp("cde_dt_incr");//有效期天數
                        var cde_dt_var = Ext.getCmp("cde_dt_var");//允收天數
                        var cde_dt_shp = Ext.getCmp("cde_dt_shp");//允出天數
                        cde_dt_incr.allowBlank = false;
                        cde_dt_incr.show();
                        cde_dt_var.allowBlank = false;
                        cde_dt_var.show();
                        cde_dt_shp.allowBlank = false;
                        cde_dt_shp.show();
                    } else
                    {
                        Ext.getCmp('pw1').setValue(false);
                        Ext.getCmp('pw2').setValue(true);
                        var cde_dt_incr = Ext.getCmp("cde_dt_incr");//有效期天數
                        var cde_dt_var = Ext.getCmp("cde_dt_var");//允收天數
                        var cde_dt_shp = Ext.getCmp("cde_dt_shp");//允出天數
                        cde_dt_incr.setValue("0");
                        cde_dt_incr.allowBlank = true;
                        cde_dt_incr.hide();
                        cde_dt_var.setValue("0");
                        cde_dt_var.allowBlank = true;
                        cde_dt_var.hide();
                        cde_dt_shp.setValue("0");
                        cde_dt_shp.allowBlank = true;
                        cde_dt_shp.hide();
                    }
                }
            }
        }
    });
    editWin.show();
}