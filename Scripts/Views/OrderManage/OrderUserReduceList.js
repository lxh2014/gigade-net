var pageSize = 25;
var info_type = "users";
var secret_info = "";
//列表頁Model
Ext.define('gigade.OrderUserReduce', {
    extend: 'Ext.data.Model',
    fields: [
              { name: "order_id", type: "int" }, //付款單號
              { name: "user_name", type: "string" }, //會員名稱
              { name: "group_name", type: "string" }, //會員群組
              { name: "name", type: "string" }, //商品類型
              { name: "type", type: "string" }, //活動運送類別
              { name: "order_type", type: "string" }, //類別使用狀況
              { name: "created", type: "string" }, //建立時間
              { name: "user_id", type: "int" }, //用戶編號  
              { name: "group_id", type: "int" },//會員群組id
              { name: "emp_id", type: "string" },
              { name: "screatedate", type: "string" },//建立日期
              { name: "user_email", type: "string" }, //用戶郵箱
              { name: "reg_date", type: "string" },
              { name: "user_password", type: "string" }, //密碼
              { name: "user_gender", type: "string" }, //性別
              { name: "user_birthday_year", type: "string" }, //年
              { name: "user_birthday_month", type: "string" }, //月
              { name: "user_birthday_day", type: "string" }, //日
              { name: "birthday", type: "string" },
              { name: "user_phone", type: "string" }, //行動電話
              { name: "user_zip", type: "string" }, //用戶地址
              { name: "user_address", type: "string" }, //用戶地址
              { name: "user_mobile", type: "string" }, //聯絡電話
              { name: "suser_reg_date", type: "string" }, //註冊日期
              { name: "user_type", type: "string" }, //用戶類別
              { name: "send_sms_ad", type: "string" }, //是否接收簡訊廣告
              { name: "adm_note", type: "string" }, //管理員備註
              { name: "mytype", type: "string" }
    ]
});
//列表頁數據源
var OrderUserReduceListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.OrderUserReduce',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderUserReduce',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
Ext.define('gigade.Users', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" }, //用戶編號     上面的是編輯的時候關係到的
        { name: "user_email", type: "string" }, //用戶郵箱
        { name: "user_name", type: "string" }, //用戶名
        { name: "user_password", type: "string" }, //密碼
        { name: "user_gender", type: "string" }, //性別
        { name: "user_birthday_year", type: "string" }, //年
        { name: "user_birthday_month", type: "string" }, //月
        { name: "user_birthday_day", type: "string" }, //日
        { name: "birthday", type: "string" }, //生日 
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_address", type: "string" }, //用戶地址
        { name: "user_actkey", type: "string" },
        { name: "user_mobile", type: "string" },
        { name: "user_phone", type: "string" }, //行動電話
        { name: "reg_date", type: "string" }, //註冊日期 
        { name: "mytype", type: "string" },//會員類別
        { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告 
        { name: "adm_note", type: "string" }, //管理員備註   上面這些編輯時要帶入的值
        { name: "user_type", type: "string" }, //用戶類別   下面的這些結合上面的會顯示在列表頁
        { name: "user_status", type: "string" }, //用戶狀態
        { name: "sfirst_time", type: "string" }, //首次註冊時間
        { name: "slast_time", type: "string" }, //下次時間
        { name: "sbe4_last_time", type: "string" }, //下下次時間
        { name: "user_company_id", type: "string" },
        { name: "user_source", type: "string" },
        { name: "source_trace", type: "string" },
        { name: "s_id", type: "string" },
        { name: "source_trace_url", type: "string" },
        { name: "redirect_name", type: "string" },
        { name: "redirect_url", type: "string" },
        { name: "paper_invoice", type: "bool" }
    ]
});
secret_info = "user_id;user_email;user_name;user_mobile";
var edit_UserStore = Ext.create('Ext.data.Store', {
    //  autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Users',
    proxy: {
        type: 'ajax',
        url: '/Member/UsersList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});

OrderUserReduceListStore.on('beforeload', function () {
    Ext.apply(OrderUserReduceListStore.proxy.extraParams,
        {
            select_type: Ext.getCmp('select_type').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            reduce_id: Ext.getCmp('reduce_id').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue(),
            group_id: Ext.getCmp('group_id').getValue(),
            type: Ext.getCmp("type").getValue().stype
        });
});
var seatchContentStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
         { "txt": "所有資料", "value": "0" },
        { "txt": "付款單號", "value": "1" },
        { "txt": "會員姓名", "value": "2" },
        { "txt": "會員編號", "value": "3" }
    ]
});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "建立時間", "value": "1" }
    ]
});

Ext.onReady(function () {
    var form = Ext.create('Ext.form.Panel', {
        id: 'form',
        height: 125,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
           {
               xtype: 'fieldcontainer',
               layout: 'hbox',
               margin: '5 0 0 5',
              
               items: [
                    {
                        fieldLabel: "查詢條件",
                        labelWidth: 60,
                        xtype: 'combobox',
                        id: 'select_type',
                        name: 'select_type',
                        store: seatchContentStore,
                        queryMode: 'local',
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        value: 0
                    },
                    {
                        xtype: 'textfield',
                        margin: '0 0 0 10',
                        fieldLabel: "查詢內容",
                        id: 'search_con',
                        labelWidth: 60,
                        name: 'search_con',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'reduce_id',
                        name: 'reduce_id',
                        store: PromotionsAmoutReduceStore,
                        queryMode: 'local',
                        margin: '0 0 0 10',
                        fieldLabel: "減免活動",
                        labelWidth: 60,
                        displayField: 'name',
                        valueField: 'id',
                        editable: false,
                        emptyText: '所有活動',
                        listeners: {
                            beforerender: function () {
                                PromotionsAmoutReduceStore.load();
                            }
                        }
                    }
               ]
           },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '0 0 0 5',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: "群組會員",
                        labelWidth: 60,
                        id: 'group_id',
                        name: 'group_id',
                        store: VipUserGroupStore, 
                        displayField: 'group_name',
                        valueField: 'group_id',
                        queryMode: 'local',
                        editable: false,
                        emptyText: '所有群組',
                        listeners: {
                            beforerender: function () {
                                VipUserGroupStore.load();
                            }
                        }
                    },
                    {
                        fieldLabel: '運送類別',
                        margin: '0 0 0 10',
                        xtype: 'radiogroup',
                        layout: 'hbox',
                        labelWidth: 60,
                        id: 'type',
                        defaults: {
                            name: 'stype'
                        },
                        colName: 'stype',
                        width: 250,
                        columns: 3,
                        vertical: true,
                        items: [
                                { id: 'id1', name: 'stype', boxLabel: "全部", inputValue: '0', checked: true },
                                { id: 'id2', name: 'stype', boxLabel: "常溫", inputValue: '1' },
                                { id: 'id3', name: 'stype', boxLabel: "低溫", inputValue: '2' }
                        ]
                    }
                ]
            },
           {
               xtype: 'fieldcontainer',
               combineErrors: true,
               layout: 'hbox',
               margin: '0 0 0 5',
               items: [
                    {
                        xtype: 'combobox',
                        id: 'date',
                        name: 'date',
                        fieldLabel: "日期條件",
                        labelWidth: 60,
                        store: dateStore,
                        displayField: 'txt',
                        valueField: 'value',
                        editable: false,
                        value: 0
                    },
                    {
                        xtype: "datefield",
                        labelWidth: 60,
                        margin: '0 0 0 10',
                        id: 'start_time',
                        name: 'start_time',
                        format: 'Y-m-d',
                        editable: false,
                        allowBlank: false,
                        value: new Date(Today().setMonth(Today().getMonth() - 1)),
                        listeners: {
                            select: function (a, b, c) {
                                var Month = new Date(this.getValue()).getMonth() + 1;
                                Ext.getCmp("end_time").setValue(new Date(new Date(this.getValue()).setMonth(Month)));
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        margin: '0 10 0 10',
                        value: "~"
                    },
                    {
                        xtype: "datefield",
                        format: 'Y-m-d',
                        id: 'end_time',
                        name: 'end_time',
                        allowBlank: false,
                        editable: false,
                        submitValue: true,
                        value: Today(),
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                var s_date = start.getValue();
                                if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert("提示信息", "開始時間不能大於結束時間！");
                                    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                }
                            }
                        }
                    }
               ]
           }
        ],
        buttonAlign: 'left',
        buttons: [
            {
                text: '查詢',
                margin: '0 10 0 10',
                iconCls: 'icon-search',
                handler: Query
            },
            {
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    this.up('form').getForm().reset();
                }
            }
        ]
    });
    var OrderUserReduceListGrid = Ext.create('Ext.grid.Panel', {
        id: 'OrderUserReduceListGrid',
        store: OrderUserReduceListStore,
        columnLines: true,
        frame: true,
        flex: 8.1,
        width: document.documentElement.clientWidth,
        columns: [
           {
               header: "付款單號", dataIndex: 'order_id', width: 120, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                   if (value != null) {
                       return '<a href=javascript:TransToOrder(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                   }
               }
           },
           {
               header: "會員姓名", dataIndex: 'user_name', width: 120, align: 'center',
               renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                   return "<a href='javascript:void(0);'  onclick='onUserEditClick()'>" + value + "</a>";
               }
           },
           { header: "會員群組", dataIndex: 'group_name', width: 160, align: 'center' },
           { header: "減免活動名稱", dataIndex: 'name', width: 180, align: 'center' },
           {
               header: "活動運送類別", dataIndex: 'type', width: 120, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                   switch (value) {
                       case "0":
                           return ' 不分 ';
                           break;
                       case "1":
                           return ' 常溫 ';
                           break;
                       case "2":
                           return ' 低溫 ';
                           break;
                   }
               }
           },
           {
               header: "類別使用狀況", dataIndex: 'order_type', width: 120, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                   switch (value) {
                       case "0": return "常溫與低溫";
                           break;
                       case "1": return "常溫";
                           break;
                       case "2": return "低溫";
                           break;
                   }
               }
           },
           { header: "建立時間", dataIndex: 'created', width: 120, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderUserReduceListStore,
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
        }
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [form, OrderUserReduceListGrid],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                OrderUserReduceListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    //OrderUserReduceListStore.load({ params: { start: 0, limit: 25 } });
});
//查詢
Query = function () {
    // OrderUserReduceListStore.removeAll();
    var select_type = Ext.getCmp('select_type');
    var search = Ext.getCmp('search_con');
   
    if (select_type.getValue() != 0 && search.getValue() == "") {
        Ext.Msg.alert("提示信息", "請輸入查詢內容!");
        return;
    }
    Ext.getCmp("OrderUserReduceListGrid").store.loadPage(1, {
        params: {
            select_type: Ext.getCmp('select_type').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            reduce_id: Ext.getCmp('reduce_id').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue(),
            group_id: Ext.getCmp('group_id').getValue(),
            type: Ext.htmlEncode(Ext.getCmp("type").getValue().stype)
        }
    });

}
//編輯
function onUserEditClick() {
    var row = Ext.getCmp("OrderUserReduceListGrid").getSelectionModel().getSelection();
    edit_UserStore.load({
        params: { relation_id: row[0].data.user_id }
    });
    var secret_type = "1";//參數表中的"會員查詢列表"
    var url = "/Member/UsersListIndex ";
    var ralated_id = row[0].data.user_id;
    var info_id = row[0].data.user_id;
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    secret_info = "user_id;";
    if (boolPassword != "-1") {
        if (boolPassword) {//驗證
            SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
        } else {
            editFunction(ralated_id);
        }
    }
   
    //editFunction(row[0], OrderUserReduceListStore);//user_id
   // editFunction(row[0].data.user_id);
}
function TransToOrder(orderId) {

    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#OrderUserReduceList');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'OrderUserReduceList',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}