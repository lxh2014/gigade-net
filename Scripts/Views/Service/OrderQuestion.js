var pageSize = 18;
var boolPassword = true;//secretcopy

Ext.define('gigade.OrderQuestion', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "question_id", type: "int" },
        { name: "order_id", type: "int" },
        { name: "question_username", type: "string" },
        { name: "question_phone", type: "string" },
        { name: "question_email", type: "string" },
        { name: "question_type", type: "int" },
        { name: "question_type_name", type: "string" },//問題分類name
        { name: "question_reply", type: "string" },
        { name: "question_reply_time", type: "int" },
        { name: "question_status", type: "int" },
        { name: "question_status_name", type: "string" },//狀態name
        { name: "question_content", type: "string" },
        { name: "question_ipfrom", type: "string" },
        { name: "question_createdate", type: "int" },
        { name: "question_file", type: "string" },
        { name: "response_createdate", type: "string" },
        { name: "question_createdates", type: "string" },
        { name: "response_createdates", type: "string" }

    ]
}); 
var OrderQuestion = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.OrderQuestion',
    proxy: {
        type: 'ajax',
        url: '/Service/OrderQuestionlist',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
var edit_OrderQuestion = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.OrderQuestion',
    proxy: {
        type: 'ajax',
        url: '/Service/OrderQuestionlist',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
OrderQuestion.on('beforeload', function () {
        Ext.apply(OrderQuestion.proxy.extraParams, {
            ddlSel: Ext.getCmp('ddlSel').getValue(),
            selcontent: Ext.getCmp('selcontent').getValue(),
            ddtSel: Ext.getCmp('ddtSel').getValue(),
            time_start: Ext.getCmp('time_start').getValue(),
            time_end: Ext.getCmp('time_end').getValue(),
            ddrSel: Ext.htmlEncode(Ext.getCmp('ddrSel').getValue()),
            ddlstatus: Ext.htmlEncode(Ext.getCmp("ddlstatus").getValue().Status),
            relation_id: "",
            isSecret: true
        });
});
//下拉框
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有問題列表", "value": "0" },
        { "txt": "問題流水號", "value": "1" },
        { "txt": "付款單號", "value": "2" },
        { "txt": "訂購人姓名", "value": "3" },
        { "txt": "訂購人信箱", "value": "4" }
    ]
});


var DDTStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "建立日期", "value": "1" },
        { "txt": "回覆日期", "value": "2" }
    ]
});
//寫死的下拉數據
var DDR1Store = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "請選擇原因", "value": "0" },
        { "txt": "商品諮詢", "value": "1" },
        { "txt": "訂購問題", "value": "2" },
        { "txt": "付款問題", "value": "3" },
        { "txt": "配送問題", "value": "4" },
        { "txt": "發票問題", "value": "5" },
        { "txt": "退換貨問題", "value": "6" },
        { "txt": "購物金\抵用金問題", "value": "7" },
        { "txt": "其他問題", "value": "8" }
    ]
});


function Query() {
    OrderQuestion.removeAll();
    var ddlSel = Ext.getCmp('ddlSel').getValue();
    var selcontent = Ext.getCmp('selcontent').getValue();
    var ddtSel = Ext.getCmp('ddtSel').getValue();
    var time_start = Ext.getCmp('time_start').getValue();
    var time_end = Ext.getCmp('time_end').getValue();
    var ddrSel = Ext.htmlEncode(Ext.getCmp('ddrSel').getValue());
    var ddlstatus = Ext.htmlEncode(Ext.getCmp("ddlstatus").getValue().Status);
    if (ddlSel != 0 && selcontent == "") {
        Ext.Msg.alert(INFORMATION, '請輸入搜索內容');
    }
    else if (ddlSel == 0 && selcontent != "") {
        Ext.Msg.alert(INFORMATION, '請選擇搜索條件');
        Ext.getCmp('selcontent').reset();
    }
    else if (ddtSel != 0 && (time_start == null || time_end == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇日期');
    }
    else if (ddtSel == 0 && (time_start != null  || time_end != null)) {
        Ext.Msg.alert(INFORMATION, '請選擇日期條件');
        Ext.getCmp('time_start').reset();
        Ext.getCmp('time_end').reset();
    }
    else {       
        Ext.getCmp("gdFgroup").store.loadPage(1);
    }
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
                        id: 'ddlSel',
                        editable: false,
                        fieldLabel: "查詢條件",
                        labelWidth: 60,
                        width: 170,
                        store: DDLStore,
                        queryMode: 'local',
                        submitValue: true,
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        value: '0',
                        forceSelection: false
                    },
                    {
                        xtype: 'textfield',
                        allowBlank: true,
                        fieldLabel: "查詢內容",
                        margin: '0 5px',
                       
                        labelWidth: 60,
                        id: 'selcontent',
                        name: 'searchcontent',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            },
                            focus: function () {
                                var searchType = Ext.getCmp("ddlSel").getValue();
                                if (searchType == null || searchType == '' || searchType == '0') {
                                    Ext.Msg.alert(INFORMATION, '請先選擇搜索類型');
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'ddtSel',
                        margin: '0 5px',
                        fieldLabel: '日期條件',
                        labelWidth: 60,
                        width: 160,
                        margin: '0 5px',
                        colName: 'ddtSel',
                        queryMode: 'local',
                        store: DDTStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: 0
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'time_start',
                        name: 'time_start',
                        margin: '0 5px',
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                        listeners: {
                            focus: function () {
                                var searchType = Ext.getCmp("ddtSel").getValue();
                                if (searchType == null || searchType == '' || searchType == '0') {
                                    Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                                    this.focus = false;
                                }
                            },
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                if (end.getValue() == null) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                else if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, DATA_TIP);
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                                //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                //    Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                //    end.setValue(setNextMonth(start.getValue(), 1));
                                //}
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    { xtype: 'displayfield', value: '~' },
                    {
                        xtype: 'datetimefield',
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5px',
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59
                        listeners: {
                            focus: function () {                  
                                var searchType = Ext.getCmp("ddtSel").getValue();
                                if (searchType == null || searchType == '' || searchType == '0') {
                                    Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                                    this.focus = false;
                                }                             
                            },                           
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                if (start.getValue() != "" && start.getValue() != null) {
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                    //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                    //    start.setValue(setNextMonth(end.getValue(), -1));
                                    //}
                                }
                                else {
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
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
                        xtype: 'combobox',
                        fieldLabel: "問題分類",
                        labelWidth: 60,
                        editable: false,
                        hidden: false,
                        id: 'ddrSel',
                        store: DDR1Store,
                        displayField: 'txt',
                        valueField: 'value',
                        value: '0'
                    },
                    {
                        xtype: 'radiogroup',
                        id: 'ddlstatus',
                        name: 'ddlstatus',
                        fieldLabel: "問題狀態",
                        labelWidth: 60,
                        margin: '0 5px',
                        colName: 'ddlstatus',
                        columns: 4,
                        //vertical: true,
                        layout: 'hbox',
                        width: 500,
                        defaults: {
                            name: 'Status',
                            width: 80,
                        },
                        items: [
                            { boxLabel: '所有狀態', id: 'all', inputValue: '-1' },
                            { boxLabel: '待回覆', id: 'DH', inputValue: '0', checked: true },
                            { boxLabel: '已回覆', id: 'YH', inputValue: '1' },
                            { boxLabel: '已處理(不會寄出通知信)', width: 150, id: 'YY', inputValue: '2' }
                        ]
                    },
                    {
                        xtype: 'button',
                        margin: '0 10 0 0',
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
                                this.up('form').getForm().reset();
                                //Ext.getCmp('ddlstatus').setValue(0);//查詢條件
                                //Ext.getCmp('ddlSel').setValue(0);//查詢條件
                                //Ext.getCmp('selcontent').setValue(null);//查詢條件
                                //Ext.getCmp('ddrSel').setValue(0);//問題分類
                                //Ext.getCmp('ddtSel').setValue(0);//日期條件
                                //Ext.getCmp('time_start').setValue("");//開始時間--time_start--delivery_date
                                //Ext.getCmp('time_end').setValue("");//結束時間--time_end--delivery_date
                                //Ext.getCmp("all").setValue(true);
                                //Ext.getCmp("DH").setValue(false);
                                //Ext.getCmp("YH").setValue(false);
                            }
                        }
                    }
                ]
            }
        ]

    });

    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: OrderQuestion, //
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
            {
                header: "回覆", dataIndex: 'question_status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 2) {
                        return "<img hidValue='0' id='img" + record.data.question_id + "' src='../../../Content/img/icons/ok1.PNG'/ onclick='onEditClick(" + record.data.question_id + "," + record.data.order_id + "," + value + ")'> ";
                    } else {
                        return "<img hidValue='1' id='img" + record.data.question_id + "' src='../../../Content/img/icons/write.PNG'/ onclick='onEditClick(" + record.data.question_id + "," + record.data.order_id + "," + value + ")'>";
                    }
                }
            },
            { header: "問題流水號", dataIndex: 'question_id', hidde: true, width: 100, align: 'center', align: 'center' },
            { header: "問題分類", dataIndex: 'question_type_name', width: 150, align: 'center' },
             {
                 header: "付款單號", dataIndex: 'order_id', width: 100, align: 'center'
                 ,
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                     // return '<a href=javascript:TransToOrder(' + value + ') >' + value + '</a>';
                     return "<a  href='javascript:void(0);' onclick='SecretLogin(" + record.data.question_id + ")'  >" + value + "</span>";

                 }
             },
            {
                header: "姓名", dataIndex: 'question_username', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.question_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "聯絡電話", dataIndex: 'question_phone', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.question_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "電子信箱", dataIndex: 'question_email', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.question_id + ")'  >" + value + "</span>";
                }
            },
           {
               header: "狀態", dataIndex: 'question_status_name', width: 100, align: 'center',
               renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                   if (record.data.question_status == 0) {//已回覆
                       return "<font color='#FF0000'>" + value + "</font>";//注意这里返回的是定义好的css类；列如：(.ppp_ddd_sss div{background-color:red})定义到你页面访问到的css文件里。
                   } else if (record.data.question_status == 1) {
                       // return "<a href='javascript:void(0);' onclick=' UpdateActive(" + record.data.question_id + ")'><img hidValue='2' id='img" + record.data.question_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                       return "<a href='javascript:void(0);' onclick=' UpdateActive(" + record.data.question_id + ")'>已回覆</a>";
                   }
                   else if (record.data.question_status == 2) {
                       return value;
                   }
               }
           },
            { header: "建立日期", dataIndex: 'question_createdates', width: 150, align: 'center' },
            {
                header: "回覆日期", dataIndex: 'response_createdates', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.question_status == 0) {
                        return "~";
                    }
                    else {
                        switch (value) {

                            case "1970-01-01 08:00:00":
                                return "~";
                                break;
                            case "0":
                                return "~";
                                break;
                            default:
                                return value;
                                break;
                        }
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: "匯出", id: 'excel', hidden: false, iconCls: 'icon-excel', handler: CsvClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderQuestion,
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
        layout: 'vbox',//fit
        id: 'orderquestion',
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
    // OrderQuestion.load({ params: { start: 0, limit: 18 } });
});

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

function SecretLogin(rid) {//secretcopy
    var secret_type = "2";//參數表中的"訂單問題列表"
    var url = "/Service/OrderQuestion";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, "", "", "");//直接彈出顯示框
        }
    }
}
/*********************************************編輯***************************************/
onEditClick = function (question_id, order_id, question_status) {
    var secret_type = "2";//參數表中的"會員查詢列表"
    var url = "/Service/OrderQuestionResponse";
    var ralated_id = question_id;
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {
        if (boolPassword) {//驗證
            SecretLoginFun(secret_type, ralated_id, true, false, true, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
        } else {
            var url = '/Service/OrderQuestionResponse?question_id=' + question_id + '&order_id=' + order_id + '&status=' + question_status;
            var panel = window.parent.parent.Ext.getCmp('ContentPanel');
            var copy = panel.down('#detial');
            if (copy) {
                copy.close();
            }
            copy = panel.add({
                id: 'detial',
                title: '回覆訂單問題',
                html: window.top.rtnFrame(url),
                closable: true
            });
            panel.setActiveTab(copy);
            panel.doLayout();
        }
    }
}
function editFunction(rowID) {
    edit_OrderQuestion.load({
        params: { relation_id: rowID },
        callback: function () {
            row = edit_OrderQuestion.getAt(0);
            var url = '/Service/OrderQuestionResponse?question_id=' + row.data.question_id + '&order_id=' + row.data.order_id + '&status=' + row.data.question_status;
            var panel = window.parent.parent.Ext.getCmp('ContentPanel');
            var copy = panel.down('#detial');
            if (copy) {
                copy.close();
            }
            copy = panel.add({
                id: 'detial',
                title: '回覆訂單問題',
                html: window.top.rtnFrame(url),
                closable: true
            });
            panel.setActiveTab(copy);
            panel.doLayout();
        }
    });
}

function CsvClick() {
    window.open('/Service/OrderQuestionExcel?ddlSel=' + Ext.getCmp('ddlSel').getValue() + '&selcontent=' + Ext.getCmp('selcontent').getValue() + '&ddtSel=' + Ext.getCmp('ddtSel').getValue() + '&time_start=' + Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('time_start').getValue()), 'Y-m-d H:i:s')) + '&time_end=' + Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('time_end').getValue()), 'Y-m-d H:i:s')) + '&ddrSel=' + Ext.getCmp('ddrSel').getValue() + '&ddlstatus=' + Ext.htmlEncode(Ext.getCmp("ddlstatus").getValue().Status));
}

//Ext.Date.format(Ext.getCmp('dateStart').getValue(), 'Y-m-d')
function TransToOrder(orderId) {

    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}

function UpdateActive(id) {
    Ext.Msg.confirm("提示信息", "確定將狀態更改為已處理?</br>如果改為已處理狀態將無法再回覆!", function (btn) {
        if (btn == "yes") {
            $.ajax({
                url: "/Service/UpdateOrderQuestionActive",
                data: {
                    "id": id,
                    "active": 2
                },
                type: "POST",
                dataType: "json",
                success: function (msg) {
                    Ext.Msg.alert(INFORMATION, "修改狀態成功!");
                    OrderQuestion.load(1);
                },
                error: function (msg) {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            });
        }
    });
}