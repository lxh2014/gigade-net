var categoryPanel;
var stageCategoryPanel;
var cateStr = '';
function initCatePanel(product_id) {
    /**************** 品類管理 Store *****************/
    var categoryStore1 = Ext.create('Ext.data.Store', {
        fields: ['ParameterType', 'ParameterCode', 'parameterName', 'Rowid', 'TopValue'],
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/Parameter/GetProCage1',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    var categoryStore2 = Ext.create('Ext.data.Store', {
        fields: ['parameterType', 'ParameterCode', 'parameterName', 'Rowid', 'TopValue'],
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/Parameter/GetProCage2',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    /**************** 前台分類 Store *****************/
    var treeStore = Ext.create('Ext.data.TreeStore', {
        proxy: {
            type: 'ajax',
            url: '/Product/GetCatagory/false?categoryType=1',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post'
        },
        root: {
            expanded: true,
            text: FRONT_MANAGEMENT,
            children: [

                ]
        }
    });
    categoryPanel = Ext.create('Ext.form.Panel', {
        id: 'catePanel',
        layout: 'hbox',
        width: 500,
        height: 100,
        padding: '5 0 5 0',
        hidden: true,
        colName: 'cate',
        border: false,
        listeners: {
            beforerender: function () {
                Ext.Ajax.request({
                    url: '/Product/GetSelectedCage',
                    method: 'POST',
                    params: {
                        ProductId: product_id
                    },
                    success: function (response) {
                        var data = Ext.decode(response.responseText);
                        if (data != null && data.success) {
                            var da = data.data;
                            Ext.getCmp('comboCate1').setRawValue(da.cate1Name);
                            Ext.getCmp('comboCate2').setRawValue(da.cate2Name);
                        }
                        else {
                            Ext.getCmp('comboCate1').hide();
                            Ext.getCmp('comboCate2').setRawValue(CATEGORY_NULL);
                        }
                    }
                });
            }
        },
        items: [{
            xtype: 'panel',
            width: 200,
            height: 90,
            border: true,
            bodyPadding: '35 0 0 55',
            items: [{
                xtype: 'displayfield',
                value: CATEGORY_MANAGEMENT
            }]
        }, {
            xtype: 'panel',
            width: 300,
            height: 90,
            bodyPadding: '15 0 0 10',
            border: true,
            items: [{
                xtype: 'combobox',
                id: 'comboCate1',
                readOnly: true
            }, {
                xtype: 'combobox',
                id: 'comboCate2',
                readOnly: true
            }]
        }]
    });
    stageCategoryPanel = Ext.create('Ext.tree.Panel', {
        id: 'stageCategoryPanel',
        frame: false,
        animate: true,
        border: true,
        colName: 'frontCate',
        hidden: true,
        lines: true,
        minWidth: 200,
        width: 500,
        height: 420,
        enableDrag: true,
        store: treeStore
    });
}
