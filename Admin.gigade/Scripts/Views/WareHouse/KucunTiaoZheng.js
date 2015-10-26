

Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
var arr = [];
var ff = [];
/*********參數表model***********/
Ext.define("gigade.kucuntiaozhengModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'ParameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});

//庫調原因
var KutiaoStore = Ext.create("Ext.data.Store", {
    model: 'gigade.kucuntiaozhengModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/Getkutiaowhy",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
Ext.define('GIGADE.KucunMessage', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "prod_sz", type: "string" },
        { name: "create_dtim", type: "string" },
        { name: "prod_qty", type: "string" },
        { name: "cde_dt", type: "string" },
        { name: "movenum", type: "int" },
        { name: "cde_dt_incr", type: "int" },//保質天數 也就是有效日期和製造日期的差值
        { name: "cde_dt_var", type: "int" },//允收天數
        { name: "po_id", type: "string" },
        { name: "cde_dt_make", type: "string" },//製造日期
        { name: "pwy_dte_ctl", type: "string" },//有效期控管
        { name: "vendor_id", type: "string" }//有效期控管
    ]
});
//獲取grid中的數據
var KucunTiaozhengStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.KucunMessage',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GeKuCunList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
//加載前先獲取ddl的值
KucunTiaozhengStore.on('beforeload', function () {
    Ext.apply(KucunTiaozhengStore.proxy.extraParams, {
        prod_id: Ext.getCmp('item_id').getValue(),
        sloc_id: Ext.getCmp('ktloc_id').getValue()
    })
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 160,
        border: 0,
        bodyPadding: 10,
        width: 1000,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        fieldLabel: '庫調單號',
                        id: 'doc_no',
                        colName: 'doc_no',
                        submitValue: false,
                        width: 273,
                        name: 'doc_no',
                        listeners: {
                            beforerender: function () {
                                Ext.Ajax.request({
                                    url: "/WareHouse/Getkutiaonumber",//獲取庫調單號
                                    method: 'post',
                                    type: 'text',
                                    params: {
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(form.responseText);
                                        if (result.success) {
                                            Ext.getCmp('doc_no').setValue(result.msg);
                                            Ext.getCmp('KT_no').setValue(result.msg);
                                        }
                                    }
                                });
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'iarc_id',
                        colName: 'iarc_id',
                        name: 'iarc_id',
                        store: KutiaoStore,
                        allowBlank: false,
                        width: 273,
                        editable: false,
                        typeAhead: true,
                        forceSelection: false,
                        queryMode: 'local',
                        fieldLabel: '調整代碼',
                        displayField: 'parameterName',
                        valueField: 'ParameterCode',
                        value: 'DR',
                        listeners: {
                            select: function () {
                                if (Ext.getCmp('iarc_id').getValue() == 'DR' || Ext.getCmp('iarc_id').getValue() == 'KR') {
                                    Ext.getCmp('po_id').show();
                                }
                                else {
                                    Ext.getCmp('po_id').setValue("");
                                    Ext.getCmp('po_id').hide();
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
                width: 1000,
                items: [
                {
                    xtype: 'textfield',
                    fieldLabel: '商品細項編號',
                    id: 'item_id',
                    colName: 'item_id',
                    submitValue: false,
                    name: 'item_id',
                    listeners: {
                        blur: function () {
                            var id = Ext.getCmp('item_id').getValue();
                            if (id.trim() == "")
                            {
                                return;
                            }
                            Ext.Ajax.request({
                                url: "/WareHouse/GetProdInfo",
                                method: 'post',
                                type: 'text',
                                params: {
                                    id: id
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        msg = result.msg;
                                        loc_name = result.locname;
                                        vendor_id = result.vendor_id;
                                        if (loc_name == null || loc_name == "") {
                                            if (result.yunsongtype == 2) {
                                                Ext.getCmp("mode_type").setValue("主料位:YY999999");
                                            }
                                            else if (result.yunsongtype == 3) {
                                                Ext.getCmp("mode_type").setValue("主料位:ZZ999999");
                                            }
                                        }
                                        else {
                                            Ext.getCmp("mode_type").setValue("主料位:" + loc_name);
                                        }
                                        Ext.getCmp("item_name").setValue(msg);
                                    }
                                    else {
                                        Ext.getCmp("item_name").setValue("沒有該商品信息！");
                                        Ext.getCmp("mode_type").setValue("");
                                    }
                                }
                            });
                            KucunTiaozhengStore.removeAll();
                            Ext.getCmp("KucunTiaozhengGrid").store.loadPage(1, {
                                params: {
                                    prod_id: Ext.getCmp('item_id').getValue(),
                                    sloc_id: Ext.getCmp('ktloc_id').getValue()
                                }
                            });
                        }
                    }
                },
                {
                    xtype: 'displayfield',
                    width: 15
                },
                {
                    xtype: 'displayfield',
                    id: 'item_name',
                    width: 400
                },
                {
                    xtype: 'displayfield',
                    id: 'mode_type',
                    width: 150
                }]                     
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '庫調料位',
                        id: 'ktloc_id',
                        regex: /^[A-Za-z]{2}\d{3}[A-Za-z]\d{2}$/,  
                        regexText: "料位不規則",
                        colName: 'ktloc_id',
                        submitValue: false,
                        name: 'ktloc_id',
                        listeners: {
                                specialkey: function (field, e) {
                                    if (e.getKey() == Ext.EventObject.ENTER) {
                                        KucunTiaozhengStore.removeAll();
                                        Ext.getCmp("KucunTiaozhengGrid").store.loadPage(1, {
                                            params: {
                                                prod_id: Ext.getCmp('item_id').getValue(),
                                                sloc_id: Ext.getCmp('ktloc_id').getValue()
                                            }
                                        });
                                        if (Ext.getCmp('ktloc_id').getValue().trim() == null || Ext.getCmp('ktloc_id').getValue().trim() == "" || Ext.getCmp('item_name').getValue().trim() == "沒有該商品信息！") {
                                            Ext.Msg.alert(INFORMATION, "請先輸入正確的料位!");
                                        }
                                        else {
                                            if (Ext.getCmp('item_id').getValue() == null || Ext.getCmp('item_id').getValue().trim() == "") {//根據料位找到其對應的item_id 然後查找其他信息
                                                Ext.Ajax.request({
                                                    url: "/WareHouse/GetProductInfoByLocId",//判斷item_id和loc_id的關係
                                                    params: {
                                                        loc_id: Ext.getCmp('ktloc_id').getValue()
                                                    },
                                                    success: function (response) {
                                                        var result = Ext.decode(response.responseText);
                                                        if (result.success) {
                                                            Ext.getCmp("item_id").setValue(result.item_id);
                                                            Ext.getCmp("item_name").setValue(result.msg);
                                                            Ext.getCmp("mode_type").setValue("主料位:" + Ext.getCmp('ktloc_id').getValue());
                                                            Ext.Ajax.request({
                                                                url: "/WareHouse/AboutItemidLocid",//判斷item_id和loc_id的關係
                                                                params: {
                                                                    titem_id: Ext.getCmp('item_id').getValue(),
                                                                    tloc_id: Ext.getCmp('ktloc_id').getValue()
                                                                },
                                                                success: function (response) {
                                                                    var result = Ext.decode(response.responseText);
                                                                    if (result.msg == 0) {
                                                                        Ext.Msg.alert(INFORMATION, "細項編號和庫調料位不對應!");
                                                                    }
                                                                }
                                                            });
                                                        }
                                                    }
                                                });
                                            }
                                            else {
                                                Ext.Ajax.request({
                                                    url: "/WareHouse/AboutItemidLocid",//判斷item_id和loc_id的關係
                                                    params: {
                                                        titem_id: Ext.getCmp('item_id').getValue(),
                                                        tloc_id: Ext.getCmp('ktloc_id').getValue()
                                                    },
                                                    success: function (response) {
                                                        var result = Ext.decode(response.responseText);
                                                        if (result.msg == 0) {
                                                            Ext.Msg.alert(INFORMATION, "細項編號和庫調料位不對應!");
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                    }
                                }
                            //}
                           // blur: function () {
                              
                            //}
                        }
                    },
                    {
                        xtype: 'displayfield',
                        width: 18
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '前置單號',
                        id: 'po_id',
                        width: 273,
                        colName: 'po_id',
                        submitValue: false,
                        name: 'po_id'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '備註',
                        id: 'remarks',
                        margin: '0 0 0 13',
                        width: 273,
                        colName: 'remarks',
                        submitValue: false,
                        name: 'remarks'
                    }
                ]
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                     {
                         xtype: 'textfield',
                         fieldLabel: '庫調單號',
                         width: 255,
                         id: 'KT_no',
                         colName: 'KT_no',
                         submitValue: false,
                         regex: /^[K][0-9]{14}$/,
                         name: 'KT_no'
                     },
                    {
                        xtype: 'displayfield',
                        width: 48
                    },
                    {
                        xtype: 'button',
                        icon: '../../../Content/img/icons/excel.gif',
                        text: "打印庫調單",
                        handler: PrintKT
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
                        width: 103
                    },
                    {
                        xtype: 'button',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'displayfield',
                        width: 48
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('item_id').setValue("");
                                Ext.getCmp('ktloc_id').setValue("");
                                Ext.getCmp('item_name').setValue("");
                                Ext.getCmp('mode_type').setValue("");
                                Ext.getCmp('remarks').setValue("");
                                Ext.getCmp('po_id').setValue("");
                                KucunTiaozhengStore.removeAll();
                                Ext.getCmp("KucunTiaozhengGrid").down('#add_new_message').setDisabled(true);
                                Ext.getCmp('KT_no').setValue(Ext.getCmp('doc_no').getValue());
                            }
                        }
                    }
                ]
            }
        ]
    });

    var KucunTiaozhengGrid = Ext.create('Ext.grid.Panel', {
        id: 'KucunTiaozhengGrid',
        store: KucunTiaozhengStore,
        height: 609,
        columnLines: true,
        frame: true,
        columns: [
            { header: "", dataIndex: 'row_id', width: 150, hidden: true, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 200, align: 'center' },
            { header: "商品規格", dataIndex: 'prod_sz', width: 200, align: 'center' },
            {
                header: "製造日期", dataIndex: 'cde_dt_make', width: 150, align: 'center',
                renderer: Ext.util.Format.dateRenderer('Y-m-d'),
                editor: {
                    xtype: 'datefield',
                    format: 'Y-m-d',
                    allowBlank: false
                }
            },
            {
                header: "有效日期", dataIndex: 'cde_dt', width: 80, align: 'center',
                renderer: Ext.util.Format.dateRenderer('Y-m-d'), editor: {
                    xtype: 'datefield',
                    format: 'Y-m-d',
                    allowBlank: false
                }
            },
            { header: "庫存數量", dataIndex: 'prod_qty', width: 150, align: 'center' },
            {
                header: "調整數量", dataIndex: 'movenum', width: 180, align: 'center',
                editor: {
                    xtype: 'numberfield',
                    id: 'mnum',
                    minValue: 0
                }
            },
            {
                header: "加/減", width: 150, align: 'center', renderer:
                function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return " <button type='button' onclick='function_add(" + record.data.row_id + "," + record.data.prod_qty + "," + record.data.movenum +")'>加+</button>&nbsp;&nbsp;&nbsp;<button type='button' onclick='function_uadd(" + record.data.row_id + "," + record.data.prod_qty + "," + record.data.movenum +")'>減-</button>"
                }
            }
        ],
        selType: 'cellmodel',
        plugins: [ Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 } ) ],
        viewConfig: { emptyText: '<span>暫無數據！</span>' },
        tbar: [
           { xtype: 'button', text: "新增庫存", id: 'add_new_message', name: 'add_new_message', iconCls: 'icon-user-add', handler:
                 function () {
                    
                             StoreAdd();
                      
                 }
           }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: KucunTiaozhengStore,
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
            },
            edit: function (editor, e) {
                if (e.field == "cde_dt_make" || e.field == "cde_dt") {
                    //如果有效日期更改的話，就更新有效時間
                    var i = 0;
                    if (e.field == "cde_dt_make") {
                        i = 1;//1表示編輯的是cde_dt_make
                    }
                    else {
                        i = 2;//1表示編輯的是cde_dt
                    }
                    //alert(Ext.Date.format(e.value, 'Y-m-d'));
                    //alert(e.originalValue);
                    if (Ext.Date.format(e.value, 'Y-m-d') != e.originalValue) {
                            Ext.Ajax.request({
                                url: "/WareHouse/selectproductexttime",
                                params: {
                                    item_id: Ext.getCmp('item_id').getValue()
                                },
                                success: function (response) {
                                    var result = Ext.decode(response.responseText);
                                    var datetimes = 0;
                                    datetimes = result.msg;
                                    Ext.Ajax.request({
                                        url: "/WareHouse/aboutmadetime",
                                        params: {
                                            cde_dtormade_dt: e.value,//現在的日期
                                            y_cde_dtormade_dt: e.originalValue,//原來的日期
                                            row_id: e.record.data.row_id,//row_id
                                            prod_qtys: e.record.data.prod_qty,//庫存數量
                                            type_id: i,//判斷編輯cde_dt_make 或者 cde_dt 
                                            datetimeday: datetimes, //0
                                            prod_id: Ext.getCmp('item_id').getValue(),//item_id
                                            po_id: Ext.getCmp('po_id').getValue(),//前置單編號
                                            iarc_id: Ext.getCmp('iarc_id').getValue(),//庫調原因
                                            sloc_id: Ext.getCmp('ktloc_id').getValue(),//料位編號
                                            doc_no: Ext.getCmp('doc_no').getValue(),//庫調單編號
                                            remarks: Ext.getCmp('remarks').getValue()//備註
                                        },
                                        success: function (response) {
                                            var result = Ext.decode(response.responseText);
                                            var message;
                                            switch (result.msg) {
                                                case 1:
                                                    message = " 製造日期不能大於當前時間!";
                                                    Ext.Msg.alert(INFORMATION, message);
                                                    break;
                                                case 3:
                                                    message = " 修改失敗!";
                                                    Ext.Msg.alert(INFORMATION, message);
                                                    break;
                                            }
                                            KucunTiaozhengStore.load();
                                        }
                                    });
                                }
                            });
                    }
                    else {
                        KucunTiaozhengStore.load();
                    }
                }
                //如果編輯的是轉移數量
                if (e.field == "movenum") {
                    //如果轉移數量不為零的話
                    if (e.value != e.originalValue) {

                        arr.push(e.record.data.row_id + "/" + e.record.data.movenum);
                        //}
                        var message = "";
                        var istip = false;


                        if (Ext.Array.contains(ff, e.rowIdx)) {
                            var index = Ext.Array.indexOf(ff, e.rowIdx, 0);
                            if (e.value == e.record.data.prod_qty) {
                                Ext.Array.splice(ff, parseInt(index + 1), 1, "true");
                            }
                            else if (e.value < e.record.data.prod_qty) {
                                Ext.Array.splice(ff, parseInt(index + 1), 1, "false");

                            }
                        }
                            //不存在就添加
                        else {
                            ff.push(e.rowIdx);
                            if (e.value == e.record.data.prod_qty) {
                                ff.push("true");
                            }
                            else if (e.value < e.record.data.prod_qty) {
                                ff.push("false");
                            }
                        }
                        if (e.rowIdx != 0) {
                            if (ff.length > 2) {
                                for (var i = 0; i < ff.length; i = i + 2) {
                                    if (parseInt(ff[i]) < parseInt(e.rowIdx)) {
                                        if (ff[i + 1] == "false") {
                                            istip = true;
                                        }
                                    }
                                }
                            }
                            else {
                                istip = true;
                            }
                        }
                        if (istip == true) {
                            message += "請遵守先進先出原則！";
                        }

                        /*檢查日期是否過期開始*/
                        Ext.Ajax.request({
                            url: "/WareHouse/JudgeDate",
                            method: 'post',
                            type: 'text',
                            params: {
                                dtstring: 1,
                                item_id: Ext.getCmp('item_id').getValue(),
                                startTime: e.record.data.cde_dt_make
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    if (result.msg == "3") {
                                        editFunction(message + "超過允出天數!");
                                        return;
                                    } else if (result.msg == "4") {
                                        editFunction("有效期為" + result.dte + "的商品已超過有效日期!");
                                        return;
                                    } else if (message != "") {
                                        editFunction(message);
                                    }
                                } else {
                                    if (message !== "") {
                                        Ext.Msg.alert(INFORMATION, message);
                                    }
                                }
                            },
                            failure: function (form, action) {
                                Ext.Msg.alert(INFORMATION, "該商品日期驗證出問題！請聯繫管理員~");
                            }
                        });

                    }
                }
            }
        }
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, KucunTiaozhengGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                KucunTiaozhengGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    KucunTiaozhengStore.load({ params: { start: 0, limit: 25 } });
})

function StoreAdd() {
    if (Ext.getCmp('ktloc_id').getValue().trim() == null || Ext.getCmp('ktloc_id').getValue().trim() == "" || Ext.getCmp('item_id').getValue().trim() == "" || Ext.getCmp('item_id').getValue() == null || Ext.getCmp('item_name').getValue().trim() == "沒有該商品信息！") {
        Ext.Msg.alert(INFORMATION, "請先輸入正確的商品細項編號或料位!");
    }
    else {
       
                addFunction(null, KucunTiaozhengStore, vendor_id);
    }
}

//查询
Query = function () {
    if (Ext.getCmp('ktloc_id').getValue().trim() == null || Ext.getCmp('ktloc_id').getValue().trim() == "" || Ext.getCmp('item_id').getValue().trim() == "" || Ext.getCmp('item_id').getValue() == null || Ext.getCmp('item_name').getValue().trim() == "沒有該商品信息！") {
        Ext.Msg.alert(INFORMATION, "請先輸入正確的商品細項編號或料位!");
    }
    else {
        //$.get('/WareHouse/GetStockByProductId', { 'loc_id': Ext.getCmp("ktloc_id").getValue() }, function (data) {
        //    //var datadata = data.parseJSON();
        //    var datadata = eval('(' + data + ')');
        //    if (datadata.islock != "0") {
        //        Ext.Msg.alert("提示", "該料位有上鎖的庫存，不能庫調!");
        //    }
        //    else {
                KucunTiaozhengStore.removeAll();
                Ext.getCmp("KucunTiaozhengGrid").store.loadPage(1, {
                    params: {
                        prod_id: Ext.getCmp('item_id').getValue(),
                        sloc_id: Ext.getCmp('ktloc_id').getValue()
                    }
                });
        //    }
           
        //});
       
    }
}

PrintKT = function ()
{
    var KtNO = Ext.getCmp('KT_no');

    if (KtNO.getValue().trim().length == 0) {
        Ext.Msg.alert("提示", "請輸入庫調單號");
    }
    else if (!KtNO.isValid())
    {
        Ext.Msg.alert("提示", "庫調單號格式錯誤!");
    }
    else
    {
        Ext.MessageBox.confirm("提示", "該單號是否庫調完成？", function (btn) {
            if (btn == "yes") {
                window.open("/WareHouse/KTPrintPDF?KT_NO=" + KtNO.getValue());
                window.location.reload();
            }
        })
    }
}
//加
function function_add(i, j, z, th) {

            if (Ext.getCmp('iarc_id').getValue() == 'DR' || Ext.getCmp('iarc_id').getValue() == 'KR') {
                if (Ext.getCmp('po_id').getValue().trim() == "") {
                    Ext.Msg.alert(INFORMATION, "前置單號不能為空");
                    return;
                }
            }
            if (z == 0) {
                Ext.Msg.alert(INFORMATION, "調整量不能為0");
                return;
            }
            else {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                myMask.show();
                Ext.Ajax.request({//調整庫存數量
                    url: "/WareHouse/InsertIialg",
                    params: {
                        row_id: i,//該行數據的行號碼
                        item_id: Ext.getCmp('item_id').getValue(),//商品細項編號
                        po_id: Ext.getCmp('po_id').getValue(),//前置單編號
                        iarc_id: Ext.getCmp('iarc_id').getValue(),
                        ktloc_id: Ext.getCmp('ktloc_id').getValue(),//料位編號
                        doc_no: Ext.getCmp('doc_no').getValue(),
                        remarks: Ext.getCmp('remarks').getValue(),//備註
                        benginnumber: j,//庫存
                        changenumber: z,//庫調數
                        kutiaotype: 1 //1表示加
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                        if (result.success) {
                          
                            Ext.Ajax.request({//調整庫存數量
                                url: "/WareHouse/KutiaoAddorReduce",
                                params: {
                                    row_id: i,//該行數據的行號碼
                                    benginnumber: j,//庫存
                                    changenumber: z,//庫調數
                                    kutiaotype: 1, //1表示加
                                    item_id: Ext.getCmp('item_id').getValue(),//商品細項編號
                                    po_id: Ext.getCmp('po_id').getValue(),//前置單編號
                                    doc_no: Ext.getCmp('doc_no').getValue(),
                                    remarks: Ext.getCmp('remarks').getValue(),//備註
                                    iarcid: Ext.getCmp('iarc_id').getValue()//調整代碼
                                 
                                },
                                success: function (response) {
                                    var result = Ext.decode(response.responseText);
                                    if (result.success) {
                                        myMask.hide();
                                        KucunTiaozhengStore.load();
                                        //Ext.Msg.alert(INFORMATION, "操作成功!");
                                        //setTimeout('Loadthis()', 4000);
                                       
                                    }
                                }
                            });
                        }
                        else {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, "插入表Iialg表失敗");
                        }
                    }
                });
            }

     
}
//減
function function_uadd(i, j, z) {
 
            if (Ext.getCmp('iarc_id').getValue() == 'DR' || Ext.getCmp('iarc_id').getValue() == 'KR') {
                if (Ext.getCmp('po_id').getValue().trim() == "") {
                    Ext.Msg.alert(INFORMATION, "前置單號不能為空");
                    return;
                }
            }
            if (z == 0) {
                Ext.Msg.alert(INFORMATION, "調整量不能為0");
                return;
            }
            else if (z > j) {
                Ext.Msg.alert(INFORMATION, "調整量不能大於庫存量");
                return;
            }
            else {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                myMask.show();
                Ext.Ajax.request({//調整庫存數量
                    url: "/WareHouse/InsertIialg",
                    params: {
                        row_id: i,//該行數據的行號碼
                        item_id: Ext.getCmp('item_id').getValue(),//商品細項編號
                        po_id: Ext.getCmp('po_id').getValue(),//前置單編號
                        iarc_id: Ext.getCmp('iarc_id').getValue(),
                        ktloc_id: Ext.getCmp('ktloc_id').getValue(),//料位編號
                        doc_no: Ext.getCmp('doc_no').getValue(),
                        remarks: Ext.getCmp('remarks').getValue(),//備註
               
                        benginnumber: j,//庫存
                        changenumber: z,//庫調數
                        kutiaotype: 2 //2表示減
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                        if (result.success) {
                          
                            Ext.Ajax.request({//調整庫存數量
                                url: "/WareHouse/KutiaoAddorReduce",
                                params: {
                                    row_id: i,//該行數據的行號碼
                                    benginnumber: j,//庫存
                                    changenumber: z,//庫調數
                                    kutiaotype: 2,//2表示減
                                    item_id: Ext.getCmp('item_id').getValue(),//商品細項編號
                                    po_id: Ext.getCmp('po_id').getValue(),//前置單編號
                                    doc_no: Ext.getCmp('doc_no').getValue(),
                                    iarcid: Ext.getCmp('iarc_id').getValue(),//調整代碼
                                    remarks: Ext.getCmp('remarks').getValue()//備註

                                },
                                success: function (response) {
                                 
                                    var result = Ext.decode(response.responseText);
                                    if (result.success) {
                                        myMask.hide();
                                        //Ext.Msg.alert(INFORMATION, "操作成功!");
                                        //setTimeout('Loadthis()', 4000);
                                        KucunTiaozhengStore.load();
                                    }
                                }
                            });
                        }
                        else {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, "插入表Iialg表失敗");
                        }
                    }
                });
            }
       
}


function Loadthis() {
    KucunTiaozhengStore.load();
}