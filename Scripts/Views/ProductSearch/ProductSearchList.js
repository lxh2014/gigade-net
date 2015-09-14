
var pageSize = 25;

//列表Model
Ext.define('gigade.ProductSearch', {
    extend: 'Ext.data.Model',
    fields:
        [
            { name: "Product_Id", type: "int" },
            { name: "Product_Name", type: "string" },
            { name: "Page_Content_1", type: "string" },
            { name: "Page_Content_2", type: "string" },
            { name: "Page_Content_3", type: "string" },
            { name: "Product_Keywords", type: "string" },
            { name: "product_detail_text", type: "string" },
        ]
});
//到Controller獲取數據
var ListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,//每頁最大數據,傳到前臺 
    model: 'gigade.ProductSearch',
    proxy: {
        type: 'ajax',
        url: '/ProductSearch/GetList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
ListStore.on('beforeload', function () {
    Ext.getCmp("gridList").store.removeAll();
    var searchKey = Ext.getCmp("search").getValue();
    var reg = new RegExp(" ", "g"); //创建正则RegExp对象
    var strflag = '';
    var searchKey = Ext.getCmp("search").getValue();
    if (Ext.getCmp("iskey").getValue()) {
        strflag = '1';
    }
    if (Ext.getCmp("notkey").getValue()) {
        strflag = '0';
    }
    var strkey = Ext.getCmp("search").getValue();
    var newKey = strkey.replace(reg, '');//將匹配正則的字符替換

    Ext.apply(ListStore.proxy.extraParams, {
        searchKey: newKey,
        flag: strflag
    });
    Ext.getCmp('product_detail_text').setValue('');
});

ListStore.on('load', function (ListStore) {
    var totalcount = ListStore.getCount();
    if (totalcount == 0) {
        Ext.MessageBox.alert("提示信息", "  ~沒有符合條件的數據～  ");
    }
});

///搜索-Query
function Query() {
    var strflag = '';
    var searchKey = Ext.getCmp("search").getValue();
    if (Ext.getCmp("iskey").getValue()) {
        strflag = '1';
    }
    if (Ext.getCmp("notkey").getValue()) {
        strflag = '0';
    }

    if (strflag == '' && searchKey == '') {
        Ext.Msg.alert("提示信息", "請選擇搜索條件")
        return false;
    }
    else {
        Ext.getCmp('product_detail_text').setValue('');
        var searchKey = Ext.getCmp("search").getValue();//搜索關鍵字
        ListStore.loadPage(1);
    }
}
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gridList").down('#remove').setDisabled(selections.length == 0);
        }
    }
});

onDeleteClick = function () {
    var row = Ext.getCmp("gridList").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }

    else if (row.length == 1) {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var id = '';
                for (var i = 0; i < row.length; i++) {
                    id += row[i].data.Product_Id;
                }
                Ext.Ajax.request({
                    url: '/ProductSearch/RemoveSystemKeyWord',
                    method: 'post',
                    params: { Product_Id: id },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            var totalcount = ListStore.getCount();
                            if (totalcount != 0) {
                                ListStore.load();
                            }

                        }
                        else {
                            Ext.Msg.alert("提示信息", "移除失敗");
                            ListStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
    else {
        Ext.Msg.alert("提示信息", "一次只能移除一條數據!")
    }
}

Ext.onReady(function () {
    ///頁面加載的時候創建grid.Panel
    var gridList = Ext.create('Ext.grid.Panel', {
        id: 'gridList',
        frame: true,
        autoHeight: true,
        columnLines: true,
        flex: 8.1,
        store: ListStore,
        columns: [
            { header: '商品編號', dataIndex: 'Product_Id', flex: 1, align: 'center' },
            { header: '商品名稱', dataIndex: 'Product_Name', flex: 2, align: 'center' },
            { header: '內容詳情一', dataIndex: 'Page_Content_1', flex: 2, align: 'center' },
            { header: '內容詳情二', dataIndex: 'Page_Content_2', flex: 2, align: 'center' },
            { header: '內容詳情三', dataIndex: 'Page_Content_3', flex: 2, align: 'center' },
            { header: '商品關鍵字', dataIndex: 'Product_Keywords', flex: 2, align: 'center' },
            { header: '商品詳情', dataIndex: 'product_detail_text', flex: 2, align: 'center' }
        ],
        tbar: [
             {
                 xtype: 'button',
                 text: '移除',
                 id: 'remove',
                 iconCls: 'icon-user-remove',
                 disabled: true,
                 handler: onDeleteClick
             },
            ///使查詢框處於右側
            '->',
            {
                xtype: 'fieldcontainer',
                fieldLabel: '是否食安關鍵字',
                labelWidth: 100,
                width: 200,
                defaultType: 'radiofield',
                defaults: {
                    flex: 1
                },
                layout: 'hbox',
                items: [
                    {
                        boxLabel: '是',
                        name: 'flog',
                        id: 'iskey',
                        checked: true
                    },
                    {
                        boxLabel: '否',
                        name: 'flog',
                        id: 'notkey',
                        checked: false
                    }
                ]
            },
            //--------------------搜索關鍵字--------------------
            {
                xtype: 'textfield',
                fieldLabel: '搜索關鍵字',
                labelWidth: 80,
                id: 'search',
                colName: 'search',
                //allowBlank: false,
                maxLength: 15,
                emptyText: '請輸入關鍵字',
                //blankText: '關鍵字不能為空',
                maxLengthText: '關鍵字長度不能超過15个字符',
                msgTarget: 'side',
                submitValue: false,
                name: 'searchCode',
                margin: "0 10 0 20",
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            },
            { xtype: 'button', text: '查詢', iconCls: 'icon-search', handler: Query },
            {
                xtype: 'button',
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    Ext.getCmp('search').setValue('');
                }
            },
        ],
        bbar:
            Ext.create('Ext.PagingToolbar',
    {
        store: ListStore,
        pageSize: pageSize,
        displayInfo: true,
        displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
        emptyMsg: "沒有記錄可以顯示"
    }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            },
            itemclick: function (view, record, item, index, e) {
                Ext.getCmp('product_detail_text').setValue("商品編號 : " + record.data.Product_Id + "<br/>" +
                    "<br/>商品名稱 : " + record.data.Product_Name + "<br/>" +
                    "<br/>內容詳情一 : " + record.data.Page_Content_1 + "<br/>" +
                    "<br/>內容詳情二 : " + record.data.Page_Content_2 + "<br/>" +
                    "<br/>內容詳情三 : " + record.data.Page_Content_3 + "<br/>" +
                    "<br/>商品關鍵字 : " + record.data.Product_Keywords + "<br/>" +
                     "<br/>商品詳情 : " + record.data.product_detail_text
                     )
            }
        },
        selModel: sm
    });
    ///商品詳情
    var formPanel = Ext.create('Ext.form.Panel', {
        id: 'formPanel',
        frame: true,
        autoScroll: true,
        bodyPadding: 10,
        width: document.documentElement.clientWidth - 50,
        items:
            [
              //商品詳情
               {
                   xtype: 'displayfield',
                   id: 'product_detail_text',
                   name: 'product_detail_text',
                   width: document.documentElement.clientWidth - 100
               },
            ]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [gridList, formPanel],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                gridList.width = document.documentElement.clientWidth;
                gridList.height = document.documentElement.clientHeight * 2 / 3;
                formPanel.width = document.documentElement.clientWidth;
                formPanel.height = document.documentElement.clientHeight / 3;
                this.doLayout();
            }
        }
    });
});
