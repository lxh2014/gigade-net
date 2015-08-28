var CallidForm;
var pageSize = 25;
//商品主料位管理Model
Ext.define('gigade.PaperClass', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "string" },
        { name: "paperID", type: "string" },
        { name: "paperName", type: "string" },
        { name: "classID", type: "string" },
        { name: "className", type: "string" },
        { name: "classType", type: "string" },
        { name: "projectNum", type: "string" },
        { name: "classContent", type: "string" },
        { name: "orderNum", type: "string" },
        { name: "isMust", type: "string" },
        { name: "status", type: "string" }
    ]
});

var PaperClassStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.PaperClass',
    proxy: {
        type: 'ajax',
        url: '/Paper/GetPaperClassList',
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
            Ext.getCmp("gdPaperClass").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdPaperClass").down('#suse').setDisabled(selections.length == 0);
            Ext.getCmp("gdPaperClass").down('#fuse').setDisabled(selections.length == 0);
        }
    }
});

Ext.define('gigade.Paper', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "paperID", type: "int" },
        { name: "paperName", type: "string" }]
});
var PaperStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    //pageSize: pageSize,
    model: 'gigade.Paper',
    proxy: {
        type: 'ajax',
        url: '/Paper/GetPaperList?isPage=false',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
            //totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

PaperClassStore.on('beforeload', function () {
    Ext.apply(PaperClassStore.proxy.extraParams, {
        paper_id: Ext.getCmp('paper') == null ? "" : Ext.getCmp('paper').getValue()
    });
});


Ext.onReady(function () {
    var gdPaperClass = Ext.create('Ext.grid.Panel', {
        id: 'gdPaperClass',
        store: PaperClassStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'id', width: 80, align: 'center' },
           // { header: "問卷編號", dataIndex: 'paperID', width: 80, align: 'center' },
            { header: "問卷名稱", dataIndex: 'paperName', width: 150, align: 'center' },
            { header: "題目編號", dataIndex: 'classID', width: 80, align: 'center' },
            { header: "題目名稱", dataIndex: 'className', width: 150, align: 'center' },
            {
                header: "題目類型", dataIndex: 'classType', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "SC":
                            return "單選";
                        case "MC":
                            return "多選";
                        case "SL":
                            return "單行";
                        case "ML":
                            return "多行";
                    }
                }
            },
            { header: "題目順序", dataIndex: 'projectNum', width: 80, align: 'center' },
            { header: "選項內容", dataIndex: 'classContent', width: 200, align: 'center' },
            { header: "選項排序", dataIndex: 'orderNum', width: 80, align: 'center' },
            {
                header: "是否必填", dataIndex: 'isMust', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return "是";
                    }
                    else {
                        return "否";
                    }
                }
            },
            {
                header: "狀態", dataIndex: 'status', width: 50, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
         { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
         { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
         { xtype: 'button', text: "批量啟用", id: 'suse', hidden: false, disabled: true, handler: onSUseClick },
         { xtype: 'button', text: "批量禁用", id: 'fuse', hidden: false, disabled: true, handler: onFUseClick },
         '->',
         {
             xtype: 'combobox', editable: false, fieldLabel: "問卷名稱", labelWidth: 60, id: 'paper', store: PaperStore, queryMode: 'remote', lastQuery: '', displayField: 'paperName', valueField: 'paperID'
             //, listeners: {
             //    render: function () {
             //        PaperStore.load({
             //            callback: function () {
             //                PaperStore.insert(0, { paperID: '0', paperName: '請選擇' });
             //                Ext.getCmp("paper").setValue(0);
             //            }
             //        });
             //    }
             //}
         },
         //{ xtype: 'combobox', editable: false, fieldLabel: "題目", labelWidth: 55, id: 'title', store: PaperStore, displayField: 'paperName', valueField: 'paperID', value: 0 },
         {
             text: SEARCH,
             iconCls: 'icon-search',
             id: 'btnQuery',
             handler: Query
         }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PaperClassStore,
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
        items: [gdPaperClass],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdPaperClass.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //PaperClassStore.load({ params: { start: 0, limit: 25 } });
    PaperStore.load({
        callback: function () {
            PaperStore.insert(0, { paperID: '0', paperName: '請選擇' });
             Ext.getCmp("paper").setValue(0);
        }
    });
});
/*************************************************************************************查詢*************************************************************************************************/
function Query(x) {
    if (Ext.getCmp('paper').getValue() != "0") {
        PaperClassStore.removeAll();
        Ext.getCmp("gdPaperClass").store.loadPage(1, {
            params: {
                paper_id: Ext.getCmp('paper') == null ? "" : Ext.getCmp('paper').getValue()

            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, "請選擇搜索條件!");
    }
}

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editPaperClassFunction(null, PaperClassStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdPaperClass").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editPaperClassFunction(row[0], PaperClassStore);
    }
}
/*************************************************************批量啟用*************************************/

onSUseClick = function () {
    var row = Ext.getCmp("gdPaperClass").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length >= 1) {
        var ids = "";
        for (var i = 0; i < row.length; i++) {
            ids += row[i].data.id + ',';
        }
        Ext.Ajax.request({
            url: "/Paper/UpdateClassState",
            method: 'post',
            params: {
                id: ids,
                "active": 1
            },
            success: function (msg) {
                for (var i = 0; i < row.length; i++) {
                    $("#img" + row[i].data.id).attr("hidValue", 0);
                    $("#img" + row[i].data.id).attr("src", "../../../Content/img/icons/accept.gif");
                }
                ProPromoStore.load();
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);

            }
        });
    }
}
/*************************************************************批量禁用*************************************/
onFUseClick = function () {
    var row = Ext.getCmp("gdPaperClass").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length >= 1) {
        var ids = "";
        for (var i = 0; i < row.length; i++) {
            ids += row[i].data.id + ',';
        }
        Ext.Ajax.request({
            url: "/Paper/UpdateClassState",
            method: 'post',
            params: {
                id: ids,
                "active": 0
            },
            success: function (msg) {
                for (var i = 0; i < row.length; i++) {
                    $("#img" + row[i].data.id).attr("hidValue", 1);
                    $("#img" + row[i].data.id).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
                ProPromoStore.load();
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);

            }
        });
    }
}
//更改狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Paper/UpdateClassState",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                PaperClassStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                PaperClassStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            PaperClassStore.load();
        }
    });
}






