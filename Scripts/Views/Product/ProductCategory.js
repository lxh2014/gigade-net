
var stageCategoryPanel;
var PRODUCT_ID = '', OLD_PRODUCT_ID = '';
var cateId = '';
var topValue = '';
var myMask = null;

var oldresultStr = '';
var re = /}{/g;
Ext.onReady(function () {

    createPanel();

})




function save(functionid) {
    var obj = Ext.getCmp('combCate2');

    if (obj != null && obj.isHidden()) {
        return true;
    }

    if (obj.getValue() == null || obj.getValue() == '') {
        obj.markInvalid(INPUT_PLEASE);
        window.parent.setMoveEnable(true);
        return false;
    }

    var asyncResult = true;

    if (!functionid) {
        functionid = '';
    }

    var resultStr = getdata();

    Ext.Ajax.request({
        url: '/Product/tempCategoryAdd?categoryType=1',
        method: 'POST',
        async: false,
        params: {
            coboType: 1,
            'ProductId': PRODUCT_ID,
            OldProductId: OLD_PRODUCT_ID,
            'cate_id': obj.getValue(),
            'oldresult': oldresultStr,
            'result': resultStr,
            "function": functionid,
            "batch": window.parent.GetBatchNo()
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (PRODUCT_ID != '') {
                if (resText.success) {
                    Ext.Msg.alert(NOTICE, SAVE_SUCCESS);
                }
                else {
                    Ext.Msg.alert(NOTICE, SAVE_FAIL);
                }
            } else {
                if (!resText.success) {
                    Ext.Msg.alert(NOTICE, SAVE_FAIL);
                    window.parent.setMoveEnable(true);
                    asyncResult = false;
                }
            }
            if(resText.success)
                stageCategoryPanel.doLayout();
        },
        failure: function (response, opts) {
            Ext.Msg.alert(NOTICE, SAVE_FAIL);
            window.parent.setMoveEnable(true);
            asyncResult = false;
        }

    });
    return asyncResult;

}


function createPanel() {

    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
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
        },
        listeners: {
            datachanged: function (store) {
                // var cate1Record = categoryStore1.findRecord('Rowid',topValue);
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
            url: '/Product/GetCatagory?categoryType=1',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post'
        },
        root: {
            text: FRONT_MANAGEMENT,
            expanded: true,
            children: [

            ]
        }
    });

    var categoryPanel = Ext.create('Ext.form.Panel', {
        id: 'catePanel',
        layout: 'hbox',
        width: 500,
        height: 100,
        padding: '5 0 5 0',
        hidden: true,
        colName: 'combCate2',
        border: false,
        listeners: {
            beforerender: function () {
                Ext.Ajax.request({
                    url: '/Product/GetSelectedCage',
                    method: 'POST',
                    params: {
                        ProductId: PRODUCT_ID,
                        OldProductId: OLD_PRODUCT_ID
                    },
                    success: function (response) {
                        var data = Ext.decode(response.responseText);
                        if (data != null && data.success) {
                            var da = data.data;
                            Ext.getCmp('combCate1').getStore().add({ parameterType: 'product_cate', ParameterCode: da.cate1Value, parameterName: da.cate1Name, Rowid: da.cate1Rowid, TopValue: da.cate1TopValue });
                            Ext.getCmp('combCate1').setValue(da.cate1Value);
                            Ext.getCmp('combCate2').getStore().add({ parameterType: 'product_cate', ParameterCode: da.cate2Value, parameterName: da.cate2Name, Rowid: da.cate2Rowid, TopValue: da.cate2TopValue });
                            Ext.getCmp('combCate2').setValue(da.cate2Value);

                            categoryStore1.load();
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
                value: CATEGORY_MANAGEMENT + '<span style="color:red;">*</span>'
            }]
        }, {
            xtype: 'panel',
            width: 300,
            height: 90,
            bodyPadding: '15 0 0 10',
            border: true,
            items: [{
                xtype: 'combobox',
                id: 'combCate1',
                store: categoryStore1,
                queryMode: 'local',
                emptyText: SELECT_PLEASE,
                editable: false,
                style: { marginBottom: '15px' },
                valueField: 'ParameterCode',
                displayField: 'parameterName',
                listeners: {
                    select: function () {
                        var z = Ext.getCmp("combCate2");
                        z.clearValue();
                        categoryStore2.removeAll();
                    },
                    beforequery: function (qe) {
                        if (categoryStore1.getCount() <= 1) {
                            categoryStore1.load();
                        }
                    }
                }

            }, {
                xtype: 'combobox',
                id: 'combCate2',
                emptyText: SELECT_PLEASE,
                editable: false,
                store: categoryStore2,
                queryMode: 'local',
                valueField: 'ParameterCode',
                displayField: 'parameterName',
                msgTarget: 'side',
                listeners: {
                    beforequery: function (qe) {
                        if (Ext.getCmp('combCate1').getValue() == null) {
                            return;
                        }
                        delete qe.combo.lastQuery;
                        categoryStore2.removeAll();
                        categoryStore2.load({
                            params: {
                                topValue: categoryStore1.findRecord('ParameterCode', Ext.getCmp("combCate1").getValue()).data.ParameterCode
                            }
                        });

                    }
                }
            }]
        }

        ]

    });

    stageCategoryPanel = Ext.create('Ext.tree.Panel', {
        id: 'stageCategoryPanel',
        colName: 'stageCategoryPanel',
        frame: false,
        animate: true,
        hidden: true,
        lines: true,
        rootVisible: false,
        onlyLeafCheckable: true,//add by xiaohuihui 2014/08/12
        minWidth: 200,
        width: 500,
        height: PRODUCT_ID != '' ? 320 : document.documentElement.clientHeight - 105,
        lines: true,
        enableDrag: true,
        store: treeStore,
        listeners: {
            afterlayout: function () {
                oldresultStr = getdata();
            }
        }
    });

    function setChecked(node, checked) {
        node.expand();
        node.checked = checked;
        node.eachChild(function (child) {
            child.set('checked', checked);
            setChecked(child, checked);
        });
    }


    stageCategoryPanel.on('checkchange', function (node, checked) {
        setChecked(node, checked);
    });


    cateViewport = Ext.create('Ext.container.Viewport', {
        id: 'cateViewport',
        layout: 'vbox',
        items: [categoryPanel, stageCategoryPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                Ext.getCmp('stageCategoryPanel').setHeight(document.documentElement.clientHeight - 105);
                this.doLayout();
            },
            beforerender: function (view) {
                window.parent.updateAuth(view, 'colName');
            }
        }
    });

    treeStore.load({ params: { ProductId: PRODUCT_ID, OldProductId: OLD_PRODUCT_ID } });

}

function getdata() {
    var resultStr = "";
    var data = stageCategoryPanel.getChecked();

    if (data.length == 0) {
        return "[]";
    }

    if (data.length > 0) {
        resultStr += "[";
        for (var i = 0, j = data.length; i < j; i++) {
            resultStr += "{Category_Id:" + data[i].data.id + ",Category_Name:'" + data[i].data.text + "'}";
        }
        resultStr = resultStr.replace(re, "},{");
        resultStr += "]";
    }

    return resultStr;

}