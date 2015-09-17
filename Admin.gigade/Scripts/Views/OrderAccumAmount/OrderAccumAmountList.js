var CallidForm;
var pageSize = 25;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//促銷贈品表Model
Ext.define('gigade.OrderAccumAmount', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "event_id", type: "string" },//編號
        { name: "event_status", type: "int" },//活動狀態
        { name: "event_start_time", type: "string" },//活動開始時間
        { name: "event_end_time", type: "string" },//活動結束時間
        { name: "event_desc", type: "string" },//活動描述
        { name: "event_name", type: "string" },//活動名稱
        { name: "event_desc_start", type: "string" },//描述結束時間
        { name: "event_desc_end", type: "string" },//描述結束時間
        { name: "update_user", type: "string" },//修改人
        { name: "event_update_time", type: "string" },//修改時間
        { name: "accum_amount", type: "int" }//累積金額
    ]
});
var OrderAccumAmountStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.OrderAccumAmount',
    proxy: {
        type: 'ajax',
        url: '/OrderAccumAmount/GetOrderAccumAmountList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("OrderAccumAmount").getSelectionModel().getSelection();
            Ext.getCmp("OrderAccumAmount").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("OrderAccumAmount").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "-1" },
        { "txt": "啟用", "value": "1" },
        { "txt": "禁用", "value": "0" }
    ]
});
OrderAccumAmountStore.on('beforeload', function () {

    Ext.apply(OrderAccumAmountStore.proxy.extraParams, {
        event_status: Ext.getCmp('ddlSel').getValue(),
        event_search: Ext.getCmp('o_id').getValue(),
        TimeStart: Ext.getCmp('start_time').getValue(),//課程開始時間
        TimeEnd: Ext.getCmp('end_time').getValue()//課程結束時間
    })

});
function Query(x) {
    OrderAccumAmountStore.removeAll();
    Ext.getCmp("OrderAccumAmount").store.loadPage(1);

}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 100,
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
                               xtype: 'textfield',
                               id: 'o_id',
                               margin: '0 5px',
                               name: 'o_id',
                               fieldLabel: '編號/活動名稱',
                               labelWidth: 90,
                               listeners: {
                                   specialkey: function (field, e) {
                                       if (e.getKey() == Ext.EventObject.ENTER) {
                                           Query();
                                       }
                                   }
                               }
                           },
                           {
                               xtype: 'combobox',
                               editable: false,
                               fieldLabel: "狀態",
                               labelWidth: 30,
                               id: 'ddlSel',
                               store: DDLStore,
                               displayField: 'txt',
                               valueField: 'value',
                               value: '-1'
                           }
                           
                   ]
               },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                       {
                           xtype: 'label',
                           width: 74,
                           margin: '0 5px',
                           text: '活動時間'

                       },
                       {
                           xtype: "datetimefield",
                           margin: '0 5,0,5',
                           editable: false,
                           id: 'start_time',
                           name: 'start_time',
                           format: 'Y-m-d 00:00:00',
                           value: Tomorrow(1 - new Date().getDate()),
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   var s_date = new Date(start.getValue());

                                   end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                   /*搜索时间加一个月限制
                                   var data1 = Date.parse(start.getValue());
                                   var data2 = Date.parse(end.getValue());
                                   var datadiff = data2 - data1;
                                   var time = 31 * 24 * 60 * 60 * 1000;
                                    if (datadiff < 0 || datadiff > time) {
                                       Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                       end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                   }
                                   else {
                                       end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                   }*/
                                   
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }

                           }
                       },
                       {
                           xtype: 'displayfield',
                           margin: '0,5,0,5',
                           value: '~ '
                       },
                       {

                           xtype: "datetimefield",
                           margin: '0 5,0,5',
                           editable: false,
                           id: 'end_time',
                           name: 'end_time',
                           format: 'Y-m-d 23:59:59',
                           value: Tomorrow(0),
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   var s_date = new Date(start.getValue());

                                   //搜索时间限定一个月
                                   //var data1 = Date.parse(start.getValue());
                                   //var data2 = Date.parse(end.getValue());
                                   //var datadiff = data2 - data1;
                                   //var time = 31 * 24 * 60 * 60 * 1000;
                                   if (end.getValue() < start.getValue()) {
                                       Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間！");
                                       end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                   }
                                   //else if (datadiff < 0 || datadiff > time) {
                                   //    Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                   //    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                   //}
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       }
                        
                     

                 ]
             },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [
                      {
                          xtype: 'button',
                          margin: '0 8 0 8',
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
                                  Ext.getCmp('o_id').setValue("");//訂單 / 細項 / 課程編號
                                  Ext.getCmp('ddlSel').setValue("-1");//狀態
                                  Ext.getCmp('start_time').setValue(Tomorrow(1 - new Date().getDate()));//開始時間--time_start--delivery_date
                                  Ext.getCmp('end_time').setValue(Tomorrow(0));//結束時間--time_end--delivery_date
                              }
                          }
                      }
                  ]
              }
        ]
    });
    var OrderAccumAmount = Ext.create('Ext.grid.Panel', {
        id: 'OrderAccumAmount',
        store: OrderAccumAmountStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
                        { header: "編號", dataIndex: 'event_id', width: 50, align: 'center' },
                        { header: "活動名稱", dataIndex: 'event_name', width: 150, align: 'center' },
                        { header: "活動描述", dataIndex: 'event_desc', width: 150, align: 'center' },
                        { header: "累積金額", dataIndex: 'accum_amount', width: 150, align: 'center' },
                        { header: "開始時間", dataIndex: 'event_start_time', width: 150, align: 'center' },
                        { header: "結束時間", dataIndex: 'event_end_time', width: 150, align: 'center' },
                        { header: "描述開始時間", dataIndex: 'event_desc_start', width: 150, align: 'center' },
                        { header: "描述結束時間", dataIndex: 'event_desc_end', width: 170, align: 'center' },
                        { header: "修改人", dataIndex: 'update_user', width: 90, align: 'center' },
                        { header: "修改時間", dataIndex: 'event_update_time', width: 150, align: 'center' },
                         {
                             header: "活動狀態",
                             dataIndex: 'event_status',
                             id: 'status',
                             align: 'center',
                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                 if (value == 1) {
                                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.event_id + ")'><img hidValue='0' id='img" + record.data.event_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                                 } else {
                                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.event_id + ")'><img hidValue='1' id='img" + record.data.event_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                                 }
                             }
                         }
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: "刪除", id: 'delete', disabled: true, iconCls: 'icon-remove', handler: RemoveClick },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderAccumAmountStore,
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
        }
          , selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [ frm,OrderAccumAmount],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                OrderAccumAmount.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
     ToolAuthority();
    //OrderAccumAmountStore.load({ params: { start: 0, limit: 25 } });
});
//*********新增********//
onAddClick = function () {
    editFunction(null, OrderAccumAmountStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("OrderAccumAmount").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示", "未選中任何行!");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示", "只能選擇一行!");
    } else {
        editFunction(row[0], OrderAccumAmountStore);
    }
}

//**************刪除****************//

RemoveClick = function () {
    var row = Ext.getCmp("OrderAccumAmount").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert("提示", "未選中任何行");
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("確認刪除?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.event_id;
                    if (i != row.length - 1) {
                        rowIDs += ',';
                    }
                }

                Ext.Ajax.request({
                    url: '/OrderAccumAmount/DeleteOrderAccumAmount',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示","刪除成功!");
                            OrderAccumAmountStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("提示","刪除超時!");
                    }
                });
            }
        });
    }
}
function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}
//更改活動狀態(啟用前先檢查該活動是否具有有效贈品，是則啟用，否則提示請設定有效贈品)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");

    $.ajax({
        url: "/OrderAccumAmount/UpdateActiveQuestion",
        data: {
            "event_id": id,
            "event_status": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            OrderAccumAmountStore.load();
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

