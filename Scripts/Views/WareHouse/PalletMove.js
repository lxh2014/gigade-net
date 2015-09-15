var CallidForm;
//var datetimes=0;//保質期
var pageSize = 25;
var rowid = [];
var arr = [];
var ff = [];
var edata = [];
//補貨到主料位Model
Ext.define('gigade.Innvd', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "create_dtim", type: "string" },
        { name: "prod_qty", type: "string" },
        { name: "cde_dt", type: "string" },
        { name: "movenum", type: "int" },
        { name: "cde_dt_incr", type: "int" },//保質天數
        { name: "cde_dt_var", type: "int" },//允收天數
        { name: "cde_dt_make", type: "string" },//製造日期
        { name: "pwy_dte_ctl", type: "string" }//有效期控管
    ]
});

var FInnvdStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Innvd',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetFPalletList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var SInnvdStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Innvd',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetSPalletList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
FInnvdStore.on('beforeload', function () {
    Ext.apply(FInnvdStore.proxy.extraParams,
        {
            prod_id: Ext.getCmp('item_id').getValue(),
            sloc_id: Ext.getCmp('sloc_id').getValue()
        });
});
SInnvdStore.on('beforeload', function () {
    Ext.apply(SInnvdStore.proxy.extraParams,
        {
            prod_id: Ext.getCmp('item_id').getValue(),
            eloc_id: Ext.getCmp('eloc_id').getValue()
        });
});
function Query(x) {
    FInnvdStore.removeAll();
    Ext.getCmp("gdFInnvd").store.loadPage(1, {
        params: {
            prod_id: Ext.getCmp('item_id').getValue(),
            sloc_id: Ext.getCmp('sloc_id').getValue()
        }
    });
    SInnvdStore.removeAll();
    Ext.getCmp("gdSInnvd").store.loadPage(1, {
        params: {
            prod_id: Ext.getCmp('item_id').getValue(),
            eloc_id: Ext.getCmp('eloc_id').getValue()
        }
    });
}

function btnQuery(x)
{
    Ext.Ajax.request({
        url: "/WareHouse/SavePallet",
        params: {
            //rowid: rowid,
            //number: number,
            num: arr,
            sloc_id: Ext.getCmp('sloc_id').getValue(),
            eloc_id: Ext.getCmp('eloc_id').getValue(),
            prod_id: Ext.getCmp('item_id').getValue()
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            //if (result.success) {
            var message;
            switch (result.msg) {
                case "0":
                    message = "源料位的數量小於轉移數量 !";
                    break;
                case "1":
                    message = "此商品料位未指定或者已被別的商品佔用 !";
                    break;
                case "2":
                    message = "請輸入轉移數量!";
                    break;
                case "3":
                    message = "補貨成功 !";
                    break;
                case "4":
                    message = "目标料位非商品指定主料位!";
                    break;
                case "5":
                    message = "料位相同!";
                    break;
                case "6":
                    message = "源料位被鎖,不可使用!";
                    break;
                case "7":
                    message = "目標料位不存在,不可使用!";
                    break;
                case "8":
                    message = "目標料位被锁,不可使用!";
                    break;
                default:
                    message = "操作失敗 !";
                    break;
            }
            Ext.Msg.alert(INFORMATION, message);
            FInnvdStore.load();
            SInnvdStore.load();
            arr.splice(0);
            //} else {
            //    Ext.Msg.alert(INFORMATION, result.msg);
            //}

        },
        failure: function (response) {
            alert(111);
            //var result = Ext.decode(response.responseText);
            //Ext.Msg.alert(INFORMATION, result.msg); 
        }
    });
}

Ext.onReady(function () {
    var FForm = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        //renderTo: Ext.getBody(),
        width: 1200,
        height: 135,
        border: false,
        plain: true,
        bodyPadding: 20,
        id: 'ffrom',
        items: [
             {
                 xtype: 'fieldcontainer',
                 id: 'validate',
                 submitValue: true,
                 layout: 'hbox',
                 items: [
                     {
                         xtype: 'textfield', id: 'item_id', fieldLabel: PUPCID, labelWidth: 120, listeners: {
                             blur: function () {
                                 var id = Ext.getCmp('item_id').getValue();
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
                                             if (loc_name == null || loc_name == "") {
                                                 Ext.getCmp('loc_name').hide();
                                                 if (result.yunsongtype == 2) {
                                                     Ext.getCmp("mode_type").setValue("主料位:YY999999");
                                                 }
                                                 else if (result.yunsongtype == 3) {
                                                     Ext.getCmp("mode_type").setValue("主料位:ZZ999999");
                                                 }
                                             }
                                             else {
                                                 Ext.getCmp('loc_name').show();
                                                 Ext.getCmp("loc_name").setValue("主料位:" + loc_name);
                                                 Ext.getCmp('mode_type').setValue("");
                                             }
                                             Ext.getCmp("product_name").setValue(msg);
                                         }
                                         else {
                                             Ext.getCmp('loc_name').show();
                                             Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                             Ext.getCmp("loc_name").setValue("");
                                             Ext.getCmp("mode_type").setValue("");
                                         }
                                     }
                                 });
                             }
                         }
                     },
                    { xtype: 'displayfield', width: 5 },
                    { xtype: 'displayfield', id: 'product_name', width: 400 },
                    { xtype: 'displayfield', id: 'loc_name', width: 150 },
                    { xtype: 'displayfield', id: 'mode_type', width: 150 }
                 ]
             },
                     //{ xtype: 'textfield', id: 'upc_id', fieldLabel: '條碼編號', labelWidth: 80 },
             { xtype: 'textfield', id: 'sloc_id', fieldLabel: YLOCID, labelWidth: 120 },
             {
                 xtype: 'fieldcontainer',
                 id: 'search',
                 submitValue: true,
                 layout: 'hbox',
                 margin: '10 0 0 0',
                 items: [
                      { xtype: 'displayfield', width: 173 },
                      { xtype: 'button', id: 'btn_search', text: SEARCH, width: 100, handler: Query },
                      { xtype: 'displayfield', width: 315 },
                      { xtype: 'button', id: 'btn_sure', text: "搬移", width: 100, handler: btnQuery },
                       { xtype: 'displayfield', width: 173 },
                      { xtype: 'textfield', id: 'eloc_id', fieldLabel: TUPCID, labelWidth: 120 }
                 ]
             }                    
        ]
        //,
        //buttonAlign: 'center',
        //buttons: [{
        //    text: SEARCH,
        //    id: 'btn_search',
        //    handler: Query,
        //    iconCls: 'ui-icon ui-icon-search-2'
        //}]
    });
    var gdFInnvd = Ext.create('Ext.grid.Panel', {
        id: 'gdFInnvd',
        title: YL,
        store: FInnvdStore,
        sortableColumns: false,
        columnLines: true,
        hidden: false,
        frame: true,
        width: 600,
        height: 300,
        columns: [
            { header: "", dataIndex: 'row_id', width: 150,hidden:true, align: 'center' },
            { header: PRODNAME, dataIndex: 'product_name', width: 150, align: 'center' },
            { header: NM, dataIndex: 'prod_qty', width: 70, align: 'center' },
            {
                header: PRODUCTMAKE, dataIndex: 'cde_dt_make', width: 120, align: 'center',
                renderer: Ext.util.Format.dateRenderer('Y-m-d')
                //,
                //editor: {
                //xtype: 'datefield',
                //format: 'Y-m-d',
                //allowBlank: false
                //}
            },
            {
                header: PRODUCTEFFECTIVE, dataIndex: 'cde_dt', width: 120, align: 'center', renderer: Ext.util.Format.dateRenderer('Y-m-d')
                //,
                //editor: {
                //    xtype: 'datefield',
                //    format: 'Y-m-d',
                //    // id:'c_dt',
                //    allowBlank: false
                //}
            },
            {
                header: MOVE, dataIndex: 'movenum', width: 80, align: 'center',
                editor: {
                    xtype: 'numberfield',
                    id: 'mnum',
                    minValue: 0,
                    listeners: {
                        change: function () {
                            var rowIndex = Ext.getCmp("gdFInnvd").getSelectionModel().getCurrentPosition().row;
                            var record = Ext.getCmp("gdFInnvd").getStore().getAt(rowIndex);
                            var prod_qty = record.data.prod_qty;
                            var mnum = Ext.getCmp("mnum").getValue();
                            if (parseInt(mnum) > parseInt(prod_qty)) {
                                Ext.Msg.alert(INFORMATION, "轉移數量須小於庫存數量！");
                                Ext.getCmp("mnum").setValue(prod_qty);
                                return;
                            }

                            //var cde_dt_make = record.data.cde_dt_make;
                            ////當前日期
                            //var allow_date = Ext.Date.format(new Date(), 'Y-m-d');
                            ////兩日期相差的天數
                            //var diff = (Date.parse(cde_dt_make) - Date.parse(allow_date)) / 86400000;
                            //if (parseInt(diff) < parseInt(record.data.cde_dt_var)) {
                            //    editFunction();
                            //}
                        }
                    }
                }

            }
        ],
        selType: 'cellmodel',
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            })
        ],
        viewConfig: {
            emptyText: '<span>暫無數據！</span>'
        },
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            },
            //cellclick:function(grid,rowIndex,columnIndex,e)
            //{
            //    var W = rowIndex;
            //    var q = e;
            //    // var record = grid.getStore().getAt(rowIndex);

            //},
            edit: function (editor, e) {
                //如果編輯的是有效日期
                //if (e.field == "cde_dt") {
                //    //如果有效日期更改的話，就更新有效時間
                //    if (e.value != e.originalValue) {
                //        Ext.Ajax.request({
                //            url: "/WareHouse/UpPalletTime",
                //            params: {
                //                cde_dt: e.value,
                //                row_id: e.record.data.row_id
                //            },
                //            success: function (response) {
                //                FInnvdStore.load();
                //            }
                //        });
                //    }

                //}
                //if (e.field == "cde_dt_make" || e.field == "cde_dt") {
                //    //如果有效日期更改的話，就更新有效時間
                //    var i = 0;
                //    if (e.field == "cde_dt_make")
                //    {
                //        i = 1;//1表示編輯的是cde_dt_make
                //    }
                //    else
                //    {
                //        i = 2;//1表示編輯的是cde_dt
                //    }
                //    if (e.value != e.originalValue) {

                //        Ext.Ajax.request({
                //            url: "/WareHouse/selectproductexttime",
                //            params: {
                //                item_id: Ext.getCmp('item_id').getValue()
                //            },
                //            success: function (response) {
                //                var result = Ext.decode(response.responseText);
                //                var datetimes = 0;
                //                datetimes = result.msg;
                //                Ext.Ajax.request({
                //                    url: "/WareHouse/aboutmadetime",
                //                    params: {
                //                        cde_dtormade_dt: e.value,
                //                        row_id: e.record.data.row_id,
                //                        prod_qtys:e.record.data.prod_qty,
                //                        type_id: i,
                //                        datetimeday: datetimes,
                //                        sloc_id: Ext.getCmp('sloc_id').getValue(),
                //                        prod_id: Ext.getCmp('item_id').getValue()
                //                    },
                //                    success: function (response) {
                //                        var result = Ext.decode(response.responseText);
                //                        var message;
                //                        switch (result.msg) {
                //                            case 1:
                //                                message = " 製造日期不能大於當前時間!";
                //                                Ext.Msg.alert(INFORMATION, message);
                //                                break;
                //                            case 3:
                //                                message = " 修改失敗!";
                //                                Ext.Msg.alert(INFORMATION, message);
                //                                break;
                //                        }
                //                        FInnvdStore.load();
                //                    }
                //                });
                //            }
                //        });

                //    }

                //}
                //如果編輯的是轉移數量
                if (e.field == "movenum") {
                    //如果轉移數量不為零的話
                    if (e.value != e.originalValue) {
                        //編輯后的傳值的數組，存row_id和movenum
                        //判斷數組中是否已存在相同的row_id，存在就替換，防止數組中存在重複的數據，因為編輯一次就傳入一次值
                        //if (Ext.Array.contains(edata,"0"+ e.record.data.row_id)) {
                        //    var index = Ext.Array.indexOf(edata, "0"+e.record.data.row_id, 0);
                        //    Ext.Array.splice(edata, parseInt(index+1),1,e.value);
                        //}
                        //不存在就添加
                        //else {
                        //    edata.push("0" + e.record.data.row_id);
                        //    edata.push(e.record.data.movenum);
                        //}
                        //var prod_qty = e.record.data.prod_qty;
                        //if (parseInt(e.value) > parseInt(prod_qty)) {
                        //    Ext.Msg.alert(INFORMATION, "轉移數量須小於庫存數量！");
                        //    return;
                        //}
                        //else {
                         arr.push(e.record.data.row_id + "/" + e.record.data.movenum);
                        //}
                        var message = "";
                        var istip = false;
                        //有效期控管的商品
                        //if (e.record.data.pwy_dte_ctl == "Y") {
                        //    var cde_dt_make = e.record.data.cde_dt_make;
                        //    //當前日期
                        //    var allow_date = Ext.Date.format(new Date(), 'Y-m-d');
                        //    //兩日期相差的天數
                        //    var diff = (Date.parse(allow_date) - Date.parse(cde_dt_make)) / 86400000;
                        //    if (parseInt(diff) > parseInt(e.record.data.cde_dt_var)) {
                        //        //editFunction();
                        //        message = "該商品已超過允收天數!";
                        //    }
                        //}

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
                        //if (message !== "") {
                        //    editFunction(message);
                        //}
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
                                    } else if (message !== "") {
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

                        /*檢查日期是否過期結束*/
                    }
                }
            }
        }
    });
    var form = Ext.create('Ext.form.Panel', {
        width: 90,
        height: 70,
        layout: 'anchor',
        border: false,
        plain: true,
        style: 'margin:100px 50px',
        defaults: {
            anchor: '95%'
        },
        flex: 0,
        defaultType: 'textfield',
        buttonAlign: 'center',
        buttons: [{
            text: CONFIRM,
            id: 'sure',
            hidden:true,
            handler: function () {
                //if (SInnvdStore.getCount()>0) {//FInnvdStore.getCount() > 0 && SInnvdStore.getCount()>0
                Ext.Ajax.request({
                    url: "/WareHouse/SavePallet",
                    params: {
                        //rowid: rowid,
                        //number: number,
                        num: arr,
                        sloc_id: Ext.getCmp('sloc_id').getValue().trim(),
                        eloc_id: Ext.getCmp('eloc_id').getValue(),
                        prod_id: Ext.getCmp('item_id').getValue()
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                        //if (result.success) {
                        var message;
                        switch (result.msg) {
                            case "0":
                                message = "源料位的數量小於轉移數量 !";
                                break;
                            case "1":
                                message = "此商品料位未指定或者已被別的商品佔用 !";
                                break;
                            case "2":
                                message = "請輸入轉移數量!";
                                break;
                            case "3":
                                message = "補貨成功 !";
                                break;
                            case "4":
                                message = "目标料位非商品指定主料位!";
                                break;
                            case "5":
                                message = "料位相同!";
                                break;
                            case "6":
                                message = "源料位被鎖,不可使用!";
                                break;
                            case "7":
                                message = "目標料位不存在,不可使用!";
                                break;
                            case "8":
                                message = "目標料位被锁,不可使用!";
                                break;
                            default:
                                message = "操作失敗 !";
                                break;
                        }
                        Ext.Msg.alert(INFORMATION, message);
                        FInnvdStore.load();
                        SInnvdStore.load();
                        arr.splice(0);
                        //} else {
                        //    Ext.Msg.alert(INFORMATION, result.msg);
                        //}

                    },
                    failure: function (response) {
                        var result = Ext.decode(response.responseText);
                        Ext.Msg.alert(INFORMATION, result.msg);
                    }
                });
                //}
                //else {
                //    Ext.Msg.alert(INFORMATION, "請輸入源料位！");
                //}
            }
        }]
    });

    var gdSInnvd = Ext.create('Ext.grid.Panel', {
        id: 'gdSInnvd',
        title: TL,
        store: SInnvdStore,
        sortableColumns: false,
        columnLines: true,
        frame: true,
        width: 600,
        height: 300,
        hidden: false,
        columns: [
            { header: PRODNAME, dataIndex: 'product_name', width: 150, align: 'center' },
            { header: NM, dataIndex: 'prod_qty', width: 70, align: 'center' },
            { header: PRODUCTMAKE, dataIndex: 'cde_dt_make', width: 120, align: 'center' },
            { header: PRODUCTEFFECTIVE, dataIndex: 'cde_dt', width: 120, align: 'center'}
        ],
        viewConfig: {
            emptyText: '<span>暫無數據！</span>'
        },
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    var SForm = Ext.create('Ext.form.Panel', {
        width: 1500,
        height: 800,
        border: false,
        plain: true,
        defaultType: 'displayfield',
        bodyPadding: 20,
        id: 'ImportFile',
        layout: {
            type: 'hbox'
        },
        items: [gdFInnvd, form, gdSInnvd]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        height: 1600,
        items: [FForm, SForm],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //gdIupc.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //SInnvdStore.load({ params: { start: 0, limit: 25 } });
});








