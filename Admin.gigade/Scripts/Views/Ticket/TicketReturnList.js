var pageSize = 25;
Ext.define('gigade.TicketReturn', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "tr_id", type: "int" },
        { name: "ticket_master_id", type: "string" },
        { name: "tr_note", type: "string" },
        { name: "tr_bank_note", type: "stirng" },
        { name: "tr_update_user", type: "int" },
        { name: "tr_create_user", type: "int" },
        { name: "tr_create_date", type: "string" },
        { name: "tr_update_date", type: "string" },
        { name: "tr_ipfrom", type: "string" },
        { name: "tr_money", type: "int" },
        { name: "tr_status", type: "int" },
        { name: "tr_reason_type", type: "string" },

    ]
});
var TicketReturnStore = Ext.create('Ext.data.Store', {
    model: 'gigade.TicketReturn',

    proxy: {
        type: 'ajax',
        url: '/Ticket/TicketReturnList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

function setNextMonth(source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    return s;
}
function Query()
{
     TicketReturnStore.removeAll();
     Ext.getCmp("TicketReturn").store.loadPage(1, {
        params: {

            start_date: Ext.getCmp('start_date').getValue(),//課程開始時間
            end_date: Ext.getCmp('end_date').getValue(),//課程結束時間


        }
    });
}

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
    selectionchange: function (sm, selections) {
        var row = Ext.getCmp("TicketReturn").getSelectionModel().getSelection();
        Ext.getCmp("TicketReturn").down('#edit').setDisabled(selections.length == 0);
    }
}
});

Ext.onReady(function () {
    var TicketReturn = Ext.create('Ext.grid.Panel', {
        id: 'TicketReturn',
        store: TicketReturnStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
             { header: "流水號", dataIndex: 'tr_id', width: 80, align: 'center' },
            { header: "訂單編號", dataIndex: 'ticket_master_id', width: 100, align: 'center' },
            { header: "備註", dataIndex: 'tr_note', width: 150, align: 'center' },
            { header: "銀行資訊", dataIndex: 'tr_bank_note', width: 120, align: 'center' },
            { header: "來源IP", dataIndex: 'tr_ipfrom', width: 120, align: 'center' },
            { header: "退款金額", dataIndex: 'tr_money', width: 120, align: 'center' },
           { header: "退款日期", dataIndex: 'tr_create_date', width: 180, align: 'center' },
           { header: "退款類型", dataIndex: 'tr_reason_type', width: 120, align: 'center' },
             {
                 header: "退款狀態", dataIndex: 'tr_status', width: 120, align: 'center', renderer: function (value) {
                     if (value == 1)
                     {
                         return "待處理";
                     }
                     else if (value == 2)
                     {
                         return "處理中";
                     }
                     else
                     {
                         return "已處理";
                     }
                 }
             },
            
        ],
        tbar: [
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
            '->',
                                         {
                                             xtype: 'datetimefield',
                                             fieldLabel:'退款日期',
                                             id: 'start_date',
                                             name: 'start_date',
                                             editable:false,
                                             margin: '0 5px',
                                             format: 'Y-m-d H:i:s',
                                             //time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                                             //listeners: {
                                             //    select: function (a, b, c) {
                                             //        var start = Ext.getCmp("start_date");
                                             //        var end = Ext.getCmp("end_date");
                                             //        if (end.getValue() == null) {
                                             //            end.setValue(setNextMonth(start.getValue(), 1));
                                             //        } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                             //            Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                             //            end.setValue(setNextMonth(start.getValue(), 1));
                                             //        }
                                             //    }
                                             //}
                                         },
                               {
                                   xtype: 'displayfield',
                                   value: '~'
                               },
                               {
                                   xtype: 'datetimefield',
                                   id: 'end_date',
                                   name: 'end_date',
                                   editable: false,
                                   margin: '0 5px',
                                   format: 'Y-m-d H:i:s',
                                   editable: false,
                                   //time: { hour: 23, min: 59, sec: 59 },
                                   //listeners: {
                                   //    select: function (a, b, c) {
                                   //        var start = Ext.getCmp("start_date");
                                   //        var end = Ext.getCmp("end_date");
                                   //        if (start.getValue() != "" && start.getValue() != null) {
                                   //            if (end.getValue() < start.getValue()) {
                                   //                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                   //                end.setValue(setNextMonth(start.getValue(), 1));
                                   //            }
                                   //            else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                   //                start.setValue(setNextMonth(end.getValue(), -1));
                                   //            }
                                   //        }
                                   //        else {
                                   //            start.setValue(setNextMonth(end.getValue(), -1));
                                   //        }
                                   //    }
                                   //}

                               },
                               {
                                   xtype: 'button',
                                   text: "查詢",
                                   handler: function () {
                                       if (Ext.getCmp('start_date').getValue() == "") {
                                           Ext.Msg.alert("提示信息", "請選擇查詢日期");

                                       }
                                       else {
                                           Query();
                                       }

                                   }   
                               },
                                 {
                                     xtype: 'button',
                                     text: "重置",
                                     handler: function () {
                                         Ext.getCmp('start_date').setValue();
                                         Ext.getCmp('end_date').setValue();
                                     }
                                 },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TicketReturnStore,
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
        items: [TicketReturn],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                TicketReturn.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    TicketReturnStore.load({ params: { start: 0, limit: 25 } });
});


//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("TicketReturn").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    } else {
        editFunction(row[0], TicketReturnStore);
    }
}


//******更改狀態******//
function UpdateActive(row_id) {
    var activeValue = $("#img" + row_id).attr("hidValue");
    $.ajax({
        url: "/MailGroup/UpMailGroupStatus",
        data: {
            "id": row_id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                MailGroupStore.load();
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                MailGroupStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert("提示信息", "操作失敗");
            MailGroupStore.load();
        }
    });
}