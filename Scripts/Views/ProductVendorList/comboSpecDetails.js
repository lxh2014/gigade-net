var re = /}{/g;
var product_id;
var isLoad = false;
var comboType;
var combSpecStroe;
var numMustBuy;
var groupNum;
var buyLimit = 0;
var currentRow;
var currentCol;
var numArray = new Array(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
var mustBuyArray = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
var comboSpecPanel;
var comboSpecGrid;
var defaultPanel;
var specPanel; //要顯示的Panel

function initComboSpecPanel(product_id) {

    specPanel = Ext.create('Ext.panel.Panel', {
        border: false
    });

    Ext.Ajax.request({
        url: '/ProductVendorList/ProcomQueryProduct',
        method: 'POST',
        async: false,
        params: {
            ProductId: product_id
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText != null && resText.data != null) {
                comboType = resText.data.Combination;
                switch (comboType) {
                    case 2: createComboFixed(product_id); specPanel.add(comboSpecPanel); specPanel.add(comboSpecGrid); break;
                    case 3: createComboOptional(product_id); specPanel.add(comboSpecPanel); specPanel.add(comboSpecGrid); break;
                    case 4: createComboGroup(product_id); specPanel.add(comboSpecPanel); specPanel.add(comboSpecGrid); break;
                    default: specPanel.add(createDefault()); break;
                }
            }
        },
        failure: function (response, opts) {
            return false;
        }
    });
}

function createDefault() {
    return Ext.create('Ext.panel.Panel', { border: false, html: 'null' });
}


//創建固定組合規格Panel
function createComboFixed(product_id) {
    var currentRow;
    var currentCol;

    combSpecStroe = Ext.create('Ext.data.Store', {
        id: 'combSpecStroe',
        fields: ['id', 'Parent_Id', 'Child_Id', 'Product_Name', 'S_Must_Buy', 'G_Must_Buy', 'Pile_Id', 'Buy_Limit'],
        proxy: {
            type: 'ajax',
            url: '/ProductVendorList/combSpecQuery',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
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
            colName: 'combination',
            hidden: false,
            value: FIXED_COMBO
        }]

    });

    var fixedGrid = Ext.create('Ext.grid.Panel', {
        title: '',
        store: combSpecStroe,
        height: 200,
        width: 400,
        layout: 'hbox',
        margin: '0 10 0 0',
        columns: [{ header: PRODUCT_NUM, colName: 'productNum', hidden: false, menuDisabled: true, sortable: false, dataIndex: 'Child_Id' },
             { header: PRODUCT_NAME, colName: 'product_name', hidden: false, menuDisabled: true, sortable: false, width: 200, dataIndex: 'Product_Name', flex: 1 },
             { header: UNIT_NUM, colName: 'unitNum', hidden: false, menuDisabled: true, sortable: false, dataIndex: 'S_Must_Buy' },
             { header: 'rid', hidden: true, dataIndex: 'id'}],
        listeners: {
            beforerender: function () {
                combSpecStroe.load({
                    params: { ProductId: product_id }
                });
            }
        }
    });

    comboSpecPanel = fixedPanel;
    comboSpecGrid = fixedGrid;

}




//創建任意組合規格Panel
function createComboOptional(product_id) {

    combSpecStroe = Ext.create('Ext.data.Store', {
        id: 'combSpecStroe',
        fields: ['Id', 'Parent_Id', 'Child_Id', 'Product_Name', 'S_Must_Buy', 'G_Must_Buy', 'Pile_Id', 'Buy_Limit'],
        proxy: {
            type: 'ajax',
            url: '/ProductVendorList/combSpecQuery',
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
            colName: 'combination',
            hidden: false,
            value: OPTIONAL_COMBO,
            margin: '0 0 20 0'
        }, {

            xtype: 'displayfield',
            id: 'disMustBuy',
            hidden: false,
            margin: '0 0 15 0',
            fieldLabel: MUST_BUY_NUM,
            colName: 'mustBuyNum'


        }, {
            xtype: 'checkbox',
            boxLabel: BUY_LIMIT,
            inputValue: '1',
            id: 'chkNum',
            colName: 'chkNum',
            hidden: false,
            margin: '0 0 15 0',
            disabled: product_id
        }],
        listeners: {
            beforerender: function () {
                combSpecStroe.load({
                    params: { ProductId: product_id },
                    callback: function (records, operation, success) {
                        if (records.length > 0) {
                            isLoad = true;
                            Ext.getCmp('optionalGrid').show();
                            Ext.getCmp('disMustBuy').setValue(records[0].data.G_Must_Buy);
                            Ext.getCmp('chkNum').setValue(records[0].data.Buy_Limit);
                            numMustBuy = records[0].data.G_Must_Buy;

                            if (!product_id) {
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
        hidden: false,
        store: combSpecStroe,
        height: 200,
        width: 400,
        columns: [{ header: PRODUCT_NUM, colName: 'productNum', menuDisabled: true, sortable: false, dataIndex: 'Child_Id' },
             { header: PRODUCT_NAME, colName: 'product_name', menuDisabled: true, sortable: false, width: 200, dataIndex: 'Product_Name', flex: 1 },
             { header: UNIT_NUM, menuDisabled: true, colName: 'unitNum', sortable: false, width: 100, dataIndex: 'S_Must_Buy' },
             { header: 'rid', hidden: true, dataIndex: 'Id'}]
    });

    comboSpecPanel = optionalPanel;
    comboSpecGrid = optionalGrid;

}


//創建群組搭配規格Panel

function createComboGroup(product_id) {

    var topPanel;
    var bottomPanel;
    var numPanel;
    var gridPanel;



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
            id: 'S_' + index,
            fields: ['Id', 'Parent_Id', 'Child_Id', 'Product_Name', 'S_Must_Buy', 'G_Must_Buy', 'Pile_Id', 'Buy_Limit'],
            proxy: {
                type: 'ajax',
                url: '/ProductVendorList/combSpecQuery',
                actionMethods: 'post',
                reader: {
                    type: 'json',
                    root: 'data'
                }
            }
        });
        specStore.load({
            params: { pileId: pileId, ProductId: product_id },
            callback: function (records, operation, success) {
                if (!product_id) {
                    Ext.getCmp('N_' + index).setValue(records.length);
                    for (var i = 1, j = records.length; i <= j; i++) {
                        Ext.getCmp('M_' + index).getStore().add({ value: i });
                    }
                    Ext.getCmp('M_' + index).setValue(records[0].data.G_Must_Buy);
                }
            }
        });
        var grid = Ext.create('Ext.grid.Panel', {
            id: 'G_' + index,
            gName: 'groupGrid',
            title: GROUP + index + PRODUCT,
            height: 200,
            width: 400,
            margin: '0 0 10 0',
            store: specStore,
            columns: [{ header: PRODUCT_NUM, colName: 'productNum', menuDisabled: true, sortable: false, dataIndex: 'Child_Id' },
             { header: PRODUCT_NAME, colName: 'product_name', menuDisabled: true, sortable: false, width: 200, dataIndex: 'Product_Name', flex: 1 },
             { header: UNIT_NUM, colName: 'unitNum', menuDisabled: true, sortable: false, width: 100, dataIndex: 'S_Must_Buy' },
             { header: 'rid', hidden: true, dataIndex: 'id'}]
        });
        bottomPanel.add(grid);
    }
    function createNumPanel(num, data) {

        if (numPanel) {
            numPanel.destroy();
        }
        if (bottomPanel.items.length > 0) {
            bottomPanel.removeAll(true);
        }
        numPanel = Ext.create('Ext.panel.Panel', {
            id: 'numPanel',
            border: false
        });

        for (var i = 1; i <= num; i++) {
            var numSelectStore = Ext.create('Ext.data.Store', {
                id: 'numSelectStore_' + i,
                fields: ['value']
            });

            var unitPnale = Ext.create('Ext.panel.Panel', {
                pName: 'numPanel',
                layout: 'hbox',
                border: false,
                padding: '5 0 5 0',
                items: [{
                    xtype: 'displayfield',
                    id: 'disMustBuy_' + i,
                    colName: 'mustBuyNum',
                    hidden: true,
                    labelWidth: 130,
                    margin: '0 20 0 0',
                    mustName: 'disMustBuy',
                    fieldLabel: GROUP + i + MUST_BUY_NUM,
                    value: isLoad ? data[i - 1].G_Must_Buy : ''

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
            hidden: false,
            checked: data[0].Buy_Limit,
            disabled: true,
            margin: '10 0 0 0',
            handler: function (e) {
                buyLimit = e.value ? 1 : 0;
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
            colName: 'combination',
            id: 'comboType',
            hidden: true,
            margin: '0 0 10 0'
        }]
    });

    bottomPanel = Ext.create('Ext.panel.Panel', {
        id: 'bottomPanel',
        border: false
    });

    comboSpecPanel = topPanel;
    comboSpecGrid = bottomPanel;

    Ext.Ajax.request({
        url: '/ProductVendorList/groupNameQuery',
        method: 'POST',
        async: false,
        params: {
            'ProductId': product_id
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
                if (!product_id) {
                    Ext.getCmp('comboGroupNum').setValue(len);
                    Ext.getCmp('groupNumPanel').show();
                }
            }
        }
    });
}

