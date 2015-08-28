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

var AppCategoryStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
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
    //    autoLoad: true
});
//查詢時所用的參數，在分頁的情況下可以附帶出查詢條件進行查詢
AppCategoryStore.on("beforeload", function () {
    Ext.apply(AppCategoryStore.proxy.extraParams, {
        category: Ext.getCmp('category') ? Ext.getCmp('category').getValue() : '',
        category1: Ext.getCmp('category1') ? Ext.getCmp('category1').getValue() : '',
        category2: Ext.getCmp('category2') ? Ext.getCmp('category2').getValue() : '',
        category3: Ext.getCmp('category3') ? Ext.getCmp('category3').getValue() : '',
        product_id: Ext.getCmp("product_id").getValue() ? Ext.getCmp("product_id").getValue() : 0
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
    AppCategoryStore.removeAll();
    Ext.getCmp("AppCategoryList").store.loadPage(1, {
        params: {

        }
    });
}

Ext.onReady(function () {
    var excelFrm = Ext.create('Ext.form.Panel', {
        id: 'excelFrm',
        layout: 'anchor',
        frame: true,
        plain: true,
        border: false,
        height: 45,
        url: '/AppService/AppCategoryUpExcel',
        width: document.documentElement.clientWidth,
        bodyStyle: "padding: 12px 12px 6px 12px;",
        items: [
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [{
                      xtype: 'filefield',
                      fieldLabel: "匯入Execl",
                      labelWidth: 70,
                      flex: 8,
                      id: 'ImportFileMsg',
                      name: 'ImportFileMsg',
                      buttonText: "請匯入檔案",
                      allowBlank: false,
                      validator: function (value) {
                          var type = value.split('.');
                          if (type[type.length - 1] == 'xls' || type[type.length - 1] == 'xlsx') {
                              return true;
                          } else {
                              return '上傳文件類型不正確！';
                          }
                      }
                  },
                  {
                      xtype: 'button',
                      text: '確定匯入',
                      id: 'btnExcel',
                      formBind: true,
                      disabled: true,
                      margin: '0 0 0 20px',
                      flex: 1,
                      handler: function () {
                          onSubmit();
                      }
                  },
                       {
                           xtype: 'displayfield',
                           value: ' ',
                           margin: '0 2px',
                           labelWidth: 10,
                           flex: 20
                       }
                  ]
              }]
    })
    var AppCategoryList = Ext.create('Ext.grid.Panel', {
        id: 'AppCategoryList',
        store: AppCategoryStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "", xtype: 'rownumberer', width: 38, align: 'center', sortable: false, menuDisabled: true },
            { header: "序號", dataIndex: 'category_id', width: 60, align: 'center', sortable: false, menuDisabled: true },
            { header: "產品ID", dataIndex: 'product_id', width: 90, align: 'center', sortable: false, menuDisabled: true },
            { header: "館別", dataIndex: 'category', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { header: "品牌ID", dataIndex: 'brand_id', width: 60, align: 'center', sortable: false, menuDisabled: true },
            { header: "品牌名稱", dataIndex: 'brand_name', width: 150, align: 'center', sortable: false, menuDisabled: true },
            { header: "分類一", dataIndex: 'category1', width: 130, align: 'center', sortable: false, menuDisabled: true },
            { header: "分類二", dataIndex: 'category2', width: 130, align: 'center', sortable: false, menuDisabled: true },
            { header: "分類三", dataIndex: 'category3', width: 130, align: 'center', sortable: false, menuDisabled: true },
            {
                header: "屬性", dataIndex: 'Sort', width: 60, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return "宅配";
                    }
                    else if (value == 2) {
                        return "店配";
                    }
                    else if (value == 3) {
                        return "24hr到貨";
                    }
                    else if (value == 4) {
                        return "宅配免運";
                    }
                    else {
                        return "";
                    }
                }
            }
        ],
        tbar: [{
            xtype: 'button', text: "刪除", id: 'remove', iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick
        }, '->', {
            xtype: 'combo',
            fieldLabel: '館別',
            listConfig: { loadMask: false },
            id: 'category',
            store: ShopClassStore,
            displayField: 'parameterName',
            labelWidth: 35,
            width: 150,
            queryMode: 'local',
            emptyText: '--請選擇--',
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
            },
        }, {
            xtype: 'combo',
            fieldLabel: '分類一',
            listConfig: { loadMask: false },
            id: 'category1',
            store: Category1Store,
            displayField: 'parameterName',
            labelWidth: 45,
            width: 150,
            name: 'category1',
            queryMode: 'local',
            emptyText: '--請選擇--',
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
            },
        }, {
            xtype: 'combo',
            fieldLabel: '分類二',
            listConfig: { loadMask: false },
            id: 'category2',
            store: Category2Store,
            displayField: 'parameterName',
            labelWidth: 45,
            width: 150,
            name: 'category2',
            queryMode: 'local',
            emptyText: '--請選擇--',
            editable: false,
            listeners: {
                select: function () {
                    selectCondition = this.getValue();
                    Ext.getCmp('category3').reset();
                    Ext.getCmp('category3').setDisabled(false);
                    Category3Store.removeAll();
                    Category3Store.load({ params: { 'selectCondition': Ext.getCmp('category').getValue(),'select1Condition': Ext.getCmp('category1').getValue(), 'select2Condition': this.getValue() } });
                }
            },
        }, {
            xtype: 'combo',
            fieldLabel: '分類三',
            listConfig: { loadMask: false },
            id: 'category3',
            store: Category3Store,
            displayField: 'parameterName',
            labelWidth: 45,
            width: 150,
            name: 'category3',
            queryMode: 'local',
            emptyText: '--請選擇--',
            editable: false,
        }, {
            xtype: 'textfield',
            fieldLabel: '產品ID',
            id: 'product_id',
            labelWidth: 45,
            width: 150,
            name: 'product_id',
            emptyText: '請輸入商品ID',
            editable: false
        }, {
            xtype: 'button',
            text: '查詢',
            iconCls: 'icon-search',
            id: 'btnQuery',
            handler: Query
        }, {
            xtype: 'button',
            text: '重置',
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
        }],
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
        items: [excelFrm, AppCategoryList],
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
    AppCategoryStore.load({ params: { start: 0, limit: 25 } });
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

function onSubmit() {
    var form = Ext.getCmp('excelFrm').getForm();
    if (form.isValid()) {
        form.submit({
            params: {
                ImportFileMsg: Ext.htmlEncode(Ext.getCmp('ImportFileMsg').getValue())
            },
            success: function (form, action) {
                var result = Ext.decode(action.response.responseText);
                if (result.success) {

                }
            },
            failure: function (form, action) {
                var result = Ext.decode(action.response.responseText);
                Ext.Msg.alert("提示", result.msg);
                alert("失敗");

            }
        });
    }
}