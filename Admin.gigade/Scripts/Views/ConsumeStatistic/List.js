/*
* 文件名稱 :List.js
* 文件功能描述 :會員消費金額統計列表js文件
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改備註 :無
 */
var pageSize = 30;

//列表Model
Ext.define('gigade.ConsumeStatistic', {
    extend: 'Ext.data.Model',
    fields:
        [
            { name: "row_id", type: "int" },
            { name: "user_id", type: "int" },
            { name: "user_name", type: "string" },
            { name: "year", type: "int" },
            { name: "month", type: "int" },
            { name: "order_product_subtotal", type: "int" },
            { name: "normal_product_subtotal", type: "int" },
            { name: "low_product_subtotal", type: "int" },
            { name: "buy_count", type: "int" },
            { name: "last_buy_time", type: "string" },
            { name: "buy_avg", type: "int" },
            { name: "note", type: "string" },
            { name: "create_datetime", type: "string" },
        ]
});
var typeStore = Ext.create('Ext.data.Store', {
    fields: ['searchTypeName', 'searchTypeValue'],
    data: [
        { "searchTypeName": "請選擇", "searchTypeValue": "0" },
        { "searchTypeName": "會員編號", "searchTypeValue": "1" },
        { "searchTypeName": "會員名稱", "searchTypeValue": "2" },
        //...
    ]
});
//到Controller獲取數據
var ListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,//每頁最大數據,傳到前臺 
    model: 'gigade.ConsumeStatistic',
    proxy: {
        type: 'ajax',
        url: '/ConsumeStatistic/GetUserOrdersSubtotalList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
ListStore.on('beforeload', function () {
    var reg = new RegExp(" ", "g"); //创建正则RegExp对象
    var strkey = Ext.getCmp("search").getValue();
    var startMoney = Ext.getCmp("startMoney").getValue();
    var endMoney = Ext.getCmp("endMoney").getValue();
    var startTime = Ext.getCmp("startTime").getValue();
    var endTime = Ext.getCmp("endTime").getValue();
    var searchType = Ext.getCmp("searchType").getValue()
    var searchKey = strkey.replace(reg, '');//將匹配正則的字符替換
    if (startMoney == 0) {
        Ext.getCmp("startMoney").setValue('');
    }
    if (endMoney == 0) {
        Ext.getCmp("endMoney").setValue('');
    }
    //判斷搜索類型以及關鍵字是否合法
    if (searchType != '0') {
        if (searchKey == '' || searchKey == null) {
            Ext.Msg.alert("提示信息", "請輸入搜索關鍵字");
            return false;
        }
    }
        //判斷金額區間數據是否合法
    else if (startMoney != null && endMoney != null) {
        if (startMoney > endMoney) {
            Ext.Msg.alert("提示信息", "金額範圍的下限金額大於上限金額");
            return false;
        }
    }
        //若沒有選擇任何搜索條件,則不允許查詢
    else if (startMoney == null && endMoney == null) {
        if (startTime == null || startTime == '') {
            Ext.Msg.alert("提示信息", "請選擇搜索條件");
            return false;
        }
    }
    Ext.getCmp("ListPanl").store.removeAll();
    Ext.apply(ListStore.proxy.extraParams, {
        startMoney: startMoney,
        endMoney: endMoney,
        startTime: startTime,
        endTime: endTime,
        searchType: searchType,
        searchKey: searchKey
    });
});

ListStore.on('load', function (ListStore) {
    var totalcount = ListStore.getCount();
    if (totalcount == 0) {
        Ext.MessageBox.alert("提示信息", "  ~沒有符合條件的數據～  ");
    }
});

///搜索-Query
function Query() {
    //默認加載第一頁數據
    ListStore.loadPage(1);
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else {
        s.setHours(23, 59, 59);
    }
    return s;
}

Ext.onReady(function () {
    ///頁面加載的時候創建grid.Panel
    var ListPanl = Ext.create('Ext.grid.Panel', {
        id: 'ListPanl',
        store: ListStore,
        width: document.documentElement.clientWidth,
        //hight: 700,//document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.8,
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        columns: [
            { header: '編號', dataIndex: 'row_id', flex: 1, align: 'center' },
            { header: '會員編號', dataIndex: 'user_id', flex: 1, align: 'center' },
            { header: '會員名稱', dataIndex: 'user_name', flex: 1, align: 'center' },
            { header: '平均購買金額', dataIndex: 'buy_avg', flex: 1, align: 'center' },
            { header: '購買數量', dataIndex: 'buy_count', flex: 1, align: 'center' },
            { header: '常溫金額', dataIndex: 'normal_product_subtotal', flex: 1, align: 'center' },
            { header: '低溫金額', dataIndex: 'low_product_subtotal', flex: 1, align: 'center' },
            { header: '消費總金額', dataIndex: 'order_product_subtotal', flex: 1, align: 'center' },
            { header: '最後購買時間', dataIndex: 'last_buy_time', flex: 1, align: 'center' },
            { header: '年', dataIndex: 'year', flex: 1, align: 'center' },
            { header: '月', dataIndex: 'month', flex: 1, align: 'center' },
            { header: '創建時間', dataIndex: 'create_datetime', flex: 1, align: 'center' },
            { header: '備註', dataIndex: 'note', flex: 1, align: 'center' }
        ],
        tbar: [
                        ///使查詢框處於右側
                        '->',
                         //================搜索條件================
                        {
                            xtype: 'combobox',
                            id: 'searchType',
                            margin: '0 5px',
                            fieldLabel: '會員編號/名稱',
                            labelWidth: 100,
                            width: 200,
                            colName: 'deliver_type',
                            queryMode: 'local',
                            editable: false,
                            lastQuery: '',
                            store: typeStore,
                            displayField: 'searchTypeName',
                            valueField: 'searchTypeValue',
                            value: "0",
                            listeners: {
                                select: function () {
                                    if (Ext.getCmp("searchType").getValue() == '0') {
                                        Ext.getCmp("search").setValue('');
                                    }
                                }
                            }
                        },
                    //================搜索關鍵字================
                    {
                        xtype: 'textfield',
                        id: 'search',
                        colName: 'search',
                        width: 100,
                        emptyText: '請輸入關鍵字',
                        msgTarget: 'side',
                        submitValue: false,
                        name: 'searchCode',
                        margin: "0 5",
                        listeners: {
                            focus: function () {
                                var searchType = Ext.getCmp("searchType").getValue();
                                if (searchType == null || searchType == '' || searchType == '0') {
                                    Ext.Msg.alert("提示信息", "請先選則搜索類型");
                                }
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                     //============金額區間===============
                    {
                        xtype: 'numberfield',
                        labelWidth: 80,
                        width: 180,
                        id: 'startMoney',
                        name: 'startMoney',
                        margin: '0 0 0 30',
                        fieldLabel: '總金額區間',
                        listeners: {
                            blur: function () {
                                var startMoney = Ext.getCmp("startMoney").getValue();
                                var endMoney = Ext.getCmp("endMoney").getValue();
                                if (startMoney != null && endMoney != null) {
                                    if (startMoney > endMoney) {
                                        Ext.Msg.alert("提示信息", "金額範圍的下限金額大於上限金額");
                                    }
                                }
                                if (startMoney == 0)
                                {
                                    Ext.getCmp("startMoney").setValue('');
                                }
                                if (endMoney == 0)
                                {
                                    Ext.getCmp("endMoney").setValue('');
                                }
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        value: '~',
                        margin: '0 5'
                    },
                     {
                         xtype: 'numberfield',
                         width: 100,
                         name: 'endMoney',
                         id: 'endMoney',
                         listeners: {
                             blur: function () {
                                 var startMoney = Ext.getCmp("startMoney").getValue();
                                 var endMoney = Ext.getCmp("endMoney").getValue();
                                 if (startMoney != null && endMoney != null) {
                                     if (startMoney > endMoney) {
                                         Ext.Msg.alert("提示信息", "金額範圍的起始金額不能大於結束金額");
                                     }
                                 }
                                 if (startMoney == 0) {
                                     Ext.getCmp("startMoney").setValue('');
                                 }
                                 if (endMoney == 0) {
                                     Ext.getCmp("endMoney").setValue('');
                                 }
                             },
                             specialkey: function (field, e) {
                                 if (e.getKey() == e.ENTER) {
                                     Query();
                                 }
                             }
                         }
                     },
                       //===========-時間區間=================
                       {
                           xtype: 'datetimefield',
                           id: 'startTime',
                           name: 'startTime',
                           fieldLabel: "創建時間區間",
                           margin: '0 5 0 40',
                           labelWidth: 100,
                           width: 255,
                           editable: false,
                           allowBlank: true,
                           format: 'Y-m-d  H:i:s',                          
                           time: { hour: 00, min: 00, sec: 00 },//標記結束時間00:00:00
                           editable: false,
                           //默認開始時間
                           //value: setNextMonth(Date.now(), -3),
                           listeners: {
                               select: function (a, b, c) {
                                   var startTime = Ext.getCmp("startTime");
                                   var endTime = Ext.getCmp("endTime");
                                   if (endTime.getValue() == null) {
                                       endTime.setValue(setNextMonth(startTime.getValue(), 3));
                                   } else if (startTime.getValue() > endTime.getValue()) {
                                       Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                       endTime.setValue(setNextMonth(startTime.getValue(), 3));
                                   }
                                   //else if (endTime.getValue() > setNextMonth(startTime.getValue(), 3)) {
                                   //    endTime.setValue(setNextMonth(startTime.getValue(), 3));
                                   //}
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == e.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       },
                        {
                            xtype: 'displayfield',
                            value: '~',
                            margin: '0 5'
                        },
                        {
                            xtype: 'datetimefield',
                            id: 'endTime',
                            name: 'endTime',
                            width: 150,
                            editable: false,
                            allowBlank: true,
                            margin: '0 10 0 5',
                            format: 'Y-m-d  H:i:s',
                            time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59    
                            // value: setNextMonth(Date.now(), 0),
                            listeners: {
                                select: function (a, b, c) {
                                    var startTime = Ext.getCmp("startTime");
                                    var endTime = Ext.getCmp("endTime");
                                    var s_date = new Date(startTime.getValue());
                                    var now_date = new Date(endTime.getValue());
                                    if (startTime.getValue() != "" && startTime.getValue() != null) {
                                        if (endTime.getValue() < startTime.getValue()) {
                                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                            startTime.setValue(setNextMonth(endTime.getValue(), -3));
                                        }
                                        //else if (endTime.getValue() > setNextMonth(startTime.getValue(), 3)) {
                                        //    startTime.setValue(setNextMonth(endTime.getValue(), -3));
                                        //}

                                    } else {
                                        startTime.setValue(setNextMonth(endTime.getValue(), -3));
                                    }
                                },
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
                            Ext.getCmp("startMoney").setValue('');
                            Ext.getCmp("endMoney").setValue('');
                            Ext.getCmp("startTime").reset();
                            Ext.getCmp("endTime").reset();
                            Ext.getCmp("searchType").setValue('0');
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
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [ListPanl],
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                ListPanl.width = document.documentElement.clientWidth;
                ListPanl.hight = document.documentElement.clientHeight,
                 this.doLayout();
            }
        }
    });
});
