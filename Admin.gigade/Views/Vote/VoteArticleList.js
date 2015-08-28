var pageSize = 18;
/***********************群組管理主頁面****************************/
//群組管理Model
Ext.define('gigade.VoteArticle', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "article_id", type: "int" },
        { name: "event_id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "article_content", type: "string" },
        { name: "article_status", type: "int" },
        { name: "article_title", type: "string" },
        { name: "article_banner", type: "string" },//問題分類name
        { name: "create_user", type: "int" },
        { name: "create_time", type: "string" },
        { name: "update_time", type: "string" },
        { name: "update_user", type: "int" },
        { name: "creat_name", type: "string" },
        { name: "upd_name", type: "string" },
        { name: "event_name", type: "string" },
        { name: 'kendo_editor', type: 'string' }
    ]
});
var VoteArticle = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.VoteArticle',
    proxy: {
        type: 'ajax',
        url: '/Vote/GetVoteArticleList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});

//下拉框
Ext.define("gigade.Parametersrc", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "event_id", type: "string" },
        { name: "event_name", type: "string" }]
});
var DDRStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Parametersrc',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Vote/GetEventId",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
VoteArticle.on('beforeload', function () {
    Ext.apply(VoteArticle.proxy.extraParams, {
    });
});
//勾選框
function Query(x) {
    VoteArticle.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            ddlSel: Ext.getCmp('ddlSel').getValue(),
            selcontent: Ext.getCmp('selcontent').getValue(),
            time_start: Ext.Date.format(Ext.getCmp('time_start').getValue(), 'Y-m-d H:i:s'),
            time_end: Ext.Date.format(Ext.getCmp('time_end').getValue(), 'Y-m-d H:i:s')
        }
    });
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 80,
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
                        xtype: 'combobox',
                        fieldLabel: "活動編號",
                        allowBlank: true,
                        editable: false,
                        hidden: false,
                        id: 'ddlSel',
                        name: 'ddlSel',
                        store: DDRStore,
                        displayField: 'event_name',
                        valueField: 'event_id',
                        typeAhead: true,
                        forceSelection: false,
                        emptyText: 'SELECT',
                        labelWidth: 60
                    },
                    {
                        xtype: 'textfield',
                        allowBlank: true,
                        fieldLabel: "文章標題",
                        margin: '0 0 0 20',
                        labelWidth: 60,
                        id: 'selcontent',
                        name: 'searchcontent'
                    },
                    {
                        xtype: 'datetimefield',
                        fieldLabel: "開始時間",
                        id: 'time_start',
                        name: 'time_start',
                        margin: '0 5px 0 5px',
                        format: 'Y-m-d 00:00:00',
                        editable: false,
                        value: Tomorrow(1 - new Date().getDate()),//new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate(), 0, 0, 0),
                        listeners: {
                            select: function () {
                                var startTime = Ext.getCmp("time_start");
                                var endTime = Ext.getCmp("time_end");
                                var s_date = new Date(startTime.getValue());
                                if (endTime.getValue() < startTime.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間!");
                                    startTime.setValue(new Date(endTime.getValue()));

                                }
                                
                            }
                        }
                    },
                    {
                        xtype: 'datetimefield',
                        fieldLabel: "結束時間",
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5px',
                        format: 'Y-m-d 23:59:59',
                        editable: false,
                        value: Tomorrow(0),
                        listeners: {
                            select: function () {
                                var startTime = Ext.getCmp("time_start");
                                var endTime = Ext.getCmp("time_end");
                                var s_date = new Date(startTime.getValue());

                                if (endTime.getValue() < startTime.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間!");
                                    endTime.setValue(new Date(startTime.getValue()));

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
                        xtype: 'button',
                        margin: '0 10 0 10',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('ddlSel').setValue(0);//查詢條件
                                Ext.getCmp('selcontent').setValue(null);//查詢條件
                                Ext.getCmp('time_start').setValue(Tomorrow(1 - new Date().getDate()));
                                Ext.getCmp('time_end').setValue(Tomorrow(0));

                            }
                        }
                    }
                ]
            }
        ]
    });
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: VoteArticle,
        flex: 1.8,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: "文章編號", dataIndex: 'article_id', width: 80, align: 'center' },
            { header: "活動編號", dataIndex: 'event_id', width: 80, align: 'center' },
            { header: "活動名稱", dataIndex: 'event_name', width: 80, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', width: 80, align: 'center' },
            { header: "文章內容", dataIndex: 'article_content', width: 200, align: 'center' },
            { header: "文章標題", dataIndex: 'article_title', width: 100, align: 'center' },
            //{ header: "文章大圖", dataIndex: 'article_banner', width: 100, align: 'center' },
            {
                header: "文章大圖", id: 'imgsmall', colName: 'article_banner',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '') {
                        return '<div style="width:50px;height:50px"><a target="_blank", href="' + record.data.article_banner + '"><img width="50px" height="50px" src="' + record.data.article_banner + '" /></a><div>'
                    } else {
                        return null;
                    }
                },
                width: 60, align: 'center', sortable: false, menuDisabled: true
            },
            { header: "創建人", dataIndex: 'creat_name', width: 80, align: 'center' },
            { header: "創建時間", dataIndex: 'create_time', width: 150, align: 'center' },
            { header: "修改人", dataIndex: 'upd_name', width: 80, align: 'center' },
            { header: "修改時間", dataIndex: 'update_time', width: 150, align: 'center' },
            {
                header: "文章狀態",
                dataIndex: 'article_status',
                align: 'center',
                id: 'articlestatus',
                hidden: false,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.article_id + ")'><img hidValue='0' id='img" + record.data.article_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.article_id + ")'><img hidValue='1' id='img" + record.data.article_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
            //{ header: "回覆日期", dataIndex: 'response_createdates', width: 150, align: 'center', renderer: DateShow }
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', handler: onEditClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VoteArticle,
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
        layout: 'vbox',//fit
        items: [frm, gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    VoteArticle.load({ params: { start: 0, limit: 18 } });
});

function DateShow(val) {
    switch (val) {
        case "1970-01-01 08:00:00":
            return "~";
            break;
        case "0":
            return "~";
            break;
        default:
            return val;
            break;
    }
}
/***************************新增***********************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, VoteArticle);
}
/*********************編輯**********************/
onEditClick = function (question_id, order_id) {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], VoteArticle);
    }
}
function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() +s);
    return d;
}
/*********************啟用/禁用**********************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Vote/UpdateStatsVoteArticle",
        data: {
            "id": id,
            "status": activeValue
        },
        type: "post",
        type: 'text',
        success: function (msg) {
            Ext.Msg.alert(INFORMATION, "修改成功!");
            VoteArticle.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}

