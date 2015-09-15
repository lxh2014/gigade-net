var CallidForm;
var pageSize = 25;
var EdmContentStore;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Sites', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "content_id", type: "int" },
    { name: "group_id", type: "int" },
    { name: "content_email_id", type: "int" },
    { name: "content_start", type: "string" },
    { name: "content_end", type: "string" },
    { name: "content_range", type: "int" },
    { name: "content_single_count", type: "int" },
    { name: "content_click", type: "int" },
    { name: "content_person", type: "int" },
    { name: "content_status", type: "int" },
    { name: "content_send_success", type: "int" },
    { name: "content_send_failed", type: "int" },
    { name: "content_from_name", type: "string" },
    { name: "content_from_email", type: "string" },
    { name: "content_reply_email", type: "string" },
    { name: "content_priority", type: "int" },
    { name: "content_title", type: "string" },
    { name: "content_body", type: "string" },
    { name: "content_createdate", type: "string" },
    { name: "content_updatedate", type: "string" },
    { name: "content_send_count", type: "int" },
    { name: "s_content_start", type: "datetime" },
    { name: "info_epaper_id", type: "int" },
    { name: "epaper_id", type: "epaper_title" }
    ]
});
var searchStatusStore = Ext.create('Ext.data.Store', {
    fields: ['StatusText', 'StatusValue'],
    data: [
    { "StatusText": "全部", "StatusValue": "0" },
    { "StatusText": "待測試", "StatusValue": "1" },
    { "StatusText": "待審核", "StatusValue": "2" },
    { "StatusText": "待發送", "StatusValue": "3" },
    { "StatusText": "發送中", "StatusValue": "4" },
    { "StatusText": "暫停", "StatusValue": "9" },
    { "StatusText": "已完成", "StatusValue": "5" },

    ]
});
EdmContentStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Sites',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetEdmContentList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

EdmContentStore.on('beforeload', function () {
    Ext.apply(EdmContentStore.proxy.extraParams,
    {
        search_text: Ext.getCmp('search_text').getValue(),
        searchStatus: Ext.getCmp('searchStatus').getValue(),
        tstart: Ext.getCmp('start').getValue(),
        tend: Ext.getCmp('end').getValue()
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdSites").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdSites").down('#remove').setDisabled(true);
            var row = Ext.getCmp("gdSites").getSelectionModel().getSelection();
            var ids = "";
            for (var i = 0; i < row.length; i++) {
                ids += row[i].data.content_status + ",";
            }
            var array = ids.split(",")
            array.sort();
            for (var i = 1; i < array.length; i++) {
                if (array[i] > 3) {
                    Ext.getCmp("gdSites").down('#remove').setDisabled(true);
                    break;
                }
                else {
                    Ext.getCmp("gdSites").down('#remove').setDisabled(false);
                }
            }
        }
    }
});
var channelTpl = new Ext.XTemplate(
'<a href="JavaScript:void{0}" >報表</a>'
);

var EditTpl = new Ext.XTemplate(
'<a href=javascript:TranToDetial("/Edm/EdmContentAdd","{content_id}")>' + "修改" + '</a> '
)

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 100,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: 'datetimefield',
                fieldLabel: "發送時間",
                id: 'start',
                name: 'start',
                margin: '0 5px 0 0',
                labelWidth: 65,
                format: 'Y-m-d H:i:s',
                editable: false,
                time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                listeners: {
                    select: function (a, b, c) {
                        var tstart = Ext.getCmp("start");
                        var tend = Ext.getCmp("end");
                        if (tend.getValue() == null) {
                            tend.setValue(setNextMonth(tstart.getValue(), 1));
                        }
                        else if (tend.getValue() < tstart.getValue()) {
                            Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                            tend.setValue(setNextMonth(tstart.getValue(), 1));
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 0 0 2',
                value: "~",
                width: 65
            },
            {
                xtype: 'datetimefield',
                id: 'end',
                name: 'end',
                margin: '0 5px 0 0',
                labelWidth: 15,
                // width: 210,
                format: 'Y-m-d H:i:s',
                editable: false,
                time: { hour: 23, min: 59, sec: 59 },//標記結束時間
                listeners: {
                    select: function (a, b, c) {
                        var tstart = Ext.getCmp("start");
                        var tend = Ext.getCmp("end");
                        if (tstart.getValue() == null) {
                            tstart.setValue(setNextMonth(tend.getValue(), -1));
                        }
                        else if (tend.getValue() < tstart.getValue()) {
                            Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                            tstart.setValue(setNextMonth(tend.getValue(), -1));
                        }
                    }
                }
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                store: searchStatusStore,
                id: 'searchStatus',
                fieldLabel: '查詢條件',
                displayField: 'StatusText',
                valueField: 'StatusValue',
                //width: 150,
                labelWidth: 65,
                margin: '0 5 0 0',
                forceSelection: false,
                editable: false,
                value: '0'
            },
            {
                xtype: 'textfield',
                fieldLabel: "郵件主旨",
                //width: 200,
                labelWidth: 65,
                margin: '0 5 0 0',
                id: 'search_text',
                name: 'search_text',
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            }]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: 'button',
                margin: '0 5 0 35',
                iconCls: 'ui-icon ui-icon-search-2',
                text: "查詢",
                id: 'btnQuery',
                handler: Query

            },
            {
                xtype: 'button',
                text: '重置',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp('searchStatus').setValue('0');
                        Ext.getCmp('search_text').setValue('');
                        Ext.getCmp('start').setValue(null);
                        Ext.getCmp('end').setValue(null);
                        // Ext.getCmp('source').setValue('');
                    }
                }
            }
            ]
        }
        ]
    });
    var gdSites = Ext.create('Ext.grid.Panel', {
        id: 'gdSites',
        store: EdmContentStore,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight - 100,
        columnLines: true,
        frame: true,
        columns: [
        { header: "編號", dataIndex: 'content_id', width: 60, align: 'center' },
        //{ header: '修改', width: 60, align: 'center', xtype: 'templatecolumn', tpl: EditTpl },
        // { header: "報表", xtype: 'templatecolumn', width: 60, tpl: channelTpl, align: 'center' },
        {
            header: "報表", width: 100, align: 'center', hidden: true, id: 'reportForm',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<a href='javascript:void(0)' onclick='TranToStatisticsList(" + record.data.content_id + ")'><img src='../../../Content/img/icon_report.gif' /></a>"
            }
        },
        {
            header: "狀態", dataIndex: 'content_status', width: 150, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                switch (record.data.content_status) {
                    case 5:
                        return "已完成";
                        break;
                    case 1:
                        //return "待測試";
                        return Ext.String.format("<a href='javascript:void(0);' onclick='onStatusClick()' >待測試</a>");
                        //return "<a href='javascript:void(0);'  onclick='onAddClick()'>" + "待測試" + "</a>";
                        break;
                    case 2:
                        return "待審核";
                        break;
                    case 3:
                        return "待發送";
                        break;
                    case 4:
                        return "發送中";
                        break;
                    case 9:
                        return "暫停";
                        break;
                    default:
                        return "未知";
                }
            }
        },
        { header: "郵件主旨", dataIndex: 'content_title', width: 200, align: 'center' },
        { header: "發送時間", dataIndex: 's_content_start', width: 150, align: 'center' },
        { header: "總發信量", dataIndex: 'content_send_count', width: 150, align: 'center' },
        { header: "開信人數", dataIndex: 'content_person', width: 80, align: 'center' },
        { header: "開信次數", dataIndex: 'content_click', width: 80, align: 'center' }
        //{ header: "內容", dataIndex: 'content_body', width: 80, align: 'center', hidden: true },
        // { header: "收信人名單", dataIndex: 'group_id', width: 80, align: 'center' }
        ],
        tbar: [
        { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', hidden: true, handler: onAddClick },
        { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "刪除", id: 'remove', hidden: false, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmContentStore,
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
        },

        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdSites],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdSites.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //EdmContentStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    var urlTran = '/Edm/EdmContentAdd';
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#addEdm');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'addEdm',
        title: '新增電子報',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdSites").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        //editFunction(row[0], EdmContentStore);
        var urlTran = '/Edm/EdmContentAdd?EdmStore=' + row[0].data.content_id;
        var panel = window.parent.parent.Ext.getCmp('ContentPanel');
        var copy = panel.down('#detial');
        if (copy) {
            copy.close();
        }
        copy = panel.add({
            id: 'detial',
            title: '編輯電子報',
            html: window.top.rtnFrame(urlTran),
            closable: true
        });
        panel.setActiveTab(copy);
        panel.doLayout();
    }
}

onStatusClick = function () {
    var row = Ext.getCmp("gdSites").getSelectionModel().getSelection();
    statusFunction(row[0], EdmContentStore);
}
//******刪除****///
onRemoveClick = function () {
    var row = Ext.getCmp("gdSites").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "未選中一行");
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.content_id + '|';
                }
                Ext.Ajax.request({
                    url: '/Edm/DeleteEdm',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", "刪除成功");
                            EdmContentStore.load();
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗");
                            EdmContentStore.load();
                        }
                    }
                });
            }
        });
    }
}
function Query(x) {
    EdmContentStore.removeAll();
    var search_text = Ext.getCmp('search_text').getValue();
    if (search_text != "" || Ext.getCmp('searchStatus').getValue() != null) {
        Ext.getCmp("gdSites").store.loadPage(1, {
            params: {
                search_text: search_text,
                searchStatus: Ext.getCmp('searchStatus').getValue(),
                tstart: Ext.getCmp('start').getValue(),
                tend: Ext.getCmp('end').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert("提示信息", "請選擇搜索條件");
    }
}


function TranToDetial(url, content_id) {

    var urlTran = url + '?EdmStore=' + content_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '編輯電子報',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
function TranToStatisticsList(content_id) {
    var urlTran = '/Edm/EdmStatisticsList?cid=' + content_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#EdmStatisticsList');
    if (copy) {
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





