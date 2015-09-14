var pageSize = 20;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.TicketDetailAllCode', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ticket_master_id", type: "int" },
        { name: "course_name", type: "string" },
        { name: "start_date", type: "string" },
        { name: "end_date", type: "string" },
        { name: "ticket_code", type: "string" },
        { name: "spec_id_1", type: "string" },
        { name: "spec_id_2", type: "string" },
        { name: "flag", type: "string" }
    ]
});
//到Controller獲取數據
var TiceketDetailAllCodeStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.TicketDetailAllCode',
    proxy: {
        type: 'ajax',
        url: '/Ticket/GetTicketDetailAllCodeList',
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
TiceketDetailAllCodeStore.on('beforeload', function () {
    Ext.apply(TiceketDetailAllCodeStore.proxy.extraParams, {
        search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue(),
        flag_status: Ext.getCmp('flag_status').getValue()
    });
});
function Query(x) {
    TiceketDetailAllCodeStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue(),
            flag_status: Ext.getCmp('flag_status').getValue()
        }
    });
}
var LoginTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '未使用', "value": "0" },
        { "txt": '已使用', "value": "1" }
    ]
});
Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: TiceketDetailAllCodeStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "訂單號", dataIndex: 'ticket_master_id', width: 70, align: 'center' },
            { header: "課程名稱", dataIndex: 'course_name', width: 120, align: 'center' },
            { header: "規格一", dataIndex: 'spec_id_1', width: 120, align: 'center' },
            { header: "規格二", dataIndex: 'spec_id_2', width: 130, align: 'center' },
            { header: "課程開始時間", dataIndex: 'start_date', width: 130, align: 'center' },
            { header: "課程結束時間", dataIndex: 'end_date', width: 130, align: 'center' },
            { header: "票券", dataIndex: 'ticket_code', width: 130, align: 'center' },
            {
                header: "票券狀態", dataIndex: 'flag', width: 130, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "0") {
                        return "未使用";
                    }
                    if (value == "1") {
                        return "已使用";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: true, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            "->",
            {
                xtype: 'combobox',
                id: 'flag_status',
                name: 'flag_status',
                fieldLabel: '票券狀態',
                queryMode: 'local',
                store: LoginTypeStore,
                displayField: 'txt',
                valueField: 'value',
                value: 0,
                typeAhead: true,
                editable: false,
                hiddenName: 'value'
            },
            {
                xtype: 'textfield', fieldLabel: "票券號/訂單號", labelWidth: 120, id: 'search_content', listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query(1);
                        }
                    }
                }
            },
                    {
                        xtype: 'button',
                        margin: '0 10 0 10',
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
                                Ext.getCmp('search_content').setValue("");
                                Ext.getCmp('flag_status').setValue(0);
                                Query();
                            }
                        }
                    }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TiceketDetailAllCodeStore,
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
        items: [gdFgroup],
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
    TiceketDetailAllCodeStore.load({ params: { start: 0, limit: pageSize } });
});

/********************************************新增*****************************************/
onAddClick = function () {
    editFunction(null, TiceketDetailAllCodeStore);
}

/*********************************************編輯***************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], TiceketDetailAllCodeStore);
    }
}



//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, pageId, areaId) {
    var activeValue = $("#img" + id).attr("hidValue"); //hidValue=1時是將要變成啟用
    var limitN = 0;
    var listN = 0;
    Ext.Ajax.request({
        url: "/WebContentType/GetDefaultLimit",
        method: 'post',
        async: false, //true為異步，false為同步
        params: {
            storeType: "web_content_type1",
            site: "7",
            page: pageId,
            area: areaId
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                listN = result.listNum;
                limitN = result.limitNum;

            }
        }
    });
    if (activeValue == "1" && listN >= limitN && limitN != 0) {//list的值大於或等於limit的值時提示信息，yes時執行，no時返回

        Ext.Msg.confirm(CONFIRM, Ext.String.format(STATUSTIP), function (btn) {
            if (btn == 'yes') {
                Ext.Ajax.request({
                    url: "/WebContentType/UpdateActive",
                    method: 'post',
                    params: {
                        id: id,
                        active: activeValue,
                        storeType: "type1"
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            TiceketDetailAllCodeStore.load();
                            if (activeValue == 1) {
                                $("#img" + id).attr("hidValue", 0);
                                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                            } else {
                                $("#img" + id).attr("hidValue", 1);
                                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                            }
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }

                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
            else { return; }
        });
    }
    else {
        Ext.Ajax.request({
            url: "/WebContentType/UpdateActive",
            method: 'post',
            params: {
                id: id,
                active: activeValue,
                storeType: "type1"
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    TiceketDetailAllCodeStore.load();
                    if (activeValue == 1) {
                        $("#img" + id).attr("hidValue", 0);
                        $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                    } else {
                        $("#img" + id).attr("hidValue", 1);
                        $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                    }
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }

            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
}
