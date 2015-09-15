
var pageSize = 20;
//供应商出货单Model
Ext.define('gigade.ArrorOrderList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_id", type: "string" },
        { name: "order_status", type: "string" },
        { name: "remark", type: "string" },
        { name: "detail_id", type: "string" },
        { name: "parent_id", type: "string" },
        { name: "pack_id", type: "string" },
        { name: "combined_mode", type: "string" },
        { name: "item_mode", type: "string" },
        { name: "cout", type: "string" },
        { name: "order_createdate", type: "string" },
        { name: "modeName", type: "string" }
    ]
});

//列表
var ArrorOrderStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ArrorOrderList',
    proxy: {
        type: 'ajax',
        url: '/Order/ArrorOrderList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});


ArrorOrderStore.on('beforeload', function () {
    Ext.apply(ArrorOrderStore.proxy.extraParams, {
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue()
    });
});

function Query() {
    if (Ext.getCmp('timestart').getValue() != null || Ext.getCmp('timeend').getValue() != null) {
        ArrorOrderStore.removeAll();
        Ext.getCmp("ArrorOrder").store.loadPage(1);
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}

Ext.onReady(function () {

    var ArrorOrder = Ext.create('Ext.grid.Panel', {
        id: 'ArrorOrder',
        store: ArrorOrderStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "訂單編號", dataIndex: 'order_id', width: 100, align: 'center' },
            //{ header: "訂單狀態", dataIndex: 'order_status', width: 120, align: 'center', hidden: true },
            { header: "訂單狀態", dataIndex: 'remark', width: 120, align: 'center' },
            { header: "細項編號", dataIndex: 'detail_id', width: 100, align: 'center' },
            { header: "父商品編號", dataIndex: 'parent_id', width: 110, align: 'center' },
            { header: "組合包編號", dataIndex: 'pack_id', width: 120, align: 'center' },
            //{ header: "組合方式", dataIndex: 'combined_mode', width: 120, align: 'center', hidden: true },
             { header: "組合方式", dataIndex: 'modeName', width: 120, align: 'center' },
            //{ header: "item_mode", dataIndex: 'item_mode', width: 120, align: 'center', hidden: true },
            { header: "數量", dataIndex: 'cout', width: 120, align: 'center' },
            { header: "時間", dataIndex: 'order_createdate', width: 120, align: 'center' }
        ],
        tbar: [
             {
                 xtype: 'button',
                 text: '匯出EXCEL',
                 iconCls: 'icon-excel',
                 id: 'btnExcel',
                 handler: function () {
                     if (Ext.getCmp('timestart').getValue() != null || Ext.getCmp('timeend').getValue() != null) {
                         var timestart = dateFormat(Ext.getCmp('timestart').getValue());
                         var timeend = dateFormat(Ext.getCmp('timeend').getValue());
                         window.open("/Order/ExportArrorOrderExcel?timestart=" + timestart + "&timeend=" + timeend);
                     }
                     else {
                         Ext.Msg.alert(INFORMATION, "請輸入查詢日期區間！");
                     }
                 }
             }, '->',
              {
                  xtype: "datetimefield",
                  margin: '5 0 0 10',
                  fieldLabel: '查詢時間',
                  labelWidth: 80,
                  id: 'timestart',
                  name: 'timestart',
                  format: 'Y-m-d H:i:s',
                  editable: false,
                  time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                  listeners: {
                      select: function (a, b, c) {
                          var start = Ext.getCmp("timestart");
                          var end = Ext.getCmp("timeend");
                          if (end.getValue() == null) {
                              end.setValue(setNextMonth(start.getValue(), 1));
                          }
                          else if (end.getValue() < start.getValue()) {
                              Ext.Msg.alert(INFORMATION, DATA_TIP);
                              start.setValue(setNextMonth(end.getValue(), -1));
                          }
                          //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {//限定區間為一月
                          //    Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                          //    end.setValue(setNextMonth(start.getValue(), 1));
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
                         margin: '5 0 0 0',
                         value: "~"
                     },
                     {

                         xtype: "datetimefield",
                         margin: '5 5 0 0',
                         id: 'timeend',
                         name: 'timeend',
                         format: 'Y-m-d H:i:s',
                         editable: false,
                         time: { hour: 23, min: 59, sec: 59 },//開始時間00：00：00
                         listeners: {
                             select: function (a, b, c) {
                                 var start = Ext.getCmp("timestart");
                                 var end = Ext.getCmp("timeend");
                                 if (start.getValue() != "" && start.getValue() != null) {
                                     if (end.getValue() < start.getValue()) {
                                         Ext.Msg.alert(INFORMATION, DATA_TIP);
                                         end.setValue(setNextMonth(start.getValue(), 1));
                                     }

                                     //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                     //    //   Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                     //    start.setValue(setNextMonth(end.getValue(), -1));
                                     //}
                                 }
                                 else {
                                     start.setValue(setNextMonth(end.getValue(), -1));
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

                       xtype: 'button',
                       text: SEARCH,
                       iconCls: 'icon-search',
                       id: 'btnQuery',
                       handler: Query
                   },
                   {
                       xtype: 'button',
                       text: RESET,
                       id: 'btn_reset',
                       listeners: {
                           click: function () {
                               Ext.getCmp("timestart").setValue("");
                               Ext.getCmp("timeend").setValue("");
                           }
                       }
                   }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ArrorOrderStore,
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
        layout: 'fit',
        items: [ArrorOrder],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ArrorOrder.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // ArrorOrderStore.load({ params: { start: 0, limit: 25 } });

    setNextMonth = function (source, n) {
        var s = new Date(source);
        s.setMonth(s.getMonth() + n);
        if (n < 0) {
            s.setHours(0, 0, 0);
        }
        else if (n > 0) {
            s.setHours(23, 59, 59);
        }
        return s;
    }
});
function dateFormat(value) {
    if (null != value) {
        return Ext.Date.format(new Date(value), 'Y-m-d H:i:s');
    } else {
        return "";
    }
}