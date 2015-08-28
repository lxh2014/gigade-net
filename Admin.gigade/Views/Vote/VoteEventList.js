var CallidForm;
var pageSize = 25;
//活動Model
Ext.define('gigade.VoteEvent', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "event_id", type: "string" },
        { name: "event_name", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "event_banner", type: "string" },
        //{ name: "bannerUrl", type: "string" },     
        { name: "event_start", type: "string" },
        { name: "event_end", type: "string" },
        { name: "word_length", type: "string" },
        { name: "vote_everyone_limit", type: "string" },
        { name: "vote_everyday_limit", type: "string" },
        { name: "number_limit", type: "string" },
        { name: "present_event_id", type: "string" },
        { name: "create_user", type: "string" },
        { name: "cuser", type: "string" },
        { name: "create_time", type: "string" },
        { name: "update_user", type: "string" },
        { name: "uuser", type: "string" },
        { name: "update_time", type: "string" },
        { name: "event_status", type: "string" },
        { name: "is_repeat", type: "string" }
    ]
});

var VoteEventStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.VoteEvent',
    proxy: {
        type: 'ajax',
        url: '/Vote/GetVoteEventList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("gdVoteEvent").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
VoteEventStore.on('beforeload', function ()
{
    Ext.apply(VoteEventStore.proxy.extraParams, {
        search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue()
    });
});
Ext.onReady(function ()
{
    var gdVoteEvent = Ext.create('Ext.grid.Panel', {
        id: 'gdVoteEvent',
        store: VoteEventStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "活動編號", dataIndex: 'event_id', width: 80, align: 'center' },
            {
                header: "活動名稱", dataIndex: 'event_name', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                    return value;
                }
            },
            {
                header: "活動備註", dataIndex: 'event_desc', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                    return value;
                }
            },
            {
                header: "活動廣告", id: 'imgsmall', colName: 'event_banner',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    if (value != '')
                    {
                        return '<div style="width:50px;height:50px"><a target="_blank", href="' + record.data.paperBanner + '"><img width="50px" height="50px" src="' + record.data.event_banner + '" /></a><div>'
                    } else
                    {
                        return null;
                    }
                },
                width: 60, align: 'center', sortable: false, menuDisabled: true
            },
            //{
            //    header: "問卷廣告鏈接", dataIndex: 'bannerUrl', width: 150, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
            //    {
            //        return Ext.String.format('<a href="{0}" target="bank">{1}</a>', value, value);
            //    }
            //},
            {
                header: "開始時間", dataIndex: 'event_start', width: 150, align: 'center'
                //, renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s')
            },
            {
                header: "結束時間", dataIndex: 'event_end', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    var event_end = Date.parse(value);
                    if (event_end < new Date())
                    {
                        //return '<p style="color:#F00;">' + Ext.Date.format(new Date(value), 'Y-m-d H:i:s') + '</p>';
                        return '<p style="color:#FF0000;">' + value + '</p>';
                    }
                    else
                    {
                        //return Ext.Date.format(new Date(value), 'Y-m-d H:i:s');
                        //return Ext.util.Format.date(value, "Y-m-d H:i:s");
                        return value;
                    }
                }
            },
            { header: "會員投票限制", dataIndex: 'vote_everyone_limit', width: 80, align: 'center' },
            { header: "每日投票限制", dataIndex: 'vote_everyday_limit', width: 80, align: 'center' },
            { header: "會員贈送次數", dataIndex: 'number_limit', width: 100, align: 'center' },
            {
                header: "促銷編號", dataIndex: 'present_event_id', width: 110, align: 'center'
            },
            {
                header: "是否重複投票", dataIndex: 'is_repeat', width: 110, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return "<span style='color:green'>是</span>"
                    } else {
                        return "<span style='color:red'>否</span>"
                    }
                }
            },
            {
                header: "創建人", dataIndex: 'cuser', width: 100, align: 'center'
            },
            {
                header: "創建時間", dataIndex: 'create_time', width: 150, align: 'center'
            },
            {
                header: "狀態", dataIndex: 'event_status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    if (value == "1")
                    {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.event_id + ")'><img hidValue='0' id='img" + record.data.event_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else
                    {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.event_id + ")'><img hidValue='1' id='img" + record.data.event_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }

        ],
        tbar: [
         { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
         { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          '->',
         {
             xtype: 'textfield', fieldLabel: "活動名稱/編號/備註", labelWidth: 120, id: 'search_content', listeners: {
                 specialkey: function (field, e)
                 {
                     // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                     // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
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
            store: VoteEventStore,
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
        items: [gdVoteEvent],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                gdVoteEvent.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    VoteEventStore.load({ params: { start: 0, limit: 25 } });
});
/*************************************************************************************查詢*************************************************************************************************/
function Query(x)
{
    VoteEventStore.removeAll();
    Ext.getCmp("gdVoteEvent").store.loadPage(1, {
        params: {
            search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue()

        }
    });
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function ()
{
    //addWin.show();
    editEventFunction(null, VoteEventStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function ()
{
    var row = Ext.getCmp("gdVoteEvent").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0)
    {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1)
    {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1)
    {
        editEventFunction(row[0], VoteEventStore);
    }
}
//更改狀態(啟用或者禁用)
function UpdateActive(id)
{
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Vote/UpdateEventState",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg)
        {  
            if (activeValue == 1)
            {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                VoteEventStore.load();
            }
            else
            {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                VoteEventStore.load();
            }
        },
        error: function (msg)
        {
            Ext.Msg.alert(INFORMATION, FAILURE);
            VoteEventStore.load();
        }
    });
}






