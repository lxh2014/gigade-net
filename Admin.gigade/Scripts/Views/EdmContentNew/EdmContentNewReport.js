Ext.onReady(function () {

    Ext.define('gigade.EdmContentNewReport', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "log_id", type: "int" },
        { name: "content_id", type: "int" },
        { name: "email_id", type: "int" },
        { name: "first_traceback", type: "string" },
        { name: "last_traceback", type: "string" },
        { name: "count", type: "int" },
        { name: "success", type: "int" },
        { name: "email", type: "string" },
        { name: "name", type: "string" },
        ]
    });
    EdmSendListCountStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        pageSize: 25,
        model: 'gigade.EdmSendListCount',
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


    var ReportForm = Ext.create('Ext.form.Panel', {
        id: 'ReportForm',
        layout: 'anchor',
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
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
        // store: EdmListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
                {
                    header: "log_id", dataIndex: 'date', width: 80, align: 'center'
                },
                {
                    header: "content_id", dataIndex: 'content_id', width: 100, align: 'center'
                },
                {
                    header: "email_id", dataIndex: 'email_id', width: 80, align: 'center'
                },
 
                {
                    header: "first_traceback", dataIndex: 'first_traceback', width: 150, align: 'center'
                },
               {
                     header: "last_traceback", dataIndex: 'last_traceback', width: 150, align: 'center'
                },
                {
                     header: "count", dataIndex: 'count', width: 150, align: 'center'
                 },
                 {
                    header: "count", dataIndex: 'count', width: 150, align: 'center'
                 },
                 ],

        tbar: [
           {
               xtype: 'button', text: "電子報統計報表", iconCls: '', id: 'edm_list',  disabled:true,
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
            //store: EdmListStore,
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
                load();
                this.doLayout();
            }
        }
    });
    //QueryAuthorityByUrl('/EdmNew/EdmContentNewReport');
});

function load()
{
    var content_id = document.getElementById('content_id').value;
    Ext.Ajax.request({
        url:'/EdmNew/Load',
        params: {
            content_id:content_id,
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.getCmp('successCount').setValue(result.successCount);
                Ext.getCmp('failCount').setValue(result.failCount);
                Ext.getCmp('totalPersonCount').setValue(result.totalPersonCount);
                Ext.getCmp('totalCount').setValue(result.totalCount);
                Ext.getCmp('openAveragePrecent').setValue(result.openAveragePrecent+"%");
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
            Ext.Msg.alert("提示信息","獲取數據出現異常！");
        }
    });
}

function onOpen_downloadClick() {
    var content_id = document.getElementById('content_id').value;
    window.open("/EdmNew/ImportKXMD?content_id="+content_id);
}

function onClose_downloadClick() {
    var content_id = document.getElementById('content_id').value;
    window.open("/EdmNew/ImportWKXMD?content_id=" + content_id);
}
function onEdm_sendClick() {
    var content_id = document.getElementById("content_id").value;
    var urlTran = '/EdmNew/EdmSendListCountView?content_id=' + content_id;
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

function onEdm_listClick()
{
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