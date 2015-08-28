var pageSize = 25;
Ext.define('gigade.TicketCount', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_name_simple", type: "string" },
        { name: "course_id", type: "int" },
        { name: "course_detail_name", type: "string" },
        { name: "spec_id_1", type: "string" },
         { name: "spec_id_2", type: "string" },
         { name: "start_date", type: "string" },
         { name: "end_date", type: "string" },
         { name: "buycount", type: "string" },
          { name: "donecount", type: "string" }
    ]
});
var TicketCountStore = Ext.create('Ext.data.Store', {
    model: 'gigade.TicketCount',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Ticket/GetCourseCountList',
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
        { "txt": "所有日期", "value": "0" },
        { "txt": "日期區間", "value": "1" }
    ]
});
var courseStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
          { "txt": "請選擇...", "value": "0" },
        { "txt": "課程名稱", "value": "1" },
        { "txt": "課程編號", "value": "2" }
    ]
});
Ext.define('gigade.vendor', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_id", type: "string" },
        { name: "vendor_name_simple", type: "string" }
    ]
});
var vendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.vendor',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Ticket/GetVendor',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

Ext.define('gigade.ticket', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ticket_id", type: "string" },
        { name: "ticket_simple", type: "string" }
    ]
});
var ticketStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ticket',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        //url: '/NewPromo/GetNewPromoCarnetList',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("TicketCount").getSelectionModel().getSelection();
        }
    }
});
TicketCountStore.on('beforeload', function () {
    Ext.apply(TicketCountStore.proxy.extraParams, {
        select_vendor: Ext.getCmp('select_vendor').getValue(),
        //   select_ticket: Ext.getCmp('select_ticket').getValue(),
        select_ticket_con: Ext.getCmp('select_ticket_con').getValue(),
        date: Ext.getCmp('date').getValue(),
        start_time: Ext.getCmp('start_time').getValue(),
        end_time: Ext.getCmp('end_time').getValue()
    });
})
function Query(x) {
    TicketCountStore.removeAll();
    Ext.getCmp("TicketCount").store.loadPage(1, {
        params: {
            select_vendor: Ext.getCmp('select_vendor').getValue(),
            //     select_ticket: Ext.getCmp('select_ticket').getValue(),
            select_ticket_con: Ext.getCmp('select_ticket_con').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue()
        }
    })
}
function TranToSetGift(eventId) {
    var url = '/Ticket/TicketMaster?course_id=' + eventId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#ticket_master');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'ticket_master',
        title: '課程訂購單',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
Ext.onReady(function () {

    var TicketForm = Ext.create('Ext.form.Panel', {
        id: 'TicketForm',
        name: 'form',
        height: 150,
        border: 0,
        defaults: { anchor: '95%', msgTarget: "side" },
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: "供應商",
                        allowBlank: true,
                        id: 'select_vendor',
                        name: 'select_vendor',
                        margin: '5 5 0 10',
                        store: vendorStore,
                        displayField: 'vendor_name_simple',
                        valueField: 'vendor_id',
                        emptyText: '所有廠商名稱'
                        //listeners: {
                        //    specialkey: function (field, e) {
                        //        if (e.getKey() == Ext.EventObject.ENTER) {
                        //            alert("enter");
                        //        }
                        //    }
                        //}
                    },
                       {
                           //xtype: 'combobox',
                           //fieldLabel: "查詢條件",
                           //allowBlank: true,
                           //id: 'select_ticket',
                           //name: 'select_ticket',
                           //margin: '5 5 0 5',
                           //store: courseStore,
                           //editable:false,
                           //displayField: 'txt',
                           //valueField: 'value',
                           //emptyText:'請選擇...',
                           //value:'0',
                           xtype: 'textfield',
                           fieldLabel: '課程名稱/課程編號',
                           id: 'select_ticket_con',
                           name: 'select_ticket_con',
                           labelWidth: 152,
                           margin: '5 5 0 10',
                       },
                //{
                //    xtype: 'textfield',
                //    id: 'select_ticket_con',
                //    name: 'select_ticket_con',
                //    margin: '5 5 0 10',
                //}
                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                     {
                         xtype: 'combobox',
                         id: 'date',
                         name: 'date',
                         fieldLabel: "課程日期",
                         margin: '5 5 0 10',
                         store: dateStore,
                         displayField: 'txt',
                         valueField: 'value',
                         value: 0
                     },
                     {
                         xtype: "datetimefield",
                         margin: '5 0 0 10',
                         id: 'start_time',
                         name: 'start_time',
                         format: 'Y-m-d H:i:s',
                         allowBlank: false,
                         value: Tomorrow(),
                         listeners: {
                             select: function (a, b, c) {
                                 var start = Ext.getCmp("start_time");
                                 var end = Ext.getCmp("end_time");
                                 var s_date = new Date(start.getValue());
                                 end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
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
                         id: 'end_time',
                         name: 'end_time',
                         format: 'Y-m-d H:i:s',
                         allowBlank: false,
                         value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                         listeners: {
                             select: function (a, b, c) {
                                 var start = Ext.getCmp("start_time");
                                 var end = Ext.getCmp("end_time");
                                 var s_date = new Date(start.getValue());
                                 if (end.getValue() < start.getValue()) {
                                     Ext.Msg.alert(INFORMATION, "上架時間不能大於下架時間！");
                                     end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                 }
                             }
                         }
                     }
                ]
            },

        ],
        buttonAlign: 'left',
        buttons: [
            {
                text: '查詢',
                margin: '5 5 0 10',
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
    var TicketCount = Ext.create('Ext.grid.Panel', {
        id: 'TicketCount',
        store: TicketCountStore,
        flex: '8.5',
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
                        { header: "編號", xtype: 'rownumberer', width: 70, align: 'center', id: 'row_id' },
                        { header: "供應商", dataIndex: 'vendor_name_simple', width: 170, align: 'center', id: 'controlvendor', hidden: true },
                        {
                            header: "課程編號", dataIndex: 'course_id', width: 100, align: 'center', id: 'courseid', hidden: true
                            , renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value > 0) {
                                    return "<a href='javascript:void(0);' onclick='TranToSetGift(\"" + record.data.course_id + "\")'>" + value + "</a>";//<span style='color:Red;'></span>
                                }
                            }
                        },
                        {
                            header: "課程名稱", dataIndex: 'course_detail_name', width: 250, align: 'center', id: 'coursename', hidden: true
                        },
                        {
                            header: "規格1", dataIndex: 'spec_id_1', width: 150, align: 'center', id: 'specid1', hidden: true
                        },
                          {
                              header: "規格2", dataIndex: 'spec_id_2', width: 150, align: 'center', id: 'specid2', hidden: true
                          },
                        {
                            header: "課程開始時間", dataIndex: 'start_date', width: 135, align: 'center', id: 'startdate', hidden: true
                        },
                        {
                            header: "課程結束時間", dataIndex: 'end_date', width: 135, align: 'center', id: 'enddate', hidden: true
                        },
                        {
                            header: "已購買人數", dataIndex: 'buycount', width: 90, align: 'center', id: 'buycount', hidden: true
                        },
                        {
                            header: "已核銷人數", dataIndex: 'donecount', width: 90, align: 'center', id: 'donecount', hidden: true
                        }

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TicketCountStore,
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
        // selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [TicketForm, TicketCount],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    TicketCountStore.load({ params: { start: 0, limit: 25 } });
    function Tomorrow() {
        var d;
        var dt;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate();
        dt = new Date(s);
        dt.setDate(dt.getDate() + 1);
        return dt;                                 // 返回日期。
    }


});
