Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
'Ext.form.Panel',
'Ext.ux.form.MultiSelect',
'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//打開頁面保存機敏資料查詢記錄

//回覆記錄的store
Ext.define('GIGADE.ReplayRecords', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'question_content', type: 'string' },//提問內容
    { name: 'question_createdate', type: 'string' },//提問時間
    { name: 'response_content', type: 'string' },//回覆內容
    { name: 'response_createdate', type: 'string' },//回覆時間
    { name: 'user_username', type: 'string' },//回覆人員
    { name: 'response_type', type: 'int' }//回覆人員
    ]
});

//獲取grid中的數據
var replayRecordsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.ReplayRecords',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Service/GetOrderQuestionResponseList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});


Ext.onReady(function () {
    var topHeight = 250;//grid的高度
    var questionID = GetQuestionId();//獲取訂單問題列表頁傳輸來的問題編號參數
    var orderID = GetOrderId();//獲取訂單問題列表頁傳輸來的訂單編號參數

    var userDetail = {
        xtype: 'fieldset',
        height: 80,
        width: document.documentElement.clientWidth - 50,
        title: '客戶詳情',
        defaults: {
            labelWidth: 70,
            layout: {
                type: 'hbox'
            }
        },
        items: [
            {
                //提問人信息
                xtype: 'fieldcontainer',
                combineErrors: true,
                flex: 1,
                items: [
                {
                    xtype: 'displayfield',
                    fieldLabel: '姓名',
                    id: 'user_name',
                    name: 'user_name',
                    width: 400
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '聯絡電話',
                    id: 'user_phone',
                    name: 'user_phone',
                    width: 400
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '電子郵件',
                    id: 'user_email',
                    name: 'user_email',
                    width: 400
                }
                ]
            },
            {
                //提問人信息
                xtype: 'fieldcontainer',
                combineErrors: true,
                flex: 1,
                items: [
                    {
                        xtype: 'displayfield',
                        fieldLabel: '建立日期',
                        id: 'question_create',
                        name: 'question_create',
                        width: 400
                    },
                     {
                         xtype: 'displayfield',
                         fieldLabel: '留言內容',
                         id: 'question_con',
                         width: 600,
                         name: 'question_con'
                     }

                ]
            }

        ]
    };
    replayRecordsStore.on('beforeload', function () {
        Ext.apply(replayRecordsStore.proxy.extraParams, {
            question_id: Ext.htmlEncode(GetQuestionId())
        });
    });
    //回覆訂單問題和意見
    var replayAction = Ext.create('Ext.form.Panel', {
        //layout: 'anchor',
        id: 'replyForm',
        // border: false,
        border: '0 0 0 0 ',
        flex: 1.8,
        height: 500,
        width: document.documentElement.clientWidth - 50,
        //url: '/Service/ReplyQuestion',
        autoScroll: true,
        items: [
        {
            xtype: 'fieldset',
            title: '回覆訂單問題和意見',
            //height: 400,
            items: [
            {
                //問題信息
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: 'displayfield',
                    fieldLabel: '建立日期',
                    id: 'question_createdate',
                    name: 'question_createdate',
                    width: 400
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '問題分類',
                    id: 'question_type',
                    name: 'question_type',
                    width: 400
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '付款單號',
                    id: 'order_id',
                    name: 'order_id',
                    width: 400
                }
                ]
            },
            {
                //提問人信息
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: 'displayfield',
                    fieldLabel: '姓名',
                    id: 'question_username',
                    name: 'question_username',
                    width: 400,
                    listeners: {
                        'click': function () {
                        }
                    }
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '聯絡電話',
                    id: 'question_phone',
                    name: 'question_phone',
                    width: 400
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '電子郵件',
                    id: 'question_email',
                    name: 'question_email',
                    width: 400
                },
                 {
                     xtype: 'displayfield',
                     fieldLabel: '電子郵件',
                     id: 'this_question_email',
                     name: 'this_question_email',
                     width: 400,
                     hidden: true
                 }
                ]
            },
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'question_status',
                name: 'question_status',
                hidden: true,
                fieldLabel: "狀態",
                colName: 'question_status',
                width: 600,
                defaults: {
                    name: 'Question',
                    margin: '0 8 0 0'
                },
                columns: 3,
                vertical: true,
                items: [
                { boxLabel: "未結案", id: 'qs1', inputValue: '0', checked: true },
                { boxLabel: "結案", id: 'qs2', inputValue: '1' },
                { boxLabel: "已結案(不會寄出通知信)", id: 'qs3', inputValue: '2' }
                ]
            },
            {
                xtype: 'checkboxgroup',
                fieldLabel: '回覆方式',
                columns: 3,
                id: 'reply',
                vertical: true,
                width: 600,
                readOnly: true,
                style: 'color:#FFFEFD;background:black;',
                items: [
                { boxLabel: 'E-mail', id: 'reply_mail', inputValue: '1' },
                { boxLabel: '簡訊', id: 'reply_sms', inputValue: '2' },
                {
                    boxLabel: '電話', id: 'reply_phone', inputValue: '3',
                    listeners: {
                        change: function (check, newValue, oldValue) {
                            var obj = Ext.getCmp("phone_time");
                            if (newValue) {
                                obj.setDisabled(false);
                            } else {
                                obj.setDisabled(true);
                            }
                        }
                    }
                }
                ]
            },
            {
                xtype: 'radiogroup',
                id: 'phone_time',
                name: 'phone_time',
                disabled: true,
                fieldLabel: "希望回覆時間",
                colName: 'phone_time',
                style: 'color:#FFFEFD;background:black;',
                width: 600,
                defaults: {
                    name: 'phone_time',
                    margin: '0 8 0 0'
                },
                columns: 3,
                vertical: true,
                items: [
                { boxLabel: "上午時段：9點-12點", id: 'pt1', inputValue: '1', checked: true },
                { boxLabel: "下午時段：2點-6點", id: 'pt2', inputValue: '2' },
                { boxLabel: "不限時段", id: 'pt3', inputValue: '3' }
                ]
            },
            {
                xtype: 'displayfield',
                fieldLabel: '留言內容',
                id: 'question_content',
                width: 600,
                name: 'question_content'
            },
            //{
            //    xtype: 'displayfield',
            //    value: "<a href='/Service/DownLoadQuestionFile?fileName=" + fileName + ">" + fileName + "</a>"
            //},
            //{
            //    xtype: 'displayfield',
            //    fieldLabel: '問題文件',
            //    //value: "<a href='/Service/DownLoadQuestionFile?fileName=fileName'>fileName</a>"
            //    value: "<a href='http://admin.gimg.tw:88/service/order_question_response.php?op=down&fname=123.png'>fileName</a>"
            //},
            {
                xtype: 'textareafield',
                fieldLabel: '回覆',
                width: 600,
                height: 200,
                id: 'response_content',
                name: 'response_content'
            },
            {
                //問題信息
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: 'button',
                    text: '回覆送出',
                    width: 60,
                    id: 'submit',
                    margin: '0 10 0 10',
                    //handler: SubmitReply
                    handler: function () {
                        var Response_content = Ext.getCmp("response_content").getValue();
                        if (Response_content == "") {
                            Ext.Msg.alert(INFORMATION, "回覆內容未填寫");
                            return;
                        } else {
                            if (Ext.getCmp("question_status").getValue().Question == 1) {
                                Ext.MessageBox.confirm(CONFIRM, IS_END, function (btn) {
                                    if (btn == "yes") {
                                        SubmitReply();
                                    }
                                    else {
                                        return false;
                                    }
                                });
                            } else if (Ext.getCmp("question_status").getValue().Question == 2) {
                                Ext.MessageBox.confirm(CONFIRM, IS_END_Y, function (btn) {
                                    if (btn == "yes") {
                                        SubmitReply();
                                    }
                                    else {
                                        return false;
                                    }
                                });
                            } else {
                                SubmitReply();
                            }
                        }
                    }
                },
                {
                    xtype: 'button',
                    text: '清空',
                    width: 60,
                    disabled: true,
                    id: 'reset',
                    margin: '0 10 0 10',
                    handler: function () {
                        Ext.getCmp("response_content").setValue("");
                        Ext.getCmp('submit').setDisabled(false);
                    }
                },
                {
                    xtype: 'button',
                    text: '回上頁',
                    width: 60,
                    id: 'return',
                    margin: '0 10 0 10',
                    handler: function () {
                        window.parent.parent.Ext.getCmp('ContentPanel').down('#detial').close();
                    }
                }
                ]
            }
            ]
        }
        ],
        listeners: {
            beforerender: function () {
                beforeReplayAction();
            }
        }
    });

    function beforeReplayAction() {
        Ext.Ajax.request({
            url: '/Service/OrderQuestionlist',
            method: 'post',
            params: {
                ddlSel: "1",//給查詢question_id 加的條件
                selcontent: Ext.htmlEncode(document.getElementById("question_id").value),
                relation_id: "",
                isSecret: false
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.getCmp('question_createdate').setValue(result.data[0].question_createdates);
                    Ext.getCmp('question_create').setValue(result.data[0].question_createdates);
                    Ext.getCmp('question_type').setValue(result.data[0].question_type_name);
                    Ext.getCmp('order_id').setValue('<a href=javascript:TransToOrder(\"' + result.data[0].order_id + '\") >' + result.data[0].order_id + '</a>');
                    Ext.getCmp('question_username').setValue(result.data[0].question_username);
                    Ext.getCmp('question_phone').setValue(result.data[0].question_phone);
                    Ext.getCmp('question_email').setValue('<a href=javascript:TransToUser(\"' + result.data[0].question_email + '\") >' + result.data[0].question_email + '</a>');
                    Ext.getCmp('this_question_email').setValue(result.data[0].question_email);
                    Ext.getCmp('user_name').setValue(result.data[0].question_username);
                    Ext.getCmp('user_phone').setValue(result.data[0].question_phone);
                    Ext.getCmp('user_email').setValue('<a href=javascript:TransToUser(\"' + result.data[0].question_email + '\") >' + result.data[0].question_email + '</a>');
                    Ext.getCmp('reply').disable(true);  //初始化時將回覆方式設置為不可用
                    var status = result.data[0].question_status;
                    if (status == 2) {
                        Ext.getCmp("qs1").setValue(false);
                        Ext.getCmp("qs2").setValue(true);
                        //如果問題狀態為結案則設置"回覆送出"按鈕不可用
                        Ext.getCmp("submit").disable(true);

                    } else {
                        Ext.getCmp("qs1").setValue(true);
                        Ext.getCmp("qs2").setValue(false);
                    }
                    var reply = result.data[0].question_reply;
                    if (reply.substring(0, 1) == 1) {
                        Ext.getCmp("reply_mail").setValue(true);
                    }
                    if (reply.substring(2, 3) == 1) {
                        Ext.getCmp("reply_sms").setValue(true);
                    }
                    if (reply.substring(4, 5) == 1) {
                        Ext.getCmp("reply_phone").setValue(true);
                    }
                    var content = "";
                    if (result.data[0].question_content.length > 44) {
                        content = result.data[0].question_content.substring(0, 40) + "...";
                    } else {
                        content = result.data[0].question_content;
                    }
                    Ext.getCmp('question_content').setValue(result.data[0].question_content);
                    Ext.getCmp('question_con').setValue(content);

                    //Ext.getCmp("reply").setDisabled(true);
                    Ext.getCmp("phone_time").setDisabled(true);
                } else {
                    Ext.Msg.alert("數據故障!");
                }
            },
            failure: function () {

            }
        });
    }
    //顯示回覆記錄
    var replayRecordsGrid = new Ext.grid.Panel({
        id: 'replayRecordsGrid',
        store: replayRecordsStore,
        region: 'center',
        autoScroll: true,
        border: 0,
        height: topHeight,
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
        { header: '提問內容', dataIndex: 'question_content', width: 800, align: 'center', hidden: true },
        { header: '提問時間', dataIndex: 'question_createdate', width: 140, align: 'center', format: 'Y-m-d H:i:s', hidden: true },
        { header: '回覆內容', dataIndex: 'response_content', width: 800, align: 'center' },
        {
            header: '回覆方式', dataIndex: 'response_type', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                switch (value) {
                    case 1:
                        return "E-mail";
                        break;
                    case 2:
                        return "簡訊";
                        break;
                    case 3:
                        return "電話";
                        break;
                    case 4:
                        return "無";
                        break;
                    default:
                        break;
                }
            }
        },
        { header: '回覆時間', dataIndex: 'response_createdate', width: 200, align: 'center', format: 'Y-m-d H:i:s' },
        { header: '回覆人員', dataIndex: 'user_username', width: 120, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: replayRecordsStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            itemdblclick: function (dataview, record, item, index, e) {
                replayAction.getForm().loadRecord(record);
                Ext.getCmp('submit').setDisabled(true);  //初始化時將回覆方式設置為不可用
                if (GetQuestionStatus() != 2) {
                    Ext.getCmp('reset').setDisabled(false);
                }
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }, beforerender: function () {
                if (GetQuestionStatus() == 2) {
                    Ext.getCmp('reset').setDisabled(true);
                }
            }
        }
    });
    var total = Ext.create('Ext.form.Panel', {
        id: 'total',
        border: false,
        autoScroll: true,
        items: [userDetail, replayRecordsGrid, replayAction]
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [total],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                replayRecordsGrid.width = document.documentElement.clientWidth - 30;
                total.height = document.documentElement.clientHeight - 20;
                total.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    QueryToolAuthorityByUrl('/Service/OrderQuestionResponse');
    replayRecordsStore.load({ params: { start: 0, limit: pageSize, question_id: Ext.htmlEncode(GetQuestionId()) } });
});

//獲取要顯示的付款單號
function GetOrderId() {
    return document.getElementById('order_id').value;
}

//查看商品詳情資料時提供pid
function GetQuestionId() {
    return document.getElementById('question_id').value;
}

//查看商品詳情資料時提供pid
function GetQuestionStatus() {
    return document.getElementById('status').value;
}

function SubmitReply() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "提交回覆中,請稍後..." });
    myMask.show();
    Ext.Ajax.request({
        url: '/Service/ReplyQuestion',
        method: 'post',
        params: {
            question_id: Ext.htmlEncode(GetQuestionId()),
            status: Ext.htmlEncode(Ext.getCmp("question_status").getValue().Question),
            response_content: Ext.getCmp("response_content").getValue(),
            question_createdate: Ext.getCmp("question_createdate").getValue(),
            question_username: Ext.getCmp("question_username").getValue(),
            //question_content: Ext.getCmp("question_content").getValue(),
            phone_time: Ext.getCmp("phone_time").getValue().phone_time,
            reply_mail: Ext.getCmp("reply_mail").getValue(),
            reply_sms: Ext.getCmp("reply_sms").getValue(),
            reply_phone: Ext.getCmp("reply_phone").getValue(),
            this_question_email: Ext.getCmp("this_question_email").getValue(),
            question_phone: Ext.getCmp("question_phone").getValue(),
            order_id: GetOrderId()
        },
        success: function (form, action) {
            myMask.hide();
            var result = Ext.decode(form.responseText);
            if (result.success) {
                //Ext.getCmp("response_content").setValue("");
                replayRecordsStore.load({ params: { start: 0, limit: pageSize, question_id: Ext.htmlEncode(GetQuestionId()) } });

                //如果'狀態'選擇'已回覆'或'已處理'則點擊'回覆送出'后進行頁面跳轉
                var question_status_Value = Ext.getCmp('question_status').getValue().Question;
                if (question_status_Value == 1 || question_status_Value == 2) { //已回覆或已處理
                    Ext.Msg.confirm("提示信息", "回覆內容已送出!是否進行頁面跳轉?", function (btn) {
                        if (btn == "yes") {
                            var panel = window.parent.parent.Ext.getCmp('ContentPanel');
                            var copy = panel.down('#detial');
                            if (copy) {
                                copy.close();
                            }
                        } else {
                            Ext.getCmp("submit").disable(true);
                        }
                    });
                } else {
                    Ext.Msg.alert(INFORMATION, "回覆內容已送出!");
                    Ext.getCmp("response_content").setValue(null);
                }
                // OrderQuestion.load();
            } else {
                Ext.Msg.alert(INFORMATION, "系統錯誤,請稍后再試!");
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, "系統錯誤,請稍后再試或聯繫我們!");
        }
    });


}
function TransToUser(user_email) {
    var url = '/Member/UsersListIndex?UserEmail=' + user_email;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#user_detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'user_detial',
        title: '會員內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
function TransToOrder(orderId) {

    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#orderdetial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'orderdetial',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
