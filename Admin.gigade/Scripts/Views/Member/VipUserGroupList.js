
var CallidForm;
var pageSize = 25;
var rows = '';
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
//Ext.apply(Ext.form.field.VTypes, {
//    daterange: function (val, field) {
//        var date = field.parseDate(val);

//        if (!date) {
//            return false;
//        }
//        this.dateRangeMax = null;
//        this.dateRangeMin = null;
//        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
//            var start = field.up('form').down('#' + field.startDateField);
//            start.setMaxValue(date);
//            //start.validate();
//            this.dateRangeMax = date;
//        } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
//            var end = field.up('form').down('#' + field.endDateField);
//            end.setMinValue(date);
//            //end.validate();
//            this.dateRangeMin = date;
//        }
//        /*  
//         * Always return true since we're only using this vtype to set the  
//         * min/max allowed values (these are tested for after the vtype test)  
//         */
//        return true;
//    },

//    daterangeText: '開始時間必須小於結束時間'
//});
Ext.define('gigade.VipUserGroup', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "int" },
        { name: "group_name", type: "string" },
        { name: "tax_id", type: "string" },
        { name: "ip", type: "string" },
        { name: "list", type: "string" },
        { name: "gift_bonus", type: "int" },
        { name: "bonus_rate", type: "int" },
        { name: "bonus_expire_day", type: "int" },
        { name: "screatedate", type: "string" },
        { name: "image_name", type: "string" },
        { name: "eng_name", type: "string" },
        { name: "check_iden", type: "int" },
        { name: "group_category", type: "int" }

    ]
});
Ext.define('gigade.VipUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "v_id", type: "int" },
        { name: "vuser_email", type: "string" },
        { name: "status", type: "int" },
        { name: "emp_id", type: "string" },
        { name: "screatedate", type: "string" },


        { name: "user_id", type: "string" },
        { name: "user_name", type: "string" }, //用戶名
        { name: "user_email", type: "string" },
        { name: "user_password", type: "string" }, //密碼
        { name: "user_gender", type: "string" }, //性別
    //        {name: "user_birthday_year", type: "int" }, //年
    //        {name: "user_birthday_month", type: "int" }, //月
    //        {name: "user_birthday_day", type: "int" }, //日
        { name: "birthday", type: "string" },
        { name: "user_mobile", type: "string" },
        { name: "user_phone", type: "string" }, //行動電話
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_address", type: "string" }, //用戶地址
        { name: "user_phone", type: "string" }, //聯絡電話
        { name: "reg_date", type: "string" }, //註冊日期


        { name: "user_source", type: "string" },
    //        {name: "user_type",type:"string"},
        { name: "mytype", type: "string" }, //用戶類別
        { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告
        { name: "adm_note", type: "string" },//管理員備註
    { name: "paper_invoice", type: "bool" }

    ]
});


var VipUserGroupStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.VipUserGroup',
    proxy: {
        type: 'ajax',
        //url:controller/fangfaming
        url: '/Member/GetVipUserGroupList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("vugGrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("vugGrid").down('#member_Manage').setDisabled(selections.length == 0);
            //            Ext.getCmp("vugGrid").down('#remove').setDisabled(selections.length == 0);
            //            Ext.getCmp("vugGrid").down('#auth').setDisabled(selections.length == 0);
            //            Ext.getCmp("vugGrid").down('#callid').setDisabled(selections.length == 0);
        }
    }
}); 


VipUserGroupStore.on('beforeload', function () {
    Ext.apply(VipUserGroupStore.proxy.extraParams, {
        group_id_or_group_name: Ext.getCmp('group_id_or_group_name').getValue()
        //dateOne: Ext.getCmp('dateOne').getValue(),
        //dateTwo: Ext.getCmp('dateTwo').getValue()
    })
});

var VipUserStore = Ext.create('Ext.data.Store', {
    autoDestroy: false,
    pageSize: pageSize,
    model: 'gigade.VipUser',
    proxy: {
        type: 'ajax',
        url: '/Member/GetVipUserList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    },
    autoLoad: true
});
VipUserStore.on('beforeload', function () {
    Ext.apply(VipUserStore.proxy.extraParams, {
        groupid: Ext.getCmp('vugGrid').getSelectionModel().getSelection()[0].data.group_id
        //dateOne: Ext.getCmp('dateOne').getValue(),
        //dateTwo: Ext.getCmp('dateTwo').getValue()
    })
});
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有會員資料", "value": "0" },
        { "txt": "電子信箱", "value": "1" },
        { "txt": "會員姓名", "value": "2" },
        { "txt": "手機號碼", "value": "3" }
    ]
});
var typeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data:[
        { "txt": "會員郵箱", "value": "1" },
        { "txt": "會員名稱", "value": "2" }        
        ]
})
//var vipUserTpl = new Ext.XTemplate(
//    '<a href="/VipUserGroup/VipUserGroupAddList?id={group_id}">{list}</a>'
//);

Ext.onReady(function () {
   
    var vugGrid = Ext.create('Ext.grid.Panel', {
        id: 'vugGrid',
        store: VipUserGroupStore,
        width: document.documentElement.clientWidth,
        columnLines: true,       
        frame: true,
        columns: [
            {
                header: "群組編號", dataIndex: 'group_id', width: 60, align: 'center'
            },
            { header: "群組名稱", dataIndex: 'group_name', width: 150, align: 'center' },
            { header: "統編", dataIndex: 'tax_id', width: 100, align: 'center' },
            {
                header: "企業網址", dataIndex: 'ip', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.tax_id != '' && record.data.eng_name != '' && record.data.group_category != 0) {
                        if (value != '') {
                            if (value.split('.')[0] == '192') {
                                return Ext.String.format('<a href="http://www.gimg.tw/btoe/{0}" target="bank">聯結</a>', record.data.eng_name);
                            }
                            else {
                                return Ext.String.format('<a href="http://www.gigade100.com/btoe/{0}" target="bank">聯結</a>', record.data.eng_name);

                            }
                        }
                    }
                    else {
                        return Ext.String.format('<a href="/Member/VipUserGroupList" target="_self">聯結</a>');
                    }
                }
            },
            
            {
                header: "企業圖片",
                dataIndex: 'image_name',
                width: 100,
                align: 'center',
                xtype: 'templatecolumn',              
                tpl: '<img name="tplImg" height=30 width=50 border=0 src="{image_name}" />'
            },
            {
                header: "群組人數", id: 'count', hidden: true, dataIndex: 'list', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format('<a href="/Member/VipUserList?id={0}" target="_self">{1}</a>', record.data.group_id, value);
                }
            },
            {
                header: "其它功能", dataIndex: '', id: 'other', width: 200, align: 'center', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.check_iden == 0) {
                        return Ext.String.format('<a href="/Member/VipUserImport?id={0}&name={1}" target="_self">匯入</a>', record.data.group_id, record.data.group_name);
                    }
                    if (record.data.check_iden == 1) {
                        return Ext.String.format('<a href="/Member/VipUserImport?id={0}&name={1}" target="_self">匯入</a>&nbsp;&nbsp;&nbsp;<a href="/Member/VipUserImport?id={0}&name={1}&check={2}" target="_self">匯入員工編號</a>', record.data.group_id, record.data.group_name, record.data.check_iden);
                    }


                }
            },
            { header: "註冊贈送購物金額", dataIndex: 'gift_bonus', width: 110, align: 'center' },
            { header: "購物贈送購物金倍率", dataIndex: 'bonus_rate', width: 150, align: 'center' },
            { header: "購物贈送購物金有效天數", dataIndex: 'bonus_expire_day', width: 150, align: 'center' },
            { header: "建立時間", dataIndex: 'screatedate', width: 180, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', hidden: true, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           { xtype: 'button', text: '人員管理', id: 'member_Manage', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: memberManage },
           '->',
          //{
          //    xtype: "datefield",
          //    fieldLabel: "建立時間",
          //    id: 'dateOne',
          //    name: 'dateOne',
          //    format: 'Y-m-d',
          //    allowBlank: false,
          //    editable: false,             
          //    submitValue: true,
          //    value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
          //    listeners: {
          //        select: function (a, b, c) {
          //            var start = Ext.getCmp("dateOne");
          //            var end = Ext.getCmp("dateTwo");
          //            if (end.getValue() == null) {
          //                end.setValue(setNextMonth(start.getValue(), 1));
          //            }
          //            else if (end.getValue() < start.getValue()) {
          //                Ext.Msg.alert(INFORMATION, DATA_TIP);
          //                end.setValue(setNextMonth(start.getValue(), 1));
          //            }
          //            else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
          //                // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
          //                end.setValue(setNextMonth(start.getValue(), 1));
          //            }
          //        }
          //    }
          //}, '~', {
          //    xtype: "datefield",
          //    format: 'Y-m-d',
          //    id: 'dateTwo',
          //    name: 'dateTwo',
          //    allowBlank: false,
          //    editable: false,            
          //    submitValue: true,
          //    value: Tomorrow(),
          //    listeners: {
          //        select: function (a, b, c) {
          //            var start = Ext.getCmp("dateOne");
          //            var end = Ext.getCmp("dateTwo");
          //            if (start.getValue() != "" && start.getValue() != null) {
          //                if (end.getValue() < start.getValue()) {
          //                    Ext.Msg.alert(INFORMATION, DATA_TIP);
          //                    start.setValue(setNextMonth(end.getValue(), -1));
          //                }
          //                else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
          //                    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
          //                    start.setValue(setNextMonth(end.getValue(), -1));
          //                }
          //            }
          //            else {
          //                start.setValue(setNextMonth(end.getValue(), -1));
          //            }
          //        }
          //    }
          //},
          {
              xtype: "textfield",
              fieldLabel: "群組編號/名稱",
              id: 'group_id_or_group_name',
              name: 'group_id_or_group_name',
              allowBlank: false,                           
              submitValue: true,
              emptyText: '請輸入群組編號/名稱',
              listeners: {
                  specialkey: function (field, e) {
                      if (e.getKey() == e.ENTER) {
                          vugQuery();
                      }
                  }
              }
          },
          
          {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: vugQuery,
                listeners: {
                    onClick: function () {
                        if (Ext.getCmp('group_id_or_group_name')=='') {
                            Ext.Msg.alert('提示信息', '請輸入查詢條件');
                        }
                    }

                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VipUserGroupStore,
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
        items: [vugGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                vugGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // VipUserGroupStore.load({ params: { start: 0, limit: 25 } });
    if (document.getElementById("valet_service").value == 1) {
        Ext.getCmp('other').show();
        Ext.getCmp('add').show();
    }
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //addWin.show();
    editFunction(null, VipUserGroupStore);

}


/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("vugGrid").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], VipUserGroupStore);
    }
}

/*********************************************************************************人員管理*************************************************************************************************/

memberManage = function () {
    var row = Ext.getCmp("vugGrid").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        
        var mmGrid = Ext.create('Ext.grid.Panel', {
            id: 'mmGrid',
            store: VipUserStore,
            width: 800,
            columnLines: true,
            frame: true,
            columns: [
                { header: "會員編號", dataIndex: 'v_id', width: 80, align: 'center' },
                { header: "會員名稱", dataIndex: 'user_name', width: 100, align: 'center' },
                { header: "會員郵箱", dataIndex: 'vuser_email', width: 250, align: 'center' },
                
                { header: "建立時間", dataIndex: 'screatedate', width: 150, align: 'center' },

                {
                    header: "操作", dataIndex: 'v_id', width: 90, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.v_id + ")'>刪除</a>";
                    }

                }
            ],
            tbar: [
                { xtype: 'button', iconCls: 'icon-user-add', text: '添加', id: 'add', handler: onAddUserClick },
                //{ xtype: 'button', text: '編輯', id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
                //{ xtype: 'button', text: '刪除', id: 'remove', iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
                '->',
                {
                    xtype: 'combobox',
                    editable: false,
                    margin: "0 5 0 0",
                    fieldLabel: '查詢類別', 
                    labelWidth: 60,
                    id: 'searchtype',
                    store: typeStore,
                    queryMode: 'local',
                    submitValue: true,
                    displayField: 'txt',
                    valueField: 'value',
                    emptyText: '請選擇',
                    value: 0,
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                mmQuery();
                            }
                        },
                        change: function () {
                            if (Ext.getCmp("searchtype").getValue() != 0) {
                                if (Ext.getCmp("searchcontent").getValue() == '') {
                                    Ext.getCmp('mmQueryBtn').setDisabled(true);//不可用
                                }
                                Ext.getCmp('searchcontent').allowBlank = false;                             
                                Ext.getCmp('resetBtn').setDisabled(false);//不可用
                                //Ext.getCmp('searchcontent').setHidden(false);
                            }
                        }
                    }
                },
                    {
                        xtype: 'textfield',
                        margin: "0 5 0 0",
                        fieldLabel: '查詢內容',
                        labelWidth: 60,
                        id: 'searchcontent',
                        name: 'searchcontent',
                        emptyText: '請輸入查詢內容',
                        allowBlank: true,
                        //hidden: true,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    mmQuery();
                                }
                            },
                            change: function () {
                                if (Ext.getCmp("searchcontent").getValue() != '') {
                                    Ext.getCmp('mmQueryBtn').setDisabled(false);//可用
                                    
                                }
                            }
                        }
                    },
                    { xtype: 'button', text: '查詢', id: 'mmQueryBtn', iconCls: 'icon-search', disabled: false, handler: mmQuery },
                    {
                        xtype: 'button', text: '重置', id: 'resetBtn', iconCls: 'ui-icon ui-icon-reset', disabled: true, handler: function () {
                            Ext.getCmp('searchtype').reset();
                            Ext.getCmp('searchcontent').reset();
                            Ext.getCmp('mmQueryBtn').setDisabled(false);

                        }
                    },
               
            ],
            bbar: Ext.create('Ext.PagingToolbar', {
                store: VipUserStore,
                pageSize: pageSize,
                displayInfo: true,
                displayMsg: NOW_DISPLAY_RECORD + ':{0}-{1}' + TOTAL + ':{2}',
                emptyMsg: NOTHING_DISPLAY
            }),
            listeners: {
                scrollershow: function (scroller) {// scroller滚动条
                    if (scroller && scroller.scrollEl) {
                        scroller.clearManagedListeners();
                        scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);

                    }
                }
            },
            //selModel: sm

        });
        var mmWin = Ext.create('Ext.window.Window', {
            title: "人員管理",
            id: "mmWin",
            iconCls: "icon-user-edit",
            width: 800,
            height: 500,
            layout: 'fit',//'fit',
            items: [mmGrid],
            constrain: true,
            closeAction: 'destroy',
            modal: true,
            resizable: false,
            labelWidth: 80,
            //bodyStyle: 'padding:5px 5px 5px 5px',
            closable: false,
            tools: [
                {
                    type: 'close',
                    qtip: "關閉窗口",
                    handler: function (event, toolEl, panel) {
                        Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                            if (btn == "yes") {
                                Ext.getCmp('mmWin').destroy();
                                VipUserStore.load();
                            }
                            else {
                                return false;
                               
                            }
                        });
                    }
                }
            ],
            listeners: {
                'show': function () {
                    Ext.getCmp("mmGrid").store.loadPage(1, {
                        params: {

                        }
                    });
                }
            }
        });
        mmWin.show();
    }
}

//function Tomorrow() {
//    var d;
//    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
//    d.setDate(d.getDate() + 1);
//    return d;
//}
//setNextMonth = function (source, n) {
//    var s = new Date(source);
//    s.setMonth(s.getMonth() + n);
//    if (n < 0) {
//        s.setHours(0, 0, 0);
//    }
//    else if (n > 0) {
//        s.setHours(23, 59, 59);
//    }
//    return s;
//}
/*********************************會員群組查詢****************************************************/
function vugQuery() {
    if (Ext.getCmp('group_id_or_group_name').getValue() == '') {
        Ext.Msg.alert('提示信息', '請輸入查詢條件');
    }
    else {
        if (Ext.getCmp('group_id_or_group_name').getValue() == 0) {
            Ext.Msg.alert('提示信息', '請輸入非零字符');
        }
        else {
            VipUserGroupStore.removeAll();
            Ext.getCmp("vugGrid").store.loadPage(1, {
                params: {
                    group_id_or_group_name: Ext.getCmp('group_id_or_group_name').getValue()
                    //dateOne: Ext.getCmp('dateOne').getValue(),
                    //dateTwo: Ext.getCmp('dateTwo').getValue()
                }
            });
        }   
    }    
}
/*********************************群組人員信息中查詢會員****************************************************/
function mmQuery() {
    if (!Ext.getCmp('searchtype').getValue() == 0) {
        var searchType = Ext.getCmp('searchtype').getValue();
        if (Ext.getCmp('searchcontent').getValue().trim() == '') {
            Ext.Msg.alert('提示信息', '請輸入查詢內容');
        }
        else {
            VipUserStore.removeAll();
            Ext.getCmp("mmGrid").store.loadPage(1, {
                params: {
                    serchs: Ext.getCmp('searchtype').getValue(),
                    serchcontent: Ext.getCmp('searchcontent').getValue().trim(),
                }
            });
        }
    }
    else {
        if (Ext.getCmp('searchcontent').getValue().trim() == '') {
            VipUserStore.removeAll();
            Ext.getCmp("mmGrid").store.loadPage(1, {
                params: {
                    serchcontent: ''
                }
            });
        }
        else {
            Ext.Msg.alert('提示信息','請輸入查詢類別')
        }
       
    }
}
/*********************************群組中新增會員****************************************************/

onAddUserClick = function () {
    rows = Ext.getCmp("vugGrid").getSelectionModel().getSelection();
    var id = rows[0].data.group_id;
    addFunction(id, VipUserStore);
}

/*************************群組中會員刪除****************************/
function UpdateActive(id) {
    Ext.MessageBox.confirm('確認框', '你確定從此群組中刪除此會員嗎', function (btn) {//Ext对大小写敏感
        //Ext.MessageBox.alert('提示框', '你刚刚点击了' + btn);
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: "/Member/DeleVipUser",
                params: {
                    vid: id
                },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    if (result.success) {
                        Ext.Msg.alert("提示", "刪除成功!");
                        VipUserStore.load();
                    } else {
                        Ext.Msg.alert("提示", "刪除失敗!");
                        VipUserStore.load();
                    }
                },
                failure: function (form, action) {
                    Ext.Msg.alert(INFORMATION, "系統出現錯誤!");
                }
            });
        };
    });
};


