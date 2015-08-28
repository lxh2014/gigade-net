
var conditionPanel;
var contentPanel
var spec1Rows = 1;
var spec2Rows = 1;
var spec1Store;
var spec2Store;
var spec1Grid;
var spec2Grid;
var specnamePanel = null;
var gridPanel = null;
var isBlur = false;
var typeVal = null;
var conditionViewport;
var isLoad = false;
var isModify = false;
var isChange = false;
var spec_title_1 = '';
var spec_title_2 = '';
var myMask = null;
var spec1Init = '';
var spec2Init = '';
var spectypeStore;
var detail_product_id;
var specPanel;

spectypeStore = Ext.create('Ext.data.Store', {
    fields: ['parameterCode', 'parameterName'],
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/getCateType',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});



/************** 規格 Model Store **************/

Ext.define('GIGADE.SPEC', {
    extend: 'Ext.data.Model',
    fields: [
            { name: 'spec_id', type: 'int' },
            { name: 'product_id', type: 'int' },
            { name: 'spec_type', type: 'int' },
            { name: 'spec_name', type: 'string' },
            { name: 'spec_sort', type: 'int' },
            { name: 'spec_status', type: 'int' }
        ]
});

spec1Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SPEC',
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/spec1TempQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});


/******************** Spec2 ***********************/

spec2Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SPEC',
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/spec2TempQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

function initSpecPanel(product_id) {
    detail_product_id = product_id;
    specPanel = Ext.create('Ext.panel.Panel', {
        border: false
    });

    Ext.Ajax.request({
        url: '/VendorProduct/QueryProduct',
        async: false,
        method: 'post',
        params: {
            ProductId: product_id
        },

        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText != null && resText.data != null) {
                typeVal = resText.data.Product_Spec;
                if (typeVal == 1) {
                    typeVal = 1;
                    spec_title_1 = resText.data.Spec_Title_1;
                    createContentPanel(1);
                    specPanel.add(specnamePanel);
                    specPanel.add(gridPanel);
                }
                else if (typeVal == 2) {
                    isLoad = true;
                    isChange = true;
                    typeVal = 2;
                    spec_title_1 = resText.data.Spec_Title_1;
                    spec_title_2 = resText.data.Spec_Title_2;
                    createContentPanel(2);
                    specPanel.add(specnamePanel);
                    specPanel.add(gridPanel);
                }
                else {
                    specnamePanel = Ext.create('Ext.panel.Panel', {
                        padding: '20 0 0 20',
                        border: false,
                        items: [{
                            xtype: 'displayfield',
                            labelWidth: 65,
                            fieldLabel: SPEC_TYPE,
                            value: SPEC_NULL
                        }]

                    });

                    specPanel.add(specnamePanel);
                }
            }
            else {

                specnamePanel = Ext.create('Ext.panel.Panel', {
                    padding: '20 0 0 20',
                    border: false,
                    items: [{
                        xtype: 'displayfield',
                        labelWidth: 65,
                        fieldLabel: SPEC_TYPE,
                        value: SPEC_NULL
                    }]

                });

                specPanel.add(specnamePanel);

            }
        },
        failure: function (response, opts) {
            alert("failure");
            Ext.create('Ext.panel.Panel', {
                padding: '20 0 0 20',
                border: false,
                renderTo: Ext.getBody(),
                items: [{
                    xtype: 'displayfield',
                    labelWidth: 65,
                    fieldLabel: SPEC_TYPE,
                    value: SPEC_NULL
                }]

            });
            return false;
        }
    });

}



function destroyPanel() {
    if (specnamePanel != null) {
        specnamePanel.destroy();
        specnamePanel = null;
    }
    if (gridPanel != null) {
        gridPanel.destroy();
        gridPanel = null;
    }
    if (spec1Grid != null) {
        spec1Grid.destroy();
        spec1Grid = null;
    }
    if (spec2Grid != null) {
        spec2Grid.destroy();
        spec2Grid = null;
    }
    spec1Store.removeAll();
    spec2Store.removeAll();
}

function createContentPanel(typeVal) {

    destroyPanel();

    /**************** 規格名稱 Panel ****************/

    specnamePanel = Ext.create('Ext.form.Panel', {
        id: 'specnamePanel',
        title: '',
        fieldDefaults: {
            msgTarget: 'side'
        },
        border: false,
        items: [{
            xtype: 'displayfield',
            id: 'txtSpec1Name',
            colName: 'txtSpec1Name',
            hidden: false,
            allowBlank: false,
            submitValue: false,
            fieldLabel: SPEC_1_NAME,
            labelWidth: 100,
            margin: '0 0 10 0'
        }],
        listeners: {
            afterrender: function (panel) {
                //   window.parent.updateAuth(panel, 'colName');
            }
        }
    });


    if (typeVal == 2) {
        specnamePanel.add({
            xtype: 'displayfield',
            id: 'txtSpec2Name',
            colName: 'txtSpec2Name',
            hidden: false,
            allowBlank: false,
            submitValue: false,
            fieldLabel: SPEC_2_NAME,
            labelWidth: 100
        });
    }


    /**************** 規格內容 Grid ****************/

    gridPanel = Ext.create('Ext.panel.Panel', {
        id: 'gridPanel',
        title: '',
        padding: '10 0 0 0',
        border: false,
        layout: 'hbox',
        items: []
    });


    /**************** 規格一 Grid ****************/
    var statusStore1 = Ext.create('Ext.data.Store', {
        id: 'statusStore1',
        autoDestroy: true,
        fields: ['parameterCode', 'parameterName'],
        autoLoad: false,
        data: [
         { parameterCode: '1', parameterName: SPEC_SHOW },
         { parameterCode: '0', parameterName: SPEC_HIDE }
     ]
    })

    function statusRenderer1(value, m, r, row, column) {
        var index = statusStore1.find("parameterCode", value);
        var recode = statusStore1.getAt(index);
        if (recode) {
            return recode.get("parameterName");
        } else {
            return value;
        }
    }

    var statusStore2 = Ext.create('Ext.data.Store', {
        id: 'statusStore2',
        autoDestroy: true,
        fields: ['parameterCode', 'parameterName'],
        autoLoad: false,
        data: [
         { parameterCode: '1', parameterName: SPEC_SHOW },
         { parameterCode: '0', parameterName: SPEC_HIDE }
     ]
    })

    function statusRenderer2(value, m, r, row, column) {
        var index = statusStore2.find("parameterCode", value);
        var recode = statusStore2.getAt(index);
        if (recode) {
            return recode.get("parameterName");
        } else {
            return value;
        }
    }

    spec1Grid = Ext.create('Ext.grid.Panel', {
        id: 'spec1Grid',
        title: SPEC_1,
        store: spec1Store,
        margin: '0 20 0 0',
        width: 355,
        height: 300,
        columns: [
        { id: 'spec1_name', colName: 'spec_name', hidden: false, menuDisabled: true, width: 150, header: SPEC_CONTENT, dataIndex: 'spec_name' },
        { id: 'spec1_sort', colName: 'spec_sort', hidden: false, menuDisabled: true, width: 100, header: SPEC_SORT, dataIndex: 'spec_sort' },
        { id: 'spec1_status', colName: 'spec_status', hidden: false, menuDisabled: true, width: 100, header: SPEC_SHOW, dataIndex: 'spec_status', renderer: statusRenderer1 },
        { id: 'spec1_id', hidden: true, dataIndex: 'spec_id' }
        ]
    });

    /**************** 規格二 Grid ****************/

    spec2Grid = Ext.create('Ext.grid.Panel', {
        id: 'spec2Grid',
        title: SPEC_2,
        store: spec2Store,
        width: 355,
        height: 300,
        columns: [
        { id: 'spec2_name', colName: 'spec_name', hidden: false, menuDisabled: true, width: 150, header: SPEC_CONTENT, dataIndex: 'spec_name' },
        { id: 'spec2_sort', colName: 'spec_sort', hidden: false, menuDisabled: true, width: 100, header: SPEC_SORT, dataIndex: 'spec_sort' },
         { id: 'spec2_status', colName: 'spec_status', hidden: false, menuDisabled: true, width: 100, header: SPEC_SHOW, dataIndex: 'spec_status', renderer: statusRenderer2 },
        { id: 'spec2_id', hidden: true, dataIndex: 'spec_id'}]

    });

    /*********** 添加規格Grid至規格Panel ***********/


    gridPanel.add(spec1Grid);

    Ext.getCmp('txtSpec1Name').setValue(spec_title_1);
    spec1Store.load({ params: { 'ProductId': detail_product_id} });



    if (typeVal == 2) {
        gridPanel.add(spec2Grid);
        Ext.getCmp('txtSpec2Name').setValue(spec_title_2);
        spec2Store.load({ params: { 'ProductId': detail_product_id} });


    }

}


