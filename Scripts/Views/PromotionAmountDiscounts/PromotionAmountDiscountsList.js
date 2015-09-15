var pageSize = 25;
Ext.define('gigade.PAD', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "name", type: "string" },
        { name: "group_id", type: "int" },
        { name: "group_name", type: "string" },
        { name: "discount", type: "int" },
        {name:'amount',type:'int'},
        {name:'quantity',type:'int'},
        {name:'site',type:'string'},
        { name: "start", type: "string" },
        { name: "end", type: "string" },
        { name: 'active', type: 'bool' },
        { name: 'kuser', type: 'int' },
        { name: 'create_user', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'muser', type: 'int' },
        { name: 'update_user', type: 'string' }

    ]
});
var PADStore = Ext.create('Ext.data.Store', {
    model: 'gigade.PAD',
    autoDestroy:true,
    proxy: {
        type: 'ajax',
        url: '/PromotionAmountDiscounts/GetList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

var searchStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "未過期", "value": "1" },
        { "txt": "已過期", "value": "0" },
    ]
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("PAD").getSelectionModel().getSelection();
            Ext.getCmp("PAD").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("PAD").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
PADStore.on('beforeload', function () {
    Ext.apply(PADStore.proxy.extraParams, {
        searchStore: Ext.getCmp('searchStore').getValue(),
    });
});


Ext.onReady(function () {
    var form = Ext.create('Ext.form.Panel', {
        bodyPadding: 10,
        layout: 'anchor',
        height: 100,
        margin: '0 10 0 0',
        width: 600,
        border: false,
        items: [
            {
                xtype: 'combobox',
                id: 'searchStore',
                name: 'searchStore',
                fieldLabel: '條件',
                store:searchStore,
                editable: false,
                displayField: 'txt',
                valueField: 'value',
                value:'1',
            }
        ],
        buttonAlign: 'left',
        buttons: [
            {
                text: '查詢',
                handler: function () {
                    Query();
                }
            }
        ]
    });
    var PAD = Ext.create('Ext.grid.Panel', {
        id: 'PAD',
        store: PADStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.8,
        columns: [
            { header: "編號", dataIndex: 'id', width: 80, align: 'center' },
            { header: "活動名稱", dataIndex: 'name', width: 150, align: 'center' },
            { header: "會員群組", dataIndex: 'group_name', width: 165, align: 'center' },
            { header: "折扣", dataIndex: 'discount', width:80, align: 'center' },
            { header: "滿額金額", dataIndex: 'amount', width: 80, align: 'center' },
            { header: "滿額件數", dataIndex: 'quantity', width: 80, align: 'center' },
            { header: "web site設定", dataIndex: 'site_name', width:255, align: 'center' },
            { header: "開始時間", dataIndex: 'start', width: 150, align: 'center' },
            { header: "結束時間", dataIndex: 'end', width: 150, align: 'center' },
            { header: "創建人", dataIndex: 'create_user', width: 100, align: 'center' },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'update_user', width: 80, align: 'center' },
            {
                header: "是否啟用", dataIndex: 'active', align: '80', align: 'center', hidden: true,id:'controlActive',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value ==true) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    }
                    else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            },
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', hidden:true,handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, hidden: true, handler: onEditClick },
                { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, hidden: true, handler: onDeleteClick },
            '->',
            {
                xtype: 'combobox',
                id: 'searchStore',
                name: 'searchStore',
                fieldLabel: '條件',
                labelWidth:65,
                store: searchStore,
                editable: false,
                displayField: 'txt',
                valueField: 'value',
                value: '1',
            },
            {
                xtype: 'button',
                text: '查詢',
                handler: function () {
                    Query()
                }
            },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PADStore,
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
        layout: 'fit',
        items: [PAD],
        renderTo: Ext.getBody(),
       // autoScroll: true,
        listeners: {
            resize: function () {
                PAD.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //PADStore.load({ params: { start: 0, limit: 25 } });


});

function Query() {
    PADStore.removeAll();
    Ext.getCmp("PAD").store.loadPage(1, {
        params: {
            searchStore: Ext.getCmp('searchStore').getValue(),
        }
    });
}
onAddClick = function () {
    editFunction(null, PADStore);
}

onEditClick = function () {
    var row = Ext.getCmp("PAD").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        editFunction(row[0], PADStore);
    }
}

onDeleteClick = function () {
    var row = Ext.getCmp("PAD").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {
        Ext.Msg.confirm("確認信息", Ext.String.format("刪除選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/PromotionAmountDiscounts/Delete',
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
                        PADStore.load();
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "刪除失敗！");
                        PADStore.load();
                    }
                });
            }
        });
    }
}

function UpdateActive(row_id, muser) {
    var activeValue = $("#img" + row_id).attr("hidValue");
    $.ajax({
        url: "/PromotionAmountDiscounts/UpPADActive",
        data: {
            "id": row_id,
            "active": activeValue,
            "muser": muser,
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "msg") {
                Ext.Msg.alert("提示信息", QuanXianInfo);
            }
            else {
                if (activeValue == 1) {
                    PADStore.removeAll();
                    PADStore.load();
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                    PADStore.load();
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                    PADStore.load();
                }
            }
        },
        error: function (msg) {
            Ext.Msg.alert("提示信息", "操作失敗");
            PADStore.load();
        }
    });
}