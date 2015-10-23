/*  
 * 
 * 文件名称：NewProductCategory.js 
 * 摘    要：單一商品與組合商品修改和新增 新類別頁面
 * 
 */
var PRODUCT_ID = '', OLD_PRODUCT_ID = '';
var oldresultStr = '';
var re = /}{/g;

var categoryNews = Ext.create('Ext.data.Store', {
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

//newtreeStore
var newtreeStore = Ext.create('Ext.data.TreeStore', {
    proxy: {
        type: 'ajax',
        url: '/Product/GetCatagory?categoryType=2&coboType=' + coboType,
        getMethod: function () { return 'get'; },
        actionMethods: 'post'
    },
    root: {
        // text: '新的類別分類',
        expanded: true,
        children: [

        ]
    }
});

var categoryPanel = Ext.create('Ext.form.Panel', {
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
            value: SHOPCLASS + '<span style="color:red;">*</span>'
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
                url: '/Product/GetProdClassify?coboType=' + coboType,
                method: 'POST',
                params: {
                    ProductId: PRODUCT_ID,
                    OldProductId: OLD_PRODUCT_ID
                },
                success: function (response) {
                    var data = Ext.decode(response.responseText);
                    Ext.getCmp('reportNew').setValue(data.Prod_Classify);
                }
            });
        }
    }
});

newStageCategoryPanel = Ext.create('Ext.tree.Panel', {
    id: 'newStageCategoryPanel',
    //colName: 'newStageCategoryPanel',
    frame: false,
    animate: true,
    //hidden: true,
    lines: true,
    rootVisible: false,
    onlyLeafCheckable: true,
    minWidth: 200,
    width: 500,
    height: PRODUCT_ID != '' ? 720 : document.documentElement.clientHeight - 65,
    lines: true,
    enableDrag: true,
    allowBlank: false,
    store: newtreeStore,
    listeners: {
        afterlayout: function () {
            oldresultStr = getdata();
        }
    }
});

Ext.onReady(function () {
    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    Ext.create('Ext.container.Viewport', {
        id: 'cateViewport',
        layout: 'vbox',
        items: [categoryPanel, newStageCategoryPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                Ext.getCmp('newStageCategoryPanel').setHeight(document.documentElement.clientHeight - 97);
                this.doLayout();
            },
            beforerender: function (view) {
                window.parent.updateAuth(view, 'colName');
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
    newStageCategoryPanel.on('checkchange', function (node, checked) {
        setChecked(node, checked);
    });
    newtreeStore.load({ params: { ProductId: PRODUCT_ID, OldProductId: OLD_PRODUCT_ID } });
})

function getdata() {
    var data = newStageCategoryPanel.getChecked();
    if (data.length == 0) {
        return "[]";
    }
    var resultStr = '';
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

function save(functionid) {
    //添加 遮罩層  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: '請稍等...' });
    }
    mask.show();
    //添加disabled屬性  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    window.parent.setMoveEnable(false);

    var obj = Ext.getCmp('reportNew');
    if (!obj.getValue()) {
        obj.markInvalid(INPUT_PLEASE);
        window.parent.setMoveEnable(true);
        mask.hide();
        return false;
    }
    var prodClassify = obj.getValue();
    var currentCategoryId = obj.valueModels[0].data.ParameterProperty;
    var node = newStageCategoryPanel.getRootNode().findChild("id", currentCategoryId, true);

    //判斷 數據是否加載完成
    if (!node) {
        window.parent.setMoveEnable(true);
        mask.hide();
        return;
    }

    var firstCheckdNode = node.findChildBy(function (e) {
        return e.isLeaf() && e.data.checked
    }, this, true);

    if (!firstCheckdNode) {
        Ext.Msg.alert(NOTICE, PARALLELISM_PROD_CLASSIFY_MUST_CHECK_ONE_MESSAGE);
        window.parent.setMoveEnable(true);
        mask.hide();
        return;
    }

    var resultStr = getdata();
    var asyncResult = true;

    Ext.Ajax.request({
        url: '/Product/tempCategoryAdd?categoryType=2',
        method: 'POST',
        async: window.parent.GetProductId() == '' ? false : true,
        params: {
            'coboType': coboType,
            'ProductId': PRODUCT_ID,
            'prodClassify': prodClassify,
            'OldProductId': OLD_PRODUCT_ID,
            'oldresult': oldresultStr,
            'result': resultStr,
            "function": functionid,
            "batch": window.parent.GetBatchNo()
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            mask.hide();
            if (PRODUCT_ID != '') {
                if (resText.success) {
                    Ext.Msg.alert(NOTICE, SAVE_SUCCESS);
                }
                else {
                    Ext.Msg.alert(NOTICE, resText.msg ? resText.msg : SAVE_FAIL);
                }
            } else {
                if (!resText.success) {
                    Ext.Msg.alert(NOTICE, resText.msg ? resText.msg : SAVE_FAIL);
                    asyncResult = false;
                }
            }
            window.parent.setMoveEnable(true);
            if (resText.success) {
                newStageCategoryPanel.doLayout();
            }
        },
        failure: function (response, opts) {
            Ext.Msg.alert(NOTICE, SAVE_FAIL);
            window.parent.setMoveEnable(true);
            asyncResult = false;
        }
    });
    return asyncResult;
}