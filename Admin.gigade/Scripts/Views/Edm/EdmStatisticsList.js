var pageSize = 25;

//電子報統計報表 Model
Ext.define('gigade.EdmStatisticsEdmList', {
    extend: 'Ext.data.Model',
    fields: [              
        { name: "date", type: "string" },
        { name: "week", type: "string" },
        { name: "total_person", type: "int" },
        { name: "personRate", type: "string" },
        { name: "total_click", type: "int" },
        { name: "clickRate", type: "string" },
        { name: "image_width", type: "int" },
    ]
});
var EdmListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.EdmStatisticsEdmList',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetStatisticsEdmList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

EdmListStore.on('beforeload', function ()
{
    Ext.apply(EdmListStore.proxy.extraParams,
        {
            cid: document.getElementById("cid").value
        });
});

Ext.onReady(function ()
{
    var TitleForm = Ext.create('Ext.form.Panel', {
        id: 'TitleForm',
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
                            id: 'content_title'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">發送時間</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'content_start'
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
                            value: '<span style="color:white;color:green;">總共發信人數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 500,
                            id: 'content_send'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">總開信人數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'content_person'
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
                            id: 'content_send_success'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">總開信次數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'content_click'
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
                            value: '<span style="color:white;color:green;">發信失敗人數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 500,
                            id: 'content_send_failed'
                        },
                        {
                            xtype: 'displayfield',
                            width: 100,
                            value: '<span style="color:white;color:green;">開信率</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 200,
                            id: 'content_openRate'
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
                            value: '<span style="color:white;color:green;">平均開信次數</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width: 500,
                            id: 'content_averageClick'
                        },
                    ]
                },
        ]

    });

    var EdmListGrid = Ext.create('Ext.grid.Panel', {
        id: 'EdmListGrid',
        store: EdmListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
                {
                    header: "日期", dataIndex: 'date', width: 150, align: 'center'
                },
                {
                    header: "週", dataIndex: 'week', width: 100, align: 'center'
                },
                {
                    header: "圖表（開信次數）", dataIndex: '', width: 280, align: 'center',
                    xtype: 'templatecolumn', tpl: '<img align="left" height=5 width={image_width} name="tplImg"  src="../../../Content/img/report_bar1.png" />'
                },
                {
                    header: "開信人數", dataIndex: 'total_person', width: 150, align: 'center'
                },
                {
                    header: "人數比率", dataIndex: 'personRate', width: 150, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                    {
                        return value + " %";
                    }
                },
                {
                    header: "開信次數", dataIndex: 'total_click', width: 150, align: 'center'
                },
                
                {
                    header: "次數比例", dataIndex: 'clickRate', width: 150, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                    {
                        return value + " %";
                    }
                }],

        tbar: [
           {
               xtype: 'button', text: "電子報統計報表", iconCls: '', id: 'edm_list',disabled:true, handler: onEdm_listClick

           },
           {
               xtype: 'button', text: "發信名單統計", iconCls: '', id: 'edm_send', handler: onEdm_sendClick
           },
           { xtype: 'button', text: "開信名單下載", iconCls: 'icon-excel', id: 'open_download',  handler: onOpen_downloadClick },
           { xtype: 'button', text: "未開信名單下載", iconCls: 'icon-excel', id: 'close_download', handler: onClose_downloadClick },

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller)
            {
                if (scroller && scroller.scrollEl)
                {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [TitleForm, EdmListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                EdmListGrid.width = document.documentElement.clientWidth;
                load();
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    QueryAuthorityByUrl('/Edm/EdmStatisticsList');
});

function QueryAuthorityByUrl(url)
{
    Ext.Ajax.request({
        url: '/FunctionGroup/GetAuthorityToolByUrl',
        method: "POST",
        params: { Url: url },
        success: function (form, action)
        {
            var data = Ext.decode(form.responseText);
            if (data.length > 0)
            {
                for (var i = 0; i < data.length; i++)
                {
                    var btn = Ext.getCmp(data[i].id);
                    if (btn)
                    {
                        btn.show();
                    }
                }
            }
        }
    });
}

onOpen_downloadClick = function ()
{

    Ext.MessageBox.show({
        msg: '正在匯出，請稍後....',
        width: 300,
        wait: true
    });
    Ext.Ajax.request({

        url: "/Edm/EdmSendExportCSV?st=1",
        timeout: 900000,
        params: {
            cid: document.getElementById("cid").value
        },

        success: function (form, action)
        {
            Ext.MessageBox.hide();
            var result = Ext.decode(form.responseText);
            if (result.success)
            {
                window.location = '../../ImportUserIOExcel/' + result.fileName;
            } else
            {
                Ext.MessageBox.hide();
                Ext.Msg.alert("提示信息", "匯出失敗或沒有數據！");
            }
        }
    });
}
onClose_downloadClick = function ()
{
    Ext.MessageBox.show({
        msg: '正在匯出，請稍後....',
        width: 300,
        wait: true
    });
    Ext.Ajax.request({
        url: "/Edm/EdmSendExportCSV?st=0",
        timeout: 900000,
        params: {
            cid: document.getElementById("cid").value
        },

        success: function (form, action)
        {
            Ext.MessageBox.hide();
            var result = Ext.decode(form.responseText);
            if (result.success)
            {
                window.location = '../../ImportUserIOExcel/' + result.fileName;
            } else
            {
                Ext.MessageBox.hide();
                Ext.Msg.alert("提示信息", "匯出失敗或沒有數據！");
            }
        }
    });
}
function onEdm_listClick()
{
    cid = document.getElementById("cid").value;
    var urlTran = '/Edm/EdmStatisticsList?cid=' + cid;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#EdmStatisticsList');
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: 'EdmStatisticsList',
        title: '電子報統計報表',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
function onEdm_sendClick()
{
    cid = document.getElementById("cid").value;
    var urlTran = '/Edm/EdmStatisticsSend?cid=' + cid;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#EdmStatisticsSend');
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: 'EdmStatisticsSend',
        title: '發信名單統計',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

function load()
{

    Ext.Ajax.request({
        url: '/Edm/EdmSendLoad',
        params: {
            cid: document.getElementById("cid").value
        },

        success: function (form, action)
        {
            var result = Ext.decode(form.responseText);
            if (result.success)
            {
                Ext.getCmp("content_start").setValue(result.data.content_start_s);
                Ext.getCmp("content_click").setValue(result.data.content_click + " 次");
                Ext.getCmp("content_person").setValue(result.data.content_person + " 人");
                Ext.getCmp("content_send").setValue(result.data.content_send + " 人" + '&nbsp<img align="right" height=5 width=' + result.data.content_imagewidth_send + '  src="../../../Content/img/report_bar1.png" />');
                Ext.getCmp("content_send_success").setValue(result.data.content_send_success + " 人" + '&nbsp<img align="right" height=5 width=' + result.data.content_imagewidth_success + '  src="../../../Content/img/report_bar3.png" />');
                Ext.getCmp("content_send_failed").setValue(result.data.content_send_failed + " 人" + '&nbsp<img align="right" height=5 width=' + result.data.content_imagewidth_failed + '  src="../../../Content/img/report_bar2.png" />');
                Ext.getCmp("content_title").setValue(result.data.content_title);
                Ext.getCmp("content_openRate").setValue(result.data.content_openRate + " %");
                Ext.getCmp("content_averageClick").setValue(result.data.content_averageClick + " 次/人");
            }
            else
            {
                Ext.Msg.alert(INFORMATION, "加載Title失敗!");
            }
        },
        failure: function ()
        {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}