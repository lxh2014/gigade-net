var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model

Ext.define('gigade.PromoAmountGiftModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "event_ids", type: "string" },
        { name: "product_id", type: "int" },
        { name: "category_id", type: "int" },
        { name: "class_id", type: "int" }, 
        { name: "brand_id", type: "int" },
        { name: "name", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "url_by", type: "int" },
        { name: "count_by", type: "int" },
        { name: "banner_image", type: "string" },
        { name: "category_link_url", type: "string" },
        { name: "group_id", type: "int" },
        { name: "group_name", type: "string" },
        { name: "condition_id", type: "int" },
        { name: "num_limit", type: "int" },
        { name: "repeat", type: "bool" },
        { name: "freight", type: "string" },
        { name: "amount", type: "int" },
        { name: "quantity", type: "int" },
        { name: "deduct_welfare", type: "int" },
        { name: "event_type", type: "string" },
        { name: "gift_type", type: "int" },
        { name: "gift_id", type: "int" },
        { name: "payment", type: "string" },
        { name:"payment_code",type:"string"},
        { name: "devicename", type: "string" },
        { name: "startdate", type: "datetime" },
        { name: "enddate", type: "datetime" },
        { name: "use_start", type: "datetime" },
        { name: "use_end", type: "datetime" },
        { name: "newactive", type: "int" },
        { name: "vendor_coverage", type: "int" },
        { name: "gift_product_number", type: "int" },
        { name: "site", type: "string" },
        { name: "kuser", type: "int" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
});

var PromoAmountGiftStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.PromoAmountGiftModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAmountGift/GetPromoAmountGiftList',
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
            Ext.getCmp("gdGift").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdGift").down('#remove').setDisabled(selections.length == 0);
        }
    }
});


//定義ddl的數據
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": TEXPIRED, "value": "1" },
        { "txt": NOTPASTDUE, "value": "0" }
    ]
});

//加載前先獲取ddl的值
PromoAmountGiftStore.on('beforeload', function () {
    Ext.apply(PromoAmountGiftStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue()
    });
});

function Query(x) {
    PromoAmountGiftStore.removeAll();
    Ext.getCmp("gdGift").store.loadPage(1);
}
//頁面載入
Ext.onReady(function () {
    var gdGift = Ext.create('Ext.grid.Panel', {
        id: 'gdGift',
        store: PromoAmountGiftStore,
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
            { header: HDID, dataIndex: 'event_ids', width: 80, align: 'center', align: 'center' },
            { header: ACTIVENAME, dataIndex: 'name', width: 100, align: 'center' },
            { header: ACTIVEDESC, dataIndex: 'event_desc', width: 100, align: 'center' },
            { header: ISBANNERURL, dataIndex: 'url_by', width: 60, align: 'center', renderer: Banner },
            { header: BANNERIMG, dataIndex: 'banner_image',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{category_link_url}" ><img width=50 name="tplImg" onmousemove="javascript:imgFadeBig(this.src,250);" onmouseout = "javascript:$(\'#imgTip\').hide()" height=50 src="{banner_image}" /></a>'

            },
            { header: GROUPNAME, dataIndex: 'group_name', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.condition_id != 0) {
                        return VIPCONDITION;
                    }
                    else {
                        if (value == "") {
                            return BUFEN;
                        } else {
                            return value;
                        }
                    }
                }
            },
            { header: TIMELIMIT, dataIndex: 'count_by', width: 60, align: 'center', renderer: Count_byLimit },
            { header: ISREPEAT, dataIndex: 'repeat', width: 60, align: 'center', renderer: ChongfuYorN },
            { header: YSCLASS, dataIndex: 'freight', width: 80, align: 'center', renderer: BufenShow },
            { header: ALLMONEY, dataIndex: 'amount', width: 80, align: 'center' },
            { header: FAREQUANTITY, dataIndex: 'quantity', width: 80, align: 'center' },
            { header: PAYTYPE, dataIndex: 'payment', width: 80, align: 'center', renderer: BufenShow },
            { header: DEVICE, dataIndex: 'devicename', width: 80, align: 'center', renderer: BufenShow },
            { header: START, dataIndex: 'startdate', width: 120, align: 'center' },
            { header: END, dataIndex: 'enddate', width: 120, align: 'center' },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            {
                header: ISACTIVE,
                dataIndex: 'newactive',
                hidden: false,
                align: 'center',
                id: 'controlactive',
                hidden: true,
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
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
            { xtype: 'combobox', editable: false, fieldLabel: OPTION, labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', value: '0', listeners: {
                "select": function (combo, record) {
                    PromoAmountGiftStore.removeAll();
                    Ext.getCmp("gdGift").store.loadPage(1, {
                        params: {
                            ddlSel: Ext.getCmp('ddlSel').getValue()
                        }
                    });
                }
            }
            },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PromoAmountGiftStore,
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
        items: [gdGift],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdGift.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});
function Count_byLimit(val) {
    switch (val) {
        case 3:
            return BYMEMBER;
            break;
        case 2:
            return BYORDER;
            break;
        case 1:
            return NOTHING;
            break;
    }
}

function Banner(val) {
    switch (val) {
        case 0:
            return NO;
            break;
        case 1:
            return YES;
            break;
    }
}

function ChongfuYorN(val) {
    switch (val) {
        case true:
            return YES;
            break;
        case false:
            return NO;
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

//添加
onAddClick = function () {

    editFunction(null, PromoAmountGiftStore);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("gdGift").getSelectionModel().getSelection();

    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], PromoAmountGiftStore);
    }
}
//刪除
onRemoveClick = function () {
    var row = Ext.getCmp("gdGift").getSelectionModel().getSelection();
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
                    url: '/PromotionsAmountGift/DeletePromoAmountGift',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);

                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        PromoAmountGiftStore.load(1);
                        if (result.success) {
                            PromoAmountGiftStore.load(1);
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
        url: "/PromotionsAmountGift/UpdateActive",
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
                PromoAmountGiftStore.removeAll();
                PromoAmountGiftStore.load();
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
            Ext.Msg.alert(INFORMATION, EDITERROR);
        }
    });
} 
