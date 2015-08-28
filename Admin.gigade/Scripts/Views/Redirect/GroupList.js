var pageSize = 23;
//群組管理Model
Ext.define('gigade.RedirectGroup', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "group_id", type: "int" }, //
    { name: "group_name", type: "string" }, //
    { name: "group_createdate", type: "int" }, //
    { name: "createdate", type: "string" }, //
    { name: "group_updatedate", type: "int" }, //
    { name: "updatedate", type: "string" }, //
    { name: "group_ipfrom", type: "string" },
    { name: "group_type", type: "string" },
    { name: "parameterName", type: "string" }
    ]
});
var RedirectGroupStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.RedirectGroup',
    proxy: {
        type: 'ajax',
        url: '/Redirect/RedirectGroupList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("gdGroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
RedirectGroupStore.on('beforeload', function ()
{
    Ext.apply(RedirectGroupStore.proxy.extraParams, {
        search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue()
    });
});
Ext.onReady(function ()
{
    var gdGroup = Ext.create('Ext.grid.Panel', {
        id: 'gdGroup',
        store: RedirectGroupStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        {
            header: "群組統計報表", dataIndex: 'group_id', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
            {
                return '<a onclick=javascript:TranToCount("/Redirect/CounterView","' + value + '")><img  id="img' + record.data.group_id + '" src="../../../Content/img/icons/record.png"/></a>';
            }
        },
        { header: "群組編號", dataIndex: 'group_id', flex: 1, align: 'center' },
        { header: "群組名稱", dataIndex: 'group_name', flex:3, align: 'center' },
        { header: "群組類型", dataIndex: 'parameterName', flex:2, align: 'center' },
        {
            header: "群組連結列表", dataIndex: 'group_id', flex: 2, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
            {
                return '<a style="color:blue;" onclick=javascript:TranToRedirect("/Redirect/RedirectList","' + value + '")>' + "網站連結列表" + '</a> ';
            }
        },
        {
            header: "連結點管理", dataIndex: 'group_id', flex: 1.5, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
            {
                return '<a style="color:blue;" onclick=javascript:TranToDetial("/Redirect/GroupImport","' + value + '")>' + "匯入" + '</a>  &nbsp; &nbsp;<a style="color:blue;" onclick=javascript:ExportCSV2(' + value + ')>' + "匯出" + '</a> ';
            }
        }
        ],
        tbar: [
        { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
        { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        '->',
        {
            xtype: 'textfield', fieldLabel: "群組名稱", labelWidth: 60, id: 'search_content', listeners: {
                specialkey: function (field, e)
                {
                    if (e.getKey() == e.ENTER)
                    {
                        Query(1);
                    }
                }
            }
        },
        {
            text: SEARCH,
            iconCls: 'icon-search',
            id: 'btnQuery',
            handler: Query
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: RedirectGroupStore,
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
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdGroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                gdGroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //RedirectGroupStore.load({ params: { start: 0, limit: 25 } });
});
/*********新增**********/
onAddClick = function ()
{
    editFunction(null, RedirectGroupStore);

}
function Query(x)
{
    RedirectGroupStore.removeAll();
    Ext.getCmp("gdGroup").store.loadPage(1, {
        params: {
            search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue()
        }
    });
}
/*******編輯*********/
onEditClick = function ()
{
    var row = Ext.getCmp("gdGroup").getSelectionModel().getSelection();
    if (row.length == 0)
    {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1)
    {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1)
    {
        editFunction(row[0], RedirectGroupStore);
    }
}

function TranToCount(url, group_id)
{
    var record = "報表";
    var urlTran = url + '?group_id=' + group_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#RedirectCount');
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: 'RedirectCount',
        title: record,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
function TranToDetial(url, group_id)
{
    var record = "連接點管理";
    var urlTran = url + '?group_id=' + group_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#groupDetail');
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: 'groupDetail',
        title: record,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
//匯出
function ExportCSV(gid)
{
    window.open("/Redirect/RedirectClickExport?group_id=" + gid);
}
//匯出
function ExportCSV2(gid)
{
    window.open("/Redirect/RedirectExport?group_id=" + gid);
}
function TranToRedirect(url, group_id)
{
    var record = "連結詳情";
    var urlTran = url + '?group_id=' + group_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#RedirectList');
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: 'RedirectList',
        title: record,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
