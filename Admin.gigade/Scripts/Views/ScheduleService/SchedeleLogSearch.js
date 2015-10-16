/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/10/12
 * 排程日誌查詢
 */


var pageSize = 25;


//// schedule_log 的model
Ext.define('GIGADE.Log', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rowid', type: 'int' },
        { name: 'schedule_code', type: 'string' },
        { name: "create_username", type: "string" },
        { name: "create_time", type: "int" },
        {name:"show_create_time",type:"string"},
         { name: "ipfrom", type: 'string' },
        //  { name: "start_time", type: "int" },//開始時間
        //{ name: "end_time", type: "int" },//結束時間
    ]
});

var Schedule_Log_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Log',
    pageSize: pageSize,
   // autoLoad: true,//自動加載
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/ScheduleService/GetScheduleLogList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define('GIGADE.ScheduleCode', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "schedule_code", type: "string" },
    ],
});

var Schedule_Code_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.ScheduleCode',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ScheduleService/GetScheduleMasterList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//加載的時候得到數據 (排程日誌查詢) 
Schedule_Log_Store.on('beforeload', function () {
    Ext.apply(Schedule_Log_Store.proxy.extraParams,
        {
            schedule_code: Ext.getCmp("schedule_code_config").getValue(),
            start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s')),
            end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s')),
        });
});

//列表頁加載
Ext.onReady(function () {
    var Schedule_log_grid = Ext.create('Ext.grid.Panel', {
        id: 'Schedule_log_grid',
        store: Schedule_Log_Store,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
            new Ext.grid.RowNumberer(),//自動顯示行號
         { header: "編號", dataIndex: "rowid", width: 80, align: 'center' },
        { header: "排程名稱", dataIndex: "schedule_code", width: 120, align: 'center' },
        { header: "創建人", dataIndex: "create_username", width: 100, align: 'center' },
        { header: "創建時間", dataIndex: "show_create_time", width: 150, align: 'center' },
        { header: "ip來源", dataIndex: "ipfrom", width: 150, align: 'center' },
        ],
        tbar: [
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: '排程Code',
                id: 'schedule_code_config',
                name: 'schedule_code_config',
                allowBlank: false,
                displayField: 'schedule_code',
                valueField: 'schedule_code',
                store: Schedule_Code_Store,
                //value: Ext.getCmp("schedule_code").getValue()
            },
               {
                   xtype: 'datefield',
                   margin: '0 0 0 10',
                   fieldLabel: '時間區間',
                   labelWidth: 60,
                   id: 'start_time',
                   format: 'Y-m-d',
                   width: 170,
                   value: Tomorrow(1 - new Date().getDate()),
                   editable: false,
                   listeners: {
                       select: function (a, b, c) {
                           var start = Ext.getCmp("start_time");
                           var end = Ext.getCmp("end_time");
                           var s_date = new Date(start.getValue());
                           end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                       },
                       specialkey: function (field, e) {
                           if (e.getKey() == Ext.EventObject.ENTER) {
                               Query();
                           }
                       }
                   }
               },
                {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '~'
                },
                {
                    xtype: 'datefield',
                    id: 'end_time',
                    format: 'Y-m-d',
                    width: 110,
                    value: Tomorrow(0),
                    editable: false,
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time");
                            var end = Ext.getCmp("end_time");
                            var s_date = new Date(start.getValue());

                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        }
                    }
                },
            {
               text: '查詢',
               margin: '0 10 0 10',
               iconCls: 'icon-search',
               hideen:true,
               handler: function () {
                   Query();
               }
           },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: Schedule_Log_Store,
            pageSize: pageSize,
            displayInfo: true,//是否顯示數據信息
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
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [Schedule_log_grid],
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                Schedule_log_grid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})

/*************************************************************************************查询信息*************************************************************************************************/

function Query(x) {
    Ext.getCmp('Schedule_log_grid').store.loadPage(1, {
        params: {

        }
    });
}

/******************************************************************************************************************************************************************************************/
function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}
