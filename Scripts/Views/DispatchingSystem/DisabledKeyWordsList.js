var pageSize = 25;
Ext.define('gigade.DisableKeywords', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'dk_id', type: 'int' },
    { name: 'dk_string', type: 'string' },
    { name: 'user_name', type: 'string' },
    { name: 'dk_created', type: 'string' },
    { name: 'dk_modified', type: 'string' },
    { name: 'dk_active', type: 'int' }
    ]
});
var DisKeyWordsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.DisableKeywords',
    proxy: {
        type: 'ajax',
        url: '/DispatchingSystem/GetKeyWordsList',
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
            //Ext.getCmp("gdList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdList").down('#delete').setDisabled(selections.length == 0);
            var row = Ext.getCmp("gdList").getSelectionModel().getSelection();

        }
    }
});
DisKeyWordsStore.on('beforeload', function () {
    Ext.apply(DisKeyWordsStore.proxy.extraParams,
    {
        starttime: Ext.getCmp('start').getValue(),
        endtime: Ext.getCmp('end').getValue(),
        text: Ext.getCmp('keyWord').getValue()
    });
});
Ext.onReady(function () {
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: DisKeyWordsStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex:1,
        columns: [
            { header: "編號", dataIndex: 'dk_id', width: 100, align: 'center', hidden: true },
            { header: "禁用資料關鍵字", dataIndex: 'dk_string', width: 150, align: 'center' },
            { header: "建立人", dataIndex: 'user_name', width: 150, align: 'center' },
            { header: "建立時間", dataIndex: 'dk_created', width: 150, align: 'center' },
            //,
            //{ header: "修改時間", dataIndex: 'dk_modified', width: 150, align: 'center' }
            {
                header: "關鍵字狀態",dataIndex: 'dk_active',align: 'center',hidden: false,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.dk_id + ")'><img hidValue='1' id='img" + record.data.dk_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.dk_id + ")'><img hidValue='0' id='img" + record.data.dk_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
        {
            xtype: 'button',
            text: ADD,
            id: 'add',
            iconCls: 'icon-user-add',
            handler: addClick
        },
        //{
        //    xtype: 'button',
        //    text: "編輯",
        //    id: 'edit',
        //    iconCls: 'ui-icon ui-icon-user-edit',
        //    disabled: true,
        //    handler: editClick
        //},
        {
            xtype: 'button',
            text: "刪除",
            id: 'delete',
            iconCls: 'icon-user-edit',
            disabled: true,
            handler: deleteClick
        },
        '->',
        {
            xtype: 'textfield',
            fieldLabel: '關鍵字',
            labelWidth: 50,
            width: 180,
            id: 'keyWord',
            name: 'keyWord',
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Query();
                    }
                }
            }
        },
        {
            xtype: 'datefield',
            fieldLabel: "建立時間",
            id: 'start',
            name: 'start',
            labelWidth: 60,
            //margin: '0 5 3 0',
            format: 'Y-m-d',
            editable: false,
            listeners: {
                select: function () {
                    var start = Ext.getCmp("start");
                    var end = Ext.getCmp("end");
                    if (end.getValue() == null) {
                        end.setValue(setNextMonth(start.getValue(), 1));
                    } else if (start.getValue() > end.getValue()) {
                        Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                        end.setValue(setNextMonth(start.getValue(), 1));
                    }
                    else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                        // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
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
            labelWidth: 10,
            //margin: '0 10 3 7'
        },
        {
            xtype: 'datefield',
            id: 'end',
            name: 'end',
            format: 'Y-m-d',
            editable: false,
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("start");
                    var end = Ext.getCmp("end");
                    var s_date = new Date(start.getValue());
                    var now_date = new Date(end.getValue());
                    if (start.getValue() != "" && start.getValue() != null) {
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                            end.setValue(setNextMonth(start.getValue(), 1));
                        } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                            //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
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
        },
        {
            xtype: 'button',
            iconCls: 'ui-icon ui-icon-search-2',
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
                    Ext.getCmp('keyWord').setValue('');
                    Ext.getCmp('start').setValue(null);
                    Ext.getCmp('end').setValue(null);
                }
            }
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DisKeyWordsStore,
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
        items: [gdList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})
var addClick = function () {
    editFunction(null, DisKeyWordsStore);
}
var editClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        editFunction(row[0], DisKeyWordsStore);
    }
}
var deleteClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.dk_id + ',';
                }
                Ext.Ajax.request({
                    url: '/DispatchingSystem/DeleteKeyWords',//執行方法
                    method: 'post',
                    params: {
                        ids: rowIDs
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.msg == 1) {
                                Ext.Msg.alert(INFORMATION, "刪除成功!");
                                DisKeyWordsStore.loadPage(1);
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "刪除失敗!");
                                DisKeyWordsStore.loadPage(1);
                            }
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "操作失敗!");
                            DisKeyWordsStore.loadPage(1);
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
function Query(x) {
    DisKeyWordsStore.removeAll();
    var text = Ext.getCmp('keyWord').getValue();
    var start = Ext.getCmp('start').getValue();
    var end = Ext.getCmp('end').getValue();
    if (text == '' && (start == null || end == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else {
        Ext.getCmp("gdList").store.loadPage(1, {
            params: {
                starttime: Ext.getCmp('start').getValue(),
                endtime: Ext.getCmp('end').getValue(),
                text: Ext.getCmp('keyWord').getValue()
            }
        });
    }
}

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
/*********************啟用/禁用**********************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/DispatchingSystem/UpdateStats",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "post",
        type: 'text',
        success: function (msg) {
            //Ext.Msg.alert(INFORMATION, "修改成功!");
            DisKeyWordsStore.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}