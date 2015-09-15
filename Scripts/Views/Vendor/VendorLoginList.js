Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
'Ext.form.Panel',
'Ext.ux.form.MultiSelect',
'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
var boolPassword = true;//標記是否需要輸入密碼
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
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.VendorLoginList', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "vendor_code", type: "string" },
    { name: "login_id", type: "int" },
    { name: "vendor_id", type: "int" },
    { name: "username", type: "string" },
    { name: "login_ipfrom", type: "string" },
    { name: "slogin_createdate", type: "string" }
    ]
});

var VendorLoginListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.VendorLoginList',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorLoginList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

//使用者Model
Ext.define('gigade.ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "name", type: "string" },
    { name: "callid", type: "string" }]
});

var ManageUserStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.ManageUser',
    proxy: {
        type: 'ajax',
        url: '/Fgroup/QueryCallid',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
VendorLoginListStore.on('beforeload', function () {
    Ext.apply(VendorLoginListStore.proxy.extraParams,
    {
        vendor_id: Ext.getCmp('vendor_id').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue()
    });
});
//VendorLoginListStore.on('load', function (store, records, options) {
//    var totalcount = records.length;
//    if (totalcount == 0) {
//        Ext.MessageBox.alert(INFORMATION, SEARCHNONE);
//    }

//});
function Query(x) {
    VendorLoginListStore.removeAll();
    if (Ext.getCmp('vendor_id').getValue() == '' && (Ext.getCmp('timestart').getValue() == null || Ext.getCmp('timeend').getValue() == null)) {
        Ext.MessageBox.alert(INFORMATION, '請輸入查詢條件');
    }
    else {
        Ext.getCmp("VendorLoginListGrid").store.loadPage(1, {
            params: {
                vendor_id: Ext.getCmp('vendor_id').getValue(),
                timestart: Ext.getCmp('timestart').getValue(),
                timeend: Ext.getCmp('timeend').getValue()
            }
        });
    }
}
Ext.onReady(function () {
    var VendorLoginListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorLoginListGrid',
        store: VendorLoginListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: LOGINID, dataIndex: 'login_id', width: 200, align: 'center' },
        { header: VENDORID, dataIndex: 'vendor_id', width: 200, align: 'center' },
        { header: VENDORCODE, dataIndex: 'vendor_code', width: 200, align: 'center' },
        {
            header: "供應商簡稱", dataIndex: 'username', width: 200, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + record.data.login_id + ")'  >" + value + "</span>";
            }
        },
        { header: LOGINIPFROM, dataIndex: 'login_ipfrom', width: 200, align: 'center' },
        { header: SLOGINCREATEDATE, dataIndex: 'slogin_createdate', width: 200, align: 'center' }
        ],
        tbar: [
        {
            xtype: 'textfield', fieldLabel: '供應商編號/編碼', id: 'vendor_id', labelWidth: 125,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Query();
                    }
                }
            }
        },
        {
            xtype: 'datetimefield', fieldLabel: TIMESTART, editable: false, id: 'timestart', labelWidth: 80,
            // value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("timestart");
                    var end = Ext.getCmp("timeend");
                    if (end.getValue() == null) {
                        end.setValue(setNextMonth(start.getValue(), 1));
                    } else if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                        start.setValue(setNextMonth(end.getValue(), -1));
                    }
                    //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                    //    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                    //    end.setValue(setNextMonth(start.getValue(), 1));
                    //}

                }
            }

        },
        { xtype: 'displayfield', value: "~", margin: '0' },
        {
            xtype: 'datetimefield', editable: false, id: 'timeend', labelWidth: 60,
            //value: Tomorrow(),
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("timestart");
                    var end = Ext.getCmp("timeend");
                    if (start.getValue() != "" && start.getValue() != null) {
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                        //    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                        //    start.setValue(setNextMonth(end.getValue(), -1));
                        //}
                    }
                    else {
                        start.setValue(setNextMonth(end.getValue(), -1));
                    }
                }
            }
        }
        , {
            text: SEARCH,
            iconCls: 'icon-search',
            id: 'btnQuery',
            handler: Query
        },
        {
            text: '重置',
            id: 'reset',
            iconCls: 'ui-icon ui-icon-reset',
            handler: function () {
                Ext.getCmp('vendor_id').reset();
                Ext.getCmp('timestart').reset()
                Ext.getCmp('timeend').reset();
            }
        }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorLoginListStore,
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

    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [VendorLoginListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorLoginListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //VendorLoginListStore.load({ params: { start: 0, limit: 25} });
});


function SecretLogin(rid) {//secretcopy
    var secret_type = "7";//參數表中的"供應商查詢列表"
    var url = "/Vendor/VendorLoginList";
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
    var d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;                                 // 返回日期。
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
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //    addWin.show();
    editFunction(null, VendorLoginListStore);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("VendorLoginListGrid").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], VendorLoginListStore);
    }
}

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("VendorLoginListGrid").getSelectionModel().getSelection();
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
                    url: '/Promotions/DeletePromotionsAccumulateRate',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            VendorLoginListStore.load();
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

