﻿var pageSize = 25;


//model
Ext.define('gigade.EpaperLogModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "log_id", type: "int" },
        { name: "epaper_id", type: "int" },
          { name: "user_id", type: "int" },
          { name: "user_name", type: "string" },
        { name: "log_description", type: "string" },
        { name: "log_ipfrom", type: "string" },
        { name: "log_createdate", type: "int" },
        { name: "LogCreateDate", type: "string" }
    ]
});

var EpaperLogtStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.EpaperLogModel',
    proxy: {
        type: 'ajax',
        url: '/EpaperContent/GetEpaperLogList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});



EpaperLogtStore.on('beforeload', function () {
    Ext.apply(EpaperLogtStore.proxy.extraParams,
        {
            epaperId: document.getElementById("epaperId").value
        });
});
//頁面載入
Ext.onReady(function () {
    var gdEpaperLog = Ext.create('Ext.grid.Panel', {
        id: 'gdEpaperLog',
        store: EpaperLogtStore,
        width: document.documentElement.clientWidth,
        columns: [
             { header: "編號", dataIndex: 'log_id', width: 60, align: 'center' },
             { header: '活動編號', dataIndex: 'epaper_id', width: 100, align: 'center' },
             { header: '使用者編號', dataIndex: 'user_id', width: 100, align: 'center' },
             { header: "使用者姓名", dataIndex: 'user_name', width: 100, align: 'center' },
             { header: "說明", dataIndex: 'log_description', width: 300, align: 'center' },
            { header: "來源", dataIndex: 'log_ipfrom', width: 100, align: 'center' },
            {
                header: '記錄時間', dataIndex: 'LogCreateDate', width: 150, align: 'center'
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EpaperLogtStore,
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
        items: [gdEpaperLog],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdEpaperLog.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    QueryToolAuthorityByUrl('/EpaperContent/EpaperLogList');
    EpaperLogtStore.load({ params: { start: 0, limit: 25 } });

});




