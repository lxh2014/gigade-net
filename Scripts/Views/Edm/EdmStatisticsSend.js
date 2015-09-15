var pageSize = 25;
var info_type = "edm_group_email";//資安數據來源的表
var secret_info = "user_id;user_name;user_email";//grid 列表頁顯示數據的列名
//日期條件Store
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
           { 'txt': '所有日期', 'value': '0' },
            { 'txt': '寄信時間', 'value': '1' },
            { 'txt': '首次開信時間', 'value': '2' },
            { 'txt': '最近開信時間', 'value': '3' }
    ]

});
//發信名單統計 Model
Ext.define('gigade.EdmStatisticsEdmSend', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "email_id", type: "int" },
        { name: "s_send_status", type: "string" },
        { name: "email_name", type: "string" },
        { name: "open_total", type: "int" },
        { name: "s_send_datetime", type: "string" },
        { name: "s_open_first", type: "string" },
        { name: "s_open_last", type: "string" },
        { name: "image_width", type: "int" },       

    ]
});
var EdmSendStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    autoLoad: true,
    pageSize: pageSize,
    model: 'gigade.EdmStatisticsEdmSend',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetStatisticsEdmSend',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

EdmSendStore.on('beforeload', function ()
{
    Ext.apply(EdmSendStore.proxy.extraParams,
        {
            cid: document.getElementById("cid").value,
            email_name: Ext.getCmp('email_name').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue()
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
                            width:500,
                            id: 'content_title'
                        },
                        {
                            xtype: 'displayfield',
                            width:100,
                            value: '<span style="color:white;color:green;">發送時間</span>'
                        },
                        {
                            xtype: 'displayfield',
                            width:200,
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
    var EdmSendGrid = Ext.create('Ext.grid.Panel', {
        id: 'EdmSendGrid',
        store: EdmSendStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
                {
                    header: "發信狀態", dataIndex: 's_send_status', width: 150, align: 'center'
                },
                {
                    header: "姓名", dataIndex: 'email_name', width: 200, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                    {                       
                       return "<span onclick='SecretLogin(" + record.data.email_id + "," + 0 + ",\"" + info_type + "\")'  >" + value + "</span>";
                    }
                },
                {
                    header: "圖表", dataIndex: '', width: 200, align: 'center',
                    xtype: 'templatecolumn', tpl: '<img align="left" height=5 width={image_width} name="tplImg"  src="../../../Content/img/report_bar1.png" />'
                },
                {
                    header: "開信次數", dataIndex: 'open_total', width: 100, align: 'center'
                },
                {
                    header: "寄信時間", dataIndex: 's_send_datetime', width: 150, align: 'center'
                },
                {
                    header: "首次開信時間", dataIndex: 's_open_first', width: 150, align: 'center'
                },
                {
                    header: "最近開信時間", dataIndex: 's_open_last', width: 150, align: 'center'
                }],

        tbar: [
           {
               xtype: 'button', text: "電子報統計報表", iconCls: '', id: 'edm_list', handler: onEdm_listClick 
              
           },
           {
               xtype: 'button', text: "發信名單統計", iconCls: '', id: 'edm_send', disabled: true, handler: onEdm_sendClick
           },
           { xtype: 'button', text: "開信名單下載",iconCls:'icon-excel', id: 'open_download', handler: onOpen_downloadClick },
           { xtype: 'button', text: "未開信名單下載", iconCls: 'icon-excel', id: 'close_download', handler: onClose_downloadClick },

        ],
        dockedItems: [
            {   //類似于tbar
                xtype: 'toolbar',
                dock: 'top',
                items: [
                    {
                        xtype: 'textfield',
                        id: 'email_name',
                        fieldLabel: '姓名',
                        labelWidth: 45,
                        width: 160,
                        listeners: {
                            specialkey: function (field, e)
                            {
                                if (e.getKey() == Ext.EventObject.ENTER)
                                {
                                    Query();
                                }
                            }
                        }
                    },
            {
                xtype: 'combobox',
                fieldLabel: '日期條件',
                margin: '0 0 0 5',
                labelWidth: 60,
                width: 170,
                id: 'date',
                store: dateStore,
                displayField: 'txt',
                valueField: 'value',
                editable: false,
                value: '0'
            },
            {
                xtype: 'datefield',
                margin: '0 0 0 5',

                id: 'start_time',
                format: 'Y-m-d',
                width: 120,
                //value: Tomorrow(1 - new Date().getDate()),
                editable: false,
                listeners: {
                    select: function (a, b, c)
                    {
                        var start = Ext.getCmp("start_time");
                        var end = Ext.getCmp("end_time");
                        var s_date = new Date(start.getValue());
                        end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                    },
                    specialkey: function (field, e)
                    {
                        if (e.getKey() == Ext.EventObject.ENTER)
                        {
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
                width: 120,
                //value: Tomorrow(0),
                editable: false,
                listeners: {
                    select: function (a, b, c)
                    {
                        var start = Ext.getCmp("start_time");
                        var end = Ext.getCmp("end_time");
                        var s_date = new Date(start.getValue());

                        if (end.getValue() < start.getValue())
                        {
                            Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                    },
                    specialkey: function (field, e)
                    {
                        if (e.getKey() == Ext.EventObject.ENTER)
                        {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'button',
                text: '查詢',
                margin: '0 2 0 5',
                iconCls: 'icon-search',
                handler: function ()
                {
                    Query();
                }
            },
            {
                xtype: 'button',
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function ()
                {
                    Ext.getCmp('email_name').setValue(null);
                    Ext.getCmp('date').setValue('0');
                    Ext.getCmp('start_time').setValue(null);
                    Ext.getCmp('end_time').setValue(null);
                }
            }
         ]}
       ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmSendStore,
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
        items: [TitleForm, EdmSendGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                EdmSendGrid.width = document.documentElement.clientWidth;
                load();
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    QueryAuthorityByUrl('/Edm/EdmStatisticsSend');
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

function TranToDetial(email_id)
{
    var urlTran = '/Edm/SendRecordList?eid=' + email_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#SendRecordList');
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: 'SendRecordList',
        title: '名單寄送記錄',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

function Query()
{
    if (Ext.getCmp('date').getValue() != 0)
    {
        if (Ext.getCmp('start_time').getValue() == null)
        {
            Ext.Msg.alert(INFORMATION, "請選擇查詢開始日期");
            return;
        }
        if (Ext.getCmp('end_time').getValue() == null)
        {
            Ext.Msg.alert(INFORMATION, "請選擇查詢結束日期");
            return;
        }
    }

    Ext.getCmp('EdmSendGrid').store.loadPage(1, {
        params: {
            email_name:Ext.getCmp('email_name').getValue(),
            date:Ext.getCmp('date').getValue(),
            start_time:Ext.getCmp('start_time').getValue(),
            end_time:Ext.getCmp('end_time').getValue()
        }
    });
}

function Tomorrow(s)
{
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "15";//參數表中的"訊息管理"
    var url = "/Edm/EdmPersonList";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }
}