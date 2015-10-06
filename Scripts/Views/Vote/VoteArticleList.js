var pageSize = 18;
/***********************群組管理主頁面****************************/
//群組管理Model
Ext.define('gigade.VoteArticle', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "article_id", type: "int" },
        { name: "product_id", type: "int" },
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
        { name: 'kendo_editor', type: 'string' },
        { name: 'product_name', type: 'string' },//
        { name: 'vote_count', type: 'int' },
        { name: 'reception_count', type: 'int' },
        { name: 'article_sort', type: 'int' },
        { name: 'prod_link', type: 'string' },
        { name: 'name', type: 'string' },
        { name: 'article_start_time', type: 'string' },
        { name: 'article_end_time', type: 'string' },
        { name: 'article_show_start_time', type: 'string' },
        { name: 'article_show_end_time', type: 'string' },
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
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
           { 'txt': '所有日期', 'value': '0' },
           { 'txt': '文章創建時間', 'value': '1'},
           { 'txt': '文章開始時間', 'value': '2' },
           { 'txt': '文章結束時間', 'value': '3' },
           { 'txt': '顯示開始時間', 'value': '4' },
           { 'txt': '顯示結束時間', 'value': '5' }
    ]
}
);
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
VoteArticle.on('beforeload', function () {
    Ext.apply(VoteArticle.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue(),
        selcontent: Ext.getCmp('selcontent').getValue(),
        date:Ext.getCmp('date').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue()
    });
});
//勾選框
function Query(x) {
    VoteArticle.removeAll();
    var date = Ext.getCmp('date').getValue();
    var time_start = Ext.getCmp('time_start').getValue();
    var time_end = Ext.getCmp('time_end').getValue();
    var ddlSel= Ext.getCmp('ddlSel').getValue();
    var selcontent = Ext.getCmp('selcontent').getValue();
    if (ddlSel == null && selcontent == "" && date == null && time_start == null && time_end == null) {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else if (date != 0 && (time_start == null || time_end == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇日期範圍');
    }
    else if ((date == 0||date == null) && (time_start != null && time_end != null)) {
        Ext.Msg.alert(INFORMATION, '請選擇日期條件');
    }
    else {
        Ext.getCmp("gdFgroup").store.loadPage(1);
    }
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        flex: 1.5,
        //height: 100,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                //combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: "活動名稱",
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
                        emptyText: '請選擇',
                        labelWidth: 60,
                        queryMode: 'remote',
                        lastQuery: '',
                        listeners: {
                            beforequery: function (e) {
                                DDRStore.load();
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        allowBlank: true,
                        fieldLabel: "文章標題",
                        margin: '0 0 0 20',
                        labelWidth: 60,
                        id: 'selcontent',
                        name: 'searchcontent',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    }                
                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'date',
                        store: dateStore,
                        fieldLabel: '查詢日期',
                        editable: false,
                        labelWidth: 60,
                        displayField: 'txt',
                        valueField: 'value',
                       // emptyText:'請選擇..'
                        value:0
                    },
                    {
                        xtype: 'datefield',
                        margin: '0 0 0 10',
                        width: 110,
                        id: 'time_start',
                        format: 'Y-m-d',                       
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                if (end.getValue() == null) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                else if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, DATA_TIP);
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }   
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        margin: '0 5 0 5',
                        value: '~',
                    },
                    {
                        xtype: 'datefield',
                        id: 'time_end',
                        format: 'Y-m-d',
                        width: 110,
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                if (start.getValue() == null) {
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                                else if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, DATA_TIP);
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }                              
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
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
                                Ext.getCmp('ddlSel').reset();//查詢條件
                                Ext.getCmp('selcontent').reset();//查詢條件
                                Ext.getCmp('time_start').reset()
                                Ext.getCmp('time_end').reset();
                                Ext.getCmp('date').reset();

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
        flex: 8.5,
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
            { header: "商品名稱", dataIndex: 'product_name', width: 80, align: 'center' },
             { header: "商品鏈接", dataIndex: 'prod_link', width: 80, align: 'center' },
            { header: "活動編號", dataIndex: 'event_id', width: 80, align: 'center' },
            { header: "活動名稱", dataIndex: 'event_name', width: 80, align: 'center' },
                { header: "會員名稱", dataIndex: 'name', width: 80, align: 'center' },
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
               { header: "排序", dataIndex: 'article_sort', width: 100, align: 'center' },
            { header: "前台顯示投票數量", dataIndex: 'vote_count', width: 100, align: 'center' },
            { header: "實際投票數量", dataIndex: 'reception_count', width: 100, align: 'center' },
            { header: "創建人", dataIndex: 'creat_name', width: 80, align: 'center', hidden: true },
            { header: "創建時間", dataIndex: 'create_time', width: 150, align: 'center' },
               { header: "文章開始時間", dataIndex: 'article_start_time', width: 150, align: 'center' },
                  { header: "文章結束時間", dataIndex: 'article_end_time', width: 150, align: 'center' },
                     { header: "顯示開始時間", dataIndex: 'article_show_start_time', width: 150, align: 'center' },
                     { header: "顯示結束時間", dataIndex: 'article_show_end_time', width: 150, align: 'center' },
            { header: "修改人", dataIndex: 'upd_name', width: 80, align: 'center', hidden: true },
            { header: "修改時間", dataIndex: 'update_time', width: 150, align: 'center', hidden: true },
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
        //   autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //VoteArticle.load({ params: { start: 0, limit: 18 } });
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
    d.setDate(d.getDate() + s);
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
            //Ext.Msg.alert(INFORMATION, "修改成功!");
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

