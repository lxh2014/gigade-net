
var pageSize = 25;
var boolPassword = true;//標記是否需要輸入密碼

//聯絡客服列表Model
Ext.define('gigade.ContactUs', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "question_id", type: "int" },
          { name: "user_id", type: "int" },
        { name: "question_type", type: "string" },
        { name: "question_type_name", type: "string" },
        { name: "question_company", type: "string" },
        { name: "question_username", type: "string" },
        { name: "question_email", type: "string" },
        { name: "question_phone", type: "string" },
        { name: "question_createdate", type: "string" },
        { name: "question_status", type: "string" },
        { name: "question_status_name", type: "string" },
        { name: "question_content", type: "string" },
        { name: "question_ipfrom", type: "string" },
        { name: "question_problem", type: "string" },
        { name: "question_problem_name", type: "string" },
        { name: "question_reply", type: "string" },
        { name: "question_reply_time", type: "string" }
    ]
}); 
//聯絡客服列表Store
var ContactUsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ContactUs',
    proxy: {
        type: 'ajax',
        url: '/Service/GetContactUsQuestionList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//聯絡客服列表Store
var edit_ContactUsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ContactUs',
    proxy: {
        type: 'ajax',
        url: '/Service/GetContactUsQuestionList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

edit_ContactUsStore.on('beforeload', function () {
    Ext.apply(edit_ContactUsStore.proxy.extraParams, {
        search_type: Ext.getCmp('search_type').getValue(),
        searchcontent: Ext.getCmp('searchcontent').getValue(),
        // date_type: Ext.getCmp('date_type').getValue(),
        dateStart: Ext.getCmp('dateStart').getValue(),
        dateEnd: Ext.getCmp('dateEnd').getValue(),
        qusetion_type: Ext.getCmp('qusetion_type').getValue(),
        radio2: Ext.getCmp('radio2').getValue(),
        radio1: Ext.getCmp('radio1').getValue(),
        radio3: Ext.getCmp('radio3').getValue(),
        radio4: Ext.getCmp('radio4').getValue(),
        relation_id: "",
        isSecret: false
    });
});

var SerachQueryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有問題列表", "value": "0" },
        { "txt": "姓名", "value": "1" },
        { "txt": "信箱", "value": "2" },
        { "txt": "電話", "value": "3" }
    ]
});


//var DateTypeStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [
//        { "txt": "所有日期", "value": "0" },
//        { "txt": "建立日期", "value": "1" }
//    ]
//});
var QuestionStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "類型", "value": "0" },
        { "txt": "聯絡客服", "value": "1" },
        { "txt": "合作提案", "value": "2" },
        { "txt": "海外客服", "value": "3" },
        { "txt": "電話客服", "value": "4" }
    ]
});

ContactUsStore.on('beforeload', function () {
    Ext.apply(ContactUsStore.proxy.extraParams, {
        search_type: Ext.getCmp('search_type').getValue(),
        searchcontent: Ext.getCmp('searchcontent').getValue(),
        // date_type: Ext.getCmp('date_type').getValue(),
        dateStart: Ext.getCmp('dateStart').getValue(),
        dateEnd: Ext.getCmp('dateEnd').getValue(),
        qusetion_type: Ext.getCmp('qusetion_type').getValue(),
        radio2: Ext.getCmp('radio2').getValue(),
        radio1: Ext.getCmp('radio1').getValue(),
        radio3: Ext.getCmp('radio3').getValue(),
        radio4: Ext.getCmp('radio4').getValue(),
        relation_id: "",
        isSecret: true
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("ContactQuestion").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

function Query() {
    ContactUsStore.removeAll();
    var search_type = Ext.getCmp('search_type').getValue();
    var searchcontent = Ext.getCmp('searchcontent').getValue();
    if (search_type != 0 && searchcontent == "") {
        Ext.Msg.alert(INFORMATION, '請輸入查詢內容');
    }
   else if (search_type == 0 && searchcontent != "") {
       Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
       Ext.getCmp('searchcontent').reset();
    }
    else {    
        Ext.getCmp("ContactQuestion").store.loadPage(1);
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
                        name: 'search_type',
                        id: 'search_type',
                        editable: false,
                        fieldLabel: "查詢條件",
                        labelWidth: 60,
                        margin: '0 5px',
                        store: SerachQueryStore,
                        queryMode: 'local',
                        submitValue: true,
                        displayField: 'txt',
                        valueField: 'value',
                        value: 0
                    },
                    {
                        xtype: 'textfield',
                        id: 'searchcontent',
                        name: 'searchcontent',
                        allowBlank: true,
                        fieldLabel: "查詢內容",
                        labelWidth: 60,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            },
                            focus: function () {
                                var searchType = Ext.getCmp("search_type").getValue();
                                if (searchType == null || searchType == '' || searchType == '0') {
                                    Ext.Msg.alert(INFORMATION, '請先選擇查詢條件');
                                }
                            }
                        }
                    },
                     {
                         xtype: "datetimefield",
                         editable: false,
                         fieldLabel: "建立日期",
                         labelWidth: 60,
                         id: 'dateStart',
                         name: 'dateStart',
                         format: 'Y-m-d H:i:s',
                         margin: '0 5 0 5',
                         time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                         listeners: {
                             select: function (a, b, c) {
                                 var start = Ext.getCmp("dateStart");
                                 var end = Ext.getCmp("dateEnd");
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

                     {
                         xtype: 'displayfield',
                         value: '~'
                     },
                         {
                             xtype: "datetimefield",
                             editable: false,
                             id: 'dateEnd',
                             name: 'dateEnd',
                             format: 'Y-m-d H:i:s',
                             margin: '0 5 0 5',
                             time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59
                             listeners: {
                                 select: function (a, b, c) {
                                     var start = Ext.getCmp("dateStart");
                                     var end = Ext.getCmp("dateEnd");
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
            id: 'qusetion_type',
            margin: '0 5px',
            labelWidth: 60,
            fieldLabel: '問題類型',
            queryMode: 'local',
            editable: false,
            store: QuestionStore,
            displayField: 'txt',
            valueField: 'value',
            value: 0
        },
           { xtype: 'displayfield', value: "問題狀況:", labelWidth: 60 },
       {
           xtype: 'radiofield',
           margin: '0 5 0 5',
           boxLabel: '所有狀態',
           labelWidth: 60,
           name: 'diaodu',
           id: 'radio1'
       },
          {
              xtype: 'radiofield',
              boxLabel: '待回覆',
              name: 'diaodu',
              id: 'radio2',
              checked: true
          },
          {
              xtype: 'radiofield',
              boxLabel: '已回覆',
              name: 'diaodu',
              margin: '0 5 0 5',
              id: 'radio3'
          },
           {
               xtype: 'radiofield',
               boxLabel: '已處理(不會寄出通知信)',
               name: 'diaodu',
               margin: '0 5 0 5',
               id: 'radio4'
           },
        {
            xtype: 'button',
            text: "查詢",
            iconCls: 'icon-search',
            margin: '0 10 0 10',
            id: 'btnQuery',
            handler: Query
        },
        {
            xtype: 'button',
            text: "重置",
            id: 'btn_reset',
            iconCls: 'ui-icon ui-icon-reset',
            listeners: {
                click: function () {
                    this.up('form').getForm().reset();
                   
                }
            }
        }
    ]
}
        ]
    });

    var ContactQuestion = Ext.create('Ext.grid.Panel', {
        id: 'ContactQuestion',
        store: ContactUsStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "問題類型", dataIndex: 'question_type_name', width: 150, align: 'center' },
            //{ header: "公司", dataIndex: 'question_company', width: 150, align: 'center' },
            {
                header: "姓名", dataIndex: 'question_username', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.question_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "信箱", dataIndex: 'question_email', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.question_id + ")'  >" + value + "</span>";

                }
            },
            {
                header: "電話", dataIndex: 'question_phone', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.question_id + ")'  >" + value + "</span>";
                }
            },
            { header: "內容", dataIndex: 'question_content', width: 150, align: 'center', flex: 1 },
            { header: "建立日期", dataIndex: 'question_createdate', width: 150, align: 'center' },
            {
                header: "狀態", dataIndex: 'question_status_name', width: 150, align: 'center',
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
            {
                header: "回覆記錄", dataIndex: 'question_status', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != 0) {
                        //return "<img hidValue='0' id='img" + record.data.question_id + "' src='../../../Content/img/icons/ok1.PNG'/ onclick='onEditClick(" + record.data.question_id + "," + record.data.order_id + ")'> ";
                        return "<img hidValue='0' id='reply_ok' src='../../../Content/img/icons/reply.png'/ onclick='TranToMores(" + record.data.question_id + ")'> ";
                    }
                }
            }
        ],
        tbar: [
           {
               xtype: 'button',
               text: '新增',
               id: 'add',
               handler: function () {
                   onAddClick();
               }
           },
           {
               xtype: 'button',
               text: '回覆郵件',
               id: 'edit',
               disabled: true,
               handler: function () {

                   var row = Ext.getCmp("ContactQuestion").getSelectionModel().getSelection();
                   if (row.length == 0) {
                       Ext.Msg.alert(INFORMATION, NO_SELECTION);
                   } else if (row.length > 1) {
                       Ext.Msg.alert(INFORMATION, ONE_SELECTION);
                   } else if (row.length == 1) {
                       if (row[0].data.question_status != 2) {
                           var secret_type = "4";//參數表中的"聯絡客服列表"
                           var url = "/Service/GetContactUsQuestionList/Edit";
                           var ralated_id = row[0].data.question_id;
                           //點擊機敏信息先保存記錄在驗證密碼是否正確
                           boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
                           if (boolPassword != "-1") {
                               if (boolPassword) {//驗證
                                   SecretLoginFun(secret_type, ralated_id, true, false, true, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
                               } else {
                                   editFunction(ralated_id);
                               }
                           }
                       } else {
                           Ext.Msg.alert(INFORMATION, "該條數據為已處理狀態無法回復!");
                       }
                   }



               }
           },
           '->',
           {
               xtype: 'button',              
               text: "匯出",
               iconCls: 'icon-excel',
               id: 'btnExcels',
               handler: function () {
                   var start = 0;
                   var end = 0;
                   if (Ext.getCmp('dateStart').getValue() != null) {
                       start = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('dateStart').getValue()), 'Y-m-d H:i:s'));
                   }
                   if (Ext.getCmp('dateEnd').getValue() != null) {
                       end = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('dateEnd').getValue()), 'Y-m-d H:i:s'));
                   }
                   window.open("/Service/GetContactUsQuestionExcelList?search_type=" + Ext.getCmp('search_type').getValue() + "&searchcontent=" + Ext.getCmp('searchcontent').getValue() + "&qusetion_type=" + Ext.getCmp('qusetion_type').getValue() + "&radio2=" + Ext.getCmp('radio2').getValue() + "&radio1=" + Ext.getCmp('radio1').getValue() + "&radio3=" + Ext.getCmp('radio3').getValue() + "&radio4=" + Ext.getCmp('radio4').getValue() + "&dateStart=" + start + "&dateEnd=" + end);// Ext.Date.format(Ext.getCmp('dateStart').getValue(), 'Y-m-d')
               }
           }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ContactUsStore,
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
        items: [frm, ContactQuestion],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ContactQuestion.width = document.documentElement.clientWidth;
                ContactQuestion.height = document.documentElement.clientHeight - 80;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //  ContactUsStore.load({ params: { start: 0, limit: 25 } });


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
//獲取用戶查詢記錄

function SecretLogin(rid) {//secretcopy
    var secret_type = "4";//參數表中的"聯絡客服列表"
    var url = "/Service/GetContactUsQuestionList";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, "", "", "");//直接彈出顯示框
        }
    }
}

onAddClick = function () {
    //addWin.show();
    addFunction(null, ContactUsStore);
}


TranToMores = function (qid) {
    var record = "回覆記錄";
    var urlTran = "/Service/ContactRecord?qid=" + qid;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#groupDetail');
    if (copy) {
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

function UpdateActive(id) {
    Ext.Msg.confirm("提示信息", "確定將狀態更改為已處理?</br>如果改為已處理狀態將無法再回覆!", function (btn) {
        if (btn == "yes") {
            $.ajax({
                url: "/Service/UpdateActive",
                data: {
                    "id": id,
                    "active": 2
                },
                type: "POST",
                dataType: "json",
                success: function (msg) {
                    Ext.Msg.alert(INFORMATION, "修改狀態成功!");
                    ContactUsStore.load(1);
                },
                error: function (msg) {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            });
        }
    });
}
