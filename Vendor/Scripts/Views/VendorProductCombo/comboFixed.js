

function createComboFixed() {
    combSpecStroe = Ext.create('Ext.data.Store', {
        id: 'combSpecStroe',
        fields: ['id', 'Parent_Id', 'Child_Id', 'Product_Name', 'S_Must_Buy', 'G_Must_Buy', 'Pile_Id', 'Buy_Limit'],
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

    function addTr() {
        combSpecStroe.add({
            id: '',
            Parent_Id: '',
            Child_Id: '',
            Product_Name: '',
            S_Must_Buy: '1',
            G_Must_Buy: '1',
            Pile_Id: '',
            Buy_Limit: ''
        });
        if (combSpecStroe.getCount() > 1) {
            cellEditing.startEditByPosition({ row: combSpecStroe.getCount() - 1, column: 1 });
        }

    }

    function removeTr(rowIdx) {
        var data = combSpecStroe.getAt(rowIdx);
        data.set('id', '');
        data.set('Parent_Id', '');
        data.set('Child_Id', '');
        data.set('Product_Name', '');
        data.set('S_Must_Buy', '0');
        data.set('G_Must_Buy', '1');
        data.set('Pile_Id', '');
        data.set('Buy_Limit', '');
    }

    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1,
        listeners: {
            edit: function (edit, e) {
                currentRow = e.rowIdx;
                currentCol = e.colIdx;
            },
            beforeedit: function () {
                if (isLoad && isEdit == "true") {
                    return false;
                }
            }
        }
    });


    var fixedPanel = Ext.create('Ext.form.Panel', {
        id: 'fixedPanel',
        title: '',
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
            value: FIXED_COMBO
        }
        , {
            xtype: 'combobox',
            store: numArray,
            hidden: true,
            editable: false,
            fieldLabel: PRODUCT_NUM,
            queryMode: 'local',
            displayField: 'value',
            valueField: 'value',
            value: '1'
        }
        ]

    });

    var fixedGrid = Ext.create('Ext.grid.Panel', {
        title: '',
        store: combSpecStroe,
        height: 200,
        width: 400,
        plugins: [cellEditing],
        tbar: [{
            xtype: 'button',
            id: 'btnAdd',
            colName: 'addCombo',
            //  hidden: true,
            text: BTN_ADD,
            iconCls: 'icon-add',
            handler: function () {
                addTr();
            }
        }, '->', {}],
        columns: [{ header: '', id: 'delCombo', colName: 'delCombo', menuDisabled: true, width: 60, align: 'center', xtype: 'actioncolumn', items: [{
            icon: '../../../Content/img/icons/cross.gif',
            handler: function (grid, rowIndex, colIndex) {
                Ext.Msg.confirm(NOTICE, DELETE_CONFIRM, function (btn) {
                    if (btn == 'yes') {
                        if (combSpecStroe.getCount() > 1) {
                            combSpecStroe.removeAt(rowIndex);
                        }
                        else {
                            removeTr(0);
                        }
                    }
                });
            }
        }]
        },
             { header: PRODUCT_NUM, colName: 'productNum', menuDisabled: true, sortable: false,
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
             { header: UNIT_NUM, colName: 'unitNum', menuDisabled: true, sortable: false, dataIndex: 'S_Must_Buy', editor: {
                 //Edit By Castle 2014/07/02
                 xtype: 'numberfield',
                 minValue: 0
             }
             },
             { header: 'rid', hidden: true, dataIndex: 'id'}],
        listeners: {
            beforerender: function () {
                combSpecStroe.load({
                    params: {
                        ProductId: PRODUCT_ID,
                        OldProductId: window.parent.GetCopyProductId()
                    },
                    callback: function (records, operation, success) {
                        if (records.length <= 0) {
                            addTr();
                        }
                        else {
                            isLoad = true;
                            if (isEdit == "true") {
                                Ext.getCmp('btnAdd').setDisabled(true);
                                Ext.getCmp('delCombo').hide();
                            }
                        }
                    }
                });
            }
        }
    });


    var viewPort = Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [fixedPanel, fixedGrid],
        padding: 20,
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            },
            afterrender: function (view) {
                window.parent.updateAuth(view, 'colName');
            }
        }
    });

}