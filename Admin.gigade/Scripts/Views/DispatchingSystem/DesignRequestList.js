var pageSize = 25;
Ext.define('gigade.DesignRequent', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "dr_id", type: "int" },
    { name: "dr_requester_id", type: "int" },
    { name: "dr_type", type: "int" },
    { name: "dr_assign_to", type: "int" },
    { name: "dr_status", type: "int" },
    { name: "dr_content_text", type: "string" },
    { name: "dr_description", type: "string" },
    { name: "dr_document_path", type: "string" },
    { name: "dr_resource_path", type: "string" },
    { name: "dr_requester_id_name", type: "string" },
    { name: "dr_assign_to_name", type: "string" },
    { name: "dr_type_tostring", type: "string" },
    { name: "dr_status_tostring", type: "string" },
    { name: "product_id", type: "int" },
    { name: "product_name", type: "string" },
    { name: "dr_created", type: "datetime" },
    { name: "dr_modified", type: "datetime" },
    { name: "dr_expected", type: "datetime" },
    { name: "Isgq", type: "int" }

    ]
});
var DesignRequentStore = Ext.create('Ext.data.Store', {
    model: 'gigade.DesignRequent',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/DispatchingSystem/GetDesignRequestList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//需求狀態store列表頁搜索
Ext.define('StatusModel', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "parameterCode", type: "int" },
        { name: "parametername", type: "string" }
    ]
});

var StatusModel1Store = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'StatusModel',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/DispatchingSystem/GetStatus",
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//需求類型store列表頁搜索
Ext.define('TypeModel1', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "parameterCode", type: "int" },
        { name: "parametername", type: "string" }

    ]
});

var TypeStore1 = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'TypeModel1',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/DispatchingSystem/GetType",
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//被指派需求執行者store列表頁搜索
Ext.define('assignToModel1', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "user_id", type: "int" },
        { name: "user_username", type: "string" }

    ]
});

var assignToUserStore1 = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'assignToModel1',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/DispatchingSystem/GetDesign",
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//日期類型store
var DateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有時間", "value": "0" },
        { "txt": "建立時間", "value": "1" }
    ]
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("DesignRequentGrid").getSelectionModel().getSelection();
            Ext.getCmp("DesignRequentGrid").down('#take').setDisabled(true);
            Ext.getCmp("DesignRequentGrid").down('#order').setDisabled(true);
            Ext.getCmp("DesignRequentGrid").down('#update').setDisabled(true);
            Ext.getCmp("DesignRequentGrid").down('#pass').setDisabled(true);
            if (row.length > 0 && row[0].data.dr_status == 2) {//審核通過可以指派人員
                Ext.getCmp("DesignRequentGrid").down('#order').setDisabled(false);
                Ext.getCmp("DesignRequentGrid").down('#take').setDisabled(false);
                //Ext.getCmp("DesignRequentGrid").down('#update').setDisabled(true);
                //Ext.getCmp("DesignRequentGrid").down('#pass').setDisabled(true);
            }
            else if (row.length > 0 && row[0].data.dr_status == 1) {//新建立的可以審核通過
                //Ext.getCmp("DesignRequentGrid").down('#order').setDisabled(true);
                //Ext.getCmp("DesignRequentGrid").down('#take').setDisabled(true);
                //Ext.getCmp("DesignRequentGrid").down('#update').setDisabled(true);
                Ext.getCmp("DesignRequentGrid").down('#pass').setDisabled(false);
            }
            else if (row.length > 0 && row[0].data.dr_status == 3) {//已指派的可以重新指派人員
                Ext.getCmp("DesignRequentGrid").down('#order').setDisabled(false);
                Ext.getCmp("DesignRequentGrid").down('#update').setDisabled(false);
                //Ext.getCmp("DesignRequentGrid").down('#take').setDisabled(true);
                //Ext.getCmp("DesignRequentGrid").down('#pass').setDisabled(true);
            }
            else if (row.length > 0 && row[0].data.dr_status < 6) {//已完結之前的狀態可以變更
                Ext.getCmp("DesignRequentGrid").down('#update').setDisabled(false);
            }
            Ext.getCmp("DesignRequentGrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("DesignRequentGrid").down('#remove').setDisabled(selections.length == 0);
        }
    }
});
Ext.onReady(function () {
    var searchFrm = Ext.create('Ext.form.Panel', {
        id: 'searchFrm',
        bodyPadding: '15',
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: "需求提出者",
                            labelWidth: 110,
                            id: 'drrequester',
                            name: 'drrequester',
                            width: 250
                        },
                        {
                            xtype: 'combobox',
                            fieldLabel: "需求類型",
                            labelWidth: 110,
                            margin: '0 0 0 5px',
                            id: 'drtype',
                            name: 'drtype',
                            width: 250,
                            store: TypeStore1,
                            emptyText: '請選擇',
                            forceSelection: true,
                            typeAhead: true,
                            triggerAction: 'all',
                            displayField: 'parametername',
                            valueField: 'parameterCode'
                        }
                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: "被指派需求執行者",
                        labelWidth: 110,
                        id: 'assign_to',
                        name: 'assign_to',
                        width: 250,
                        store: assignToUserStore1,
                        emptyText: '請選擇',
                        forceSelection: true,
                        typeAhead: true,
                        triggerAction: 'all',
                        displayField: 'user_username',
                        valueField: 'user_id'
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: "需求狀態",
                        labelWidth: 110,
                        margin: '0 0 0 5px',
                        id: 'status',
                        name: 'status',
                        width: 250,
                        store: StatusModel1Store,
                        emptyText: '請選擇',
                        allowBlank: true,
                        forceSelection: true,
                        typeAhead: true,
                        triggerAction: 'all',
                        displayField: 'parametername',
                        valueField: 'parameterCode'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                   {
                       xtype: 'combobox',
                       fieldLabel: '查詢時間',
                       store: DateStore,
                       id: 'search_date',
                       valueField: 'value',
                       editable: false,
                       labelWidth: 110,
                       width: 250,
                       value: '1',
                       displayField: 'txt',
                       //margin: '0 0 0 10',

                   },
                   {
                       xtype: 'datetimefield',
                       id: 'start_time',
                       name: 'start_time',
                       format: 'Y-m-d  H:i:s',
                       time: { hour: 00, min: 00, sec: 00 },
                       editable: false,
                       width: 150,
                       value: new Date(Tomorrow(-1).setMonth(Tomorrow(-1).getMonth() - 1)),
                       listeners: {
                           select: function () {
                               var start = Ext.getCmp("start_time");
                               var end = Ext.getCmp("end_time");
                               if (end.getValue() == null) {
                                   end.setValue(setNextMonth(start.getValue(), 1));
                               } else if (start.getValue() > end.getValue()) {
                                   Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                   end.setValue(setNextMonth(start.getValue(), 1));
                               }
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
                        value: '~',
                        labelWidth: 10
                    },
                   {
                       xtype: 'datetimefield',
                       id: 'end_time',
                       name: 'end_time',
                       format: 'Y-m-d  H:i:s',
                       time: { hour: 23, min: 59, sec: 59 },
                       width: 150,
                       editable: false,
                       value: setNextMonth(Tomorrow(-1), 0),
                       listeners: {
                           select: function (a, b, c) {
                               var start = Ext.getCmp("start_time");
                               var end = Ext.getCmp("end_time");
                               if (start.getValue() != "" && start.getValue() != null) {
                                   if (end.getValue() < start.getValue()) {
                                       Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                       start.setValue(setNextMonth(end.getValue(), -1));
                                   }
                               } else {
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
            }
        ],
        buttonAlign: 'left',
        buttons: [
            {
                text: '查詢',
                handler: function () {
                    Query();
                }
            },
            {
                text: '重置',
                handler: function () {
                    this.up('form').getForm().reset();
                }
            }
        ]
    });
    var DesignRequentGrid = Ext.create('Ext.grid.Panel', {
        id: 'DesignRequentGrid',
        store: DesignRequentStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.1,
        columns: [
            { header: "ID", dataIndex: 'dr_id', width: 100, align: 'center', hidden: true },
            {
                header: "需求提出者", dataIndex: 'dr_requester_id_name', width: 100, align: 'center'
            },
            { header: "需求類型", dataIndex: 'dr_type_tostring', width: 100, align: 'center' },
            {
                header: "被指派需求執行者", dataIndex: 'dr_assign_to_name', width: 120, align: 'center'
            },
            { header: "文案內容", dataIndex: 'dr_content_text', width: 200, align: 'center' },
            { header: "需求描述", dataIndex: 'dr_description', width: 150, align: 'center' },
            { header: "素材路徑", dataIndex: 'dr_resource_path', width: 150, align: 'center' },
            { header: "文件路徑", dataIndex: 'dr_document_path', width: 150, align: 'center' },
            {
                header: "需求狀態", dataIndex: 'dr_status_tostring', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data["dr_status"] == 6) {
                        return "<font color='red'>" + value + "</font>";
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: "期望完成時間", dataIndex: 'dr_expected', width: 120, align: 'center' },
            { header: "建立時間", dataIndex: 'dr_created', width: 120, align: 'center' },
            { header: "最後修改時間", dataIndex: 'dr_modified', width: 120, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick, hidden: true },
            { xtype: 'button', text: "刪除", id: 'remove', iconCls: 'ui-icon ui-icon-user-delete', disabled: true, handler: onRemoveClick, hidden: true },
            { xtype: 'button', text: "認領工作", id: 'take', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, hidden: true, handler: TakeJob },
            { xtype: 'button', text: "指派人員", id: 'order', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, hidden: true, handler: OrderPeople },
            { xtype: 'button', text: "變更狀態", id: 'update', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, hidden: true, handler: UpdateStatus },
            { xtype: 'button', text: "審核通過", id: 'pass', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, hidden: true, handler: UpdateStatus }
        ],
        viewConfig: {
            forceFit: true,
            getRowClass: function (record, rowIndex, rowParams, store) {
                if (record.data.Isgq == 1) {
                    return 'ems_actual_type';//注意这里返回的是定义好的css类；列如：(.ppp_ddd_sss div{background-color:red})定义到你页面访问到的css文件里。
                }
            }
        },//改變grid內的某行顏色;
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DesignRequentStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
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
        items: [searchFrm, DesignRequentGrid],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                DesignRequentGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    DesignRequentStore.load({ params: { start: 0, limit: pageSize } });
});

DesignRequentStore.on('beforeload', function () {
    Ext.apply(DesignRequentStore.proxy.extraParams,
        {
            dr_requester: Ext.getCmp('drrequester').getValue(),
            dr_type: Ext.getCmp('drtype').getValue(),
            assign_to: Ext.getCmp('assign_to').getValue(),
            status: Ext.getCmp('status').getValue(),
            search_date: Ext.getCmp('search_date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue()
        });
});

//*********新增********//
onAddClick = function () {
    editFunction(null, DesignRequentStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("DesignRequentGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        editFunction(row[0], DesignRequentStore);
    }
}

//查詢
function Query() {
    Ext.getCmp("DesignRequentGrid").store.loadPage(1, {
        params: {
            dr_requester: Ext.getCmp('drrequester').getValue(),
            dr_type: Ext.getCmp('drtype').getValue(),
            assign_to: Ext.getCmp('assign_to').getValue(),
            status: Ext.getCmp('status').getValue(),
            search_date: Ext.getCmp('search_date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue()
        }
    });
}

//*********刪除********//
onRemoveClick = function () {
    var row = Ext.getCmp("DesignRequentGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        Ext.Msg.confirm("確認信息", Ext.String.format("刪除選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs = rowIDs + row[i].data.dr_id + ",";
                }
                Ext.Ajax.request({
                    url: '/DispatchingSystem/DelDesignRequest',
                    method: 'post',
                    params: {
                        dr_id: rowIDs,
                        dr_status: row[0].data.dr_status,
                        dr_assign_to: row[0].data.dr_assign_to
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.msg == 0) {
                                Ext.Msg.alert("提示信息", "刪除成功！");
                            }
                            else if (result.msg == 2) {
                                Ext.Msg.alert("提示信息", "您只能刪除自己申請的需求!");
                            }
                            else if (result.msg == 3) {
                                Ext.Msg.alert("提示信息", "通知美工mail發送失敗請電話通知!");
                            }
                            else {
                                Ext.Msg.alert("提示信息", "刪除失敗！");
                            }
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗！");
                        }
                        DesignRequentStore.load();
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "刪除失敗！");
                        DesignRequentStore.load();
                    }
                });
            }
            else {
            }
        });
    }
}
OrderPeople = function () {
    var row = Ext.getCmp("DesignRequentGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        DesigneeFunction(row[0].data.dr_id, row[0].data.dr_status);
    }
}
UpdateStatus = function () {
    var row = Ext.getCmp("DesignRequentGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        Ext.Msg.confirm("確認信息", "確定變更狀態?", function (btn) {
            if (btn == 'yes') {
                Ext.Ajax.request({
                    url: '/DispatchingSystem/UpdStatus',
                    method: 'post',
                    params: {
                        dr_id: row[0].data.dr_id,
                        dr_status: row[0].data.dr_status,
                        dr_type: row[0].data.dr_type,
                        product_id: row[0].data.product_id,
                        product_detail_text: row[0].data.dr_content_text
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.msg == "3") {
                                Ext.Msg.alert("提示信息", "更新內頁文失敗！");
                            }
                            else if (result.msg == "2") {
                                Ext.Msg.alert("提示信息", "沒有權限!");
                            }
                            else {
                                Ext.Msg.alert("提示信息", "更改成功！");
                            }
                        }
                        else {
                            Ext.Msg.alert("提示信息", "更改失敗！");
                        }
                        DesignRequentStore.load();
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "狀態更改失敗！");
                        DesignRequentStore.load();
                    }
                });
            }
        });
    }
}
function Tomorrow(n) {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    dt.setDate(dt.getDate() + n);
    return dt;                  // 返回日期。
}
function setNextMonth(source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n >= 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
//*********認領工作********//
TakeJob = function () {
    var row = Ext.getCmp("DesignRequentGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        Ext.Msg.confirm("確認信息", Ext.String.format("確定要認領這 {0} 條工作？", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs = rowIDs + row[i].data.dr_id + ",";
                }
                Ext.Ajax.request({
                    url: '/DispatchingSystem/TakeJob',
                    method: 'post',
                    params: { dr_id: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.msg == 0) {
                                Ext.Msg.alert("提示信息", "認領成功！");
                            }
                            else if (result.msg == 4) {
                                Ext.Msg.alert("提示信息", "郵件發送失敗,請確認email后重新認領！");
                            }
                            else {
                                Ext.Msg.alert("提示信息", "認領失敗！");
                            }
                        }
                        else {
                            Ext.Msg.alert("提示信息", "認領失敗！");
                        }
                        DesignRequentStore.load();
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "認領失敗！");
                        DesignRequentStore.load();
                    }
                });
            }
            else {
            }
        })
    }
}

