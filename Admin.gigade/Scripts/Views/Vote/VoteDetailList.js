var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "user_name;user_id";
Ext.define('gigade.VoteDetail', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vote_id", type: "int" },
        { name: "article_id", type: "int" },
        { name: "article_title", type: "string" },
        { name: "user_id", type: "int" },
        { name: "ip", type: "string" },
        { name: "vote_status", type: "int" },
        { name: "create_user", type: "int" },
        { name: "update_user", type: "int" },
        { name: "create_time", type: "string" },//user_name
        { name: "user_name", type: "string" },
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
var edit_VoteDetailStore = Ext.create('Ext.data.Store', {
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
        vote_status: Ext.htmlEncode(Ext.getCmp("vote_status").getValue()),
        relation_id: "",
        isSecret: true
    });
});

edit_VoteDetailStore.on('beforeload', function () {
    Ext.apply(edit_VoteDetailStore.proxy.extraParams, {
        article_id: Ext.getCmp('vote_article').getValue(),
        searchContent: Ext.getCmp('searchContent').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue(),
        vote_status: Ext.htmlEncode(Ext.getCmp("vote_status").getValue()),
        relation_id: "",
        isSecret: false
    });
});
function Query(x) {
    if (Ext.getCmp("time_start").getValue() ==null) {
        Ext.Msg.alert("提示信息", "日期條件未選擇！");
        return;
    }
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
                        queryMode: 'remote',//
                        width: 200,
                        labelWidth: 55,
                        margin: '5 10 0 5',
                        displayField: 'article_title',
                        valueField: 'article_id',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        fieldLabel: "文章標題",
                        emptyText: '請選擇文章標題',
                        lastQuery: '',
                        listeners: {
                            beforequery: function (qe) {
                                ArticleStore.load();
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: "文章編號/標題/會員編號",
                        //width: 200,
                        labelWidth: 140,
                        margin: '5 10 0 0',
                        id: 'searchContent',
                        name: 'searchContent',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
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
                          margin: '5 10 0 0',
                          displayField: 'status_name',
                          valueField: 'status_id',
                          typeAhead: true,
                          forceSelection: false,
                          editable: false,
                          fieldLabel: "查詢狀態",
                          emptyText: '請選擇',
                          //value: -1
                      }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                       {
                           xtype: "datetimefield",
                           fieldLabel: "日期條件",
                           labelWidth: 55,
                           margin: '5 0 0 5',
                           id: 'time_start',
                           name: 'time_start',
                           editable: false,
                           allowBlank: false,
                           submitValue: true,
                           format: 'Y-m-d H:i:s',
                           time: { hour: 00, min: 00, sec: 00 },
                           width: 215,
                          // value: Tomorrow(1 - new Date().getDate()),
                           listeners: {
                               select: function () {
                                   var startTime = Ext.getCmp("time_start");
                                   var endTime = Ext.getCmp("time_end");
                                   if (endTime.getValue() == null) {
                                       endTime.setValue(setNextMonth(startTime.getValue(), 1));
                                   }
                                   else if (endTime.getValue() < startTime.getValue()) {
                                       endTime.setValue(setNextMonth(startTime.getValue(), 1));
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
                        margin: '5 0 0 5',
                        value: "~"
                    },
                    {
                        xtype: "datetimefield",
                        id: 'time_end',
                        name: 'time_end',
                        margin: '5 0 0 5',
                         format: 'Y-m-d H:i:s',
                         editable: false,
                         time: { hour: 23, min: 59, sec: 59 },
                        allowBlank: false,
                        submitValue: true,
                        width: 155,
                        //value: Tomorrow(0),
                        listeners: {
                            select: function () {
                                var startTime = Ext.getCmp("time_start");
                                var endTime = Ext.getCmp("time_end");
                                if (startTime.getValue() == null) {
                                    startTime.setValue(setNextMonth(endTime.getValue(), -1));
                                }
                                else if (endTime.getValue() < startTime.getValue()) {
                                    startTime.setValue(setNextMonth(endTime.getValue(), -1));
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
            }
        ],
        buttonAlign: 'left',
        buttons: [
            {
                margin: "0 8 0 8",
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
            { header: "文章標題", dataIndex: 'article_title', width: 200, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', width: 100, align: 'center' },
            {
                header: "會員名稱", dataIndex: 'user_name', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.vote_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";

                }
            },


            { header: "IP", dataIndex: 'ip', width: 120, align: 'center' },
            { header: "創建時間", dataIndex: 'create_time', width: 140, align: 'center' },
            { header: "會員名稱", dataIndex: 'user_name', width: 140, align: 'center', hidden: true },
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
                xtype: 'button',
                text: "匯出",
                id: 'export',
                hidden: false,
                iconCls: 'icon-excel',
                handler: function () {
                    window.open("/Vote/VoteDetailExportExcel?article_id="
                        + Ext.getCmp('vote_article').getValue()
                        + "&searchContent=" + Ext.getCmp('searchContent').getValue()
                        + "&vote_status=" + Ext.getCmp('vote_status').getValue()
                        + "&time_start=" + Ext.Date.format(Ext.getCmp('time_start').getValue(), 'Y-m-d H:i:s')
                        + "&time_end=" + Ext.Date.format(Ext.getCmp('time_end').getValue(), 'Y-m-d H:i:s'));
                }
            }
            //{
            //    xtype: 'button',
            //    id: 'export',
            //    text: '匯出',
            //    iconCls: 'icon-excel',
            //    hidden: true,
            //    handler: Export
            //}
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
    //VoteDetailStore.load({ params: { start: 0, limit: pageSize } });
});
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "11";//參數表中的"活動管理模塊"
    var url = "/Vote/VoteDetail";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口 7:客戶信息類型user:會員 order：訂單 vendor：供應商 8：客戶id9：要顯示的客戶信息
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}

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
    if (s < 0) {
        d.setHours(0, 0, 0);
    }
    else {
        d.setHours(23,59, 59);
    }
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
        var secret_type = "11";//參數表中的"會員查詢列表"
        var url = "/Vote/VoteDetail/Edit ";
        var ralated_id = row[0].data.vote_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id);
            }
        }
       // editFunction(row[0], VoteDetailStore);
    }
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
var Export = function () {
    var article_id = Ext.getCmp('vote_article').getValue();
    var searchContent = Ext.getCmp('searchContent').getValue();
    var time_start = Ext.getCmp('time_start').getValue();
    var time_end = Ext.getCmp('time_end').getValue();
    var vote_status = Ext.htmlEncode(Ext.getCmp("vote_status").getValue());
    if (article_id == null && searchContent == "" && time_start == null && time_end == null && vote_status == null) {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else {
        Ext.MessageBox.show({
            msg: 'Loading....',
            wait: true
        });
        Ext.Ajax.request({
            url: '/Vote/VoteDetailExportExcel',
            timeout: 900000,
            params: {            
                article_id: article_id,
                searchContent: searchContent,
                vote_status: vote_status,
                time_start: time_start,
                time_end: time_end,
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    window.open = '../../ImportUserIOExcel/' + result.ExcelName;
                    Ext.MessageBox.hide();
                } else {
                    Ext.MessageBox.hide();
                    Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據,請先搜索查看!");
                }
            }
        });
    }
}