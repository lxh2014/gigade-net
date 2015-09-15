
var pageSize = 25;
//物流信息Model
Ext.define('gigade.Logistics', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rid", type: "int" },
        { name: "delivery_store_id", type: "int" },
         { name: "delivery_store_name", type: "string" },
        { name: "freight_big_area", type: "int" },
        { name: "freight_type", type: "int" },
        { name: "freight_big_area_name", type: "string" },
        { name: "freight_type_name", type: "string" },
        { name: "delivery_freight_set", type: "int" },
        { name: "active", type: "int" },
        { name: "charge_type", type: "int" },
        { name: "shipping_fee", type: "int" },
        { name: "return_fee", type: "int" },
        { name: "size_limitation", type: "int" },
        { name: "len", type: "int" },
        { name: "wid", type: "int" },
        { name: "hei", type: "int" },
        { name: "wei", type: "int" },
        { name: "pod", type: "int" },
        { name: "note", type: "string" }
    ]
});
//搜索框model
Ext.define("gigade.logisticsName1", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }]
});

//搜索框物流數據
var logisticsNameStore1 = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.logisticsName1',
    proxy: {
        type: 'ajax',
        url: '/Logistics/GetLogisticsName',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
    ,
    autoLoad: true
});

var LogisticsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Logistics',
    proxy: {
        type: 'ajax',
        url: '/Logistics/LoadList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

LogisticsStore.on('beforeload', function () {
    Ext.apply(LogisticsStore.proxy.extraParams,
        {
            searchcontent: Ext.getCmp('searchcontent').getValue()
        });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdLogistics").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdLogistics").down('#delete').setDisabled(selections.length == 0);
        }
    }
});


function Query() {
    if (Ext.getCmp('searchcontent').getValue() != 0) {
        LogisticsStore.removeAll();
        Ext.getCmp("gdLogistics").store.loadPage(1);
    } else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}

Ext.onReady(function () {
    var gdLogistics = Ext.create('Ext.grid.Panel', {
        id: 'gdLogistics',
        store: LogisticsStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: RID, dataIndex: 'rid', width: 120, hidden: false, align: 'center' },
            { header: ID, dataIndex: 'delivery_store_name', width: 150, align: 'center' },
            { header: AREA, dataIndex: 'freight_big_area_name', width: 70, align: 'center' },
            { header: TYPE, dataIndex: 'freight_type_name', width: 90, align: 'center' },
            {
                header: OFTENLOW, dataIndex: 'delivery_freight_set', width: 54, align: 'center',
                renderer: function (value) {
                    if (value == "1") {
                        return ROOM;
                    }
                    else if (value == "2") {
                        return MICROTHERM;
                    }
                }
            },
            {
                header: CHARGEMETHOD, dataIndex: 'charge_type', width: 60, align: 'center',
                renderer: function (value) {
                    if (value == "1") {
                        return "固定";
                    }
                    else if (value == "2") {
                        return "累加";
                    }
                }
            },
            { header: FARE, dataIndex: 'shipping_fee', width: 70, align: 'center' },
            { header: INVERSEFARE, dataIndex: 'return_fee', width: 70, align: 'center' },
            {
                header: RESTRICT, dataIndex: 'size_limitation', width: 100, align: 'center',
                renderer: function (value) {
                    if (value == "0") {
                        return "無";
                    }
                    else if (value == "1") {
                        return "有";
                    }
                }
            },
            { header: LENGTH, dataIndex: 'len', width: 70, align: 'center' },
            { header: HEIGHT, dataIndex: 'wid', width: 70, align: 'center' },
            { header: WIDTH, dataIndex: 'hei', width: 70, align: 'center' },
            { header: WEIGHT, dataIndex: 'wei', width: 70, align: 'center' },
            {
                header: CASHONDELIVERY, dataIndex: 'pod', width: 64, align: 'center',
                renderer: function (value) {
                    if (value == "0") {
                        return "否";
                    }
                    else if (value == "1") {
                        return "是";
                    }
                }
            },
            { header: NOTE, dataIndex: 'note', width: 120, align: 'center' },
            {
                header: ENABLE, dataIndex: 'active', width: 60, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {

                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rid + ")'><img hidValue='0' id='img" + record.data.rid + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {

                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rid + ")'><img hidValue='1' id='img" + record.data.rid + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           { xtype: 'button', text: DELETE, id: 'delete', hidden: false, iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick },

           '->',
           {
               xtype: 'combobox',
               editable: false,
               fieldLabel: "物流名稱",
               labelWidth: 70,
               id: 'searchcontent',
               lastQuery: "",
               name: 'searchcontent',
               store: logisticsNameStore1,
               displayField: 'parameterName',
               valueField: 'parameterCode',
               emptyText: '請選擇...',
               value: 0
           },
           {
               xtype: 'button',
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
          {
              xtype: 'button',
              text: RESET,
              id: 'btn_reset',
              listeners: {
                  click: function () {
                      Ext.getCmp("searchcontent").setValue("");
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: LogisticsStore,
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
        items: [gdLogistics],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdLogistics.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // LogisticsStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, LogisticsStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdLogistics").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], LogisticsStore);
    }
}

/*************************************************************************************更改狀態******************************************************************************************************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Logistics/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            LogisticsStore.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            alert("修改失敗");
        }
    })
}

/*************************************************************************************刪除******************************************************************************************************/
onDeleteClick = function () {
    var row = Ext.getCmp("gdLogistics").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.rid + ',';
                }
                Ext.Ajax.request({
                    url: '/Logistics/DeleteShippingCarriorById',//執行方法
                    method: 'post',
                    params: { rid: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "刪除成功!");
                            LogisticsStore.loadPage(1);
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "無法刪除!");
                            LogisticsStore.loadPage(1);
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





