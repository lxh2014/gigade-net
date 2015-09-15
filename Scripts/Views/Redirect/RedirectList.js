var pageSize = 24;
var group_id; 
var group_name;
//群組管理Model
Ext.define('gigade.Redirect', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "redirect_id", type: "int" }, //
        { name: "redirect_name", type: "string" }, //
        { name: "group_id", type: "int" },
        { name: "user_group_id", type: "int" },
        { name: "group_name", type: "string" }, //
        { name: "redirect_url", type: "string" }, //
        { name: "redirect_status", type: "int" }, //
        { name: "redirect_total", type: "int" }, //
        { name: "redirect_note", type: "string" }, //
        { name: "sredirect_createdate", type: "string" }, //
        { name: "sredirect_updatedate", type: "string" }, //
        { name: "redirect_ipfrom", type: "string" }, //
        { name: "user", type: "string" }, 
        { name: "order", type: "string" }
    ]
});
var RedirectStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Redirect',
    proxy: {
        type: 'ajax',
        url: '/Redirect/GetRedirectList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});
RedirectStore.on('beforeload', function () {
    Ext.apply(RedirectStore.proxy.extraParams,
       {
           group_id: group_id
       });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdRedirect").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
Ext.onReady(function () {
    group_id = document.getElementById("group_id").value;
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 40,
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
                    xtype: 'displayfield',
                    id: 'group_name',
                    name: 'group_name',
                    value: '<div style="color:#F00;font-size:20px;">'+document.getElementById('group_name').value+'</div>'
                }
            ]
        }
        ]
    });
    var gdRedirect = Ext.create('Ext.grid.Panel', {
        id: 'gdRedirect',
        store: RedirectStore,
        columnLines: true,
        flex: 1.8,
        frame: true,
        columns: [
        {
            header: "報表", dataIndex: 'redirect_id', flex: 0.7, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return '<a href=javascript:TranToCount("/Redirect/CounterView","' + value + '")><img  id="img' + record.data.group_id + '" src="../../../Content/img/icons/record.png"/></a>';
            }
        },
        { header: "編號", dataIndex: 'redirect_id', flex: 0.7, align: 'center' },
        { header: "名稱", dataIndex: 'redirect_name', flex: 3.25 },
        {
            header: "目的連結", flex: 3.25, dataIndex: 'redirect_url', xtype: 'templatecolumn',
            tpl: '<a target="_blank" href="{redirect_url}" >{redirect_url}</a>'
        },
        { header: "點閱次數", dataIndex: 'redirect_total', flex: 0.7, align: 'center' },
        {
            header: "狀態", dataIndex: 'redirect_status', flex: 0.7, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "正常";
                } else {
                    return "<span style='font-size:14px; color:#F00;'>停用</span>";
                }
            }
        },
        {
            header: "連結網址", flex: 1, dataIndex: 'redirect_id', align: 'center', renderer:
            function (value, cellmeta, record, rowIndex, columnIndex, store) {               
                return '<a style="color:blue;"  target="_blank" href=' + document.getElementById('LinkAdress').value + '/public/link.php?r=' + record.data.redirect_id + '>' + "連結網址" + '</a> ';
            }
        },
        {
            header: "設置廣告", flex: 1, dataIndex: 'redirect_id', align: 'center', renderer:
            function (value, cellmeta, record, rowIndex, columnIndex, store) {               
                return '<a style="color:blue;"  title=' + document.getElementById('LinkAdress').value + '/public/link.php?r=' + record.data.redirect_id + ' onclick=javascript:function_add("' + document.getElementById('LinkAdress').value + '/public/link.php?r=' + record.data.redirect_id + '")>' + "設置廣告" + '</a> ';
            }
        }
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: RedirectStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        viewConfig: {
            forceFit: true
        },
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
        items: [frm,gdRedirect],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdRedirect.width = document.documentElement.clientWidth;
                gdRedirect.height = document.documentElement.clientHeight-20;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    RedirectStore.load({ params: { start: 0, limit: 25 } });
}); 
/*新增********/
onAddClick = function () {
    editRedirectFunction(null, RedirectStore);

}

/*編輯********/
onEditClick = function () {
    var row = Ext.getCmp("gdRedirect").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editRedirectFunction(row[0], RedirectStore);
    }
}

function TranToCount(url, group_id) {
    var record = "報表";
    var urlTran = url + '?redirect_id=' + group_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#RedirectCount');
    if (copy) {
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
//連結到廣告
function function_add(i)
{
    editFunction(null,null,i);
}


function TranToRedirect(url, redirect_url) {
    var record = "連結詳情";
    var urlTran = url + '?redirect_url=' + redirect_url;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#RedirectList');
    if (copy) {
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