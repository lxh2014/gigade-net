var pageSize = 27;
Ext.define('gigade.iwmsrecord', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_id", type: "int" },
        { name: "item_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "act_pick_qty", type: "int" },
        { name: "made_dt", type: "string" },
        { name: "cde_dt", type: "string" },
        { name: "cde_dt_incr", type: "int" },
        { name: "user_username", type: "string" },
        { name: "create_date", type: "string" },
        { name: "cde_dt_shp", type: "int" },
        { name: "product_spec_name", type: "string" },
        { name: "product_id", type: "int" },
    ]
});
var iwmsrecordStore = Ext.create('Ext.data.Store', {
    model: 'gigade.iwmsrecord',
    autoDestroy: true,
    autoLoad: false,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIwmsRecord',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "製造日期", "value": "made_dt" },
        { "txt": "有效日期", "value": "cde_dt" },
        { "txt": "理貨日期", "value": "create_date" }
    ]
});


Ext.define('GIGADE.Groupuser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'user_id', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
})
var GroupUsersStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Groupuser',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetGroupUsers',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
})
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
Ext.onReady(function () {
    var gdiwmsrecord = Ext.create('Ext.grid.Panel', {
        id: 'gdiwmsrecord',
        flex: 1.8,
        store: iwmsrecordStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [{ header: "訂單編號", dataIndex: 'order_id', width: 100, align: 'center' },
            { header: "商品編號", dataIndex: 'product_id', width: 70, align: 'center' },
            { header: "細項編號", dataIndex: 'item_id', width: 70, align: 'center' },
              { header: "商品名稱", dataIndex: 'product_name', width: 250, align: 'center' },
               { header: "商品規格", dataIndex: 'product_spec_name', width: 250, align: 'center' },
              
                { header: "檢貨數量", dataIndex: 'act_pick_qty', width: 70, align: 'center' },
                    {
                        header: "製造日期", dataIndex: 'made_dt', width: 150, align: 'center',
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value.length > 10) {
                                return value.substr(0, 10);
                            } else { return value; }
                        }
                    },
                    {
                        header: "有效日期", dataIndex: 'cde_dt', width: 150, align: 'center',
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value.length > 10) {
                                return value.substr(0, 10);
                            } else { return value; }
                        }
                    },
                       {
                           header: "理貨日期", dataIndex: 'create_date', width: 150, align: 'center',
                           renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                               if (value == '0001-01-01 00:00:00') {
                                   return "null";
                               } else { return value; }
                           }
                       },
                    { header: "理貨員", dataIndex: 'user_username', width: 70, align: 'center' },
                    { header: "保存期限", dataIndex: 'cde_dt_incr', width: 70, align: 'center' },
                    { header: "允出天數", dataIndex: 'cde_dt_shp', width: 70, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: iwmsrecordStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY,
            handler: Query
        })
    });
    iwmsrecordStore.on('beforeload', function () {
        Ext.apply(iwmsrecordStore.proxy.extraParams, {
            oid: Ext.getCmp('oid').getValue(),
            productname: Ext.getCmp('productname').getValue(),
            time_start: Ext.getCmp('start_time').getValue(),
            time_end: Ext.getCmp('end_time').getValue(),
            dateSel: Ext.getCmp('dateSel').getValue(),
            username: Ext.getCmp('username').getValue()
        });
    });
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        border: 0,
        bodyPadding: 13,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    margin: '0 0 18 0',
                    items: [
                        {
                            xtype: 'textfield',
                            allowBlank: true,
                            fieldLabel: "商品編號",
                            margin: '0 0 0 0',
                            labelWidth: 100,
                            width: 230,
                            id: 'oid',
                            name: 'searchcontentid',
                            regex: /^\d+$/,
                            regexText: '请输入數字',
                            listeners: {
                                specialkey: function (field, e) {
                                    if (e.getKey() == Ext.EventObject.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        },
                            {
                                xtype: 'textfield',
                                allowBlank: true,
                                fieldLabel: "商品名稱",
                                margin: '0 0 0 10',
                                labelWidth: 70,
                                width: 200,
                                id: 'productname',
                                name: 'searchcontentname',
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
                                fieldLabel: "理貨員",
                                id: 'username',
                                neme: 'searchcontentusername',
                                store: GroupUsersStore,
                                displayField: 'user_username',
                                valueField: 'user_username',
                                queryMode: 'local',
                                margin: '0 0 0 10',
                                typeAhead: true,
                                allowBlank: true,
                                listeners: {
                                    specialkey: function (field, e) {
                                        if (e.getKey() == Ext.EventObject.ENTER) {
                                            Query();
                                        }
                                    }
                                }
                            }
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    margin: '0 0 0 0',
                    items: [
                        {
                            xtype: 'combobox',
                            //editable: false,
                            fieldLabel: "日期類型",
                            labelWidth: 100,
                            width: 190,
                            id: 'dateSel',
                            name: 'searchcontentdate',
                            store: dateStore,
                            displayField: 'txt',
                            valueField: 'value',
                            emptyText: '請選擇...',
                            value: 0
                        },
                       {
                           xtype: "datefield",
                           editable: false,
                           margin: '0 0 0 5',
                           id: 'start_time',
                           name: 'start_time',
                           format: 'Y/m/d',
                           listeners: {
                               select: function () {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   var s_date = new Date(start.getValue());
                                   if (end.getValue() == null) {
                                       end.setValue(setNextMonth(start.getValue(), 1));
                                   } else if (start.getValue() > end.getValue()) {
                                       Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                       end.setValue(setNextMonth(start.getValue(), 1));
                                   }
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       },
                       { xtype: 'displayfield', value: '~ ' },
                       {
                           xtype: "datefield",
                           editable: false,
                           id: 'end_time',
                           name: 'end_time',
                           format: 'Y/m/d',
                           listeners: {
                               select: function () {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   var s_date = new Date(end.getValue());
                                   var now_date = new Date(end.getValue());
                                   if (start.getValue() != "" && start.getValue() != null) {
                                       if (end.getValue() < start.getValue()) {
                                           Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                           end.setValue(setNextMonth(start.getValue(), 1));
                                       }
                                   } else {
                                       start.setValue(setNextMonth(end.getValue(), -1));
                                   }

                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       }
                    ]
                },

                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,//如果设置为 true, 则 field 容器自动将其包含的所有属性域的校验错误组合为单个错误信息, 并显示到 配置的 msgTarget 上. 默认值 false.
                    layout: 'hbox',
                    margin: '10 0 0 0',
                    items:
                     [

                        {
                            xtype: 'button',
                            text: SEARCH,
                            iconCls: 'icon-search',
                            id: 'btnQuery',
                            handler: Query
                        },
                         {
                             xtype: 'button',
                             text: RESET,
                             margin: '0 0 0 10',
                             id: 'btn_reset1',
                             iconCls: 'ui-icon ui-icon-reset',
                             listeners: {
                                 click: function () {
                                     Ext.getCmp("oid").setValue("");
                                     Ext.getCmp("productname").setValue("");
                                     Ext.getCmp("start_time").setValue("");
                                     Ext.getCmp("end_time").setValue("");
                                     Ext.getCmp("dateSel").setValue(0);
                                     Ext.getCmp('username').setValue("");
                                 }
                             }
                         }
                     ]
                }
        ]
    });
    function Query() {
        iwmsrecordStore.removeAll();
        var falg = true;
        if (!Ext.getCmp('start_time').getValue() == '') {
            if (Ext.getCmp('dateSel').getValue() == 0) {
                Ext.Msg.alert("提示", "請選擇日期類型");
                return false;
            }
            else { falg = false; }
        }
        var oid = Ext.getCmp('oid');
        var productname = Ext.getCmp('productname');
        var usernames = Ext.getCmp('username');
        if (oid.getValue().trim() !="") { falg = false; }
        if (productname.getValue().trim() != "") {falg = false;}
        if (usernames.getValue() != undefined)
        {
            var ab = usernames.getValue();
            if (ab.trim() != "") {
                falg = false;
            }
        }
        
        if (falg) {
            Ext.Msg.alert("提示", "請輸入查詢條件");
            return false;
        }
        var oid = Ext.getCmp('oid').getValue();
        var productname = Ext.getCmp('productname').getValue();
        iwmsrecordStore.removeAll();
        Ext.getCmp("gdiwmsrecord").store.loadPage(1,
            {
                params: {
                    oid: oid,
                    productname: productname,
                    time_start: Ext.getCmp('start_time').getValue(),
                    time_end: Ext.getCmp('end_time').getValue(),
                    dateSel: Ext.getCmp('dateSel').getValue(),
                    username: Ext.getCmp('username').getValue()
                }
            });
    };
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm,gdiwmsrecord],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                gdiwmsrecord.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});