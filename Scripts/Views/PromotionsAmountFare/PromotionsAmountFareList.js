
var pageSize = 25;

Ext.define('gigade.PromoAmountFareModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "event_id", type: "string" },
        { name: "name", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "url_by", type: "int" },
        { name: "banner_image", type: "string" },
        { name: "category_link_url", type: "string" },
        { name: "group_name", type: "string" },
        { name: "condition_name", type: "string" },
        { name: "delivery_store", type: "int" },
        { name: "typeName", type: "string" },
        { name: "event_type_name", type: "string" },
        { name: "event_type", type: "string" }, 
        { name: "payment_name", type: "string" },
        { name: "payment_code", type: "string" },
        { name: "deviceName", type: "string" },
        { name: "amount", type: "string" },
        { name: "quantity", type: "string" },
         { name: "site", type: "string" },
        { name: "site_name", type: "string" },
        { name: "start_time", type: "datetime" },
        { name: "end_time", type: "datetime" },
        { name: "class_id", type: "string" },
        { name: "brand_name", type: "string" },
        { name: "brand_id", type: "string" },
        { name: "condition_id", type: "string" },
        { name: "category_id", type: "string" },
        { name: "active", type: "string" },
        { name: "product_id", type: "int" },
        { name: "fare_percent", type: "int" },
        { name: "vendor_coverage", type: "int" },
        { name: "off_times", type: "int" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
});

var PromoAmountFareStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.PromoAmountFareModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAmountFare/GetPromoAmountFareList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFare").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdFare").down('#remove').setDisabled(selections.length == 0);

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
    PromoAmountFareStore.removeAll();
    Ext.getCmp("gdFare").store.loadPage(1, {
        params: {
            expiredSel: Ext.getCmp('expiredSel').getValue()
        }
    });
}

//加載前先獲取ddl的值
PromoAmountFareStore.on('beforeload', function () {
    Ext.apply(PromoAmountFareStore.proxy.extraParams, {
        expiredSel: Ext.getCmp('expiredSel').getValue()
    })

});


//頁面載入
Ext.onReady(function () {
    var gdFare = Ext.create('Ext.grid.Panel', {
        id: 'gdFare',
        store: PromoAmountFareStore,
        width: document.documentElement.clientWidth,
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
            { header: "id", dataIndex: 'id', width: 60, hidden: true, align: 'center' },
            { header: HDID, dataIndex: 'event_id', width: 65, align: 'center' },
            { header: ACTIVENAME, dataIndex: 'name', width: 130, align: 'center' },
            { header: ACTIVEDESC, dataIndex: 'event_desc', width: 100, align: 'center' },
            { header: ISBANNERURL, dataIndex: 'url_by', width: 120, align: 'center', renderer: IsBannerShow },
             { header: BANNERIMG,
                 dataIndex: 'banner_image',
                 width: 80,
                 align: 'center',
                 xtype: 'templatecolumn',
                 tpl: '<a target="_blank" href="{category_link_url}" ><img width=50 name="tplImg" onmousemove="javascript:imgFadeBig(this.src,250);" onmouseout = "javascript:$(\'#imgTip\').hide()" height=50 src="{banner_image}" /></a>'

             },
            { header: VIPGROUP, dataIndex: 'group_name', width: 100, align: 'center',
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
            { header: DELIVERYSTORE, dataIndex: 'delivery_store', width: 130, align: 'center', renderer: DeliverStoreShow },
            { header: YSCLASS, dataIndex: 'typeName', width: 130, align: 'center', renderer: BufenShow },
            { header: PROSORT, dataIndex: 'event_type_name', width: 130, align: 'center' },
            { header: PAYTYPE, dataIndex: 'payment_name', width: 130, align: 'center', renderer: BufenShow },
            { header: DEVICE, dataIndex: 'deviceName', width: 72, align: 'center', renderer: BufenShow },
            { header: BEGINTIME, dataIndex: 'start_time', width: 130, align: 'center' },
            { header: ENDTIME, dataIndex: 'end_time', width: 130, align: 'center' },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
              { header: ISACTIVE,
                  dataIndex: 'active',
                  id: 'controlactive',
                  hidden: true,
                  align: 'center',
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      if (value == "true") {
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
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
            { xtype: 'combobox', editable: false, fieldLabel: OPTION, labelWidth: 60, id: 'expiredSel', store: ExpireStore, displayField: 'txt', valueField: 'value', value: '0' },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PromoAmountFareStore,
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
        items: [gdFare],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFare.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
   // PromoAmountFareStore.load({ params: { start: 0, limit: 25} });
});

//添加
onAddClick = function () {
    editFunction(null, PromoAmountFareStore);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("gdFare").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], PromoAmountFareStore);

    }
}
//刪除
onRemoveClick = function () {
    var row = Ext.getCmp("gdFare").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/PromotionsAmountFare/DeletePromoAmountFare',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            PromoAmountFareStore.load();
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


function imgFadeBig(img, width, height) {
    var e = this.event;
    if (img.split('/').length != 5) {
        $("#imgTip").attr("src", img)
            .css({
                "top": (e.clientY < height ? e.clientY : e.clientY - height) + "px",
                "left": (e.clientX) + "px",
                "width": width + "px",
                "height": height + "px"
            }).show();
    }
}

function IsBannerShow(val) {
    switch (val) {
        case 0:
            return NO;
            break;
        case 1:
            return YES;
            break;

    }
}

function DeliverStoreShow(val) {
    switch (val) {
        case 1:
            return DELIVERDEFAULT;
            break;
    }
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




//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    //alert(activeValue);
    $.ajax({
        url: "/PromotionsAmountFare/UpdateActive",
        data: {
            "id": id,
            "muser": muser,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "stop") {
                Ext.Msg.alert("提示信息",QuanXianInfo);
            }
            else if (msg.success == "true") {
                PromoAmountFareStore.removeAll();
                PromoAmountFareStore.load();
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


