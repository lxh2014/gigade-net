Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
var ccc = "1";
var boolPassword = true;//標記是否需要輸入密碼


ShopClassStore.load();
VendorStore.load();

/**********************************************************************群組管理主頁面**************************************************************************************/
//品牌管理Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Brand_Id", type: "int" }, //流水號
        { name: "Vendor_Id", type: "string" },
        { name: "Brand_Name", type: "string" }, //品牌名稱
        { name: "Brand_Sort", type: "int" }, //排序
        { name: "Brand_Status", type: "string" }, //狀態
        { name: "Image_Name", type: "string" },
        { name: "Image_Status", type: "string" },
        { name: "Image_Link_Mode", type: "string" },
        { name: "Image_Link_Url", type: "string" },
        { name: "Brand_Msg", type: "string" },
        { name: "begin_time", type: "string" }, //
        { name: "end_time", type: "string" },
        { name: "Brand_Msg_Start_Time", type: "string" },
        { name: "Brand_Msg_End_Time", type: "string" },
        { name: "Cucumber_Brand", type: "string" },
        { name: "Media_Report_Link_Url", type: "string" },
        { name: "Resume_Image", type: "string" },
        { name: "Resume_Image_Link", type: "string" },
        { name: "Promotion_Banner_Image", type: "string" },
        { name: "Promotion_Banner_Image_Link", type: "string" },
        { name: "class_name", type: "string" },
        { name: "classIds", type: "string" },
        { name: "vendor_name_simple", type: "string" }, //供應商名稱
        { name: "Event", type: "string" }
    ]
});
//品牌列表數據
var VendorBrandStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorBrandList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//頁面加載時判斷是否有數據
VendorBrandStore.on('load', function (store, records, options) {
    var totalcount = records.length;
    if (totalcount == 0) {
        Ext.MessageBox.alert(INFORMATION, "～目前无資料～");
    }
});
VendorBrandStore.on('beforeload', function () {
    Ext.apply(VendorBrandStore.proxy.extraParams, {
        serchs: Ext.getCmp('serchs').getValue(),
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        relation_id: "",
        isSecret: true
    });
});

var edit_VendorBrandStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorBrandList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
edit_VendorBrandStore.on('beforeload', function () {
    Ext.apply(edit_VendorBrandStore.proxy.extraParams, {
        serchs: Ext.getCmp('serchs').getValue(),
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        relation_id: "",
        isSecret: false
    });
});
//使用者Model
Ext.define('gigade.ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "name", type: "string" },
        { name: "callid", type: "string" }
    ]
});
//多選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            //    Ext.getCmp("gdFgroup").down('#add').setDisabled(selections.length == 0);
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
            //Ext.getCmp("gdFgroup").down('#auth').setDisabled(selections.length == 0);
            //Ext.getCmp("gdFgroup").down('#callid').setDisabled(selections.length == 0);
        }
    }
});
//查詢條件
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "請選擇", "value": "0" },
        { "txt": "供應商簡稱", "value": "1" },
        { "txt": "統一編號", "value": "2" },
        { "txt": "品牌名稱", "value": "3" }
    ]
});
function Query(x) {
    VendorBrandStore.removeAll();
    if (Ext.getCmp('serchs').getValue() == "0") {
        Ext.Msg.alert(INFORMATION, "請選擇查詢條件");
        return;
    }
    else if (Ext.getCmp('serchcontent').getValue().trim() == "") {
        Ext.Msg.alert(INFORMATION, "請填寫查詢內容");
        return;
    }
    else {
        Ext.getCmp("gdFgroup").store.loadPage(1, {
            params: {
                serchs: Ext.getCmp('serchs').getValue(),
                serchcontent: Ext.getCmp('serchcontent').getValue(),
                relation_id: "",
                isSecret: true
            }
        });
    }
}
Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: VendorBrandStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "品牌編號", dataIndex: 'Brand_Id', width: 80, align: 'center' },
            { header: "館別", dataIndex: 'class_name', width: 200, align: 'center' },
            {
                header: "供應商", dataIndex: 'vendor_name_simple', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.Brand_Id + ")'  >" + value + "</span>";
                }
            },
            { header: "品牌名稱", dataIndex: 'Brand_Name', width: 180, align: 'center' },
            { header: "排序", dataIndex: 'Brand_Sort', width: 100, align: 'center' },
             {
                 header: "形象圖",
                 dataIndex: 'Image_Name',
                 width: 80,
                 align: 'center',
                 xtype: 'templatecolumn',
                 tpl: '<a target="_blank" href="{Image_Name}" ><img width=50 name="tplImg"  height=50 src="{Image_Name}" /></a>'

             },
             {
                 header: "安心聲明圖",
                 dataIndex: 'Resume_Image',
                 width: 80,
                 align: 'center',
                 xtype: 'templatecolumn',
                 tpl: '<a target="_blank" href="{Resume_Image}" ><img width=50 name="tplImg"  height=50 src="{Resume_Image}" /></a>'

             },
             {
                 header: "促銷圖",
                 dataIndex: 'Promotion_Banner_Image',
                 width: 80,
                 align: 'center',
                 xtype: 'templatecolumn',
                 tpl: '<a target="_blank" href="{Promotion_Banner_Image}" ><img width=50 name="tplImg" height=50 src="{Promotion_Banner_Image}" /></a>'

             },

            {
                header: "狀態", dataIndex: 'Brand_Status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 2) {
                        return "<span style='color:red;' border='1px'>隱藏</span>";
                    } else if (value == 1) {
                        return "顯示";
                    }
                }
            },
            {
                header: "品牌故事",
                dataIndex: 'Brand_Id',
                width: 150,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != null) {
                        return Ext.String.format("<a href='javascript:void(0);' onclick='TranToPictureMaintain({0})'>品牌故事</a>", record.data.Brand_Id);
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
            {
                xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick},
            '->',
           { xtype: 'combobox', editable: false, fieldLabel: "查詢條件", labelWidth: 59, id: 'serchs', width: 200, store: DDLStore, displayField: 'txt', valueField: 'value', value: 0 },
           {
               xtype: 'textfield', fieldLabel: "查詢內容", id: 'serchcontent', labelWidth: 59,
               listeners: {
                   specialkey: function (field, e) {
                       if (e.getKey() == e.ENTER) {
                           Query();
                       }
                   }
               }
           },
           {
               text: SEARCH, iconCls: 'icon-search', id: 'btnQuery', handler: Query              
           },
           {
               text: '重置', id: 'reset', iconCls: 'ui-icon ui-icon-reset', handler: function () {
                   Ext.getCmp('serchs').setValue(0);
                   Ext.getCmp('serchcontent').setValue('');
               }
           }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorBrandStore,
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
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //VendorBrandStore.load({ params: { start: 0, limit: 25} });
});
function SecretLogin(rid) {//secretcopy
    var secret_type = "7";//參數表中的"供應商查詢列表"
    var url = "/Vendor/VendorBrandList";
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


/********************************新增**********************************/
onAddClick = function () {
    addFunction(null, VendorBrandStore);
}

/*******************************編輯***********************************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        if (VendorStore.findExact("vendor_id", row[0].data.Vendor_Id, 0) > -1) {
            var secret_type = "7";//參數表中的"供應商查詢列表"
            var url = "/Vendor/VendorBrandList/Edit";
            var ralated_id = row[0].data.Brand_Id;
            boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
            if (boolPassword != "-1") {
                if (boolPassword) {//驗證
                    SecretLoginFun(secret_type, ralated_id, true, false, true, url, null, null, null);//先彈出驗證框，關閉時在彈出顯示框
                } else {
                    editFunction(ralated_id);
                }
            }
        }
        else {
            Ext.Msg.alert(INFORMATION, "失格供應商下的品牌不可編輯！");
        }
    }

}

//訂單編號跳轉
function TranToPictureMaintain(brandId) {
    var url = '/Vendor/PictureMaintain?Brand_Id=' + brandId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#PictureMaintain');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'PictureMaintain',
        title: '圖檔維護',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}