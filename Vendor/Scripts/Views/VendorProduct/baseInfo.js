var a;
var columnCount_freight;
var columnCount_mode;
var PRODUCT_ID, OLD_PRODUCT_ID = '';


//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string"}]
});

// 品牌store該供應商下所有品牌
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/VendorBrand/QueryBrand",
        //noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});

Ext.onReady(function () {
    var tomorrow = Tomorrow();
    var tomRepeat = Tomorrow();
    PRODUCT_ID = window.parent.GetProductId(); //獲取修改時的pid
    OLD_PRODUCT_ID = window.parent.GetCopyProductId(); //獲取複製時的pid
    var baseInfo = Ext.create('Ext.form.Panel', {
        id: 'baseinfo',
        border: false,
        autoScroll: true,
        defaults: {
            labelWidth: 80,
            padding: '8 0 0 0'
        },
        items: [{ //品牌      
            xtype: 'combobox',
            allowBlank: false,
            // hidden: true,
            id: 'brand_id',
            name: 'Brand_Id',
            colName: 'brand_id',
            store: brandStore,
            queryMode: 'local',
            displayField: 'brand_name',
            valueField: 'brand_id',
            typeAhead: true,
            forceSelection: true,
            emptyText: SELECT,
            fieldLabel: BRAND + '<span style="color:red">*</span>'
        }, {//商品名稱
            xtype: 'textfield',
            id: 'Product_name',
            allowBlank: false,
            colName: 'Product_name',
            //  hidden: true,
            width: 400,
            name: 'Product_Name',
            fieldLabel: PRODUCT_NAME + '<span style="color:red">*</span>'
        }, {//排序
            xtype: 'numberfield',
            decimalPrecision: 0,
            minValue: 0,
            maxValue: 9999,
            value: 0,
            // hidden: true,
            id: 'product_sort',
            colName: 'product_sort',
            allowBlank: false,
            fieldLabel: PRODUCT_SORT,
            name: 'Product_Sort'
        }, {//廠商商品編號
            xtype: 'textfield',
            // hidden: true,
            id: 'product_vendor_code',
            colName: 'product_vendor_code',
            fieldLabel: PRODUCT_VENDOR_CODE,
            name: 'Product_Vendor_Code'
        }, {//上架時間
            xtype: 'panel',
            //  hidden: true,
            colName: 'product_start',
            layout: 'hbox',
            id: 'product_start',
            allowBlank: false,
            border: false,
            items: [
        {
            xtype: 'datetimefield',
            disabledMin: true,
            disabledSec: true,
            format: 'Y-m-d H:i:s',
            id: 'p_start',
            fieldLabel: PRODUCT_START,
            allowBlank: false,
            labelWidth: 80,
            value: tomRepeat,
            margin: '0 8 0 0',
            listeners: {
                select: function (a, b, c) {
                    var Year = new Date(this.getValue()).getFullYear() + 10;
                    Ext.getCmp("pt_end").setValue(new Date(new Date(this.getValue()).setFullYear(Year)));
                }
            }
        }
    ]
        }, {//下架時間
            xtype: 'panel',
            // hidden: true,
            layout: 'hbox',
            border: false,
            id: 'product_end',
            colName: 'product_end',
            items: [
        {
            xtype: 'datetimefield',
            disabledMin: true,
            disabledSec: true,
            format: 'Y-m-d H:i:s',
            id: 'pt_end',
            value: new Date(tomorrow.setFullYear(tomorrow.getFullYear() + 10)),
            allowBlank: false,
            fieldLabel: PRODUCT_END,
            labelWidth: 80,
            margin: '0 8 0 0',
            listeners: {

        }
    }
    ]
        }, {
            xtype: 'panel',
            // hidden: true,
            layout: 'hbox',
            border: false,
            id: 'expect_time',
            colName: 'expect_time',
            items: [
        {//預計出貨時間
            xtype: 'datetimefield',
            disabledMin: true,
            disabledSec: true,
            format: 'Y-m-d H:i:s',
            id: 'e_time',
            fieldLabel: EXPECT_TIME,
            labelWidth: 80,
            margin: '0 8 0 0'
        }, {//出貨信息
            xtype: 'textfield',
            // hidden: true,
            width: 300,
            id: 'expect_msg',
            labelWidth: 80,
            fieldLabel: EXPECT_MSG,
            colName: 'expect_msg',
            name: 'expect_msg'
        }
    ]
        }, {//運送方式
            xtype: 'radiogroup',
            //  hidden: true,
            id: 'product_freight_set',
            colName: 'product_freight_set',
            fieldLabel: PRODUCT_FREIGHT_SET,
            width: 500,
            name: 'Product_Freight_Set'
        }, {//出貨方式
            xtype: 'radiogroup',
            // hidden: true,
            id: 'product_mode',
            name: 'Product_Mode',
            colName: 'product_mode',
            fieldLabel: PRODUCT_MODE,
            width: 250
        }, {//營業稅
            xtype: 'radiogroup',
            // hidden: true,
            id: 'tax_type',
            fieldLabel: TAX_TYPE,
            colName: 'tax_type',
            width: 200,
            defaults: {
                name: 'Tax_Type'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: TAX_TYPE1, inputValue: '1', checked: true },
            { boxLabel: TAX_TYPE2, inputValue: '3' }
        ]
        }, {
            xtype: 'button',
            iconCls: 'icon_rewind',
            text: SAVE,
            hidden: true,
            handler: function () {

            }

        }, {//商品組合類型
            xtype: 'displayfield',
            id: 'combination',
            name: 'combination',
            hidden: window.parent.GetProductId() == "", //新增時不顯示，修改和複製時顯示
            value: SILGLE_PROD,
            fieldLabel: COMBINATION
        }],
        listeners: {
            beforerender: function () {
                //檢測臨時表中是否有數據
                Ext.Ajax.request({
                    url: '/VendorProduct/QueryProduct',
                    method: 'post',
                    params: {
                        // "ProductId": window.parent.GetProductId(),
                        "ProductId": PRODUCT_ID,
                        "OldProductId": OLD_PRODUCT_ID
                    },
                    success: function (response) {
                        var reStr = eval("(" + response.responseText + ")");
                        if (reStr.data != null && window.parent.GetProductId() == "" && OLD_PRODUCT_ID == '') {//商品新增時判斷是否有以前未新增成功的數據
                            window.parent.Is_Continue(reStr.data.Product_Id);
                            //去掉右上角關閉按鈕
                        }
                        else {//不是新增時則加載數據
                            Page_Load();
                        }
                    }
                });
            }
        }
    });

    Ext.create('Ext.Viewport', {
        layout: 'fit',
        items: [baseInfo],
        border: false,
        style: {
            padding: '5 0 0 5'
        },
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            },
            afterrender: function () {
                window.parent.updateAuth(baseInfo, 'colName');
            }
        }
    });


    //運送方式
    var product_freight = Ext.get("freight").dom.value;
    Ext.getCmp("product_freight_set").update(product_freight);


    //出貨方式
    var product_mode = Ext.get("mode").dom.value;
    Ext.getCmp("product_mode").update(product_mode);

    $(":radio[name='product_mode']").bind('click', function () {
        if (this.checked && this.value != '2') {
            Ext.Msg.alert(INFORMATION, BAG_CHECK_INFORMATION);
        }
    });
});


function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + 1;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}


function Page_Load() {
    /*查詢出該條商品信息并加載*/
    $(".x-tool-close").show();
    Ext.getCmp("baseinfo").getForm().load({
        type: 'ajax',
        url: '/VendorProduct/QueryProduct',
        actionMethods: 'post',
        params: {
            "ProductId": PRODUCT_ID,
            "OldProductId": OLD_PRODUCT_ID
        },
        success: function (response, opts) {
            var resText = eval("(" + opts.response.responseText + ")");
            //數據加載的時候將product_id保存
            window.parent.SetProductId(resText.data.Product_Id); //加載數據時將product_id複製給隱藏域，便於後續使用
            if (resText.data.Product_Start != "0") {
                var date_start = new Date(resText.data.Product_Start * 1000);
                Ext.getCmp("p_start").setValue(date_start);
            }

            if (resText.data.Product_End != "0") {
                var date_end = new Date(resText.data.Product_End * 1000);
                Ext.getCmp("pt_end").setValue(date_end);
            }

            if (resText.data.Expect_Time != "0") {
                var expect_time = new Date(resText.data.Expect_Time * 1000);
                Ext.getCmp("e_time").setValue(expect_time);
            }

            $("#product_freight" + resText.data.Product_Freight_Set + "").attr("checked", "checked");

            $("#product_mode" + resText.data.Product_Mode + "").attr("checked", "checked");
            $("#tax" + resText.data.Tax_Type + "").attr("checked", "checked");

            Ext.getCmp("tax_type").setValue(resText.data.Tax_Type);

        }
    }
       );
    /**/
}

//驗證表單
function validateForm(product_start, product_end, expect_time) {

    if (!Ext.getCmp('baseinfo').getForm().isValid()) {
        return;
    }

    if (!Ext.getCmp("brand_id").isValid() || Ext.getCmp("brand_id").getValue() == SELECT) {
        Ext.Msg.alert(PROMPT, BRAND_NOT_NULL);
        return;
    }
    if (!Ext.getCmp("Product_name").isValid()) {
        Ext.Msg.alert(PROMPT, PRODUCT_NAME_NOT_NULL);
        return;
    }
    if (!Ext.getCmp("product_sort").isValid()) {
        Ext.Msg.alert(PROMPT, PRDUCT_SORT);
        return;
    }
    if (!Ext.getCmp("product_vendor_code").isValid()) {
        Ext.Msg.alert(PROMPT, PRODUCT_VENDOR_CODE_NOT_NULL);
        return;
    }

    if (window.parent.GetProductId() == "") {
        if (Ext.getCmp("p_start").rawValue != "" && product_start < new Date()) {
            Ext.Msg.alert(PROMPT, PRODUCT_START_LIMIT);
            return;
        }
        if (Ext.getCmp("pt_end").rawValue != "" && product_end < new Date()) {
            Ext.Msg.alert(PROMPT, PRODUCT_END_LIMIT);
            return;
        }
        if (Ext.getCmp("e_time").rawValue != "" && expect_time < new Date()) {
            Ext.Msg.alert(PROMPT, EXPECT_TIME_LIMIT);
            return;
        }
    }

    if (product_end != "0" && product_end < product_start) {
        Ext.Msg.alert(PROMPT, START_END_LIMIT);
        return;
    }

    //若填寫預計出貨時間則預計出貨信息必填
    if (Ext.getCmp("e_time").rawValue != "") {
        if (Ext.getCmp("expect_msg").getValue() == "") { Ext.Msg.alert(PROMPT, "請填寫預計出貨信息."); return; }
    }
    return true;
}

//保存表單說明：點擊下一步時保存，保存成功則跳轉
function save(functionid) {
    var product_start = "0";
    var product_end = "0";
    var expect_time = "0";

    if (Ext.getCmp("p_start").rawValue != "") {
        product_start = Ext.getCmp("p_start").rawValue;
    }
    if (Ext.getCmp("pt_end").rawValue != "") {
        product_end = Ext.getCmp("pt_end").rawValue;
    }
    if (Ext.getCmp("e_time").rawValue != "") {
        expect_time = Ext.getCmp("e_time").rawValue;
    }

    var retVal = true;
    if (!validateForm(product_start, product_end, expect_time)) {//驗證不通過時不跳轉
        window.parent.setMoveEnable(true);
        return false;
    }


    //儲存數據
    Ext.Ajax.request({
        url: '/VendorProduct/SaveBaseInfo',
        method: 'POST',
        async: false,
        params: {
            OldProductId: OLD_PRODUCT_ID,
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            "brand_id": Ext.htmlEncode(Ext.getCmp("brand_id").getValue()),
            "product_name": Ext.htmlEncode(Ext.getCmp("Product_name").getValue()),
            "product_sort": Ext.htmlEncode(Ext.getCmp("product_sort").getValue()),
            "product_vendor_code": Ext.htmlEncode(Ext.getCmp("product_vendor_code").getValue()),
            "product_start": product_start,
            "product_end": product_end,
            "expect_time": expect_time,
            "product_freight_set": Ext.htmlEncode($(":radio[name='product_freight']:checked")[0].value),
            "product_mode": Ext.htmlEncode($(":radio[name='product_mode']:checked")[0].value),
            "tax_type": Ext.htmlEncode(Ext.getCmp("tax_type").getValue().Tax_Type),
            "expect_msg": Ext.htmlEncode(Ext.getCmp('expect_msg').getValue())

        }, success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success == true && resMsg.rid != null) {
                PRODUCT_ID = resMsg.rid;
                window.parent.SetProductId(PRODUCT_ID);
            }
            if (resMsg.success == true && resMsg.msg != null && window.parent.GetIsEdit() == "true") {
                Ext.Msg.alert(PROMPT, resMsg.msg);
            }
            if (resMsg.success == false) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
                retVal = false;
                window.parent.setMoveEnable(true);
            }
        }
    });

    return retVal;
}

