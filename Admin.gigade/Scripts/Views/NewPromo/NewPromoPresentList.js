var pageSize = 25;
//var evnetId = document.getElementById("evnet_id").value;
//商品主料位管理Model
Ext.define('gigade.NewPromoPresent', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "trow_id", type: "int" },
        { name: "tgroup_id", type: "int" },
        { name: "event_id", type: "string" },
        { name: "gift_type", type: "string" },
        { name: "tstart", type: "string" },
        { name: "tend", type: "string" },
        { name: "ticket_name", type: "string" },
        { name: "ticket_serial", type: "string" },
        { name: "gift_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "deduct_welfare", type: "string" },
        { name: "gift_amount", type: "string" },
        { name: "gift_amount_over", type: "string" },
        { name: "freight_price", type: "string" },
        { name: "status", type: "string" },
        { name: "welfare_mulriple", type: "double" },
        { name: "group_name", type: "string" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' },
        { name: 'bonus_expire_day', type: 'int' } //購物金抵用券有效天數


    //{ name: "isSame", type: "string" }
    ]
});


var PromoRresentStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.NewPromoPresent',
    proxy: {
        type: 'ajax',
        url: '/NewPromo/GetNewPromoPresentList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
PromoRresentStore.on('beforeload', function () {
    Ext.apply(PromoRresentStore.proxy.extraParams, {
        entId: Ext.getCmp('entId').getValue()
    });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("gbNewPromoPresent").getSelectionModel().getSelection();
            Ext.getCmp("gbNewPromoPresent").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gbNewPromoPresent").down('#remove').setDisabled(selections.length == 0);
        }
    }
});

function Query(x) {
    if (Ext.getCmp('entId').getValue().trim() != "") {
        PromoRresentStore.removeAll();
        Ext.getCmp("gbNewPromoPresent").store.loadPage(1, {
            params: {
                entId: Ext.getCmp('entId').getValue()

            }
        })
    } else {
        Ext.Msg.alert(INFORMATION, "请输入搜索内容!");
    }
}

Ext.onReady(function () {
    var gbNewPromoPresent = Ext.create('Ext.grid.Panel', {
        id: 'gbNewPromoPresent',
        store: PromoRresentStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: ROWID, dataIndex: 'trow_id', width: 90, align: 'center' },
            { header: EVENTID, dataIndex: 'event_id', width: 90, align: 'center' },
            {
                 header: USERGROUP, dataIndex: 'group_name', width: 90, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == "" || value == null) {
                         return NOSELECT;
                     }
                     else {
                         return value;
                     }
                 }
             },
            {
                header: GIFTTIPE, dataIndex: 'gift_type', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return PRODUCT;
                    }
                    else if (value == 2) {
                        return BONUS;
                    }
                    else if (value == 3) {
                        return DIYKONG;
                    }
                    else {
                        return value;
                    }
                }
            },

            {
                header: GIFTNAME, dataIndex: 'gift_type', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return record.data.product_name;
                    }
                    else {
                        return record.data.ticket_name;
                    }
                }
            },
            { header: BONUSWELFARE, dataIndex: 'deduct_welfare', width: 90, align: 'center' },
            { header: GIFTNUM, dataIndex: 'gift_amount', width: 90, align: 'center' },
            { header: FREIGHTPRICE, dataIndex: 'freight_price', width: 90, align: 'center' },
             { header: DATESTART, dataIndex: 'tstart', width: 150, align: 'center' },
            { header: DATEEND, dataIndex: 'tend', width: 150, align: 'center' },
            //{
            //    header: "與活動時間是否一致", dataIndex: 'isSame', width: 135, align: 'center', renderer: function (value) {
            //        if (value == "是") {
            //            return "<span style='color:green'>是</span>";
            //        }
            //        else {
            //            return "<span style='color:red'>否</span>";
            //        }
            //    }
            //},
            { header: '購物金抵用券有效天數', dataIndex: 'bonus_expire_day', width: 150, align: 'center' },
             { header: 'muser', dataIndex: 'muser', hidden: true },
             { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            {
                header: ACTIVE,
                dataIndex: 'status',
                id: 'status',
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "true") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.trow_id + "," + record.data.muser + ")'><img hidValue='0' id='img" + record.data.trow_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.trow_id + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.trow_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, disabled: true, id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', handler: onEditClick },
             { xtype: 'button', text: REMOVE, id: 'remove', disabled: true, iconCls: 'icon-user-remove', handler: onRemoveClick },
             '->',
              {
                  xtype: 'textfield',
                  id: 'entId',
                  margin: '0 5px',
                  name: 'entId',
                  fieldLabel: '活動編號',
                  labelWidth: 70,
                  listeners: {
                      specialkey: function (field, e) {
                          if (e.getKey() == Ext.EventObject.ENTER) {
                              Query();
                          }
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
            store: PromoRresentStore,
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
        items: [gbNewPromoPresent],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gbNewPromoPresent.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    QueryToolAuthorityByUrl('/NewPromo/NewPromoPresent');
    //PromoRresentStore.load({ params: { start: 0, limit: 25 } });
});
onAddClick = function () {
    editPresentFunction(null, PromoRresentStore, null);
}
onEditClick = function () {
    var row = Ext.getCmp("gbNewPromoPresent").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editPresentFunction(row[0], PromoRresentStore, null);
    }
}

//更改贈品狀態(設置贈品可用與不可用,當禁用贈品時將判斷該贈品是否是最後一個有效贈品，若是則提示將會同時禁用活動，并執行)
function UpdateActive(id,muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    var giftNum = 0;
    var bool = true;
    $.ajax({
        url: "/NewPromo/UpdateNewPromoPresentActive",
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
                return;
            }
            else if (msg.success == "true") {
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
            PromoRresentStore.load();
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}

onRemoveClick = function () {
    var giftNum = 0;//該活動的有效贈品數
    var activeNum = 0;
    var row = Ext.getCmp("gbNewPromoPresent").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.trow_id + '|';
                }
                //獲取該活動的有效贈品數
                Ext.Ajax.request({
                        url: '/NewPromo/DeleteNewPromoPresent',
                        method: 'post',
                        params: { rowID: rowIDs },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                PromoRresentStore.load();
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