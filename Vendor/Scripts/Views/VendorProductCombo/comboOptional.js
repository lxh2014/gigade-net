

function createComboOptional() {

    combSpecStroe = Ext.create('Ext.data.Store', {
        id: 'combSpecStroe',
        fields: ['Id', 'Parent_Id', 'Child_Id', 'Product_Name', 'S_Must_Buy', 'G_Must_Buy', 'Pile_Id', 'Buy_Limit'],
        proxy: {
            type: 'ajax',
            url: '/VendorProductCombo/combSpecQuery',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }

    });

    var numSelectStore = Ext.create('Ext.data.Store', {
        id: 'numSelectStore',
        fields: ['value']
    });

    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1,
        listeners: {
            beforeedit: function (e, a) {
                if (isLoad && isEdit == "true") {
                    return false;
                }
                if (numMustBuy == null) {
                    Ext.getCmp('combMustBuy').markInvalid(NOT_NULL);
                    return false;
                }
                if (e.colIdx == 2) {
                }
            },
            edit: function (edit, e) {
                currentRow = e.rowIdx;
                currentCol = e.colIdx;
            }
        }
    });

    //判斷 僅限每種一單位 是否可以被勾選
    function CheckAble() {
        var bool = true;
        var num = Ext.getCmp('comNum').getValue();
        var mustbuy = Ext.getCmp('combMustBuy').getValue();
        if (parseInt(mustbuy) > parseInt(num)) {
            bool = false;
        }
        return bool;
    }


    var optionalPanel = Ext.create('Ext.form.Panel', {
        id: 'optionalPanel',
        margin: '0 0 20 0',
        fieldDefaults: {
            msgTarget: 'side'
        },
        border: false,
        items: [{
            xtype: 'displayfield',
            fieldLabel: COMBO_TYPE,
            colName: 'comboType',
            //  hidden: true,
            value: OPTIONAL_COMBO,
            margin: '0 0 20 0'
        }, {
            xtype: 'panel',
            border: false,
            //  hidden: true,
            id: 'p_comNum',
            items: [{
                xtype: 'combobox',
                id: 'comNum',
                store: numArray,
                editable: false,
                hidden: isEdit == "true",
                fieldLabel: PRODUCT_NUMB + '<span style="color:red;">*</span>',
                colName: 'productNumb',
                allowBlank: false,
                queryMode: 'local',
                displayField: 'value',
                valueField: 'value',
                margin: '0 0 15 0',
                listeners: {
                    select: function (com) {
                        numSelectStore.removeAll();
                        combSpecStroe.removeAll();
                        Ext.getCmp('combMustBuy').setValue('0');
                        //Ext.getCmp('combMustBuy').setMaxValue(com.value);
                        Ext.getCmp('chkNum').setValue(false);
                        for (var i = 1, j = com.value; i <= j; i++) {
                            numSelectStore.add({ value: i });
                            combSpecStroe.add({
                                id: '',
                                Parent_Id: '',
                                Child_Id: '',
                                Product_Name: '',
                                S_Must_Buy: '0',
                                G_Must_Buy: '0',
                                Pile_Id: '',
                                Buy_Limit: ''
                            });
                        }
                        Ext.getCmp('optionalGrid').show();
                    },
                    change: function (com) {
                        //Ext.getCmp('combMustBuy').setMaxValue(com.value);
                    }
                }
            }]

        }, {
            xtype: 'panel',
            border: false,
            hidden: isEdit == 'true',
            id: 'p_combMustBuy',
            items: [{
                xtype: 'numberfield',
                id: 'combMustBuy',
                colName: 'mustBuyNum',
                //  hidden: true,
                fieldLabel: MUST_BUY_NUM + '<span style="color:red;">*</span>',
                minValue: 0,
                decimalPrecision: 0,
                allowBlank: false,
                margin: '0 0 15 0',
                listeners: {
                    change: function (e) {
                        numMustBuy = e.value;
                    }
                }
            }]

        }, {
            xtype: 'panel',
            border: false,
            hidden: isEdit == 'false',
            id: 'p_disMustBuy',
            items: [{
                xtype: 'displayfield',
                id: 'disMustBuy',
                //  hidden: true,
                margin: '0 0 15 0',
                fieldLabel: MUST_BUY_NUM,
                colName: 'mustBuyNum'
            }]

        }, {
            xtype: 'checkbox',
            boxLabel: BUY_LIMIT,
            inputValue: '1',
            id: 'chkNum',
            colName: 'chkNum',
            //  hidden: true,
            margin: '0 0 15 0',
            disabled: isEdit == 'true',
            handler: function (e) {
                if (CheckAble()) {
                    buyLimit = e.value ? 1 : 0;
                    if (e.value) {
                        for (var i = 0, j = combSpecStroe.getCount(); i < j; i++) {
                            if (combSpecStroe.getAt(i).data.S_Must_Buy != 0) {
                                combSpecStroe.getAt(i).set('S_Must_Buy', '1');
                            }
                        }
                        Ext.getCmp('combMustBuy').setMaxValue(Ext.getCmp('comNum').getValue());
                    }
                    else {
                        Ext.getCmp('combMustBuy').setMaxValue(Number.MAX_VALUE);
                        Ext.getCmp('combMustBuy').clearInvalid()
                    }

                }
                else {
                    this.setValue(false);
                    Ext.getCmp('combMustBuy').setMaxValue(Number.MAX_VALUE);
                    Ext.getCmp('combMustBuy').clearInvalid()
                    return false;
                }
            }
        }],
        listeners: {
            afterrender: function (panel) {
                window.parent.updateAuth(panel, 'colName');
            },
            beforerender: function () {
                combSpecStroe.load({
                    params: {
                        ProductId: PRODUCT_ID,
                        OldProductId: window.parent.GetCopyProductId()
                    },
                    callback: function (records, operation, success) {
                        if (records.length > 0) {
                            isLoad = true;
                            Ext.getCmp('optionalGrid').show();
                            Ext.getCmp('comNum').setValue(records.length);
                            Ext.getCmp('disMustBuy').setValue(records[0].data.G_Must_Buy);
                            Ext.getCmp('combMustBuy').setValue(records[0].data.G_Must_Buy);
                            Ext.getCmp('chkNum').setValue(records[0].data.Buy_Limit);
                            numMustBuy = records[0].data.G_Must_Buy;
                            // window.parent.setShow(optionalGrid, 'colName');

                            if (isEdit != "true") {
                                Ext.getCmp('p_comNum').show();
                                Ext.getCmp('p_combMustBuy').show();
                            }
                        }
                        else {
                            Ext.getCmp('p_comNum').show();
                            Ext.getCmp('p_combMustBuy').show();

                        }

                    }
                });
            }
        }

    });

    var optionalGrid = Ext.create('Ext.grid.Panel', {
        id: 'optionalGrid',
        title: '',
        hidden: true,
        store: combSpecStroe,
        height: 200,
        width: 400,
        plugins: [cellEditing],
        listeners: {
            viewready: function (grid) {
                window.parent.updateAuth(grid, 'colName');
            }
        },
        columns: [{ header: PRODUCT_NUM, colName: 'productNum', menuDisabled: true, sortable: false,
            editor: { xtype: 'textfield',
                width: 200,
                allowBlank: false,
                listeners: {
                    blur: function (e) {
                        if (e.value == '') {
                            return;
                        }
                        Ext.Ajax.request({
                            url: '/VendorProductCombo/QueryProduct',
                            method: 'POST',
                            params: {
                                ProductId: e.value,
                                childId: true
                            },
                            success: function (response, opts) {
                                var resText = eval("(" + response.responseText + ")");
                                if (resText != null && resText.data != null) {
                                    productCheck(resText);
                                }
                                else {
                                    combSpecStroe.getAt(currentRow).set('Product_Name', '');
                                }
                            },
                            failure: function (response, opts) {
                                return false;
                            }
                        });
                    }
                }
            }, dataIndex: 'Child_Id'
        },
             { header: PRODUCT_NAME, colName: 'productName', menuDisabled: true, sortable: false, width: 200, dataIndex: 'Product_Name', flex: 1 },
             { header: UNIT_NUM, menuDisabled: true, colName: 'unitNum', sortable: false, width: 100, dataIndex: 'S_Must_Buy', editor: {
                 xtype: 'combobox',
                 queryMode: 'local',
                 store: mustBuyArray,
                 displayField: 'value',
                 valueField: 'value',
                 editable: false,
                 listeners: {
                     beforequery: function () {
                         //                         if (Ext.getCmp('chkNum').getValue()) {
                         //                             for (var i = 0, j = this.getStore().getCount() - 2; i < j; i++) {
                         //                                 this.getStore().removeAt(2);
                         //                             }
                         //                         }
                         //                         else {
                         //                             if (this.getStore().getCount() <= 2) {
                         //                                 this.getStore().removeAll();
                         //                                 this.getStore().add({ field1: '0' }, { field1: '1' }, { field1: '2' }, { field1: '3' }, { field1: '4' }, { field1: '5' }, { field1: '6' }, { field1: '7' }, { field1: '8' }, { field1: '9' }, { field1: '10' });
                         //                             }
                         //                         }
                         this.getStore().removeAll();
                         var t = Ext.getCmp('chkNum').getValue() ? 1 : numMustBuy;
                         for (var i = 0; i <= t; i++) {
                             this.getStore().add({ field1: i });
                         }
                     }
                 }
             }
             },
             { header: 'rid', hidden: true, dataIndex: 'Id'}]
    });

    var optionalViewPort = Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [optionalPanel, optionalGrid],
        padding: 20,
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });

}
