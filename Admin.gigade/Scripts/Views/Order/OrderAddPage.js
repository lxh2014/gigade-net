/*  
 * 
 * 文件名称：OrderAddPage.js 
 * 摘    要：訂單輸入頁面
 * 
 */
var myPanel;
var currentCol = 0;
var currentRow = 0;
//var combChannel;
var myMask;
var storeType;
var isSuper;
var docWidth = document.documentElement.clientWidth;
var docHeight = document.documentElement.clientHeight;
var receipt_to;
//var ship_logistics;
var retrieve_mode;

//物流模式
var storeModeStroe = Ext.create('Ext.data.Store', {
    id: 'storeModeStroe',
    fields: ['shipping_carrior', 'shipco', 'sort', 'retrieve_mode'],
    autoDestroy: true,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Order/GetStore',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//超商店家(订购人)
var b_superStore = Ext.create('Ext.data.Store', {
    id: 'b_superStore',
    name: 'b_superStore',
    autoDestroy: true,
    autoLoad: false,
    fields: ['store_id', 'store_name', 'address'],
    proxy: {
        type: 'ajax',
        url: '/Order/GetSuperStore',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//超商店家(收件人)
var r_superStore = Ext.create('Ext.data.Store', {
    id: 'r_superStore',
    name: 'r_superStore',
    autoDestroy: true,
    autoLoad: false,
    fields: ['store_id', 'store_name', 'address'],
    proxy: {
        type: 'ajax',
        url: '/Order/GetSuperStore',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//付款方式
var paymentStore = Ext.create('Ext.data.Store', {
    id: 'paymentStore',
    autoDestroy: true,
    fields: ['parameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Order/GetPayment',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//地址Model
Ext.define('gigade.City', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "middle", type: "string" },
        { name: "middlecode", type: "string" }]
});

//認購人公司地址Store
var CityStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.City',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryCity",
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//收件人公司地址Store
var r_CityStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.City',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryCity",
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//郵編Model
Ext.define('gigade.Zip', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "zipcode", type: "string" },
        { name: "small", type: "string" }]
});

//訂購人公司郵編Store
var ZipStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    remoteSort: false,
    autoLoad: false,
    model: 'gigade.Zip',
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryZip",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//收件人公司郵編Store
var r_ZipStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    remoteSort: false,
    autoLoad: false,
    model: 'gigade.Zip',
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryZip",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//add by zhuoqin0830w  2015/02/25  公關單與報廢單功能
var orderPatternStore = Ext.create('Ext.data.Store', {
    id: 'orderPatternStore',
    autoDestroy: true,
    fields: ['ParameterCode', 'parameterName', 'TopValue'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryParaByXml?paraType=orderPattern',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var depStore = Ext.create('Ext.data.Store', {
    id: 'depStore',
    autoDestroy: true,
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/GetDeptByTopValue',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//add by zhuoqin0830w  2015/07/03
var siteStore = Ext.create('Ext.data.Store', {
    id: 'siteStore',
    autoDestroy: true,
    fields: ['Site_Id', 'Site_Name', 'Cart_Delivery'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//Store加載數據之前的處理
ZipStore.on('beforeload', function () {
    Ext.apply(ZipStore.proxy.extraParams,
    {
        topValue: Ext.getCmp("cob_ccity").getValue(),
        topText: Ext.getCmp("cob_ccity").getRawValue()
    });
});

r_ZipStore.on('beforeload', function () {
    Ext.apply(r_ZipStore.proxy.extraParams,
    {
        topValue: Ext.getCmp("r_cob_ccity").getValue(),
        topText: Ext.getCmp("r_cob_ccity").getRawValue()
    });
});

onAddClick = function () {
    addWin.show();
}

//收件人信息同訂購人
function sameAsBuyerClick() {
    var c = Ext.getCmp("cob_ccity");
    if ($("#chkSame").attr("checked")) {
        Ext.getCmp("r_txtCNFullName").setValue(Ext.getCmp("txtCNFullName").getRawValue());
        Ext.getCmp("r_txtActionPhone").setValue(Ext.getCmp("txtActionPhone").getValue());
        Ext.getCmp("r_txtContactPhoneHead").setValue(Ext.getCmp("txtContactPhoneHead").getValue());
        Ext.getCmp("r_txtContactPhoneContent").setValue(Ext.getCmp("txtContactPhoneContent").getValue());
        Ext.getCmp("r_txtContactAddress").setValue(Ext.getCmp("b_txtAddress").getValue());
        Ext.getCmp("g_checkActionSex").setValue({ 'g_checkActionSex': Ext.getCmp("s_checkActionSex").getValue().s_checkActionSex }); //add by wwei0216w 添加收購人性別同訂購人相同 2015/1/21
        //add by zhuoqin0830w  2015/11/12
        Ext.getCmp("r_userEmail").setValue(Ext.getCmp("d_userEmail").getValue());
        Ext.getCmp("r_userID").setValue(Ext.getCmp("d_userID").getValue());
        if (isSuper) {
            r_superStore.load({ params: { storeId: Ext.getCmp('combStoreMode').getValue() } });
            Ext.getCmp('r_combSuperMarket').setValue(Ext.getCmp('b_combSuperMarket').getValue());
        }
        else {
            if (Ext.getCmp("cob_ccity").getValue() == null) {
                return;
            }
            r_CityStore.load();
            Ext.getCmp("r_cob_ccity").setFieldDefaults(Ext.getCmp("cob_ccity").getValue());
            Ext.getCmp("r_cob_ccity").setValue(Ext.getCmp("cob_ccity").getValue());
            r_ZipStore.load({
                params: {
                    topValue: Ext.getCmp("cob_ccity").getValue(),
                    topText: Ext.getCmp("cob_ccity").getRawValue()
                }
            });
            Ext.getCmp("r_cob_czip").setValue(Ext.getCmp("cob_czip").getValue());
        }
    }
}

//訂購人信息同收件人
function sameAsReceiverClick() {
    var c = Ext.getCmp("cob_ccity");
    if ($("#chkSameReceiver").attr("checked")) {
        Ext.getCmp("txtCNFullName").setValue(Ext.getCmp("r_txtCNFullName").getValue());
        Ext.getCmp("txtActionPhone").setValue(Ext.getCmp("r_txtActionPhone").getValue());
        Ext.getCmp("txtContactPhoneHead").setValue(Ext.getCmp("r_txtContactPhoneHead").getValue());
        Ext.getCmp("txtContactPhoneContent").setValue(Ext.getCmp("r_txtContactPhoneContent").getValue());
        Ext.getCmp("b_txtAddress").setValue(Ext.getCmp("r_txtContactAddress").getValue());
        Ext.getCmp("s_checkActionSex").setValue({ 's_checkActionSex': Ext.getCmp("g_checkActionSex").getValue().g_checkActionSex }); //add by wwei0216w 添加訂購人人性別同收購人相同 2015/1/21
        //add by zhuoqin0830w  2015/11/12
        Ext.getCmp("d_userEmail").setValue(Ext.getCmp("r_userEmail").getValue());
        Ext.getCmp("d_userID").setValue(Ext.getCmp("r_userID").getValue());
        if (isSuper) {
            b_superStore.load({ params: { storeId: Ext.getCmp('combStoreMode').getValue() } });
            Ext.getCmp('b_combSuperMarket').setValue(Ext.getCmp('r_combSuperMarket').getValue());
        } else {
            if (Ext.getCmp("r_cob_ccity").getValue() == null) {
                return;
            }
            CityStore.load();
            Ext.getCmp("cob_ccity").setFieldDefaults(Ext.getCmp("r_cob_ccity").getValue());
            Ext.getCmp("cob_ccity").setValue(Ext.getCmp("r_cob_ccity").getValue());

            ZipStore.load({
                params: {
                    topValue: Ext.getCmp("r_cob_ccity").getValue(),
                    topText: Ext.getCmp("r_cob_ccity").getRawValue()
                }
            });
            Ext.getCmp("cob_czip").setValue(Ext.getCmp("r_cob_czip").getValue());
        }
    }
}

function combChannelSelect(comb, record) {
    Ext.getCmp('btnSubmit').setDisabled(true);
    Ext.getCmp('combStoreMode').getStore().removeAll();
    Ext.getCmp('combStoreMode').clearValue();
    receipt_to = record[0].data.receipt_to;
    storeType = parseInt(record[0].data.channel_type);
    orderStore.removeAll();

    orderStore.add({
        product_id: '',
        item_id: '',
        product_name: '',
        spec1_show: '',
        spec2_show: '',
        product_cost: '0',
        Event_Item_Money: '0',//活動售價  add by zhuoqin0830w  2015/11/16
        Event_Item_Cost: '0',//活動成本
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        stock: '0',
        buynum: '0',
        sumprice: '0',
        empty: ''
    });
}

myPanel = Ext.create('Ext.form.Panel', {
    layout: 'anchor',
    defaults: { anchor: "95%" },
    //height: docHeight / 2,
    height: 450,
    width: docWidth * 0.8,
    minWidth: 1100,
    bodyPadding: '10 0 0 10',
    plain: true,
    border: false,
    id: 'inforForm',
    url: '/Order/OrderSave',
    items: [{
        columnWidth: 1,
        height: location.pathname == "/Order/InteriorOrderAdd" ? 135 : 85,
        border: false,
        items: [{
            xtype: 'panel',
            layout: 'hbox',
            border: false,
            defaultType: 'combobox',
            items: [{
                xtype: 'combobox',
                fieldLabel: CHANNEL_NAME,
                labelWidth: 65,
                allowBlank: false,
                store: outsiteStore,
                displayField: 'channel_name_full',
                name: 'combChannelId',
                id: 'combChannelId',
                valueField: 'channel_id',
                style: { 'margin-right': '10px' },
                editable: false,
                listeners: {
                    select: combChannelSelect
                }
            }, {//add by zhuoqin0830w  2015/07/03  添加站臺欄位
                fieldLabel: SITE,
                editable: false,
                hidden: true,
                labelWidth: 65,
                style: { 'margin-right': '10px' },
                store: siteStore,
                id: 'Site_Id',
                name: 'Site_Id',
                displayField: 'Site_Name',
                valueField: 'Site_Id',
                queryMode: 'local',
                colName: 'site_id',
                listeners: {//edit by zhuoqin0830w  2015/11/16   根據站台的選擇來顯示grid中的相關信息
                    select: function (comb, record) {
                        var combOrderDate = Ext.getCmp("combOrderDate").getValue();
                        var Site_Id = record[0].data.Site_Id;
                        var result;
                        if (combOrderDate != null && combOrderDate != "") {
                            for (var i = 0; i <= orderStore.data.length; i++) {
                                if (orderStore.getAt(i).get("product_id") != " " && orderStore.getAt(i).get("product_id") != "" && orderStore.getAt(i).get("product_id") != null) {
                                    if (orderStore.getAt(i).get("item_id") != "0") {//組合商品在後台中傳到前台的item_id為0 所以在這使用item_id進行判斷
                                        result = searchByProID(orderStore.getAt(i).get("item_id"), i, null, null, orderStore.getAt(i), null, Site_Id, combOrderDate);
                                        if (result == 0) { orderStore.removeAt(i); i--; continue; }
                                    } else {
                                        var curData = orderStore.getAt(i);
                                        var length = curData.get("g_must_buy");
                                        var parentIdx = findParentIdx(i);
                                        var parentBuyNum = orderStore.getAt(parentIdx).get('buynum');
                                        for (var j = 1; j <= length;) {
                                            if (orderStore.getAt(j + i).get("product_id") == " ") { //實行 j + i 的原因：  因為組合商品中需要刪除子商品后再進行添加操作 而如果不實行的話 則會在頁面上重複顯示
                                                orderStore.removeAt(j + i);
                                                length--;
                                            }
                                            else {
                                                break;
                                            }
                                        }
                                        result = searchByProID(orderStore.getAt(i).get("product_id"), i, null, null, orderStore.getAt(i), null, Site_Id, combOrderDate);
                                        if (result == 0) { orderStore.removeAt(i); i--; continue; }
                                    }
                                }
                            }
                        }
                    }
                }
            }, {
                fieldLabel: STORE_MODE,
                editable: false,
                allowBlank: false,
                store: storeModeStroe,
                queryMode: 'local',
                //submitValue: false,
                id: 'combStoreMode',
                name: 'combStoreMode',
                style: { 'margin-right': '10px' },
                labelWidth: 65,
                displayField: 'shipco',
                valueField: 'shipping_carrior',
                listeners: {
                    beforequery: function (qe) {
                        if (Ext.getCmp('combChannelId').getValue() == null) {
                            return;
                        }
                        storeModeStroe.load({ params: { storeType: storeType, channel_id: Ext.getCmp('combChannelId').getValue() } });
                    },
                    select: function (comb, record) {
                        Ext.getCmp('b_txtAddress').setRawValue('');
                        Ext.getCmp('r_txtContactAddress').setRawValue('');
                        retrieve_mode = record[0].data.retrieve_mode;
                        var sort = record[0].data.sort;
                        if (sort == 1) {
                            isSuper = true;
                            Ext.getCmp('b_fcZip').hide();
                            Ext.getCmp('r_fcZip').hide();
                            Ext.getCmp('b_combSuperMarket').show();
                            Ext.getCmp('r_combSuperMarket').show();
                        }
                        else {
                            isSuper = false;
                            Ext.getCmp('b_fcZip').show();
                            Ext.getCmp('r_fcZip').show();
                            Ext.getCmp('b_combSuperMarket').hide();
                            Ext.getCmp('r_combSuperMarket').hide();
                        }

                        Ext.getCmp('cob_ccity').setRawValue(null);
                        Ext.getCmp('cob_czip').setRawValue(null);
                        Ext.getCmp('r_cob_ccity').setRawValue(null);
                        Ext.getCmp('r_cob_czip').setRawValue(null);
                        Ext.getCmp('cob_ccity').setValue(null);
                        Ext.getCmp('cob_czip').setValue(null);
                        Ext.getCmp('r_cob_ccity').setValue(null);
                        Ext.getCmp('r_cob_czip').setValue(null);

                        r_ZipStore.removeAll();
                        ZipStore.removeAll();

                        Ext.getCmp('b_combSuperMarket').setRawValue('');
                        Ext.getCmp('r_combSuperMarket').setRawValue('');
                        Ext.getCmp('b_combSuperMarket').setValue('');
                        Ext.getCmp('r_combSuperMarket').setValue('');

                        b_superStore.removeAll();
                        r_superStore.removeAll();
                    }
                }
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: ORDER_DATE,
                labelWidth: 65,
                layout: 'column',
                items: [{
                    xtype: 'datetimefield',
                    disabledMin: true,
                    disabledSec: true,
                    name: 'combOrderDate',
                    id: 'combOrderDate',
                    allowBlank: false,
                    editable: false,
                    maxValue: new Date(),
                    format: 'Y-m-d H:i:s',
                    listeners: {//edit by zhuoqin0830w  2015/11/16   根據訂單日期判斷活動價格和活動成本是否顯示
                        select: function (a, b, c) {
                            var combOrderDate = Ext.getCmp("combOrderDate").getValue();
                            var Site_Id = Ext.getCmp("Site_Id").getValue();
                            var result;
                            if (combOrderDate != null && combOrderDate != "") {
                                for (var i = 0; i < orderStore.data.length; i++) {
                                    if (orderStore.getAt(i).get("product_id") != " " && orderStore.getAt(i).get("product_id") != "" && orderStore.getAt(i).get("product_id") != null) {
                                        if (orderStore.getAt(i).get("item_id") != "0") {//組合商品在後台中傳到前台的item_id為0 所以在這使用item_id進行判斷
                                            result = searchByProID(orderStore.getAt(i).get("item_id"), i, null, null, orderStore.getAt(i), null, Site_Id, combOrderDate);
                                            if (result == 0) { orderStore.removeAt(i); i--; continue; }
                                        } else {
                                            var curData = orderStore.getAt(i);
                                            var length = curData.get("g_must_buy");
                                            var parentIdx = findParentIdx(i);
                                            var parentBuyNum = orderStore.getAt(parentIdx).get('buynum');
                                            for (var j = 1; j <= length;) {
                                                if (orderStore.getAt(j + i).get("product_id") == " ") { //實行 j + i 的原因：  因為組合商品中需要刪除子商品后在進行添加操作 而如果不實行的話 則會在頁面上
                                                    orderStore.removeAt(j + i);
                                                    length--;
                                                }
                                                else {
                                                    break;
                                                }
                                            }
                                            result = searchByProID(orderStore.getAt(i).get("product_id"), i, null, null, orderStore.getAt(i), null, Site_Id, combOrderDate);
                                            if (result == 0) { orderStore.removeAt(i); i--; continue; }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }]
            }]
        }, {
            xtype: 'panel',
            layout: 'hbox',
            border: false,
            defaultType: 'combobox',
            items: [{
                fieldLabel: PAYMENT_WAY,
                name: 'combPayMent',
                allowBlank: false,
                editable: false,
                labelWidth: 65,
                style: { 'margin-right': '10px' },
                store: paymentStore,
                displayField: 'parameterName',
                valueField: 'parameterCode'
            }, {
                xtype: 'datefield',
                fieldLabel: LATEST_DELIVERY_DATE,
                name: 'combLatestDeliverDate',
                allowBlank: true,
                editable: false,
                labelWidth: 65,
                style: { 'margin-right': '10px' },
                format: 'Y-m-d'
            }, {//add by zhuoqin0830w  2015/02/25  公關單與報廢單功能
                xtype: 'combobox',
                fieldLabel: BILLTYPE,
                labelWidth: 65,
                store: orderPatternStore,
                displayField: 'parameterName',
                valueField: 'ParameterCode',
                id: "BillType",
                name: 'BillType',
                style: { 'margin-right': '10px' },
                colName: 'BillType',
                editable: false,
                hidden: true,
                listeners: {
                    select: function (comb, record) {
                        if (Ext.getCmp("BillType").getValue()) {
                            Ext.getCmp("dep").show().clearValue();
                            Ext.getCmp("dep").allowBlank = false;
                        }
                        else {
                            Ext.getCmp("dep").hide().clearValue();
                            Ext.getCmp("dep").allowBlank = true;
                        }
                        // add by zhuoqin0830w  2015/04/09  根據 單據類型 提取 部門類別 的 stroe
                        //清空 部門類別 的stroe
                        Ext.getCmp('dep').getStore().removeAll();
                        Ext.getCmp('dep').clearValue();
                        depStore.load({ params: { TopValue: record[0].data.TopValue } });
                    }
                }
            }, {//add by zhuoqin0830w  2015/04/09   公關單與報廢單功能
                xtype: 'combobox',
                fieldLabel: DEPTTYPE,//部門
                labelWidth: 65,
                queryMode: 'local',
                listConfig: { loadMask: false },
                store: depStore,
                displayField: 'parameterName',
                valueField: 'ParameterCode',
                id: "dep",
                name: 'dep',
                colName: 'dep',
                editable: false,
                hidden: true
            }]
        }, {
            xtype: 'panel',
            layout: 'hbox',
            border: false,
            defaultType: 'combobox',
            items: [{
                fieldLabel: ORDER_STATUS,
                editable: false,
                allowBlank: false,
                name: 'combOrderStatus',
                labelWidth: 65,
                style: { 'margin-right': '10px' },
                store: orderStatusStore,
                displayField: 'parameterName',
                valueField: 'parameterCode'
            }, {
                xtype: 'textfield',
                id: 'txtTradeNumber',
                submitValue: false,
                fieldLabel: TRADE_NUMBER,
                name: 'txtTradeNumber',
                style: { 'margin-right': '10px' },
                labelWidth: 65
            }, {
                xtype: 'textfield',
                submitValue: false,
                fieldLabel: ORDER_ID,
                id: 'txtOrderId',
                name: 'txtOrderId',
                style: { 'margin-right': '10px' },
                labelWidth: 65
            }, {
                xtype: 'textfield',
                submitValue: false,
                id: 'txtDeliverNumber',
                name: 'txtDeliverNumber',
                fieldLabel: DELIVERY_NUMBER,
                labelWidth: 85
            }]
        }, {
            xtype: 'panel',
            layout: 'hbox',
            border: false,
            defaultType: 'combobox',
            items: [{
                xtype: 'radiogroup',
                hidden: true,
                id: 'HolidayDeliver',
                labelWidth: 90,
                fieldLabel: ORDER_ISREST,
                colName: 'HolidayDeliver',
                width: 500,
                defaults: {
                    name: 'HolidayDeliver'
                },
                columns: 4,
                colName: 'HolidayDeliver',
                vertical: true,
                items: [
                    {
                        boxLabel: YES, inputValue: '1', checked: true

                    },
                    { boxLabel: NO, inputValue: '0' }, ]
            }]
        }, {
            xtype: 'panel',
            layout: 'hbox',
            border: false,
            defaultType: 'combobox',
            items: [{
                xtype: 'radiogroup',
                colName: 'EstimatedArrivalPeriod',
                hidden: true,
                id: 'EstimatedArrivalPeriod',//配送系統 add2014/09/15
                labelWidth: 90,
                fieldLabel: HOPE_TIME,
                width: 500,
                defaults: {
                    name: 'EstimatedArrivalPeriod'
                },
                columns: 4,
                vertical: true,
                items: [
                { boxLabel: IDSRESTRICT, inputValue: '0', checked: true },
                { boxLabel: PRE12, inputValue: '1' },
                { boxLabel: AFTERNOON, inputValue: '2' },
                { boxLabel: NIGHT, inputValue: '3' }
                ]
            }]
        }]
    }, {
        xtype: 'panel',
        border: false,
        width: '100%',
        bodyPadding: '5 0 0 0 ',
        layout: 'hbox',
        items: [{
            xtype: 'fieldset',
            title: '<input id="chkSameReceiver"  onclick="sameAsReceiverClick()" type="checkbox" /><label for="chkSameReceiver">' + SAME_AS_RECEIVER + '</label> ' + BUYER_INFO + '',
            width: 450,
            defaultType: 'textfield',
            layout: 'anchor',
            style: { 'margin-right': '30px' },
            items: [OrderName, {
                xtype: 'fieldcontainer',
                id: 'MAIL_GENDER',
                labelWidth: 65,
                layout: 'hbox',
                defaultType: 'textfield',
                items: [{
                    fieldLabel: CNFULLNAME,
                    name: 'b_txtName',
                    readOnly: true,
                    labelWidth: 65,
                    width: 200,
                    id: 'txtCNFullName',
                }, {//add by zhuoqin0830w  2015/11/12
                    fieldLabel: USEREMAIL,
                    name: 'd_userEmail',
                    readOnly: true,
                    margin: '0 0 0 10',
                    labelWidth: 65,
                    width: 200,
                    id: 'd_userEmail',
                }]
            }, {
                fieldLabel: ACTION_PHONENO,
                name: 'b_txtMobel',
                labelWidth: 65,
                vtype: 'regxMobileNo',
                allowBlank: true,
                width: 200,
                id: 'txtActionPhone'
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: CONTACT_PHONENO,
                labelWidth: 65,
                layout: 'hbox',
                defaultType: 'textfield',
                items: [{
                    width: 60,
                    submitValue: false,
                    name: 'b_txtPhoneHead',
                    id: 'txtContactPhoneHead'
                }, {
                    xtype: 'displayfield',
                    value: '  -  '
                }, {
                    width: 120,
                    submitValue: false,
                    name: 'b_txtPhoneContent',
                    id: 'txtContactPhoneContent'
                }]
            }, {
                xtype: 'fieldcontainer',
                id: 'b_fcZip',
                fieldLabel: POSTNO,
                labelWidth: 65,
                layout: 'hbox',
                defaultType: 'combobox',
                items: [{
                    editable: false,
                    labelWidth: 65,
                    width: 60,
                    store: CityStore,
                    displayField: 'middle',
                    valueField: 'middlecode',
                    queryMode: 'local',
                    id: 'cob_ccity',
                    name: 'b_combCity',
                    listeners: {
                        "select": function (combo, record) {
                            var z = Ext.getCmp("cob_czip");
                            z.clearValue();
                            ZipStore.removeAll();
                        },
                        beforequery: function (qe) {
                            CityStore.load();
                        }
                    }
                }, {
                    xtype: 'displayfield',
                    value: ' - '
                }, {
                    store: ZipStore,
                    editable: false,
                    width: 120,
                    queryMode: 'local',
                    style: { float: 'left' },
                    displayField: 'small',
                    valueField: 'zipcode',
                    id: 'cob_czip',
                    name: 'b_combZip',
                    listeners: {
                        beforequery: function (qe) {
                            if (Ext.getCmp('cob_ccity').getValue() == null) {
                                return;
                            }
                            delete qe.combo.lastQuery;
                            ZipStore.load({ params: { topValue: Ext.getCmp("cob_ccity").getValue() } });
                        }
                    }
                }]
            }, {
                xtype: 'combobox',
                id: 'b_combSuperMarket',
                name: 'b_combSuperMarket',
                store: b_superStore,
                displayField: 'store_name',
                valueField: 'store_id',
                queryMode: 'local',
                hidden: true,
                editable: false,
                labelWidth: 65,
                fieldLabel: SUPER_STORE,
                listeners: {
                    beforequery: function (qe) {
                        b_superStore.load({ params: { storeId: Ext.getCmp('combStoreMode').getValue() } });
                    },
                    select: function (comb, record) {
                        Ext.getCmp('b_txtAddress').setValue(record[0].data.address);
                    }
                }
            }, {
                fieldLabel: CONTACT_ADDRESS,
                labelWidth: 65,
                width: 300,
                submitValue: false,
                name: 'b_txtAddress',
                id: 'b_txtAddress'
            }]
        }, {
            xtype: 'fieldset',
            title: '<input id="chkSame" onclick="sameAsBuyerClick()" type="checkbox" /><label for="chkSame">' + SAME_AS_BUYER + '</label> ' + RECEIVER_INFO + '',
            width: 450,
            defaultType: 'textfield',
            layout: 'anchor',
            items: [{
                xtype: 'fieldcontainer',
                labelWidth: 65,
                layout: 'hbox',
                items: [{//add by zhuoqin0830w  2015/11/12
                    xtype: 'textfield',
                    fieldLabel: USERID,
                    name: 'r_userID',
                    allowBlank: false,
                    labelWidth: 65,
                    vtype: 'regxUserID',
                    width: 200,
                    id: 'r_userID',
                }, {
                    xtype: 'radiogroup',
                    id: 'g_checkActionSex',
                    labelWidth: 45,
                    margin: '0 0 0 10',
                    fieldLabel: GENDER,
                    width: 150,
                    defaults: {
                        name: 'g_checkActionSex'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                { boxLabel: BOY, inputValue: '1' },
                { boxLabel: GIRL, inputValue: '0', checked: true }]
                }]
            }, {
                xtype: 'fieldcontainer',
                labelWidth: 65,
                layout: 'hbox',
                items: [{
                    xtype: 'textfield',
                    fieldLabel: CNFULLNAME,
                    submitValue: false,
                    allowBlank: false,
                    labelWidth: 65,
                    width: 200,
                    id: 'r_txtCNFullName',
                    name: 'r_txtCNFullName'
                }, {//add by zhuoqin0830w  2015/11/12
                    fieldLabel: USEREMAIL,
                    xtype: 'textfield',
                    name: 'r_userEmail',
                    labelWidth: 65,
                    margin: '0 0 0 10',
                    width: 200,
                    id: 'r_userEmail',
                }]
            }, {
                fieldLabel: ACTION_PHONENO,
                labelWidth: 65,
                width: 200,
                vtype: 'regxMobileNo',
                id: 'r_txtActionPhone',
                name: 'r_txtActionPhone'
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: CONTACT_PHONENO,
                labelWidth: 65,
                layout: 'hbox',
                defaultType: 'textfield',
                items: [{
                    width: 60,
                    submitValue: false,
                    id: 'r_txtContactPhoneHead',
                    name: 'r_txtContactPhoneHead'
                }, {
                    xtype: 'displayfield',
                    value: '  -  '
                }, {
                    width: 120,
                    submitValue: false,
                    id: 'r_txtContactPhoneContent',
                    name: 'r_txtContactPhoneContent'
                }]
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: POSTNO,
                labelWidth: 65,
                id: 'r_fcZip',
                layout: 'hbox',
                defaultType: 'combobox',
                items: [{
                    editable: false,
                    labelWidth: 65,
                    width: 60,
                    store: r_CityStore,
                    displayField: 'middle',
                    valueField: 'middlecode',
                    queryMode: 'local',
                    id: 'r_cob_ccity',
                    name: 'r_combCity',
                    listeners: {
                        "select": function (combo, record) {
                            var z = Ext.getCmp("r_cob_czip");
                            z.clearValue();
                            r_ZipStore.removeAll();
                        },
                        beforequery: function (qe) {
                            r_CityStore.load();
                        }
                    }
                }, {
                    xtype: 'displayfield',
                    value: ' - '
                }, {
                    store: r_ZipStore,
                    editable: false,
                    width: 120,
                    queryMode: 'local',
                    style: { float: 'left' },
                    displayField: 'small',
                    valueField: 'zipcode',
                    id: 'r_cob_czip',
                    name: 'r_combZip',
                    listeners: {
                        beforequery: function (qe) {
                            if (Ext.getCmp('r_cob_ccity').getValue() == null) {
                                return;
                            }
                            delete qe.combo.lastQuery;
                            r_ZipStore.load({
                                params: {
                                    topValue: Ext.getCmp("r_cob_ccity").getValue()
                                }
                            });
                        }
                    }
                }]
            }, {
                xtype: 'combobox',
                id: 'r_combSuperMarket',
                name: 'r_combSuperMarket',
                store: r_superStore,
                displayField: 'store_name',
                valueField: 'store_id',
                queryMode: 'local',
                hidden: true,
                editable: false,
                labelWidth: 65,
                fieldLabel: SUPER_STORE,
                listeners: {
                    beforequery: function (qe) {
                        r_superStore.load({ params: { storeId: Ext.getCmp('combStoreMode').getValue() } });
                    },
                    select: function (comb, record) {
                        Ext.getCmp('r_txtContactAddress').setValue(record[0].data.address);
                    }
                }
            }, {
                fieldLabel: CONTACT_ADDRESS,
                labelWidth: 65,
                width: 300,
                submitValue: false,
                id: 'r_txtContactAddress',
                name: 'r_txtContactAddress'
            }]
        }]
    }, {
        xtype: 'panel',
        layout: 'hbox',
        defaultType: 'textarea',
        border: false,
        bodyPadding: '0 0 0 0 ',
        items: [{
            labelWidth: 65,
            width: 450,
            submitValue: false,
            id: 'txtareAdminNote',
            name: 'txtareAdminNote',
            style: { 'margin-right': '30px' },
            fieldLabel: ADMIN_NOTE
        }, {
            labelWidth: 65,
            width: 450,
            submitValue: false,
            id: 'txtareCartNote',
            name: 'txtareCartNote',
            fieldLabel: CART_NOTE
        }]
    }]
});

//訂單Model
Ext.define('GIGADE.ORDER', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'Product_Id', type: 'string' },
    { name: 'product_id', type: 'string' },
    { name: 'item_id', type: 'string' },
    { name: 'product_name', type: 'string' },
    { name: 'child', type: 'string' },
    { name: 'spec1', type: 'string' },
    { name: 'spec2', type: 'string' },
    { name: 'spec1_show', type: 'string' },
    { name: 'spec2_show', type: 'string' },
    { name: 'original_price', type: 'int' },
    { name: 'product_cost', type: 'int' }, //售價
    { name: 'Event_Item_Money', type: 'int' },//活動售價 add by zhuoqin0830w  2015/11/16
    { name: 'cost', type: 'int' },  //組合商品之成本
    { name: 'Event_Item_Cost', type: 'int' },//活動成本
    { name: 'Item_Cost', type: 'int' }, //單一商品之成本
    { name: 'stock', type: 'int' },
    { name: 'buynum', type: 'int' },
    { name: 'sumprice', type: 'int' },
    { name: 'empty', type: 'string' },
    { name: 'parent_id', type: 'string' },
    { name: 'empty', type: 'string' },
    { name: 's_must_buy', type: 'int' },
    { name: 'child_scale', type: 'string' },  //售價拆分比例
    { name: 'child_cost_scale', type: 'string' },   //成本拆分比例
    { name: 'combination', type: 'int' },
    { name: 'site_type', type: 'int' },
    { name: 'ignore_stock', type: 'int' }, //庫存為0時是否可以販賣的狀態
    { name: 'price_type', type: 'int' },
    { name: 'childCount', type: 'int' },        //組合商品下子商品數量
    { name: 'childSum', type: 'int' },          //組合下必選商品的必購數量之和
    { name: 'isAdd', type: 'string' },          //組合時是否為手動添加的行
    { name: 'price_master_id', type: 'int' },
    { name: "product_status_name", type: "string" }]
});

//訂單Store
var orderStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.ORDER',
    data: [{
        Product_Id: '',
        product_id: '',
        parent_id: '',
        item_id: '',
        product_name: '',
        child: '',
        spec1: '',
        spec2: '',
        spec1_show: '',
        spec2_show: '',
        original_price: '0',        //原始售价
        product_cost: '0',          //现售价
        Event_Item_Money: '0',//活動售價  add by zhuoqin0830w 2015/11/16
        cost: '0',
        Event_Item_Cost: '0', //活動成本
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        Item_Cost: '0',   //子商品的成本(规格同价取price_master表反之取item_price)
        stock: '0',
        buynum: '0',
        sumprice: '0',
        empty: '',
        s_must_buy: '0',
        child_scale: '',            //拆分比例
        child_cost_scale: '',
        combination: 0,
        buy_limit: 0,
        site_type: 0, //站臺類型 1:吉甲地  2:合作外站
        ignore_stock: 0,
        price_type: 0, //價格類型
        childCount: 0,
        childSum: 0,
        isAdd: 'false'
    }],
    listeners: {
        update: function (store, record) {
            if (record.data.product_id == " ") {
                return;
            }
            Ext.getCmp("txtTotalPrice").setValue(accountSum());
            //add by zhuoqin0830w  2015/07/31  計算需要返還的購物金
            Ext.getCmp("txtTotalAccbonus").setValue(accBonus());
            if (orderStore.getCount() > 0) {
                var numSum = 0;
                for (var i = 0, j = orderStore.getCount() ; i < j; i++) {
                    var data = orderStore.getAt(i);
                    if (data.get("buynum") != 0) {
                        numSum += parseInt(data.get("buynum"));    // 
                        numSum += parseInt(data.get("buynum") != 0);
                    }
                }
                if (numSum != 0) {
                    Ext.getCmp("btnSubmit").setDisabled(false);
                }
                else {
                    Ext.getCmp("btnSubmit").setDisabled(true);
                    Ext.getCmp("txtTotalPrice").setValue(accountSum());
                    //add by zhuoqin0830w  2015/07/31  計算需要返還的購物金
                    Ext.getCmp("txtTotalAccbonus").setValue(accBonus());
                }
            }
        }
    }
});

//添加Grid行
function addTr() {
    orderStore.add({
        product_id: '',
        item_id: '',
        product_name: '',
        child: '',
        spec1_show: '',
        spec2_show: '',
        product_cost: '0',
        Event_Item_Money: '0',//活動售價  add by zhuoqin0830w   2015/11/16
        Event_Item_Cost: '0',//活動成本
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        stock: '0',
        buynum: '0',
        sumprice: '0',
        empty: ''
    });
    cellEditing.startEditByPosition({ row: orderStore.getCount() - 1, column: 1 });
    currentRow = -1;
}

//任選組合=>添加行
function addComboSelTr(Product_Id, parent_id, rowIdx, combination, buy_limit, isAdd) {
    var childCount = orderStore.getAt(rowIdx).get('childCount');
    /****** 如果當前顯示的行數之和等於該組合下的子商品數,則不能再添加行 *****/
    if (orderStore.getAt(rowIdx + childCount)) {
        if (orderStore.getAt(rowIdx + childCount).get('parent_id') == orderStore.getAt(rowIdx).get('product_id')) {
            return;
        }
    }
    /**************************************************************/

    /********* 如果當前子商品的購買數量之和等於需選數量則不能再添加 *******/
    var g_must_buy = orderStore.getAt(rowIdx).data.g_must_buy;
    var childSum = orderStore.getAt(rowIdx).data.childSum;
    var index = rowIdx + 1;
    for (index; ; index++) {
        if (!orderStore.getAt(index) || orderStore.getAt(index).get('parent_id') != orderStore.getAt(rowIdx).get('product_id')) {
            break;
        }
        if (orderStore.getAt(index).get('isAdd') == 'true') {
            childSum += orderStore.getAt(index).get('s_must_buy');
        }
    }
    if (childSum >= g_must_buy) {
        return;
    }
    /*************************************************************/

    orderStore.insert(rowIdx + 1, {
        Product_Id: Product_Id, product_id: ' ',
        parent_id: parent_id,
        item_id: '',
        product_name: "",
        child: SELECT,
        spec1_show: '',
        spec2_show: '',
        original_price: '0',
        product_cost: '0',
        Event_Item_Money: '0',//活動售價  add by zhuoqin0830w  2015/11/16
        Event_Item_Cost: '0',//活動成本
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        stock: '0',
        buynum: '0',
        sumprice: '0',
        empty: '',
        combination: combination,
        buy_limit: buy_limit,
        isAdd: isAdd
    });
    // cellEditing.startEditByPosition({ row: orderStore.getCount() - 1, column: 1 });
}

//固定組合=>添加行
function addComboTr(data, parentData, rowIdx) {
    var combination = parentData.child;
    orderStore.insert(rowIdx + 1, {
        Product_Id: data.Product_Id,
        parent_id: data.parent_id,
        product_id: ' ',
        item_id: data.Item_Id,
        product_name: "",
        child: data.product_name,
        spec1_show: data.Spec_Name_1 == "" ? '' : data.Spec_Name_1,
        spec2_show: data.Spec_Name_2 == "" ? '' : data.Spec_Name_2,
        spec1: data.Spec_Id_1,
        spec2: data.Spec_Id_2,
        original_price: data.original_price,
        product_cost: data.product_cost,
        cost: data.cost,//add 2014/10/23
        Item_Cost: data.Item_Cost,
        Event_Item_Cost: data.Event_Item_Cost,//活動成本
        Event_Item_Money: data.Event_Item_Money,//活動售價  add by zhuoqin0830w  2015/11/16
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        stock: data.Item_Stock,
        s_must_buy: data.s_must_buy,
        buynum: '0',
        sumprice: '0',
        empty: '',
        combination: combination,
        price_type: data.price_type,
        price_master_id: data.price_master_id,
        ignore_stock: data.ignore_stock
    });
}

//任選必購子商品 ＝> 添加行
function addMustBuyTr(childData, parentData, rowIdx) {
    orderStore.insert(rowIdx + 1, {
        Product_Id: childData.Product_Id,
        parent_id: parentData.product_id,
        product_id: ' ',
        item_id: childData.Item_Id,
        product_name: "",
        child: childData.product_name,
        original_price: childData.original_price,
        Item_Cost: childData.Item_Cost,
        product_cost: childData.product_cost,
        cost: childData.cost,//add 2014/10/23
        Event_Item_Cost: childData.Event_Item_Cost,//活動成本
        Event_Item_Money: childData.Event_Item_Money,//活動售價  add  by zhuoqin0830w  2015/11/16
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        stock: childData.Item_Stock,
        s_must_buy: childData.s_must_buy,
        buynum: childData.s_must_buy,
        sumprice: '0',
        empty: '',
        combination: parentData.child,
        price_type: parentData.price_type,
        price_master_id: childData.price_master_id,
        isAdd: false,
        ignore_stock: childData.ignore_stock
    });
}

//合作外站=>添加行
function addComboCooperationTr(data, rowIdx) {
    orderStore.insert(rowIdx + 1, {
        Product_Id: data.product_id, product_id: ' ',
        item_id: data.item_id,
        product_name: "",
        child: data.product_name,
        spec1_show: data.spec1_show,
        spec2_show: data.spec2_show,
        product_cost: data.product_cost,
        cost: data.cost,//add 2014/10/23
        Event_Item_Cost: data.Event_Item_Cost,//活動成本
        Event_Item_Money: data.Event_Item_Money,//活動售價  add by zhuoqin0830w  2015/11/16
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        stock: data.stock,
        buynum: '0',
        sumprice: '0',
        empty: '',
        s_must_buy: data.s_must_buy,
        site_type: 2,
        price_type: data.price_type,
        price_master_id: data.price_master_id,
        ignore_stock: data.ignore_stock,
        parent_id: data.parent_id,
        Item_Cost: data.Item_Cost
    });
    // cellEditing.startEditByPosition({ row: orderStore.getCount() - 1, column: 1 });
}

//列編輯
var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1,
    listeners: {
        beforeedit: function (e) {
            currentCol = e.colIdx;
            currentRow = e.rowIdx;
            var data = orderGrid.getStore().data.getAt(e.rowIdx);
            if (e.colIdx == 1) {//賣場商品編號
                var channelValue = Ext.getCmp('combChannelId').getValue();
                if (channelValue == "" || channelValue == null) {
                    Ext.getCmp('combChannelId').markInvalid(OUTSITE_SELECT);
                    Ext.getCmp('combChannelId').focus();
                    return;
                }
                if (data.data.product_id == " ") {//不可編輯
                    return false;
                }
            }

            if (e.colIdx == 3) {//子商品名稱
                if (!data.data.child) {
                    return false;
                }
                var curData = orderGrid.getStore().data.getAt(e.rowIdx);
                var parent_id;
                var price_type;
                var pid = curData.get("Product_Id");
                var parentData = null;
                for (var i = currentRow; i >= 0; i--) {
                    if (orderStore.data.items[i].data.product_id != " ") {
                        parentData = orderStore.getAt(i).data;
                        parent_id = parentData.product_id;
                        price_type = parentData.price_type;
                        break;
                    }
                }

                //固定組合不需要選擇子商品
                if (data.data.combination == 2) {
                    return false;
                }
                //合作外站不需要選擇子商品
                if (data.data.site_type == 2) {
                    return false;
                }
                //任選組合必購子商品不可編輯
                if ((data.data.s_must_buy > 0 && data.data.isAdd == 'false') || parentData.buynum > 0) {
                    return false;
                }

                if (pid) {
                    var date = new Date();
                    Ext.Ajax.request({
                        url: '/Order/getChildren?t=' + date.getMilliseconds(),
                        method: 'POST',
                        params: {
                            'pid': parent_id,
                            'price_type': price_type
                        },
                        success: function (response, opts) {
                            var resText = eval("(" + response.responseText + ")");
                            var child_list = resText.data;

                            orderGrid.columns[3].getEditor().getStore().removeAll();
                            for (var a = 0; a < child_list.length; a++) {
                                orderGrid.columns[3].getEditor().getStore().add({
                                    Product_Name: child_list[a].Product_Name,
                                    Child_Id: child_list[a].Child_Id
                                });
                            }
                        },
                        failure: function (response) {
                        }
                    });
                }
            }

            if (e.colIdx == 4) {//規格一
                if (!data.data.spec1_show) {
                    return false;
                }
                if (storeType == CHANNEL_TYPE_COOPERATOR) {
                    return false;
                }
                //判斷商品中的parent_id是否為空  如果為空 表示商品為 單一商品 則 規格不可編輯  add by zhuoqin0830w  2015/07/14
                if (data.data.parent_id == "") {
                    return false;
                }
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    pid = orderGrid.getStore().data.getAt(e.rowIdx).get("Product_Id");
                }
                if (pid == "") {
                    orderGrid.columns[4].getEditor().getStore().removeAll();
                    orderGrid.columns[5].getEditor().getStore().removeAll();
                    return;
                }
                var date = new Date();
                Ext.Ajax.request({
                    url: '/Order/getSpec1?t=' + date.getMilliseconds(),
                    method: 'POST',
                    params: {
                        'pid': pid
                    },
                    success: function (response, opts) {
                        var resText = eval("(" + response.responseText + ")");
                        var spec1_list = resText.data;

                        orderGrid.columns[4].getEditor().getStore().removeAll();
                        for (var a = 0; a < spec1_list.length; a++) {
                            orderGrid.columns[4].getEditor().getStore().add({ spec_name: spec1_list[a].spec_name, spec_id: spec1_list[a].spec_id });
                        }
                    },
                    failure: function (response) {
                    }
                });
            }

            if (e.colIdx == 5) {//規格二
                if (!data.data.spec2_show) {
                    return false;
                }
                if (storeType == CHANNEL_TYPE_COOPERATOR) {
                    return false;
                }
                //判斷商品中的parent_id是否為空  如果為空 表示商品為 單一商品 則 規格不可編輯  add by zhuoqin0830w  2015/07/14
                if (data.data.parent_id == "") {
                    return false;
                }
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    pid = orderGrid.getStore().data.getAt(e.rowIdx).get("Product_Id");
                }
                if (pid == "") {
                    orderGrid.columns[4].getEditor().getStore().removeAll();
                    orderGrid.columns[5].getEditor().getStore().removeAll();
                    return;
                }
                var date = new Date();
                Ext.Ajax.request({
                    url: '/Order/getSpec2?t=' + date.getMilliseconds(),
                    method: 'POST',
                    params: {
                        'pid': pid
                    },
                    success: function (response, opts) {
                        var resText = eval("(" + response.responseText + ")");
                        var spec2_list = resText.data;
                        orderGrid.columns[5].getEditor().getStore().removeAll();
                        for (var a = 0; a < spec2_list.length; a++) {
                            orderGrid.columns[5].getEditor().getStore().add({ spec_name: spec2_list[a].spec_name, spec_id: spec2_list[a].spec_id });
                        }
                    },
                    failure: function (response) {
                    }
                });
            }

            if (e.colIdx == 6) {//定價
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    return false;
                }
                var parentIdx = findParentIdx(e.rowIdx);
                var parentData = orderStore.getAt(parentIdx);
                if (parentData.data.buynum > 0) {
                    return false;
                }
            }

            if (e.colIdx == 7) {//成本
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    return false;
                }
                var parentIdx = findParentIdx(e.rowIdx);
                var parentData = orderStore.getAt(parentIdx);
                if (parentData.data.buynum > 0) {
                    return false;
                }
            }

            if (e.colIdx == 8) {//活動售價
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    return false;
                }
                var parentIdx = findParentIdx(e.rowIdx);
                var parentData = orderStore.getAt(parentIdx);
                if (parentData.data.buynum > 0) {
                    return false;
                }
            }

            if (e.colIdx == 9) {//活動成本
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    return false;
                }
                // add by zhuoqin0830w  2015/04/30  判斷 數量是否大於 0  如果大於0 則不能更改
                var parentIdx = findParentIdx(e.rowIdx);
                var parentData = orderStore.getAt(parentIdx);
                if (parentData.data.buynum > 0) {
                    return false;
                }
            }

            if (e.colIdx == 11) {//購物金金額
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    return false;
                }
                //判斷 抵用券金額  是否輸入   add by zhuoqin0830w  2015/09/08
                for (var i = 0; i < orderGrid.getStore().data.length; i++) {
                    if (orderGrid.getStore().data.getAt(i).data.Deduct_Welfare > 0) {
                        Ext.Msg.alert(INFORMATION, ONLYONE_BONUS_OR_WELFARE);
                        return false;
                    }
                }
            }

            if (e.colIdx == 12) {//抵用卷金額
                var pid = orderGrid.getStore().data.getAt(e.rowIdx).get("product_id");
                if (pid == " ") {
                    return false;
                }
                //判斷 購物金金額  是否輸入  add by zhuoqin0830w  2015/09/08
                for (var i = 0; i < orderGrid.getStore().data.length; i++) {
                    if (orderGrid.getStore().data.getAt(i).data.Deduct_Bonus > 0) {
                        Ext.Msg.alert(INFORMATION, ONLYONE_BONUS_OR_WELFARE);
                        return false;
                    }
                }
            }

            if (e.colIdx == 10) {//數量
                var curData = orderGrid.getStore().data.getAt(e.rowIdx);
                var pid = curData.get("product_id");
                var combination = curData.get("combination");
                var buy_limit = curData.get("buy_limit");
                var parentIdx = findParentIdx(e.rowIdx);
                var childIdx = parentIdx + 1;
                var parentData = orderStore.getAt(parentIdx);
                var ChileSum = 0;
                var IgnoreSum = 0;
                //查詢下面的子商品是否庫存為0時都可以販賣,諾都可以就不用限定庫存
                for (childIdx; ; childIdx++) {
                    var curData = orderStore.getAt(childIdx);
                    if (!curData || curData.get('parent_id') != parentData.get('product_id')) {
                        break;
                    }
                    ChileSum++;
                    if (curData.get("ignore_stock") == 1) {
                        IgnoreSum++;
                    }
                }
                if (ChileSum == 0) {
                    if (parentData.get("ignore_stock") == 1) {
                        ChileSum = 1;
                        IgnoreSum = 1;
                    }
                    else {
                        IgnoreSum = 1;
                    }
                }
                //任選無規格不限定一單位時,組合商品的購買數量為0時  可以編輯子商品購買數量
                if (pid == " " && (combination != 3 || buy_limit != 0 || parentData.data.buynum > 0)) {
                    return false;
                }
                //                var stock = data.get("stock");
                //                if (ChileSum != IgnoreSum) {
                //                    if (stock == 0) {
                //                        e.column.getEditor ().setDisabled(true);
                //                    }
                //                    else {
                //                        e.column.getEditor().setDisabled(false)
                //                    }
                //                }
                //                else {
                //                    e.column.getEditor().maxValue = Number.MAX_VALUE;
                //                }
            }
        },
        edit: function (editor, e) {
            var data = orderStore.getAt(e.rowIdx);
            var cost_total = parseFloat(data.get("product_cost"));
            var event_item_money_total = parseFloat(data.get("Event_Item_Money"));//add by zhuoqin0830w 2015/12/02 獲取活動價
            if (e.colIdx == 1) {//商品編號
                for (var i = e.rowIdx + 1; i <= orderStore.data.length; i) {
                    if (!orderStore.data.items[i]) {
                        break;
                    }
                    else {
                        if (orderStore.data.items[i].data.product_id == " ") {
                            orderStore.removeAt(i);
                        }
                        else {
                            break;
                        }
                    }
                }
                removeStore(e.rowIdx);
                orderGrid.columns[4].getEditor().getStore().removeAll();
                orderGrid.columns[5].getEditor().getStore().removeAll();
                data.set("spec1_show", "");
                data.set("spec2_show", "");
                var channelValue = Ext.getCmp('combChannelId').getValue();
                //添加設置站台的條件  edit by zhuoqin0830w  2015/11/16
                var Site_Id = Ext.getCmp("Site_Id").getValue();
                var combOrderDate = Ext.getCmp("combOrderDate").getValue();
                if ((channelValue == "" || channelValue == null) || (Site_Id == "" || Site_Id == null) || (combOrderDate == "" || combOrderDate == null)) {
                    Ext.Msg.alert(INFORMATION, SET_CHANNEL_AND_SITE_AND_ORDER_DATE);
                    return;
                }
                searchByProID(e.value, e.rowIdx, null, null, e, null, Site_Id, combOrderDate);
            }

            if (e.colIdx == 11) {//購物金
                //判斷輸入 的 購物金金額 是否 大於 商品定價
                for (var i = 0; i < orderStore.data.length; i++) {
                    var cost = orderStore.getAt(i).get("product_cost");
                    var bonus = orderStore.getAt(i).get("Deduct_Bonus");
                    if (bonus == null || bonus == "") {
                        orderStore.getAt(i).set("Deduct_Bonus", "0");
                        return;
                    }
                    if (bonus > cost) {
                        Ext.Msg.alert(INFORMATION, DEDUCT_BONUS + BOUNS_OR_WELFARE_IS_NO_PRICE);
                        orderStore.getAt(i).set("Deduct_Bonus", "0");
                    }
                }
            }

            if (e.colIdx == 12) {//抵用券
                //判斷輸入 的 抵用券金額 是否 大於 商品定價
                for (var i = 0; i < orderStore.data.length; i++) {
                    var cost = orderStore.getAt(i).get("product_cost");
                    var welfare = orderStore.getAt(i).get("Deduct_Welfare");
                    if (welfare == null || welfare == "") {
                        orderStore.getAt(i).set("Deduct_Welfare", "0");
                        return;
                    }
                    if (welfare > cost) {
                        Ext.Msg.alert(INFORMATION, DEDUCT_WELFARE + BOUNS_OR_WELFARE_IS_NO_PRICE);
                        orderStore.getAt(i).set("Deduct_Welfare", "0");
                    }
                }
            }

            if (e.colIdx == 10 || e.colIdx == 6) {
                var data = orderStore.getAt(e.rowIdx);

                //返還購物金  add by zhuoqin0830w  2015/08/03
                var bonus = Math.round((data.get("product_cost") - data.get("Item_Cost")) * 0.08);
                if (bonus > 0) {
                    bonus = bonus < 5 ? parseInt(5 * data.get("buynum")) : parseInt(bonus * data.get("buynum"));
                } else { bonus = 0; }
                data.set("accumulated_bonus", bonus);
                //父商品下子商品的返還金
                var s_must_buy = data.get("s_must_buy");
                var child_scale = data.get("child_scale");
                if (child_scale) {
                    var acc_bonus = bonus;
                    var child_scale_list = child_scale.split(',');
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(e.rowIdx + i);
                        var bonus = Math.round(acc_bonus * child_scale_list[i - 1]);
                        acc_bonus -= bonus;
                        current_child.set("accumulated_bonus", bonus);
                    }
                }

                //任選無規格不限定一單位時編輯子商品購買數量
                if (data.get('combination') == 3 && data.get('buy_limit') == 0) {
                    //此操作只針對數量的變動,價格的變動不需要在此處理
                    if (e.colIdx == 6) {
                        return;
                    }
                    //編輯組合商品數量時
                    if (!data.get('parent_id')) {
                        var index = e.rowIdx + 1;
                        var totalPrice = 0;
                        var parentBuyNum = data.get("buynum");
                        var totalMustBuyNum = 0;
                        var childNumIsNull = false;
                        for (index; ; index++) {
                            var curData = orderStore.getAt(index);
                            if (!curData || curData.get('parent_id') != data.get('product_id')) {
                                break;
                            }
                            if (curData.get('buynum') == 0) {
                                childNumIsNull = true;
                                break;
                            }
                            totalMustBuyNum += curData.get('s_must_buy');
                        }
                        if (childNumIsNull) {
                            Ext.Msg.alert(INFORMATION, CHILD_PRODUCT_BUYNUM_ISNULL);
                            data.set('buynum', 0);
                            return false;
                        }
                        if (totalMustBuyNum != data.get('g_must_buy')) {
                            Ext.Msg.alert(INFORMATION, GROUP_PRODUCT_BUYNUM + data.get('g_must_buy') + '<br/>' + CHILD_PRODUCT_BUYNUM + totalMustBuyNum + PLEASE_CHOOSE_AGAIN);
                            data.set('buynum', 0);
                            return false;
                        }
                        index = e.rowIdx + 1;
                        for (index; ; index++) {
                            var curData = orderStore.getAt(index);
                            if (!curData || curData.get('parent_id') != data.get('product_id')) {
                                break;
                            }
                            if (parentBuyNum == 0) {
                                parentBuyNum = 1;
                            }
                            var childBuyNum = curData.get("s_must_buy") * parentBuyNum;
                            //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                            var event_item_money = curData.get('Event_Item_Money');
                            var cost = curData.get('product_cost');
                            var curSum = event_item_money == 0 ? cost * childBuyNum : event_item_money * childBuyNum;
                            curData.set("buynum", childBuyNum);
                            curData.set("sumprice", curSum);
                            totalPrice += curSum;
                        }
                        if (data.get("buynum") == 0 || data.get("buynum") == 1) {
                            //data.set('product_cost', totalPrice);
                            data.set("sumprice", data.get("buynum") == 0 ? 0 : totalPrice);
                        }
                        else {
                            data.set("sumprice", totalPrice);
                        }
                    }
                    else {
                        //非指定任選 當組合商品的購買數量為1時 編輯子商品數量
                        var parentIdx = findParentIdx(e.rowIdx);
                        var childIdx = parentIdx + 1;
                        var parentData = orderStore.getAt(parentIdx);
                        var totalPrice = 0;
                        var totalMustBuyNum = 0;
                        var event_item_money = curData.get('Event_Item_Money');//add by zhuoqin0830w 2015/12/02 獲取活動價
                        var cost = curData.get('product_cost');//add by zhuoqin0830w 2015/12/02 獲取定價
                        for (childIdx; ; childIdx++) {
                            var curData = orderStore.getAt(childIdx);
                            if (!curData || curData.get('parent_id') != parentData.get('product_id')) {
                                break;
                            }
                            if (childIdx == e.rowIdx) {
                                continue;
                            }
                            totalMustBuyNum += curData.get('s_must_buy');
                            //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                            var curSumPrice = event_item_money == 0 ? cost * curData.get('s_must_buy') : event_item_money * curData.get('s_must_buy');
                            totalPrice += curSumPrice;
                            curData.set('sumprice', curSumPrice);
                        }
                        totalMustBuyNum += e.value;
                        if (totalMustBuyNum > parentData.get('g_must_buy')) {
                            Ext.Msg.alert(INFORMATION, GROUP_PRODUCT_BUYNUM + parentData.get('g_must_buy') + ', <br/>' + CHILD_PRODUCT_BUYNUM + totalMustBuyNum + PLEASE_CHOOSE_AGAIN);
                            e.value = 1;
                        }
                        data.set("s_must_buy", e.value);
                        data.set("buynum", e.value);
                        //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                        data.set("sumprice", event_item_money == 0 ? cost * curData.get('buynum') : event_item_money * curData.get('buynum'));
                        //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                        totalPrice += event_item_money == 0 ? cost * curData.get('buynum') : event_item_money * curData.get('buynum');
                        if (parentData.get('price_type') == 2) {
                            parentData.set("product_cost", totalPrice);
                        }
                    }
                }
                else {
                    var totalPrice = 0;
                    //計算子商品購買數量及價格
                    if (data.get("g_must_buy")) {
                        for (var i = 1; i <= data.get("g_must_buy") ; i++) {
                            var current = orderStore.getAt(e.rowIdx + i);
                            var childBuyNum = current.get("s_must_buy") * data.get("buynum");
                            var cost = current.get("product_cost");//add by zhuoqin0830w 2015/12/02 獲取定價
                            var event_item_money = current.get("Event_Item_Money");//add by zhuoqin0830w 2015/12/02 獲取活動價
                            current.set("buynum", childBuyNum);
                            //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                            current.set("sumprice", event_item_money == 0 ? cost * childBuyNum : event_item_money * childBuyNum);
                            totalPrice += event_item_money == 0 ? cost * childBuyNum : event_item_money * childBuyNum;
                        }
                    }
                    //edit by xinglu0624w  當前組合商品價格類型為按比例拆分或者當前商品為單一商品時，總價 ＝ 當前商品之定價 * 當前商品數量
                    if (data.get('price_type') == 1 || data.get('price_type') == 0) {
                        //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                        data.set("sumprice", event_item_money_total == 0 ? parseInt(data.get("buynum")) * cost_total : parseInt(data.get("buynum")) * event_item_money_total);
                    }
                    else if (data.get('price_type') == 2) {
                        data.set("sumprice", totalPrice);
                    }
                }
            }
        },
        validateedit: function (editor, e) { }
    }
});

var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
    clicksToMoveEditor: 1,
    autoCancel: false
});

function findParentIdx(index) {
    var parentIdx = 0;
    for (index; ; index--) {
        var curData = orderStore.getAt(index).data;
        if (!curData.parent_id) {
            parentIdx = index;
            break;
        }
    }
    return parentIdx;
}

/****************  驗證（Vtype）  *******************/
//商品編號
Ext.apply(Ext.form.field.VTypes, {
    regxPNo: function (val, field) {
        return /^\d{5,}$/.test(val)//5
    },
    regxPNoText: FOURNUMBER_INPUT
});

//價格
Ext.apply(Ext.form.field.VTypes, {
    regxMoney: function (val, field) {
        return /^[1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|[0-9]$/.test
(val);
    },
    regxMoneyText: FORMAT_ERROR
});

//行動電話
Ext.apply(Ext.form.field.VTypes, {
    regxMobileNo: function (val, field) {
        return /^09\d{8}$/.test(val)
    },
    regxMobileNoText: PHONENO_ERROR
});

//會員編號  add by zhuoqin0830w  2015/11/12
Ext.apply(Ext.form.field.VTypes, {
    regxUserID: function (val, field) {
        return /^[1-9]\d*$/.test(val);
    },
    regxUserIDText: FORMAT_ERROR
});

/**************************************************/

//計算總價
function accountSum() {
    var freight = 0;
    if (Ext.getCmp('txtLowFreight').getValue() != null) {
        freight = parseFloat(Ext.getCmp('txtLowFreight').getValue());
    }
    if (Ext.getCmp('txtNormalFreight').getValue() != null) {
        freight += parseFloat(Ext.getCmp('txtNormalFreight').getValue());
    }
    var totalPrice = parseFloat(freight);
    for (var i = 0, j = orderStore.getCount() ; i < j; i++) {
        if (orderStore.getAt(i).get("product_id") != " ") {
            // 使 總價 減去 購物金 和 抵用金  edit by zhuoqin0830w  2015/05/14
            totalPrice += parseFloat(orderStore.getAt(i).get("sumprice") - orderStore.getAt(i).get('Deduct_Welfare') - orderStore.getAt(i).get('Deduct_Bonus'));
        }
    }
    //判斷總價是否為負數 如果 為 負數 則不能夠進行 保存  add by zhuoqin0830w  2015/05/12
    if (totalPrice < 0) {
        Ext.getCmp('btnSubmit').setDisabled(true);
    } else {
        Ext.getCmp('btnSubmit').setDisabled(false);
    }
    return totalPrice;
}

//add by zhuoqin0830w  2015/07/31  計算需要返還的購物金
function accBonus() {
    var bonus = 0;
    for (var i = 0, j = orderStore.getCount() ; i < j; i++) {
        if (orderStore.getAt(i).get("product_id") != " ") {
            var price = parseFloat(orderStore.getAt(i).get("product_cost"));
            var cost = parseFloat(orderStore.getAt(i).get("Item_Cost"));
            var buynum = parseInt(orderStore.getAt(i).get("buynum"));

            if (buynum == 0) {
                bonus += 0;
            } else {
                var accBonus = Math.round(((price - cost) * 0.08));
                //判斷計算后的返還金是否大於0 如果大於0則進行判斷計算 如果不是 則直接返還0  edit by zhuoqin0830w  2015/08/20
                if (accBonus > 0) {
                    bonus += accBonus < 5 ? parseInt(5 * buynum) : parseInt(accBonus * buynum);
                } else {
                    bonus += 0;
                }
            }
        }
    }
    return bonus;
}

//刪除Store指定行
function removeStore(rowIdx) {
    var data = orderStore.getAt(rowIdx);
    data.set('product_id', '');
    data.set('item_id', '');
    data.set('product_name', '');
    data.set('spec1', '');
    data.set('spec2', '');
    data.set('spec1_show', '');
    data.set('spec2_show', '');
    data.set('Item_Cost', '0');
    data.set('product_cost', '0');
    data.set('Event_Item_Money', '0');//活動售價  add by zhuoqin0830w 2015/11/16
    data.set('Event_Item_Cost', '0');//活動成本
    data.set('Deduct_Welfare', '0');//購物金金額
    data.set('Deduct_Bonus', '0');//抵用卷金額
    data.set('stock', '0');
    data.set('buynum', '0');
    data.set('sumprice', '0');
    data.set('product_status_name', '');
}

//清空
function clearCmp() {
    Ext.getCmp('txtLowFreight').setValue("");
    Ext.getCmp('txtNormalFreight').setValue("");
    Ext.getCmp("txtTotalPrice").setValue("0");

    //添加清空返還購物金欄位  add by zhuoqin0830w  2015/07/31
    Ext.getCmp("txtTotalAccbonus").setValue("0");
}

//訂單生成成功後的操作
function afterSuccess() {
    orderStore.removeAll();
    ZipStore.removeAll();
    r_ZipStore.removeAll();

    myPanel.getForm().reset();
    clearCmp();
    $("#chkSame").removeAttr("checked");
    $("#chkSameReceiver").removeAttr("checked");
    Ext.getCmp("btnSubmit").setDisabled(true);
    orderStore.add({
        product_id: '',
        item_id: '',
        product_name: '',
        spec1_show: '',
        spec2_show: '',
        product_cost: '0',
        Event_Item_Money: '0',//活動售價  add by zhuoqin0830w   2015/11/16
        Event_Item_Cost: '0',//活動成本
        Deduct_Welfare: '0',//購物金金額
        Deduct_Bonus: '0',//抵用卷金額
        stock: '0',
        buynum: '0',
        sumprice: '0',
        empty: ''
    });

    Ext.getCmp('b_fcZip').show();
    Ext.getCmp('r_fcZip').show();
    Ext.getCmp('b_combSuperMarket').hide();
    Ext.getCmp('r_combSuperMarket').hide();
}

//商品Grid
var orderGrid = Ext.create('Ext.grid.Panel', {
    id: 'orderGrid',
    enableColumnMove: false,
    store: orderStore,
    plugins: [cellEditing],
    width: docWidth,
    minHeight: 300,
    height: docHeight - 450,
    tbar: {
        bodyStyle: 'padding:2px 5px;',
        items: [{
            xtype: 'button',
            text: BTN_ADD,
            border: true,
            iconCls: 'icon-add',
            width: 80,
            handler: function () {
                addTr();
            }
        }]
    },
    dockedItems: [{
        xtype: 'panel',
        dock: 'bottom',
        layout: {
            type: 'table',
            columns: 2
        },
        height: 115,
        border: false,
        frame: true,
        bodyStyle: 'padding:5px',
        defaultType: 'textfield',
        items: [{
            fieldLabel: NORMAL_FREIGHT,//常溫運費
            xtype: 'numberfield',
            id: 'txtNormalFreight',
            labelWidth: 65,
            vtype: 'regxMoney',
            height: 20,
            width: 150,
            minValue: 0,
            style: { 'margin-right': '10px' },
            listeners: {
                change: function () {
                    Ext.getCmp("txtTotalPrice").setValue(accountSum());
                }
            }
        }, {
            fieldLabel: LOW_FREIGHT,//低溫運費
            xtype: 'numberfield',
            id: 'txtLowFreight',
            vtype: 'regxMoney',
            labelWidth: 65,
            height: 20,
            width: 150,
            minValue: 0,
            style: { 'margin-right': '10px' },
            listeners: {
                change: function () {
                    Ext.getCmp("txtTotalPrice").setValue(accountSum());
                }
            }
        }, {
            xtype: 'displayfield',
            fieldLabel: TOTAL_PRICE,
            id: 'txtTotalPrice',
            labelWidth: 65,
            height: 20,
            width: 150
        }, {//add by zhuoqin0830w  2015/07/31  添加返還購物金欄位
            xtype: 'displayfield',
            hidden: true,
            fieldLabel: ACCUMULATED_BONUS,
            id: 'txtTotalAccbonus',
            labelWidth: 65,
            height: 20,
            width: 150
        }, {
            xtype: 'button',
            id: 'btnSubmit',
            text: BTN_SURE,
            disabled: true,
            hidden: true,
            style: { 'margin-left': 70 },
            width: 80,
            listeners: {
                click: function () {
                    var existNum = 0;
                    var isSubmit = true;
                    var isComplete = true;
                    var tempStr = "";
                    var index = 1;
                    for (var i = 0, j = orderStore.getCount() ; i < j; i++) {
                        var data = orderStore.getAt(i);
                        if (!data.get("product_id")) {
                            isComplete = false;
                            break;
                        }
                        if (data.get("buynum") == 0 && data.get("product_id") != "" && !data.get('parent_id')) {
                            tempStr += data.get('product_id') + '：' + data.get('product_name') + ' ' + data.get('spec1_show') + ' ' + data.get('spec2_show') + '\n';
                            index++;
                            isSubmit = false;
                        }
                        else {//判斷無規格任選商品子商品是否選擇
                            if (data.get("child") == SELECT) {
                                isSubmit = false;
                            }
                        }
                    }
                    if (!isComplete) {
                        Ext.Msg.alert(INFORMATION, COMPLETE_ORDER);
                        return;
                    }
                    if (!isSubmit) {
                        Ext.Msg.alert(INFORMATION, PRODUCT + '\n' + tempStr + '\n' + PRODUCT_BUYNUM_ISNULL);
                        return;
                    }
                    var form = myPanel.getForm();
                    if (form.isValid()) {
                        var isRight = true;
                        if (storeType == CHANNEL_TYPE_COOPERATOR && Ext.getCmp('txtTradeNumber').getValue() == '') {
                            Ext.getCmp('txtTradeNumber').markInvalid(VALUE_EMPTY);
                            isRight = false;
                        }
                        if (isSuper) {
                            if (Ext.getCmp('r_combSuperMarket').getValue() == null || Ext.getCmp('r_combSuperMarket').getValue() == '') {
                                Ext.getCmp('r_combSuperMarket').markInvalid(VALUE_EMPTY);
                                isRight = false;
                            }
                            if (Ext.getCmp('b_combSuperMarket').getValue() == null || Ext.getCmp('b_combSuperMarket').getValue() == '') {
                                Ext.getCmp('b_combSuperMarket').markInvalid(VALUE_EMPTY);
                                isRight = false;
                            }
                        }
                        else {
                            if (Ext.getCmp('cob_ccity').getValue() == null || Ext.getCmp('cob_ccity').getValue() == '') {
                                Ext.getCmp('cob_ccity').markInvalid(VALUE_EMPTY);
                                isRight = false;
                            }
                            if (Ext.getCmp('cob_czip').getValue() == null || Ext.getCmp('cob_czip').getValue() == '') {
                                Ext.getCmp('cob_czip').markInvalid(VALUE_EMPTY);
                                isRight = false;
                            }
                            if (Ext.getCmp('r_cob_ccity').getValue() == null || Ext.getCmp('r_cob_ccity').getValue() == '') {
                                Ext.getCmp('r_cob_ccity').markInvalid(VALUE_EMPTY);
                                isRight = false;
                            }
                            if (Ext.getCmp('r_cob_czip').getValue() == null || Ext.getCmp('r_cob_czip').getValue() == '') {
                                Ext.getCmp('r_cob_czip').markInvalid(VALUE_EMPTY);
                                isRight = false;
                            }
                        }
                        if (Ext.getCmp('b_txtAddress').getValue() == '') {
                            Ext.getCmp('b_txtAddress').markInvalid(VALUE_EMPTY);
                            isRight = false;
                        }
                        if (Ext.getCmp('r_txtContactAddress').getValue() == '') {
                            Ext.getCmp('r_txtContactAddress').markInvalid(VALUE_EMPTY);
                            isRight = false;
                        }
                        if (!isRight) {
                            return;
                        }
                        var normalFreight = Ext.getCmp("txtNormalFreight");
                        var lowFreight = Ext.getCmp("txtLowFreight");
                        var mormal, low;
                        if (normalFreight.getValue() == null) {
                            mormal = -1;
                        } if (lowFreight.getValue() == null) {
                            low = -1
                        }
                        if (mormal < 0 && low < 0) {
                            Ext.Msg.alert(INFORMATION, SET_FREIGHT);
                            return;
                        }
                        if (!normalFreight.isValid() || !lowFreight.isValid()) {
                            return;
                        }
                        var productTitle;
                        if (storeType == 1) {
                            productTitle = 'coop_product_id';
                        }
                        else {
                            productTitle = 'product_id';
                        }
                        var parent_group_id = 0;
                        var gridData = "[";
                        for (var i = 0, j = orderStore.getCount() ; i < j; i++) {
                            var data = orderStore.getAt(i);
                            var parent_id;
                            if (data.get("product_id") == " ") {
                                //組合子商品
                                existNum += parseInt(data.get("buynum"));
                                gridData += "{" + productTitle + ":'" + data.get('Product_Id') + "',item_id:" + data.get('item_id') + ",buynum:" + data.get('buynum');
                                //add by zhuoqin0830w  2015/07/31  向 後臺傳遞返還購物金的值  添加活動售價  ',Event_Item_Money:' + data.get('Event_Item_Money')  add by zhuoqin0830w  2015/11/16
                                gridData += ",accumulated_bonus:" + data.get('accumulated_bonus') + ',Event_Item_Money:' + data.get('Event_Item_Money');
                                //添加 活動成本 金額顯示  ",Event_Item_Cost:" + data.get('Event_Item_Cost')  edit by zhuoqin0830w  2015/04/30
                                gridData += ",Item_Cost:'" + data.get('Item_Cost') + "',product_cost:" + data.get('product_cost') + ",Event_Item_Cost:" + data.get('Event_Item_Cost') + ",sumprice:" + data.get('sumprice');
                                //添加 購物金 和 抵用金 顯示   ",Deduct_Bonus:" + data.get('Deduct_Bonus') + ",Deduct_Welfare:" +   data.get('Deduct_Welfare')  edit by zhuoqin0830w  2015/05/14
                                gridData += ",parent_id:'" + parent_id + "',product_name:'" + Ext.htmlEncode(data.get('child')) + "',deduct_bonus:" + data.get('Deduct_Bonus') + ",deduct_welfare:" + data.get('Deduct_Welfare') + ",s_must_buy:" + data.get('s_must_buy') + ",group_id:" + parent_group_id;
                                gridData += ",price_master_id:" + data.get('price_master_id') + ",ignore_stock:" + data.get('ignore_stock') + "}";
                            }
                            else {
                                existNum += parseInt(data.get("buynum"));
                                parent_id = data.get('product_id');
                                if (data.get("item_id") != 0) {
                                    //單一商品
                                    gridData += "{" + productTitle + ":'" + data.get('product_id') + "',item_id:" + data.get('item_id') + ",buynum:" + data.get('buynum');
                                    //add by zhuoqin0830w  2015/07/31 向後臺傳遞返還購物金的值   添加活動售價  ',Event_Item_Money:' + data.get('Event_Item_Money')  add by zhuoqin0830w  2015/11/16
                                    gridData += ",accumulated_bonus:" + data.get('accumulated_bonus') + ',Event_Item_Money:' + data.get('Event_Item_Money');
                                    //添加 活動成本 金額顯示 ",Event_Item_Cost:" + data.get('Event_Item_Cost')  zhuoqin0830w  2015/04/30
                                    gridData += ",product_cost:" + data.get('product_cost') + ",Item_Cost:" + data.get('Item_Cost') + ",Event_Item_Cost:" + data.get('Event_Item_Cost') + ",sumprice:" + data.get('sumprice');
                                    //添加 購物金 和 抵用金 顯示  ",Deduct_Bonus:" + data.get('Deduct_Bonus') + ",Deduct_Welfare:" +  data.get('Deduct_Welfare')  edit by zhuoqin0830w  2015/05/14
                                    gridData += ",parent_id:0,product_name:'" + Ext.htmlEncode(data.get('product_name')) + "',deduct_bonus:" + data.get('Deduct_Bonus') + ",deduct_welfare:" + data.get('Deduct_Welfare') + ",ignore_stock:" + data.get('ignore_stock') + "}";//edit by jiajun
                                }
                                else {
                                    //組合父級商品
                                    gridData += "{" + productTitle + ":'" + data.get('product_id') + "',item_id:" + data.get('item_id');
                                    //add by zhuoqin0830w  2015/07/31 向後臺傳遞返還購物金的值     添加活動售價  ',Event_Item_Money:' + data.get('Event_Item_Money')  add by zhuoqin0830w  2015/11/16
                                    gridData += ",accumulated_bonus:" + data.get('accumulated_bonus') + ',Event_Item_Money:' + data.get('Event_Item_Money');
                                    //添加 活動成本 金額顯示  ",Event_Item_Cost:" + data.get('Event_Item_Cost')  zhuoqin0830w   2015/04/30
                                    gridData += ",buynum:" + data.get('buynum') + ",Item_Cost:'" + data.get('Item_Cost') + "',product_cost:" + data.get('product_cost') + ",Event_Item_Cost:" + data.get('Event_Item_Cost') + ",sumprice:" + data.get('sumprice');
                                    //添加 購物金 和 抵用金 顯示   ",Deduct_Bonus:" + data.get('Deduct_Bonus') + ",Deduct_Welfare:" + data.get('Deduct_Welfare')  edit by zhuoqin0830w  2015/05/14
                                    gridData += ",parent_id:0,product_name:'" + Ext.htmlEncode(data.get('product_name')) + "',group_id:" + (i + 1) + ",deduct_bonus:" + data.get('Deduct_Bonus') + ",deduct_welfare:" + data.get('Deduct_Welfare') + ",price_type:" + data.get('price_type') + "}";
                                    parent_group_id = (i + 1);
                                }
                            }
                        }
                        var re = /}{/g;
                        gridData = gridData.replace(re, "},{");
                        gridData += "]";
                        if (existNum == 0) {
                            Ext.Msg.alert(INFORMATION, NOTHING_ORDER);
                            return;
                        }
                        //edit by zhuoqin0830w 2015/08/26
                        if (Ext.getCmp('txtTotalPrice').getValue() >= 0) {
                            Ext.getCmp('btnSubmit').setDisabled(true);
                        } else {
                            Ext.Msg.alert(INFORMATION, TOTAL_PRICE_ISNULL);
                            return;
                        }
                        if (!myMask) {
                            myMask = new Ext.LoadMask(Ext.getBody(), { msg: PLEASE_WAIT });
                        }
                        myMask.show();
                        var ssex = Ext.getCmp("g_checkActionSex").getValue().g_checkActionSex;
                        var gsex = Ext.getCmp("s_checkActionSex").getValue().s_checkActionSex;
                        form.submit({
                            params: {
                                //combStoreMode: Ext.htmlEncode(Ext.getCmp('combStoreMode').getValue()),
                                //combStoreMode: ship_logistics,
                                retrieve_mode: retrieve_mode,
                                normalFright: Ext.htmlEncode(Ext.getCmp('txtNormalFreight').getValue()),
                                lowFright: Ext.htmlEncode(Ext.getCmp('txtLowFreight').getValue()),
                                totalPrice: Ext.htmlEncode(Ext.getCmp('txtTotalPrice').getValue()),
                                storeType: storeType,
                                receipt_to: receipt_to,
                                gridData: Ext.htmlEncode(gridData),
                                txtareAdminNote: Ext.htmlEncode(Ext.getCmp('txtareAdminNote').getValue()),
                                txtareCartNote: Ext.htmlEncode(Ext.getCmp('txtareCartNote').getValue()),
                                txtTradeNumber: Ext.htmlEncode(Ext.getCmp('txtTradeNumber').getValue()),
                                txtOrderId: Ext.htmlEncode(Ext.getCmp('txtOrderId').getValue()),
                                txtDeliverNumber: Ext.htmlEncode(Ext.getCmp('txtDeliverNumber').getValue()),

                                b_txtName: Ext.htmlEncode(Ext.getCmp('txtCNFullName').getRawValue()),
                                userId: Ext.htmlEncode(Ext.getCmp('d_userID').getValue()),//內部訂單使用訂購人的user_id edit by xiangwang0413w 2014/10/29
                                b_txtPhoneHead: Ext.htmlEncode(Ext.getCmp('txtContactPhoneHead').getValue()),
                                b_txtPhoneContent: Ext.htmlEncode(Ext.getCmp('txtContactPhoneContent').getValue()),
                                b_txtAddress: Ext.htmlEncode(Ext.getCmp('b_txtAddress').getValue()),
                                r_txtCNFullName: Ext.htmlEncode(Ext.getCmp('r_txtCNFullName').getValue()),
                                r_txtContactPhoneHead: Ext.htmlEncode(Ext.getCmp('r_txtContactPhoneHead').getValue()),
                                r_txtContactPhoneContent: Ext.htmlEncode(Ext.getCmp('r_txtContactPhoneContent').getValue()),
                                r_txtContactAddress: Ext.htmlEncode(Ext.getCmp('r_txtContactAddress').getValue()),

                                //add by wwei0216w 2104/11/18
                                HolidayDeliver: Ext.htmlEncode(Ext.getCmp("HolidayDeliver").getValue().HolidayDeliver),
                                EstimatedArrivalPeriod: Ext.htmlEncode(Ext.getCmp("EstimatedArrivalPeriod").getValue.EstimatedArrivalPeriod),
                                AddresseeSex: Ext.getCmp("g_checkActionSex").getValue().g_checkActionSex,
                                ServiceSex: Ext.getCmp("s_checkActionSex").getValue().s_checkActionSex,
                                //add by zhuoqin0830w  獲取 Cart_Delivery  2015/07/03
                                Cart_Delivery: location.pathname == "/Order/InteriorOrderAdd" ? Ext.getCmp("Site_Id").valueModels[0].data.Cart_Delivery : 0
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    afterSuccess();
                                    myMask.hide();
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                                Ext.getCmp('btnSubmit').setDisabled(false);
                            },
                            failure: function (form, action) {
                                myMask.hide();
                                if (!action.response.responseText) {
                                    Ext.Msg.alert(INFORMATION, ADDORDER_FAILURE);
                                    return;
                                }
                                var result = Ext.decode(action.response.responseText);
                                if (!result.success) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                                Ext.getCmp('btnSubmit').setDisabled(false);
                            }
                        });
                    }
                }
            }
        }]
    }],
    columns: [{
        header: '',
        width: 60,
        align: 'center',
        sortable: false,
        menuDisabled: true,
        xtype: 'actioncolumn',
        items: [{
            getClass: function (v, meta, rec) {
                if (rec.get("combination") == 3 && rec.get("buy_limit") == 0 && rec.get("parent_id") != 0) {
                    //任選不限定一單位時新增的行需添加刪除按鈕  
                    return 'toolImgMinus';
                }
                if (rec.get("product_id") == " ") {
                    return 'toolImghide';
                }
                else {
                    return 'toolImgshow';
                }
            },
            handler: function (grid, rowIndex, colIndex) {
                currentRow = rowIndex;
                if (orderStore.getCount() > 1) {
                    var curData = orderStore.getAt(rowIndex);
                    var length = curData.get("g_must_buy");
                    var parentIdx = findParentIdx(currentRow);
                    var parentBuyNum = orderStore.getAt(parentIdx).get('buynum');
                    if (curData.get('parent_id') && parentBuyNum > 0) {
                        return;
                    }
                    orderStore.removeAt(rowIndex);
                    for (var i = 1; i <= length; i++) {
                        if (!orderStore.getAt(rowIndex)) {
                            break;
                        }
                        else {
                            if (orderStore.getAt(rowIndex).get("product_id") == " ") {
                                orderStore.removeAt(rowIndex);
                            }
                            else {
                                break;
                            }
                        }
                    }
                    if (orderStore.getCount() == 0) {
                        addTr();
                    }
                }
                else if (orderStore.getCount() == 1) {
                    removeStore(0);
                }

                if (curData && curData.get('product_id') == ' ' && curData.get('item_id')) {
                    CalculatePrice(curData.data.combination, curData.data.price_type);
                }

                //計算所有訂單的總價
                var numSum = 0;
                for (var i = 0, j = orderStore.getCount() ; i < j; i++) {
                    var data = orderStore.getAt(i);
                    if (data.get("buynum") != 0) {
                        numSum += parseInt(data.get("buynum"));
                    }
                }
                Ext.getCmp("txtTotalPrice").setValue(accountSum());
                //add by zhuoqin0830w  2015/07/31  計算需要返還的購物金
                Ext.getCmp("txtTotalAccbonus").setValue(accBonus());
                if (numSum == 0) {
                    Ext.getCmp("btnSubmit").setDisabled(true);
                    clearCmp();
                }
            }
        }]
    }, {
        header: PRODUCT_ID, sortable: false, menuDisabled: true, dataIndex: 'product_id', width: 150,
        editor: {
            xtype: 'textfield',
            //vtype: 'regxPNo',
            id: 'txtProductId'
        }
    }, {
        header: PRODUCT_NAME, sortable: false, menuDisabled: true, dataIndex: 'product_name', width: 200
    }, {
        header: CHILD_PRODUCT_NAME, sortable: false, menuDisabled: true, dataIndex: 'child', width: 200, renderer: function (value, m, r, row, column) {
            var regx = /^\d{5,}$/;    //edit by Jiajun 2014/10/10
            if (regx.test(value)) {
                if (orderGrid.columns[3].getEditor().getStore().findRecord('Child_Id', value) != null) {
                    return orderGrid.columns[3].getEditor().getStore().findRecord('Child_Id', value).data.Product_Name
                }
                else {
                    return value;
                }
            }
            else if (orderStore.getAt(row).get('product_id') != ' ' && orderStore.getAt(row).get('combination') == 3 && orderStore.getAt(row).get('buy_limit') == 0) {
                var childCount = orderStore.getAt(row).get('childCount');

                /****** 如果當前顯示的行數之和等於該組合下的子商品數,則不能再添加行 *****/
                if (orderStore.getAt(row + childCount)) {
                    if (orderStore.getAt(row + childCount).get('parent_id') == orderStore.getAt(row).get('product_id')) {
                        return value;
                    }
                }
                /**************************************************************/

                /********* 如果當前子商品的購買數量之和等於需選數量則不能再添加 *******/
                var g_must_buy = orderStore.getAt(row).data.g_must_buy;
                var childSum = orderStore.getAt(row).data.childSum;
                if (childSum >= g_must_buy) {
                    return value;
                }
                /*************************************************************/

                //組合類型為無規格任選并且不限定一單位時，返回添加按鈕。
                return "<div class='toolImgAdd' onclick='addComboSelTr(0," + orderStore.getAt(row).get('Product_Id') + "," + row + "," + orderStore.getAt(row).get('combination') + "," + orderStore.getAt(row).get('buy_limit') + ",true)'></div>";
            }
            else {
                return value;
            }
        }, editor: {
            xtype: 'combobox',
            id: 'children',
            editable: false,
            queryMode: 'local',
            displayField: 'Product_Name',
            valueField: 'Product_Name',
            listeners: {
                select: function (combo, rec) {
                    var status = true;
                    if (storeType == CHANNEL_TYPE_COOPERATOR) {
                        return;
                    }
                    if (combo.value != SELECT) {
                        orderStore.getAt(currentRow).set("child", combo.rawValue);
                        var Product_Id;
                        for (var i = currentRow - 1, j = currentRow + 1; i >= 0 || j < orderStore.data.length; i--, j++) {
                            if (i >= 0) {
                                if (orderStore.getAt(i).get("product_id") != " ") {
                                    Product_Id = orderStore.getAt(i).get("product_id");
                                    i = 0;
                                }
                                else {
                                    if (orderStore.getAt(i).get("child") == combo.value) {
                                        status = false;
                                        combo.setValue(SELECT);
                                        removeChildStore(orderStore.getAt(currentRow));
                                        Ext.Msg.alert(PROMPT, CHILD_PROD_HAS_SELECTED);
                                    }
                                }
                            }
                            if (j < orderStore.data.length) {
                                if (orderStore.getAt(j).get("product_id") != " ") {
                                    j = orderStore.data.length
                                }
                                else {
                                    if (orderStore.getAt(j).get("child") == combo.value) {
                                        status = false;
                                        combo.setValue(SELECT);
                                        removeChildStore(orderStore.getAt(currentRow));
                                        Ext.Msg.alert(PROMPT, CHILD_PROD_HAS_SELECTED);
                                    }
                                }
                            }
                        }
                        if (status) {
                            var result = searchByProID(rec[0].data.Child_Id, currentRow, null, null, null, Product_Id, null, null);
                            if (result == 0) {
                                status = false;
                                combo.setValue(SELECT);
                                removeChildStore(orderStore.getAt(currentRow));
                                //Ext.Msg.alert(PROMPT, "此單一商品規 格價格數據錯誤，請重新選擇。");
                            }
                        }
                        //選擇錯誤后，父級售價重新計算，購買數量重置
                        if (!status) {
                            var t_price = 0;
                            var p_id = 0;
                            for (i = currentRow - 1, j = currentRow + 1; i >= 0 || j < orderStore.data.length; i--, j++) {
                                if (i >= 0) {
                                    var curData = orderStore.data.items[i].data;
                                    if (curData.product_id == " ") {
                                        t_price += curData.original_price * (curData.s_must_buy == 0 ? 1 : curData.s_must_buy);
                                    } else { p_id = i; i = -1; }
                                }

                                if (j < orderStore.data.length) {
                                    if (orderStore.data.items[j].data.product_id == " ") {
                                        t_price += orderStore.data.items[j].data.original_price * orderStore.data.items[j].data.s_must_buy;
                                    } else { j = orderStore.data.length; }
                                }
                            }
                            if (orderStore.getAt(p_id).get('price_type') == 2) {
                                orderStore.getAt(p_id).set("product_cost", t_price);
                            }
                            orderStore.getAt(p_id).set("buynum", 0);
                        }
                    }
                }
            }
        }
    }, {
        header: SPEC1, sortable: false, menuDisabled: true, dataIndex: 'spec1', renderer: function (value, m, r, row, column) {
            return orderStore.getAt(row).get("spec1_show");
        }, editor: {
            xtype: 'combobox',
            editable: false,
            id: 'combSpec1',
            queryMode: 'local',
            displayField: 'spec_name',
            valueField: 'spec_id',
            listeners: {
                select: function (combo, record) {
                    if (storeType == CHANNEL_TYPE_COOPERATOR) {
                        return;
                    }
                    var val;
                    if (orderStore.getAt(currentRow).get("combination")) {
                        val = orderStore.getAt(currentRow).get("Product_Id");
                    }
                    else {
                        val = orderStore.getAt(currentRow).get("product_id");
                    }
                    var Product_Id;
                    for (var i = currentRow; i >= 0; i--) {
                        if (orderStore.getAt(i).get("product_id") != " ") {
                            Product_Id = orderStore.getAt(i).get("product_id");
                            break;
                        }
                    }
                    var spec1 = record[0].data.spec_id;
                    var spec2 = orderStore.getAt(currentRow).get("spec2");
                    var result;
                    result = searchByProID(val, currentRow, spec1, spec2, null, Product_Id, null, null);
                    if (result == 0) {
                        return;
                    }
                    orderStore.getAt(currentRow).set("spec1_show", record[0].data.spec_name);
                }
            }
        }
    }, {
        header: SPEC2, sortable: false, menuDisabled: true, dataIndex: 'spec2', renderer: function (value, m, r, row, column) {
            return orderStore.getAt(row).get("spec2_show");
        }, editor: {
            xtype: 'combobox',
            editable: false,
            id: 'combSpec2',
            queryMode: 'local',
            displayField: 'spec_name',
            valueField: 'spec_id',
            listeners: {
                select: function (combo, record) {
                    if (storeType == CHANNEL_TYPE_COOPERATOR) {
                        return;
                    }
                    var val;
                    if (orderStore.getAt(currentRow).get("combination")) {
                        val = orderStore.getAt(currentRow).get("Product_Id");
                    }
                    else {
                        val = orderStore.getAt(currentRow).get("product_id");
                    }
                    var Product_Id;
                    for (var i = currentRow; i >= 0; i--) {
                        if (orderStore.getAt(i).get("product_id") != " ") {
                            Product_Id = orderStore.getAt(i).get("product_id");
                            break;
                        }
                    }
                    var spec1 = orderStore.getAt(currentRow).get("spec1");
                    var spec2 = record[0].data.spec_id;
                    var result;
                    result = searchByProID(val, currentRow, spec1, spec2, null, Product_Id, null, null);
                    if (result == 0) {
                        return;
                    }
                    orderStore.getAt(currentRow).set("spec2_show", record[0].data.spec_name);
                }
            }
        }
    }, {
        header: PRICE, sortable: false, menuDisabled: true, dataIndex: 'product_cost', width: 80,
        editor: {
            xtype: 'numberfield',
            minValue: 0,
            vtype: 'regxMoney',
            listeners: {
                blur: function (a, b) {
                    var s_must_buy = orderStore.getAt(currentRow).get("s_must_buy"); //此處s_must_buy表示組合商品中必選數量不為0的記錄數
                    var child_scale = orderStore.getAt(currentRow).get("child_scale");
                    var price = a.rawValue;
                    orderStore.getAt(currentRow).set("product_cost", price);
                    if (!child_scale) {
                        return;
                    }
                    var child_scale_list = child_scale.split(',');
                    var beforePriceCount = 0;
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (num == 0) {
                            return;
                        }
                    }
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (i == s_must_buy) {
                            //var result = Math.round((price -  beforePriceCount) / num);
                            var result = Math.round((price) / num);
                            current_child.set("product_cost", result);
                        }
                        else {
                            var result = Math.round((price * child_scale_list[i - 1]) / num)
                            beforePriceCount += result * num;
                            price -= result * num;
                            current_child.set("product_cost", result);
                        }
                    }
                }
            }
        }
    }, {
        header: COST, id: 'cost', sortable: false, menuDisabled: true, dataIndex: 'Item_Cost', hidden: true, width: 80,
        editor: {
            xtype: 'numberfield',
            minValue: 0,
            vtype: 'regxMoney',
            listeners: {
                blur: function (a, b) {
                    var s_must_buy = orderStore.getAt(currentRow).get("s_must_buy");
                    var child_scale = orderStore.getAt(currentRow).get("child_scale");
                    var price = a.rawValue;
                    orderStore.getAt(currentRow).set("Item_Cost", price);
                    if (!child_scale) {
                        return;
                    }
                    var child_scale_list = child_scale.split(',');
                    var beforePriceCount = 0;
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (num == 0) {
                            return;
                        }
                    }
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (i == s_must_buy) {
                            var result = Math.round((price) / num);
                            current_child.set("Item_Cost", result);
                        }
                        else {
                            var result = Math.round((price * child_scale_list[i - 1]) / num)
                            beforePriceCount += result * num;
                            price -= result * num;
                            current_child.set("Item_Cost", result);
                        }
                    }
                }
            }
        }
    }, {//edit by zhuoqin0830w  2015/11/16   添加活動售價金額
        header: EVENT_PRICE, id: 'Event_Item_Money', dataIndex: 'Event_Item_Money', hidden: true, sortable: false, menuDisabled: true, width: 80,
        editor: {
            xtype: 'numberfield',
            minValue: 0,
            vtype: 'regxMoney',
            listeners: {
                blur: function (a, b) {
                    var s_must_buy = orderStore.getAt(currentRow).get("s_must_buy");
                    var child_scale = orderStore.getAt(currentRow).get("child_scale");
                    var price = a.rawValue;
                    orderStore.getAt(currentRow).set("Event_Item_Money", price);
                    if (!child_scale) {
                        return;
                    }
                    var child_scale_list = child_scale.split(',');
                    var beforePriceCount = 0;
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (num == 0) {
                            return;
                        }
                    }
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (i == s_must_buy) {
                            var result = Math.round((price) / num);
                            current_child.set("Event_Item_Money", result);
                        }
                        else {
                            var result = Math.round((price * child_scale_list[i - 1]) / num)
                            beforePriceCount += result * num;
                            price -= result * num;
                            current_child.set("Event_Item_Money", result);
                        }
                    }
                }
            }
        }
    }, {//edit by zhuoqin0830w  2015/04/30  添加活動成本金額
        header: EVENT_COST, id: 'Event_Item_Cost', dataIndex: 'Event_Item_Cost', hidden: true, sortable: false, menuDisabled: true, width: 80,
        editor: {
            xtype: 'numberfield',
            minValue: 0,
            vtype: 'regxMoney',
            listeners: {
                blur: function (a, b) {
                    var s_must_buy = orderStore.getAt(currentRow).get("s_must_buy");
                    var child_scale = orderStore.getAt(currentRow).get("child_scale");
                    var price = a.rawValue;
                    orderStore.getAt(currentRow).set("Event_Item_Cost", price);
                    if (!child_scale) {
                        return;
                    }
                    var child_scale_list = child_scale.split(',');
                    var beforePriceCount = 0;
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (num == 0) {
                            return;
                        }
                    }
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (i == s_must_buy) {
                            var result = Math.round((price) / num);
                            current_child.set("Event_Item_Cost", result);
                        }
                        else {
                            var result = Math.round((price * child_scale_list[i - 1]) / num)
                            beforePriceCount += result * num;
                            price -= result * num;
                            current_child.set("Event_Item_Cost", result);
                        }
                    }
                }
            }
        }
    }, {
        header: BUY_NUM, id: 'buynum', dataIndex: 'buynum', sortable: false, menuDisabled: true, width: 80,
        editor: {
            xtype: 'numberfield',
            decimalPrecision: 0,
            minValue: 0,
            listeners: {
                blur: function (self, The, eOpts) {
                    var Product_Id = orderStore.getAt(currentRow).data.Product_Id; //子商品product_id
                    var parent_id = orderStore.getAt(currentRow).data.parent_id;
                    var product_id = orderStore.getAt(currentRow).data.product_id; //父商品product_id
                    var price_type = orderStore.getAt(currentRow).data.price_type;
                    var combination = orderStore.getAt(currentRow).data.combination;
                    if (product_id == " ") {
                        orderStore.getAt(currentRow).set("s_must_buy", self.value);
                        CalculatePrice(combination, price_type, self.value);
                    }
                }
            }
        }
    }, {//edit by zhuoqin0830w  2015/05/13  添加購物金金額
        header: DEDUCT_BONUS, id: 'Deduct_Bonus', dataIndex: 'Deduct_Bonus', hidden: true, sortable: false, menuDisabled: true, width: 80,
        editor: {
            xtype: 'numberfield',
            minValue: 0,
            vtype: 'regxMoney',
            listeners: {
                blur: function (a, b) {
                    var s_must_buy = orderStore.getAt(currentRow).get("s_must_buy");
                    var child_scale = orderStore.getAt(currentRow).get("child_scale");
                    var price = 0;
                    if (a.rawValue != "") {
                        price = a.rawValue;
                    }
                    orderStore.getAt(currentRow).set("Deduct_Bonus", price);
                    if (!child_scale) {
                        return;
                    }
                    var child_scale_list = child_scale.split(',');
                    var beforePriceCount = 0;
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (num == 0) {
                            return;
                        }
                    }
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (i == s_must_buy) {
                            var result = Math.round((price) / num);
                            current_child.set("Deduct_Bonus", result);
                        }
                        else {
                            var result = Math.round((price * child_scale_list[i - 1]) / num)
                            beforePriceCount += result * num;
                            price -= result * num;
                            current_child.set("Deduct_Bonus", result);
                        }
                    }
                }
            }
        }
    }, {//edit by zhuoqin0830w  2015/05/13  添加抵用券金額
        header: DEDUCT_WELFARE, id: 'Deduct_Welfare', dataIndex: 'Deduct_Welfare', hidden: true, sortable: false, menuDisabled: true, width: 80,
        editor: {
            xtype: 'numberfield',
            minValue: 0,
            vtype: 'regxMoney',
            listeners: {
                blur: function (a, b) {
                    var s_must_buy = orderStore.getAt(currentRow).get("s_must_buy");
                    var child_scale = orderStore.getAt(currentRow).get("child_scale");
                    var price = 0;
                    if (a.rawValue != "") {
                        price = a.rawValue;
                    }
                    orderStore.getAt(currentRow).set("Deduct_Welfare", price);
                    if (!child_scale) {
                        return;
                    }
                    var child_scale_list = child_scale.split(',');
                    var beforePriceCount = 0;
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (num == 0) {
                            return;
                        }
                    }
                    for (var i = 1; i <= s_must_buy; i++) {
                        var current_child = orderStore.getAt(currentRow + i);
                        var num = current_child.get("s_must_buy");
                        if (i == s_must_buy) {
                            var result = Math.round((price) / num);
                            current_child.set("Deduct_Welfare", result);
                        }
                        else {
                            var result = Math.round((price * child_scale_list[i - 1]) / num)
                            beforePriceCount += result * num;
                            price -= result * num;
                            current_child.set("Deduct_Welfare", result);
                        }
                    }
                }
            }
        }
    }, {
        header: STOCK, sortable: false, menuDisabled: true, dataIndex: 'stock', width: 80
    }, {
        header: SUM_PRICE, dataIndex: 'sumprice', width: 120, sortable: false, menuDisabled: true,
        renderer: function (value, m, r, row, column) {
            var data = orderStore.getAt(row);
            if (data.get("price_type") == 2) {
                return value;
            }
            var cost = parseFloat(data.get("product_cost"));
            var event_item_money = parseFloat(data.get("Event_Item_Money"));
            var Deduct_Bonus = parseFloat(data.get('Deduct_Bonus'));
            var Deduct_Welfare = parseFloat(data.get('Deduct_Welfare'));
            //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
            var price = event_item_money == 0 ? cost : event_item_money;
            return parseInt(data.get("buynum")) * price - Deduct_Welfare - Deduct_Bonus;
        }
    }, {
        width: 120, hidden: true, dataIndex: 'spec1_show', sortable: false, menuDisabled: true
    }, {
        width: 120, hidden: true, dataIndex: 'spec2_show', sortable: false, menuDisabled: true
    }, {
        header: PRODUCT_STATUS, dataIndex: 'product_status_name', id: 'product_status_name', hidden: true, sortable: false, menuDisabled: true
    }, {
        header: ACCUMULATED_BONUS, id: 'accumulated_bonus', dataIndex: 'accumulated_bonus', hidden: true, sortable: false, menuDisabled: true, width: 80
    }]
    //{ header: PRODUCT_ID, dataIndex: 'Product_Id', hidden: false, sortable: false, menuDisabled: true },
    //{ header: 'item_id', dataIndex: 'item_id', hidden: true, sortable: false, menuDisabled: true },
    //{ header: 'child_price_scale', dataIndex: 'child_scale', hidden: false, sortable: false, menuDisabled: true },
    //{ header: 's_must_buy', dataIndex: 's_must_buy', hidden: false, sortable: false, menuDisabled: true },
    //{ header: 'child_cost_scale', dataIndex: 'child_cost_scale', hidden: false, sortable: false, menuDisabled: true },
});

Ext.onReady(function () {
    Ext.create('Ext.container.Viewport', {
        layout: 'anchor',
        items: [myPanel, orderGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,//edit by zhuoqin0830w  添加頁面滾動條  ahon  說需要更改  2015/09/24 11:38
        listeners: {
            resize: function () {
                orderGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});

function removeChildStore(data) {
    data.set("child", SELECT);
    data.set("product_cost", 0);
    data.set("stock", 0);
    data.set("buynum", 0);
    data.set("s_must_buy", 0);
}

//根據商品編號查詢商品信息
function searchByProID(val, rowIdx, spec1, spec2, obj, Product_Id, Site_Id, combOrderDate) {
    switch (storeType) {
        case CHANNEL_TYPE_COOPERATOR: searchByCooperator(val, rowIdx, spec1, spec2, obj); break; //合作外站
        case CHANNEL_TYPE_GIGADE: return searchByGigade(val, rowIdx, spec1, spec2, obj, Product_Id, Site_Id, combOrderDate); break; //吉甲地站臺
        default:
    }
}

var rowCount;

//Gigade之商品查詢
function searchByGigade(val, rowIdx, spec1, spec2, obj, Product_Id, Site_Id, combOrderDate) {
    var regx = /^\d{5,}$/;
    if (!regx.test(val) && !Product_Id) {
        Ext.Msg.alert(INFORMATION, FOURNUMBER_INPUT);
        return;
    }
    var searchResult;
    //輸入 的 id 值 長度 為 6 則表示選擇的是 單一商品中的Item_id 
    if (val.length == 6) {
        Ext.Ajax.request({
            url: '/Order/OrderInfoQueryBySoleGigade',
            method: 'POST',
            async: false,
            params: {
                item_id: val == " " ? 0 : val,
                Site_Id: Site_Id,
                combOrderDate: combOrderDate,
            },
            success: function (response, opts) {
                var resText = Ext.decode("(" + response.responseText + ")");
                if (resText.success && resText.data[0].product_id != '') {
                    var data = resText.data[0];
                    var storeData = orderStore.getAt(rowIdx);
                    storeData.set("item_id", data.item_id);
                    storeData.set("product_cost", data.product_cost);
                    if (data.original_price) {
                        storeData.set("original_price", data.original_price);
                    }
                    else {
                        storeData.set("original_price", data.product_cost);
                    }
                    storeData.set("cost", data.cost);  //edit 2014/10/23
                    storeData.set("product_status_name", data.product_status_name) //edit 2014/10/28
                    storeData.set("Item_Cost", data.Item_Cost);
                    storeData.set("Event_Item_Cost", data.Event_Item_Cost);
                    storeData.set("Event_Item_Money", data.Event_Item_Money);//活動售價  add by zhuoqin0830w  2015/11/16
                    storeData.set("stock", data.stock);
                    storeData.set("spec1", data.spec1);
                    storeData.set("spec2", data.spec2);
                    storeData.set("Product_Id", data.product_id);
                    storeData.set("spec1_show", data.Spec_Name_1);
                    storeData.set("spec2_show", data.Spec_Name_2);
                    storeData.set("g_must_buy", data.g_must_buy);
                    storeData.set("child_scale", data.child_scale);
                    storeData.set("child_cost_scale", data.child_cost_scale);
                    storeData.set("price_type", data.price_type);
                    storeData.set("combination", data.child);
                    storeData.set("buy_limit", data.buy_limit);
                    storeData.set("childCount", data.childCount);
                    storeData.set("childSum", data.childSum);
                    storeData.set("ignore_stock", data.ignore_stock);
                    //僅限一單位時，選擇子商品後s_must_buy默認設為1
                    if (storeData.data.isAdd == 'false') {
                        storeData.set("buynum", data.s_must_buy == 0 ? 1 : data.s_must_buy);
                        storeData.set("s_must_buy", data.s_must_buy == 0 ? 1 : data.s_must_buy);
                    }
                    else {
                        storeData.set("s_must_buy", data.s_must_buy);
                    }
                    if (!Product_Id) {
                        storeData.set("product_id", data.product_id);
                        storeData.set("product_name", data.product_name);
                    }
                    var price_type = storeData.get("price_type");
                    if (storeData.data.child != undefined && storeData.data.child != "") {
                        CalculatePrice(data.child, price_type);
                    }
                    //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                    var cost = parseFloat(storeData.get("product_cost"));
                    var event_item_money = parseFloat(storeData.get("Event_Item_Money"));
                    storeData.set("sumprice", event_item_money == 0 ? cost * parseInt(storeData.get("buynum")) : event_item_money * parseInt(storeData.get("buynum")));
                }
                else {
                    if (resText.msg) {
                        searchResult = 0;
                        Ext.Msg.alert(INFORMATION, resText.msg);
                    }
                    else {
                        if (obj != null) {
                            Ext.Msg.alert(INFORMATION, PRODUCT_ID_WRONG);
                        }
                    }
                }
            },
            failure: function (response) {
                var data = Ext.decode("(" + response.responseText + ")");
                Ext.Msg.alert(INFORMATION, PRODUCT_ID_WRONG);
            }
        });
    }
    //輸入 的 id 值 長度 為 5 則表示選擇的是 組合商品商品中的product_id
    if (val.length == 5) {
        Ext.Ajax.request({
            url: '/Order/OrderInfoQueryByGigade',
            method: 'POST',
            async: false,
            params: {
                'pid': val == " " ? Product_Id : val,
                'parent_id': Product_Id,
                'spec1': spec1,
                'spec2': spec2,
                Site_Id: Site_Id,
                combOrderDate: combOrderDate,
            },
            success: function (response, opts) {
                var resText = Ext.decode("(" + response.responseText + ")");
                if (resText.success && resText.data[0].product_id != '') {
                    var data = resText.data[0];
                    var storeData = orderStore.getAt(rowIdx);
                    storeData.set("item_id", data.item_id);
                    storeData.set("product_cost", data.product_cost);
                    if (data.original_price) {
                        storeData.set("original_price", data.original_price);
                    }
                    else {
                        storeData.set("original_price", data.product_cost);
                    }
                    storeData.set("cost", data.cost);  //edit  2014/10/23
                    storeData.set("product_status_name", data.product_status_name) //edit 2014/10/28
                    storeData.set("Item_Cost", data.Item_Cost);
                    storeData.set("Event_Item_Cost", data.Event_Item_Cost);
                    storeData.set("Event_Item_Money", data.Event_Item_Money);//活動售價  add by zhuoqin0830w  2015/11/16
                    storeData.set("stock", data.stock);
                    storeData.set("spec1", data.spec1);
                    storeData.set("spec2", data.spec2);
                    storeData.set("Product_Id", data.product_id);
                    storeData.set("spec1_show", data.Spec_Name_1);
                    storeData.set("spec2_show", data.Spec_Name_2);
                    storeData.set("g_must_buy", data.g_must_buy);
                    storeData.set("child_scale", data.child_scale);
                    storeData.set("child_cost_scale", data.child_cost_scale);
                    storeData.set("price_type", data.price_type);
                    storeData.set("combination", data.child);
                    storeData.set("buy_limit", data.buy_limit);
                    storeData.set("childCount", data.childCount);
                    storeData.set("childSum", data.childSum);
                    storeData.set("ignore_stock", data.ignore_stock);
                    //僅限一單位時，選擇子商品後s_must_buy默認設為1
                    if (storeData.data.isAdd == 'false') {
                        storeData.set("buynum", data.s_must_buy == 0 ? 1 : data.s_must_buy);
                        storeData.set("s_must_buy", data.s_must_buy == 0 ? 1 : data.s_must_buy);
                    }
                    else {
                        storeData.set("s_must_buy", data.s_must_buy);
                    }
                    if (!Product_Id) {
                        storeData.set("product_id", data.product_id);
                        storeData.set("product_name", data.product_name);
                    }
                    var price_type = storeData.get("price_type");
                    /*********************計算價格********************************/
                    if (storeData.data.child != undefined && storeData.data.child != "") {
                        CalculatePrice(data.child, price_type);
                    }

                    /**************************end******************************/
                    /*if (storeData.get("product_id") != " ") {
                    if (data.stock != 0) {
                    storeData.set("buynum", 1);
                    }
                    else {
                    storeData.set("buynum", 0);
                    }
                    }*/
                    //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                    var cost = parseFloat(storeData.get("product_cost"));
                    var event_item_money = parseFloat(storeData.get("Event_Item_Money"));
                    storeData.set("sumprice", event_item_money == 0 ? cost * parseInt(storeData.get("buynum")) : event_item_money * parseInt(storeData.get("buynum")));
                    //組合商品
                    if (data.g_must_buy != 0) {
                        if (data.child == 3) {
                            var mustBuyCount = 0;
                            if (resText.child) {
                                for (var i = resText.child.length - 1; i >= 0; i--) {

                                    var curData = resText.child[i];
                                    if (curData.s_must_buy > 0) {
                                        mustBuyCount++;
                                        addMustBuyTr(resText.child[i], data, rowIdx);
                                    }
                                }
                            }
                            if (resText.data[0].buy_limit == 1) {
                                var mustBuyNumber = data.g_must_buy - mustBuyCount;
                                for (var i = 0; i < mustBuyNumber; i++) {
                                    //僅限一單位時添加g_must_buy行
                                    //addComboSelTr(resText.child[i].Product_Id, 0, rowIdx, 0, 0, false);
                                    addComboSelTr(0, 0, rowIdx, 0, 0, false);
                                }
                            }
                        }
                        else {
                            for (var i = data.g_must_buy - 1; i >= 0; i--) {
                                addComboTr(resText.child[i], data, rowIdx);
                                storeData.set("price_type", resText.child[i].price_type);
                            }
                        }
                    }
                }
                else {
                    if (resText.msg) {
                        searchResult = 0;
                        Ext.Msg.alert(INFORMATION, resText.msg);
                    }
                    else {
                        if (obj != null) {
                            Ext.Msg.alert(INFORMATION, PRODUCT_ID_WRONG);
                        }
                    }
                }
            },
            failure: function (response) {
                var data = Ext.decode("(" + response.responseText + ")");
                Ext.Msg.alert(INFORMATION, PRODUCT_ID_WRONG);
            }
        });
    }
    return searchResult;
}

//合作外站之商品查詢
function searchByCooperator(val, rowIdx, spec1, spec2, obj) {
    var channelId = Ext.getCmp('combChannelId').getValue();
    if (val == "") {
        return;
    }
    Ext.Ajax.request({
        url: '/Order/OrderInfoQueryByCooperator',
        method: 'POST',
        params: {
            'pid': val,
            'spec1': spec1,
            'spec2': spec2,
            'channelId': channelId
        },
        success: function (response, opts) {
            var resText = Ext.decode(response.responseText);
            if (resText.success) {
                var data = resText.data[0];
                var storeData = orderStore.getAt(rowIdx);
                //var index = orderStore.find("product_id", data.product_id);
                storeData.set("product_id", data.product_id == 0 ? data.out_product_id : data.product_id);
                storeData.set("item_id", data.item_id);
                storeData.set("product_name", data.product_name);
                //storeData.set("cost", data.cost); //add 2014/10/23
                storeData.set("product_cost", data.product_cost);
                storeData.set("Event_Item_Cost", data.Event_Item_Cost);
                storeData.set("Event_Item_Money", data.Event_Item_Money);//活動售價  add by zhuoqin0830w  2015/11/16
                storeData.set("spec1_show", data.spec1_show);
                storeData.set("spec2_show", data.spec2_show);
                storeData.set("stock", data.stock);
                storeData.set("s_must_buy", data.s_must_buy);
                storeData.set("g_must_buy", data.g_must_buy);
                storeData.set("child_scale", data.child_scale);
                storeData.set("Item_Cost", data.Item_Cost);
                storeData.set("ignore_stock", data.ignore_stock);

                //                if (data.stock != 0) {
                //                    storeData.set("buynum", 1);
                //                }
                //                else {
                //                    storeData.set("buynum", 0);
                //                }

                //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
                var cost = parseFloat(storeData.get("product_cost"));
                var event_item_money = parseFloat(storeData.get("Event_Item_Money"));
                storeData.set("sumprice", event_item_money == 0 ? cost * parseInt(storeData.get("buynum")) : event_item_money * parseInt(storeData.get("buynum")));

                if (data.g_must_buy != 0) {
                    for (var i = 0; i < data.g_must_buy; i++) {
                        addComboCooperationTr(resText.child[i], rowIdx);
                        storeData.set("price_type", resText.child[i].price_type);
                    }
                }
            }
            else {
                if (resText.msg && resText.msg != '') {
                    Ext.Msg.alert(INFORMATION, resText.msg);
                }
                else {
                    if (obj != null) {
                        Ext.Msg.alert(INFORMATION, PRODUCT_ID_WRONG);
                    }
                }
                removeStore(rowIdx);
            }
        },
        failure: function (response) {
            var resText = eval("(" + response.responseText + ")");
        }
    });
}

function CalculatePrice(combination, price_type) {
    var parent_id_row;
    var up_index = 0;
    var dow_index = 0;
    var childrenTotalPrice; //子商品總價
    var childrenTotalCost;  //子商品总成本
    var upPartPrice = 0;
    var downPartPrice = 0;
    var upPartCost = 0;
    var downPartCost = 0;
    var selectStatus = true; //標記所有子商品是否都已經選擇
    if (combination == 2) {    //固定組合
        for (i = currentRow, j = currentRow + 1; i >= 0 || j < orderStore.data.length; i--, j++) {
            if (i >= 0) {
                var curData = orderStore.data.items[i].data;
                if (curData.product_id == " ") {
                    up_index++;
                    upPartPrice += curData.original_price * (curData.s_must_buy == 0 ? 1 : curData.s_must_buy);
                    upPartCost += curData.Item_Cost * (curData.s_must_buy == 0 ? 1 : curData.s_must_buy);
                } else { parent_id_row = i; i = -1; }
            }
            if (j < orderStore.data.length) {
                if (orderStore.data.items[j].data.product_id == " ") {
                    dow_index++;
                    downPartPrice += orderStore.data.items[j].data.original_price * orderStore.data.items[j].data.s_must_buy;
                    downPartCost += orderStore.data.items[j].data.Item_Cost * orderStore.data.items[j].data.s_must_buy;
                } else { j = orderStore.data.length; }
            }
        }
    }
    else {
        for (i = currentRow, j = currentRow + 1; i >= 0 || j < orderStore.data.length; i--, j++) {
            if (i >= 0) {
                var curData = orderStore.data.items[i].data;
                if (curData.product_id == " ") {
                    //判斷當前行子商品是否選擇完畢
                    up_index++;
                    if (curData.child == SELECT) {
                        selectStatus = false;
                    }
                    else {
                        upPartPrice += curData.original_price * (curData.s_must_buy == 0 ? 1 : curData.s_must_buy);
                        upPartCost += curData.Item_Cost * (curData.s_must_buy == 0 ? 1 : curData.s_must_buy);
                    }
                } else { parent_id_row = i; i = -1; }
            }
            if (j < orderStore.data.length) {
                if (orderStore.data.items[j].data.product_id == " ") {
                    //判斷當前行子商品是否選擇完畢
                    dow_index++;
                    if (orderStore.data.items[j].data.child == SELECT) {
                        selectStatus = false;
                    } else {
                        downPartPrice += orderStore.data.items[j].data.original_price * orderStore.data.items[j].data.s_must_buy;
                        downPartCost += orderStore.data.items[j].data.Item_Cost * orderStore.data.items[j].data.s_must_buy;
                    }
                } else { j = orderStore.data.length; }
            }
        }
    }
    childrenTotalPrice = upPartPrice + downPartPrice;
    childrenTotalCost = upPartCost + downPartCost;
    rowCount = up_index + dow_index;
    if (!selectStatus) { return; };
    //判斷是否開始計算組合商品各按比例拆分狀態下的單一商品價格
    if (price_type == 2) {
        orderStore.getAt(parent_id_row).set("product_cost", childrenTotalPrice);
        var frontStock = orderStore.getAt(parent_id_row + 1).data;
        var minStock = parseInt(frontStock.stock / (frontStock.s_must_buy == 0 ? 1 : frontStock.s_must_buy));
        //最小庫存為組合下子商品最小庫存/該商品的必購數量
        for (var i = 1; i <= rowCount; i++) {
            var curDa = orderStore.data.items[parent_id_row + i].data;
            var stock = parseInt(curDa.stock / (curDa.s_must_buy == 0 ? 1 : curDa.s_must_buy));
            if (stock < minStock) {
                minStock = stock;
            }
        }
        orderStore.getAt(parent_id_row).set("stock", minStock);
        //根據活動時間判斷使用定價還是活動價  edit by zhuoqin0830w  2015/12/02
        var cost = orderStore.getAt(parent_id_row).get("product_cost");
        var event_item_money = orderStore.getAt(parent_id_row).get("Event_Item_Money");
        orderStore.getAt(parent_id_row).set("sumprice", event_item_money == 0 ? cost * orderStore.getAt(parent_id_row).get("buynum") : event_item_money * orderStore.getAt(parent_id_row).get("buynum"));        //add by xxl
        orderStore.getAt(parent_id_row).set("s_must_buy", rowCount);
    }
    else {
        var combo_price = orderStore.data.items[parent_id_row].data.product_cost;
        var combo_cost = orderStore.data.items[parent_id_row].data.Item_Cost;
        var beforePrice = 0;
        var frontMustBuy = orderStore.data.items[parent_id_row + 1].data.s_must_buy;  //子商品的第一筆必選數量
        var minStock = parseInt(orderStore.data.items[parent_id_row + 1].data.stock / (frontMustBuy == 0 ? 1 : frontMustBuy)); //最小庫存
        var child_scale = "";
        var child_cost_scale = "";
        for (var i = 1; i <= rowCount; i++) {
            //計算價格
            var price = orderStore.data.items[parent_id_row + i].data.original_price;
            var cost = orderStore.data.items[parent_id_row + i].data.Item_Cost;
            var num = orderStore.data.items[parent_id_row + i].data.s_must_buy == 0 ? 1 : orderStore.data.items[parent_id_row + i].data.s_must_buy;
            var stock = parseInt(orderStore.data.items[parent_id_row + i].data.stock / num);
            if (stock < minStock) {
                minStock = stock;
            }
            //new logic
            var child_product_price = 0;
            var child_product_cost = 0;
            child_product_price = price * num;
            child_product_cost = cost * num;

            var priceScale = (child_product_price / childrenTotalPrice);
            var priceResult = Math.round(combo_price * priceScale / num);

            var costScale = (child_product_cost / childrenTotalCost);
            var costResult = Math.round(combo_cost * costScale / num);

            child_scale += priceScale + ",";
            child_cost_scale += costScale + ",";

            childrenTotalPrice -= child_product_price;
            childrenTotalCost -= child_product_cost;

            combo_price -= priceResult * num;
            combo_cost -= costResult * num;

            orderStore.getAt(parent_id_row + i).set("product_cost", priceResult);
            //orderStore.getAt(parent_id_row + i).set("Item_Cost", costResult);
            //            if (i == rowCount) {
            //                orderStore.getAt(parent_id_row + i).set("product_cost", Math.round((combo_price - beforePrice) / num));
            //            }
            //            else {
            //                var child_product_price = 0;

            //                child_product_price = price * num;
            //                var result = Math.round(combo_price *  (child_product_price / childrenTotalPrice) / num);
            //                child_scale += (child_product_price / childrenTotalPrice) + ",";
            //                beforePrice += result * num;
            //                orderStore.getAt(parent_id_row +  i).set("product_cost", result);

            //            }
        }
        child_scale = child_scale.substring(0, child_scale.length -
1);
        child_cost_scale = child_cost_scale.substring(0, child_cost_scale.length - 1);
        orderStore.getAt(parent_id_row).set("stock", minStock);
        //            if (minStock == 0) {
        //                orderStore.getAt(parent_id_row).set ("buynum", 0);
        //                for (var i = 1; i <= rowCount; i++) {
        //                    if (orderStore.getAt(parent_id_row + i).data.isAdd != 'false') {
        //                        orderStore.getAt(parent_id_row + i).set("buynum", 0);
        //                        orderStore.getAt(parent_id_row +  i).set("s_must_buy", 0);
        //                    }
        //                }
        //            }

        orderStore.getAt(parent_id_row).set("child_scale", child_scale);
        orderStore.getAt(parent_id_row).set("child_cost_scale", child_cost_scale);
        orderStore.getAt(parent_id_row).set("s_must_buy", rowCount);
    }
}