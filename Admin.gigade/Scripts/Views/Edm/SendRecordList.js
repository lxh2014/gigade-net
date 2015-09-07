pageSize = 25;
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
Ext.define('gigade.EdmSend', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'send_status', type: 'int' },
    { name: 'content_id', type: 'int' },
    { name: 'content_title', type: 'string' },
    { name: 'open_total', type: 'int' },
    { name: 'sendtime', type: 'string' },
    { name: 'firsttime', type: 'string' },
    { name: 'lasttime', type: 'string' },
    { name: 'email_name', type: 'string' },
    { name: 'email_address', type: 'string' }
    ]
});
var SendRecordStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    autoLoad: true,
    pageSize: pageSize,
    model: 'gigade.EdmSend',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetSendRecordList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

SendRecordStore.on('beforeload', function ()
{
    Ext.apply(SendRecordStore.proxy.extraParams,
        {
            eid: document.getElementById("eid").value
        });
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
       // height: 70,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        {
            
            xtype: 'fieldcontainer',
            layout: 'hbox',
            fieldLabel: '姓名',
            items: [
                {
                    xtype: 'displayfield',
                    id: 'email_name',
                    name: 'email_name',
                    width: 200,
                    height:20
                }
                ]
        },
        {

            xtype: 'fieldcontainer',
            layout: 'hbox',
            fieldLabel: '電子郵件',
            items: [
                {
                    xtype: 'displayfield',
                    id: 'email_address',
                    name: 'email_address',
                    width: 200,
                    height: 20
                }
            ]
        }
        ]
    });
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',        
        store: SendRecordStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        //height: document.documentElement.clientHeight - 70,
        frame: true,
        flex: 9.4,
        columns: [
        {
            header: "電子報ID", dataIndex: 'content_id', flex: 1, align: 'center', hidden: true
        },
        {
            header: "email", dataIndex: 'email_address', flex: 1, align: 'center', hidden: true
        },
        {
            header: "name", dataIndex: 'email_name', flex: 1, align: 'center', hidden: true
        },
        {
            header: "發信狀態", dataIndex: 'send_status', width: 150, flex: 1, align: 'center',
            renderer: function (value) {
                if (value == 1) {
                    return "成功";
                }
            }
        },
        {
            header: "電子報名稱", dataIndex: 'content_title', flex: 3, align: 'left',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
            {
                return "<a href='javascript:void(0)' onclick='TranToDetial(" + record.data.content_id + ")'>" + value + "</a>"
            }
        },
        { header: "開信次數", dataIndex: 'open_total', width: 150, flex: 1, align: 'center' },
        { header: "寄信時間", dataIndex: 'sendtime', width: 150, flex: 1, align: 'center' },
        { header: "首次開信時間", dataIndex: 'firsttime', width: 150, flex: 1, align: 'center' },
        { header: "最近開信時間", dataIndex: 'lasttime', width: 150, flex: 1, align: 'center' }
        ],
        tbar: [
            '->',
           {
               xtype: 'textfield',
               id: 'content_title',
               fieldLabel: '電子報名稱',
               labelWidth: 80,
               width: 200,
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
                width:170,
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
                width: 120,
                id: 'start_time',
                format: 'Y-m-d',
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
                    Ext.getCmp('content_title').setValue(null);
                    Ext.getCmp('date').setValue('0');
                    Ext.getCmp('start_time').setValue(null);
                    Ext.getCmp('end_time').setValue(null);
                }
            },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SendRecordStore,
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
        items: [frm, gdList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                getEmailName();
                this.doLayout();
            }
        }
    });
})
function getEmailName()
{
    Ext.Ajax.request({
        url: '/Edm/GetEmailByID',
        params: {
            eid: document.getElementById("eid").value
        },

        success: function (form, action)
        {
            var result = Ext.decode(form.responseText);
            if (result.success)
            {
                Ext.getCmp("email_name").setValue(result.email_name);
                Ext.getCmp("email_address").setValue("<a href=\"mailto:"+result.email_address+"\">"+result.email_address+"</a>");              
            }
            else
            {
                Ext.Msg.alert(INFORMATION, "加載EmailName失敗!");
            }
        },
        failure: function ()
        {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}

function TranToDetial(content_id)
{
    var urlTran = '/Edm/EdmStatisticsList?cid=' + content_id;
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
    SendRecordStore.loadPage(1, {
        params: {
            content_title: Ext.getCmp('content_title').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue()
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