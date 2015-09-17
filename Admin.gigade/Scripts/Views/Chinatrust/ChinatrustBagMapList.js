var pageSize = 11;
/***********************群組管理主頁面****************************/
//群組管理Model
Ext.define('gigade.ChinatrustMapBag', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "map_id", type: "int" },
        { name: "bag_id", type: "string" },
        { name: "product_id", type: "int" },
        { name: "linkurl", type: "string" },
        { name: "link", type: "string" },
        { name: "product_forbid_banner", type: "string" },
        { name: "product_active_banner", type: "string" },
        { name: "forbid_banner", type: "string" },
        { name: "active_banner", type: "string" },
        { name: "product_name", type: "string" },
        { name: "map_active", type: "int" },
        { name: "map_sort", type: "int" },
        { name: "event_name", type: "string" },
        { name: "bag_name", type: "string" },
        { name: "ad_product_id", type: "string" },
        { name: "product_desc", type: "string" }
    ]
});
var ChinatrustMapBag = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.ChinatrustMapBag',
    proxy: {
        type: 'ajax',
        url: '/Chinatrust/GetChinaTrustBagMapList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});

//下拉框
Ext.define("gigade.Parametersrc", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "bag_id", type: "string" },
        { name: "bag_name", type: "string" }]
});
var DDRStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Parametersrc',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Chinatrust/GetBag",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

var searchStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
          { 'txt': '請選擇...', 'value': '0' },
          { 'txt': '活動名稱', 'value': '1' },
          { 'txt': '區域包名稱', 'value': '2' },
    ]
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
ChinatrustMapBag.on('beforeload', function () {
    Ext.apply(ChinatrustMapBag.proxy.extraParams, {
        bagid: document.getElementById("bag_id").value,
        search_con: Ext.getCmp('search_con').getValue(),
        con: Ext.getCmp('con').getValue()
    });
});
//勾選框
function Query(x) {
    if (Ext.getCmp('search_con').getValue() != "0" && Ext.getCmp('con').getValue().trim() != "") {
        ChinatrustMapBag.removeAll();
        Ext.getCmp("gdFgroup").store.loadPage(1, {
            params: {
                search_con: Ext.getCmp('search_con').getValue(),
                con: Ext.getCmp('con').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, "請選擇搜索條件!");
    }
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height:45,
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
                        id: 'search_con',
                        name: 'search_con',
                        fieldLabel: '查詢條件',
                        displayField: 'txt',
                        valueField: 'value',
                        labelWidth:70,
                        editable:false,
                        store: searchStore,
                        value:'0'
                    },
                     {
                         xtype: 'textfield',
                         id: 'con',
                         name: 'con',
                         margin: '0 0 0 2',
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == Ext.EventObject.ENTER) {
                                     Query();
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
                                Ext.getCmp('search_con').setValue(0);
                                Ext.getCmp('con').setValue('');
                            }
                        }
                    }
                ]
            }
        ]
    });
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: ChinatrustMapBag,
        flex: 1.8,
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
            { header: "編號", dataIndex: 'map_id', width: 80, align: 'center' },
            { header: "區域包編號", dataIndex: 'bag_id', width: 80, align: 'center' },
            { header: "區域包名稱", dataIndex: 'bag_name', width: 80, align: 'center' },
            { header: "活動名稱", dataIndex: 'event_name', width: 80, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 150, align: 'center' },
            { header: "商品鏈接", dataIndex: 'linkurl', width: 200, align: 'center' },
            { header: "排序", dataIndex: 'map_sort', width: 80, align: 'center' },
            { header: "廣告商品編號", dataIndex: 'ad_product_id', width: 200, align: 'center' },
            { header: "商品描述", dataIndex: 'product_desc', width: 150, align: 'center' },
            { header: "product_forbid_banner", dataIndex: 'product_forbid_banner', width: 50, align: 'center', hidden: true },
            { header: "product_active_banner", dataIndex: 'product_active_banner', width: 50, align: 'center', hidden: true },
            {
                header: "商品失效圖片", dataIndex: 'forbid_banner', width: 150, align: 'center', width: 100,
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{forbid_banner}" ><img width=50 name="tplImg"  height=50 src="{forbid_banner}" /></a>'
            },
            {
                header: "商品有效圖片", dataIndex: 'active_banner', width: 150, align: 'center', width: 100,
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{active_banner}" ><img width=50 name="tplImg"  height=50 src="{active_banner}" /></a>'
            },
            {
                header: "商品關係狀態",dataIndex: 'map_active',align: 'center',id: 'articlestatus',hidden: false,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.map_id + ")'><img hidValue='0' id='img" + record.data.map_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.map_id + ")'><img hidValue='1' id='img" + record.data.map_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', handler: onEditClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ChinatrustMapBag,
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
        layout: 'vbox',//fit
        items: [frm,gdFgroup],
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
    //ChinatrustMapBag.load({ params: { start: 0, limit: pageSize } });
});

/***************************新增***********************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, ChinatrustMapBag);
}
/*********************編輯**********************/
onEditClick = function (question_id, order_id) {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ChinatrustMapBag);
    }
}
/*********************啟用/禁用**********************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Chinatrust/UpdateStats",
        data: {
            "id": id,
            "status": activeValue
        },
        type: "post",
        type: 'text',
        success: function (msg) {
            //Ext.Msg.alert(INFORMATION, "修改成功!");
            ChinatrustMapBag.load();
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

