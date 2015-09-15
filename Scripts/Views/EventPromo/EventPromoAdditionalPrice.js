var currentRecord = { data: {} };
var pageSize = 19;
Ext.Loader.setConfig({ enabled: true });
Ext.require([
    'Ext.data.*',
    'Ext.util.*',
    'Ext.view.View',
    'Ext.ux.DataView.DragSelector',
    'Ext.ux.DataView.LabelEditor'
]);
//促銷加價購列表
Ext.define('GIGADE.PROMOPRICE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'row_id', type: 'int' },
        { name: 'event_id', type: 'string' },
        { name: 'group_id', type: 'string' },
        { name: 'event_name', type: 'string' },
        { name: 'event_desc', type: 'string' },
        { name: 'site_id', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'user_condition_id', type: 'int' },
        { name: 'condition_type', type: 'int' },
        { name: 'device', type: 'int' },
        { name: 'event_start', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'event_end', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'event_status', type: 'int' },
        { name: 'modify_user', type: 'int' },
        { name: 'user_username', type: 'string' },
        { name: 'quantity', type: 'int' },
        { name: 'amount', type: 'int' },
        { name: 'num_limit', type: 'string' }

    ]
});

var eventPromoAdditionalPriceStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PROMOPRICE',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetEventPromoAdditionalPrice',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'
        }
    }
});

//促銷商品細項
Ext.define('GIGADE.PROMOPRODUCTITEM', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'row_id', type: 'int' },
        { name: 'group_id', type: 'int' },
        { name: 'product_id', type: 'int' },
        { name: 'create_user', type: 'int' },
        { name: 'create_time', type: 'string'},
        { name: 'add_price', type: 'int' },
        { name: 'modify_user', type: 'int' },
        { name: 'modify_time', type: 'string'},
        { name: 'product_status', type: 'int' },
        { name: 'product_name', type: 'string' },
        { name: 'group_name', type: 'string' }
    ]
});

var eventPromoProductItemStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PROMOPRODUCTITEM',
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetEventPromoAdditionalPriceProduct',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});
//促銷商品群組
Ext.define('GIGADE.PROMOPRODUCTGROUP', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'group_id', type: 'int' },
        { name: 'group_name', type: 'string' },
        { name: 'user_username', type: 'string' },
        { name: 'create_date', type: 'string'},
        { name: 'modify_user', type: 'int' },
        { name: 'modify_time', type: 'string'},
        { name: 'group_status', type: 'int' },
    ]
});

var productGroupCboStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PROMOPRODUCTGROUP',
    autoLoad: true,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetProductGroupCbo',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var productGroupStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PROMOPRODUCTGROUP',
    autoLoad: true,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetProductGroupCbo',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
eventPromoAdditionalPriceStore.on("beforeload", function () {
    Ext.apply(eventPromoAdditionalPriceStore.proxy.extraParams, {
        event_id: Ext.getCmp("event_id_left") ? Ext.getCmp("event_id_left").getValue() : '',
        event_name: Ext.getCmp('event_name_left') ? Ext.getCmp('event_name_left').getValue() : ''
    })
})

function Search() {
    Ext.getCmp('event_id_left').setValue(Ext.getCmp('event_id_left').getValue().replace(/\s+/g, ","));
    if (!Ext.getCmp('event_id_left').isValid()) return;
    eventPromoAdditionalPriceStore.removeAll();
    eventPromoAdditionalPriceStore.loadPage(1);
}
//左邊活動列表
var eventPromoAdditionalPrice = Ext.create('Ext.grid.Panel', {
    id: ' eventPromoAdditionalPrice',
    autoScroll: true,
    layout: 'anchor',
    height: document.documentElement.clientHeight - 12,
    border: false,
    frame: false,
    store: eventPromoAdditionalPriceStore,
    dockedItems: [{
        id: 'dockedItem',
        xtype: 'toolbar',
        layout: 'column',
        dock: 'top',
        items: [{
            xtype: 'textfield',
            fieldLabel: '活動編號',
            id: 'event_id_left',
            name: 'event_id_left',
            margin: '3 0 0 0',
            width: 210,
            labelWidth: 60,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Search();
                    }
                }
            }
        }, {
            xtype: 'textfield',
            fieldLabel: '活動名稱',
            id: 'event_name_left',
            name: 'event_name_left',
            width: 210,
            labelWidth: 60,
            margin: '3 0 5 0',
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Search();
                    }
                }
            }
        }, {
            xtype: 'button',
            text: '查詢',
            id: 'grid_btn_search',
            iconCls: 'ui-icon ui-icon-search',
            margin: ' 0 0 5 10',
            width: 65,
            handler: Search
        }, {
            xtype: 'button',
            text: '新增活動',
            id: 'grid_btn_add',
            iconCls: 'ui-icon ui-icon-add',
            margin: ' 0 0 5 10',
            width: 75,
            handler: function () {
                var record = { data: { 'row_id': 0 } };
                currentRecord = record;
                Ext.getCmp('west-region-container').setDisabled(true);
                Ext.getCmp('center').getForm().reset();
                eventPromoProductItemStore.removeAll();
                ac_store.removeAll();
            }
        }]
    }],
    columns: [
        { header: '活動編號', dataIndex: 'event_id', align: 'left', width: 60, menuDisabled: true, sortable: false },
        { header: '活動名稱', dataIndex: 'event_name', align: 'left', menuDisabled: true, sortable: false, flex: 1 },
        { header: '開始時間', dataIndex: 'event_start', align: 'center', width: 70, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d <br> H:i:s') },
        { header: '結束時間', dataIndex: 'event_end', align: 'center', width: 70, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d <br> H:i:s') },
         { header: '異動者', dataIndex: 'user_username', align: 'left', width: 40, menuDisabled: true, sortable: false },
        {
            header: '狀態', dataIndex: 'event_status', align: 'center', width: 40, menuDisabled: true, sortable: false,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "<img hidValue='0' id='img" + record.data.row_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                } else if (value == 0) {
                    return "<img hidValue='1' id='img" + record.data.row_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                }
            }
        }
    ],

    bbar: Ext.create('Ext.PagingToolbar', {
        store: eventPromoAdditionalPriceStore,
        dock: 'bottom',
        pageSize: pageSize,
        displayInfo: true
    }),
    listeners: {
        itemclick: function (view, record, item, index, e) {
            if (e.getTarget('.x-grid-cell').cellIndex != 5) {
                LoadDetail(currentRecord = record);
            }
            else {
                Ext.MessageBox.confirm("提示", Ext.String.format("是否要{0}該活動?", record.data.event_status == 1 ? "禁用" : "啟用"), function (btn) {
                    if (btn == "yes") {
                        UpdateActive(record);
                    }
                    else {
                        return false;
                    }
                });
            }

        },
        resize: function () {
            this.doLayout();
        }
    }
})
//更改活動狀態注：活動的最近修改人不可啟用該活動
function UpdateActive(row) {
    var activeValue = row.data.event_status == 0 ? 1 : 0;
    $.ajax({
        url: "/EventPromo/UpdateActiveAdditionalPrice",
        data: {
            "event_id": row.data.event_id,
            "mo_user": row.data.modify_user,
            "type": row.data.condition_type,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "stop") {
                Ext.Msg.alert(INFORMATION, "管理員不能啟用自己禁用的數據！");
            }
            else if (msg.success == "true") {
                eventPromoAdditionalPriceStore.load();
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}



function LoadDetail(record) {
    if (record.data.event_id == undefined || record.data.event_id == 0) {
        Ext.getCmp('center').getForm().reset();
       eventPromoProductItemStore.removeAll();
        ac_store.removeAll();
    } else {
        center.getForm().loadRecord(record);
        switch (record.data.device) {
            case 0:
                Ext.getCmp("bufen").setValue(true);
                break;
            case 1:
                Ext.getCmp("pc").setValue(true);
                break;
            case 4:
                Ext.getCmp("mobilepad").setValue(true);
                break;
        }

        SiteStore.load({
            callback: function () {
                var siteIDs = record.data.site_id.split(',');
                var arrTemp = new Array();
                for (var i = 0; i < siteIDs.length; i++) {
                    arrTemp.push(SiteStore.getAt(SiteStore.find("Site_Id", siteIDs[i])));
                }
                Ext.getCmp('site_id').setValue(arrTemp);
            }
        });
        //根據促銷設定分別抓取設定的值
        var condiStr = "";
        Ext.Ajax.request({
            url: '/EventPromo/GetCondiType',
            method: 'post',
            async: false, //true為異步，false為同步
            params: {
                event_id: record.data.event_id,
                condiType: record.data.condition_type
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    condiStr = result.conStr;
                }
            }
        });


        switch (record.data.condition_type) {
            case 1:
                BrandStore.load({
                    params: {
                        isShowGrade: 1
                    },
                    callback: function () {
                        var brandIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < brandIDs.length; i++) {
                            arrTemp.push(BrandStore.getAt(BrandStore.find("brand_id", brandIDs[i])));
                        }
                        Ext.getCmp('p_brand').setValue(arrTemp);
                    }
                })
                break;
            case 2://設定類別
                var treeIds = condiStr.split(',');

                var category_ids = "";
                var category_names = "";
                for (var i = 0; i < treeIds.length; i++) {
                    var cats = treeIds[i].split('&');
                    category_ids += cats[0] + ",";
                    category_names += cats[1] + ",";
                }
                if (category_ids != "") {
                    category_ids = category_ids.substring(0, category_ids.lastIndexOf(','));
                }
                if (category_names != "") {
                    category_names = category_names.substring(0, category_names.lastIndexOf(','));
                }
                Ext.getCmp('category_name').setValue(category_names);
                Ext.getCmp('category_id').setValue(category_ids);
                Ext.getCmp('center').doLayout();
                break;
            case 3:
                ShopClassStore.load({
                    callback: function () {
                        var classIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < classIDs.length; i++) {
                            arrTemp.push(ShopClassStore.getAt(ShopClassStore.find("class_id", classIDs[i])));
                        }
                        Ext.getCmp('p_class').setValue(arrTemp);
                    }
                })
                break;
            case 4:
                ac_store.removeAll();
                var ac_storeIDs = condiStr.split(',');//獲取到的是已11111&3,....類似這樣的形式
                var resultproductid = "";
                for (var i = 0; i < ac_storeIDs.length; i++) {
                    var product_names_two = "";
                    var results = ac_storeIDs[i].split('&');
                    var t_product_id = results[0];
                    var t_product_num = results[1];
                    resultproductid = resultproductid + t_product_id + ',';//product_id相互連接
                    product_names_two = results[2];
                    ac_store.add({
                        product_id: t_product_id,
                        product_name: product_names_two,
                        product_num: t_product_num
                    });
                }
                if (resultproductid.indexOf(',') >= 0) {
                    Ext.getCmp('p_product_id').setValue(resultproductid.substring(0, resultproductid.lastIndexOf(',')));
                }
                Ext.getCmp('p_product_name').setValue(condiStr);
                break;
            case 5:
                ShopCartStore.load({
                    callback: function () {
                        var cartIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < cartIDs.length; i++) {
                            arrTemp.push(ShopCartStore.getAt(ShopCartStore.find("cart_id", cartIDs[i])));
                        }
                        Ext.getCmp('p_cart').setValue(arrTemp);
                    }
                })
                break;
            case 6:
                PaymentStore.load({
                    callback: function () {
                        var payIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < payIDs.length; i++) {
                            arrTemp.push(PaymentStore.getAt(PaymentStore.find("parameterCode", payIDs[i])));
                        }
                        Ext.getCmp('p_payment').setValue(arrTemp);
                    }
                })
                break;
        }
        eventPromoProductItemStore.load({
            params: {
                group_id: record.data.group_id
            }
        });
    }

}
//複選框列
var product_cm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("productItemGrid").down('#t_edit_product').setDisabled(selections.length == 0);
            Ext.getCmp("productItemGrid").down('#t_del_product').setDisabled(selections.length == 0);
        }
    }
});
var productGroupGrid_sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("group_edit").setDisabled(selections.length == 0);
            Ext.getCmp("group_remove").setDisabled(selections.length == 0);
        }
    }
});

function editProducts() {

    var sms = Ext.getCmp("productItemGrid").getSelectionModel().getSelection();
    if (sms.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (sms.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (sms.length == 1) {
        addProducts(sms[0],eventPromoProductItemStore);
    }

}
function delProducts() {
    var sms = Ext.getCmp("productItemGrid").getSelectionModel().getSelection();
    Ext.MessageBox.confirm("提示", Ext.String.format("確定要移除選中{0}條數據?",sms.length), function (btn) {
        if (btn == "yes") {
            for (var i = 0; i < sms.length; i++) {
                eventPromoProductItemStore.remove(sms[i]);
            }
        }
        else {
            return false;
        }
    });
}
//根據商品編號，判斷是否存在該商品
function checkProduct()
{
    var pid = Ext.getCmp("product_id");
    var pname = Ext.getCmp("product_name");
    if (pid.getValue() != "" && pid.getValue() != null) {
        Ext.Ajax.request({
            url: '/EventPromo/GetProdName',
            params: {
                product_id: Ext.htmlEncode(pid.getValue())
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    pname.setValue(result.prod_name);
                }
                else {
                    Ext.Msg.alert(INFORMATION, "獲取商品名稱失敗！");
                    pname.setValue("");
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, "獲取商品名稱失敗！");
                pname.setValue("");
            }
        });
    }
    else {
        pname.setValue("");
    }
}
function addProducts(row, store) {
    //判斷是否選擇群組
    if (Ext.getCmp("group_id").getValue() == null) {
        Ext.Msg.alert("提示", '請選擇商品群組');
        return false;
    }
    var detailAddFrm = Ext.create('Ext.form.Panel', {
        id: 'detailAddFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        defaults: { msgTarget: "side", labelWidth: 80 },
        items: [
             {
                 xtype: 'numberfield',
                 fieldLabel: '商品編號',
                 id: 'product_id',
                 name: 'product_id',
                 minValue: 10000,
                 allowDecimals: false,
                 allowBlank: true,
                 width: 310,
                 hideTrigger:true,
                 listeners: {
                     'blur': function () {
                         checkProduct();
                     },
                     change: function () {
                         Ext.getCmp("product_name").setValue("");
                     }
                 }
             },
                    {
                        xtype: 'displayfield',
                        fieldLabel: '商品名稱',
                        id: 'product_name',
                        name: 'product_name',
                        allowBlank: true,
                        width: 310
                    },
            {
                xtype: 'numberfield',
                fieldLabel: '加購價格',
                id: 'add_price',
                name: 'add_price',
                minValue: 0,
                value: 0,
                maxValue: 2147483647,
                allowBlank: false,
                allowDecimals: false,
                width: 310
            }
        ],
        buttons: [{
            text: '重置',
            handler: function () {
                if (row) {
                    this.up('form').getForm().loadRecord(row);
                }
                else {
                    this.up('form').getForm().reset();
                }
            }
        },
        {
            text: '確定',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    if (Ext.getCmp("product_name").getValue() == "" || Ext.getCmp("product_name").getValue().indexOf("不可選擇組合商品") > -1) {
                        Ext.Msg.alert(INFORMATION, "請輸入有效的商品編號！");
                        return false;
                    }
                    //驗證商品是否通過審核
                    if (row) {
                        row.set('product_name', Ext.getCmp("product_name").getValue());
                        row.set('group_name', Ext.getCmp("group_id").getRawValue());
                        row.set('product_id', Ext.getCmp("product_id").getValue());
                        row.set('group_id', Ext.getCmp("group_id").getValue());
                        row.set('add_price', Ext.getCmp("add_price").getValue());
                    }
                    else {
                        store.add({
                            product_name:Ext.getCmp("product_name").getValue(),
                            group_name: Ext.getCmp("group_id").getRawValue(),
                            product_id: Ext.getCmp("product_id").getValue(),
                            group_id: Ext.getCmp("group_id").getValue(),
                            add_price: Ext.getCmp("add_price").getValue()
                        });
                    }
                    detailAddWin.destroy();
                }
            }
        }]
    })

    var detailAddWin = Ext.create('Ext.window.Window', {
        title: '商品詳情',
        width: 400,
        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [detailAddFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        constrain:true,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                if (row) {
                    detailAddFrm.getForm().loadRecord(row);

                }
            }
        }
    })
    detailAddWin.show();
}
function editGroup()
{
    var sms = Ext.getCmp("productGroupGrid").getSelectionModel().getSelection();
    if (sms.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (sms.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (sms.length == 1) {
        addGroup(sms[0]);
    }
}
function delGroup()
{
    var sms = Ext.getCmp("productGroupGrid").getSelectionModel().getSelection();
    Ext.MessageBox.confirm("提示", Ext.String.format("確定要移除選中{0}條數據?", sms.length), function (btn) {
        if (btn == "yes") {
            var delGroupIDs="";
            for (var i = 0; i < sms.length; i++) {
                delGroupIDs +=sms[i].data.group_id+ ",";
            }
            Ext.Ajax.request({
                url: '/EventPromo/DelEventPromoAdditionalPriceGroup',
                params: {
                    group_ids:delGroupIDs
                },
                timeout: 360000,
                success: function (response) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        productGroupCboStore.removeAll();
                        productGroupCboStore.load();
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }

                },
                failure: function (response, opts) {
                    if (response.timedout) {
                        Ext.Msg.alert(INFORMATION, TIME_OUT);
                    }
                }
            });
        }
        else {
            return false;
        }
    });
}
//群組添加
function addGroup(row)
{
    var groupAddPanl = Ext.create('Ext.form.Panel', {
        id: 'groupAddPanl',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        defaults: { msgTarget: "side", labelWidth: 80 },
        items: [
             {
                 xtype: 'textfield',
                 fieldLabel: '群組名稱',
                 id: 'group_name',
                 name: 'group_name',
                 minValue: 10000,
                 allowDecimals: false,
                 allowBlank: false,
                 margin:'10 0',
                 width: 310,
                 listeners: {
                     blur: function () {
                         var group_name = Ext.getCmp("group_name");
                         if (group_name.getValue() != "" && group_name.getValue() != null) {
                             Ext.Ajax.request({
                                 url: '/EventPromo/GetGroupName',
                                 params: {
                                     'group_name':group_name.getValue()
                                 },
                                 success: function (form, action) {
                                     var result = Ext.decode(form.responseText);
                                     if (result.success) {
                                         Ext.Msg.alert(INFORMATION, "已存在該群組名稱！");
                                         group_name.setValue("");
                                     }
                                 },
                                 failure: function () {
                                     Ext.Msg.alert(INFORMATION, "獲取群組名稱失敗！");
                                     pname.setValue("");
                                 }
                             });
                         }
                         else {
                             group_name.setValue("");
                         }

                     },
                     specialkey: function (field, e) {
                         if (e.getKey() == e.ENTER) {
                             var form = this.up('form').getForm();
                             if (form.isValid()) {
                                 Ext.Ajax.request({
                                     url: '/EventPromo/SaveEventPromoAdditionalPriceGroup',
                                     params: {
                                         group_name: Ext.getCmp("group_name").getValue(),
                                         group_id: row == null ? 0 : row.data.group_id
                                     },
                                     timeout: 360000,
                                     success: function (response) {
                                         var res = Ext.decode(response.responseText);
                                         if (res.success) {
                                             Ext.Msg.alert(INFORMATION, SUCCESS);
                                             productGroupCboStore.removeAll();
                                             productGroupCboStore.load();
                                             groupAddWin.destroy();
                                         }
                                         else {
                                             Ext.Msg.alert(INFORMATION, FAILURE);
                                         }

                                     },
                                     failure: function (response, opts) {
                                         if (response.timedout) {
                                             Ext.Msg.alert(INFORMATION, TIME_OUT);
                                         }
                                     }
                                 });
                             }
                         }
                     }
                 }
             } 
        ],
        buttons: [{
            text: '重置',
            handler: function () {
                if (row) {
                    this.up('form').getForm().loadRecord(row);
                }
                else {
                    this.up('form').getForm().reset();
                }
            }
        },
        {
            text: '確定',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    Ext.Ajax.request({
                        url: '/EventPromo/SaveEventPromoAdditionalPriceGroup',
                        params: {
                            group_name: Ext.getCmp("group_name").getValue(),
                            group_id:row==null?0:row.data.group_id
                        },
                        timeout: 360000,
                        success: function (response) {
                            var res = Ext.decode(response.responseText);
                            if (res.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                productGroupCboStore.removeAll();
                                productGroupCboStore.load();
                                groupAddWin.destroy();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }

                        },
                        failure: function (response, opts) {
                            if (response.timedout) {
                                Ext.Msg.alert(INFORMATION, TIME_OUT);
                            }
                        }
                    });
                }
            }
        }]
    })
    var groupAddWin = Ext.create('Ext.window.Window', {
        title: '群組添加',
        width: 400,
        height: document.documentElement.clientHeight * 260 / 783-100,
        layout: 'fit',
        items: [groupAddPanl],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                if (row) {
                    groupAddPanl.getForm().loadRecord(row);
                }
            }
        }
    });
    groupAddWin.show();
}

//var productItemGrid = new Ext.grid.Panel({
//    id: 'productItemGrid',
//    store: eventPromoProductItemStore,
//    autoScroll: true,
//    border: 0,
//    frame: false,
//    columns: [
//        { header: "商品編號", dataIndex: 'product_id', width: 100, menuDisabled: true, sortable: false },
//        { header: "商品名稱", dataIndex: 'product_name', width: 150, menuDisabled: true, sortable: false, flex: 1 },
//        { header: "群組編號", dataIndex: 'group_id', width: 100, menuDisabled: true, sortable: false },
//        { header: "群組名稱", dataIndex: 'group_name', width: 100, menuDisabled: true, sortable: false },
//        { header: '加購價', dataIndex: 'add_price', width: 100, menuDisabled: true, sortable: false }
//    ],
//    tbar: [
//        { xtype: 'button', id: 'item_add', text: "新增", iconCls: 'icon-add', handler: function () { addProducts(null, eventPromoProductItemStore); } },
        
//        { xtype: 'button', id: 't_edit_product', text: "編輯", iconCls: 'icon-edit',handler:function(){editProducts();} ,disabled: true },

//        { xtype: 'button', id: 't_del_product', text: "移除", iconCls: 'icon-remove',handler:function(){delProducts();}, disabled: true }

//    ],
//    viewConfig: { emptyText: '<span>暫無數據！</span>' },
//    selModel: product_cm,
//    listeners: {
//        scrollershow: function (scroller)
//        {
//            if (scroller && scroller.scrollEl)
//            {
//                scroller.clearManagedListeners();
//                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
//            }
//        }
//    }

//});
var productGroupGrid = new Ext.grid.Panel({
    id: 'productGroupGrid',
    store: productGroupCboStore,
    autoScroll: true,
    border: 0,
    viewConfig: {
        enableTextSelection: true,
        stripeRows: false,
        getRowClass: function (record, rowIndex, rowParams, store) {
            return "x-selectable";
        }
    },
    columns: [
        { header: "群組編號", dataIndex: 'group_id', width: 100, menuDisabled: true, sortable: false},
        { header: "群組名稱", dataIndex: 'group_name', width: 100, menuDisabled: true, sortable: false, flex: 1 },
        { header: "創建人", dataIndex: 'user_username', width: 100, menuDisabled: true, sortable: false },
        { header: "創建時間", dataIndex: 'create_date', width: 150, menuDisabled: true, sortable: false},
    ],
    tbar: [
        { xtype: 'button', id: 'group_add_panel', text: "新增", iconCls: 'icon-add', handler: function () { addGroup(null); } },
        
        { xtype: 'button', id: 'group_edit', text: "編輯", iconCls: 'icon-edit', disabled: true, handler: function () { editGroup(); } },

        { xtype: 'button', id: 'group_remove', text: "移除", iconCls: 'icon-remove', disabled: true, handler: function () { delGroup(); } }

    ],
    viewConfig: { emptyText: '<span>暫無數據！</span>' },
    selModel: productGroupGrid_sm,
    listeners: {
        scrollershow: function (scroller) {
            if (scroller && scroller.scrollEl) {
                scroller.clearManagedListeners();
                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
            }
        }
    }

});

var groupAddWin = Ext.create('Ext.window.Window', {
    title: '群組管理',
    width: 600,
    height: document.documentElement.clientHeight-200,
    layout: 'fit',
    items: [productGroupGrid],
    closeAction: 'hide',
    modal: true,
    resizable: false,
    constrain: true,
    bodyStyle: 'padding:5px 5px 5px 5px'
})

var center = Ext.create('Ext.form.Panel', {
    id: 'center',
    autoScroll: true,
    border: false,
    frame: false,
    layout: { type: 'vbox', align: 'stretch' },
    defaults: { margin: '2 2 2 2' },
    items: [
        {
            flex: 2.0,
            title: '基本資料',
            autoScroll: true,
            frame: false,
            items: [
                {
                    xtype: 'container',
                    autoScroll: true,
                    defaults: { margin: '0 5 5 10', labelWidth: 60, autoScroll: true, width: 1150 },
                    items: [

                        {
                            xtype: 'container',
                            layout: 'hbox',
                            autoScroll: true,
                            defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                            id: 'part1',
                            items: [{
                                xtype: 'textfield',
                                id: 'event_name',
                                fieldLabel: '活動名稱',
                                margin: '3 5 5 0',
                                allowBlank: false
                            }, {
                                xtype: 'textfield',
                                id: 'event_desc',
                                allowBlank: false,
                                fieldLabel: '活動描述'
                            }, {
                                xtype: 'datetimefield',
                                format: 'Y-m-d H:i:s',
                                time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                                id: 'event_start',
                                labelWidth: 60,
                                width: 220,
                                allowBlank: false,
                                editable: false,
                                celleditable: false,
                                minValue: new Date(),
                                fieldLabel: '開始時間',
                                listeners: {
                                    select: function (a, b, c) {
                                        var start = Ext.getCmp("event_start");
                                        var end = Ext.getCmp("event_end");
                                        if (end.getValue() == null) {
                                            end.setValue(setNextMonth(start.getValue(), 1));
                                        } else if (end.getValue() < start.getValue()) {
                                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                                            start.setValue(setNextMonth(end.getValue(), -1));
                                        }
                                    }
                                }
                            }, {
                                xtype: 'datetimefield',
                                format: 'Y-m-d H:i:s',
                                time: { hour: 23, min: 59, sec: 59 },
                                id: 'event_end',
                                labelWidth: 60,
                                width: 220,
                                allowBlank: false,
                                editable: false,
                                celleditable: false,
                                minValue: new Date(),
                                fieldLabel: '結束時間',
                                listeners: {
                                    select: function (a, b, c) {
                                        var start = Ext.getCmp("event_start");
                                        var end = Ext.getCmp("event_end");
                                        if (start.getValue() != "" && start.getValue() != null) {
                                            if (end.getValue() < start.getValue()) {
                                                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                                end.setValue(setNextMonth(start.getValue(), 1));
                                            }
                                        }
                                        else {
                                            start.setValue(setNextMonth(end.getValue(), -1));
                                        }
                                    }
                                }
                            }]
                        },
                         {
                             xtype: 'container',
                             layout: 'hbox',
                             id: 'part2',
                             defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                             items: [
                                   {
                                       xtype: 'combobox', //Wibset
                                       allowBlank: false,
                                       editable: false,
                                       fieldLabel: "活動站台",
                                       margin: '3 5 5 0',
                                       id: 'site_id',
                                       name: 'site_id',
                                       hiddenName: 'site_id',
                                       store: SiteStore,
                                       displayField: 'Site_Name',
                                       valueField: 'Site_Id',
                                       typeAhead: true,
                                       forceSelection: false,
                                       queryMode: 'local',
                                       multiSelect: true, //多選
                                       emptyText: "請選擇",
                                       listeners: {
                                           beforequery: function (qe) {
                                               delete qe.combo.lastQuery;
                                               SiteStore.load();
                                           }
                                       }
                                   }
                              ,
                               {

                                   xtype: 'combobox',
                                   allowBlank: false,
                                   fieldLabel: '會員條件',
                                   id: 'user_condition_id',
                                   name: 'user_condition_id',
                                   store: UserConStore,
                                   displayField: 'condi_name',
                                   valueField: 'condi_id',
                                   hiddenname: 'condi_id',
                                   typeAhead: true,
                                   queryMode: 'local',
                                   lastQuery: '',
                                   emptyText: "請選擇",
                                   forceSelection: false,
                                   listeners: {
                                       blur: function (combo, eOpts) {
                                           var val = combo.getRawValue();
                                           if (val != "") {
                                               var store = combo.store;
                                               if (store.find("condi_name", val) == -1) {
                                                   combo.reset();
                                                   combo.isValid();
                                                   Ext.Msg.alert(INFORMATION, "該會員條件不存在！");
                                               }
                                           }
                                       }
                                   }

                               },
                              {
                                  xtype: 'radiogroup',
                                  hidden: false,
                                  id: 'device',
                                  name: 'device',
                                  fieldLabel: "裝置",
                                  colName: 'device',
                                  width: 280,
                                  defaults: {
                                      name: 'device',
                                      margin: '0 8 0 0'
                                  },
                                  columns: 3,
                                  vertical: true,
                                  items: [
                                  { boxLabel: "不分", id: 'bufen', inputValue: '0', checked: true },
                                  { boxLabel: "PC", id: 'pc', inputValue: '1' },
                                  { boxLabel: "手機/平板", id: 'mobilepad', inputValue: '4' }
                                  ]
                              }

                             ]
                         },
                         {
                             xtype: 'container',
                             layout: 'hbox',
                             id: 'part5',
                             defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                             items: [
                                    {
                                        xtype: 'numberfield',
                                        fieldLabel: "滿額件數",
                                        margin: '3 5 5 0',
                                        name: 'quantity',
                                        id: 'quantity',
                                        allowBlank: false,
                                        minValue: 0,
                                        maxValue: 2147483647,
                                        hideTrigger: true
                                    }
                              ,
                              {
                                  xtype: 'numberfield',
                                  fieldLabel: "滿額金額",
                                  name: 'amount',
                                  id: 'amount',
                                  allowBlank: false,
                                  minValue: 0,
                                  maxValue: 2147483647,
                                  hideTrigger: true
                              },
                            {
                                xtype: 'numberfield',
                                fieldLabel: "限購數量",
                                name: 'num_limit',
                                id: 'num_limit',
                                allowBlank: false,
                                minValue: 0,
                                maxValue: 2147483647,
                                hideTrigger: true
                            },
                                 
                            {
                                xtype: 'combobox',
                                allowBlank: false,
                                fieldLabel: '商品群組',
                                id: 'group_id',
                                name: 'product_group_price',
                                store: productGroupStore,
                                displayField: 'group_name',
                                valueField: 'group_id',
                                typeAhead: true,
                                queryMode:'local',
                                lastQuery: '',
                                emptyText: "請選擇",
                                forceSelection: false,
                                listeners: {
                                        blur: function (combo, eOpts) {
                                            var val = combo.getRawValue();
                                            if (val != "") {
                                                var store = combo.store;
                                                if (store.find("group_name", val) == -1) {
                                                    combo.reset();
                                                    combo.isValid();
                                                    Ext.Msg.alert(INFORMATION, "該群組不存在！");
                                                }
                                            }
                                        }
                                    , beforequery: function (qe) {
                                        delete qe.combo.lastQuery;
                                        productGroupStore.load();
                                    }
                                    , change: function (combo, selvalue) {
                                        eventPromoProductItemStore.removeAll();
                                        eventPromoProductItemStore.load({
                                            params: {
                                                group_id: selvalue
                                            }
                                        });
                                    }
                                }
                            },
                            {
                                xtype: 'button',
                                text: "群組管理",
                                name: 'group_add',
                                id: 'group_add',
                                width:55,
                                handler: function () {
                                    productGroupCboStore.load();
                                    groupAddWin.show();
                                }
                            }
                             ]
                         },
                        {
                            xtype: 'container',
                            layout: 'hbox',
                            autoScroll: true,
                            defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                            id: 'part3',
                            items: [
                               {
                                   xtype: 'combobox',
                                   allowBlank: false,
                                   editable: false,
                                   fieldLabel: '商品設定',
                                   margin: '3 5 5 0',
                                   id: 'condition_type',
                                   name: 'condition_type',
                                   store: ConTypeStore,
                                   displayField: 'condi_name',
                                   valueField: 'condi_id',
                                   typeAhead: true,
                                   lastQuery: '',
                                   emptyText: "請選擇",
                                   forceSelection: false,
                                   listeners: {
                                       change: function (combo, records, eOpts) {
                                           var value = Ext.getCmp("condition_type").getValue();
                                           Ext.getCmp('p_brand').setValue("").hide();
                                           Ext.getCmp('p_class').setValue("").hide();
                                           Ext.getCmp('p_category').hide();
                                           Ext.getCmp('p_product').hide();
                                           Ext.getCmp('p_product_id').setValue("").hide();
                                           Ext.getCmp('p_cart').setValue("").hide();
                                           Ext.getCmp('p_payment').setValue("").hide();
                                           Ext.getCmp('category_name').setValue("").hide();
                                           Ext.getCmp('category_id').setValue("").hide();
                                           Ext.getCmp('part4').hide();
                                           ac_store.removeAll();
                                           if (value == "1") {
                                               Ext.getCmp('p_brand').show();
                                           } else if (value == "2") {
                                               Ext.getCmp('part4').show();
                                               Ext.getCmp('category_name').show();
                                               Ext.getCmp('category_id').show();
                                               Ext.getCmp('p_category').show();

                                           } else if (value == "3") {
                                               Ext.getCmp('p_class').show();
                                           } else if (value == "4") {
                                               Ext.getCmp("p_product_id").show();
                                               Ext.getCmp("p_product").show();
                                           } else if (value == "5") {
                                               Ext.getCmp('p_cart').show();
                                           } else if (value == "6") {
                                               Ext.getCmp('p_payment').show();
                                           }
                                       }
                                   }
                               },


                                 {
                                     xtype: 'combobox', //Wibset
                                     editable: false,
                                     hidden: true,
                                     fieldLabel: "活動品牌",
                                     margin: '3 5 5 10',
                                     id: 'p_brand',
                                     name: 'p_brand',
                                     hiddenName: 'p_brand',
                                     store: BrandStore,
                                     displayField: 'brand_name',
                                     valueField: 'brand_id',
                                     typeAhead: true,
                                     forceSelection: false,
                                     queryMode: 'local',
                                     multiSelect: true, //多選
                                     emptyText: "請選擇",
                                     listeners: {
                                         beforequery: function (qe) {
                                             delete qe.combo.lastQuery;
                                             BrandStore.load({
                                                 params: {
                                                     isShowGrade: 1
                                                 }
                                             });
                                         }
                                     }
                                 },
                                  {
                                      xtype: 'combobox', //Wibset
                                      editable: false,
                                      hidden: true,
                                      fieldLabel: "活動館別",
                                      margin: '3 5 5 10',
                                      id: 'p_class',
                                      name: 'p_class',
                                      hiddenName: 'class_id',
                                      store: ShopClassStore,
                                      displayField: 'class_name',
                                      valueField: 'class_id',
                                      typeAhead: true,
                                      forceSelection: false,
                                      queryMode: 'local',
                                      multiSelect: true, //多選
                                      emptyText: "請選擇",
                                      listeners: {
                                          beforequery: function (qe) {
                                              delete qe.combo.lastQuery;
                                              ShopClassStore.load();
                                          }
                                      }
                                  },
                                   {
                                       xtype: 'combobox', //Wibset
                                       editable: false,
                                       hidden: true,
                                       fieldLabel: "付款方式",
                                       margin: '3 5 5 10',
                                       id: 'p_payment',
                                       name: 'p_payment',
                                       hiddenName: 'parameterCode',
                                       store: PaymentStore,
                                       displayField: 'parameterName',
                                       valueField: 'parameterCode',
                                       typeAhead: true,
                                       forceSelection: false,
                                       queryMode: 'local',
                                       multiSelect: true, //多選
                                       emptyText: "請選擇",
                                       listeners: {
                                           beforequery: function (qe) {
                                               delete qe.combo.lastQuery;
                                               PaymentStore.load();
                                           }
                                       }
                                   },
                                   {
                                       xtype: 'combobox', //Wibset
                                       editable: false,
                                       hidden: true,
                                       fieldLabel: "購物車",
                                       margin: '3 5 5 10',
                                       id: 'p_cart',
                                       name: 'p_cart',
                                       hiddenName: 'cart_id',
                                       store: ShopCartStore,
                                       displayField: 'cart_name',
                                       valueField: 'cart_id',
                                       typeAhead: true,
                                       forceSelection: false,
                                       queryMode: 'local',
                                       multiSelect: true, //多選
                                       emptyText: "請選擇",
                                       listeners: {
                                           beforequery: function (qe) {
                                               delete qe.combo.lastQuery;
                                               ShopCartStore.load();
                                           }
                                       }

                                   },
                                       {
                                           xtype: 'textfield',
                                           fieldLabel: "商品編號",
                                           hidden: true,
                                           readOnly: true,
                                           id: 'p_product_id',
                                           name: 'p_product_id',
                                           width: 200
                                       },
                                       {
                                           xtype: 'textfield',
                                           fieldLabel: "商品名稱",
                                           hidden: true,
                                           readOnly: true,
                                           id: 'p_product_name',
                                           name: 'p_product_name',
                                           width: 200
                                       },
                                    {//商品設定
                                        xtype: 'button',
                                        text: "選擇商品",
                                        hidden: true,
                                        id: 'p_product',
                                        name: 'p_product',
                                        width: 80,
                                        handler: function () {
                                            var selectproductid = Ext.getCmp("p_product_id").getValue();
                                            PromationMationShow();
                                        }
                                    },
                            {
                                xtype: 'button',
                                text: "請選擇類別",
                                hidden: true,
                                id: 'p_category',
                                name: 'p_category',
                                width: 100,
                                handler: function () {
                                    var categoryid = Ext.getCmp("category_id").getValue();
                                    var categoryname = Ext.getCmp("category_name").getValue();
                                    SelectImgCate(categoryid, categoryname, treeCateStore);
                                }
                            },
                              {//類別id
                                  xtype: 'displayfield',
                                  fieldLabel: "類別ID",
                                  editable: false,
                                  hidden: true,
                                  id: 'category_id',
                                  name: 'category_id',
                                  width: 400
                              }                       
                            ]
                        },
                        {
                            xtype: 'container',
                            layout: 'hbox',
                            autoScroll: true,
                            hidden: true,
                            defaults: { margin: '1 85 8 1', labelWidth: 60 },
                            id: 'part4',
                            items: [
                                  { xtype: 'tbfill' },
                               {
                                   //類別名稱
                                   xtype: 'displayfield',
                                   fieldLabel: "類別名稱",
                                   hidden: true,
                                   editable: false,
                                   id: 'category_name',
                                   name: 'category_name',
                                   width: 600
                               }
                            ]
                        },
                        
                        {
                            title: '商品細項',
                            xtype: 'gridpanel',
                            margins: '5 4 5 5',
                            id: 'productItemGrid',
                            store: eventPromoProductItemStore,
                            autoScroll: true,
                            border: 0,
                            frame: false,
                            columns: [
                                { header: "商品編號", dataIndex: 'product_id', width: 100, menuDisabled: true, sortable: false },
                                { header: "商品名稱", dataIndex: 'product_name', width: 150, menuDisabled: true, sortable: false, flex: 1 },
                                { header: "群組編號", dataIndex: 'group_id', width: 100, menuDisabled: true, sortable: false },
                                { header: "群組名稱", dataIndex: 'group_name', width: 100, menuDisabled: true, sortable: false },
                                { header: '加購價', dataIndex: 'add_price', width: 100, menuDisabled: true, sortable: false }
                            ],
                            tbar: [
                                { xtype: 'button', id: 'item_add', text: "新增", iconCls: 'icon-add', handler: function () { addProducts(null, eventPromoProductItemStore); } },

                                { xtype: 'button', id: 't_edit_product', text: "編輯", iconCls: 'icon-edit', handler: function () { editProducts(); }, disabled: true },

                                { xtype: 'button', id: 't_del_product', text: "移除", iconCls: 'icon-remove', handler: function () { delProducts(); }, disabled: true }

                            ],
                            viewConfig: { emptyText: '<span>暫無數據！</span>' },
                            selModel: product_cm,
                            listeners: {
                                scrollershow: function (scroller) {
                                    if (scroller && scroller.scrollEl) {
                                        scroller.clearManagedListeners();
                                        scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                                    }
                                }
                            }
                        }
                        //{
                        //    title: '活動商品',
                        //    xtype: 'gridpanel',
                        //    frame: false,
                        //    layout: 'fit',
                        //    //width: 500,
                        //    margins: '5 4 5 5',
                        //    items: [
                        //    {
                        //        title: "商品細項",
                        //        items: [productItemGrid],
                        //    }
                        //    //,
                        //    //{
                        //    //    title: "商品群組",
                        //    //    items: [productGroupGrid],
                        //    //}
                        //    ]
                        //}
                    ]
                }]
        }],
    bbar: [{
        text: '保存',
        id: 'btn_save',
        iconCls: 'ui-icon ui-icon-checked'
        ,
        handler: Save
    }, {
        text: '重置',
        id: 'btn_reset',
        iconCls: 'ui-icon ui-icon-reset',
        handler: function () { LoadDetail(currentRecord); }
    }, {
        text: '取消',
        id: 'btn_cancel',
        iconCls: 'ui-icon ui-icon-cancel',
        handler: function () {
            Ext.getCmp('west-region-container').setDisabled(false);
        }
    }]
})



function Save()
{
    if (Ext.getCmp('center').getForm().isValid())
    {
        if (eventPromoProductItemStore.data.length <= 0)
        {
            Ext.Msg.alert(INFORMATION, '請將商品細項資料填寫完整');
            return;
        }

        var myMask = new Ext.LoadMask(Ext.getBody(), {
            msg: 'Loading...'
        });
        myMask.show();

        var eventPromoAdditionalPrice = {};
        eventPromoAdditionalPrice.row_id = currentRecord.data.row_id == null ? 0 : currentRecord.data.row_id;
        eventPromoAdditionalPrice.event_name = Ext.htmlEncode(Ext.getCmp('event_name').getValue());
        eventPromoAdditionalPrice.event_desc = Ext.htmlEncode(Ext.getCmp('event_desc').getValue());
        eventPromoAdditionalPrice.site_id = Ext.htmlEncode(Ext.getCmp('site_id').getValue());
        eventPromoAdditionalPrice.user_condition_id = Ext.htmlEncode(Ext.getCmp('user_condition_id').getValue());
        eventPromoAdditionalPrice.condition_type = Ext.htmlEncode(Ext.getCmp('condition_type').getValue());
        eventPromoAdditionalPrice.device = Ext.htmlEncode(Ext.getCmp('device').getValue().device);
        eventPromoAdditionalPrice.event_start = Ext.Date.format(new Date(Ext.getCmp('event_start').getValue()), 'Y-m-d H:i:s');
        eventPromoAdditionalPrice.event_end = Ext.Date.format(new Date(Ext.getCmp('event_end').getValue()), 'Y-m-d H:i:s');
        eventPromoAdditionalPrice.event_status = 0;//改動后默認狀態為停用
        eventPromoAdditionalPrice.quantity = Ext.htmlEncode(Ext.getCmp('quantity').getValue());
        eventPromoAdditionalPrice.amount = Ext.htmlEncode(Ext.getCmp('amount').getValue());
        eventPromoAdditionalPrice.num_limit = Ext.htmlEncode(Ext.getCmp('num_limit').getValue());
        eventPromoAdditionalPrice.group_id = Ext.htmlEncode(Ext.getCmp('group_id').getValue());

        var eventPromoAdditionalPriceProduct = [];

        for (var i = 0, j =eventPromoProductItemStore.data.length ; i < j; i++) {
            var record = eventPromoProductItemStore.data.items[i];
            eventPromoAdditionalPriceProduct.push({
                'row_id': record.get("row_id"),
                'group_id': Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                'product_id': record.get("product_id"),
                'add_price': record.get("add_price")
            });
        }
        var condiType = "";
        switch (eventPromoAdditionalPrice.condition_type) {
            case "1"://品牌
                condiType = Ext.getCmp('p_brand').getValue();
                break;
            case "2"://類別
                condiType = Ext.getCmp('category_id').getValue();//類別內包含的值
                break;
            case "3"://館別
                condiType = Ext.getCmp('p_class').getValue();
                break;
            case "4"://商品
                condiType = Ext.getCmp('p_product_name').getValue();//商品編號&數量,形式
                break;
            case "5"://購物車
                condiType = Ext.getCmp('p_cart').getValue();
                break;
            case "6"://付款方式
                condiType = Ext.getCmp('p_payment').getValue();
                break;
        }

        Ext.Ajax.request({
            url: '/EventPromo/SaveEventPromoAdditionalPrice',
            params: {
                eventPromoAdditionalPriceStr: Ext.encode(eventPromoAdditionalPrice),
                eventPromoAdditionalPriceProductStr: Ext.encode(eventPromoAdditionalPriceProduct),
                condiType: condiType
            },
            timeout: 360000,
            success: function (response) {
                var res = Ext.decode(response.responseText);
                if (res.success) {
                    Ext.Msg.alert(INFORMATION, SUCCESS);
                    myMask.hide();
                    Ext.getCmp('west-region-container').setDisabled(false);
                    Ext.getCmp('center').getForm().reset();
                    eventPromoProductItemStore.removeAll();
                    Search();
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    myMask.hide();
                }

            },
            failure: function (response, opts) {
                if (response.timedout) {
                    Ext.Msg.alert(INFORMATION, TIME_OUT);
                }
                myMask.hide();
            }
        });

    }
}


Ext.onReady(function () {
    treeCateStore.load();//商品類別
    UserConStore.load();//活動會員條件store
    ConTypeStore.load();//參與活動的對象類型

    Ext.QuickTips.init();
    Ext.create('Ext.Viewport', {
        id: "index",
        autoScroll: true,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'border',
        items: [{
            region: 'west',//左西
            xtype: 'panel',
            autoScroll: true,
            frame: false,
            width: 400,
            margins: '5 4 5 5',
            id: 'west-region-container',
            layout: 'anchor',
            items: eventPromoAdditionalPrice
        }
        ,
        {
            region: 'center',//中間
            id: 'center-region-container',
            xtype: 'panel',
            frame: false,
            layout: 'fit',
            width: 500,
            margins: '5 4 5 5',
            items: center
        }
        ],
        listeners: {
            resize: function () {
                Ext.getCmp("index").width = document.documentElement.clientWidth;
                Ext.getCmp("index").height = document.documentElement.clientHeight;
                this.doLayout();
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        renderTo: Ext.getBody()
    });

});