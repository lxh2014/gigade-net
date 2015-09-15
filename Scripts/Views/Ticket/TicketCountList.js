var pageSize = 25;
Ext.define('gigade.TicketCount', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_name_simple", type: "string" },
        { name: "course_id", type: "int" },
        { name: "course_name", type: "string" },
        { name: "spec_name_1", type: "string" },
        { name: "spec_name_2", type: "string" },
        { name: "start_date", type: "string" },
        { name: "end_date", type: "string" },
        { name: "sales_number", type: "string" },
        { name: "used_number", type: "string" },
    { name: "ticket_detail_id", type: "int" }//用於跳轉
    ]
});
var TicketCountStore = Ext.create('Ext.data.Store', {
    model: 'gigade.TicketCount',
    autoDestroy: true,
    pageSize: pageSize,
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



TicketCountStore.on('beforeload', function () {
    Ext.apply(TicketCountStore.proxy.extraParams, {
        select_vendor: Ext.getCmp('select_vendor').getValue(),
        select_content: Ext.getCmp('select_content').getValue(),
        start_time: Ext.getCmp('start_time').getValue(),
        end_time: Ext.getCmp('end_time').getValue()
    });
})
function Query() {
    if (Ext.getCmp('select_vendor').getValue() != "" || Ext.getCmp('select_content').getValue() != "" || Ext.getCmp('start_time').getValue() != null || Ext.getCmp('end_time').getValue() != null) {
        TicketCountStore.removeAll();
        TicketCountStore.loadPage(1);
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }

}
function TranToDetail(tdi) {
    var url = '/Ticket/TicketDetail?ticket_detail_id=' + tdi;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#ticket_detail');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'ticket_detail',
        title: '課程訂購單明細',
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
        height: 120,
        border: 0,
        defaults: { anchor: '95%', msgTarget: "side" },
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: "供應商簡稱",
                        labelWidth: 90,
                        id: 'select_vendor',
                        name: 'select_vendor',
                        margin: '5 5 0 10',
                        listeners: {

                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                       {

                           xtype: 'textfield',
                           fieldLabel: '課程編號/名稱',
                           labelWidth: 90,
                           id: 'select_content',
                           name: 'select_content',
                           margin: '5 5 0 10',
                           listeners: {
                               specialkey: function (field, e) {
                                   if (e.getKey() == e.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       }

                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                     {
                         xtype: "datetimefield",
                         margin: '5 0 0 10',
                         fieldLabel: "課程日期",
                         labelWidth: 80,
                         id: 'start_time',
                         name: 'start_time',
                         format: 'Y-m-d H:i:s',
                         editable: false,
                         time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                         listeners: {
                             select: function (a, b, c) {
                                 var start = Ext.getCmp("start_time");
                                 var end = Ext.getCmp("end_time");
                                 if (end.getValue() == null) {
                                     end.setValue(setNextMonth(start.getValue(), 1));
                                 }
                                 else if (end.getValue() < start.getValue()) {
                                     Ext.Msg.alert(INFORMATION, DATA_TIP);
                                     start.setValue(setNextMonth(end.getValue(), -1));
                                 }
                                 //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
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
                         id: 'end_time',
                         name: 'end_time',
                         format: 'Y-m-d H:i:s',
                         editable: false,
                         time: { hour: 23, min: 59, sec: 59 },//開始時間00：00：00
                         listeners: {
                             select: function (a, b, c) {
                                 var start = Ext.getCmp("start_time");
                                 var end = Ext.getCmp("end_time");
                                 if (start.getValue() != "" && start.getValue() != null) {
                                     if (end.getValue() < start.getValue()) {
                                         Ext.Msg.alert(INFORMATION, DATA_TIP);
                                         end.setValue(setNextMonth(start.getValue(), 1));
                                     }
                                     //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
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
                     }
                ]
            }
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

                        { header: "供應商", dataIndex: 'vendor_name_simple', flex: 1, align: 'center', id: 'controlvendor' },
                        {
                            header: "課程編號", dataIndex: 'course_id', width: 100, align: 'center', id: 'courseid'
                            , renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return "<a href='javascript:void(0);' onclick='TranToDetail(" + record.data.ticket_detail_id + ")'>" + value + "</a>";
                            }
                        },
                        {
                            header: "課程名稱", dataIndex: 'course_name', flex: 1, align: 'center', id: 'coursename'
                        },
                        {
                            header: "規格1", dataIndex: 'spec_name_1', width: 150, align: 'center', id: 'specid1'
                        },
                          {
                              header: "規格2", dataIndex: 'spec_name_2', width: 150, align: 'center', id: 'specid2'
                          },
                        {
                            header: "課程開始時間", dataIndex: 'start_date', width: 135, align: 'center', id: 'startdate'
                        },
                        {
                            header: "課程結束時間", dataIndex: 'end_date', width: 135, align: 'center', id: 'enddate'
                        },
                        {
                            header: "已購買人數", dataIndex: 'sales_number', width: 90, align: 'center', id: 'buycount'
                        },
                        {
                            header: "已核銷人數", dataIndex: 'used_number', width: 90, align: 'center', id: 'donecount'
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
        }
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
