var pageSize = 25;
Ext.define('gigade.EdmGroup', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "int" },
        { name: "group_name", type: "string" },
        { name: "group_total_email", type: "int" },
        { name: "status", type: "int" },
        { name: "s_group_createdate", type: "string" },
        { name: "s_group_updatedate", type: "string" },
        { name: "total_content", type: "int" },

    ]
});
var EdmGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.EdmGroup',
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Edm/GetEdmGroupList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
EdmGroupStore.on('beforeload', function () {
    Ext.apply(EdmGroupStore.proxy.extraParams,
        {
            selectType: Ext.getCmp('selectType').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            timestart: Ext.getCmp('start').getValue(),
            timeend: Ext.getCmp('end').getValue(),
            dateType: Ext.getCmp('dateType').getValue()
        });
});
var EdmSearchStore = Ext.create('Ext.data.Store', {
    fields: ["txt", "value"],
    data: [
    { "txt": "群組編號", "value": "0" },
    { "txt": "群組名稱", "value": "1" },
    ]
});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ["dtxt", "dvalue"],
    data: [
    { "dtxt": "建立時間", "dvalue": "0" },
    { "dtxt": "更新時間", "dvalue": "1" },
    ]
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("EdmGroup").getSelectionModel().getSelection();
            Ext.getCmp("EdmGroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
var EditTpl = new Ext.XTemplate(
        '<a href=javascript:TranToDetial("/Edm/EdmGroupEmail","{group_id}")>' + "{group_total_email}" + '</a> '
    );
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 70,
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
                         fieldLabel: '查詢條件',
                         labelWidth: 60,
                         width: 160,
                         id: 'selectType',
                         editable: false,
                         displayField: 'txt',
                         valueField: 'value',
                         emptyText: '請選擇',
                         store: EdmSearchStore,
                     },
                     {
                         xtype: 'textfield',
                         //fieldLabel: KEY,
                         labelWidth: 60,
                         margin: '0 5 0 5',
                         id: 'search_con',
                         name: 'search_con',
                         value: '',
                         width: 120,
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == Ext.EventObject.ENTER) {
                                     onQuery();
                                 }
                             },
                             focus: function () {
                                 var selectType = Ext.getCmp("selectType").getValue();
                                 if (selectType == null || selectType == '') {
                                     Ext.Msg.alert("提示信息", "請先選則查詢類型");
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
                        fieldLabel: '日期條件',
                        labelWidth: 60,
                        width: 160,
                        id: 'dateType',
                        editable: false,
                        displayField: 'dtxt',
                        valueField: 'dvalue',
                        emptyText: '請選擇',
                        store: dateStore,
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'start',
                        name: 'start',
                        margin: '0 5 0 5',
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                        listeners: {
                            select: function (a, b, c) {
                                var tstart = Ext.getCmp("start");
                                var tend = Ext.getCmp("end");
                                if (tend.getValue() == null) {
                                    tend.setValue(setNextMonth(tstart.getValue(), 1));
                                }
                                else if (tend.getValue() < tstart.getValue()) {
                                     
                                    tend.setValue(setNextMonth(tstart.getValue(), 1));
                                }
                            },
                            focus: function () {
                                var dateType = Ext.getCmp("dateType").getValue();
                                if (dateType == null || dateType == '') {
                                    Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                                    this.focus = false;
                                }
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        margin: '0 2 0 0',
                        value: "~"
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'end',
                        name: 'end',
                        margin: '0 5 0 0',
                        labelWidth: 15,
                        // width: 210,
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 23, min: 59, sec: 59 },//標記結束時間
                        listeners: {
                            select: function (a, b, c) {
                                var tstart = Ext.getCmp("start");
                                var tend = Ext.getCmp("end");
                                if (tstart.getValue() == null) {
                                    tstart.setValue(setNextMonth(tend.getValue(), -1));
                                }
                                else if (tend.getValue() < tstart.getValue()) {
                                    
                                    tstart.setValue(setNextMonth(tend.getValue(), -1));
                                }
                            },
                            focus: function () {
                                var dateType = Ext.getCmp("dateType").getValue();
                                if (dateType == null || dateType == '') {
                                    Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                                    this.focus = false;
                                }
                            }
                        }
                    },
                    { xtype: 'button', text: '查詢', iconCls: 'icon-search', handler: onQuery },
                    {
                        xtype: 'button',
                        text: '重置',
                        margin: '0 5 0 5',
                        iconCls: 'ui-icon ui-icon-reset',
                        handler: function () {
                            Ext.getCmp('selectType').reset();
                            Ext.getCmp('search_con').reset();
                            Ext.getCmp('start').reset();
                            Ext.getCmp('end').reset();
                            Ext.getCmp('dateType').reset();
                        }
                    }
                ]
            }

        ]
    });
    var EdmGroup = Ext.create('Ext.grid.Panel', {
        id: 'EdmGroup',
        store: EdmGroupStore,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight - 70,
        columnLines: true,
        frame: true,
        columns: [
            { header: "群組編號", dataIndex: 'group_id', width: 150, align: 'center' },
            { header: "群組名稱", dataIndex: 'group_name', width: 150, align: 'center' },
            {
                header: "名單數", dataIndex: 'group_total_email', width: 120, align: 'center', id: 'security_group_total_email', hidden: true,
                xtype: 'templatecolumn', tpl: EditTpl
            },
             { header: "電子報數", dataIndex: 'total_content', width: 120, align: 'center' },
             {
                 header: "其他功能", width: 120, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     return "<a href='javascript:void(0);'onclick='Import(" + record.data.group_id + ")'>匯入</a>" + "&nbsp&nbsp&nbsp&nbsp&nbsp" + "<a href='javascript:void(0)' onclick='Export(" + record.data.group_id + ")'>匯出</a>";
                 }
             },
            { header: "建立時間", dataIndex: 's_group_createdate', width: 120, align: 'center' },
              { header: "更新時間", dataIndex: 's_group_updatedate', width: 120, align: 'center' },
            {
                header: "刪除", dataIndex: 'total_content', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return "<a href='javascript:void(0);'onclick='onRemoveClick(" + record.data.group_id + ")'>刪除</a>";

                    }
                }
            },

        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmGroupStore,
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
        items: [frm, EdmGroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                EdmGroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // MailGroupStore.load({ params: { start: 0, limit: 25 } });
});

//*********新增********//
onAddClick = function () {
    editFunction(null, EdmGroupStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("EdmGroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    } else {
        editFunction(row[0], EdmGroupStore);
    }
}

//*********刪除********//
onRemoveClick = function (group_id) {
    var row = Ext.getCmp("EdmGroup").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {
        Ext.Msg.confirm("確認信息", Ext.String.format("刪除選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = group_id + '|';
                //for (var i = 0; i < row.length; i++) {
                //    rowIDs += row[i].data.group_id + '|';
                //}

                Ext.Ajax.request({
                    url: '/Edm/DeleteEdmGroup',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", "刪除成功！");
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗！");
                        }
                        EdmGroupStore.load();
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "刪除失敗！");
                        EdmGroupStore.load();
                    }
                });
            }
        });
    }
}

function ExportExeclUserMessage(id) {
    window.open("/NewPromo/ExportNewPromoRecordList?event_id=" + id);
}
Import = function () {
    var row = Ext.getCmp("EdmGroup").getSelectionModel().getSelection();
    ImportFunction(row[0], EdmGroupStore);
}
function Export(group_id) {
    window.open("/Edm/Export?group_id=" + group_id);
    //Ext.MessageBox.show({
    //    msg: '正在匯出，請稍後....',
    //    width: 300,
    //    wait: true
    //});
    //Ext.Ajax.request({
    //    url: "/Edm/Export",
    //    timeout: 900000,
    //    params: {
    //        group_id: group_id
    //    },
    //    success: function (form, action) {
    //        Ext.MessageBox.hide();
    //        var result = Ext.decode(form.responseText);
    //        if (result.success) {
    //            window.location = '../../ImportUserIOExcel/' + result.fileName;
    //        } else {
    //            Ext.MessageBox.hide();
    //            Ext.Msg.alert("提示信息", "匯出失敗或沒有數據！");
    //        }
    //    }
    //});
}

function TranToDetial(url, group_id) {

    var urlTran = url + '?group_id=' + group_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#group_id');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'group_id',
        title: '電子報群組名單列表',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
function onQuery() {
    var searchT = Ext.getCmp('selectType').getValue();
    var searchC = Ext.getCmp('search_con').getValue();
    var start = Ext.getCmp('start').getValue();
    var end = Ext.getCmp('end').getValue();
    var dateType = Ext.getCmp('dateType').getValue();
    if ((searchT != '' && searchT != null) && searchC == "") {
        Ext.Msg.alert(INFORMATION, '請輸入查詢內容');
    }
    else if ((searchT == '' || searchT == null) && searchC != "") {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else if ((dateType != '' && dateType != null) && (start == null || end == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇日期');
    }
    else if ((dateType == '' || dateType == null) && (start != null || end != null)) {
        Ext.Msg.alert(INFORMATION, '請選擇日期條件');
    }
    else {
        Ext.getCmp('EdmGroup').store.loadPage(1);
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