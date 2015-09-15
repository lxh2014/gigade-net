
var newstageCategoryPanel;
var cateStr = '';
var newtreeStoreToo

function newinitCatePanel(product_id) {
    //樹
    newtreeStoreToo = Ext.create('Ext.data.TreeStore', {
        id: 'newtreeStoreToo',
        proxy: {
            type: 'ajax',
            url: '/Product/GetCatagory/false?categoryType=2',
            actionMethods: 'post'
        },
        root: {
            expanded: true,
            children: [

            ]
        }
    });


    categoryNews = Ext.create('Ext.data.Store', {

        fields: ['ParameterCode', 'parameterName', 'ParameterProperty'],
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/Parameter/QueryParaByXml?paraType=productCategory',
            noCache: false,
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });


    newstageCategoryPanel = Ext.create('Ext.tree.Panel', {
        id: 'newstageCategoryPanel',
        frame: false,
        animate: true,
        border: true,
        rootVisible: false,
        //colName: 'frontCate',
        //hidden: true,
        lines: true,
        minWidth: 200,
        width: 500,
        height: 420,
        enableDrag: true,
        store: newtreeStoreToo
    });



    newcategoryPanel = Ext.create('Ext.form.Panel', {
        id: 'reportNewProduct',
        layout: 'hbox',
        width: 500,
        height: 100,
        padding: '5 0 5 0',
        border: false,
        items: [{
            xtype: 'panel',
            width: 200,
            height: 90,
            border: true,
            bodyPadding: '35 0 0 55',
            items: [{
                xtype: 'displayfield',
                value: SHOPCLASS
            }]
        }, {
            xtype: 'panel',
            width: 300,
            height: 90,
            bodyPadding: '15 0 0 10',
            border: true,
            items: [{
                xtype: 'combobox',
                id: 'reportNew',
                store: categoryNews,
                queryMode: 'local',
                editable: false,
                style: { marginBottom: '15px' },
                valueField: 'ParameterCode',
                displayField: 'parameterName'
            }]
        }],
        listeners: {
            beforerender: function () {
                Ext.Ajax.request({
                    url: '/Product/GetProdClassify',
                    method: 'POST',
                    params: {
                        ProductId: product_id
                    },
                    success: function (response) {
                        var data = Ext.decode(response.responseText);
                        Ext.getCmp('reportNew').setValue(data.Prod_Classify);
                    }
                });
            }
        }
    });



}

