editFunction = function (row, store) {
    var type;
    //配送區域
    var logisticsAreaStore = Ext.create('Ext.data.Store', {
        autoLoad: false,
        model: 'gigade.logisticsName1',
        proxy: {
            type: 'ajax',
            url: '/Logistics/GetLogisticsArea',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
     
    //配送模式
    var logisticsTypeStore = Ext.create('Ext.data.Store', {
        autoLoad: false,
        model: 'gigade.logisticsName1',
        proxy: {
            type: 'ajax',
            url: '/Logistics/GetLogisticsType',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    logisticsAreaStore.load();


    Ext.define("gigade.logisticsName", {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'parameterCode', type: 'string' },
            { name: 'parameterName', type: 'string' }]
    });
   logisticsTypeStore.load({ params: { ltype: type } });
    //搜索框物流數據
    var logisticsNameStore = Ext.create('Ext.data.Store', {
     //   autoDestroy: true,
        model: 'gigade.logisticsName',
        proxy: {
            type: 'ajax',
            url: '/Logistics/GetLogisticsName',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }

    });
    logisticsNameStore.load();
    //常低溫
    var logisticsOftenLowStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        fields: ['name', 'value'],
        data: [{ name: "常溫", value: 1 }, { name: "低溫", value: 2 }]
    });
    //收費方式
    var logisticsChargeMethodStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        fields: ['name', 'value'],
        data: [{ name: "固定", value: 1 }, { name: "累加", value: 2 }]
    });

    var editLogisticsFrm = Ext.create('Ext.form.Panel', {
        id: 'editLogisticsFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        url: '/Logistics/LogisticsSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                id: 'rid',
                name: 'rid',
                fieldLabel: 'rid',
                value: '',
                hidden: true
            },
            {
                xtype: 'combobox',
                fieldLabel: "物流",
                id: 'delivery_store_id',
                name: 'delivery_store_id',
                labelWidth: 60,
                lastQuery: '',
                store: logisticsNameStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                value: 1
            },
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: "配送區域",
                lastQuery: "",
                labelWidth: 60,
                id: 'freight_big_area',
                name: 'freight_big_area',
                store: logisticsAreaStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                triggerAction: 'all',
                value: "",
                listeners: {
                    "select": function () {
                        var area = Ext.getCmp('freight_big_area');
                        var types = Ext.getCmp('freight_type');
                        if (area.getValue() != undefined && types.getValue() != undefined) {
                            types.setDisabled(false);
                        }
                        type = Ext.getCmp("freight_big_area").getValue();
                        types.clearValue();
                        logisticsTypeStore.removeAll();

                    }
                }
            }, {
                xtype: 'combobox',
                editable: false,
                fieldLabel: "配送方式",
                labelWidth: 60,
                lastQuery: "",
                id: 'freight_type',
                name: 'freight_type',
                store: logisticsTypeStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                value: "",
                listeners: {
                    'beforequery': function (qe) {
                        logisticsTypeStore.load({ params: { ltype: type } });
                    }
                }
            }, {
                xtype: 'combobox',
                id: 'delivery_freight_set',
                name: 'delivery_freight_set',
                fieldLabel: '常溫/低溫',
                labelWidth: 60,
                queryMode: 'local',
                editable: false,
                disabled: false,
                store: logisticsOftenLowStore,
                displayField: 'name',
                valueField: 'value',
                value: 1,
                allowBlank: false
            }, {
                xtype: 'combobox',
                id: 'charge_type',
                name: 'charge_type',
                fieldLabel: '收費方式',
                labelWidth: 60,
                queryMode: 'local',
                editable: false,
                store: logisticsChargeMethodStore,
                displayField: 'name',
                valueField: 'value',
                value: ""
            },

            {
                xtype: 'numberfield',
                fieldLabel: '運費',
                labelWidth: 60,
                id: 'shipping_fee',
                name: 'shipping_fee',
                submitValue: true,
                minValue: 0,
                value: 0,
                allowDecimals: false,
                allowBlank: false
            }, {
                xtype: 'numberfield',
                fieldLabel: '逆運費',
                labelWidth: 60,
                id: 'return_fee',
                name: 'return_fee',
                submitValue: true,
                minValue: 0,
                value: 0,
                allowDecimals: false,
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: NOTE,
                labelWidth: 60,
                id: 'note',
                name: 'note'
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 width: 500,
                 items: [
                     {
                         xtype: 'displayfield',
                         width: 100,
                         value: '是否貨到付款:'
                     },
                     {
                         xtype: 'radiogroup',
                         allowBlank: false,
                         columns: 2,
                         width: 400,
                         id: 'payver',
                         colName: 'payver',
                         vertical: true,
                         name: 'payver',
                         margin: '1 0 1 25',
                         items: [{
                             boxLabel: '是',
                             name: 'payver',
                             id: 'yc',
                             checked: true,
                             inputValue: 1
                         },
                         {
                             boxLabel: '否',
                             name: 'payver',
                             id: 'nc',
                             inputValue: 0
                         }]
                     }
                 ]
             },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 width: 500,
                 items: [
                     {
                         xtype: 'displayfield',
                         width: 100,
                         value: '是否有size限制:'
                     },
                     {
                         xtype: 'radiogroup',
                         hidden: false,
                         id: 'sizever',
                         name: 'sizever',
                         colName: 'sizever',
                         columns: 2,
                         vertical: true,
                         width: 400,
                         margin: '1 0 1 25',
                         items: [{
                             boxLabel: '是',
                             name: 'sizever',
                             id: 'yes',
                             checked: true,
                             inputValue: 1,
                             listeners: {
                                 change: function (radio, newValue, oldValue) {
                                     var length = Ext.getCmp("len");
                                     var width = Ext.getCmp("wid");
                                     var tall = Ext.getCmp("hei");
                                     var weight = Ext.getCmp("wei");
                                     if (newValue) {
                                         length.allowBlank = false;
                                         length.setDisabled(false);
                                         width.allowBlank = false;
                                         width.setDisabled(false);
                                         tall.allowBlank = false;
                                         tall.setDisabled(false);
                                         weight.allowBlank = false;
                                         weight.setDisabled(false);
                                     }
                                 }
                             }
                         },
                         {
                             boxLabel: '否',
                             name: 'sizever',
                             id: 'no',
                             inputValue: 0,
                             listeners: {
                                 change: function (radio, newValue, oldValue) {
                                     var length = Ext.getCmp("len");
                                     var width = Ext.getCmp("wid");
                                     var tall = Ext.getCmp("hei");
                                     var weight = Ext.getCmp("wei");
                                     if (newValue) {
                                         length.allowBlank = true;
                                         length.setDisabled(true);
                                         width.allowBlank = true;
                                         width.setDisabled(true);
                                         tall.allowBlank = true;
                                         tall.setDisabled(true);
                                         weight.allowBlank = true;
                                         weight.setDisabled(true);
                                     }
                                 }
                             }
                         }]
                     }

                 ]
             },
                   {
                       xtype: 'numberfield',
                       fieldLabel: '限長',
                       labelWidth: 60,
                       id: 'len',
                       name: 'len',
                       submitValue: true,
                       minValue: 0,
                       allowDecimals: false,
                       value: 0,
                       allowBlank: false
                   }, {
                       xtype: 'numberfield',
                       fieldLabel: '限寬',
                       labelWidth: 60,
                       id: 'wid',
                       name: 'wid',
                       submitValue: true,
                       minValue: 0,
                       allowDecimals: false,
                       value: 0,
                       allowBlank: false
                   }, {
                       xtype: 'numberfield',
                       fieldLabel: '限高',
                       labelWidth: 60,
                       id: 'hei',
                       name: 'hei',
                       submitValue: true,
                       minValue: 0,
                       allowDecimals: false,
                       value: 0,
                       allowBlank: false
                   }, {
                       xtype: 'numberfield',
                       fieldLabel: '限重',
                       labelWidth: 60,
                       id: 'wei',
                       name: 'wei',
                       submitValue: true,
                       minValue: 0,
                       allowDecimals: false,
                       value: 0,
                       allowBlank: false
                   }

        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            dsid: Ext.getCmp("rid").getValue(),
                            dsid: Ext.getCmp("delivery_store_id").getValue(),
                            fbarea: Ext.getCmp("freight_big_area").getValue(),
                            ftype: Ext.getCmp("freight_type").getValue(),
                            dfset: Ext.getCmp("delivery_freight_set").getValue(),
                            ctype: Ext.getCmp("charge_type").getValue(),
                            sfree: Ext.getCmp("shipping_fee").getValue(),
                            rfree: Ext.getCmp("return_fee").getValue(),
                            slimit: Ext.getCmp("sizever").getValue(),
                            length: Ext.getCmp("len").getValue(),
                            width: Ext.getCmp("wid").getValue(),
                            height: Ext.getCmp("hei").getValue(),
                            weight: Ext.getCmp("wei").getValue(),
                            pod: Ext.getCmp("payver").getValue(),
                            note: Ext.getCmp("note").getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.res == 0) {
                                    Ext.Msg.alert(INFORMATION, "添加信息已存在!");
                                }
                                else if (result.res == 1) {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    LogisticsStore.load();
                                    editUserWin.close();
                                }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                editUserWin.close();
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            editUserWin.close();
                        }
                    })
                }
            }
        }]
    });
    var editUserWin = Ext.create('Ext.window.Window', {
        id: 'editLogisticsWin',
        title: '物流公司',
        width: 500,
        iconCls: 'icon-user-edit',
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [editLogisticsFrm],
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
                         editUserWin.destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                if (row != null) {
                    editLogisticsFrm.getForm().loadRecord(row);
                    Ext.getCmp("delivery_store_id").setDisabled(true);
                    Ext.getCmp("delivery_freight_set").setDisabled(true);
                    if (row.data.size_limitation == 0) {
                        Ext.getCmp("no").setValue(true);
                    }
                    if (row.data.size_limitation == 1) {
                        Ext.getCmp("yes").setValue(true);
                    }
                    if (row.data.pod == 0) {
                        Ext.getCmp("nc").setValue(true);
                    }
                    if (row.data.pod == 1) {
                        Ext.getCmp("yc").setValue(true);
                    }
                    if (row.data.freight_big_area == 0) {
                        Ext.getCmp("freight_big_area").setValue("");
                    }
                    if (row.data.freight_type == 0) {
                        Ext.getCmp("freight_type").setValue("");
                    }
                    if (row.data.delivery_freight_set == 0) {
                        Ext.getCmp("delivery_freight_set").setValue("");
                    }
                    if (row.data.charge_type == 0) {
                        Ext.getCmp("charge_type").setValue("");
                    }
                    type = row.data.freight_big_area;
                }
            }
        }
    });
    editUserWin.show();

}
