var CallidForm;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
//实现验证开始时间必须小于结束时间
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

    daterangeText: 'Start date must be less than end date'
});
var pageSize = 25;
//促銷贈品表Model
Ext.define('gigade.TicketDetail', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ticket_detail_id", type: "int" },//細項編號
        { name: "ticket_master_id", type: "int" },//訂單編號
        { name: "vendor_id", type: "int" },//供應商名稱
        { name: "vendor_name_simple", type: "string" },//供應商名稱
        { name: "course_id", type: "int" },//課程編號
        { name: "course_detail_id", type: "int" },
        { name: "product_name", type: "string" },//商品名稱
        { name: "start_date", type: "string" },//課程開始時間
        { name: "end_date", type: "string" },//課程結束時間
        { name: "single_cost", type: "int" },//商品成本
        { name: "single_price", type: "int" },//商品價格
        { name: "spec_id_1", type: "string" },//規格一
        { name: "spec_id_2", type: "string" },//規格二
        { name: "single_money", type: "string" },//實際出售價格
        { name: "flag", type: "int" },//狀態
        { name: "ticket_Code", type: "string" }//存虛擬編碼
    ]
});

var TicketDetailStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.TicketDetail',
    proxy: {
        type: 'ajax',
        url: '/Ticket/GetTicketDetailList',
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
            var row = Ext.getCmp("TicketDetail").getSelectionModel().getSelection();
            if (row != "") {
                if (row.length == 1) {
                    if (row[0].data.flag == 1) {
                        Ext.getCmp("TicketDetail").down('#UpdateStatus').setDisabled(true);
                    }
                    else {
                        Ext.getCmp("TicketDetail").down('#UpdateStatus').setDisabled(false);
                    }
                }
                else {
                    for (var i = 0; i < row.length; i++) {
                        if (row[i].data.flag == 1) {
                            Ext.getCmp("TicketDetail").down('#UpdateStatus').setDisabled(true);
                            break;
                        }
                        else {
                            Ext.getCmp("TicketDetail").down('#UpdateStatus').setDisabled(selections.length == 0);
                        }
                    }
                }
            }
        }
    }
});
TicketDetailStore.on('beforeload', function () {

    Ext.apply(TicketDetailStore.proxy.extraParams, {
        Search: Ext.getCmp('ddlSel').getValue(),
        MasterID: Ext.getCmp('o_id').getValue(),
        TimeStart: Ext.getCmp('course_start').getValue(),//課程開始時間
        TimeEnd: Ext.getCmp('course_end').getValue()//課程結束時間
    })

});

var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "-1" },
        { "txt": "未核銷", "value": "0" },
        { "txt": "已核銷", "value": "1" }
    ]
});
function Query(x) {
    TicketDetailStore.removeAll();
    Ext.getCmp("TicketDetail").store.loadPage(1);

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
                               maxLength: 9,
                               maskRe: /^\d$/,
                               name: 'o_id',
                               fieldLabel: '訂單/細項/課程編號',
                               labelWidth: 120
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
                          xtype: 'displayfield',
                          margin: '0 5px',
                          fieldLabel: '課程日期'
                      },
                      {
                          xtype: 'datefield',
                          id: 'course_start',
                          name: 'course_start',
                          margin: '0 5,0,5',
                          editable: false,
                          value: TheMonthFirstDay(),
                          format: 'Y/m/d',
                          vtype: 'daterange',
                          endDateField: 'course_end'
                      },
                      {
                          xtype: 'displayfield',
                          margin: '0,0,0,5',
                          value: '~'
                      },
                      {
                          xtype: 'datefield',
                          id: 'course_end',
                          name: 'course_end',
                          margin: '0 5px',
                          editable: false,
                          value: Tomorrow(1),
                          format: 'Y/m/d',
                          vtype: 'daterange',
                          startDateField: 'course_start'
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
                                  Ext.getCmp('course_start').setValue(TheMonthFirstDay());//開始時間--time_start--delivery_date
                                  Ext.getCmp('course_end').setValue(Tomorrow(1));//結束時間--time_end--delivery_date
                              }
                          }
                      }
                  ]
              }
        ],
        listeners: {
            beforerender: function () {
                var master_id = document.getElementById("hid_master_id").value;
                if (master_id != "") {
                    Ext.getCmp('o_id').setValue(master_id);
                }
            }
        }
    });
    var TicketDetail = Ext.create('Ext.grid.Panel', {
        id: 'TicketDetail',
        store: TicketDetailStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "訂單編號", dataIndex: 'ticket_master_id', width: 80, align: 'center', id: 'ticket_master_id', hidden: true },
            { header: "細項編號", dataIndex: 'ticket_detail_id', width: 80, align: 'center', id: 'ticket_detail_id', hidden: true },
            {
                header: "供應商", dataIndex: 'vendor_id', width: 150, align: 'center', id: 'vendor_id', hidden: true, renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return record.data.vendor_name_simple;
                }
            },
            {
                header: "課程編號", dataIndex: 'course_id', width: 80, align: 'center', id: 'course_detail_id', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value > 0) {
                        return "<a href='javascript:void(0);' onclick='TranToTicketMaster(\"" + record.data.course_id + "\")'>" + value + "</a>";//<span style='color:Red;'></span>
                    }
                }
            },
            { header: "商品名稱", dataIndex: 'product_name', width: 160, align: 'center', id: 'product_name', hidden: true },

            { header: "課程開始時間", dataIndex: 'start_date', width: 120, align: 'center', id: 'start_date', hidden: true },
            { header: "課程結束時間", dataIndex: 'end_date', width: 120, align: 'center', id: 'end_date', hidden: true },
            { header: "規格1", dataIndex: 'spec_id_1', width: 120, align: 'center', id: 'spec_id_1', hidden: true },
            { header: "規格2", dataIndex: 'spec_id_2', width: 120, align: 'center', id: 'spec_id_2', hidden: true },
            { header: "商品成本", dataIndex: 'single_cost', width: 60, align: 'center', id: 'single_cost', hidden: true },
            { header: "商品價格", dataIndex: 'single_price', width: 60, align: 'center', id: 'single_price', hidden: true },
            { header: "實際出售", dataIndex: 'single_money', width: 60, align: 'center', id: 'single_money', hidden: true },
            {
                header: "狀態", dataIndex: 'flag', width: 60, align: 'center', id: 'flag', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.flag == 0) {
                        return "<div style='color:green;'> " + "未核銷" + "</div>";
                    } else if (record.data.flag == 1) {
                        return "<div style='color:red;'> " + "已核銷" + "</div>";
                    }
                }
            },
            { header: "隨機碼", dataIndex: 'ticket_Code', width: 120, align: 'center', id: 'ticket_Code', hidden: true },
        ],
        tbar: [
          { xtype: 'button', text: "核銷", id: 'UpdateStatus', hidden: false, disabled: true, iconCls: 'ui-icon ui-icon-dis', handler: UpdateDetailsClick },//iconCls: 'icon-remove',
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TicketDetailStore,
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
        items: [frm, TicketDetail],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                TicketDetail.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    // ToolAuthority();
    QueryAuthorityByUrl('/Ticket/TicketDetail');
    TicketDetailStore.load({ params: { start: 0, limit: 25 } });
});
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


function TranToTicketMaster(eventId) {
    var url = '/Ticket/TicketMaster?course_id=' + eventId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#ticket_master');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'ticket_master',
        title: '課程訂購單',
        // html: window.top.rtnFrame(url),
        //html: '<iframe width=100% height=100% src=' + url + '/>',]
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}





UpdateDetailsClick = function () {
    var row = Ext.getCmp("TicketDetail").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("確定核銷?"), function (btn) {//"核銷選中" + row.length + "條數據？"
            if (btn == 'yes') {

                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    if (row[i].data.flag == 1)
                        continue;
                    rowIDs += row[i].data.ticket_detail_id + ',';
                }

                Ext.Ajax.request({
                    url: '/Ticket/UpdateTicketStatus',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        TicketDetailStore.load();

                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                        TicketDetailStore.load();
                    }
                });
            }
        });
    }
}
function TheMonthFirstDay() {
    var times;
    times = new Date();
    return new Date(times.getFullYear(), times.getMonth(), 1);
}
function Tomorrow(days) {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + days);
    return d;
}
function ExportExeclUserMessage(id) {
    window.open("/NewPromo/ExportNewPromoRecordList?event_id=" + id);
}
