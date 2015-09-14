var pageSize = 25;
var boolPassword = true;//標記是否需要輸入密碼

var SerachStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
       // { "txt": "請選擇", "value": "0" },
        { "txt": "會員姓名", "value": "1" },
        { "txt": "商品名稱", "value": "2" }
    ]
});
var QuestionStatus = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有狀態", "value": "-1" },
        { "txt": "尚未補貨通知", "value": "0" },
        { "txt": "已補貨通知", "value": "1" },
        { "txt": "不再補貨", "value": "2" },
        { "txt": "預計補貨", "value": "3" }
    ]
}); 
Ext.define('gigade.ArrivalNoticeList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "user_id", type: "int" },
          { name: "item_id", type: "uint" },
        { name: "product_id", type: "uint" },
        { name: "status", type: "int" },
        { name: "create_time", type: "uint" },
         { name: "s_create_time", type: "string" },
           { name: "user_name", type: "string" },
             { name: "product_name", type: "string" },
             { name: 'item_stock', type: 'int' },
              { name: "s_coming_time", type: "string" },
                 { name: "user_email", type: "string" },

    ]
});
var ArrivalNoticeListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.ArrivalNoticeList',
    proxy: {
        type: 'ajax',
        url: '/Service/GetArrivalNoticeList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("ArrivalNotice").getSelectionModel().getSelection();
            if (row != "") {
                for (var i = 0; i < row.length; i++) {
                    if (row[i].data.status != 0) {
                        Ext.getCmp("ArrivalNotice").down('#edit').setDisabled(true);
                        Ext.getCmp("ArrivalNotice").down('#nolonger').setDisabled(true);
                        Ext.getCmp("ArrivalNotice").down('#comingg').setDisabled(true);
                        break;
                    }
                    else {
                        Ext.getCmp("ArrivalNotice").down('#edit').setDisabled(selections.length == 0);
                        Ext.getCmp("ArrivalNotice").down('#nolonger').setDisabled(selections.length == 0);
                        Ext.getCmp("ArrivalNotice").down('#comingg').setDisabled(selections.length == 0);
                    }
                }

            }


        }
    }
});
ArrivalNoticeListStore.on('beforeload', function () {
    Ext.apply(ArrivalNoticeListStore.proxy.extraParams,
        {
            condition: Ext.getCmp('condition').getValue(),
            searchCon: Ext.getCmp('searchCon').getValue(),
            status: Ext.getCmp('status').getValue(),
            searchNum: Ext.getCmp('searchNum').getValue()
        });
});
Ext.onReady(function () {
    var ArrivalNotice = Ext.create('Ext.grid.Panel', {
        id: 'ArrivalNotice',
        store: ArrivalNoticeListStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'id', width: 60, align: 'center', hidden: true },
             {
                 header: "會員姓名", dataIndex: 'user_name', width: 90, align: 'center'
                 ,
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     return "<a href='javascript:void(0);' onclick='SecretLogin(" + record.data.id + "," + record.data.user_id + ")'  >" + value + "</a>";

                 }
             },
              { header: '商品名稱', dataIndex: 'product_name', width: 315, align: 'center' },
             {
                 header: '商品庫存', dataIndex: 'item_stock', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (record.data.item_stock <= 0) {
                         return "<font style='color:red;'> " + value + "</font>";
                     }
                     else {
                         return value;
                     }

                 }

             },
             {
                 header: '問題狀態', dataIndex: 'status', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (record.data.status == 0) {
                         return "尚未補貨通知";
                     }
                     else if (record.data.status == 1) {
                         return "已補貨通知";
                     }
                     else if (record.data.status == 2) {
                         return "不再補貨";
                     }
                     else {
                         return "預計補貨";
                     }
                 }
             },
             { header: "追蹤時間", dataIndex: 's_create_time', width: 135, align: 'center' },
            { header: "預計補貨日期", dataIndex: 's_coming_time', width: 135, align: 'center' },
        ],
        tbar: [
            { xtype: 'button', text: '修改通知狀態', id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onIgnoreClick },
           {
               xtype: 'button', text: '不再補貨', id: 'nolonger', hidden: false, iconCls: 'icon-user-edit',
               disabled: true,
               handler: nolongerClick
           },
            {
                xtype: 'button', text: '預計補貨', id: 'comingg', hidden: false, iconCls: 'icon-user-edit',
                disabled: true,
                handler: comingClick
            },

            '->',
            {
                xtype: 'combobox',
                fieldLabel: '查詢條件',
                store: SerachStore,
                id: 'condition',
                valueField: 'value',
                displayField: 'txt',
                labelWidth: 60,
                editable: false,
                margin: '0 5 0 0',
                emptyText:'請選擇...'

            },
            {
                xtype: 'textfield',
                fieldLabel: '查詢內容',
                id: 'searchCon',
                labelWidth: 60,
                margin: '0 5 0 0',
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    },
                    focus: function () {
                        var searchType = Ext.getCmp("condition").getValue();
                        if (searchType == null || searchType == '' || searchType == '0') {
                            Ext.Msg.alert("提示信息", "請先選則搜索類型");
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                value: '商品庫存 >',
                labelWidth: 70
            },
            {
                xtype: 'numberfield',
                id: 'searchNum',
                width: 60,
                value: 0,
                //hideTrigger: true,
                mouseWheelEnabled: true,
                allowDecimals: false,
                allowNegative: false,
                margin: '0 5 0 0',
            },
            {
                xtype: 'combobox',
                fieldLabel: '問題狀態',
                store: QuestionStatus,
                id: 'status',
                valueField: 'value',
                displayField: 'txt',
                value: -1,
                labelWidth: 60,
            },
            {
                xtype: 'button', text: '查詢', handler: Query, iconCls: 'icon-search',
            },
            {
                xtype: 'button', text: '重置', iconCls: 'ui-icon ui-icon-reset', handler: function () {
                    Ext.getCmp('condition').reset();
                    Ext.getCmp('searchCon').setValue('');
                    Ext.getCmp('status').reset();
                    Ext.getCmp('searchNum').reset();
                }
            },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ArrivalNoticeListStore,
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
        layout: 'fit',
        items: [ArrivalNotice],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ArrivalNotice.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // ArrivalNoticeListStore.load({ params: { start: 0, limit: 25 } });

});

//群組管理Model 
Ext.define('gigade.Users', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" }, //用戶編號     上面的是編輯的時候關係到的
        { name: "user_email", type: "string" }, //用戶郵箱
        { name: "user_name", type: "string" }, //用戶名
        { name: "user_password", type: "string" }, //密碼
        { name: "user_gender", type: "string" }, //性別
        { name: "user_birthday_year", type: "string" }, //年
        { name: "user_birthday_month", type: "string" }, //月
        { name: "user_birthday_day", type: "string" }, //日
        { name: "birthday", type: "string" }, //生日 
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_address", type: "string" }, //用戶地址
        { name: "user_mobile", type: "string" },
        { name: "user_phone", type: "string" }, //行動電話
        { name: "reg_date", type: "string" }, //註冊日期 
        { name: "mytype", type: "string" },//會員類別
        { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告 
        { name: "adm_note", type: "string" }, //管理員備註   上面這些編輯時要帶入的值
        { name: "user_type", type: "string" }, //用戶類別   下面的這些結合上面的會顯示在列表頁
        { name: "user_status", type: "string" }, //用戶狀態
        { name: "sfirst_time", type: "string" }, //首次註冊時間
        { name: "slast_time", type: "string" }, //下次時間
        { name: "sbe4_last_time", type: "string" }, //下下次時間
        { name: "user_company_id", type: "string" },
        { name: "user_source", type: "string" },
        { name: "source_trace", type: "string" },
        { name: "s_id", type: "string" },
        { name: "source_trace_url", type: "string" },
        { name: "redirect_name", type: "string" },
        { name: "redirect_url", type: "string" },
        { name: "paper_invoice", type: "bool" }
    ]
});

//用作編輯時獲得數據包含機敏信息
var edit_UserStore = Ext.create('Ext.data.Store', {
    //  autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Users',
    proxy: {
        type: 'ajax',
        url: '/Member/UsersList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});

function SecretLogin(rid, uid) {
    var secret_type = "4";
    var url = "/Service/ArrivalNotice";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            SecretLoginFun(secret_type, uid, true, false, true, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
        } else {
            editFunction(uid, edit_UserStore);
        }
    }
}

//修改
onIgnoreClick = function () {
    var row = Ext.getCmp("ArrivalNotice").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    } else {
        Ext.Msg.confirm("確認信息", Ext.String.format("變更選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "變更狀態中..." });
                myMask.show();
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + "," + row[i].data.user_id + "," + row[i].data.product_name + '∑';
                    if (row[i].data.item_stock < 1) {
                        rowIDs = '';
                        myMask.hide();
                        return Ext.Msg.alert("提示信息", "選中的第" + (i + 1) + "條數據庫存有誤,請檢查庫存數量!");
                    }
                }
                Ext.Ajax.request({
                    url: '/Service/IgnoreNotice',
                    method: 'post',
                    timeout: 5000000,
                    params: {
                        rowID: rowIDs,
                        type: 1,
                    },
                    success: function (form, action) {
                        myMask.hide();
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.msg == 0) {
                                Ext.Msg.alert("提示信息", "變更成功！");
                            }
                            else {
                                Ext.Msg.alert("提示信息", "郵件發送失敗！");
                            }
                        }
                        ArrivalNoticeListStore.load();
                    },
                    failure: function (response, options) {
                        myMask.hide();
                        var reques = response.status;
                        if (reques == "-1") {
                            Ext.Msg.alert("提示信息", "操作超時！");
                        }
                        else {
                            Ext.Msg.alert("提示信息", "變更失敗！");
                        }
                        ArrivalNoticeListStore.load();
                    }
                });
            }
        });
    }
}

function nolongerClick() {
    var row = Ext.getCmp("ArrivalNotice").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {

        nolongerFunction(row);
    }
}
function comingClick() {
    var row = Ext.getCmp("ArrivalNotice").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {

        comingFunction(row);
    }
}

function Query() {
    var condition = Ext.getCmp('condition').getValue();
    var searchCon = Ext.getCmp('searchCon').getValue();
    if (condition != null && searchCon == "") {
        Ext.Msg.alert("提示信息", "請輸入搜索內容");
    }
    else {
        ArrivalNoticeListStore.removeAll();
        Ext.getCmp("ArrivalNotice").store.loadPage(1, {
            params: {
                condition: Ext.getCmp('condition').getValue(),
                searchCon: Ext.getCmp('searchCon').getValue(),
                status: Ext.getCmp('status').getValue(),
                searchNum: Ext.getCmp('searchNum').getValue()
            }
        });
    }
}

comingFunction = function (row) {

    var form = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        items: [
            {
                xtype: 'datetimefield',
                fieldLabel: '預計捕獲日期',
                id: 'coming',
                allowBlank: false,
                format: 'Y-m-d H:i:s',
                margin: "27 10 0 20",
                editable: false,
                value: new Date()
            }
        ],
        buttons: [
            {
                text: '保存',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        save();
                    }
                }
            }
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '預計捕獲日期',
        iconCls: 'icon-user-edit',
        width: 350,
        height: 150,
        layout: 'fit',
        items: [form],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
               {
                   type: 'close',
                   handler: function (event, toolEl, panel) {
                       Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                           if (btn == "yes") {
                               editWin.destroy();
                           }
                           else {
                               return false;
                           }
                       });
                   }
               }]
    });
    editWin.show();

    function save() {
        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "變更狀態中..." });
        var rowIDs = "";
        for (var i = 0; i < row.length; i++) {
            rowIDs += row[i].data.id + "," + row[i].data.user_id + "," + row[i].data.product_name + '∑';
        }
        myMask.show();
        Ext.Ajax.request({
            url: '/Service/NoOrComing',
            method: 'post',
            timeout: 5000000,
            params: {
                rowID: rowIDs,
                type: 3,
                coming_time: Ext.getCmp('coming').getValue()
            },
            success: function (form, action) {
                myMask.hide();
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    if (result.msg == 0) {
                        Ext.Msg.alert("提示信息", "變更成功！");
                    }
                    else {
                        Ext.Msg.alert("提示信息", "郵件發送失敗！");
                    }
                    editWin.close();
                    ArrivalNoticeListStore.load();
                }
                else {
                    myMask.hide();
                    Ext.Msg.alert("提示信息", "保存失敗");
                    editWin.close();
                    ArrivalNoticeListStore.load();
                }
            },
            failure: function (response, options) {
                myMask.hide();
                var reques = response.status;
                if (reques == "-1") {
                    Ext.Msg.alert("提示信息", "操作超時！");
                }
                else {
                    Ext.Msg.alert("提示信息", "變更失敗！");
                }
                ArrivalNoticeListStore.load();
            }
        });
    }
}

nolongerFunction = function (row) {
    var form2 = Ext.create('Ext.form.Panel', {
        id: 'editFrm2',
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '推薦商品網址',
                id: 'recommend',
                margin: "27 10 0 20",
                vtype: 'url',
            }
        ],
        buttons: [
            {
                text: '保存',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        save2();
                    }
                }
            }
        ]
    });
    var editWin2 = Ext.create('Ext.window.Window', {
        title: '預計捕獲日期',
        iconCls: 'icon-user-edit',
        width: 350,
        height: 150,
        layout: 'fit',
        items: [form2],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
               {
                   type: 'close',
                   handler: function (event, toolEl, panel) {
                       Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                           if (btn == "yes") {
                               editWin2.destroy();
                           }
                           else {
                               return false;
                           }
                       });
                   }
               }]
    });
    editWin2.show();
    function save2() {
        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "變更狀態中..." });
        var rowIDs = "";
        for (var i = 0; i < row.length; i++) {
            rowIDs += row[i].data.id + "," + row[i].data.user_id + "," + row[i].data.product_name + '∑';
        }

        myMask.show();
        Ext.Ajax.request({
            url: '/Service/NoOrComing',
            method: 'post',
            timeout: 5000000,
            params: {
                rowID: rowIDs,
                type: 2,
                recommend: Ext.getCmp('recommend').getValue()
            },
            success: function (form, action) {
                myMask.hide();
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    if (result.msg == 0) {
                        Ext.Msg.alert("提示信息", "變更成功！");
                    }
                    else {
                        Ext.Msg.alert("提示信息", "郵件發送失敗！");
                    }
                    editWin2.close();
                    ArrivalNoticeListStore.load();
                }
                else {
                    myMask.hide();
                    Ext.Msg.alert("提示信息", "保存失敗");
                    editWin2.close();
                    ArrivalNoticeListStore.load();
                }
            },
            failure: function (response, options) {
                myMask.hide();
                var reques = response.status;
                if (reques == "-1") {
                    Ext.Msg.alert("提示信息", "操作超時！");
                }
                else {
                    Ext.Msg.alert("提示信息", "變更失敗！");
                }
                ArrivalNoticeListStore.load();
            }
        });
    }

}
