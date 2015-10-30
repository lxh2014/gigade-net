Ext.onReady(function () {

    Ext.define('gigade.EdmContentNewReport', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "trace_day", type: "string" },
        { name: "openPerson", type: "int" },
        { name: "openCount", type: "int" },
        { name: "avgPerson", type: "string" },
        { name: "avgCount", type: "string" },
        ]
    });
    EdmContentNewReportStore = Ext.create('Ext.data.Store', {
        autoLoad: 'true',
        pageSize: 25,
        model: 'gigade.EdmContentNewReport',
        proxy: {
            type: 'ajax',
            url: '/EdmNew/EdmContentNewReportList',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
    });


    Ext.define('gigade.CreatedateAndLogId', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "log_id", type: "int" },
        { name: "createdate", type: "string" },
         ]
    });
    CreatedateAndLogIdStore = Ext.create('Ext.data.Store', {
        model: 'gigade.CreatedateAndLogId',
        autoLoad:true,
        proxy: {
            type: 'ajax',
            url: '/EdmNew/CreatedateAndLogId',
            reader: {
                type: 'json',
                root: 'data',
            }
        }
    });





    CreatedateAndLogIdStore.on('beforeload', function () {
        Ext.apply(CreatedateAndLogIdStore.proxy.extraParams,
        {
            content_id: document.getElementById('content_id').value,
        });
    });

    EdmContentNewReportStore.on('beforeload', function () {
        Ext.apply(EdmContentNewReportStore.proxy.extraParams,
        {
            content_id: document.getElementById('content_id').value,
            log_id: Ext.getCmp('log_id').getValue(),
        });
    });

    var ReportForm = Ext.create('Ext.form.Panel', {
        id: 'ReportForm',
        layout: 'anchor',
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
               {
                   xtype: 'combobox',
                   store: CreatedateAndLogIdStore,
                   displayField: 'createdate',
                   valueField: 'log_id',
                   id:'log_id',
                   fieldLabel: '電子報統計報表',
                   lastQuery:'',
                   editable: false,
                   listeners: {
                       select: function () {
                           var log_id = Ext.getCmp('log_id').getValue();
                           load();
                           EdmContentNewReportStore.load();

                       }
                   }
               },
                {
                    xtype: 'displayfield',
                    value: '<span style="color:white;color:green;font-size:20px;margin-left: 200px">開　信　狀　況　統　計　摘　要</span>'
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    items: [
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">郵件主旨</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 500,
                            id: 'subject'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">發送時間</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'date'
                        },
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    items: [
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">發信成功人數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 500,
                            id: 'successCount'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">發信失敗人數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'failCount'
                        },
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    items: [
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">總開信人數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 500,
                            id: 'totalPersonCount'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">總開信次數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'totalCount'
                        },
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    items: [
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">開信率</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 500,
                            id: 'openAveragePrecent'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">平均開信次數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'openAverageCount'
                        },
                    ]
                },
        ]

    });

    var EdmListGrid = Ext.create('Ext.grid.Panel', {
        id: 'EdmListGrid',
        store: EdmContentNewReportStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
              {
                  header: "開信時間", dataIndex: 'trace_day', width: 150, align: 'center'
              },
                {
                    header: "開信人數", dataIndex: 'openPerson', width: 150, align: 'center'
                },
                  {
                      header: "開信次數", dataIndex: 'openCount', width: 150, align: 'center'
                  },
                   {
                       header: "人數比率", dataIndex: 'avgPerson', width: 80, align: 'center'
                   },
                     {
                         header: "次數比率", dataIndex: 'avgCount', width: 150, align: 'center'
                     },
        ],

        tbar: [
           {
               xtype: 'button', text: "電子報統計報表", iconCls: '', id: 'edm_list', disabled: true,
               handler: onEdm_listClick

           },
           {
               xtype: 'button', text: "發信名單統計", iconCls: '', id: 'edm_send',
               handler: onEdm_sendClick
           },
           {
               xtype: 'button', text: "開信名單下載", iconCls: 'icon-excel', id: 'open_download',
               handler: onOpen_downloadClick
           },
           {
               xtype: 'button', text: "未開信名單下載", iconCls: 'icon-excel', id: 'close_download',
               handler: onClose_downloadClick
           },

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmContentNewReportStore,
            pageSize: 25,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "共計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
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
        items: [ReportForm, EdmListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                EdmListGrid.width = document.documentElement.clientWidth;
                var log_id = document.getElementById('log_id').value;
                if (log_id != "" && log_id != 0) {
                    Ext.getCmp('log_id').setValue(log_id)
                } else {
                    CreatedateAndLogIdStore.on('load', function () {
                        Ext.getCmp('log_id').select(CreatedateAndLogIdStore.getAt(0));
                        load();
                        EdmContentNewReportStore.load();
                    });
                }
                load();
                this.doLayout();
            }
        }
    });
    //QueryAuthorityByUrl('/EdmNew/EdmContentNewReport');
});

function load() {
    var content_id = document.getElementById('content_id').value;
    Ext.Ajax.request({
        url: '/EdmNew/Load',
        params: {
            content_id: content_id,
            log_id: Ext.getCmp('log_id').getValue(),
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.getCmp('successCount').setValue(result.successCount);
                Ext.getCmp('failCount').setValue(result.failCount);
                Ext.getCmp('totalPersonCount').setValue(result.totalPersonCount);
                Ext.getCmp('totalCount').setValue(result.totalCount);
                Ext.getCmp('openAveragePrecent').setValue(result.openAveragePrecent);
                Ext.getCmp('openAverageCount').setValue(result.openAverageCount);
                Ext.getCmp('openAverageCount').setValue(result.openAverageCount);
                Ext.getCmp('subject').setValue(result.subject);
                if (result.date == "0001-01-01 00:00:00") {
                    Ext.getCmp('date').setValue("");
                }
                else {
                    Ext.getCmp('date').setValue(result.date);
                }

            }
            else {
                Ext.Msg.alert("提示信息", "賦值出錯！");
            }
        },
        failure: function () {
            Ext.Msg.alert("提示信息", "獲取數據出現異常！");
        }
    });
}

function onOpen_downloadClick() {
    var content_id = document.getElementById('content_id').value;
    var log_id = Ext.getCmp('log_id').getValue();
    window.open("/EdmNew/ImportKXMD?content_id=" + content_id + "&log_id=" + log_id);
}

function onClose_downloadClick() {
    var log_id = Ext.getCmp('log_id').getValue();
    var content_id = document.getElementById('content_id').value;
    window.open("/EdmNew/ImportWKXMD?content_id=" + content_id + "&log_id=" + log_id);
}

function onEdm_sendClick() {
    var content_id = document.getElementById("content_id").value;
    var log_id = Ext.getCmp('log_id').getValue();
  
    var urlTran = '/EdmNew/EdmSendListCountView?content_id=' + content_id + "&log_id=" + log_id;;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#EdmSendListCount');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'EdmSendListCount',
        title: '發信名單統計',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

function onEdm_listClick() {
    var content_id = document.getElementById("content_id").value;
    var urlTran = '/EdmNew/EdmContentNewReport?content_id=' + content_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#EdmListGrid');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'EdmListGrid',
        title: '電子報統計報表',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}