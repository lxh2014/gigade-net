var pageSize = 25;
Ext.define('gigade.Aseld', {
    extend: 'Ext.data.Model',
    fields: [
     { name: "row_id", type: "int" },
     { name: "product_id", type: "string" },
     { name: "product_name", type: "string" },
     { name: "item_id", type: "string" },
     { name: "spec", type: "string" },
     { name: "out_qty", type: "string" },
     { name: "act_pick_qty", type: "string" },
     { name: "create_dtim", type: "string" },
     { name: "ord_qty", type: "string" },
     { name: "loc_id", type: "string" }, 
     { name: "assg_id", type: "string" }, 
     { name: "upc_id", type: "string" }
    ]
});
var AseldStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Aseld',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/WareHouse/AseldList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'

        }
    },
    listeners: {
        load: function (store) {
            var count = store.getCount();
            if (count == 0) {
                Ext.getCmp('outExcel').setDisabled(true);
            }
            else {
                Ext.getCmp('outExcel').setDisabled(false);
            }
        }
    }
});
function Query() {
    var falg = 0;
    var assg_id = Ext.getCmp('assg_id').getValue(); if (assg_id.trim() != '') { falg++; }
    var start_time = Ext.getCmp('time_start').getValue();
    var end_time = Ext.getCmp('time_end').getValue();
    if (start_time != null && end_time != null) {
        falg++;
    }
    if (start_time == null && end_time != null) {
        Ext.Msg.alert("提示", "請把創建時間補充完整");
        return false;
    }
    if (start_time != null && end_time == null) {
        Ext.Msg.alert("提示", "請把創建時間補充完整");
        return false;
    }
    if (falg == 0) {
        Ext.Msg.alert("提示", "請輸入查詢條件");
        AseldStore.removeAll();
        return false;
    }
    var form = Ext.getCmp('frm').getForm();
    if (form.isValid()) {
        AseldStore.removeAll();
        Ext.getCmp("gdAccum").store.loadPage(1,
            {
                params: {
                    assg_id: assg_id,
                    start_time: start_time,
                    end_time: end_time
                }
            });
    }
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',//anchor固定
        height: 80,
        border: 0,
        bodyPadding: 5,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: "工作代號",
                        labelWidth: 55,
                        name: 'assg_id',
                        id: 'assg_id',
                        margin: '5 0 0 5',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },

                     {
                         xtype: 'datefield',
                         fieldLabel: "創建時間",
                         labelWidth: 60,
                         // width: 210,
                         id: 'time_start',
                         name: 'time_start',
                         margin: '5 0 0 5',
                         format: 'Y-m-d',
                         editable: false,
                         listeners: {
                             select: function () {
                                 if (Ext.getCmp("time_start").getValue() != null) {
                                     Ext.getCmp("time_end").setMinValue(Ext.getCmp("time_start").getValue());
                                 }
                             }
                              , specialkey: function (field, e) {
                                  if (e.getKey() == Ext.EventObject.ENTER) {
                                      Query();
                                  }
                              }

                         }
                     },
                        {
                            xtype: 'label',
                            forId: 'myFieldId',
                            text: '~',
                            margin: '5 0 0 5'
                        },
                        {
                            xtype: 'datefield',
                            labelWidth: 60,
                            // width: 210,
                            id: 'time_end',
                            name: 'time_end',
                            margin: '5 0 0 5',
                            format: 'Y-m-d',
                            editable: false,
                            listeners: {
                                select: function () {
                                    if (Ext.getCmp("time_end").getValue() != null) {
                                        Ext.getCmp("time_start").setMaxValue(Ext.getCmp("time_end").getValue());
                                    }
                                }
                                , specialkey: function (field, e) {
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
                    margin: '5 0 0 0',
                    items:
                     [
                        {
                            xtype: 'button',
                            margin: '0 10 0 10',
                            iconCls: 'icon-search',
                            text: "查詢",
                            handler: Query
                        },
                        {
                            xtype: 'button',
                            text: '重置',
                            id: 'btn_reset',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners:
                            {
                                click: function () {
                                    frm.getForm().reset();
                                    var datetime1 = new Date();
                                    datetime1.setFullYear(2000, 1, 1);
                                    var datetime2 = new Date();
                                    datetime2.setFullYear(2100, 1, 1);
                                    Ext.getCmp("time_start").setMinValue(datetime1);
                                    Ext.getCmp("time_start").setMaxValue(datetime2);
                                    Ext.getCmp("time_end").setMinValue(datetime1);
                                    Ext.getCmp("time_end").setMaxValue(datetime2);
                                }
                            }
                        }
                     ]
                }
        ]

    });
    AseldStore.on('beforeload', function () {
        var start_time = Ext.getCmp('time_start').getValue();
        var end_time = Ext.getCmp('time_end').getValue();
        if (start_time == null && end_time != null) {
            Ext.Msg.alert("提示", "請把創建時間補充完整");
            return false;
        }
        if (start_time != null && end_time == null) {
            Ext.Msg.alert("提示", "請把創建時間補充完整");
            return false;
        }
        Ext.apply(AseldStore.proxy.extraParams, {
            assg_id: Ext.getCmp('assg_id').getValue(),
            start_time: Ext.getCmp('time_start').getValue(),
            end_time: Ext.getCmp('time_end').getValue()
        });
    });
    var gdAccum = Ext.create('Ext.grid.Panel', {
        id: 'gdAccum',
        flex: 1.8,
        store: AseldStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [
            { header: "工作代號", dataIndex: 'assg_id', width: 120, align: 'center' },
            { header: "商品編號", dataIndex: 'product_id', width: 60, align: 'center' },
            { header: "條碼", dataIndex: 'upc_id', width: 120, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 300, align: 'center' },
            { header: "細項編號", dataIndex: 'item_id', width: 85, align: 'center' },
            { header: "料位編號", dataIndex: 'loc_id', width: 85, align: 'center' },
            { header: "規格", dataIndex: 'spec', width: 170, align: 'center' },
            { header: "訂货量", dataIndex: 'ord_qty', width: 60, align: 'center' },
            { header: "已撿貨量 ", dataIndex: 'act_pick_qty', width: 65, align: 'center' },
            { header: "待检货量", dataIndex: 'out_qty', width: 65, align: 'center' },
            { header: "創建時間", dataIndex: 'create_dtim', width: 120, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: "總量撿貨報表匯出PDF", id: 'outExcel', icon: '../../../Content/img/icons/excel.gif', handler: outExcel }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: AseldStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdAccum],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                gdAccum.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});
outExcel = function () {
    //var falg = 0;
    //var assg_id = Ext.getCmp('assg_id').getValue(); if (assg_id.trim() != '') { falg++; }
    //var start_time = Ext.getCmp('time_start').getValue();
    //var end_time = Ext.getCmp('time_end').getValue();
    //if (start_time != null && end_time != null) {
    //    falg++;
    //}
    //if (falg == 0) {
    //    Ext.Msg.alert("提示", "請輸入查詢條件");
    //    return false;
    // }
    var assg_id = Ext.getCmp('assg_id').getValue();
    var start_time = Ext.getCmp('time_start').getValue();
    var end_time = Ext.getCmp('time_end').getValue();
    if (start_time == null && end_time != null) {
        Ext.Msg.alert("提示", "請把創建時間補充完整");
        return false;
    }
    if (start_time != null && end_time == null) {
        Ext.Msg.alert("提示", "請把創建時間補充完整");
        return false;
    }
    if (start_time != null) {
        start_time = Ext.htmlEncode(Ext.Date.format(new Date(start_time), 'Y-m-d 00:00:00'));
    }
    if (end_time != null) {
        end_time = Ext.htmlEncode(Ext.Date.format(new Date(end_time), 'Y-m-d 00:00:00'));
    }
    var params = 'assg_id=' + Ext.getCmp('assg_id').getValue() + "&start_time=" + start_time + "&end_time=" + end_time;
    window.open('/WareHouse/AseldPDFS?' + params);
}