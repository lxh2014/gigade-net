var pageSize = 25;
var event_type = "";

Ext.define('gigade.Discount', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "event_id", type: "string" },
        { name: "id", type: "int" },
        { name: "name", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "class_id", type: "int" },
        { name: "class_name", type: "string" },
         { name: "brand_id", type: "int" },
        { name: "brand_name", type: "string" },
        { name: "group_name", type: "string" },
        { name: "condition_id", type: "int" },
        { name: "condition_name", type: "string" },
        { name: "category_id", type: "int" },
        { name: "banner_image", type: "string" },
        { name: "category_link_url", type: "string" },
        { name: 'devicename', type: 'string' },
        { name: "vendor_coverage", type: "int" },
        { name: "active", type: "int" },
        { name: "site", type: "string" },
        { name: "siteId", type: "string" },
        { name: "url_by", type: "int" },
        { name: "startdate", type: "datetime" },
        { name: "enddate", type: "datetime" },
        { name: "amount", type: "int" },
        { name: "quantity", type: "int" },
        { name: "discount", type: "int" },
        { name: "product_id", type: "int" },
        { name: "isallclass", type: "int" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
}); 

var DiscountStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Discount',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/PromotionsDiscount/DiscountZheList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//獲取數據行數
        }
    }
})

//加載前先獲取ddl的值
DiscountStore.on('beforeload', function () {
    Ext.apply(DiscountStore.proxy.extraParams, {
        expiredSel: Ext.getCmp('expiredSel').getValue(),
        isurl: Ext.getCmp('isurl').getValue()
    })

});

function Query() {
    DiscountStore.removeAll();
    Ext.getCmp("gdDiscount").store.loadPage(1);
}

Ext.onReady(function () {
    event_type = document.getElementById("condition_name").value;

    //前面選擇框 選擇之後顯示編輯刪除
    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("gdDiscount").down('#edit').setDisabled(selections.length == 0);
                Ext.getCmp("gdDiscount").down('#remove').setDisabled(selections.length == 0);
            }
        }
    });
    var gdDiscount = Ext.create('Ext.grid.Panel', {
        id: "gdDiscount",
        store: DiscountStore,
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
            { header: EVENTID, dataIndex: 'event_id', width: 100, align: 'center', align: 'center' },
            { header: 'id', dataIndex: 'id', width: 100, hidden: true, align: 'center' },
            { header: 'isallclass', dataIndex: 'isallclass', width: 100, hidden: true, align: 'center' },
            { header: ACTIVENAME, dataIndex: 'name', width: 100, align: 'center' },
            { header: ACTIVEDESC, dataIndex: 'event_desc', width: 100, align: 'center' },
            { header: SHOPCLASS, dataIndex: 'class_name', width: 130, align: 'center' },
            { header: BLANDNAME, dataIndex: 'brand_name', width: 120, align: 'center' },
            {
                header: GROUPNAME, dataIndex: 'group_name', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.condition_id != "0") {
                        return VIPCONDITION;
                    }
                    else if (value == "") {
                        return BUFEN;
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: VIPCONDITION, dataIndex: 'condition_name', width: 80, align: 'center', hidden: true },
            { header: 'condition_id', dataIndex: 'condition_id', hidden: false, width: 80, align: 'center', hidden: true },
            { header: 'category_id', dataIndex: 'category_id', hidden: true, width: 80, align: 'center' },
            {
                header: BANNERIMG,
                dataIndex: 'banner_image',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{category_link_url}" ><img width=50 name="tplImg"  height=50 src="{banner_image}" /></a>'
            },
            { header: DEVICE, dataIndex: 'devicename', width: 80, align: 'center', renderer: BufenShow },
            { header: VENDORCOVERAGE, dataIndex: 'vendor_coverage', width: 80, align: 'center' },
            { header: WEBSITESET, dataIndex: 'site', width: 80, align: 'center', renderer: BufenShow },
            { header: START, dataIndex: 'startdate', width: 125, align: 'center' },
            { header: END, dataIndex: 'enddate', width: 125, align: 'center' },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            {
                header: '是否啟用',
                dataIndex: 'active',
                id: 'controlactive',
                hidden: true,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: DEL, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
            { xtype: 'combobox', editable: false, fieldLabel: OPTION, labelWidth: 60, id: 'expiredSel', store: ExpireZheStore, displayField: 'txt', valueField: 'value', value: '0' },
            { xtype: 'combobox', editable: false, fieldLabel: "是否專區", labelWidth: 60, id: 'isurl', store: UrlZheStore, displayField: 'txt', valueField: 'value', value: '0' },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DiscountStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm//前面選擇框
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdDiscount],
        renderTo: Ext.getBody()
    });
    ToolAuthority();
});

/*************************************************************************************新增*************************************************************************************************/




onAddClick = function () {
    fewDiscountForm("", DiscountStore, event_type, "Add");
}
/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdDiscount").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        fewDiscountForm(row[0], DiscountStore, event_type, "Edit");
    }
}

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("gdDiscount").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/PromotionsDiscount/Deletezhe',
                    method: 'post',
                    params: { rowID: rowIDs, event_type: event_type },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            DiscountStore.load();

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

//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/PromotionsDiscount/UpdateActive",
        data: {
            "id": id,
            "muser": muser,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "stop") {
                Ext.Msg.alert("提示信息", QuanXianInfo);
            }
            else if (msg.success == "true") {
                DiscountStore.removeAll();
                DiscountStore.load();
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
            }
            else {
                Ext.Msg.alert("提示信息", "更改失敗");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}


function BufenShow(val) {
    switch (val) {
        case "":
            return BUFEN;
            break;
        default:
            return val;
            break;
    }
}

