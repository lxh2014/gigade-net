Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//聲明grid
Ext.define('GIGADE.VendorList', {
    extend: 'Ext.data.Model',
    fields: [
                { name: "vendor_id", type: "string" }, //供應商id
                { name: "vendor_code", type: "string" }, //供應商編號
                { name: "vendor_name_simple", type: "string" }, //供應商名稱
                { name: "account_year", type: "int" }, //年
                { name: "account_month", type: "int" }, //月
                { name: "m_product_money", type: "int" }, //商品總售價
                { name: "m_product_cost", type: "int" }, //商品總成本
                { name: "m_money_creditcard_1", type: "int" }, //一期刷卡費
                { name: "m_money_creditcard_3", type: "int" },
                { name: "m_freight_delivery_low", type: "int" }, //自行出貨低溫運費
                { name: "m_freight_delivery_normal", type: "int" }, //自行出貨常溫運費
                { name: "m_dispatch_freight_delivery_normal", type: "int" }, //調度倉常溫運費
                { name: "m_dispatch_freight_delivery_low", type: "int" }, //調度倉低溫運費
                { name: "m_freight_return_low", type: "int" }, //低溫逆物流
                { name: "m_account_amount", type: "int" },
                { name: "m_all_deduction", type: "int" }, //商品凈售價
                { name: "m_gift", type: "int" }, //贈品總成本
                { name: "dispatch", type: "int" },
                { name: "m_bag_check_money", type: "int" }, //寄倉費用
                { name: "m_freight_return_normal", type: "int" }, //常溫逆物流
                { name: "detail", type: "string" }, //明細
                { name: "searchEmail", type: "string" },
                { name: "searchName", type: "string" },
                { name: "searchInvoice", type: "string" },
                { name: "amount", type: "int" }, //小計
                { name: "content", type: "string" }

    ]
});
//獲取grid中的數據
var VendorListALLStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VendorList',
    proxy: {
        type: 'ajax',
        url: '/Accountant/GetVendorAccountMonthList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
//運送方式Model
Ext.define("gigade.typeModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "type_id", type: "string" },
        { name: "type_name", type: "string" }]
});
//運送方式Store
var searchVendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.typeModel',
    autoLoad: true,
    data: [
    { type_id: '0', type_name: "所有供應商資料" },
    { type_id: '1', type_name: "電子信箱" },
    { type_id: '2', type_name: "供應商簡稱" },
    { type_id: '4', type_name: "供應商編號" },
    { type_id: '5', type_name: "供應商編碼" },
    { type_id: '3', type_name: "統一編號" }
    ]
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {

            Ext.getCmp("VendorListGrid").down('#Remove').setDisabled(selections.length == 0);
            Ext.getCmp("VendorListGrid").down('#Edit').setDisabled(selections.length == 0)

        }
    }
});
//加載前先獲取ddl的值
VendorListALLStore.on('beforeload', function () {
    Ext.apply(VendorListALLStore.proxy.extraParams, {
        dateType: Ext.getCmp('ven_type').getValue(),
        dateCon: Ext.getCmp('search_con').getValue(),
        dateOne: Ext.getCmp('dateOne').getRawValue(),
        dateTwo: Ext.getCmp('dateTwo').getRawValue()
    })

});
//查询
Query = function () {

    VendorListALLStore.removeAll();
    Ext.getCmp("VendorListGrid").store.loadPage(1, {
        params: {
            dateType: Ext.getCmp('ven_type').getValue(),
            dateCon: Ext.getCmp('search_con').getValue(),
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue()
        }
    });
}
function thMoth() {
    var moth = new Date().getMonth();
    if (moth == 0) {
        moth = 12;
        return moth;
    }
    else {
        return moth;
    }
}
function thYear() {
    var year = new Date().getFullYear();
    var moth = new Date().getMonth();
    if (moth == 0) {
        year = year - 1;
        return year;
    }
    else {
        return year;
    }
}
Ext.onReady(function () {

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        flex: 1.2,
        layout: 'anchor',
        height: 70,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
         {
             xtype: 'label',
             margin: '5 0 0 0',
             style: 'fontSize:25px ;color:orange',
             text: "供應商業績月報表"
         },
         {
             xtype: 'label',
             id: "lblday",
             margin: '5 0 0 0',
             //text: '【' + thYear() + "年" + thMoth() + '月】',
             style: 'fontSize:20px ;color:red'
         },

                 {
                     xtype: 'fieldcontainer',
                     combineErrors: true,
                     layout: 'hbox',
                     items: [
                                {
                                    xtype: 'combobox',
                                    allowBlank: true,
                                    hidden: false,
                                    id: 'ven_type',
                                    name: 'ven_type',
                                    store: searchVendorStore,
                                    queryMode: 'local',
                                    width: 200,
                                    labelWidth: 80,
                                    margin: '5 10 0 5',
                                    displayField: 'type_name',
                                    valueField: 'type_id',
                                    typeAhead: true,
                                    forceSelection: false,
                                    editable: false,
                                    value: 0,
                                    fieldLabel: "查詢條件"
                                },
                                 {
                                     xtype: 'textfield',
                                     fieldLabel: "查詢內容",
                                     width: 200,
                                     labelWidth: 80,
                                     margin: '5 10 0 0',
                                     id: 'search_con',
                                     name: 'search_con',
                                     listeners: {
                                         specialkey: function (field, e) {
                                             if (e.getKey() == e.ENTER) {
                                                 Query();
                                             }
                                         }
                                     }
                                 },
                                 {
                                     xtype: "numberfield",
                                     fieldLabel: "報表月份",
                                     width: 200,
                                     labelWidth: 80,
                                     margin: '5 0 0 0',
                                     id: 'dateOne',
                                     name: 'dateOne',
                                     allowBlank: false,
                                     maxValue: new Date().getFullYear().toString(),
                                     value: thYear(),
                                     minValue: '2011'
                                 },
                                 {
                                     xtype: 'displayfield',
                                     margin: '5 0 0 0',
                                     value: "~"
                                 },
                                 {
                                     xtype: "numberfield",
                                     id: 'dateTwo',
                                     name: 'dateTwo',
                                     margin: '5 0 0 0',
                                     width: 120,
                                     allowBlank: false,
                                     value: thMoth(),
                                     minValue: '1',
                                     maxValue: '12'
                                 },
                                   {
                                       xtype: 'button',
                                       iconCls: 'icon-search',
                                       text: "查詢",
                                       margin: '5 0 6 6',
                                       handler: Query,
                                       listeners: {
                                           render: function () {
                                               var lblday = Ext.getCmp('lblday');
                                               a = Ext.getCmp('dateOne').getRawValue();
                                               b = Ext.getCmp('dateTwo').getRawValue();
                                               lblday.setText("【" + a + "年" + b + "月】");
                                           },
                                           'click': function () {
                                               var lblday = Ext.getCmp('lblday');
                                               a = Ext.getCmp('dateOne').getRawValue();
                                               b = Ext.getCmp('dateTwo').getRawValue();
                                               lblday.setText("【" + a + "年" + b + "月】");
                                           }

                                       }
                                   }
                     ]
                 }
        ]
    });

    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: VendorListALLStore,
        height: 699,
        flex: 8.8,
        columnLines: true,
        frame: true,
        columns: [
            { header: "明細", dataIndex: 'detail', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) { return Ext.String.format('<a href=javascript:VendorAccountMonthToDetial(' + '{0}' + "," + '{1}' + ',' + '{2}' + ',' + '"{3}"' + ')>明細 </a>', record.data.account_year, record.data.account_month, record.data.vendor_id, record.data.vendor_code); } },
            { header: "供應商編號", dataIndex: 'vendor_id', width: 100, align: 'center' },
            { header: "供應商編碼", dataIndex: 'vendor_code', width: 100, align: 'center' },
            { header: "供應商簡稱", dataIndex: 'vendor_name_simple', width: 150, align: 'center' },
            { header: "商品總售價", dataIndex: 'm_product_money', width: 80, align: 'center' },
            { header: "商品凈售價", dataIndex: 'm_all_deduction', width: 100, align: 'center' },
            { header: "商品總成本", dataIndex: 'm_product_cost', width: 100, align: 'center' },
            { header: "贈品總成本", dataIndex: 'm_gift', width: 100, align: 'center' },
            { header: "一期刷卡費", dataIndex: 'm_money_creditcard_1', width: 80, align: 'center' },
            { header: "自行出貨常溫運費", dataIndex: 'm_freight_delivery_normal', width: 100, align: 'center' },
            { header: "自行出貨低溫運費", dataIndex: 'm_freight_delivery_low', width: 100, align: 'center' },
            { header: "調度倉常溫運費", dataIndex: 'm_dispatch_freight_delivery_normal', width: 100, align: 'center' },
            { header: "調度倉低溫運費", dataIndex: 'm_dispatch_freight_delivery_low', width: 100, align: 'center' },
            { header: "常溫逆物流", dataIndex: 'm_freight_return_normal', width: 80, align: 'center' },
            { header: "低溫逆物流", dataIndex: 'm_freight_return_low', align: 'center' },
            { header: "寄倉費用", dataIndex: 'm_bag_check_money', align: 'center' },
            {
                header: "小計", dataIndex: 'amount', align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    amount = record.data.m_account_amount - record.data.m_bag_check_money;
                    return amount;
                }
            }



        ],
        tbar: [
            { xtype: 'button', id: 'Add', text: ADD, iconCls: 'icon-add', hidden: true, handler: onAddClick },
            { xtype: 'button', id: 'Edit', text: EDIT, iconCls: 'icon-edit', hidden: true, disabled: true, handler: onEditClick },
            { xtype: 'button', id: 'Remove', text: REMOVE, iconCls: 'icon-remove', hidden: true, disabled: true, handler: onRemoveClick }, '->',
            {
                xtype: 'button', id: 'btnExcel', text: '對賬總表', iconCls: 'icon-excel', handler: function () {
                    window.open("/Accountant/VendorAccountCountExport?dateone=" + Ext.getCmp("dateOne").getValue() + "&datetwo=" + Ext.getCmp("dateTwo").getValue() + "&dateType=" + Ext.getCmp('ven_type').getValue() + "&dateCon=" + Ext.getCmp('search_con').getValue());
                }
            },
             {
                 xtype: 'button', id: 'btnAllExcel', text: '總表明細', iconCls: 'icon-excel', handler: function () {
                     window.open("/Accountant/ExportVendorAccountMonthAll?dateone=" + Ext.getCmp("dateOne").getValue() + "&datetwo=" + Ext.getCmp("dateTwo").getValue());
                 }
             },
              {
                  xtype: 'button', id: 'btnPiCiExcel', text: '各供應商明細', iconCls: 'icon-excel', handler: function () {
                      //window.open("/Accountant/AllExportVendorAccountMonthDetail?dateone=" + Ext.getCmp("dateOne").getValue() + "&datetwo=" + Ext.getCmp("dateTwo").getValue());
                      Ext.MessageBox.show({
                          msg: '正在匯出，請稍後....',
                          width: 300,
                          wait: true
                      });
                      Ext.Ajax.request({
                          url: '/Accountant/AllExportVendorAccountMonthDetail',
                          timeout: 600000,
                          params: {
                              dateOne: Ext.getCmp('dateOne').getValue(),
                              dateTwo: Ext.getCmp('dateTwo').getValue()
                          },
                          success: function (response) {
                              Ext.MessageBox.hide();
                              var result = eval("(" + response.responseText + ")");
                              if (result.success == "true") {
                                  var url = Ext.String.format('<a href="../../ImportUserIOExcel/供應商對賬報表.zip">點此下載</a>');
                                  Ext.MessageBox.alert("下載提示", "" + url + "");
                              }
                              else {
                                  Ext.Msg.alert("提示信息", "下載出錯");
                              }
                          }
                      });
                  }
              },

        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorListALLStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, VendorListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorListGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });

    ToolAuthority();
    VendorListALLStore.load({ params: { start: 0, limit: 25 } });

})
//function Tomorrow() {
//    var d;
//    var s = "";
//    d = new Date();                             // 创建 Date 对象。
//    s += d.getFullYear() + "/";                     // 获取年份。
//    s += (d.getMonth() + 1) + "/";              // 获取月份。
//    s += d.getDate() + 1;                          // 获取日。
//    return (new Date(s));                                 // 返回日期。
//}
//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/VendorList/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {

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



onAddClick = function () {
    editFunction(null, VendorListALLStore);
}



onEditClick = function () {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], VendorListALLStore);
    }


}


onRemoveClick = function () {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
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
                    url: '/Promotions/DeletePromotionsAmountGift',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            VendorListALLStore.load(1);
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

/************匯入到Excel**ATM************/
function ExportVAMCount() {
    //Ext.Ajax.request({
    //    url: "/VendorAccountMonth/GetVAMExcle",
    //    success: function (response) {
    //        //                    alert(response.responseText);
    //        if (response.responseText == "true") {
    //            Ext.MessageBox.alert("下載提示", "<a href='../../ImportUserIOExcel/vendor_account_month.csv'>點擊下載</a>");
    //        }
    //    }
    //});
}
function VendorAccountMonthToDetial(dateOne, dateTwo, vendor_id, vendor_code) {
    var RECORD = "";
    var urlTran = '/Accountant/VendorAccountDetail' + '?dateOne=' + dateOne + '&dateTwo=' + dateTwo + '&vendor_id=' + vendor_id + '&vendor_code=' + vendor_code;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '業績明細報表',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}