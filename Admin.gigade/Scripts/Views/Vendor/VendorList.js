var winDetail;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
var boolPassword = true;//標記是否需要輸入密碼

//聲明grid
Ext.define('GIGADE.VendorList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_id", type: "string" },
        { name: "vendor_code", type: "string" },
        { name: "vendor_name_simple", type: "string" },
        { name: "checkout_type", type: "string" },
        { name: "agr_date", type: "string" },
        { name: "agr_start", type: "string" },
        { name: "agr_end", type: "string" },
        { name: "freight_normal_limit", type: "decimal" },
        { name: "freight_normal_money", type: "decimal" },
        { name: "freight_low_limit", type: "decimal" },
        { name: "freight_low_money", type: "decimal" },
        { name: "freight_return_normal_money", type: "decimal" },
        { name: "freight_return_low_money", type: "decimal" },
        { name: "vendor_status", type: "int" },
        { name: "vendor_email", type: "string" },
        { name: "vendor_password", type: "string" },
        { name: "vendor_name_full", type: "string" },
        { name: "vendor_invoice", type: "string" },
        { name: "company_phone", type: "string" },
        { name: "vendor_confirm_code", type: "string" },
        { name: "vendor_note", type: "string" },
        { name: "vendor_login_attempts", type: "int" },
        { name: "assist", type: "int" },
        { name: "product_manage", type: "int" },
        { name: "gigade_bunus_percent", type: "int" },
        { name: "gigade_bunus_threshold", type: "int" },
        { name: "company_fax", type: "string" },
        { name: "company_person", type: "string" },
        { name: "company_zip", type: "int" },
        { name: "company_address", type: "string" },
        { name: "invoice_zip", type: "int" },
        { name: "invoice_address", type: "string" },
        { name: "contact_type_1", type: "int" },
        { name: "contact_name_1", type: "string" },
        { name: "contact_phone_1_1", type: "string" },
        { name: "contact_phone_2_1", type: "string" },
        { name: "contact_mobile_1", type: "string" },
        { name: "contact_email_1", type: "string" },
        { name: "contact_type_2", type: "int" },
        { name: "contact_name_2", type: "string" },
        { name: "contact_phone_1_2", type: "string" },
        { name: "contact_phone_2_2", type: "string" },
        { name: "contact_mobile_2", type: "string" },
        { name: "contact_email_2", type: "string" },
        { name: "contact_type_3", type: "int" },
        { name: "contact_name_3", type: "string" },
        { name: "contact_phone_1_3", type: "string" },
        { name: "contact_phone_2_3", type: "string" },
        { name: "contact_mobile_3", type: "string" },
        { name: "contact_email_3", type: "string" },
        { name: "contact_type_4", type: "int" },
        { name: "contact_name_4", type: "string" },
        { name: "contact_phone_1_4", type: "string" },
        { name: "contact_phone_2_4", type: "string" },
        { name: "contact_mobile_4", type: "string" },
        { name: "contact_email_4", type: "string" },
        { name: "contact_type_5", type: "int" },
        { name: "contact_name_5", type: "string" },
        { name: "contact_phone_1_5", type: "string" },
        { name: "contact_phone_2_5", type: "string" },
        { name: "contact_mobile_5", type: "string" },
        { name: "contact_email_5", type: "string" },
        { name: "cost_percent", type: "int" },
        { name: "creditcard_1_percent", type: "int" },
        { name: "creditcard_3_percent", type: "string" },
        { name: "sales_limit", type: "int" },
        { name: "bonus_percent", type: "int" },
        { name: "checkout_other", type: "string" },
        { name: "bank_code", type: "string" },
        { name: "bank_name", type: "string" },
        { name: "bank_number", type: "string" },
        { name: "bank_account", type: "string" },
        { name: "i_bigcode", type: "string" },
        { name: "c_bigcode", type: "string" },
        { name: "i_midcode", type: "string" },
        { name: "c_midcode", type: "string" },
        { name: "i_zipcode", type: "string" },
        { name: "c_zipcode", type: "string" },
        { name: "i_middle", type: "string" },
        { name: "c_middle", type: "string" },
        { name: "i_zip", type: "string" },
        { name: "c_zip", type: "string" },
        { name: "erp_id", type: "string" },
        { name: "manage_name", type: "string" },
        { name: "c_zip", type: "string" },
        { name: "erp_id", type: "string" },
        { name: "procurement_days", type: "int" },
        { name: "self_send_days", type: "int" },
        { name: "stuff_ware_days", type: "int" },
        { name: "dispatch_days", type: "int" },
        { name: "manage_email", type: "string" },
        { name: "vendor_mode", type: "string" },
        { name: "vendor_type", type: "string" },
        { name: "vendor_type_name", type: "string" },
        { name: "vendor_company_address", type: "string" }  //供應商公司地址
    ]
});
//獲取grid中的數據
var VendorListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VendorList',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorList',
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
        { name: "type_name", type: "string" }
    ]
});
//運送方式Store
var searchVendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.typeModel',
    autoLoad: true,
    data: [
        { type_id: '0', type_name: "請選擇" },
        { type_id: '6', type_name: "供應商編號" },
        { type_id: '7', type_name: "供應商編碼" },
        { type_id: '3', type_name: "供應商名稱" },
        { type_id: '2', type_name: "供應商簡稱" },
        { type_id: '1', type_name: "電子信箱" },
        { type_id: '4', type_name: "統一編號" },
        { type_id: '5', type_name: "ERP廠商編號" },

    ]
});
//運送方式Model
Ext.define("gigade.statusModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "type_id", type: "string" },
        { name: "type_name", type: "string" }
    ]
});
//運送方式Store
var searchStatusrStore = Ext.create('Ext.data.Store', {
    model: 'gigade.statusModel',
    autoLoad: true,
    data: [
        { type_id: '-1', type_name: "所有狀態" },
        { type_id: '1', type_name: "調度倉" },
    ]
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("VendorListGrid").down('#Edit').setDisabled(selections.length == 0);
            Ext.getCmp("VendorListGrid").down('#EditPass').setDisabled(selections.length == 0);
            if (selections.length == 1) {
                var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
                Ext.getCmp("VendorListGrid").down('#Grade').setDisabled(row[0].data.vendor_status != 3);
            }
            else {
                Ext.getCmp("VendorListGrid").down('#Grade').setDisabled(selections.length == 0);
            }
        }
    }
});
//加載前先獲取ddl的值
VendorListStore.on('beforeload', function () {
    var checked = Ext.getCmp("vendortype").items;
    var check = "";
    for (var i = 0; i < checked.length; i++) {
        if (checked.get(i).checked) {
            check = check + checked.get(i).inputValue + ",";
        }
    }
    Ext.apply(VendorListStore.proxy.extraParams, {
        dateType: Ext.getCmp('ven_type').getValue(),
        dateCon: Ext.getCmp('search_con').getValue().trim(),
        // dateStatus: Ext.getCmp('ven_status').getValue(),
        dateOne: Ext.getCmp('dateOne').getRawValue(),
        dateTwo: Ext.getCmp('dateTwo').getRawValue(),
        vendortype: check,
        relation_id: "",
        isSecret: true
    });
});

var edit_VendorListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VendorList',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

edit_VendorListStore.on('beforeload', function () {
    Ext.apply(VendorListStore.proxy.extraParams, {
        dateType: Ext.getCmp('ven_type').getValue(),
        dateCon: Ext.getCmp('search_con').getValue(),
        // dateStatus: Ext.getCmp('ven_status').getValue(),
        dateOne: Ext.getCmp('dateOne').getRawValue(),
        dateTwo: Ext.getCmp('dateTwo').getRawValue(),
        relation_id: "",
        isSecret: false
    });
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        border: 0,
        width: document.documentElement.clientWidth - 10,
        items: [
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
                        value: '0',
                        labelWidth: 80,
                        margin: '5 10 0 5',
                        displayField: 'type_name',
                        valueField: 'type_id',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        fieldLabel: "查詢條件",
                        listeners: {
                            afterRender: function (combo) {
                                combo.setValue(0);
                            }
                        }
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
                                    Query(1);
                                }
                            }
                        }
                    },
                    {
                        xtype: 'checkboxgroup',
                        fieldLabel: '供應商類型',
                        labelWidth: 80,
                        width: 350,
                        margin: '5 10 0 5',
                        id: 'vendortype',
                        name: 'vendortype',
                        columns: 3,
                        items: [
                            { boxLabel: '食品供應商', name: 'vtype', inputValue: '1' },
                            { boxLabel: '用品供應商', name: 'vtype', inputValue: '2' },
                            { boxLabel: '休閒供應商', name: 'vtype', inputValue: '3' }
                        ]
                    }

                ]
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                      {
                          xtype: "datetimefield",
                          fieldLabel: "合約簽訂日",
                          width: 240,
                          labelWidth: 80,
                          margin: '5 0 0 5',
                          id: 'dateOne',
                          name: 'dateOne',
                          format: 'Y-m-d H:i:s',
                          //time: { hour: 00, min: 00, sec: 00 },
                          submitValue: true,
                          editable: false,
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("dateOne");
                                  var end = Ext.getCmp("dateTwo");
                                  if (end.getValue() == null) {
                                      end.setValue(setNextMonth(start.getValue(), 1));
                                  } else if (end.getValue() < start.getValue()) {
                                      Ext.Msg.alert(INFORMATION, DATA_TIP);
                                      end.setValue(setNextMonth(start.getValue(), 1));
                                  }
                                  else if (end.getValue() > setNextMonth(start.getValue(), 3)) {
                                      // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                      end.setValue(setNextMonth(start.getValue(), 3));
                                  }
                              }
                          }
                      },
                     {
                         xtype: 'displayfield',
                         margin: '5 5 0 5', 
                         value: "~"
                     }
                     , {
                         xtype: "datetimefield",
                         format: 'Y-m-d H:i:s',
                         //time: { hour: 23, min: 59, sec: 59 },
                         id: 'dateTwo',
                         name: 'dateTwo',
                         margin: '5 0 0 0',
                         editable: false,
                         width: 160,
                         submitValue: true,
                         listeners: {
                             select: function (a, b, c) {
                                 var start = Ext.getCmp("dateOne");
                                 var end = Ext.getCmp("dateTwo");
                                 if (start.getValue() != "" && start.getValue() != null) {
                                     if (end.getValue() < start.getValue()) {
                                         Ext.Msg.alert(INFORMATION, DATA_TIP);
                                         start.setValue(setNextMonth(end.getValue(), -1));
                                     }
                                     else if (end.getValue() > setNextMonth(start.getValue(), 3)) {
                                         // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                         start.setValue(setNextMonth(end.getValue(), -3));
                                     }
                                 }
                                 else {
                                     start.setValue(setNextMonth(end.getValue(), -1));
                                 }
                             }
                         }
                     }
                 ]
             }

        ],
        buttonAlign: 'left',
        buttons: [
            {
                iconCls: 'icon-search',
                text: "查詢",
                handler: Query
            },
            {
                text: '重置',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    this.up('form').getForm().reset();
                }
            }
        ]
    });

    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: VendorListStore,
        //  height: document.documentElement.clientHeight - 85,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: "供應商編號", dataIndex: 'vendor_id', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href="#"  style="color:black" onclick="javascript:showDetail(' + record.data.vendor_id + ')">' + record.data.vendor_id + '</a>'
                }
            },
            { header: "供應商編碼", dataIndex: 'vendor_code', width: 90, align: 'center' },
            {
                header: "供應商名稱", dataIndex: 'vendor_name_full', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.vendor_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "供應商簡稱", dataIndex: 'vendor_name_simple', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.vendor_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "公司地址", dataIndex: 'vendor_company_address', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.vendor_id + ")'  >" + value + "</span>";
                }

            },
            { header: "供應商類型", dataIndex: 'vendor_type_name', width: 120, align: 'center' },
            { header: "自出出貨天數", dataIndex: 'self_send_days', width: 80, align: 'center' },
            { header: "寄倉出貨天數", dataIndex: 'stuff_ware_days', width: 80, align: 'center' },
            { header: "調度出貨天數", dataIndex: 'dispatch_days', width: 80, align: 'center' },
            { header: "調度倉模式", dataIndex: 'vendor_mode', width: 100, align: 'center' },
            {
                header: "結賬方式", dataIndex: 'checkout_type', width: 60, align: 'center',
                renderer: function (val) {
                    switch (val) {
                        case "1":
                            return "月結";
                            break;
                        case "2":
                            return "半月結";
                            break;
                        case "3":
                            return "其他";
                            break;
                    }
                }
            },
            {
                header: "合約簽訂日", dataIndex: 'agr_date', width: 80, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '' && value.length >= 10) {
                        return value.substring(0, 10);
                    }
                }
            },
            {
                header: "合約開始", dataIndex: 'agr_start', width: 100, align: 'center', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '' && value.length >= 10) {
                        return value.substring(0, 10);
                    }
                }
            },
            {
                header: "合約結束", dataIndex: 'agr_end', width: 100, align: 'center', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '' && value.length >= 10) {
                        return value.substring(0, 10);
                    }
                }
            },
            { header: "成本(%)", dataIndex: 'cost_percent', width: 60, align: 'center' },
            { header: "刷卡(%)", dataIndex: 'creditcard_1_percent', width: 60, align: 'center' },
            { header: "常溫門檻", dataIndex: 'freight_normal_limit', width: 60, align: 'center' },
            { header: "常溫運費", dataIndex: 'freight_normal_money', width: 60, align: 'center' },
            { header: "低溫門檻", dataIndex: 'freight_low_limit', width: 60, align: 'center' },
            { header: "低溫運費", dataIndex: 'freight_low_money', width: 60, align: 'center' },
            { header: "常溫逆物流", dataIndex: 'freight_return_normal_money', width: 70, align: 'center' },
            { header: "低溫逆物流", dataIndex: 'freight_return_low_money', width: 70, align: 'center' },
            {
                header: "狀態",
                dataIndex: 'vendor_status',
                id: 'controlactive',
                //width:50, 
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return '啟用';
                        // return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.vendor_id + ")'><img hidValue='0' id='img" + record.data.vendor_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else if (value == 2) {
                        return '<font color="#FF0000">停用</font>';
                        // return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.vendor_id + ")'><img hidValue='1' id='img" + record.data.vendor_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                    else if (value == 3) {//廠商失格，所有品牌下商品全部下架 add by shuangshuang0420j 20150624
                        return '<font color="#FF0000">失格</font>';
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', id: 'Add', text: ADD, iconCls: 'icon-add', hidden: false, handler: onAddClick },
            { xtype: 'button', id: 'Edit', text: EDIT, iconCls: 'icon-edit', hidden: true, disabled: true, handler: onEditClick },
            { xtype: 'button', id: 'EditPass', text: "修改密碼", iconCls: 'ui-icon ui-icon-key', hidden: true, disabled: true, handler: onEditPassClick },
            { xtype: 'button', id: 'Export', text: '匯出', icon: '../../../Content/img/icons/excel.gif', hidden: true, handler: ExportCSV }
            ,
              { xtype: 'button', id: 'Grade', text: '解除失格', icon: '../../../Content/img/icons/hmenu-unlock.png', hidden: true, disabled: true, handler: unLock }

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
            store: VendorListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, VendorListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorListGrid.width = document.documentElement.clientWidth;
                VendorListGrid.height = document.documentElement.clientHeight - 120;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    //VendorListStore.load({ params: { start: 0, limit: 25 } });
})

function SecretLogin(rid) {//secretcopy
    var secret_type = "7";//參數表中的"供應商查詢列表"
    var url = "/Vendor/VendorList";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url);//直接彈出顯示框
        }
    }
}

function Tomorrow() {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    dt.setDate(dt.getDate() + 1);
    return dt;                                 // 返回日期。
}



onAddClick = function () {
    editFunction(null);
}

onEditClick = function () {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        if (row[0].data.vendor_status != 3) {
            var secret_type = "7";//參數表中的"供應商查詢列表"
            var url = "/Vendor/VendorList/Edit";
            var ralated_id = row[0].data.vendor_id;
            boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
            if (boolPassword != "-1") {
                if (boolPassword) {//驗證
                    SecretLoginFun(secret_type, ralated_id, true, false, true, url, null, null, null);//先彈出驗證框，關閉時在彈出顯示框
                } else {
                    editFunction(ralated_id);
                }
            }
        } else {
            Ext.Msg.alert(INFORMATION, "失格商品不可編輯！");
        }

        // editFunction(row[0], VendorListStore, true);
    }
}

function ExportCSV() {
    var checked = Ext.getCmp("vendortype").items;
    var check = "";
    for (var i = 0; i < checked.length; i++) {
        if (checked.get(i).checked) {
            check = check + checked.get(i).inputValue + ",";
        }
    }
    Ext.Ajax.request({
        url: "/Vendor/VendorExportCSV",
        params: {
            dateType: Ext.getCmp('ven_type').getValue(),
            dateCon: Ext.getCmp('search_con').getValue(),
            // dateStatus: Ext.getCmp('ven_status').getValue(),
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue(),
            vendortype: check
        },
        success: function (response) {
            if (response.responseText.split(',')[0] == "true") {
                window.location.href = '../..' + document.getElementById('vendorCsvPath').value + response.responseText.split(',')[1];
            }
        }
    });
}

//供應商名字跳轉
function VendorFunction(rid) {
    edit_VendorListStore.load({
        params: { relation_id: rid },
        callback: function () {
            row = edit_VendorListStore.getAt(0);
            var secret_type = "7";//參數表中的"供應商查詢列表"
            var url = "/Vendor/VendorList/connect";
            var ralated_id = row.data.vendor_id;
            boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
            if (boolPassword != "-1") {
                if (boolPassword) {//驗證
                    SecretLoginFun(secret_type, ralated_id, true, false, true, url, null, null, null);//先彈出驗證框，關閉時在彈出顯示框
                } else {
                    VendorInfoFunction(row);
                }
            }
            //VendorInfoFunction(row);
        }
    });
}

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
//查询
Query = function () {
    VendorListStore.removeAll();
    var checked = Ext.getCmp("vendortype").items;
    var check = "";
    for (var i = 0; i < checked.length; i++) {
        if (checked.get(i).checked) {
            check = check + checked.get(i).inputValue + ",";
        }
    }
    if (Ext.getCmp('ven_type').getValue() == 0 && Ext.getCmp('search_con').getValue() != '') {
        Ext.Msg.alert(INFORMATION, "請選擇查詢條件");
    }
    else if (Ext.getCmp('ven_type').getValue() != 0 && Ext.getCmp('search_con').getValue() == '') {
        Ext.Msg.alert(INFORMATION, "請輸入查詢內容");
    }
    else if (check != "" || (Ext.getCmp('ven_type').getValue() != 0 && Ext.getCmp('search_con').getValue().trim() != "") || Ext.getCmp('dateOne').getValue() != null || Ext.getCmp('dateTwo').getValue() != null) {
        var regex = /^[0-9]{1,9}$/;
        var searchcon = Ext.getCmp('search_con').getValue().trim();
        var result = regex.test(searchcon);
        if (Ext.getCmp('ven_type').getValue() == 6 && result == false) {
            Ext.Msg.alert(INFORMATION, "供應商編號格式輸入錯誤");

        }
        else {
            Ext.getCmp("VendorListGrid").store.loadPage(1, {
                params: {
                    dateType: Ext.getCmp('ven_type').getValue(),
                    dateCon: Ext.getCmp('search_con').getValue().trim(),
                    //dateStatus: Ext.getCmp('ven_status').getValue(),
                    dateOne: Ext.getCmp('dateOne').getValue(),
                    dateTwo: Ext.getCmp('dateTwo').getValue(),
                    vendortype: check,
                    relation_id: "",
                    isSecret: true
                }
            });
        }
    }
    else {
        Ext.Msg.alert(INFORMATION, "請輸入查詢內容");
    }
}

onEditPassClick = function () {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editPassFunction(row[0].data.vendor_id, row[0].data.vendor_password);
    }
}
//修改密碼方法
function editPassFunction(vendor_id, pass) {

    var editPassFrm = Ext.create('Ext.form.Panel', {
        id: 'editPassFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Vendor/EditPass',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: "原密碼",
                allowBlank: false,
                id: 'oldPass',
                inputType: 'password',
                name: 'oldPass',
                listeners: {
                    blur: function (text, o) {
                        var oldPass = Ext.htmlEncode(Ext.getCmp("oldPass").getValue());
                        if (oldPass != null && oldPass != "" && oldPass != "gigadegigade") {//gigadegigade預留後門，便於修改密碼，此密碼不驗證 需求來源鴻飛
                            if (pass != hex_sha256(oldPass)) {
                                Ext.Msg.alert(INFORMATION, "原密碼輸入錯誤！");
                                text.setValue("");
                            }
                        }
                    }
                }
            },
            {
                xtype: 'textfield',
                fieldLabel: "新密碼",
                allowBlank: false,
                inputType: 'password',
                id: 'newPass',
                name: 'newPass',
                listeners: {
                    blur: function (text, o) {
                        var newP = Ext.htmlEncode(Ext.getCmp("newPass").getValue());
                        var reP = Ext.htmlEncode(Ext.getCmp("rePass").getValue())
                        if (reP != null && reP != "") {
                            if (reP != newP) {
                                Ext.Msg.alert(INFORMATION, "確認密碼與新密碼不相同！");
                                text.setValue("");
                            }
                        }
                    }
                }
            },
            {
                xtype: 'textfield',
                fieldLabel: "確認密碼",
                inputType: 'password',
                allowBlank: false,
                id: 'rePass',
                name: 'rePass',
                listeners: {
                    blur: function (text, o) {
                        var newP = Ext.htmlEncode(Ext.getCmp("newPass").getValue());
                        var reP = Ext.htmlEncode(Ext.getCmp("rePass").getValue())
                        if (newP != null && newP != "") {
                            if (reP != newP) {
                                Ext.Msg.alert(INFORMATION, "確認密碼與新密碼不相同！");
                                text.setValue("");
                            }
                        }
                    }
                }
            }
        ],
        buttons: [
            {
                text: SAVE,
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        if (vendor_id != "") {
                            form.submit({
                                params: {
                                    newPass: Ext.htmlEncode(Ext.getCmp("newPass").getValue()),
                                    vendorId: vendor_id
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        VendorListStore.loadPage(1);
                                        editPassWin.close();
                                    } else {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                    }
                }
            }
        ]
    });

    var editPassWin = Ext.create('Ext.window.Window', {
        title: "密碼修改",
        id: 'editPassWin',
        iconCls: 'ui-icon ui-icon-key',
        width: 300,
        height: 200,
        layout: 'fit',
        items: [editPassFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: '是否關閉',
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editPassWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                editPassFrm.getForm().reset(); //如果是編輯的話
            }
        }
    });
    editPassWin.show();
}


function unLock() {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        if (row[0].data.vendor_status == 3) {

            var editGradeFrm = Ext.create('Ext.form.Panel', {
                id: 'editGradeFrm',
                frame: true,
                plain: true,
                layout: 'anchor',
                url: '/Vendor/UnGrade',
                defaults: { anchor: "95%", msgTarget: "side" },
                items: [
                           {
                               xtype: 'radiogroup',
                               id: 'isStatus',
                               name: 'isStatus',
                               fieldLabel: '解除失格后狀態',
                               colName: 'isStatus',
                               defaults: {
                                   name: 'isStatus',
                                   margin: '0 4 0 0'
                               },
                               columns: 2,
                               vertical: true,
                               items: [
                                   {
                                       boxLabel: '啟用', id: 'isOn', inputValue: '1', checked: true
                                   },
                                   {
                                       boxLabel: '停用', id: 'isOff', inputValue: '2'
                                   }
                               ]
                           }
                ],
                buttons: [
                    {
                        text: SAVE,
                        formBind: true,
                        disabled: true,
                        handler: function () {
                            var form = this.up('form').getForm();
                            if (form.isValid()) {
                                form.submit({
                                    params: {
                                        vendor_id: row[0].data.vendor_id,
                                        active: Ext.getCmp('isStatus').getValue().isStatus
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert(INFORMATION, '解除成功！');
                                            VendorListStore.loadPage(1);
                                            editGradeWin.close();
                                        } else {
                                            Ext.Msg.alert(INFORMATION, '解除失敗！');
                                        }
                                    },
                                    failure: function () {
                                        Ext.Msg.alert(INFORMATION, '解除失敗！');
                                    }
                                });
                            }
                        }
                    }
                ]
            });

            var editGradeWin = Ext.create('Ext.window.Window', {
                title: "解除失格",
                id: 'editGradeWin',
                iconCls: 'icon-unlock',
                width: 300,
                height: 120,
                layout: 'fit',
                items: [editGradeFrm],
                constrain: true, //束縛窗口在框架內
                closeAction: 'destroy',
                modal: true,
                resizable: false,
                bodyStyle: 'padding:5px 5px 5px 5px',
                closable: false,
                tools: [
                    {
                        type: 'close',
                        qtip: '是否關閉',
                        handler: function (event, toolEl, panel) {
                            Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                                if (btn == "yes") {
                                    Ext.getCmp('editGradeWin').destroy();
                                } else {
                                    return false;
                                }
                            });
                        }
                    }
                ],
                listeners: {
                    'show': function () {
                        editGradeFrm.getForm().reset(); //如果是編輯的話
                    }
                }
            });
            editGradeWin.show();


        }
    }
}
/**********************************顯示供應商詳細信息*****************************************/
function showDetail(Vendor_id) {
    var secret_type = '20';
    var url = "/Vendor/VendorDetails?Vendor_id=" + Vendor_id;
    var ralated_id = Vendor_id;
    var info_id = Vendor_id;
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {
        if (boolPassword) {
            SecretLoginFun(secret_type, ralated_id, true, false, true, url);//先彈出驗證框，關閉時在彈出顯示框
            //SecretLoginFun(secret_type, ralated_id, false, true, false, url);//直接彈出顯示框
        }
        else {
            // productId = 15382;//product_id
            if (winDetail == undefined) {
                winDetail = Ext.create('Ext.window.Window', {
                    title: '供應商詳細信息',
                    constrain: true,
                    modal: true,
                    resizable: false,
                    height: document.documentElement.clientHeight * 565 / 683,
                    width: 800,
                    autoScroll: false,
                    layout: 'fit',
                    html: "<iframe scrolling='no' frameborder=0 width=100% height=100% src='/Vendor/VendorDetails?Vendor_id=" + Vendor_id + "'></iframe>",
                    listeners: {
                        close: function (e) {
                            winDetail = undefined;
                            tabs = new Array();
                        }
                    }
                }).show();
            }
        }
    }
}