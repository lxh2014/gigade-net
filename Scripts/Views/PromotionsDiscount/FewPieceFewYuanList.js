
var pageSize = 25;
var event_type_list = "";
Ext.define('gigade.Price', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "url_by", type: "int" },
        { name: "event_id", type: "string" },
        { name: "id", type: "int" },
        { name: "name", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "class_name", type: "string" },
        { name: "class_id", type: "int" },
        { name: "brand_name", type: "string" },
        { name: "group_name", type: "string" },
        { name: "condition_id", type: "int" },
        { name: "condition_name", type: "string" },
        { name: "category_id", type: "int" },
        { name: "banner_image", type: "string" },
        { name: "category_link_url", type: "string" },
        { name: 'devicename', type: 'string' },
        { name: "site", type: "string" },
        { name: "siteId", type: "string" },
        { name: "startdate", type: "datetime" },
        { name: "enddate", type: "datetime" },
        { name: "vendor_coverage", type: "int" },
        { name: "active", type: "int" },
        { name: "isallclass", type: "int" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
}); 

var PriceStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Price',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/PromotionsDiscount/DiscountList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//獲取數據行數
        }
    }
});
//定義ddl的數據
var ExpireStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": NOTPASTDUE, "value": "0" },
    { "txt": TEXPIRED, "value": "1" }
    ]
})
function Query() {
    PriceStore.removeAll();
    Ext.getCmp("gdPrice").store.loadPage(1);
}

//加載前先獲取ddl的值
PriceStore.on('beforeload', function () {
    Ext.apply(PriceStore.proxy.extraParams, {
        expiredSel: Ext.getCmp('expiredSel').getValue()
    })

});


Ext.onReady(function () {
    event_type_list = document.getElementById("condition_name").value;


    //前面選擇框 選擇之後顯示編輯刪除
    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("gdPrice").down('#edit').setDisabled(selections.length == 0);
                Ext.getCmp("gdPrice").down('#remove').setDisabled(selections.length == 0);
            }
        }
    });
    var gdPrice = Ext.create('Ext.grid.Panel', {
        id: "gdPrice",
        store: PriceStore,
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
            { header: EVENTID, dataIndex: 'event_id', width: 100, align: 'center' },
            { header: 'id', dataIndex: 'id', width: 100, hidden: true, align: 'center' },
            { header: 'isallclass', dataIndex: 'isallclass', width: 100, hidden: true, align: 'center' },
            { header: ACTIVENAME, dataIndex: 'name', width: 120, align: 'center' },
            { header: ACTIVEDESC, dataIndex: 'event_desc', flex: 1, align: 'center' },
            { header: CLASSNAME, dataIndex: 'class_name', width: 80, align: 'center' },
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
            { header: 'condition_id', dataIndex: 'condition_id', hidden: false, width: 80, align: 'center', hidden: true },
            { header: VIPCONDITION, dataIndex: 'condition_name', width: 80, align: 'center', hidden: true },
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
            { header: WEBSITESET, dataIndex: 'site', flex: 1, align: 'center', renderer: BufenShow },
            { header: START, dataIndex: 'startdate', width: 120, align: 'center' },
            { header: END, dataIndex: 'enddate', width: 120, align: 'center' },
            { header: 'vendor_coverage', dataIndex: 'vendor_coverage', hidden: true },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            {
                header: ISACTIVE,
                dataIndex: 'active',
                id: 'controlactive',
                hidden: true,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id +","+record.data.muser+ ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'ui-icon ui-icon-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'ui-icon ui-icon-pencil', disabled: true, handler: onEditClick },
            { xtype: 'button', text: DEL, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
            {
                xtype: 'combobox', editable: false, fieldLabel: OPTION, labelWidth: 60, id: 'expiredSel', store: ExpireStore, displayField: 'txt', valueField: 'value', value: '0'

            },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PriceStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm//前面選擇框
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdPrice],
        renderTo: Ext.getBody()
    });
    ToolAuthority();
});



/*************************************************************************************新增*************************************************************************************************/

onAddClick = function () {

    fewYuanForm("", PriceStore, event_type_list, "Add");
}
/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdPrice").getSelectionModel().getSelection();

    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        fewYuanForm(row[0], PriceStore, event_type_list, "Edit");
    }
}

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("gdPrice").getSelectionModel().getSelection();
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
                    params: { rowID: rowIDs, event_type: event_type_list },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            PriceStore.load();
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
function UpdateActive(id,muser) {
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
            if(msg.success=="stop")
            {
                Ext.Msg.alert("提示信息", QuanXianInfo);
            }
            else if (msg.success == "true") {
                PriceStore.load();
                PriceStore.load();
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

