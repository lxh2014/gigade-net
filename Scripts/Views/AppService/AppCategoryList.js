/*
* 文件名稱 :AppCategoryList.js
* 文件功能描述 :訊息公告編輯JS
* 版權宣告 :
* 開發人員 : 白明威
* 版本資訊 : 1.0
* 日期 : 2015.8.27
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.AppCategory', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "category_id", type: "int" },//序號
        { name: "category", type: "string" },//館別
        { name: "brand_id", type: "int" },//品牌ID
        { name: "brand_name", type: "string" },//品牌名稱
        { name: "category1", type: "string" },//分類一
        { name: "category2", type: "string" },//分類二
        { name: "category3", type: "string" },//分類三
        { name: "product_id", type: "int" },//產品ID
        { name: "property", type: "string" },//屬性
    ]
});

//頁面顯示數據
var AppCategoryStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    autoLoad: false,
    model: 'gigade.AppCategory',
    proxy: {
        type: 'ajax',
        url: '/AppService/GetAppCategoryList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//查詢時所用的參數，在分頁的情況下可以附帶出查詢條件進行查詢
AppCategoryStore.on("beforeload", function () {
    var category = Ext.getCmp('category').getValue();
    var category1 = Ext.getCmp('category1').getValue();
    var category2 = Ext.getCmp('category2').getValue();
    var category3 = Ext.getCmp('category3').getValue();
    var product_id = Ext.getCmp('product_id').getValue();
    AppCategoryStore.removeAll();
    if (category == null && category1 == null && category2 == null && category3 == null && product_id == '') {
        Ext.Msg.alert(INFORMATION, SEARCHNULLTEXT);
        return false;
    }
    Ext.apply(AppCategoryStore.proxy.extraParams, {
        category: category,
        category1: category1,
        category2: category2,
        category3: category3,
        product_id: product_id
    });
});

//館別store
var ShopClassStore = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/AppService/QueryPara?paraType=category',
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//分類一store
var Category1Store = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/AppService/QueryPara?paraType=category1',
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//分類二store
var Category2Store = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/AppService/QueryPara?paraType=category2',
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//分類三store
var Category3Store = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/AppService/QueryPara?paraType=category3',
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//刪除數據，根據選擇的行數判斷刪除按鈕是否被disabled
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("remove").setDisabled(selections.length == 0);
        }
    }
});

//查詢按鈕所觸發的事件
function Query(x) {
    Ext.getCmp("AppCategoryList").store.loadPage(1, {
        params: {

        }
    });
}

//匯入Excel的格式驗證
Ext.apply(Ext.form.field.VTypes, {
    excelFilter: function (val, field) {
        var excels = 'xls,xlsx'.split(','); //上傳的文件格式
        var type = val.split('.')[val.split('.').length - 1].toLocaleLowerCase();
        for (var i = 0; i < excels.length; i++) {
            if (excels[i] == type) {
                return true;
            }
        }
        return false;
    },
    excelFilterText: IMPORT_TYPE_ERROR,
});

Ext.onReady(function () {
    //回車鍵查詢
    // edit by zhuoqin0830w  2015/09/22  以兼容火狐瀏覽器
    document.onkeydown = function (event) {
        e = event ? event : (window.event ? window.event : null);
        if (e.keyCode == 13) {
            $("#btnQuery").click();
        }
    };

    //頁面數據顯示
    var AppCategoryList = Ext.create('Ext.grid.Panel', {
        id: 'AppCategoryList',
        store: AppCategoryStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "", xtype: 'rownumberer', width: 38, align: 'center', sortable: false, menuDisabled: true },
            { header: CATEGORY_ID, dataIndex: 'category_id', width: 60, align: 'center', sortable: false, menuDisabled: true },
            { header: PRODUCT_ID, dataIndex: 'product_id', width: 90, align: 'center', sortable: false, menuDisabled: true },
            { header: CATEGORY, dataIndex: 'category', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { header: BRAND_ID, dataIndex: 'brand_id', width: 60, align: 'center', sortable: false, menuDisabled: true },
            { header: BRAND_NAME, dataIndex: 'brand_name', width: 150, align: 'center', sortable: false, menuDisabled: true },
            { header: CATEGORY_1, dataIndex: 'category1', width: 130, align: 'center', sortable: false, menuDisabled: true },
            { header: CATEGORY_2, dataIndex: 'category2', width: 130, align: 'center', sortable: false, menuDisabled: true },
            { header: CATEGORY_3, dataIndex: 'category3', width: 130, align: 'center', sortable: false, menuDisabled: true },
            {
                header: PROPERTY, dataIndex: 'property', width: 60, align: 'center', sortable: false, menuDisabled: true,
                renderer: function (value) {
                    if (value == 1) {
                        return HOME_DISPATCH;
                    }
                    else if (value == 2) {
                        return SHOP_DISPATCH;
                    }
                    else if (value == 3) {
                        return IN24H_DISPATCH;
                    }
                    else if (value == 4) {
                        return SHOP_DISPATCH_NO_SHIP;
                    }
                    else {
                        return "";
                    }
                }
            }
        ],
        dockedItems: [{
            dock: 'top',
            xtype: 'toolbar',
            items: [{
                xtype: 'combo',
                fieldLabel: CATEGORY,
                listConfig: { loadMask: false },
                id: 'category',
                store: ShopClassStore,
                displayField: 'parameterName',
                labelWidth: 35,
                width: 150,
                queryMode: 'local',
                emptyText: PLEASE_SELECT,
                name: 'category',
                editable: false,
                listeners: {
                    select: function () {
                        selectCondition = this.getValue();
                        Ext.getCmp('category1').reset();
                        Category1Store.removeAll();
                        Category1Store.load({ params: { 'selectCondition': this.getValue() } });
                        Ext.getCmp('category2').reset();
                        Ext.getCmp('category2').setDisabled(true);
                        Ext.getCmp('category3').reset();
                        Ext.getCmp('category3').setDisabled(true);
                    }
                }
            }, {
                xtype: 'combo',
                fieldLabel: CATEGORY_1,
                listConfig: { loadMask: false },
                id: 'category1',
                store: Category1Store,
                displayField: 'parameterName',
                labelWidth: 45,
                width: 150,
                name: 'category1',
                queryMode: 'local',
                emptyText: PLEASE_SELECT,
                editable: false,
                listeners: {
                    select: function () {
                        selectCondition = this.getValue();
                        Ext.getCmp('category2').reset();
                        Ext.getCmp('category2').setDisabled(false);
                        Category2Store.removeAll();
                        Category2Store.load({ params: { 'selectCondition': Ext.getCmp('category').getValue(), 'select1Condition': this.getValue() } });
                        Ext.getCmp('category3').reset();
                        Ext.getCmp('category3').setDisabled(true);
                    }
                }
            }, {
                xtype: 'combo',
                fieldLabel: CATEGORY_2,
                listConfig: { loadMask: false },
                id: 'category2',
                store: Category2Store,
                displayField: 'parameterName',
                labelWidth: 45,
                width: 150,
                name: 'category2',
                queryMode: 'local',
                emptyText: PLEASE_SELECT,
                editable: false,
                listeners: {
                    select: function () {
                        selectCondition = this.getValue();
                        Ext.getCmp('category3').reset();
                        Ext.getCmp('category3').setDisabled(false);
                        Category3Store.removeAll();
                        Category3Store.load({ params: { 'selectCondition': Ext.getCmp('category').getValue(), 'select1Condition': Ext.getCmp('category1').getValue(), 'select2Condition': this.getValue() } });
                    }
                }
            }, {
                xtype: 'combo',
                fieldLabel: CATEGORY_3,
                listConfig: { loadMask: false },
                id: 'category3',
                store: Category3Store,
                displayField: 'parameterName',
                labelWidth: 45,
                width: 150,
                name: 'category3',
                queryMode: 'local',
                emptyText: PLEASE_SELECT,
                editable: false
            }, {
                xtype: 'textfield',
                fieldLabel: PRODUCT_ID,
                id: 'product_id',
                labelWidth: 45,
                width: 150,
                name: 'product_id',
                emptyText: PLEASE_IMPORT_PRODUCT_ID,
                editable: false,
                enableKeyEvents: true,
                listeners: {
                    keyup: function (e, event) {
                        if (event.keyCode == 13) {
                            $("#btnQuery").click();
                        }
                    }
                }
            }, {
                xtype: 'button',
                text: QUERY,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }, {
                xtype: 'button',
                text: RESET,
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp('category').reset();
                        Ext.getCmp('category1').reset();
                        Ext.getCmp('category2').reset();
                        Ext.getCmp('category3').reset();
                        Ext.getCmp('product_id').reset();
                        ShopClassStore.load();
                        Category1Store.load();
                        Category2Store.load();
                        Category3Store.load();
                        Ext.getCmp('category2').setDisabled(false);
                        Ext.getCmp('category3').setDisabled(false);
                    }
                }
            }]
        }],
        tbar: [
            { xtype: 'button', text: DELETE, id: 'remove', iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            { xtype: 'button', text: IMPORT_EXCEL, id: 'insertexcel', iconCls: 'icon-add', disabled: false, handler: onInsertExcelClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: AppCategoryStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [AppCategoryList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                AppCategoryList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //AppCategoryStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("AppCategoryList").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.category_id + '|';
                }
                Ext.Ajax.request({
                    url: '/AppService/AppCategoryDelete',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            AppCategoryStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

/*************************************************************************************Excel匯入*************************************************************************************************/
var excelFrm = Ext.create('Ext.form.Panel', {
    id: 'excelFrm',
    layout: 'anchor',
    frame: true,
    plain: true,
    width: 570,
    height: 52,
    border: false,
    url: '/AppService/AppCategoryUpExcel',
    bodyStyle: "padding: 12px 12px 6px 12px;",
    items: [{
        xtype: 'fieldcontainer',
        //combineErrors: true,
        layout: 'hbox',
        items: [{
            xtype: 'filefield',
            fieldLabel: IMPORT_EXCEL,//匯入Excel
            labelWidth: 70,
            width: 350,
            id: 'ImportFileMsg',
            name: 'ImportFileMsg',
            buttonText: PLEASE_IMPORT_EXCEL,//請匯入檔案
            allowBlank: false,
            vtype: 'excelFilter'
        }, {
            xtype: 'button',
            text: IMPORT_AFFIRM,//確定匯入
            id: 'btnExcel',
            formBind: true,
            disabled: true,
            width: 80,
            margin: '0 10 0 10px',
            //flex: 1,
            handler: function () {
                var form = this.up('form').getForm();
                //驗證表單
                if (form.isValid()) {
                    //提交表單
                    form.submit({
                        success: function (from, action) {
                            addExcel.hide();
                            if (action.result.errorRow == null) {
                                Ext.Msg.alert(INFORMATION, Ext.String.format(IMPORT_RESULT_TRUE_MESSAGE, action.result.total));
                            } else {
                                Ext.Msg.alert(INFORMATION, Ext.String.format(IMPORT_RESULT_TRUE_ERROR_MESSAGE, action.result.total, action.result.fail, action.result.errorRow ? action.result.errorRow : 0));
                            }
                        },
                        failure: function (from, action) {
                            Ext.Msg.alert(INFORMATION, Ext.String.format(IMPORT_RESULT_FALSE_MESSAGE, action.result.msg));
                        }
                    })
                }
            }
        }, {
            xtype: 'displayfield',
            value: '<a href="/AppService/CategoryTemplate">' + EXCEL_FORMWORK_DOWNLOAD + '</a>'//範本下載
        }, {
            xtype: 'displayfield',
            value: ' ',
            margin: '0 2px',
            labelWidth: 10,
            flex: 20
        }]
    }]
})

var addExcel = Ext.create('Ext.window.Window', {
    title: IMPORT_EXCEL,
    id: 'addExcel',
    width: 600,
    height: 100,
    iconCls: 'ui-icon ui-icon-add',
    plain: true,
    border: false,
    modal: true,
    resizable: false,
    draggable: true,
    hidden: true,
    bodyStyle: "padding: 10px 10px 7px 10px;",
    layout: 'fit',
    items: [excelFrm],
    closable: false,
    tools: [{
        type: 'close',
        handler: function (event, toolEl, panel) {
            addExcel.hide();
        }
    }]
})

function onInsertExcelClick() {
    excelFrm.getForm().reset();
    addExcel.show();
}