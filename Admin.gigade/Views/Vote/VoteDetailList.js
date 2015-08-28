var pageSize = 25;
Ext.define('gigade.VoteDetail', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vote_id", type: "int" },
        { name: "article_id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "ip", type: "string" },
        { name: "vote_status", type: "int" },
        { name: "create_user", type: "int" },
        { name: "update_user", type: "int" },
        { name: "create_time", type: "string" },
        { name: "update_time", type: "string" }
    ]
});

var VoteDetailStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.VoteDetail',
    proxy: {
        type: 'ajax',
        url: '/Vote/VoteDetailList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
//勾選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
//文章Model
Ext.define("gigade.VoteArticle", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "article_id", type: "string" },
        { name: "article_title", type: "string" }
    ]
});
var ArticleStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VoteArticle',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Vote/GetArticle",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var StatusStore = Ext.create('Ext.data.Store', {
    fields: ['status_name', 'status_id'],
    data: [
        { "status_name": "所有狀態", "status_id": "-1" },
        { "status_name": "已啟用", "status_id": "1" },
        { "status_name": "未啟用", "status_id": "0" }
    ]
});
VoteDetailStore.on('beforeload', function () {
    Ext.apply(VoteDetailStore.proxy.extraParams, {
        article_id: Ext.getCmp('vote_article').getValue(),
        searchContent: Ext.getCmp('searchContent').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue(),
        vote_status: Ext.htmlEncode(Ext.getCmp("vote_status").getValue())
    });
});

function Query(x) {
    VoteDetailStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1);
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
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
                        allowBlank: true,
                        hidden: false,
                        id: 'vote_article',
                        name: 'vote_article',
                        store: ArticleStore,
                        queryMode: 'local',
                        width: 200,
                        labelWidth: 55,
                        margin: '5 10 0 5',
                        displayField: 'article_title',
                        valueField: 'article_id',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        fieldLabel: "查詢條件",
                        emptyText: '請選擇文章標題',
                        listeners: {
                            afterRender: function (combo) {
                                combo.setValue(0);
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: "查詢內容",
                        width: 200,
                        labelWidth: 55,
                        margin: '5 10 0 0',
                        id: 'searchContent',
                        name: 'searchContent'
                    },
                    {
                        xtype: "datetimefield",
                        fieldLabel: "日期條件",
                        labelWidth: 55,
                        margin: '5 0 0 0',
                        id: 'time_start',
                        name: 'time_start',
                        format: 'Y-m-d 00:00:00',
                        editable: false,
                        allowBlank: false,
                        submitValue: true,
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
                        xtype: 'displayfield',
                        margin: '5 0 0 0',
                        value: "~"
                    },
                    {
                        xtype: "datetimefield",
                        id: 'time_end',
                        name: 'time_end',
                        margin: '5 0 0 0',
                       // format: 'Y-m-d H:i:s',
                        format: 'Y-m-d 23:59:59',
                        editable: false,
                        allowBlank: false,
                        submitValue: true,
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
                        xtype: 'combobox',
                        allowBlank: true,
                        hidden: false,
                        id: 'vote_status',
                        name: 'vote_status',
                        store: StatusStore,
                        queryMode: 'local',
                        width: 200,
                        labelWidth: 55,
                        margin: '0 100 0 5',
                        displayField: 'status_name',
                        valueField: 'status_id',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        fieldLabel: "查詢狀態",
                        emptyText: '所有狀態',
                        value:-1
                    }
                ]
            }
        ],
        buttonAlign: 'left',
        buttons: [
            {
                margin:"0 0 0 5",
                iconCls: 'icon-search',
                text: "查詢",
                handler: Query
            },
            {
                text: '重置',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    this.up('form').getForm().reset();
                }
            }
        ]
    });

    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: VoteDetailStore,
        flex: 1.8,
        width: document.documentElement.clientWidth,
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
            { header: "編號", dataIndex: 'vote_id', width: 50, align: 'center' },
            { header: "文章編號", dataIndex: 'article_id', width: 100, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', width: 100, align: 'center' },
            { header: "IP", dataIndex: 'ip', width: 120, align: 'center' },
            { header: "創建時間", dataIndex: 'create_time', width: 140, align: 'center' },
            {
                header: "啟用",
                dataIndex: 'vote_status',
                width: 80,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.vote_id + ")'><img hidValue='0' id='img" + record.data.vote_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.vote_id + ")'><img hidValue='1' id='img" + record.data.vote_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: '新增', id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            {
                xtype: 'button', text: "匯出", id: 'export', hidden: false, iconCls: 'icon-user-edit', handler: function () {
                    window.open("/Vote/VoteDetailExportExcel?article_id=" + Ext.getCmp('vote_article').getValue() + "&searchContent=" + Ext.getCmp('searchContent').getValue() + "&vote_status=" + Ext.getCmp('vote_status').getValue() + "&time_start=" + Ext.Date.format(Ext.getCmp('time_start').getValue(), 'Y-m-d') + "&time_end=" + Ext.Date.format(Ext.getCmp('time_end').getValue(), 'Y-m-d'));
                }
            }// disabled: true,

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VoteDetailStore,
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
    ArticleStore.load();
    VoteDetailStore.load({ params: { start: 0, limit: pageSize } });
});

//function BuyShow(val) {
//    switch (val) {
//        case 1:
//            return "公司型號";
//            break;
//        case 0:
//            return "個人";
//            break;
//        default:
//            return val;
//            break;
//    }
//}
//function DeviceShow(val) {
//    switch (val) {
//        case 1:
//            return "簽回";
//            break;
//        case 0:
//            return "未簽回";
//            break;
//        default:
//            return val;
//            break;
//    }
//}

//function StatusShow(val) {
//    switch (val) {
//        case 1:
//            return "存入資料庫";
//            break;
//        case 2:
//            return "上傳發票機";
//            break;
//        default:
//            return val;
//            break;
//    }
//}
//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Vote/UpdateVoteDetaiStatus",
        data: {
            "vote_id": id,
            "vote_status": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            VoteDetailStore.load();
            //if (activeValue == 1) {
            //    $("#img" + id).attr("hidValue", 0);
            //    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            //} else {
            //    $("#img" + id).attr("hidValue", 1);
            //    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            //}
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}
//新增
onAddClick = function () {
    editFunction(null, VoteDetailStore);
}

//編輯
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    //alert(row[0].data.vote_id);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        //alert(row[0].data);
        editFunction(row[0], VoteDetailStore);
    }
}

// function ToCsv() {
  
//    window.open('/Vote/VoteDetailExportExcel?article_id=' + Ext.getCmp('vote_article').getValue() + '&searchContent=' +Ext.htmlEncode(Ext.getCmp('searchContent').getValue()) + ' &time_start='+Ext.getCmp('time_start').getValue()+'&time_end=' +Ext.getCmp('time_end').getValue()+'&vote_status='+ Ext.htmlEncode(Ext.getCmp("vote_status").getValue()) );
  
//}
