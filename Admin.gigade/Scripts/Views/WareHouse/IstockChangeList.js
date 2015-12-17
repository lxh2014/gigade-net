var pageSize = 27;
Ext.define('gigade.istockchange', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "typename", type: "string" },
        { name: "item_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "specname", type: "string" },
        { name: "sc_trans_id", type: "string" },
        { name: "sc_cd_id", type: "string" },
        { name: "sc_num_chg", type: "int" },
        { name: "sc_num_new", type: "int" },
        { name: "sc_time", type: "string" },
        { name: "manager", type: "string" },
        { name: "sc_note", type: "string" },
        { name: "istockwhy", type: "string" }
    ]
});
var istockchangeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.istockchange',
    autoDestroy: true,
    autoLoad: false,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIstockChange',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n >= 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
Ext.onReady(function () {
    var gdistockchange = Ext.create('Ext.grid.Panel', {
        id: 'gdistockchange',
        flex: 1.8,
        store: istockchangeStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [{ header: "交易類型", dataIndex: 'typename', width: 100, align: 'center' },
            { header: "細項編號", dataIndex: 'item_id', width: 70, align: 'center' },
              { header: "商品名稱", dataIndex: 'product_name', width: 250, align: 'center' },
              { header: "商品規格", dataIndex: 'specname', width: 200, align: 'center' },
                { header: "交易單號", dataIndex: 'sc_trans_id', width: 120, align: 'center' },
                { header: "前置單號", dataIndex: 'sc_cd_id', width: 70, align: 'center' },
                 { header: "交易數量", dataIndex: 'sc_num_chg', width: 70, align: 'center' },
                    { header: "結餘數量", dataIndex: 'sc_num_new', width: 70, align: 'center' },
                    { header: "操作日期", dataIndex: 'sc_time', width: 150, align: 'center', },
                    { header: "管理員", dataIndex: 'manager', width: 70, align: 'center' },
                    { header: "帳卡原因", dataIndex: 'istockwhy', width: 70, align: 'center' },
                    { header: "備註", dataIndex: 'sc_note', width: 100, align: 'center' }
        ],
        tbar: [
          { xtype: 'button', text: "匯出Excel", id: 'ExportOut', icon: '../../../Content/img/icons/excel.gif', handler: onExportOut }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: istockchangeStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY,
            handler: Query
        })
    });
    istockchangeStore.on('beforeload', function () {
        Ext.apply(istockchangeStore.proxy.extraParams, {
            oid: Ext.getCmp('oid').getValue(),
            time_start: Ext.getCmp('start_time').getValue(),
            time_end: Ext.getCmp('end_time').getValue(),
        });
    });
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 90,
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
                            fieldLabel: "商品細項編號/條碼",
                            margin: '0 0 0 0',
                            labelWidth: 120,
                            width: 250,
                            id: 'oid',
                            name: 'searchcontentid',
                            listeners: {
                                specialkey: function (field, e) {
                                    if (e.getKey() == Ext.EventObject.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        },
                            { xtype: 'displayfield', value: '時間區間:', margin: '0 0 0 15' },
                              {
                                  xtype: "datetimefield",
                                  editable: false,
                                  margin: '0 0 0 5',
                                  id: 'start_time',
                                  name: 'start_time',
                                  format: 'Y-m-d  H:i:s',
                                  time: { hour: 00, min: 00, sec: 00 },
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
                           xtype: "datetimefield",
                           editable: false,
                           id: 'end_time',
                           name: 'end_time',
                           format: 'Y-m-d  H:i:s',
                           time: { hour: 23, min: 59, sec: 59 },
                           listeners: {
                               select: function () {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   var s_date = new Date(end.getValue());
                                   var now_date = new Date(end.getValue());
                                   if (start.getValue() != "" && start.getValue() != null) {
                                       if (end.getValue() < start.getValue()) {
                                           Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                           start.setValue(setNextMonth(end.getValue(), -1));
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
                                     Ext.getCmp("start_time").setValue("");
                                     Ext.getCmp("end_time").setValue("");
                                 }
                             }
                         }
                     ]
                }
        ]
    });
    function onExportOut() {
        var oid = Ext.getCmp('oid');
        if (oid.getValue().trim() == "") {
            Ext.Msg.alert("提示", "請輸入商品細項編號/條碼");
            return false;
        }

        var url = "oid=" + Ext.getCmp('oid').getValue() + "&start_time=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&end_time=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s');
        window.open("/WareHouse/IstockChangeExcelList?" + url);
    }
    function Query() {
        var num = 0;
        var oid = Ext.getCmp('oid');
        var start = Ext.getCmp('start_time');
        if (oid.getValue().trim() != "") {
            num++;
        }
        if (start.getValue() != null) {
            num++;
        }
        if (num == 0) {
            Ext.Msg.alert("提示", "請輸入查詢條件");
            return false;
        }
        var oid = Ext.getCmp('oid').getValue();
        istockchangeStore.removeAll();
        Ext.getCmp("gdistockchange").store.loadPage(1,
            {
                params: {
                    oid: oid,
                    time_start: Ext.getCmp('start_time').getValue(),
                    time_end: Ext.getCmp('end_time').getValue()
                }
            });
    };
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdistockchange],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                istockchangeStore.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});