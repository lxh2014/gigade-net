var pageSize = 25;
Ext.apply(Ext.form.field.VTypes, {
    daterange: function (val, field) {
        var date = field.parseDate(val);

        if (!date) {
            return false;
        }
        this.dateRangeMax = null;
        this.dateRangeMin = null;
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            //start.validate();
            this.dateRangeMax = date;
        } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            //end.validate();
            this.dateRangeMin = date;
        }
        /*  
         * Always return true since we're only using this vtype to set the  
         * min/max allowed values (these are tested for after the vtype test)  
         */
        return true;
    },

    daterangeText: '開始時間必須小於結束時間'
});
Ext.define('gigade.TicketMaster', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ticket_master_id", type: "string" },
        { name: "vendor_id", type: "string" },
        { name: "course_id", type: "string" },
        { name: "course_detail_name", type: "string" },
        { name: "start_date", type: "string" },
        { name: "end_date", type: "string" },
        { name: "order_name", type: "string" },
        { name: "order_payment", type: "string" },
        { name: "order_amount", type: "string" },
        { name: "order_createdate", type: "string" },
        { name: "order_status", type: "string" },
        { name: "invoice_status", type: "string" },
        { name: "billing_checked", type: "string" },
        { name: "delivery_name", type: "string" },
        { name: "delivery_mobile", type: "string" },
        { name: "delivery_phone", type: "string" },
        { name: "delivery_zip", type: "string" },
        { name: "delivery_address", type: "string" },
        { name: "order_name", type: "string" },
        { name: "order_mobile", type: "string" },
        { name: "order_phone", type: "string" },
        { name: "order_zip", type: "string" },
        { name: "order_address", type: "string" }
    ]
});
var TicketMasterStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.TicketMaster',
    proxy: {
        type: 'ajax',
        url: '/Ticket/GetTicketMasterList',
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
            Ext.getCmp("gdTicketMaster").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
//加載前
TicketMasterStore.on('beforeload', function () {
    Ext.apply(TicketMasterStore.proxy.extraParams, {
        order_status: Ext.getCmp('order_status').getValue(),//訂單狀態
        order_payment: Ext.getCmp('order_payment').getValue(),//付款方式
        order_id: Ext.getCmp('o_id').getValue(),//訂單編號
        order_name: Ext.getCmp('o_name').getValue(),//訂購人
        ticket_start: Ext.getCmp('ticket_start').getValue(),//訂單開始時間
        ticket_end: Ext.getCmp('ticket_end').getValue(),//訂單結束時間
        course_search: Ext.getCmp('course_search').getValue(),//課程編號或課程名稱
        course_start: Ext.getCmp('course_start').getValue(),//課程開始時間
        course_end: Ext.getCmp('course_end').getValue(),//課程結束時間
        bill_check: Ext.getCmp('bill_check').getValue()//是否對賬
    })

});
Ext.onReady(function () {
    //頁面加載時創建grid
    var gdTicketMaster = Ext.create('Ext.grid.Panel', {
        id: 'gdTicketMaster',
        store: TicketMasterStore,
        // height: 645,
        flex: 8.5,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: '訂單編號', dataIndex: 'ticket_master_id', width: 80, align: 'center', id: 'ticket_master_id', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value > 0) {
                        return "<a href='javascript:void(0);' onclick='TranToTicketDetail(\"" + record.data.ticket_master_id + "\")'>" + value + "</a>";//<span style='color:Red;'></span>
                    }
                }
            },
            { header: '供應商編號', dataIndex: 'vendor_id', width: 80, align: 'center', id: 'vendor_id', hidden: true },
            {
                header: '課程編號', dataIndex: 'course_id', width: 80, align: 'center', id: 'course_detail_id', hidden: true
            },
             {
                 header: '課程名稱', dataIndex: 'course_detail_name', width: 150, align: 'center', id: 'course_detail_name', hidden: true
             },
            { header: '課程開始時間', dataIndex: 'start_date', width: 150, align: 'center', id: 'start_date', hidden: true },
            {
                header: '課程結束時間', dataIndex: 'end_date', width: 150, align: 'center', id: 'end_date', hidden: true
            },
             {
                 header: '訂購人', dataIndex: 'order_name', width: 60, align: 'center', id: 'order_name', hidden: true
             },
            { header: '付款方式', dataIndex: 'order_payment', width: 120, align: 'center', id: 'orderpayment', hidden: true },
            {
                header: '付款金額', dataIndex: 'order_amount', width: 60, align: 'center', id: 'order_amount', hidden: true
            },
            {
                header: '訂單創建時間', dataIndex: 'order_createdate', width: 150, align: 'center', id: 'order_createdate', hidden: true
            },
            { header: '訂單狀態', dataIndex: 'order_status', width: 70, align: 'center', id: 'orderstatus', hidden: true },
            {
                header: '發票狀態', dataIndex: 'invoice_status', width: 80, align: 'center', id: 'invoice_status', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "0":
                            return "待開立";
                            break;
                        case "1":
                            return "已開立";
                            break;
                        case "2":
                            return "歸檔";
                            break;
                    }
                }
            },
             {
                 header: '是否對賬', dataIndex: 'billing_checked', width: 60, align: 'center', id: 'billing_checked', hidden: true,
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == "false") {
                         return "否";
                     }
                     else {
                         return "是";
                     }
                 }
             },
            {
                header: '備註', dataIndex: 'note_admin', width: 90, align: 'center', id: 'note_admin', hidden: true
            }
        ],
        tbar: [
           { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm,
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TicketMasterStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, gdTicketMaster],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdTicketMaster.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });
    QueryAuthorityByUrl('/Ticket/TicketMaster');
    TicketMasterStore.load({ params: { start: 0, limit: 25 } });
})
/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdTicketMaster").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editTicketMasterFunction(row[0], TicketMasterStore);

    }
}
/*************************權限設置******************************/
function QueryAuthorityByUrl(url) {
    Ext.Ajax.request({
        url: '/FunctionGroup/GetAuthorityToolByUrl',
        method: "POST",
        params: { Url: url },
        success: function (form, action) {
            var data = Ext.decode(form.responseText);
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var btn = Ext.getCmp(data[i].id);
                    if (btn) {
                        btn.show();
                    }
                }
            }
        }
    });
}

function TranToTicketDetail(ticket_master_id) {
    var url = '/Ticket/TicketDetail?ticket_master_id=' + ticket_master_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#ticket_detail');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'ticket_detail',
        title: '課程訂購單明細',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
