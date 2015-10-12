/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/8/24
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
         { name: "ipfrom", type: 'string' },
    ]
});

var Schedule_Log_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Log',
    proxy: {
        type: 'ajax',

        // url: '/ScheduleService/GetScheduleLogList',

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
            //schedule_code: Ext.getCmp("schedule_code").getValue(),
        });
});


var schedule_log_search = Ext.create('Ext.grid.Panel', {
    id: 'schedule_log_search',
    store: Schedule_Log_Store,
    columnLines: true,
    autoScroll: true,
    border: 0,
    columns: [
         //new Ext.grid.RowNumberer(),//自動顯示行號
       

    ],
    listeners: {
        scrollershow: function (scroller) {
            if (scroller && scroller.scrollEl) {
                scroller.clearManagedListeners();
                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
            }
        }
    }
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
        { header: "排程名稱", dataIndex: "schedule_code", width: 80, align: 'center' },
        { header: "創建人", dataIndex: "create_username", width: 100, align: 'center' },
        { header: "創建時間", dataIndex: "create_time", width: 60, align: 'center' },
        { header: "ip來源", dataIndex: "ipfrom", width: 60, align: 'center' },
        ],
        tbar: [
           //{
           //    text: '查詢',
           //    margin: '0 10 0 10',
           //    iconCls: 'icon-search',
           //    hideen:true,
           //    handler: function () {
           //        Query();
           //    }
           //},
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
        //selModel: sm
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [Schedule_log_grid],
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                EdmTemplateGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})