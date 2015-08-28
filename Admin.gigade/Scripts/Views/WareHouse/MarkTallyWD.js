var maxnumber = 0;
var arrPickRowId = new Array();
var arrPickInfo = new Array();
var pickCount = 0;
var ff = [];
var deliver_id;


//庫存Model
Ext.define('gigade.Stock', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'row_id', type: 'int' },
        { name: 'item_id', type: 'int' },
        { name: 'cde_dt', type: "string" },
        { name: 'made_date', type: "string" },
        { name: 'prod_qty', type: 'int' },
        { name: 'vendor_id', type: 'int' },
        { name: 'cde_dt_shp', type: 'int' }//允出天數
    ]
});
//庫存Store
var StockStore = Ext.create('Ext.data.Store', {
    pageSize: 25,
    model: 'gigade.Stock',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetStockByProductId',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//庫存信息載入之前添加查詢條件
StockStore.on('beforeload', function () {
    Ext.apply(StockStore.proxy.extraParams, {
        loc_id: Ext.getCmp("loc_id").getValue()
    });
});
StockStore.on('load', function () {
    $.get('/WareHouse/GetStockByProductId', { 'loc_id': Ext.getCmp("loc_id").getValue() }, function (data) {
        //var datadata = data.parseJSON();
        var datadata = eval('(' + data + ')');
        if (datadata.islock != "0") {
            Ext.getCmp('presentation').show();
        }
        if (datadata.totalCount > 0) {
            //Ext.getCmp('presentation').show();
            Ext.getCmp('deliver').hide();
        }
    });
});
function refushthis()
{
    StockStore.load();
}

//寄倉流程
Ext.onReady(function () {
    //庫存信息grid
    var gdStock = Ext.create('Ext.grid.Panel', {
        id: 'gdStock',
        title: '庫存信息',
        store: StockStore,
        columnLines: true,
        width: 600,
        height: 300,
        columnLines: true,
        frame: true,
        hidden: true,
        columns: [         
            {
                header: '製造日期', dataIndex: 'made_date', width: 100, align: 'center',
                renderer: Ext.util.Format.dateRenderer('Y-m-d'), editor: {
                    xtype: 'datefield',
                    format: 'Y-m-d',
                    allowBlank: false
                }
            },
            {
                header: '有效日期', dataIndex: 'cde_dt', width: 100, align: 'center',
                renderer: Ext.util.Format.dateRenderer('Y-m-d'), editor: {
                    xtype: 'datefield',
                    format: 'Y-m-d',
                    allowBlank: false
                }
            },
            { header: '庫存數量', dataIndex: 'prod_qty', width: 100, align: 'center' },
            {
                header: '撿貨數量',id: 'pick_num',dataIndex: 'pick_num', width: 100,align: 'center',    
                editor: {xtype: 'numberfield', id: 'pnum',minValue: 0}  
            }
        ],
        selType: 'cellmodel',
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            })
        ],
        viewConfig: {
            emptyText: '<span>暫無數據!</span>'
        },
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManageListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            },
            edit: function (editor, e) {
                //修改製造日期
                if (e.field == "made_date" || e.field == "cde_dt") {
                    //如果有效日期更改的話，就更新有效時間
                    var i = 0;
                    if (e.field == "made_date") {
                        i = 1;//1表示編輯的是cde_dt_make
                    }
                    else {
                        i = 2;//1表示編輯的是cde_dt
                    }
                    if (e.record.data.row_id != "" && e.record.data.row_id != "0") {
                        if (Ext.Date.format(e.value, 'Y-m-d') != e.originalValue) {
                         
                                Ext.Ajax.request({
                                    url: "/WareHouse/selectproductexttime",
                                    params: {
                                        item_id: e.record.data.item_id
                                    },
                                    success: function (response) {
                                        var result = Ext.decode(response.responseText);
                                        var datetimes = 0;
                                        datetimes = result.msg;
                                        Ext.Ajax.request({
                                            url: "/WareHouse/aboutmadetime",
                                            params: {
                                                cde_dtormade_dt: e.value,
                                                y_cde_dtormade_dt: e.originalValue,
                                                row_id: e.record.data.row_id,
                                                prod_qtys: e.record.data.prod_qty,
                                                type_id: i,
                                                datetimeday: datetimes,
                                                sloc_id: Ext.htmlEncode(Ext.getCmp('loc_id').getValue()),
                                                prod_id: e.record.data.item_id
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
                                                StockStore.load();
                                            }
                                        });
                                    }
                                });
                            }
                           
                        else {
                            StockStore.load();
                        }
                    }
                }
                if (e.field === "pick_num") {
                    if (e.value !== e.originalValue) {
                        var gdStock = Ext.getCmp("gdStock").store.data.items;
                        var pickNum = Ext.getCmp("out_qty").getValue();
                        var prod_qty = e.row.children[2].textContent;//選中行的庫存量
                        var pnum = e.value;//選中行的撿貨量
                        var made_date = e.row.children[0].textContent;//選中行的製造日期
                        Ext.getCmp('deliver').show();
                        pickCount = 0;  //重置實際撿貨量
                        for (var i = 0; i < gdStock.length; i++) {
                            if (gdStock[i].get("pick_num") !== 0 && !isNaN(gdStock[i].get("pick_num"))) {
                                pickCount += gdStock[i].get("pick_num");
                            }
                        }
                        Ext.getCmp("act_pick_qty").setValue(pickCount); //顯示當前撿貨數量

                        ////////////////////魚下方更換位置//////////////////////////////
                        //撿貨數量與庫存數量作比較
                        var row_id = e.record.data.row_id;
                        var inputValue = e.value;
                        arrPickRowId.push(row_id);
                        arrPickInfo.push(inputValue);
                        var message = "";
                        if (inputValue != "0" && inputValue != null) {
                            /*判斷先進先出*/
                            var istip = false;
                            if (Ext.Array.contains(ff, e.rowIdx)) {
                                var index = Ext.Array.indexOf(ff, e.rowIdx, 0);
                                if (e.value == e.record.data.prod_qty) {
                                    Ext.Array.splice(ff, parseInt(index + 1), 1, "true");
                                } else if (e.value < e.record.data.prod_qty) {
                                    Ext.Array.splice(ff, parseInt(index + 1), 1, "false");
                                }
                            } else {  //不存在就添加
                                ff.push(e.rowIdx);
                                if (e.value == e.record.data.prod_qty) {
                                    ff.push("true");
                                } else if (e.value < e.record.data.prod_qty) {
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
                                } else {
                                    istip = true;
                                }
                            }
                            if (istip == true) {
                                message += "請遵守先進先出原則！";
                                //Ext.getCmp("act_pick_qty").setValue(0);
                            }
                            /*檢查日期是否過期開始*/
                            Ext.Ajax.request({
                                url: "/WareHouse/JudgeDate",
                                method: 'post',
                                type: 'text',
                                params: {
                                    dtstring: 2,
                                    item_id: e.record.data.item_id,
                                    startTime: e.record.data.cde_dt
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        if (result.msg == "3") {
                                            message += "超過允出天數!";
                                            //editFunction(message + "超過允出天數!");
                                            //return;
                                        } else if (result.msg == "4") {
                                            message += "有效期為" + result.dte + "的商品已超過有效日期!!";
                                            //editFunction(message + "有效期為" + result.dte + "的商品已超過有效日期!");
                                            //return;
                                        }
                                        //else if (message.length > 0) {
                                        //    Ext.Msg.alert(INFORMATION, message);
                                        //}
                                    }
                                    //else {
                                    //    if (message.length > 0) {
                                    //        Ext.Msg.alert(INFORMATION, message);
                                    //    }
                                    //}
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "該商品日期驗證出問題！請聯繫管理員~");
                                }
                            });/*檢查日期是否過期結束*/
                        }
                        ////////////////////魚下方更換位置//////////////////////////////

                        ////////////////////魚上方更換位置//////////////////////////////
                        if (pickNum >= pickCount) {
                            Ext.getCmp("act_pick_qty").setValue(pickCount); //顯示當前撿貨數量
                            var R = 0; var S = 0;
                            Ext.Ajax.request({
                                url: "/WareHouse/GetSum",
                                method: 'post',
                                async: true,
                                type: 'text',
                                params: {
                                    item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),
                                    lcat_id: "S"
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        S = result.S;
                                        R = result.R;
                                        //if (parseInt(S) < parseInt(pnum)) {
                                        //    if (parseInt(R) > 0) {
                                        //        Ext.getCmp("pnum").setValue(0);
                                        //        Ext.getCmp("act_pick_qty").setValue(0);
                                        //        Ext.Msg.alert(INFORMATION, message+"請補貨主料位!");
                                        //    }
                                        //    else {
                                        //        Ext.MessageBox.confirm("提示",message + "該項目庫存不夠，是否庫調？", function (btn) {
                                        //            if (btn == "yes") {
                                        //                onAddClick();
                                        //            }
                                        //            else {
                                        //                Ext.getCmp("pnum").setValue(0);
                                        //                Ext.getCmp("act_pick_qty").setValue(0);
                                        //                return false;
                                        //            }
                                        //        });
                                        //    }
                                        //}
                                        //else
                                        if (parseInt(pnum) > parseInt(prod_qty)) {
                                            //2015-08-17  庫調邏輯變更:直接庫調
                                            if (Ext.getCmp("loc_id").getValue() == "YY999999")
                                            {
                                                Ext.getCmp("act_pick_qty").setValue(0);
                                                Ext.Msg.alert(INFORMATION, "該商品沒有主料位,不能庫調!");
                                                setTimeout('refushthis()', 4000)
                                            
                                            }
                                            else
                                            {
                                                Ext.MessageBox.confirm("提示", "該商品庫存不夠，確認直接進行商品數量庫調？", function (btn) {
                                                    if (btn == "yes") {
                                                        Ext.Ajax.request({
                                                            url: "/WareHouse/RFKT",
                                                            method: 'post',
                                                            type: 'text',
                                                            params: {
                                                                item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),
                                                                prod_qty: prod_qty,
                                                                pnum: pnum,
                                                                made_date: made_date,
                                                                order_id: Ext.getCmp("ord_id").getValue(),
                                                                loc_id: Ext.getCmp("loc_id").getValue()
                                                            },
                                                            success: function (form, action) {
                                                                var result = Ext.decode(form.responseText);
                                                                if (result.success) {
                                                                    if (result.msg == "100") {
                                                                        Ext.getCmp("act_pick_qty").setValue(0);
                                                                        StockStore.load();
                                                                    }
                                                                    if (result.msg == "2")
                                                                    {
                                                                        Ext.Msg.alert("提示", "今日的庫存已鎖，不能庫調");
                                                                        Ext.getCmp("act_pick_qty").setValue(0);
                                                                        Ext.getCmp("pnum").setValue(0);
                                                                        StockStore.load();
                                                                    }
                                                                } else {
                                                                    Ext.Msg.alert(INFORMATION, "系統故障,內部庫調失敗!");
                                                                }
                                                            }
                                                        });
                                                    }
                                                    else {
                                                        Ext.getCmp("pnum").setValue(0);
                                                        Ext.getCmp("act_pick_qty").setValue(0);
                                                        StockStore.load();
                                                        return false;
                                                    }
                                                })

                                            }
                                            //撿貨數量大於庫存判斷1.該輔料為沒有庫存進行庫調.2.輔料位有庫存可以去進行補貨.
                                            //Ext.Ajax.request({
                                            //    url: "/WareHouse/GetSum",
                                            //    method: 'post',
                                            //    type: 'text',
                                            //    params: {
                                            //        item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),
                                            //        lcat_id: "R",
                                            //        made_date: made_date
                                            //    },
                                            //    success: function (form, action) {
                                            //        var result = Ext.decode(form.responseText);
                                            //        if (result.success) {
                                            //            S = result.S;
                                            //            R = result.R;
                                            //            if (parseInt(R) > 0) {
                                            //                Ext.Msg.alert(INFORMATION,message+ "請補貨主料位!");
                                            //            }
                                            //            else {
                                            //                //Ext.MessageBox.confirm("提示", message+"該項目庫存不夠，是否庫調？", function (btn) {
                                            //                //    if (btn == "yes") {
                                            //                //        onAddClick();
                                            //                //    }
                                            //                //    else {
                                            //                //        Ext.getCmp("pnum").setValue(0);
                                            //                //        Ext.getCmp("act_pick_qty").setValue(0);
                                            //                //        return false;
                                            //                //    }
                                            //                //});
                                            //            }
                                            //            Ext.getCmp("act_pick_qty").setValue(0);
                                            //        }else {
                                            //            if (message.length > 0) {
                                            //                Ext.Msg.alert(INFORMATION, message);
                                            //            }
                                            //        }
                                            //    }
                                            //});
                                        }
                                        else {
                                            if (message.length > 0) {
                                                Ext.Msg.alert(INFORMATION, message);
                                            }
                                        }
                                    }else {
                                        if (message.length > 0)
                                        {
                                            Ext.Msg.alert(INFORMATION, message);
                                        }
                                    }
                                }
                            });
                        }
                        else {
                            e.record.set("pick_num", 0);
                            pickCount = 0;//重新獲取當前撿貨數量
                            for (var i = 0; i < gdStock.length; i++) {
                                if (gdStock[i].get("pick_num") !== 0 && !isNaN(gdStock[i].get("pick_num"))) {
                                    pickCount += gdStock[i].get("pick_num");
                                }
                            }
                            Ext.getCmp("act_pick_qty").setValue(pickCount);
                            Ext.Msg.alert(INFORMATION, "實際撿貨量不能大於撿貨數量!");
                        }
                        ////////////////////魚上方更換位置//////////////////////////////
                    }
                    //alert('edit:end');
                }
            }
        }
    });
    var addTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 900,
        url: '/WareHouse/GETMarkTallyWD',
        defaults: {
            labelWidth: 100,
            width: 250,
            margin: '5 0 0 10'
        },
        border: false,
        plain: true,
        id: 'AddUserPForm',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '工作代號',
                id: 'assg_id',
                name: 'assg_id'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '出貨單號',
                id: 'deliver_code',
                name: 'deliver_code'
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                width: 1000,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        fieldLabel: '訂單號',
                        id: 'ord_id',
                        width: 250,
                        name: 'ord_id'
                    },
                    {
                        xtype: 'displayfield',
                        fieldLabel: '備註',
                        id: 'note_order',
                        name: 'note_order',
                        hidden: true
                    }
                ]
            },
            {
                xtype: 'displayfield',
                fieldLabel: '撿貨料位',
                id: 'loc_id',
                name: 'loc_id'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Hash撿貨料位',
                id: 'hash_loc_id',
                name: 'hash_loc_id' ,
                hidden:true
            },
            {
                xtype: "textfield",
                id: 'upc_id',
                name: 'upc_id',
                fieldLabel: '料位條碼',
                submitValue: true,
                hidden: true,
                listeners: {
                    change: function () {
                        var loc_id = Ext.getCmp("loc_id").getValue();
                        var loc_id_Input = Ext.getCmp("upc_id").getValue().toUpperCase().trim();
                        var hash_loc_id = Ext.getCmp("hash_loc_id").getValue().toUpperCase();
                        if (loc_id == loc_id_Input || (hash_loc_id == loc_id_Input&&hash_loc_id.length>15)) {
                            Ext.getCmp("product_name").show();
                            Ext.getCmp("upc").show();
                            Ext.getCmp("upc").focus();
                            Ext.getCmp("upc_id").setValue(loc_id_Input);
                            Ext.getCmp("err").hide();
                        } else {
                            if (loc_id_Input.length == 8) {
                                Ext.getCmp("upc_id").setValue("");
                                Ext.getCmp("upc_id").focus();
                                Ext.getCmp("err").show();
                            }
                            Ext.getCmp("product_name").hide();
                            Ext.getCmp("upc").hide();
                            Ext.getCmp("out_qty").hide();
                            Ext.getCmp("act_pick_qty").hide();
                            Ext.getCmp("upc").setValue("");
                            Ext.getCmp("act_pick_qty").setValue("");//清空實際撿貨量
                            Ext.getCmp("gdStock").hide();
                            Ext.getCmp("deliver").hide();
                            Ext.getCmp("deliver").setValue("");//清空輸入用於驗證的定單編號
                            Ext.getCmp("presentation").hide();
                            Ext.getCmp("err2").hide();
                        }
                    }
                }
            },
            {
                id: 'err',
                border: false,
                html: '<div style="color:red;">提示：這個料位不存在，請重新掃描!</div>',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '品名',
                id: 'product_name',
                name: 'product_name',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '產品條碼/商品細項編號',
                id: 'upc',
                name: 'upc',
                hidden: true,
                listeners: {
                    change: function () {
                        //判斷該商品item對應的條碼
                        if (Ext.getCmp('upc').getValue().length > 5) {
                            Ext.Ajax.request({
                                url: "/WareHouse/Judgeupcid",
                                method: 'post',
                                type: 'text',
                                params: {
                                    item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),
                                    upc_id: Ext.getCmp("upc").getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        Ext.getCmp("err1").hide();
                                        Ext.getCmp("out_qty").show();
                                        Ext.getCmp("act_pick_qty").show();
                                        //載入庫存信息
                                        Ext.getCmp("gdStock").show();
                                        Ext.getCmp('gdStock').store.loadPage(1, {
                                            params: {
                                                item_id: document.getElementById('hid_item_id').value
                                            }
                                        });
                                        Ext.getCmp("deliver").show();
                                    } else {
                                        if (result.msg == 1) {
                                            Ext.getCmp("err1").setValue("<div style='color:red;'>這個條碼對應到" + result.itemid + "品號，請重新掃描正確商品!</div>");
                                        }
                                        else if (result.msg == 2) {
                                            Ext.getCmp("err1").setValue("<div style='color:red;'>這個條碼不存在，請重新掃描!</div>");
                                        }
                                        Ext.getCmp("err1").show();
                                        Ext.getCmp("upc").setValue("");//清空輸入用於驗證的定單編號
                                        Ext.getCmp("out_qty").hide();
                                        Ext.getCmp("act_pick_qty").hide();
                                        Ext.getCmp("gdStock").hide();
                                        Ext.getCmp("deliver").hide();
                                        Ext.getCmp("deliver").setValue("");//清空輸入用於驗證的定單編號
                                        Ext.getCmp("upc").focus();
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "條碼錯誤,請稍后再試,如有重複出錯請聯繫管理員!!");
                                    Ext.getCmp("upc").focus();
                                }
                            });
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: '提示',
                id: 'err1',
                name: 'err1',
                style: 'color:red;',
                width: 600,
                hidden: true
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                width: 600,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        fieldLabel: '撿貨數量',
                        id: 'out_qty',
                        name: 'out_qty',
                        hidden: true
                    },
                    {
                        xtype: 'textfield',
                        id: 'deliver',
                        fieldLabel: '出貨單號',
                        margin: '0 0 0 20',
                        allowBlank: false,
                        submitValue: true,
                        hidden: true,
                        listeners: {
                            change: function () {
                                Ext.getCmp("btn_sure").setDisabled(true);
                                var deliver = Ext.getCmp("deliver").getValue();
                                if (deliver == Ext.getCmp("deliver_code").getValue()) {
                                    Ext.getCmp("err2").hide();
                                    Ext.getCmp("btn_sure").setDisabled(false);
                                    Ext.getCmp("amsure").setDisabled(false);
                                    Ext.getCmp("amsure").show();
                                }
                                else {
                                    if (deliver.length == 9) {
                                        Ext.getCmp("err2").show();
                                        Ext.getCmp("err2").setValue("<div style='color:red;'>不是該出貨單的單號,請重新輸入!</div>");
                                        Ext.getCmp("deliver").setValue();
                                        Ext.getCmp("deliver").focus();
                                    }
                                    Ext.getCmp("amsure").hide();
                                }
                            }
                        }
                    }                   
                 ]
             },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                width: 800,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        fieldLabel: '實際撿貨量統計',
                        id: 'act_pick_qty',
                        name: 'act_pick_qty',
                        margin: '0 20 0 0',
                        hidden: true
                    },
                     {
                         xtype: 'displayfield',
                         id: 'err2',
                         name: 'err2',
                         width: 400,
                         style: 'color:red;',
                         hidden: true
                     },
                    {
                        xtype: 'button',
                        margin: '0 0 0 20',
                        text: "確認撿貨",
                        id: 'amsure',
                        hidden:true,
                        disabled:true,
                        handler: JhQuery
                    }
                   ]
               },
            {
                id: 'presentation',
                border: false,
                html: '<div style="color:red;">提示：料位有上鎖的庫存，請注意</div>',
                hidden: true
            },
            gdStock
        ],
        buttonAlign: 'left',
        buttons: [
            {
                id: 'btn_sure', 
                text: '確認撿貨',
                hidden:true,
                formBind: true,
                disabled: true,
                handler: function () { 
                }
            }
        ],
        listeners: {
            afterrender: function () {
                //獲取工作代號用於顯示并連帶出相關信息
                var assg_id = document.getElementById("hid_assg_id").value;
                Ext.getCmp("assg_id").setValue(assg_id);
                LoadWorkInfo(assg_id);
            }
        }
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        id: 'Mark',
        items: addTab,
        autoScroll: true
    });

});

//初始化頁面加載信息
function LoadWorkInfo(assg_id) {
 
    Ext.Ajax.request({
        url: '/WareHouse/JudgeAssg',
        method: 'post',
        params: {
            assg_id: assg_id
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                //請求成功,控制下一步的操作
                if (result.data.length > 0) {
                    document.getElementById("hid_item_id").value = result.data[0].item_id;
                    document.getElementById("hid_upc_id").value = result.data[0].upc_id;
                    document.getElementById("hid_ordd_id").value = result.data[0].ordd_id;
                    document.getElementById("hid_seld_id").value = result.data[0].seld_id;
                    document.getElementById("hid_ord_qty").value = result.data[0].ord_qty;
                    Ext.getCmp("ord_id").setValue(result.data[0].ord_id);
                    Ext.getCmp("deliver_code").setValue(result.data[0].deliver_code);
                    Ext.getCmp("hash_loc_id").setValue(result.data[0].hash_loc_id);
                    deliver_id = result.data[0].deliver_id;
                    Ext.getCmp("loc_id").setValue(result.data[0].sel_loc);
                    var prod = "";
                    if (result.data[0].prod_sz != "")
                    {
                        prod = "(" + result.data[0].prod_sz + ")";
                    }
                    Ext.getCmp("product_name").setValue(result.data[0].description + prod);
                    Ext.getCmp("out_qty").setValue(result.data[0].out_qty);
                    Ext.getCmp("upc_id").show();
                    Ext.getCmp("amsure").hide();
                    Ext.getCmp('upc_id').focus();
                    //20150504 add訂單備註信息
                    if (result.data[0].note_order.length > 0) {
                        Ext.getCmp("note_order").setValue(result.data[0].note_order);
                        Ext.getCmp("note_order").show();
                    }
                    else {
                        Ext.getCmp("note_order").hide();
                    }
                    //Ext.getCmp("act_pick_qty").setMaxValue(result.data[0].out_qty);//實際撿貨數量不能大於缺貨量(實際是定貨數量[因數據插入時定貨量與缺貨量相同,撿貨時撿幾個就扣幾個缺貨量故缺貨量就是每次需要撿貨的數量])
                } else {
                    //document.location.href = "/WareHouse/MarkTally";    //商品撿完后轉到輸工作代號與定單號頁面
                }
            } else {
                //Ext.Msg.alert("提示","您輸入的料位條碼不符,請查找對應的料位!");
            }
        },
        failure: function () {
            //Ext.Msg.alert("提示","您輸入的料位條碼不符,請查找對應的料位!");
        }
    });
}
//庫存新增
onAddClick = function () {
    var loc = Ext.getCmp('loc_id').getValue();
    if (loc != 'YY999999' && loc.length == 8) {
        addFunction(null, StockStore);
    }
    else {
        Ext.Msg.alert(INFORMATION, "該商品無主料位,不能庫調!");
        //Ext.getCmp("pick_num").setValue(0);
        Ext.getCmp("act_pick_qty").setValue(0);
    }
    
}
//新增庫存頁面
function addFunction(row, store) {
    var cde_dt_shp;
    var pwy_dte_ctl;
    var cde_dt_var;
    var cde_dt_incr;
    var vendor_id;
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 120,
        url: '/WareHouse/InsertIinvd',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'numberfield',
                fieldLabel: "數量",
                name: 'num',
                id: 'num',
                minValue: 0,
                allowBlank: false
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "製造日期",
                combineErrors: true,
                height: 24,
                margins: '0 200 0 0',
                layout: 'hbox',
                id: 'createtime',
                //hidden: true,
                defaults: {
                    flex: 1,
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'us',
                        inputValue: "1",
                        id: "us1",
                        checked: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var i = Ext.getCmp('startTime');//製造日期
                                var j = Ext.getCmp('cde_dt');
                                if (newValue) {
                                    j.allowBlank = true;
                                    i.setDisabled(false);
                                    j.setDisabled(true);
                                    j.setValue(null);
                                    i.allowBlank = false;
                                }
                            }
                        }
                    },
                    {
                        xtype: "datefield",
                        fieldLabel: "製造日期",
                        id: 'startTime',
                        name: 'startTime',
                        submitValue: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                if (Ext.getCmp('startTime').getValue() != "" && Ext.getCmp('startTime').getValue() != null) {
                                    Ext.Ajax.request({
                                        url: "/WareHouse/JudgeDate",
                                        method: 'post',
                                        type: 'text',
                                        params: {
                                            dtstring: 1,
                                            item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),
                                            startTime: Ext.Date.format(Ext.getCmp('startTime').getValue(), 'Y-m-d')
                                        },
                                        success: function (form, action) {
                                            var result = Ext.decode(form.responseText);
                                            if (result.success) {
                                                if (result.msg == "1") {
                                                    Ext.Msg.alert(INFORMATION, "該商品製造日期大於今天");
                                                    Ext.getCmp('startTime').setValue(null);
                                                } else if (result.msg == "3") {
                                                    editFunction("超過允出天數!");
                                                    return;
                                                } else if (result.msg == "4") {
                                                    editFunction("有效期為" + result.dte + "的商品已超過有效日期!");
                                                    return;
                                                }
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "有效日期",
                combineErrors: true,
                //hidden: true,
                layout: 'hbox',
                height: 24,
                //margins: '0 200 0 0',
                id: 'cdttime',
                defaults: {
                    flex: 1,
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'us',
                        inputValue: "2",
                        id: "us2",
                        //disabled: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var i = Ext.getCmp('startTime');//製造日期
                                var j = Ext.getCmp('cde_dt');
                                if (newValue) {
                                    i.setDisabled(true);
                                    j.setDisabled(false);
                                    i.allowBlank = true;
                                    i.setValue(null);
                                    j.allowBlank = false;
                                }
                            }
                        }
                    },
                    {
                        xtype: "datefield",
                        fieldLabel: "有效日期",
                        id: 'cde_dt',
                        name: 'cde_dt',
                        disabled: true,
                        submitValue: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                if (Ext.getCmp('cde_dt').getValue() != "" && Ext.getCmp('cde_dt').getValue() != null) {
                                    Ext.Ajax.request({
                                        url: "/WareHouse/JudgeDate",
                                        method: 'post',
                                        type: 'text',
                                        params: {
                                            dtstring: 2,
                                            item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),
                                            startTime: Ext.Date.format(Ext.getCmp('cde_dt').getValue(), 'Y-m-d')
                                        },
                                        success: function (form, action) {
                                            var result = Ext.decode(form.responseText);
                                            if (result.success) {
                                                if (result.msg == "1") {
                                                    Ext.Msg.alert(INFORMATION, "該商品製造日期大於今天");
                                                    Ext.getCmp('startTime').setValue(null);
                                                } else if (result.msg == "3") {
                                                    editFunction("超過允出天數!");
                                                    return;
                                                } else if (result.msg == "4") {
                                                    editFunction("有效期為" + result.dte + "的商品已超過有效日期!");
                                                    return;
                                                }
                                            }
                                        },
                                        failure: function (form, action) {
                                            Ext.Msg.alert(INFORMATION, "保存失敗,請稍后再試,如有重複出錯請聯繫管理員!");
                                        }
                                    });
                                }
                            }
                        }
                    }
                ]
            }
        ]
        ,
        buttons: [
            {
            text: SAVE,
            formBind: true,
            handler: function () {
                var form = this.up('form').getForm();
                    Ext.Ajax.request({
                        url: "/WareHouse/Getprodbyid",
                        method: 'post',
                        async: false,
                        type: 'text',
                        params: {
                            id: Ext.htmlEncode(document.getElementById("hid_item_id").value)
                        },
                        success: function (form, action) {                                
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                cde_dt_shp = result.cde_dt_shp;
                                pwy_dte_ctl = result.pwy_dte_ctl;
                                cde_dt_var = result.cde_dt_var;
                                cde_dt_incr = result.cde_dt_incr;
                                vendor_id = result.vendor_id;
                            }                            
                        }
                    });
                if (form.isValid()) {
                    form.submit({
                        params: {
                            //id: Ext.htmlEncode(Ext.getCmp('plas_prdd_id').getValue()),//條碼
                            iialg: 'Y',
                            item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),//商品品號
                            product_name: Ext.htmlEncode(Ext.getCmp('product_name').getValue()),//品名
                            prod_qty: Ext.htmlEncode(Ext.getCmp('num').getValue()),//數量
                            startTime: Ext.htmlEncode(Ext.getCmp('startTime').getValue()),//創建時間
                            cde_dt: Ext.htmlEncode(Ext.getCmp('cde_dt').getValue()),//有效時間
                            plas_loc_id: Ext.htmlEncode(Ext.getCmp('loc_id').getValue()),//上架料位
                            loc_id: Ext.htmlEncode(Ext.getCmp('loc_id').getValue()),//主料位
                            iarc_id:'PC',
                            cde_dt_var: cde_dt_var,
                            cde_dt_incr: cde_dt_incr,
                            vendor_id: vendor_id
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                StockStore.load();//
                                editWin.close();
                            } else {
                                Ext.MessageBox.alert(ERRORSHOW + result.success);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "系統異常,請稍后再試,如有重複出錯請聯繫管理員!");
                        }
                    });
                }                
            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: "庫存調整",
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 550,
        height: 380,
        autoScroll: true,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
        ,
        listeners: {
            'show': function () {
                if (row == null) {
                    // editFrm.getForm().loadRecord(row); //如果是添加的話
                    editFrm.getForm().reset();
                } else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的

                }
            }
        }
    });
    editWin.show();
}

function JhQuery()
{
    Ext.getCmp("amsure").setDisabled(true);
    var pickNum = Ext.getCmp("out_qty").getValue();
    if (parseInt(pickCount) > parseInt(pickNum)) {
        Ext.Msg.alert(INFORMATION, "實際撿貨量不能大於撿貨數量!");
        return;
    }
    if (Ext.getCmp("deliver").getValue() != Ext.getCmp("deliver_code").getValue()) {
        Ext.Msg.alert(INFORMATION, "出貨單號錯誤!");
        return;
    }
    var form = this.up('form').getForm();
    if (form.isValid()) {
        form.submit({
            params: {
                commodity_type: 2,//寄倉調度的編號
                seld_id: Ext.htmlEncode(document.getElementById("hid_seld_id").value),//aseld的流水號
                out_qty: Ext.htmlEncode(Ext.getCmp('out_qty').getValue()),//缺貨數量
                ord_qty: Ext.htmlEncode(document.getElementById("hid_ord_qty").value),//訂貨數量
                act_pick_qty: Ext.htmlEncode(Ext.getCmp('act_pick_qty').getValue()),//實際揀貨量
                item_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),//商品細項編號
                ordd_id: Ext.htmlEncode(document.getElementById("hid_ordd_id").value),
                assg_id: Ext.htmlEncode(Ext.getCmp('assg_id').getValue()),//工作項目編號
                ord_id: Ext.htmlEncode(Ext.getCmp('ord_id').getValue()),
                deliver_id: Ext.htmlEncode(Ext.getCmp('deliver').getValue()),
                pickRowId: arrPickRowId,
                pickInfo: arrPickInfo
            },
            success: function (form, action) {
                var result = Ext.decode(action.response.responseText);
                if (result.success) {
                    arrPickRowId = new Array();
                    arrPickInfo = new Array();
                    pickCount = 0;
                    pickNum = 0;
                    var msg = "";
                    var fa = 0;
                    if (result.qty > 0) {
                        msg = "該商品缺貨! ";
                    }
                    if (result.ord == 0) {
                        if (result.can == 0) {
                            msg += "該訂單可以封箱! ";
                        } else {
                            if (result.flag == 0) {
                                fa = 1;
                                editFunction("請移交主管處理");
                            }
                            else {
                                editFunction("請移交主管處理!");
                                //LoadWorkInfo(Ext.getCmp("assg_id").getValue());
                                Ext.getCmp("upc_id").setValue("");  //清空料位條碼繼續進行撿貨
                            }
                        }
                    }
                    if (result.flag == 0&& fa==0) {
                        //不繼續往下循環
                        Ext.Msg.alert("提示", "該項目已經走完!");
                        setTimeout('closedthis()', 3000);
                       
                        //Ext.MessageBox.confirm("提示", msg + "該項目已經走完，是否繼續？", function (btn) {
                        //    if (btn == "yes") {
                        //        document.location.href = "/WareHouse/MarkTally";
                        //    }
                        //    else {
                               
                        //        document.location.href = "/WareHouse/MarkTally";
                                
                                
                        //    }
                        //});
                    } else if( fa == 0){//續下一個商品。                                        
                        if (msg !== "") {
                            Ext.MessageBox.confirm("提示", msg, function (btn) {
                                if (btn == "yes") {
                                    return true;
                                }
                                else {
                                    return false;
                                }
                            })
                        }
                        LoadWorkInfo(Ext.getCmp("assg_id").getValue());
                        Ext.getCmp("upc_id").setValue("");  //清空料位條碼繼續進行撿貨
                    }
                } else {
                    Ext.Msg.alert(INFORMATION, "程序運行出錯,請聯繫開發人員!");
                }
            },
            failure: function (form, action) {
                Ext.Msg.alert(INFORMATION, "系統異常,請稍后再試,如有重複出錯請聯繫管理員!");
            }
        });
    }
}
function closedthis() {
    document.location.href = "/WareHouse/MarkTally";
}
var config = {
    title: "自定义对话框",
    msg: "这是一个自定义对话框",
    width: 400,
    multiline: true,
    closable: false,
    buttons: Ext.MessageBox.YESNOCANCEL,
    icon: Ext.MessageBox.QUESTION,
    fn: function (btn, txt) {
        Ext.MessageBox.alert("Result", "你点击了'yes'按钮<br>,输入的值是：" + txt);

    }
}