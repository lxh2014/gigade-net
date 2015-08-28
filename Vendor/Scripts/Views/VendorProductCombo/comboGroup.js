
function createComboGroup() {

    var topPanel;
    var bottomPanel;
    var numPanel;
    var gridPanel;
    var viewPort;

    //判斷 僅限一單位是否可以被選中
    function CheckAble() {
        var bool = true;
        for (var i = 1; i <= groupNum; i++) {
            var nId = 'N_' + i;
            var mustId = 'M_' + i;
            var num = Ext.getCmp(nId).getValue();
            var mustBuyNum = Ext.getCmp(mustId).getValue();
            if (parseInt(mustBuyNum) > parseInt(num)) {
                bool = false;
                break;
            }
        }
        return bool;
    }

    //為需選擇數量設定最大值  maxType:為1時最大值為商品數量，為2時最大值為Number的最大值 
    function SetMax(maxType) {
        for (var i = 1; i <= groupNum; i++) {
            var nId = 'N_' + i;
            var mustId = 'M_' + i;
            var num = Ext.getCmp(nId).getValue();
            Ext.getCmp(mustId).setMaxValue(maxType == 1 ? num : Number.MAX_VALUE);

        }
    }

    /*創建Grid*/
    function createGridPanel(index, value, pileId) {


        if (Ext.getCmp('G_' + index)) {
            Ext.getCmp('G_' + index).getStore().removeAll();
            for (var i = 0; i < value; i++) {
                Ext.getCmp('G_' + index).getStore().add({
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
            return;
        }
        var specStore = Ext.create('Ext.data.Store', {
            id: '' + index + '_S',
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

        if (isLoad) {
            specStore.load({
                params: { pileId: pileId, ProductId: PRODUCT_ID, OldProductId: window.parent.GetCopyProductId() },
                callback: function (records, operation, success) {
                    if (isEdit != "true") {
                        Ext.getCmp('N_' + index).setValue(records.length);
                        Ext.getCmp('M_' + index).setValue(records[0].data.G_Must_Buy);

                    }
                }
            });
        }

        for (var i = 0; i < value; i++) {
            specStore.add({
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


        var grid = Ext.create('Ext.grid.Panel', {
            id: 'G_' + index,
            gName: 'groupGrid',
            title: GROUP + index + PRODUCT,
            height: 200,
            width: 400,
            margin: '0 0 10 0',
            store: specStore,
            plugins: [
                Ext.create('Ext.grid.plugin.CellEditing', {
                    clicksToEdit: 1,
                    listeners: {
                        beforeedit: function (edit, e) {
                            if (isLoad && isEdit == "true") {
                                return false;
                            }
                            if (Ext.getCmp('M_' + index).getValue() == null) {
                                Ext.getCmp('M_' + index).markInvalid(NOT_NULL);
                                return false;
                            }
                        },
                        edit: function (edit, e) {
                            currentRow = e.rowIdx;
                            currentCol = e.colIdx;
                            if (e.colIdx == 2) {
                                if (Ext.getCmp('chkNum').getValue()) {

                                }
                            }
                        }
                    }
                })
            ],
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
                                        //檢查臨時表商品數據是否是狀態==12
                                        var productid = resText.data.Product_Id ;
                                        var reg = /^\d+$/;
                                        if (!reg.test(productid)) {
                                            if (resText.data.Temp_Status != 12) {
                                                combSpecStroe.getAt(currentRow).set('Product_Name', '');
                                                Ext.Msg.alert(INFORMATION, INPUT_TEMP_OVER);
                                            }
                                        }

                                        //檢查是否是單一商品
                                        if (resText.data.Combination != 1) {
                                            Ext.Msg.alert(INFORMATION, INPUT_MUSTBE_SINGLE);
                                            Ext.getCmp('G_' + index).getStore().getAt(currentRow).set('Product_Name', '');
                                            return;
                                        }
                                        //檢驗是否是該供應商下面的商品
                                        Ext.Ajax.request({
                                            url: '/VendorProductCombo/ISVendorProduct',
                                            method: 'POST',
                                            params: {
                                                ProductId: productid
                                            },
                                            success: function (response, opts) {
                                                var result = Ext.decode(response.responseText);
                                                if (!result.success) {
                                                    Ext.getCmp('G_' + index).getStore().getAt(currentRow).set('Product_Name', '');
                                                    Ext.Msg.alert(INFORMATION, SAME_VENDOR);
                                                    return;
                                                }
                                            }
                                        });

                                        //检查出貨方式
                                        if (productModeCheck(tempData, resText)) {
                                            Ext.getCmp('G_' + index).getStore().getAt(currentRow).set('Product_Name', resText.data.Product_Name);
                                        }
                                        else {
                                            Ext.getCmp('G_' + index).getStore().getAt(currentRow).set('Product_Name', '');
                                            return;
                                        }

                                        //检测运费模式
                                        if (freightCheck(tempData, resText)) {
                                            Ext.getCmp('G_' + index).getStore().getAt(currentRow).set('Product_Name', resText.data.Product_Name);
                                        }
                                        else {
                                            Ext.getCmp('G_' + index).getStore().getAt(currentRow).set('Product_Name', '');
                                        }
                                    }
                                    else {
                                        Ext.getCmp('G_' + index).getStore().getAt(currentRow).set('Product_Name', '');
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
             { header: UNIT_NUM, colName: 'unitNum', menuDisabled: true, sortable: false, width: 100, dataIndex: 'S_Must_Buy', editor: {
                 xtype: 'combobox',
                 queryMode: 'local',
                 store: mustBuyArray,
                 displayField: 'value',
                 valueField: 'value',
                 editable: false,
                 listeners: {
                     beforequery: function (qe) {
                         delete qe.combo.lastQuery;
                         //                         if (Ext.getCmp('chkNum').getValue()) {
                         //                             for (var i = 0, j = this.getStore().getCount() - 2; i < j; i++) {
                         //                                 this.getStore().removeAt(2);
                         //                             }
                         //                         }
                         //                         else {
                         //                             this.getStore().removeAll();
                         //                             this.getStore().add({ field1: '0' }, { field1: '1' }, { field1: '2' }, { field1: '3' }, { field1: '4' }, { field1: '5' }, { field1: '6' }, { field1: '7' }, { field1: '8' }, { field1: '9' }, { field1: '10' });
                         //                         }
                         this.getStore().removeAll();
                         var t = Ext.getCmp('chkNum').getValue() ? 1 : Ext.getCmp('M_' + index).getValue();
                         for (var i = 0; i <= t; i++) {
                             this.getStore().add({ field1: i });
                         }
                     }
                 }
             }
             },
             { header: 'rid', hidden: true, dataIndex: 'id'}],
            listeners: {
                viewready: function (view) {
                    window.parent.updateAuth(view, 'colName');
                }
            }
        });
        bottomPanel.add(grid);
    }


    function createNumPanel(num, data) {

        if (numPanel) {
            numPanel.destroy();
            isLoad = false;
        }
        if (bottomPanel.items.length > 0) {
            bottomPanel.removeAll(true);
        }
        numPanel = Ext.create('Ext.panel.Panel', {
            id: 'numPanel',
            border: false,
            listeners: {
                afterrender: function (panel) {
                    window.parent.updateAuth(panel, 'colName');
                }
            }
        });

        for (var i = 1; i <= num; i++) {
            var unitPnale = Ext.create('Ext.panel.Panel', {
                pName: 'numPanel',
                layout: 'hbox',
                border: false,
                padding: '5 0 5 0',
                items: [{
                    xtype: 'panel',
                    hidden: isLoad && isEdit == "true",
                    border: false,
                    defaultType: 'combobox',
                    items: [{
                        id: 'N_' + i,
                        index: i,
                        fieldLabel: GROUP + i + PRODUCT_NUMB + '<span style="color:red;">*</span>',
                        colName: 'productNumb',
                        //                      hidden: true,
                        allowBlank: false,
                        labelWidth: 100,
                        store: numArray,
                        queryMode: 'local',
                        margin: '0 20 0 0',
                        editable: false,
                        listeners: {
                            select: function (e) {
                                createGridPanel(e.index, e.value);
                                Ext.getCmp('M_' + e.index).setValue("0");
                                //Ext.getCmp('M_' + e.index).setMaxValue(e.value);
                            }
                        }
                    }]

                }, {
                    xtype: 'panel',
                    hidden: isLoad && isEdit == "true",
                    border: false,
                    defaultType: 'combobox',
                    items: [{
                        id: 'M_' + i,
                        xtype: 'numberfield',
                        minValue: 0,
                        mustName: 'comboMustBuy',
                        fieldLabel: GROUP + i + MUST_BUY_NUM + '<span style="color:red;">*</span>',
                        labelWidth: 130,
                        msgTarget: 'side',
                        decimalPrecision: 0,
                        margin: '0 20 0 0',
                        comboName: 'combMustBuy',
                        colName: 'mustBuyNum',
                        //                        hidden: true,
                        allowBlank: false
                    }]

                }, {
                    xtype: 'panel',
                    border: false,
                    hidden: !(isLoad && isEdit == "true"),
                    items: [{
                        xtype: 'displayfield',
                        id: 'disMustBuy_' + i,
                        colName: 'mustBuyNum',
                        //                        hidden: true,
                        labelWidth: 130,
                        margin: '0 20 0 0',
                        mustName: 'disMustBuy',
                        fieldLabel: GROUP + i + MUST_BUY_NUM + '<span style="color:red;">*</span>',
                        value: isLoad ? data[i - 1].G_Must_Buy : ''
                    }]

                }]
            });
            numPanel.add(unitPnale);
        }
        numPanel.add({
            xtype: 'checkbox',
            boxLabel: BUY_LIMIT,
            inputValue: '1',
            id: 'chkNum',
            colName: 'chkNum',
            //            hidden: true,
            checked: isLoad ? data[0].Buy_Limit : false,
            disabled: isEdit == "true",
            margin: '10 0 0 0',
            handler: function (e) {
                buyLimit = e.value ? 1 : 0;
                if (e.value) {
                    if (CheckAble()) {
                        for (var i = 1; i <= groupNum; i++) {
                            if (Ext.getCmp('G_' + i) != null) {
                                for (var m = 0, n = Ext.getCmp('G_' + i).getStore().getCount(); m < n; m++) {
                                    if (Ext.getCmp('G_' + i).getStore().getAt(m).data.S_Must_Buy != 0) {
                                        Ext.getCmp('G_' + i).getStore().getAt(m).set('S_Must_Buy', '1');
                                    }
                                }
                            }
                        }
                        SetMax(1);
                    }
                    else {
                        this.setValue(false);
                    }

                }
                else {
                    SetMax(2);
                }


            }
        });
        topPanel.add(numPanel);
    }

    topPanel = Ext.create('Ext.form.Panel', {
        id: 'topPanel',
        fieldDefaults: {
            msgTarget: 'side'
        },
        margin: '0 0 20 0',
        border: false,
        items: [{
            xtype: 'displayfield',
            fieldLabel: COMBO_TYPE,
            value: GROUP_COMBO,
            disName: 'comboType',
            labelWidth: 65,
            colName: 'comboType',
            id: 'comboType',
            colName: 'comboType',
            //            hidden: true,
            margin: '0 0 10 0'
        },
        {
            xtype: 'panel',
            id: 'groupNumPanel',
            layout: 'hbox',
            border: false,
            //hidden: true,
            items: [{
                xtype: 'combobox',
                store: numArray,
                editable: false,
                fieldLabel: GROUP_NUM + '<span style="color:red;">*</span>',
                id: 'comboGroupNum',
                allowBlank: false,
                colName: 'comboGroupNum',
                //hidden: true,
                queryMode: 'local',
                margin: '0 0 10 0',
                listeners: {
                    select: function (e) {
                        groupNum = e.value;
                        createNumPanel(e.value);
                    }
                }
            }, {
                xtype: 'panel',
                padding: '5 0 0 10',
                border: false,
                html: '<span style="color:red;font-weight:bold;font-size:11px;font-family:雅黑;float:left">' + GROUP_NUM_NOTICE + '</span>'
            }]
        }],
        listeners: {
            afterrender: function (panel) {
                window.parent.updateAuth(panel, 'colName');
            }
        }
    });

    bottomPanel = Ext.create('Ext.panel.Panel', {
        id: 'bottomPanel',
        border: false
    });


    viewPort = Ext.create('Ext.container.Viewport', {
        layout: 'anchor',
        padding: 20,
        items: [topPanel, bottomPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //orderGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    //window.parent.setShow(topPanel, 'id');

    Ext.Ajax.request({
        url: '/VendorProductCombo/groupNameQuery',
        method: 'POST',
        async: false,
        params: {
            ProductId: PRODUCT_ID,
            OldProductId: window.parent.GetCopyProductId()
        },
        success: function (response, opts) {
            var data = eval("(" + response.responseText + ")");
            var len = data.length;
            if (len > 0) {
                isLoad = true;
                groupNum = len;
                createNumPanel(len, data);
                for (var i = 1; i <= len; i++) {
                    createGridPanel(i, 1, data[i - 1].Pile_Id);
                }
                if (isEdit != "true") {
                    Ext.getCmp('comboGroupNum').setValue(len);
                    Ext.getCmp('groupNumPanel').show();
                } else {
                    Ext.getCmp('groupNumPanel').hide();
                }

            }
            else {
                Ext.getCmp('groupNumPanel').show();
            }
        },
        failure: function (response, opts) {

        }
    });

}