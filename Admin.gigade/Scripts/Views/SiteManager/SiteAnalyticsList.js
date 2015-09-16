var pageSize = 25;
Ext.define('gigade.SiteAnalyticsList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "sa_id", type: "int" },
        { name: "sa_date", type: "string" },
        { name: "sa_session", type: "int" },
        { name: "sa_user", type: "int" },
        { name: "sa_create_time", type: "string" },
        { name: "sa_create_user", type: "int" },
        { name: "s_sa_create_user", type: "string" },
        { name: "s_sa_date", type: "string" },
        { name: "s_sa_create_time", type: "string" },
             
        { name: "sa_pageviews", type: "int" },
        { name: "sa_pages_session", type: "string" },
        { name: "sa_bounce_rate", type: "string" },
        { name: "sa_avg_session_duration", type: "string" },
        { name: "sa_modify_time_query", type: "string" },
        { name: "sa_modify_username", type: "string" },
        
        
    ]
});
var SiteAnalyticsListStore = Ext.create('Ext.data.Store', {
    model: 'gigade.SiteAnalyticsList',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/SiteManager/GetSiteAnalyticsList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

var DateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日索引", "value": "0" },
        { "txt": "單日日索引", "value": "1" },
    ]
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("SiteAnalyticsList").getSelectionModel().getSelection();
            Ext.getCmp("SiteAnalyticsList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("SiteAnalyticsList").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
SiteAnalyticsListStore.on('beforeload', function () {
    Ext.apply(SiteAnalyticsListStore.proxy.extraParams, {
        serch_sa_date: Ext.getCmp("serch_sa_date").getValue(),
        search_con: Ext.getCmp("search_con").getValue(),
    });
});
/***************************新增***********************/
onAddClick = function () {
    editFunction(null, SiteAnalyticsListStore);
}
/*********************編輯**********************/
onEditClick = function () {
    var row = Ext.getCmp("SiteAnalyticsList").getSelectionModel().getSelection();//獲取選中的行數
    if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], SiteAnalyticsListStore);
    }
}
//刪除
onDeleteClick = function () {
    var row = Ext.getCmp("SiteAnalyticsList").getSelectionModel().getSelection();
    Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
        if (btn == 'yes') {
            var rowIDs = '';
            for (var i = 0; i < row.length; i++) {
                rowIDs += row[i].data["sa_id"] + ',';
            }
            Ext.Ajax.request({
                url: '/SiteManager/DeleteSiteAnalyticsById',//執行方法
                method: 'post',
                params: { ids: rowIDs },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    if (result.success) {
                        Ext.Msg.alert(INFORMATION, "刪除成功!");
                        for (var i = 0; i < row.length; i++) {
                            SiteAnalyticsListStore.remove(row[i]);
                        }
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, "無法刪除!");
                    }
                },
                failure: function () {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            });
        }
    });
}
 Ext.onReady(function () {

     function Query() {
         SiteAnalyticsListStore.removeAll();
         Ext.getCmp("SiteAnalyticsList").store.loadPage(1, {
             params: {
                 serch_sa_date: Ext.getCmp("serch_sa_date").getValue(),
                 search_con: Ext.getCmp("search_con").getValue(),
             }
         });
     }

     outExcel = function () {
         
         var params = 'search_con=' + Ext.getCmp("search_con").getValue() + '&serch_sa_date=' + Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('serch_sa_date').getValue()), 'Y-m-d'))
         window.open('/SiteManager/ExportExcel?' + params);
     }
    var form = Ext.create('Ext.form.Panel', {
        bodyPadding: 10,
        layout: 'anchor',
        height: 100,
        margin: '0 10 0 0',
        width: 600,
        border: false,
        url: '/SiteManager/ImportExcel',
        items: [
           {
               xtype: 'fieldcontainer',
               layout: 'hbox',
               items: [
            {
                xtype: 'filefield',
                id: 'ImportExcel',
                width: 400,
                labelWidth:70,
                anchor: '100%',
                name: 'ImportExcel',
                fieldLabel: '匯入Excel',
                allowBlank: false,
                buttonText: '選擇Excel...'
            },
            {
                xtype: 'displayfield',
                margin:'0 0 0 10',
                value: '<a href="/Template/SiteAnalytics/SiteAnalytics.xlsx">範例下載</a>'
            },
               ],
           }
        ],
      
        buttonAlign: 'right',
        buttons: [
            {
                text: '匯入',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid())
                    {
                        form.submit({

                            waitMsg: '正在匯入...',
                            params: {
                                ImportExcel: Ext.htmlEncode(Ext.getCmp('ImportExcel').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert("提示信息", "匯入成功");
                                    SiteAnalyticsListStore.load();
                                }
                                else {
                                    Ext.Msg.alert("提示信息", "匯入失敗");
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert("提示信息", "匯入失敗");
                            }
                        });
                    }
                }
            }
        ]
    });

    var SiteAnalyticsList = Ext.create('Ext.grid.Panel', {
        id: 'SiteAnalyticsList',
        store: SiteAnalyticsListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.8,
        columns: [
            { header: "編號", dataIndex: 'sa_id', width: 80, align: 'center' },
            { header: "日索引", dataIndex: 's_sa_date', width: 150, align: 'center' },
            { header: "造訪數", dataIndex: 'sa_session', align: '120', align: 'center' },
            { header: "使用者", dataIndex: 'sa_user', align: '120', align: 'center' },
            { header: "創建時間", dataIndex: 's_sa_create_time', width: 165, align: 'center' },
            { header: "創建人", dataIndex: 's_sa_create_user', align: '80', align: 'center' },

            { header: "瀏覽量", dataIndex: 'sa_pageviews', width: 50, align: 'center' },
            { header: "單次造訪頁數", dataIndex: 'sa_pages_session', width: 120, align: 'center' },
            { header: "跳出率", dataIndex: 'sa_bounce_rate', align: '120', align: 'center' },
            { header: "平均停留時間", dataIndex: 'sa_avg_session_duration', width: 165, align: 'center' },
            { header: "異動時間", dataIndex: 'sa_modify_time_query', width: 120, align: 'center' },
            { header: "異動人員", dataIndex: 'sa_modify_username', width: 120, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: "新增", id: 'add', iconCls: 'icon-user-add', handler: onAddClick }, '-',
           { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }, '-',
           { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick }, '-',
            { xtype: 'button', text: "匯出Excel", id: 'outExcel', icon: '../../../Content/img/icons/excel.gif', handler: outExcel },
            '->',
            {
                xtype: 'combobox',
                fieldLabel: '查詢條件',
                displayField: 'txt',
                id:'search_con',
                labelWidth:65,
                valueField: 'value',
                editable:false,
                value: '1',
                store: DateStore,
                listeners: {
                    'select': function () {
                        if (Ext.getCmp('search_con').getValue() == 0) {
                            Ext.getCmp('serch_sa_date').setDisabled(true);
                        }
                        else {
                            Ext.getCmp('serch_sa_date').setDisabled(false);
                        }
                    }
                }
            },
            {
                xtype: 'datefield',
                format: 'Y-m-d',
                //disabled:true,
                id: 'serch_sa_date',
                name: 'serch_sa_date',
               editable: false,
                value:new Date()
            },
            {
                xtype: 'button',
                text:'查詢',
                handler: function () {
                    Query()
                }
            },
              {
                  xtype: 'button',
                  text: '重置',
                  handler: function () {
                      Ext.getCmp('search_con').setValue('0');
                      Ext.getCmp('serch_sa_date').setValue(new Date());
                      Ext.getCmp('serch_sa_date').setDisabled(true);
                  }
              },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SiteAnalyticsListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [form,SiteAnalyticsList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                SiteAnalyticsList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
   // SiteAnalyticsListStore.load({ params: { start: 0, limit: 25 } });


});