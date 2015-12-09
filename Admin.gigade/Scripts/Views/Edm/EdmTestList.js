pageSize = 25;
var info_type = "edm_test";//資安數據來源的表
var secret_info = "user_id;user_name;user_email";//grid 列表頁顯示數據的列名
//查詢條件Store
var selectStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
            { 'txt': '電子郵件', 'value': '0' },
            { 'txt': '姓名', 'value': '1' },
    ]

});
//日期條件
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
            { 'txt': '全部日期', 'value': '0' },
            { 'txt': '建立時間', 'value': '1' },
            { 'txt': '更新時間', 'value': '2' },
    ]

});
//訂閱狀態Store
var statusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
            { 'txt': '全部', 'value': '0' },    
            { 'txt': '已訂閱', 'value': '1' },
            { 'txt': '未訂閱', 'value': '2' },
    ]

});
Ext.define('gigade.EdmTest', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'email_id', type: 'int' },
        { name: 'email_address', type: 'string' },
        { name: 'test_username', type: 'string' },
		{ name: 'test_status', type: 'int' },
        { name: 'test_createdate', type: 'string' },
        { name: 'test_updatedate', type: 'string' }
    ]
});
var EdmTestStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.EdmTest',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetEdmTestList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
EdmTestStore.on('beforeload', function () {
    Ext.apply(EdmTestStore.proxy.extraParams,
        {
            selectType: Ext.getCmp('selectType').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            isSecret: true
        });
});
var Edit_EdmTestStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.EdmTest',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetEdmTestList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
Edit_EdmTestStore.on('beforeload', function () {
    Ext.apply(Edit_EdmTestStore.proxy.extraParams,
        {
            selectType: Ext.getCmp('selectType').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            dateCon: Ext.getCmp('dateCon').getValue(),
            date_start: Ext.getCmp('timestart').getValue(),
            date_end: Ext.getCmp('timeend').getValue(),
            activeStatus: Ext.getCmp('activeStatus').getValue(),
            relation_id: "",
            isSecret: false
        });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdList").down('#remove').setDisabled(selections.length == 0);
        }
    }
});
Ext.onReady(function ()
{
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        border: 0,
        layout: 'anchor',
        height: 110,
        width: document.documentElement.clientWidth,
        items: [
              {
                  xtype: 'fieldcontainer',
                  layout: 'hbox',
                  items: [
                         {
                             xtype: 'combobox',
                             store: selectStore,
                             id: 'selectType',
                             fieldLabel: '查詢條件',
                             displayField: 'txt',
                             valueField: 'value',
                             width: 180,
                             labelWidth: 60,
                             margin: '5 5 2 2',
                             forceSelection: false,
                             editable: false,
                             value: '0'
                         },
                          {
                              xtype: 'textfield',
                              fieldLabel: "關鍵字",
                              width: 180,
                              labelWidth: 60,
                              margin: '5 0 2 2',
                              id: 'search_con',
                              name: 'search_con',
                              value: "",
                              listeners: {
                                  specialkey: function (field, e)
                                  {
                                      if (e.getKey() == Ext.EventObject.ENTER)
                                      {
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
                        id: 'dateCon',
                        name: 'dateCon',
                        store: dateStore,
                        displayField: 'txt',
                        valueField: 'value',
                        fieldLabel: '日期條件',
                        value: '0',
                        editable: false,
                        labelWidth: 60,
                        width: 180,
                        margin: '0 5 0 2'
                    },
                    {
                        xtype: 'datetimefield', allowBlank: true, id: 'timestart',
                        format: 'Y-m-d H:i:s',
                        name: 'serchcontent',
                        editable: false,
                        labelWidth: 60,
                        time: { hour: 00, min: 00, sec: 00 },
                        listeners: {//value: new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate(), 0, 0, 0),
                            select: function (a, b, c)
                            {
                                var start = Ext.getCmp("timestart");
                                var end = Ext.getCmp("timeend");
                                if (end.getValue() == null)
                                {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                } else if (end.getValue() < start.getValue())
                                {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                        {
                            xtype: 'datetimefield',
                            allowBlank: true,
                            id: 'timeend',
                            format: 'Y-m-d H:i:s',
                            editable: false,
                            name: 'serchcontent',
                            time: { hour: 23, min: 59, sec: 59 },
                            listeners: {//value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 23, 59, 59), 
                                select: function (a, b, c)
                                {
                                    var start = Ext.getCmp("timestart");
                                    var end = Ext.getCmp("timeend");
                                    if ( start.getValue()== null)
                                    {
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }
                                    else if (end.getValue() < start.getValue()) {
                                        start.setValue(setNextMonth(end.getValue(), -1));
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
                            store: statusStore,
                            id: 'activeStatus',
                            fieldLabel: '訂閱狀態',
                            displayField: 'txt',
                            valueField: 'value',
                            width: 180,
                            labelWidth: 60,
                            margin: '0 5 5 2',
                            forceSelection: false,
                            editable: false,
                            value: '0'
                        },
                      //buttons: [
                          {
                              xtype:'button',
                              text: SEARCH,
                              id: 'btnQuery',
                              margin: '0 5 0 33',
                              iconCls: 'ui-icon ui-icon-search-2',
                              handler: Query
                          },
                        {
                            xtype:'button',
                            text: RESET,
                            id: 'btn_reset',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners: {
                                click: function ()
                                {
                                    Ext.getCmp('selectType').setValue('0');
                                    Ext.getCmp('search_con').setValue(null);
                                    Ext.getCmp('dateCon').setValue('0');
                                    Ext.getCmp('activeStatus').setValue('0');
                                    Ext.getCmp('timestart').setValue(null);
                                    Ext.getCmp('timeend').setValue(null);
                                }
                            }
                        },
                      ]
              }
        ]
        
    });
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: EdmTestStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex:8.8,
        columns: [
             { header: "郵箱編號", dataIndex: 'email_id', id: 'emailid', name: 'emailid', flex: 1, align: 'center' },
             {
                 header: "電子郵件", dataIndex: 'email_address', flex: 3, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     return "<span onclick='SecretLogin(" + record.data.email_id + "," + 0 + ",\"" + info_type + "\")'  >" + value + "</span>";
                 }
             },
             {
                 header: "姓名", dataIndex: 'test_username', flex: 1, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                     return "<span onclick='SecretLogin(" + record.data.email_id + "," + 0 + ",\"" + info_type + "\")'  >" + value + "</span>";
                 }
             },
             {
                 header: "訂閱狀態",
                 dataIndex: 'test_status',
                 flex: 1,
                 align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         return "已訂閱";
                     }
                     else {
                         return "<span style='color:red'>未訂閱</span>";
                     }
                 }
             },
             { header: "建立時間", dataIndex: 'test_createdate', flex: 2, align: 'center' },
             { header: "更新時間", dataIndex: 'test_updatedate', flex: 2, align: 'center' }
        ],
        tbar: [
            {
                xtype: 'button',
                text: ADD,
                id: 'add',
                iconCls: 'icon-user-add',
                handler: onAddClick
            },
            {
                xtype: 'button',
                text: EDIT,
                id: 'edit',
                iconCls: 'icon-user-edit',
                disabled: true,
                handler: onEditClick
            },
             {
                 xtype: 'button',
                 text: REMOVE,
                 id: 'remove',
                 iconCls: 'icon-user-remove',
                 disabled: true,
                 handler: onDeleteClick
             },
             '->',
            // {
            //     xtype: 'combobox', fieldLabel: '類型', margin: '0 0 0 10', labelWidth: 45, width: 160, id: 'selectType', editable: false,
            //     displayField: 'txt', valueField: 'value', emptyText: '請選擇', store: selectStore,
            //     listeners: {
            //         specialkey: function (field, e) {
            //             if (e.getKey() == Ext.EventObject.ENTER) {
            //                 Query();
            //             }
            //         }
            //     }
            // },
            //{
            //    xtype: 'textfield',
            //    margin: '0 0 0 5',
            //    labelWidth: 60,
            //    id: 'search_con',
            //    name: 'search_con',
            //    value: '',
            //    width: 120,
            //    listeners: {
            //        specialkey: function (field, e) {
            //            if (e.getKey() == Ext.EventObject.ENTER) {
            //                Query();
            //            }
            //        }
            //    }
            //},
            //{ xtype: 'button', text: '查詢', iconCls: 'icon-search', handler: Query },
            //{
            //    xtype: 'button',
            //    text: '重置',
            //    iconCls: 'ui-icon ui-icon-reset',
            //    handler: function () {
            //        Ext.getCmp('selectType').setValue(null);
            //        Ext.getCmp('search_con').setValue('');
            //    }
            //}

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmTestStore,
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
        layout: 'vbox',//searchForm
        items: [searchForm,gdList],
        renderTo: Ext.getBody(),
        //autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // EdmTestStore.load({ params: { start: 0, limit: 25 } });
});
onAddClick = function () {
    addFunction(EdmTestStore);
}
onEditClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        //editFunction(row[0], EdmTestStore);
        var secret_type = "15";//參數表中的"會員查詢列表"
        var url = "/edm/EdmTest?&& email_id=" + row[0].data.email_id + "/Edit";
        var ralated_id = row[0].data.email_id;
        var info_id = "0";
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id, Edit_EdmTestStore, info_id);
            }
        }
    }
}
onDeleteClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var ids = '';
                for (var i = 0; i < row.length; i++) {
                    ids += row[i].data.email_id + '|';
                }
                Ext.Ajax.request({
                    url: '/Edm/DeleteEdmTest',
                    method: 'post',
                    params: { email_id: ids },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            EdmTestStore.load();
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗");
                            EdmTestStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

function Query()
{
    if (Ext.getCmp('dateCon').getValue() != "0")
    {
        if (Ext.getCmp('timestart').getValue() == ("" || null))
        {
            Ext.Msg.alert(INFORMATION, "請選擇查詢日期");
            return;
        }
        if (Ext.getCmp('timeend').getValue() == ("" || null))
        {
            Ext.Msg.alert(INFORMATION, "請選擇查詢日期");
            return;
        }
    }
    EdmTestStore.removeAll();
    Ext.getCmp("gdList").store.loadPage(1, {
        params: {
            selectType: Ext.getCmp('selectType').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            dateCon: Ext.getCmp('dateCon').getValue(),
            date_start: Ext.getCmp('timestart').getValue(),
            date_end: Ext.getCmp('timeend').getValue(),
            activeStatus: Ext.getCmp('activeStatus').getValue()
        }
    });

}
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "15";//參數表中的"訊息管理"
    var url = "/Edm/GetEdmTestList";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }
}
setNextMonth = function (source, n)
{
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0)
    {
        s.setHours(0, 0, 0);
    }
    else if (n > 0)
    {
        s.setHours(23, 59, 59);
    }
    return s;
}
